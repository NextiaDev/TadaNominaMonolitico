using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion.Costeo
{
    public class ModelPatronas
    {
        public int IdRegistroPatronal { get; set; }
        public string Nombre { get; set; }
        public decimal Monto { get; set; }
    }
}