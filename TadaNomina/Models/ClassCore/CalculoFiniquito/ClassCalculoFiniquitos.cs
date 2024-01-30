using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.CalculoFiniquito
{
    public class ClassCalculoFiniquitos : ClassProcesosFiniquitos
    {
        /// <summary>
        ///     Método que se usa para procesar el finiquito del empleado.
        /// </summary>
        /// <param name="_IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="_IdEmpledao">Identificador del empleado</param>
        /// <param name="_IdUsuario">identificador del usuario de la sesión activa</param>
        /// <exception cref="Exception"></exception>
        public void Procesar(int _IdPeriodoNomina, int? _IdEmpledao, int _IdUsuario)
        {
            ClassIncidencias cincidencias = new ClassIncidencias();
            contador = 0;
            IdPeriodoNomina = _IdPeriodoNomina;            
            IdUsuario = _IdUsuario;

            GetListas(IdPeriodoNomina, _IdEmpledao, _IdUsuario);
            GetIncidencias(_IdPeriodoNomina);
            GetConfiguracionFiniquitos(IdPeriodoNomina);
            ObtenConfiguracionFiniquitoGral();
            GetConceptosFiniquitos();

            List<vEmpleados> empleadosProceso = listEmpleados.Where(x=> ListConfiguracionFiniquito.Select(y=>y.IdEmpleado).Contains(x.IdEmpleado)).ToList();


            if (_IdEmpledao != null)
            {
                empleadosProceso = listEmpleados.Where(x => x.IdEmpleado == _IdEmpledao).ToList();
                cincidencias.DeleteAllEmpleadoFinquitos(_IdPeriodoNomina, (int)_IdEmpledao);                
            }
            else
            {                
                cincidencias.DeleteAllFiniquitos(_IdPeriodoNomina, empleadosProceso.Select(x=> x.IdEmpleado).ToList());
            }
            
            foreach (var item in empleadosProceso)
            {
                nominaTrabajo = new NominaTrabajo();
                IdEmpleado = item.IdEmpleado;
                IdPrestacionesEmpleado = item.IdPrestaciones;
                SDI = GetSD(item.SDI);
                SD_IMSS = GetSD(item.SDIMSS);
                SD_Real = GetSD(item.SD);
                AniosEsquema = 0;
                nominaTrabajo.FechaAltaIMSS = item.FechaAltaIMSS;
                nominaTrabajo.FechaReconocimientoAntiguedad = item.FechaReconocimientoAntiguedad;

                configuracionFiniquito = ListConfiguracionFiniquito.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == _IdPeriodoNomina && x.IdEstatus == 1).FirstOrDefault();                                
                try { _fechaBaja = (DateTime)configuracionFiniquito.FechaBajaFin; } catch { try { _fechaBaja = (DateTime)item.FechaBaja; } catch { _fechaBaja = DateTime.Now; } }

                if (UnidadNegocio.FiniquitosFechasDiferentes == "S")               
                    if (item.FechaAltaIMSS != null) { _fechaReconocimientoAntiguedad = (DateTime)item.FechaAltaIMSS; } else { _fechaReconocimientoAntiguedad = (DateTime)item.FechaReconocimientoAntiguedad; }
                else
                    if (item.FechaReconocimientoAntiguedad != null) { _fechaReconocimientoAntiguedad = (DateTime)item.FechaReconocimientoAntiguedad; } else { _fechaReconocimientoAntiguedad = (DateTime)item.FechaAltaIMSS; }

                nominaTrabajo.FechaBaja = _fechaBaja;                                
                _FechaInicioEjercicio = ObtenIncioEjercicio(Periodo.FechaFin, _fechaReconocimientoAntiguedad, _fechaBaja);
                nominaTrabajo.Anios = Math.Round(ObtenAntiguedadEmpleado(_fechaReconocimientoAntiguedad, _fechaBaja), 2);
                if (nominaTrabajo.Anios < 0) { throw new Exception("La fecha de alta es superior a la de baja: Empleado: " + item.ClaveEmpleado + " - " + item.NombreCompleto + " alta:" + _fechaReconocimientoAntiguedad + " - Baja:" + _fechaBaja); }
                AntiguedadGeneradaUltimoAño = Math.Round(ObtenUltimoPeriodoCalculo_Neto(_fechaReconocimientoAntiguedad, _fechaBaja), 6);

                if (UnidadNegocio.FiniquitosFechasDiferentes == "S")
                {
                    if (item.FechaReconocimientoAntiguedad != null) { _fechaReconocimientoAntiguedadEsquema = (DateTime)item.FechaReconocimientoAntiguedad; } else { _fechaReconocimientoAntiguedadEsquema = (DateTime)item.FechaAltaIMSS; }
                    AniosEsquema = Math.Round(ObtenAntiguedadEmpleado(_fechaReconocimientoAntiguedadEsquema, _fechaBaja), 2);
                    if (AniosEsquema < 0) { throw new Exception("La fecha de alta en el es superior a la de baja: Empleado: " + item.ClaveEmpleado + " - " + item.NombreCompleto + " alta:" + _fechaReconocimientoAntiguedadEsquema + " - Baja:" + _fechaBaja); }
                    AntiguedadGeneradaUltimoAñoEsquema = Math.Round(ObtenUltimoPeriodoCalculo_Neto(_fechaReconocimientoAntiguedadEsquema, _fechaBaja), 6);
                }

                DiasTrabajadosEjercicio = _fechaBaja.Subtract(_FechaInicioEjercicio).Days + 1;
                FactorDiasTrabajadosEjercicio = Math.Round(DiasTrabajadosEjercicio / 365M, 6);

                incidenciasEmpleado = listIncidencias.Where(x => x.IdEmpleado == item.IdEmpleado).ToList();

                CalculaVacaciones();
                CalculaPrimaVacacional();
                CalculaAguinaldo();
                CalculaLiquidacion();
                GetIncidenciasEmpleado(IdPeriodoNomina, IdEmpleado);
                incidenciasEmpleado = listIncidencias;
                configuracionNominaEmpleado = getconfiguracionEmpleadoNomina(IdEmpleado);

                Porcesa_Nomina_Tradicional(item);
                Procesa_Nomina_Esquema(item);

                DeleteRegistroNominaTrabajo();
                if (ValidaInsercionRegistro())
                {
                    GuardarNominaTrabajoFiniquitos(item, IdPeriodoNomina);
                }
                contador++;
            }
        }

        /// <summary>
        ///     Método para el proceso de nómina tradicional
        /// </summary>
        /// <param name="item">Hace referencia a la Vista Empleados de la Base de datos</param>
        private void Porcesa_Nomina_Tradicional(vEmpleados item)
        {            
            try { Porcentaje_Riesgo_Trabajo_Patronal = ListRegistroPatronal.Where(x => x.IdRegistroPatronal == item.IdRegistroPatronal).Select(x => x.RiesgoTrabajo).FirstOrDefault(); }
            catch { Porcentaje_Riesgo_Trabajo_Patronal = 0; }

            DiasPago = 0;
            DiasTrabajados_IMSS = 0;


            nominaTrabajo.FactorIntegracion = prestaciones.Where(x => x.IdPrestaciones == item.IdPrestaciones).Select(x => x.FactorIntegracion).FirstOrDefault();
            if (nominaTrabajo.FactorIntegracion == null)
                nominaTrabajo.FactorIntegracion = prestaciones.Where(x => x.IdCliente == 0).Select(x => x.FactorIntegracion).FirstOrDefault();

            GetDiasTrabajados();

            nominaTrabajo.SueldoPagado = 0;
            nominaTrabajo.SueldoPagado += nominaTrabajo.DiasTrabajados * SD_IMSS; 

            nominaTrabajo.ER = (decimal)nominaTrabajo.SueldoPagado;
            nominaTrabajo.ER += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER").Select(X => X.Monto).Sum();

            CalculaBaseGravadaLiquidacion();            
            CalculaISR();
            Calcula_Cuotas_Obreras(null);


            nominaTrabajo.ER += nominaTrabajo.SubsidioPagar;
            nominaTrabajo.ER += nominaTrabajo.ReintegroISR;

            CalculaISN();

            nominaTrabajo.DD = 0;
            nominaTrabajo.DD += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "DD").Select(X => X.Monto).Sum();
            nominaTrabajo.DD += nominaTrabajo.ImpuestoRetener;
            nominaTrabajo.DD += nominaTrabajo.ISRLiquidacion;
            nominaTrabajo.DD += nominaTrabajo.IMSS_Obrero;

            nominaTrabajo.Neto = 0;
            nominaTrabajo.Neto = nominaTrabajo.ER - nominaTrabajo.DD;
        }

        /// <summary>
        ///     Método para el proceso de nómina por esquema
        /// </summary>
        /// <param name="item">Variable que contiene los datos necesarios </param>
        private void Procesa_Nomina_Esquema(vEmpleados item)
        {
            nominaTrabajo.Apoyo = 0;
            nominaTrabajo.ERS = 0;
            nominaTrabajo.DDS = 0;
            nominaTrabajo.Netos = 0;
            SD_Esquema = 0;
            decimal NetoPagar = 0;
            //try { NetoPagar = (decimal)item.NetoPagar; } catch { }

            if (NetoPagar > 0)
            {
                nominaTrabajo.Apoyo = NetoPagar - (decimal)nominaTrabajo.Neto;
            }
            else
            {
                SD_Esquema = SD_Real - SD_IMSS;

                
                nominaTrabajo.Apoyo += nominaTrabajo.DiasTrabajados * SD_Esquema;
                decimal apoyoVacacional = (decimal)nominaTrabajo.Dias_Vacaciones * SD_Esquema;
                nominaTrabajo.ERS += apoyoVacacional;
                

                nominaTrabajo.ERS += nominaTrabajo.Apoyo;
                nominaTrabajo.ERS += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaS.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.ClaveGpo != "002").Select(X => X.MontoEsquema).Sum();

                nominaTrabajo.DDS += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaS.Contains(x.TipoEsquema) && x.TipoConcepto == "DD").Select(X => X.MontoEsquema).Sum();
                nominaTrabajo.Netos = nominaTrabajo.ERS - nominaTrabajo.DDS;
            }
        }

        /// <summary>
        ///     Método que obtiene los días que han trabajado los empleados restando las ausencias o faltas que tenga 
        /// </summary>
        private void GetDiasTrabajados()
        {
            nominaTrabajo.Faltas = 0;
            nominaTrabajo.Incapacidades = 0;

            nominaTrabajo.Faltas = (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.ClaveGpo == "500").Select(X => X.Cantidad).Sum();
            nominaTrabajo.Incapacidades = (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.ClaveGpo == "501").Select(X => X.Cantidad).Sum();

            nominaTrabajo.DiasTrabajados = DiasPago;
            nominaTrabajo.DiasTrabajados += (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "ER" && x.AfectaSeldo == "SI").Select(X => X.Cantidad).Sum();
            nominaTrabajo.DiasTrabajados -= (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "DD" && x.AfectaSeldo == "SI").Select(X => X.Cantidad).Sum();
            nominaTrabajo.DiasTrabajados -= (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoDato == "Cantidades" && x.TipoConcepto == "ER" && x.ClaveGpo == "002" && x.AfectaSeldo == "SI").Select(X => X.Cantidad).Sum();
            
            if (nominaTrabajo.DiasTrabajados < 0) { nominaTrabajo.DiasTrabajados = 0;  }            
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="FechaEjercicio"></param>
        /// <param name="FechaReconocimiento"></param>
        /// <param name="FechaBaja"></param>
        /// <returns></returns>
        public DateTime ObtenIncioEjercicio(DateTime FechaEjercicio, DateTime FechaReconocimiento, DateTime FechaBaja)
        {
            DateTime FechaInicio = DateTime.Parse("01/01/" + FechaEjercicio.Year);
            if (FechaBaja < FechaEjercicio)
            {
                FechaInicio = DateTime.Parse("01/01/" + FechaBaja.Year);
            }

            int days = FechaInicio.Subtract(FechaReconocimiento).Days;

            if (days < 0)
                FechaInicio = FechaReconocimiento;

            return FechaInicio;
        }

        /// <summary>
        ///     Obtiene el tiempo de antigüedad que lleva el empleado
        /// </summary>
        /// <param name="FechaIngreso">Variable que contiene la fecha en que ingresó el empleado a la empresa</param>
        /// <param name="FechaBaja">Variable que contiene la fecha en la que el empleado dejó de trabajar en la empresa</param>
        /// <returns>Regresa el timepo de antigüedad en años</returns>
        /// <exception cref="Exception"></exception>
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
        ///     Método que obtiene los dias que el empleado lleva trabajando durante el año
        /// </summary>
        /// <param name="FechaAlta">Variable que contiene la fecha en la que el empleado se dio de alta en la empresa</param>
        /// <param name="FechaBaja">Variable que contiene la fecha en la que el empleado se dio de baja en la empresa</param>
        /// <returns>Regresa la cantidad de días que lleva el empleado en la empresa</returns>
        /// <exception cref="Exception"></exception>
        public decimal ObtenUltimoPeriodoCalculo_Neto(DateTime FechaAlta, DateTime FechaBaja)
        {
            try
            {
                DateTime FechaIncio = DateTime.Parse(FechaAlta.Day + "/" + FechaAlta.Month + "/" + FechaBaja.Year);
                int numeroDias = FechaBaja.Subtract(FechaIncio).Days;
                if (numeroDias < 0)
                {
                    FechaIncio = FechaIncio.AddYears(-1);
                    numeroDias = FechaBaja.Subtract(FechaIncio).Days;
                }

                numeroDias += 1;
                decimal antiguedadUltimoAño = numeroDias / 365M;

                return antiguedadUltimoAño;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        private void CalculaVacaciones()
        {
            nominaTrabajo.Dias_Vacaciones = 0;
            DiasVacacionesEsquema = 0;
            DiasVacacionesFactorIntegracion = ObtenDiasVacacionesPorFactorIntegracion((decimal)nominaTrabajo.Anios, IdPrestacionesEmpleado);
            int? IdConceptoVac = 0;
            try { IdConceptoVac = conceptosFiniquitos.IdConceptoVacaciones; } catch { IdConceptoVac = 0; }

            if (IdConceptoVac != 0)
            {
                if (configuracionFiniquito.BanderaVac != null && configuracionFiniquito.BanderaVac == 1)
                {
                    try { nominaTrabajo.Dias_Vacaciones = (decimal)incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.ClaveConcepto == conceptosFiniquitos.ClaveConceptoVac).Select(X => X.Cantidad).Sum(); } catch { nominaTrabajo.Dias_Vacaciones = 0;  }
                    int? Id = null;
                    try { Id = incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.ClaveConcepto == conceptosFiniquitos.ClaveConceptoVac).Select(x=> x.IdIncidencia).FirstOrDefault(); } catch { }
                                        
                    UpdateGravadosExentos(Id, SD_IMSS);

                    nominaTrabajo.Sueldo_Vacaciones = nominaTrabajo.Dias_Vacaciones * SD_IMSS;
                }
                else
                {                   
                    nominaTrabajo.Dias_Vacaciones = Math.Round((AntiguedadGeneradaUltimoAño * DiasVacacionesFactorIntegracion), 14);
                    nominaTrabajo.Sueldo_Vacaciones = nominaTrabajo.Dias_Vacaciones * SD_IMSS;
                    DiasVacacionesEsquema = (decimal)nominaTrabajo.Dias_Vacaciones;

                    if (AniosEsquema > 0)
                    {
                        DiasVacacionesFactorIntegracionEsquema = ObtenDiasVacacionesPorFactorIntegracion(AniosEsquema, IdPrestacionesEmpleado);
                        DiasVacacionesEsquema = Math.Round((AntiguedadGeneradaUltimoAñoEsquema * DiasVacacionesFactorIntegracionEsquema), 14);
                    }

                    if (nominaTrabajo.Dias_Vacaciones > 0 && IdConceptoVac != null)
                    {
                        CrearIncidencias((int)IdConceptoVac, (decimal)nominaTrabajo.Dias_Vacaciones, DiasVacacionesEsquema, 0, null, null, null, FactorDiasTrabajadosEjercicio);
                    }
                }
            }            
        }

        /// <summary>
        ///     Calcula la prima vacacional del empleado.
        /// </summary>
        private void CalculaPrimaVacacional()
        {
            PorcentajePVFactorIntegracion = 0;
            PorcentajePVFactorIntegracion = ObtenPorcentajePVPorFactorIntegracion((decimal)nominaTrabajo.Anios, IdPrestacionesEmpleado);
            int? IdConceptoPV = 0;
            try { IdConceptoPV = conceptosFiniquitos.IdConceptoPV; } catch { IdConceptoPV = 0; }

            if (IdConceptoPV != 0)
            {
                if (configuracionFiniquito.BanderaPV != null && configuracionFiniquito.BanderaPV == 1)
                {
                    int? Id = null;
                    try { Id = incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.ClaveConcepto == conceptosFiniquitos.ClaveConceptoPV).Select(X => X.IdIncidencia).FirstOrDefault(); } catch { }
                    UpdateGravadosExentos(Id, SD_IMSS);
                }
                else
                {
                    if (nominaTrabajo.Dias_Vacaciones > 0 && IdConceptoPV != null)
                    {
                        CrearIncidencias((int)IdConceptoPV, (decimal)nominaTrabajo.Dias_Vacaciones, DiasVacacionesEsquema, 0, PorcentajePVFactorIntegracion, null, null, AntiguedadGeneradaUltimoAño);
                    }
                }
            }
        }

        /// <summary>
        ///     Calcula el aguinaldo correspondiente a cada empleado
        /// </summary>
        private void CalculaAguinaldo()
        {
            DiasAguinaldoFactorIntegracion = 0;
            DiasAguinaldoFactorIntegracionEsquema = 0;
            DiasAguinaldoFactorIntegracion = ObtenDiasAguinaldoPorFactorIntegracion((decimal)nominaTrabajo.Anios, IdPrestacionesEmpleado);
            decimal _FactorAguinaldo = 0;
            decimal _FactorAguinaldoEsquema = 0;
            int? IdConceptoAguinaldo = 0;
            if (conceptosFiniquitos.IdConceptoAguinaldo != null)
                IdConceptoAguinaldo = conceptosFiniquitos.IdConceptoAguinaldo;
            
            if (IdConceptoAguinaldo != 0)
            {
                if (configuracionFiniquito.BanderaAguinaldo != null && configuracionFiniquito.BanderaAguinaldo == 1)
                {
                    int? Id = null;
                    try { Id = incidenciasEmpleado.Where(x => _tipoEsquemaT.Contains(x.TipoEsquema) && x.TipoConcepto == "ER" && x.ClaveConcepto == conceptosFiniquitos.ClaveConceptoAguinaldo).Select(X => X.IdIncidencia).FirstOrDefault(); } catch { }
                    UpdateGravadosExentos(Id, SD_IMSS);
                }
                else
                {
                                      
                    _FactorAguinaldo = Math.Round(FactorDiasTrabajadosEjercicio * DiasAguinaldoFactorIntegracion, 6);
                    _FactorAguinaldoEsquema = _FactorAguinaldo;

                    if (AniosEsquema > 0)
                    {
                        DiasAguinaldoFactorIntegracionEsquema = ObtenDiasAguinaldoPorFactorIntegracion(AniosEsquema, IdPrestacionesEmpleado);
                        _FactorAguinaldoEsquema = Math.Round(AntiguedadGeneradaUltimoAñoEsquema * DiasAguinaldoFactorIntegracionEsquema, 6);
                    }

                    if (IdConceptoAguinaldo != null)
                    {
                        CrearIncidencias((int)IdConceptoAguinaldo, _FactorAguinaldo, _FactorAguinaldoEsquema, 0, null, null, null, FactorDiasTrabajadosEjercicio);
                    }
                }
            }
        }

        /// <summary>
        ///     Calcula la cantidad con la que se va a liquidar al empleado
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CalculaLiquidacion()
        {
            
            if (configuracionFiniquito.BanderaLiquidacion != null && configuracionFiniquito.BanderaLiquidacion == 1)
            {
                var sdCalculo = configuracionFiniquito.LiquidacionSDI == 1 ? SDI : SD_IMSS;                
                ImporteParaSumarAlSDI = 0;
                diasPeriodo = 1;                
                factorIntegraciosSDILiquidacion = UnidadNegocio.GenerarIntegradoPVyAgui == "S" ? (DiasAguinaldoFactorIntegracion / 365M) + ((DiasVacacionesFactorIntegracion * PorcentajePVFactorIntegracion) / 365M) + 1 : 1;
                getImporteIntegraSDI(clves, periodoInt);
                getDiasPeriodos(periodoInt);
                var importesumar = ImporteParaSumarAlSDI / diasPeriodo;
                sdCalculo = sdCalculo * (decimal)factorIntegraciosSDILiquidacion;
                sdCalculo += importesumar ?? 0;
                nominaTrabajo.SDI_Liquidacion = 0;
                nominaTrabajo.SDI_Liquidacion = sdCalculo;

                int? IdConceptoI90d = 0;
                if (conceptosFiniquitos.IdConceptoIndem3M != null)
                    IdConceptoI90d = conceptosFiniquitos.IdConceptoIndem3M;
                else
                    throw new Exception("No se puede calcular el concepto 'Indemnizacion 3 meses' porque no hay ningun concepto configurado");
                
                if (IdConceptoI90d != 0)
                {
                    if (configuracionFiniquito.Bandera90d == null || configuracionFiniquito.Bandera90d != 1)
                    {
                        CrearIncidencias((int)IdConceptoI90d, 90, 90, 0, null, sdCalculo, null, AntiguedadGeneradaUltimoAño);
                    }
                }

                int? IdConcepto20d = 0;
                if(conceptosFiniquitos.IdConceptoIndem20D != null)
                    IdConcepto20d = conceptosFiniquitos.IdConceptoIndem20D;
                else
                    throw new Exception("No se puede calcular el concepto '20 dias por año' porque no hay ningun concepto configurado");

                if (IdConcepto20d != 0)
                {
                    if (configuracionFiniquito.Bandera20d == null || configuracionFiniquito.Bandera20d != 1)
                    {
                        decimal factor20d = (decimal)nominaTrabajo.Anios * 20M;
                        decimal factor20dEsq = factor20d;

                        if(AniosEsquema > 0)
                            factor20dEsq = AniosEsquema * 20M;

                        CrearIncidencias((int)IdConcepto20d, factor20d, factor20dEsq, 0, null, sdCalculo, null, FactorDiasTrabajadosEjercicio);
                    }
                }

                int? IdConceptoPA = 0;
                if (conceptosFiniquitos.IdConceptoIndemPA != null)
                    IdConceptoPA = conceptosFiniquitos.IdConceptoIndemPA;
                else
                    throw new Exception("No se puede calcular el concepto 'Prima de Antiguedad' porque no hay ningun concepto configurado");
                
                if (IdConceptoPA != 0)
                {
                    if (configuracionFiniquito.BanderaPA == null || configuracionFiniquito.BanderaPA != 1)
                    {
                        decimal TopePA = (decimal)SueldosMinimos.SalarioMinimoGeneral * 2;
                        decimal SDCalculo = sdCalculo;
                        decimal? SDCalculoReal = SD_Real;

                        if (sdCalculo > TopePA)
                            SDCalculo = TopePA;

                        if (SD_Real > TopePA)
                            SDCalculoReal = TopePA;

                        decimal factorPA = (decimal)nominaTrabajo.Anios * 12M;
                        decimal factorPAEsq = factorPA;

                        if (AniosEsquema > 0)
                            factorPAEsq = AniosEsquema * 12;

                        if (SDCalculoReal == 0)
                            SDCalculoReal = null;

                        CrearIncidencias((int)IdConceptoPA, factorPA, factorPAEsq, 0, null, SDCalculo, SDCalculoReal, FactorDiasTrabajadosEjercicio);
                    }
                }
            }                
        }

        /// <summary>
        ///  Calcula la base gravada d los conceptos que involucran la liquidación.   
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CalculaBaseGravadaLiquidacion()
        {
            nominaTrabajo.BaseGravadaLiquidacion = 0;
            decimal indem90d = 0;
            decimal indem20d = 0;
            decimal indemPA = 0;
            decimal TotalLiquidacion = 0;
            int? IdConceptoI90d = null;
            int? IdConcepto20d = null;  
            int? IdConceptoPA = null; 
            List<int?> idsLiquidacion = new List<int?>();
            
            try { IdConceptoI90d = conceptosFiniquitos.IdConceptoIndem3M; idsLiquidacion.Add(IdConceptoI90d); } catch { throw new Exception("falta configurar conceptos de liquidacion."); }
            try { IdConcepto20d = conceptosFiniquitos.IdConceptoIndem20D; idsLiquidacion.Add(IdConcepto20d); } catch { throw new Exception("falta configurar conceptos de liquidacion."); }
            try { IdConceptoPA = conceptosFiniquitos.IdConceptoIndemPA; idsLiquidacion.Add(IdConceptoPA); } catch { throw new Exception("falta configurar conceptos de liquidacion."); }

            indem90d = (decimal)incidenciasEmpleado.Where(x => x.IdConcepto == IdConceptoI90d).Select(x => x.Monto).Sum();
            indem20d = (decimal)incidenciasEmpleado.Where(x => x.IdConcepto == IdConcepto20d).Select(x => x.Monto).Sum();
            indemPA = (decimal)incidenciasEmpleado.Where(x => x.IdConcepto == IdConceptoPA).Select(x => x.Monto).Sum();

            TotalLiquidacion = indem90d + indem20d + indemPA;
            decimal _ExentoTotal = ((decimal)SueldosMinimos.UMA * 90M) * (decimal)nominaTrabajo.Anios;
            decimal ExentoLiquidacion = 0;
            decimal GravadoLiquidacion = TotalLiquidacion;

            if (_ExentoTotal >= TotalLiquidacion)
            {
                ExentoLiquidacion = TotalLiquidacion;
                GravadoLiquidacion = 0;
            }
            else
            {
                GravadoLiquidacion = TotalLiquidacion - _ExentoTotal;
                ExentoLiquidacion = _ExentoTotal;
            }

            nominaTrabajo.BaseGravadaLiquidacion = GravadoLiquidacion;
            nominaTrabajo.TotalLiquidacion = TotalLiquidacion;
            nominaTrabajo.ExentoLiquidacion = ExentoLiquidacion;            
            
            if ((indem90d > 0 && indem20d > 0 && indemPA > 0) || (indem90d > 0 && indem20d > 0) || (indem20d > 0 && indemPA > 0) || (indem90d > 0 && indemPA > 0))
            {
                decimal SMO = SD_IMSS * 30;
                decimal ISR_SMO = CalculaISR(SMO, Periodo.FechaFin, "05", false);
                decimal Factor = Math.Round(ISR_SMO / SMO, 6);
                nominaTrabajo.ISRLiquidacion = nominaTrabajo.BaseGravadaLiquidacion * Factor;
            }
            else
                nominaTrabajo.ISRLiquidacion = CalculaISR((decimal)nominaTrabajo.BaseGravadaLiquidacion, Periodo.FechaFin, "05", false);

            updateExentosGravadosLiquidacion(idsLiquidacion.ToArray());
        }

        /// <summary>
        /// se actualizan los gravados y excentos en los conceptos de liquidación de forma proporcional.
        /// </summary>
        /// <param name="indem90d">importe del concepto 3 meses de indemnización</param>
        /// <param name="indem20d">importe del copncepto 20 dias por año</param>
        /// <param name="indemPA">importe del concepto prima de antiguedad</param>
        /// <param name="TotalLiquidacion">importe total de la liquidación</param>
        /// <param name="ExentoLiquidacion">importe total exento de la liquidación</param>
        public void updateExentosGravadosLiquidacion(decimal indem90d, decimal indem20d, decimal indemPA, decimal TotalLiquidacion, decimal ExentoLiquidacion)
        {
            decimal cienPorciento = 100;
            decimal porcentajeindem90d = ((indem90d * cienPorciento) / TotalLiquidacion) * 0.01M;
            decimal porcentajeindem20d = ((indem20d * cienPorciento) / TotalLiquidacion) * 0.01M;
            decimal porcentajeindemPA = ((indemPA * cienPorciento) / TotalLiquidacion) * 0.01M;            
        }

        public void updateExentosGravadosLiquidacion(int?[] idsConceptosLiquidacion)
        {
            List<vIncidencias> incidenciasLiquidacion = incidenciasEmpleado.Where(x => idsConceptosLiquidacion.Contains((int)x.IdConcepto)).ToList();

            decimal cienPorciento = 100;
            foreach (var i in incidenciasLiquidacion)
            { 
                decimal porcentajeIndem = (((i.Monto ?? 0) * cienPorciento) / (nominaTrabajo.TotalLiquidacion ?? 1 )) * 0.01M;
                decimal exento = Math.Round(porcentajeIndem * (nominaTrabajo.ExentoLiquidacion ?? 1), 2);
                decimal gravado = (i.Monto ?? 0) - exento;

                updateExentosGravadoLiquidacion(i.IdIncidencia, exento, gravado);
            }
        }

        public void updateExentosGravadoLiquidacion(int IdIncidencia, decimal Exento, decimal Gravado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = entidad.Incidencias.Where(x => x.IdIncidencia == IdIncidencia).FirstOrDefault();

                if (incidencia != null)
                {
                    incidencia.Exento = Exento;
                    incidencia.Gravado = Gravado;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Calcula el impuesto sobre la nómina
        /// </summary>
        public new void CalculaISN()
        {
            decimal porcentaje = 0;
            decimal incidenciasNOISN = 0;
            try { porcentaje = (decimal)listEntidades.Where(x => x.Id == (int)IdEntidadFederativa).Select(x => x.ISN).First() * 0.01M; } catch { }
            if (porcentaje == 0) { try { porcentaje = (decimal)UnidadNegocio.PorcentajeISN * 0.01M; } catch { porcentaje = 0; } }

            var reintISR = nominaTrabajo.ReintegroISR ?? 0;
            incidenciasNOISN = (decimal)incidenciasEmpleado.Where(x => x.TipoConcepto == "ER" && x.IntegraISN == "NO").Select(X => X.Monto).Sum();
            decimal totalPercepcionesSinSubsidio = (decimal)(nominaTrabajo.ER - (nominaTrabajo.SubsidioPagar + reintISR) - incidenciasNOISN);
            nominaTrabajo.ISN = totalPercepcionesSinSubsidio * porcentaje; 

            if (Periodo.TipoNomina == "PTU")
            {
                nominaTrabajo.ISN = 0;
            }
        }

        /// <summary>
        ///     Genera las incidencias en un periodo de nómina determinado
        /// </summary>
        /// <param name="IdConcepto">Variable que contiene el concepto con el cual se va a generar la incidencia</param>
        /// <param name="Cantidad">Variable que contiene las cantidades dentro del mismo registro de incidencia</param>
        /// <param name="CantidadEsquema"></param>
        /// <param name="Monto">Variable que contiene el monto a descontar o agregar en la nómina</param>
        /// <param name="Porcentaje"></param>
        /// <param name="SDCalculo"></param>
        /// <param name="SDCalculoReal"></param>
        public void CrearIncidencias(int IdConcepto, decimal Cantidad, decimal CantidadEsquema, decimal Monto, decimal? Porcentaje, decimal? SDCalculo, decimal? SDCalculoReal, decimal FactorDiasTrabajadosEjercicio)
        {
            ClassIncidencias cincidencias = new ClassIncidencias();

            if (UnidadNegocio.FiniquitosExentoCompleto == "S")
                cincidencias.NewIncindencia(IdPeriodoNomina, IdConcepto, IdEmpleado, Cantidad, CantidadEsquema, Monto, 0, null, null, null, null, 1, null, null, null, Porcentaje, SDCalculo, SDCalculoReal, IdUsuario);
            else
            {
                cincidencias.NewIncindencia(IdPeriodoNomina, IdConcepto, IdEmpleado, Cantidad, CantidadEsquema, Monto, 0, null, null, null, null, 1, null, null, FactorDiasTrabajadosEjercicio, Porcentaje, SDCalculo, SDCalculoReal, IdUsuario);

            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="IdIncidencia"></param>
        public void UpdateGravadosExentos(int? IdIncidencia, decimal? SDIMSS)
        {
            if (UnidadNegocio.FiniquitosExentoCompleto != "S")
            {
                ClassIncidencias cincidencias = new ClassIncidencias();
                if (IdIncidencia != null)
                    cincidencias.UpdateExentosGravados((int)IdIncidencia, FactorDiasTrabajadosEjercicio, Periodo.FechaFin, SDIMSS);
            }
        }
    }
}