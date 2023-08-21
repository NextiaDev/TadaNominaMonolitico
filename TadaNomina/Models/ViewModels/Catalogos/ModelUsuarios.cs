using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelUsuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        public string IdCliente { get; set; }
        public string Cliente { get; set; }
        public IList<SelectListItem> LCliente { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Contraseña { get; set; }
        [Required]
        public string Correo { get; set; }                
        [Required]
        [Display(Name = "Tipo Usuario")]        
        public string TipoUsuario { get; set; }
        public IList<SelectListItem> LTipoUsuario { get; set; }

        public string UnidadesAcceso { get; set; }
        public string ClientesAcceso { get; set; }
        public string Foto { get; set; }
        

        public bool Validacion { get; set; }
        public string Mensaje { get; set; }
    }
}