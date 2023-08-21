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
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelFonacot model)
        {
            ClassFonacot cf = new ClassFonacot();            
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                if (model.IdEmpleado>0)
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
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            ClassFonacot cf = new ClassFonacot();
            var model = cf.getCreditoFonacotById(id);
            return View(model);
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
    }
}