using Delva.AppCode.TimbradoTurboPAC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ClassCancelarTimbrado: ClassProcesosTimbrado
    {
        /// <summary>
        /// Metodo para generar modelo para cancelacion de timbrado por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Uniad de negocio</param>
        /// <returns>Modelo con información para cancelar timbrado</returns>
        public ModelCancelarTimbrado GetModeloTimbradoNomina(int IdUnidadNegocio)
        {
            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            ClassCancelar cct = new ClassCancelar();
            ModelCancelarTimbrado model = new ModelCancelarTimbrado();
            List<SelectListItem> lperiodos = new List<SelectListItem>();
            List<SelectListItem> lMotivos = new List<SelectListItem>();
            List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoNominasAcumuladas(IdUnidadNegocio);
            List<Cat_MotivosCancelacionSAT> lmotivos = cct.getMotivosCancelacionSAT();

            lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });
            lmotivos.ForEach(x=> { lMotivos.Add(new SelectListItem { Text = x.Clave + " - " + x.Descripcion, Value = x.Clave }); });

            model.lPeriodos = lperiodos;
            model.lMotivosCancalacion = lMotivos;

            return model;
        }

        /// <summary>
        /// Metodo que obtiene los datos para cancelar timbrado
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="Id"></param>
        /// <param name="Tipo"></param>
        public void CancelacionPeriodoNomina(int IdPeriodoNomina, int IdUsuario, Guid Id, string Tipo)
        {            
            var datos = ObtenDatosTimbradoNominaPeriodo(IdPeriodoNomina);
            foreach (var item in datos)
            {
                Cancelar(item, Id, Tipo, IdUsuario);
            }
        }        

        /// <summary>
        /// Metodo para cancelar timbrado
        /// </summary>
        /// <param name="datos">Datos del timbrado</param>
        /// <param name="id"></param>
        /// <param name="Tipo"></param>
        /// <param name="IdUsuario">Usuario</param>
        /// <exception cref="Exception">En caso de algun problema, este metodo cacha el error</exception>
        public void Cancelar(vTimbradoNomina datos, Guid id, string Tipo, int IdUsuario)
        {
            try
            {
                string Exito = string.Empty;
                string uuid = string.Empty;
                                
                string json = JsonConvert.SerializeObject(CreaJSONCancelacion(datos));
                string base64EncodedExternalAccount = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                var cancelar = CancelarTimbre(base64EncodedExternalAccount, Tipo);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cancelar);
                XmlNodeList nExito = doc.GetElementsByTagName("exito");
                Exito = nExito[0].InnerText.ToUpper();

                if (Exito == "TRUE")
                {
                    XmlNodeList nuuid = doc.GetElementsByTagName("uuid");
                    try { uuid = nuuid[0].InnerText; } catch { uuid = string.Empty; }
                    ActualizaRegistroTimbraado(uuid, IdUsuario);
                }
                if (Exito == "FALSE")
                {
                    GuardaErrorCancelacion(datos, id, IdUsuario);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Metodo para solicitar cancelacion de timbrado
        /// </summary>
        /// <param name="JSON">Información del timbrado</param>
        /// <param name="Tipo"></param>
        /// <returns>Respuesta de timbrado</returns>
        public string CancelarTimbre(string JSON, string Tipo)
        {
            String contentXML = "";
            String response = "";            
            Get_Token(Tipo);
            
            try
            {
                contentXML += "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://www.masnegocio.com/servicios\">";
                contentXML += "<soapenv:Header/>";
                contentXML += "<soapenv:Body>";
                contentXML += "<ser:CancelacionRequest>";
                contentXML += "<ser:token>" + Token + "</ser:token>";
                contentXML += "<ser:jsonB64>" + JSON + "</ser:jsonB64>";
                contentXML += "<ser:liberarNoControl>true</ser:liberarNoControl>";
                contentXML += "<ser:version>3.3</ser:version>";
                contentXML += "</ser:CancelacionRequest>";
                contentXML += "</soapenv:Body>";
                contentXML += "</soapenv:Envelope>";

                Uri uri = new Uri("https://face-timbre.masnegocio.com:8585/mn-pac-servicios/servicios");

                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

                webRequest.ContentType = "text/xml;charset=\"UTF-8\"";

                webRequest.ContentLength = contentXML.Length;

                webRequest.Headers.Add("SOAPAction", "\"http://www.masnegocio.com/servicios/cancelacion\"");
                webRequest.Method = "POST";

                webRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(contentXML), 0, contentXML.Length);

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
        /// Cancelación de timbrado por Claves
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="Id"></param>
        /// <param name="Tipo"></param>
        /// <param name="Claves"></param>
        public void CancelacionPeriodoNominaClaves(int IdPeriodoNomina, int IdUsuario, Guid Id, string Tipo, string[] Claves)
        {            
            var datos = ObtenDatosTimbradoNominaPeriodo(IdPeriodoNomina).Where(x => x.IdEstatus == 1 && Claves.Contains(x.ClaveEmpleado));
            foreach (var item in datos)
            {
                Cancelar(item, Id, Tipo, IdUsuario);
            }
        }

        /// <summary>
        /// Cancelacion de timbrado por Folio fiscal
        /// </summary>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="Id"></param>
        /// <param name="FolioUUID">Folio fiscal</param>
        /// <param name="Tipo"></param>
        public void CancelacionNominaUUID(int IdUsuario, Guid Id, string[] FolioUUID, string Tipo)
        {            
            List<vTimbradoNomina> datos = ObtenDatosTimbradoNominaSinPeriodo(FolioUUID);
            foreach (var item in datos)
            {
                Cancelar(item, Id, Tipo, IdUsuario);
            }
        }

    }
}