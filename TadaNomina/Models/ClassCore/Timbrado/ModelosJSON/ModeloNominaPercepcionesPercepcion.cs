using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloNominaPercepcionesPercepcion
    {
        public string TipoPercepcion { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public decimal ImporteGravado { get; set; }
        public decimal ImporteExento { get; set; }
        public ModeloHorasExtra[] HorasExtra { get; set; }
    }
}