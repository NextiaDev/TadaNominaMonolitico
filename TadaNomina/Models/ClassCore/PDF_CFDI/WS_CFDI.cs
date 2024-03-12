using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ClassCore.PDF_CFDI
{
    public class WS_CFDI
    {
        /// <summary>
        ///     Método que obtiene el CFDI en formato PDF
        /// </summary>
        /// <param name="xml">Cadena con el XML para timbrar</param>
        /// <param name="leyenda">Leyenda que aparece en el CFDI</param>
        /// <param name="Firma">Firma del empleado</param>
        /// <param name="SMO">Sueldo mínimo Oficial</param>
        /// <returns>Cadena con el archivo en b64</returns>
        public string getPDF(string xml, string leyenda, string Firma, decimal? SMO)
        {
            string xmlB64 = Statics.Base64Encode(xml);
            string servidor = ClassSistema.getUrlSistema() + "/WS_CFDI";
            //string servidor = "http://localhost:51218/";
            Uri apiUrl = new Uri(servidor + "/api/Archivos/GetPDF2");

            ModelGetRecibos datos = new ModelGetRecibos() 
            {
                xmlB64 = xmlB64,
                Leyenda = leyenda,
                firmaB64 = Firma,
                SMO = SMO
            };

            var _datos = JsonConvert.SerializeObject(datos);

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Clear();
            client.Headers["Content-type"] = "application/json";
            string arch = client.UploadString(apiUrl, _datos);

            var _arch = JsonConvert.DeserializeObject(arch);
            return _arch.ToString().Trim();
        }

        /// <summary>
        ///     Método que obtiene el CFDI en formato PDF
        /// </summary>
        /// <param name="xml">Cadena con el XML para timbrar</param>
        /// <param name="leyenda">Leyenda que aparece en el CFDI</param>
        /// <param name="Firma">Firma del empleado</param>
        /// <param name="SMO">Sueldo mínimo Oficial</param>
        /// <param name="direccionPatrona">Dirección física de la patrona</param>
        /// <param name="SD">Sueldo diario</param>
        /// <param name="IdSindicatoClientes">d sindicato del cliente</param>
        /// <param name="banderaSindicalizados">Bandera para saber si aparece el dato "Sindicalizado" en el CFDI</param>
        /// <param name="idGrupo">Id del grupo al que pertenece el cliente</param>
        /// <returns>Cadena con el archivo en b64</returns>
        public string getPDF(string xml, string leyenda, string Firma, decimal? SMO, string direccionPatrona, decimal? SD, int? IdSindicatoClientes, string banderaSindicalizados, int? idGrupo, string DireccionEmpleado)
        {
            string xmlB64 = Statics.Base64Encode(xml);
            string servidor = ClassSistema.getUrlSistema() + "/WS_CFDI";
            //string servidor = "http://localhost:51218/";
            Uri apiUrl = new Uri(servidor + "/api/Archivos/GetPDF2");

            ModelGetRecibos datos = new ModelGetRecibos()
            {
                xmlB64 = xmlB64,
                Leyenda = leyenda,
                firmaB64 = Firma,
                SMO = SMO,
                SD = SD,
                DireccionPatrona = direccionPatrona,
                IdSindicatoClientes = IdSindicatoClientes,
                BanderaSindicalizados = banderaSindicalizados,
                IdGrupo = idGrupo,
                DireccionEmpleado = DireccionEmpleado
            };

            var _datos = JsonConvert.SerializeObject(datos);

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Clear();
            client.Headers["Content-type"] = "application/json";
            string arch = client.UploadString(apiUrl, _datos);

            var _arch = JsonConvert.DeserializeObject(arch);
            return _arch.ToString().Trim();
        }

        /// <summary>
        ///     Método que guarda el PDF
        /// </summary>
        /// <param name="xml">Cadena con el XML para timbrar</param>
        /// <param name="leyenda">Leyenda que aparece en el CFDI</param>
        /// <param name="ruta">Ruta del archivo</param>
        /// <param name="Firma">Firma del empleado<param>
        /// <param name="SMO">Sueldo mínimo Oficial</param>

        public void guardaPDF(string xml, string leyenda, string ruta, string Firma, decimal? SMO)
        {
            string[] firmas;
            string _firma = string.Empty;
            if (Firma != null)
            {
                firmas = Firma.Split(',');
                _firma = firmas[1];
            }
            
            string b64 = getPDF(xml, leyenda, _firma, SMO);
            byte[] bytes = System.Convert.FromBase64String(b64);

            using (FileStream stream = new FileStream(ruta, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, false))
                {
                    writer.Write(bytes, 0, bytes.Length);
                    writer.Close();
                }
                   
            }
        }

        /// <summary>
        ///     Método que guarda el PDF
        /// </summary>
        /// <param name="xml">Cadena con el XML para timbrar</param>
        /// <param name="leyenda">Leyenda que aparece en el CFDI</param>
        /// <param name="ruta">Ruta del archivo</param>
        /// <param name="Firma">Firma del empleado</param>
        /// <param name="SMO">Sueldo mínimo Oficial</param>
        /// <param name="direccionPatrona">Dirección física de la patrona</param>
        /// <param name="SD">Sueldo diario</param>
        /// <param name="IdSindicatoClientes">Id sindicato del cliente</param>
        /// <param name="banderaSindicalizados">Bandera para saber si aparece el dato "Sindicalizado" en el CFDI</param>
        /// <param name="idGrupo">Id del grupo al que pertenece el cliente</param>
        public void guardaPDF(string xml, string leyenda, string ruta, string Firma, decimal? SMO, string direccionPatrona, decimal? SD, int? IdSindicatoClientes, string banderaSindicalizados, int? idGrupo, string direccionEmpleado)
        {
            string[] firmas;
            string _firma = string.Empty;
            if (Firma != null)
            {
                firmas = Firma.Split(',');
                _firma = firmas[1];
            }

            string b64 = getPDF(xml, leyenda, _firma, SMO, direccionPatrona, SD, IdSindicatoClientes, banderaSindicalizados, idGrupo, direccionEmpleado);
            byte[] bytes = System.Convert.FromBase64String(b64);

            using (FileStream stream = new FileStream(ruta, FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, false);
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();
            }
        }
    }
}
