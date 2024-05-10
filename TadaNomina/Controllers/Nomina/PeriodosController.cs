using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.CalculoFiniquito;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PeriodosController : BaseController
    {
        // GET: Periodos
        public ActionResult Index()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model =  cperiodo.GetModelPeriodoNominas(IdUnidadNegocio);
                return View(model);
            }
        }

        public ActionResult Create()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelPeriodoNomina collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
                    int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                    int IdCliente_PTU = (int)Session["sIdCliente"];
                    int IdUsuario = (int)Session["sIdUsuario"];
                    string token = (string)Session["sToken"];
                    collection.IdUnidadNegocio = IdUnidadNegocio;
                    collection.IdCliente_PTU = IdCliente_PTU;
                    if (DateTime.Parse(collection.FechaFin) < DateTime.Parse(collection.FechaInicio)) { throw new Exception("Las fecha final no puede ser menor a la fecha inicial."); }

                    // Validamos si existen pendientes por timbrar
                    if (cperiodos.ValidaTimbrado(IdUnidadNegocio) > 0)
                    {
                        ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio);
                        model.Validacion = false;
                        model.Mensaje = "Existen periodos de nómina con recibos pendientes por timbrar, favor de validar!";
                        return View(model);
                    }

                    cperiodos.AddPeriodoNomina(collection, token);

                    return RedirectToAction("Index", "Periodos");
                }
                else
                {
                    int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                    ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
                    ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio);
                    model.Validacion = false;
                    model.Mensaje = "Faltan datos por capturar, favor de validar!";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
                ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio);

                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
                return View(model);
            }
        }

        public ActionResult Edit(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModelPeriodoNomina collection)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            try
            {                
                if (ModelState.IsValid)
                {                    
                    int IdUsuario = (int)Session["sIdUsuario"];                    
                    int IdCliente_PTU = (int)Session["sIdCliente"];                    
                    cperiodos.EditPeriodoNomina(collection, IdUsuario);

                    return RedirectToAction("Index", "Periodos");
                }
                else
                {
                    int IdUnidad = (int)Session["sIdUnidadNegocio"];
                    ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(collection.IdPeriodoNomina);
                    modelo.IdsPeriodosAjuste.Replace("0", "");
                    ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);                    
                    return View(model);
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(collection.IdPeriodoNomina);
                ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);
                return View(model);
            }
        }
       
        [HttpPost]
        public JsonResult Delete(int IdPeriodoNomina)
        {
            try
            {
                ClassPeriodoNomina periodo = new ClassPeriodoNomina();
                int IdUsuario = (int)Session["sIdUsuario"];
                periodo.DeletePeriodoNomina(IdPeriodoNomina, IdUsuario);

                ClassProcesosFiniquitos cpf = new ClassProcesosFiniquitos();
                cpf.DeleteAllConfiguracionFiniquitos(IdPeriodoNomina);

                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detail(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AcumularNomina(int IdPeriodoNomina, DateTime FechaDispersion)
        {
            try
            {                
                ClassPeriodoNomina periodo = new ClassPeriodoNomina();
                ClassFonacot fonacot = new ClassFonacot();
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                var detalles = periodo.getDatosNominaByIdPeriodo(IdPeriodoNomina);
                periodo.AcumularPeriodoNomina(IdPeriodoNomina, FechaDispersion, IdUsuario);
                fonacot.consolidaIncidenciasFonacot(IdPeriodoNomina);

                ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                var uni = cun.getUnidadesnegocioId(IdUnidad);

                if (uni.NotificarAcumular == "S" && uni.CorreosNotificacion != null && uni.CorreosNotificacion.Length > 0)
                {
                    ClassNotificaciones cn = new ClassNotificaciones();
                    ModelPeriodoNomina modelo = periodo.GetModelPeriodoNominasId(IdPeriodoNomina);
                    var correos = uni.CorreosNotificacion.Replace(" ", "").Trim().Split(',').ToArray();
                                        
                    string html = "<html><body>¡Atencion! Se Acumulo el Periodo de Nómina: <h4> " + modelo.Periodo +
                        "</h4> <br /><br /> <table border='1'><tr><th>Unidad</th><th>Periodo</th><th>F. Inicial</th><th>F. Final</th><th>F. Dispersión</th><th>ISR</th><th>Carga Obrera</th><th>Carga Patronal</th><th>Total Percepciones</th><th>Total Deducciones</th><th>Neto</th></tr>";
                    html += "<tr><td>" + modelo.UnidaNegocio + "</td><td>" + modelo.Periodo + "</td><td>" + modelo.FechaInicio + "</td><td>" + modelo.FechaFin + "</td><td>" + FechaDispersion.ToShortDateString() + 
                        "</td><td>" + detalles.Select(x => x.isr).Sum().Value.ToString("C") + "</td><td>" + detalles.Select(x => x.cargaObrera).Sum().Value.ToString("C") + 
                        "</td><td>" + detalles.Select(x => x.cargaPatronal).Sum().Value.ToString("C") + "</td><td>" + detalles.Select(x => x.totalPerc).Sum().Value.ToString("C") + 
                        "</td><td>" + detalles.Select(x => x.totalDed).Sum().Value.ToString("C") + "</td><td>" + detalles.Select(x => x.neto).Sum().Value.ToString("C") + "</td></tr>";
                    html += "</table></body></html>";

                    cn.EnviarCorreoTada(html, "Acumulación de Periodo", correos, "Nómina");
                }

                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Acumulados()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model = cperiodo.GetModelPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();
                return View(model);
            }
        }

        public ActionResult DesAcumularNomina(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);

            return PartialView("DesAcumularNomina", model);
        }

        [HttpPost]
        public ActionResult DesAcumularNomina(ModelPeriodoNomina modelo, int Id)
        {
            try
            {
                ClassPeriodoNomina periodo = new ClassPeriodoNomina();
                ClassFonacot fonacot = new ClassFonacot();
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassTimbradoNomina ctimbrado = new ClassTimbradoNomina();
                int cantidad = ctimbrado.GetCantidadTimbresPeriodoNomina(Id);
                if (cantidad == 0)
                {
                    periodo.DesAcumularPeriodoNomina(Id, IdUsuario);
                    fonacot.desacumulaIncidenciasFonacot(Id);
                }
                
                return RedirectToAction("Acumulados");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public ActionResult NominasAcumuladas()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model = cperiodo.GetModelPeriodoNominasAcumuladas(IdUnidadNegocio);
                return View(model);
            }
        }
    }
}
