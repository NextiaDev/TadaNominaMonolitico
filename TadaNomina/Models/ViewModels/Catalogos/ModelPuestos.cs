using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelPuestos
    {
        [Key]
        public int IdCliente { get; set; }
        [Display(Name ="Departamento")]
        public int? IdDepartamento { get; set; }
        public List<System.Web.Mvc.SelectListItem> lDepartamentos { get; set; }
        public int IdPuesto { get; set; }

        [Required]
        [Display(Name = "Clave Puesto:")]
        public string Clave { get; set; }

        [Required]
        [Display(Name ="Nombre Puesto:")]
        public string Puesto { get; set; }

        public string Cliente { get; set; }

        //[Required]
        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }

        public int? validaPuesto { get; set; }
        public int? ValidaEmpleado { get; set; }

    }
}