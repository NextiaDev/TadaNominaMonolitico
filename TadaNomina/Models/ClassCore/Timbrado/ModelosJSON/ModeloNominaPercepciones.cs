using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloNominaPercepciones
    {
        public decimal TotalSueldos { get; set; }
        public bool TotalSueldosSpecified { get; set; }
        public string TotalSeparacionIndemnizacion { get; set; }
        public bool TotalSeparacionIndemnizacionSpecified { get; set; }
        public string TotalJubilacionPensionRetiro { get; set; }
        public bool TotalJubilacionPensionRetiroSpecified { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalExento { get; set; }
        public ModeloNominaPercepcionesPercepcion[] Percepcion { get; set; }
        public ModeloPercepcionesJubilacionPensionRetiro JubilacionPensionRetiro { get; set; }
        public ModeloPercepcionesSeparacionIndemnizacion SeparacionIndemnizacion { get; set; }
    }
}