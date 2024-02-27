using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelEmpleadosNetos
    {
        public int IdEmpleado {  get; set; }
        public string ClaveEmpleado { get; set; }
        public string Nombre { get; set; }
        public decimal? Neto {  get; set; }
    }
}
