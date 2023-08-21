using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mTimbradoResult
    {
        public string Referencia { get; set; }
        public string Fecha { get; set; }
        public string Contador { get; set; }
        public List<mFacturasResult> Facturas { get; set;}
        public mCifrasControl CifrasControl { get; set; }
    }
}