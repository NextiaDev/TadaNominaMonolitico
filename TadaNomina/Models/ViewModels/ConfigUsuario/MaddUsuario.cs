using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.ConfigUsuario
{
    public class MaddUsuario
    {
        public int? IdUsuario { get; set; }
        [Required]
        public string IdCliente { get; set; }
        public string IdUnidadNegocio { get; set; }
        public int? IdEmpleado { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        [Required]
        public string correo { get; set; }
        [Required]
        public string Usuario { get; set; }
        public string Password { get; set; }
        public bool Nomina { get; set; }
        public bool RHCloud { get; set; }
        public bool IMSS { get; set; }
        public bool Contabilidad { get; set; }
        public bool Tesoreria { get; set; }

    }
}