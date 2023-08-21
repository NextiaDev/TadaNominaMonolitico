using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelMoverFiniquitos
    {
        public List<vNominaSelectFiniquito> calculos { get; set; }
        public int IdPeriodo { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
    }
}