using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelIncidencias
    {
        [Key]
        public int IdIncidencia { get; set; }
        [Required]
        [Display(Name = "Empleado:")]
        public int IdEmpleado { get; set; }
        [Display(Name = "Clave:")]
        public string ClaveEmpleado { get; set; }
        [Display(Name = "Nombre:")]
        public string NombreEmpleado { get; set; }
        public List<SelectListItem> LEmpleados { get; set; }
        [Required]
        [Display(Name = "Periodo de Nomina:")]
        public int IdPeriodoNomina { get; set; }
        [Display(Name = "Periodo:")]
        public string PeriodoNomina { get; set; }
        public string TipoPeriodo { get; set; }
        public List<SelectListItem> LPeriodo { get; set; }
        [Required]
        [Display(Name = "Concepto:")]
        public int IdConcepto { get; set; }
        [Display(Name = "Clave Cpto:")]
        public string ClaveConcepto { get; set; }
        [Display(Name = "Concepto:")]
        public string Concepto { get; set; }
        public List<SelectListItem> LConcepto { get; set; }
        [Display(Name = "Cantidad:")]
        public decimal Cantidad { get; set; }
        [Display(Name = "Cantidad Esq:")]
        public decimal? CantidadEsq { get; set; }
        [Display(Name = "Monto:")]
        public decimal Monto { get; set; }
        [Display(Name = "Exento:")]
        public decimal Exento { get; set; }
        [Display(Name = "Gravado:")]
        public decimal Gravado { get; set; }
        [Display(Name = "Monto Esq:")]
        public decimal? MontoEsquema { get; set; }
        [Display(Name = "Exento Esq:")]
        public decimal? ExentoEsquema { get; set; }
        [Display(Name = "Gravado Esq:")]
        public decimal? GravadoEsquema { get; set; }
        [Display(Name = "Fecha Inicio:")]
        public DateTime? FechaIncio { get; set; }
        [Display(Name = "Fecha Fin:")]
        public DateTime? FechaFinal { get; set; }
        [Display(Name = "Folio:")]
        public string Folio { get; set; }
        [Display(Name = "Observaciones:")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }
        public string TipoEsquema { get; set; }
        public int? BanderaFiniquitos { get; set; }
        public int? BanderaAguinaldo { get; set; }
        public int? BanderaIncidenciasFijas { get; set; }
        public int? BanderaVacaciones { get; set; }   
        public int? BanderaCompensaciones { get; set; }
        public int? BanderaIncidencia { get; set; }
        public int? BanderaAdelantoNominaPULPI { get; set; }
        public int? BanderaConceptoEspecial { get; set; }
        

        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
        
        public string Mensaje { get; set; }
        public bool validacion { get; set; }

        [Display(Name = "Elegir Formato:")]
        public int idFormato { get; set; }
        public List<SelectListItem> Lformato { get; set; }
        public int _IdConcepto { get; set; }

        public List<Cat_RegistroPatronal> registroPat { get; set; }

        public List<Cat_HonorariosFacturas> cat_hono { get; set; }

    }
}