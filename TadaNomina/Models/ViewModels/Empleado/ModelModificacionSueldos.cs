using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels
{
    public class ModelModificacionSueldos
    {
        public int IdEmpleado { get; set; }
        public int IdUnidadNegocio { get; set; }

        [Display(Name = "Sueldo Diario")]
        public decimal? SD { get; set; }

        [Display(Name = "Sueldo Diario Base")]
        public decimal? SDIMSS { get; set; }

        [Display(Name = "Sueldo Diario Integrado")]
        public decimal? SDI { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        [Display(Name = "Fecha del Movimiento")]

        public string FechaMovimiento { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [Display(Name = "Sueldo Diario Integrado")]
        public decimal? SDISueldos { get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        [Display(Name = "Sueldo Diario Base")]
        public decimal? SDIMSSSueldos { get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        [Display(Name = "Sueldo Diario")]
        public decimal? SDSueldos { get; set; }
        public string FechaReconocimientoAntiguedad { get; set; }
    }
}