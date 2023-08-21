using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelEmpleadoFiniquito
    {
        [Key]
        public int IdEmpleado { get; set; }
        [Key]
        public int IdPeriodoNomina { get; set; }
        [Display(Name ="Cve.")]
        public string ClaveEmpleado { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "RFC")]
        public string RFC { get; set; }
        [Display(Name = "Vac.")]
        public bool Liquidacion { get; set; }
        public int? _Liquidacion { get; set; }
        [Display(Name = "Fecha Baja")]
        public string FechaBaja { get; set; }
        public string ER { get; set; }
        public string DD { get; set; }
        public string Neto { get; set; }

        public int? IdEstatus { get; set; }
        public string Periodo { get; set; }
    }
}