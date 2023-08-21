using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloConceptos
    {
        //public string InformacionAduanera { get; set; }
		//public string CuentaPredial { get; set; }
		//public string ComplementoConcepto { get; set; }
		//public string Parte { get; set; }
        public string ClaveProdServ { get; set; }
        //public string NoIdentificacion { get; set; }
        public string Cantidad { get; set; }
        public string ClaveUnidad { get; set; }
        //public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public string ValorUnitario { get; set; }
        public string Importe { get; set; }
        public string Descuento { get; set; }
        public bool DescuentoSpecified{ get; set; }
    }
}