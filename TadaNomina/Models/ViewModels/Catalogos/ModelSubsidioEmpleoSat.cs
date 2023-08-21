using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelSubsidioEmpleoSat
    {
        public int IdSubsidio { get; set; }
        public int IdTipoNomina { get; set; }
        public decimal LimiteInferior { get; set; }
        public decimal LimiteSuperior { get; set; }
        public decimal CreditoSalario { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public int IdEstatus { get; set; }

        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
        public string Mensaje { get; set; }
        public bool validacion { get; set; }

        public List<SelectListItem> LTipoNomina { get; set; }
    }
}
