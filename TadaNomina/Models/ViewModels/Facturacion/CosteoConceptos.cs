using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class CosteoConceptos
    {
        public int idCosteosConcepto { get; set; }
        public int? idCosteo { get; set; }
        public string descripcion { get; set; }
        public string tipoDatoFacturacion { get; set; }
        public string observaciones { get; set; }
        public int? orden { get; set; }
        public string visible { get; set; }
        public int? idEstatus { get; set; }
        public int? idCaptura { get; set; }
        public DateTime? fechaCaptura { get; set; }
        public int? idModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
    }
}