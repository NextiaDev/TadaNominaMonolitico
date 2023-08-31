using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Services
{
    public class sUnidadNegocio
    {
        /// <summary>
        ///     Método que obtiene una lista de Unidades de negocio
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con información de las unidades de negocio</returns>
        public List<ModelUnidadNegocio> getSelectUnidadesNegocio(int IdCliente, string token)
        {
            try
            {
                var servicio = "/api/UnidadNegocio/getSelectUnidadNegocio?IdCliente=" + IdCliente;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {                    
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<List<ModelUnidadNegocio>>(result);

                    return list;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }
    }
}