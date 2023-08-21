using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado.ModelosJSON
{
    public class ModeloItemsSCB
    {
        public string ObjectType { get; set; }
        public string Version { get; set; }
        public string TipoNomina { get; set; }
        public string FechaPago { get; set; }
        public string FechaInicialPago { get; set; }
        public string FechaFinalPago { get; set; }
        public decimal NumDiasPagados { get; set; }
        public decimal TotalPercepciones { get; set; }
        public bool TotalPercepcionesSpecified { get; set; }
        public decimal TotalDeducciones { get; set; }
        public bool TotalDeduccionesSpecified { get; set; }
        public decimal TotalOtrosPagos { get; set; }
        public bool TotalOtrosPagosSpecified { get; set; }
        public ModeloNominaEmisor Emisor { get; set; }
        public ModeloNominaReceptorSBC Receptor { get; set; }
        public ModeloNominaPercepciones Percepciones { get; set; }
        public ModeloNominaDeducciones Deducciones { get; set; }
        public ModeloNominaOtrosPagos[] OtrosPagos { get; set; }
        public ModeloNominaIncapacidades[] Incapacidades { get; set; }
    }
}