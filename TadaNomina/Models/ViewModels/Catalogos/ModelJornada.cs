using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelJornada
    {
        public int IdJornada { get; set; }
        public int IdCliente { get; set; }
        //[Required]
        public string Clave { get; set; }
        //[Required]
        public string Jornada { get; set; }
        public decimal Horas { get; set; }
        
        public DateTime FechaCaptura { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }
    }
}