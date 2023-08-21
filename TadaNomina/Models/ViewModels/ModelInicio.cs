using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ViewModels
{
    public class ModelInicio
    {
        public int CantidadEmpleadosActivos { get; set; }
        public int CantidadPeriodosProcesados { get; set; }
        public decimal empTradicionales { get; set; }
        public decimal empEsquema { get; set; }
        public decimal empMixto { get; set; }

        //nomina trabajo
        public string nombrePer { get; set; }
        public int EmpleadosProcesados { get; set; }
        public string Neto { get; set; }
        public string NetoS { get; set; }
        public string IMSS { get; set; }
        public string ISR { get; set; }

        //nomina
        public string nombrePerC { get; set; }
        public int EmpleadosProcesadosC { get; set; }
        public string NetoC { get; set; }
        public string NetoSC { get; set; }
        public string IMSSC { get; set; }
        public string ISRC { get; set; }

        public int PeridosProcesados { get; set; }
        public ModelDetalleTimbradoInicio detalleTimbradoInicio { get; set; }
    }
}