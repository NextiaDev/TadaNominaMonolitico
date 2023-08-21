using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mDetalleRespuestaLote
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public respuestaWebServiceDetalleRespuestaModel respuestaWebService { get; set; }
    }

    public class respuestaWebServiceDetalleRespuestaModel
    {
        public loteModel lote { get; set; }
        public List<movimientosLoteModel> movimientosLote { get; set; }
    }
    public class loteModel
    {
        public string loteStatus { get; set; }
        public string tipoLote { get; set; }
        public string registroPatronal { get; set; }
        public string nombre { get; set; }
        public string fechaTrans { get; set; }
        public string idLote { get; set; }
    }

    public class movimientosLoteModel
    {
        public string causaBaja { get; set; }
        public string claveTrabajador { get; set; }
        public string codigoErrorMovimiento { get; set; }
        public string curp { get; set; }
        public string extemporaneo { get; set; }
        public string fecha_mov { get; set; }
        public string mensajeErrorMovimientos { get; set; }
        public string nombreAsegurado { get; set; }
        public string nombreSINDO { get; set; }
        public string nss { get; set; }
        public string registroPatronal { get; set; }
        public string salarioBase { get; set; }
        public string semanaJornadaRed { get; set; }
        public string tipo_mov { get; set; }
        public string tipo_trab { get; set; }
        public string tipoSalario { get; set; }
        public string umf { get; set; }
    }
}