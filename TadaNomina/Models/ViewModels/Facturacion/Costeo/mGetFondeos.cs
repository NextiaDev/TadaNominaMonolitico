using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion.Costeo
{
    public class mGetFondeos
    {
        public int IdUnidadNegocio { get; set; }
        public int[] IdPeriodoNomina { get; set; }
        public int IdCosteo { get; set; }
    }
}