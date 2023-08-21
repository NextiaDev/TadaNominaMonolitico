using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelPiramidacion
    {
        public int? IdPeriodoNomina { get; set; }  
        public string Periodo { get; set; }
        public int? IdConcepto { get; set; }        
        public List<SelectListItem> lConceptos { get; set; }
        public List<ModelPiramidados> Calculos { get; set; }
    }
}