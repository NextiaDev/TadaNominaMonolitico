using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelNewSolicitud
    {

        [Required]
        [Display(Name = "Periodo: ")]
        public string PeriodoNomina { get; set; }
        [Required]
        [Display(Name = "Esquema: ")]
        public string Esquema { get; set; }
        public List<SelectListItem> lEsquema { get; set; }
        [Required]
        [Display(Name = "Importe: ")]
        public decimal Importe { get; set; }
        [Required]
        public decimal Honorario { get; set; }
        [Required]
        public decimal Subtotal { get; set; }
        [Required]
        public decimal IVA { get; set; }
        [Required]
        public decimal Total { get; set; }


        
    }
}