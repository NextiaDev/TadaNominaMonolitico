using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Costeos;
using TadaNomina.Models.ClassCore.CosteosConfig;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Facturacion
{
    public class FondeosController : BaseController
    {
        public ActionResult FondeosFijos()
        {
            try
            {
                var sf = new sFondeos();
                var model = sf.GetFondeos((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString()).Where(x=> x.IdPeriodoNomina == 0).ToList();
                return View(model);
            }
            catch
            {
                return View(new List<vCosteos_Fondeos>());
            }
        }

        public ActionResult FondeosPeriodo()
        {
            var model = new ModelFondeosPeriodo();
            var cp = new ClassPeriodoNomina();
            model.lPeriodos = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
            return View(model);            
        }

        [HttpPost]
        public ActionResult FondeosPeriodo(ModelFondeosPeriodo model)
        {            
            var cp = new ClassPeriodoNomina();
            model.lPeriodos = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
            
            try
            {
                var sf = new sFondeos();
                model.fondeos = sf.GetFondeos((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString()).Where(x => x.IdPeriodoNomina == model.IdPeriodoNomina).ToList();                
            }
            catch
            {
                model.fondeos = new List<vCosteos_Fondeos>();
            }

            return View(model);
        }

        public ActionResult CreateFondeo()
        {
            var model = new ModelFondeos();
            var cpn = new ClassPeriodoNomina();
            var cc = new ClassCosteos();
            var rp = new ClassRegistroPatronal();
            var ccn = new ClassConceptos();

            model.lCosteos = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
            model.lPeriodo = cpn.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
            model.lPatrona = new List<SelectListItem>();
            model.lDivision = new List<SelectListItem>();
            model.lConceptos = ccn.getSelectConceptos((int)Session["sIdCliente"]);
            model.lConceptosSelect = new List<SelectListItem>();

            return View(model);
        }

        public ActionResult CreateFondeoPeriodo()
        {
            var model = new ModelFondeos();
            var cpn = new ClassPeriodoNomina();
            var cc = new ClassCosteos();
            var rp = new ClassRegistroPatronal();
            var ccn = new ClassConceptos();

            model.lCosteos = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
            model.lPeriodo = cpn.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
            model.lPatrona = new List<SelectListItem>();
            model.lDivision = new List<SelectListItem>();
            model.lConceptos = ccn.getSelectConceptos((int)Session["sIdCliente"]);
            model.lConceptosSelect = new List<SelectListItem>();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateFondeo(ModelFondeos model)
        {
            var cpn = new ClassPeriodoNomina();
            var cc = new ClassCosteos();
            var rp = new ClassRegistroPatronal();
            var ccn = new ClassConceptos();

            model.lCosteos = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
            model.lPeriodo = cpn.GetSeleccionPeriodo((int)Session["sIdUnidadNegocio"]);
            model.lPatrona = new List<SelectListItem>();
            model.lDivision = new List<SelectListItem>();
            model.lConceptos = ccn.getSelectConceptos((int)Session["sIdCliente"]);
            model.lConceptosSelect = ccn.getSelectConceptosIds(model.IdsConceptosSelec, (int)Session["sIdCliente"]);

            if (model.IdCosteo != null)
            {
                var ccf = new ClassConfigCosteo();
                var costeo = ccf.GetListCosteo((int)model.IdCosteo, Session["sToken"].ToString());

                if (costeo.dividirPatronal == "SI")
                    model.lPatrona = rp.getSelectRegistro((int)Session["sIdCliente"]);

                model.lDivision = getListDivision(costeo.separadoPor);
            }

            if (model.Importe == 0 && model.IdPeriodoNomina == null && model.Descripcion != null && model.IdsConceptosSelec != null && model.IdCosteo != null)
            {
                model.conceptos = string.Join(",", model.IdsConceptosSelec);
                var sf = new sFondeos();
                sf.AddFondeos(model, (int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
                return RedirectToAction("FondeosFijos", "Fondeos");
            }
            else
                ViewBag.Mensaje = "La informacion capturada es incorrecta.";

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateFondeoPeriodo(ModelFondeos model)
        {
            var cpn = new ClassPeriodoNomina();
            var cc = new ClassCosteos();
            var rp = new ClassRegistroPatronal();
            var ccn = new ClassConceptos();

            model.lCosteos = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
            model.lPeriodo = cpn.GetSeleccionPeriodo((int)Session["sIdUnidadNegocio"]);
            model.lPatrona = new List<SelectListItem>();
            model.lDivision = new List<SelectListItem>();
            model.lConceptos = ccn.getSelectConceptos((int)Session["sIdCliente"]);
            model.lConceptosSelect = ccn.getSelectConceptosIds(model.IdsConceptosSelec, (int)Session["sIdCliente"]);

            if (model.IdCosteo != null)
            {
                var ccf = new ClassConfigCosteo();
                var costeo = ccf.GetListCosteo((int)model.IdCosteo, Session["sToken"].ToString());

                if (costeo.dividirPatronal == "SI")
                    model.lPatrona = rp.getSelectRegistro((int)Session["sIdCliente"]);

                model.lDivision = getListDivision(costeo.separadoPor);
            }

            if (model.Importe > 0 && model.IdPeriodoNomina != null && model.Descripcion != null && model.IdsConceptosSelec == null && model.IdCosteo != null)
            {
                var sf = new sFondeos();
                sf.AddFondeos(model, (int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
                return RedirectToAction("FondeosPeriodo", "Fondeos");
            }            
            else
                ViewBag.Mensaje = "La informacion capturada es incorrecta.";

            return View(model);
        }

        [HttpPost]
        public JsonResult getListas(int IdCosteo)
        {
            var model = new ModelFondeos();
            var ccf = new ClassConfigCosteo();
            var rp = new ClassRegistroPatronal();

            var costeo = ccf.GetListCosteo(IdCosteo, Session["sToken"].ToString());

            if (costeo.dividirPatronal == "SI")
                model.lPatrona = rp.getSelectRegistro((int)Session["sIdCliente"]);

            model.lDivision = getListDivision(costeo.separadoPor);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> getListDivision(string SeparadoPor)
        {
            var list = new List<SelectListItem>();
            if (SeparadoPor != "GENERAL")
            {
                switch (SeparadoPor)
                {
                    case "CENTRO DE COSTOS":
                        var cc = new ClassCentrosCostos();
                        list = cc.getSelectCentroCostos((int)Session["sIdCliente"]);
                        break;
                    case "DEPARTAMENTOS":
                        var cd = new ClassDepartamentos();
                        list = cd.getSelectDepartamento((int)Session["sIdCliente"]);
                        break;
                    case "PUESTOS":
                        var cp = new ClassPuestos();
                        list = cp.getSelectPuestos((int)Session["sIdCliente"]);
                        break;
                }
            }

            return list;
        }

        public ActionResult DeleteFijo(int id)
        {
            var sf = new sFondeos();
            var model = sf.GetFondeo(id, Session["sToken"].ToString());
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteFijo(vCosteos_Fondeos model)
        {
            var sf = new sFondeos();
            sf.deleteFondeo(model.IdOtroConcepto, Session["sToken"].ToString());
            return RedirectToAction("FondeosFijos", "Fondeos");
        }

        public ActionResult DeletePeriodo(int id)
        {
            var sf = new sFondeos();
            var model = sf.GetFondeo(id, Session["sToken"].ToString());
            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePeriodo(vCosteos_Fondeos model)
        {
            var sf = new sFondeos();
            sf.deleteFondeo(model.IdOtroConcepto, Session["sToken"].ToString());
            return RedirectToAction("FondeosPeriodo", "Fondeos");
        }
    }
}