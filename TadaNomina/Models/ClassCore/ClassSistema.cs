using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class ClassSistema
    {
        /// <summary>
        /// Método que muestra la Url del sistema.
        /// </summary>
        /// <returns>Regresa la url del sistema.</returns>
        public static string getUrlSistema()
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                string _url = string.Empty;
                var url = (from b in entidad.UrlSistema select b).FirstOrDefault();

                if (url != null)
                    _url = url.URLSistema1;

                return _url;
            }
        }

        /// <summary>
        /// Método que muestra las cuentas de correo del sistema.
        /// </summary>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public SendMail getSendCorreo()
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {

                var user = (from b in entidad.SendMail.Where(x => x.idSendMail == 2 && x.Idestatus == 1) select b).FirstOrDefault();

                return user;

            }
        }

        public SendMail getSendCorreoTada()
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var user = (from b in entidad.SendMail.Where(x => x.idSendMail == 1 && x.Idestatus == 1) select b).FirstOrDefault();

                return user;

            }
        }
    }
}