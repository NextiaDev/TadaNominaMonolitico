using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class ModelIndex
    {
        public int IdRegistroPatronal { get; set; }
        public List<SelectListItem> lRegistros { get; set; }
        public List<CuentasModel> lcuentas { get; set; }
    }
}