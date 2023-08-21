using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class CosteosModel
    {
        public int idCosteo { get; set; }
        public int idCliente { get; set; }
        public int idUnidadNegocio { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        public double cuotaFija { get; set; }
        [Required]
        [Display(Name = "Tipo de Nómina:")]
        public string tipoNomina { get; set; }
        public List<SelectListItem> LTipoNomina { get; set; }
        [Required]
        [Display(Name = "Tipo de Esquema:")]
        public string tipoEsquema { get; set; }
        public List<SelectListItem> LEsquema { get; set; }
        [Required]
        [Display(Name = "Separar por Registro Patronal?:")]
        public string dividirPatronal { get; set; }

        [Required]
        [Display(Name = "Obtener Costeo Por:")]
        public string separadoPor { get; set; }
        [Required]
        [Display(Name = "Costear Por Descripción:")]
        public int agruparPorDescripcion { get; set; }
        public int idEstatus { get; set; }
        public int idCaptura { get; set; }
        public DateTime fechaCaptura { get; set; }
        public int idModifica { get; set; }
        public DateTime fechaModifica { get; set; }
        public List<SelectListItem> RegistroP { get; set; }
        public List<SelectListItem> ObtenerCosteo { get; set; }
        public List<SelectListItem> CostearporDesc { get; set; }
    }
}