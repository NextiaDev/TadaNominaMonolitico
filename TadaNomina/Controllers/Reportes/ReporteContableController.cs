using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Contabilidad;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteContableController : BaseController
    {
        /// <summary>
        /// Crea un modelo de ModelReporteByFechas
        /// </summary>
        /// <returns>Envia un Modelo de ModelReporteByFechas a la vista</returns>
        public ActionResult RepContableByFechas()
        {
            ModelReporteByFechas model = new ModelReporteByFechas();
            return View(model);
        }

        /// <summary>
        /// Descarga un reporte contable con base en un rango de fechas
        /// </summary>
        /// <param name="model">Rango de fechas</param>
        /// <returns>Reporte contable</returns>
        public FileResult DescargarRepContable(ModelReporteByFechas model)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassContabilidad conta = new ClassContabilidad();
            DateTime fInicial = DateTime.Parse(model.fInicial);
            DateTime fFinal = DateTime.Parse(model.fFinal);

            string nombreArchivo = conta.RegresaNombreReporteContable(fInicial, fFinal);
            DataTable dt = conta.GetDataTableForReporteConatble(IdUnidadNegocio, fInicial, fFinal);

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
        public ActionResult RepContableByIdPeriodoNomina()
        {
            ClassContabilidad contabilidad = new ClassContabilidad();
            ModelReporteByIdPeriodo model = contabilidad.GetModelReporteByIdPeriodo(int.Parse(Session["sIdUnidadNegocio"].ToString()));
            return View(model);
        }
        /// <summary>
        /// Metodo para descargar el Excel del reporte contable por periodo de nomina
        /// </summary>
        /// <param name="model">Modelo que conetiene el Id del Periodo de NOmina</param>
        /// <returns></returns>
        public FileResult DescargarRepContableByIdPeriodo(ModelReporteByIdPeriodo model)
        {
            ClassContabilidad conta = new ClassContabilidad();

            string nombreArchivo = conta.RegresaNombreReporteContable(model.IdPeriodoNomina);
            DataTable dt = conta.GetDataTableForReporteContableByIdPeriodoNomina(model.IdPeriodoNomina);

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
