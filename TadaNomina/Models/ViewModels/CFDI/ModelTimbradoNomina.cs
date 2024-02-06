using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelTimbradoNomina
    {
        [Required]
        [Display(Name = "Seleccione el Periodo:")]        
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
        [Display(Name = "Claves de empleados:")]
        public string ClavesATimbrar { get; set; }
        public List<LogErrores> errores { get; set; }
        [Display(Name = "Versión:")]
        public string version { get; set; }
        public List<SelectListItem> lversion { get; set; }

        [Display(Name = "Uso del XML:")]
        public string tipoTimbrado { get; set; }
        public List<SelectListItem> ltipo { get; set; }
        [Display(Name = "Claves Empleado (Separadas por coma):")]
        public string ClavesEmpleado { get; set; }

        public string MensajeContador { get; set; }
        public string RegistrosNomina { get; set; }
        public string RegistrosYaTimbrados { get; set; }
        public Dictionary<string, int> registrosPatrona { get; set; }
                
        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}
