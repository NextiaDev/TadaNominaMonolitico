using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Controllers
{
    public class IndexController : BaseController
    {
        // GET: Index
        public ActionResult Index()
        {
            var model = new ModelInicio();
            try
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassInicio ci = new ClassInicio();
                model = ci.getInfoInicio(IdUnidadNegocio);

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Default", new { mensaje = ex.Message });
            }
        }
    }
}