using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers.Nomina
{
    public class AusentismosController : BaseController
    {
        // GET: Ausentismos
        //public ActionResult Index()
        //{            
        //    ClassAusentismos ca = new ClassAusentismos();
        //    var model = ca.getModelAusentismos((int)Session["sIdUnidadNegocio"]);
        //    return View(model);
        //}

        public ActionResult Create()
        {
            return View();
        }

        public JsonResult BuscaEmpleado(string clave)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado cemp = new ClassEmpleado();
            var emp = cemp.GetEmpleadosByClave(clave, IdUnidadNegocio).FirstOrDefault();

            if(emp != null)
                return Json(emp, JsonRequestBehavior.AllowGet);
            else
                return Json("El Empleado con la clave que ingreso no existe!", JsonRequestBehavior.AllowGet);
        }
    }
}