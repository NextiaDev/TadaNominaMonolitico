using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.LayoutB;
using TadaNomina.Models.ViewModels.LayoutBancos;

namespace TadaNomina.Controllers.LayoutBancos
{
    public class ArchivotxtDispersionController : BaseController
    {

        private readonly cArchivosDispersion cad = new cArchivosDispersion();

        public ActionResult Index()
        {
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            mArchivoDispersion model = cad.getArchivoDispersion(IdUnidadNegocio, IdCliente);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(mArchivoDispersion model)
        {
            try
            {
                ClassSantander cs = new ClassSantander();
                int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
                byte[] bytes = cad.getTxt(model, IdUnidadNegocio);
                string fechaHoy = DateTime.Now.ToString("dd-MM-yy").Replace("-", "");

                FileContentResult result;
                switch (model.IdBanco)
                {
                    case 18:
                        result = File(bytes, "text/plain", model.IdPeriodoNomina + ".pag");
                        break;
                    default:
                        result = File(bytes, "text/plain", model.IdPeriodoNomina + ".txt");
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                int IdCliente = int.Parse(Session["sIdCliente"].ToString());
                int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
                mArchivoDispersion m = cad.getArchivoDispersion(IdUnidadNegocio, IdCliente);

                ViewBag.Mensaje = ex.Message;
                return View(m);
            }
        }

        public JsonResult getValidacionBanco(int IdBanco)
        {
            string resultado = cad.getValidaciónBanco(IdBanco);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}