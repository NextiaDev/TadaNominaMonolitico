using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.LayoutBancos
{
    public class ModelBancomer
    {
        [Display(Name = "Número de cuenta")]
        public string NumeroCuenta { get; set; }

        [Display(Name = "Neto a pagar")]
        public string NetoPagar { get; set; }

        [Display(Name = "Nombre empleado")]
        public string NombreCompleto { get; set; }

        public List<SelectListItem> PeriodosN { get; set; }

        public string IdPeriodoNomina { get; set; }

        public int IdRegistroPatronal { get; set; }

        public string TipoNomina { get; set; }

        public string ClaveTransfer { get; set; }

        public int ClaveBanco { get; set; }

    }
}