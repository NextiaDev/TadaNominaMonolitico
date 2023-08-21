using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelConceptosFacturacion
    {

        public int IdConceptoFacturacion { get; set; }
        public int? IdFacturadora { get; set; }
        public string DescripcionClaveSat { get; set; }
        public string Concepto { get; set; }
        public string ClaveSat { get; set; }
        public int? IdEstatus { get; set; }
        public int? IdCaptura { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public int? IdModifica { get; set; }
        public DateTime? FechaModifica { get; set; }


    }
}