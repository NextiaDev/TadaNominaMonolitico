using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelDepartamentos
    {
        [Key]
        public int IdDepartamento { get; set; }
        public int IdCliente { get; set; }        
        //[Display(Name = "Cliente:")]        
        public string Cliente { get; set; }
        [Required]
        [Display(Name = "Clave Departamento:")]
        public string Clave { get; set; }
        [Required]
        [Display(Name = "Nombre Departamento:")]
        public string Departamento { get; set; }

        
        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
        public int? ValidaDepto { get; set; }
        public int? ValidaEmpleado { get; set; }

    }
}