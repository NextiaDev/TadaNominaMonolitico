using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelRegistroPatronal
    {
        [Key]
        public int IdRegistroPatronal { get; set; }
        public int IdCliente { get; set; }

        [Required]
        [Display(Name = "Nombre Patronal: ")]
        public string NombrePatrona { get; set; }
        [Required]
        [Display(Name = "Registro Patronal: ")]
        public string RegistroPatronal { get; set; }
        [Required]
        [Display(Name = "RFC: ")]
        public string RFC { get; set; }
        [Required]
        [Display(Name = "Clase: ")]
        public int Clase { get; set; }
        [Required]
        [Display(Name = "Riesgo de Trabajo: ")]
        public decimal RiesgoTrabajo { get; set; }

        public string RiesgoTrabajoS { get; set; }

        [Required]
        [Display(Name = "Dirección Completa: ")]
        public string Direccion { get; set; }
        [Required]
        [Display(Name = "Código Postal: ")]
        public string CP { get; set; }
        [Required]
        [Display(Name = "País:")]
        public string Pais { get; set; }
        [Required]
        [Display(Name = "Entidad Federativa: ")]
        public string EntidadFederativa { get; set; }
        [Required]
        [Display(Name = "Municipio: ")]
        public string Municipio { get; set; }
        [Required]
        [Display(Name = "Colonia: ")]
        public string Colonia { get; set; }
        [Required]
        [Display(Name = "Calle: ")]
        public string Calle { get; set; }

        [Required]
        [Display(Name = "Actividad Económica:")]
        public int IdActividadEconomica { get; set; }
        [Display(Name = "Nombre Actividad Económica:")]
        public string ActividadEconomica { get; set; }
        public List<SelectListItem> LActividadEconomica { get; set; }
        [Required]
        [Display(Name = "Sello Digital: ")]
        public string SelloDigital { get; set; }
        [Display(Name = "Ruta Certificada: ")]
        public string RutaCer { get; set; }
        [Display(Name = "Key Ruta: ")]
        public string RutaKey { get; set; }
        [Display(Name = "Key Password: ")]
        public string KeyPass { get; set; }
        [Display(Name = "Logo: ")]
        public string Logo { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }

        [Display(Name = "Banco:")]
        public int? IdBanco { get; set; }

        public List<SelectListItem> LBancos { get; set; }

        [Display(Name = "Nombre Banco:")]
        public string NombreCorto { get; set; }

        [Display(Name = "Cuenta bancaria:")]
        public string CuentaBancaria { get; set; }

        [Display(Name = "Sucursal:")]
        public string Sucursal { get; set; }

        [Display(Name = "Plaza:")]
        public string Plaza { get; set; }

        [Display(Name = "Plaza Banxico:")]
        public string PlazaBanxico { get; set; }

        [Display(Name = "Concepto:")]
        public string Concepto { get; set; }

        [Display(Name = "Estado de cuenta:")]
        public string EstadoCuenta { get; set; }

        [Display(Name = "Referencia ordenante:")]
        public string ReferenciaOrdenante { get; set; }

        [Display(Name = "Forma de aplicacion:")]
        public string FormaAplicacion { get; set; }
        public string RepresentanteLegal { get; set; }

    }

}