using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelDescargaCFDI
    {
        public int? _IdPeriodo { get; set; }
        [Required]
        [Display(Name = "Seleccione el Periodo:")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }

        [Display(Name = "Seleccione el formato:")]
        public string TipoArchivo { get; set; }
        public List<SelectListItem> lTipoArchivo { get; set; }

        [Display(Name = "Dividir Por:")]
        public string DividirPor { get; set; }
        public List<SelectListItem> lDividir { get; set; }

        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}