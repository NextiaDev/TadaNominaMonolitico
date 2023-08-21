using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class CosteoModel
    {
        public int idCosteo { get; set; }
        public int? idCliente { get; set; }
        public int? idUnidadNegocio { get; set; }
        public string descripcion { get; set; }
        public decimal? cuotaFija { get; set; }
        public string tipoNomina { get; set; }
        public string tipoEsquema { get; set; }
        public string dividirPatronal { get; set; }
        public string separadoPor { get; set; }
        public int? agruparPorDescripcion { get; set; }
        public int? idEstatus { get; set; }
        public int idCaptura { get; set; }
        public DateTime? fechaCaptura { get; set; }
        public int? idModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
    }
}