using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelPiramidados
    {   
        public int IdConceptoPiramido { get; set; }
        public int? IdEmpleado { get; set; }
        public string ClaveEmpleado { get; set; }
        public string Nombre { get; set; }
        public int? IdConcepto { get; set; }
        public string Concepto { get; set; }
        public decimal? DiasPago { get; set; }
        public decimal? SD { get; set; }
        public decimal? SMB { get; set; }
        public decimal? Importe { get; set; }
        public decimal? Neto { get; set; }
        public decimal? ISR_SMO { get; set; }
        public decimal? ISR_Total { get; set; }
        public decimal? ISR_Cobrar { get; set; }
    }
}