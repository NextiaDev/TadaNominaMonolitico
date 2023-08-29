using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.Inicio;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ClassCore;
using TadaNomina.Services;

namespace TadaNomina.Controllers
{
    public class DefaultController : BaseController
    {
        // GET: Default
        public ActionResult Index()
        {            
            var listClientes = new List<SelectListItem>();
            var modelSeleccionarNomina = new ModelSelecionarNomina();
            string token = Session["sToken"].ToString();
            var _clientes = new sClientes();
            var clientes = new List<ModelCliente>();
            try { clientes = _clientes.getClientes(token).OrderBy(x=>x.Cliente).ToList(); } catch { }
            clientes.ForEach(x => { listClientes.Add(new SelectListItem { Text = x.Cliente, Value = x.IdCliente.ToString() }); });
            modelSeleccionarNomina.cliente = listClientes;

            var listUnidades = new List<SelectListItem>();
            modelSeleccionarNomina.unidadNegocio = listUnidades;
            ViewBag.Lista = clientes;

            return View(modelSeleccionarNomina);            
        }

        [HttpPost]
        public ActionResult Index(int? idcliente, int? IdunidadNegocio)
        {

            ModelSelecionarNomina seleccionarNomina = new ModelSelecionarNomina();
            seleccionarNomina.IdCliente= idcliente;
            seleccionarNomina.IdunidadNegocio = IdunidadNegocio;

            try
            {
                if (ModelState.IsValid)
                {
                    var listClientes = new List<SelectListItem>();
                    var listUnidades = new List<SelectListItem>();
                    List<ModelUnidadNegocio> unidades = null;
                    string tipoUsuario = Session["sTipoUsuario"].ToString();
                    var _clientes = new sClientes();
                    string token = Session["sToken"].ToString();
                    var clientes = _clientes.getClientes(token).OrderBy(x => x.Cliente).ToList();

                    clientes.ForEach(x => { listClientes.Add(new SelectListItem { Text = x.Cliente, Value = x.IdCliente.ToString() }); });
                    seleccionarNomina.cliente = listClientes;

                    if (seleccionarNomina.IdCliente != null)
                    {
                        var _unidad = new sUnidadNegocio();
                        string Unidades = string.Empty;
                        try { Unidades = Session["sIdUnidades"].ToString(); } catch { }
                        unidades = _unidad.getSelectUnidadesNegocio((int)seleccionarNomina.IdCliente, token).ToList();
                        unidades.ForEach(x => { listUnidades.Add(new SelectListItem { Text = x.UnidadNegocio, Value = x.IdUnidadNegocio.ToString() }); });

                        seleccionarNomina.unidadNegocio = listUnidades;

                        Session["sIdCliente"] = clientes.Where(x => x.IdCliente == seleccionarNomina.IdCliente).Select(x => x.IdCliente).FirstOrDefault();
                        Session["sCliente"] = clientes.Where(x => x.IdCliente == seleccionarNomina.IdCliente).Select(x => x.Cliente).FirstOrDefault();
                        Session["sClienteAdministrado"] = clientes.Where(x => x.IdCliente == seleccionarNomina.IdCliente).Select(x => x.ClienteAdministrado).FirstOrDefault();
                    }

                    if (seleccionarNomina.IdunidadNegocio != null)
                    {
                        Session["sNomina"] = unidades.Where(x => x.IdUnidadNegocio == seleccionarNomina.IdunidadNegocio).Select(x => x.UnidadNegocio).FirstOrDefault();
                        Session["sIdUnidadNegocio"] = unidades.Where(x => x.IdUnidadNegocio == seleccionarNomina.IdunidadNegocio).Select(x => x.IdUnidadNegocio).FirstOrDefault();
                        
                        ViewBag.NominaSeleccionada = Session["sNomina"].ToString();
                        ViewBag.IdNomia = Session["sIdUnidadNegocio"].ToString();
                        ViewBag.IdCliente = Session["sIdCliente"].ToString();

                        return Json("ok", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if(seleccionarNomina.IdCliente != null)
                        {
                          return Json(seleccionarNomina, JsonRequestBehavior.AllowGet);

                        }

                          return View("Index", seleccionarNomina);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Default");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Default");
            }            
        }
    }
}