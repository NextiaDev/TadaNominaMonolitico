using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteByIdPeriodo
    {
        [Required]
        [Display(Name = "Seleccione el Periodo:")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
    }
}