using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class ConceptosChecadorModel
    {
        public int IdConceptoChecador { get; set; }

        [Required]
        [Display(Name ="Concepto")]
        public int IdConceptoNomina { get; set; }

        [Required]
        [Display(Name = "Concepto checador")]
        public string IdConceptoGV { get; set; }

        [Display(Name ="Descripcion Checador")]
        public string DescripcionGV { get; set; }
        public int IdEstatus { get; set; }
        public int IdCaptura { get; set; }
        public DateTime FechaCaptura { get; set; }
        public int IdModifica { get; set; }
        public DateTime FechaModifica { get; set; }

        ////////// Cat_ConceptosNomina
        ///////
        [Display(Name="Clave Concepto")]
        public string ClaveConcepto { get; set; }

        [Display(Name ="Clave SAT")]
        public string ClaveSAT { get; set; }
        public string Concepto { get; set; }
        public int IdCliente { get; set; }
        public bool Pagable { get; set; }
    }
}