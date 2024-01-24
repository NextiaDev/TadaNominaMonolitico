using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.ConfigUsuario
{
    public class MInfoUsuario
    {
        public int IdUsuario { get; set; }
        public string IdCliente { get; set; }
        public string IdUnidadNegocio { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Correo { get; set; }
        public string Usuario1 { get; set; }
        public string Nomina { get; set; }
        public string Rhcloud { get; set; }
        public string IMSS { get; set; }
        public string Contabilidad { get; set; }
        public string Tesoreria { get; set; }
    }
}