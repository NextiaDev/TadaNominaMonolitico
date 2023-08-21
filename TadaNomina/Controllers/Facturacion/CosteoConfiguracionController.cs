using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Costeos;
using TadaNomina.Models.ClassCore.CosteosConfig;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Facturacion
{
    public class CosteoConfiguracionController : BaseController
    {
        // GET: CosteoConfiguracion
        //Costeos
        public ActionResult ConfiguracionCosteo()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var cc = new ClassConfigCosteo();
            var list = cc.GetListCosteos((int)Session["sIdUnidadNegocio"], Session["sToken"].ToString());
            List<CosteosModel> lst = new List<CosteosModel>();


            foreach (var item in list)
            {
                var model = new CosteosModel();
                model.idCosteo = item.idCosteo;
                model.descripcion = item.descripcion;
                model.tipoNomina = item.tipoNomina;
                model.dividirPatronal = item.dividirPatronal;
                model.separadoPor = item.separadoPor;
                lst.Add(model);
            }

            return View(lst);

        }
        public ActionResult Create()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            CosteosModel model = cCosteos.FindListPeriodos();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(CosteosModel model)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var cc = new ClassConfigCosteo();

            try
            {
                var resutl = cc.AddConfiguracionCosteo(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()), int.Parse(Session["sIdUnidadNegocio"].ToString()), model.descripcion, model.tipoNomina, model.tipoEsquema, model.dividirPatronal, model.separadoPor, model.agruparPorDescripcion);
                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;
            }
            catch (Exception ex)
            {
                ViewBag.Validacion = false;
                ViewBag.Mensaje = ex.Message;
            }

            return RedirectToAction("ConfiguracionCosteo", "CosteoConfiguracion");
        }

        public ActionResult Edit(int idCosteo)
        {
            var cc = new ClassConfigCosteo();
            var list = cc.GetListCosteo(idCosteo, Session["sToken"].ToString());
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            var select = new List<SelectListItem>();
            var model = cCosteos.FindListPeriodos();
            model.descripcion = list.descripcion;
            model.tipoNomina = list.tipoNomina;
            model.LTipoNomina.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });
            model.tipoEsquema = list.tipoEsquema;
            model.LEsquema.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });
            model.dividirPatronal = list.dividirPatronal;
            model.RegistroP.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });
            model.separadoPor = list.separadoPor;
            model.ObtenerCosteo.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });
            model.agruparPorDescripcion = list.agruparPorDescripcion.Value;
            model.CostearporDesc.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });


            return View(model);

        }

        [HttpPost]
        public ActionResult Edit(CosteosModel model)
        {
            var cc = new ClassConfigCosteo();
            var resutl = cc.Editar(Session["sToken"].ToString(), model.idCosteo, int.Parse(Session["sIdCliente"].ToString()), int.Parse(Session["sIdUnidadNegocio"].ToString()), model.descripcion, model.tipoNomina, model.tipoEsquema, model.dividirPatronal, model.separadoPor, model.agruparPorDescripcion);

            return RedirectToAction("ConfiguracionCosteo", "CosteoConfiguracion");

        }

        [HttpPost]
        public ActionResult DeleteCosteo(int idCosteo)
        {
            var cc = new ClassConfigCosteo();
            var list = cc.removeCosteo(idCosteo, Session["sToken"].ToString());

            return RedirectToAction("ConfiguracionCosteo", "CosteoConfiguracion");
        }

        //CosteosConceptos
        public ActionResult GetConceptos(int idCosteo)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            var cc = new ClassConfigCosteo();
            var list = cc.GetListConceptos(idCosteo, Session["sToken"].ToString()).Where(x => x.idEstatus == 1);

            Session["GetConceptos"] = list.ToList();

            List<CosteoConceptosM> lst = new List<CosteoConceptosM>();

            foreach (var item in list)
            {
                var model = new CosteoConceptosM();
                model.idCosteo = item.idCosteo.Value;
                model.idCosteosConcepto = item.idCosteosConcepto;
                model.descripcion = item.descripcion;
                model.observaciones = item.observaciones;
                model.visible = item.visible;
                model.orden = item.orden.Value;
                lst.Add(model);
            }

            var data = new CosteosConceptosById
            {
                IdCosteo = idCosteo,
                idCosteo = idCosteo,
                CosteoConceptos = lst
            };

            return View(data);

        }

        public ActionResult CreateConceptos(int IdCosteo)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            Session["idcosteo"] = IdCosteo;
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            CosteoConceptosM model = cCosteos.FindLisConceptos();
            model.idCosteo = IdCosteo;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateConceptos(CosteoConceptosM model)
        {
            int idcot = (int)Session["idcosteo"];
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var cc = new ClassConfigCosteo();
            try
            {
                var resutl = cc.AddConceptos(Session["sToken"].ToString(), model.idCosteo, model.descripcion, model.tipoDatoFacturacion, model.observaciones, model.orden, model.visible);
                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;

            }
            catch (Exception ex)
            {
                ViewBag.Validacion = false;
                ViewBag.Mensaje = ex.Message;
            }
            var cCosteos = new ClassConfigCosteo();
            model = cCosteos.FindLisConceptos();
            model.idCosteo = idcot;
            return View(model);
        }


        public ActionResult EditConcepto(int idCosteosConcepto, int idCosteo)
        {
            var cc = new ClassConfigCosteo();
            var list = cc.GetListConcepto(idCosteosConcepto, Session["sToken"].ToString());
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            var select = new List<SelectListItem>();
            var model = cCosteos.FindLisConceptos();
            model.descripcion = list.descripcion;
            model.orden = list.orden.Value;
            model.observaciones = list.observaciones;
            model.tipoDatoFacturacion = list.tipoDatoFacturacion;
            model.Lfacturacion.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });
            model.visible = list.visible;
            model.lVisible.ForEach(x => { select.Add(new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); });
            model.idCosteo = idCosteo;
            ViewBag.IdCosteo = model.idCosteo;

            return View(model);

        }

        [HttpPost]
        public ActionResult EditConcepto(CosteoConceptosM model)
        {
            ViewBag.IdCosteo = model.idCosteo;
            var cc = new ClassConfigCosteo();
            try
            {
                var resutl = cc.EditarConceptos(Session["sToken"].ToString(), model.idCosteosConcepto, model.descripcion, model.tipoDatoFacturacion, model.observaciones, model.orden, model.visible);
                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;
            }
            catch (Exception)
            {

                throw;
            }
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            model = cCosteos.FindLisConceptos();
           
            return View(model);

        }

        [HttpPost]
        public ActionResult DeleteConcepto(int idCosteosConcepto, int idCosteo)
        {
            var cc = new ClassConfigCosteo();
            var list = cc.removeConcepto(idCosteosConcepto, Session["sToken"].ToString());

            return RedirectToAction("GetConceptos", "CosteoConfiguracion", new { idCosteo = idCosteo });

        }


        //ConfiguracionC

        public ActionResult GetConceptosConfiguracion(int idCosteosConcepto, int idCosteo)
        {

            var ccc = new sContabilidad();
            List<CosteoConceptos> getCosteoConceptos = (List<CosteoConceptos>)Session["GetConceptos"];
            CosteoConceptos costeoConceptos = getCosteoConceptos.Where(n => n.idCosteo == idCosteo).Where(x=> x.idCosteosConcepto == idCosteosConcepto).FirstOrDefault();
            Session["costeoConceptos"] = costeoConceptos;
            CosteoConceptos costeoConcepto = (CosteoConceptos)Session["costeoConceptos"];
            List<ConfiguraConceptos> ListConceptosConfiguracion = (List<ConfiguraConceptos>)Session["GetConceptosConfiguracion"];
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var cc = new ClassConfigCosteo();
            var list = cc.GetListConceptosConfiguracion(idCosteosConcepto, Session["sToken"].ToString()).Where(x => x.idEstatus == 1);
            Session["GetConceptosConfiguracion"] = list.ToList();
            List<ConfiguraConceptosM> lst = new List<ConfiguraConceptosM>();
            foreach (var item in list)
            {

                var model = new ConfiguraConceptosM();

                if (item.tipoConcepto == "NOMINA")
                {
                    var TipoNomina = ccc.GetTipoCatNomina(Session["sToken"].ToString()).Where(x => x.idCatTipoNomina == int.Parse(item.concepto));

                    foreach (var itema in TipoNomina)
                    {
                        model.descripcion = itema.nombre;
                    }

                    model.concepto = model.idCatTipoNomina.ToString();
                }
                else if (item.tipoConcepto == "INCIDENCIAS")
                {
                    var Conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString())).Where(x => x.IdConcepto == int.Parse(item.concepto));

                    //foreach (var itema in Conceptos)
                    //{
                    //    model.descripcion = itema.d;
                    //}

                    model.descripcion = item.descripcion;
                }

                else if (item.tipoConcepto == "YA CONFIGURADO")
                {
                    var lista = Session["GetConceptos"];
                    List<CosteoConceptos> lsts = (List<CosteoConceptos>)lista;
                    var orden = lsts.Where(x => x.idCosteosConcepto == idCosteosConcepto).FirstOrDefault();

                    model.descripcion = item.descripcion;

                }

                model.idcosteo = idCosteo;
                model.concepto = costeoConcepto.descripcion;
                model.idCosteoConceptoConfiguracion = item.idCosteoConceptoConfiguracion.Value;
                model.idUnidadNegocio = item.idUnidadNegocio;
                model.idCosteosConcepto = item.idCosteosConcepto.Value;
                model.descripcion = item.descripcion;
                model.operador = item.operador;
                model.tipoConcepto = item.tipoConcepto;
                model.descripcionValor = item.descripcionValor;
                model.valor = item.valor.Value;
                model.operadorGral = item.operadorGral;
                lst.Add(model);
            }


            var data = new CosteosConceptosConfiguracionById
            {
                idCosteosConcepto = idCosteosConcepto,
                idCosteo = idCosteo,
                CosteoConceptos = lst
            };


            ViewBag.Concepto = costeoConcepto;
            return View(data);


        }

        public ActionResult CreateConceptosConfiguracion(int idCosteosConcepto)
        {
            var ccc = new sContabilidad();
            var TipoNomina = ccc.GetTipoCatNomina(Session["sToken"].ToString());
            Session["idcosteo"] = idCosteosConcepto;
            var lista = Session["GetConceptos"];
            List<CosteoConceptos> lst = (List<CosteoConceptos>)lista;
            var orden = lst.Where(x => x.idCosteosConcepto == idCosteosConcepto).FirstOrDefault();
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var cc = new ClassConfigCosteo();
            CosteoConceptos costeoConcepto = (CosteoConceptos)Session["costeoConceptos"];
            var selectt = new List<SelectListItem>();
            var select = new List<SelectListItem>();
            var confi = new List<SelectListItem>();
            var otro = new List<SelectListItem>();
            otro.Add(new SelectListItem { Text= "Recuperaciones IMSS", Value= "1" });
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            var Conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()));
            Conceptos.ForEach(x => { select.Add(new SelectListItem { Text = x.DescripcionConcepto, Value = x.IdConcepto.ToString() }); });
            TipoNomina.ForEach(x => { selectt.Add(new SelectListItem { Text = x.nombre, Value = x.idCatTipoNomina.ToString() }); });
            //lst.ForEach(x => { confi.Add(new SelectListItem { Text = x.descripcion, Value = x.idCosteosConcepto.ToString() }); });
            List<CosteoConceptos> a = lst.Where(x => x.orden < orden.orden && x.idEstatus == 1).ToList();
            a.ForEach(x => { confi.Add(new SelectListItem { Text = x.descripcion, Value = x.idCosteosConcepto.ToString() }); });
            ConfiguraConceptosM model = cCosteos.FindLisConceptosConfigur();
            model.idCosteosConcepto = idCosteosConcepto;
            model.Lconceptos = select;
            model.LNomina = selectt;
            model.LConfig = confi;
            model.Lotro = otro;
            ViewBag.Concepto = costeoConcepto;
            ViewBag.Idcosteo = idCosteosConcepto;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateConceptosConfiguracion(ConfiguraConceptosM model)
        {

            var lista = Session["GetConceptos"];
            var ccc = new sContabilidad();
            var TipoNomina = ccc.GetTipoCatNomina(Session["sToken"].ToString());
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int idcot = (int)Session["idcosteo"];
            var select = new List<SelectListItem>();
            var selectt = new List<SelectListItem>();
            var confi = new List<SelectListItem>();
            var otro = new List<SelectListItem>();
            otro.Add(new SelectListItem { Text = "Recuperaciones IMSS", Value = "1" });
            List<CosteoConceptos> lst = (List<CosteoConceptos>)lista;
            var orden = lst.Where(x => x.idCosteosConcepto == model.idCosteosConcepto).FirstOrDefault();

            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            var cc = new ClassConfigCosteo();

            var Conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()));
            Conceptos.ForEach(x => { select.Add(new SelectListItem { Text = x.DescripcionConcepto, Value = x.IdConcepto.ToString() }); });
            TipoNomina.ForEach(x => { selectt.Add(new SelectListItem { Text = x.nombre, Value = x.idCatTipoNomina.ToString() }); });
            List<CosteoConceptos> a = lst.Where(x => x.orden < orden.orden && x.idEstatus == 1).ToList();
            var descr = a.Where(x => x.idCosteosConcepto == model.idconfig).FirstOrDefault();
            a.ForEach(x => { confi.Add(new SelectListItem { Text = x.descripcion, Value = x.idCosteosConcepto.ToString() }); });
                        

            if (model.tipoConcepto == "NOMINA")
            {
                model.concepto = model.idCatTipoNomina.ToString();
            }

            else if (model.tipoConcepto == "INCIDENCIAS")
            {
                model.concepto = model.concepto.ToString();
            }

            else if (model.tipoConcepto == "YA CONFIGURADO")
            {
                model.concepto = model.idconfig.ToString();
                model.descripcion = descr.descripcion;
            }
            else if (model.tipoConcepto == "OTRO")
            {                
                model.descripcion = "Recuperaciones IMSS";
            }

            try
            {
                var resutl = cc.AddConceptosConfiguracion(Session["sToken"].ToString(), IdUnidadNegocio, model.idCosteosConcepto, model.concepto, model.descripcion, model.operador, model.tipoConcepto, model.descripcionValor, model.valor, model.operadorGral);
                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;

            }
            catch (Exception ex)
            {
                ViewBag.Validacion = false;
                ViewBag.Mensaje = ex.Message;
            }

            model = cCosteos.FindLisConceptosConfigur();
            model.idCosteosConcepto = idcot;
            model.LConfig = confi;
            model.LNomina = selectt;
            model.Lconceptos = select;
            model.Lotro = otro;

            return View(model);
        }


        public ActionResult EditConceptoConfiguracion(int IdCosteoConceptoConfiguracion)
        {
            Session["IdCosteoConceptoConfiguracion"] = IdCosteoConceptoConfiguracion;

            List<ConfiguraConceptos> ListConceptosConfiguracion = (List<ConfiguraConceptos>)Session["GetConceptosConfiguracion"];

            ConfiguraConceptos configuraConceptos = ListConceptosConfiguracion.Where(n => n.idCosteoConceptoConfiguracion == IdCosteoConceptoConfiguracion).FirstOrDefault();

            Session["ConceptosConfiguracion"] = configuraConceptos;

            CosteoConceptos costeoConcepto = (CosteoConceptos)Session["costeoConceptos"];

            var cc = new ClassConfigCosteo();

            var list = cc.GetListConceptoConfiguracion(IdCosteoConceptoConfiguracion, Session["sToken"].ToString());
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            var select = new List<SelectListItem>();
            var Conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()));
            Conceptos.ForEach(x => { select.Add(new SelectListItem { Text = x.DescripcionConcepto, Value = x.IdConcepto.ToString() }); });
            ConfiguraConceptosM model = cCosteos.FindLisConceptosConfigur();
            model.operador = list.operador;
            model.operadorGral = list.operadorGral;
            model.valor = list.valor.Value;
            model.tipoConcepto = list.tipoConcepto;
            model.concepto = list.concepto;
            model.descripcion = list.descripcion;

            model.Lconceptos = select;
            list.tipoConcepto = model.tipoConcepto;

            ViewBag.ConceptosConfiguracion = configuraConceptos;
            ViewBag.Concepto = costeoConcepto;

            return View(model);

        }

        [HttpPost]
        public ActionResult EditConceptoConfiguracion(ConfiguraConceptosM model)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            CosteoConceptos costeoConcepto = (CosteoConceptos)Session["costeoConceptos"];

            var cc = new ClassConfigCosteo();
            try
            {
                var resutl = cc.EditarConceptosConfiguracion(Session["sToken"].ToString(), IdUnidadNegocio, model.idCosteoConceptoConfiguracion, costeoConcepto.idCosteosConcepto, model.concepto, model.descripcion, model.operador, model.tipoConcepto, model.descripcionValor, model.valor, model.operadorGral);
                ViewBag.Validacion = true;
                ViewBag.Mensaje = resutl;
            }
            catch { }

            var select = new List<SelectListItem>();
            ClassConfigCosteo cCosteos = new ClassConfigCosteo();
            var Conceptos = cc.GetTipoCatConceptos(Session["sToken"].ToString(), int.Parse(Session["sIdCliente"].ToString()));
            Conceptos.ForEach(x => { select.Add(new SelectListItem { Text = x.DescripcionConcepto, Value = x.IdConcepto.ToString() }); });
            model = cCosteos.FindLisConceptosConfigur();            
            model.Lconceptos = select;

            List<ConfiguraConceptos> ListConceptosConfiguracion = (List<ConfiguraConceptos>)Session["GetConceptosConfiguracion"];
            int IdCosteoConceptoConfiguracion = (int)Session["IdCosteoConceptoConfiguracion"];
            ConfiguraConceptos configuraConceptos = ListConceptosConfiguracion.Where(n => n.idCosteoConceptoConfiguracion == IdCosteoConceptoConfiguracion).FirstOrDefault();

            ViewBag.ConceptosConfiguracion = configuraConceptos;
            ViewBag.Concepto = costeoConcepto;

            return View(model);

        }

        [HttpPost]
        public ActionResult DeleteConceptoConfiguracion(ConfiguraConceptosM model, int IdCosteoConceptoConfiguracion)
        {
            CosteoConceptos costeoConcepto = (CosteoConceptos)Session["costeoConceptos"];

            var cc = new ClassConfigCosteo();
            var list = cc.removeConceptoConfiguracion(IdCosteoConceptoConfiguracion, Session["sToken"].ToString());

            model.idCosteosConcepto = costeoConcepto.idCosteosConcepto;
            model.idcosteo = costeoConcepto.idCosteo.Value;
            return RedirectToAction("GetConceptosConfiguracion", "CosteoConfiguracion", new { model.idCosteosConcepto, model.idcosteo });
        }
    }
}