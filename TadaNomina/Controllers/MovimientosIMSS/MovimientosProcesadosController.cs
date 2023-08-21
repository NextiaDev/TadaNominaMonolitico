using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class MovimientosProcesadosController : BaseController
    {
        // GET: MovimientosProcesados
        public ActionResult Index(string mensaje)
        {
            cMovimientosProcesados cmp = new cMovimientosProcesados();
            var modelo = new mMovimientosProcesados();
            modelo.ListaTipoMov = cmp.GetTiposMovientos();
            ViewBag.Mensaje = mensaje;
            return View(modelo);
        }

        [HttpPost]
        public ActionResult ListadoMovimientos(mMovimientosProcesados model)
        {
            cMovimientosProcesados cmp = new cMovimientosProcesados();
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            var listado = cmp.GetMovimientosProcesados(model, IdCliete);
            if (listado.Count > 0)
            {
                return View(listado);
            }
            else
            {
                string e = "Sin movimientos enviados";
                return RedirectToAction("Index", new { mensaje = e });
            }
        }
    }
}