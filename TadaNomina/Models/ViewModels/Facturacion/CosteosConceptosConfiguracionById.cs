using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class CosteosConceptosConfiguracionById
    {
        public int idCosteosConcepto { get; set; }
        public CosteosConceptosConfiguracionById()
        {
            CosteoConceptos = new List<ConfiguraConceptosM>();
        }
        public int idCosteo { get; set; }
        public IEnumerable<ConfiguraConceptosM> CosteoConceptos { get; set; }
    }
}