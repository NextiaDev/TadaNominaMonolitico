using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mDetalleLote
    {
        public mRespuestaLote RespuestaGeneral { get; set; }
        public mDetalleRespuestaLote RespuestaDetalle { get; set; }
    }
}