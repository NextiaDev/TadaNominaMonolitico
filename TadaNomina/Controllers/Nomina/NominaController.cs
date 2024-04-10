using ClosedXML.Excel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using TadaNomina.Models;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.CalculoFiniquito;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.ClassCore.Reportes;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Models.ViewModels.Reportes;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Nomina
{
    /// <summary>
    /// Controlador del Proceso Nómina
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: documentar el codigo
    /// </summary>
    public class NominaController : BaseController
    {
        // GET: Nomina
        public ActionResult Index()
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
        /// Accion principal que carga la pantalla proceso de nómina.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult ProcesaNominaGeneral(int pIdPeriodoNomina)
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                ClassNomina classNomina = new ClassNomina();

                ModelProcesaNominaGeneral modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(pIdPeriodoNomina));

                if (Session["sTipoUsuario"].ToString() == "System")
                    modelo.Periodo += " (" + modelo.IdPeriodoNomina + ")";

                return View(modelo);
            }
        }

        /// <summary>
        /// Accion resultante del proceso de nómina.
        /// </summary>
        /// <param name="model">Objeto con la información proporcionada desde la vista.</param>
        /// <returns>Resultados del proceso de nómina a la vista</returns>
        [HttpPost]
        public ActionResult ProcesaNominaGeneral(ModelProcesaNominaGeneral model)
        {
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            ClassNomina classNomina = new ClassNomina();
            int IdUsuario = (int)Session["sIdUsuario"];

            ModelProcesaNominaGeneral modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(model.IdPeriodoNomina));

            try
            {
                ClassCalculoNomina cCalculo = new ClassCalculoNomina();
                cCalculo.Procesar(model.IdPeriodoNomina, null, IdUsuario);
                modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(model.IdPeriodoNomina));

                modelo.validacion = true;
                modelo.Mensaje = "La nomina se proceso de manera correcta, se procesaron " + cCalculo.contador + " empleados";
            }
            catch (Exception ex)
            {
                modelo.validacion = false;
                modelo.Mensaje = "No se pudo procesar la nomina, error: " + ex.Message;
            }

            return View(modelo);
        }

        [HttpPost]
        public JsonResult ActualizaNetos(string valores)
        { 
            try
            {                
                ClassActualizaNetos can = new ClassActualizaNetos();
                can.ActualizaNetosString(valores);
                return Json(new { result = "Ok", mensaje = "Se actualizaron los netos de forma correcta." });   
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Metodo para definir si se inserta automaticamente la incidencia de aguinaldo.
        /// </summary>
        /// <param name="AguinaldoSINO">true/false para insertar incidencia.</param>
        /// <returns>Resultado si se logro el cambio.</returns>
        [HttpPost]
        public JsonResult getAguinaldoAutomatico(bool AguinaldoSINO)
        {
            
            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int IdUsuario = (int)Session["sIdUsuario"];

            int? IncidenciasAguinaldoAutomaticas = null;
            if (AguinaldoSINO)
                IncidenciasAguinaldoAutomaticas = 1;
            else
                IncidenciasAguinaldoAutomaticas = null;

            try { 
                cu.ActualizaBanderaIncidenciasAguinaldo(IdUnidadNegocio, IncidenciasAguinaldoAutomaticas, IdUsuario);
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex) {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
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
                        if(nomEmp != null)
                            emp = classNomina.GetvEmpeladosByClaveByIdUnidadnegocio(nomEmp.IdEmpleado);
                    }

                    if (emp != null)
                    {
                        return RedirectToAction("ProcesaNominaIndividual", new { emp.IdEmpleado, IdPeriodoNomina = pIdPeriodoNomina });
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
            catch(Exception)
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
        public ActionResult ProcesaNominaIndividual(int IdEmpleado, int IdPeriodoNomina)
        {
            ClassNomina classNomina = new ClassNomina();
            int IdUsuario = (int)Session["sIdUsuario"];

            var modelo = classNomina.GetModelNominaIndividual(IdEmpleado, IdPeriodoNomina, "Nomina");

            return View("ProcesaNominaIndividual", modelo);
        }

        /// <summary>
        /// Accion que carla la informacionn resultante del proceso de nomina individual.
        /// </summary>
        /// <param name="model">Datos cargados por el usuario desde la vista.</param>
        /// <returns>Informacionn resultante del proceso de nomina individual</returns>
        [HttpPost]
        public ActionResult ProcesaNominaIndividual(ModelNominaIndividual model)
        {
            ClassNomina classNomina = new ClassNomina();
            int IdUsuario = (int)Session["sIdUsuario"];

            ModelNominaIndividual modelo;

            try
            {
                classNomina.Proceso_Nomina_Individual(model, IdUsuario);
                modelo = classNomina.GetModelNominaIndividual(model.IdEmpleado, model.IdPeriodoNomina, "Nomina");                                
                return RedirectToAction("ProcesaNominaIndividual", new { model.IdEmpleado, model.IdPeriodoNomina });
            }
            catch (Exception ex)
            {
                modelo = classNomina.GetModelNominaIndividual(model.IdEmpleado, model.IdPeriodoNomina, "Nomina");
                modelo.validacion = false;
                modelo.Mensaje = "No se pudo procesar el calculo, error: " + ex.Message;
                return View(modelo);
            }            
        }

        /// <summary>
        /// Accion que elimina las incidencias cargadas para un empleado especifico en un periodo de nómina especifico.
        /// </summary>
        /// <param name="pIdEmpleado">Identificador del empleado</param>
        /// <param name="pIdPeriodoNomina">Identificadot del periodo de nómina.</param>
        /// <param name="TipoNomina">Tipo de nómina que se esta procesando.</param>
        /// <returns></returns>
        public ActionResult EliminarIncidencias(int pIdEmpleado, int pIdPeriodoNomina, string TipoNomina)
        {
            try
            {
                ClassIncidencias cincidencias = new ClassIncidencias();
                cincidencias.DeleteAllEmpleado(pIdPeriodoNomina, pIdEmpleado);

                if(TipoNomina == "Finiquitos")                
                    return RedirectToAction("ProcesarFiniquitoIndividual", new { IdEmpleado = pIdEmpleado, pIdPeriodoNomina });                
                else                
                    return RedirectToAction("ProcesaNominaIndividual", new { IdEmpleado = pIdEmpleado, IdPeriodoNomina = pIdPeriodoNomina });
            }
            catch
            {
                if (TipoNomina == "Finiquitos")                
                    return RedirectToAction("ProcesarFiniquitoIndividual", new { IdEmpleado = pIdEmpleado, pIdPeriodoNomina });
                else                
                    return RedirectToAction("ProcesaNominaIndividual", new { IdEmpleado = pIdEmpleado, IdPeriodoNomina = pIdPeriodoNomina });
            }
        }

        /// <summary>
        /// Accion para borrar todo el calculo de nómina del periodo.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Confirmación a la vista.</returns>
        public ActionResult DeleteAllNomina(int pIdPeriodoNomina)
        {
            try
            {
                ClassCalculoNomina cnomina = new ClassCalculoNomina();
                cnomina.DeleteNominaTrabajo(pIdPeriodoNomina);

                return RedirectToAction("ProcesaNominaGeneral", new { pIdPeriodoNomina });
            }
            catch
            {
                return RedirectToAction("ProcesaNominaGeneral", new { pIdPeriodoNomina });
            }

        }

        /// <summary>
        /// Accion para eliminar del proceso de nómina a los empleados que fueron baja.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Confirmación a la vista.</returns>
        public ActionResult DeleteNominaBajas(int pIdPeriodoNomina)
        {
            try
            {
                ClassCalculoNomina cnomina = new ClassCalculoNomina();
                cnomina.DeleteNominaTrabajoBajas(pIdPeriodoNomina, (int)Session["sIdUnidadNegocio"]);

                return RedirectToAction("ProcesaNominaGeneral", new { pIdPeriodoNomina });
            }
            catch
            {

                return RedirectToAction("ProcesaNominaGeneral", new { pIdPeriodoNomina });
            }

        }

        /// <summary>
        /// Accion para eliminar el calculo de un empleado especifico en un periodo de nómina especifico.
        /// </summary>
        /// <param name="pIdEmpleado">Identificador del empleado.</param>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="TipoNomina">Tipo de nómina que se esta procesando.</param>
        /// <returns>Confirmación a la vista.</returns>
        public ActionResult EliminarCalculo(int pIdEmpleado, int pIdPeriodoNomina, string TipoNomina)
        {
            try
            {
                ClassCalculoNomina cnomina = new ClassCalculoNomina();
                cnomina.DeleteRegistroNominaTrabajo(pIdEmpleado, pIdPeriodoNomina);

                if (TipoNomina == "Finiquitos")                
                    return RedirectToAction("ProcesarFiniquitoIndividual", new { IdEmpleado = pIdEmpleado, pIdPeriodoNomina });                
                else                
                    return RedirectToAction("ProcesaNominaIndividual", new { IdEmpleado = pIdEmpleado, IdPeriodoNomina = pIdPeriodoNomina });                
            }
            catch
            {
                if (TipoNomina == "Finiquitos")                
                    return RedirectToAction("ProcesarFiniquitoIndividual", new { IdEmpleado = pIdEmpleado, pIdPeriodoNomina });                
                else                
                    return RedirectToAction("ProcesaNominaIndividual", new { IdEmpleado = pIdEmpleado, IdPeriodoNomina = pIdPeriodoNomina });                
            }
        }

        /// <summary>
        /// Accion para descargar el reporte de nómina.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Reporte a la vista.</returns>
        public ActionResult DescargarReporte(int pIdPeriodoNomina)
        {
            int IdUsuario = (int)Session["sIdUsuario"];
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            ClassNomina classNomina = new ClassNomina();
            ModelProcesaNominaGeneral modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(pIdPeriodoNomina));

            return View("ProcesaNominaGeneral", modelo);
        }

        /// <summary>
        /// Accion para cargar la información necesaria para los finiquitos de un periodo de nómina.
        /// </summary>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="finiquitos">Objeto con la información de los empleados a procesar.</param>
        /// <returns>Información a la vista.</returns>
        public ActionResult ProcesaFiniquitos(int pIdPeriodoNomina, List<ModelEmpleadoFiniquito> finiquitos)
        {
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            ClassNomina classNomina = new ClassNomina();
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            var modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(pIdPeriodoNomina));
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];

            ViewBag.ValidaConceptos = true;
            if (cfiniquitos.ValidaCamposFiniquitos(IdCliente))
            {
                ViewBag.ValidaConceptos = false;
            }

            string Claves = cfiniquitos.GetClavesFiniquitosConfigurados(pIdPeriodoNomina);
            string Ids = cfiniquitos.GetIdsFiniquitosConfigurados(pIdPeriodoNomina);
            modelo.Claves = Claves;
            modelo.Ids = Ids;
            modelo.lPeriodos = new List<SelectListItem>();
            modelo.lPeriodosSeleccionados = new List<SelectListItem>();

            modelo.lPeriodos = classPeriodoNomina.GetSeleccionPeriodoAcumuladoNominaOrdinaria((int)Session["sIdUnidadNegocio"]);

            if (finiquitos != null && finiquitos.Count > 0)            
                modelo.empleados = finiquitos.Where(x => x.IdEstatus != 5).ToList(); 
            else
                modelo.empleados = cfiniquitos.SearchEmpleadosFiniquitosIds2(Ids, IdUnidadNegocio, pIdPeriodoNomina, null);

            modelo.TotalEmpleados = modelo.empleados.Count;

            return View(modelo);
        }

        /// <summary>
        /// Acción que procesa los finiquitos de un periodo de nómina.
        /// </summary>
        /// <param name="model">Informacion capturada por el usuario.</param>
        /// <returns>Resultados a la vista.</returns>
        [HttpPost]
        public ActionResult ProcesaFiniquitos(ModelProcesaNominaGeneral model)
        {            
            var classPeriodoNomina = new ClassPeriodoNomina();
            var classNomina = new ClassNomina();
            var cfiniquitos = new ClassProcesosFiniquitos();
            var modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(model.IdPeriodoNomina));
            
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                modelo.empleados = cfiniquitos.SearchEmpleadosFiniquitosIds2(model.Ids, IdUnidadNegocio, model.IdPeriodoNomina, model.empleados);
                modelo.TotalEmpleados = modelo.empleados.Count;

                if (modelo.empleados != null)
                    cfiniquitos.GuardaConfiguracionFiniquitos(modelo, IdUsuario);

                ClassCalculoFiniquitos cfiniquito = new ClassCalculoFiniquitos();
                cfiniquito.Procesar(model.IdPeriodoNomina, null, IdUsuario);
                modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(model.IdPeriodoNomina));
                modelo.empleados = cfiniquitos.SearchEmpleadosFiniquitosIds2(model.Ids, IdUnidadNegocio, model.IdPeriodoNomina, model.empleados);
                modelo.TotalEmpleados = modelo.empleados.Count;
                modelo.lPeriodos = new List<SelectListItem>();
                modelo.lPeriodosSeleccionados = new List<SelectListItem>();

                modelo.lPeriodos = classPeriodoNomina.GetSeleccionPeriodoAcumuladoNominaOrdinaria((int)Session["sIdUnidadNegocio"]);

                modelo.validacion = true;
                modelo.Mensaje = "Atención: Se procesaron correctamente todos los calculos.";
            }
            catch (Exception ex)
            {
                modelo.validacion = false;
                modelo.Mensaje = "Error: " + ex.Message;
            }            
            
            return View(modelo);
        }

        /// <summary>
        /// Acción para eliminar todos los finquitos de un periodo de nómina.
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Resutados a la vista.</returns>
        public ActionResult EliminarAllFiniquitos(int IdPeriodoNomina)
        {
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            cfiniquitos.BorrarAllFiniquitos(IdPeriodoNomina);

            return RedirectToAction("ProcesaFiniquitos", new { pIdPeriodoNomina = IdPeriodoNomina });
        }

        /// <summary>
        /// Acción para borrar el finiquito de un empleado.
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Confirmación a la vista.</returns>
        public ActionResult Delete(int IdEmpleado, int pIdPeriodoNomina)
        {
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            cfiniquitos.BorrarFiniquito(IdEmpleado, pIdPeriodoNomina);

            return RedirectToAction("ProcesaFiniquitos", new { pIdPeriodoNomina });
        }
        
        /// <summary>
        /// Acción que carga información para el calculo de un finiquito individual.
        /// </summary>
        /// <param name="IdEmpleado">Identificador de un empleado.</param>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <returns>Información a la vista.</returns>
        public ActionResult ProcesarFiniquitoIndividual(int IdEmpleado, int pIdPeriodoNomina)
        {
            ClassNomina classNomina = new ClassNomina();
            ModelNominaIndividual modelo = classNomina.GetModelNominaIndividual(IdEmpleado, pIdPeriodoNomina, "Finiquitos");

            return View(modelo);
        }

        /// <summary>
        /// Acción que procesa un finiquito individual.
        /// </summary>
        /// <param name="model">Información cargada por el usuario.</param>
        /// <returns>Resultados a la vista.</returns>
        [HttpPost]
        public ActionResult ProcesarFiniquitoIndividual(ModelNominaIndividual model)
        {
            ClassNomina classNomina = new ClassNomina();            
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {                
                classNomina.Proceso_Finiquito_Individual(model, IdUsuario);

                ModelNominaIndividual modelo = classNomina.GetModelNominaIndividual(model.IdEmpleado, model.IdPeriodoNomina, "Finiquitos");
                modelo.validacion = true;
                modelo.Mensaje = "Se actualizo el calculo de forma correcta!";

                return RedirectToAction("ProcesarFiniquitoIndividual", new { model.IdEmpleado, pIdPeriodoNomina = model.IdPeriodoNomina });
            }
            catch (Exception ex)
            {
                ModelNominaIndividual modelo = classNomina.GetModelNominaIndividual(model.IdEmpleado, model.IdPeriodoNomina, "Finiquitos");
                modelo.validacion = false;
                modelo.Mensaje = ex.Message;

                return View(modelo);
            }            
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">Identificador</param>
        /// <returns>Archivo al a vista.</returns>
        public ActionResult ObtenerMisEnviosXmlExcel(int Id)
        {
            ClassReportesNomina cempleados = new ClassReportesNomina();
            List<DataRow> rows = new List<DataRow>();
            
            var stream = new MemoryStream();
            var serialicer = new XmlSerializer(typeof(DataRowCollection));

            //Cargo los datos
            DataTable datos = cempleados.GetTableNomina(Id);
            datos.TableName = "Datos";
            DataRowCollection dataRows = datos.Rows;
            //Lo transformo en un XML y lo guardo en memoria
            serialicer.Serialize(stream, dataRows);
            
            stream.Position = 0;

            //devuelvo el XML de la memoria como un fichero .xls
            return File(stream, "application/vnd.ms-excel", "Pedidos.xls");
        }

        /// <summary>
        /// Acción que cambiar la configuración avanzada de un finquito.
        /// </summary>
        /// <param name="IdConfiguracionFiniquito">Identificador del calculo de finquito.</param>
        /// <param name="banderaVac">Bandera para cambiar el estastus del calculo de vacaciones.</param>
        /// <param name="banderaPV">Bandera para cambiar el estastus del calculo de prima vacacional.</param>
        /// <param name="banderaAgui">Bandera para cambiar el estastus del calculo de aguinaldo.</param>
        /// <param name="bandera90d">Bandera para cambiar el estastus del calculo de 3 meses de indemnización.</param>
        /// <param name="bandera20d">Bandera para cambiar el estastus del calculo de 20 dias por año de antiguedad.</param>
        /// <param name="banderaPA">Bandera para cambiar el estastus del calculo de Prima de antiguedad.</param>
        /// <param name="LiquidacionSDI">Bandera para cambiar el salario con el que se procesa un finiquito.</param>
        /// <returns>Confirmación a la vista.</returns>
        [HttpPost]
        public JsonResult GuardarConfAvanzada(int IdConfiguracionFiniquito, bool banderaVac, bool banderaPV, bool banderaAgui, bool bandera90d, bool bandera20d, bool banderaPA, bool LiquidacionSDI, bool ExentoProporcionalLiquidacion)
        {
            try
            {                
                ClassProcesosFiniquitos cpf = new ClassProcesosFiniquitos();
                
                cpf.UpdateConfiguracionAvanzada(IdConfiguracionFiniquito, banderaVac, banderaPV, banderaAgui, bandera90d, bandera20d, banderaPA, LiquidacionSDI, ExentoProporcionalLiquidacion, (int)Session["sIdUsuario"]);
                return Json("Exito", JsonRequestBehavior.AllowGet);                
            }
            catch
            {
                return Json("No se pudo actualizar la configuración del cálculo.", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GuardarConfAvanzadaNomina(int IdPeriodoNomina, int IdEmpleado, bool suspenderTradicional, bool suspenderEsquema, bool suspenderCS, decimal? pagSueldos, decimal? cobCargas, bool incidenciasAut)
        {
            try
            {
                var cpf = new ClassProcesosNomina();
                int? suspTrad = null;
                int? suspEsq = null;
                int? suspCS = null;
                int? incAut = null;

                if (suspenderTradicional)
                    suspTrad = 1;

                if (suspenderEsquema)
                    suspEsq = 1;

                if (suspenderCS)
                    suspCS = 1;

                if (incidenciasAut)
                    incAut = 1;

                cpf.ConfiguracionAvanzadaNomina(IdEmpleado, IdPeriodoNomina, suspTrad, suspEsq, suspCS, pagSueldos, cobCargas, incAut, (int)Session["sIdUsuario"]);
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("No se pudo actualizar la configuración del cálculo.", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetConfiguracionAvanzada(int IdPeriodoNomina, int IdEmpleado)
        {
            var cpf = new ClassProcesosNomina();
            var conf = cpf.GetConfiguracionNominaPeriodoEmpleado(IdPeriodoNomina, IdEmpleado);
            return Json(conf, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CambiaLiquidacion(bool? Liquidacion, int? IdEmpleado, int? IdPeriodoNomina)
        {
            try
            {
                var pf = new ClassProcesosFiniquitos( );
                pf.UpdateLiquidacion((bool)Liquidacion, (int)IdEmpleado, (int)IdPeriodoNomina, (int)Session["sIdUsuario"]);
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ActualizaFechaBaja(string fechaBaja, int IdEmpleado, int IdPeriodoNomina)
        {
            try
            {
                var pf = new ClassProcesosFiniquitos();
                pf.UpdateFechaBaja(fechaBaja, IdEmpleado, IdPeriodoNomina, (int)Session["sIdUsuario"]);
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpPost]
        public JsonResult BuscarEmpleados(string clave, string nombre, string rfc, string claves)
        {            
            var cpf = new ClassEmpleado();
            var conf = cpf.GetEmpleadosByCamposBusqueda(clave, nombre, rfc, claves, (int)Session["sIdUnidadNegocio"]);
            return Json(conf, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult addEmpleado(string ids, int IdPeriodo)
        {
            try
            {
                var modelo = new ModelProcesaNominaGeneral();
                var cfiniquitos = new ClassProcesosFiniquitos();
               
                var idsEmp = ids.Split(',').Where(x=> x != string.Empty).ToArray();
                List<int> idsEmpInt = Array.ConvertAll(idsEmp, int.Parse).ToList();
                ClassEmpleado cEmp = new ClassEmpleado();
                var emp = cEmp.getvEmpleadosByListIds(idsEmpInt, (int)Session["sIdUnidadNegocio"]);
                cfiniquitos.GuardaConfiguracionFiniquitos(emp, IdPeriodo, (int)Session["sIdUsuario"]);

                modelo.empleados = cfiniquitos.SearchEmpleadosFiniquitosIds2(ids, (int)Session["sIdUnidadNegocio"], IdPeriodo, null);

                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public JsonResult ActualizaSueldoReal(int IdEmpleado, decimal? nuevoSueldo)
        {
            try
            {
                if (nuevoSueldo != null)
                {
                    var cpf = new ClassProcesosFiniquitos();
                    cpf.UpdateSueldoReal(IdEmpleado, (decimal)nuevoSueldo, (int)Session["sIdUsuario"]);
                    return Json("Exito", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("La información capturada es incorrecta.");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult FormatoFiniquito(int IdPeriodoNomina, int IdEmpleado)
        {
            cPDF cp = new cPDF();
            byte[] arch = cp.getPDFTrad(IdPeriodoNomina, IdEmpleado);
            return File(arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "P" +IdPeriodoNomina + "E" + IdEmpleado + "T.pdf");
        }

        public FileResult FormatosFiniquitos(int IdPeriodoNomina)
        {
            
            cPDF cp = new cPDF();
            ClassNomina classNomina = new ClassNomina();
            List<string> files = new List<string>();
            var datosNomina = classNomina.GetDatavNominaTrabajoFiniquitos(IdPeriodoNomina);

            string ruta = @"D:\TadaNomina\RecibosFiniquitosPDF\" + IdPeriodoNomina + @"\";
            if (!Directory.Exists(ruta))            
                Directory.CreateDirectory(ruta);
            

            foreach (var item in datosNomina)
            {
                byte[] arch = cp.getPDFTrad(IdPeriodoNomina, item.IdEmpleado);
                var nombreArchivo = item.IdPeriodoNomina + "_" + item.ClaveEmpleado + "T.pdf";
                string ruta_PDF = @"D:\TadaNomina\RecibosFiniquitosPDF\" + IdPeriodoNomina + @"\" + nombreArchivo;

                System.IO.File.WriteAllBytes(ruta_PDF, arch);
                files.Add(ruta_PDF);
            }

            CreateZipFile(files, ruta + IdPeriodoNomina + ".zip");

            byte[] _arch = System.IO.File.ReadAllBytes(ruta + IdPeriodoNomina + ".zip");
            return File(_arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "P" + IdPeriodoNomina + "T.zip");
        }

        public FileResult FormatoFiniquitoS(int IdPeriodoNomina, int IdEmpleado)
        {
            cPDFS cp = new cPDFS();
            byte[] arch = cp.getPDFEsq(IdPeriodoNomina, IdEmpleado);
            return File(arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "P" + IdPeriodoNomina + "E" + IdEmpleado + "S.pdf");
        }

        public FileResult FormatosFiniquitoSs(int IdPeriodoNomina)
        {
            cPDFS cp = new cPDFS();
            ClassNomina classNomina = new ClassNomina();
            List<string> files = new List<string>();
            var datosNomina = classNomina.GetDatavNominaTrabajoFiniquitos(IdPeriodoNomina);

            string ruta = @"D:\TadaNomina\RecibosFiniquitosPDF\" + IdPeriodoNomina + @"\";
            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);


            foreach (var item in datosNomina)
            {
                byte[] arch = cp.getPDFEsq(IdPeriodoNomina, item.IdEmpleado);
                var nombreArchivo = item.IdPeriodoNomina + "_" + item.ClaveEmpleado + "S.pdf";
                string ruta_PDF = @"D:\TadaNomina\RecibosFiniquitosPDF\" + IdPeriodoNomina + @"\" + nombreArchivo;

                System.IO.File.WriteAllBytes(ruta_PDF, arch);
                files.Add(ruta_PDF);
            }

            CreateZipFile(files, ruta + IdPeriodoNomina + ".zip");

            byte[] _arch = System.IO.File.ReadAllBytes(ruta + IdPeriodoNomina + ".zip");
            return File(_arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "P" + IdPeriodoNomina + "S.zip");
        }

        public FileResult FormatoFiniquitoR(int IdPeriodoNomina, int IdEmpleado)
        {
            cPDFR cp = new cPDFR();
            byte[] arch = cp.getPDFReal(IdPeriodoNomina, IdEmpleado);
            return File(arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "P" + IdPeriodoNomina + "E" + IdEmpleado + "S.pdf");
        }

        public FileResult FormatoFiniquitoRs(int IdPeriodoNomina)
        {
            cPDFR cp = new cPDFR();            
            ClassNomina classNomina = new ClassNomina();
            List<string> files = new List<string>();
            var datosNomina = classNomina.GetDatavNominaTrabajoFiniquitos(IdPeriodoNomina);

            string ruta = @"D:\TadaNomina\RecibosFiniquitosPDF\" + IdPeriodoNomina + @"\";
            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);


            foreach (var item in datosNomina)
            {
                byte[] arch = cp.getPDFReal(IdPeriodoNomina, item.IdEmpleado);
                var nombreArchivo = item.IdPeriodoNomina + "_" + item.ClaveEmpleado + "R.pdf";
                string ruta_PDF = @"D:\TadaNomina\RecibosFiniquitosPDF\" + IdPeriodoNomina + @"\" + nombreArchivo;

                System.IO.File.WriteAllBytes(ruta_PDF, arch);
                files.Add(ruta_PDF);
            }

            CreateZipFile(files, ruta + IdPeriodoNomina + ".zip");

            byte[] _arch = System.IO.File.ReadAllBytes(ruta + IdPeriodoNomina + ".zip");
            return File(_arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "P" + IdPeriodoNomina + "R.zip");
        }

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

        public ActionResult Cerrar(int IdEmpleado, int pIdPeriodoNomina)
        {
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            cfiniquitos.CerrarFiniquito(IdEmpleado, pIdPeriodoNomina);

            return RedirectToAction("ProcesaFiniquitos", new { pIdPeriodoNomina });
        }

        public ActionResult AcumularAllFiniquitos(int IdPeriodoNomina)
        {
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            cfiniquitos.CerrarAllFiniquito(IdPeriodoNomina);

            return RedirectToAction("ProcesaFiniquitos", new { pIdPeriodoNomina = IdPeriodoNomina });
        }

        public ActionResult Bloquear(int IdEmpleado, int pIdPeriodoNomina)
        {
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            cfiniquitos.BloquearFiniquito(IdEmpleado, pIdPeriodoNomina);

            return RedirectToAction("ProcesaFiniquitos", new { pIdPeriodoNomina });
        }

        public ActionResult Abrir(int IdEmpleado, int pIdPeriodoNomina)
        {
            ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
            cfiniquitos.AbrirFiniquito(IdEmpleado, pIdPeriodoNomina);

            return RedirectToAction("ProcesaFiniquitos", new { pIdPeriodoNomina });
        }

        [HttpPost]
        public JsonResult UploadFiles()
        {
            try
            {
                int? IdPeriodoNomina = int.Parse(Request.Form.GetValues(0).First());
                bool ConsideraSMO = bool.Parse(Request.Form.GetValues(1).First());

                if (Request.Files.Count > 0)
                {
                    var files = Request.Files;
                    ClassEmpleado ce = new ClassEmpleado();
                    
                    int cont = 0;

                    foreach (string str in files)
                    {
                        HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                        if (file != null)
                        {

                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            string result = System.Text.Encoding.UTF8.GetString(binData);
                            string[] rows = result.Replace("\r", "").Split('\n').ToArray();

                            for (int i = 1; i <= rows.Count() - 1; i++)
                            {
                                if (rows[i].Length > 0)
                                {
                                    string[] datos = rows[i].Split(',').ToArray();
                                    int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                                    int IdCliente = (int)Session["sIdCliente"];
                                    if (datos[0].Length > 0 && datos[1].Length > 0 && datos[2].Length > 0)
                                    {
                                        var idemp = ce.GetEmpleadosByClave(datos[0], IdUnidadNegocio).FirstOrDefault();
                                       
                                        decimal sd = 0;
                                        decimal sd_real = 0;
                                        decimal sdi = 0;
                                        decimal neto = 0;
                                        DateTime? fechaimss = null;
                                        DateTime? fechaRecAnt = null;

                                        try { sd = decimal.Parse(datos[1]); } catch { }
                                        try { sd_real = decimal.Parse(datos[2]); } catch { }
                                        try { sdi = decimal.Parse(datos[3]); } catch { }
                                        try { neto = decimal.Parse(datos[4]); } catch { }
                                        try { fechaimss = DateTime.Parse(datos[5]); } catch { }
                                        try { fechaRecAnt = DateTime.Parse(datos[6]); } catch { }

                                        if (idemp != null)
                                            ce.ModificaDatosProy(idemp.IdEmpleado, sd, sd_real, sdi, neto, fechaimss, fechaRecAnt);

                                        cont++;
                                    }
                                }
                            }
                        }

                    }
                    return Json(new { result = true, contador = cont });
                }
                else
                {
                    return Json(new { result = false, mensaje = "No hay archivos para cargar." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult LimpiarDatos()
        {
            try
            {
                ClassEmpleado ce = new ClassEmpleado();
                ce.EliminaDatosProyección((int)Session["sIdUnidadNegocio"]);

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult getDatosConfAjuste(int IdPeriodoNomina)
        {
            try
            {
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                ClassPeriodoNomina cp = new ClassPeriodoNomina();
                var datos = cp.GetModelPeriodoNominasIdConf(IdPeriodoNomina, IdUnidad);
                
                return Json(new {result = "Ok", datos});
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult getDatosConceptosPagar(int IdPeriodoNomina)
        {
            try
            {                
                ClassPeriodoNomina cun = new ClassPeriodoNomina();
                var datos = cun.GetPeriodo(IdPeriodoNomina);
                
                return Json(new { result = "Ok", datos });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult guardaConfiguracionAjuste(int IdPeriodoNomina, string empleados, string IdsPeriodosAjuste)
        {
            try
            {
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassPeriodoNomina cp = new ClassPeriodoNomina();

                cp.guardarPeriodoNominaConfAjuste(IdPeriodoNomina, empleados, IdsPeriodosAjuste, IdUsuario);
                return Json(new { result = "Ok", mensaje = "Se guardo correctamente la información." });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult guardaDatosConfSDI(int IdPeriodoNomina, string selectedIndices, bool LiquidacionSDI, string ConceptosIntegran)
        {
            try
            {
                ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                cun.UpdateUnidadNegocioDatosLiquidacion((int)Session["sIdUnidadNegocio"], LiquidacionSDI, ConceptosIntegran);

                ClassPeriodoNomina cpn = new ClassPeriodoNomina();
                cpn.EditPeriodoNominaDatosSDI(IdPeriodoNomina, selectedIndices);

                return Json(new { result = "Ok", mensaje = "Se guardo la información correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult getDatosConfSDI(int IdPeriodoNomina )
        {
            try
            {
                ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                var data = cun.getUnidadesnegocioId((int)Session["sIdUnidadNegocio"]);
                var datos = new { data.CalcularLiquidacionSDI, data.ConceptosSDILiquidacion };

                ClassPeriodoNomina cpn = new ClassPeriodoNomina();
                var _periodo = cpn.GetPeriodo(IdPeriodoNomina);
                string[] periodo = new string[0];
                if(_periodo.PeriodosIntegracionSDI != null)
                    periodo = _periodo.PeriodosIntegracionSDI.Split(',').Where(x=> x != string.Empty).ToArray();

                var periodoInt = Array.ConvertAll(periodo, int.Parse);
                var periodosSelect = cpn.getPeriodosIds(periodoInt);

                var datos2 =  periodosSelect.Select(x=> new { x.IdPeriodoNomina, x.Periodo });
                
                return Json(new { result = "Ok", datos, datos2 });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult guardaDatosConceptosGral(int IdPeriodoNomina, bool BanderaNoVacaciones, bool BanderaNoPV, bool BanderaNoAguinaldo, bool BanderaNo90Dias, bool BanderaNo20Dias, bool BanderaNoPA)
        {
            try
            {
                ClassPeriodoNomina cun = new ClassPeriodoNomina();
                cun.EditPeriodoNominaConceptosAut(IdPeriodoNomina, BanderaNoVacaciones, BanderaNoPV, BanderaNoAguinaldo, BanderaNo90Dias, BanderaNo20Dias, BanderaNoPA);

                return Json(new { result = "Ok", mensaje = "Se guardo la información correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult actualizaFechaBajaGral(int? IdPeriodoNomina, string FechaBaja)
        {
            try
            {
                var pf = new ClassProcesosFiniquitos();
                pf.UpdateFechaBajaGral(FechaBaja ?? DateTime.Now.ToShortDateString(), IdPeriodoNomina ?? 0, (int)Session["sIdUsuario"]);
                return Json(new { result = "Ok" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "er", mensaje = ex.Message });
            }
        }

        public ActionResult getArchivoLog(int IdPeriodoNomina)
        {
            string path = @"C:\TadaNomina\LogsFRONT\Nomina\";
            string nombre = "Periodo_" + IdPeriodoNomina + "_IdUsuario_" + Session["sIdUsuario"] + ".txt";

            return File(path + nombre, "text/plain", "nomina.txt");
        }
    }
}
