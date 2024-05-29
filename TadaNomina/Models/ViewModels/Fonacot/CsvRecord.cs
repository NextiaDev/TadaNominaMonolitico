using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Fonacot
{
    public class CsvRecord
    {
        public int idEmpleado { get; set; }

        public string ClaveEmpleado { get; set; }
        public string NoFonacot { get; set; }
        public string NoCredito { get; set; }
        public int Plazos { get; set; }
        public decimal CuotasPagadas { get; set; }
        public decimal RetencionMensual { get; set; }
        public string Activo { get; set; }
    }
}