using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelConfEmpleadoHoras
    {
        public vEmpleados empleado { get; set; }
        public List<ModePersonalACargo> empleados { get; set; }
        public decimal CuotaFija { get; set; }
        public decimal CostoxHora { get; set; }
        public decimal MetaHoras { get; set; }
        public string TipoBono { get; set; }
        public List<SelectListItem> lTipoBono { get; set; }
        public decimal Bono { get; set; }
        public List<ModelMaterias> Materias { get; set; }

    }
}