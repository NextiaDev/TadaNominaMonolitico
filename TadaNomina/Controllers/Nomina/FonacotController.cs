using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class FonacotController : BaseController
    {
        // GET: Fonacot
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassFonacot ci = new ClassFonacot();
            var model = ci.GetModelFonacot(IdUnidadNegocio);
            return View(model);
        }

        public ActionResult Create()
        {
            ClassFonacot cf = new ClassFonacot();
            var model = new ModelFonacot();
            model.Activo = true;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            try
            {
                ClassFonacot cf = new ClassFonacot();
                var model = cf.getCreditoFonacotById(id);
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult Edit(ModelFonacot model)
        {
            ClassFonacot ci = new ClassFonacot();
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                if (model != null)
                {
                    ci.editFonacot(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información del Credito!";
                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelFonacot model)
        {
            ClassFonacot cf = new ClassFonacot();
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                if (model.IdEmpleado > 0)
                {
                    cf.newCreditoFonacot(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información del credito!";
                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        public JsonResult BuscaEmpleado(string clave)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado cemp = new ClassEmpleado();
            var emp = cemp.GetEmpleadosByClave(clave, IdUnidadNegocio).FirstOrDefault();

            if (emp != null)
                return Json(emp, JsonRequestBehavior.AllowGet);
            else
                return Json("El Empleado con la clave que ingreso no existe!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            ClassFonacot cf = new ClassFonacot();
            var model = cf.getCreditoFonacotById(id);
            return PartialView(model);
        }

        public ActionResult Delete(int id)
        {
            ClassFonacot cf = new ClassFonacot();
            var model = cf.getCreditoFonacotById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Delete(ModelFonacot model)
        {
            try
            {
                ClassFonacot cf = new ClassFonacot();
                cf.DeleteCreditoFonacot(model.IdCreditoFonacot, (int)Session["sIdUsuario"]);
                cf.DeleteIncidenciasFonacot(model.IdCreditoFonacot, (int)Session["sIdUsuario"]);
            }
            catch { }

            return RedirectToAction("Index");
        }


        /// <summary>
        ///     Método que modifica el estdo del crédito, en caso de ser inactivo no se considerará en el cálculo de la nómina
        /// </summary>
        /// <param name="IdCredito">Id del crédito</param>
        /// <returns>Estatus del movimiento</returns>
        [HttpPost]
        public JsonResult CambiaStatusCredito(int IdCredito)
        {
            try
            {
                ClassFonacot cI = new ClassFonacot();
                int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());
                var res = cI.CambiaEstatus(IdCredito, IdUsuario);
                if (res == 1)
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                    return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }
    }
}