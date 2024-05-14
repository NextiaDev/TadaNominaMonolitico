using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Nomina
{
    public class SaldosController : BaseController
    {
        // GET: Saldos
        public ActionResult Index()
        {
            var ss = new ClassSaldos();
            var list = ss.GetMovimientos((int)Session["sIdUnidadNegocio"]);
            return View(list); 
        }

        public ActionResult Create()
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ClassSaldos cincidencias = new ClassSaldos();
            ModeloSaldos modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
            return View(modelo);
        }

        [HttpPost]
        public ActionResult Create(ModeloSaldos model)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassSaldos cl = new ClassSaldos();
            try
            {
                if (model.IdTipo == "Periodo de Tiempo")
                {
                    cl.newSaldoPeriodo(model.IdEmpleado, model.IdConcepto, model.IdTipo, model.Monto, model.FechaIncio.ToString(), model.FechaFinal.ToString(),Convert.ToInt32(model.Indefinido) , model.Observaciones, IdUsuario); 
                }
                else
                {
                    cl.newSaldo(model.IdEmpleado, model.IdConcepto, model.IdTipo, model.SaldoI, model.SaldoA, model.MontoP, model.NumeroP, model.Observaciones, IdUsuario);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("Index", "Saldos");

        }

        public ActionResult IndexConceptos(int IdConcepto, string ClaveConcepto, string Concepto)
        {
            ClassSaldos cl = new ClassSaldos();

            try
            {
                ViewBag.IdConcepto = IdConcepto;
                 var lis = cl.getSaldosList(IdConcepto, (int)Session["sIdUnidadNegocio"]);
                return View(lis);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult EditarSaldoU(int idSaldo, string Tipo, int idConcepto, decimal saldoInicial, decimal saldoActual, decimal descuentoPeriodo, decimal numeroPeriodos, string observaciones)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                string token = Session["sToken"].ToString();
                ClassIncidencias cIncidencias = new ClassIncidencias();
                var cs = new ClassSaldos();
                var inc = cs.getincidencia(idSaldo);

                if (inc != null)
                {
                    cIncidencias.DeleteIncidencia(inc.IdIncidencia, token);

                }
                
                cs.editSaldo(idSaldo, IdUsuario, Tipo, idConcepto, saldoInicial, saldoActual, descuentoPeriodo, numeroPeriodos, observaciones);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult EditarSaldoUPeriodoTiempo(int idSaldo, string Tipo, int idConcepto, decimal descuentoPeriodo, string FechaInicial, string FechaFinal, string observaciones, string indefinidon)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

            int resultado = 0;

            if (indefinidon == "true")
            {
                resultado = 1;
            }
            else
            {
                resultado = 0;
            }

            try
            {
                string token = Session["sToken"].ToString();
                ClassIncidencias cIncidencias = new ClassIncidencias();
                var cs = new ClassSaldos();
                var inc = cs.getincidencia(idSaldo);

                if (inc != null)
                {
                    cIncidencias.DeleteIncidencia(inc.IdIncidencia, token);

                }
                cs.editSaldoPeriodo(idSaldo, IdUsuario, Tipo, idConcepto, descuentoPeriodo, FechaInicial, FechaFinal, observaciones, resultado);
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]

        public JsonResult EditarSaldos(int idSaldo)
        {

            try
            {
                var cs = new ClassSaldos();
                var emp = cs.getSaldo(idSaldo);
                return Json(emp, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

     
        public JsonResult getConceptos()
        {
            int IdCliente = (int)Session["sIdCliente"];
            var cs = new ClassConceptos();
            var result = cs.GetvConceptos(IdCliente);
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult SuspenderSaldo(int idSaldo)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                var cs = new ClassSaldos();
                cs.Suspender(idSaldo, IdUsuario);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }



        }


        [HttpPost]
        public JsonResult ActivarSaldo(int idSaldo)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                var cs = new ClassSaldos();
                cs.Activar(idSaldo, IdUsuario);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult DeleteSaldos(int idSaldo)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                string token = Session["sToken"].ToString();
                ClassIncidencias cIncidencias = new ClassIncidencias();
                var cs = new ClassSaldos();
                var inc = cs.getincidencia(idSaldo);

                if (inc != null)
                {
                    cIncidencias.DeleteIncidencia(inc.IdIncidencia, token);

                }
                 cs.deleteSaldo(idSaldo, IdUsuario);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Método que desactiva todos los saldos
        /// </summary>
        /// <returns>Respuesta del movimiento</returns>
        [HttpPost]
        public JsonResult DesactivaSaldos(int tipoMov, int IdConcepto)
        {
            try
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassSaldos cS = new ClassSaldos();
                var res = cS.DesactivaSaldos(IdUnidadNegocio, IdUsuario, tipoMov, IdConcepto);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }
    }
}