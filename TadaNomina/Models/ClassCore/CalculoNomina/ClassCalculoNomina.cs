using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using TadaNomina.Models.ClassCore.CalculoAguinaldo;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using System.Linq.Dynamic.Core;
using SAT.Services.ConsultaCFDIService;
using DocumentFormat.OpenXml.Wordprocessing;
using TadaNomina.Models.ClassCore.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.CalculoNomina
{
    /// <summary>
    /// Calculo de Nomina
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 16/05/2022, Razón: documentar el codigo
    /// </summary>
    public class ClassCalculoNomina: ClassProcesosAguinaldo
    {
        /// <summary>
        /// Metodo que genera y guarda todos los calculos del proceso de nómina.
        /// </summary>
        /// <param name="_IdPeriodoNomina"> Es el id del periodo que se va a cualcular</param>
        /// <param name="_IdEmpledao"> Este parametro es opcional, solo se ocupa cuando se quiere procesar el calculo de un solo empleado</param>
        /// <param name="_IdUsuario">Usuario del sistema que esta procesando.</param>
        public void Procesar(int _IdPeriodoNomina, int? _IdEmpledao, int _IdUsuario)
        {
            try
            {
                contador = 0;
                IdPeriodoNomina = _IdPeriodoNomina;
                IdUsuario = _IdUsuario;

                GetPeriodoNomina(IdPeriodoNomina);

                if (Periodo.TipoNomina == "PTU")
                    InsertaIncidenciasPTU(_IdPeriodoNomina, _IdUsuario);

                // Seteamos variables para percepciones especiales
                percepcionesEspecialesGravado = 0;
                percepcionesEspecialesExcento = 0;
                percepcionesEspecialesEsquema = 0;

                GetListas(IdPeriodoNomina, _IdEmpledao, _IdUsuario);

                int[] EstatusNomina = { 1, 3 };
                var empleadosProceso = listEmpleados;
                if (Periodo.TipoNomina != "Complemento")
                    empleadosProceso = listEmpleados.Where(x => EstatusNomina.Contains(x.IdEstatus)).ToList();

                if (Periodo.TipoNomina == "PTU")
                {
                    empleadosProceso = listEmpleados;
                }

                if (_IdEmpledao != null)
                    empleadosProceso = empleadosProceso.Where(x => x.IdEmpleado == _IdEmpledao).ToList();

                var IdsEmpleadosProceso = string.Join(",",empleadosProceso.Select(x => x.IdEmpleado).ToList());

                //calculo de septimo día, se pone en esta pocición para que actue solo sobre los empleados que se van a procesar
                if (UnidadNegocio.SeptimoDia == "S" && UnidadNegocio.IdConceptoSeptimoDia != null)
                    ProcesaIncidenciasSeptimoDia(empleadosProceso, _IdPeriodoNomina, (int)UnidadNegocio.IdConceptoSeptimoDia, IdUsuario);

                try { GetIncidencias(_IdPeriodoNomina); }
                catch (Exception ex) { Statics.generaLog(IdPeriodoNomina, IdUsuario, "Fallo al llenar las listas con los datos necesarios: " + ex.Message, "Nomina"); }

                if (Periodo.TipoNomina == "Aguinaldo")
                {
                    if (UnidadNegocio.IncidenciasAguinaldoAutomaticas == 1)
                    {
                        CalculaAguinaldos(Periodo, empleadosProceso, UnidadNegocio.IdCliente, IdUsuario);
                        GetIncidencias(_IdPeriodoNomina);
                    }
                }

                foreach (var item in empleadosProceso)
                {
                    try
                    {
                        nominaTrabajo = new NominaTrabajo();
                        IdEmpleado = item.IdEmpleado;
                        ClaveEmpleado = item.ClaveEmpleado;
                        RFC = item.Rfc;
                        IdEstatus = item.IdEstatus;
                        EsquemaPago = item.Esquema;
                        SDI = GetSD(item.SDI);
                        SD_IMSS = GetSD(item.SDIMSS);
                        SD_Real = GetSD(item.SD);
                        IdEntidadFederativa = item.IdEntidad;
                        IdPrestacionesEmpleado = item.IdPrestaciones;
                        nominaTrabajo.Anios = Math.Round(ObtenAntiguedadEmpleado(item.FechaReconocimientoAntiguedad, Periodo.FechaFin), 2);

                        if (Periodo.TipoNomina == "Proyeccion")
                        {
                            if (item.SDI_Proyeccion != null && item.SDI_Proyeccion > 0) { SDI = GetSD(item.SDI_Proyeccion); }
                            if (item.SDIMSS_Proyeccion != null && item.SDIMSS_Proyeccion > 0) { SD_IMSS = GetSD(item.SDIMSS_Proyeccion); }
                            if (item.SD_Proyeccion != null && item.SD_Proyeccion > 0) { SD_Real = GetSD(item.SD_Proyeccion); }
                        }

                        incidenciasEmpleado = listIncidencias.Where(x => x.IdEmpleado == item.IdEmpleado).ToList();
                        configuracionNominaEmpleado = getconfiguracionEmpleadoNomina(IdEmpleado);
                        conceptosPiramidadosEmpleado = conceptosPiramidados.Where(x => x.IdEmpleado == IdEmpleado).ToList();

                        ClassCalculoAdelantoNomina cCAN = new ClassCalculoAdelantoNomina();
                        cCAN.CalculaAdelantoNomina(item, IdPeriodoNomina, IdUsuario);

                        Porcesa_Nomina_Tradicional(item);
                        Procesa_Nomina_Real();
                        Procesa_Nomina_Esquema(item);

                        if (Periodo.DescuentosFijos == "SI")
                        {
                            if (IdEstatus == 1 || configuracionNominaEmpleado.IncidenciasAutomaticas == 1)
                            {
                                decimal? restaPension = incidenciasEmpleado.Where(x => x.IntegraPension == "NO").Select(x => x.Monto).Sum();

                                ProcesaPension(incidenciasEmpleado, nominaTrabajo, item, pensionAlimenticia.Where(x => x.IdEmpleado == IdEmpleado).ToList(), IdPeriodoNomina, (decimal)(nominaTrabajo.ER - (nominaTrabajo.ImpuestoRetener ?? 0) - nominaTrabajo.IMSS_Obrero - restaPension), (decimal)nominaTrabajo.ERS, IdUsuario, UnidadNegocio.IdCliente);
                                ProcesaSaldos(saldos.Where(x => x.IdEmpleado == IdEmpleado).ToList(), IdEmpleado, IdPeriodoNomina, IdUsuario);
                            }
                        }

                        incidenciasEmpleado = GetIncidenciasEmpleado_(IdPeriodoNomina, IdEmpleado);
                        nominaTrabajo.TotalEfectivo = nominaTrabajo.Neto - incidenciasEmpleado.Where(x => x.PagoEfectivo == "NO").Sum(x => x.Monto);

                        CalculaISN();

                        DeleteRegistroNominaTrabajo();
                        if (ValidaInsercionRegistro())
                        {
                            GuardarNominaTrabajo(item, IdPeriodoNomina);
                        }

                        if (UnidadNegocio.CalculaProvision == "S" && Periodo.TipoNomina == "Nomina")
                        {
                            ProcesaProvision(nominaTrabajo.DiasTrabajados, IdPrestacionesEmpleado, item.FechaReconocimientoAntiguedad, item.FechaAltaIMSS, (decimal)(UnidadNegocio.FactorDiasProvision ?? throw new Exception("No se encontro el factor para calcular la provisión.")));
                        }

                        contador++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Fallo al procesar nomina del empleado: " + item.IdEmpleado + " - " + item.ClaveEmpleado + " - " + item.NombreCompleto + ". cont: " + contador + "." + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en proceso general. " + ex.Message);
            }
        }

        /// <summary>
        /// Metodo que calcula la parte tradicional del calculo de nómina.
        /// </summary>
        /// <param name="datosEmpleados">Objeto con toda la información del empleado en proceso.</param>
        private void Porcesa_Nomina_Tradicional(vEmpleados datosEmpleados)
        {
            try
            {
                try { Porcentaje_Riesgo_Trabajo_Patronal = ListRegistroPatronal.Where(x => x.IdRegistroPatronal == datosEmpleados.IdRegistroPatronal).Select(x => x.RiesgoTrabajo).FirstOrDefault(); }
                catch { Porcentaje_Riesgo_Trabajo_Patronal = 0; }

                string[] _tipoNomina = { "Complemento", "Aguinaldo", "PTU" };
                if (!_tipoNomina.Contains(Periodo.TipoNomina) && UnidadNegocio.Honorarios != "S")
                    getDiasPorAlta(datosEmpleados.FechaAltaIMSS, Periodo.FechaInicio, Periodo.FechaFin);

                DiasPago = datosEmpleados.DiasPago;
                DiasTrabajados_IMSS = (decimal)datosEmpleados.DiasIMSS;

                //en caso de ser nomina semanal y con 7mo dia quita un día para que no afecte la incidencia creada
                if (UnidadNegocio.SeptimoDia == "S" && UnidadNegocio.IdConceptoSeptimoDia != null && UnidadNegocio.IdConceptoSeptimoDia != 0 && configuracionNominaEmpleado.SupenderSueldoTradicional != 1)
                {
                    DiasPago -= 1;
                    DiasTrabajados_IMSS -= DiasTrabajados_IMSS > 0 ? DiasTrabajados_IMSS += 1 : 0;
                }

                // Cuando el tipo de nomina tiene 0 en dias de pago se calcula con dias naturales
                if (DiasPago == 0 && datosEmpleados.TipoNomina != "Honorarios")
                    DiasPago = Periodo.FechaFin.Subtract(Periodo.FechaInicio).Days + 1;

                if (Periodo.TablaDiaria == "S")
                {
                    DiasPago = Periodo.FechaFin.Subtract(Periodo.FechaInicio).Days + 1M;
                    DiasTrabajados_IMSS = DiasPago;
                }

                DiasPago += DiasMasPorAlta;
                DiasPago -= DiasMenosPorAlta;

                if (_tipoNomina.Contains(Periodo.TipoNomina) || datosEmpleados.IdEstatus == 3)
                {
                    DiasPago = 0;
                    DiasTrabajados_IMSS = 0;
                }

                nominaTrabajo.FactorIntegracion = prestaciones.Where(x => x.IdPrestaciones == datosEmpleados.IdPrestaciones).OrderByDescending(x=> x.FechaInicioVigencia).Select(x => x.FactorIntegracion).FirstOrDefault() ?? 0;
                if (nominaTrabajo.FactorIntegracion == null)
                    nominaTrabajo.FactorIntegracion = prestaciones.Where(x => x.IdCliente == 0).OrderByDescending(x => x.FechaInicioVigencia).Select(x => x.FactorIntegracion).FirstOrDefault();

                nominaTrabajo.FechaAltaIMSS = datosEmpleados.FechaAltaIMSS;
                nominaTrabajo.FechaReconocimientoAntiguedad = datosEmpleados.FechaReconocimientoAntiguedad;

                GetDiasTrabajados(_tipoEsquemaT);

                nominaTrabajo.SueldoPagado = 0;

                if ((datosEmpleados.TipoContrato == "PRACTICANTE" && datosEmpleados.IdJornada != 0 && datosEmpleados.IdJornada != null))
                {
                    decimal sdHoras = (decimal)(datosEmpleados.SD / 8);
                    decimal horas = obtenHorasJornadaLaboral((int)datosEmpleados.IdJornada);
                    decimal SDRE = sdHoras * horas;
                    nominaTrabajo.SueldoPagado = TipoNomina.DiasPago * SDRE;
                }
                else
                {
                    nominaTrabajo.SueldoPagado += (nominaTrabajo.DiasTrabajados + fraccionHorasMas - fraccionHorasMenos) * SD_IMSS;


                }
                nominaTrabajo.Sueldo_Vacaciones = nominaTrabajo.Dias_Vacaciones * SD_IMSS;
                if (configuracionNominaEmpleado.SupenderSueldoTradicional == 1) { nominaTrabajo.SueldoPagado = 0; }

                nominaTrabajo.ER = (decimal)nominaTrabajo.SueldoPagado;
                SumaIncidencias();

                if (UnidadNegocio.SeptimoDia == "S" && UnidadNegocio.IdConceptoSeptimoDia != null && UnidadNegocio.IdConceptoSeptimoDia > 0)
                {
                    decimal septimoDiaInc = incidenciasEmpleado.Where(x => x.IdConcepto == (int)UnidadNegocio.IdConceptoSeptimoDia).Select(x => x.Cantidad).Sum() ?? 0;
                    nominaTrabajo.DiasTrabajados += septimoDiaInc;

                    if (septimoDiaInc > 0)
                    {
                        var diasMenosFraccion = incidenciasEmpleado.Where(x => x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.AfectaSeldo == "SI" && x.CalculoDiasHoras == "Horas").ToList();
                        foreach (var ifrac in diasMenosFraccion)
                        {
                            decimal fraccion = Math.Round((ifrac.Cantidad ?? 0) * (1M / (6M * (ifrac.SDEntre ?? 1))), 2);
                            nominaTrabajo.DiasTrabajados += fraccion;
                        }
                    }
                }

                if ((UnidadNegocio.PercepcionesEspeciales == "S" && datosEmpleados.IdEstatus == 1) || (configuracionNominaEmpleado.IncidenciasAutomaticas == 1))
                {
                    if (configuracionNominaEmpleado.SupenderSueldoTradicional == null)
                    {
                        GetPercepciones_pp(datosEmpleados.IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, datosEmpleados.IdPuesto, datosEmpleados.Compensacion_Dia_Trabajado, nominaTrabajo.DiasTrabajados, nominaTrabajo.Faltas, SD_IMSS, IdUsuario, datosEmpleados.IdCliente, datosEmpleados.IdCentroCostos);
                        nominaTrabajo.ER += (percepcionesEspecialesGravado + percepcionesEspecialesExcento);
                    }
                }

                //condigo para insertar incidencias que se calculan automaticamente
                ConceptosFormulados(datosEmpleados, "IF");
                ConceptosFormulados(datosEmpleados, "ER");
                ProcesaIncidenciasMultiplicaDT();
                nominaTrabajo.ER += montoIncidenciasMultiplicaDT;

                if (Periodo.TipoNomina == "PTU")
                {
                    CalculaISR_PTU(datosEmpleados.IdEstatus, datosEmpleados.SDIMSS);
                }
                else if (Periodo.TipoNomina == "Complemento" && conceptosPiramidadosEmpleado.Count > 0)
                {
                    CalculaISR_Piramidados();
                }
                else if ((Periodo.TipoNomina == "Aguinaldo" && UnidadNegocio.ISRAguinaldoL174 == "S"))
                {
                    CalculaISRAguinaldoL174();
                }
                else if ((Periodo.TipoNomina == "Complemento" && UnidadNegocio.ISRProyeccionMensual == "S"))
                {
                    CalculaISRComplementoProyMensual();
                }
                else
                {
                    if ((UnidadNegocio.ConfiguracionSueldos != "Netos Tradicional(Piramida)" && UnidadNegocio.ConfiguracionSueldos != "Netos Tradicional(Piramida ART 93)") || (datosEmpleados.NetoPagar ?? 0) == 0)
                        CalculaISR();
                }

                if (ValidacionDiasEquivalentes() && UnidadNegocio.BanderaDiasEquivalentes == "SI")
                {
                    List<int> mesesvariables = new List<int>() { 1, 3, 5, 7, 9, 11 };
                    var mesfin = Periodo.FechaFin.Month;
                    if (mesesvariables.Contains(mesfin))
                    {
                        Calcula_Cuotas_ObrerasEquivalentes();
                    }
                    else
                    {
                        Calcula_Cuotas_Obreras(null);
                    }
                }
                else
                {
                    Calcula_Cuotas_Obreras(null);
                }

                //configuracion para piramidar sueldos
                PiramidaSueldosConCompensacion(datosEmpleados);

                if (UnidadNegocio.ISRProyeccionMensual == "S" && Periodo.TipoNomina == "Nomina")
                {
                    nominaTrabajo.BaseGravada = baseMostrar;
                    nominaTrabajo.BaseGravadaP = baseMostrar;
                }

                if (Periodo.DescuentosFijos == "SI" && Periodo.TipoNomina == "Nomina")
                {
                    if (datosEmpleados.IdEstatus == 1 || configuracionNominaEmpleado.IncidenciasAutomaticas == 1)
                    {
                        ProcesaCredito(creditosInfonavit.Where(x => x.IdEmpleado == IdEmpleado).FirstOrDefault(), IdPeriodoNomina, (decimal)SueldosMinimos.UnidadMixta, DiasTrabajados_IMSS, IdUsuario, datosEmpleados.TipoNomina, datosEmpleados.IdUnidadNegocio);
                        ProcesaCreditoFonacot(creditosFonacot.Where(x => x.IdEmpleado == IdEmpleado).ToList(), IdPeriodoNomina, IdUsuario, datosEmpleados.TipoNomina);
                    }
                }

                nominaTrabajo.ER += nominaTrabajo.SubsidioPagar ?? 0;
                nominaTrabajo.ER += nominaTrabajo.ReintegroISR ?? 0;

                //CalculaISN();

                nominaTrabajo.DD = 0;
                nominaTrabajo.DD += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "DD").Select(X => X.Monto).Sum();
                nominaTrabajo.DD += nominaTrabajo.ImpuestoRetener;
                nominaTrabajo.DD += nominaTrabajo.IMSS_Obrero;
                nominaTrabajo.DD += montoCreditoInfonavit;
                nominaTrabajo.DD += montoCreditoFonacot;

                if ((UnidadNegocio.BanderaCuotaSindical == "S" && datosEmpleados.IdEstatus == 1))
                {
                    int IdConcepto = 0;
                    int IdConceptoVacaciones = 0;
                    try { IdConceptoVacaciones = (int)conceptosConfigurados.IdConceptoVacaciones; } catch { throw new Exception("Hay Cuota Sindical , no se configuro ningun concepto. "); }
                    try { IdConcepto = (int)conceptosConfigurados.idConceptoCuotaSindical; } catch { throw new Exception("Hay Cuota Sindical , no se configuro ningun concepto. "); }

                    CalculaCuotasSindicales(IdPeriodoNomina, IdEmpleado, IdConcepto, IdConceptoVacaciones, nominaTrabajo.DiasTrabajados, SD_IMSS, IdUsuario);
                }

                if ((UnidadNegocio.DeduccionesEspeciales == "S" && datosEmpleados.IdEstatus == 1) || (configuracionNominaEmpleado.IncidenciasAutomaticas == 1))
                {
                    if (configuracionNominaEmpleado.SupenderSueldoTradicional == null)
                        nominaTrabajo.DD += GetDeduccionesEspeciales(datosEmpleados.IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, datosEmpleados.IdPuesto, datosEmpleados.Compensacion_Dia_Trabajado, nominaTrabajo.DiasTrabajados, nominaTrabajo.Faltas, SD_IMSS, IdUsuario, datosEmpleados.IdCliente, datosEmpleados.IdCentroCostos);
                }

                //procesa todos los conceptos formulados de deduccion
                ConceptosFormulados(datosEmpleados, "DD");

                if (nominaTrabajo.ER <= 0)
                    nominaTrabajo.DD = 0;
                //EliminaMontosIncidenciasAusentismos();            

                nominaTrabajo.Neto = 0;
                nominaTrabajo.Neto = nominaTrabajo.ER - nominaTrabajo.DD;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo al procesar nomina tradicional del empleado: " + datosEmpleados.IdEmpleado + " - " + datosEmpleados.ClaveEmpleado + " - " + datosEmpleados.NombreCompleto + ". " + ex.Message);
            }
        }

        private void SumaIncidencias()
        {
            if ((conceptosConfigurados.IdConceptoCompensacion != null && conceptosConfigurados.IdConceptoCompensacion != 0) || (conceptosConfigurados.IdConceptoArt93Fraclll != null && conceptosConfigurados.IdConceptoArt93Fraclll != 0))
            {
                decimal importeIncidencias = (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll ).Select(X => X.Monto).Sum();
                importeIncidencias += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "OTRO" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll ).Select(X => X.Monto).Sum();
                nominaTrabajo.ER += importeIncidencias;
            }
            else
            {
                decimal importeIncidencias = (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.MultiplicaDT != "SI" ).Select(X => X.Monto).Sum();
                importeIncidencias += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "OTRO" && x.MultiplicaDT != "SI" ).Select(X => X.Monto).Sum();
                nominaTrabajo.ER += importeIncidencias;
            }
        }
        
        /// <summary>
        /// Metodo que calcula la parte esquema del calculo de nómina
        /// </summary>
        /// <param name="item">Objeto con toda la información del empleado en proceso.</param>
        /// <exception cref="Exception">Excepcion devuelta por el metodo en caso de un error.</exception>
        private void Procesa_Nomina_Esquema(vEmpleados item)
        {
            nominaTrabajo.Apoyo = 0;
            nominaTrabajo.ERS = 0;
            nominaTrabajo.DDS = 0;
            nominaTrabajo.Netos = 0;
            SD_Esquema = 0;

            decimal NetoPagar = 0;
            try { NetoPagar = (decimal)item.NetoPagar; } catch { }
            
            string[] _tipoNomina = { "Complemento", "Aguinaldo", "PTU" };
            if (!_tipoNomina.Contains(Periodo.TipoNomina))
            {
                if (NetoPagar > 0)
                    nominaTrabajo.Apoyo = NetoPagar - (decimal)nominaTrabajo.Neto;
                else
                {
                    if (EsquemaPago != "100% TRADICIONAL")
                    {
                        SD_Esquema = SD_Real - SD_IMSS;
                        if (UnidadNegocio.ConfiguracionSueldos == "Netos(Impuestos)")
                        {
                            if (nominaTrabajo.DiasTrabajados > 0)
                                nominaTrabajo.Apoyo += (SD_Real * (nominaTrabajo.DiasTrabajados)) - (nominaTrabajo.ER - nominaTrabajo.ImpuestoRetener - nominaTrabajo.IMSS_Obrero);

                            if (nominaTrabajo.Apoyo < 0)
                                nominaTrabajo.Apoyo = 0;
                        }
                        else if (UnidadNegocio.ConfiguracionSueldos == "Netos(Real)" || UnidadNegocio.ConfiguracionSueldos == "NetosPagar")
                        {
                            nominaTrabajo.Apoyo += nominaTrabajo.SueldoPagado_Real - nominaTrabajo.SueldoPagado;
                            nominaTrabajo.Apoyo += nominaTrabajo.IMSS_Obrero;
                            nominaTrabajo.Apoyo += nominaTrabajo.ImpuestoRetener;
                            nominaTrabajo.Apoyo -= nominaTrabajo.SubsidioPagar;

                            if (nominaTrabajo.Apoyo < 0)
                                nominaTrabajo.Apoyo = 0;
                        }
                        else
                        {
                            nominaTrabajo.Apoyo += nominaTrabajo.DiasTrabajados * SD_Esquema;
                        }
                    }
                }
            }

            if (Periodo.TipoNomina == "Aguinaldo" && NetoPagar > 0)
            {                
                ClassConceptosFiniquitos cfiniquitos = new ClassConceptosFiniquitos();
                int? IdConcepto = null;
                try { IdConcepto = (cfiniquitos.GetvConfiguracionConceptosFiniquitos(item.IdCliente).IdConceptoAguinaldo).Value; } catch (Exception ex) { throw new Exception("No se ha configurado el concepto para aguinaldo." + ex.Message); }

                var incidencia_  = incidenciasEmpleado.Where(x => x.IdConcepto == IdConcepto && x.BanderaAguinaldos == 1).FirstOrDefault();

                if (incidencia_ != null)
                {
                    incidenciasEmpleado.Remove(incidencia_);
                    decimal nuevoMontoEsquema = 0;
                    nuevoMontoEsquema = NetoPagar - (decimal)nominaTrabajo.Neto;
                    nominaTrabajo.ERS += nuevoMontoEsquema;
                    UpdateImporteEsquema((int)incidencia_.IdIncidencia, nuevoMontoEsquema);
                }
            }

            if (configuracionNominaEmpleado.SuspenderSueldoEsquema == 1) { nominaTrabajo.Apoyo = 0; }
            if (EsquemaPago != "100% TRADICIONAL")
            {
                nominaTrabajo.Apoyo += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.SumaNetoFinal == "SI").Select(X => X.Monto).Sum();
                nominaTrabajo.Apoyo -= (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "DD" && x.SumaNetoFinal == "SI").Select(X => X.Monto).Sum();
            }

            if (UnidadNegocio.ConfiguracionSueldos == "NetosPagar" && item.NetoPagar != null && item.NetoPagar > 0)
            {
                nominaTrabajo.Apoyo -= (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaS.Contains(x.TipoEsquema) && x.TipoConcepto == "ER").Select(X => X.MontoEsquema).Sum();
                nominaTrabajo.Apoyo += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaS.Contains(x.TipoEsquema) && x.TipoConcepto == "DD").Select(X => X.MontoEsquema).Sum();
            }

            nominaTrabajo.ERS += nominaTrabajo.Apoyo;
            nominaTrabajo.ERS += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaS.Contains(x.TipoEsquema) && x.TipoConcepto == "ER").Select(X => X.MontoEsquema).Sum();
            nominaTrabajo.ERS += percepcionesEspecialesEsquema;
            nominaTrabajo.DDS += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaS.Contains(x.TipoEsquema) && x.TipoConcepto == "DD").Select(X => X.MontoEsquema).Sum();
            nominaTrabajo.DDS += montoCreditoInfonavitEsq;
            
            nominaTrabajo.Netos = nominaTrabajo.ERS - nominaTrabajo.DDS;
            //CerosEnNegativos();
        }        

        /// <summary>
        /// Metodo que calcula la nómina real.
        /// </summary>
        private void Procesa_Nomina_Real()
        {
            try
            {
                nominaTrabajo.SueldoPagado_Real = 0;
                nominaTrabajo.Total_ER_Real = 0;
                nominaTrabajo.Total_DD_Real = 0;

                if (UnidadNegocio.ConfiguracionSueldos == "Netos(Real)" || UnidadNegocio.ConfiguracionSueldos == "NetosPagar" || UnidadNegocio.ConfiguracionSueldos == "Real-Tradicional")
                {
                    nominaTrabajo.AniosReal = nominaTrabajo.Anios;

                    //calcula sueldos
                    nominaTrabajo.SueldoPagado_Real = 0;
                    nominaTrabajo.SueldoPagado_Real += SD_Real * nominaTrabajo.DiasTrabajados;                    

                    //calcula vacaciones
                    nominaTrabajo.Sueldo_Vacaciones_Real = 0;
                    nominaTrabajo.Sueldo_Vacaciones_Real = nominaTrabajo.Dias_Vacaciones * SD_Real;

                    //total de percepciones
                    nominaTrabajo.Total_ER_Real = 0;
                    nominaTrabajo.Total_ER_Real += nominaTrabajo.SueldoPagado_Real;

                    nominaTrabajo.Total_ER_Real += UnidadNegocio.ConfiguracionSueldos == "Real-Tradicional" ? 
                        (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "ER").Select(X => X.MontoReal).Sum()
                        : (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "ER").Select(X => X.Monto + X.MontoEsquema).Sum();

                    //Base gravada e ISR / Subsidio al empleo                    
                    CalculaISR_Real();                                        
                    nominaTrabajo.Total_ER_Real += nominaTrabajo.Subsidio_Real;

                    //cuotas obrero patronales
                    nominaTrabajo.IMSS_Obrero_Real = 0;
                    nominaTrabajo.IMSS_Patronal_Real = 0;

                    //total de deducciones
                    nominaTrabajo.Total_DD_Real = 0;
                    nominaTrabajo.Total_DD_Real += nominaTrabajo.ISR_Real + nominaTrabajo.IMSS_Obrero_Real;
                    nominaTrabajo.Total_DD_Real += UnidadNegocio.ConfiguracionSueldos == "Real-Tradicional" ?
                        (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "DD").Select(X => X.MontoReal).Sum()
                        : (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "DD").Select(X => X.Monto + X.MontoEsquema).Sum();

                    //netos reales
                    nominaTrabajo.Neto_Real = 0;
                    nominaTrabajo.Neto_Real += nominaTrabajo.Total_ER_Real - nominaTrabajo.Total_DD_Real;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo al calcular la nómina real del empleado:" + IdEmpleado + " - " + ClaveEmpleado + " - " + ". " + ex.Message);
            }
           
        }

        /// <summary>
        /// Metodo que setea todos los valores en cero.
        /// </summary>
        private void CerosEnNegativos()
        {
            if (nominaTrabajo.Apoyo < 0)
                nominaTrabajo.Apoyo = 0;

            if (nominaTrabajo.ERS < 0)
                nominaTrabajo.ERS = 0;

            if (nominaTrabajo.DDS < 0)
                nominaTrabajo.DDS = 0;

            if (nominaTrabajo.Netos < 0)
                nominaTrabajo.Netos = 0;
        }

        /// <summary>
        /// Metodo para actualizar el importe esquema de la incidencias generada en el calculo de aguinaldo.
        /// </summary>
        /// <param name="IdIncidencia">Identificador de la incidencia de aguinaldo</param>
        /// <param name="NuevoMonto">Importe con el cual se actualiza la incidencia.</param>
        public void UpdateImporteEsquema(int? IdIncidencia, decimal NuevoMonto)
        {
            ClassIncidencias cincidencias = new ClassIncidencias();
            if (IdIncidencia != null)
                cincidencias.updateMontoEsquemaAguinado((int)IdIncidencia, NuevoMonto);
        }

        private void EliminaMontosIncidenciasAusentismos()
        {
            string[] gpos = { "500", "501" };
            var incidencias = incidenciasEmpleado.Where(x => gpos.Contains(x.ClaveGpo) && _tipoEsquemaT.Contains(x.TipoEsquema)).ToList();

            ClassIncidencias cins = new ClassIncidencias();
            foreach (var item in incidencias)
            {
                cins.EliminaMontosAusentismos(item.IdIncidencia);
            }
        }

        private decimal getImporteConcepto(vEmpleados datosEmpleados, decimal imss)
        {
            decimal importeConcepto = 0;
            importeConcepto += (datosEmpleados.NetoPagar ?? 0) - (nominaTrabajo.SueldoPagado ?? 0) + (nominaTrabajo.ImpuestoRetener ?? 0) + imss;
            importeConcepto -= incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll).Select(X => X.Monto).Sum() ?? 0;
            importeConcepto -= incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "OTRO" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll).Select(X => X.Monto).Sum() ?? 0;
            importeConcepto += incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "DD" && x.MultiplicaDT != "SI" && x.ClaveSAT != "001").Select(X => X.Monto).Sum() ?? 0;
            return importeConcepto;
        }

        /// <summary>
        /// Función que piramida el ímporte dado.
        /// </summary>
        /// <param name="importe">Importe a piramidar</param>
        /// <param name="FechaFin">Fecha que se utiliza para obtener las tablas mas recientes de impuestos.</param>
        /// <returns></returns>
        public decimal Piramida(decimal importe, DateTime FechaFin)
        {
            decimal ISR_Asimilado;
            decimal apoyo = 0;
            var DatoAlQueLlegar = importe;
            decimal VarialbeGravada = DatoAlQueLlegar;
            decimal datoCondicion = DatoAlQueLlegar;
            while (datoCondicion >= .005M)
            {
                var impuestoAsimilado = CalculaISR(VarialbeGravada, FechaFin, false);

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

        /// <summary>
        /// Inserta conceptos de monto para las nominas que piramidan
        /// </summary>
        /// <param name="IdConcepto">Identificador del concepto al que se insertara la incidencia.</param>
        /// <param name="Monto">Monto o importe que se insertara.</param>
        public void InsertaIncidenciaConceptoCompensacionPiramidar(int IdConcepto, decimal Monto)
        {
            if (IdConcepto != 0)
            {
                ClassIncidencias cins = new ClassIncidencias();
                ModelIncidencias model = new ModelIncidencias();
                cins.DeleteIncidencia(IdConcepto, IdEmpleado, IdPeriodoNomina);

                model.IdEmpleado = IdEmpleado;
                model.IdPeriodoNomina = IdPeriodoNomina;
                model.IdConcepto = (int)IdConcepto;
                model.Monto = Monto;
                model.Observaciones = "PDUP SYSTEM Concepto creado por el sistema para las nominas que piramidan";
                model.MontoEsquema = 0;

                if (model.IdConcepto != 0)
                    cins.NewIncindencia(model, IdUsuario);
            }
        }

        /// <summary>
        /// Inserta conceptos de cantidad para las nominas que piramidan.
        /// </summary>
        /// <param name="IdConcepto">Identificador del concepto al que se insertara la incidencia.</param>
        /// <param name="Cantidad">CAntidad que se insertara.</param>
        public void InsertaIncidenciaConceptoCompensacionFaltas(int IdConcepto, decimal Cantidad)
        {
            if (IdConcepto != 0)
            {
                ClassIncidencias cins = new ClassIncidencias();
                ModelIncidencias model = new ModelIncidencias();
                cins.DeleteIncidencia(IdConcepto, IdEmpleado, IdPeriodoNomina);

                model.IdEmpleado = IdEmpleado;
                model.IdPeriodoNomina = IdPeriodoNomina;
                model.IdConcepto = (int)IdConcepto;
                model.Cantidad = Cantidad;
                model.Observaciones = "PDUP SYSTEM Concepto creado por el sistema para las nominas que piramidan";                

                if (model.IdConcepto != 0)
                    cins.NewIncindencia(model, IdUsuario);
            }
        }

        /// <summary>
        /// Elimina la incidencia creada automaticamente por los conceptos formulados.
        /// </summary>
        /// <param name="IdConcepto">Id del concepto formulado.</param>
        public void DeleteIncidencia(int IdConcepto, bool Calcular)
        {
            ClassIncidencias cins = new ClassIncidencias();
            if (IdConcepto != 0 && Calcular)
            {   
                ModelIncidencias model = new ModelIncidencias();
                cins.DeleteIncidencia(IdConcepto, IdEmpleado, IdPeriodoNomina);
            }
            else             
                cins.DeleteBanderaIncidencia(IdConcepto, IdEmpleado, IdPeriodoNomina);            
        }

        /// <summary>
        /// Obtiene los calculos y crea las incidencias de los conceptos que estan formulados.
        /// </summary>
        /// <param name="datosEmpleados">objeto de vEmpleados para tener disponibles todos los datos del empleado</param>
        /// <param name="TipoConcepto">Tipo de concepto (ER, DD, OTRO)</param>
        private void ConceptosFormulados(vEmpleados datosEmpleados, string TipoConcepto)
        {
            var conceptosFormulaER = conceptosNominaFormula.Where(x => x.TipoConcepto == TipoConcepto && (x.Formula != string.Empty && x.Formula != null)).OrderBy(x => x.Orden ?? 0).ToList();

            if (conceptosFormulaER.Count > 0)
            {
                ExpressionContext context = new ExpressionContext();
                context.Imports.AddType(typeof(Math));

                foreach (var icform in conceptosFormulaER)
                {
                    List<string> lineas = icform.Formula.Replace(" ", "").Replace("\n", "").Replace("\r", "").Split(';').ToList();
                    string Formula = string.Empty;
                    string Omitidos = string.Empty;
                    string Unicamente = string.Empty;
                    string Condicion = string.Empty;
                    List<string> ClavesEmpleadoUnicamente = new List<string>();
                    List<string> ClavesConceptoUnicamente = new List<string>();
                    List<string> ClavesEmpleadoOmitidos = new List<string>();
                    List<string> ClavesConceptoOmitidos = new List<string>();
                    List<string> Condiciones = new List<string>();
                    bool Calcular = true;
                    decimal? valorCalculo = null;

                    GetAccionRealizar(lineas, ref Formula, ref Omitidos, ref Unicamente, ref Condicion);
                    GetCondiciones(Condicion, ref Calcular, ref valorCalculo);
                    GetDatosUnicamente(Unicamente, ClavesEmpleadoUnicamente, ClavesConceptoUnicamente);
                    GetDatosOmitidos(Omitidos, ClavesEmpleadoOmitidos, ClavesConceptoOmitidos);

                    if (ClavesEmpleadoUnicamente.Count() > 0)
                    {
                        Calcular = false;
                        if (ClavesEmpleadoUnicamente.Contains(ClaveEmpleado))
                            Calcular = true;
                    }

                    if (ClavesConceptoUnicamente.Count() > 0)
                    {
                        Calcular = false;
                        if (incidenciasEmpleado.Where(x => ClavesConceptoUnicamente.Contains(x.ClaveConcepto)).Any())
                            Calcular = true;
                    }

                    if (ClavesEmpleadoOmitidos.Contains(ClaveEmpleado))
                        Calcular = false;

                    if (incidenciasEmpleado.Where(x => ClavesConceptoOmitidos.Contains(x.ClaveConcepto)).Count() > 0)
                        Calcular = false;

                    if (icform.CalculoAutomatico == "SI")
                    {
                        DeleteIncidencia(icform.IdConcepto, Calcular);

                        if (Calcular)
                        {
                            Formula = GetFormulaConceptosNominaIncidencias(Formula);
                            Formula = GetFormulaTablaEquivalencias(datosEmpleados, icform, Formula);
                            Formula = Formula.Replace("VALOR_CONDICION", (valorCalculo ?? 0).ToString());

                            IDynamicExpression e = context.CompileDynamic(Formula);
                            var resul = Math.Round(Convert.ToDecimal(e.Evaluate()), 2);


                            if (resul != 0)
                                InsertaIncidenciaConceptoFormulado(icform.IdConcepto, resul, 0, icform.Formula + "|" + Formula);
                        }
                    }
                }

                var incidenciasAuto = GetIncidenciasEmpleadoPagoAutomatico(IdPeriodoNomina, IdEmpleado).Where(x => x.TipoConcepto == TipoConcepto);
                incidenciasEmpleado.AddRange(incidenciasAuto);

                if (TipoConcepto == "ER")
                    nominaTrabajo.ER += incidenciasAuto.Select(x => x.Monto).Sum();

                if (TipoConcepto == "OTRO")
                    nominaTrabajo.ER += incidenciasAuto.Select(x => x.Monto).Sum();

                if (TipoConcepto == "DD")
                    nominaTrabajo.DD += incidenciasAuto.Select(x => x.Monto).Sum();
            }
        }

        private string GetFormulaTablaEquivalencias(vEmpleados datosEmpleados, vConceptos icform, string Formula)
        {
            foreach (var iEquiv in tablaEquivalencias)
            {
                var resultado = "0";
                if (icform.Formula.Contains(iEquiv.Clave))
                {
                    if (tablaEquivalencias != null)
                    {
                        if (iEquiv.Tabla == "Nomina")
                        {
                            List<NominaTrabajo> lnomina = new List<NominaTrabajo>();
                            lnomina.Add(nominaTrabajo);

                            resultado = lnomina.AsQueryable().Select(iEquiv.Campo).Sum().ToString();
                        }

                        if (iEquiv.Tabla == "Empleados")
                        {
                            List<vEmpleados> lemp = new List<vEmpleados>();
                            lemp.Add(datosEmpleados);

                            resultado = lemp.AsQueryable().Select(iEquiv.Campo).Sum().ToString();
                        }

                        if (iEquiv.Tabla == "FactorIntegracion")
                        {
                            if (iEquiv.Clave == "SBC")
                            {
                                decimal factor = (prestaciones.Where(x => nominaTrabajo.Anios >= x.Limite_Inferior && nominaTrabajo.Anios <= x.Limite_Superior)
                                    .Select(x => x.FactorIntegracion).FirstOrDefault() ?? 1.0493M);

                                resultado = (SD_IMSS * factor).ToString();
                            }
                        }
                    }

                    Formula = Formula.Replace(iEquiv.Clave, resultado);
                }
            }

            return Formula;
        }

        private string GetFormulaConceptosNominaIncidencias(string Formula)
        {
            foreach (var lc in conceptosNominaFormula)
            {
                decimal monto = 0;

                var claveConcepto = "\"" + lc.ClaveConcepto.Trim().ToUpper() + "\"";
                if (Formula.Contains(claveConcepto))
                {

                    //Se agrega una condiciones para calcular monto
                    if (lc.TipoDato == "Cantidades")
                        monto = incidenciasEmpleado.Where(x => x.ClaveConcepto == lc.ClaveConcepto).Select(x => x.Cantidad).Sum() ?? 0;
                    else
                        monto = incidenciasEmpleado.Where(x => x.ClaveConcepto == lc.ClaveConcepto).Select(x => x.Monto).Sum() ?? 0;

                    Formula = Formula.Replace(claveConcepto, monto.ToString());
                }
            }

            return Formula;
        }

        private void GetCondiciones(string Condicion, ref bool Calcular, ref decimal? valorCalculo)
        {
            if (Condicion != string.Empty)
            {
                List<string> condiciones = new List<string>();

                try { condiciones = Condicion.ToUpper().Trim().Split(',').ToList(); } catch { }
                if (condiciones.Count() > 0)
                {
                    Calcular = false;
                    foreach (var cond in condiciones)
                    {
                        if (cond != null && cond != string.Empty)
                        {
                            //la cadena se puede componer por 6 campos -> TABLA|CAMPO DE LA TABLA|TIPO DE CAMPO RESULTADO(NUMERICO, TEXTO)|VALOR DEL REGISTRO(val1=x&val2=y)|VALOR SE TOMA DE OTRO CAMPO DE LA TABLA
                            string[] datosCond = cond.Trim().Split('|');
                            string tabla = string.Empty;
                            string campo = string.Empty;
                            string TipoValor = string.Empty;
                            string valor = string.Empty;
                            string valorCampo = string.Empty;
                            string[] valores = new string[0];
                            dynamic resultado = null;
                            Dictionary<string, string> dvalores = new Dictionary<string, string>();

                            try { tabla = datosCond[0]; } catch { }
                            try { campo = datosCond[1]; } catch { }
                            try { TipoValor = datosCond[2]; } catch { }
                            try { valor = datosCond[3]; } catch { }
                            try { valorCampo = datosCond[4]; } catch { }

                            if (valor != string.Empty)
                            {
                                valores = valor.Trim().Split('&');

                                foreach (var v in valores)
                                {
                                    if (v != string.Empty)
                                    {
                                        string[] vdatos = v.Trim().Split('=');
                                        string _key = null;
                                        string _value = null;
                                        try { _key = vdatos[0]; } catch { }
                                        try { _value = vdatos[1]; } catch { }
                                        dvalores.Add(_key, _value);
                                    }
                                }
                            }

                            if (tabla != string.Empty && campo != string.Empty)
                            {
                                resultado = GetConsultaCondicion(tabla, campo, TipoValor);
                            }

                            if (TipoValor == "NUMERICO")
                            {
                                int resul = (int)resultado;

                                if (dvalores.Select(x => int.Parse(x.Key)).ToList().Contains(resul))
                                {
                                    var _valor = dvalores.Where(x => int.Parse(x.Key) == resul).Select(x => x.Value).FirstOrDefault();

                                    if (_valor != null)
                                    {
                                        try { valorCalculo = decimal.Parse(_valor); } catch { }
                                    }

                                    if (valorCampo != string.Empty)
                                    {
                                        try { valorCalculo = (decimal)GetConsultaCondicion(tabla, valorCampo, TipoValor); } catch { }
                                    }

                                    Calcular = true;   
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void GetDatosOmitidos(string Omitidos, List<string> ClavesEmpleadoOmitidos, List<string> ClavesConceptoOmitidos)
        {
            if (Omitidos != string.Empty)
            {
                List<string> claves = new List<string>();

                try { claves = Omitidos.ToUpper().Trim().Split(',').ToList(); } catch { }
                if (claves.Count() > 0)
                {
                    foreach (var clave in claves.Where(x => x.Length > 3))
                    {
                        if (clave.Substring(0, 3) == "EMP")
                        {
                            string nuevaClave = clave.Trim().Replace("EMP", "");
                            ClavesEmpleadoOmitidos.Add(nuevaClave);
                        }

                        if (clave.Substring(0, 3) == "CON")
                        {
                            string nuevaClave = clave.Trim().Replace("CON", "");
                            ClavesConceptoOmitidos.Add(nuevaClave);
                        }
                    }
                }
            }
        }

        private static void GetDatosUnicamente(string Unicamente, List<string> ClavesEmpleadoUnicamente, List<string> ClavesConceptoUnicamente)
        {
            if (Unicamente != string.Empty)
            {
                List<string> claves = new List<string>();

                try { claves = Unicamente.ToUpper().Trim().Split(',').ToList(); } catch { }
                if (claves.Count() > 0)
                {
                    foreach (var clave in claves.Where(x => x.Length > 3))
                    {
                        if (clave.Substring(0, 3) == "EMP")
                        {
                            string nuevaClave = clave.Trim().Replace("EMP", "");
                            ClavesEmpleadoUnicamente.Add(nuevaClave);
                        }

                        if (clave.Substring(0, 3) == "CON")
                        {
                            string nuevaClave = clave.Trim().Replace("CON", "");
                            ClavesConceptoUnicamente.Add(nuevaClave);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Separa la cedena de la formula para saber que acción se va a realizar
        /// </summary>
        /// <param name="lineas"></param>
        /// <param name="Formula"></param>
        /// <param name="Omitidos"></param>
        /// <param name="Unicamente"></param>
        /// <param name="Condicion"></param>
        private static void GetAccionRealizar(List<string> lineas, ref string Formula, ref string Omitidos, ref string Unicamente, ref string Condicion)
        {
            foreach (var line in lineas)
            {
                List<string> datos = line.Replace(" ", "").Replace("\n", "").Replace("\r", "").Split(':').ToList();
                string accion = datos[0].ToUpper();

                if (accion == "CALCULO")
                    try { Formula = datos[1]; } catch { }

                if (accion == "EXCEPTO")
                    try { Omitidos = datos[1]; } catch { }

                if (accion == "SOLO")
                    try { Unicamente = datos[1]; } catch { }

                if (accion == "CONDICION")
                    try { Condicion = datos[1]; } catch { }
            }
        }

        /// <summary>
        /// Obtiene la consulta para la condición capturada en la formula
        /// </summary>
        /// <param name="tabla">tabla a la que se hara la consulta</param>
        /// <param name="campo">campo contra el que se hace la validación</param>
        /// <returns></returns>
        private dynamic GetConsultaCondicion(string tabla, string campo, string tipoDato)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                string consulta = "select " + campo + " from " + tabla + " where IdEmpleado = " + IdEmpleado;
                
                if (tipoDato == "NUMERICO")
                {
                    decimal? _result = 0;
                    try { _result = entidad.Database.SqlQuery<decimal?>(consulta).FirstOrDefault(); }
                    catch { _result = entidad.Database.SqlQuery<int?>(consulta).FirstOrDefault(); }
                    dynamic result = _result;
                    return result;
                }

                if (tipoDato == "TEXTO")
                {
                    var resutl = entidad.Database.SqlQuery<string>(consulta).FirstOrDefault();
                    dynamic result = resutl;
                    return result;
                }

                return null;
            }
        }

        public decimal ObtenAntiguedadEmpleado(DateTime? FechaIngreso, DateTime FechaBaja)
        {
            try
            {
                decimal Antiguedad = 0;
                if (FechaIngreso != null)
                {
                    Decimal Dias = (FechaBaja.Subtract((DateTime)FechaIngreso).Days) + 1;
                    Antiguedad = (Dias / 365);
                }
                return Antiguedad;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Metodo para calcular los ISR de los sueldos de los empleados para los tipos de nómina donde se piramida. 
        /// </summary>
        /// <param name="datosEmpleados">Objeto de tipo vEMpleados para tener toda la información del enmpleado.</param>
        private void PiramidaSueldosConCompensacion(vEmpleados datosEmpleados)
        {
            if ((UnidadNegocio.ConfiguracionSueldos == "Netos Tradicional(Piramida)" || UnidadNegocio.ConfiguracionSueldos == "Netos Tradicional(Piramida ART 93)") && datosEmpleados.NetoPagar > 0)
            {
                decimal imss = 0;
                imss = getIMSS();

                if (UnidadNegocio.ConfiguracionSueldos == "Netos Tradicional(Piramida ART 93)")
                {
                    CalculaISR();
                    decimal importeConcepto = 0;
                    importeConcepto = getImporteConcepto(datosEmpleados, imss);

                    if (importeConcepto == 0)
                    {
                        nominaTrabajo.SueldoPagado -= 10;
                        nominaTrabajo.ER -= 10;
                        importeConcepto += 10;
                    }
                    if (importeConcepto < 0)
                    {
                        decimal cantFaltas = Math.Abs(importeConcepto) / (datosEmpleados.SDIMSS ?? 0);
                        decimal faltasInsertar = Math.Ceiling(cantFaltas);

                        InsertaIncidenciaConceptoCompensacionFaltas(conceptosConfigurados.IdConceptoFaltas ?? 0, faltasInsertar);

                        nominaTrabajo.DiasTrabajados -= faltasInsertar;
                        nominaTrabajo.DiasTrabajadosIMSS -= faltasInsertar;
                        nominaTrabajo.SueldoPagado -= faltasInsertar * datosEmpleados.SDIMSS;
                        nominaTrabajo.ER -= faltasInsertar * datosEmpleados.SDIMSS;
                        nominaTrabajo.Faltas += faltasInsertar;
                        DiasTrabajados_IMSS -= faltasInsertar;
                        Dias_Faltados += faltasInsertar;

                        Calcula_Cuotas_Obreras(nominaTrabajo.DiasTrabajadosIMSS);
                        CalculaISR();
                        imss = getIMSS();

                        importeConcepto = getImporteConcepto(datosEmpleados, imss);
                    }

                    InsertaIncidenciaConceptoCompensacionPiramidar(conceptosConfigurados.IdConceptoArt93Fraclll ?? 0, importeConcepto);
                    nominaTrabajo.ER += importeConcepto;
                }
                else
                {
                    decimal importeAPiramidar = 0;
                    importeAPiramidar += datosEmpleados.NetoPagar ?? 0;
                    importeAPiramidar += imss;
                    importeAPiramidar -= incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll).Select(X => X.Exento).Sum() ?? 0;
                    importeAPiramidar -= incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "OTRO" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll).Select(X => X.Monto).Sum() ?? 0;
                    importeAPiramidar += incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "DD" && x.MultiplicaDT != "SI" && x.ClaveSAT != "001").Select(X => X.Monto).Sum() ?? 0;
                    decimal montoBruto = Piramida(importeAPiramidar, Periodo.FechaFin);

                    decimal incidenciasPercepcion = 0;
                    incidenciasPercepcion = incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll).Select(X => X.Gravado).Sum() ?? 0;
                    incidenciasPercepcion += incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "OTRO" && x.MultiplicaDT != "SI" && x.IdConcepto != conceptosConfigurados.IdConceptoCompensacion && x.IdConcepto != conceptosConfigurados.IdConceptoArt93Fraclll).Select(X => X.Gravado).Sum() ?? 0;
                    decimal importeConcepto = montoBruto - ((nominaTrabajo.SueldoPagado ?? 0) + incidenciasPercepcion);
                    InsertaIncidenciaConceptoCompensacionPiramidar(conceptosConfigurados.IdConceptoCompensacion ?? 0, importeConcepto);
                    nominaTrabajo.ER += importeConcepto;
                    percepcionesEspecialesGravado = 0;
                    percepcionesEspecialesGravado += importeConcepto;

                    CalculaISR();
                }
            }
        }

        private decimal getIMSS()
        {
            decimal imss = 0;
            imss += nominaTrabajo.IMSS_Obrero ?? 0;
            imss += incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "DD" && x.ClaveSAT == "001").Select(X => X.Monto).Sum() ?? 0;
            return imss;
        }

        /// <summary>
        /// Inserta las incidencias de los conceptos formulados.
        /// </summary>
        /// <param name="IdConcepto"></param>
        /// <param name="Monto"></param>
        /// <param name="Formula"></param>
        public void InsertaIncidenciaConceptoFormulado(int IdConcepto, decimal Monto, decimal MontoEsquema, string Formula)
        {
            if (IdConcepto != 0)
            {
                ClassIncidencias cins = new ClassIncidencias();
                ModelIncidencias model = new ModelIncidencias();                

                model.IdEmpleado = IdEmpleado;
                model.IdPeriodoNomina = IdPeriodoNomina;
                model.IdConcepto = (int)IdConcepto;
                model.Monto = Monto;
                model.Observaciones = "PDUP SYSTEM Concepto creado por el sistema para los conceptos formulados";
                model.MontoEsquema = 0;
                model.BanderaConceptoEspecial = 1;
                model.FormulaEjecutada = Formula;

                if (model.IdConcepto != 0)
                    cins.NewIncindencia(model, IdUsuario);
            }
        }

        public decimal obtenHorasJornadaLaboral(int idjornada)
        {
            decimal idj = 0;
            try
            {
                using (TadaNominaEntities db = new TadaNominaEntities())
                {
                    var jornada = (from b in db.Cat_Jornadas
                                   where b.IdEstatus == 1 && b.IdJornada == idjornada
                                   select b).FirstOrDefault();

                    if (jornada != null)
                    {
                        idj = (decimal)jornada.Horas;

                    }

                }
                return idj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
