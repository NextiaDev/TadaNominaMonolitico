using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Controllers.Facturacion
{
    public class ArchivosAltasController : BaseController
    {
        // GET: ArchivosAltas
        public ActionResult Index()
        {
            var caa = new ClassArchivosAltas();
            var model = caa.getArchivosAltas((int)Session["sIdUnidadNegocio"]);
            return View(model);
        }

        public ActionResult ArchivoAltas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ArchivoAltas(ModelArchivoAltas model)
        {
            try
            {
                var caa = new ClassArchivosAltas();
                var Id = caa.newArchivoAltas((int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], model.file.FileName, model.Observaciones, (int)Session["sIdUsuario"]);
                caa.guardarArchivo(Id, model.file);

                string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;
                ClassNotificaciones n = new ClassNotificaciones();
                string Correos = n.ExtraeCorreos("ALTAS", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], null);
                Message = string.Empty;
                Message += "Tienes un nuevo archivo de altas del cliente " + Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();
                Titulo = "Archivo Altas " + Session["sCliente"].ToString() + " - " + Session["sNomina"].ToString();

                Modulo = "ALTAS";
                n.EnviarCorreo(Message, Titulo, Correos, Modulo);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View(model);
            }            
        }

        public ActionResult Descargar(int IdArchivoAltas)
        {
            var caa = new ClassArchivosAltas();
            var reg = caa.getArchivoAltas(IdArchivoAltas);
            var arch = caa.getFile(IdArchivoAltas, reg);

            return File(arch, System.Net.Mime.MediaTypeNames.Application.Octet, reg.Ruta);
        }
    }
}