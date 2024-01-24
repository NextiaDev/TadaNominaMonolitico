using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using TadaNomina.Models.ClassCore.TimbradoTP;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ClassTimbradoNomina: ClassProcesosTimbrado
    {          
        /// <summary>
        /// Metodo para agregar informacion de periodos acumulados en modelo ModelTimbradoNomina
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <returns>Modelo con periodos acumulados</returns>
        public ModelTimbradoNomina GetModeloTimbradoNomina(int IdUnidadNegocio)
        {
            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            ModelTimbradoNomina model = new ModelTimbradoNomina();
            List<SelectListItem> lperiodos = new List<SelectListItem>();
            List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoNominasAcumuladas(IdUnidadNegocio)
                .OrderByDescending(x=> x.IdPeriodoNomina).Take(250).ToList();
                        
            lvperiodos.ForEach(x=> { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });
            List<SelectListItem> lversion = new List<SelectListItem>
            {
                new SelectListItem { Text = "CFDI 3.3", Value = "3.3" },
                new SelectListItem { Text = "CFDI 4.0", Value = "4.0" }
            };            

            model.lPeriodos = lperiodos;
            model.lversion = lversion;

            return model;
        }        

        /// <summary>
        /// Metodo para timbrar por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="Id"></param>
        /// <param name="IdUsuario">Usuario</param>
        public void TimbradoNomina(int IdPeriodo, Guid Id, int IdUsuario)
        {
            List<sp_InformacionXML_Nomina1_Result> informacion = GetInformacionXML(IdPeriodo);

            Get_Token("Produccion");            
            
            foreach (var i in informacion)
            {
                if (i.SDI > 0 && i.ER > 0)
                    Timbra(i, Id, IdUsuario);
            }            
        }

        /// <summary>
        /// Metodo para timbrar por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="Id"></param>
        /// <param name="IdUsuario">Usuario</param>
        public void TimbradoPeriodoNomina(int IdPeriodo, int IdUnidadNegocio, int IdCliente, Guid Id, int IdUsuario)
        {
            Get_Token("Produccion");

            var cunidad = new ClassUnidadesNegocio();
            var cgxml = new cGeneraXML();

            var yaTimbrados = getYaTimbradosPeriodo(IdPeriodo).Select(y => y.IdEmpleado).ToList();
            var informacion = cgxml.getRegistrosvXMLPeriodo(IdPeriodo).Where(x => !yaTimbrados.Contains(x.IdEmpleado)).ToList();
            var unidad = cunidad.getUnidadesnegocioId(IdUnidadNegocio);
            var cliente = cunidad.getClienteById(IdCliente);

            foreach (var i in informacion)
            {
                string NoControl = "PER" + IdPeriodo + "-EMP" + i.IdEmpleado;
                string xmlB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(i.XML));
                var emitir = Emitir(xmlB64, NoControl);

                string Exito = string.Empty;
                string uuid = string.Empty;
                string fechaTimbrado = string.Empty;
                string facturaTimbrada = string.Empty;
                string pdfB64 = string.Empty;
                int anioMes = 0;
                string Codigo = string.Empty;
                string Texto = string.Empty;
                string Observaciones = string.Empty;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(emitir);
                XmlNodeList nExito = doc.GetElementsByTagName("exito");
                Exito = nExito[0].InnerText;

                if (Exito.ToUpper() == "FALSE")
                {
                    XmlNodeList nCodigo = doc.GetElementsByTagName("codigo");
                    try { Codigo = nCodigo[0].InnerText; } catch { Codigo = string.Empty; }
                    XmlNodeList nTexto = doc.GetElementsByTagName("texto");
                    try { Texto = nTexto[0].InnerText; } catch { Texto = string.Empty; }
                    XmlNodeList nObservaciones = doc.GetElementsByTagName("observaciones");
                    try { Observaciones = nObservaciones[0].InnerText; } catch { Observaciones = string.Empty; }

                    GuardaError(i.Rfc, IdUsuario, (i.IdPeriodoNomina ?? 0), Id, Codigo, Texto, Observaciones);
                }
                if (Exito.ToUpper() == "TRUE")
                {
                    XmlNodeList nuuid = doc.GetElementsByTagName("uuid");
                    try { uuid = nuuid[0].InnerText; } catch { uuid = string.Empty; }
                    XmlNodeList nfecha = doc.GetElementsByTagName("fechaTimbrado");
                    try { fechaTimbrado = nfecha[0].InnerText; } catch { fechaTimbrado = string.Empty; }
                    XmlNodeList nfacturaTimbrada = doc.GetElementsByTagName("facturaTimbradaB64");
                    try { facturaTimbrada = nfacturaTimbrada[0].InnerText; } catch { facturaTimbrada = string.Empty; }
                    XmlNodeList nanioMes = doc.GetElementsByTagName("anioMes");
                    try { anioMes = int.Parse(nanioMes[0].InnerText); } catch { anioMes = 0; }
                    XmlNodeList nPdf = doc.GetElementsByTagName("pdfB64");
                    try { pdfB64 = nPdf[0].InnerText; } catch { pdfB64 = string.Empty; }

                    string _facturaTimbrada = Statics.Base64Decode(facturaTimbrada);

                    GuardaTablaTimbrado(i, IdUsuario, (i.IdPeriodoNomina ?? 0), uuid, fechaTimbrado, anioMes, _facturaTimbrada, i.Leyenda);
                }
            }
        }

        public List<vTimbradoNomina> getYaTimbradosPeriodo(int IdPeriodoNomina)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrados = entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1).ToList();

                return timbrados;
            }
        }

        /// <summary>
        /// Metodo para timbrar
        /// </summary>
        /// <param name="i">Informacion para timbrar</param>
        /// <param name="Id"></param>
        /// <param name="IdUsuario">Usuario</param>
        public void Timbra(sp_InformacionXML_Nomina1_Result i, Guid Id, int IdUsuario)
        {
            string json = ""; //GetJSON(i);
            string SueldoMensual = (i.SueldoDiario * 30).ToString();
            string JSONleyenda = ""; // JsonConvert.SerializeObject(ObtenLeyendaCFDI(i.Leyenda));

            string base64EncodedExternalAccount = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            string LeyendaB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JSONleyenda));
            string NoControl = "PER" + i.IdPeriodoNomina + "-EMP" + i.Rfc.ToUpper().Trim() + "-T1";
            string PlantillaPDF = string.Empty;
            string emitir = string.Empty;

            PlantillaPDF = "D132931C-5375-49C4-9A53-D54BC80FDF64";
            emitir = Emitir(base64EncodedExternalAccount, NoControl, LeyendaB64);

            string Exito = string.Empty;
            string uuid = string.Empty;
            string fechaTimbrado = string.Empty;
            string facturaTimbrada = string.Empty;
            string pdfB64 = string.Empty;
            int anioMes = 0;
            string Codigo = string.Empty;
            string Texto = string.Empty;
            string Observaciones = string.Empty;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(emitir);
            XmlNodeList nExito = doc.GetElementsByTagName("exito");
            Exito = nExito[0].InnerText;

            if (Exito.ToUpper() == "FALSE")
            {
                XmlNodeList nCodigo = doc.GetElementsByTagName("codigo");
                try { Codigo = nCodigo[0].InnerText; } catch { Codigo = string.Empty; }
                XmlNodeList nTexto = doc.GetElementsByTagName("texto");
                try { Texto = nTexto[0].InnerText; } catch { Texto = string.Empty; }
                XmlNodeList nObservaciones = doc.GetElementsByTagName("observaciones");
                try { Observaciones = nObservaciones[0].InnerText; } catch { Observaciones = string.Empty; }

                GuardaError(i, IdUsuario, i.IdPeriodoNomina, Id, Codigo, Texto, Observaciones);
            }
            if (Exito.ToUpper() == "TRUE")
            {
                XmlNodeList nuuid = doc.GetElementsByTagName("uuid");
                try { uuid = nuuid[0].InnerText; } catch { uuid = string.Empty; }
                XmlNodeList nfecha = doc.GetElementsByTagName("fechaTimbrado");
                try { fechaTimbrado = nfecha[0].InnerText; } catch { fechaTimbrado = string.Empty; }
                XmlNodeList nfacturaTimbrada = doc.GetElementsByTagName("facturaTimbradaB64");
                try { facturaTimbrada = nfacturaTimbrada[0].InnerText; } catch { facturaTimbrada = string.Empty; }
                XmlNodeList nanioMes = doc.GetElementsByTagName("anioMes");
                try { anioMes = int.Parse(nanioMes[0].InnerText); } catch { anioMes = 0; }
                XmlNodeList nPdf = doc.GetElementsByTagName("pdfB64");
                try { pdfB64 = nPdf[0].InnerText; } catch { pdfB64 = string.Empty; }

                string _facturaTimbrada = Statics.Base64Decode(facturaTimbrada);

                GuardaTablaTimbrado(i, IdUsuario, i.IdPeriodoNomina, uuid, fechaTimbrado, anioMes, _facturaTimbrada, i.Leyenda);
            }
        }

        /// <summary>
        /// Metodo para emitir timbrado de nómina basado en un archivo JSON
        /// </summary>
        /// <param name="json">Informacion de la nomina en formato JSON</param>
        /// <param name="NoControl">Número de control</param>
        /// <param name="LeyendaB64">Leyenda en base 64</param>
        /// <returns>Respuesta</returns>
        public string Emitir(string json, string NoControl, string LeyendaB64)
        {

            string contentXML = "";
            string response = "";

            contentXML += "<soapenv:Envelope xmlns:ser=\"http://www.masnegocio.com/servicios\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            contentXML += "<soapenv:Header/>";
            contentXML += "<soapenv:Body>";
            contentXML += "<ser:EmisionRequest>";
            contentXML += "<ser:token>" + Token + "</ser:token>";

            if (!String.IsNullOrEmpty(NoControl))
            {
                contentXML += "<ser:noControl>" + NoControl.Trim() + "</ser:noControl>";
            }

            contentXML += "<ser:plantillaPDF>D132931C-5375-49C4-9A53-D54BC80FDF64</ser:plantillaPDF>";
            contentXML += "<ser:jsonB64>" + json + "</ser:jsonB64>";
            contentXML += "<ser:datosAdicionalesJSONB64>" + LeyendaB64 + "</ser:datosAdicionalesJSONB64>";
            contentXML += "</ser:EmisionRequest>";
            contentXML += "</soapenv:Body>";
            contentXML += "</soapenv:Envelope>";

            Uri uri = new Uri(URI);
            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.ContentType = "text/xml;charset=\"UTF-8\"";

            webRequest.ContentLength = contentXML.Length;

            webRequest.Headers.Add("SOAPAction", "\"http://www.masnegocio.com/servicios/emision\"");
            webRequest.Method = "POST";

            webRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(contentXML), 0, contentXML.Length);

            
            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }

            }
            catch { }
            

            return response;
        }


        /// <summary>
        /// Metodo para emitir timbrado de nomina basado en el XML ya sellado
        /// </summary>
        /// <param name="xmlB64">XML ya sellado en base 64</param>
        /// <param name="NoControl">No de control que sirve para no duplicar timbrado.</param>
        /// <returns></returns>
        private string Emitir(string xmlB64, string NoControl)
        {
            string contentXML = "";
            string response = "";

            contentXML += "<soapenv:Envelope xmlns:ser=\"http://www.masnegocio.com/servicios\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            contentXML += "<soapenv:Header/>";
            contentXML += "<soapenv:Body>";
            contentXML += "<ser:EmisionRequest>";
            contentXML += "<ser:token>" + Token + "</ser:token>";

            if (!string.IsNullOrEmpty(NoControl))
            {
                contentXML += "<ser:noControl>" + NoControl.Trim() + "</ser:noControl>";
            }

            contentXML += "<ser:xmlB64>" + xmlB64 + "</ser:xmlB64>";
            contentXML += "</ser:EmisionRequest>";
            contentXML += "</soapenv:Body>";
            contentXML += "</soapenv:Envelope>";

            Uri uri = new Uri(URI);
            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.ContentType = "text/xml;charset=\"UTF-8\"";

            webRequest.ContentLength = contentXML.Length;

            webRequest.Headers.Add("SOAPAction", "\"http://www.masnegocio.com/servicios/emision\"");
            webRequest.Method = "POST";

            webRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(contentXML), 0, contentXML.Length);

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
            }

            return response;
        }
    }
}