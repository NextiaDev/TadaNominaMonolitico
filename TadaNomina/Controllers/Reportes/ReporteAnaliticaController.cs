using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteAnaliticaController : Controller
    {        
        // GET: ReporteAnalitica
        public ActionResult Index()
        {
            try
            {
                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                cReportesAnalitica cr = new cReportesAnalitica();
                ModelAnalitica m = cr.GetAnaliticaByIdCliente(IdCliente);

                if (m.Analitica != null)
                    return View(m);

                return RedirectToAction("Index", "Index");
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}