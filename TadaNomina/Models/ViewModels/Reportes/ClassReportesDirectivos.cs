using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ClassReportesDirectivos
    {
        public int NumEmpleados { get; set; }
        public decimal TotalPercep { get; set; }
        public decimal TotalPercepEsq { get; set; }
        public decimal ImssObrero { get; set; }
        public decimal TotalPatron { get; set; }
        public decimal ISN { get; set; }
        public decimal ISR { get; set; }
        public decimal TotalDeduc { get; set; }
        public decimal TotalApoyo { get; set; }
    }
}