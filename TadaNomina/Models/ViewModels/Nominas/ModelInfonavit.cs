using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelInfonavit
    {
        [Key]
        public int IdCreditoInfonavit { get; set; }        
        public int? IdEmpleado { get; set; }
        [Display(Name = "Cve.")]
        public string ClaveEmp { get; set; }
        [Display(Name = "Nombre")]
        public string Empleado { get; set; }
        [Display(Name = "R.F.C.")]
        public string rfc { get; set; }
        [Required]
        public string Tipo { get; set; }
        public List<SelectListItem> lTipo { get; set; }
        [Required]
        [Display(Name = "No. Cred.")]
        public string NoCredito { get; set; }
        [Required]
        [Display(Name ="Factor")]
        public decimal? CantidadUnidad { get; set; } 
        public int? IdEstatus { get; set; }
        public bool Estatus { get; set; }
        [Display(Name = "f. Captura")]
        public DateTime? fechaCaptura { get; set; }

        [Display(Name = "Porcentaje Tradicional")] 
        public decimal PorcentajeTradicional { get; set; }
        public bool Validacion { get; set; }
        public string Mensaje { get; set; }

        [Display(Name = "¿No cobrar el Seguro Vivienda?")]
        public bool BanderaSeguroVivienda { get; set; }
    }
}