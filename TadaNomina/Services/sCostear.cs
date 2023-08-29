using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Services
{
    public class sCostear
    {
        /// <summary>
        ///     Método que obtiene una lista de costeos
        /// </summary>
        /// <param name="datos">Variable que contiene los datos de un costeo</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la inforamción de los costeos</returns>
        public List<mResultCosteo> GetCosteo(mGetCosteo datos, string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costear/getCosteo");
                var _datos = JsonConvert.SerializeObject(datos);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.UploadString(url, _datos);

                    var datResult = JsonConvert.DeserializeObject<List<mResultCosteo>>(result);

                    return datResult;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }            
        }

        /// <summary>
        ///     Método que guarda un nuevo costeo
        /// </summary>
        /// <param name="datos">Variable que contiene la información del costeo</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="periodos">Variable que contiene los periodos</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string GuardartCosteo(mGetCosteo datos, int IdCliente, int IdUnidadNegocio, string periodos,  string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costear/guardaCosteos?IdCliente=" + IdCliente + "&IdUnidadNegocio=" + IdUnidadNegocio + "&periodos=" + periodos);
                var _datos = JsonConvert.SerializeObject(datos);

                using (var wc = new WebClient())
                {
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.UploadString(url, _datos);

                    return result;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        /// <summary>
        ///     Método que guarda 
        /// </summary>
        /// <param name="datos">Variable que contiene la información del costeo</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="periodos">Variable que contiene los periodos</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string GuardartCosteoDif(mGetCosteo datos, int IdCliente, int IdUnidadNegocio, string periodos, string Token)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costear/guardaCosteosDif?IdCliente=" + IdCliente + "&IdUnidadNegocio=" + IdUnidadNegocio + "&periodos=" + periodos);
                var _datos = JsonConvert.SerializeObject(datos);

                using (var wc = new WebClient())
                {
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + Token;
                    var result = wc.UploadString(url, _datos);

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