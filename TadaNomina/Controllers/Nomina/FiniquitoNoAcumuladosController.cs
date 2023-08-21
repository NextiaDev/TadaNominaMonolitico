using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.CalculoFiniquito;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class FiniquitoNoAcumuladosController : BaseController
    {
        // GET: FiniquitoNoAcumulados
        public ActionResult Index()
        {
            var model = new ModelMoverFiniquitos();
            var cfna = new ClassFiniquitosNoAcumulados();
            var list = cfna.getFiniquitosNoAcumulados((int)Session["sIdUnidadNegocio"]);

            var cp = new ClassPeriodoNomina();
            var per = cp.GetSeleccionPeriodoFiniquitos((int)Session["sIdUnidadNegocio"]);

            model.calculos = list;
            model.lPeriodos = per;

            return View(model);
        }

        [HttpPost]
        public JsonResult moverFiniquito(int IdEmpleado, int IdPeriodo, int nuevoIdPeriodo)
        {
            try
            {
                var cf = new ClassFiniquitosNoAcumulados();
                cf.cambioPeriodoFiniquito(IdPeriodo, IdEmpleado, nuevoIdPeriodo);

                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}