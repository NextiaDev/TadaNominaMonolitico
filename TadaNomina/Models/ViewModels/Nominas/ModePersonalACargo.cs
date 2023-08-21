using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModePersonalACargo
    {
        public int IdEmpleado { get; set; }
        public string Cve { get; set; }
        public string Nombre { get; set; }
        public bool check { get; set; }
    }
}