using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mTimbrar
    {       
        public string Referencia { get; set; }
        public string Fecha { get; set; }
        public List<mFacturas> Facturas { get; set; }  
    }
}