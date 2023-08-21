using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelPatronasDivision
    {
        public List<SelectListItem> lDivision { get; set; }
        public List<SelectListItem> lPatrona { get; set; }
    }
}