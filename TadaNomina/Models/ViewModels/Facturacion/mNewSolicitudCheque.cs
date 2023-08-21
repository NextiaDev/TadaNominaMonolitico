using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class mNewSolicitudCheque
    {

        [Required]
        [Display(Name ="Periodo")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
        [Required]
        public HttpPostedFileBase archivo { get; set; }
        public string Observaciones { get; set; }
    }
}