using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelGetCosteo
    {
        public int IdCosteo { get; set; }
        public int IdCliente { get; set; }
        public List<SelectListItem> lCosteo { get; set; }
        public string Esquema { get; set; }
        public int IdUnidadNegocio { get; set; }
        public List<SelectListItem> lUnidadNegocio { get; set; }        
        
        [Display(Name ="Periodos Disp.")]
        public int[] IdsPeriodos { get; set; } 
        public List<SelectListItem> lPeriodos { get; set; }
        [Required]
        [Display(Name = "Periodos Selec.")]
        public int[] IdsPeriodosSelecionados { get; set; }
        public List<SelectListItem> lPeriodosSeleccionados { get; set; }
        public string DescripcionPeriodos { get; set; }
        public string ClienteUnidad { get; set; }

        public List<mResultCosteo> lcosteos { get; set; }        
    }
}