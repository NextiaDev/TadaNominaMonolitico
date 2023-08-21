using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Controllers.Administracion
{
    public class TipoAusentismoController : Controller
    {
        // GET: TipoAusentismo
        /// <summary>
        /// Acción que muestra los ausentismos.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}