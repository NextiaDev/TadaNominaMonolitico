using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelRecuperaciones
    {
        public int IdEmpleado { get; set; }
        
        [Display(Name = "Periodo de Nomina")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> PeriodosList { get; set; }

        public int IdRegistroPatronal { get; set; }

        public decimal monto { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(Name = "Concepto Recuperación")]
        public string ConceptoRecuperacion { get; set; }
        public List<SelectListItem> ConceptoList { get; set; }
        
        public int Idusuario { get; set; } 

    }
}