using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelActualizaNetos
    {
        [Required]
        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }
    }
}