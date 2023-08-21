using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloNominaDeduccionesDeduccion
    {
        public string TipoDeduccion { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
    }
}