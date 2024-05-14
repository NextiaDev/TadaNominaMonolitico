using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelIncidenciaIndividual
    {
        public int Id { get; set; }
        public int IdConcepto{ get; set; }
        public string Agrupador { get; set; }        
        public string ClaveConcepto { get; set; }
        public string Concepto { get; set; }
        public string TipoConcepto { get; set; }
        public string TipoDato { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CantidadEsq { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoEsq { get; set; }
        public string TipoEsquema { get; set; }
        public int? BanderaFiniquitos { get; set; }   
        public int? BanderaAguinaldo { get; set; }
        public int? BanderaIncidenciasFijas { get; set; }
        public int? BanderaConceptoEspecial { get; set; }
        public int? BanderaInfonavit { get; set; }
        public int? BanderaFonacot { get; set; }
        public int? BanderaPensionAlimenticia { get; set; }
        public int? BanderaVacaciones { get; set; }
        public int? BanderaAusentismos { get; set; }
        public int? BanderaAdelantoPULPI { get; set; }
        public int? BanderaPiramidacion { get; set; }
        public string MultiplicaDT { get; set; }
        public int? BanderaSaldos { get; set; }
        public int? BanderaCompensaciones { get; set; }
        public int? BanderaIncidencia { get; set; }
        public string ModuloCaptura { get; set; }
    }

    public class ModelReciboTradicional
    {
        [Display(Name ="RFC:")]
        public string RFC { get; set; }
        [Display(Name = "CURP:")]
        public string CURP { get; set; }
        [Display(Name = "NSS:")]
        public string NSS { get; set; }
        public decimal SD {  get; set; }        
        public decimal SDI {  get; set; }        
        
        [Display(Name = "Dias Laborados:")]
        public decimal DiasLaborados { get; set; }
        [Display(Name = "Dias Vacaciones:")]
        public decimal DiasVacaciones { get; set; }
        [Display(Name = "Incapacidades:")]
        public decimal Incapacidades { get; set; }
        [Display(Name = "Faltas:")]
        public decimal Faltas { get; set; }
        [Display(Name = "Base Gravada:")]
        public decimal BaseGravada { get; set; }
        [Display(Name = "IMSS Patron:")]
        public decimal TotalPatron { get; set; }
        public List<sp_ReciboTradicionalPercepciones_Result> IncidenciasRecibo { get; set; }
        public List<sp_ReciboTradicionalDeducciones_Result> IncidenciasReciboDec { get; set; }
    }

    public class ModelReciboEsquema
    {        
        public List<sp_ReciboEsquemaPercepciones_Result> IncidenciasReciboEsquema { get; set; }
        public List<sp_ReciboEsquemaDeducciones_Result> IncidenciasReciboDecEsquema { get; set; }
    }

    public class ModelReciboReal
    {
        public decimal SDI { get; set; } = 0;

        [Display(Name = "Carga Patronal:")]
        public decimal TotalPatron { get; set; } = 0;
                
        public decimal ISN { get; set; } = 0;
        public List<sp_ReciboRealPercepciones_Result> IncidenciasReciboReal { get; set; }
        public List<sp_ReciboRealDeducciones_Result> IncidenciasReciboDecReal { get; set; }
    }

    public class ModelNominaIndividual
    {
        [Key]
        public int IdPeriodoNomina { get; set; }
        [Key]
        public int IdEmpleado { get; set; }        
        [Display(Name="No. Empleado:")]
        public string claveEmpleado {get; set; }
        [Display(Name = "Nombre:")]
        public string NombreCompletoEmpleado { get; set; }
        [Display(Name = "Sueldo Diario Real:")]
        public string SueldoDiario { get; set; }
        [Display(Name = "Sueldo Diario Esq:")]
        public string SueldoDiarioEsq { get; set; }
        [Display(Name = "Sueldo Diario:")]
        public string SueldoDiarioIMSS { get; set; }
        [Display(Name = "Sueldo Diario Integrado:")]
        public string SDI { get; set; }
        [Display(Name = "Fecha de alta ante IMSS:")]
        public string FechaAltaImss { get; set; }        
        [Display(Name = "F. Rec. de Antiguedad:")]
        public string FechaReconocimientoAntiguedad { get; set; }
        [Display(Name = "Fecha de Baja:")]
        public string FechaBaja { get; set; }
        [Display(Name = "Total de Percepciones Tradicional:")]
        public string ER { get; set; }
        [Display(Name = "Total de Deducciones Tradicional:")]
        public string DD { get; set; }
        [Display(Name = "Neto Tradicional:")]
        public string Neto { get; set; }
        [Display(Name = "Total de Percepciones Esquema:")]
        public string ERS { get; set; }
        [Display(Name = "Total de Deducciones Esquema:")]
        public string DDS { get; set; }
        [Display(Name = "Neto Esquema:")]
        public string NetoS { get; set; }
        public string ERR { get; set; }
        [Display(Name = "Total de Deducciones Esquema:")]
        public string DDR { get; set; }
        [Display(Name = "Neto Esquema:")]
        public string NetoR { get; set; }
        public bool Liquidacion { get; set; }

        public List<ModelIncidenciaIndividual> ListIncidencias { get; set; }
        public ModelReciboTradicional ReciboTradicional { get; set; }
        public ModelReciboEsquema ReciboEsquema { get; set; }
        public ModelReciboReal ReciboReal { get; set; }
        public int IdConfiguracionFiniquito { get; set; }
        public bool banderaVac { get; set; }
        public bool banderaPV { get; set; }
        public bool banderaAgui { get; set; }
        public bool banderaAguiEsq { get; set; }
        public bool bandera90d { get; set; }
        public bool bandera20d { get; set; }
        public bool banderaPA { get; set; }
        public bool LiquidacionSDI { get; set; }
        public bool BanderaExentoLiquidacionProporcional { get; set; }
        public int IdAguinaldoConfigurado { get; set; }

        [Display(Name = "Neto a Pagar:")]
        public string NetoPagar { get; set; }
        [Display(Name = "Total a Pagar:")]
        public string TotalRecibir { get; set; }
        public int IdEstatus { get; set; }

        [Display(Name = "Periodo:")]
        public string NombrePeriodo { get; set; }
        [Display(Name = "Fechas:")]
        public string FechasPeriodo { get; set; }
        [Display(Name = "Configuración de Sueldos:")]
        public string ConfiguracionSueldos { get; set; }

        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }  

}
