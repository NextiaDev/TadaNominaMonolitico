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
using System.Threading;
using RestSharp.Authenticators;
using TadaNomina.Models.ViewModels.Nominas;
using System.Linq.Dynamic.Core.Tokenizer;
using ServiceStack;
using System.Globalization;

namespace TadaNomina.Models.ClassCore.RelojChecador
{
    public class cUsers
    {
        public List<UserModel> ListarUsuariosGV()
        {
            List<Empleados> lstxUN;
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            ClassAccesosGV CAGV = new ClassAccesosGV();
            List<UserModel> GetUsers = new List<UserModel>();

            int? IdCliente = int.Parse(HttpContext.Current.Session["sIdCliente"].ToString());

            var accesos = CAGV.DatosGV((int)IdCliente);

            if(accesos != null)
            {
                string curl = Statics.ServidorGeoVictoriaOauth + "/api";

                RestClient client = new RestClient(curl)
                {
                    Authenticator = OAuth1Authenticator.ForProtectedResource(Statics.DesEncriptar(accesos.ClaveAPI), Statics.DesEncriptar(accesos.Secreto), string.Empty, string.Empty)
                };

                var request = new RestRequest("/User/List", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute(request);

                GetUsers = JsonConvert.DeserializeObject<List<UserModel>>(response.Content);

                //////////////////////////////////////////////////////////////////////////////////////////////////////////
                int IdUnidadNegocio = (int)HttpContext.Current.Session["sIdUnidadNegocio"];

                using (TadaEmpleadosEntities ctx = new TadaEmpleadosEntities())
                {
                    lstxUN = ctx.Empleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1).ToList();
                }
                List<UserModel> remove = new List<UserModel>();
                List<UserModel> remove2 = new List<UserModel>();
                bool bandera = false;
                string valida = null;
                string auxiliar = null;
                foreach (var item in GetUsers)
                {
                    bandera = false;
                    valida = null;
                    auxiliar = null;
                    if (item.Identifier != null)
                    {
                        if (item.Identifier.Length == 10)
                        {
                            auxiliar = "0" + item.Identifier;
                        }
                        else if (HttpContext.Current.Session["sIdCliente"].ToString() == "141")
                        {
                            bandera = true;
                            auxiliar = Quitar0(item.Identifier);
                        }
                        else
                        {
                            auxiliar = item.Identifier;
                        }
                        if (bandera)
                        {
                            valida = lstxUN.Where(x => x.ClaveEmpleado == auxiliar).Select(x => x.ClaveEmpleado).FirstOrDefault();
                        }
                        else
                        {
                            valida = lstxUN.Where(x => x.Imss == auxiliar).Select(x => x.Imss).FirstOrDefault();
                        }

                        if (valida == null)
                        {
                            remove.Add(item);
                        }
                    }
                    else
                    {
                        remove2.Add(item);
                    }
                }

                foreach (var item in remove)
                {
                    GetUsers.Remove(item);
                }

                foreach (var item in remove2)
                {
                    if (item.Identifier == null)
                        GetUsers.Remove(item);
                }
            }
            return GetUsers;
        }

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
            wClient.Encoding = System.Text.Encoding.UTF8;
            wClient.Headers["Content-Type"] = "application/json";
            wClient.Headers["Authorization"] = token;
            result = wClient.UploadString(url, datos);
            r = JsonConvert.DeserializeObject<LibroDeAsistenciaModel>(result);

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
                wClient.Encoding = System.Text.Encoding.UTF8;
                wClient.Headers["Content-Type"] = "application/json";
                wClient.Headers["Authorization"] = token;
                string result = wClient.UploadString(url, datos);
                List<RemuneracionesModel> r = JsonConvert.DeserializeObject<List<RemuneracionesModel>>(result);
                for (int i = 0; (i < usuXC && i < r.Count()); i++)
                {
                    x.Add(null);
                    x[contador] = r[i];
                    contador++;
                }
                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i].Identifier.Length == 10)
                    {
                        x[i].Identifier = "0" + x[i].Identifier;
                    }
                }
            }
            return x;
        }

        public List<SelectListItem> AtteBookXN(string token, string StartDate, string EndDate, List<string> UsersIds)
        {
            List<SelectListItem> z = new List<SelectListItem>();
            LibroDeAsistenciaModel r;
            for (int item = 0; item < UsersIds.Count; item++)
            {
                r = AttendanceBook(token,StartDate,EndDate, UsersIds[item]);
                Thread.Sleep(250);
                for (int i = 0; i < r.Users.Count; i++)
                {
                    if (r.Users[i].Enabled == 1)
                    {
                        for (int y = 0; y < r.Users[i].PlannedInterval.Count; y++)
                        {
                            if (r.Users[i].Identifier.Length < 11)
                            {
                                r.Users[i].Identifier = "0" + r.Users[i].Identifier;
                            }
                            if (r.Users[i].PlannedInterval[y].Delay != "00:00")
                            {
                                z.Add(new SelectListItem
                                {
                                    Text = r.Users[i].Identifier,
                                    Value = "1619"
                                });
                            }
                            if (r.Users[i].PlannedInterval[y].TimeOffs.Count > 0)
                            {
                                for (int w = 0; w < r.Users[i].PlannedInterval[y].TimeOffs.Count; w++)
                                {
                                    z.Add(new SelectListItem
                                    {
                                        Text = r.Users[i].Identifier,
                                        Value = r.Users[i].PlannedInterval[y].TimeOffs[w].TimeOffTypeId
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return z;
        }

        public List<IncidenciasModel> LstIncidencias(List<RemuneracionesModel> RemuModel, List<SelectListItem> AtteBook, int IdCliente)
        {
            List<IncidenciasModel> lst = new List<IncidenciasModel>();
            List<ConceptosChecadorModel> lstConceptos = new List<ConceptosChecadorModel>();
            int falta;

            /////Conceptos
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                //Concepto de falta
                falta = (from concepto in ctx.Cat_ConceptosNomina
                         where concepto.ClaveConcepto == "500" && concepto.IdCliente == IdCliente
                         select concepto.IdConcepto).FirstOrDefault();

                //Lista de los conceptos casados de GeoVictoria con TADA
                lstConceptos = (from conceptoC in ctx.Cat_ConceptosChecador
                                join conceptoN in ctx.Cat_ConceptosNomina on conceptoC.IdConceptoNomina equals conceptoN.IdConcepto
                                where conceptoC.IdEstatus == 1 && conceptoN.IdCliente == IdCliente
                                select new ConceptosChecadorModel
                                {
                                    IdConceptoChecador = conceptoC.IdConceptoChecador,
                                    IdConceptoNomina = conceptoC.IdConceptoNomina,
                                    IdConceptoGV = conceptoC.IdConceptoGV,
                                    DescripcionGV = conceptoC.DescripcionGV,
                                    Pagable = conceptoC.Pagable
                                }).ToList();
            }

            //////Faltas del checador
            for (int item = 0; item < RemuModel.Count; item++)
            {
                if (RemuModel[item].Absent > 0)
                {
                    lst.Add(new IncidenciasModel
                    {
                        Identifier = RemuModel[item].Identifier, //Usuario
                        Concepto = falta,                        //Tipo de Incidencia
                        Cantidad = RemuModel[item].Absent
                    });
                }
            }

            //////Incidencias Geovictoria
            for (int item = 0; item < AtteBook.Count; item++)
            {
                foreach (var i in lstConceptos.Where(x => x.IdConceptoGV == AtteBook[item].Value))
                {
                    //Busca si ya existe una incidencia especifica de un usuario
                    var lista = lst.Where(x => x.Identifier == AtteBook[item].Text && x.Concepto == i.IdConceptoNomina).FirstOrDefault();

                    //si existe suma uno
                    if (lista != null)
                    {
                        lista.Cantidad += 1;
                    }

                    //si no existe crea la incidencia para ese usuario
                    else
                    {
                        lst.Add(new IncidenciasModel
                        {
                            Identifier = AtteBook[item].Text,
                            Concepto = i.IdConceptoNomina,
                            Cantidad = 1
                        });
                    }

                    //Si no es pagable el concepto de incidencia GeoVictoria genera una falta, se elimina esa falta
                    if (i.Pagable == false)
                    {
                        //Busca el usuario con la falta generada por la incidencia no pagable
                        var lista2 = lst.Where(x => x.Identifier == AtteBook[item].Text && x.Concepto == falta).FirstOrDefault();
                        if (lista2 != null)
                        {
                            //Si encuentra resta 1 falta al total
                            if (lista2.Cantidad > 1)
                            {
                                lista2.Cantidad -= 1;
                            }

                            //Si tiene una sola falta elimina el registro
                            else
                            {
                                lst.Remove(lista2);
                            }
                        }
                    }
                }
            }
            for (int item = 0; item < AtteBook.Count; item++)
            {
                //Figura del 7mo día para BiciMex
                if (IdCliente == 30)
                {
                    foreach (var j in lst)
                    {
                        //Busca a un usuario que tenga una incidencia de falta
                        var lista3 = lst.Where(x => x.Concepto == falta && x.Identifier == AtteBook[item].Text).FirstOrDefault();
                        if (lista3 != null)
                        {
                            //Si tiene 6 falta equivale a 7
                            if (lista3.Cantidad == 6)
                            {
                                lista3.Cantidad = 7;
                            }
                        }
                    }
                }
            }

            return lst;
        }

        /// <summary>
        /// Metodo para generar premios de puntualidad en un periodo de nomina
        /// </summary>
        /// <param name="lstIncidencia">Lista de faltas</param>
        /// <param name="pIdPeriodoNomina">Periodo de nomina</param>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="idUsuario">Usuario quien captura la incidencia</param>
        /// <param name="usuarios">Lista de empleados</param>
        /// <returns>Lista de incidencias generadas</returns>
        public List<IncidenciasModel> IncidenciasBonoPuntualidad(List<IncidenciasModel> lstIncidencia, int pIdPeriodoNomina, int IdCliente, int idUsuario, List<string> usuarios)
        {
            List<IncidenciasModel> lstbono = new List<IncidenciasModel>();
            try
            {
                List<SelectListItem> resultado = new List<SelectListItem>();
                var empleado = ClaveEmpl((int)IdCliente);
                int falta;
                var lstIds = IdEmplImss(empleado);
                int? idBonoPuntualidad = null;
                List<int> lstEmpleados = new List<int>();


                using (TadaNominaEntities ctx = new TadaNominaEntities())
                {
                    falta = (from concepto in ctx.Cat_ConceptosNomina
                             where concepto.ClaveConcepto == "500" && concepto.IdCliente == IdCliente
                             select concepto.IdConcepto).FirstOrDefault();

                    idBonoPuntualidad = ctx.Cat_ConceptosNomina.Where(x => x.IdConceptoSistema == 3346 && x.IdCliente == IdCliente && x.IdEstatus == 1).Select(x => x.IdConcepto).FirstOrDefault();
                }
                var lstEmpFalta = lstIncidencia.Where(x => x.Concepto == falta).Select(x => x.Identifier);
                for(var i = 0; i < usuarios.Count; i++)
                {
                    foreach (var item in usuarios[i].Split(',').Where(x=>!lstEmpFalta.Contains(x)))
                    {
                        lstbono.Add(new IncidenciasModel
                        {
                            Cantidad = 1,
                            Identifier = item,
                            Concepto = (int)idBonoPuntualidad,
                        });
                    }
                }

                for (int item = 0; item < lstbono.Count; item++)
                {
                    string ClaveEmpleado = "";

                    //Cambia Clave imss a Calve Empleado
                    foreach (var i in lstIds.Where(m => m.Value == lstbono[item].Identifier))
                    {
                        ClaveEmpleado = i.Text;
                    }

                    //Inserta incidencias
                    if (ClaveEmpleado != "" && idBonoPuntualidad != null)
                    {
                        decimal monto = CalculaMontoBonoPuntualidad(int.Parse(ClaveEmpleado), empleado);
                        
                        if(monto != 0 && !lstEmpleados.Contains(int.Parse(ClaveEmpleado)))
                        {
                            ClassIncidencias ci = new ClassIncidencias();

                            //Datos a insertar
                            ModelIncidencias consulta = new ModelIncidencias
                            {
                                IdPeriodoNomina = pIdPeriodoNomina,
                                IdEmpleado = int.Parse(ClaveEmpleado),
                                IdConcepto = (int)idBonoPuntualidad,
                                Cantidad = 1,
                                BanderaChecadores = 1,
                                Observaciones = "",
                                Folio = "",
                                Monto = monto,
                                MontoEsquema = 0,
                                CantidadEsq = 0,
                            };

                            ci.NewIncindencia(consulta, idUsuario);

                            lstEmpleados.Add(consulta.IdEmpleado);

                            resultado.Add(new SelectListItem
                            {
                                Value = ClaveEmpleado + " | " + lstbono[item].Concepto,
                                Text = "Successfull"
                            });
                        }
                    }
                    else
                    {
                        resultado.Add(new SelectListItem
                        {
                            Value = lstbono[item].Identifier + " | " + lstbono[item].Concepto,
                            Text = "Failure"
                        }
                        );
                    }
                }

                return lstbono;
            }
            catch (Exception)
            {

                return lstbono;
            }
        }

        public decimal CalculaMontoBonoPuntualidad(int ClaveEmpleado, List<vEmpleados> lstEmp)
        {
            decimal res = 0m;
            try
            {
                foreach (var item in lstEmp.Where(x => x.IdEmpleado == ClaveEmpleado))
                {
                    if (item.SD != null && item.SD != 0)
                    {
                        res = 7.0m * 0.1m * (decimal)item.SD;
                    }
                    else if (item.SDI != null && item.SDI != null)
                    {
                        res = 7.0m * 0.1m * (decimal)item.SDI;
                    }
                }
                return res;
            }
            catch
            {
                return 0;
            }
        }

        public List<SelectListItem> IncidenciasGV(List<IncidenciasModel> lstIncidencias, string token, int pIdPeriodoNomina, int IdCliente, int idUsuario)
        {
            string nToken = "Bearer " + token;
            var empleado = ClaveEmpl(IdCliente);
            var lstIds = IdEmplImss(empleado);
            List<SelectListItem> resultado = new List<SelectListItem>();
            List<SelectListItem> respuesta = new List<SelectListItem>();

            //Procesa Incidencias
            for (int item = 0; item < lstIncidencias.Count; item++)
            {
                string ClaveEmpleado = "";

                //Cambia Clave imss a Calve Empleado
                foreach (var i in lstIds.Where(m => m.Value == lstIncidencias[item].Identifier))
                {
                    ClaveEmpleado = i.Text;
                }

                //Inserta incidencias
                if (ClaveEmpleado != "")
                {
                    ClassIncidencias ci = new ClassIncidencias();

                    //Datos a insertar
                    ModelIncidencias consulta = new ModelIncidencias
                    {
                        IdPeriodoNomina = pIdPeriodoNomina,
                        IdEmpleado = int.Parse(ClaveEmpleado),
                        IdConcepto = lstIncidencias[item].Concepto,
                        Cantidad = (decimal)lstIncidencias[item].Cantidad,
                        BanderaChecadores = 1,
                        Observaciones = "",
                        Folio = "",
                        Monto = 0,
                        MontoEsquema = 0,
                        CantidadEsq = 0,
                    };

                    ci.NewIncindencia(consulta, idUsuario);

                    resultado.Add(new SelectListItem
                    {
                        Value = ClaveEmpleado + " | " + lstIncidencias[item].Concepto,
                        Text = "Successfull"
                    });
                }
                else
                {
                    resultado.Add(new SelectListItem
                    {
                        Value = lstIncidencias[item].Identifier + " | " + lstIncidencias[item].Concepto,
                        Text = "Failure"
                    }
                    );
                }
            }
            return resultado;
        }

        public List<string> LstUsuarios(int usuXC)
        {
            var lst = ListarUsuariosGV();
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

        public List<SelectListItem> NombreU()
        {
            var lst = ListarUsuariosGV();
            List<SelectListItem> nombres = new List<SelectListItem>();
            string comodin;
            foreach (var item in lst)
            {
                if (item.Enabled == 1)
                {
                    if (item.Identifier.Length == 10)
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
                }
            }
            return nombres;
        }

        public List<SelectListItem> NombreUxC()
        {
            int IdUnidadNegocio = (int)HttpContext.Current.Session["sIdUnidadNegocio"];
            List<SelectListItem> nombres = new List<SelectListItem>();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                nombres = ctx.Empleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1).Select(x => new SelectListItem
                {
                    Value = x.ClaveEmpleado,
                    Text = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre
                }).ToList();
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

        public string ClaveEmpleado(string imss, int IdCliente)
        {
            string empleado = "";
            using (TadaEmpleadosEntities ctx = new TadaEmpleadosEntities())
            {
                empleado = ctx.vEmpleados.Where(x => x.Imss == imss && x.IdCliente == IdCliente && x.IdEstatus == 1).Select(x => x.ClaveEmpleado).FirstOrDefault();
            }
            return empleado;
        }

        public string CheckUsuario(string usuario)
        {
            var lstGV = ListarUsuariosGV();
            bool bandera = false;
            string usu;

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

        public int UsuariosXConsulta(DateTime FechaFinChecador, DateTime FechaInicioChecador)
        {
            TimeSpan difFechas = FechaFinChecador - FechaInicioChecador;
            int dias = difFechas.Days;
            int usuXCon = 0;

            if (dias <= 7)
            {
                usuXCon = 199;
            }
            else if (dias <= 10)
            {
                usuXCon = 149;
            }
            else if (dias <= 15)
            {
                usuXCon = 99;
            }
            else if (dias <= 31)
            {
                usuXCon = 48;
            }

            return usuXCon;
        }

        public string FechaChecador(DateTime FechaChecador, char Indicador)
        {
            if (Indicador == 'I')
            {
                string d1 = FechaChecador.ToString("yyyy-MM-dd");
                string da1 = d1.Replace("-", "");
                string dat1 = da1 + "000000";
                return dat1;
            }
            else
            {
                string d2 = FechaChecador.ToString("yyyy-MM-dd");
                string da2 = d2.Replace("-", "");
                string dat2 = da2 + "235959";
                return dat2;
            }
        }

        public mResultEdit Enable(Empleado um, AccesosGVModel accesos)
        {
            try
            {
                var datos = new UserModel()
                {
                    Identifier = um.Imss,
                    Email = um.CorreoElectronico
                };

                var _datos = JsonConvert.SerializeObject(datos);

                //string curl = Statics.ServidorGeoVictoriaOauthTest + "/api";
                string curl = Statics.ServidorGeoVictoriaOauth + "/api";

                RestClient client = new RestClient(curl)
                {
                    Authenticator = OAuth1Authenticator.ForProtectedResource(Statics.DesEncriptar(accesos.ClaveAPI), Statics.DesEncriptar(accesos.Secreto), string.Empty, string.Empty)
                };

                var request = new RestRequest("/User/Enable", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(_datos);

                var response = client.Execute(request);
                var responseDesObj = JsonConvert.DeserializeObject<mResultEdit>(response.Content);

                return responseDesObj;
            }
            catch
            {
                mResultEdit mre = new mResultEdit();
                return mre;
            }
        }

        public string Completa0(string Clave)
        {
            string r = Clave;
            if (Clave.Length < 8)
            {
                for (int i = Clave.Length; i <= 8; i++)
                {
                    r = 0 + r;
                }
            }
            return r;
        }

        public string Quitar0(string Clave)
        {
            string r;
            if (Clave.Substring(0, 1) == "0")
            {
                r = Quitar0(Clave.Substring(1, Clave.Length - 1));
            }
            else
            {
                r = Clave;
            }
            return r;
        }

        public mResultEdit Add(Empleado um, AccesosGVModel accesos)
        {
            try
            {
                var datos = new UserModel()
                {
                    Identifier = um.Imss,
                    Name = um.Nombre,
                    LastName = um.ApellidoMaterno != null ? um.ApellidoPaterno + " " + um.ApellidoMaterno : um.ApellidoPaterno,
                    Email = um.CorreoElectronico
                };

                if (HttpContext.Current.Session["sIdCliente"].ToString() == "141")
                {
                    if (um.ClaveEmpleado.Substring(0, 1) == "H")
                    {
                        datos.Identifier = Completa0(um.ClaveEmpleado.Substring(1, um.ClaveEmpleado.Length - 1));
                        datos.Custom1 = "H";
                    }
                }

                var _datos = JsonConvert.SerializeObject(datos);

                string curl = Statics.ServidorGeoVictoriaOauth + "/api";

                RestClient client = new RestClient(curl)
                {
                    Authenticator = OAuth1Authenticator.ForProtectedResource(Statics.DesEncriptar(accesos.ClaveAPI), Statics.DesEncriptar(accesos.Secreto), string.Empty, string.Empty)
                };

                var request = new RestRequest("/User/Add", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(_datos);

                var response = client.Execute(request);
                var responseDesObj = JsonConvert.DeserializeObject<mResultEdit>(response.Content);

                return responseDesObj;
            }
            catch(Exception ex)
            {
                mResultEdit mre = new mResultEdit();
                
                return mre;
            }
        }

        public mResultEdit Edit(Empleado um, AccesosGVModel accesos)
        {
            try
            {
                var datos = new UserModel()
                {
                    Identifier = um.Imss,
                    Name = um.Nombre,
                    LastName = um.ApellidoMaterno != null ? um.ApellidoPaterno + " " + um.ApellidoMaterno : um.ApellidoPaterno,
                    Email = um.CorreoElectronico
                };

                if (HttpContext.Current.Session["sIdCliente"].ToString() == "141")
                {
                    if (um.ClaveEmpleado.Substring(0, 1) == "H")
                    {
                        datos.Identifier = Completa0(um.ClaveEmpleado.Substring(1, um.ClaveEmpleado.Length - 1));
                        datos.Custom1 = "H";
                    }
                }

                var _datos = JsonConvert.SerializeObject(datos);

                string curl = Statics.ServidorGeoVictoriaOauth + "/api";

                RestClient client = new RestClient(curl)
                {
                    Authenticator = OAuth1Authenticator.ForProtectedResource(Statics.DesEncriptar(accesos.ClaveAPI), Statics.DesEncriptar(accesos.Secreto), string.Empty, string.Empty)
                };

                var request = new RestRequest("/User/Edit", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(_datos);

                var response = client.Execute(request);
                var responseDesObj = JsonConvert.DeserializeObject<mResultEdit>(response.Content);

                return responseDesObj;
            }
            catch
            {
                mResultEdit mre = new mResultEdit();
                return mre;
            }
        }

        public mResultEdit Disable(Empleado Empleado, AccesosGVModel accesos)
        {
            try
            {
                var datos = new UserModel()
                {
                    Identifier = Empleado.Imss,
                    Email = Empleado.CorreoElectronico
                };
                if (HttpContext.Current.Session["sIdCliente"].ToString() == "141")
                {
                    if (Empleado.ClaveEmpleado.Substring(0, 1) == "H")
                    {
                        datos.Identifier = Completa0(Empleado.ClaveEmpleado.Substring(1, Empleado.ClaveEmpleado.Length - 1));
                    }
                }

                var _datos = JsonConvert.SerializeObject(datos);

                //string curl = Statics.ServidorGeoVictoriaOauthTest + "/api";
                string curl = Statics.ServidorGeoVictoriaOauth + "/api";

                RestClient client = new RestClient(curl)
                {
                    Authenticator = OAuth1Authenticator.ForProtectedResource(Statics.DesEncriptar(accesos.ClaveAPI), Statics.DesEncriptar(accesos.Secreto), string.Empty, string.Empty)
                };

                var request = new RestRequest("/User/Disable", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(_datos);

                var response = client.Execute(request);
                var responseDesObj = JsonConvert.DeserializeObject<mResultEdit>(response.Content);

                return responseDesObj;
            }
            catch
            {
                mResultEdit mre = new mResultEdit();
                return mre;
            }
        }

        public List<IncidenciasModel> GetIncidenciasHorasExtra(List<RemuneracionesModel> listremu, int IdCliente)
        {
            var result = new List<IncidenciasModel>();

            var conceptohoradoble = new Cat_ConceptosNomina();
            var conceptohoratriple = new Cat_ConceptosNomina();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var conceptohoradoblecliente = ctx.Cat_ConceptosNomina.FirstOrDefault(p => p.IdConceptoSistema == 280 && p.IdCliente == IdCliente && p.IdEstatus == 1);
                var conceptohoratriplecliente = ctx.Cat_ConceptosNomina.FirstOrDefault(p => p.IdConceptoSistema == 1547 && p.IdCliente == IdCliente && p.IdEstatus == 1);
                conceptohoradoble = conceptohoradoblecliente != null ? conceptohoradoblecliente : ctx.Cat_ConceptosNomina.FirstOrDefault(p => p.IdConcepto == 280);
                conceptohoratriple = conceptohoratriplecliente != null ? conceptohoratriplecliente : ctx.Cat_ConceptosNomina.FirstOrDefault(p => p.IdConcepto == 1547);
            }
            foreach (var item in listremu)
            {
                var horasextra = int.Parse(item.TotalAuthorizedExtraTime.Substring(0, 2));
                if (horasextra > 0)
                {
                    if (horasextra > 9)
                    {
                        var listadoble = new List<IncidenciasModel>();
                        var model1 = new IncidenciasModel();
                        model1.Identifier = item.Identifier;
                        model1.Concepto = conceptohoradoble.IdConcepto;
                        model1.Cantidad = 9;
                        listadoble.Add(model1);
                        var model2 = new IncidenciasModel();
                        model2.Identifier = item.Identifier;
                        model2.Concepto = conceptohoratriple.IdConcepto;
                        model2.Cantidad = horasextra - 9;
                        listadoble.Add(model2);
                        result.AddRange(listadoble);
                    }
                    else
                    {
                        var model = new IncidenciasModel();
                        model.Identifier = item.Identifier;
                        model.Concepto = conceptohoradoble.IdConcepto;
                        model.Cantidad = horasextra;
                        result.Add(model);
                    }
                }
            }
            return result;
        }

        public List<IncidenciasModel> GetHrsNoTrabajadas(List<RemuneracionesModel> lstremu, int IdCliente)
        {
            List<IncidenciasModel> lst = new List<IncidenciasModel>();
            string conConcepto = null;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                conConcepto = ctx.Cat_ConceptosNomina.Where(x=> x.IdCliente == IdCliente && x.IdEstatus == 1 && x.IdConcepto == 2912).Select(x=>x.CalculoDiasHoras).FirstOrDefault();
            }
            foreach(var item in lstremu)
            {
                int hrs = int.Parse(item.NonWorkedHours.Substring(0, 2));
                int minE = int.Parse(item.NonWorkedHours.Substring(3, 2));
                double r = (minE > 0) ? (minE / 60.00) : 0.00;
                double minD = Math.Round(r, 2);
                double cantidad = hrs + minD;
                if (hrs > 0)
                {
                    var model1 = new IncidenciasModel
                    {
                        Identifier = item.Identifier,
                        Concepto = 2912,
                        Cantidad = conConcepto == "Horas" ? cantidad : (cantidad / 8)
                    };
                    lst.Add(model1);
                }
            };
            return lst;
        }

        /// <summary>
        /// Metodo para obtener las incidencias de descansos trabajados y días festivos trabajados en un periodo de nomina
        /// </summary>
        /// <param name="Token">Token GeoVictoria</param>
        /// <param name="StartDate">Fecha de inicio del periodo</param>
        /// <param name="EndDate">Fecga de fin del periodo</param>
        /// <param name="UserIds">Empleados</param>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="IdUnidadN">Unidad de negocio</param>
        /// <returns>Lista de incidencias</returns>
        public List<IncidenciasModel> GetHolidaysDescansosTrabajados(string Token, string StartDate, string EndDate, List<string> UserIds, int IdCliente, int IdUnidadN)
        {
            List<IncidenciasModel> lst = new List<IncidenciasModel>();
            //Variable de concepto de incidencia
            int holyConcepto = 0;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {

                holyConcepto = ctx.Cat_ConceptosNomina.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1 && x.IdConceptoSistema == 3129).Select(x => x.IdConcepto).FirstOrDefault();
                //Cambia concepto dependiendo de la unidad de negocio
                if(IdUnidadN == 278)
                {
                    holyConcepto = 2852;
                }
                else if (IdUnidadN == 299)
                {
                    holyConcepto = 2890;
                }
                else
                {
                    holyConcepto = 2566;
                }
            }
            for (int i = 0; i < UserIds.Count; i++)
            {
                //Incidencias de los colaboradores en el periodo de tiempo
                LibroDeAsistenciaModel LstIncidencias = AttendanceBook(Token, StartDate, EndDate, UserIds[i]);
                Thread.Sleep(250);
                foreach(var user in LstIncidencias.Users)
                {
                    if (user.Enabled == 1)
                    {
                        for (int y = 0; y < user.PlannedInterval.Count; y++)
                        {
                            //Se valida que el día sea festivo y tenga registros para aplicar la incidencia
                            if (user.PlannedInterval[y].Holiday == "True" && user.PlannedInterval[y].Punches.Count > 0)
                            {
                                //Lista para guardar todos los registros del colaborador
                                List<DateTime> tiempos = new List<DateTime>();
                                foreach (var punches in user.PlannedInterval[y].Punches)
                                {
                                    var yy = int.Parse(punches.Date.Substring(0, 4));
                                    var mm = int.Parse(punches.Date.Substring(4, 2));
                                    var dd = int.Parse(punches.Date.Substring(6, 2));
                                    var h = int.Parse(punches.Date.Substring(8, 2));
                                    var m = int.Parse(punches.Date.Substring(10, 2));
                                    var s = int.Parse(punches.Date.Substring(12, 2));
                                    var t = new DateTime(yy, mm, dd, h, m, s);
                                    tiempos.Add(t);
                                }
                                //Aplica si tiene mas de un registro y tener datos con que trabajar
                                if (tiempos.Count > 1)
                                {
                                    var entrada = tiempos.OrderBy(x => x).FirstOrDefault();
                                    var salida = tiempos.OrderByDescending(x => x).FirstOrDefault();
                                    var whrs = salida - entrada;

                                    int minE = whrs.Minutes;
                                    double r = (minE > 0) ? (minE / 60.00) : 0.00;
                                    double minD = Math.Round(r, 2);
                                    double cantidad = whrs.Hours + minD;
                                    //Agrega incidencias
                                    lst.Add(new IncidenciasModel
                                    {
                                        Identifier = user.Identifier,
                                        Concepto = holyConcepto,
                                        Cantidad = cantidad
                                    });
                                }
                            }
                            //Se valida si trabajó en sus descansos programados
                            if (user.PlannedInterval[y].Shifts[0].Type == "NotWorking" && user.PlannedInterval[y].Punches.Count > 0)
                            {
                                //Lista para guardar todos los registros del colaborador
                                List<DateTime> tiempos = new List<DateTime>();
                                foreach (var punches in user.PlannedInterval[y].Punches)
                                {
                                    var yy = int.Parse(punches.Date.Substring(0, 4));
                                    var mm = int.Parse(punches.Date.Substring(4, 2));
                                    var dd = int.Parse(punches.Date.Substring(6, 2));
                                    var h = int.Parse(punches.Date.Substring(8, 2));
                                    var m = int.Parse(punches.Date.Substring(10, 2));
                                    var s = int.Parse(punches.Date.Substring(12, 2));
                                    var t = new DateTime(yy, mm, dd, h, m, s);
                                    tiempos.Add(t);
                                }
                                //Aplica si tiene mas de un registro y tener datos con que trabajar
                                if (tiempos.Count > 1)
                                {
                                    var entrada = tiempos.OrderBy(x => x).FirstOrDefault();
                                    var salida = tiempos.OrderByDescending(x => x).FirstOrDefault();
                                    var whrs = salida - entrada;

                                    int minE = whrs.Minutes;
                                    double r = (minE > 0) ? (minE / 60.00) : 0.00;
                                    double minD = Math.Round(r, 2);
                                    double cantidad = whrs.Hours + minD;
                                    //Agrega incidencias
                                    lst.Add(new IncidenciasModel
                                    {
                                        Identifier = user.Identifier,
                                        Concepto = holyConcepto,
                                        Cantidad = cantidad
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return lst;
        }

        /// <summary>
        /// Metodo para obtener las incidencias de prima dominical por un rango de fechas
        /// </summary>
        /// <param name="Token">token GeoVictoria</param>
        /// <param name="StartDate">Fecha Inicio Periodo</param>
        /// <param name="EndDate">Fecha fin Periodo</param>
        /// <param name="UserIds">Lista de identificadores de los colobordores(GeoVictoria)</param>
        /// <param name="IdCliente">Identificador del cliente (variable de sesion)</param>
        /// <returns></returns>
        public List<IncidenciasModel> GetPrimaDominicalxHora(string Token, string StartDate, string EndDate, List<string> UserIds, int IdCliente)
        {
            List<IncidenciasModel> result = new List<IncidenciasModel>();
            var conceptoPrimaDominical = new Cat_ConceptosNomina();
            try
            {
                using (TadaNominaEntities ctx = new TadaNominaEntities())
                {
                    //Se toma el valor de IdConceptoSistema = 274 debido a que es un concepto de refiere a la prima dominical
                    conceptoPrimaDominical = ctx.Cat_ConceptosNomina.Where(p => p.IdCliente == IdCliente && p.IdEstatus == 1 && p.IdConceptoSistema == 274).FirstOrDefault();
                }
                if(conceptoPrimaDominical != null)
                {
                    for(int i = 0; i < UserIds.Count; i++)
                    {
                        //Se obtiene los registros de los colaboradores de los checadores de GeoVictoria
                        LibroDeAsistenciaModel LstIncidencias = AttendanceBook(Token, StartDate, EndDate, UserIds[i]);
                        result = GetIncidenciasPrimaDominical(LstIncidencias, conceptoPrimaDominical.IdConcepto);
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// Metodo para tranformar los registros de GeoVictoria a incidencias de nomina
        /// </summary>
        /// <param name="listageovic">informacion obteniada de GeoVictoria</param>
        /// <param name="IdConceptoPrimaDominical">IdConcepto prima dominical</param>
        /// <returns></returns>
        public List<IncidenciasModel> GetIncidenciasPrimaDominical(LibroDeAsistenciaModel listageovic, int IdConceptoPrimaDominical)
        {
            var result = new List<IncidenciasModel>();
            if(listageovic.Users.Count == 0) return result;
            foreach(var item in  listageovic.Users)
            {
                if (item.Enabled == 1)
                {
                    var id = item.Identifier;
                    var listaplanned = item.PlannedInterval;
                    // se filtran todos los registros que sean domingos validando el campo "Date" dentro del listado de "PlannedInterval" se espera que la fecha llegue en formato "yyyyMMddHHmmSS"
                    var domingos = listaplanned.Where(p => DateTime.ParseExact(p.Date.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture).DayOfWeek.ToString() == "Sunday" && p.Punches.Count > 0).ToList();
                    if (domingos.Count > 0) 
                    {
                        var listadecimales = GetDecimalesHoras(domingos);
                        var totalhoras = listadecimales.Sum(p => Convert.ToDouble(p));
                        result.Add(new IncidenciasModel
                        {
                            Identifier = item.Identifier,
                            Concepto = IdConceptoPrimaDominical,
                            Cantidad = totalhoras
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Metodo para obtener las horas trabajadas totales
        /// </summary>
        /// <param name="listainterval">listado de registros realizados en un dia domingo</param>
        /// <returns></returns>
        public List<decimal> GetDecimalesHoras(List<LAPlannedIntervalModel> listainterval)
        {
            var result = new List<decimal>();
            if(listainterval.Count == 0) return result;
            foreach (var item in listainterval)
            {
                var listaregistros = item.Punches;
                var fechaInicio = DateTime.ParseExact(listaregistros[0].Date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var fechaFin = DateTime.ParseExact(listaregistros[listaregistros.Count - 1].Date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var resta = fechaFin - fechaInicio;
                int minE = resta.Minutes;
                decimal r = (minE > 0) ? (minE / 60.00M) : 0.00M;
                decimal minD = Math.Round(r, 2);
                decimal cantidad = resta.Hours + minD;
                result.Add(cantidad);
            }
            return result;
        }
    }
}