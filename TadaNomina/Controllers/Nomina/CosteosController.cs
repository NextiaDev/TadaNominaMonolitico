using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Costeos;
using TadaNomina.Models.ViewModels.Costeos;

namespace TadaNomina.Controllers.Nomina
{
    public class CosteosController : BaseController
    {
        // GET: Costeos
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassCosteos cc = new ClassCosteos();
            var model = cc.getModelCosteo(IdUnidadNegocio);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ModelCosteos m)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassCosteos cc = new ClassCosteos();
            var periodos = m.IdsPeriodo;
            var model = cc.getModelCosteo(IdUnidadNegocio);
            m.lPeriodos = model.lPeriodos;
            if (periodos.Length > 0)
            {                
                var costeo = cc.getCosteoTrad(periodos).OrderBy(x=> x.orden).ToList();
                m.costeo = costeo;
            }

            return View(m);
        }

        public ActionResult Limpiar()
        {
            return RedirectToAction("Index");
        }
    }
}