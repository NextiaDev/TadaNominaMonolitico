using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore
{
    public class Statics
    {
        public static string rutaGralArchivos = @"D:\\TadaNomina\";

        public static string Encriptar(string _cadenaAencriptar)
        {
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            return Convert.ToBase64String(encryted);
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public static string DesEncriptar(string _cadenaAdesencriptar)
        {
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            return System.Text.Encoding.Unicode.GetString(decryted);
        }

        public static String obtenFechaXML(String Fecha)
        {
            String Dia = Fecha.Substring(0, 2);
            String Mes = Fecha.Substring(3, 2);
            String Anio = Fecha.Substring(6, 4);

            String x = Anio + "-" + Mes + "-" + Dia;
            return x;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void CreateZipFile(string rutaFile)
        {
            try
            {
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    if (!Directory.Exists(rutaFile))
                        throw new Exception("No se encuentra la carpeta especificada.");
                    zip.AddDirectory(rutaFile);
                    zip.Save(rutaFile + ".zip");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string DarFormatoClaveEmpleado(string ClaveEmpleado)
        {
            string Clave = string.Empty;
            if (ClaveEmpleado.Length == 1)
                Clave = "00000" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 2)
                Clave = "0000" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 3)
                Clave = "000" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 4)
                Clave = "00" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 5)
                Clave = "0" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 6)
                Clave = ClaveEmpleado;

            if (Clave == string.Empty)
                Clave = ClaveEmpleado;

            return Clave;
        }

        public static void generaLog(int IdPeriodo, int IdUsuario, string Linea, string Modulo)
        {
            string path = Modulo;
            string nombre = "Periodo_" + IdPeriodo + "_IdUsuario_" + IdUsuario + ".txt";
            cLog oLog = new cLog();            

            oLog.AddOtroLog(Linea, path, nombre);
        }

        public static string ServidorGeoVictoriaToken = "https://customerapi.geovictoria.com";
        public static string ServidorGeoVictoriaOauth = "https://apiv3.geovictoria.com";
        public static string ServidorGeoVictoriaTokenTest = "https://customerapi-sandbox.geovictoria.com/";
        public static string ServidorGeoVictoriaOauthTest = "https://apiv3-sandbox.geovictoria.com/";
    }
}
