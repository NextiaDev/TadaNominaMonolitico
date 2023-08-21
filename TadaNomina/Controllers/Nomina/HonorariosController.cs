using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using SAT.Services.ConsultaCFDIService;
using SW.Services.Status;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Models.ViewModels.Reportes;
using TadaNomina.Services;


using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TadaNomina.Controllers.Nomina
{
    public class HonorariosController : BaseController
    {
        // GET: Honorarios
        public ActionResult Index(int pIdPeriodoNomina)
        {
            int IdUnidadNegocio = 0;
            Session["sIdperiodonomina"] = pIdPeriodoNomina;
            ViewBag.pIdPeriodoNomina = pIdPeriodoNomina;

            try
            {
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            }
            catch { IdUnidadNegocio = 0; }


            if (IdUnidadNegocio == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                cHonorarios ClsEmpl = new cHonorarios();
                return View(ClsEmpl.getHonorariosporPeriodo(pIdPeriodoNomina));
            }
        }


        public ActionResult ReportesHonorarios()
        {
            int IdUnidadNegocio = 0;
            List<SelectListItem> Estatus_ = new List<SelectListItem>();

            try
            {
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            }
            catch { IdUnidadNegocio = 0; }


            if (IdUnidadNegocio == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {

                Estatus_.Add(new SelectListItem { Text = "Completados", Value = "1" });
                Estatus_.Add(new SelectListItem { Text = "Rechazados", Value = "2" });
                Estatus_.Add(new SelectListItem { Text = "Proceso", Value = "3" });
                Estatus_.Add(new SelectListItem { Text = "Todos", Value = "1,2,3" });


                ModelReporteHonorarios m = new ModelReporteHonorarios()
                {
                    LEstatus = Estatus_,

                };


                return View(m);

            }
        }

        public FileResult DescargarReporteHonorariosByClienteFechas(ModelReporteHonorarios m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string Estatus = m.Estatus;

            DataTable dt = reportes.GetDataTableReporteHonorarios(fInicial, fFinal, Estatus, IdCliente);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Honorarios.xlsx");
                }
            }
        }


        public ActionResult HonorariosPeriodos()
        {

            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model = cperiodo.GetModelPeriodoNominasHonorarios(IdUnidadNegocio);
                return View(model);
            }
        }

        public ActionResult HonorariosUser(int Periodo)
        {

            int IdUnidadNegocio = 0;
            Session["sIdperiodonomina"] = Periodo;
            ViewBag.pIdPeriodoNomina = Periodo;

            try
            {
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            }
            catch { IdUnidadNegocio = 0; }


            if (IdUnidadNegocio == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                cHonorarios ClsEmpl = new cHonorarios();
                return View(ClsEmpl.getHonorariosporPeriodo(Periodo));
            }
        }


        public FileResult DownloadPDF(int ID)
        {
            // Obtener contenido del archivo


            cHonorarios ClsEmpl = new cHonorarios();
            var a = ClsEmpl.getHonorariosporUsuario(ID);

            byte[] arch = System.IO.File.ReadAllBytes(a.RutaPDF);
            string name = System.IO.Path.GetFileName(a.RutaPDF);
            return File(arch, "application/octet-stream", name);

        }

        public FileResult DownloadXML(int ID)
        {
            // Obtener contenido del archivo


            cHonorarios ClsEmpl = new cHonorarios();
            var a = ClsEmpl.getHonorariosporUsuario(ID);

            byte[] arch = System.IO.File.ReadAllBytes(a.RutaXML);
            string name = System.IO.Path.GetFileName(a.RutaXML);
            return File(arch, "application/octet-stream", name);

        }


        public FileResult Download(int ID)
        {
            // Obtener contenido del archivo


            cHonorarios ClsEmpl = new cHonorarios();
            var a = ClsEmpl.getHonorariosporUsuario(ID);

            byte[] arch = System.IO.File.ReadAllBytes(a.URL_Archivos);
            string name = System.IO.Path.GetFileName(a.URL_Archivos);
            return File(arch, "application/octet-stream", name);

        }

        public ActionResult CreateLayout(int pIdPeriodoNomina)
        {
            int IdCliente = (int)Session["sIdCliente"];

            ClassEmpleado ce = new ClassEmpleado();
            List<Cat_RegistroPatronal> registroPat = new List<Cat_RegistroPatronal>();
            List<Cat_HonorariosFacturas> catHonorarios = new List<Cat_HonorariosFacturas>();

            int periodo = (int)Session["sIdperiodonomina"];
            ModelIncidencias modelo = new ModelIncidencias();
            var model = ce.getCatalogos(IdCliente);

            modelo.IdPeriodoNomina = periodo;
            modelo.registroPat = model.registroPat;
            modelo.cat_hono = model.Cat_Hono;
            ViewBag.Periodo = periodo;


            return View(modelo);
        }

        [HttpPost]
        public ActionResult CreateLayout(ModelIncidencias model)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];
            int periodo = (int)Session["sIdperiodonomina"];

            ViewBag.Periodo = periodo;


            ClassIncidencias cincidencias = new ClassIncidencias();
            ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);

            if (ModelState.IsValid)
            {
                if (model.Archivo.ContentLength > 0)
                {

                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);

                    ModelErroresIncidencias errores = cincidencias.GetIncidenciasArchivoHonorarios(_path, IdUnidad, periodo, IdUsuario);
                    ViewBag.Finalizado = "SI";

                    return TextFile(errores);


                }
            }

            return View(modelo);
        }


        public ActionResult TextFile(ModelErroresIncidencias model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE INCIDENCIAS DEL ARCHIVO: " + model.Path);
            tw.WriteLine("");
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("Numero de Registros Leidos: " + model.Errores);
            tw.WriteLine("Insertados correctamente: " + model.Correctos);
            tw.WriteLine("No Insertados: " + model.Errores);
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("");

            if (list.Count > 0)
            {
                tw.WriteLine("Detalle de los errores:");
                tw.WriteLine("");
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
            else
            {
                tw.WriteLine("El archivo se cargo correctamente.");
            }

            tw.Flush();
            tw.Close();
            return File(memoryStream.GetBuffer(), "text/plain", "resultado.txt");
        }
        public ActionResult nuevoHonorario()
        {

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            int periodo = (int)Session["sIdperiodonomina"];


            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                int idperiodo = periodo;

                if (idperiodo > 0)
                {
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                    List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoHonorarios(IdUnidadNegocio);
                    Empleado empleado = new Empleado();
                    try { empleado.RegistrosPatronalesList = GetRegistrosPatronales(IdCliente); }
                    catch { empleado.RegistrosPatronalesList = new List<SelectListItem>(); }


                    try { empleado.HonorarioFactura = GetRegistrosHonorarios(IdCliente); }
                    catch { empleado.HonorarioFactura = new List<SelectListItem>(); }


                    List<SelectListItem> lperiodos = new List<SelectListItem>();
                    List<SelectListItem> emple = new List<SelectListItem>();

                    lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });
                    List<Empleado> empleados = classEmpleado.GetEmpleadosHonorarios(IdUnidadNegocio);
                    empleados.ForEach(x => { emple.Add(new SelectListItem { Value = x.IdEmpleado.ToString(), Text = x.Nombre + " " + x.ApellidoMaterno }); });


                    empleado.lPeriodos = lperiodos;
                    empleado.LEmpleados = emple;

                    return View(empleado);
                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "1 ¡Modificacion Errónea!";
                    ViewBag.message = "No Fue posible encontrar al empleado solicitado.";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "2 ¡Modificacion Errónea!";
                ViewBag.message = "No Fue posible encontrar al empleado solictado." + ex.Message;
                return View("Response");
            }

        }




        [HttpPost]
        public JsonResult GetEmpleados()
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleados(IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }


        [HttpPost]
        public JsonResult GetEmpleadosByNombre(string name)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosByNombreHonorarios(name, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }


        public JsonResult GetEmpleadosByClave(string clave)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                var data = empleados.GetEmpleadosByClaveHonorarios(clave, IdUnidadNegocio);
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }


        public ActionResult Seach()
        {

            try
            {
                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }

        }


        public List<SelectListItem> GetRegistrosPatronales(int idCliente)
        {
            List<SelectListItem> RegistrosPatronales = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var rp = (from b in entity.Cat_RegistroPatronal where b.IdCliente == idCliente select b).ToList();
                rp.ForEach(x => { RegistrosPatronales.Add(new SelectListItem { Text = x.RFC.ToUpper() + "--" + x.NombrePatrona.ToUpper() }); });

                var es = (from b in entity.vClienteEmpresaEspecializada where b.IdCliente == idCliente select b).ToList();
                es.ForEach(x => { RegistrosPatronales.Add(new SelectListItem { Text = x.RFC.ToUpper() + "--" + x.NombrePatrona.ToUpper() + "-" + x.RFC.ToUpper(), Value = x.IdRegistroPatronal.ToString() }); });
            }

            return RegistrosPatronales;
        }


        public List<SelectListItem> GetRegistrosHonorarios(int idCliente)
        {
            List<SelectListItem> RegistrosHonorarios = new List<SelectListItem>();

            using (NominaEntities1 entity = new NominaEntities1())
            {
                var rp = (from b in entity.Cat_HonorariosFacturas where b.IdCliente == idCliente select b).ToList();
                rp.ForEach(x => { RegistrosHonorarios.Add(new SelectListItem { Text = x.Clave.ToUpper() + "-" + x.Descripcion.ToUpper(), Value = x.IdFactura.ToString() }); });


            }

            return RegistrosHonorarios;
        }


        [HttpPost]
        public ActionResult nuevoHonorario(mHonorarios modelo)
        {
            int periodo = (int)Session["sIdperiodonomina"];

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            cHonorarios cl = new cHonorarios();
            modelo.IdPeriodoNomina = periodo;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                cl.GuardarHonorarios(modelo, IdUsuario);

                return RedirectToAction("Index", "Honorarios", new { pIdPeriodoNomina = periodo });
            }
            catch (Exception)
            {

                throw;
            }
        }



        public ActionResult HonorariosEmp(int id)
        {

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {

                if (id > 0)
                {
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    vHonorarios empleado = classEmpleado.GetEmpleadosHonorariosv(id);
                    Empleado modelo = new Empleado();

                    modelo.ClaveEmpleado = empleado.ClaveEmpleado;
                    modelo.ObservacionesH = empleado.Observaciones;
                    modelo.ConceptoPago = empleado.Clave + " - " + empleado.Descripcion;
                    modelo.ObservacionesH = empleado.Observaciones;
                    modelo.Patrona = empleado.NombrePatrona;
                    modelo.Periodo = empleado.Periodo;
                    modelo.Nombre = empleado.Nombre;
                    modelo.ApellidoPaterno = empleado.ApellidoPaterno;
                    modelo.ApellidoMaterno = empleado.ApellidoMaterno;
                    modelo.Subtotal = (decimal)empleado.SubTotal;
                    modelo.iva = (decimal)empleado.IVA;
                    modelo.totalfactura = (decimal)empleado.TotalFactura;
                    modelo.retencionisr = (decimal)empleado.RetencionISR;
                    modelo.retencioniva = (decimal)empleado.RetencionIVA;
                    modelo.totalRetencion = (decimal)empleado.Total;
                    modelo.ObservacionesUsuario = empleado.ObservacionesUsuario;
                    modelo.ObservacionesCliente = empleado.ObservacionesCliente;



                    return View(modelo);
                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "1 ¡Modificacion Errónea!";
                    ViewBag.message = "No Fue posible encontrar al empleado solicitado.";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "2 ¡Modificacion Errónea!";
                ViewBag.message = "No Fue posible encontrar al empleado solictado." + ex.Message;
                return View("Response");
            }
        }

        [HttpPost]
        public ActionResult HonorariosEmp(mHonorarios modelo)
        {

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            cHonorarios cl = new cHonorarios();


            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                cl.GuardarHonorarios(modelo, IdUsuario);

                return RedirectToAction("Index", "Honorarios");
            }
            catch (Exception)
            {

                throw;
            }
        }




        [HttpPost]
        public JsonResult CalcularHonorariosbrutos(string brutos, string clave)
        {
            double retencionisr = 0;

            cHonorarios cl = new cHonorarios();
            var datos = cl.getEmpleadosTipo(int.Parse(clave));




            double subtotal = Math.Round(double.Parse(brutos));
            double iva = Math.Round(subtotal * 0.16, 2);
            double totalF = Math.Round(subtotal + iva, 2);
            if (datos.TipoContrato == "RESICO")
            {
                retencionisr = Math.Round(subtotal * .0125, 2);

            }
            else
            {
                retencionisr = Math.Round(subtotal * 0.1, 2);

            }
            double retencioniva = Math.Round(iva / 3 * 2, 2);
            double totalconretencion = Math.Round(totalF - retencionisr - retencioniva, 2);


            var mHonorarios = new mHonorarios
            {
                subtotal = subtotal,
                iva = iva,
                totalfactura = totalF,
                retencionisr = retencionisr,
                retencioniva = retencioniva,
                totalRetencion = totalconretencion

            };

            return Json(mHonorarios, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult CalcularHonorarisNetos(string netos, string clave)
        {
            double retencionisr = 0;

            cHonorarios cl = new cHonorarios();
            var datos = cl.getEmpleadosTipo(int.Parse(clave));

            double subtotal = Math.Round(double.Parse(netos) / 0.953333334, 2);
            double iva = Math.Round(subtotal * 0.16, 2);
            double totalF = Math.Round(subtotal + iva, 2);
            if (datos.TipoContrato == "RESICO")
            {
                retencionisr = Math.Round(subtotal * .0125, 2);

            }
            else
            {
                retencionisr = Math.Round(subtotal * 0.1, 2);

            }
            double retencioniva = Math.Round(iva / 3 * 2, 2);
            double totalconretencion = Math.Round(totalF - retencionisr - retencioniva, 2);


            var mHonorarios = new mHonorarios
            {
                subtotal = subtotal,
                iva = iva,
                totalfactura = totalF,
                retencionisr = retencionisr,
                retencioniva = retencioniva,
                totalRetencion = totalconretencion

            };

            return Json(mHonorarios, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult DeleteHonorarios(int idHonorario, string Observaciones)
        {
            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            cHonorarios cl = new cHonorarios();
            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                cl.DeleteHonorarios(idHonorario, Observaciones, IdUsuario);
                return Json("OK", JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]

        public string EncodeParam(string param)
        {
            byte[] array = Encoding.ASCII.GetBytes(param);
            return Server.UrlTokenEncode(array);
        }


        public ActionResult Edit(string data)
        {

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            int periodo = (int)Session["sIdperiodonomina"];


            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                int idperiodo = periodo;

                if (idperiodo > 0)
                {
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                    List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoHonorarios(IdUnidadNegocio);
                    Empleado empleado = new Empleado();
                    int idEmpleado = Convert.ToInt32(DecodeParam(data));
                    empleado = classEmpleado.GetEmpleadoToEditHono(idEmpleado, IdUnidadNegocio, IdCliente);

                    try { empleado.RegistrosPatronalesList = GetRegistrosPatronales(IdCliente); }
                    catch { empleado.RegistrosPatronalesList = new List<SelectListItem>(); }


                    try { empleado.HonorarioFactura = GetRegistrosHonorarios(IdCliente); }
                    catch { empleado.HonorarioFactura = new List<SelectListItem>(); }


                    List<SelectListItem> lperiodos = new List<SelectListItem>();
                    List<SelectListItem> emple = new List<SelectListItem>();

                    lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });
                    List<Empleado> empleados = classEmpleado.GetEmpleadosHonorarios(IdUnidadNegocio);
                    empleados.ForEach(x => { emple.Add(new SelectListItem { Value = x.IdEmpleado.ToString(), Text = x.Nombre + " " + x.ApellidoMaterno }); });


                    empleado.lPeriodos = lperiodos;
                    empleado.LEmpleados = emple;
                   

                    return View(empleado);
                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "1 ¡Modificacion Errónea!";
                    ViewBag.message = "No Fue posible encontrar al empleado solicitado.";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "2 ¡Modificacion Errónea!";
                ViewBag.message = "No Fue posible encontrar al empleado solictado." + ex.Message;
                return View("Response");
            }

        }


        [HttpPost]
        public ActionResult Edit(mHonorarios modelo)
        {
            int periodo = (int)Session["sIdperiodonomina"];

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            cHonorarios cl = new cHonorarios();
            modelo.IdPeriodoNomina = periodo;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                cl.GuardarHonorarios(modelo, IdUsuario);

                return RedirectToAction("Index", "Honorarios", new { pIdPeriodoNomina = periodo });
            }
            catch (Exception)
            {

                throw;
            }
        }


        private string DecodeParam(string param)
        {
            byte[] array = Server.UrlTokenDecode(param);
            return Encoding.UTF8.GetString(array);
        }

        public ActionResult HonorariosEliminados(int pIdPeriodoNomina)
        {

            int IdUnidadNegocio = 0;
            Session["sIdperiodonomina"] = pIdPeriodoNomina;
            ViewBag.pIdPeriodoNomina = pIdPeriodoNomina;

            try
            {
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            }
            catch { IdUnidadNegocio = 0; }


            if (IdUnidadNegocio == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                cHonorarios ClsEmpl = new cHonorarios();
                return View(ClsEmpl.getHonorariosporPeriodoEliminados(pIdPeriodoNomina));
            }
        }


        public ActionResult Revision(int? id)
        {
            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            Session["sidHonorario"] = id;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                Session["sidHonorario"] = id;
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {

                if (id > 0)
                {
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    vHonorarios empleado = classEmpleado.GetEmpleadosHonorariosv(id.Value);
                    Empleado modelo = new Empleado();

                    modelo.idHono = empleado.IdHonorarios;
                    modelo.ClaveEmpleado = empleado.ClaveEmpleado;
                    modelo.ObservacionesH = empleado.Observaciones;
                    modelo.ConceptoPago = empleado.Clave + " - " + empleado.Descripcion;
                    modelo.ObservacionesH = empleado.Observaciones;
                    modelo.Patrona = empleado.NombrePatrona;
                    modelo.Periodo = empleado.Periodo;
                    modelo.Nombre = empleado.Nombre;
                    modelo.ApellidoPaterno = empleado.ApellidoPaterno;
                    modelo.ApellidoMaterno = empleado.ApellidoMaterno;
                    modelo.Subtotal = (decimal)empleado.SubTotal;
                    modelo.iva = (decimal)empleado.IVA;
                    modelo.totalfactura = (decimal)empleado.TotalFactura;
                    modelo.retencionisr = (decimal)empleado.RetencionISR;
                    modelo.retencioniva = (decimal)empleado.RetencionIVA;
                    modelo.totalRetencion = (decimal)empleado.Total;
                    modelo.ObservacionesUsuario = empleado.ObservacionesUsuario;
                    modelo.ObservacionesCliente = empleado.ObservacionesCliente;
                    modelo.Rfc_Emisor = empleado.RFC_Emisor;
                    try
                    {
                        modelo.Rfc_Receptor = empleado.RFC_Recepctor;

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        modelo.TotalXML = (decimal)empleado.TotalXML;

                    }
                    catch (Exception)
                    {

                        modelo.TotalXML = 0;
                    }
                    try
                    {
                        modelo.UUID = empleado.UUID;

                    }
                    catch (Exception)
                    {

                        modelo.UUID = "No contiene informacion";
                    }
                    return View(modelo);
                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "1 ¡Modificacion Errónea!";
                    ViewBag.message = "No Fue posible encontrar al empleado solicitado.";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "2 ¡Modificacion Errónea!";
                ViewBag.message = "No Fue posible encontrar al empleado solictado." + ex.Message;
                return View("Response");
            }

        }

        [HttpPost]
        public JsonResult ValidarXMLCargar(string Emisor, string Receptor, string TotalXML, string UUID)
        {
            cHonorarios cl = new cHonorarios();

            int idHonor = int.Parse(Session["sidHonorario"].ToString());


            CFDIHONO xmlActual = new CFDIHONO();


            Status status = new Status("https://consultaqr.facturaelectronica.sat.gob.mx/ConsultaCFDIService.svc");
            Acuse response = status.GetStatusCFDI(Emisor, Receptor, TotalXML, UUID);
            xmlActual.CODIGO_ESTATUS = response.CodigoEstatus;
            xmlActual.ESTADO = response.Estado;
            xmlActual.ES_CANCELABLE = response.EsCancelable;
            xmlActual.ESTATUS_CANCELACION = response.EstatusCancelacion;
            cl.GuardarHistoricoHonorarios(idHonor, xmlActual.ESTADO);
            return Json(idHonor, JsonRequestBehavior.AllowGet);


        }


        public ActionResult ValidacionesLista(int id)
        {
            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            Session["sidHonorario"] = id;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {

                if (id > 0)
                {

                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    return View(classEmpleado.Historial(id));
                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "1 ¡Modificacion Errónea!";
                    ViewBag.message = "No Fue posible encontrar al empleado solicitado.";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "2 ¡Modificacion Errónea!";
                ViewBag.message = "No Fue posible encontrar al empleado solictado." + ex.Message;
                return View("Response");
            }

        }

    }
}