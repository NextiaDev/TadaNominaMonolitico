using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels
{
    public class ModelCambioPass
    {
        [Key]
        public int IdUsuario { get; set; }
        [Display(Name ="Usuario:")]
        public string Usuario { get; set; }
        [Display(Name = "Nombre:")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Contraseña actual:")]
        [DataType(DataType.Password)]        
        public string Contraseña { get; set; }
        [Required]
        [Display(Name = "Nueva contraseña:")]
        [DataType(DataType.Password)]
        public string NuevaContraseña { get; set; }
        [Required]
        [Display(Name = "Confirma contraseña:")]
        [DataType(DataType.Password)]
        public string ConfirmaContraseña { get; set; }       
        
        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}