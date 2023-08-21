using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class AccesosGVModel
    {
        public int IdAccesosGeovictoria { get; set; }
        public string ClaveAPI { get; set; }
        public string Secreto { get; set; }
        public int IdCliente { get; set;}
        public int IdEstatus { get; set; }
        public int? IdCaptura { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public int? IdModifica { get; set; }
        public DateTime? FechaModifica { get; set;}
    }
}