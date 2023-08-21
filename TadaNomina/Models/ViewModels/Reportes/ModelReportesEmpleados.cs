using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReportesEmpleados
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Cliente")]
        public string Cliente { get; set; }
        [Display(Name = "Unidad de Negocio")]
        public string UnidadNegocio { get; set; }
        [Display(Name = "Nombre del Reporte")]
        public string NombreReporte { get; set; }
    }
}