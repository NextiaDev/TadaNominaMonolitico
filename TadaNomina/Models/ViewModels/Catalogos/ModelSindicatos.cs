using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels
{
    public class ModelSindicatos
    {
        public int IdSindicato { get; set; }

        [Required]
        [Display(Name ="Nombre Sindicato")]
        public string NombreSindicato { get; set; }

        [Required]
        [Display(Name ="Nombre Corto")]
        public string NombreCorto { get; set; }

        [Display(Name ="Grupo")]
        public int? Grupo { get; set; }

        public int? IdBanco { get; set; }

        public int IdEstatus { get; set; }

        public DateTime FechaCaptura { get; set; }

        public int IdCaptura { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int IdModificacion { get; set; }
    }
}