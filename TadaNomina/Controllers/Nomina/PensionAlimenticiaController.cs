using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PensionAlimenticiaController : BaseController
    {
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticia(IdUnidadNegocio);
            return View(model);
        }

        public ActionResult Create()
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = new ModelPensionAlimenticia();
            model.lTipo = ci.getTipos();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticiaId(id);
            model.lTipo = ci.getTipos();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelPensionAlimenticia model)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            model.lTipo = ci.getTipos();
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                if (model != null)
                {
                    ci.createPension(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información de la pensión!";
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
        public ActionResult Edit(ModelPensionAlimenticia model)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                if (model != null)
                {
                    ci.editPension(model, IdUsuario);
                    model.lTipo = ci.getTipos();
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información de la pensión!";
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
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticiaId(id);
            return PartialView(model);
        }

        public ActionResult Delete(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticiaId(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Delete(ModelPensionAlimenticia model)
        {
            try
            {
                ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
                ci.DeletePension(model.IdPensionAlimenticia, (int)Session["sIdUsuario"]);
                ci.DeleteIncidenciasPension(model.IdPensionAlimenticia, (int)Session["sIdUsuario"]);
            }
            catch { }

            return RedirectToAction("Index");
        }
    }
}