using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelConceptosFiniquito
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Cliente")] 
        public int IdCliente { get; set; }
        [Display(Name = "Concepto para vacaciones:")]
        public int? IdConceptoVacaciones { get; set; }
        public List<SelectListItem> lConceptoVacaciones { get; set; }
        [Display(Name = "Concepto para prima vacacional:")]
        public int? IdConceptoPV { get; set; }
        public List<SelectListItem> lConceptoPV { get; set; }
        [Display(Name = "Concepto para aguinaldo:")]
        public int? IdConceptoAguinaldo { get; set; }
        public List<SelectListItem> lConceptoAguinaldo { get; set; }
        [Display(Name = "Concepto para 90 días de indemnización:")]
        public int? IdConcepto3M { get; set; }
        public List<SelectListItem> lConcepto3M { get; set; }
        [Display(Name = "Concepto para 20 días por año:")]
        public int? IdConcepto20D { get; set; }
        public List<SelectListItem> lConcepto20D { get; set; }
        [Display(Name = "Concepto para prima de antiguedad:")]
        public int? IdConceptoPA { get; set; }
        public List<SelectListItem> lConceptoPA { get; set; }
        [Display(Name = "Concepto para Fonacot:")]
        public int? IdConceptoFonacot { get; set; }
        public List<SelectListItem> lConceptoFonacot { get; set; }
        [Display(Name = "Concepto para Credito de Vivienda:")]
        public int? IdConceptoInfonavit { get; set; }
        public List<SelectListItem> lConceptoInfonavit { get; set; }
        [Display(Name = "Concepto para Pensión Alimenticia:")]
        public int? IdConceptoPensionAlimenticia { get; set; }
        public List<SelectListItem> lConceptoPensionAlimenticia { get; set; }

        [Display(Name = "Concepto para Cuota Sindical:")]
        public int? IdConceptoCuotaSindical { get; set; }
        public List<SelectListItem> lConceptoCuotaSindical { get; set; }

        [Display(Name = "Concepto para Subsidio Ausentismos:")]
        public int? IdConceptoSubAusentismo { get; set; }
        public List<SelectListItem> lConceptoSubAusen { get; set; }


        [Display(Name = "Concepto para SeptimoDia:")]
        public int? IdConceptoSeptimoDia { get; set; }
        public List<SelectListItem> lConceptoSeptimoDias { get; set; }



        [Display(Name = "Concepto para Compensación(Piramidación):")]
        public int? IdConceptoCompensacion { get; set; }
        public List<SelectListItem> lConceptoCuotaCompensacion { get; set; }

        [Display(Name = "Concepto para Faltas(Piramidación):")]
        public int? IdConceptoFaltas { get; set; }
        public List<SelectListItem> lConceptoFaltas { get; set; }

        [Display(Name = "Concepto para Art 93 Frac lll(Piramidación):")]
        public int? IdConceptoArt93Fraclll { get; set; }
        public List<SelectListItem> lConceptoArt93Fraclll { get; set; }


        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}