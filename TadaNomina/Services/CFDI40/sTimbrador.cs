using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Services.CFDI40.Models;

namespace TadaNomina.Services.CFDI40
{
    public class sTimbrador
    {
        public mTimbradoResult sTimbrado(mTimbrar model, string Token)
        {
            try
            {
                var servicio = "api/Timbrado?access_token=" + Token;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, JsonConvert.SerializeObject(model));

                    var list = JsonConvert.DeserializeObject<mTimbradoResult>(result);

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