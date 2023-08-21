using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class Faltas
    {
        public int? IdEmpleado { get; set; }
        public decimal? TotalFaltas { get; set; }
        public decimal? TotalIncapacidades { get; set; }
        public decimal? TotalPermisos { get; set; }
        public int? IdPeriodoNomina { get; set; }
    }
}