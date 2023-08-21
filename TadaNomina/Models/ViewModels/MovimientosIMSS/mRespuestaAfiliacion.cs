using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mRespuestaAfiliacion
    {
        public string estado { get; set; }
        public string mensaje { get; set; }
        public RespuestaWebServiceAfiliacionModel RespuestaWebService { get; set; }
    }

    public class RespuestaWebServiceAfiliacionModel
    {
        public reporteLoteAfiliacionModel reporteLote { get; set; }
        public List<movRechazadoModel> movRechazado { get; set; }
    }

    public class reporteLoteAfiliacionModel
    {
        public string folioFirma { get; set; }
        public string idLote { get; set; }
        public string secuenciaNotariaRecibido { get; set; }
        public string recibo { get; set; }
        public string fechaTrans { get; set; }
    }

    public class movRechazadoModel
    {
        public string registroPatronal { get; set; }
        public string digitoVerificadorRP { get; set; }
        public string nss { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombreAsegurado { get; set; }
        public string sdi { get; set; }
        public string tipo_trab { get; set; }
        public string tipoSalario { get; set; }
        public string semanaJornadaRed { get; set; }
        public string fecha_mov { get; set; }
        public string unidadMedicinaFam { get; set; }
        public string tipo_mov { get; set; }
        public string timestampEnvio { get; set; }
        public string claveTrabajador { get; set; }
        public string causaBaja { get; set; }
        public string curp { get; set; }
        public string idFormato { get; set; }
        public string codigoErrorMovimiento { get; set; }
    }
}