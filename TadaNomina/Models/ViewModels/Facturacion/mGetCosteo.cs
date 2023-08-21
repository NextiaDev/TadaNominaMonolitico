using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class mGetCosteo
    {
        public int[] IdPeriodoNomina { get; set; }
        public string DescripcionPeriodos { get; set; }
        public int IdCosteo { get; set; }
        public int IdCliente { get; set; }
        public int IdUnidadNegocio { get; set; }
        public string ClienteUnidad { get; set; }
        public List<ModelFactDif> fact { get; set; }
    }
}