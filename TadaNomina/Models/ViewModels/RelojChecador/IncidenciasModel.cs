using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class IncidenciasModel
    {
        public string Identifier { get; set; }
        public int Concepto { get; set; }
        public double Cantidad { get; set; }
    }
}