using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mMovimientosEnviados
    {
        public List<mMovimientosIMSS> errores { get; set; }
        public List<mMovimientosIMSS> correctos { get; set; }
    }
}