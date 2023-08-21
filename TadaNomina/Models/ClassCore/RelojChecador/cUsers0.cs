using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore.RelojChecador;
using TadaNomina.Models.ViewModels.RelojChecador;
using TadaNomina.Models.DB;
using TadaNomina.Services;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Models.ClassCore.RelojChecador
{
    public class cUsers
    {
        public LibroDeAsistenciaModel AttendanceBook(string token, string StartDate, string EndDate, string UserIds)
        {
            string datos;
            string cadena;
            string result;
            Uri url;
            LibroDeAsistenciaModel r;
            WebClient wClient = new WebClient();
            LibroDeAsistenciaModel consulta = new LibroDeAsistenciaModel
            {
                StartDate = StartDate,
                EndDate = EndDate,
                UserIds = UserIds
            };
            datos = JsonConvert.SerializeObject(consulta);
            cadena = Statics.ServidorGeoVictoriaToken + "/api/v1/AttendanceBook";
            url = new Uri(cadena);
            wClient.Headers["Content-Type"] = "application/json";
            wClient.Headers["Authorization"] = token;
            result = wClient.UploadString(url, datos);
            r = JsonConvert.DeserializeObject<LibroDeAsistenciaModel>(result);
            return r;
        }

        public mResultEdit Add(Empleado um, string token)
        {
            UserModel datos;
            Uri url;
            WebClient wclient = new WebClient();
            string _datos;
            string curl;
            string result;

            datos = new UserModel()
            {
                Identifier = um.Imss,
                Name = um.Nombre,
                LastName = um.ApellidoPaterno + " "+ um.ApellidoMaterno,
                Email = um.CorreoElectronico,
            };

            _datos = JsonConvert.SerializeObject(datos);
            curl = Statics.ServidorGeoVictoriaToken + "/api/v1/User/Add";
            url = new Uri(curl);
            wclient.Headers["Content-type"] = "application/json";
            wclient.Headers["Authorization"] = token;
            result = wclient.UploadString(url, _datos);
            var r = JsonConvert.DeserializeObject<mResultEdit>(result);
            return r;
        }

        public List<RemuneracionesModel> Consolidated(string token, string _StartDate, string _EndDate, List<string> _UserIds, int usuXC)
        {
            int contador = 0;
            List<RemuneracionesModel> x = new List<RemuneracionesModel>();
            for (int item = 0; item < _UserIds.Count(); item++)
            {
                RemuneracionesModel consulta = new RemuneracionesModel
                {
                    IncludeAll = 0,
                    StartDate = _StartDate,
                    EndDate = _EndDate,
                    UserIds = _UserIds[item]
                };
                string datos = JsonConvert.SerializeObject(consulta);
                string cadena = Statics.ServidorGeoVictoriaToken + "/api/v1/Consolidated";
                Uri url = new Uri(cadena);
                WebClient wClient = new WebClient();
                wClient.Headers["Content-Type"] = "application/json";
                wClient.Headers["Authorization"] = token;
                string result = wClient.UploadString(url, datos);
                List<RemuneracionesModel>  r = JsonConvert.DeserializeObject<List<RemuneracionesModel>>(result);
                for (int i = 0; (i < usuXC && i < r.Count()); i++)
                {
                    x.Add(null);
                    x[contador] = r[i];
                    contador++;
                }
                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i].Identifier.Length < 11)
                    {
                        x[i].Identifier = "0" + x[i].Identifier;
                    }
                }
            }
            return x;
        }

        public List<string> lstUsuarios(string token, int usuXC)
        {
            var lst = ListarUsuariosGV(token);
            List<string> usuarios = new List<string>();
            string usuario = "";
            int contador = 0;
            for (var item = 0; item < lst.Count();)
            {
                for (var i = 0; (i < usuXC && item < lst.Count()); i++)
                {
                    if (usuario == "")
                    {
                        usuario = lst[item].Identifier;
                    }
                    else
                    {
                        usuario = usuario + "," + lst[item].Identifier;
                    }
                    item++;
                }
                usuarios.Add("");
                usuarios[contador] = usuario;
                contador++;
                usuario = "";
            }
            
            return usuarios;
        }

        public List<SelectListItem> NombreU(string token)
        {
            var lst = ListarUsuariosGV(token);
            List<SelectListItem> nombres= new List<SelectListItem>();
            int contador = 0;
            string comodin = "";
            foreach(var item in lst)
            {
                if (lst[contador].Identifier.Length < 11)
                {
                    comodin = "0" + item.Identifier;
                }
                else
                {
                    comodin = item.Identifier;
                }
                nombres.Add(new SelectListItem
                {
                    Value = comodin,
                    Text = item.Name + " " + item.LastName
                });
                contador++;
            }
            return nombres;
        }

        public List<SelectListItem> ClaveEmpImss(List<vEmpleados> modelEmp)
        {
            List<SelectListItem> clave = new List<SelectListItem>();
            foreach(var item in modelEmp)
            {
                clave.Add(new SelectListItem
                {
                    Value = item.Imss,
                    Text = item.ClaveEmpleado
                });
            }
            return clave;
        }

        public List<SelectListItem> IdEmplImss(List<vEmpleados> modelEmp)
        {
            List<SelectListItem> idEmpl = new List<SelectListItem>();
            foreach (var item in modelEmp)
            {
                idEmpl.Add(new SelectListItem
                {
                    Value = item.Imss,
                    Text = item.IdEmpleado.ToString()
                });
            }
            return idEmpl;
        }

        public string EliminaIncidenciasChecador(string _token, int _IdPeriodoNomina)
        {
            try
            {
                var servicio = "/api/Incidencias/DeleteAllIncidenciasChecador?IdPeriodoNomina=" + _IdPeriodoNomina;
                Uri apiUrl = new Uri(sStatics.servidor + servicio);

                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + _token;
                    var result = wc.UploadString(apiUrl, "");

                    return result;
                }
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        public List<SelectListItem> IncidenciasGV(List<RemuneracionesModel> RemuModel, string token, int pIdPeriodoNomina, int IdCliente)
        {
            string nToken = "Bearer " + token;
            var empleado = ClaveEmpl(IdCliente);
            var lstIds = IdEmplImss(empleado);
            List<SelectListItem> resultado = new List<SelectListItem>();
            List<SelectListItem> respuesta = new List<SelectListItem>();
            for (int item = 0; item < RemuModel.Count; item++)
            {
                string comodin = "";
                if (RemuModel[item].Absent > 0)
                {
                    foreach (var i in lstIds.Where(m => m.Value == RemuModel[item].Identifier))
                    {
                        comodin = i.Text;
                    }
                    if (comodin != "")
                    {
                        IncidenciasChecadorModel consulta = new IncidenciasChecadorModel
                        {
                            idPeriodoNomina = pIdPeriodoNomina,
                            idEmpleado = int.Parse(comodin),
                            idConcepto = 462,
                            cantidad = RemuModel[item].Absent,
                            banderaChecadores = 1,
                            observaciones = "",
                            folio = "",
                            monto = 0,
                            montoEsquema = 0,
                            cantidadEsquema = 0,
                        };
                        string datos = JsonConvert.SerializeObject(consulta);
                        string cadena = sStatics.relojChecador + "api/Incidencias/newIncidencia";
                        Uri url = new Uri(cadena);
                        WebClient wClient = new WebClient();
                        wClient.Headers["Content-Type"] = "application/json";
                        wClient.Headers["Authorization"] = nToken;
                        string result = wClient.UploadString(url, datos);

                        resultado.Add(new SelectListItem
                        {
                            Value = comodin,
                        }
                        );
                    }
                    else
                    {
                        resultado.Add(new SelectListItem
                        {
                            Value = RemuModel[item].Identifier,
                            Text = "Failure"
                        }
                        );
                    }
                }
            }
            return resultado;
        }

        public List<vEmpleados> ClaveEmpl(int IdCleinte)
        {
            TadaEmpleadosEntities ctx = new TadaEmpleadosEntities();
            List<vEmpleados> empleado = new List<vEmpleados>();
            using (ctx)
            {
                empleado = (from a in ctx.vEmpleados.Where(a => a.IdCliente == IdCleinte && a.IdEstatus == 1) select a).ToList();
            }
            return empleado;
        }

        public List<UserModel> ListarUsuariosGV(string token)
        {
            List<UserModel> GetUsers = new List<UserModel>();

            var client = new RestClient(Statics.ServidorGeoVictoriaToken + "/api/v1/User/List");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", token);

            IRestResponse response = client.Execute(request);

            GetUsers = new JavaScriptSerializer().Deserialize<List<UserModel>>(response.Content);

            return GetUsers;

        }

        public string CheckUsuario(string token, string usuario)
        {
            var lstGV = ListarUsuariosGV(token);
            bool bandera = false;
            string usu = "";

            foreach (var item in lstGV.Where(m => m.Identifier == usuario))
            {
                bandera = true;
            }

            if (bandera == false)
            {
                usu = usuario.Substring(1, 10);
            }
            else
            {
                usu = usuario;
            }
            return usu;
        }
    }
}