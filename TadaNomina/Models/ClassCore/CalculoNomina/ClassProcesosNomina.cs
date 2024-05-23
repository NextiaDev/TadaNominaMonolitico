using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.CalculoNomina
{
    /// <summary>
    /// Procesos de Nómina
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 16/05/2022, Razon: Documentar el codigo.
    /// </summary>
    public class ClassProcesosNomina
    {
        #region propiedades y variables que se ocuapan en el proceso de nómina.
        internal Cat_UnidadNegocio UnidadNegocio;
        internal PeriodoNomina Periodo;
        internal List<vEmpleados> listEmpleados;
        internal List<vEmpleados> listEmpleadosSinAjuste = new List<vEmpleados>();
        internal List<Cat_EntidadFederativa> listEntidades;
        internal List<vPrestacionesFactor> prestaciones;
        internal List<ImpuestoSat> ListImpuestos;
        internal List<ImpuestoSat> ListImpuestos_Ajuste;
        internal List<ImpuestoSat> ListImpuestos_AjusteSecundario;
        internal List<ImpuestoSat> ListImpuestos_Auxiliar;
        internal List<ImpuestoSat> ListImpuestos_Anterior;
        internal List<sp_EmpleadosAjusteAnual_Result> ListaEmpleadosAjusteAnual;
        internal List<SubsidioEmpleoSat> ListSubsidio;
        internal List<SubsidioEmpleoSat> ListSubsidio_Ajuste;
        internal List<SubsidioEmpleoSat> ListSubsidio_Auxiliar;
        internal List<SubsidioEmpleoSat> ListSubsidio_Anterior;
        internal List<ImpuestosIMSS> ListImpuestosIMSS;
        internal List<FactoresCyV_IMSS> ListFactoresCyV_IMSS;
        internal Sueldos_Minimos SueldosMinimos;
        internal Cat_TipoNomina TipoNomina;
        internal NominaTrabajo nominaTrabajo;
        internal List<vIncidencias> listIncidencias;
        internal List<vIncidencias> incidenciasEmpleado;
        internal List<vCreditoInfonavit> creditosInfonavit;
        internal List<vCreditoFonacot> creditosFonacot;
        internal List<vPensionAlimenticia> pensionAlimenticia;
        internal List<vSaldos> saldos;        
        internal List<Cat_RegistroPatronal> ListRegistroPatronal;
        internal List<vNomina> ListNominaAjuste;
        internal List<vNomina> ListNominaAjusteSecundario;
        internal List<vNomina> ListNominaAjusteAnterior;
        internal List<vConfiguracionFiniquito> ListConfiguracionFiniquito;
        internal vConfiguracionFiniquito configuracionFiniquito;
        internal vConfiguracionConceptosFiniquitos conceptosFiniquitos;
        internal vConfiguracionConceptosFiniquitos conceptosConfigurados;
        internal List<Faltas> Lista_Total_Faltas;
        internal List<ConfiguracionNominaPeriodoEmpleado> lconfiguracionNominaEmpleado;
        internal ConfiguracionNominaPeriodoEmpleado configuracionNominaEmpleado;
        internal List<sp_RegresaAusentismos_Result> ausentismos;
        internal List<vConceptosPiramidados> conceptosPiramidados;
        internal List<vConceptosPiramidados> conceptosPiramidadosEmpleado;
        internal string[] _tipoEsquemaT = { "Tradicional", "Mixto" };
        internal string[] _tipoEsquemaS = { "Esquema", "Mixto" };
        internal string[] _tipoEsquemaR = { "Tradicional", "Esquema", "Mixto" };
        internal ConfiguracionFechasCalculos configuracionFechas;
        internal List<ModelDiasTrabajadosAguinaldo> ListDiasTrabajadosAguinaldo;
        internal List<vConceptos> conceptosNominaFormula;
        internal List<FormulasEquivalencias> tablaEquivalencias;
        internal bool AjusteSecundario = false;
        internal bool AjusteAnual = false;
        internal int contador;
        internal int IdUsuario;
        internal int IdPeriodoNomina;
        internal int IdEmpleado;
        internal string ClaveEmpleado;
        internal string RFC;
        internal int? IdEntidadFederativa;
        internal int IdEstatus;
        internal string EsquemaPago;
        internal decimal DiasPago;
        internal decimal DiasPagoCalculoSubsidio;
        internal decimal DiasMenosPorAlta;
        internal decimal DiasMasPorAlta;
        internal decimal CreditoSalario;
        internal decimal Dias_Faltados;
        internal decimal Dias_Faltados_IMSS;
        internal decimal Incapacidades;
        internal decimal DiasTrabajados_IMSS;
        internal decimal SD_IMSS;
        internal decimal SD_Esquema;
        internal decimal SD_Real;
        internal decimal SDI;
        internal decimal UMA;
        internal decimal Sueldo_Minimo;
        internal decimal Porcentaje_Riesgo_Trabajo_Patronal;
        internal decimal Antiguedad;
        internal decimal AntiguedadTrad;
        internal decimal AniosEsquema;
        internal decimal AntiguedadGeneradaUltimoAño;
        internal decimal AntiguedadGeneradaUltimoAñoEsquema;
        internal decimal DiasTrabajadosEjercicio;
        internal decimal DiasTrabajadosEjercicioTrad;
        internal decimal FactorDiasTrabajadosEjercicio;
        internal decimal DiasVacacionesFactorIntegracion;
        internal decimal DiasVacacionesFactorIntegracionEsquema;
        internal decimal DiasVacacionesEsquema;
        internal decimal PorcentajePVFactorIntegracion;
        internal decimal PorcentajePVFactorIntegracionEsquema;
        internal decimal DiasAguinaldoFactorIntegracion;
        internal decimal DiasAguinaldoFactorIntegracionEsquema;
        internal decimal? percepcionesEspecialesExcento;
        internal decimal? percepcionesEspecialesGravado;
        internal decimal? percepcionesEspecialesEsquema;        
        internal decimal? baseMostrar;        
        internal decimal? ISR_Mensual_Ajuste;        
        internal decimal? ISR_Mensual_Ajuste_Anterior;        
        internal decimal? valorTopeMensualSubsidio;        
        internal decimal? SubsidioAnterioAjuster;        
        internal int? IdPrestacionesEmpleado;
        internal DateTime _fechaBaja;
        internal DateTime _fechaReconocimientoAntiguedad;
        internal DateTime _fechaAltaSS;
        internal DateTime _fechaReconocimientoAntiguedadEsquema;
        internal DateTime _FechaInicioEjercicio;
        internal DateTime _FechaInicioEjercicioTrad;
        internal decimal fraccionHorasMas;
        internal decimal fraccionHorasMenos;

        internal decimal montoPension;
        internal decimal montoPensionEsq;
        internal decimal montoCreditoInfonavit;
        internal decimal montoCreditoInfonavitEsq;
        internal decimal montoCreditoFonacot;
        internal decimal montoIncidenciasMultiplicaDT;
        internal decimal montoIncidenciasMultiplicaDTGrabado;
        internal decimal numDiasTablasDiarias = 1M;

        //datos para integracion de sueldo
        internal string[] clves = new string[0];
        internal int[] periodoInt = new int[0];
        internal decimal? ImporteParaSumarAlSDI;
        internal decimal? factorIntegraciosSDILiquidacion;
        internal int diasPeriodo;
        #endregion

        /// <summary>
        /// Metodo que carga desde la base de datos a memoria todas las listas necesarias para el calculo de la nómina.
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo que se va a procesar</param>
        /// <param name="IdEmpleado">Paramentro opcional, para saber cuando el sistema solo va a procesar a cierto empleado</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema que esta realizando el proceso.</param>
        public void GetListas(int IdPeriodo, int? IdEmpleado, int IdUsuario)
        {
            try
            {
                GetPeriodoNomina(IdPeriodo);
                GetSueldosMinimos(Periodo.FechaFin);
                ProcesaIncidenciasProgramadas(Periodo.DescuentosFijos, Periodo.IdUnidadNegocio, IdPeriodo, IdEmpleado, IdUsuario);
                GetDatosUnidadNegocio(Periodo.IdUnidadNegocio);
                GetConceptosConfigurados();

                GetListEmpleados(UnidadNegocio.IdUnidadNegocio);
                GetListConceptosNominaFormula(UnidadNegocio.IdCliente);
                GetTablaEquivalencias(UnidadNegocio.IdCliente);
                listEmpleadosSinAjuste = new List<vEmpleados>();

                if (Periodo.TipoNomina == "Nomina")
                {
                    ProcesaVacaciones(Periodo.IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, IdUsuario);
                    GetAusentismos(IdPeriodoNomina);

                    if (IdEmpleado != null)
                        ProcesaAusentismos(IdPeriodoNomina, (int)IdEmpleado, IdUsuario);
                    else
                        ProcesaAusentismos(IdPeriodoNomina, IdUsuario);

                    if (IdEmpleado != null)
                        ProcesoCompensacionesPagos(IdPeriodoNomina, (int)IdEmpleado, IdUsuario);
                    else
                        ProcesoCompensacionesPagos(IdPeriodoNomina, IdUsuario);
                }

                if (Periodo.TipoNomina == "Complemento")
                {
                    if (IdEmpleado != null)
                        ProcesoCompensaciones(IdPeriodoNomina, (int)IdEmpleado, IdUsuario);
                    else
                        ProcesoCompensaciones(IdPeriodoNomina, IdUsuario);
                }

                getPiramidados(IdPeriodo, IdEmpleado, IdUsuario);

                if (UnidadNegocio.ConceptosSDILiquidacion != null && UnidadNegocio.ConceptosSDILiquidacion != string.Empty && Periodo.PeriodosIntegracionSDI != string.Empty && Periodo.PeriodosIntegracionSDI != null)
                {
                    string[] periodo = new string[0];
                    if (Periodo.PeriodosIntegracionSDI != null)
                        periodo = Periodo.PeriodosIntegracionSDI.Split(',').Where(x => x != string.Empty).ToArray();

                    periodoInt = Array.ConvertAll(periodo, int.Parse);

                    if (UnidadNegocio.ConceptosSDILiquidacion != null)
                        clves = UnidadNegocio.ConceptosSDILiquidacion.Split(',').Where(x => x != string.Empty).ToArray();
                    //getImporteIntegraSDI(clves, periodoInt);
                }

                GetTipoNomina(UnidadNegocio.IdTipoNomina);
                GetPrestaciones(UnidadNegocio.IdCliente, Periodo.FechaFin);

                if (UnidadNegocio.FiniquitosTablaMensual == "S" && Periodo.TipoNomina == "Finiquitos")
                {
                    GetImpuestosSAT(4, Periodo.FechaFin);
                    GetSubsidioSAT(4, Periodo.FechaFin);
                }
                else
                {
                    if (Periodo.TablaDiaria == "S" || UnidadNegocio.ISRProyeccionMensual == "S")
                    {
                        GetImpuestosSAT(4, Periodo.FechaFin);
                        GetSubsidioSAT(4, Periodo.FechaFin);
                    }
                    else
                    {
                        GetImpuestosSAT(UnidadNegocio.IdTipoNomina, Periodo.FechaFin);
                        GetSubsidioSAT(UnidadNegocio.IdTipoNomina, Periodo.FechaFin);
                    }
                }

                GetRegistrosPatronales(UnidadNegocio.IdCliente);
                GetImpuestosIMSS();
                GetFactoresCyVIMSS(Periodo.FechaFin);
                GetCreditos(UnidadNegocio.IdUnidadNegocio);
                GetPensiones(UnidadNegocio.IdUnidadNegocio);
                GetCreditosFonacot(UnidadNegocio.IdUnidadNegocio);
                getSaldos(UnidadNegocio.IdUnidadNegocio, Periodo.FechaInicio, Periodo.FechaFin);
                GetConfiguracionNominaPeriodoEmpleado();
                GetListEntidades();

                if (Periodo.AjusteDeImpuestos == "SI")
                {
                    if (Periodo.AjusteAnual == "S")
                    {
                        GetImpuestosSAT_Ajuste_Anual(Periodo.FechaFin);
                        GetEmpleadosAjusteAnual(UnidadNegocio.IdUnidadNegocio, Periodo.FechaFin.Year);

                        var cvesEmpSinAjuste = Periodo.EmpleadosSinAjuste != null && Periodo.EmpleadosSinAjuste != "" ?
                            Periodo.EmpleadosSinAjuste.Replace(" ", "").Split(',').ToList() : new List<string>();
                        GetListEmpleadosSinAjuste(Periodo.IdUnidadNegocio, cvesEmpSinAjuste);

                        if (Periodo.PeriodosAjusteSecundario != null && Periodo.PeriodosAjusteSecundario.Length > 0 && listEmpleadosSinAjuste.Count > 0)
                        {
                            AjusteSecundario = true;
                            GetNominaAjusteSecundario(Periodo.PeriodosAjusteSecundario);
                            GetImpuestosSAT_AjusteSecundario(Periodo.FechaFin);
                        }
                    }
                    else
                        GetImpuestosSAT_Ajuste(Periodo.FechaFin);

                    GetSubsidioSAT_Ajuste(Periodo.FechaFin);
                    GetNominaAjuste(Periodo.SeAjustaraConPeriodo);

                    if (ListaEmpleadosAjusteAnual != null)
                        ListNominaAjuste = ListNominaAjuste.Where(x => ListaEmpleadosAjusteAnual.Select(y => y.Rfc).ToList().Contains(x.Rfc)).ToList();
                }

                // Para el cao de una nomina de PTU cargamos tabla mensual para todos los calculos
                if (Periodo.TipoNomina == "PTU")
                {
                    GetImpuestosSAT(4, Periodo.FechaFin);
                    GetSubsidioSAT(4, Periodo.FechaFin);
                    GetListEmpleadosPTU(Periodo.IdPeriodoNomina);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al llenar listas. " + ex.Message);
            }
        }

        private void GetListConceptosNominaFormula(int idCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                conceptosNominaFormula = entidad.vConceptos.Where(x => x.IdCliente == idCliente && x.IdEstatus == 1 ).ToList();
            }
        }

        private void GetTablaEquivalencias(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                tablaEquivalencias = entidad.FormulasEquivalencias.Where(x => x.IdEstatus == 1 && x.IdCliente == IdCliente).ToList();
            }
        }

        public void getSaldos(int IdUnidadNegocio, DateTime FechaInicio, DateTime FechaFin)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                saldos = entidad.vSaldos.Where(x => x.IdUnidadNegocio == IdUnidadNegocio &&
                    (x.IdEstatus == 1 && FechaInicio >= x.FechaInicial && (FechaFin <= x.FechaFinal || x.Indefinido == 1) && x.Tipo == "Periodo de Tiempo") ||
                    (x.Tipo == "Saldos" && x.IdEstatus == 1)).ToList();
            }
        }

        public void getImporteIntegraSDI(string[] ClaveConcepto, int[] Periodos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {                
                ImporteParaSumarAlSDI = entidad.vIncidencias_Consolidadas
                    .Where(x => x.IdEstatus == 1 && ClaveConcepto.Contains(x.ClaveConcepto) && Periodos
                    .Contains((int)x.IdPeriodoNomina) && x.IdEmpleado == IdEmpleado).Select(x=> x.Monto).Sum() ?? 0;               
            }
        }

        public void getDiasPeriodos(int[] Periodos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var nomina = entidad.Nomina.Where(x => Periodos.Contains((int)x.IdPeriodoNomina) && x.IdEmpleado == IdEmpleado).Select(x=> x.DiasTrabajados + x.Dias_Vacaciones).Sum();

                if (nomina != null)
                    diasPeriodo = (int)nomina;
            }
        }

        public void getPiramidados(int IdPeriodo, int? IdEmpleado, int IdUsuario)
        {
            ClassPiramidados cp = new ClassPiramidados();
            conceptosPiramidados = cp.ProcesaPiramidados(IdPeriodo, IdEmpleado, IdUsuario);
        }

        public void ProcesaAusentismos(int IdPeriodoNomina, int IdEmpleado, int IdUsuario)
        {
            Ausen au = new Ausen();
            var ausentismo = ausentismos.Where(x=> x.idempleado == IdEmpleado).Where(x=> x.IdEstatus == 1).ToList();
            au.ProcesaAusentismos(ausentismo, conceptosConfigurados,  IdPeriodoNomina, IdUsuario);           
        }

        public void ProcesaAusentismos(int IdPeriodoNomina, int IdUsuario)
        {
            Ausen au = new Ausen();           

            au.ProcesaAusentismos(ausentismos, conceptosConfigurados, IdPeriodoNomina, IdUsuario);
        }

        /// <summary>
        /// Obtiene el listado de ausentismos que aplicaran en determinado periodo de nómina.
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        public void GetAusentismos(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                string consulta = "sp_RegresaAusentismos " + IdPeriodoNomina;
                var ausentismos = entidad.Database.SqlQuery<sp_RegresaAusentismos_Result>(consulta).ToList();

                this.ausentismos = ausentismos;
            }
        }

        /// <summary>
        /// Metodo que obtiene las tablas de impuestos del IMSS
        /// </summary>
        public void GetImpuestosIMSS()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var impuestos = entidad.ImpuestosIMSS.Where(x => x.IdEstatus == 1).ToList();

                ListImpuestosIMSS = impuestos;
            }
        }

        /// <summary>
        /// Metodo que va por los datos del periodo de nómina 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina.</param>
        public void GetPeriodoNomina(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo).FirstOrDefault();

                Periodo = periodo;
            }
        }

        /// <summary>
        /// Metodo que obtiene los datos de la unidad de negocio a la que pertece el pertenece el periodo que se esta procensando.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        public void GetDatosUnidadNegocio(int IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var unidad = entidad.Cat_UnidadNegocio.Where(x => x.IdUnidadNegocio == IdUnidadNegocio).FirstOrDefault();

                UnidadNegocio = unidad;
            }
        }

        public void GetTipoNomina(int IdTipoNomina)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var tipoNomina = entidad.Cat_TipoNomina.Where(x => x.IdTipoNomina == IdTipoNomina).FirstOrDefault();

                TipoNomina = tipoNomina;
            }
        }

        /// <summary>
        /// Metodo que obtiene los datos de todos los registros patronales del cliente.
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente.</param>
        public void GetRegistrosPatronales(int IdCliente)
        {
            ListRegistroPatronal = ObtenerRPByIdCliente(IdCliente);
        }

        /// <summary>
        /// Metodo que obtiene las incidencias capturadas para un periodo especifico.
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo del que se obtendran las incidencias</param>
        public void GetIncidencias(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                listIncidencias = entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && x.BanderaConceptoEspecial != 1 && x.BanderaPensionAlimenticia == null && x.BanderaInfonavit == null && x.BanderaFonacot == null && x.BanderaAdelantoPULPI == null && x.CalculoAutomatico != "SI").ToList();                
            }
        }

        public List<vIncidencias> GetIncidenciasFaltasIncapacidadesPara7moDIa(int IdPeriodo, List<int> IdsEmpleados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                string[] agrupadores = { "500", "501" };
                return entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && IdsEmpleados.Contains((int)x.IdEmpleado) && agrupadores.Contains(x.ClaveGpo)).ToList();
            }
        }

        public List<vIncidencias> GetIncidenciasHorasPara7moDIa(int IdPeriodo, List<int> IdsEmpleados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && IdsEmpleados.Contains((int)x.IdEmpleado) && x.TipoConcepto == "DD" && x.TipoDato == "Cantidades" && x.CalculoDiasHoras == "Horas" && x.AfectaSeldo == "SI").ToList();
            }
        }       

        /// <summary>
        /// Metodo que obtiene las incidencias del periodo para un empleados especifico
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina.</param>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        public void GetIncidenciasEmpleado(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _incidnecias = (from b in entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado) select b).ToList();

                listIncidencias = _incidnecias;
            }
        }

        public List<vIncidencias> GetIncidenciasEmpleado_(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _incidnecias = (from b in entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado) select b).ToList();

                 return  _incidnecias;
            }
        }

        public List<vIncidencias> GetIncidenciasEmpleadoPagoAutomatico(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _incidnecias = (from b in entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.CalculoAutomatico == "SI") select b).ToList();

                return _incidnecias;
            }
        }

        /// <summary>
        /// Metodo que calcula los premios y las compensaciones, mediante la ejecucion de un procedimiento almacenado de la base de datos.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador del la unidad de negocio.</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        /// <param name="IdPuesto">Identificador del puesto que ocupa el empleado.</param>
        /// <param name="compPuesto"></param>
        /// <param name="diasLaborados">Cantidad de dias laborados del periodo de este empleado.</param>
        /// <param name="faltas">Cantidad de faltas en el periodo de este empleado.</param>
        /// <param name="SDIMSS">Salario Diario de la parte tradicional.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema que esta realizando el calculo.</param>
        /// <param name="idCliente">Identificador del cliente.</param>
        /// <param name="idCentroCostos">Identificador del centro de costos.</param>
        public void GetPercepciones_pp(int IdUnidadNegocio, int IdPeriodoNomina, int IdEmpleado, int? IdPuesto, decimal? compPuesto, decimal? diasLaborados, decimal? faltas, decimal SDIMSS, int IdUsuario, int? idCliente, int? idCentroCostos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                string consulta = "sp_Premios_Compensaciones " + IdUnidadNegocio + "," + IdPeriodoNomina + "," + IdEmpleado + "," + (IdPuesto ?? 0) + "," + (compPuesto ?? 0) + "," + (diasLaborados ?? 0) + "," + (faltas ?? 0) + "," + SDIMSS + "," + IdUsuario + "," + (idCliente ?? 0) + "," + (idCentroCostos ?? 0);
                var percepciones = entidad.Database.SqlQuery<sp_Premios_Compensaciones_Result>(consulta).FirstOrDefault();
                //var percepciones = (from b in entidad.sp_Premios_Compensaciones(IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, IdPuesto, compPuesto, diasLaborados, faltas, SDIMSS, IdUsuario, idCliente, idCentroCostos) select b).FirstOrDefault();

                percepcionesEspecialesExcento = percepciones.SumaExento;
                percepcionesEspecialesGravado = percepciones.SumaGravado;
                percepcionesEspecialesEsquema = percepciones.SumaEsquema;
            }
        }

        /// <summary>
        /// Metodo que obtiene y calcula las deducciones especiales de un empleado.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina.</param>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        /// <param name="IdPuesto">Identificador del puesto del empleado.</param>
        /// <param name="compPuesto"></param>
        /// <param name="diasLaborados">Cantidad de dias laborados en el periodo para este empleado.</param>
        /// <param name="faltas">Cantidad de faltas en el periodo para este empleado.</param>
        /// <param name="SDIMSS">Salario Diario de la parte tradicional.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema que esta realizando el calculo.</param>
        /// <param name="idCliente">Identificador del cliente.</param>
        /// <param name="idCentroCostos">Identificador del centro de costos.</param>
        /// <returns>Regresa un valor de tipo decimal con la sumatoria de todos los conceptos de deducción especial.</returns>
        public decimal GetDeduccionesEspeciales(int IdUnidadNegocio, int IdPeriodoNomina, int IdEmpleado, int? IdPuesto, decimal? compPuesto, decimal? diasLaborados, decimal? faltas, decimal SDIMSS, int IdUsuario, int? idCliente, int? idCentroCostos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                string consulta = "sp_DeduccionesEspeciales " + IdUnidadNegocio + "," + IdPeriodoNomina + "," + IdEmpleado + "," + (IdPuesto ?? 0) + "," + (compPuesto ?? 0) + "," + (diasLaborados ?? 0) + "," + (faltas ?? 0) + "," + SDIMSS + "," + IdUsuario + "," + (idCliente ?? 0) + "," + (idCentroCostos ?? 0);
                var percepciones = entidad.Database.SqlQuery<decimal?>(consulta).FirstOrDefault();
                //var percepciones = (from b in entidad.sp_DeduccionesEspeciales(IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, IdPuesto, compPuesto, diasLaborados, faltas, SDIMSS, IdUsuario, idCliente, idCentroCostos) select b).FirstOrDefault();

                if (percepciones != null)
                    return percepciones.Value;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Metodo que obtiene la lista de empleados a procesar.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        public void GetListEmpleados(int IdUnidadNegocio)
        {
            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                int[] status = { 1, 2, 3 };
                var _empleados = (from b in entidad.vEmpleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && status.Contains(x.IdEstatus)) select b).ToList();

                listEmpleados = _empleados;
            }
        }

        public void GetListEmpleadosSinAjuste(int IdUnidadNegocio, List<string> claves)
        {
            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                int[] status = { 1, 2, 3 };
                var _empleados = (from b in entidad.vEmpleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && status.Contains(x.IdEstatus) && claves.Contains(x.ClaveEmpleado)) select b).ToList();

                listEmpleadosSinAjuste = _empleados;
            }
        }

        /// <summary>
        /// Metodo que obtiene los factores correspondientes a las prestaciones.
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente.</param>
        public void GetPrestaciones(int IdCliente, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var consulta = @"select* from vPrestacionesFactor 
                                 where IdEstatus = 1 and IdEstatusFactor = 1 and IdCliente in (0, " + IdCliente + @")";
                
                //and FechaInicioVigencia in (
                //                    select top 1 FechaInicioVigencia from vPrestacionesFactor 
                //                    where FechaInicioVigencia <= '" + fechaFinPeriodo.ToString("yyyyMMdd") + "' and IdEstatus = 1 and IdEstatusFactor = 1 and IdCliente in (0, " + IdCliente + ") order by 1 desc)";

                var _prestacion = entidad.Database.SqlQuery<vPrestacionesFactor>(consulta).ToList();

                prestaciones = _prestacion;
            }
        }

        /// <summary>
        /// Metodo que obtiene las tablas de impuestos para un tipo de nómina especifico.
        /// </summary>
        /// <param name="IdTipoNomina">Identificador dek tipo de nomina.</param>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void GetImpuestosSAT(int IdTipoNomina, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _impuestos = (from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.EstatusId == 1
                                  && x.FechaInicio == (from c in entidad.ImpuestoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.EstatusId == 1 && y.IdTipoNomina == IdTipoNomina)
                                                       select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                  select b).ToList();

                ListImpuestos = _impuestos;
            }
        }

        /// <summary>
        /// Metodo que obtiene las tablas de subsidio para un tipo de nómina especifico.
        /// </summary>
        /// <param name="IdTipoNomina">Identificador del tipo de nómina.</param>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void GetSubsidioSAT(int IdTipoNomina, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _subsidio = (from b in entidad.SubsidioEmpleoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.IdEstatus == 1
                                 && x.FechaInicio == (from c in entidad.SubsidioEmpleoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.IdEstatus == 1 && y.IdTipoNomina == IdTipoNomina)
                                                      select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                 select b).ToList();

                ListSubsidio = _subsidio;
            }
        }

        /// <summary>
        /// Metodo que obtiene las tablas de impuestos mensuales para el caso de un periodo con ajuste de impuestos.
        /// </summary>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void GetImpuestosSAT_Ajuste(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _impuestos = (from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == 4 && x.EstatusId == 1
                                  && x.FechaInicio == (from c in entidad.ImpuestoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.EstatusId == 1 && y.IdTipoNomina == 4)
                                                       select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                  select b).ToList();

                ListImpuestos_Ajuste = _impuestos;
            }
        }

        public void GetImpuestosSAT_AjusteSecundario(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _impuestos = (from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == 4 && x.EstatusId == 1
                                  && x.FechaInicio == (from c in entidad.ImpuestoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.EstatusId == 1 && y.IdTipoNomina == 4)
                                                       select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                  select b).ToList();

                ListImpuestos_AjusteSecundario = _impuestos;
            }
        }

        /// <summary>
        /// Metodo que obtiene las tablas de impuestos mensuales para el caso de un periodo con ajuste de impuestos anual.
        /// </summary>
        /// <param name="fechaFinPeriodo"></param>
        public void GetImpuestosSAT_Ajuste_Anual(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _impuestos = (from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == 11 && x.EstatusId == 1
                                  && x.FechaInicio == (from c in entidad.ImpuestoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.EstatusId == 1 && y.IdTipoNomina == 11)
                                                       select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                  select b).ToList();

                ListImpuestos_Ajuste = _impuestos;
            }
        }

        public void GetEmpleadosAjusteAnual(int IdUnidadNegocio, int Año)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var empleados = entidad.sp_EmpleadosAjusteAnual(IdUnidadNegocio, Año).ToList();

                ListaEmpleadosAjusteAnual = empleados;
            }
        }
               
        /// <summary>
        /// Metodo que obtiene las tablas de impuestos auxiliares.
        /// </summary>
        /// <param name="IdTipoNomina">Identificador del tipo de nómina.</param>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void GetImpuestosSAT_Auxiliar(int IdTipoNomina, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _impuestos = (from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.EstatusId == 1
                                  && x.FechaInicio == (from c in entidad.ImpuestoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.EstatusId == 1 && y.IdTipoNomina == IdTipoNomina)
                                                       select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                  select b).ToList();

                ListImpuestos_Auxiliar = _impuestos;
            }
        }

        /// <summary>
        /// Metodo que obtiene las tablas de subsidio mensuales para el caso de un periodo con ajuste de impuestos.
        /// </summary>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void GetSubsidioSAT_Ajuste(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _subsidio = (from b in entidad.SubsidioEmpleoSat.Where(x => x.IdTipoNomina == 4 && x.IdEstatus == 1
                                 && x.FechaInicio == (from c in entidad.SubsidioEmpleoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.IdEstatus == 1 && y.IdTipoNomina == 4)
                                                      select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                 select b).ToList();

                ListSubsidio_Ajuste = _subsidio;
            }
        }
                

        /// <summary>
        /// Metodo qye obtiene las tablas de subsidio auxiliares.
        /// </summary>
        /// <param name="IdTipoNomina">Identificador del tipo de nómina.</param>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void GetSubsidioSAT_Auxiliar(int IdTipoNomina, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _subsidio = (from b in entidad.SubsidioEmpleoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.IdEstatus == 1
                                 && x.FechaInicio == (from c in entidad.SubsidioEmpleoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.IdEstatus == 1 && y.IdTipoNomina == IdTipoNomina)
                                                      select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                 select b).ToList();

                ListSubsidio_Auxiliar = _subsidio;
            }
        }

        /// <summary>
        /// Metodo que obtiene los valores del Sueldo Minimo General Vigente y La UMA
        /// </summary>
        /// <param name="fechaFinPeriodo">Fecha de fin del periodo, para obtener el dato segun corresponda</param>
        private void GetSueldosMinimos(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int año = fechaFinPeriodo.Year;
                var sueldos_minimos = (from b in entidad.Sueldos_Minimos.Where(x => x.Ano == año && fechaFinPeriodo >= x.FechaInicio)
                                       select b).OrderByDescending(x => x.FechaInicio).FirstOrDefault();

                SueldosMinimos = sueldos_minimos;
            }
        }

        /// <summary>
        /// Metodo que obtiene los registros de nómina con la que se ajusta el periodo.
        /// </summary>
        /// <param name="periodosAjuste">cadena con los Identificadores de periodo de nómina separados por comas.</param>
        private void GetNominaAjuste(string periodosAjuste)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {                
                string[] _periodos = periodosAjuste.Replace(" ", "").Split(',');
                _periodos = _periodos.Where(x => x != "").ToArray();
                int[] idsPeriodos = Array.ConvertAll(_periodos, int.Parse);

                var nomina = (from b in entidad.vNomina.Where(x => idsPeriodos.Contains(x.IdPeriodoNomina)) select b).ToList();

                ListNominaAjuste = nomina;
            }
        }

        private void GetNominaAjusteSecundario(string periodosAjusteSecundario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                string[] _periodos = periodosAjusteSecundario.Replace(" ", "").Split(',');
                _periodos = _periodos.Where(x => x != "").ToArray();
                int[] idsPeriodos = Array.ConvertAll(_periodos, int.Parse);

                var nomina = (from b in entidad.vNomina.Where(x => idsPeriodos.Contains(x.IdPeriodoNomina)) select b).ToList();

                ListNominaAjusteSecundario = nomina;
            }
        }

        /// <summary>
        /// Metodo que ayuda a validar si el registro del calculo se guardara en la base de datos o no.
        /// </summary>
        /// <returns>true/false si debe de insertar en la base de datos.</returns>
        public bool ValidaInsercionRegistro()
        {
            bool valida = true;

            if (Periodo.TipoNomina == "Complemento")
            {
                if (nominaTrabajo.ER <= 0 && nominaTrabajo.ERS <= 0)
                {
                    if (nominaTrabajo.DD <= 0 && nominaTrabajo.DDS <= 0)
                    {
                        valida = false;
                    }
                }
            }

            return valida;
        }

        /// <summary>
        /// Metodo para guardar los registros calculados en la tabla correspondiente de la base de datos.
        /// </summary>
        /// <param name="item">Objeto del tipo vEmpleados con toda la información del empleado.</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina. </param>
        public void GuardarNominaTrabajo(vEmpleados item, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                nominaTrabajo.IdEmpleado = item.IdEmpleado;
                nominaTrabajo.IdPeriodoNomina = IdPeriodo;
                nominaTrabajo.IdRegistroPatronal = item.IdRegistroPatronal;
                nominaTrabajo.RiesgoTrabajo = ListRegistroPatronal.Where(x => x.IdRegistroPatronal == item.IdRegistroPatronal).Select(x => x.RiesgoTrabajo).FirstOrDefault();
                nominaTrabajo.IdEntidad = item.IdEntidad;
                nominaTrabajo.IdCentroCostos = item.IdCentroCostos;
                nominaTrabajo.IdDepartamento = item.IdDepartamento;
                nominaTrabajo.IdPuesto = item.IdPuesto;
                nominaTrabajo.IdBancoTrad = item.IdBancoTrad;
                nominaTrabajo.IdArea = item.IdArea;
                nominaTrabajo.IdSucursal = item.IdSucursal;
                nominaTrabajo.IdSindicato = item.idSindicato;
                nominaTrabajo.CuentaBancariaTrad = item.CuentaBancariaTrad;
                nominaTrabajo.CuentaInterbancariaTrad = item.CuentaInterbancariaTrad;
                nominaTrabajo.FechaAltaIMSS = item.FechaAltaIMSS;
                nominaTrabajo.FechaReconocimientoAntiguedad = item.FechaReconocimientoAntiguedad;
                nominaTrabajo.AjusteImpuesto = Periodo.AjusteDeImpuestos;

                nominaTrabajo.SDI = SDI;
                nominaTrabajo.SueldoDiarioReal = SD_Real;
                nominaTrabajo.SueldoDiario = SD_IMSS;

                nominaTrabajo.IdEstatus = 1;
                nominaTrabajo.IdCaptura = IdUsuario;
                nominaTrabajo.FechaCaptura = DateTime.Now;

                if (Periodo.TipoNomina == "Aguinaldo" && ListDiasTrabajadosAguinaldo != null)
                    nominaTrabajo.DiasTrabajados = ListDiasTrabajadosAguinaldo.Where(x => x.IdEmpleado == IdEmpleado).Select(x => x.DiasTrabajados).Sum();

                entidad.NominaTrabajo.Add(nominaTrabajo);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para guardar los registros calculados en la tabla correspondiente de la base de datos, especificamente para calculos de finiquitos.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="IdPeriodo"></param>
        public void GuardarNominaTrabajoFiniquitos(vEmpleados item, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                nominaTrabajo.IdEmpleado = item.IdEmpleado;
                nominaTrabajo.IdPeriodoNomina = IdPeriodo;
                nominaTrabajo.IdRegistroPatronal = item.IdRegistroPatronal;
                nominaTrabajo.RiesgoTrabajo = ListRegistroPatronal.Where(x => x.IdRegistroPatronal == item.IdRegistroPatronal).Select(x => x.RiesgoTrabajo).FirstOrDefault();
                nominaTrabajo.IdEntidad = item.IdEntidad;
                nominaTrabajo.IdCentroCostos = item.IdCentroCostos;
                nominaTrabajo.IdDepartamento = item.IdDepartamento;
                nominaTrabajo.IdPuesto = item.IdPuesto;
                nominaTrabajo.IdBancoTrad = item.IdBancoTrad;
                nominaTrabajo.IdArea = item.IdArea;
                nominaTrabajo.IdSucursal = item.IdSucursal;
                nominaTrabajo.IdSindicato = item.idSindicato;
                nominaTrabajo.CuentaBancariaTrad = item.CuentaBancariaTrad;
                nominaTrabajo.CuentaInterbancariaTrad = item.CuentaInterbancariaTrad;                
                nominaTrabajo.AjusteImpuesto = Periodo.AjusteDeImpuestos;

                nominaTrabajo.SDI = SDI;
                nominaTrabajo.SueldoDiarioReal = SD_Real;
                nominaTrabajo.SueldoDiario = SD_IMSS;

                nominaTrabajo.IdEstatus = 4;
                nominaTrabajo.IdCaptura = IdUsuario;
                nominaTrabajo.FechaCaptura = DateTime.Now;

                entidad.NominaTrabajo.Add(nominaTrabajo);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para borrar todos los registros de la tabla de nómina de la base de datos.
        /// </summary>
        public void DeleteRegistroNominaTrabajo()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (registro != null)
                {
                    entidad.NominaTrabajo.Remove(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para borrar un registro especifico de la tabla de nómina de la base de datos. 
        /// </summary>
        /// <param name="IdEmpleado"></param>
        /// <param name="IdPeriodoNomina"></param>
        public void DeleteRegistroNominaTrabajo(int IdEmpleado, int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (registro != null)
                {
                    entidad.NominaTrabajo.Remove(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para borrar todos los registros de la tabla de nómina de un periodo de nómina.
        /// </summary>
        /// <param name="pIdPeriodo">Identificador del periodo de nómina.</param>
        public void DeleteNominaTrabajo(int pIdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == pIdPeriodo) select b);

                if (registro != null)
                {
                    entidad.NominaTrabajo.RemoveRange(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para borrar todos los registros de la tabla de nómina de los empleados que causaron baja.
        /// </summary>
        /// <param name="pIdPeriodo"></param>
        /// <param name="IdUnidadNegocio"></param>
        public void DeleteNominaTrabajoBajas(int pIdPeriodo, int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var ce = new ClassEmpleado();
                var emp = ce.GetEmpleadoByUnidadNegocioBajas(IdUnidadNegocio).Select(x => x.IdEmpleado).ToList();
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == pIdPeriodo && emp.Contains(x.IdEmpleado)) select b);

                if (registro != null)
                {
                    entidad.NominaTrabajo.RemoveRange(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Valida si un registro de la tabla de nómina ya fue insertado.
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina.</param>
        /// <returns>true/false si se inserto el registro o no.</returns>
        public bool ValidaRegistroNominaTrabajo(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (registro != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Metodo que obtiene las prestaciones correspondientes.
        /// </summary>
        /// <param name="IdPrestaciones"></param>
        public void GetPrestacionesEmpleado(int? IdPrestaciones)
        {
            IdPrestacionesEmpleado = 1;
            if (IdPrestaciones != null && IdPrestaciones != 0)
                IdPrestacionesEmpleado = (int)IdPrestaciones;
        }

        /// <summary>
        /// Metodo que calcula los dias trabajados del del empleado.
        /// </summary>
        /// <param name="tipoEsq">Arreglo que especifica el tipo de esquema en que se le paga al empleaddo</param>
        public void GetDiasTrabajados(string[] tipoEsq)
        {
            nominaTrabajo.Faltas = 0;
            nominaTrabajo.Incapacidades = 0;
            nominaTrabajo.Dias_Vacaciones = 0;
            decimal diasMas = 0;
            decimal diasMenos = 0;
            fraccionHorasMas = 0;
            fraccionHorasMenos = 0;

            nominaTrabajo.Faltas = (decimal)incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.ClaveGpo == "500" && x.CalculoDiasHoras != "Horas" && x.IdConcepto != conceptosConfigurados.IdConceptoFaltas).Select(X => X.Cantidad).Sum();
            nominaTrabajo.Incapacidades = (decimal)incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.ClaveGpo == "501").Select(X => X.Cantidad).Sum();
            nominaTrabajo.Dias_Vacaciones = (decimal)incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "ER" && x.ClaveGpo == "002").Select(X => X.Cantidad).Sum();
            
            

            nominaTrabajo.DiasTrabajados = DiasPago;
            diasMas += (decimal)incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "ER" && x.AfectaSeldo == "SI" && x.ClaveGpo != "002" && x.CalculoDiasHoras != "Horas").Select(X => X.Cantidad).Sum();
            diasMenos += (decimal)incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.AfectaSeldo == "SI" && x.CalculoDiasHoras != "Horas" && x.IdConcepto != conceptosConfigurados.IdConceptoFaltas).Select(X => X.Cantidad).Sum();
           
            // conceptos de prestaciones especiales paternidad 
            diasMenos += (decimal)incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "ER" && x.ClaveGpo == "902").Select(X => X.Cantidad).Sum();

            // se obtienen los conceptos que van a sumar a los dias trabajados en horas.
            var diasMasFraccion = incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "ER" && x.AfectaSeldo == "SI" && x.ClaveGpo != "002" && x.CalculoDiasHoras == "Horas").ToList();

            foreach (var df in diasMasFraccion)
            {
                decimal fraccion = (df.Cantidad ?? 0) * (1M / (df.SDEntre ?? 1));
                fraccionHorasMas = fraccion;
            }
            //////

            // se obtienen los conceptos que van a restar a los dias trabajados en horas.
            var diasMenosFraccion = incidenciasEmpleado.Where(x => tipoEsq.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.AfectaSeldo == "SI" && x.CalculoDiasHoras == "Horas").ToList();

            foreach (var df in diasMenosFraccion)
            {
                decimal fraccion = (df.Cantidad ?? 0) * ( 1M / (df.SDEntre ?? 1));                
                fraccionHorasMenos = fraccion;
            }
            //////

            diasMenos += (decimal)nominaTrabajo.Dias_Vacaciones;
            if (configuracionNominaEmpleado.DiasSueldo > 0) { nominaTrabajo.DiasTrabajados += configuracionNominaEmpleado.DiasSueldo; }

            if (UnidadNegocio.DiasFraccioandos == "S" && UnidadNegocio.FactorDiasFraccionados > 0)
            {
                decimal diasEspecificosMas = 0;
                decimal diasEspecificosMenos = 0;
                if (UnidadNegocio.IdsConceptosFraccionados != null && UnidadNegocio.IdsConceptosFraccionados.Length > 0)
                {
                    var clavesFraccionadas = UnidadNegocio.IdsConceptosFraccionados.Replace(" ", "").Split(',').ToArray();
                    diasEspecificosMas = (decimal)incidenciasEmpleado.Where(x => clavesFraccionadas.Contains(x.ClaveConcepto) && x.TipoConcepto == "ER" && x.TipoDato == "Cantidades").Select(x => x.Cantidad).Sum();
                    diasEspecificosMenos = (decimal)incidenciasEmpleado.Where(x => clavesFraccionadas.Contains(x.ClaveConcepto) && x.TipoConcepto == "DD" && x.TipoDato == "Cantidades").Select(x => x.Cantidad).Sum();

                    if (diasEspecificosMas > 0)
                    {
                        var masAux = diasMas - diasEspecificosMas;
                        var masFac = diasEspecificosMas * (decimal)UnidadNegocio.FactorDiasFraccionados;

                        diasMas = masAux + masFac;
                    }

                    if (diasEspecificosMenos > 0)
                    {
                        var menosAux = diasMenos - diasEspecificosMenos;
                        var menosFac = diasEspecificosMenos * (decimal)UnidadNegocio.FactorDiasFraccionados;

                        diasMenos = menosAux + menosFac;
                    }
                }
                else
                {
                    diasMas = diasMas * (decimal)UnidadNegocio.FactorDiasFraccionados;
                    diasMenos = diasMenos * (decimal)UnidadNegocio.FactorDiasFraccionados;
                }
            }            

            nominaTrabajo.DiasTrabajados += diasMas;
            nominaTrabajo.DiasTrabajados -= diasMenos;
            
            if (nominaTrabajo.DiasTrabajados < 1) { nominaTrabajo.DiasTrabajados = 0; }

            if (UnidadNegocio.FaltasImporte == "S")
            {
                string[] gpos = { "500", "501" };
                var diasSueldo = nominaTrabajo.DiasTrabajados - incidenciasEmpleado.Where(x => gpos.Contains(x.ClaveGpo) && _tipoEsquemaT.Contains(x.TipoEsquema)).Sum(x => x.Cantidad);

                if (diasSueldo < 1 && Periodo.TipoNomina == "Nomina")
                    nominaTrabajo.DiasTrabajados = 15;
            }
        }

        /// <summary>
        /// Metodo que calculo el Impuesto sobre la Nomina
        /// </summary>
        public void CalculaISN()
        {
            decimal porcentaje = 0;
            decimal incidenciasNOISN = 0;
            try { porcentaje = (decimal)listEntidades.Where(x => x.Id == (int)IdEntidadFederativa).Select(x => x.ISN).First() * 0.01M; } catch { }
            if (porcentaje == 0) { try { porcentaje = (decimal)UnidadNegocio.PorcentajeISN * 0.01M; } catch { porcentaje = 0; } }

            var reintISR = nominaTrabajo.ReintegroISR ?? 0;
            incidenciasNOISN = (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && x.IntegraISN == "NO").Select(X => X.Monto).Sum();
            incidenciasNOISN += (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "OTRO" && x.IntegraISN == "NO").Select(X => X.Monto).Sum();
            decimal totalPercepcionesSinSubsidio = (decimal)(nominaTrabajo.ER - (nominaTrabajo.SubsidioPagar + reintISR) - incidenciasNOISN);
            nominaTrabajo.BaseISN = totalPercepcionesSinSubsidio;
            nominaTrabajo.PorcentajeISN = porcentaje;
            nominaTrabajo.ISN = totalPercepcionesSinSubsidio * porcentaje;

            if (Periodo.TipoNomina == "PTU")
            {
                nominaTrabajo.BaseISN = 0;
                nominaTrabajo.PorcentajeISN = 0;
                nominaTrabajo.ISN = 0;
            }
        }

        public void CalculaISN_Real()
        {
            decimal porcentaje = 0;
            decimal incidenciasNOISN = 0;
            try { porcentaje = (decimal)listEntidades.Where(x => x.Id == (int)IdEntidadFederativa).Select(x => x.ISN).First() * 0.01M; } catch { }
            if (porcentaje == 0) { try { porcentaje = (decimal)UnidadNegocio.PorcentajeISN * 0.01M; } catch { porcentaje = 0; } }
                        
            incidenciasNOISN = (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && x.IntegraISN == "NO").Select(X => X.MontoReal).Sum();
            incidenciasNOISN += (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "OTRO" && x.IntegraISN == "NO").Select(X => X.MontoReal).Sum();
            decimal totalPercepcionesSinSubsidio = ((nominaTrabajo.Total_ER_Real ?? 0) - (nominaTrabajo.Subsidio_Real ?? 0)  - incidenciasNOISN);          
            nominaTrabajo.ISN_Real = totalPercepcionesSinSubsidio * porcentaje;

            if (Periodo.TipoNomina == "PTU")
            {               
                nominaTrabajo.ISN_Real = 0;
            }
        }

        /// <summary>
        /// Metodo que setea el valor del salario diario del empleado, para evitar errores de calculo.
        /// </summary>
        /// <param name="SD">Salario Diario del empleado</param>
        /// <returns>Importe que contiene el SD</returns>
        public decimal GetSD(decimal? SD)
        {
            decimal sueldo = 0;

            if (SD != null)
            {
                sueldo = (decimal)SD;
            }

            return sueldo;
        }

        /// <summary>
        /// Metodo que calcula las incidencias que se programan para inyectarse automaticamente cada periodo.
        /// </summary>
        /// <param name="IncluirDescuentos">Para saber si se incluyen los descuentos.</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo que se esta procesando.</param>
        /// <param name="IdEmpleado">Identificador del empleados que se esta calculando.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema que esta realizando el calculo.</param>
        public void ProcesaIncidenciasProgramadas(string IncluirDescuentos, int IdUnidadNegocio, int IdPeriodoNomina, int? IdEmpleado, int IdUsuario)
        {
            ClassIncidenciasProgramadas cins = new ClassIncidenciasProgramadas();
            if (IdEmpleado != null)
                cins.ProcesaIncidenciasProgramadas(IncluirDescuentos, IdUnidadNegocio, IdPeriodoNomina, (int)IdEmpleado, IdUsuario);
            else
                cins.ProcesaIncidenciasProgramadas(IncluirDescuentos, IdUnidadNegocio, IdPeriodoNomina, IdUsuario);
        }

        /// <summary>
        /// Metodo que obtiene los creditos infonavit para determinada unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        public void GetCreditos(int IdUnidadNegocio)
        {
            ClassInfonavit ci = new ClassInfonavit();
            creditosInfonavit = ci.getCreditos(IdUnidadNegocio).Where(x => x.Activo != "NO").ToList();
        }

        /// <summary>
        /// Metodo que obtiene las pensiones alimenticias para determinada unidad de negocio. 
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        public void GetPensiones(int IdUnidadNegocio)
        {
            ClassPensiones ci = new ClassPensiones();
            pensionAlimenticia = ci.getPensiones(IdUnidadNegocio);
        }

        /// <summary>
        /// Metodo que procesa los creditos infonavit que se incluiran en el periodo que se esta calculando.
        /// </summary>
        /// <param name="credito">Objeto de tipo vCreditoInfonavit con el listado de creditos que aplicaran.</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="UMI">Unidad de Medida</param>
        /// <param name="diasPeriodo">Cantidad de dias que comprenden el periodo de nómina.</param>
        /// <param name="IdUsuario">Identificador del Usuario del sistema.</param>
        /// <param name="ClaveTipoNomina">Clave del catalogo del SAT que especifica el tipo de nómina.</param>
        /// <exception cref="Exception">Excepcion devuelta por el metodo en caso de un error.</exception>
        public void ProcesaCredito(vCreditoInfonavit credito, int IdPeriodoNomina, decimal UMI, decimal diasPeriodo, int IdUsuario, string ClaveTipoNomina, int IdUnidadNegocio)
        {
            montoCreditoInfonavit = 0;
            montoCreditoInfonavitEsq = 0;
            if (credito != null)
            {
                int IdConcepto = 0;
                try { IdConcepto = (int)conceptosConfigurados.IdConceptoInfonavit; } catch { throw new Exception("Hay creditos INFONAVIT cargadas pero no se configuro ningun concepto. "); }
                ClassInfonavit ci = new ClassInfonavit();
                ci.procesaCreditosInfonavit(credito, IdPeriodoNomina, UMI, diasPeriodo, IdConcepto, IdUsuario, SDI, ClaveTipoNomina, Periodo.FechaFin, IdUnidadNegocio);

                montoCreditoInfonavit = ci.montoCredito;
                montoCreditoInfonavitEsq = ci.montoCreditoEsq;
            }
        }

        /// <summary>
        /// Metodo que calcula y procesa las Pensiones Alimenticias capturadas en el sistema.
        /// </summary>
        /// <param name="pensionAlimenticia">Listado de pensiones a procesar.</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="totalPercepciones">Total de percepciones, estas se ocupan en el calculo</param>
        /// <param name="totalPercepcionesEsq">Total de percepciones, estas se ocupan en el calculo</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema</param>
        /// <exception cref="Exception">Excepcion devuelta por el metodo en caso de un error al procesar.</exception>
        public void ProcesaPension(List<vIncidencias> incidenciasEmpleado, NominaTrabajo nom, vEmpleados datosEmpleados, List<vPensionAlimenticia> pensionAlimenticia, int IdPeriodoNomina, decimal totalPercepciones, decimal totalPercepcionesEsq, int IdUsuario, int idcliente)
        {
            try
            {
                montoPension = 0;
                montoPensionEsq = 0;
                if (pensionAlimenticia != null && pensionAlimenticia.Count > 0)
                {
                    int IdConcepto = 0;
                    try { IdConcepto = (int)conceptosConfigurados.IdConceptoPensionAlimenticia; } catch { throw new Exception("Hay pensiones cargadas pero no se configuro ningun concepto. "); }
                    ClassPensiones ci = new ClassPensiones();

                    foreach (var item in pensionAlimenticia)
                    {
                        ci.procesaPensionAlimenticia(incidenciasEmpleado, nom, datosEmpleados, item, IdPeriodoNomina, totalPercepciones, totalPercepcionesEsq, IdConcepto, IdUsuario, idcliente);

                        montoPension = ci.montoPension;
                        montoPensionEsq = ci.montoPensionEsq;

                        //Actualiza nomina
                        nominaTrabajo.DD += montoPension;
                        nominaTrabajo.DDS += montoPensionEsq;
                        nominaTrabajo.Neto -= montoPension;
                        nominaTrabajo.Netos -= montoPensionEsq;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar pensión. " + ex.Message);
            }
        }

        /// <summary>
        /// Carga los conceptos de nomina configurados para este cliente.
        /// </summary>
        public void GetConceptosConfigurados()
        {
            ClassConceptosFiniquitos cconceptos = new ClassConceptosFiniquitos();

            conceptosConfigurados = cconceptos.GetvConfiguracionConceptosFiniquitos(UnidadNegocio.IdCliente);
        }

        /// <summary>
        /// Metodo que obtiene los creditos fonacot correspondientes a determinada unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        public void GetCreditosFonacot(int IdUnidadNegocio)
        {
            ClassFonacot cf = new ClassFonacot();
            creditosFonacot = cf.getCreditosFonacot(IdUnidadNegocio).Where(x => x.Activo != "NO").ToList();
        }

        /// <summary>
        /// Metodo que obtiene en caso de que exista una configuración especial para el proceso.
        /// </summary>
        public void GetConfiguracionNominaPeriodoEmpleado()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var configuracion = (from b in entidad.ConfiguracionNominaPeriodoEmpleado.Where(x => x.IdPeriodoNomina == Periodo.IdPeriodoNomina && x.IdEstatus == 1) select b).ToList();
                lconfiguracionNominaEmpleado = configuracion;
            }
        }

        /// <summary>
        /// Metodo que obtiene el catalogo del SAT de entidades federativas.
        /// </summary>
        public void GetListEntidades()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var entidades = (from b in entidad.Cat_EntidadFederativa select b).ToList();
                listEntidades = entidades;
            }
        }

        public ConfiguracionNominaPeriodoEmpleado GetConfiguracionNominaPeriodoEmpleado(int IdPeriodoNomina, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var configuracion = (from b in entidad.ConfiguracionNominaPeriodoEmpleado.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado) select b).FirstOrDefault();
                return configuracion;
            }
        }

        /// <summary>
        /// Metodo que calcula y procesa los credito fonacot que estan capturados en el sistema.
        /// </summary>
        /// <param name="creditos"></param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema que es procesando el calculo.</param>
        /// <param name="ClaveTipoNomina"></param>
        /// <exception cref="Exception">expecion devuelta por el metodo en caso de un error.</exception>
        public void ProcesaCreditoFonacot(List<vCreditoFonacot> creditos, int IdPeriodoNomina, int IdUsuario, string ClaveTipoNomina)
        {
            montoCreditoFonacot = 0;
            foreach (var item in creditos)
            {
                int IdConcepto = 0;
                try { IdConcepto = (int)conceptosConfigurados.IdConceptoFonacot; } catch { throw new Exception("Hay credito Fonacot pero no se configuro ningun concepto. "); }
                ClassFonacot cf = new ClassFonacot();
                cf.ProcesaCreditoFonacot(item, IdPeriodoNomina, IdConcepto, IdUsuario, ClaveTipoNomina);

                montoCreditoFonacot += cf.montoDescuentoPeriodo;
            }
        }

        /// <summary>
        /// Metodo que calcula incidencias que tienen una configuración especial basada en el los dias trabajados.
        /// </summary>
        public void ProcesaIncidenciasMultiplicaDT()
        {
            ClassIncidencias ci = new ClassIncidencias();
            var incidencias = incidenciasEmpleado.Where(x => x.MultiplicaDT == "SI").ToList();
            montoIncidenciasMultiplicaDT = 0;
            montoIncidenciasMultiplicaDTGrabado = 0;
            foreach (var item in incidencias)
            {
                decimal monto = (decimal)item.Cantidad * (decimal)nominaTrabajo.DiasTrabajados;
                ci.UpdateExentosGravados(item.IdIncidencia, monto, null, Periodo.FechaFin, SD_IMSS);
                montoIncidenciasMultiplicaDTGrabado += ci.Gravado;
                montoIncidenciasMultiplicaDT += monto;
            }
        }

        /// <summary>
        /// Metodo que obtiene en caso de que existan dias que resten a los dias trabajados del periodo para un empleado. 
        /// </summary>
        /// <param name="fechaAlta">Fecha de alta del empleado</param>
        /// <param name="fechaIncioPeriodo">Fecha inicial del periodo de nómina.</param>
        /// <param name="fechaFinPeriodo">Fecha final del periodo de nómina.</param>
        public void getDiasPorAlta(DateTime? fechaAlta, DateTime fechaIncioPeriodo, DateTime fechaFinPeriodo)
        {            
            DiasMenosPorAlta = 0;
            DiasMasPorAlta = 0;
            if (fechaAlta != null) 
            {
                //Dias Menos
                if (UnidadNegocio.DiasAltaMenos == "S")
                {
                    if (fechaAlta > fechaIncioPeriodo && fechaAlta < fechaFinPeriodo)
                    {
                        decimal diasMenos = ((DateTime)fechaAlta).Subtract(fechaIncioPeriodo).Days;
                        DiasMenosPorAlta = diasMenos;

                        if (UnidadNegocio.DiasAltaMenosFraccionados == "S" && UnidadNegocio.FactorDiasFraccionados > 0)
                            DiasMenosPorAlta = diasMenos * (decimal)UnidadNegocio.FactorDiasFraccionados;
                    }
                }

                //Dias Mas
                if (UnidadNegocio.DiasAltaMas == "S" && !validaDiasPagadosNominasAnt())
                {
                    var diasAnteriores = getDiasPeriodoByClaveSat(TipoNomina.Clave_Sat);

                    if (fechaAlta < fechaIncioPeriodo && diasAnteriores > 0)
                    {
                        DateTime fechaValidacion = fechaIncioPeriodo.AddDays(-(diasAnteriores + 1));
                        if (fechaAlta >= fechaValidacion)
                        {
                            decimal diasMas = fechaIncioPeriodo.Subtract((DateTime)fechaAlta).Days;
                            DiasMasPorAlta = diasMas;

                            if (UnidadNegocio.DiasAltaMasFraccionados == "S" && UnidadNegocio.FactorDiasFraccionados > 0 )
                                DiasMasPorAlta = diasMas * (decimal)UnidadNegocio.FactorDiasFraccionados;
                        }
                    }
                }
            }
        }

        public bool validaDiasPagadosNominasAnt()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registros = entidad.Nomina.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado).FirstOrDefault();

                return registros != null ? true : false;
            }
        }

        public int getDiasPeriodoByClaveSat(string claveSat)
        {
            int dias = 0;
            switch (claveSat)
            {
                case "02":
                    dias = 7;
                    break;
                case "03":
                    dias = 14;
                    break;
                case "04":
                    dias = 15;
                    break;
                case "05":
                    dias = 30;
                    break;
                default:
                    dias = 0;
                    break;
            }

            return dias;
        }
               
        /// <summary>
        /// Metodo que obtiene datos de configuración adicional que se deba considerar para el calculo de nómina de un empleado.
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns>Regresa un objeto con la configuracíon</returns>
        public ConfiguracionNominaPeriodoEmpleado getconfiguracionEmpleadoNomina(int IdEmpleado)
        {
            var conf = lconfiguracionNominaEmpleado.Where(x => x.IdEmpleado == IdEmpleado).FirstOrDefault();

            if (conf == null)
                conf = new ConfiguracionNominaPeriodoEmpleado();

            return conf;
        }

        /// <summary>
        /// Metodo para modificar la configuración avanzada para el proceso de nómina para un empleado.
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="SuspencionTradicional">Bandera que indica si se debe suspender el calculo tradicional.</param>
        /// <param name="SuspencionEsq">Bandera que indica si se debe suspender el calculo esquema.</param>
        /// <param name="SuspencionCS">Bandera que indica si de deben suspender las cargas sociales.</param>
        /// <param name="pagSueldos">Cantidad de días que se deben pagar de sueldos.</param>
        /// <param name="cobCargas">Cantidad de días de cargas socuales que se deben de cobrar.</param>
        /// <param name="incidenciasAut">Bandera que indica si se deben calcular las incidencias automaticas.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema.</param>
        public void ConfiguracionAvanzadaNomina(int IdEmpleado, int IdPeriodoNomina, int? SuspencionTradicional, int? SuspencionEsq, int? SuspencionCS, decimal? pagSueldos, decimal? cobCargas, int? incidenciasAut, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var conf = (from b in entidad.ConfiguracionNominaPeriodoEmpleado.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1) select b).FirstOrDefault();

                if (conf == null)
                {
                    ConfiguracionNominaPeriodoEmpleado ca = new ConfiguracionNominaPeriodoEmpleado();
                    ca.IdEmpleado = IdEmpleado;
                    ca.IdPeriodoNomina = IdPeriodoNomina;
                    ca.SupenderSueldoTradicional = SuspencionTradicional;
                    ca.SuspenderSueldoEsquema = SuspencionEsq;
                    ca.SuspenderCargasSociales = SuspencionCS;
                    ca.DiasSueldo = pagSueldos;
                    ca.DiasCargaSocial = cobCargas;
                    ca.IncidenciasAutomaticas = incidenciasAut;
                    ca.IdEstatus = 1;
                    ca.IdCaptura = IdUsuario;
                    ca.FechaCaptura = DateTime.Now;

                    entidad.ConfiguracionNominaPeriodoEmpleado.Add(ca);
                }
                else
                {
                    conf.SupenderSueldoTradicional = SuspencionTradicional;
                    conf.SuspenderSueldoEsquema = SuspencionEsq;
                    conf.SuspenderCargasSociales = SuspencionCS;
                    conf.DiasSueldo = pagSueldos;
                    conf.DiasCargaSocial = cobCargas;
                    conf.IncidenciasAutomaticas = incidenciasAut;
                    conf.IdEstatus = 1;
                    conf.IdModifica = IdUsuario;
                    conf.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo que obtiene todos los registros patronales del cliente.
        /// </summary>
        /// <param name="IdCliente">Identificador de cliente.</param>
        /// <returns>una lista de objetos de tipo Cat_RegistroPatronal</returns>
        public List<Cat_RegistroPatronal> ObtenerRPByIdCliente(int IdCliente)
        {
            List<Cat_RegistroPatronal> listRegistros = new List<Cat_RegistroPatronal>();

            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                var rpPropios = (from b in entidad.Cat_RegistroPatronal where b.IdCliente == IdCliente && b.IdEstatus == 1 select b).ToList();
                foreach (var item in rpPropios)
                {
                    listRegistros.Add(item);
                }

                var rpAsignados = (from b in entidad.vClienteEmpresaEspecializada
                                   join c in entidad.Cat_RegistroPatronal on b.IdRegistroPatronal equals c.IdRegistroPatronal
                                   where b.IdCliente == IdCliente && b.IdEstatus == 1 && c.IdEstatus == 1
                                   select c).ToList();

                foreach (var item in rpAsignados)
                {
                    listRegistros.Add(item);
                }
            }

            return listRegistros;
        }

        /// <summary>
        /// Obtiene la lista de empleados a los que se le calculara el PTU.
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        public void GetListEmpleadosPTU(int IdPeriodoNomina)
        {
            listEmpleados.Clear();

            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                int[] status = { 1, 2, 3 };
                var _empleados = (from b in entidad.vEmpleados
                                  join c in entidad.vEmpleadosPTU on b.IdEmpleado equals c.IdEmpleado
                                  where c.IdPeriodoNomina == IdPeriodoNomina
                                  select b).ToList();

                listEmpleados = _empleados;
            }
        }

        /// <summary>
        /// Metodo que inserta los registros calculados de PTU en la tabla de incidencias en el concepto especifico.
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema.</param>
        public void InsertaIncidenciasPTU(int IdPeriodoNomina, int IdUsuario)
        {
            string sp = "sp_Nominas_CalculoPTU";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    cmd.Parameters.Add("IdUsuario", SqlDbType.Int).Value = IdUsuario;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

            }
        }

        /// <summary>
        /// Metodo para procesar las vacaciones autorizadas desde el portal de empleados
        /// </summary>
        /// <param name="IdUnidadNegocio">Id de la Unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Id del periodo de nomina</param>
        /// <param name="IdEmpleado">Id del empleado</param>
        /// <param name="IdUsuario">Id del usuario que opera la nomina</param>
        public void ProcesaVacaciones(int IdUnidadNegocio, int IdPeriodoNomina, int? IdEmpleado, int IdUsuario)
        {
            ClassVacaciones cv = new ClassVacaciones();
            int IdConceptoV = cv.IdConceptoVacaciones(IdUnidadNegocio);

            if (IdEmpleado != null)
                cv.ProcesaIncidenciasVacaciones(IdUnidadNegocio, IdPeriodoNomina, (int)IdEmpleado, IdUsuario, IdConceptoV);
            else
                cv.ProcesaIncidenciasVacaciones(IdUnidadNegocio, IdPeriodoNomina, IdUsuario, IdConceptoV);
        }

        /// <summary>
        /// Metodo para que calculo los elementos de Provision para los clientes que lo soliciten
        /// </summary>
        /// <param name="pDiasTrabajados">Dias Trabajados</param>
        /// <param name="IdPrestaciones">ID de las prestaciones del colaborador</param>
        /// <param name="FechaReconocimientoAntiguedad">Fecha de reconocimiento de antiguedad</param>
        /// <param name="FechaAltaIMSS">Fecha de alta IMSS</param>
        /// <exception cref="Exception">Excepcion desde el metodo de Provisión</exception>
        public void ProcesaProvision(decimal? pDiasTrabajados, int? IdPrestaciones, DateTime? FechaReconocimientoAntiguedad, DateTime? FechaAltaIMSS, decimal FactorDiasProvision)
        {
            try
            {
                decimal? _DiasVacaciones = 0;
                decimal? _DiasAguinaldo = 0;
                decimal? _PrimaVacacional = 0;

                decimal? _ProvisionVacaciones = 0;
                decimal? _ProvisionAguinaldo = 0;
                decimal? _ProvisionPrimaVacacional = 0;

                GetPrestacionesEmpleado(IdPrestaciones);
                _fechaReconocimientoAntiguedad = GetFechaIngreso(FechaReconocimientoAntiguedad, FechaAltaIMSS, null);
                Antiguedad = Math.Round((Periodo.FechaFin.Subtract(_fechaReconocimientoAntiguedad).Days) / 365M, 4);

                if (Antiguedad < 0) { throw new Exception("La antiguedad no puede ser negativa: " + Antiguedad); }
                var _factor = prestaciones.Where(x => x.Limite_Superior >= Antiguedad && x.Limite_Inferior <= Antiguedad && x.IdPrestaciones == IdPrestacionesEmpleado).OrderByDescending(x => x.FechaInicioVigencia).FirstOrDefault();

                if (_factor != null)
                {
                    _DiasVacaciones = _factor.Vacaciones;
                    _DiasAguinaldo = _factor.Aguinaldo;
                    _PrimaVacacional = _factor.PrimaVacacional * .1M;
                }

                _ProvisionAguinaldo = ((SD_IMSS * _DiasAguinaldo) / FactorDiasProvision) * pDiasTrabajados;
                _ProvisionVacaciones = ((SD_IMSS * _DiasVacaciones) / FactorDiasProvision) * pDiasTrabajados;
                _ProvisionPrimaVacacional = ((SD_IMSS * _PrimaVacacional) / FactorDiasProvision) * pDiasTrabajados * _DiasVacaciones;

                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    var prov = (from b in entidad.NominaProvision.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                    if (prov != null)
                    {
                        prov.DiasAguinaldo = _DiasAguinaldo;
                        prov.DiasVacaciones = _DiasVacaciones;
                        prov.PorcentajePV = _PrimaVacacional;

                        prov.ProvisionAguinaldo = _ProvisionAguinaldo;
                        prov.ProvisionVacaciones = _ProvisionVacaciones;
                        prov.ProvisionPrimaVacacional = _ProvisionPrimaVacacional;

                        prov.IdEstatus = 1;
                        prov.IdCaptura = IdUsuario;
                        prov.FechaCaptura = DateTime.Now;

                        entidad.SaveChanges();
                    }
                    else
                    {
                        NominaProvision nominaProvision = new NominaProvision()
                        {
                            IdEmpleado = IdEmpleado,
                            IdPeriodoNomina = IdPeriodoNomina,

                            DiasAguinaldo = _DiasAguinaldo,
                            DiasVacaciones = _DiasVacaciones,
                            PorcentajePV = _PrimaVacacional,

                            ProvisionAguinaldo = _ProvisionAguinaldo,
                            ProvisionVacaciones = _ProvisionVacaciones,
                            ProvisionPrimaVacacional = _ProvisionPrimaVacacional,

                            IdEstatus = 1,
                            IdCaptura = IdUsuario,
                            FechaCaptura = DateTime.Now
                        };

                        entidad.NominaProvision.Add(nominaProvision);
                        entidad.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problemas con el cálculo de la provisión en el empleado: " + IdEmpleado + "-" + ClaveEmpleado + ". " + ex.Message);
            }

        }

        public DateTime GetFechaIngreso(DateTime? FechaRecAnt, DateTime? FechaIMSS, string conf)
        {
            DateTime? fechaAlta = null;

            if (conf != null)
            {
                if (conf == "Alta IMSS")
                {
                    if (FechaRecAnt != null)
                        fechaAlta = FechaRecAnt;

                    if (FechaIMSS != null)
                        fechaAlta = FechaIMSS;
                }
                else
                {
                    if (FechaIMSS != null)
                        fechaAlta = FechaIMSS;

                    if (FechaRecAnt != null)
                        fechaAlta = FechaRecAnt;
                }
            }
            else
            {
                if (FechaIMSS != null)
                    fechaAlta = FechaIMSS;

                if (FechaRecAnt != null)
                    fechaAlta = FechaRecAnt;
            }

            return (DateTime)fechaAlta;
        }

        private void GetFactoresCyVIMSS(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int año = fechaFinPeriodo.Year;
                var factoresCyV = (from b in entidad.FactoresCyV_IMSS.Where(x => x.FechaInicio ==
                                   (from c in entidad.FactoresCyV_IMSS.Where(y => y.FechaInicio <= fechaFinPeriodo) select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                   select b).ToList();
                ListFactoresCyV_IMSS = factoresCyV;
            }
        }


        public void ProcesaSaldos(List<vSaldos> saldo, int IdEmpleado, int IdPeriodoNomina, int IdUsuario)
        {
            try
            {
                ClassSaldos cs = new ClassSaldos();

                cs.ProcesaSaldosList(saldo, IdEmpleado, IdPeriodoNomina, IdUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar saldos." + ex.Message);
            }
        }


        public void ProcesoCompensaciones(int IdPeriodoNomina, int IdEmpleado, int IdUsuario)
        {
            try
            {
                ClassIncidencias cl = new ClassIncidencias();
                cCompensaciones au = new cCompensaciones();
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    var ordenes = entidad.vCompensaciones.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeridoNomina == IdPeriodoNomina).Where(x => x.IdEstatus == 1).ToList();
                    var sum = ordenes.Select(c => c.Importe).Sum();
                    var idconcepto = ordenes.Select(c => c.IdConceptoNomina).FirstOrDefault();
                    var id = ordenes.Select(c => c.IdConceptoCompensacion).FirstOrDefault();
                    ModelIncidencias model = new ModelIncidencias();
                    au.DeleteIncidenciaCompensaciones(IdPeriodoNomina, IdEmpleado);

                    model.IdEmpleado = IdEmpleado;
                    model.IdPeriodoNomina = IdPeriodoNomina;
                    model.IdConcepto = (int)idconcepto;
                    model.Monto = sum;
                    model.Observaciones = "PDUP SYSTEM BALLISTIC";
                    model.MontoEsquema = 0;
                    model.BanderaCompensaciones = 1;

                    if (model.IdConcepto != 0)
                        cl.NewIncindencia(model, IdUsuario);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar las compensaciones." + ex.Message);
            }
        }

        public void ProcesoCompensaciones(int IdPeriodoNomina, int IdUsuario)
        {
            try
            {
                cCompensaciones au = new cCompensaciones();
                var select = new List<vCompensaciones>();
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    select = entidad.vCompensaciones.Where(x => x.IdPeridoNomina == IdPeriodoNomina).Where(x => x.IdEstatus == 1).ToList();
                }

                au.ProcesarIncidenciasCompe(select, IdPeriodoNomina, IdUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar compensaciones." + ex.Message);
            }
        }

        public void ProcesaIncidenciasSeptimoDia(List<vEmpleados> empleadosProceso, int IdPeriodo, int IdConcepto, int IdUsuario)
        {
            try
            {
                EliminaIncidenciasSeptimoDia(IdPeriodo, empleadosProceso.Select(x => x.IdEmpleado).ToList(), IdConcepto);
                var incidenciasINc = GetIncidenciasFaltasIncapacidadesPara7moDIa(IdPeriodo, empleadosProceso.Select(x => x.IdEmpleado).ToList());
                var incidenciasHorasINc = GetIncidenciasHorasPara7moDIa(IdPeriodo, empleadosProceso.Select(x => x.IdEmpleado).ToList());

                ClassIncidencias ci = new ClassIncidencias();

                foreach (var item in empleadosProceso)
                {
                    var confEmp = getconfiguracionEmpleadoNomina(item.IdEmpleado);
                    if (confEmp.SupenderSueldoTradicional != 1)
                    {
                        decimal cantidadSeptimoDia = 1;
                        var factorSeptimoDia = 1 / 6M;

                        //operacion para faltas
                        var faltas = incidenciasINc.Where(x => x.ClaveGpo == "500" && x.IdEmpleado == item.IdEmpleado && x.CalculoDiasHoras != "Horas").Select(x => x.Cantidad).Sum() ?? 0M;
                        var factorFaltas = (faltas * factorSeptimoDia);

                        // se obtienen los conceptos que van a restar a los dias trabajados en horas.
                        var diasMenosFraccion = incidenciasHorasINc.Where(x => x.IdEmpleado == item.IdEmpleado).ToList();

                        foreach (var df in diasMenosFraccion)
                        {
                            decimal fraccion = (df.Cantidad ?? 0) * (1M / (6M * (df.SDEntre ?? 1)));
                            factorFaltas += fraccion;
                        }
                        //////

                        //operacion para incapacidades maternidad y riesgo de trabajo
                        string[] clavesIncap = { "01", "03" };
                        var incapacidades = incidenciasINc.Where(x => x.ClaveGpo == "501" && clavesIncap.Contains(x.ClaveSAT) && x.IdEmpleado == item.IdEmpleado).Select(x => x.Cantidad).Sum();
                        if (incapacidades > 0)
                            cantidadSeptimoDia = 0;

                        //operacion para incapacidades enfermedad gral
                        decimal factorIncapacidadGral = 0;
                        var incapacidadesEnfGral = incidenciasINc.Where(x => x.ClaveGpo == "501" && x.ClaveSAT == "02" && x.IdEmpleado == item.IdEmpleado).Select(x => x.Cantidad).ToList();
                        if (incapacidadesEnfGral.Count() > 0)
                            factorIncapacidadGral = ((decimal)incapacidadesEnfGral.Sum() * factorSeptimoDia);

                        cantidadSeptimoDia -= (factorFaltas + factorIncapacidadGral);
                        if (cantidadSeptimoDia < 0) { cantidadSeptimoDia = 0; }

                        if (cantidadSeptimoDia > 0)
                        {
                            ModelIncidencias mi = new ModelIncidencias()
                            {
                                IdEmpleado = item.IdEmpleado,
                                IdPeriodoNomina = IdPeriodo,
                                IdConcepto = IdConcepto,
                                Cantidad = cantidadSeptimoDia,
                                Monto = 0,
                                CantidadEsq = 0,
                                Observaciones = "PDUP SYSTEM",
                                MontoEsquema = 0,
                            };

                            ci.NewIncindencia(mi, IdUsuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar las incidencias de 7mo día.");
            }
        }
               
        public void EliminaIncidenciasSeptimoDia(int IdPeriodoNomina, List<int> IdsEmpleado, int IdConcepto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdConcepto == IdConcepto && x.IdPeriodoNomina == IdPeriodoNomina && IdsEmpleado.Contains((int)x.IdEmpleado)).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public void EliminaIncidenciasSeptimoDiaByIdEmpleado(int IdPeriodoNomina, int IdEmpleado, int IdConcepto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdConcepto == IdConcepto && x.IdPeriodoNomina == IdPeriodoNomina && x.IdEmpleado == IdEmpleado).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public void ProcesoCompensacionesPagos(int IdPeriodoNomina, int IdEmpleado, int IdUsuario)
        {
            ClassIncidencias cl = new ClassIncidencias();
            cCompensaciones au = new cCompensaciones();
            using (TadaBallisticEntities entidad = new TadaBallisticEntities())
            {
                var ordenes = entidad.VPagosServicios.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina).Where(x => x.Expr2 == 1).ToList();

                if (ordenes.Count != 0)
                {
                    var sum = ordenes.Select(c => c.Pago).Sum();
                    var idconcepto = ordenes.Select(c => c.IdConcepto).FirstOrDefault();
                    var id = ordenes.Select(c => c.IdConcepto).FirstOrDefault();
                    ModelIncidencias model = new ModelIncidencias();
                    au.DeleteIncidenciaCompensaciones(IdPeriodoNomina, IdEmpleado);

                    model.IdEmpleado = IdEmpleado;
                    model.IdPeriodoNomina = IdPeriodoNomina;
                    try
                    {
                        model.IdConcepto = (int)idconcepto;

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    model.Monto = sum;
                    model.Observaciones = "PDUP SYSTEM BALLISTIC";
                    model.MontoEsquema = 0;
                    model.BanderaCompensaciones = 1;

                    if (model.IdConcepto != 0)
                        cl.NewIncindencia(model, IdUsuario);
                }
                else
                {
                    au.DeleteIncidenciaCompensaciones(IdPeriodoNomina, IdEmpleado);

                }


            }

        }

        public void ProcesoCompensacionesPagos(int IdPeriodoNomina, int IdUsuario)
        {
            cCompensaciones au = new cCompensaciones();
            var select = new List<TadaNomina.Models.DB.VPagosServicios>();
            using (TadaBallisticEntities entidad = new TadaBallisticEntities())
            {
                select = entidad.VPagosServicios.Where(x => x.IdPeriodoNomina == IdPeriodoNomina).Where(x => x.IdEstatus == 1).ToList();
            }

            if(select.Count != 0)
            {
                au.ProcesarIncidenciasPagos(select, IdPeriodoNomina, IdUsuario);

            }
        }

        public bool ValidaCobroCOPS(int? IdPrestaciones, DateTime? FechaReconocimientoAntiguedad, DateTime? FechaAltaIMSS, decimal SDIMSS_Actual, decimal SDI_Actual)
        {
            bool resultado = false;
            decimal _SDI_Limite = 0;

            GetPrestacionesEmpleado(IdPrestaciones);
            _fechaReconocimientoAntiguedad = GetFechaIngreso(FechaReconocimientoAntiguedad, FechaAltaIMSS, null);
            Antiguedad = Math.Round((Periodo.FechaFin.Subtract(_fechaReconocimientoAntiguedad).Days) / 365M, 4);

            var _factor = prestaciones.Where(x => x.Limite_Superior >= Antiguedad && x.Limite_Inferior <= Antiguedad && x.IdPrestaciones == IdPrestacionesEmpleado).OrderByDescending(x => x.FechaInicioVigencia).FirstOrDefault();
            if (_factor != null)
            {
                _SDI_Limite= Math.Round(SDIMSS_Actual*(decimal)_factor.FactorIntegracion,2);

                if (_SDI_Limite < SDI_Actual) { resultado = true; }                
            }

            return resultado;
        }

        public void CalculaCuotasSindicales(int IdPeriodoNomina, int IdEmpleado, int idconceptoCuota, int idconceptoVacaciones, decimal? diasLaborados, decimal SDIMSS, int IdUsuario)
        {

            string sp = "SP_CalculaCuotasSindicales";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdEmpleado", SqlDbType.Int).Value = IdEmpleado;
                    cmd.Parameters.Add("IdConcepto", SqlDbType.Int).Value = idconceptoCuota;
                    cmd.Parameters.Add("IdConceptoVacaciones", SqlDbType.Int).Value = idconceptoVacaciones;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    cmd.Parameters.Add("DiasLaborados", SqlDbType.Decimal).Value = diasLaborados;
                    cmd.Parameters.Add("SDIMSS", SqlDbType.Decimal).Value = SDIMSS;
                    cmd.Parameters.Add("IdCaptura", SqlDbType.Int).Value = IdUsuario;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

            }
        }

    }
}
