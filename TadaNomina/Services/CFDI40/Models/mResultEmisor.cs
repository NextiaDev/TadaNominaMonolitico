using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mResultEmisor
    {
        public string RFCEmisor { get; set; }
        public string RazonSocial { get; set; }
        public string Cliente { get; set; }
        public string Valido { get; set; }
        public List<mErrores> Errores { get; set; }
    }
}