using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.LayoutBancos
{
    public class ModelBanamex
    {
        public int IdBancoEmpleado { get; set; }
        public string NumCliente { get; set; }

        public string NombreEmpresa { get; set; }

        public string MontoTotal { get; set; }

        public string NumeroSucursal { get; set; }

        public string CuentaCargo { get; set; }

        public string TotalEmpleados { get; set; }

        public string NombreUnidadNegocio { get; set; }

        public string IdPeriodoNomina { get; set; }

        public string Nombre { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public string NetoPagar { get; set; }

        public List<SelectListItem> PeriodosNomina { get; set; }

        public string FechaHoy { get; set; }

        public int IdRegistroPatronal { get; set; }

        public string NombreCompleto { get; set; }

        public string NumeroCuentaInter { get; set; }

        public string ClaveBanco { get; set; }

        public string NumeroCuenta { get; set; }

        public DateTime FechaDispersion { get; set; }

        public string ContratoBancario { get; set; }
    }
}