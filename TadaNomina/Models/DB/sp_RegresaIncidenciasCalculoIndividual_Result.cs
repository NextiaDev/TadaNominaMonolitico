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
    
    public partial class sp_RegresaIncidenciasCalculoIndividual_Result
    {
        public int IdIncidencia { get; set; }
        public string Concepto { get; set; }
        public int IdConcepto { get; set; }
        public string ClaveConcepto { get; set; }
        public string Agrupador { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadEsquema { get; set; }
        public decimal MontoTrad { get; set; }
        public decimal MontoEsq { get; set; }
        public int IdPeriodoNomina { get; set; }
        public string TipoConcepto { get; set; }
        public string TipoDato { get; set; }
        public Nullable<int> IdCliente { get; set; }
        public string TipoEsquema { get; set; }
        public Nullable<int> BanderaFiniquitos { get; set; }
        public Nullable<int> BanderaAguinaldos { get; set; }
        public Nullable<int> BanderaIncidenciasFijas { get; set; }
        public string MultiplicaDT { get; set; }
        public Nullable<int> BanderaConceptoEspecial { get; set; }
        public Nullable<int> BanderaInfonavit { get; set; }
        public Nullable<int> BanderaFonacot { get; set; }
        public Nullable<int> BanderaPensionAlimenticia { get; set; }
        public Nullable<int> BanderaSDI { get; set; }
        public Nullable<int> BanderaVacaciones { get; set; }
        public Nullable<int> BanderaAdelantoPULPI { get; set; }
        public Nullable<int> BanderaAusentismos { get; set; }
        public Nullable<int> BanderaPiramidacion { get; set; }
        public Nullable<int> BanderaBallistic { get; set; }
        public Nullable<int> BanderaSaldos { get; set; }
        public Nullable<int> BanderaCompensaciones { get; set; }
        public Nullable<int> BanderaIncidencia { get; set; }
    }
}
