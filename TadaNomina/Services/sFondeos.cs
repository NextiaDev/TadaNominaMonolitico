using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;

namespace TadaNomina.Services
{
    public class sFondeos
    {
        /// <summary>
        ///     Método que obtiene una lista de fondeos
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información de los fondeos</returns>
        public List<vCosteos_Fondeos> GetFondeos(int IdUnidadNegocio, string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosFondeos/getFondeos?IdUnidadNegocio=" + IdUnidadNegocio);

                using (var wc = new WebClient())
                {
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.DownloadString(url);

                    var datResult = JsonConvert.DeserializeObject<List<vCosteos_Fondeos>>(result);

                    return datResult;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que obtiene un fondeo
        /// </summary>
        /// <param name="IdFondeo">Variable que contiene el id del fondeo</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Modelo con la información del fondeo</returns>
        public vCosteos_Fondeos GetFondeo(int IdFondeo, string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosFondeos/getFondeo?IdFondeo=" + IdFondeo);

                using (var wc = new WebClient())
                {
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.DownloadString(url);

                    var datResult = JsonConvert.DeserializeObject<vCosteos_Fondeos>(result);

                    return datResult;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que agrega un nuevo fondeo
        /// </summary>
        /// <param name="nfondeo">Variable que contiene la información del nuevo fondeo</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string AddFondeos(ModelFondeos nfondeo, int IdUnidadNegocio, string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosFondeos/AddFondeo");

                var _model = new mNewFondeo()
                {
                    IdPeriodoNomina = nfondeo.IdPeriodoNomina,
                    IdUnidadNegocio = IdUnidadNegocio,
                    IdCosteo = nfondeo.IdCosteo,
                    IdPatrona = nfondeo.IdPatrona,
                    IdDivision = nfondeo.IdDivision,
                    Descripcion = nfondeo.Descripcion,
                    Importe = nfondeo.Importe,
                    Conceptos = nfondeo.conceptos
                };

                var model = JsonConvert.SerializeObject(_model);

                using (var wc = new WebClient())
                {
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.UploadString(url, model);                    

                    return result;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que modifica el estado de un fondeo a "eliminado"
        /// </summary>
        /// <param name="IdFondeo">Variable que contiene el id del fondeo</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string deleteFondeo(int IdFondeo, string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosFondeos/deleteFondeo?Idfondeo=" + IdFondeo);

                using (var wc = new WebClient())
                {
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.UploadString(url, "");

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