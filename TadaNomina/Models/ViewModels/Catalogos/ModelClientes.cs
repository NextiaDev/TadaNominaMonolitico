using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelClientes
    {
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage ="Favor de capturar Cliente")]
        [Display(Name ="Cliente:")]
        public string Cliente { get; set; }

        [Required(ErrorMessage = "Favor de capturar Razon Social")]
        [Display(Name = "Razón Social:")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "Favor de capturar RFC")]
        [StringLength(13, ErrorMessage ="El RFC no pueder ser mayor a 13 caracteres")]
        [Display(Name = "RFC:")]
        public string RFC { get; set; }
       
        [Display(Name = "Teléfono:")]
        public string Telefono { get; set; }

        [Display(Name = "Contacto:")]
        public string Contacto { get; set; }

        [Display(Name = "Correo:")]
        public string Correo { get; set; }        

        public DateTime? FechaInicioProduccion { get; set; }

        public List<SelectListItem> selectPAC { get; set; }

        public int? IdPAC { get; set; }


        public List<SelectListItem> LGrupos { get; set; }

        public int? IdGrupo { get; set; }
    }
}