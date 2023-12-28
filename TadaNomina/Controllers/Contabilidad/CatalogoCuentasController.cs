using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Contabilidad;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Contabilidad
{
    public class CatalogoCuentasController : BaseController
    {
        // GET: CatalogoCuentas
        public ActionResult Index(int? idRegistroPatronal)
        {
            var model = new ModelIndex();
            var cc = new sContabilidad();
            var list = cc.GetCuentas((int)Session["sIdCliente"], 1, null, Session["sToken"].ToString());

            var reg = new ClassRegistroPatronal();

            if (idRegistroPatronal != null)
                model.lcuentas = list.Where(x => x.IdRegistroPatronal == idRegistroPatronal).ToList();
            else
                model.lcuentas = list;

            model.lRegistros = reg.getSelectRegistro((int)Session["sIdCliente"]);

            return View(model);
        }

        [HttpGet]
        public ActionResult newCuenta()
        {
            var cc = new sContabilidad();
            var reg = new ClassRegistroPatronal();

            var TipoNomina = cc.GetTipoNomina(Session["sToken"].ToString());
            var tipoCuenta = cc.GetTipoCuenta(Session["sToken"].ToString());
            var select = new List<SelectListItem>();
            var selectCuenta = new List<SelectListItem>();
            TipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.tipoNomina, Value = x.idTipoNomina.ToString() }); });
            tipoCuenta.ForEach(x => { selectCuenta.Add(new SelectListItem { Text = x.TipoCuenta, Value = x.IdTipoCuenta.ToString() }); });

            var model = new ModelNuevaCuentaContable();
            model.lTipoNomina = select;
            model.lTipoCenta = selectCuenta;
            model.lConceptos = new List<SelectListItem>();
            model.lRegistroPatronal = reg.getSelectRegistro((int)Session["sIdCliente"]);

            return View(model);
        }

        [HttpPost]
        public ActionResult newCuenta(ModelNuevaCuentaContable model)
        {

            var cc = new sContabilidad();
            var reg = new ClassRegistroPatronal();

            var TipoNomina = cc.GetTipoNomina(Session["sToken"].ToString());
            var tipoCuenta = cc.GetTipoCuenta(Session["sToken"].ToString());
            var select = new List<SelectListItem>();
            var selectCuenta = new List<SelectListItem>();
            TipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.tipoNomina, Value = x.idTipoNomina.ToString() }); });
            tipoCuenta.ForEach(x => { selectCuenta.Add(new SelectListItem { Text = x.TipoCuenta, Value = x.IdTipoCuenta.ToString() }); });

            model.lTipoNomina = select;
            model.lTipoCenta = selectCuenta;
            model.lConceptos = new List<SelectListItem>();
            model.lRegistroPatronal = reg.getSelectRegistro((int)Session["sIdCliente"]);

            try
            {
                if (model.IdTipoNomina == null) { model.IdTipoNomina = 0; }
                var resutl = cc.AddCuentaContable(Session["sToken"].ToString(), model.IdReferencia, int.Parse(Session["sIdCliente"].ToString()), model.IdRegistroPatronal, 1, model.IdRegistroPatronal, model.Clave, model.Descripcion, model.IdTipoNomina, model.Concepto, model.IdTipoCuenta);

                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;
            }
            catch (Exception ex)
            {
                ViewBag.Validacion = false;
                ViewBag.Mensaje = ex.Message;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult editCuenta(int IdCuenta)
        {
            var cc = new sContabilidad();

            var TipoNomina = cc.GetTipoNomina(Session["sToken"].ToString());
            var tipoCuenta = cc.GetTipoCuenta(Session["sToken"].ToString());
            var cuenta = cc.GetCuenta(IdCuenta, Session["sToken"].ToString());
            var select = new List<SelectListItem>();
            var selectCuenta = new List<SelectListItem>();
            TipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.tipoNomina, Value = x.idTipoNomina.ToString() }); });
            tipoCuenta.ForEach(x => { selectCuenta.Add(new SelectListItem { Text = x.TipoCuenta, Value = x.IdTipoCuenta.ToString() }); });

            var model = new ModelNuevaCuentaContable();
            model.lTipoNomina = select;
            model.lTipoCenta = selectCuenta;
            model.Clave = cuenta.clave;
            model.Descripcion = cuenta.descripcion;
            model.IdTipoNomina = cuenta.idTipoNomina;

            if (model.IdTipoNomina == 1)
            {
                var Nomina = cc.GetTipoCatNomina(Session["sToken"].ToString());
                var _lconceptos = new List<SelectListItem>();
                Nomina.ForEach(x => { _lconceptos.Add(new SelectListItem { Text = x.nombre, Value = x.idCatTipoNomina.ToString() }); });
                model.lConceptos = _lconceptos;
            }
            else if (model.IdTipoNomina == 2)
            {
                var conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()));
                var _lconceptos = new List<SelectListItem>();
                conceptos.ForEach(x => { _lconceptos.Add(new SelectListItem { Text = x.ClaveConcepto + " - " + x.DescripcionConcepto, Value = x.IdConcepto.ToString() }); });
                model.lConceptos = _lconceptos;
            }
            else
            {
                model.lConceptos = new List<SelectListItem>();
            }

            model.IdTipoCuenta = cuenta.IdTipoCuenta;
            model.Concepto = cuenta.conceptoNomina;

            return View(model);
        }

        [HttpPost]
        public ActionResult editCuenta(int IdCuenta, ModelNuevaCuentaContable model)
        {

            var cc = new sContabilidad();

            var TipoNomina = cc.GetTipoNomina(Session["sToken"].ToString());
            var tipoCuenta = cc.GetTipoCuenta(Session["sToken"].ToString());
            var select = new List<SelectListItem>();
            var selectCuenta = new List<SelectListItem>();
            TipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.tipoNomina, Value = x.idTipoNomina.ToString() }); });
            tipoCuenta.ForEach(x => { selectCuenta.Add(new SelectListItem { Text = x.TipoCuenta, Value = x.IdTipoCuenta.ToString() }); });

            model.lTipoNomina = select;
            model.lTipoCenta = selectCuenta;
            model.lConceptos = new List<SelectListItem>();


            try
            {
                if (model.IdTipoNomina == null) { model.IdTipoNomina = 0; }
                var resutl = cc.editCuentaContable(Session["sToken"].ToString(), IdCuenta, model.Clave, model.Descripcion, model.IdTipoNomina, model.Concepto, model.IdTipoCuenta);

                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;
            }
            catch (Exception ex)
            {
                ViewBag.Validacion = false;
                ViewBag.Mensaje = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult getLitConceptos(int IdTipoNomina)
        {            
            var cc = new sContabilidad();
            if (IdTipoNomina == 1)
            {
                var Nomina = cc.GetTipoCatNomina(Session["sToken"].ToString());
                return Json(Nomina, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()));
                return Json(Conceptos, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VerSubCuentas(int IdCuenta, int Nivel, string _Descripcion)
        {
            var cc = new sContabilidad();                        
            var model = new ModelCuentas();
            model.IdCuenta = IdCuenta;
            model.Nivel = Nivel;
            model._Descripcion = _Descripcion;
           
            model.cuentas = cc.GetCuentas((int)Session["sIdCliente"], (int)Nivel + 1, IdCuenta, Session["sToken"].ToString());

            return View(model);
        }

        public ActionResult newSubCuenta(int IdCuenta, int Nivel, string _Descripcion)
        {
            var cc = new sContabilidad();
            var TipoNomina = cc.GetTipoNomina(Session["sToken"].ToString());
            var tipoCuenta = cc.GetTipoCuenta(Session["sToken"].ToString());
            var select = new List<SelectListItem>();
            var selectCuenta = new List<SelectListItem>();
            TipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.tipoNomina, Value = x.idTipoNomina.ToString() }); });
            tipoCuenta.ForEach(x => { selectCuenta.Add(new SelectListItem { Text = x.TipoCuenta, Value = x.IdTipoCuenta.ToString() }); });
            
            var model = new ModelNuevaCuentaContable();
            model.IdReferencia = IdCuenta;
            model.Nivel = Nivel;
            model._Descripcion = _Descripcion;
            model.lTipoNomina = select;
            model.lTipoCenta = selectCuenta;
            model.lConceptos = new List<SelectListItem>();

            return View(model);
        }

        [HttpPost]
        public ActionResult newSubCuenta(ModelNuevaCuentaContable model)
        {
            var cc = new sContabilidad();

            var TipoNomina = cc.GetTipoNomina(Session["sToken"].ToString());
            var tipoCuenta = cc.GetTipoCuenta(Session["sToken"].ToString());
            var select = new List<SelectListItem>();
            var selectCuenta = new List<SelectListItem>();
            TipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.tipoNomina, Value = x.idTipoNomina.ToString() }); });
            tipoCuenta.ForEach(x => { selectCuenta.Add(new SelectListItem { Text = x.TipoCuenta, Value = x.IdTipoCuenta.ToString() }); });

            model.lTipoNomina = select;
            model.lTipoCenta = selectCuenta;
            model.lConceptos = new List<SelectListItem>();

            try
            {
                if (model.IdTipoNomina == null) { model.IdTipoNomina = 0; }
                var resutl = cc.AddCuentaContable(Session["sToken"].ToString(), model.IdReferencia, int.Parse(Session["sIdCliente"].ToString()), model.IdRegistroPatronal, (int)model.Nivel + 1, model.IdCuentaCliente, model.Clave, model.Descripcion, model.IdTipoNomina, model.Concepto, model.IdTipoCuenta);

                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;
            }
            catch (Exception ex)
            {
                ViewBag.Validacion = false;
                ViewBag.Mensaje = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult deleteCuenta(int _id, int? IdCuenta_, int? Nivel_, string _Descripcion_)
        {
            try
            {
                var cc = new sContabilidad();
                var resul = cc.removeCuentas(_id, Session["sToken"].ToString());
            }
            catch
            {
            }

            if(IdCuenta_ != null)
                return RedirectToAction("VerSubC uentas", new { IdCuenta = IdCuenta_, Nivel = Nivel_, _Descripcion = _Descripcion_ } );
            else
                return RedirectToAction("Index");
        }
    }
}