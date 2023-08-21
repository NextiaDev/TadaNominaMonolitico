using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelImpuestoSat
    {
        public int IdImpuesto { get; set; }

        [Display(Name = "Tipo De Nómina:")]
        public string TipoNomina { get; set; }

        //[Required]
        [Display(Name = "Tipo De Nómina:")]
        public Nullable<int> IdTipoNomina { get; set; }

        [Required]
        [Display(Name = "Límite Inferior:")]
        public decimal LimiteInferior { get; set; }

        [Required]
        [Display(Name = "Límite Superior:")]
        public decimal LimiteSuperior { get; set; }

        [Required]
        [Display(Name = "Cuota Fija:")]
        public decimal CuotaFija { get; set; }

        [Required]
        [Display(Name = "Porcentaje:")]
        public decimal Porcentaje { get; set; }

        [Display(Name = "Fecha Inicio:")]
        public string FechaInicio { get; set; }



        public List<SelectListItem> LTipoNomina { get; set; }        

        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
        public string Mensaje { get; set; }
        public bool validacion { get; set; }


    }
}