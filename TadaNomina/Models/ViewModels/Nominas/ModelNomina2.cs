using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelNomina2
    {
        public int IdPeriodoNomina { get; set; }
        public List<ModelNomina> nomina { get; set; }
    }
}