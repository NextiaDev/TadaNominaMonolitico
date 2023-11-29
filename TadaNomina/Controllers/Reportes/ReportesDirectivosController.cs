using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReportesDirectivosController : BaseController
    {
        // GET: ReportesDirectivos
        public ActionResult Index()
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ViewBag.LstPeriodos = crd.GetLstPeriodos(IdUnidadNegocio);
            return View();
        }

        [HttpPost]
        public ActionResult Index(string[] LstPeriodos)
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            var datos = crd.GetReporteByPeriodo(LstPeriodos);
            var excel = crd.ExcelByPeriodo(datos);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Periodos.xlsx");
        }

        public ActionResult Entidades()
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ViewBag.LstPeriodos = crd.GetLstPeriodos(IdUnidadNegocio);
            return View();
        }

        [HttpPost]
        public ActionResult Entidades(string[] lstPeriodos)
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            var datos = crd.GetReporteByEntidad(lstPeriodos);
            var excel = crd.ExcelByEntidad(datos);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Entidades.xlsx");
        }

        public ActionResult CC()
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ViewBag.LstPeriodos = crd.GetLstPeriodos(IdUnidadNegocio);
            return View();
        }

        [HttpPost]
        public ActionResult CC(string[] lstPeriodos)
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            var datos = crd.GetReporteByCC(lstPeriodos);
            var excel = crd.ExcelByCC(datos);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CentroCosto.xlsx");
        }

        public ActionResult Patrona()
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ViewBag.LstPeriodos = crd.GetLstPeriodos(IdUnidadNegocio);
            return View();
        }

        [HttpPost]
        public ActionResult Patrona(string[] lstPeriodos)
        {
            ClassReportesDirectivos crd = new ClassReportesDirectivos();
            var datos = crd.GetReporteByPatrona(lstPeriodos);
            var excel = crd.ExcelByPatrona(datos);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Patronas.xlsx");
        }
    }
}