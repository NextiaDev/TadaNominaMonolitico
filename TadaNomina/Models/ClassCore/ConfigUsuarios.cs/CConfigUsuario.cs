using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.ConfigUsuario;
using TadaNomina.Services;
using static ClosedXML.Excel.XLPredefinedFormat;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TadaNomina.Models.ClassCore.ConfigUsuarios.cs
{
    public class CConfigUsuario
    {

        public List<MInfoUsuario> GetUsuariosActive(string token)
        {
            var servicio = "/api/Usuarios/GetUsuarios";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<MInfoUsuario> result = new List<MInfoUsuario>();
                try { result = JsonConvert.DeserializeObject<List<MInfoUsuario>>(responsebody); } catch { result = new List<MInfoUsuario>(); }
                return result;
            }
        }

        public List<Cat_Clientes> GetCklientesToSelect(string token)
        {
            var servicio = "/api/Usuarios/GetClienteToAsign";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<Cat_Clientes> result = new List<Cat_Clientes>();
                try { result = JsonConvert.DeserializeObject<List<Cat_Clientes>>(responsebody); } catch { result = new List<Cat_Clientes>(); }
                return result;
            }
        }

        public List<Cat_UnidadNegocio> GetUnidadesNegocioToAsign(string token)
        {
            var servicio = "/api/Usuarios/GetUnidadesNegocioToAsign";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<Cat_UnidadNegocio> result = new List<Cat_UnidadNegocio>();
                try { result = JsonConvert.DeserializeObject<List<Cat_UnidadNegocio>>(responsebody); } catch { result = new List<Cat_UnidadNegocio>(); }
                return result;
            }
        }

        public MResultCRUD AddUsuario(string token, MaddUsuario request)
        {
            var result = new MResultCRUD();
            try
            {
                var servicio = "/api/Usuarios/AddUsuario";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);
                var modelrequest = new
                {
                    idCliente = request.IdCliente,
                    idUnidadNegocio = request.IdUnidadNegocio,
                    nombre = request.Nombre,
                    apellidoPaterno = request.ApellidoPaterno,
                    apellidoMaterno = request.ApellidoMaterno,
                    correo = request.correo,
                    usuario = request.Usuario.ToUpper(),
                    password = request.Password,
                    nomina = request.Nomina,
                    rhCloud = request.RHCloud,
                    imss = request.IMSS,
                    contabilidad = request.Contabilidad,
                    tesoreria = request.Tesoreria,
                    tipo = request.TIPO
                };
                var data = JsonConvert.SerializeObject(modelrequest);
                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var response = wc.UploadString(apiUrl, data);
                }
                return result = new MResultCRUD
                {
                    Result = "OK",
                    Mensaje = "El Usuario se creó correctamente"
                };
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                switch (error)
                {
                    case "The remote server returned an error: (409) Conflict.":
                        result = new MResultCRUD
                        {
                            Result = "INVALIDO",
                            Mensaje = "Ya existe un usuario con el mismo nombre o un el correo elelectronico ya esta registrado"
                        };
                        break;
                    case "The remote server returned an error: (400) Conflict.":
                        result = new MResultCRUD
                        {
                            Result = "INVALIDO",
                            Mensaje = "Los datos capturados son incorrectos."
                        };
                        break;
                }
                return result;
            }
        }

        public MResultCRUD EditUsuario(string token, MaddUsuario request)
        {
            var result = new MResultCRUD();
            try
            {
                var servicio = "/api/Usuarios/EditUsuario";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);
                var modelrequest = new
                {
                    idUsuario = request.IdUsuario,
                    idCliente = request.IdCliente,
                    idUnidadNegocio = request.IdUnidadNegocio,
                    nombre = request.Nombre,
                    apellidoPaterno = request.ApellidoPaterno,
                    apellidoMaterno = request.ApellidoMaterno,
                    correo = request.correo,
                    usuario = request.Usuario.ToUpper(),
                    nomina = request.Nomina,
                    rhCloud = request.RHCloud,
                    imss = request.IMSS,
                    contabilidad = request.Contabilidad,
                    tesoreria = request.Tesoreria,
                };
                var data = JsonConvert.SerializeObject(modelrequest);
                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var response = wc.UploadString(apiUrl, data);
                }
                return result = new MResultCRUD
                {
                    Result = "OK",
                    Mensaje = "El Usuario se editó correctamente"
                };
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                switch (error)
                {
                    case "The remote server returned an error: (409) Conflict.":
                        result = new MResultCRUD
                        {
                            Result = "INVALIDO",
                            Mensaje = "Ya existe un usuario con el mismo nombre o un el correo elelectronico ya esta registrado"
                        };
                        break;
                    case "The remote server returned an error: (400) Conflict.":
                        result = new MResultCRUD
                        {
                            Result = "INVALIDO",
                            Mensaje = "Los datos capturados son incorrectos."
                        };
                        break;
                }
                return result;
            }
        }

        public MResultCRUD DeleteUsuario(string token, int IdUsuario)
        {
            var result = new MResultCRUD();
            try
            {
                var servicio = "/api/Usuarios/DeleteUsuario";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);
                var modelrequest = new
                {
                    idUsuario = IdUsuario
                };
                var data = JsonConvert.SerializeObject(modelrequest);
                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var response = wc.UploadString(apiUrl, "DELETE", data);
                }
                return result = new MResultCRUD
                {
                    Result = "OK",
                    Mensaje = "El Usuario se eliminó correctamente"
                };
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                switch (error)
                {
                    case "The remote server returned an error: (400) Conflict.":
                        result = new MResultCRUD
                        {
                            Result = "INVALIDO",
                            Mensaje = "Los datos son incorrectos."
                        };
                        break;
                }
                return result;
            }
        }
    }
}