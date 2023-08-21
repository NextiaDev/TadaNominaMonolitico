using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ModeloComprobanteJSON
    {
        public ModeloEmisor Emisor { get; set; }
        public ModeloReceptor Receptor { get; set; }
        public ModeloConceptos[] Conceptos { get; set; }
        public ModeloImpuestos impuestos { get; set; }
        public ModeloComplemento Complemento { get; set; }
        public string Addenda { get; set; }
        public string Version { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string Fecha { get; set; }
        public string Sello { get; set; }
        public string FormaPago { get; set; }
        public string NoCertificado { get; set; }
        public string Certificado { get; set; }
        public string CondicionesDePago { get; set; }
        public string SubTotal { get; set; }
        public string Descuento { get; set; }
        public bool DescuentoSpecified { get; set; }
        public string Moneda { get; set; }
        public string TipoCambio { get; set; }
        public bool TipoCambioSpecified { get; set; }
        public string Total { get; set; }
        public string TipoDeComprobante { get; set; }
        public string MetodoPago { get; set; }
        public string LugarExpedicion { get; set; }
        public string Confirmacion { get; set; }
    }
}