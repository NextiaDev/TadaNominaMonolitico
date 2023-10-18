using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelConceptos
    {
        [Key]
        public int IdConcepto { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        [Required]
        [Display(Name = "Clave Agrupadora:")]
        public string ClaveGpo { get; set; }
        public List<SelectListItem> LAgrupador { get; set; }
        [Required]
        [Display(Name = "Clave Concepto:")]
        public string ClaveConcepto { get; set; }
        [Required]
        [Display(Name = "Clave SAT:")]
        public string ClaveSAT { get; set; }
        [Required]
        [Display(Name = "Nombre Concepto:")]
        public string Concepto { get; set; }
        [Display(Name = "Información Concepto:")]
        [DataType(DataType.MultilineText)]
        public string Informacion { get; set; }
        [Required]
        [Display(Name = "Tipo de Concepto:")]
        public string TipoConcepto { get; set; }
        public IList<SelectListItem> LTipoConcepto { get; set; }
        [Required]
        [Display(Name = "Tipo de Dato:")]
        public string TipoDato { get; set; }
        public IList<SelectListItem> LTipoDato { get; set; }
        [Required]
        [Display(Name = "Tipo Esquema:")]
        public string TipoEsquema { get; set; }
        public IList<SelectListItem> LTipoEsquema { get; set; }
        [Required]
        [Display(Name = "¿Calcula Montos?:")]
        public string CalculaMontos { get; set; }
        public IList<SelectListItem> LCalculaMontos { get; set; }
        [Required]
        [Display(Name = "¿SD se multiplica por?:")]
        public decimal SDPor { get; set; }
        [Required]
        [Display(Name = "¿SD se divide entre?:")]
        public decimal SDEntre { get; set; }

        [Required]
        [Display(Name = "¿Afecta días de sueldo?:")]
        public string AfectaSueldo { get; set; }
        public IList<SelectListItem> LAfectaSueldo { get; set; }
        [Required]
        [Display(Name = "¿Afecta carga social?:")]
        public string AfectaCargaSocial { get; set; }
        public IList<SelectListItem> LAfectaCargaSocial { get; set; }
        [Display(Name = "¿Suma al neto final?")]
        public string sumaNetoFinal { get; set; }
        public List<SelectListItem> lSumaNeto { get; set; }
        [Required]
        [Display(Name = "¿Integra para la base gravada?:")]
        public string Integrable { get; set; }
        public IList<SelectListItem> LIntegra { get; set; }
        [Required]
        [Display(Name = "¿Integra para la variabilidad?:")]
        public string IntegraSDI { get; set; }
        public IList<SelectListItem> LIntegraSDI { get; set; }
        [Required]
        [Display(Name = "¿Exenta?:")]
        public string Exenta { get; set; }
        public IList<SelectListItem> LExenta { get; set; }
        [Display(Name = "Unidad por la que Exenta:")]
        public string UnidadExenta { get; set; }
        public IList<SelectListItem> LUExenta { get; set; }
        [Display(Name = "Cantidad de unidades que Exenta:")]
        public decimal CantidadExenta { get; set; }
        [Display(Name = "Porcentaje Gravado:")]
        public decimal PorcentajeGravado { get; set; }

        [Display(Name = "Exenta Por Unidad:")]
        public string ExcentaPorUnidad { get; set; }
        public List<SelectListItem> lExcentaPorUnidad { get; set; }


        [Display(Name ="¿Multiplica Por Días Trab?")]        
        public string MultiplicacDiasTrabajados { get; set; }
        public List<SelectListItem> lMultiplicaDiasTrabajados { get; set; }

        [Display(Name = "¿Crea Concepto Adicional?")]
        public string ConceptoAdicional { get; set; }
        public List<SelectListItem> lConceptoAdicional { get; set; }


        [Display(Name = "Conceptos:")]
        public string ClaveConceptos { get; set; }
        public List<SelectListItem> LClaveConcepto { get; set; }

        [Display(Name = "¿Considerar Factor y Valor?")]
        public string FactoryValor { get; set; }
        public List<SelectListItem> lFactoryValor { get; set; }

        [Display(Name = "Excenta SMGV 100%")]
        public string smgvalcien { get; set; }
        public List<SelectListItem> lsmgvalcien { get; set; }


        [Display(Name = "Piramidar")]
        public string Piramida { get; set; }
        public List<SelectListItem> lPiramidal { get; set; }       
        
        [Display(Name = "Pago en efectivo")]
        public string PagoEfectivo { get; set; }
        public List<SelectListItem> lPagoEfectivo { get; set; }

        
        [Display(Name = "¿El cálculo será por días u horas?")]
        public string DiasHoras { get; set; }
        public List<SelectListItem> lDiasHoras { get; set; }
    }
}