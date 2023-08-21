using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelRepByFechasTM
    {
        public int IdUnidadnegocio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Inicial")]
        public string fIncial { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Final")]
        public string fFinal { get; set; }

        [Required]
        [Display(Name = "Seleccionar")]
        public string tipoMovimiento { get; set; }
        [Display(Name = "Seleccionar")]
        public List<SelectListItem> ltipoMovimiento { get; set; }
    }
}