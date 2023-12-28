using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelFactores
    {
        public int IdConcepto { get; set; }
        public string NombreConcepto { get; set; }
        public List<Conceptos_Factores> lFactores { get; set; }
    }
}