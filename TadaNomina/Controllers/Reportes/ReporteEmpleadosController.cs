using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteEmpleadosController : BaseController
    {
        // GET: ReporteEmpleados

        /// <summary>
        /// Muestra los tipos de reportes que se pueden generar de los empleados
        /// </summary>
        /// <returns>Tipo de rportes</returns>
        public ActionResult Index()
        {
            string Cliente = Session["sCliente"].ToString();
            string Unidad = Session["sNomina"].ToString();
            ClassReportesEmpleados cempleados = new ClassReportesEmpleados();
            var model = cempleados.GetListReportes(Cliente, Unidad);
            return View(model);
        }

        /// <summary>
        /// Descarga el tipo de reporte que se haya seleccionado
        /// </summary>
        /// <param name="id">Tipo de reporte</param>
        /// <returns>Reporte de empleados</returns>
        public FileResult Descargar(int id)
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
            ClassReportesEmpleados cempleados = new ClassReportesEmpleados();
            DataTable dt = cempleados.GetDataTableForEmpleados(IdUnidadnegocio, id);
            
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
}
