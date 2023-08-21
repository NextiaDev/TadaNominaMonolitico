using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelIncidenciasProgramadas
    {
        [Key]
        public int IdIncidenciaProgramada { get; set; }
        [Required]
        [Display(Name ="Empleado")]
        public int IdEmpleado { get; set; }
        public string NoEmp { get; set; }
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Concepto")]
        public int IdConcepto { get; set; }
        public string CveConcepto { get; set; }
        
        public string Concepto { get; set; }
        [Required]
        public decimal Cantidad { get; set; }
        [Required]
        public decimal Monto { get; set; }
        [Required]
        [Display(Name = "Monto Esquema")]
        public decimal MontoEsq { get; set; }  
        public bool Activo { get; set; }
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }
        public int IdEstatus { get; set; }
        public List<SelectListItem> LEmpleados { get; set; }
        public List<SelectListItem> LConcepto { get; set; }
        public string TipoDato { get; set; }

        public bool Validacion { get; set; }
        public string Mensaje { get; set; }
    }
}