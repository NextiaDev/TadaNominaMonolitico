using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers.Administracion
{
    public class BancosController : Controller
    {
        // GET: Bancos
        /// <summary>
        /// Acción que lista los bancos.
        /// </summary>
        /// <returns>Regresa la vista con el listado de los bancos.</returns>
        public ActionResult Index()
        {
            ClassBancos cb = new ClassBancos();
            var list = cb.getBancos();
            return View(list);
        }

        /// <summary>
        /// Acción que descarga el archivo de los bancos.
        /// </summary>
        /// <returns>Regresa el archivo con los bancos.</returns>
        public JsonResult Descargar()
        {
            ClassBancos cBancos = new ClassBancos();
            DataTable dt = cBancos.GetTableBancos();
                        
            var json = JsonConvert.SerializeObject(dt, Formatting.Indented);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}