using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado.ModelosJSON
{
    public class ModeloCancelacion
    {
        public string fechaTImbrado { get; set; }
        public string rfcEmisor { get; set; }
        public string noCertificado { get; set; }
        public string UUID { get; set; }
    }
}