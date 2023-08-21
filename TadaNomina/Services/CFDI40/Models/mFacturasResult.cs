using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mFacturasResult
    {
        public int Contador { get; set; }
        public string XMLTimbrado { get; set; }
        public string ArchivoQR { get; set; }
        public string UUID { get; set; }
        public string FechaSello { get; set; }
        public string SelloDigital { get; set; }
        public string NumeroCertificado { get; set; }
        public mReporteIncidentes ReporteIncidentes { get; set; }
    }
}