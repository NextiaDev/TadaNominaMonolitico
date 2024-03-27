using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;
using TadaNomina.Models.ViewModels.MovimientosIMSS;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class EnvioMovimientosController : BaseController
    {

        // GET: EnvioMovimientos
        public ActionResult Index(string mensaje)
        {
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            cEnvioMovimientos cem = new cEnvioMovimientos();
            var listado = cem.GetMovimientosCambios(IdCliete);
            ViewBag.Mensaje = mensaje;
            return View(listado);
        }

        [HttpPost]
        public JsonResult RecibirEmpleados(RecibirEmpleadoModel model)
        {
            int IdCliete = int.Parse(Session["sIdCliente"].ToString());
            cEnvioMovimientos cem = new cEnvioMovimientos();
            var spresult = cem.GetMovimientos(IdCliete);

            var listempleados = spresult.Where(x => model.idempleado.Contains(x.IdEmpleado)).ToList();
            string mensaje = cem.EnviarMov(listempleados);
            if (mensaje == null)
            {

                return Json("OK", JsonRequestBehavior.AllowGet);


            }
            else
            {
                return Json(mensaje, JsonRequestBehavior.AllowGet);

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