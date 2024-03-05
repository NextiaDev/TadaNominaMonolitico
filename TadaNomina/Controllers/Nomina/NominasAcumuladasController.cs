using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Models.ViewModels.Reportes;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Nomina
{
    public class NominasAcumuladasController : BaseController
    {
        // GET: NominasAcumuladas
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();

            return View(periodos);           
        }
        /// <summary>
        /// Accion que muestra los datos generales del periodo de nómina.
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <returns></returns>
        public ActionResult NominaGeneral(int IdPeriodoNomina)
        {
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            ClassNomina classNomina = new ClassNomina();

            ModelProcesaNominaGeneral modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(IdPeriodoNomina));
            return View(modelo);
        }

        /// <summary>
        /// Accion para cargar los elementos necesarios para la busqueda de un empleado.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult BuscarEmpleadoIndividual(int pIdPeriodoNomina)
        {
            ViewBag.IdPeriodoProcesar = pIdPeriodoNomina;
            return View();
        }

        /// <summary>
        /// Accion que regresa la información resultante de la busqueda de un empleado.
        /// </summary>
        /// <param name="inpBuscar">Clave del empleado a buscar</param>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Resultados de la busqueda del empleado a la vista.</returns>
        [HttpPost]
        public ActionResult BuscarEmpleadoIndividual(string inpBuscar, string pIdPeriodoNomina)
        {
            try
            {
                ViewBag.IdPeriodoProcesar = pIdPeriodoNomina;
                if (ModelState.IsValid)
                {
                    int IdUnidad = (int)Session["sIdUnidadNegocio"];
                    var classNomina = new ClassNomina();
                    var cpn = new ClassPeriodoNomina();
                    var periodo = cpn.GetPeriodo(int.Parse(pIdPeriodoNomina));
                    int[] status = { 1, 3 };

                    var emp_ = classNomina.GetvEmpeladosByClaveByIdUnidadnegocio(inpBuscar, IdUnidad);
                    vEmpleados emp = new vEmpleados();

                    if (periodo.TipoNomina != "Complemento")
                        emp = emp_.Where(x => status.Contains(x.IdEstatus)).FirstOrDefault();
                    else
                        emp = emp_.FirstOrDefault();

                    if (emp == null)
                    {
                        var nomEmp = classNomina.GetvNominaTrabajo(inpBuscar, int.Parse(pIdPeriodoNomina));
                        if (nomEmp != null)
                            emp = classNomina.GetvEmpeladosByClaveByIdUnidadnegocio(nomEmp.IdEmpleado);
                    }

                    if (emp != null)
                    {
                        return RedirectToAction("NominaIndividual", new { emp.IdEmpleado, IdPeriodoNomina = pIdPeriodoNomina });
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        /// <summary>
        /// Accion que carga la información necesaria para el proceso individual de nómina.
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado a procesar</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns></returns>
        public ActionResult NominaIndividual(int IdEmpleado, int IdPeriodoNomina)
        {
            ClassNomina classNomina = new ClassNomina();
            int IdUsuario = (int)Session["sIdUsuario"];

            var modelo = classNomina.GetModelNominaIndividualAcumulado(IdEmpleado, IdPeriodoNomina, "Nomina");

            return View(modelo);
        }

        /// <summary>
        /// Acción que regresa un archivo a la vista.
        /// </summary>
        /// <param name="Id">Identificador del archivo.</param>
        /// <returns>Archivoa la vista.</returns>
        public FileResult Descargar(int Id)
        {
            int IdUnidadnegocio = (int)Session["sIdUnidadNegocio"];
            cReportesNomina crn = new cReportesNomina();
            ClassReportesNomina cempleados = new ClassReportesNomina();
            DataTable dt = cempleados.GetTableNomina(Id);

            if (Session["sIdCliente"].ToString() == "123")
            {
                DataTable dt2 = crn.DatosProcesados(dt);
                DataTable info = crn.InfoDatosProcesados(Id, IdUnidadnegocio);
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
    }
}
