using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelSolicitudDispersion
    {
       
        public int Num { get; set; }
        public int IdfacturasContabilidad { get; set; }
        public int IdUnidadNegocio { get; set; }

            
        public int? IdClienteRazonSocialFacturacion { get; set; }

            
        public string MetodoPago { get; set; }
            
        public string UsoCFDI { get; set; }
            
        public string FormaPago { get; set; }
        public string Descripcion { get; set; }

        public int IdEstatus { get; set; }
        public int IdEstatusTesoreria { get; set; }
        public string Estatus { get; set; }
        public string TipoEsquema { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public decimal NTotal { get; set; }

            
        public int? IdEmpresaFacturadora { get; set; }
            
        public int? IdConceptoFacturacion { get; set; }

        public string Observaciones { get; set; }

        public HttpPostedFileBase FileAttach1 { get; set; }

        public HttpPostedFileBase FileAttach2 { get; set; }
        public HttpPostedFileBase FileAttach3 { get; set; }

        public string Costeo_HTML { get; set; }
        public string Costeo_Json { get; set; }

        public string Comprobante { get; set; }
        public string ArchivoBancos { get; set; }
        public string AutorizacionDispersion { get; set; }

        public int CargaFactura { get; set; }

        public int CargaRep { get; set; }
        public string Facturadora { get; set; }

        public string ConceptoFacturacion { get; set; }

        public string Razonsocial { get; set; }

        public int IdCancelacion { get; set; }

        public string FacturaPDF { get; set; }

        public string FacturaXML { get; set; }

        public string REP_PDF { get; set; }

        public string REP_XML { get; set; }

        public string ObservacionesParaTesoreria { get; set; }
        
        [Required]
        public string FechaPago { get; set; }

        public string Periodo { get; set; }
    }
}