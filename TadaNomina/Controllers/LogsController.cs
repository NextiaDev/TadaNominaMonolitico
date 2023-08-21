using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers
{
    public class LogsController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Logs(DateTime? date)
        {
            try
            {
                if (date == null) { throw new Exception("Debe capturar una fecha para poder mostrar los Logs"); }

                cLog cl = new cLog();
                var lg = cl.getLog((DateTime)date);
                return Json(new { result = "Ok", log = lg });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult getReporte(DateTime? _date)
        {
            try
            {
                if (_date == null) { throw new Exception("Debe capturar una fecha para poder descargar los Logs"); }
                cLog cr = new cLog();
                var result = cr.getLogBytes((DateTime)_date);
                var _date_ = (DateTime)_date;
                var name = "log_" + _date_.Year + "_" + _date_.Month + "_" + _date_.Day + ".txt";
                return File(result, "text/plain", name);
            }
            catch (Exception)
            {
                var file = new byte[0];
                return File(file, "text/plain", "error");
            }
        }
    }
}