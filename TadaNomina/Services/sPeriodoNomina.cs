using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Services
{
    public class sPeriodoNomina
    {
        /// <summary>
        ///     Método que obtiene una lista de periodos de nómina
        /// </summary>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns>Lista de modelos con la información de los periodos de nómina</returns>
        public List<PeriodoNomina> getPeriodoNomina(string token, int IdUnidadNegocio)
        {
            try
            {
                var servicio = "/api/PeriodosNomina/getPeriodos?IdUnidadNegocio=" + IdUnidadNegocio;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<List<PeriodoNomina>>(result);

                    return list;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que obtiene la información de un periodo de nómina
        /// </summary>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Modelo con la información del periodo de nómina</returns>
        public PeriodoNomina getPeriodoNominaId(string token, int IdPeriodoNomina)
        {
            try
            {
                var servicio = "/api/PeriodosNomina/getPeriodo?IdPeriodoNomina=" + IdPeriodoNomina;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<PeriodoNomina>(result);

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