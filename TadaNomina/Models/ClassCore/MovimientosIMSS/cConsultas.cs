using Newtonsoft.Json;
using RestSharp;
using SW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cConsultas
    {
        public List<Cat_RegistroPatronal> GetRegistrosPatronales(int IdCliente)
        {
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.IdEstatus == 1 && p.CertificadoIMSS != null && p.UsuarioIMSS != null && p.ContraseñaIMSS != null && p.IdCliente == IdCliente).ToList();
                return query;
            }
        }

        public RespuestaLotesIMSS GetLotesByRegistroPatronal(int IdRegistroPatronal)
        {
            var response = new RespuestaLotesIMSS();
            var reg = GetInfoRegistroPatronalById(IdRegistroPatronal);
            byte[] cer = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
            string certi = Convert.ToBase64String(cer);
            if(reg.KeyIMSS != null)
            {
                byte[] k = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                string key = Convert.ToBase64String(k);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = key,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = ""
                });
                //var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/movtos/lotes");
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", json, ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);
                //var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaLotesIMSS>(response.Content);
                //return responseDESERETI;

                Uri servicio = new Uri("http://www.desereti.com/tada/services/afiliacion/movtos/lotes");
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(900000);
                    var respuesta = httpClient.PostAsync(servicio, contenido).Result;
                    respuesta.EnsureSuccessStatusCode();
                    var formatRespuesta = respuesta.Content.ReadAsStringAsync().Result;
                    response = JsonConvert.DeserializeObject<RespuestaLotesIMSS>(formatRespuesta);
                }
                return response;
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = ""
                });

                Uri servicio = new Uri("http://www.desereti.com/tada/services/afiliacion/movtos/lotes");
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                //var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/movtos/lotes");
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", json, ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);
                //var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaLotesIMSS>(response.Content);
                //return responseDESERETI;

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(900000);
                    var respuesta = httpClient.PostAsync(servicio, contenido).Result;
                    respuesta.EnsureSuccessStatusCode();
                    var formatRespuesta = respuesta.Content.ReadAsStringAsync().Result;
                    response = JsonConvert.DeserializeObject<RespuestaLotesIMSS>(formatRespuesta);
                }
                return response;
            }
        }

        public mDetalleLote GetDetalleLote(int IdRegistroPatronal, string Lote)
        {
            mDetalleLote mdl = new mDetalleLote();
            mdl.RespuestaGeneral = GetRespuestaLote(IdRegistroPatronal, Lote);
            mdl.RespuestaDetalle = GetRespuestaDetalleLote(IdRegistroPatronal, Lote);
            return mdl;
        }

        public mRespuestaLote GetRespuestaLote(int IdRegistroPatronal, string Lote)
        {
            var reg = GetInfoRegistroPatronalById(IdRegistroPatronal);
            if (reg.KeyIMSS != null)
            {
                byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
                string certi = Convert.ToBase64String(archivo);
                byte[] archivokey = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                string keyIMSS = Convert.ToBase64String(archivokey);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = keyIMSS,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote,
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/respuesta");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mRespuestaLote>(response.Content);

                return (responseDESERETI);
            }
            else
            {
                byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
                string certi = Convert.ToBase64String(archivo);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote,
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/respuesta");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mRespuestaLote>(response.Content);

                return responseDESERETI;

            }
        }

        public mDetalleRespuestaLote GetRespuestaDetalleLote(int IdRegistroPatronal, string Lote)
        {
            var reg = GetInfoRegistroPatronalById(IdRegistroPatronal);
            if (reg.KeyIMSS != null)
            {
                byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
                string certi = Convert.ToBase64String(archivo);
                byte[] archivokey = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                string keyIMSS = Convert.ToBase64String(archivokey);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = keyIMSS,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote,
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/respuesta/detalle");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mDetalleRespuestaLote>(response.Content);

                return (responseDESERETI);
            }
            else
            {
                byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
                string certi = Convert.ToBase64String(archivo);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote,
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/respuesta/detalle");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mDetalleRespuestaLote>(response.Content);

                return responseDESERETI;
            }
        }

        public Cat_RegistroPatronal GetInfoRegistroPatronalById(int IdRegistroPatronal)
        {
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.IdRegistroPatronal == IdRegistroPatronal).FirstOrDefault();
                return query;
            }
        }

        public byte[] GetPDFRespuestaGeneral(int IdRegistroPatronal, string Lote)
        {
            var reg = GetInfoRegistroPatronalById(IdRegistroPatronal);
            byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
            string certi = Convert.ToBase64String(archivo);

            if (reg.KeyIMSS != null)
            {
                byte[] archivo2 = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                string keyIMSS = Convert.ToBase64String(archivo2);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = keyIMSS,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/respuesta/PDF");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mPDFRespuestaGeneral>(response.Content);

                string file = responseDESERETI.RespuestaWebService.archivoReporteDetallado.archivoReporteDetallado;
                string base64data = string.Empty;
                string fileName = (responseDESERETI.RespuestaWebService.archivoReporteDetallado.nombreArchivoReporteDetallado);
                base64data = file;
                byte[] bytes = Convert.FromBase64String(base64data);
                return bytes;
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/respuesta/PDF");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mPDFRespuestaGeneral>(response.Content);

                string file = responseDESERETI.RespuestaWebService.archivoReporteDetallado.archivoReporteDetallado;
                string base64data = string.Empty;
                string fileName = (responseDESERETI.RespuestaWebService.archivoReporteDetallado.nombreArchivoReporteDetallado);
                base64data = file;
                byte[] bytes = Convert.FromBase64String(base64data);
                return bytes;
            }
        }

        public byte[] GetPDFDetalleRespuesta(int IdRegistroPatronal, string Lote)
        {
            var reg = GetInfoRegistroPatronalById(IdRegistroPatronal);
            byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
            string certi = Convert.ToBase64String(archivo);

            if (reg.KeyIMSS != null)
            {
                byte[] archivo2 = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                string keyIMSS = Convert.ToBase64String(archivo2);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = keyIMSS,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/acuseImss");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mPDFDetalleRrspuesta>(response.Content);

                string file = responseDESERETI.respuestaWebService.ArchivoReciboDispmag.archivoReciboDispmag;
                byte[] bytes = Convert.FromBase64String(file);
                return bytes;
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 10),
                    lote = Lote
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/acuseImss");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mPDFDetalleRrspuesta>(response.Content);

                string file = responseDESERETI.respuestaWebService.ArchivoReciboDispmag.archivoReciboDispmag;
                byte[] bytes = Convert.FromBase64String(file);
                return bytes;
            }
        }

        public mEmisiones GetmEmisiones(int IdRegistroPatronal)
        {
            mEmisiones me = new mEmisiones();
            me.infoRegistro = GetInfoRegistroPatronalById(IdRegistroPatronal);
            me.tiposArchivos = GetTipoArchivoEmision();
            return me;
        }

        public List<SelectListItem> GetTipoArchivoEmision()
        {
            List<SelectListItem> listatipoemision = new List<SelectListItem>();
            listatipoemision.Add(new SelectListItem() { Text = "EMA", Value = "EMA" });
            listatipoemision.Add(new SelectListItem() { Text = "EBA", Value = "EBA" });
            listatipoemision.Add(new SelectListItem() { Text = "EMA_SUA", Value = "EMA_SUA" });
            listatipoemision.Add(new SelectListItem() { Text = "EBA_SUA", Value = "EBA_SUA" });
            listatipoemision.Add(new SelectListItem() { Text = "EMA_PDF", Value = "EMA_PDF" });
            listatipoemision.Add(new SelectListItem() { Text = "EBA_PDF", Value = "EBA_PDF" });
            listatipoemision.Add(new SelectListItem() { Text = "EXCEL", Value = "EXCEL" });
            return listatipoemision;
        }

        public mRespuestaEmisiones GetRespuestaEmision(int IdRegistroPatronal, string tipoemision)
        {
            var reg = GetInfoRegistroPatronalById(IdRegistroPatronal);
            byte[] archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
            string certi = Convert.ToBase64String(archivo);
            string resultado = string.Empty;

            if (reg.KeyIMSS != null)
            {
                byte[] archivo2 = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                string keyIMSS = Convert.ToBase64String(archivo2);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = keyIMSS,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString(),
                    lote = "null",
                    tipoEmision = tipoemision
                });
                var client = new RestClient("http://www.desereti.com/tada/services/emision/descarga");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mRespuestaEmisiones>(response.Content);
                return responseDESERETI;
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString(),
                    lote = "null",
                    tipoEmision = tipoemision
                });
                var client = new RestClient("http://www.desereti.com/tada/services/emision/descarga");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mRespuestaEmisiones>(response.Content);
                return responseDESERETI;
            }
        }

    }
}