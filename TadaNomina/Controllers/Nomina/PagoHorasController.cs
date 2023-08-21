using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.EAM;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PagoHorasController : BaseController
    {
        // GET: PagoHoras
        public ActionResult Index()
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

        public ActionResult ConfiguracionEmpleados()
        {
            ClassEmpleado ce = new ClassEmpleado();
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var model = ce.GetEmpleadoByUnidadNegocio(IdUnidadNegocio);
            return View(model);
        }
                
        public ActionResult ConfiguracionEmpleado(int id)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassConfiguracion cc = new ClassConfiguracion();
            var model = cc.GetModel(id, IdUnidadNegocio);

            return View(model);
        }

        [HttpPost]
        public JsonResult GuardaTodo(ModelConfEmpleadoHoras m)
        {
            return Json("Exito", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardaInformacionPrincipal(decimal cuotaFija, decimal cobroHora, decimal meta, string tipo, decimal bono)
        {
            return Json("Exito", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardaMaterias(string materias)
        {
            if (materias != "{[null]}")
            {
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GuardaPersonalACargo(string idsPersonal)
        {
            return Json("Exito", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListHoras(int pIdPeriodoNomina)
        {
            return View();
        }
    }
}