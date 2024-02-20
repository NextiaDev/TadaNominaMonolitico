using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelDeleteCliente
    {
        public int IdCliente { get; set; }
        public DateTime FechaBaja { get; set; }
        public string MotivoBaja { get; set; }
    }
}