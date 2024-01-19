using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteNomina
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; }
        
        [Display(Name = "Unidad de Negocio")]        
        public string UnidadNegocio { get; set; }

        [Display(Name = "Nombre del Periodo")]
        public string Periodo { get; set; }

        [Display(Name = "Fecha Inicial")]
        public string FechaInicio { get; set; }

        [Display(Name = "Fecha Final")]
        public string FechaFin { get; set; }
        [Display(Name = "Estatus")]
        public int IdEstatus { get; set; }
    }
}