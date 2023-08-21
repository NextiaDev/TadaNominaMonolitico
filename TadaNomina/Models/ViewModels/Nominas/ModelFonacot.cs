using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelFonacot
    {
        [Key]
        public int IdCreditoFonacot { get; set; }
        public int IdEmpleado { get; set; }

        [Display(Name = "Nombre.")]
        public string NombreEmpleado { get; set; }
        [Display(Name = "R.F.C.")]
        public string RFC { get; set; }

        [Display(Name = "Cve. Empleado:")]
        public string ClaveEmp { get; set; }

        public string NombrePatrona { get; set; }

        [Display(Name ="No. FONACOT:")]
        public string NoTrabajadorFonacot { get; set; }

        [Display(Name = "No. Crédito:")]
        public string NumeroCredito { get; set; }

        public int Plazos { get; set; }

        [Display(Name = "Cuotas Pagadas:")]
        public decimal CuotasPagadas { get; set; }

        [Display(Name = "Ret. Mensual:")]
        public decimal RetencionMensual { get; set; }

        public int IdEstatus { get; set; }
        public bool Validacion { get; set; }
        public string Mensaje { get; set; }
    }

}