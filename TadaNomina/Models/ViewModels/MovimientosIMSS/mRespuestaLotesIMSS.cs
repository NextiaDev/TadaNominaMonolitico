using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mRespuestaLotesIMSS
    {
        public string idLote { get; set; }
        public string tipoLote { get; set; }
        public string loteStatus { get; set; }
        public string fechaTrans { get; set; }
    }

    public class respuestaWebService
    {
        public List<mRespuestaLotesIMSS> resumenLotes { get; set; }
    }

    public class RespuestaLotesIMSS
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public respuestaWebService RespuestaWebService { get; set; }
    }
}