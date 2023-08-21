using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class MovimientosSinRespuestaController : BaseController
    {
        // GET: MovimientosSinRespuesta
        public ActionResult Index(string mensaje)
        {
            cMovimientosSinRespuesta cmsr = new cMovimientosSinRespuesta();
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            var listado = cmsr.GetMovimientosSinRspuesta(IdCliete);
            ViewBag.Mensaje = mensaje;
            return View(listado);
        }

        public ActionResult Respuesta()
        {
            cMovimientosSinRespuesta cmsr = new cMovimientosSinRespuesta();
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            string mensaje = cmsr.AddRespuesta(IdCliete);
            return RedirectToAction("Index", new {mensaje = mensaje});
        }
    }
}