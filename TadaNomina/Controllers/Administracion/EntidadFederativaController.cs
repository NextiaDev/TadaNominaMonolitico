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
    public class EntidadFederativaController : BaseController
    {
        // GET: EntidadFederativa
        /// <summary>
        /// Acción que lista las entidades.
        /// </summary>
        /// <returns>Regresa la vista con las entidades.</returns>
        public ActionResult Index()
        {
            ClassEntidadFederativa cef = new ClassEntidadFederativa();
            var list = cef.getEntidades();
            return View(list);
        }

        /// <summary>
        /// Acción que descarga las entidades.
        /// </summary>
        /// <returns>Regresa un Json con la información.</returns>
        public JsonResult Descargar()
        {
            ClassEntidadFederativa cBancos = new ClassEntidadFederativa();
            DataTable dt = cBancos.GetTableEntidades();

            var json = JsonConvert.SerializeObject(dt, Formatting.Indented);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}