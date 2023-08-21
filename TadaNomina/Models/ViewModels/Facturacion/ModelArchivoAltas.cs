using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelArchivoAltas
    {
        public string Cliente { get; set; }
        public string UnidadNegocio { get; set; }
        public string ruta { get; set; }

        [Required]
        public HttpPostedFileBase file { get; set; }
        public string Observaciones { get; set; }
        public string Estatus { get; set; }
    }
}