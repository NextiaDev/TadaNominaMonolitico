using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mRespuestaEmisiones
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public respuestaWebServiceEmisionModel respuestaWebService { get; set; }
    }

    public class respuestaWebServiceEmisionModel
    {
        public Descarga descarga { get; set; }
        public descargaDiscoModel descargaDisco { get; set; }
        public DescargaEMA descargaEMA { get; set; }
        public DescargaEBA descargaEBA { get; set; }
        public DescargaEmaSua descargaEmaSua { get; set; }
        public DescargaEbaSUA descargaEbaSUA { get; set; }
        public DescargaEmaPDF descargaEmaPDF { get; set; }
        public DescargaEbaPDF descargaEbaPDF { get; set; }
        public DescargaEXCEL descargaEXCEL { get; set; }
        public PeriodoEba periodoEba { get; set; }
        public RespuestaPeriodo respuestPeriodo { get; set; }
    }
    public class descargaDiscoModel
    {
        public string archivoEmisionDisco { get; set; }
        public string descargaDisco { get; set; }
        public string folioNotaria { get; set; }
        public string nombreArchivoEmisionDisco { get; set; }
        public string periodoAnual { get; set; }
        public string periodoMensual { get; set; }
        public string recibo { get; set; }
    }

    public class Descarga
    {
        public string timestamp { get; set; }
        public string periodoAnual { get; set; }
        public string periodoMensual { get; set; }
        public string registroPatronal { get; set; }
        public string tipoEmis { get; set; }
    }

    public class DescargaEMA
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }

    public class DescargaEBA
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }

    public class DescargaEmaSua
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }

    public class DescargaEbaSUA
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }

    public class DescargaEmaPDF
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }

    public class DescargaEbaPDF
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }
    public class DescargaEXCEL
    {
        public string tamanoArchivo { get; set; }
        public string estadoDescarga { get; set; }
    }

    public class PeriodoEba
    {
        public string periodoAnualEba { get; set; }
        public string periodoMensualEba { get; set; }
    }

    public class RespuestaPeriodo
    {
        public string periodoAnual { get; set; }
        public string periodoMensual { get; set; }
    }
}