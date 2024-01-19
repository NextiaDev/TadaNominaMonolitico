using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TadaNomina.Services
{
    public class sException
    {
        /// <summary>
        ///     Método que obtiene las excepciones
        /// </summary>
        /// <param name="ex">Variable que contiene la excepción generada por el servidor</param>
        /// <returns>Mensaje de error</returns>
        public static Exception GetException(WebException ex)
        {
            using (var errorResponseStream = ex.Response.GetResponseStream())
            {
                using (var errorReadStream = new StreamReader(errorResponseStream, Encoding.UTF8))
                {
                    var err = errorReadStream.ReadToEnd();
                    var error = err != "" ? err : ex.Message;
                    return new Exception("Error en servicio: " + error);
                }
            }
        }
    }
}