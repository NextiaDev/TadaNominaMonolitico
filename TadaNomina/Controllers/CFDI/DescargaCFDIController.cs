using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.PDF_CFDI;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Controllers.CFDI
{
    public class DescargaCFDIController : BaseController
    {
        // GET: DescargaCFDI
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassDescargaCFDI cDescarga = new ClassDescargaCFDI();
            ModelDescargaCFDI model = cDescarga.GetModel(IdUnidadNegocio);

            return View(model);
        }

        /// <summary>
        ///     Acción que descarga un zip con los CFDI de un cliente
        /// </summary>
        /// <param name="model">Modelo con información para la descarga</param>
        /// <returns>Zip con los CFDI o XML del cliente</returns>
        [HttpPost]
        public ActionResult Index(ModelDescargaCFDI model)
        {   
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ClassDescargaCFDI cDescarga = new ClassDescargaCFDI();
            ModelDescargaCFDI modelo = cDescarga.GetModel(IdUnidad);
            ViewBag.IdPeriodoNomina = model.IdPeriodoNomina;

            ///condigo para borrar. Descarga separado por patrona y por entidad.
            //string path = "C://Timbrado/";
            //ClassProcesosTimbrado cpt = new ClassProcesosTimbrado();
            //CrearXML cxml = new CrearXML();
            //ClassEntidadFederativa ce = new ClassEntidadFederativa();

            //var list = cpt.GetvTimbradosPeriodo();

            //if (!Directory.Exists(path))            
            //    Directory.CreateDirectory(path);

            //List<Cat_EntidadFederativa> listaEntidades = ce.getEntidades();
            
            //foreach (var i in list)
            //{
            //    var entidad = listaEntidades.Where(x => x.Id == i.IdEntidad).Select(x=> x.Nombre).FirstOrDefault() ?? "";

            //    if (!Directory.Exists(path + "/" + i.RFC_Patronal))
            //        Directory.CreateDirectory(path + "/" + i.RFC_Patronal);

            //    if (!Directory.Exists(path + "/" + i.RFC_Patronal + "/" + entidad))
            //        Directory.CreateDirectory(path + "/" + i.RFC_Patronal + "/" + entidad);

            //    cxml.crearXML(i.CFDI_Timbrado, path + "/" + i.RFC_Patronal + "/" + entidad + "/" + i.IdPeriodoNomina + "_" + i.IdEmpleado + "_" + i.RFC + ".xml");
            //}

            try
            {
                ClassDescargaCFDI cd = new ClassDescargaCFDI();

                switch (model.DividirPor)
                {
                    case "General":
                        cd.GetZip(model.IdPeriodoNomina, model.TipoArchivo, IdCliente);
                        break;
                    case "RegistroPatronal":                       
                        cd.GetZipReg(model.IdPeriodoNomina, model.TipoArchivo, IdCliente);
                        break;
                    case "CentroCostos":
                        cd.GetZipCC(model.IdPeriodoNomina, model.TipoArchivo, IdCliente);
                        break;
                }
            
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
                
        public FileResult DownloadExcel(int IdPeriodoNomina)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ClassDescargaCFDI cDescarga = new ClassDescargaCFDI();
            ModelDescargaCFDI modelo = cDescarga.GetModel(IdUnidad);
            ViewBag.IdPeriodoNomina = IdPeriodoNomina;
            try
            {
                cGeneraExcel_CFDI cg = new cGeneraExcel_CFDI();
                var result = cg.generaExcel(IdPeriodoNomina);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(result, "Grid");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);                        
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                modelo.validacion = false;
                modelo.Mensaje = "Los archivos NO se generaron de forma correcta: " + ex.Message;
                byte[] data = Encoding.UTF8.GetBytes(ex.Message);
                return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
            }  
        }

        public ActionResult Download(int IdPeriodoNomina)
        {
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(@"D:\TadaNomina\DescargaCFDINomina\" + IdPeriodoNomina + ".zip");
                string fileName = IdPeriodoNomina + ".zip";

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch(Exception ex) 
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

        public ActionResult DispersarRecibo()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            ClassDescargaCFDI cDescarga = new ClassDescargaCFDI();
            ModelDescargaCFDI model = cDescarga.GetModel(IdUnidadNegocio);

            return View(model);
        }

        [HttpPost]
        public ActionResult DispersarRecibo(ModelDescargaCFDI model)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassDescargaCFDI cDescarga = new ClassDescargaCFDI();

            ModelDescargaCFDI modelo = cDescarga.GetModel(IdUnidadNegocio);
            ClassDescargaCFDI cfdi = new ClassDescargaCFDI();

            try
            {
                cfdi.SendCorreo(model.IdPeriodoNomina, model.TipoArchivo, model.validacion);
                modelo.validacion = true;
                modelo.Mensaje = "Los archivos se enviaron.";
            }
            catch (Exception ex)
            {
                modelo.validacion = false;
                modelo.Mensaje = "Los archivos NO se generaron de forma correcta: " + ex.Message;
                throw;
            }

            model = cfdi.GetModel(IdUnidadNegocio);

            if (model.validacion == false)
            {
                model.Mensaje = "Se enviaron correctamente";
            }

            return View(modelo);
        }
    }
}
