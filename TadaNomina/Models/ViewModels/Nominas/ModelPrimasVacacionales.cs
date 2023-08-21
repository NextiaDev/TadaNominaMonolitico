using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelPrimasVacacionales
    {
        public int IdEmpleado { get; set; }
        [Display(Name="Cve")]
        public string ClaveEmpleado { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Días")]
        public decimal DiasVacaciones { get; set; }
        [Display(Name = "Total")]
        public decimal PVReal { get; set; }
        [Display(Name = "Tradicional")]
        public decimal PV { get; set; }
        [Display(Name = "Esquema")]
        public decimal PVEsq { get; set; }
        public decimal? SD { get; set; }
        public decimal? SDIMSS { get; set; }
        public decimal Factor { get; set; }
    }
}