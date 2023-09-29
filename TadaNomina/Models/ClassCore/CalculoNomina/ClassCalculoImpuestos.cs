using DocumentFormat.OpenXml.Office.CustomUI;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Models.ClassCore.CalculoNomina
{
    /// <summary>
    /// Calculo de Impuestos
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: documentar el codigo
    /// </summary>
    public class ClassCalculoImpuestos: ClassProcesosNomina
    {
        /// <summary>
        /// Metodo para calcular el impuesto sobre la renta.
        /// </summary>
        public void CalculaISR()
        {
            procesoAjusteSecundario();
            Obten_Base_Gravada();
            if (nominaTrabajo.BaseGravada > 0)
            {
                Obten_Limite_Inferior();
                Obten_Diferencia_Limite();
                Obten_Porcentaje_Calculado();
                Obten_ISR();                
                Obten_Subsidio_Al_Empleo();                
                Obten_Impuestos_Retenidos();
            }
            else
            {
                nominaTrabajo.LimiteInferior = 0;
                nominaTrabajo.Porcentaje = 0;
                nominaTrabajo.CuotaFija = 0;
                nominaTrabajo.DiferenciaLimite = 0;
                nominaTrabajo.PorcentajeCalculado = 0;
                nominaTrabajo.ISR = 0;
                nominaTrabajo.Subsidio = 0;
                nominaTrabajo.SubsidioPagar = 0;
                nominaTrabajo.ImpuestoRetener = 0;
                nominaTrabajo.ReintegroISR = 0;
            }
            
            if(ListNominaAjusteAnterior != null)
                ListNominaAjuste = ListNominaAjusteAnterior;

            if(ListImpuestos_Anterior != null)
                ListImpuestos_Ajuste = ListImpuestos_Anterior;            
        }

        public void CalculaISRAguinaldoL174()
        {            
            try
            {
                nominaTrabajo.Subsidio = 0;
                nominaTrabajo.SubsidioPagar = 0;
                nominaTrabajo.BaseGravada = 0;
                nominaTrabajo.BaseGravada += incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && _tipoEsquemaT.Contains(x.TipoEsquema) && x.Integrable == "SI" && x.ClaveGpo != "003").Select(x => x.Gravado).Sum();

                if (nominaTrabajo.BaseGravada > 0)
                {
                    var sueldoMensual = SD_IMSS * 30;
                    var ISR_prev = CalculaISR((decimal)sueldoMensual, Periodo.FechaFin, "05", false);
                    var aguinaldoMensualizado = (nominaTrabajo.BaseGravada / 365M) * 30.4M;
                    var NetoSueldoOrdinario = sueldoMensual - ISR_prev;
                    var sueldoMasAguinaldo = sueldoMensual + aguinaldoMensualizado;
                    var ISRCausado = CalculaISR((decimal)sueldoMasAguinaldo, Periodo.FechaFin, "05", false);
                    var sueldoNetoMenosISR = sueldoMasAguinaldo - ISRCausado;
                    var diferenciaRYAA = ISRCausado - ISR_prev;
                    var Porcentaje = diferenciaRYAA / aguinaldoMensualizado;
                    var IsrRet = Porcentaje * nominaTrabajo.BaseGravada;
                    
                    nominaTrabajo.ISR = IsrRet;
                    nominaTrabajo.ImpuestoRetener = IsrRet;
                }
                else
                {                    
                    nominaTrabajo.ISR = 0;
                    nominaTrabajo.ImpuestoRetener = 0;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error al calcular el ISR L174 del empleado: " + IdEmpleado + " - " + ClaveEmpleado + ", ex: " + ex.Message);
            }
        }

        /// <summary>
        /// Metodo para obtener la base gravada con la que se calcula el impuesto.
        /// </summary>
        protected void Obten_Base_Gravada()
        {
            nominaTrabajo.BaseGravada = 0;
            nominaTrabajo.BaseGravada += nominaTrabajo.SueldoPagado;

            if (UnidadNegocio.FaltasImporte == "S")
            {
                if (nominaTrabajo.DiasTrabajados > 0)
                {
                    string[] gpos = { "500", "501" };
                    var diasSueldo = nominaTrabajo.DiasTrabajados - incidenciasEmpleado.Where(x => gpos.Contains(x.ClaveGpo) && _tipoEsquemaT.Contains(x.TipoEsquema)).Sum(x => x.Cantidad);

                    nominaTrabajo.BaseGravada = diasSueldo >= 1 ? diasSueldo * SD_IMSS : 0;
                    nominaTrabajo.DiasTrabajados = diasSueldo;
                }
            }

            if (percepcionesEspecialesGravado == null) { percepcionesEspecialesGravado = 0; }
            nominaTrabajo.BaseGravada += percepcionesEspecialesGravado;
            nominaTrabajo.BaseGravada += incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && _tipoEsquemaT.Contains(x.TipoEsquema) && x.Integrable == "SI" && x.ClaveGpo != "003").Select(x => x.Gravado).Sum();

            nominaTrabajo.BaseGravada += montoIncidenciasMultiplicaDTGrabado;
            nominaTrabajo.BaseGravadaP = nominaTrabajo.BaseGravada;

            //obtener base gravada cuando se hace proyeccion mensual.
            if (UnidadNegocio.ISRProyeccionMensual == "S")
            {
                var baseGravDiaria = nominaTrabajo.BaseGravada / DiasPago;
                nominaTrabajo.BaseGravada = baseGravDiaria * (UnidadNegocio.FactorDiasMesISR ?? 0);
            }
            
            if (Periodo.AjusteDeImpuestos == "SI" && nominaTrabajo.BaseGravada > 0)
                nominaTrabajo.BaseGravada += ListNominaAjuste.Where(x => x.Rfc == RFC).Select(x => x.BaseGravadaP).Sum();
        }

        private void procesoAjusteSecundario()
        {
            if (listEmpleadosSinAjuste != null)            
                AjusteAnual = listEmpleadosSinAjuste.Where(x => x.IdEmpleado == IdEmpleado).FirstOrDefault() != null ? false : true;

            ListNominaAjusteAnterior = ListNominaAjuste;
            ListImpuestos_Anterior = ListImpuestos_Ajuste;

            sp_EmpleadosAjusteAnual_Result emp = null;
            if (ListaEmpleadosAjusteAnual != null)
                emp = ListaEmpleadosAjusteAnual.Where(x => x.idempleado == IdEmpleado).FirstOrDefault();

            if (AjusteSecundario && !AjusteAnual || (emp == null && AjusteSecundario))
            {
                ListNominaAjuste = ListNominaAjusteSecundario;
                ListImpuestos_Ajuste = ListImpuestos_AjusteSecundario;
            }            
        }

        /// <summary>
        /// Metodo para obtener el limite inferios de las tablas de impuestos.
        /// </summary>
        /// <exception cref="Exception">Excepcion devuelta por el metodo en caso de un error.</exception>
        protected void Obten_Limite_Inferior()
        {
            nominaTrabajo.LimiteInferior = 0;
            nominaTrabajo.Porcentaje = 0;
            nominaTrabajo.CuotaFija = 0;

            if (Periodo.TablaDiaria == "S")            
                GetTablasEnBaseDiasPeriodo();            

            try
            {
                if (Periodo.AjusteDeImpuestos == "SI")
                {                       
                    if (ListNominaAjuste.Where(x => x.Rfc == RFC).FirstOrDefault() == null)
                    {
                        nominaTrabajo.LimiteInferior += ListImpuestos.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().LimiteInferior;
                        nominaTrabajo.Porcentaje += ListImpuestos.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().Porcentaje;
                        nominaTrabajo.CuotaFija += ListImpuestos.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().CuotaFija;
                    }
                    else
                    {
                        nominaTrabajo.LimiteInferior += ListImpuestos_Ajuste.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().LimiteInferior;
                        nominaTrabajo.Porcentaje += ListImpuestos_Ajuste.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().Porcentaje;
                        nominaTrabajo.CuotaFija += ListImpuestos_Ajuste.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().CuotaFija;
                    }
                }
                else
                {
                    nominaTrabajo.LimiteInferior += ListImpuestos.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().LimiteInferior;
                    nominaTrabajo.Porcentaje += ListImpuestos.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().Porcentaje;
                    nominaTrabajo.CuotaFija += ListImpuestos.Where(x => x.LimiteSuperior >= nominaTrabajo.BaseGravada && x.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault().CuotaFija;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en tablas de impuestos. " + ex.Message);
            }
            
        }

        private void GetTablasEnBaseDiasPeriodo()
        {
            numDiasTablasDiarias = Periodo.FechaFin.Subtract(Periodo.FechaInicio).Days + 1;

            List<DB.ImpuestoSat> limpuestos = new List<DB.ImpuestoSat>();

            decimal factorAño = 365M / 12M;
            ListImpuestos_Anterior = ListImpuestos;
            ListSubsidio_Anterior = ListSubsidio;

            for (int i = 0; i<= ListImpuestos.Count -1; i++)
            {                
                DB.ImpuestoSat impuestos = new DB.ImpuestoSat();
                impuestos.IdImpuesto = ListImpuestos[i].IdImpuesto;
                impuestos.LimiteInferior = ListImpuestos[i].LimiteInferior <= 0.01M ? 0.01M : Math.Round(limpuestos[i-1].LimiteSuperior + 0.01M, 2);
                impuestos.LimiteSuperior = Math.Round((ListImpuestos[i].LimiteSuperior / factorAño) * numDiasTablasDiarias, 2);
                impuestos.CuotaFija = Math.Round((ListImpuestos[i].CuotaFija / factorAño) * numDiasTablasDiarias, 2);
                impuestos.Porcentaje = (ListImpuestos[i].Porcentaje);
                impuestos.FechaInicio = ListImpuestos[i].FechaInicio;
                impuestos.EstatusId = ListImpuestos[i].EstatusId;

                limpuestos.Add(impuestos);
            }

            ListImpuestos = limpuestos;

            List<DB.SubsidioEmpleoSat> lsubsidio = new List<DB.SubsidioEmpleoSat>();

            for (int i = 0; i<= ListSubsidio.Count -1; i++)
            {
                DB.SubsidioEmpleoSat subsidio = new DB.SubsidioEmpleoSat();
                subsidio.IdSubsidio = ListSubsidio[i].IdSubsidio;
                subsidio.LimiteInferior = ListSubsidio[i].LimiteInferior <= 0.01M ? 0.01M : Math.Round(lsubsidio[i -1].LimiteSuperior + 0.01M, 2);
                subsidio.LimiteSuperior = Math.Round((ListSubsidio[i].LimiteSuperior / factorAño) * numDiasTablasDiarias , 2);
                subsidio.CreditoSalario = Math.Round((ListSubsidio[i].CreditoSalario / factorAño) * numDiasTablasDiarias, 2);
                subsidio.FechaInicio = ListSubsidio[i].FechaInicio;
                subsidio.IdEstatus = ListSubsidio[i].IdEstatus;

                lsubsidio.Add(subsidio);
            }

            ListSubsidio = lsubsidio;
        }

        /// <summary>
        /// Metodo para obtener el la diferencia entre la base gravada y el limite inferior
        /// </summary>
        protected void Obten_Diferencia_Limite()
        {
            nominaTrabajo.DiferenciaLimite = 0;
            nominaTrabajo.DiferenciaLimite = nominaTrabajo.BaseGravada - nominaTrabajo.LimiteInferior;
        }

        /// <summary>
        /// Metodo para obtener el porcentaje calculado.
        /// </summary>
        protected void Obten_Porcentaje_Calculado()
        {
            nominaTrabajo.PorcentajeCalculado = 0;
            decimal resultset = ((decimal)nominaTrabajo.DiferenciaLimite * ((decimal)nominaTrabajo.Porcentaje)) / 100;
            nominaTrabajo.PorcentajeCalculado = decimal.Round(resultset, 2);
        }

        /// <summary>
        /// Metodo que obtiene el ISR causado.
        /// </summary>
        protected void Obten_ISR()
        {
            nominaTrabajo.ReintegroISR = 0;
            nominaTrabajo.ISR = 0;
            
            nominaTrabajo.ISR = nominaTrabajo.CuotaFija + nominaTrabajo.PorcentajeCalculado;

            //se obtiene el ISR mensualizado cuando hay proyeccion mensual
            if (UnidadNegocio.ISRProyeccionMensual == "S")
            {
                var ISRDiario = nominaTrabajo.ISR / UnidadNegocio.FactorDiasMesISR;
                nominaTrabajo.ISR = ISRDiario * DiasPago;
            }

            if (Periodo.AjusteDeImpuestos == "SI" && !AjusteAnual)
            {               
                if (ListNominaAjuste.Where(b => b.Rfc == RFC).FirstOrDefault() != null)
                {
                    var _isr = (from b in ListNominaAjuste.Where(b => b.Rfc == RFC) select b.ISR).Sum();
                    nominaTrabajo.ISR = nominaTrabajo.ISR - _isr;

                    if (nominaTrabajo.ISR < 0)
                    {
                        nominaTrabajo.ReintegroISR = Math.Abs((decimal)nominaTrabajo.ISR);
                        nominaTrabajo.ISR = 0;
                    }
                }
            }
            
        }

        /// <summary>
        /// Metodo que obtiene el subsidio al empleo efectivamente pagado.
        /// </summary>
        protected void Obten_Subsidio_Al_Empleo()
        {
            CreditoSalario = 0;
            nominaTrabajo.Subsidio = 0;
            string[] _tipoNom = UnidadNegocio.FiniquitosSubsidio == "S" ? new[] { "Complemento", "Aguinaldo" } : new[] { "Complemento", "Finiquitos", "Aguinaldo" };
            if (!_tipoNom.Contains( Periodo.TipoNomina))
            {
                if (Periodo.AjusteDeImpuestos == "NO")
                {
                    CalculoSubsidio();
                }
                else
                {
                    var _listaNomAjuste = ListNominaAjuste.Where(b => b.Rfc == RFC).FirstOrDefault();

                    if (_listaNomAjuste != null && !AjusteAnual)
                    {
                        CalculoSubsidio_Ajuste();
                    }
                    else
                    {
                        CalculoSubsidio();
                    }
                }
            }

            if (Periodo.TipoNomina != "Finiquitos" && UnidadNegocio.FiniquitosSubsidio != "S")
            {
                var dias = nominaTrabajo.DiasTrabajados + nominaTrabajo.Dias_Vacaciones;
                if (dias <= 0 && UnidadNegocio.SubsidioSinSueldo != "S")
                {
                    CreditoSalario = 0;
                    nominaTrabajo.Subsidio = 0;
                }
            }

            if (UnidadNegocio.ISRProyeccionMensual == "S")
            {
                CreditoSalario = 0;
                nominaTrabajo.Subsidio = 0;
            }
        }

        /// <summary>
        /// Metodo que obtiene el subsidio al empleo causado.
        /// </summary>
        private void CalculoSubsidio()
        {
            var query = ListSubsidio.Where(b=> b.LimiteSuperior >= nominaTrabajo.BaseGravada && b.LimiteInferior <= nominaTrabajo.BaseGravada).FirstOrDefault();

            if (query == null || AjusteAnual)            
                CreditoSalario = 0;            
            else            
                CreditoSalario = query.CreditoSalario;            

            nominaTrabajo.Subsidio = CreditoSalario;
        }

        /// <summary>
        /// Mentodo que obtiene el subsido al empleo causado cuando hay ajuste de impuestos.
        /// </summary>
        private void CalculoSubsidio_Ajuste()
        {
            nominaTrabajo.Subsidio = 0;

            var query = (from b in ListSubsidio_Ajuste.Where( b=> b.LimiteSuperior >= nominaTrabajo.BaseGravada && b.LimiteInferior <= nominaTrabajo.BaseGravada) select b).FirstOrDefault();

            if (query != null)
            {
                CreditoSalario = decimal.Parse(query.CreditoSalario.ToString());               
                var queryAjuste = (from b in ListNominaAjuste.Where(b => b.IdEmpleado == IdEmpleado) select b.Subsidio).Sum();
                if (queryAjuste != null) { CreditoSalario = CreditoSalario - (decimal)queryAjuste; }
                if (CreditoSalario < 0) { CreditoSalario = 0; }

                nominaTrabajo.Subsidio = CreditoSalario;
            }            
        }

        /// <summary>
        /// Metodo que obtiene el impuesto a reteer y el subsidio a pagar.
        /// </summary>
        protected void Obten_Impuestos_Retenidos()
        {            
            nominaTrabajo.SubsidioPagar = 0;
            nominaTrabajo.ImpuestoRetener = 0;
            decimal resultset = (decimal)nominaTrabajo.ISR - CreditoSalario;
           
            if (resultset < 0)
            {
                nominaTrabajo.SubsidioPagar = Math.Abs(resultset);
                nominaTrabajo.ImpuestoRetener = 0;
            }
            else
            {
                nominaTrabajo.SubsidioPagar = 0;
                nominaTrabajo.ImpuestoRetener = resultset;
            }

            if (AjusteAnual && Periodo.AjusteAnual == "S")
            {
                decimal reintegroAnual = 0;
                decimal subsidioEntregadoAnual = 0;
                decimal subsidioCausadoAnual = 0;
                decimal ISRRetAnual = 0;
                reintegroAnual = ListNominaAjuste.Where(x => x.Rfc == RFC).Select(x => x.ReintegroISR).Sum() ?? 0;
                subsidioEntregadoAnual = ListNominaAjuste.Where(x => x.Rfc == RFC).Select(x => x.SubsidioPagar).Sum() ?? 0;
                subsidioCausadoAnual = ListNominaAjuste.Where(x => x.Rfc == RFC).Select(x => x.Subsidio).Sum() ?? 0;
                ISRRetAnual = ListNominaAjuste.Where(x => x.Rfc == RFC).Select(x => x.ImpuestoRetener).Sum() ?? 0;

                decimal resultset2 = (((decimal)nominaTrabajo.ISR - subsidioCausadoAnual) - ISRRetAnual) + reintegroAnual + subsidioEntregadoAnual;

                if (resultset2 < 0)
                {
                    nominaTrabajo.ReintegroISR = Math.Abs(resultset2);
                    nominaTrabajo.ImpuestoRetener = 0;
                }
                else
                {
                    nominaTrabajo.ReintegroISR = 0;
                    nominaTrabajo.ImpuestoRetener = resultset2;
                }                
            }

            if (UnidadNegocio.RetencionISR_SMGV == "S")            
                nominaTrabajo.ImpuestoRetener = nominaTrabajo.ImpuestoRetener > 0 ? nominaTrabajo.ImpuestoRetener = 0 : nominaTrabajo.ImpuestoRetener;
            

            if (Periodo.TablaDiaria == "S")
            {
                ListImpuestos = ListImpuestos_Anterior;
                ListSubsidio = ListSubsidio_Anterior;
            }
        }

        /// <summary>
        /// Metodo para calcular las Cargas Sociales que corresponden al trabajador.
        /// </summary>
        protected void Calcula_Cuotas_Obreras()
        {
            UMA = (decimal)SueldosMinimos.UMA;
            Sueldo_Minimo = (decimal)SueldosMinimos.SalarioMinimoGeneral;

            Calcula_Dias_IMSS();
            Evalua_Tipo_Insidencia();
                        

            if ((Periodo.TipoNomina == "Complemento" && DiasTrabajados_IMSS < 1) || (UnidadNegocio.NoCalcularCargaObrera == "S" && UnidadNegocio.NoCalcularCargaPatronal == "S"))
            {
                SinCargarObreroPatronal();
            }
            else
            {
                
                foreach (var item in ListImpuestosIMSS.Where(c => c.TipoCuota == "Obrera"))
                {
                    switch (item.Descripcion)
                    {
                        #region Obrera
                        case "EXCEDENTE OBRERA":

                            if (SDI < (UMA * 3))
                            {
                                nominaTrabajo.Excedente_Obrera = 0;
                            }
                            else
                            {
                                decimal op1 = SDI - (UMA * 3);
                                decimal op2 = op1 * decimal.Parse(item.Porcentaje.ToString());
                                decimal op3 = decimal.Round((op2 * (DiasTrabajados_IMSS + Dias_Faltados)) / 100, 2);

                                nominaTrabajo.Excedente_Obrera = Math.Round(op3, 2);
                            }

                            break;

                        case "PRESTACIONES DINERO":

                            decimal _PrestacionDinero = (DiasTrabajados_IMSS + Dias_Faltados);
                            nominaTrabajo.Prestaciones_Dinero = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _PrestacionDinero) * (decimal)item.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "GASTOS MED. PENSION":

                            decimal _GMP = (DiasTrabajados_IMSS + Dias_Faltados); ;
                            nominaTrabajo.Gastos_Med_Pension = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _GMP) * (decimal)item.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "INVALIDEZ Y VIDA":

                            decimal _InvalidezVida = DiasTrabajados_IMSS;
                            nominaTrabajo.Invalidez_Vida = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _InvalidezVida) * (decimal)item.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "CESANTIA Y VEJEZ":

                            decimal _CesantiaVejez = DiasTrabajados_IMSS;
                            nominaTrabajo.Cesantia_Vejez = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _CesantiaVejez) * (decimal)item.Porcentaje) / (decimal)100, 2), 2);
                            break;
                            #endregion
                    }
                }
                

                decimal DiasTrabajados_IMSS_Patronal = DiasTrabajados_IMSS;

                foreach (var item2 in ListImpuestosIMSS.Where(c => c.TipoCuota == "Patronal"))
                {
                    switch (item2.Descripcion)
                    {
                        #region Patronal
                        case "CUOTA FIJA":

                            decimal _CuotaFija = (DiasTrabajados_IMSS_Patronal + Dias_Faltados);

                            if (SDI == 0)
                            {
                                nominaTrabajo.Cuota_Fija_Patronal = 0;
                            }
                            else
                            {
                                nominaTrabajo.Cuota_Fija_Patronal = Math.Round(decimal.Round(((decimal.Round(UMA, 2) * _CuotaFija) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            }

                            break;

                        case "EXCEDENTE PATRONAL":

                            if (SDI < (UMA * 3))
                            {
                                nominaTrabajo.Excedente_Patronal = 0;
                            }
                            else
                            {
                                decimal op1 = 0;
                                op1 = SDI - (UMA * 3);
                                decimal op2 = decimal.Round(op1, 2) * decimal.Round(decimal.Parse(item2.Porcentaje.ToString()), 3);
                                decimal op3 = decimal.Round((op2 * (DiasTrabajados_IMSS_Patronal + Dias_Faltados)) / 100, 2);

                                nominaTrabajo.Excedente_Patronal = Math.Round(op3, 2);
                            }

                            break;

                        case "PREST. DINERO":

                            decimal _PrestDinero = (DiasTrabajados_IMSS_Patronal + Dias_Faltados);
                            nominaTrabajo.Prestamo_Dinero_Patronal = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _PrestDinero) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "GASTOS MED. PENSION":

                            decimal _GastosMed = (DiasTrabajados_IMSS_Patronal + Dias_Faltados); ;
                            nominaTrabajo.Gastos_Med_Pension_Patronal = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _GastosMed) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "RIESGO DE TRABAJO":

                            decimal _RiesgoT = DiasTrabajados_IMSS_Patronal;
                            nominaTrabajo.Riesgo_Trabajo_Patronal = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * Porcentaje_Riesgo_Trabajo_Patronal) * _RiesgoT) / (decimal)100, 2), 2);
                            break;

                        case "INVALIDEZ Y VIDA":

                            decimal _InvalidezVida = DiasTrabajados_IMSS_Patronal;
                            nominaTrabajo.Invalidez_Vida_Patronal = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _InvalidezVida) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "GUARD. PREST EN ESPECIE":

                            decimal _Guarderia = DiasTrabajados_IMSS_Patronal;
                            nominaTrabajo.Prestamo_Especie = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _Guarderia) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "RETIRO":
                            decimal _Retiro = (DiasTrabajados_IMSS_Patronal + Incapacidades);
                            nominaTrabajo.Retiro = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _Retiro) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            break;

                        case "CESANTIA Y VEJEZ":
                            decimal _CesantiaVejez = DiasTrabajados_IMSS_Patronal;
                            nominaTrabajo.Cesantia_Vejez_Patronal = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _CesantiaVejez) * CalculaPorcentajeCyV_IMSS(SDI)) / (decimal)100, 2), 2);
                            break;

                        case "INFONAVIT":
                            decimal _Infonavit = (DiasTrabajados_IMSS_Patronal + Incapacidades);
                            nominaTrabajo.INFONAVIT_Patronal = Math.Round(decimal.Round(((decimal.Round(SDI, 2) * _Infonavit) * (decimal)item2.Porcentaje) / (decimal)100, 2), 2);
                            break;
                            #endregion
                    }
                }

                string[] gpos = { "500", "501" };
                var faltasIncapacidades = incidenciasEmpleado.Where(x => gpos.Contains(x.ClaveGpo) && _tipoEsquemaT.Contains(x.TipoEsquema)).Sum(x => x.Cantidad);

                decimal diasPago_ = (int)TipoNomina.DiasPago;
                if (Periodo.TablaDiaria == "S" || diasPago_==0)
                    diasPago_ = Periodo.FechaFin.Subtract(Periodo.FechaInicio).Days + 1; ;

                if (SD_IMSS <= Sueldo_Minimo || (configuracionNominaEmpleado.SupenderSueldoTradicional == 1) || (configuracionNominaEmpleado.DiasCargaSocial > 0 && nominaTrabajo.DiasTrabajados < 1 ) || faltasIncapacidades >= diasPago_)
                {
                    if (UnidadNegocio.CobraCOPS_Empleado_SMGV=="S" && ValidaCobroCOPS(IdPrestacionesEmpleado, nominaTrabajo.FechaReconocimientoAntiguedad, nominaTrabajo.FechaAltaIMSS, SD_IMSS,SDI))
                    {
                        nominaTrabajo.IMSS_Obrero = nominaTrabajo.Excedente_Obrera + nominaTrabajo.Prestaciones_Dinero + nominaTrabajo.Gastos_Med_Pension + nominaTrabajo.Invalidez_Vida + nominaTrabajo.Cesantia_Vejez;
                        nominaTrabajo.Total_Patron = nominaTrabajo.Cuota_Fija_Patronal + nominaTrabajo.Excedente_Patronal + nominaTrabajo.Prestamo_Dinero_Patronal + nominaTrabajo.Gastos_Med_Pension_Patronal + nominaTrabajo.Invalidez_Vida_Patronal
                        + nominaTrabajo.Prestamo_Especie + nominaTrabajo.Retiro + nominaTrabajo.Cesantia_Vejez_Patronal + nominaTrabajo.INFONAVIT_Patronal + nominaTrabajo.Riesgo_Trabajo_Patronal;
                    }
                    else
                    {
                        CargaObreraAlPatron();
                    }                    
                }
                else
                {
                    nominaTrabajo.IMSS_Obrero = nominaTrabajo.Excedente_Obrera + nominaTrabajo.Prestaciones_Dinero + nominaTrabajo.Gastos_Med_Pension + nominaTrabajo.Invalidez_Vida + nominaTrabajo.Cesantia_Vejez;
                    nominaTrabajo.Total_Patron = nominaTrabajo.Cuota_Fija_Patronal + nominaTrabajo.Excedente_Patronal + nominaTrabajo.Prestamo_Dinero_Patronal + nominaTrabajo.Gastos_Med_Pension_Patronal + nominaTrabajo.Invalidez_Vida_Patronal
                        + nominaTrabajo.Prestamo_Especie + nominaTrabajo.Retiro + nominaTrabajo.Cesantia_Vejez_Patronal + nominaTrabajo.INFONAVIT_Patronal + nominaTrabajo.Riesgo_Trabajo_Patronal;
                }

                if (UnidadNegocio.NoCalcularCargaPatronal == "S")
                    SinCargaPatronal();

                if (UnidadNegocio.NoCalcularCargaObrera == "S")
                    SincargaObrera();
            }                        

            // Para que las nominas de PTU no calculen COPS
            if (Periodo.TipoNomina=="PTU")
            {
                SinCargarObreroPatronal();
            }
        }

        /// <summary>
        /// Metodo para calcular los dias que se cobraran de carga social.
        /// </summary>
        public void Calcula_Dias_IMSS()
        {
            string[] _tipoNomina = { "Complemento", "Finiquitos", "Aguinaldo", "PTU" };
            
            if (DiasTrabajados_IMSS == 0 && !_tipoNomina.Contains(Periodo.TipoNomina) && IdEstatus == 1)
            {                
                DiasTrabajados_IMSS = Periodo.FechaFin.Subtract(Periodo.FechaInicio).Days + 1;
            }

            string[] grupos = { "500", "501" };

            DiasTrabajados_IMSS = DiasTrabajados_IMSS + (decimal)listIncidencias.Where(x => x.IdEmpleado == IdEmpleado && _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades"
                                                                                        && x.TipoConcepto == "ER" && x.AfectaCargaSocial == "SI" && x.IdEstatus == 1 && !grupos.Contains(x.ClaveGpo)).Sum(x => x.Cantidad);

            DiasTrabajados_IMSS = DiasTrabajados_IMSS - (decimal)listIncidencias.Where(x => x.IdEmpleado == IdEmpleado && _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades"
                                                                                        && x.TipoConcepto == "DD" && x.AfectaCargaSocial == "SI" && x.IdEstatus == 1 && !grupos.Contains(x.ClaveGpo)).Sum(x => x.Cantidad);

            if (configuracionNominaEmpleado.DiasCargaSocial > 0) { DiasTrabajados_IMSS += (decimal)configuracionNominaEmpleado.DiasCargaSocial; }

            DiasTrabajados_IMSS -= DiasMenosPorAlta;
            DiasTrabajados_IMSS += DiasMasPorAlta;
            nominaTrabajo.DiasTrabajadosIMSS = DiasTrabajados_IMSS;
            
            if (configuracionNominaEmpleado.SuspenderCargasSociales == 1 || nominaTrabajo.DiasTrabajadosIMSS < 0) 
            {
                DiasTrabajados_IMSS = 0;
                nominaTrabajo.DiasTrabajadosIMSS = 0;
            }
        }

        /// <summary>
        /// Metodo para cargar las cuotas obreras al patron.
        /// </summary>
        protected void CargaObreraAlPatron()
        {
            nominaTrabajo.Excedente_Patronal = nominaTrabajo.Excedente_Patronal + nominaTrabajo.Excedente_Obrera;
            nominaTrabajo.Prestamo_Dinero_Patronal = nominaTrabajo.Prestamo_Dinero_Patronal + nominaTrabajo.Prestaciones_Dinero;
            nominaTrabajo.Gastos_Med_Pension_Patronal = nominaTrabajo.Gastos_Med_Pension_Patronal + nominaTrabajo.Gastos_Med_Pension;
            nominaTrabajo.Invalidez_Vida_Patronal = nominaTrabajo.Invalidez_Vida_Patronal + nominaTrabajo.Invalidez_Vida;
            nominaTrabajo.Cesantia_Vejez_Patronal = nominaTrabajo.Cesantia_Vejez_Patronal + nominaTrabajo.Cesantia_Vejez;

            nominaTrabajo.Total_Patron = nominaTrabajo.Cuota_Fija_Patronal + nominaTrabajo.Excedente_Patronal + nominaTrabajo.Prestamo_Dinero_Patronal + nominaTrabajo.Gastos_Med_Pension_Patronal + nominaTrabajo.Invalidez_Vida_Patronal
                + nominaTrabajo.Prestamo_Especie + nominaTrabajo.Retiro + nominaTrabajo.Cesantia_Vejez_Patronal + nominaTrabajo.INFONAVIT_Patronal + nominaTrabajo.Riesgo_Trabajo_Patronal;

            nominaTrabajo.IMSS_Obrero = 0;
            nominaTrabajo.Excedente_Obrera = 0;
            nominaTrabajo.Prestaciones_Dinero = 0;
            nominaTrabajo.Gastos_Med_Pension = 0;
            nominaTrabajo.Invalidez_Vida = 0;
            nominaTrabajo.Cesantia_Vejez = 0;
        }

        /// <summary>
        /// Metodo que quita las cargas obreras y patronales, seteando sus valores en 0.
        /// </summary>
        protected void SinCargarObreroPatronal()
        {            
            nominaTrabajo.Excedente_Patronal = 0;
            nominaTrabajo.Prestamo_Dinero_Patronal = 0;
            nominaTrabajo.Gastos_Med_Pension_Patronal = 0;
            nominaTrabajo.Invalidez_Vida_Patronal = 0;
            nominaTrabajo.Cesantia_Vejez_Patronal = 0;
            nominaTrabajo.Total_Patron = 0;

            nominaTrabajo.IMSS_Obrero = 0;
            nominaTrabajo.Excedente_Obrera = 0;
            nominaTrabajo.Prestaciones_Dinero = 0;
            nominaTrabajo.Gastos_Med_Pension = 0;
            nominaTrabajo.Invalidez_Vida = 0;
            nominaTrabajo.Cesantia_Vejez = 0;
        }

        protected void SinCargaPatronal()
        {
            nominaTrabajo.Excedente_Patronal = 0;
            nominaTrabajo.Prestamo_Dinero_Patronal = 0;
            nominaTrabajo.Gastos_Med_Pension_Patronal = 0;
            nominaTrabajo.Invalidez_Vida_Patronal = 0;
            nominaTrabajo.Cesantia_Vejez_Patronal = 0;
            nominaTrabajo.Total_Patron = 0;
        }

        protected void SincargaObrera()
        {
            nominaTrabajo.IMSS_Obrero = 0;
            nominaTrabajo.Excedente_Obrera = 0;
            nominaTrabajo.Prestaciones_Dinero = 0;
            nominaTrabajo.Gastos_Med_Pension = 0;
            nominaTrabajo.Invalidez_Vida = 0;
            nominaTrabajo.Cesantia_Vejez = 0;
        }

        /// <summary>
        /// Metodo para evaluar el tipo de incidencia, ya sea falta o incapcidad.
        /// </summary>
        private void Evalua_Tipo_Insidencia()
        {   
            Dias_Faltados = (decimal)listIncidencias.Where(b => b.IdPeriodoNomina == IdPeriodoNomina && b.IdEmpleado == IdEmpleado && b.TipoDato == "Cantidades" && b.TipoConcepto == "DD"
                                                                && b.IdEstatus == 1 && _tipoEsquemaT.Contains(b.TipoEsquema) && b.ClaveGpo == "500" && b.AfectaCargaSocial == "SI").Sum(x => x.Cantidad);

            Dias_Faltados_IMSS = Dias_Faltados;

            Incapacidades = (decimal)listIncidencias.Where(b => b.IdPeriodoNomina == IdPeriodoNomina && b.IdEmpleado == IdEmpleado && b.TipoDato == "Cantidades" && b.TipoConcepto == "DD"
                                                                && b.IdEstatus == 1 && _tipoEsquemaT.Contains(b.TipoEsquema) && b.ClaveGpo == "501" && b.AfectaCargaSocial == "SI").Sum(x => x.Cantidad);

            if (Incapacidades == 15 && DiasTrabajados_IMSS == 15.20M) { Incapacidades += .20M; }
                        
            DiasTrabajados_IMSS = DiasTrabajados_IMSS - (Dias_Faltados + Incapacidades);

            if (DiasTrabajados_IMSS < 1)
            {
                DiasTrabajados_IMSS = 0;
                Dias_Faltados_IMSS = 0;
                Dias_Faltados = 0;
            }
        }

        /// <summary>
        /// Calcula el ISR en base a la base gravada que se proporciona, si el resultado es negativo es porque se debe de pagar subsidio
        /// </summary>
        /// <param name="BaseGravada">Base gravada que se utilizara para generar el calculo</param>
        /// <param name="FechaFin">Para obtener los valores correspondientes de las tablas de impuestos</param>
        /// <param name="TipoNomina">para obtener las tablas de impuestos correspondientes, se debe de proporcionar la clave SAT</param>
        /// <param name="CalculaSubsidio">se debe de especificar si la operacion calculara subsidio al empleo</param>
        /// <returns>Importe con en valos del ISR.</returns>
        public decimal CalculaISR(decimal BaseGravada, DateTime FechaFin, string TipoNomina, bool CalculaSubsidio)
        {
            decimal result = 0;
            decimal LimiteInferior = 0;
            decimal Porcentaje = 0;
            decimal CuotaFija = 0;
            decimal DiferenciaLimite = 0;
            decimal PorcentajeCalculado = 0;
            decimal ISR = 0;
            decimal CreditoSalario = 0;
            decimal Subsidio = 0;
            int IdTipoNomina = ObtenTipoNomina(TipoNomina);
            
            GetImpuestosSAT_Auxiliar(IdTipoNomina, FechaFin);
            GetSubsidioSAT_Auxiliar(IdTipoNomina, FechaFin);

            if (BaseGravada > 0)
            {

                LimiteInferior += ListImpuestos_Auxiliar.Where(x => x.LimiteSuperior >= BaseGravada && x.LimiteInferior <= BaseGravada).FirstOrDefault().LimiteInferior;
                Porcentaje += ListImpuestos_Auxiliar.Where(x => x.LimiteSuperior >= BaseGravada && x.LimiteInferior <= BaseGravada).FirstOrDefault().Porcentaje;
                CuotaFija += ListImpuestos_Auxiliar.Where(x => x.LimiteSuperior >= BaseGravada && x.LimiteInferior <= BaseGravada).FirstOrDefault().CuotaFija;

                DiferenciaLimite = BaseGravada - LimiteInferior;

                decimal resultset = (DiferenciaLimite * Porcentaje) / 100;
                PorcentajeCalculado = decimal.Round(resultset, 2);

                ISR = CuotaFija + PorcentajeCalculado;

                if (CalculaSubsidio)
                {
                    var query = (from b in ListSubsidio_Auxiliar.Where(b => b.LimiteSuperior >= BaseGravada && b.LimiteInferior <= BaseGravada) select b).FirstOrDefault();

                    if (query == null)
                    {
                        CreditoSalario = 0;
                    }
                    else
                    {
                        CreditoSalario = decimal.Parse(query.CreditoSalario.ToString());
                    }

                    Subsidio = CreditoSalario;
                }

                result = ISR - CreditoSalario;
            }

            return result;
        }

        /// <summary>
        /// Metodo para obtener el Identificador del tipo de nómina que se esta calculando.
        /// </summary>
        /// <param name="ClaveSAT">Clave del cátalogo del SAT para tipo nómina.</param>
        /// <returns>Identificador del tipo de nómina.</returns>
        protected int ObtenTipoNomina(string ClaveSAT)
        {
            int IdTipoNomina = 0;
            switch (ClaveSAT)
            {
                case "02":
                    IdTipoNomina = 1;
                    break;
                case "03":
                    IdTipoNomina = 2;
                    break;
                case "04":
                    IdTipoNomina = 3;
                    break;
                case "05":
                    IdTipoNomina = 4;
                    break;
            }

            return IdTipoNomina;
        }

        /// <summary>
        /// Calcula el ISR para un periodo de tipo PTU
        /// </summary>
        /// <param name="IdEstatusEmpleado">Identificador del estatus del empleado.</param>
        /// <param name="SDIMSSEmpleado">Salario Diario tradicional del empleado que se esta calculando.</param>
        public void CalculaISR_PTU(int IdEstatusEmpleado, decimal? SDIMSSEmpleado)
        {
            decimal? _baseGravadaPTU = 0;
            decimal _isr_BG_SM = 0;
            decimal _isr_SM = 0;
            decimal _isr_Final = 0;

            if (IdEstatusEmpleado == 1)
            {
                _baseGravadaPTU += incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && _tipoEsquemaT.Contains(x.TipoEsquema) && x.Integrable == "SI" && x.ClaveGpo != "003").Select(x => x.Gravado).Sum();
                _baseGravadaPTU += SDIMSSEmpleado * 30;

                _isr_BG_SM = CalculaISR((decimal)_baseGravadaPTU, DateTime.Now, "05", true);
                _isr_SM = CalculaISR((decimal)SDIMSSEmpleado * 30, DateTime.Now, "05", true);

                _isr_Final = _isr_BG_SM - _isr_SM;
            }
            else
            {
                _baseGravadaPTU += incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && _tipoEsquemaT.Contains(x.TipoEsquema) && x.Integrable == "SI" && x.ClaveGpo != "003").Select(x => x.Gravado).Sum();
                _isr_Final = CalculaISR((decimal)_baseGravadaPTU, DateTime.Now, "05", false);
            }

            nominaTrabajo.BaseGravada = _baseGravadaPTU;
            nominaTrabajo.BaseGravadaP = incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && _tipoEsquemaT.Contains(x.TipoEsquema) && x.Integrable == "SI" && x.ClaveGpo != "003").Select(x => x.Gravado).Sum();

            nominaTrabajo.LimiteInferior = 0;
            nominaTrabajo.Porcentaje = 0;
            nominaTrabajo.CuotaFija = 0;
            nominaTrabajo.DiferenciaLimite = 0;
            nominaTrabajo.PorcentajeCalculado = 0;
            nominaTrabajo.ISR = _isr_Final;
            nominaTrabajo.Subsidio = 0;
            nominaTrabajo.SubsidioPagar = 0;
            nominaTrabajo.ImpuestoRetener = _isr_Final;
            nominaTrabajo.ReintegroISR = 0;
        }

        public void CalculaISR_Piramidados()
        {
            nominaTrabajo.LimiteInferior = 0;
            nominaTrabajo.Porcentaje = 0;
            nominaTrabajo.CuotaFija = 0;
            nominaTrabajo.DiferenciaLimite = 0;
            nominaTrabajo.PorcentajeCalculado = 0;
            nominaTrabajo.ISR = conceptosPiramidadosEmpleado.Select(x=> x.ISR_Cobrar).Sum();
            nominaTrabajo.Subsidio = 0;
            nominaTrabajo.SubsidioPagar = 0;
            nominaTrabajo.ImpuestoRetener = conceptosPiramidadosEmpleado.Select(x => x.ISR_Cobrar).Sum();
            nominaTrabajo.ReintegroISR = 0;
        }

        public decimal CalculaISRPiramidado(decimal importe, DateTime FechaFin)
        {
            decimal ISR_Asimilado;
            decimal apoyo = 0;            
            var DatoAlQueLlegar = importe;
            decimal VarialbeGravada = DatoAlQueLlegar;
            decimal datoCondicion = DatoAlQueLlegar;
            while (datoCondicion >= .005M)
            {                
                var impuestoAsimilado = CalculaISR(VarialbeGravada, FechaFin, "05", false);
                                
                apoyo = VarialbeGravada;
                ISR_Asimilado = impuestoAsimilado;
                if (ISR_Asimilado < 0)                
                    ISR_Asimilado = 0;                

                var NetoAsimilado = apoyo - ISR_Asimilado;
                datoCondicion = importe - NetoAsimilado;
                VarialbeGravada += datoCondicion;
            }

            return apoyo;
        }

        public decimal CalculaPorcentajeCyV_IMSS(decimal SDI)
        {
            decimal porcentaje = 0;

            return porcentaje = ListFactoresCyV_IMSS.Where(x => x.LimiteInferior <= SDI && x.LimiteSuperior >= SDI).FirstOrDefault().Porcentaje;

        }        
    }
}