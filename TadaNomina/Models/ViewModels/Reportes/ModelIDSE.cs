using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelIDSE
    {
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        [Display(Name = "Unidad de Negocio")]
        public string UnidadNegocio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]        
        [Display(Name = "Fecha Inicial")]
        public string fIncial { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Final")]
        public string fFinal { get; set; }

        [Required]
        [Display(Name = "Tipo Mov.")]
        public string tipoMovimiento { get; set; }
        [Display(Name = "Tipo Mov.")]
        public List<SelectListItem> ltipoMovimiento { get; set; }
    }
}