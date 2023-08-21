using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mActualizacionSDI
    {
        [Required(ErrorMessage = "No se ha seleccionado ningun archivo")]
        [Display(Name = "Seleccionar Archivo (csv):")]
        public HttpPostedFileBase Archivo { get; set; }

        [Display(Name = "Observaciones:")]
        public string Observaciones { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }
    }
}