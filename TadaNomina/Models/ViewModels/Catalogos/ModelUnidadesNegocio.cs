using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelUnidadesNegocio
    {
        [Key]
        public int IdUnidadNegocio { get; set; }

        public int IdCliente { get; set; }
        public string Cliente { get; set; }

        [Required(ErrorMessage = "Favor de capturar el Nombre de la Unidad de Negocio")]
        [Display(Name = "Unidad de Negocio:")]
        public string UnidadNegocio { get; set; }

        [Required(ErrorMessage ="Favor de seleccionar Tipo de Nomina")]
        [Display(Name = "Tipo de Nomina:")]
        public int? IdTipoNomina { get; set; }

        [Required(ErrorMessage = "Debe ingresar el porcentaje par el calculo de ISN")]
        [Display(Name = "Porcentaje para calculo de ISN:")]
        public decimal? PorcentajeISN { get; set; } = 0;      

        [Required(ErrorMessage = "Debe ingresar la configuracion de sueldos")]
        [Display(Name = "Configuración de Sueldos:")]
        public string ConfiguracionSueldos { get; set; }

        public List<SelectListItem> TipoNomina { get; set; }
        public List<SelectListItem> LConfiguracionSueldos { get; set; }
    }
}