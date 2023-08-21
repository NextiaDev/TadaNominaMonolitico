using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloNominaOtrosPagos
    {
        public string TipoOtroPago { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public ModeloNominaOtrosPagosSubsidioEmpleo SubsidioAlEmpleo { get; set; }
    }
}