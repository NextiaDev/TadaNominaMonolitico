using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mMovimientosSinRespuesta
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string RegistroPatronal { get; set; }
        public string TipoMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        public int? Envio { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Lote { get; set; }
        public int RespuestaLote { get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string IdEstatus { get; set; }
        public int IdCaptura { get; set; }
        public DateTime FechaCaptura { get; set; }
        public int IdModifica { get; set; }
        public DateTime FechaModifica { get; set; }
        public int IdMovimiento { get; set; }
        public int IdRegistroPatronal { get; set; }
        public string CodigoERROREnvio { get; set; }
        public string ErrorEnvio { get; set; }
        public string NombrePatrona { get; set; }
    }
}