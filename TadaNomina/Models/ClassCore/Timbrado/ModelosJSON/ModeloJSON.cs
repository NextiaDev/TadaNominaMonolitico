using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloJSON
    {
        public string TIpo { get; set; }
        public string ComprobanteXmlBase64 { get; set; }
        public ModeloComprobanteJSON ComprobanteJson { get; set; }
        public string Version { get; set; }
    }
}