using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReportesDirectivosController : BaseController
    {
        // GET: ReportesDirectivos
        public ActionResult Index()
        {
            ClassReportesRirectivos crd = new ClassReportesRirectivos();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ViewBag.LstPeriodos = crd.GetLstPeriodos(IdUnidadNegocio);
            return View();
        }

        [HttpPost]
        public ActionResult Index(string[] LstPeriodos)
        {

            return View();
        }
    }
}