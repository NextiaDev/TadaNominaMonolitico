using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelNewPagoAdicional
    {
        public string Comentarios { get; set; }
        public HttpPostedFileBase Archivo { get; set; }        
    }
}