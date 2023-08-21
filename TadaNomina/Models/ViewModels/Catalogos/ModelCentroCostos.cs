using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelCentroCostos
    {
        public int IdCentroCostos { get; set; }
        public int IdCliente { get; set; }
        //[Required]
        public string Clave { get; set; }      
        //[Required]
        public string CentroCostos { get; set; }

        //[Required]
        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
        public string Mensaje { get; set; }
        public bool validacion { get; set; }
        public int? ValidaEmpleado { get; set; }

    }
}