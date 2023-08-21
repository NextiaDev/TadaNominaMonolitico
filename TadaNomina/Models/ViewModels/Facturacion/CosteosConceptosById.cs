using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class CosteosConceptosById
    {
        public int IdCosteo { get; set; }
        public CosteosConceptosById()
        {
            CosteoConceptos = new List<CosteoConceptosM>();
        }
        public int idCosteo { get; set; }
        public IEnumerable<CosteoConceptosM> CosteoConceptos { get; set; }
    }
}