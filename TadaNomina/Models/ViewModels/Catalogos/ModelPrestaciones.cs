using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelPrestaciones
    {
        [Key]
        public int IdPrestacion { get; set; }        
        public int IdCliente { get; set; }
        [Display(Name ="Cliente: ")]
        public string Cliente { get; set; }
        [Display(Name ="Nombre descriptivo: ")]
        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Solo se permiten letras y espacios en blanco.")]
        public string Prestacion { get; set; }  
        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}