using DocumentFormat.OpenXml.Office.CustomUI;
using System;
using System.Collections.Generic;
using System.Linq;
using TadaNomina.Models.ClassCore.CalculoAguinaldo;
using TadaNomina.Models.DB;
using TadaNomina.Services;

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
            GetConceptosConfigurados();

            int[] EstatusNomina = { 1, 3 };
            var empleadosProceso = listEmpleados;
            if (Periodo.TipoNomina != "Complemento")
                empleadosProceso = listEmpleados.Where(x=>EstatusNomina.Contains(x.IdEstatus)).ToList();
            
            if (Periodo.TipoNomina == "PTU")
            {
                empleadosProceso = listEmpleados;
            }

            if (_IdEmpledao != null)
                empleadosProceso = empleadosProceso.Where(x=>x.IdEmpleado == _IdEmpledao).ToList();

            //calculo de septimo día, se pone en esta pocición para que actue solo sobre los empleados que se van a procesar
            if (UnidadNegocio.SeptimoDia == "S" && UnidadNegocio.IdConceptoSeptimoDia != null)
                ProcesaIncidenciasSeptimoDia(empleadosProceso, _IdPeriodoNomina, (int)UnidadNegocio.IdConceptoSeptimoDia, IdUsuario);
            
            GetIncidencias(_IdPeriodoNomina);

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
                        ProcesaPension(pensionAlimenticia.Where(x => x.IdEmpleado == IdEmpleado).ToList(), IdPeriodoNomina, (decimal)(nominaTrabajo.ER - nominaTrabajo.ImpuestoRetener - nominaTrabajo.IMSS_Obrero), (decimal)nominaTrabajo.ERS, IdUsuario);
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
                    ProcesaProvision(nominaTrabajo.DiasTrabajados, IdPrestacionesEmpleado, item.FechaReconocimientoAntiguedad, item.FechaAltaIMSS, (decimal)UnidadNegocio.FactorDiasProvision);
                }                                

                contador++;
            }
        }

        /// <summary>
        /// Metodo que calcula la parte tradicional del calculo de nómina.
        /// </summary>
        /// <param name="datosEmpleados">Objeto con toda la información del empleado en proceso.</param>
        private void Porcesa_Nomina_Tradicional(vEmpleados datosEmpleados)
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

            nominaTrabajo.FactorIntegracion = prestaciones.Where(x => x.IdPrestaciones == datosEmpleados.IdPrestaciones).Select(x => x.FactorIntegracion).FirstOrDefault() ?? 0;
            if(nominaTrabajo.FactorIntegracion == null)
                nominaTrabajo.FactorIntegracion = prestaciones.Where(x => x.IdCliente == 0).Select(x => x.FactorIntegracion).FirstOrDefault();

            nominaTrabajo.FechaAltaIMSS= datosEmpleados.FechaAltaIMSS;
            nominaTrabajo.FechaReconocimientoAntiguedad= datosEmpleados.FechaReconocimientoAntiguedad;

            GetDiasTrabajados(_tipoEsquemaT);
            
            nominaTrabajo.SueldoPagado = 0;
            nominaTrabajo.SueldoPagado += nominaTrabajo.DiasTrabajados * SD_IMSS;
            nominaTrabajo.Sueldo_Vacaciones = nominaTrabajo.Dias_Vacaciones * SD_IMSS;
            if (configuracionNominaEmpleado.SupenderSueldoTradicional == 1) { nominaTrabajo.SueldoPagado = 0; }
            
            nominaTrabajo.ER = (decimal)nominaTrabajo.SueldoPagado;
            nominaTrabajo.ER += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.MultiplicaDT != "SI").Select(X => X.Monto).Sum();

            if (UnidadNegocio.SeptimoDia == "S" && UnidadNegocio.IdConceptoSeptimoDia != null && UnidadNegocio.IdConceptoSeptimoDia > 0)
                nominaTrabajo.DiasTrabajados += incidenciasEmpleado.Where(x=> x.IdConcepto == (int)UnidadNegocio.IdConceptoSeptimoDia).Select(x=>x.Cantidad).Sum();

            if ((UnidadNegocio.PercepcionesEspeciales == "S" && datosEmpleados.IdEstatus == 1) || (configuracionNominaEmpleado.IncidenciasAutomaticas == 1))
            {
                if (configuracionNominaEmpleado.SupenderSueldoTradicional == null)
                {
                    GetPercepciones_pp(datosEmpleados.IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, datosEmpleados.IdPuesto, datosEmpleados.Compensacion_Dia_Trabajado, nominaTrabajo.DiasTrabajados, nominaTrabajo.Faltas, SD_IMSS, IdUsuario, datosEmpleados.IdCliente, datosEmpleados.IdCentroCostos);
                    nominaTrabajo.ER += (percepcionesEspecialesGravado + percepcionesEspecialesExcento);
                }
            }

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
            else if (Periodo.TipoNomina == "Aguinaldo" && UnidadNegocio.ISRAguinaldoL174 == "S")
            {
                CalculaISRAguinaldoL174();
            }
            else
            {
                CalculaISR();
            }

            if (ValidacionDiasEquivalentes())
            {
                List<int> mesesvariables = new List<int>() { 1, 3, 5, 7, 9, 11 };
                var mesfin = Periodo.FechaFin.Month;
                if (mesesvariables.Contains(mesfin))
                {
                    Calcula_Cuotas_ObrerasEquivalentes();
                }
                else
                {
                    Calcula_Cuotas_Obreras();
                }
            }
            else
            {
                Calcula_Cuotas_Obreras();
            }

            if (Periodo.DescuentosFijos == "SI" && Periodo.TipoNomina=="Nomina")
            {
                if (datosEmpleados.IdEstatus == 1 || configuracionNominaEmpleado.IncidenciasAutomaticas == 1)
                {
                    ProcesaCredito(creditosInfonavit.Where(x => x.IdEmpleado == IdEmpleado).FirstOrDefault(), IdPeriodoNomina, (decimal)SueldosMinimos.UnidadMixta, DiasTrabajados_IMSS, IdUsuario, datosEmpleados.TipoNomina);
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

            if ((UnidadNegocio.DeduccionesEspeciales == "S" && datosEmpleados.IdEstatus == 1) || (configuracionNominaEmpleado.IncidenciasAutomaticas == 1))
            {
                if (configuracionNominaEmpleado.SupenderSueldoTradicional == null)
                    nominaTrabajo.DD += GetDeduccionesEspeciales(datosEmpleados.IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, datosEmpleados.IdPuesto, datosEmpleados.Compensacion_Dia_Trabajado, nominaTrabajo.DiasTrabajados, nominaTrabajo.Faltas, SD_IMSS, IdUsuario, datosEmpleados.IdCliente, datosEmpleados.IdCentroCostos);
            }

            if (nominaTrabajo.ER <= 0)            
                nominaTrabajo.DD = 0;
                //EliminaMontosIncidenciasAusentismos();            

            nominaTrabajo.Neto = 0;
            nominaTrabajo.Neto = nominaTrabajo.ER - nominaTrabajo.DD;                       
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
                        else if (UnidadNegocio.ConfiguracionSueldos == "Netos(Real)")
                        {
                            nominaTrabajo.Apoyo += nominaTrabajo.SueldoPagado_Real - nominaTrabajo.SueldoPagado;
                            nominaTrabajo.Apoyo += nominaTrabajo.IMSS_Obrero;
                            nominaTrabajo.Apoyo += nominaTrabajo.ImpuestoRetener;
                            nominaTrabajo.Apoyo -= nominaTrabajo.SubsidioPagar;
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
            nominaTrabajo.SueldoPagado_Real = 0;
            nominaTrabajo.Total_ER_Real = 0;
            nominaTrabajo.Total_DD_Real = 0;

            if (UnidadNegocio.ConfiguracionSueldos == "Netos(Real)")
            {
                nominaTrabajo.SueldoPagado_Real = 0;
                nominaTrabajo.SueldoPagado_Real += SD_Real * nominaTrabajo.DiasTrabajados;

                nominaTrabajo.Total_ER_Real = 0;
                nominaTrabajo.Total_ER_Real += SD_Real * nominaTrabajo.SueldoPagado_Real;
                nominaTrabajo.Total_ER_Real += (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "ER").Select(X => X.Monto + X.MontoEsquema).Sum();                

                nominaTrabajo.Total_DD_Real = 0;
                nominaTrabajo.Total_DD_Real += (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "DD").Select(X => X.Monto + X.MontoEsquema).Sum();
                
                nominaTrabajo.Neto_Real = 0;
                nominaTrabajo.Neto_Real += nominaTrabajo.Total_ER_Real - nominaTrabajo.Total_ER_Real;
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
    }
}