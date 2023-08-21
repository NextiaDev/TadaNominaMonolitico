using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mCredencialesMovimientosIMSS
    {
        public int IdReg { get; set; }
        [Required]
        [Display(Name = "Archivo .pfx, .cer, o .p12")]
        public HttpPostedFileBase Archivo { get; set; }

        [Display(Name = "Archivo .key en caso de ser necesario")]
        public HttpPostedFileBase Key { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UsuarioIMSS { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string ContrasenaIMSS { get; set; }
    }
}