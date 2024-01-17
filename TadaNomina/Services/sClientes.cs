using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ViewModels.ConfigUsuario;

namespace TadaNomina.Services
{
    public class sClientes
    {
        /// <summary>
        ///     Método que obtiene los clientes
        /// </summary>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información del cliente </returns>
        public List<ModelCliente> getClientes(string token)
        {
            try
            {
                var servicio = "/api/Clientes/getClientes";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<List<ModelCliente>>(result);

                    return list;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que obtiene la lista de los clientes
        /// </summary>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de clientes</returns>
        public List<Cat_Clientes> getListClientes(string token)
        {
            try
            {
                var servicio = "/api/Clientes/getListClientes";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<List<Cat_Clientes>>(result);

                    return list;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que obtiene la inforamción de un cliente
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Información del cliente</returns>
        public ModelClientes getModelClienteByID(int IdCliente, string token)
        {
            try
            {
                var servicio = "/api/Clientes/getModelClienteByID?IdCliente=" + IdCliente;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var cli = JsonConvert.DeserializeObject<ModelClientes>(result);
                    cli.selectPAC = GetSlectListPAC(token);

                    return cli;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que genera un nuevo cliente
        /// </summary>
        /// <param name="m">Variable que contiene la información de los clientes</param>
        /// <param name="token">Variable que contieneel JWT para consumir una API</param>
        /// <returns>mensaje del movimiento</returns>
        public string AddCliente(ModelClientes m, string token)
        {
            try
            {
                var servicio = "/api/Clientes/AddCliente";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                var data = JsonConvert.SerializeObject( new
                {
                    Cliente = m.Cliente,
                    RazonSocial = m.RazonSocial,
                    RFC = m.RFC,
                    Telefono = m.Telefono,
                    Contacto = m.Contacto,
                    Correo = m.Correo,
                    FechaInicioProduccion = m.FechaInicioProduccion,
                    IdPAC = m.IdPAC
                });

                using (var wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.UploadString(apiUrl, data);

                    return result.ToString();
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que modifica la información de un cliente
        /// </summary>
        /// <param name="m">Variable que contiene la información del cliente</param>
        /// <param name="token">Variable que contieneel JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string UpdateCliente(ModelClientes m, string token)
        {
            try
            {
                var servicio = "/api/Clientes/UpdateCliente";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                var data = JsonConvert.SerializeObject(m);

                using (var wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.UploadString(apiUrl, data);

                    return result.ToString();
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que cambia el estado de un cliente a eliminado
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string DeleteCliente(ModelDeleteCliente model, string token)
        {
            try
            {
                var servicio = "/api/Clientes/DeleteCliente";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                var data = JsonConvert.SerializeObject(model);

                using (var wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.UploadString(apiUrl, "DELETE", data);

                    return result.ToString();
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        public ModelClientes GetInfoToCreate(string token)
        {
            var result = new ModelClientes();
            result.selectPAC = GetSlectListPAC(token);
            return result;
        }

        public List<SelectListItem> GetSlectListPAC(string token)
        {
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Seleccionar...",
                    Value = ""
                }
            };
            var pacs = GetPACs(token);
            pacs.ForEach(p => result.Add(new SelectListItem { Text = p.PAC, Value = p.IdPAC.ToString() }));
            return result;
        }

        public List<ModelPAC> GetPACs(string token)
        {
            var servicio = "/api/Clientes/GetPAC";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<ModelPAC> result = new List<ModelPAC>();
                try { result = JsonConvert.DeserializeObject<List<ModelPAC>>(responsebody); } catch { result = new List<ModelPAC>(); }
                return result;
            }
        }
    }
}