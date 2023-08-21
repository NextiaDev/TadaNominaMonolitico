using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mReporteIncidentes
    {
        public string RfcEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public List<string> Incidentes { get; set; }
    }
}