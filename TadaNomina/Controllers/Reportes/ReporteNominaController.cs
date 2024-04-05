using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteNominaController : BaseController
    {
        // GET: ReporteNomina

        /// <summary>
        /// Muestra nominas acumuladas y pueden descargar el reporte de la nomina
        /// </summary>
        /// <returns>Nominas acumuladas</returns>
        public ActionResult Index()
        {
            cReportesNomina cReportes = new cReportesNomina();
            List<ModelReporteNomina> model = cReportes.GetListaPeriodosAcumulados((int)Session["sIdUnidadNegocio"]);

           


            return View(model);
        }

        /// <summary>
        /// Descarga el reporte de la nomina seleccionada
        /// </summary>
        /// <param name="id">Identificador de la nomina</param>
        /// <returns>Reporte de nomina acumulada formato excel</returns>
        public ActionResult Descargar(int id)
        {
            try
            {
                int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];

                cReportesNomina cempleados = new cReportesNomina();
                DataTable dt = cempleados.GetDataTableForNomina(id);
                if (Session["sIdCliente"].ToString() == "123")
                {
                    DataTable dt2 = cempleados.DatosProcesados(dt);
                    DataTable info = cempleados.InfoDatosProcesados(id, IdUnidadnegocio);
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add().Cell(1, 1).InsertTable(info).Cell(6, 1).InsertTable(dt2);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                        }
                    }
                }
                else
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Grid");
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                        }
                    }
                }
            }
            catch
            {
                return File(new byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
            }
            
        }

        /// <summary>
        /// Descarga reporte de nomina acumulada en formato PDF
        /// </summary>
        /// <returns>Reporte de nomina en formato PDF</returns>
        public ActionResult ReporteTipoPDf(string validacion)
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
            cReportesNomina cempleados = new cReportesNomina();
            List<ModelReporteNomina> model = cempleados.GetListaPeriodosAcumuladosActivos(IdUnidadnegocio);

            if (validacion != null)
            {
                ViewBag.Mensaje = "Error: " + validacion;

            }
            return View(model);
        }
        public ActionResult DescargaUnidadNegocio(int id)
        {
            string validacion;

            try
            {
                int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
                int Cliente = (int)Session["sIdCliente"];
                string nomina = Session["sNomina"].ToString();
                cReportesNomina cReportes = new cReportesNomina();
                var listPer = cReportes.GetPeriodosActivosyCerrados(IdUnidadnegocio);
                PeriodoNomina act = new PeriodoNomina();
                act = listPer.Where(x => x.IdPeriodoNomina == id).FirstOrDefault();
                vRegistroPatronal vista = new vRegistroPatronal();
                var LisR = cReportes.GetRegistroPatronal(IdUnidadnegocio);
                vista = LisR.Where(x => x.IdUnidadNegocio == IdUnidadnegocio).FirstOrDefault();

                ClassReportesPDF pdf = new ClassReportesPDF(act, vista);


                List<string> files = new List<string>();
                string formato = "-FORMATO-VariosArchivos";
                string ruta_PDFS = string.Empty;
                string ruta_PDFS_ZIP = string.Empty;

                string ruta_Principal = @"D:\Apps\TadaNomina\XML\" + Session["sIdUnidades"].ToString() + formato + @"\";
                ruta_PDFS = @"D:\Apps\TadaNomina\XML\" + Session["sIdUnidades"].ToString() + formato + @"\" + id + @"\PDFS\";
                ruta_PDFS_ZIP = @"D:\Apps\TadaNomina\XML\" + Session["sIdUnidades"].ToString() + formato + @"\" + id;

                if (!Directory.Exists(ruta_Principal))
                {
                    System.IO.Directory.CreateDirectory(ruta_Principal);
                }
                else
                {
                    System.IO.Directory.Delete(ruta_Principal, true);
                }


                if (!Directory.Exists(ruta_PDFS))
                {
                    System.IO.Directory.CreateDirectory(ruta_PDFS);
                }
                else
                {
                    System.IO.Directory.Delete(ruta_PDFS, true);
                }


                using (NominaEntities1 entidad1 = new NominaEntities1())
                {
                    var query = from b in entidad1.Cat_CentroCostos
                                where b.IdCliente == Cliente && b.IdEstatus == 1
                                select b;






                    List<sp_PDFSunset1_Result> datos = new List<sp_PDFSunset1_Result>();

                    using (NominaEntities1 entidad = new NominaEntities1())
                    {
                        string consulta = "sp_PDFSunset " + id;
                        datos = entidad.Database.SqlQuery<sp_PDFSunset1_Result>(consulta).ToList();
                    }
                    List<string> archivos = new List<string>();


                    if (datos.Count() > 0)
                    {
                        string archivoFisicotmp = ruta_PDFS + id + "-" + nomina.Replace("\"", "").Trim();
                        archivos.Add(nomina);
                        cReportesNomina cn = new cReportesNomina();
                        int[] periodos = new int[1];
                        periodos[0] = id;


                        var ins = cn.GetvInsidenciasPeriodo(periodos);

                        pdf.CrearPdf(archivoFisicotmp + ".pdf", ins, datos, act);
                        files.Add(archivoFisicotmp + ".pdf");



                        CreateZipFile(files, ruta_PDFS_ZIP + @"\" + nomina.Replace("\"", "") + @".zip");
                    }


                    byte[] fileBytes = null;
                    string fileName = string.Empty;
                    if (archivos.Count > 0)
                    {
                        foreach (var item in archivos)
                        {
                            fileBytes = System.IO.File.ReadAllBytes(ruta_PDFS_ZIP + @"\" + item + ".zip");
                            fileName = item + ".zip";
                        }
                    }

                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

                }
            }
            catch (Exception ex)
            {

                validacion = ex.Message.ToString();

                return RedirectToAction("ReporteTipoPDf", "ReporteNomina", new { validacion });
            }



        }
        /// <summary>
        /// Reporte nomina por centros de costos en formato PDF
        ///         /// Se  agrega un try y un catch

        /// </summary>
        /// <param name="id">Periodo de nomina</param>
        /// <returns>Reporte en formato PDF</returns>
        public ActionResult DescargaCentros(int id)
        {

            string validacion;
            try
            {
                int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
                int Cliente = (int)Session["sIdCliente"];

                cReportesNomina cReportes = new cReportesNomina();
                var listPer = cReportes.GetPeriodosActivosyCerrados(IdUnidadnegocio);
                PeriodoNomina act = new PeriodoNomina();
                act = listPer.Where(x => x.IdPeriodoNomina == id).FirstOrDefault();
                vRegistroPatronal vista = new vRegistroPatronal();
                var LisR = cReportes.GetRegistroPatronal(IdUnidadnegocio);
                vista = LisR.Where(x => x.IdUnidadNegocio == IdUnidadnegocio).FirstOrDefault();

                ClassReportesPDF pdf = new ClassReportesPDF(act, vista);


                List<string> files = new List<string>();
                string formato = "-FORMATO-VariosArchivos";
                string ruta_PDFS = string.Empty;
                string ruta_PDFS_ZIP = string.Empty;

                string ruta_Principal = @"D:\Apps\TadaNomina\XML\" + Session["sIdUnidades"].ToString() + formato + @"\";
                ruta_PDFS = @"D:\Apps\TadaNomina\XML\" + Session["sIdUnidades"].ToString() + formato + @"\" + id + @"\PDFS\";
                ruta_PDFS_ZIP = @"D:\Apps\TadaNomina\XML\" + Session["sIdUnidades"].ToString() + formato + @"\" + id;

                if (!Directory.Exists(ruta_Principal))
                {
                    System.IO.Directory.CreateDirectory(ruta_Principal);
                }
                else
                {
                    System.IO.Directory.Delete(ruta_Principal, true);
                }


                if (!Directory.Exists(ruta_PDFS))
                {
                    System.IO.Directory.CreateDirectory(ruta_PDFS);
                }
                else
                {
                    System.IO.Directory.Delete(ruta_PDFS, true);
                }


                using (NominaEntities1 entidad1 = new NominaEntities1())
                {
                    var query = from b in entidad1.Cat_CentroCostos
                                where b.IdCliente == Cliente && b.IdEstatus == 1
                                select b;

                    List<sp_PDFSunset1_Result> datos = new List<sp_PDFSunset1_Result>();

                    using (NominaEntities1 entidad = new NominaEntities1())
                    {
                        string consulta = "sp_PDFSunset " + id;
                        datos = entidad.Database.SqlQuery<sp_PDFSunset1_Result>(consulta).ToList();
                    }
                    List<string> archivos = new List<string>();
                    foreach (var item in query)
                    {
                        List<sp_PDFSunset1_Result> queryPorCentroCostos = (from myRow in datos
                                                                           where myRow.IdCentroCostos == item.IdCentroCostos
                                                                           orderby myRow.Nombre ascending
                                                                           select myRow).OrderBy(x => x.Puesto).ToList();

                        if (queryPorCentroCostos.Count() > 0)
                        {
                            string archivoFisicotmp = ruta_PDFS + id + "-" + item.CentroCostos.Replace("\"", "").Trim();
                            archivos.Add(item.CentroCostos);
                            cReportesNomina cn = new cReportesNomina();
                            int[] periodos = new int[1];
                            periodos[0] = id;

                            var ins = cn.GetvInsidencias(periodos, item.IdCentroCostos);

                            pdf.CrearPdf(archivoFisicotmp + ".pdf", ins, queryPorCentroCostos, act);
                            files.Add(archivoFisicotmp + ".pdf");

                            CreateZipFile(files, ruta_PDFS_ZIP + @"\" + item.CentroCostos.Replace("\"", "") + @".zip");
                            Session["Query"] = queryPorCentroCostos;
                        }
                    }
                    byte[] fileBytes = null;
                    string fileName = string.Empty;
                    if (archivos.Count > 0)
                    {
                        foreach (var item in archivos)
                        {
                            fileBytes = System.IO.File.ReadAllBytes(ruta_PDFS_ZIP + @"\" + item + ".zip");
                            fileName = item + ".zip";
                        }
                    }

                    return File(fileBytes ?? new byte[0], System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            catch (Exception ex)
            {
                validacion = ex.Message.ToString();

                return RedirectToAction("ReporteTipoPDf", "ReporteNomina", new { validacion });

            }



        }

        /// <summary>
        /// Reporte de nomina por departamentos en formato PDF
        /// </summary>
        /// <param name="id">Periodo de nomina</param>
        /// <returns>Reporte de nomina en formato PDF</returns>
        ///         /// Se  agrega un try y un catch

        public ActionResult DescargaDepart(int id)
        {
            string validacion;

            try
            {
                int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
                int Cliente = (int)Session["sIdCliente"];

                cReportesNomina cReportes = new cReportesNomina();
                var listPer = cReportes.GetPeriodosActivosyCerrados(IdUnidadnegocio);
                PeriodoNomina act = new PeriodoNomina();
                act = listPer.Where(x => x.IdPeriodoNomina == id).FirstOrDefault();
                vRegistroPatronal vista = new vRegistroPatronal();
                var LisR = cReportes.GetRegistroPatronal(IdUnidadnegocio);
                vista = LisR.Where(x => x.IdUnidadNegocio == IdUnidadnegocio).FirstOrDefault();

                ClassReportesPDF pdf = new ClassReportesPDF(act, vista);


                List<string> files = new List<string>();
                string formato = "-FORMATO-VariosArchivos";
                string ruta_PDFS = string.Empty;
                string ruta_PDFS_ZIP = string.Empty;

                string ruta_Principal = Statics.rutaGralArchivos + @"PDF\" + Session["sIdUnidades"].ToString() + formato + @"\";
                ruta_PDFS = Statics.rutaGralArchivos + @"PDF\" + Session["sIdUnidades"].ToString() + formato + @"\" + id + @"\PDFS\";
                ruta_PDFS_ZIP = Statics.rutaGralArchivos + @"PDF\" + Session["sIdUnidades"].ToString() + formato + @"\" + id;

                if (!Directory.Exists(ruta_Principal))
                {
                    System.IO.Directory.CreateDirectory(ruta_Principal);
                }
                else
                {
                    System.IO.Directory.Delete(ruta_Principal, true);
                }


                if (!Directory.Exists(ruta_PDFS))
                {
                    System.IO.Directory.CreateDirectory(ruta_PDFS);
                }
                else
                {
                    System.IO.Directory.Delete(ruta_PDFS, true);
                }


                using (NominaEntities1 entidad1 = new NominaEntities1())
                {
                    var query = from b in entidad1.Cat_Departamentos
                                where b.IdCliente == Cliente
                                select b;


                    List<sp_PDFSunset1_Result> datos = new List<sp_PDFSunset1_Result>();

                    using (NominaEntities1 entidad = new NominaEntities1())
                    {
                        string consulta = "sp_PDFSunset " + id;
                        datos = entidad.Database.SqlQuery<sp_PDFSunset1_Result>(consulta).ToList();
                    }
                    List<string> archivos = new List<string>();
                    foreach (var item in query)
                    {


                        List<sp_PDFSunset1_Result> queryPorCentroCostos = (from myRow in datos
                                                                           where myRow.IdDepartamento == item.IdDepartamento
                                                                           orderby myRow.Nombre ascending
                                                                           select myRow).OrderBy(x => x.Puesto).ToList();

                        if (queryPorCentroCostos.Count() > 0)
                        {
                            string archivoFisicotmp = ruta_PDFS + id + "-" + item.Departamento.Replace("\"", "").Trim();
                            archivos.Add(item.Departamento);
                            cReportesNomina cn = new cReportesNomina();
                            int[] periodos = new int[1];
                            periodos[0] = id;
                            var ins = cn.GetvInsidenciasdepar(periodos, item.IdDepartamento);

                            pdf.CrearPdf(archivoFisicotmp + ".pdf", ins, queryPorCentroCostos, act);
                            files.Add(archivoFisicotmp + ".pdf");



                            CreateZipFile(files, ruta_PDFS_ZIP + @"\" + item.Departamento.Replace("\"", "") + @".zip");
                            Session["Query"] = queryPorCentroCostos;
                        }

                    }
                    byte[] fileBytes = null;
                    string fileName = string.Empty;
                    if (archivos.Count > 0)
                    {
                        foreach (var item in archivos)
                        {
                            fileBytes = System.IO.File.ReadAllBytes(ruta_PDFS_ZIP + @"\" + item + ".zip");
                            fileName = item + ".zip";
                        }
                    }

                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

                }
            }
            catch (Exception ex)
            {
                validacion = ex.Message.ToString();

                return RedirectToAction("ReporteTipoPDf", "ReporteNomina", new { validacion });

            }




        }

        /// <summary>
        /// Lista periodos de nomina acumulados y activos
        /// </summary>
        /// <returns>Lista de periodos de nomina</returns>
        public ActionResult NuevosReportes()
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
            cReportesNomina cempleados = new cReportesNomina();
            List<ModelReporteNomina> model = cempleados.GetListaPeriodosAcumuladosActivosReportes(IdUnidadnegocio);

            return View(model);
        }

        /// <summary>
        /// Visualiza reporte de nomina
        /// </summary>
        /// <param name="id">Periodo de nomina</param>
        /// <returns>Reporte de nomina</returns>
        public ActionResult NuevosReportesResumen(int id)
        {


            //OBTENER INFORMACION DE LA BASE
            List<sp_CosteoResumenMensual_Result> ListSpReport = new List<sp_CosteoResumenMensual_Result>();
            using (TadaTimbradoEntities report = new TadaTimbradoEntities())
            {
                ListSpReport = report.sp_CosteoResumenMensual(id.ToString()).ToList();
            }


            string path = @"D:\Apps\TadaNomina\TemplateHTML\TemplateNomina.html";

            //string path = @"C:\Users\Alberto\OneDrive\Escritorio\Ultimaversion\TadaNomina\TemplateHTML\TemplateNomina.html";


            StringBuilder strReadHTML = new StringBuilder();

            foreach (sp_CosteoResumenMensual_Result spReport in ListSpReport)
            {
                string readHTML = System.IO.File.ReadAllText(path);
                if (spReport.Empresa != string.Empty)
                {
                    readHTML = readHTML.Replace("#REGISTROP", spReport.Empresa);
                }
                if (spReport.RFCPatrona != string.Empty)
                {
                    readHTML = readHTML.Replace("#RFC", spReport.RFCPatrona);
                }
                if (spReport.PERIODO != string.Empty)
                {
                    readHTML = readHTML.Replace("#PERIODON", spReport.PERIODO);
                }
                if (spReport.NOMINA != string.Empty)
                {
                    readHTML = readHTML.Replace("#NOMINA", spReport.NOMINA);
                }
                if (spReport.NOMINA != string.Empty)
                {
                    readHTML = readHTML.Replace("#NOMINA", spReport.NOMINA);
                }

                if (spReport.SUELDO != 0)
                {
                    readHTML = readHTML.Replace("#SUELDO", spReport.SUELDO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SUELDO", "0.00");

                }

                if (spReport.PRIMA_VACACIONAL != 0)
                {
                    readHTML = readHTML.Replace("#PRIMAV", spReport.PRIMA_VACACIONAL.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#PRIMAV", "0.00");

                }

                if (spReport.VACACIONES != 0)
                {
                    readHTML = readHTML.Replace("#VACACIONES", spReport.VACACIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#VACACIONES", "0.00");

                }
                if (spReport.AGUINALDO != 0)
                {
                    readHTML = readHTML.Replace("#AGUINALDO", spReport.AGUINALDO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#AGUINALDO", "0.00");

                }
                if (spReport.TOTAL_PERCEPCIONES != 0)
                {
                    readHTML = readHTML.Replace("#TOTALP", spReport.TOTAL_PERCEPCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALP", "0.00");

                }

                //

                if (spReport.ImpuestoRetenido != 0)
                {
                    readHTML = readHTML.Replace("#ISR", spReport.ImpuestoRetenido.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#ISR", "0.00");

                }

                if (spReport.IMSS != 0)
                {
                    readHTML = readHTML.Replace("#IMSS", spReport.IMSS.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#IMSS", "0.00");

                }
                if (spReport.OTRAS_PERCEPCIONES != 0)
                {
                    readHTML = readHTML.Replace("#PRESTAMOP", spReport.OTRAS_PERCEPCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#PRESTAMOP", "0.00");

                }
                if (spReport.FONACOT != 0)
                {
                    readHTML = readHTML.Replace("#PRESTAMOF", spReport.FONACOT.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#PRESTAMOF", "0.00");

                }
                if (spReport.INFONAVIT != 0)
                {
                    readHTML = readHTML.Replace("#CREDITOI", spReport.INFONAVIT.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#CREDITOI", "0.00");

                }
                if (spReport.SubsidioEntregado != 0)
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", spReport.SubsidioEntregado.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", "0.00");

                }
                if (spReport.TOTAL_DEDUCCIONES != 0)
                {
                    readHTML = readHTML.Replace("#TOTALD", spReport.TOTAL_DEDUCCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALD", "0.00");

                }
                if (spReport.NETO != 0)
                {
                    readHTML = readHTML.Replace("#NETOP", spReport.NETO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#NETOP", "0.00");

                }
                if (spReport.BaseGravada != 0)
                {
                    readHTML = readHTML.Replace("#TOTALG", spReport.BaseGravada.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALG", "0.00");

                }
                if (spReport.SubsidioEntregado != 0)
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", spReport.SubsidioEntregado.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", "0.00");

                }
                if (spReport.TOTAL_PERCEPCIONES != 0)
                {
                    readHTML = readHTML.Replace("#TOTALP", spReport.TOTAL_PERCEPCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALP", "0.00");

                }

                if (spReport.IMSS_EyM_E != 0)
                {
                    readHTML = readHTML.Replace("#IMS_EyM_E", spReport.IMSS_EyM_E.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#IMS_EyM_E", "0.00");

                }


                if (spReport.IMSS_IyV_E != 0)
                {
                    readHTML = readHTML.Replace("#DOS", spReport.IMSS_IyV_E.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#DOS", "0.00");

                }


                if (spReport.IMSS_CyV_E != 0)
                {
                    readHTML = readHTML.Replace("#TRES", spReport.IMSS_CyV_E.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TRES", "0.00");

                }


                if (spReport.IMSS_OBRERO != 0)
                {
                    readHTML = readHTML.Replace("#CUATRO", spReport.IMSS_OBRERO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#CUATRO", "0.00");

                }


                if (spReport.IMSS_EyM_P != 0)
                {
                    readHTML = readHTML.Replace("#CINCO", spReport.IMSS_EyM_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#CINCO", "0.00");

                }


                if (spReport.IMSS_IyV_P != 0)
                {
                    readHTML = readHTML.Replace("#SEIS", spReport.IMSS_IyV_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SEIS", "0.00");

                }


                if (spReport.IMSS_CyV_P != 0)
                {
                    readHTML = readHTML.Replace("#SIETE", spReport.IMSS_CyV_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SIETE", "0.00");

                }

                if (spReport.IMSS_RT_P != 0)
                {
                    readHTML = readHTML.Replace("#OCHO", spReport.IMSS_RT_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#OCHO", "0.00");

                }

                if (spReport.IMSS_GUA_P != 0)
                {
                    readHTML = readHTML.Replace("#NUEVE", spReport.IMSS_GUA_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#NUEVE", "0.00");

                }

                if (spReport.IMSS_RET_P != 0)
                {
                    readHTML = readHTML.Replace("#DIEZ", spReport.IMSS_RET_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#DIEZ", "0.00");

                }

                if (spReport.INFONAVIT_P != 0)
                {
                    readHTML = readHTML.Replace("#ONCE", spReport.INFONAVIT_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#ONCE", "0.00");

                }
                if (spReport.TotalPatron != 0)
                {
                    readHTML = readHTML.Replace("#TOTALPA", spReport.TotalPatron.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALPA", "0.00");

                }
                if (spReport.ISN != 0)
                {
                    readHTML = readHTML.Replace("#ISN", spReport.ISN.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#ISN", "0.00");

                }

                strReadHTML.AppendLine(readHTML);
            }

            ViewBag.readHTML = strReadHTML.ToString();
            return View();
        }

        /// <summary>
        /// Visualiza reporte de nomina por Centros de costos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reporte de nomina</returns>
        public ActionResult NuevosReportesResumenCC(int id)
        {


            //OBTENER INFORMACION DE LA BASE
            List<sp_CosteoResumenMensualSoloSucursal1_Result> ListSpReport = new List<sp_CosteoResumenMensualSoloSucursal1_Result>();
            using (TadaTimbradoEntities report = new TadaTimbradoEntities())
            {
                ListSpReport = report.sp_CosteoResumenMensualSoloSucursal1(id.ToString()).ToList();
            }


            string path = @"D:\Apps\TadaNomina\TemplateHTML\TemplateNominaCC.html";

            //string path = @"C:\Users\Alberto\OneDrive\Escritorio\Ultimaversion\TadaNomina\TemplateHTML\TemplateNominaCC.html";


            StringBuilder strReadHTML = new StringBuilder();

            foreach (sp_CosteoResumenMensualSoloSucursal1_Result spReport in ListSpReport)
            {
                string readHTML = System.IO.File.ReadAllText(path);
                if (spReport.Sucursal != string.Empty)
                {
                    readHTML = readHTML.Replace("#REGISTROP", spReport.Sucursal);
                }

                if (spReport.PERIODO != string.Empty)
                {
                    readHTML = readHTML.Replace("#PERIODON", spReport.PERIODO);
                }
                if (spReport.NOMINA != string.Empty)
                {
                    readHTML = readHTML.Replace("#NOMINA", spReport.NOMINA);
                }
                if (spReport.NOMINA != string.Empty)
                {
                    readHTML = readHTML.Replace("#NOMINA", spReport.NOMINA);
                }

                if (spReport.SUELDO != 0)
                {
                    readHTML = readHTML.Replace("#SUELDO", spReport.SUELDO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SUELDO", "0.00");

                }

                if (spReport.PRIMA_VACACIONAL != 0)
                {
                    readHTML = readHTML.Replace("#PRIMAV", spReport.PRIMA_VACACIONAL.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#PRIMAV", "0.00");

                }

                if (spReport.VACACIONES != 0)
                {
                    readHTML = readHTML.Replace("#VACACIONES", spReport.VACACIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#VACACIONES", "0.00");

                }
                if (spReport.AGUINALDO != 0)
                {
                    readHTML = readHTML.Replace("#AGUINALDO", spReport.AGUINALDO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#AGUINALDO", "0.00");

                }
                if (spReport.TOTAL_PERCEPCIONES != 0)
                {
                    readHTML = readHTML.Replace("#TOTALP", spReport.TOTAL_PERCEPCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALP", "0.00");

                }

                //

                if (spReport.ImpuestoRetenido != 0)
                {
                    readHTML = readHTML.Replace("#ISR", spReport.ImpuestoRetenido.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#ISR", "0.00");

                }

                if (spReport.IMSS != 0)
                {
                    readHTML = readHTML.Replace("#IMSS", spReport.IMSS.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#IMSS", "0.00");

                }
                if (spReport.OTRAS_PERCEPCIONES != 0)
                {
                    readHTML = readHTML.Replace("#PRESTAMOP", spReport.OTRAS_PERCEPCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#PRESTAMOP", "0.00");

                }
                if (spReport.FONACOT != 0)
                {
                    readHTML = readHTML.Replace("#PRESTAMOF", spReport.FONACOT.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#PRESTAMOF", "0.00");

                }
                if (spReport.INFONAVIT != 0)
                {
                    readHTML = readHTML.Replace("#CREDITOI", spReport.INFONAVIT.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#CREDITOI", "0.00");

                }
                if (spReport.SubsidioEntregado != 0)
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", spReport.SubsidioEntregado.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", "0.00");

                }
                if (spReport.TOTAL_DEDUCCIONES != 0)
                {
                    readHTML = readHTML.Replace("#TOTALD", spReport.TOTAL_DEDUCCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALD", "0.00");

                }
                if (spReport.NETO != 0)
                {
                    readHTML = readHTML.Replace("#NETOP", spReport.NETO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#NETOP", "0.00");

                }
                if (spReport.BaseGravada != 0)
                {
                    readHTML = readHTML.Replace("#TOTALG", spReport.BaseGravada.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALG", "0.00");

                }
                if (spReport.SubsidioEntregado != 0)
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", spReport.SubsidioEntregado.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SUBSIDIOE", "0.00");

                }
                if (spReport.TOTAL_PERCEPCIONES != 0)
                {
                    readHTML = readHTML.Replace("#TOTALP", spReport.TOTAL_PERCEPCIONES.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTALP", "0.00");

                }

                if (spReport.IMSS_EyM_E != 0)
                {
                    readHTML = readHTML.Replace("#IMS_EyM_E", spReport.IMSS_EyM_E.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#IMS_EyM_E", "0.00");

                }


                if (spReport.IMSS_IyV_E != 0)
                {
                    readHTML = readHTML.Replace("#DOS", spReport.IMSS_IyV_E.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#DOS", "0.00");

                }


                if (spReport.IMSS_CyV_E != 0)
                {
                    readHTML = readHTML.Replace("#TRES", spReport.IMSS_CyV_E.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TRES", "0.00");

                }


                if (spReport.IMSS_OBRERO != 0)
                {
                    readHTML = readHTML.Replace("#CUATRO", spReport.IMSS_OBRERO.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#CUATRO", "0.00");

                }


                if (spReport.IMSS_EyM_P != 0)
                {
                    readHTML = readHTML.Replace("#CINCO", spReport.IMSS_EyM_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#CINCO", "0.00");

                }


                if (spReport.IMSS_IyV_P != 0)
                {
                    readHTML = readHTML.Replace("#SEIS", spReport.IMSS_IyV_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SEIS", "0.00");

                }


                if (spReport.IMSS_CyV_P != 0)
                {
                    readHTML = readHTML.Replace("#SIETE", spReport.IMSS_CyV_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#SIETE", "0.00");

                }

                if (spReport.IMSS_RT_P != 0)
                {
                    readHTML = readHTML.Replace("#OCHO", spReport.IMSS_RT_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#OCHO", "0.00");

                }

                if (spReport.IMSS_GUA_P != 0)
                {
                    readHTML = readHTML.Replace("#NUEVE", spReport.IMSS_GUA_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#NUEVE", "0.00");

                }

                if (spReport.IMSS_RET_P != 0)
                {
                    readHTML = readHTML.Replace("#DIEZ", spReport.IMSS_RET_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#DIEZ", "0.00");

                }

                if (spReport.INFONAVIT_P != 0)
                {
                    readHTML = readHTML.Replace("#ONCE", spReport.INFONAVIT_P.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#ONCE", "0.00");

                }
                if (spReport.TotalPatron != 0)
                {
                    readHTML = readHTML.Replace("#TOTPA", spReport.TotalPatron.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#TOTPA", "0.00");

                }
                if (spReport.ISN != 0)
                {
                    readHTML = readHTML.Replace("#ISN", spReport.ISN.ToString());
                }
                else
                {
                    readHTML = readHTML.Replace("#ISN", "0.00");

                }


                strReadHTML.AppendLine(readHTML);
            }

            ViewBag.readHTML = strReadHTML.ToString();
            return View();
        }

        /// <summary>
        /// Descarga reporte de monina (Resumen) en formato Excel
        /// </summary>
        /// <param name="GridHtml">Tabla en HTML</param>
        /// <returns>Descarga formato de Excel</returns>
        [HttpPost]
        [ValidateInput(false)]
        public FileResult NuevosReportesResumen(string GridHtml)
        {

            return File(Encoding.Default.GetBytes(GridHtml), "application/vnd.ms-excel", "Ambiente_de_trabajo.xls");
        }

        /// <summary>
        /// Descarga reporte de nomina por centro de costos en formato excel
        /// </summary>
        /// <param name="GridHtml">Tabla en HTML</param>
        /// <returns>Reporte de nomina por centros de costos en formato excel </returns>
        [HttpPost]
        [ValidateInput(false)]
        public FileResult NuevosReportesResumenCC(string GridHtml)
        {

            return File(Encoding.Default.GetBytes(GridHtml), "application/vnd.ms-excel", "Ambiente_de_trabajo.xls");
        }

        /// <summary>
        /// Reporte de nomina por centros de costos y sucursales
        /// </summary>
        /// <param name="id">Periodo de nomina</param>
        /// <returns>Reporte de nomina por centros de costos y sucursales en formato Excel</returns>
        public FileResult DescargarCC(int id)
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];

            cReportesNomina cempleados = new cReportesNomina();
            DataTable dt = cempleados.GetDataTableForNominaCC(id);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Grid");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        /// <summary>
        /// Crea un formato de archivos ZIP
        /// </summary>
        /// <param name="items">Archivo</param>
        /// <param name="destination">Destino</param>
        private void CreateZipFile(List<string> items, string destination)
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
                zip.Save(destination);
            }
        }

        /// <summary>
        /// Formulario para descargar reporte de altas, bajas por fehcas de empleados
        /// </summary>
        /// <returns>Reporte lleno</returns>
        public ActionResult RepAltasBajasByFechas()
        {
            cReportesNomina reportes = new cReportesNomina();
            ModelRepByFechasTM m = reportes.getModelAltasBajasEmpleados();
            return View(m);
        }

        /// <summary>
        /// Reoirte de altas y bajas por rango de fechas
        /// </summary>
        /// <param name="m">Tipo de reporte altas o bajas</param>
        /// <returns>Reporte de empleados</returns>
        public FileResult DescargarRepAltasBajasByFechas(ModelRepByFechasTM m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fIncial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = m.tipoMovimiento;

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableAltasBajasEmpleados(IdUnidadNegocio,fInicial, fFinal,seleccion);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Reporte de saldos
        /// </summary>
        /// <returns>Saldos</returns>
        public ActionResult SaldosPorCliente()
        {
            int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());
            cReportesNomina cReporte = new cReportesNomina();
            var s = cReporte.ListaSaldosPorCliente(IdUsuario);

            return View(s);
        }

        /// <summary>
        /// Resumen de reporte de saldos
        /// </summary>
        /// <param name="IdFacturadora">Facturadora</param>
        /// <returns>Resumen de saldos por facturadora</returns>
        public ActionResult DetalleSaldosCliente(int IdFacturadora)
        {
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            cReportesNomina cRep = new cReportesNomina();
            var d = cRep.DetalleSaldosPorCliente(IdCliente, IdFacturadora);

            return View(d);
        }

        /// <summary>
        /// formulario para obtener reporte de ISN por rango de fechas
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult ReporteISN()
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }

        /// <summary>
        /// Reporte de ISN por rango de fechas
        /// </summary>
        /// <param name="m">Rango de fechas</param>
        /// <returns>Reporte en formato Excel</returns>
        public FileResult DescargarReporteISN(ModelReporteByFechas m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = "ReporteISN_";

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteISN(IdUnidadNegocio, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Reporte de incidencia generadas dentro de un rango de fechas
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult ReporteIncidenciasByClienteFechas()
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }

        /// <summary>
        /// Reporte de incidencias generadas por rango de fechas
        /// </summary>
        /// <param name="m">Rango de fechas</param>
        /// <returns>Reporte de incidencias en formato Excel</returns>
        public FileResult DescargarReporteIncidenciasByClienteFechas(ModelReporteByFechas m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = "ReporteIncidenciasClienteFechas_";

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteIncidenciasByClienteFechas(IdCliente, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Formulario para descargar reporte de nomina acumulada por empleado
        /// </summary>
        /// <returns>Formulario de rango de fechas</returns>
        public ActionResult ReporteNominaAcumuladoPorEmpleado() 
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }

        /// <summary>
        /// Reporte de nomina acumulada por empleado
        /// </summary>
        /// <param name="m">rango de fechas</param>
        /// <returns>Reporte de nominas acumuladas por empleado</returns>
        public FileResult DescargarReporteNominaAcumuladoPorEmpleado(ModelReporteByFechas m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = "ReporteNominaAcumuladoPorEmpleado_";

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteNominaAcumuladoPorEmpleado(IdCliente, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// formulario de rango de fechas para descargar reporte especial agrupado por Centros de costos en un rango de fechas
        /// </summary>
        /// <returns>Formulario de rango de fechas</returns>
        public ActionResult ReporteEspecialAgrupadoPorCC()
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }

        /// <summary>
        /// Reporte especial agrupado por centros de costos
        /// </summary>
        /// <param name="m">Rango de fechas</param>
        /// <returns>Reporte en formato Excel</returns>
        public FileResult DescargarReporteEspecialAgrupadoPorCC(ModelReporteByFechas m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdCLiente = int.Parse(Session["sIdCliente"].ToString());
            string nomina = Session["sNomina"].ToString();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = "ReporteAcumuladoByCC_";

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteEspecialAgrupadoPorCC(fInicial, fFinal, IdUnidadNegocio);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Formulario para descargar reporte de nominas acumuladas por rango de fechas
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult ReporteAcumuladoByIdCliente()
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }

        /// <summary>
        /// Reporte de nominas acumuladas por rango de fechas
        /// </summary>
        /// <param name="m">Rango de fechas</param>
        /// <returns>Reporte de nominas acumuladas en formato Excel</returns>
        public FileResult DescargarReporteAcumuladoByIdCliente(ModelReporteByFechas m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = "ReporteAcumulado_";

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteAcumualdoByCliente(IdCliente, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Formulario para reporte de facuración por rango de fechas
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult ReporteFacturacionByIdCliente()
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }

        /// <summary>
        /// Descarga reporte de fecturacion por rango de fechas
        /// </summary>
        /// <param name="m">Rango de fechas</param>
        /// <returns>Reporte de facturación por rango de fechas</returns>
        public FileResult DescargarReporteFacturacionByIdCliente(ModelReporteByFechas m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = "ReporteFacturacion_";

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteFacturacionByCliente(IdCliente, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Descarga reporte de empleados administrados
        /// </summary>
        /// <returns>Reporte de empleados administrados</returns>
        public FileResult DescargarReporteEmpleadosAdministradosByIdUsuario()
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());
            string seleccion = "ReporteEmpleadosAdministrados.xlsx";

            string nombreArchivo = seleccion;
            DataTable dt = reportes.GetDataTableReporteEmpleadosAdministradosByUsuario(IdUsuario);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Formulario de reportes timbrados o no por rango de fechas
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult ReporteTimbrado()
        {
            cReportesNomina reportes = new cReportesNomina();
            ModelRepByFechasTM m = reportes.GetModelReporteTimbrado();
            return View(m);
        }

        /// <summary>
        /// Reporte de timbrado por rango de fechas
        /// </summary>
        /// <param name="m">Tipo de reporte, timbrado o no timbrado por rango de fechas</param>
        /// <returns>Reporte de timbrado</returns>
        public FileResult DescargarReporteTimbrado(ModelRepByFechasTM m)
        {
            cReportesNomina reportes = new cReportesNomina();
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            string nomina = Session["sNomina"].ToString();
            DateTime fInicial = DateTime.Parse(m.fIncial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            string seleccion = m.tipoMovimiento;

            string nombreArchivo = reportes.RegresaNombreArchivo(seleccion, nomina, fInicial, fFinal);
            DataTable dt = reportes.GetDataTableReporteTimbrado(IdUnidadNegocio, fInicial, fFinal, seleccion);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }



        public ActionResult AusentimosReporte()
        {
            ModelReporteByFechas m = new ModelReporteByFechas();
            return View(m);
        }



        public FileResult AusentimosReportes(ModelReporteByFechas m)
        {
            int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
            cReportesNomina reportes = new cReportesNomina();
            DateTime fInicial = DateTime.Parse(m.fInicial);
            DateTime fFinal = DateTime.Parse(m.fFinal);
            DataTable dt = reportes.GetDataTableAusentimos(IdUnidadNegocio, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ausentismos.xls");
                }
            }
        }


        /// <summary>
        /// Reporte de de nomina por periodo
        /// </summary>
        /// <param name="m">Tipo de reporte, timbrado o no timbrado por rango de fechas</param>
        /// <returns>Reporte de de nomina por periodo</returns>

        public ActionResult RepNominaByIdPeriodoNominaAcumulado()
        {
            cReportesNomina cReportes = new cReportesNomina();
            ModelReporteByIdPeriodo model = cReportes.GetModelReporteByIdPeriodo((int)Session["sIdUnidadNegocio"]);

            return View(model);
        }


        /// <summary>
        /// Metodo para descargar el Excel del reporte contable por periodo de nomina
        /// </summary>
        /// <param name="model">Modelo que conetiene el Id del Periodo de NOmina</param>
        /// <returns></returns>
        public FileResult DescargarRepByIdPeriodoAcumulado(ModelReporteByIdPeriodo model)
        {
            cReportesNomina rnbyperiodo = new cReportesNomina();

            string nombreArchivo = rnbyperiodo.RegresaNombreReporte(model.IdPeriodoNomina);
            DataTable dt = rnbyperiodo.GetDataTableForReporteByIdPeriodoNomina(model.IdPeriodoNomina);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "ReporteContable");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

    }
}