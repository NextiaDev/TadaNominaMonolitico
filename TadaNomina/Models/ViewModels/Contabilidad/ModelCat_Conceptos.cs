using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class ModelCat_Conceptos
    {
        public int IdConcepto { get; set; }
        public string ClaveConcepto { get; set; }
        public string DescripcionConcepto { get; set; }
        public string TipoConcepto { get; set; }
    }
}