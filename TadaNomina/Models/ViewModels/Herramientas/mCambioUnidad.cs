using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Herramientas
{
    public class mCambioUnidad
    {  
        [Display(Name = "Ingrese las Claves de Empleados:")]
        public string Claves { get; set; }        
       
        [Required]
        [Display(Name = "Nueva Unidad de Negocio:")]
        public int NuevaUnidadNegocio { get; set; }
        public List<SelectListItem> lUnidadNegocio { get; set; }

        public string Mensaje { get; set; }
        public bool Validacion { get; set; }
    }
}