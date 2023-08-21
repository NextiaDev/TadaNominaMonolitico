using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelAcumularPeriodo
    {
        public int Id { get; set; }
        public decimal? isr { get; set; }
        public decimal? cargaObrera { get; set; }
        public decimal? cargaPatronal { get; set; }
        public decimal? totalPerc { get; set; }
        public decimal? totalDed { get; set; }
        public decimal? neto { get; set; }
    }
}