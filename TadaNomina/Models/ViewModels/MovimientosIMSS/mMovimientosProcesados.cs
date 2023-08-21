using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mMovimientosProcesados
    {
        [Required(ErrorMessage = "{0} es campo requerido")]
        [Display(Name = "Tipo de movimiento")]
        public string tipomov { get; set; }
        [Required(ErrorMessage = "{0} es campo requerido")]
        [Display(Name = "Fecha Inicial")]
        public string fecha1 { get; set; }
        [Required(ErrorMessage = "{0} es campo requerido")]
        [Display(Name = "Fecha Final")]
        public string fecha2 { get; set; }
        public List<SelectListItem> ListaTipoMov { get; set; }

        //Tabla  
        public string lote { get; set; }
        public int? RespuestaLote { get; set; }
        public string RegistroPatronal { get; set; }
        public string NSS { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombre { get; set; }
        public string FechaMovimiento { get; set; }
        public string FechaTransmision { get; set; }
        public string TipoMovimiento { get; set; }
        public string IdEmpleado { get; set; }
        public string NombreERROR { get; set; }
        public string NombrePatrona { get; set; }
    }
}