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
    public class SantanderController : BaseController
    {
        // GET: Santander
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SantanderMismo(ModelSantander ms)
        {
            try
            {
                int IdCliente = 0;
                int IdUnidadNegocio = 0;
                try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
                try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

                int IdPeriodoNomina = int.Parse(ms.IdPeriodoNomina);
                ClassSantander cs = new ClassSantander();
                var model = cs.GeneraTxtSantander(IdPeriodoNomina, IdUnidadNegocio);
                byte[] bytes = Encoding.ASCII.GetBytes(model);
                string nombrePerido = cs.GetNombrePeriodoNomina(ms.IdPeriodoNomina);

                return File(bytes, "text/plain", nombrePerido + ".txt");
            }
            catch (Exception ex)
            {

                return RedirectToAction("GetPeriodoNomina", "Santander", new { mensajeError = ex.Message });
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
                ClassSantander cs = new ClassSantander();
                ModelSantander ms = new ModelSantander();
                ms.PeriodosNomina = cs.GetPeriodosN(IdUnidadNegocio);

                return View(ms);
            }
        }

        public ActionResult SantanderInter(ModelSantander ms)
        {
            int IdCliente = 0;
            int IdUnidadNegocio = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            try
            {
                int IdPeriodoNomina = int.Parse(ms.IdPeriodoNomina);
                ClassSantander cs = new ClassSantander();
                var model = cs.GeneraTxtSantanderInter(IdPeriodoNomina, IdUnidadNegocio);
                byte[] bytes = Encoding.ASCII.GetBytes(model);
                string nombrePeriodo = cs.GetNombrePeriodoNomina(ms.IdPeriodoNomina);

                return File(bytes, "text/plain", nombrePeriodo + ".txt");
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return RedirectToAction("GetPeriodoNomina", "Santander", new { mensajeError = mensaje });
            }
        }
    }
}