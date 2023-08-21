using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Dispersion
{
    public class ModelDispersa
    {
        [Display(Name = "Periodo a Dispersar:")]
        public int IdPeriodo { get; set; }
        public List<SelectListItem> lPeriodos { get; set; } 
        public List<ModelPatrona> Saldos { get; set; }        
        public decimal TotalDispersar { get; set; }
        public int TotalMovimientos { get; set; }
        public List<Nomina> ListEmpleadosSinCuenta { get; set; }
    }
}