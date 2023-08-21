using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels
{
    public class EmpleadoPeriodo
    {
        public int IdPeriodoNomina { get; set; }
        public string Periodo { get; set; }
        public bool Validacion { get; set; }
    }
}