using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloNominaIncapacidades
    {
        public int DiasIncapacidad { get; set; }
        public string TipoIncapacidad { get; set; }
        public decimal ImporteMonetario { get; set; }
        public bool ImporteMonetarioSpecified { get; set; }
    }
}