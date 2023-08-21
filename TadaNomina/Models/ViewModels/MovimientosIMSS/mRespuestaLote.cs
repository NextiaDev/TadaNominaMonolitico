using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mRespuestaLote
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public respuestaWebServiceRespuesta respuestaWebService { get; set; }
    }

    public class respuestaWebServiceRespuesta
    {
        public relacionMovimientos relacionMovimientos { get; set; }
        public resumenLotesRespuesta resumenLotes { get; set; }
    }
    public class relacionMovimientos
    {
        public string registroPatronal { get; set; }
        public string nombre { get; set; }
        public string bajaRechazado { get; set; }
        public string bajaAceptado { get; set; }
        public string modificacionRechazado { get; set; }
        public string modificacionAceptado { get; set; }
        public string reingresoRechazado { get; set; }
        public string reingresoAceptado { get; set; }
        public string recibo { get; set; }
    }

    public class resumenLotesRespuesta
    {
        public string fechaAcuseRecibo { get; set; }
        public string tipoLote { get; set; }
        public string fechaTrans { get; set; }
        public string idLote { get; set; }
    }
}