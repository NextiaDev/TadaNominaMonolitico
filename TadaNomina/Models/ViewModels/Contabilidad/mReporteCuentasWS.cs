using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class mReporteCuentasWS
    {
        public string valor1 { get; set; }
        public string cuenta { get; set; }
        public string valor2 { get; set; }
        public int? Tipo { get; set; }
        public decimal? Importe { get; set; }
        public decimal? valor3 { get; set; }
        public decimal valor4 { get; set; }
        public string Periodo { get; set; }
        public string CentroCostos { get; set; }
        public string Descripcion { get; set; }
        public string suc { get; set; }
        public string CC { get; set; }
    }
}