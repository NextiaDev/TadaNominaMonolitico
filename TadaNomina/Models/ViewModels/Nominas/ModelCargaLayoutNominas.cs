using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelCargaLayoutNominas
    {
        [Required]
        [Display(Name = "Periodo de Nomina:")]
        public int IdPeriodoNomina { get; set; }
        [Display(Name = "Periodo:")]
        public string PeriodoNomina { get; set; }
        public string TipoPeriodo { get; set; }
        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }
    }
}