using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloCertificados
    {
        public string certificadoBase64 { get; set; }
        public string llaveBase64 { get; set; }
        public string password { get; set; }
    }
}