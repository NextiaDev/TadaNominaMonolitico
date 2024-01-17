using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Services
{
    public class sIncidencias
    {
        /// <summary>
        ///     Método que obtiene una lista de incidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información de las incidencias</returns>
        public List<vIncidencias> GetvIncindencias(int IdPeriodoNomina, string token)
        {
            try
            {
                var servicio = "/api/Incidencias/GetListIncidencias?IdPeriodo=" + IdPeriodoNomina;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var list = JsonConvert.DeserializeObject<List<vIncidencias>>(result);

                    return list;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que obtiene una incidencia
        /// </summary>
        /// <param name="IdIncidencia">Variable que contiene el id de la incidencia</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Modelo con la información de la incidencia</returns>
        public vIncidencias GetvIncindencia(int IdIncidencia, string token)
        {
            try
            {
                var servicio = "/api/Incidencias/GetIncidencia?IdIncidencia=" + IdIncidencia;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding= System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.DownloadString(apiUrl);

                    var incidecia = JsonConvert.DeserializeObject<vIncidencias>(result);

                    return incidecia;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que cambia el estado de la incidencia a "eliminado"
        /// </summary>
        /// <param name="IdIncidencia">Variable que contiene el id de la indicencia</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string DeleteIncindencia(int IdIncidencia, string token)
        {
            try
            {
                var servicio = "/api/Incidencias/DeleteIncidencia?IdIncidencia=" + IdIncidencia;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.UploadString(apiUrl, "");

                    return result;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que elimina el registro de las incidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns></returns>
        public string DeleteAllIncindencia(int IdPeriodoNomina, string token)
        {
            try
            {
                var servicio = "/api/Incidencias/DeleteAllIncidencias?IdPeriodoNomina=" + IdPeriodoNomina;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;
                    var result = wc.UploadString(apiUrl, "");

                    return result;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }
    }
}