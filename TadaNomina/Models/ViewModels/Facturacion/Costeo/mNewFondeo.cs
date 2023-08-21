using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion.Costeo
{
    public class mNewFondeo
    {
        public int? IdFondeo { get; set; }
        public int? IdPeriodoNomina { get; set; }
        public int? IdUnidadNegocio { get; set; }
        public int? IdCosteo { get; set; }
        public int? IdPatrona { get; set; }
        public int? IdDivision { get; set; }
        public string Esquema { get; set; }
        public string Descripcion { get; set; }
        public decimal Importe { get; set; }
        public string Conceptos { get; set; }
    }
}