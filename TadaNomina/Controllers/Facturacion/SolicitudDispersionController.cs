using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Controllers.Facturacion
{
    public class SolicitudDispersionController : BaseController
    {
        // GET: SolicitudDispersion
        public ActionResult Index()
        {
            var csf = new ClassSolicitudDispersion();
            var lista = csf.listar();
            return View(lista);
        }

        public ActionResult Solicitud(string _ID, string _Desc)
        {
            var msf = new ModelSolicitudDispersion();
            msf.Periodo = Statics.DesEncriptar(_Desc);
            var csf = new ClassSolicitudFacturas();

            ViewBag.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            msf.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            ViewBag.listaRS = csf.ComboRSFacturacion();            
            ViewBag.listaFacturadoras = csf.ComboFacturadoras();
            ViewBag.listaConceptos = csf.ComboConceptos();

            var costeo = csf.listarArchivos(int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString())));
            msf.IdClienteRazonSocialFacturacion = costeo.IdClienteRazonSocialFacturacion;
            msf.IdEmpresaFacturadora = costeo.IdEmpresaFacturadora;
            
            if (msf.IdEmpresaFacturadora != null)
            {
                ViewBag.listaConceptos = csf.ComboConceptos((int)msf.IdEmpresaFacturadora);
                msf.IdConceptoFacturacion = costeo.IdConceptoFacturacion;
            }

            return View(msf);
        }

        [HttpPost]
        public ActionResult Solicitud(ModelSolicitudDispersion msf)
        {
            try
            {
                var csf = new ClassSolicitudDispersion();

                string carpeta = msf.IdfacturasContabilidad.ToString();
                string ruta = Statics.rutaGralArchivos + @"ArchivosTesoreria\" + carpeta;
                List<string> archivosAdj = new List<string>();

                foreach (string filename in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[filename];  
                    if (file != null && file.ContentLength > 0)
                    {
                        archivosAdj.Add(file.FileName);
                        if (!Directory.Exists(ruta))
                        {
                            Directory.CreateDirectory(ruta);
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                        }
                        else
                        {
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                        }
                    }
                }

                var solicitud = csf.RegistraSolicitudD(msf);

                if (solicitud == true)
                {
                    string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;

                    var cliente = Session["sCliente"].ToString();
                    var unidad = Session["sNomina"].ToString();

                    ClassNotificaciones n = new ClassNotificaciones();
                    ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                    var _unidad = cun.getUnidadesnegocioId((int)Session["sIdUnidadNegocio"]);
                    var confidencial = _unidad.Confidencial;
                    string Correos = n.ExtraeCorreos("DISPERSION", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], confidencial);
                    string archivosAdjCorreo = archivosAdj.Count > 0 ? "Se adjuntaron los siguientes archivos: " + string.Join("|", archivosAdj) : "";

                    Message = string.Empty;
                    Message += "Tienes una solicitud de dispersion del cliente " + cliente + " - " + unidad + " - " + msf.Periodo + " <br /> " + archivosAdjCorreo;
                    Titulo = "Solicitud de Dispersion " + cliente + " - " + unidad;
                    Modulo = "Dispersion";
                    n.EnviarCorreo(Message, Titulo, Correos, Session["sCorreo"].ToString(), Modulo);

                    ViewBag.ShowDialog = true;
                } 
            }
            catch (Exception ex)
            {

                ViewBag.ShowDialog = false;
                ViewBag.Mensaje = ex.Message;
            }

            return View();

        }

        [HttpPost]
        public ActionResult Eliminar(ModelSolicitudFacturas msf)
        {
            var csf = new ClassSolicitudDispersion();

            msf.IdEstatus = 5;
            msf.IdCancelacion = (int)Session["sIdUsuario"];

            csf.Eliminar(msf);

            return RedirectToAction("Index");
        }

        public JsonResult Conceptos(int IdFacturadora)
        {

            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            List<SelectListItem> lst = new List<SelectListItem>();

            lst = csf.ComboConceptos(IdFacturadora);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerCosteo(string _ID, string _PA)
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();


            int IdFacturaContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));


            var cadena = csf.TraeCosteo(IdFacturaContabilidad);

            ViewBag.Costeo = cadena.Replace("col-md-6 col-md-offset-3", "col-md-12").Replace("col-md-6", "col-md-12"); ;

            ViewBag.Principal = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_PA.ToString()));



            return View();
        }

        public ActionResult VerCosteoRechazado(string _ID, string _PA)
        {
            var csf = new ClassSolicitudDispersion();

            int IdFacturaContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            var cadena = csf.getSolicitud(IdFacturaContabilidad);

            ViewBag.Costeo = cadena.Costeo_HTML.Replace("col-md-6 col-md-offset-3", "col-md-12").Replace("col-md-6", "col-md-12"); ;

            ViewBag.Principal = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_PA.ToString()));

            return View(cadena);
        }

        public ActionResult VerCosteoFinalizado(string _ID, string _PA)
        {
            var csf = new ClassSolicitudDispersion();

            int IdFacturaContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            var cadena = csf.getSolicitud(IdFacturaContabilidad);

            ViewBag.Costeo = cadena.Costeo_HTML.Replace("col-md-6 col-md-offset-3", "col-md-12").Replace("col-md-6", "col-md-12"); ;

            ViewBag.Principal = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_PA.ToString()));

            return View(cadena);
        }

        public ActionResult DescargaComprobanteCEP(int IdFacturasContabilidad)
        {
            var ccosteos = new ClassSolicitudDispersion();
            var ruta = ccosteos.getSolicitud(IdFacturasContabilidad);
            string nombreArch = Path.GetFileName(ruta.CmprobanteDispersion);
            byte[] fileBytes = System.IO.File.ReadAllBytes(ruta.CmprobanteDispersion);
            string fileName = nombreArch;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export_Excel(string GridHtml)
        {


            return File(Encoding.Default.GetBytes(GridHtml), "application/vnd.ms-excel", "CosteoExcel.xls");
        }

        public ActionResult SFinalizadas()
        {
            var csf = new ClassSolicitudDispersion();
            var lista = csf.getSolicitudesFinalizadas((int)Session["sIdUnidadNegocio"]);
            return View(lista);
        }

        public ActionResult SRechazadas()
        {
            var csf = new ClassSolicitudDispersion();
            var lista = csf.getSolicitudesRechazadas((int)Session["sIdUnidadNegocio"]);
            return View(lista);
        }

        public ActionResult DescargarArchivos(int IdFacturasContabilidad)
        {
            try
            {
                var ccosteos = new ClassSolicitudDispersion();
                var rutas = ccosteos.getSolicitud(IdFacturasContabilidad);
                string ruta = Statics.rutaGralArchivos + @"ArchCargadosSolicitudDispersion\ZIP";

                if (rutas.AdjuntarComprobante != null || rutas.ArchivoBancos != null || rutas.ComprobanteAutorizacion != null)
                {
                    if (!Directory.Exists(ruta))
                    {
                        Directory.CreateDirectory(ruta);
                    }

                    List<string> files = new List<string>();

                    try
                    {
                        files.Add(rutas.AdjuntarComprobante);
                        string nombreArch = Path.GetFileName(rutas.AdjuntarComprobante);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(rutas.AdjuntarComprobante);
                        using (FileStream file = new FileStream(ruta + @"\" + nombreArch, FileMode.Create))
                        {
                            file.Write(fileBytes, 0, fileBytes.Length);

                            file.Close();
                        }
                    }
                    catch { }

                    try
                    {
                        files.Add(rutas.ArchivoBancos);
                        string nombreArch = Path.GetFileName(rutas.ArchivoBancos);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(rutas.ArchivoBancos);
                        using (FileStream file = new FileStream(ruta + @"\" + nombreArch, FileMode.Create))
                        {
                            file.Write(fileBytes, 0, fileBytes.Length);

                            file.Close();
                        }
                    }catch { }

                    try
                    {
                        files.Add(rutas.ComprobanteAutorizacion);
                        string nombreArch = Path.GetFileName(rutas.ComprobanteAutorizacion);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(rutas.ComprobanteAutorizacion);
                        using (FileStream file = new FileStream(ruta + @"\" + nombreArch, FileMode.Create))
                        {
                            file.Write(fileBytes, 0, fileBytes.Length);

                            file.Close();
                        }
                    } catch { }

                    if (files.Count() > 0)
                    {
                        GetZipComprobante(files, IdFacturasContabilidad.ToString());

                        byte[] fileByte = System.IO.File.ReadAllBytes(Statics.rutaGralArchivos + @"ArchCargadosSolicitudDispersion\" + IdFacturasContabilidad.ToString() + ".zip");
                        return File(fileByte, System.Net.Mime.MediaTypeNames.Application.Octet, IdFacturasContabilidad.ToString() + ".zip");
                    }
                    else
                        throw new Exception("No se pudieron crear los archivos.");
                }
                else
                    throw new Exception("No se pudieron crear los archivos.");
            }
            catch 
            {
                return RedirectToAction("Index");
            }
        }

        public void GetZipComprobante(List<string> lista, string nombre)
        {
            string rutaZIP = Statics.rutaGralArchivos + @"ArchCargadosSolicitudDispersion\" + nombre + ".zip";
            if (Directory.Exists(rutaZIP))
            {
                Directory.Delete(rutaZIP, true);
            }

            if (lista.Count > 0)
            {
                CreateZip(lista, rutaZIP);
            }
        }

        public void CreateZip(List<string> items, string destino)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                foreach (string item in items)
                {
                    if (System.IO.File.Exists(item))
                    {
                        zip.AddFile(item, "");
                    }
                    else if (System.IO.Directory.Exists(item))
                    {
                        zip.AddDirectory(item, new System.IO.DirectoryInfo(item).Name);
                    }
                }
                zip.Save(destino);
            }
        }
    }
}