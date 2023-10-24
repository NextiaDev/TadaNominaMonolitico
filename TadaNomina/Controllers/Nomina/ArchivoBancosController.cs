using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    /// <summary>
    /// Controlador para la descarga de archivo de bancos.
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 23/05/2022, Razón: documentar el codigo
    /// </summary>
    public class ArchivoBancosController : BaseController
    {
        /// <summary>
        /// Acción para selección del periodo de nómina.
        /// </summary>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult SelecionaPeriodo()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominas(IdUnidadNegocio);

                return View(periodos);
            }
        }

        /// <summary>
        /// Accion para descargar el reporte de bancos con las cuentas bancarias de los trabajadores para su pago tradicional.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Archivo de bancos en formato excel.</returns>
        public FileResult DescargarTradicional(int pIdPeriodoNomina)
        {
            ClassArchivosBancos cempleados = new ClassArchivosBancos();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            DataTable dt = cempleados.GetTableTradicional(IdUnidad, pIdPeriodoNomina);

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
        /// Accion para descargar el reporte de bancos con las cuentas bancarias de los trabajadores para su pago esquema.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Archivo de bancos en formato excel.</returns>
        public FileResult DescargarEsquema(int pIdPeriodoNomina)
        {
            ClassArchivosBancos cempleados = new ClassArchivosBancos();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            DataTable dt = cempleados.GetTableEsquema(IdUnidad, pIdPeriodoNomina);

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

        public FileResult SCuenta(int pIdPeriodoNomina)
        {
            ClassArchivosBancos cempleados = new ClassArchivosBancos();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            DataTable dt = cempleados.GetTableSn(IdUnidad, pIdPeriodoNomina);

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


        public FileResult DescargarHonorario(int pIdPeriodoNomina)
        {
            ClassArchivosBancos cempleados = new ClassArchivosBancos();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            DataTable dt = cempleados.GetTableHonorarios(IdUnidad, pIdPeriodoNomina);

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