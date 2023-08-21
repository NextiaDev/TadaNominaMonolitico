using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelMaterias
    {
        public int IdMateria { get; set; }
        public string Materia { get; set; }
        public decimal? costo { get; set; }
        public bool check { get; set; }
    }
}