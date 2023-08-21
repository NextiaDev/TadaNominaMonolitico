using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.ViewModels.RelojChecador;

namespace TadaNomina.Models.ClassCore.RelojChecador
{
    public class cRegistroAsistencias
    {
        public TokenGeoVictoria GetToken(AccesosGVModel ModelGV)
        {
            var datos = new LoginGeoVictoria()
            {
                User = Statics.DesEncriptar(ModelGV.ClaveAPI),
                Password = Statics.DesEncriptar(ModelGV.Secreto)
            };

            var _datos = JsonConvert.SerializeObject(datos);
            string cadenaUrl = Statics.ServidorGeoVictoriaToken + "/api/v1/Login";
            Uri url = new Uri(cadenaUrl);
            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            var result = wc.UploadString(url, _datos);
            var token = JsonConvert.DeserializeObject<TokenGeoVictoria>(result);
            return token;
        }
    }
}