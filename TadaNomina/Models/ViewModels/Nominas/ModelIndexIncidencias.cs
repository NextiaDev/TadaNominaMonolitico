using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelIndexIncidencias
    {
        public List<ModelIncidencias> Incidencias { get; set; }
        public int IdPeridoNomina { get; set; }
        public int? IdConcepto { get; set; }
        public List<SelectListItem> LConcepto { get; set; }
    }
}