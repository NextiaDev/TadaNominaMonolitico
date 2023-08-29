using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Herramientas
{
    public class mMigracionRP
    {
        [Display(Name = "Ingrese las Claves de Empleados:")]
        public string Claves { get; set; }

        [Required]
        [Display(Name = "Nuevo Registro Patronal:")]
        public int IdNuevoRegistroPatronal { get; set; }
        public List<SelectListItem> lregistros { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha de Baja")]
        public string fBaja { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha de Alta")]
        public string fAlta { get; set; }
        [Display(Name = "Conserva Antigüedad")]
        public bool ConservaAntiguedad { get; set; }

        public string Mensaje { get; set; }
        public bool Validacion { get; set; }
    }
}