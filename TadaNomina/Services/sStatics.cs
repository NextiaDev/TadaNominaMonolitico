using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TadaNomina.Services
{
    public class sStatics
    {
        /// <summary>
        ///     Rutas de los servidores donde están publicados los WebServices
        /// </summary>


        public static string ruaWC = ConfigurationManager.AppSettings.Get("API_Connection");
        public static string servidor = ruaWC;
        //public static string servidor = "http://localhost:25677";

        public static string ruaWCC = ConfigurationManager.AppSettings.Get("API_ConnectionCont");
        public static string ServidorContabilidad = ruaWCC;
        //public static string ServidorContabilidad = "https://localhost:44352/";

        public static string relojChecador = "https://servicios.tada.mx/api_nomina/";

        //Datos de conexion TP QA
        //public static string rutaAPI_TP = "https://uat.turbopac.mx/APITimbrador/";
        //Datos de conexion TP Prod
        
        //public static string rutaAPI_TP = "https://bovedaemision.turbopac.mx/TPApiTimbradoV40/";
        public static string rutaAPI_TP = "https://bovedaemision.turbopacmx.com/TPApiTimbradoV40/";

        public static string username = "Tadamx";
        public static string password = "BTKfA6ZStvwAy%2BUXkWldYA%3D%3D";
    }
}