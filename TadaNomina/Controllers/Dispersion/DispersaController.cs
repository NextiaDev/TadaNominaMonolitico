using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Dispersion;
using TadaNomina.Models.ViewModels.Dispersion;

namespace TadaNomina.Controllers.Dispersion
{
    public class DispersaController : BaseController
    {
        // GET: Dispersa
        public ActionResult Index()
        {            
            var cd = new ClassDispersion();
            var model = cd.getModel((int)Session["sIdUnidadNegocio"], (int)Session["sIdCliente"], 0);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ModelDispersa model)
        {
            var cd = new ClassDispersion();
            model.lPeriodos = cd.getList((int)Session["sIdUnidadNegocio"]);
            var idsReg = cd.getRegistroPatronalesNomina(model.IdPeriodo);
            model.Saldos = cd.getSaldo((int)Session["sIdCliente"], idsReg);
            model.TotalDispersar = cd.getTotalDispersar(model.IdPeriodo);
            model.TotalMovimientos = cd.getTotalMovimientos(model.IdPeriodo);
            model.ListEmpleadosSinCuenta = cd.getEmpleadosSinCuenta(model.IdPeriodo);

            return View(model);
        }
    }
}