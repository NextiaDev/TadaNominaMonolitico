using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelFondeosPeriodo
    {
        public int? IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
        public List<vCosteos_Fondeos> fondeos { get; set; }
    }
}