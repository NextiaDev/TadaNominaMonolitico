using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PiramidadosController : BaseController
    {
        public ActionResult SeleccionarPeriodo()
        {
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominas((int)Session["sIdUnidadNegocio"]).Where(x=> x.TipoNomina == "Complemento").ToList();

            return View(periodos);            
        }

        public ActionResult Index(int IdPeriodoNomina)
        {
            ClassPiramidados cp = new ClassPiramidados();
            ModelPiramidacion mp = new ModelPiramidacion();
            ClassPeriodoNomina cpn = new ClassPeriodoNomina();
            ClassConceptos cc = new ClassConceptos();
            
            var periodo = cpn.GetPeriodo(IdPeriodoNomina);
            mp.lConceptos = cc.getSelectConceptosPiramidables((int)Session["sIdCliente"]);
            mp.Periodo = periodo.Periodo + " (" + periodo.FechaInicio.ToShortDateString() + " - " + periodo.FechaFin.ToShortDateString() + ")";
            mp.IdPeriodoNomina = IdPeriodoNomina;
            mp.Calculos = cp.getCalculosPeriodo(IdPeriodoNomina);
            return View(mp);
        }

        [HttpPost]
        public JsonResult getEmpleado(string ClaveEmpleado)
        {
            try
            {
                ClassEmpleado ce = new ClassEmpleado();
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                var emp = ce.GetEmpleadosByClaveAllEstatus(ClaveEmpleado, IdUnidadNegocio).FirstOrDefault();

                if(emp != null)
                    return Json(new { result = true, emp });
                else
                    return Json(new { result = false, mensaje = "No se encontro información con la clave indicada." });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult piramida(int? IdEmpleado, string datosEmp, decimal? SD, bool ConsideraSMO, decimal Importe)
        {
            try
            {
                ClassPiramidados cp = new ClassPiramidados();
                ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                var unidadNegocio = cun.getUnidadesnegocioId((int)Session["sIdUnidadNegocio"]);
                var tipoNom = cun.getTipoNominaById(unidadNegocio.IdTipoNomina);
                var diasPago = getDiasPagos(tipoNom);                
                decimal SM = ConsideraSMO ? diasPago * (SD ?? 0) : 0;
                decimal ISRSM = cp.CalculaISR(SM, DateTime.Now, false);
                decimal NetoSueldo = SM - ISRSM;
                decimal ImporteConConcepto = NetoSueldo + Importe;
                decimal PiramidadoConcepto = cp.piramida(ImporteConConcepto, DateTime.Now);
                decimal ISRT = PiramidadoConcepto - ImporteConConcepto;
                decimal ISRR = ISRT - ISRSM;
                decimal TP = Importe + ISRR;

                return Json(new
                {
                    IdEmpleado,
                    datosEmp,
                    SD_F = SD != null ? ((decimal)SD).ToString("C") : "$0.00",  
                    SD,
                    SM_F = SM.ToString("C"),
                    SM,
                    diasPago,
                    ISRSM_F = ISRSM.ToString("C"),
                    ISRSM,
                    NetoSueldo_F = NetoSueldo.ToString("C"),
                    NetoSueldo,
                    ImporteConConcepto_F = ImporteConConcepto.ToString("C"),
                    ImporteConConcepto,
                    PiramidadoConcepto_F = PiramidadoConcepto.ToString("C"),
                    PiramidadoConcepto,
                    ISRT_F = ISRT.ToString("C"),
                    ISRT,
                    ISRR_F = ISRR.ToString("C"),
                    ISRR,
                    TP_F = TP.ToString("C"),
                    TP
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }            
        }

        [HttpPost]
        public JsonResult AgregaPersonalizado(int? IdEmpleado, string datosEmp, decimal? SD, bool ConsideraSMO, decimal? Importe, decimal? TotalPercepcionesCap, decimal? ISRCap, decimal? NetoCap)
        {
            try
            {
                if ((TotalPercepcionesCap - ISRCap) == NetoCap)
                {
                    Importe = Importe ?? 0;
                    ClassPiramidados cp = new ClassPiramidados();
                    ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                    var unidadNegocio = cun.getUnidadesnegocioId((int)Session["sIdUnidadNegocio"]);
                    var tipoNom = cun.getTipoNominaById(unidadNegocio.IdTipoNomina);
                    var diasPago = getDiasPagos(tipoNom);
                    decimal SM = ConsideraSMO ? diasPago * (SD ?? 0) : 0;
                    decimal ISRSM = 0;
                    decimal NetoSueldo = SM - ISRSM;
                    decimal ImporteConConcepto = NetoCap ?? 0;
                    decimal PiramidadoConcepto = 0;
                    decimal ISRT = ISRCap ?? 0;
                    decimal ISRR = ISRCap ?? 0;
                    decimal TP = TotalPercepcionesCap ?? 0;

                    return Json(new
                    {
                        IdEmpleado,
                        datosEmp,
                        SD_F = SD != null ? ((decimal)SD).ToString("C") : "$0.00",
                        SD,
                        SM_F = SM.ToString("C"),
                        SM,
                        diasPago,
                        ISRSM_F = ISRSM.ToString("C"),
                        ISRSM,
                        NetoSueldo_F = NetoSueldo.ToString("C"),
                        NetoSueldo,
                        ImporteConConcepto_F = ImporteConConcepto.ToString("C"),
                        ImporteConConcepto,
                        PiramidadoConcepto_F = PiramidadoConcepto.ToString("C"),
                        PiramidadoConcepto,
                        ISRT_F = ISRT.ToString("C"),
                        ISRT,
                        ISRR_F = ISRR.ToString("C"),
                        ISRR,
                        TP_F = TP.ToString("C"),
                        TP
                    });
                }
                else
                    throw new Exception("Los importes ingresados son incorrectos, favor de validar.");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public decimal getDiasPagos(Cat_TipoNomina tipo)
        {
            if (tipo.Clave_Sat == "04" && tipo.DiasPago > 15)
                return tipo.DiasPago * 2;
            else
                return 30;            
        }

        public JsonResult AddPiramidado(int? IdPeriodoNomina, int? IdEmpleado, int? IdConcepto, decimal? diasPago, decimal? SD, decimal? SM, decimal? ISRSM, decimal? NetoSueldo, decimal? Importe, decimal? ImporteConConcepto, decimal? ISRT, decimal? ISRR, decimal? TP, bool ConsideraSMO)
        {
            try
            {
                string ConsideraSMO_ = ConsideraSMO ? "SI" : "NO";
                ClassPiramidados cp = new ClassPiramidados();
                cp.AddConceptoPiramidado(IdPeriodoNomina, IdEmpleado, IdConcepto, diasPago, SD, SM, ISRSM, NetoSueldo, Importe, ImporteConConcepto, ISRT, ISRR, TP, ConsideraSMO_, "Mensual", (int)Session["sIdUsuario"]);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult getConceptoById(int IdConceptoConfigurado)
        {
            try
            {
                ClassPiramidados cp = new ClassPiramidados();
                var concepto = cp.getConceptoById(IdConceptoConfigurado);
                return Json(new { result = true, concepto });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, mensaje = ex.Message });
            }
        }

        public JsonResult DeleteCalculo(int IdConceptoConfigurado)
        {
            try
            {
                ClassPiramidados cp = new ClassPiramidados();
                cp.DeleteConceptoPiramidado(IdConceptoConfigurado);
                return Json("Ok");
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        public JsonResult DeleteAllCalculos(int IdPeriodoNomina)
        {
            try
            {
                ClassPiramidados cp = new ClassPiramidados();
                cp.DeleteConceptoPiramidadoByPeriodo(IdPeriodoNomina);
                return Json("Ok");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
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
                    ClassConceptos cc = new ClassConceptos();
                    ClassPiramidados cp = new ClassPiramidados();
                    ClassUnidadesNegocio cun = new ClassUnidadesNegocio();

                    var unidadNegocio = cun.getUnidadesnegocioId((int)Session["sIdUnidadNegocio"]);
                    var tipoNom = cun.getTipoNominaById(unidadNegocio.IdTipoNomina);
                    var diasPago = getDiasPagos(tipoNom);
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
                                        var emp = ce.GetEmpleadosByClave(datos[0], IdUnidadNegocio).FirstOrDefault();
                                        var concepto = cc.GetvConcepto(datos[1], IdCliente);
                                        decimal Importe = 0;
                                        try { Importe = decimal.Parse(datos[2].ToString()); } catch { }
                                                                                
                                        decimal SMO = diasPago * (emp.SDIMSS ?? 0);
                                        decimal SM = ConsideraSMO ? diasPago * (emp.SDIMSS ?? 0) : 0;
                                        decimal ISRSM = cp.CalculaISR(SM, DateTime.Now, false);
                                        decimal NetoSueldo = SM - ISRSM;
                                        decimal ImporteConConcepto = NetoSueldo + Importe;
                                        decimal PiramidadoConcepto = cp.piramida(ImporteConConcepto, DateTime.Now);
                                        decimal ISRT = PiramidadoConcepto - ImporteConConcepto;
                                        decimal ISRR = ISRT - ISRSM;
                                        decimal TP = Importe + ISRR;
                                        string ConsideraSMO_ = ConsideraSMO ? "SI" : "NO";

                                        cp.AddConceptoPiramidado(IdPeriodoNomina, emp.IdEmpleado, concepto.IdConcepto, diasPago, emp.SDIMSS, SMO, ISRSM, NetoSueldo, Importe, ImporteConConcepto, ISRT, ISRR, TP, ConsideraSMO_, "Mensual", (int)Session["sIdUsuario"]);
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
    }
}