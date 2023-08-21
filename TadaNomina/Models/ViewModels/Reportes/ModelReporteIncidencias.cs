using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteIncidencias
    {
        public string Periodo { get; set; }
        public string ClaveEmpleado { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombre { get; set; }
        public string ClaveConcepto { get; set; }
        public string Concepto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Monto { get; set; }
        public decimal Exento { get; set; }
        public decimal Gravado { get; set; }
    }
}