using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class CosteoConceptosM
    {
        public int idCosteosConcepto { get; set; }
        public int idCosteo { get; set; }
        [Required]
        [Display(Name = "Descripcion")]
        public string descripcion { get; set; }
        [Display(Name = "Tipo de Facturacion:")]
        public string tipoDatoFacturacion { get; set; }
        public List<SelectListItem> Lfacturacion { get; set; }
        [Display(Name = "Observaciones")]
        public string observaciones { get; set; }
        [Required]
        [Display(Name = "Orden en el que aparece")]
        public int orden { get; set; }
        [Required]
        [Display(Name = "Visible")]
        public string visible { get; set; }
        public List<SelectListItem> lVisible { get; set; }
        public int idEstatus { get; set; }
        public int idCaptura { get; set; }
        public DateTime fechaCaptura { get; set; }
        public int idModifica { get; set; }
        public DateTime fechaModifica { get; set; }
    }
}