using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.IMSS;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteIMSSController : BaseController
    {
        // GET: ReporteIMSS

        /// <summary>
        /// Formulario para obtener un reporte por los movimientos ante el imss
        /// </summary>
        /// <returns>Formuladio de llenado</returns>
        public ActionResult IDSE()
        {
            ClassReportesIMSS cri = new ClassReportesIMSS();
            var model = cri.getModelIDSE();
            return View(model);
        }

        /// <summary>
        /// Descarga reporte de movimientos seleccionados ante el imss
        /// </summary>
        /// <param name="model">Tipo de movimiento</param>
        /// <returns>Reporte de movimientos</returns>
        public FileResult DescargarIDSE(ModelIDSE model)
        {

            ClassIMSS imss = new ClassIMSS();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            DateTime fInicial = DateTime.Parse(model.fIncial);
            DateTime fFinal = DateTime.Parse(model.fFinal);

            string text = imss.RegresaMovimientosIDSE(IdCliente, fInicial, fFinal, model.tipoMovimiento);
            string nombreArchivo = imss.RegresaNombreArchivo(fInicial, fFinal, model.tipoMovimiento);

            var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

            return File(stream, "text/plain", nombreArchivo);

        }

        /// <summary>
        /// Formulario para limitar las fechas del reporte de SUA por tipo de movimientos
        /// </summary>
        /// <returns>Formulario para obtener reporte</returns>
        public ActionResult SUA()
        {
            ClassReportesIMSS cri = new ClassReportesIMSS();
            var model = cri.getModelIDSE();
            return View(model);
        }

        /// <summary>
        /// Reporte de movimientos (SUA) hechos por tipo y fechas seleccionadas
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Reporte de SUA</returns>
        public FileResult DescargarSUA(ModelIDSE model)
        {
            ClassIMSS imss = new ClassIMSS();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            DateTime fInicial = DateTime.Parse(model.fIncial);
            DateTime fFinal = DateTime.Parse(model.fFinal);

            string text = imss.RegresaMovimientosSUA(IdCliente, fInicial, fFinal, model.tipoMovimiento);
            string nombreArchivo = imss.RegresaNombreArchivoSUA(fInicial, fFinal, model.tipoMovimiento);

            var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

            return File(stream, "text/plain", nombreArchivo);
        }

        /// <summary>
        /// Formulario para obtener la variabilidad
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult Variabilidad()
        {
            ModelReporteByFechas model = new ModelReporteByFechas();
            return View(model);
        }

        /// <summary>
        /// Reporte de variabilidad
        /// </summary>
        /// <param name="model">Formulario lleno con limitaciones de las fechas</param>
        /// <returns></returns>
        public FileResult DescargarVariabilidad(ModelReporteByFechas model)
        {
            int IdCliente = (int)Session["sIdCliente"];
            ClassIMSS imss = new ClassIMSS();
            DateTime fInicial = DateTime.Parse(model.fInicial);
            DateTime fFinal = DateTime.Parse(model.fFinal);

            string nombreArchivo = imss.RegresaNombreArchivoVariabilidad(fInicial, fFinal);
            DataTable dt = imss.GetDataTableForVariabilidad(IdCliente, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Variabilidad");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }

        /// <summary>
        /// Formulario para obtener reporte de recuperaciones IMSS
        /// </summary>
        /// <returns>Formulario</returns>
        public ActionResult RecuperacionesIMSS()
        {
            ModelReporteByFechas model = new ModelReporteByFechas();
            return View(model);
        }

        /// <summary>
        /// Reporte de las recuperaciones IMSS
        /// </summary>
        /// <param name="model">Formulario delimitando las fechas del reporte</param>
        /// <returns>Reporte de recuperaciones IMSS</returns>
        public FileResult DescargarRecuperacionesIMSS(ModelReporteByFechas model)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassIMSS imss = new ClassIMSS();
            DateTime fInicial = DateTime.Parse(model.fInicial);
            DateTime fFinal = DateTime.Parse(model.fFinal);

            string nombreArchivo = imss.RegresaNombreArchivoRecuperaciones(fInicial, fFinal);
            DataTable dt = imss.GetDataTableRecuperacionesIMSS(IdUnidadNegocio, fInicial, fFinal);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Recuperaciones IMSS");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }
    }
}