using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Inicio
{
    public class ModelSelecionarNomina
    {
        [Required]
        [Display(Name = "Cliente")]
        public int? IdCliente { get; set; }
     
        [Display(Name = "Unidad de Negocio")]
        public int? IdunidadNegocio { get; set; }
        public IList<SelectListItem> cliente { get; set; }
        public IList<SelectListItem> unidadNegocio { get; set; }
    }
}