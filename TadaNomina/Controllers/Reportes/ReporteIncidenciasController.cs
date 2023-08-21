using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteIncidenciasController : BaseController
    {
        // GET: ReporteIncidencias

        /// <summary>
        /// Muestra los periodos acumulados para poder descargar reporte de incidencias
        /// </summary>
        /// <returns>Periodos acumulados</returns>
        public ActionResult Index()
        {
            var cReportes = new cReportesNomina();
            var model = cReportes.GetListaPeriodosAcumulados((int)Session["sIdUnidadNegocio"]);

            return View(model);
        }

        /// <summary>
        /// Descarga el reporte de incidencias por reporte seleccionado
        /// </summary>
        /// <param name="id">Reporte seleccionado a descagar</param>
        /// <returns>Reporte de incidencias de un periodo acumulado</returns>
        public FileResult Descargar(int id)
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];

            var cempleados = new cReportesNomina();
            var dt = cempleados.GetDataTableForIncidencias(id);
                       
            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Grid");
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }
    }
}