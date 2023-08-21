using DocumentFormat.OpenXml.Drawing;
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
    public class sEmisores
    {
        public mResultEmisor sAltaEmisor(mNewEmisor model, string Token)
        {
            try
            {
                var servicio = "api/AltaEmisor?access_token=" + Token;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);
                var datos = JsonConvert.SerializeObject(model);
                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, datos);

                    var list = JsonConvert.DeserializeObject<mResultEmisor>(result);

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

        public mResultEmisor sEditarEmisor(mNewEmisor model, string Token)
        {
            try
            {
                var servicio = "api/EditarEmisor?access_token=" + Token;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, "PUT", JsonConvert.SerializeObject(model));

                    var list = JsonConvert.DeserializeObject<mResultEmisor>(result);

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

        public mResultEmisor sCargaCertificado(string rfcEmisor, string certificadoBase64, string keyBase64, string passwordCertificado, string Token)
        {
            try
            {
                var servicio = "api/CargarCertificado?access_token=" + Token + "&rfcEmisor=" + rfcEmisor
                    + "&certificadoBase64=" + certificadoBase64 + "&keyBase64=" + keyBase64 + "&passwordCertificado=" + passwordCertificado;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, "POST");

                    var list = JsonConvert.DeserializeObject<mResultEmisor>(result);

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

        public mResultEmisor sCargaLogotipo(string rfcEmisor, string imagenBase64, string Token)
        {
            try
            {
                var servicio = "api/CargarLogotipo?access_token=" + Token + "&rfcEmisor=" + rfcEmisor + "&imagenBase64=" + imagenBase64;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = System.Text.Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, "POST");

                    var list = JsonConvert.DeserializeObject<mResultEmisor>(result);

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

        public mResultEmisor sEditarLogo(string rfcEmisor, string imagenBase64, string Token)
        {
            try
            {
                var servicio = "api/EditarLogo?access_token=" + Token + "&rfcEmisor=" + rfcEmisor + "&imagenBase64=" + imagenBase64;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, "PUT");

                    var list = JsonConvert.DeserializeObject<mResultEmisor>(result);

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

        public mResultEmisor sCargarCartaConsentimiento(string RFC, string Representante, string certificadoFIEL64, string llaveFIEL64, string passwordFIEL, string Token)
        {
            try
            {
                var servicio = "api/CargarCartaConsentimiento?access_token=" + Token + "&rfcEmisor=" 
                    + RFC + "&representante=" + Representante + "&certificadoFIEL64=" + certificadoFIEL64 + "&llaveFIEL64=" 
                    + llaveFIEL64 + "&passwordFIEL=" + passwordFIEL;
                Uri apiUrl = new Uri(sStatics.rutaAPI_TP + servicio);

                using (var wc = new MyWebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    var result = wc.UploadString(apiUrl, "POST");

                    var list = JsonConvert.DeserializeObject<mResultEmisor>(result);

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