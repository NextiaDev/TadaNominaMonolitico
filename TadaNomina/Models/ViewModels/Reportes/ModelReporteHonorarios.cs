using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ServiceStack.LicenseUtils;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteHonorarios
    {

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Inicial")]
        public string fInicial { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Fecha Final")]
        public string fFinal { get; set; }



        public string Estatus { get; set; }
        public List<SelectListItem> LEstatus { get; set; }
    }
}