using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.ConfigUsuario
{
    public class MIndexUsuario
    {
        public List<MInfoUsuario> Usuarios { get; set; }
        public List<Cat_Clientes> ClientesToAsign { get; set; }
        public List<Cat_UnidadNegocio> UnidadNegocioToAsign { get; set; }
        public string Result { get; set; }
        public string MensajeResult { get; set; }
    }
}