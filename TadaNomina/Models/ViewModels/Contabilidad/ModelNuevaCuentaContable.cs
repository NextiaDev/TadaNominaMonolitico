using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class ModelNuevaCuentaContable
    {
        public int? IdReferencia { get; set; }
        public int? Nivel { get; set; }
        [Display(Name = "Id Registro")]
        public int? IdCuentaCliente { get; set; }
        public int? IdRegistroPatronal { get; set; }
        public List<SelectListItem> lRegistroPatronal { get; set; }
        public string _Descripcion { get; set; }
        [Required]
        [Display(Name = "Cve Contable")]
        public string Clave { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Tipo de Concepto")]
        public int? IdTipoNomina { get; set; }
        public List<SelectListItem> lTipoNomina { get; set; }

        [Display(Name = "Concepto")]
        public string Concepto { get; set; }
        public List<SelectListItem> lConceptos { get; set; }
        public int? IdTipoCuenta { get; set; }
        [Display(Name = "Tipo Cuenta")]
        public List<SelectListItem> lTipoCenta { get; set; }
    }
}