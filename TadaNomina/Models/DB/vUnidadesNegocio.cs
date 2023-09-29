//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TadaNomina.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class vUnidadesNegocio
    {
        public int IdUnidadNegocio { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string UnidadNegocio { get; set; }
        public int IdTipoNomina { get; set; }
        public int IdEstatus { get; set; }
        public string TipoNomina { get; set; }
        public Nullable<decimal> PorcentajeISN { get; set; }
        public string ConfiguracionSueldos { get; set; }
        public int ClienteAdministrado { get; set; }
        public Nullable<int> IdSindicato { get; set; }
        public decimal DiasPago { get; set; }
        public Nullable<int> IncidenciasAguinaldoAutomaticas { get; set; }
        public string PercepcionesEspeciales { get; set; }
        public string DeduccionesEspeciales { get; set; }
        public string FiniquitosFechasDiferentes { get; set; }
        public string FiniquitosTablaMensual { get; set; }
        public string FiniquitosSubsidio { get; set; }
        public string FiniquitosExentoCompleto { get; set; }
        public Nullable<int> Confidencial { get; set; }
        public string CalculaProvision { get; set; }
        public string DiasFraccioandos { get; set; }
        public Nullable<decimal> FactorDiasFraccionados { get; set; }
        public string IdsConceptosFraccionados { get; set; }
        public string Honorarios { get; set; }
        public string ValidacionAcumulaPeriodo { get; set; }
        public string IdsValidacionAcumulaPeriodo { get; set; }
        public string NotificarAcumular { get; set; }
        public string CorreosNotificacion { get; set; }
        public string DiasAltaMas { get; set; }
        public string DiasAltaMenos { get; set; }
        public string DiasAltaMasFraccionados { get; set; }
        public string DiasAltaMenosFraccionados { get; set; }
        public string FaltasImporte { get; set; }
        public string ValesEnEfectivo { get; set; }
        public string MostrarLiquidacionFormato { get; set; }
        public string ConsideraAusentismosEnAguinaldo { get; set; }
        public string LeyendaReciboFiniquitos { get; set; }
        public string NoCalcularCargaObrera { get; set; }
        public string NoCalcularCargaPatronal { get; set; }
        public string AguinaldoExentoCompleto { get; set; }
        public string SubsidioSinSueldo { get; set; }
        public string ConceptosSDILiquidacion { get; set; }
        public string CalcularLiquidacionSDI { get; set; }
        public string ISRAguinaldoL174 { get; set; }
        public string Clave_Sat { get; set; }
        public string GenerarIntegradoPVyAgui { get; set; }
        public string SeptimoDia { get; set; }
        public Nullable<int> TipoCliente { get; set; }
        public string AcuerdoEconomico { get; set; }
        public Nullable<int> TipoCobro { get; set; }
    }
}
