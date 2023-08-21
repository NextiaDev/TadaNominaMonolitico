using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore.CalculoAguinaldo
{
    /// <summary>
    /// Calculo de Aguinaldo
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 16/05/2022, Razón: Documentar codigo.
    /// </summary>
    public class ClassProcesosAguinaldo: ClassCalculoImpuestos
    {
        /// <summary>
        /// Metodo que calcula la nomina cuando se trata de un periodo de aguinaldo
        /// </summary>
        /// <param name="p">Objeto de tipo PeriodoNomina.</param>
        /// <param name="e">Listado de empleados.</param>
        /// <param name="IdCliente">Identificador del cliente.</param>
        /// <param name="IdUsuario">Identificador del usuario del sistema.</param>
        /// <exception cref="Exception">Excepcion devuelta por el metodo en caso de un error.</exception>
        public void CalculaAguinaldos(PeriodoNomina p, List<vEmpleados> e, int IdCliente, int IdUsuario)
        {
            ClassIncidencias cincidencias = new ClassIncidencias();
            cincidencias.DeleteIncidenciasAguinaldo(p.IdPeriodoNomina, e.Select(x => x.IdEmpleado).ToArray());
            Carga_Lista_Faltas(p.IdUnidadNegocio, p.FechaFin.Year);

            // Obtenemos la configuracion para el aguinaldo
            ClassFechasCalculos cf = new ClassFechasCalculos();
            configuracionFechas = cf.GetConfiguracionFechas(p.IdUnidadNegocio); 
            string conf_fecha = null;

            if (configuracionFechas != null)
            {
                conf_fecha = configuracionFechas.FAguinaldoReal;
            }

            ClassConceptosFiniquitos cfiniquitos = new ClassConceptosFiniquitos();
            int DiasTotalesPeriodo = p.FechaFin.Subtract(p.FechaInicio).Days + 1;
            int? IdConcepto = null;
            try { IdConcepto = (cfiniquitos.GetvConfiguracionConceptosFiniquitos(IdCliente).IdConceptoAguinaldo).Value; } catch(Exception ex) { throw new Exception("No se ha configurado el concepto para aguinaldo." + ex.Message); }
            if (IdConcepto != null)
            {
                ListDiasTrabajadosAguinaldo = new List<ModelDiasTrabajadosAguinaldo>();
                foreach (var emp in e)
                {
                    decimal Faltas = 0;
                    try { Faltas = (decimal)Lista_Total_Faltas.Where(x => x.IdEmpleado == emp.IdEmpleado).Select(x => x.TotalFaltas).Sum(); } catch { Faltas = 0; }

                    decimal Incapacidades = 0;
                    try { Incapacidades = (decimal)Lista_Total_Faltas.Where(x => x.IdEmpleado == emp.IdEmpleado).Select(x => x.TotalIncapacidades).Sum(); } catch { Incapacidades = 0; }

                    GetPrestacionesEmpleado(emp.IdPrestaciones);
                    _fechaReconocimientoAntiguedad = GetFechaIngreso(emp.FechaReconocimientoAntiguedad, emp.FechaAltaIMSS, conf_fecha);                    
                    _FechaInicioEjercicio = GetFechaInicioCalculo(p.FechaInicio, _fechaReconocimientoAntiguedad);  
                    DiasTrabajadosEjercicio = (p.FechaFin.Subtract(_FechaInicioEjercicio).Days + 1) - (Faltas + Incapacidades);
                    ListDiasTrabajadosAguinaldo.Add(new ModelDiasTrabajadosAguinaldo { IdEmpleado = emp.IdEmpleado, DiasTrabajados = DiasTrabajadosEjercicio });
                    Antiguedad = (p.FechaFin.Subtract(_fechaReconocimientoAntiguedad).Days) / 365M;
                    DiasAguinaldoFactorIntegracion = ObtenDiasAguinaldoPorFactorIntegracion(Antiguedad);
                    decimal FactorAguinaldo = DiasTrabajadosEjercicio / DiasTotalesPeriodo;
                    decimal FactorDiasAguinaldo = DiasAguinaldoFactorIntegracion * FactorAguinaldo;
                    
                    ClassIncidencias inc = new ClassIncidencias();
                    if (UnidadNegocio.AguinaldoExentoCompleto == "S")
                        inc.NewIncindencia(p.IdPeriodoNomina, (int)IdConcepto, emp.IdEmpleado, FactorDiasAguinaldo, FactorDiasAguinaldo, 0, 0, null, null, null, null, null, 1, null, null, null, null, null, IdUsuario);
                    else
                        inc.NewIncindencia(p.IdPeriodoNomina, (int)IdConcepto, emp.IdEmpleado, FactorDiasAguinaldo, FactorDiasAguinaldo, 0, 0, null, null, null, null, null, 1, null, FactorAguinaldo, null, null, null, IdUsuario);
                }
            }
            else
            {
                throw new Exception("No esta configurado el concepto de aguinaldo");
            }
        }

        /// <summary>
        /// Metodo que obtiene la fecha de calculo del empleado, puede ser la fecha de alta ante la institución de seguridad social o la fecha reconocimiento de antiguedad.
        /// </summary>
        /// <param name="FechaRecAnt">Fecha de Reconocimiento de Antiguedad.</param>
        /// <param name="FechaIMSS">Fecha de Alta ante la Instancia de Seguridad Social.</param>
        /// <param name="conf"></param>
        /// <returns>Regresa un campo de tipo DateTime</returns>
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

        /// <summary>
        /// Metodo que obtiene la Fecha en la que iniciara el calculo.
        /// </summary>
        /// <param name="FechaIncioPeriodo">Fecha fin del periodo de nómina.</param>
        /// <param name="FechaAlta">Fecha de alta del empleado.</param>
        /// <returns></returns>
        public DateTime GetFechaInicioCalculo(DateTime FechaIncioPeriodo, DateTime FechaAlta)
        {
            DateTime fechaInicio = FechaIncioPeriodo;

            if (FechaIncioPeriodo < FechaAlta)
                fechaInicio = FechaAlta;

            return fechaInicio;
        }

        /// <summary>
        /// Metodo que obtiene los dias que se le pagaran en base a la antiguedad del empleado.
        /// </summary>
        /// <param name="Antiguedad">Factor antiguedad en decimales.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Exepción devuelta por el metoso en caso de algun error.</exception>
        public decimal ObtenDiasAguinaldoPorFactorIntegracion(decimal Antiguedad)
        {
            try
            {
                decimal _antiguedad = Math.Round(Antiguedad, 2);
                var Dias = (from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad && x.IdPrestaciones == IdPrestacionesEmpleado) select b.Aguinaldo).First();

                return (decimal)Dias;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Metodo que carga el listado de faltas que se consideraran en el periodo de aguinaldo.
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        /// <param name="Año">Año del ciclo que se esta procesando.</param>
        private void Carga_Lista_Faltas(int IdUnidadNegocio, int Año)
        {
            using (NominaEntities1 context = new NominaEntities1())
            {
                var queryFaltas = (from b in context.sp_AcumuladoFaltasAnual(IdUnidadNegocio, Año) select b);

                List<Faltas> Listado_faltas = new List<Faltas>();
                foreach (var item in queryFaltas)
                {
                    Listado_faltas.Add(new Faltas
                    {
                        IdEmpleado = item.IdEmpleado,
                        TotalFaltas = item.Faltas,
                        TotalIncapacidades = item.Incapacidades,                        
                    });
                }

                Lista_Total_Faltas = Listado_faltas;
            }
        }

        /// <summary>
        /// Metodo que carga el listado de faltas que se consideraran en el periodo de aguinaldo, para un empleado especifico.
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado.</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        /// <param name="Año">Año del ciclo que se esta procesando.</param>
        private void Carga_Lista_Faltas(int IdEmpleado, int IdUnidadNegocio, int Año)
        {
            using (NominaEntities1 context = new NominaEntities1())
            {
                var queryFaltas = (from b in context.sp_AcumuladoFaltasAnual(IdUnidadNegocio, Año) select b);

                List<Faltas> Listado_faltas = new List<Faltas>();
                foreach (var item in queryFaltas.Where(x => x.IdEmpleado == IdEmpleado))
                {
                    Listado_faltas.Add(new Faltas
                    {
                        IdEmpleado = item.IdEmpleado,
                        TotalFaltas = item.Faltas,
                        TotalIncapacidades = item.Incapacidades,
                    });
                }

                Lista_Total_Faltas = Listado_faltas;
            }
        }        
    }
}