using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class ClassProcesoNominal
    {
        private int IdEmpleado;
        private int IdEntidad;
        private decimal Sueldo;
        private decimal SueldoReal;
        private float? PeriodoPago;
        private decimal DiasTrabajados_IMSS;
        protected decimal DiasAdicionalesIMSS;
        private decimal Porcentaje_Riesgo_Trabajo_Patronal;
        protected int? IdDepartamento;
        protected int? IdBancoTrad;
        protected string CuentaBancoTrad;
        protected string CuentaInterbancariaTrad;
        protected int? IdPuesto;
        private decimal Anios;
        private decimal Faltas;
        private decimal FaltasPorBaja;
        private decimal PermisoSinGoceDeSueldo;
        private decimal Incapacidades;
        private decimal IdCentroCostos;
        private decimal IdRegistroPatronal;
        private decimal SueldoDiario;
        private decimal SueldoDiarioEsquema;
        private decimal SueldoDiarioReal;
        private decimal? DiasTrabajados;
        private decimal Dias_Vacaciones;
        private DateTime FechaActual;
        private DateTime FechaPeriodoNomina;
        private DateTime? FechaALTAIMSS;
        private DateTime? Fecha_Reconocimiento_Antiguedad;
        private decimal Sueldo_Vacaciones;
        private decimal Prima_Vacacional;
        private decimal Prima_VacacionalEsquema;
        private decimal Dias_Vacaciones_Como_Prestacion_Por_Factor_Integracion;
        private decimal? FactorIntegrado;
        private decimal SueldoPagado;
        private decimal Apoyo;
        private decimal BaseGravada;
        private decimal LimiteInferior;
        private decimal Porcentaje;
        private decimal CuotaFija;
        private decimal DiferenciaLimite;
        private decimal PorcentajeCalculado;
        private decimal ISR;
        private decimal CreditoSalario;
        private decimal SubsidioAPagar;
        private decimal ImpuestoRetenido;
        private decimal Honorario;
        private decimal HonorarioAcobrar;
        private decimal ISN;
        private decimal SDI;
        private decimal Sueldo_Minimo_A;
        private decimal Excedente_Obrera;
        private decimal Prestaciones_Dinero;
        private decimal Gastos_Med_Pension;
        private decimal Invalidez_Vida_Patronal;
        private decimal Invalidez_Vida;
        private decimal Cesantia_Vejez;
        private decimal Cuota_Fija_Patronal;
        private decimal Excedente_Patronal;
        private decimal Prestamo_Dinero_Patronal;
        private decimal Gastos_Med_Pension_Patronal;
        private decimal Riesgo_Trabajo_Patronal;
        private decimal Prestamo_Especie;
        private decimal Retiro;
        private decimal Cesantia_Vejez_Patronal;
        private decimal INFONAVIT_Patronal;
        private int numeroTotalDeRegistros;
        private string ConfiguracionSueldos;
        private string ConfiguracionNominal;
        private decimal Total_ER;
        private decimal Total_DD;
        private decimal Total_ERS;
        private decimal Total_DDS;
        private decimal Neto_IMSS;
        private decimal Neto_Esquema;
        private decimal Total_Patron;
        private decimal IMSS;
        private decimal IVA;
        private decimal TotalCosteo;
        private decimal TotalSubsidioAPagar;
        private decimal Total;
        private string EsquemaCobroISN;
        protected string decimal_Pago_IMSS;
        protected decimal Dias_Faltados { get; set; }
        public decimal Dias_Laborados { get; set; }

        private string EmpleadosBaja_Dias;
        private int Estatus_Empleado;

        public int AñoDelPeriodoNomina { get; set; }
        public int UltimoDiaMes { get; set; }
        public int DiferenciaDiasPeriodos { get; set; }
        public string AjusteImpuestos { get; set; }
        public int PeriodoAjuste { get; set; }
        public int IdUnidadNegocio { get; set; }
        public int IdPeriodoNomina { get; set; }
        public int IdPeriodoNominal_TipoNomina { get; set; }
        public int IdCliente { get; set; }
        public int IdCaptura { get; set; }

        /// <summary>
        /// Método para configurar todos los datos en cero.
        /// </summary>
        private void LimpiaDatos()
        {
            IdEmpleado = 0;
            Sueldo = 0;
            SueldoReal = 0;
            PeriodoPago = 0;
            DiasAdicionalesIMSS = 0;
            DiasTrabajados_IMSS = 0;
            Porcentaje_Riesgo_Trabajo_Patronal = 0;
            Anios = 0;
            Apoyo = 0;
            BaseGravada = 0;
            SueldoDiario = 0;
            SueldoDiarioEsquema = 0;
            SueldoDiarioReal = 0;
            DiasTrabajados = 0;
            IdPuesto = 0;
            Dias_Vacaciones = 0;
            Faltas = 0;
            FaltasPorBaja = 0;
            PermisoSinGoceDeSueldo = 0;
            Sueldo_Vacaciones = 0;
            Prima_Vacacional = 0;
            Dias_Vacaciones_Como_Prestacion_Por_Factor_Integracion = 0;
            FactorIntegrado = 0;
            SueldoPagado = 0;
            BaseGravada = 0;
            LimiteInferior = 0;
            Porcentaje = 0;
            HonorarioAcobrar = 0;
            Honorario = 0;
            CuotaFija = 0;
            DiferenciaLimite = 0;
            PorcentajeCalculado = 0;
            ISR = 0;
            CreditoSalario = 0;
            SubsidioAPagar = 0;
            ImpuestoRetenido = 0;
            ISN = 0;
            SDI = 0;
            Sueldo_Minimo_A = 0;
            Excedente_Obrera = 0;
            Prestaciones_Dinero = 0;
            Gastos_Med_Pension = 0;
            Invalidez_Vida_Patronal = 0;
            Prima_VacacionalEsquema = 0;
            Invalidez_Vida = 0;
            Cesantia_Vejez = 0;
            Cuota_Fija_Patronal = 0;
            Excedente_Patronal = 0;
            Prestamo_Dinero_Patronal = 0;
            Gastos_Med_Pension_Patronal = 0;
            Riesgo_Trabajo_Patronal = 0;
            Prestamo_Especie = 0;
            Retiro = 0;
            Cesantia_Vejez_Patronal = 0;
            INFONAVIT_Patronal = 0;
            IVA = 0;
            TotalCosteo = 0;
            TotalSubsidioAPagar = 0;
            Neto_IMSS = 0;
            Neto_Esquema = 0;

            IdCentroCostos = 0;
            IdDepartamento = 0;
            IdBancoTrad = 0;
            CuentaBancoTrad = string.Empty;
        }

        /// <summary>
        /// Método para modificar el estatus de los recibos de esquema.
        /// </summary>
        /// <param name="_IdPeriodoNomina">Recibe la variable int para modificar el status.</param>
        public void CambiaEstatusRecibosEsquema(int _IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var query = entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == _IdPeriodoNomina).First();
                query.RecibosComplemento = "Si";

                entidad.SaveChanges();
            }
        }
    }
}