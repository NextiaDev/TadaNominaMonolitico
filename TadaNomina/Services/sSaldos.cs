using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Services
{
    public class sSaldos
    {
        /// <summary>
        ///     Método que obtiene una lista de saldos 
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con información de los saldos</returns>
        public List<ModelSaldos> getSaldos(int IdUnidadNegocio, string token)
        {
            try
            {
                var servicio = "/api/Saldos/getSaldos?IdUnidadNegocio=" + IdUnidadNegocio;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<List<ModelSaldos>>(result);

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