using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.LayoutBancos
{
    public class MBanorte
    {
        public int IdRegistroPatronal { get; set; }

        public int IdEmpleado { get; set; }

        public string NumeroCuenta { get; set; }

        public string NumeroCuentaInter { get; set; }

        public int IdBanco { get; set; }

        public string ClaveBanco { get; set; }

        public string FechaDispersion { get; set; }

        public string Nombre { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public decimal? NetoPagar { get; set; }

        public List<SelectListItem> PeriodosNomina { get; set; }

        public string IdPeriodoNomina { get; set; }

        public decimal TotalNomina { get; set; }

        public string Empresa { get; set; }
    }
}