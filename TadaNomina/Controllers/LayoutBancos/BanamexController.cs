using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.LayoutB;
using TadaNomina.Models.ViewModels.LayoutBancos;

namespace TadaNomina.Controllers.LayoutBancos
{
    public class BanamexController : Controller
    {
        // GET: Banamex
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BanamexTxt(ModelBanamex mb, string NumCliente, string ClvSucursal, string RefNum, string RefAlfaNum, string NombreEmpresa)
        {
            try
            {
                int IdCliente = 0;
                int IdUnidadNegocio = 0;
                try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
                try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

                int IdPeriodoNomina = int.Parse(mb.IdPeriodoNomina);
                ClassBanamex cb = new ClassBanamex();
                var model = cb.GeneraTxtBanamex(IdPeriodoNomina, IdUnidadNegocio, NumCliente, ClvSucursal, RefNum, RefAlfaNum, NombreEmpresa);
                byte[] bytes = Encoding.ASCII.GetBytes(model);
                string nombrePerido = cb.GetNombrePeriodoNomina(mb.IdPeriodoNomina);

                return File(bytes, "text/plain", nombrePerido + ".txt");
            }
            catch (Exception ex)
            {

                return RedirectToAction("GetPeriodoNomina", "Banamex", new { mensajeError = ex.Message });
            }
        }

        public ActionResult GetPeriodoNomina(string mensajeError)
        {
            ViewBag.mensaje = mensajeError;
            int IdCliente = 0;
            int IdUnidadNegocio = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdCliente == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassBanamex cb = new ClassBanamex();
                ModelBanamex mb = new ModelBanamex();
                mb.PeriodosNomina = cb.GetPeriodosN(IdUnidadNegocio);

                return View(mb);
            }
        }
    }
}