using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Models.ViewModels.Reportes;
using TadaNomina.Models.ViewModels.RelojChecador;
using TadaNomina.Models.ClassCore.RelojChecador;

namespace TadaNomina.Controllers.Nomina
{
    public class IncidenciasController : BaseController
    {
        public ActionResult SelecionaPeriodo()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominas(IdUnidadNegocio);

                return View(periodos);
            }
        }

        //GET: Incidencias
        public ActionResult Index(int pIdPeriodoNomina, int? MostrarTodas)
        {
            ViewBag.pIdPeriodoNomina = pIdPeriodoNomina;
            ClassPeriodoNomina cPeriodos = new ClassPeriodoNomina();
            ClassIncidencias cIncidencias = new ClassIncidencias();

            PeriodoNomina periodo = cPeriodos.GetPeriodo(pIdPeriodoNomina);

            if (periodo.TipoNomina == "Nomina")
            {
                ViewBag.TipoNomina = "OK";
            }
            else
            {
                ViewBag.TipoNomina = "ERROR";
            }

            if (Session["FlagFechasRelojChecador"]== null)
            {
                Session["FlagFechasRelojChecador"] = "OK"; 
            }
            
            string flagChecador = Session["FlagFechasRelojChecador"].ToString();
            if (flagChecador == "OK")
            {
                ViewBag.FlagRelojChecador = flagChecador;
            }
            else
            {
                ViewBag.FlagRelojChecador = flagChecador;
                Session["FlagFechasRelojChecador"] = "OK";
            }

            string token = Session["sToken"].ToString();
            List<PeriodoNomina> lperiodos = cPeriodos.GetPeriodoNominas(pIdPeriodoNomina);
            List<ModelIncidencias> lincidencias = new List<ModelIncidencias>();
            try { lincidencias = cIncidencias.GetModelIncidencias(pIdPeriodoNomina, token).OrderByDescending(x => x.IdIncidencia).ToList(); } catch { }
            
            ViewBag.Bandera = false;
            if (lincidencias.Count > 500 )
            {
                if (MostrarTodas == null)
                {
                    ViewBag.Bandera = true;
                    lincidencias = lincidencias.Take(500).ToList();
                }                
            }            

            ViewBag.SeleccionarPeriodo = lperiodos;

            return View(lincidencias);
        }

        // GET: Incidencias/Details/5
        public ActionResult Details(int id)
        {
            string token = Session["sToken"].ToString();
            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias incidencia = cincidencias.GetModelIncidencia(id, token);

            return PartialView(incidencia);
        }

        // GET: Incidencias/Create
        public ActionResult Create(int pIdPeriodoNomina)
        {
            ViewData["IdPeriodoNomina"] = pIdPeriodoNomina;
            ViewBag.Periodo = pIdPeriodoNomina;
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
            return View(modelo);
        }

       

        // POST: Incidencias/Create
        [HttpPost]
        public ActionResult Create(ModelIncidencias collection)
        {
            ViewBag.Periodo = collection.IdPeriodoNomina;
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];

            try
            {
                if (ModelState.IsValid)
                {
                    ClassIncidencias cins = new ClassIncidencias();
                    int IdUsuario = (int)Session["sIdUsuario"];
                    cins.NewIncindencia(collection, IdUsuario);

                    ClassIncidencias cincidencias = new ClassIncidencias();
                    ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
                    
                    modelo.validacion = true;
                    modelo.Mensaje = "La incidencia se guardo de forma correcta!";
                    return View(modelo);
                }
                else
                {
                    ClassIncidencias cincidencias = new ClassIncidencias();
                    ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
                    modelo.validacion = false;
                    modelo.Mensaje = "La incidencia no se pudo insertar!";
                    return View(modelo);
                }
            }
            catch(Exception ex)
            {
                ClassIncidencias cincidencias = new ClassIncidencias();
                ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
                modelo.validacion = false;
                modelo.Mensaje = "La incidencia no se pudo insertar: " + ex;
                return View(modelo);
            }
        }

        // GET: Incidencias/Edit/5
        public ActionResult Edit(int id)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            string token = Session["sToken"].ToString();
            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias incidencia = cincidencias.GetModelIncidencia(id, token);
            ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente, incidencia);

            return View(modelo);
        }

        // POST: Incidencias/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Incidencias/Delete/5
        public ActionResult Delete(int id)
        {
            string token = Session["sToken"].ToString();
            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias incidencia = cincidencias.GetModelIncidencia(id, token);
            ViewData["IdPeriodoNomina"] = incidencia.IdPeriodoNomina;

            return PartialView(incidencia);
        }

        // POST: Incidencias/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, int IdPeriodoNomina)
        {
            try
            {
                int IdUsuario = (int)Session["sIdUsuario"];
                string token = Session["sToken"].ToString();
                ClassIncidencias cIncidencias = new ClassIncidencias();
                cIncidencias.DeleteIncidencia(id, token);
                
                return RedirectToAction("Index", new { pIdPeriodoNomina = IdPeriodoNomina });
            }
            catch
            {
                return RedirectToAction("Index", new { pIdPeriodoNomina = IdPeriodoNomina });
            }
        }

        public JsonResult ObtenTipoDato(int Id)
        {
            ClassConceptos cconceptos = new ClassConceptos();
            var TipoDato = cconceptos.GetvConcepto(Id);

            return Json(TipoDato, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLayout(int pIdPeriodoNomina)
        {
            ViewData["IdPeriodoNomina"] = pIdPeriodoNomina;
            ViewBag.Periodo = pIdPeriodoNomina;

            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
            modelo = cincidencias.FindListIncidencias(IdUnidad);
            modelo.IdPeriodoNomina = pIdPeriodoNomina;
            return View(modelo);
        }

        public FileResult Excel()
        {
            int IdCliente = (int)Session["sIdCliente"];

            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado initEmpleado = new ClassEmpleado();

            var arch = initEmpleado.ExcelIncidencias(IdUnidadNegocio, IdCliente);

            return File(arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Empleados.xlsx");
        }


        [HttpPost]
        public ActionResult CreateLayout(ModelIncidencias model)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ViewBag.Periodo = model.IdPeriodoNomina;

            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);

            if (ModelState.IsValid)
            {
                if (model.Archivo.ContentLength > 0)
                {
                    if (model.idFormato == 1)
                    {
                        string fileName = Path.GetFileName(model.Archivo.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                        model.Archivo.SaveAs(_path);

                        ModelErroresIncidencias errores = cincidencias.GetIncidenciasArchivo(_path, IdCliente, IdUnidad, model.IdPeriodoNomina, IdUsuario);
                        ViewBag.Finalizado = "SI";

                        return TextFile(errores);
                    }
                    else
                    {
                        string fileName = Path.GetFileName(model.Archivo.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                        model.Archivo.SaveAs(_path);

                        ModelErroresIncidencias errores = cincidencias.GetIncidenciasArchivodos(_path, IdCliente, IdUnidad, model.IdPeriodoNomina, IdUsuario);
                        ViewBag.Finalizado = "SI";

                        return TextFile(errores);
                    }
                }                
            }

            return View(modelo);
        }
                
        public ActionResult DeleteAll(int pIdPeriodoNomina, int? IdConcepto)
        {
            try
            {
                string token = Session["sToken"].ToString();
                ClassIncidencias cincidencias = new ClassIncidencias();

                if(IdConcepto != null)
                    cincidencias.DeleteAllIdConcepto(pIdPeriodoNomina, (int)IdConcepto);
                else
                    cincidencias.DeleteAll(pIdPeriodoNomina, token);

                return RedirectToAction("Index", new { pIdPeriodoNomina = pIdPeriodoNomina });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { pIdPeriodoNomina = pIdPeriodoNomina });
            }
        }

        public ActionResult TextFile(ModelErroresIncidencias model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;
            
            tw.WriteLine("DETALLE DE CARGA DE INCIDENCIAS DEL ARCHIVO: " + model.Path);
            tw.WriteLine("");
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("Numero de Registros Leidos: " + model.Errores);
            tw.WriteLine("Insertados correctamente: " + model.Correctos);
            tw.WriteLine("No Insertados: " + model.Errores);
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("");

            if (list.Count > 0)
            {
                tw.WriteLine("Detalle de los errores:");
                tw.WriteLine("");
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
            else
            {
                tw.WriteLine("El archivo se cargo correctamente.");
            }

            tw.Flush();
            tw.Close();
            return File(memoryStream.GetBuffer(), "text/plain", "resultado.txt");
        }

        public JsonResult Descargar(int Id)
        {
            ClassReportesIncidencias rincidencias = new ClassReportesIncidencias();
            List<ModelReporteIncidencias> incidencias = rincidencias.GetReporteIncidencias(Id);
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(incidencias);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Checador(int pIdPeriodoNomina)
        {
            var cRegistro = new cRegistroAsistencias();
            ClassAccesosGV CAGV = new ClassAccesosGV();
           
            string reloj = Session["sRelojChecador"].ToString();
            if (reloj != "SI")
            {
                return View();   
            }
            int? IdCliente = int.Parse(Session["sIdClientes"].ToString());

            var Accesos = CAGV.DatosGV((int)IdCliente);

            var token = cRegistro.GetToken(Accesos).token;
            Session["sTokenGeovictoria"] = token.ToString();
            int IdCleinte = Int32.Parse(Session["sIdClientes"].ToString());
            if (Session["sTokenGeovictoria"] == null)
            {
                return View();
            }

            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }

            else
            {
                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                PeriodoNomina vPeriodoNom = classPeriodoNomina.GetPeriodo((int)pIdPeriodoNomina);
                cUsers cu = new cUsers();

                if (vPeriodoNom.FechaFinChecador == null || vPeriodoNom.FechaInicioChecador == null) {
                    Session["FlagFechasRelojChecador"] = "Empty";
                    return RedirectToAction("Index", "Incidencias");
                }

                DateTime FechaHoy = DateTime.Now;
                DateTime FechaUtilizadaFin = new DateTime();
                DateTime FechaFinChecador = (DateTime)vPeriodoNom.FechaFinChecador;
                DateTime FechaInicioChecador = (DateTime)vPeriodoNom.FechaInicioChecador;

                Session["FlagFechasRelojChecador"] = "OK";

                if ((DateTime)vPeriodoNom.FechaFinChecador > FechaHoy)
                {
                    FechaUtilizadaFin = FechaHoy;
                }
                else
                {
                    FechaUtilizadaFin = (DateTime)vPeriodoNom.FechaFinChecador;
                }
                    
                TimeSpan difFechas = FechaUtilizadaFin - (DateTime)vPeriodoNom.FechaInicioChecador;
                int dias = difFechas.Days;
                int usuXCon = 0;

                if (dias <= 7)
                {
                    usuXCon = 199;
                }
                else if(dias <= 10)
                {
                    usuXCon = 149;
                }
                else if (dias <= 15)
                {
                    usuXCon = 99;
                }
                else if(dias <= 31)
                {
                    usuXCon = 48;
                }

                string d1 = FechaInicioChecador.ToString("yyyy-MM-dd");
                string da1 = d1.Replace("-", "");
                string dat1 = da1 + "000000";
                string d2 = FechaUtilizadaFin.ToString("yyyy-MM-dd");
                string da2 = d2.Replace("-", "");
                string dat2 = da2 + "235959";

                var usuarios = cu.lstUsuarios(token,usuXCon);
                var b = cu.Consolidated(token, dat1, dat2, usuarios,usuXCon);
                var consulta = cu.ClaveEmpl(IdCleinte);
                var lstClaves = cu.ClaveEmpImss(consulta);

                ViewBag.lstClaves = lstClaves;
                ViewBag.fechaInicio = vPeriodoNom.FechaInicioChecador;
                ViewBag.fechaFinal = vPeriodoNom.FechaFinChecador;
                ViewBag.nombres = cu.NombreU(token);
                ViewBag.vPeriodo = pIdPeriodoNomina;
                ViewBag.Remuneraciones = b;
                return View(b);
            }
        }

        public ActionResult AttendanceBook(string Identifier, int pIdPeriodoNomina)
        {

            var cRegistro = new cRegistroAsistencias();
            ClassAccesosGV CAGV = new ClassAccesosGV();

            string reloj = Session["sRelojChecador"].ToString();
            if (reloj != "SI")
            {
                return View();
            }
            int? IdCliente = int.Parse(Session["sIdClientes"].ToString());

            var Accesos = CAGV.DatosGV((int)IdCliente);

            var token = cRegistro.GetToken(Accesos).token;
            Session["sTokenGeovictoria"] = token.ToString();
            if (Session["sTokenGeovictoria"] == null)
            {
                return View();
            }

            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }

            else
            {
                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                vPeriodoNomina vPeriodoNom = classPeriodoNomina.GetvPeriodoNominasId(pIdPeriodoNomina);
                cUsers cu = new cUsers();

                DateTime FechaFinChecador = (DateTime)vPeriodoNom.FechaFinChecador;
                DateTime FechaInicioChecador = (DateTime)vPeriodoNom.FechaInicioChecador;

                if (vPeriodoNom.FechaFinChecador == null || vPeriodoNom.FechaInicioChecador == null)
                {
                    Session["FlagFechasRelojChecador"] = "Empty";
                    return RedirectToAction("Index", "Incidencias");
                }

                
                string d1 = FechaInicioChecador.ToString("yyyy-MM-dd");
                string da1 = d1.Replace("-", "");
                string dat1 = da1 + "000000";
                string d2 = FechaFinChecador.ToString("yyyy-MM-dd");
                string da2 = d2.Replace("-", "");
                string dat2 = da2 + "235959";

                string usuario = cu.CheckUsuario(token,Identifier);
                var b = cu.AttendanceBook(token, dat1, dat2, usuario);

                ViewBag.fechaInicio = vPeriodoNom.FechaInicioChecador;
                ViewBag.fechaFinal = vPeriodoNom.FechaFinChecador;
                ViewBag.nombres = cu.NombreU(token);
                ViewBag.vPeriodo = pIdPeriodoNomina;

                return View(b);
            }
        }

        public ActionResult GeneraIncidencias(int pIdPeriodoNomina)
        {
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            vPeriodoNomina vPeriodoNom = classPeriodoNomina.GetvPeriodoNominasId(pIdPeriodoNomina);
            cUsers cu = new cUsers();
            int? IdCliente = int.Parse(Session["sIdClientes"].ToString());
            string token = Session["sToken"].ToString();
            string tokenGV = Session["sTokenGeovictoria"].ToString();
            if (Session["sTokenGeovictoria"] == null)
                return RedirectToAction("Checador", pIdPeriodoNomina);

            
            DateTime FechaFinChecador = (DateTime)vPeriodoNom.FechaFinChecador;
            DateTime FechaInicioChecador = (DateTime)vPeriodoNom.FechaInicioChecador;

            if (vPeriodoNom.FechaFinChecador == null || vPeriodoNom.FechaInicioChecador == null)
            {
                Session["FlagFechasRelojChecador"] = "Empty";
                return RedirectToAction("Index", "Incidencias");
            }

            TimeSpan difFechas = (DateTime)vPeriodoNom.FechaFinChecador - (DateTime)vPeriodoNom.FechaInicioChecador;
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

            string d1 = FechaInicioChecador.ToString("yyyy-MM-dd");
            string da1 = d1.Replace("-", "");
            string dat1 = da1 + "000000";
            string d2 = FechaFinChecador.ToString("yyyy-MM-dd");
            string da2 = d2.Replace("-", "");
            string dat2 = da2 + "235959";

            var usuarios = cu.lstUsuarios(tokenGV, usuXCon);
            var a = cu.EliminaIncidenciasChecador(token, pIdPeriodoNomina);
            var b = cu.Consolidated(tokenGV, dat1, dat2, usuarios, usuXCon);
            var c = cu.IncidenciasGV(b, token, pIdPeriodoNomina, (int)IdCliente);

            return RedirectToAction("Index", "Incidencias", new { pIdPeriodoNomina = pIdPeriodoNomina });
        }
    }
}
