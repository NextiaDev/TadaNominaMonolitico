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
                BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, false);
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();
            }
        }        
    }
}