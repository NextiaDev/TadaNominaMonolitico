using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion.Costeo
{
    public class ModelDetalleCosteo
    {
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public string conceptoFacturacion { get; set; }
    }
}