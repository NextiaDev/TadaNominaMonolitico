using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Facturacion.Costeo
{
    public class ModelCosteo
    {
        public string Titulo { get; set; }
        public string regPat { get; set; }
        public string sindicatos { get; set; }
        public string Cliente { get; set; }
        public string UnidadNegocio { get; set; }
        public string Facturadora { get; set; }
        public string RazonCliente { get; set; }
        public string ConceptoFacturacion { get; set; }
        public string Periodos { get; set; }
        public List<ModelDetalleCosteo> detalle { get; set; }
        public List<Costeos_Movimientos> lmovimientos { get; set; }
        public List<ModelPatronas> lPatronas { get; set; }
        public List<ModelSindicatos> lsindicato { get; set; }
        public Guid g { get; set; }

        public string Esquema { get; set; }
        public decimal? Fonacot { get; set; }
        public decimal? FonacotS { get; set; }
        public decimal? Pension { get; set; }
        public decimal? PensionS { get; set; }

        public List<Costeos_Fondeos> lConceptos { get; set; }
    }
}