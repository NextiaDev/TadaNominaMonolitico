using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelCSD
    {
        [Required]
        [Display(Name="Archivo .Cer:")]
        public HttpPostedFileBase ArchivoCer { get; set; }
        [Required]
        [Display(Name = "Archivo .Key:")]
        public HttpPostedFileBase ArchivoKey { get; set; }
        [Required]
        [Display(Name = "Contraseña:")]
        [DataType(DataType.Password)]
        public string pass { get; set; }
    }
}