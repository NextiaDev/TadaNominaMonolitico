using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ConfiguraConceptosM
    {

        public int idCosteoConceptoConfiguracion { get; set; }
        public int idUnidadNegocio { get; set; }
        public int idCosteosConcepto { get; set; }
        [Display(Name = "Tipo Nomina")]
        public int idCatTipoNomina { get; set; }
        public int idcosteo { get; set; }

        [Display(Name = "Ya configurado")]
        public int idconfig { get; set; }        

        public List<SelectListItem> LConfig { get; set; }

        [Display(Name = "Otro")]
        public int idOtro { get; set; }
        public List<SelectListItem> Lotro { get; set; }

        [Display(Name = "Concepto")]
        public string concepto { get; set; }
        public List<SelectListItem> Lconceptos { get; set; }
        public List<SelectListItem> LNomina { get; set; }

        [Display(Name = "Descripccion")]
        public string descripcion { get; set; }

        [Display(Name = "Operador")]
        public string operador { get; set; }
        public List<SelectListItem> lOperador { get; set; }

        [Required]
        [Display(Name = "Tipo de Concepto")]
        public string tipoConcepto { get; set; }
        public List<SelectListItem> lTipoConcepto { get; set; }
        public string descripcionValor { get; set; }
        public decimal valor { get; set; }
        public string operadorGral { get; set; }
        public List<SelectListItem> loperadorGral { get; set; }

        public int idEstatus { get; set; }
        public int idCaptura { get; set; }
        public DateTime fechaCaptura { get; set; }
        public int idModifica { get; set; }
        public DateTime fechaModifica { get; set; }
    }
}