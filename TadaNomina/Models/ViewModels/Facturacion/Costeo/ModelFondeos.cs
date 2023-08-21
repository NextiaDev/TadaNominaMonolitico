using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion.Costeo
{
    public class ModelFondeos
    {        
        [Display(Name ="Periodo")]
        public int? IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodo { get; set; }
        [Display(Name = "Costeo")]
        public int? IdCosteo { get; set; }
        public List<SelectListItem> lCosteos { get; set; }
        [Display(Name = "Patrona")]
        public int? IdPatrona { get; set; }
        public List<SelectListItem> lPatrona { get; set; }
        [Display(Name = "División")]
        public int? IdDivision { get; set; }
        public List<SelectListItem> lDivision { get; set; }
        [Display(Name = "Conceptos")]
        public int?[] IdsConcetos { get; set; }
        public List<SelectListItem> lConceptos { get; set; }
        [Display(Name = "Conceptos Agregados")]
        public int?[] IdsConceptosSelec { get; set; }
        public List<SelectListItem> lConceptosSelect { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        public decimal Importe { get; set; }
        public string conceptos { get; set; }
        public bool guardar { get; set; }
    }
}