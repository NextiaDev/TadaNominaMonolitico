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
using TadaNomina.Services.CFDI40;

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
            List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(X=> X.IdPeriodoNomina).ToList();
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
        public void CancelaPeriodoNomina(int IdPeriodoNomina, string ClaveSAT, string[] claves, int IdUsuario, Guid Id)
        {
            var timbrados = ObtenDatosTimbradoNominaPeriodo(IdPeriodoNomina);

            if (claves != null && claves.Count() > 0)
                timbrados = timbrados.Where(x => claves.Contains(x.ClaveEmpleado)).ToList();

            foreach (var item in timbrados)
            {
                Cancelar(item, ClaveSAT, string.Empty, Id, IdUsuario);
            }
        }

        public List<TimbradoNomina> getTimbradosPeriodoRFC(int IdPeriodo, string RFC)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var cfdis = entidad.TimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.RFC == RFC && x.IdEstatus == 1).OrderByDescending(x => x.FechaTimbrado);

                return cfdis.ToList();
            }
        }

        public XmlNomina getXMlNomina(int IdXMl)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = entidad.XmlNomina.Where(x => x.IdXml == IdXMl).FirstOrDefault();

                return registro;
            }
        }

        public void cancelaCFDISRelacionadosPrevios(int IdPeriodoNomina, Guid id, int IdUsuario)
        {
            var relacionados = getRelacionados(IdPeriodoNomina);

            foreach (var item in relacionados)
            {
                if (item.cantidad >= 2)
                {
                    var cfdis = getTimbradosPeriodoRFC(IdPeriodoNomina, item.rfc);
                    var idXMl = cfdis[0].IdXml ?? 0;
                    var uuidNuevo = cfdis[0].FolioUDDI;
                    var CFDIRel = getXMlNomina(idXMl).FoliosUUIDRelacionados;

                    if (CFDIRel != null && CFDIRel != string.Empty)
                        CancelaPeriodoNominaRelacion(IdPeriodoNomina, CFDIRel, uuidNuevo, "01", null, IdUsuario, id);
                }
            }
        }

        /// <summary>
        /// Metodo que obtiene los datos para cancelar timbrado con relacion o sustitución de CFDI.
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="Id"></param>
        /// <param name="Tipo"></param>
        public void CancelaPeriodoNominaRelacion(int IdPeriodoNomina, string FolioUUIDSeparadoComas, string FolioRelacionado, string ClaveSAT, string[] claves, int IdUsuario, Guid Id)
        {
            List<string> folios = FolioUUIDSeparadoComas.Split(',').ToList();
            var timbrados = ObtendatosTimbradoNominaByFoliosUUID(IdPeriodoNomina, folios);

            if (claves != null && claves.Count() > 0)
                timbrados = timbrados.Where(x => claves.Contains(x.ClaveEmpleado)).ToList();

            foreach (var item in timbrados)
            {
                Cancelar(item, ClaveSAT, FolioRelacionado, Id, IdUsuario);
            }
        }

        public List<modelRelacionados> getRelacionados(int IdPeriodo)
        {
            var consulta = "select count(rfc) as cantidad, rfc from TimbradoNomina where IdPeriodoNomina = " + IdPeriodo + " and IdEstatus = 1 group by rfc";

            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var result = entidad.Database.SqlQuery<modelRelacionados>(consulta);

                return result.ToList();
            }
        }

        public void Cancelar(vTimbradoNomina datos, string ClaveSat, string FolioRelacionado, Guid id, int IdUsuario)
        {
            try
            {
                string Exito = string.Empty;
                string uuid = string.Empty;
                string Detalle = string.Empty;

                try { creaPfx(datos.rutaCer, datos.rutaKey, datos.KeyPass.Trim(), datos.PFXCancelacionTimbrado); } catch (Exception ex) { throw new Exception("No se pudo crear el archivo PFX.", ex); }
                byte[] pfx = Array.Empty<byte>();
                try { pfx = File.ReadAllBytes(datos.PFXCancelacionTimbrado); } catch (Exception ex) { throw new Exception("No se puede leer el archivo PFX.", ex); }
                var xml = getXMLCancelacionSignature(datos.RFC_Patronal, datos.FolioUDDI, ClaveSat, FolioRelacionado, pfx, datos.KeyPass);
                string base64EncodedExternalAccount = Statics.Base64Encode(xml);

                var cancelar = CancelarTimbre(base64EncodedExternalAccount, datos.Neto, datos.RFC);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cancelar);
                XmlNodeList nExito = doc.GetElementsByTagName("exito");                
                Exito = nExito[0].InnerText.ToUpper();
                
                if (Exito == "TRUE")
                    ActualizaRegistroTimbraado(datos.FolioUDDI, IdUsuario);

                if (Exito == "FALSE")
                {
                    XmlNodeList nDetalle = doc.GetElementsByTagName("codigo");                    
                    XmlNodeList nDetalleText = doc.GetElementsByTagName("texto");                    
                    Detalle = nDetalle[0].InnerText.ToUpper();
                    Detalle += " - ";
                    Detalle += nDetalleText[0].InnerText.ToUpper();

                    GuardaErrorCancelacion(datos, id, Detalle, IdUsuario);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void creaPfx(string rutaCer, string rutaKey, string pass, string ruta)
        {
            if (!File.Exists(ruta))
            {
                var oPFX = new Pfx(rutaCer, rutaKey, pass, ruta, Path.GetDirectoryName(ruta) + "/");

                oPFX.creaPFX();
            }
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
                    GuardaErrorCancelacion(datos, id, "", IdUsuario);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private object CreaJSONCancelacion(vTimbradoNomina datos)
        {
            throw new NotImplementedException();
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

        public string CancelarTimbre(string xmlb64, decimal? total, string rfcReceptor)
        {
            String contentXML = "";
            String response = "";
            Get_Token("Produccion");

            try
            {
                contentXML += "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://www.masnegocio.com/servicios\">";
                contentXML += "<soapenv:Header/>";
                contentXML += "<soapenv:Body>";
                contentXML += "<ser:CancelacionRequest>";
                contentXML += "<ser:token>" + Token + "</ser:token>";
                contentXML += "<ser:xmlB64>" + xmlb64 + "</ser:xmlB64>";
                contentXML += "<ser:liberarNoControl>true</ser:liberarNoControl>";
                contentXML += "<ser:total>" + total + "</ser:total>";
                contentXML += "<ser:rfcReceptor>" + rfcReceptor + "</ser:rfcReceptor>";
                contentXML += "<ser:version>4.0</ser:version>";
                contentXML += "</ser:CancelacionRequest>";
                contentXML += "</soapenv:Body>";
                contentXML += "</soapenv:Envelope>";

                Uri uri = new Uri(URI);

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
