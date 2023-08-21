using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mEmisiones
    {
        public string tipoemision { get; set; }
        public List<SelectListItem> tiposArchivos { get; set; }
        public Cat_RegistroPatronal infoRegistro { get; set; }
    }
}