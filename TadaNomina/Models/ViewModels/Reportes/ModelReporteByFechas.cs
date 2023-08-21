using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteByFechas
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Inicial")]
        public string fInicial { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Final")]
        public string fFinal { get; set; }
    }
}