using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelCliente
    {
        public int IdCliente { get; set; }
        public string Cliente { get; set; }

        public int ClienteAdministrado { get; set; }
    }
}