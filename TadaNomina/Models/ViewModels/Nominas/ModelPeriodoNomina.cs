using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelPeriodoNomina
    {
        [Key]
        public int IdPeriodoNomina { get; set; }
        public int IdUnidadNegocio { get; set; }
        [Display(Name = "Unidad")]
        public string UnidaNegocio { get; set; }
        [Required]
        [Display(Name = "Nombre del Periodo:")]
        public string Periodo { get; set; }
        [Required]
        [Display(Name ="Tipo de Nómina:")]
        public string TipoNomina { get; set; }
        public List<SelectListItem> LTipoNomina { get; set; }
        [Required]
        [Display(Name = "Fecha Inicial:")]
        public string FechaInicio { get; set; }
        [Required]
        [Display(Name = "Fecha Final:")]
        public string FechaFin { get; set; }
        [Required]
        [Display(Name = "Ajuste de Impuestos?:")]
        public string AjusteImpuestos { get; set; }

        public List<SelectListItem> LAjuste { get; set; }
        [Display(Name ="Periodos disponibles Ajuste:")]
        public int IdPeriodoAjuste { get; set; }
        [Display(Name ="Periodos Acumulados:")]
        public string PeriodoAjuste { get; set; }
        [Display(Name = "Ajuste Anual")]
        public bool AjusteAnual { get; set; }
        [Display(Name = "Tabla de Impuestos Diaria")]
        public bool TablaIDiaria { get; set; }
        public List<SelectListItem> LPAjuste { get; set; }
        [Display(Name ="Identificadores:")]
        [DataType(DataType.MultilineText)]
        public string IdsPeriodosAjuste { get; set; }
        [Display(Name ="Periodos Ajuste:")]
        [DataType(DataType.MultilineText)]
        public string PeriodosAjuste { get; set; }
        [Display(Name ="Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }
        [Display(Name = "Timbrados")]
        public int timbradosPeriodo { get; set; }
        [Display(Name = "Omitir Descuentos")]
        public bool OmitirDescuentosFijos { get; set; }

        public DateTime? FechaDispersion { get;set; }

        [Display(Name = "Fecha Inicial Para Reloj Checador")]
        public string FechaInicioChecador { get; set; }
        [Display(Name = "Fecha Final Para Reloj Checador")]
        public string FechaFinChecador { get; set; }

        public int IdCliente_PTU { get; set; }
        [Display(Name = "Registro Patronal")]
        public int? IdRegistroPatronal_PTU { get; set; }
        public List<SelectListItem> listRegistrosPatronalesCliente { get; set; }
        [Display(Name = "Importe a dispersar")]
        public decimal Monto_PTU { get; set; }
        [Display(Name = "Año")]
        public int Año_PTU { get; set; }
        public string ValidacionAcumulaPeriodo { get; set; }
        public string[] idsValidacion { get; set; }
        public decimal? CargaObrera { get; set; }
        public decimal? CargaPatronal { get; set; }
        public decimal? ISR { get; set; }
        public decimal? TotalPercepciones { get; set; }
        public decimal? TotalDeducciones { get; set; }
        public decimal? NetoPagar { get; set; }
        public string FechaDispersion_ {  get; set; }
        public bool CalculoPatronaPtu { get; set; }

        public int IdEstatus { get; set; }
        public bool Validacion { get; set; }
        public string Mensaje { get; set; }
    }
}
