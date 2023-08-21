using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mPDFDetalleRrspuesta
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public respuestaWebServiceAcuseIMSSModel respuestaWebService { get; set; }
    }

    public class respuestaWebServiceAcuseIMSSModel
    {
        public archivoReciboDispmagModel ArchivoReciboDispmag { get; set; }
    }

    public class archivoReciboDispmagModel
    {
        public string archivoReciboDispmag { get; set; }
        public string recibi { get; set; }
    }
}