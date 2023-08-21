using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.LayoutB;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.LayoutBancos;
using TadaNomina.Services;

namespace TadaNomina.Controllers.LayoutBancos
{
    public class BancomerController : BaseController
    {
        // GET: Bancomer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MismoBanco(ModelBancomer mb, int IdPeriodoNomina, int IdUnidadNegocio)
        {
            try
            {
                int IdCliente = 0;
                try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
                try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

                int IdPeridoNomina = int.Parse(mb.IdPeriodoNomina);
                ClassBancomer cb = new ClassBancomer();
                var model = cb.GeneraTxtBBVA(IdPeridoNomina, IdUnidadNegocio);
                byte[] bytes = Encoding.ASCII.GetBytes(model);
                string nombrePeriodo = cb.GetNombrePeriodoNomina(mb.IdPeriodoNomina);

                return File(bytes, "text/plain", nombrePeriodo + ".txt");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GetPeriodoNomina", "Santander", new { mensajeError = ex.Message });
            }
        }

        public ActionResult GetPeriodoNomina()
        {
            int IdCliente = 0;
            int IdUnidadNegocio = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdCliente == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassBancomer cb = new ClassBancomer();

                ModelBancomer mb = new ModelBancomer();
                mb.PeriodosN = cb.GetPeriodosN(IdUnidadNegocio);

                return View(mb);
            }
        }

        public ActionResult BancomerInter(ModelBancomer mb)
        {
            string model = string.Empty;
            int IdCliente = 0;
            int IdUnidadNegocio = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            int IdPeridoNomina = int.Parse(mb.IdPeriodoNomina);
            ClassBancomer cb = new ClassBancomer();
            try
            {
                model = cb.GeneraTxtBBVAInterbancario(IdPeridoNomina, IdUnidadNegocio);
                byte[] bytes = Encoding.ASCII.GetBytes(model);
                string nombrePeriodo = cb.GetNombrePeriodoNomina(mb.IdPeriodoNomina);

                return File(bytes, "text/plain", nombrePeriodo + ".txt");
            }
            catch
            {
                string Mensaje = "No existen empleados para Interbancario";
                return RedirectToAction("GetPeriodoNomina", "Santander", new { mensajeError = Mensaje });
            }
        }
    }
}