using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelAreas
    {
        public int idArea { get; set; }
        public int IdCliente { get; set; }
        //[Required]
        public string idcentrodecostos { get; set; }
        //[Required]
        public string Clave { get; set; }
        [Display(Name = "Área")]
        public string Area { get; set; }

        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
        public DateTime FechaM { get; set; }
        public string Mensaje { get; set; }
        public bool validacion { get; set; }
        public int? ValidaAreas { get; set; }
        public int? ValidaEmpleado { get; set; }
    }
}