using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Dispersion
{
    public class ModelPatrona
    {
        public Cat_RegistroPatronal registroPatronal { get; set; }
        public decimal saldo { get; set; }
        public decimal montoDispersar { get; set; }
    }
}