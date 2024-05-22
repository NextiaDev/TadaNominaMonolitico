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
                
                cLog cr = new cLog();                
                Statics.CreateZipFile(Statics.rutaGralArchivos + "Logs");
                var result = System.IO.File.ReadAllBytes(Statics.rutaGralArchivos + "Logs.zip");
                return File(result, "text/plain", Statics.rutaGralArchivos + "Logs.zip");
            }
            catch (Exception)
            {
                var file = new byte[0];
                return File(file, "text/plain", "error");
            }
        }
    }
}
