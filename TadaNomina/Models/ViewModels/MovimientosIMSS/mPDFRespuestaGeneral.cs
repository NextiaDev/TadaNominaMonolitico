using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mPDFRespuestaGeneral
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public RespuestaWebServiceRespuestaPDFModel RespuestaWebService { get; set; }
    }

    public class RespuestaWebServiceRespuestaPDFModel
    {
        public archivoReporteDetalladoModel archivoReporteDetallado { get; set; }
    }

    public class archivoReporteDetalladoModel
    {
        public string archivoReporteDetallado { get; set; }
        public string nombreArchivoReporteDetallado { get; set; }
    }
}