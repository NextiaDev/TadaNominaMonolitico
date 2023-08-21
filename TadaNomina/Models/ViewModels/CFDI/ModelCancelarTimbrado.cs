using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelCancelarTimbrado
    {
        [Required]
        [Display(Name = "Seleccione el Periodo:")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
        [Display(Name ="Ingresar Claves (Separadas por coma):")]
        [DataType(DataType.MultilineText)]
        public string ClavesEmpleado { get; set; }

        [Required]
        [Display(Name = "Motivo de Cancelacion:")]
        public string motivoCancelacion { get; set; }
        public List<SelectListItem> lMotivosCancalacion { get; set; }
        public List<LogErrores> errores { get; set; }
        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}