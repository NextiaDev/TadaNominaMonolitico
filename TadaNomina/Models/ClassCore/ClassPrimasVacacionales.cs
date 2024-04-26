using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassPrimasVacacionales
    {
        public decimal DiasVacaciones { get; set; }
        public decimal Porcentaje { get; set; }        
        public decimal PV { get; set; }
        public decimal _factorConcepto = 0;


        /// <summary>
        /// Método para listar la prima vacacional por periodo de nómina específico.
        /// </summary>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <returns>Regresa la lista de la prima vacacional.</returns>
        public List<ModelPrimasVacacionales> GetPrimasVacacionalesPeriodo(int IdPeriodoNomina)
        {

            List<ModelPrimasVacacionales> lPrimas = new List<ModelPrimasVacacionales>();

            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            var periodo = cperiodo.GetvPeriodoNominasId(IdPeriodoNomina);

            ClassEmpleado emp = new ClassEmpleado();
            var lemp = emp.GetEmpleadoByUnidadNegocio(periodo.IdUnidadNegocio);

            ClassFechasCalculos cf = new ClassFechasCalculos();
            var conf = cf.GetConfiguracionFechas(periodo.IdUnidadNegocio);

            var IdConceptoPV = ValidaConfiguracionConcepto(periodo.IdCliente);
            
            if (IdConceptoPV != null)
            {
                ClassConceptos cc = new ClassConceptos();
                var concepto = cc.GetvConcepto((int)IdConceptoPV);

                if (concepto != null)
                {
                    if (concepto.SDPor != null && concepto.SDPor > 0)
                        _factorConcepto = (decimal)concepto.SDPor;
                }
            }

            foreach (var item in lemp)
            {
                DateTime? FechaIngreso = null;
                if (conf.FPVReal != null && conf.FPVReal != string.Empty)
                {
                    FechaIngreso = ObtenFechaCalculo(item, conf, "Real");
                    decimal Antiguedad = periodo.FechaFin.Subtract((DateTime)FechaIngreso).Days / 365M;

                    if (ValidaSiAplicaPV((DateTime)FechaIngreso, periodo.FechaInicio, periodo.FechaFin, Antiguedad))
                    {
                        ModelPrimasVacacionales mPrimas = new ModelPrimasVacacionales();

                        int IdPrestaciones = 1;
                        try { IdPrestaciones = (int)item.IdPrestaciones; } catch { }
                                               
                        mPrimas.IdEmpleado = item.IdEmpleado;
                        mPrimas.ClaveEmpleado = item.ClaveEmpleado;
                        mPrimas.Nombre = item.ApellidoPaterno + " " + item.ApellidoMaterno + " " + item.Nombre;
                        CalculaPV(IdPrestaciones, (decimal)item.SDIMSS, Antiguedad, periodo.IdUnidadNegocio);
                        mPrimas.PV = PV;
                        mPrimas.PVReal = PV;
                        mPrimas.DiasVacaciones = DiasVacaciones;
                        mPrimas.SD = item.SD;
                        mPrimas.SDIMSS = item.SDIMSS;
                        mPrimas.Factor = Porcentaje;
                        
                        lPrimas.Add(mPrimas);
                    }
                }                
            } 
            
            return lPrimas;
        }

        /// <summary>
        /// Método para validar las fechas en que se realizan los cálculos de la prima vacacional.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la validación de las fechas de los cálculos de la prima vacacional.</returns>
        public bool validaConfiguracion(int IdUnidadNegocio)
        {
            ClassFechasCalculos cf = new ClassFechasCalculos();
            var conf = cf.GetConfiguracionFechas(IdUnidadNegocio);

            if (conf != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Método para obtener la fecha del cálculo de la prima vacacional.
        /// </summary>
        /// <param name="emp">Recibe el modelo de empleados.</param>
        /// <param name="conf">Recibe el modelo de la configuración de las fechas de los cálculos de las primas vacacionales.</param>
        /// <param name="Calculo">Recibe la variable del cálculo.</param>
        /// <returns>Regresa la fecha del cálculo de la prima vacacional.</returns>
        public DateTime? ObtenFechaCalculo(Empleados emp, ConfiguracionFechasCalculos conf, string Calculo)
        {
            DateTime? fecha = null;
            string fechaCalculo = string.Empty;
            if (Calculo == "Real")
                fechaCalculo = conf.FPVReal;            

            if (Calculo == "Tradicional")
                fechaCalculo = conf.FPV;            

            if (Calculo == "Esquema")
                fechaCalculo = conf.FPVEsq;            

            if (fechaCalculo == "Alta IMSS")
                fecha = emp.FechaAltaIMSS;

            if (fechaCalculo == "Reconocimiento Antiguedad")
                fecha = emp.FechaReconocimientoAntiguedad;

            return fecha;        
        }

        /// <summary>
        /// Método para obtener la fecha del cálculo de la prima vacacional.
        /// </summary>
        /// <param name="emp">Recibe el modelo de empleados.</param>
        /// <param name="conf">Recibe el modelo de la configuración de las fechas de los cálculos de las primas vacacionales.</param>
        /// <param name="Calculo">Recibe la variable del cálculo.</param>
        /// <returns>Regresa la fecha del cálculo de la prima vacacional.</returns>
        public DateTime? ObtenFechaCalculo(vEmpleados emp, ConfiguracionFechasCalculos conf, string Calculo)
        {
            DateTime? fecha = null;
            string fechaCalculo = string.Empty;
            if (Calculo == "Real")
                fechaCalculo = conf.FPVReal;

            if (Calculo == "Tradicional")
                fechaCalculo = conf.FPV;

            if (Calculo == "Esquema")
                fechaCalculo = conf.FPVEsq;

            if (fechaCalculo == "Alta IMSS")
                fecha = emp.FechaAltaIMSS;

            if (fechaCalculo == "Reconocimiento Antiguedad")
                fecha = emp.FechaReconocimientoAntiguedad;

            if(fecha == null)
                fecha = emp.FechaReconocimientoAntiguedad;

            return fecha;
        }

        /// <summary>
        /// Método para calcular la prima vacacional.
        /// </summary>
        /// <param name="IdPrestaciones">Recibe el identificador de la prestación.</param>
        /// <param name="SD">Recibe una variable decimal.</param>
        /// <param name="Antiguedad">Recibe una variable decimal.</param>
        public void CalculaPV(int IdPrestaciones, decimal SD, decimal Antiguedad, int? idunidad)
        {

            if (Antiguedad < 0) { Antiguedad = 0; }
            GetDias(IdPrestaciones, Antiguedad, idunidad);

            PV = (DiasVacaciones * SD) * Porcentaje;
           
        }

        /// <summary>
        /// Método para validar si aplica el pago de la prima vacacional.
        /// </summary>
        /// <param name="FechaIngreso">Recibe la fecha de ingreso del empleado.</param>
        /// <param name="FechaInicio">Recibe la fecha inicial.</param>
        /// <param name="FechaFin">Recibe la fecha final.</param>
        /// <param name="Antiguedad">Recibe la variable decimal.</param>
        /// <returns>Regresa la validación de la fecha para corroborar si aplica pago de prima vacacional.</returns>
        private bool ValidaSiAplicaPV(DateTime FechaIngreso, DateTime FechaInicio, DateTime FechaFin, decimal Antiguedad)
        {
            bool valida = false;            

            if (Antiguedad >= 1)
            {
                DateTime FechaIngresoConAñoActual = DateTime.Parse(FechaFin.Year + "-" + FechaIngreso.Month + "-" + FechaIngreso.Day);

                if (FechaIngresoConAñoActual >= FechaInicio && FechaIngresoConAñoActual <= FechaFin)
                    valida = true;            
            }

            return valida;
        }

        /// <summary>
        /// Método para cálcular los días de prima vacacional.
        /// </summary>
        /// <param name="IdPrestaciones">Recibe el identificador de la prestación.</param>
        /// <param name="Antiguedad">Recibe la variable decimal.</param>
        public void GetDias(int IdPrestaciones, decimal Antiguedad, int? Idunidad)
        {
            DiasVacaciones = 0;
            Porcentaje = 0;
            var factor = new FactorIntegracion();
            if (Antiguedad > 1)
                Antiguedad -= 1;


            var cs = new ClassUnidadesNegocio();

            var unidad = cs.getUnidadesnegocioId(Idunidad.Value);

            if (unidad.BanderaPrestacionesPatronEnteros == "S")            
                factor = GetFactorIntegracion(IdPrestaciones).Where(x => x.Limite_Superior > Antiguedad && x.Limite_Inferior <= Antiguedad).OrderByDescending(x => x.FechaInicioVigencia).FirstOrDefault();            
            else            
                factor = GetFactorIntegracion(IdPrestaciones).Where(x => x.Limite_Superior >= Antiguedad && x.Limite_Inferior <= Antiguedad).OrderByDescending(x => x.FechaInicioVigencia).FirstOrDefault();

            decimal _antiguedad = Math.Round(Antiguedad, 4);

            if (factor != null)
            {
                DiasVacaciones = (decimal)factor.Vacaciones;
                Porcentaje = (decimal)factor.PrimaVacacional * 0.1M;
            }

            if (_factorConcepto > 0)
                Porcentaje = _factorConcepto;
        }



        /// <summary>
        /// Método que lista el valor del factor de integración para la prima vacacional.
        /// </summary>
        /// <param name="IdPrestaciones">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la lista de los factores de integración.</returns>
        private List<FactorIntegracion> GetFactorIntegracion(int IdPrestaciones)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var factor = entidad.FactorIntegracion.Where(x => x.IdPrestaciones == IdPrestaciones && x.IdEstatus == 1).ToList();

                return factor;
            }
        }

        /// <summary>
        /// Método para aplicar la prima vacional considerando incidencias.
        /// </summary>
        /// <param name="lista">Recibe la lista del modelo de la prima vacacional.</param>
        /// <param name="IdConcepto">Recibe el identificador del concepto.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        public void AplicaPrimasVacacionales(List<ModelPrimasVacacionales> lista, int IdConcepto, int IdUsuario, int IdPeriodoNomina)
        {
            if (lista.Count>0)
            {
                ClassIncidencias incidencias = new ClassIncidencias();
                foreach (var item in lista)
                {
                    ModelIncidencias inc = new ModelIncidencias()
                    {
                        IdEmpleado = item.IdEmpleado,
                        IdPeriodoNomina= IdPeriodoNomina,
                        Cantidad = item.DiasVacaciones,
                        IdConcepto= IdConcepto
                    };

                    incidencias.NewIncindencia(inc, IdUsuario);
                }
            }
        }

        /// <summary>
        /// Método para validar los conceptos de los finiquitos.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la configuración de los conceptos de los finiquitos.</returns>
        public int? ValidaConfiguracionConcepto(int IdCliente)
        {
            ClassConceptosFiniquitos cf = new ClassConceptosFiniquitos();
            int? conf = cf.GetConfiguracionConceptosFiniquitos(IdCliente).IdConceptoPV;

            return conf;
        }
    }
}
