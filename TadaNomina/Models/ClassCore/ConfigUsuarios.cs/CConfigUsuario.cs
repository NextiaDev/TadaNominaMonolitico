using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.ConfigUsuario;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.ConfigUsuarios.cs
{
    public class CConfigUsuario
    {

        public List<MInfoUsuario> GetUsuariosActive(string token)
        {
            var servicio = "/api/Usuarios/GetUsuarios";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<MInfoUsuario> result = new List<MInfoUsuario>();
                try { result = JsonConvert.DeserializeObject<List<MInfoUsuario>>(responsebody); } catch { result = new List<MInfoUsuario>(); }
                return result;
            }
        }

        public List<Cat_Clientes> GetCklientesToSelect(string token)
        {
            var servicio = "/api/Usuarios/GetClienteToAsign";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<Cat_Clientes> result = new List<Cat_Clientes>();
                try { result = JsonConvert.DeserializeObject<List<Cat_Clientes>>(responsebody); } catch { result = new List<Cat_Clientes>(); }
                return result;
            }
        }

        public List<Cat_UnidadNegocio> GetUnidadesNegocioToAsign(string token)
        {
            var servicio = "/api/Usuarios/GetUnidadesNegocioToAsign";
            Uri apiUrl = new Uri(sStatics.servidor + servicio);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string responsebody = response.Content.ReadAsStringAsync().Result;
                List<Cat_UnidadNegocio> result = new List<Cat_UnidadNegocio>();
                try { result = JsonConvert.DeserializeObject<List<Cat_UnidadNegocio>>(responsebody); } catch { result = new List<Cat_UnidadNegocio>(); }
                return result;
            }
        }
    }
}