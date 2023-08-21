using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelDetalleTimbradoInicio
    {
        public string UnidadNegocio { get; set; }
        public int? Nomina { get; set; }
        public int? Timbrados { get; set; }
        public int? NoTimbrados { get; set; }
    }
}