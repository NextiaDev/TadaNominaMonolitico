using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelBasePensionAlimenticia
    {

        [Key]
        public int IdBasePensionAlimenticia { get; set; }
        [Display(Name = "NOMBRE")]
        public string Nombre { get; set; }
        [Display(Name = "DESCRIPCIÓN")]

        public string Descripcion { get; set; }

        [Display(Name = "FORMULA")]

        public string Formula { get; set; }

        public string FechaModificacion { get; set; }

        [Display(Name = "CONCEPTOS")]
        public string Conceptos { get; set; }
        public List<SelectListItem> LTipoNomina { get; set; }


        public int? IdEstatus { get; set; }
        public bool Estatus { get; set; }
        [Display(Name = "f. Captura")]
        public DateTime? fechaCaptura { get; set; }

        public bool Validacion { get; set; }
        public string Mensaje { get; set; }
    }

}