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
    }
}
