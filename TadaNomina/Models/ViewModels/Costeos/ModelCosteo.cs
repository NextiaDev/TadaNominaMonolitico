using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Costeos
{
    public class ModelCosteos
    {
        public List<SelectListItem> lPeriodos { get; set; }
        public int IdPeriodo { get; set; }
        public string IdsPeriodo { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Periodos { get; set; }

        public List<sp_Costeo_Result> costeo { get; set; }
    }
}