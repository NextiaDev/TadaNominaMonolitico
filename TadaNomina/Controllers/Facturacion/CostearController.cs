using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Costeos;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;

namespace TadaNomina.Controllers.Facturacion
{
    public class CostearController : BaseController
    {
        // GET: Costear
        public ActionResult Index()
        {
            var cc = new ClassCosteos();
            var cu = new ClassUnidadesNegocio();
            var cp = new ClassPeriodoNomina();
            var getCosteo = new ModelGetCosteo();

            getCosteo.lCosteo = new List<SelectListItem>();
            getCosteo.lUnidadNegocio = new List<SelectListItem>();
            getCosteo.lPeriodos = new List<SelectListItem>();
            getCosteo.lPeriodosSeleccionados = new List<SelectListItem>();

            try
            {
                getCosteo.lCosteo = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
                getCosteo.lUnidadNegocio = cu.getSelectUnidadNegocio((int)Session["sIdCliente"], Session["sToken"].ToString());
                getCosteo.IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                getCosteo.lPeriodos = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
                getCosteo.lPeriodosSeleccionados = new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje1 = "Error: " + ex.Message;
            }

            return View(getCosteo);
        }

        [HttpPost]
        public ActionResult Index(ModelGetCosteo model)
        {
            try
            {
                var cc = new ClassCosteos();
                var cu = new ClassUnidadesNegocio();
                var cp = new ClassPeriodoNomina();
                var getCosteo = new ModelGetCosteo();
                List<SelectListItem> perOtros = new List<SelectListItem>();

                if (model.IdUnidadNegocio != (int)Session["sIdUnidadNegocio"])
                {
                    perOtros = cp.GetSeleccionAllPeriodo(model.IdUnidadNegocio);
                }

                model.lCosteo = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
                model.lUnidadNegocio = cu.getSelectUnidadNegocio((int)Session["sIdCliente"], Session["sToken"].ToString());
                var per = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
                string[] idsSelect = Array.ConvertAll(model.IdsPeriodosSelecionados, s => s.ToString());
                model.lPeriodos = per.Where(x => !idsSelect.Contains(x.Value)).ToList();
                model.lPeriodosSeleccionados = per.Where(x => idsSelect.Contains(x.Value)).ToList();
                model.lPeriodosSeleccionados.AddRange(perOtros.Where(x => idsSelect.Contains(x.Value)).ToList());
                var descPer = per.Where(x => idsSelect.Contains(x.Value)).Select(x => x.Text).ToList();
                model.DescripcionPeriodos = string.Join("-", descPer);
                model.ClienteUnidad = Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();
                model.IdCliente = (int)Session["sIdCliente"];
                model.IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ViewBag.IdsPeriodosSelecionados = string.Join(",", model.IdsPeriodosSelecionados);
                if (model.IdsPeriodosSelecionados.Count() > 0)
                {
                    model.lcosteos = cc.getCosteoString(model, Session["sToken"].ToString());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje1 = "Error: " + ex.Message;
            }


            return View(model);
        }

        public ActionResult GuardarCosteo(string IdsPeriodosSelecionados, int IdCosteo)
        {
            var cc = new ClassCosteos();
            var cu = new ClassUnidadesNegocio();
            var cp = new ClassPeriodoNomina();
            var getCosteo = new ModelGetCosteo();
            var Model = new ModelGetCosteo();
            Model.IdCosteo = IdCosteo;
            Model.lCosteo = cc.getSelectCosteo((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
            Model.lUnidadNegocio = cu.getSelectUnidadNegocio((int)Session["sIdCliente"], Session["sToken"].ToString());
            var per = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);

            if (IdsPeriodosSelecionados != null)
            {
                string[] idsSelect = IdsPeriodosSelecionados.Split(',').ToArray();
                Model.IdsPeriodosSelecionados = Array.ConvertAll(idsSelect, int.Parse);
                Model.lPeriodos = per.Where(x => !idsSelect.Contains(x.Value)).ToList();
                Model.lPeriodosSeleccionados = per.Where(x => idsSelect.Contains(x.Value)).ToList();
                var descPer = per.Where(x => idsSelect.Contains(x.Value)).Select(x => x.Text).ToList();
                Model.DescripcionPeriodos = string.Join("-", descPer);
                Model.ClienteUnidad = Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();
                Model.IdCliente = (int)Session["sIdCliente"];
                Model.IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

                if (Model.lPeriodosSeleccionados.Count() > 0)
                {
                    try
                    {
                        var ids = Model.lPeriodosSeleccionados.Select(x => x.Value).ToArray();
                        var idsInt = Array.ConvertAll(ids, int.Parse);
                        var _periodos = cp.getPeriodosIds(idsInt).Where(x => x.IdEstatus == 1).Any();

                        if (!_periodos)
                        {
                            var mensaje = cc.guardarCosteos(Model, Model.IdCliente, Model.IdUnidadNegocio, IdsPeriodosSelecionados, Session["sToken"].ToString());
                            ViewBag.Mensaje = mensaje;
                            ViewBag.Validacion = true;
                        }
                        else
                            throw new Exception("No se puede solicitar ya que existen periodos abiertos.");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Mensaje = "Error: " + ex.Message;
                        ViewBag.Validacion = false;
                    }
                }
            }

            return View("Index", Model);
        }

        [HttpPost]
        public JsonResult guardaCosteosDif(string datos, string IdsPeriodosSelecionados, int IdCosteo)
        {
            try
            {
                var rows = datos.Split('&').ToList();
                var ml = new List<ModelFactDif>();
                foreach (var item in rows)
                {
                    if (item != string.Empty)
                    {
                        var valores = item.Split('|').ToArray();
                        var model = new ModelFactDif()
                        {
                            subtotal = decimal.Parse(valores[0]),
                            iva = decimal.Parse(valores[1]),
                            total = decimal.Parse(valores[2])
                        };

                        ml.Add(model);
                    }
                }

                var cc = new ClassCosteos();
                var cp = new ClassPeriodoNomina();
                var per = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]);
                string[] idsSelect = IdsPeriodosSelecionados.Split(',').ToArray();
                var _IdsPeriodosSelecionados = Array.ConvertAll(idsSelect, int.Parse);
                var descPer = per.Where(x => idsSelect.Contains(x.Value)).Select(x => x.Text).ToList();
                var DescripcionPeriodos = string.Join("-", descPer);
                var ClienteUnidad = Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();
                var mensaje = cc.guardarCosteosDif(_IdsPeriodosSelecionados, DescripcionPeriodos, IdCosteo, ClienteUnidad, ml, (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], IdsPeriodosSelecionados, Session["sToken"].ToString());

                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult getPeriodos(int IdUnidadNegocio)
        {
            var cpn = new ClassPeriodoNomina();
            var periodos = cpn.GetSeleccionAllPeriodo(IdUnidadNegocio);
            return Json(periodos, JsonRequestBehavior.AllowGet);
        }
    }
}