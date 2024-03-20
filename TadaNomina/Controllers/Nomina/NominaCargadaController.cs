using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.ClassCore.NominasCargadas;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Controllers.Nomina
{
    public class NominaCargadaController : BaseController
    {
        /// <summary>
        /// Método para cargara la vista de la carga de nominas proporcionadas.
        /// </summary>
        /// <param name="pIdPeriodoNomina"></param>
        /// <returns></returns>
        public ActionResult CargaNomina(int pIdPeriodoNomina)
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                ClassNominasCargadas classNomina = new ClassNominasCargadas();

                ModelNominasCargadas modelo = classNomina.GetModelNominasCargadas(classPeriodoNomina.GetvPeriodoNominasId(pIdPeriodoNomina));

                if (Session["sTipoUsuario"].ToString() == "System")
                    modelo.Periodo += " (" + modelo.IdPeriodoNomina + ")";

                return View(modelo);
            }
        }

        /// <summary>
        /// Acción que regresa un archivo a la vista.
        /// </summary>
        /// <param name="Id">Identificador del periodo de nómina.</param>
        /// <returns>Archivoa la vista.</returns>
        public FileResult Descargar(int Id)
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
            cReportesNomina crn = new cReportesNomina();
            ClassReportesNomina cempleados = new ClassReportesNomina();
            DataTable dt = cempleados.GetTableNomina(Id);


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
        /// Método para descargar en Excel el layout para la carga de nominas proporcionadas incluye la parte de la nomina y los conceptos.
        /// </summary>
        /// <returns></returns>
        public FileResult Excel()
        {
            int IdCliente = (int)Session["sIdCliente"];

            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado initEmpleado = new ClassEmpleado();

            var arch = ExcelIncidencias(IdUnidadNegocio, IdCliente);

            return File(arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CargaNomina.xlsx");
        }

        /// <summary>
        /// Método para obtner el layout para cargar nominas proporcionadas
        /// </summary>
        /// <param name="idunidadnegocio">Identificador de la unidad de negocio</param>
        /// <param name="idcliente">Identificador del cliente</param>
        /// <returns></returns>
        public byte[] ExcelIncidencias(int idunidadnegocio, int idcliente)
        {

            TadaEmpleados entities = new TadaEmpleados();
            var insi = (from b in entities.Cat_ConceptosNomina.Where(x => x.IdEstatus == 1 && x.IdCliente == idcliente && x.TipoEsquema != "Esquema" && x.TipoDato == "Pesos") select b).ToList();

            List<string> ListFirstRow = new List<string>();
            List<string> ListSecondRow = new List<string>();

            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");

            ListSecondRow.Add("ClaveEmpleado");
            ListSecondRow.Add("RFC");
            ListSecondRow.Add("Nombre");
            ListSecondRow.Add("ApellidoPaterno");
            ListSecondRow.Add("ApellidoMaterno");
            ListSecondRow.Add("Sueldo");
            ListSecondRow.Add("Subsidio");
            ListSecondRow.Add("TotalPercepciones");
            ListSecondRow.Add("ISR");
            ListSecondRow.Add("IMSS");
            ListSecondRow.Add("TotalDeducciones");
            ListSecondRow.Add("Neto");
            ListSecondRow.Add("ISN");

            foreach (var item in insi)
            {
                ListFirstRow.Add(item.IdConcepto.ToString());
                ListSecondRow.Add(item.ClaveConcepto + "-" + item.Concepto);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var xl = wb.Worksheets.Add("CargaNomina");
                int i = 1;
                foreach (string column in ListFirstRow)
                {
                    xl.Cell(1, i).Value = column;

                    if (i <= 13)
                    {
                        xl.Cell(1, i).Style.Fill.BackgroundColor = XLColor.Red;
                    }
                    else
                    {
                        xl.Cell(1, i).Style.Fill.BackgroundColor = XLColor.BlueBell;
                    }
                    xl.Cell(1, i).Style.Font.FontColor = XLColor.White;
                    xl.Cell(1, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    i++;
                }

                i = 1;
                foreach (string column in ListSecondRow)
                {
                    xl.Cell(2, i).Value = column;
                    if (i <= 13)
                    {
                        xl.Cell(2, i).Style.Fill.BackgroundColor = XLColor.Red;

                    }
                    else
                    {
                        xl.Cell(2, i).Style.Fill.BackgroundColor = XLColor.BlueBell;
                    }
                    xl.Cell(2, i).Style.Font.FontColor = XLColor.White;
                    xl.Cell(2, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    i++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// Método para presentar la pantalla en donde se carga el layout de las nominas proporcionadas.
        /// </summary>
        /// <param name="pIdPeriodoNomina"></param>
        /// <param name="TipoPeriodo"></param>
        /// <returns></returns>
        public ActionResult CargaLayoutNomina(int pIdPeriodoNomina, string TipoPeriodo)
        {
            ViewData["IdPeriodoNomina"] = pIdPeriodoNomina;
            ViewBag.Periodo = pIdPeriodoNomina;
            ViewBag.TipoPeriodo = TipoPeriodo;

            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ModelCargaLayoutNominas modelo = new ModelCargaLayoutNominas();
            modelo.IdPeriodoNomina = pIdPeriodoNomina;
            modelo.TipoPeriodo = TipoPeriodo;
            return View(modelo);
        }

        [HttpPost]
        public ActionResult CargaLayoutNomina(ModelCargaLayoutNominas model)
        {
            try
            {
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];

                ViewBag.Periodo = model.IdPeriodoNomina;
                ViewBag.TipoPeriodo = model.TipoPeriodo;

                ClassIncidencias cincidencias = new ClassIncidencias();
                ModelIncidencias modelo = cincidencias.LlenaListasIncidencias(IdUnidad, IdCliente);
                ClassNominasCargadas classNomina = new ClassNominasCargadas();

                if (ModelState.IsValid)
                {
                    if (model.Archivo.ContentLength > 0)
                    {

                        string fileName = Path.GetFileName(model.Archivo.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                        model.Archivo.SaveAs(_path);

                        classNomina.GetIncidenciasNominasCargadas(_path, IdCliente, IdUnidad, model.IdPeriodoNomina, model.TipoPeriodo, IdUsuario);
                        ViewBag.Finalizado = "SI";
                    }
                }

                ModelCargaLayoutNominas m = new ModelCargaLayoutNominas();
                m.IdPeriodoNomina = model.IdPeriodoNomina;
                m.TipoPeriodo = model.TipoPeriodo;
                m.validacion = true;
                m.Mensaje = "Se guardo correctamente la Nómina";

                return View(m);
            }
            catch (Exception ex)
            {
                model.validacion = false;
                model.Mensaje = "No se guardo correctamente la Nómina";
                return View(model);
            }
        }

        /// <summary>
        /// Accion para borrar la nómina cargada del periodo.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Confirmación a la vista.</returns>
        public ActionResult DeleteAllNomina(int pIdPeriodoNomina)
        {
            try
            {
                ClassNominasCargadas cnomina = new ClassNominasCargadas();
                cnomina.DeleteNominaTrabajo(pIdPeriodoNomina);

                return RedirectToAction("CargaNomina", new { pIdPeriodoNomina });
            }
            catch
            {
                return RedirectToAction("CargaNomina", new { pIdPeriodoNomina });
            }

        }
    }
}