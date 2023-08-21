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
    public class SolicitudChequesController : BaseController
    {
        // GET: SolicitudCheques
        public ActionResult Index()
        {
            var csc = new ClassSolicitudCheques();
            var list = csc.getSolicitudesActivas((int)Session["sIdUnidadNegocio"]);
            return View(list);
        }

        public ActionResult GetFinalizados()
        {
            var csc = new ClassSolicitudCheques();
            var list = csc.getSolicitudesFinalizadas((int)Session["sIdUnidadNegocio"]);
            return View(list);
        }

        public ActionResult GetRechazados()
        {
            var csc = new ClassSolicitudCheques();
            var list = csc.getSolicitudesRechazadas((int)Session["sIdUnidadNegocio"]);
            return View(list);
        }

        public ActionResult New()
        {
            var model = new mNewSolicitudCheque();
            var cp = new ClassPeriodoNomina();
            model.lPeriodos = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult New(mNewSolicitudCheque model)
        {              
            var cp = new ClassPeriodoNomina();
            model.lPeriodos = cp.GetSeleccionAllPeriodo((int)Session["sIdUnidadNegocio"]).OrderByDescending(x => x.Value).Take(10).ToList();

            try
            {
                var cs = new ClassSolicitudCheques();
                var Id = cs.newSolicitudCheque(model.IdPeriodoNomina, model.archivo.FileName, model.Observaciones, (int)Session["sIdUsuario"]);
                cs.guardarArchivo(Id, model.archivo);

                string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;
                ClassNotificaciones n = new ClassNotificaciones();
                string Correos = n.ExtraeCorreos("CHEQUES", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], null);
                Message = string.Empty;
                Message += "Tienes una nueva solicitud de cheques del cliente " + Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();
                Titulo = "Solicitud Cheques " + Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();

                Modulo = "CHEQUES";
                n.EnviarCorreo(Message, Titulo, Correos, Modulo);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View(model);
            }            
        }

        public ActionResult Descargar(int IdSolicitudCheque)
        {
            var ccheques = new ClassSolicitudCheques();
            var ruta = ccheques.getSolicitud(IdSolicitudCheque);
            string nombreArch = Path.GetFileName(ruta.rutaArchivo);
            byte[] fileBytes = System.IO.File.ReadAllBytes(Statics.rutaGralArchivos + @"SolicitdCheques\" + IdSolicitudCheque + @"\" + ruta.rutaArchivo);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArch);
        }
    }
}