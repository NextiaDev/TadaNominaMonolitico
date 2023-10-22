using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class mGetReporte
    {
        public string User { get; set; }
        public string Token { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public int IdPeriodoNomina { get; set; }
        public int? IdRegistroPatronal { get; set; }
        public string RFC { get; set; }
    }
}