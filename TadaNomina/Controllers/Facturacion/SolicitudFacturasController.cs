using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Controllers.Facturacion
{
    public class SolicitudFacturasController : BaseController
    {
        // GET: SolicitudFacturas
        public ActionResult Index()
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            return View(csf.listar());
        }

        public ActionResult SFinalizadas()
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

         

            return View(csf.listarfinalizadas());
        }

        public ActionResult NewSolicitud()
        {
            var model = new ModelNewSolicitud();
            GetListas(model);
            return View(model);
        }

        //[HttpPost]
        //public ActionResult NewSolicitud(ModelNewSolicitud model)
        //{
        //    GetListas(model);

        //    try
        //    {
        //        if (model.Subtotal > 0 && model.IVA > 0 && model.Total > 0)
        //        {
        //            var csf = new ClassSolicitudFacturas();
        //            csf.newSolicitud((int)Session["sIdCliente"], Session["sCliente"].ToString(), (int)Session["sIdUnidadNegocio"], Session["sNomina"].ToString(), model.PeriodoNomina, model.Esquema, model.Importe, model.Honorario, model.Subtotal, model.IVA, model.Total, (int)Session["sIdUsuario"]);

        //            return RedirectToAction("Index");
        //        }
        //        else
        //            throw new Exception("Los campos Subtotal, IVA y Total no pueden ser igual a cero.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Mensaje = "Error: " + ex.Message;                
        //    }

        //    return View(model);
        //}

        [HttpPost]
        public JsonResult NuevaSolicitud(string _PeriodoNomina, string _Esquema, string _Importe, string _Honorario, string _Subtotal, string _IVA, string _Total)
        {
            //GetListas(model);
            string resultado = string.Empty;
            try
            {
                if (decimal.Parse(_Subtotal) > 0 && decimal.Parse(_IVA) > 0 && decimal.Parse(_Total) > 0)
                {
                    var csf = new ClassSolicitudFacturas();
                    csf.newSolicitud((int)Session["sIdCliente"], Session["sCliente"].ToString(), (int)Session["sIdUnidadNegocio"], Session["sNomina"].ToString(), _PeriodoNomina, _Esquema, decimal.Parse(_Importe), decimal.Parse(_Honorario), decimal.Parse(_Subtotal), decimal.Parse(_IVA), decimal.Parse(_Total), (int)Session["sIdUsuario"]);

                    resultado = "CORRECTO: Se creo la solicitud correctamente";
                    return Json(resultado, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resultado = "ERROR: No se pudo crear la solicitud";
                    return Json(resultado, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: " + ex.Message;
            }

            resultado = "ERROR: No se pudo crear la solicitud";
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        private static void GetListas(ModelNewSolicitud model)
        {
            model.lEsquema = new List<SelectListItem>();
            model.lEsquema.Clear();
            model.lEsquema.Add(new SelectListItem { Text = "AMBOS" });
            model.lEsquema.Add(new SelectListItem { Text = "TRADICIONAL" });
            model.lEsquema.Add(new SelectListItem { Text = "ESQUEMA" });            
        }

        public ActionResult Solicitud(string _ID)
        {
            ModelSolicitudFacturas msf = new ModelSolicitudFacturas();

            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            ViewBag.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            msf.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            ViewBag.listaRS = csf.ComboRSFacturacion();
            ViewBag.listaTP = csf.ComboTipoPago();
            ViewBag.listaMP = csf.ComboMetodoPago();
            ViewBag.listaCFDI = csf.ComboCFDI();
            ViewBag.listaFacturadoras = csf.ComboFacturadoras();
            ViewBag.listaConceptos = csf.ComboConceptos();

            var costeo = csf.listarArchivos(int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString())));
            msf.IdClienteRazonSocialFacturacion = costeo.IdClienteRazonSocialFacturacion;
            msf.IdEmpresaFacturadora = costeo.IdEmpresaFacturadora;
            msf.ArchivoBancos = costeo.ArchivoBancos;
            msf.Comprobante = costeo.Comprobante;

            msf.Total = costeo.Total;

            if (msf.IdEmpresaFacturadora != null)
            {
                ViewBag.listaConceptos = csf.ComboConceptos((int)msf.IdEmpresaFacturadora);
                msf.IdConceptoFacturacion = costeo.IdConceptoFacturacion;
            }

            return View(msf);
        }


        [HttpPost]

        public ActionResult Solicitud(ModelSolicitudFacturas msf)
        {
            try
            {
                ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

                if (ModelState.IsValid && msf.validacion == 1)
                {
                    string carpeta = msf.IdfacturasContabilidad.ToString();
                    string ruta = Statics.rutaGralArchivos + @"ArchivosFactura\" + carpeta;

                    foreach (string filename in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[filename];


                        if (file != null && file.ContentLength > 0)
                        {
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

                    var solicitud = csf.RegistraSolicitud(msf);

                    if (solicitud == true)
                    {
                        ViewBag.ShowDialog = true;

                        string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;

                        var cliente = Session["sCliente"].ToString();
                        var unidad = Session["sNomina"].ToString();

                        ClassNotificaciones n = new ClassNotificaciones();

                        string Correos = n.ExtraeCorreos("FACTURACION", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"], null);

                        Message = "Tienes una solicitud de factura del cliente " + cliente + " - " + unidad;
                        Titulo = "Solicitud de factura " + cliente + " - " + unidad;

                        Modulo = "Facturación";
                        n.EnviarCorreo(Message, Titulo, Correos, Modulo);

                    }

                    



                }
                else
                {
                    ViewBag.listaRS = csf.ComboRSFacturacion();
                    ViewBag.listaTP = csf.ComboTipoPago();
                    ViewBag.listaMP = csf.ComboMetodoPago();
                    ViewBag.listaCFDI = csf.ComboCFDI();
                    ViewBag.listaFacturadoras = csf.ComboFacturadoras();
                    ViewBag.listaConceptos = csf.ComboConceptos();
                    ViewBag.IdfacturasContabilidad = msf.IdfacturasContabilidad;

                    if (msf.IdEmpresaFacturadora != null)
                    {
                        ViewBag.listaConceptos = csf.ComboConceptos((int)msf.IdEmpresaFacturadora);
                      
                            if(msf.habilitar == true)
                        {
                            var tsaldo = csf.VerificaSaldo((int)msf.IdEmpresaFacturadora);
                            msf.Saldo = tsaldo.Saldo;
                        }
                            
                        
                 

                    }
                }



              
            }
            catch (Exception ex)
            {
                ViewBag.ShowDialog = false;
                ViewBag.Mensaje = ex.Message;

                //return View();

                
            }

            return View(msf);
        }

        public JsonResult Conceptos(int IdFacturadora)
        {

            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            List<SelectListItem> lst = new List<SelectListItem>();

            lst = csf.ComboConceptos(IdFacturadora);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Eliminar(ModelSolicitudFacturas msf)
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            msf.IdEstatus = 6;
            msf.IdCancelacion = (int)Session["sIdUsuario"]; 

          csf.Eliminar(msf);

            return RedirectToAction("Index");
        }



        public ActionResult Archivos(string _ID)
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();          
            int IdFacturaContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));            

            return View(csf.listarArchivos(IdFacturaContabilidad));
        }


        public ActionResult Download(string Arch)
        {
            string file = Arch;
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }


        public ActionResult VerCosteo(string _ID , string _PA)
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();
            int IdFacturaContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));
                        
            var cadena = csf.TraeCosteo(IdFacturaContabilidad);

            ViewBag.Costeo = cadena.Replace("col-md-6 col-md-offset-3", "col-md-12").Replace("col-md-6", "col-md-12"); ;

            ViewBag.Principal = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_PA.ToString()));



            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export_Excel(string GridHtml)
        {
            return File(Encoding.Default.GetBytes(GridHtml), "application/vnd.ms-excel", "CosteoExcel.xls");
        }

        public ActionResult Pendientes()
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();
            return View(csf.listarPendientes());
        }

        public ActionResult SolicitudPendiente(string _ID)
        {
            ModelSolicitudFacturas msf = new ModelSolicitudFacturas();
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();
            ViewBag.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));
            msf.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));
            ViewBag.listaRS = csf.ComboRSFacturacion();
            ViewBag.listaTP = csf.ComboTipoPago();
            ViewBag.listaMP = csf.ComboMetodoPago();
            ViewBag.listaCFDI = csf.ComboCFDI();
            ViewBag.listaFacturadoras = csf.ComboFacturadoras();
            ViewBag.listaConceptos = csf.ComboConceptos();
            var costeo = csf.listarArchivos(int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString())));
            msf.IdClienteRazonSocialFacturacion = costeo.IdClienteRazonSocialFacturacion;
            msf.IdEmpresaFacturadora = costeo.IdEmpresaFacturadora;
            msf.ArchivoBancos = costeo.ArchivoBancos;
            msf.Comprobante = costeo.Comprobante;
            if (msf.IdEmpresaFacturadora != null)
            {
                ViewBag.listaConceptos = csf.ComboConceptos((int)msf.IdEmpresaFacturadora);
                msf.IdConceptoFacturacion = costeo.IdConceptoFacturacion;
            }
            return View(msf);
        }


        [HttpPost]

        public ActionResult SolicitudPendiente(ModelSolicitudFacturas msf)
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();


            string carpeta = msf.IdfacturasContabilidad.ToString();
            string ruta = Statics.rutaGralArchivos + @"ArchivosFactura\" + carpeta;

            foreach (string filename in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[filename];


                if (file != null && file.ContentLength > 0)
                {
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

           
                string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;

                var cliente = Session["sCliente"].ToString();
                var unidad = Session["sNomina"].ToString();

                ClassNotificaciones n = new ClassNotificaciones();

                string Correos = n.ExtraeCorreos("FACTURACION", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"],null);

                Message = "Tienes un nuevo comprobante de pago (CEP) " + cliente + " - " + unidad;
                Titulo = "COMPROBANTE PAGO" + cliente + " - " + unidad;

                Modulo = "Facturación";
                n.EnviarCorreo(Message, Titulo, Correos, Modulo);

            

           


            var solicitud = csf.RegistraSolicitudPendiente(msf);

            if (solicitud == true)
            {
                ViewBag.ShowDialog = true;

            }


            return View(msf);
        }



        public ActionResult PendientesTesoreria()
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            return View(csf.listarPendientesTesoreria());
        }


        public ActionResult SolicitudPendienteTesoreria(string _ID)
        {
            ModelSolicitudFacturas msf = new ModelSolicitudFacturas();

            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            ViewBag.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            msf.IdfacturasContabilidad = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString()));

            ViewBag.listaRS = csf.ComboRSFacturacion();
            ViewBag.listaTP = csf.ComboTipoPago();
            ViewBag.listaMP = csf.ComboMetodoPago();
            ViewBag.listaCFDI = csf.ComboCFDI();
            ViewBag.listaFacturadoras = csf.ComboFacturadoras();
            ViewBag.listaConceptos = csf.ComboConceptos();

            var costeo = csf.listarArchivos(int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(_ID.ToString())));
            msf.IdClienteRazonSocialFacturacion = costeo.IdClienteRazonSocialFacturacion;
            msf.IdEmpresaFacturadora = costeo.IdEmpresaFacturadora;
            msf.ArchivoBancos = costeo.ArchivoBancos;
            msf.Comprobante = costeo.Comprobante;

            if (msf.IdEmpresaFacturadora != null)
            {
                ViewBag.listaConceptos = csf.ComboConceptos((int)msf.IdEmpresaFacturadora);
                msf.IdConceptoFacturacion = costeo.IdConceptoFacturacion;
            }

            return View(msf);
        }


        [HttpPost]
        public ActionResult SolicitudPendienteTesoreria(ModelSolicitudFacturas msf)
        {
            ClassSolicitudFacturas csf = new ClassSolicitudFacturas();

            string carpeta = msf.IdfacturasContabilidad.ToString();
            string ruta = Statics.rutaGralArchivos + @"ArchivosTesoreria\" + carpeta;

            foreach (string filename in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[filename];


                if (file != null && file.ContentLength > 0)
                {
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

            string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;

            var cliente = Session["sCliente"].ToString();
            var unidad = Session["sNomina"].ToString();

            ClassNotificaciones n = new ClassNotificaciones();

            string Correos = n.ExtraeCorreos("DISPERSION", (int)Session["sIdCliente"], (int)Session["sIdUnidadNegocio"],null);

            Message = "Tienes un nuevo comprobante de pago (CEP)" + cliente + " - " + unidad;
            Titulo = "COMPROBANTE PAGO" + cliente + " - " + unidad;

            Modulo = "DISPERSION";
            n.EnviarCorreo(Message, Titulo, Correos, Modulo);

            var solicitud = csf.RegistraSolicitudPendienteTesoreria(msf);

            if (solicitud == true)
            {
                ViewBag.ShowDialog = true;

            }


            return View(msf);
        }

        
        public JsonResult ConsultaSaldo(int IdFacturadora)
        {

            var vsaldo = string.Empty;

            var sf = new ClassSolicitudFacturas();
          
            var query = sf.VerificaSaldo(IdFacturadora);

            string dato = string.Empty;

            if (query == null)
            {
                dato = "Error";
            }
            else
            {
                vsaldo = query.Saldo.ToString();
                dato = "OK";

            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = dato, resultsaldo = vsaldo }
            };

        }

     }
}