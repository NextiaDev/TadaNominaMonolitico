using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers.Administracion
{
    public class ComunicadosController : BaseController
    {
        // GET: Comunicados
        /// <summary>
        /// Acción que muestra los comunicados de una unidad de negocio.
        /// </summary>
        /// <returns>Regresa la vista con ls comunicados de la unidad de negocio.</returns>
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassComunicados cc = new ClassComunicados();
            var com = cc.getComunicados(IdUnidadNegocio);

            return View(com);
        }


    }
}