using System;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.PDF_CFDI;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Controllers.CFDI
{
    public class DescargaCFDIxRegistroController : BaseController
    {
        // GET: DescargaCFDIxRegistro
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ClassDescargaCFDIxRegistro cDescarga = new ClassDescargaCFDIxRegistro();
            ModelDescargaCFDI model = cDescarga.GetModel(IdUnidadNegocio);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ModelDescargaCFDI model)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ClassDescargaCFDIxRegistro cDescarga = new ClassDescargaCFDIxRegistro();
            ModelDescargaCFDI modelo = cDescarga.GetModel(IdUnidad);
            ViewBag.IdPeriodoNomina = model.IdPeriodoNomina;
            try
            {
                ClassDescargaCFDIxRegistro cd = new ClassDescargaCFDIxRegistro();
                cd.GetZip(model.IdPeriodoNomina, model.TipoArchivo, IdUnidad);

                modelo.validacion = true;
                modelo.Mensaje = "Los archivos se generaron de forma correcta.";
            }
            catch (Exception ex)
            {
                modelo.validacion = false;
                modelo.Mensaje = "Los archivos NO se generaron de forma correcta: " + ex.Message;
            }

            return View(modelo);

        }

        public ActionResult Download(int IdPeriodoNomina)
        {
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(@"D:\TadaNomina\DescargaCFDINomina\" + IdPeriodoNomina + ".zip");
                string fileName = IdPeriodoNomina + ".zip";

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                ClassDescargaCFDI cDescarga = new ClassDescargaCFDI();
                ModelDescargaCFDI modelo = cDescarga.GetModel(IdUnidad);
                ViewBag.IdPeriodoNomina = IdPeriodoNomina;

                modelo.validacion = false;
                modelo.Mensaje = "No se pudo descargar el archivo: " + ex.Message;

                return View("Index", modelo);
            }
        }
    }
}