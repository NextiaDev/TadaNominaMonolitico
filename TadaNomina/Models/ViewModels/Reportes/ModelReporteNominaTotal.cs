using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteNominaTotal
    {
        public bool Bandera { get; set; }
        public string Nombre { get; set; }
        public decimal Total { get; set; }
    }
}