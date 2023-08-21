using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelNewSaldo
    {
        public int idEmpleado { get; set; }        
        public int IdConcepto { get; set; }
        public List<SelectListItem> lConceptos { get; set; }
        public decimal saldoInicial { get; set; }
        public decimal saldoActual { get; set; }
        public decimal descuentoPeriodo { get; set; }
        public decimal numeroPeriodos { get; set; }  
    }
}