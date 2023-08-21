using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelRegistroPatronal
    {
        [Key]
        public int IdRegistroPatronal { get; set; }
        [Required]
        [Display(Name ="Cliente")]
        public int IdCliente { get; set; }
        [Required]
        [Display(Name = "Razón Social")]
        public string NombrePatrona { get; set; }
        [Required]
        [Display(Name = "Registro Patronal")]
        public string RegistroPatronal { get; set; }
        [Required]
        [Display(Name = "R.F.C.")]
        public string RFC { get; set; }
        [Required]
        [Display(Name = "Clase")]
        public int Clase { get; set; }
        public List<SelectListItem> LClase { get; set; }
        [Required]
        [Display(Name = "Riesgo de Trabajo")]
        public decimal RiesgoTrabajo { get; set; }
        [Required]
        [Display(Name = "Calle y Numero")]
        public string CalleNumero { get; set; }
        [Required]
        [Display(Name = "Colonia")]
        public string Colonia { get; set; }
        [Required]
        [Display(Name = "Municipio")]
        public string Municipio { get; set; }
        [Required]
        [Display(Name = "Entidad Federativa")]
        public string EntidadFederativa { get; set; }
        [Required]
        [Display(Name = "País")]
        public string Pais { get; set; }
        [Required]
        [Display(Name = "Codigo Postal")]
        public string CP { get; set; }
        [Required]
        [Display(Name = "Actividad Economina")]
        public int IdActividadEconomica { get; set; }
        public string ActividadEconomina { get; set; }
        public List<SelectListItem> LActividad { get; set; }
        [Required]
        [Display(Name = "Certificado Digital")]
        public string CertificadoDigital { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        public string KeyPass { get; set; }        
        [Display(Name = "Logo")]
        public string Logo { get; set; }        
    }
}