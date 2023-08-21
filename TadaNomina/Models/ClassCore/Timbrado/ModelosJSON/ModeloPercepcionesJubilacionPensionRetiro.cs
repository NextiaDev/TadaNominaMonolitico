using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloPercepcionesJubilacionPensionRetiro
    {
        public decimal TotalUnaExhibicion { get; set;}
        public bool TotalUnaExhibicionSpecified { get; set; }
        public decimal TotalParcialidad { get; set; }
        public bool TotalParcialidadSpecified { get; set; }
        public decimal MontoDiario { get; set; }
        public bool MontoDiarioSpecified { get; set; }
        public decimal IngresoAcumulable { get; set; }
        public bool IngresoAcumulableSpecified { get; set; }        
    }
}