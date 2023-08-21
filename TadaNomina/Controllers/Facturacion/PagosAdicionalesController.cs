using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Controllers.Facturacion
{
    public class PagosAdicionalesController : BaseController
    {
        // GET: PagosAdicionales
        public ActionResult Index()
        {
            var cpa = new ClassPagosAdicionales();
            var list = cpa.getPagosAdicionales((int)Session["sIdUnidadNegocio"]);
            return View(list);
        }

        public ActionResult Finalizados()
        {
            var cpa = new ClassPagosAdicionales();
            var list = cpa.getPagosAdicionalesFinalizados((int)Session["sIdUnidadNegocio"]);
            return View(list);
        }

        public ActionResult Rechazados()
        {
            var cpa = new ClassPagosAdicionales();
            var list = cpa.getPagosAdicionalesRechazados((int)Session["sIdUnidadNegocio"]);
            return View(list);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(ModelNewPagoAdicional model)
        {
            var cp = new ClassPagosAdicionales();           

            try
            {
                string archivo = string.Empty;
                if(model.Archivo != null)
                    archivo = model.Archivo.FileName;

                var Id = cp.NewPagoAdicional((int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], model.Comentarios, archivo, (int)Session["sIdUsuario"]);

                if (model.Archivo != null)
                    cp.guardarArchivo(Id, model.Archivo);

                string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;
                ClassNotificaciones n = new ClassNotificaciones();
                string Correos = n.ExtraeCorreos("ADICIONALES", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], null);
                Message = string.Empty;
                Message += "Tienes una nueva solicitud de Pago Adicional " + Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();
                Titulo = "Pago Adicional " + Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();

                Modulo = "ADICIONALES";
                n.EnviarCorreo(Message, Titulo, Correos, Modulo);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View(model);
            }
        }

        public ActionResult Descargar(int IdPagoAdicional)
        {
            var cadicionales = new ClassPagosAdicionales();
            var ruta = cadicionales.getPagoAdicional(IdPagoAdicional);
            string nombreArch = Path.GetFileName(ruta.Archivo);
            byte[] fileBytes = System.IO.File.ReadAllBytes(Statics.rutaGralArchivos + @"PagosAdicionales\" + IdPagoAdicional + @"\" + ruta.Archivo);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArch);
        }
    }
}