using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloNominaDeducciones
    {
        public decimal TotalOtrasDeducciones { get; set; }
        public bool TotalOtrasDeduccionesSpecified { get; set; }
        public decimal TotalImpuestosRetenidos { get; set; }
        public bool TotalImpuestosRetenidosSpecified { get; set; }
        public ModeloNominaDeduccionesDeduccion[] Deduccion { get; set; }
    }
}