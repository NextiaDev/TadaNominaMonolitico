using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ConfiguraConceptos
    {
        public int? idCosteoConceptoConfiguracion { get; set; }
        public int idUnidadNegocio { get; set; }
        public int? idCosteosConcepto { get; set; }
        public string concepto { get; set; }
        public string descripcion { get; set; }
        public string operador { get; set; }
        public string tipoConcepto { get; set; }
        public string descripcionValor { get; set; }
        public decimal? valor { get; set; }
        public string operadorGral { get; set; }
        public int? idEstatus { get; set; }
        public int? idCaptura { get; set; }
        public DateTime? fechaCaptura { get; set; }
        public int? idModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
    }
}