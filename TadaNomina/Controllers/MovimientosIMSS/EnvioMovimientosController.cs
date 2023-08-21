using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class EnvioMovimientosController : BaseController
    {

        // GET: EnvioMovimientos
        public ActionResult Index(string mensaje)
        {
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            cEnvioMovimientos cem = new cEnvioMovimientos();
            var listado = cem.GetMovimientos(IdCliete);
            ViewBag.Mensaje = mensaje;
            return View(listado);
        }

        public ActionResult EnviarMov()
        {
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            cEnvioMovimientos cem = new cEnvioMovimientos();
            var listado = cem.GetMovimientos(IdCliete);
            string mensaje = cem.EnviarMov(listado);
            if(mensaje==null)
            {
                return RedirectToAction("MovEnviados");
            }
            else
            {
                return RedirectToAction("Index", new {mensaje = mensaje});
            }
        }

        public ActionResult MovEnviados()
        {
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            mMovimientosEnviados model = new mMovimientosEnviados();
            cEnvioMovimientos movdia = new cEnvioMovimientos();
            model.errores = movdia.MovimientosError(IdCliete);
            model.correctos = movdia.MovimientosCorrectos(IdCliete);
            return View(model);
        }
    }
}