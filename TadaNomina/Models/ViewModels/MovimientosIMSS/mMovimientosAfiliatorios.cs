using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mMovimientosAfiliatorios
    {
        public string Lote { get; set; }
        public string NombrePatrona { get; set; }
        public string ActividadEconomica { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombre { get; set; }
        public string Imss { get; set; }
        public string FechaMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public string FechaEnvio { get; set; }
        public string Origen { get; set; }
        public int IdRegistroPatronal { get; set; }
    }
}