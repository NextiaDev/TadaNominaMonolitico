using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using TadaNomina.Models.ViewModels;
using TadaNomina.Services.CFDI40.Models;

namespace TadaNomina.Services.CFDI40
{
    public class sGetToken
    {
        public mAuth sGetAcceso()
        {
            try
            {
                var servicio = "api/Token?username=" + sStatics.username + "&password=" + sStatics.password;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);                

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, "POST");

                    var list = JsonConvert.DeserializeObject<mAuth>(result);

                    return list;
                }
            }
            catch (WebException wex)
            {
                string error = string.Empty;
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            error = reader.ReadToEnd();
                        }
                    }
                }

                throw new Exception(error);
            }
        }
    }
}