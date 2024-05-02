using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelPensionAlimenticia
    {
        [Key]
        public int IdPensionAlimenticia { get; set; }
        public int IdEmpleado { get; set; }
        [Display(Name = "Cve.")]
        public string ClaveEmp { get; set; }
        [Display(Name = "Nombre")]
        public string Empleado { get; set; }
        [Display(Name = "R.F.C.")]
        public string rfc { get; set; }

        public string TipoPension { get; set; }

        [Display(Name = "Tipo")]
        public List<SelectListItem> lTipo { get; set; }

        [Display(Name = "Base de Calculo")]
        public int BaseCalculo { get; set; }

        [Display(Name = "Base de Calculo")]
        public List<SelectListItem> lTipoBase { get; set; }


        [Display(Name = "Valor")]
        public decimal? Valor { get; set; }
        [Display(Name = "ValorEsq")]
        public decimal? ValorEsq { get; set; }
        [Display(Name = "Beneficiari@")]
        public string NombreBeneficiario { get; set; }
        public int? IdEstatus { get; set; }
        public bool Estatus { get; set; }
        [Display(Name = "f. Captura")]
        public DateTime? fechaCaptura { get; set; }

        public bool Validacion { get; set; }
        public string Mensaje { get; set; }

        [Display(Name = "¿El crédito estará activo?")]
        public bool Activo { get; set; }
    }
}