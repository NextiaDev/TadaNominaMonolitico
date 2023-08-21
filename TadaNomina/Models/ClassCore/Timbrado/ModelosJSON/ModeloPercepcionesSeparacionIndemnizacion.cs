using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloPercepcionesSeparacionIndemnizacion
    {
        public decimal TotalPagado { get; set; }
        public decimal NumAniosServicio { get; set; }
        public decimal UltimoSueldoMensOrd { get; set; }
        public decimal IngresoAcumulable { get; set; }
        public decimal IngresoNoAcumulable { get; set; }
    }
}