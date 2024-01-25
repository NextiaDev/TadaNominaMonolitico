using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelFactorIntegracion
    {
        [Key]
        public int IdFactorIntegracion { get; set; }
        public int IdPrestaciones { get; set; }
        [Required]
        [Display(Name = "Límite Inferior:")]
        public decimal Limite_Inferior { get; set; }
        [Required]
        [Display(Name = "Límite Superior:")]
        public decimal Limite_Superior { get; set; }
        [Required]
        [Display(Name = "Aguinaldo:")]
        public int Aguinaldo { get; set; }
        [Required]
        [Display(Name = "Vacaciones:")]
        public int Vacaciones { get; set; }
        [Required]
        [Display(Name = "Prima Vacacional:")]
        public decimal PrimaVacacional { get; set; }
        [Required]
        [Display(Name = "Prima Vacacional SDI:")]
        public decimal PrimaVacacionalSDI { get; set; }
        [Required]
        [Display(Name = "Factor De Integración:"), DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal FactorIntegracion { get; set; }

        [Display(Name = "Fecha de Inicio Vigencia:")]
        public DateTime? FechaInicioVigencia { get; set; }

        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }

    }
}