using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Services
{
    public class sAcceso
    {
        /// <summary>
        ///     Método que autoriza el acceso a la aplicación
        /// </summary>
        /// <param name="model">Variable que contiene los datos de acceso</param>
        /// <returns>Datos del empleado que inició sesión</returns>
        /// <exception cref="Exception">Mensaje con el error en caso de que el acceso sea denegado</exception>
        public ModelUser sGetAcceso(ModelLogin model)
        {
            try
            {
                var servicio = "/api/Login/Authenticate";
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                var data = JsonConvert.SerializeObject(model);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";                    
                    var result = wc.UploadString(apiUrl, data);

                    var list = JsonConvert.DeserializeObject<ModelUser>(result);

                    return list;
                }
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}