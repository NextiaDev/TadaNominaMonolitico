using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.LayoutBancos
{
    public class ModelSantander
    {
        public string NumeroCuenta { get; set; }

        public string Nombre { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public string NetoPagar { get; set; }

        public List<SelectListItem> PeriodosNomina { get; set; }

        public string IdPeriodoNomina { get; set; }

        public string FechaHoy { get; set; }

        public int IdRegistroPatronal { get; set; }

        public string NombreCompleto { get; set; }

        public string NumeroCuentaInter { get; set; }

        public string ClaveTransfer { get; set; }
    }
}