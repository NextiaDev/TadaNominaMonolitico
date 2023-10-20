using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelCatSindicatosClientes
    {
        public int idSincato { get; set; }
        public int idCliente { get; set; }
        public string Sindicato { get; set; }
        public decimal CuotaSindical { get; set; }
    }
}