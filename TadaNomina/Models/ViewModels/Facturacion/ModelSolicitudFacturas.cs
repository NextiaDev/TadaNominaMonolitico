using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelSolicitudFacturas
    {
        public int Num { get; set; }
        public int IdfacturasContabilidad { get; set; }
        public int IdUnidadNegocio { get; set; }

        public string Periodo { get; set; }


        [Required(ErrorMessage = "Campo obligatorio")]
        public int? IdClienteRazonSocialFacturacion { get; set; }
                   
        [Required(ErrorMessage = "Campo obligatorio")]
        public string MetodoPago { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        public string UsoCFDI { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
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

        [Required(ErrorMessage = "Campo obligatorio")]
        public int? IdEmpresaFacturadora { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        public int? IdConceptoFacturacion { get; set; }

        public string Observaciones { get; set; }


        public string ObservacionesRep { get; set; }

        public bool Tesoreria { get; set; }

        public bool Contabilidad { get; set; }

        public HttpPostedFileBase FileAttach1 { get; set; }

        public HttpPostedFileBase FileAttach2 { get; set; }
                

        public string Costeo_HTML { get; set; }
        public string Costeo_Json { get; set; }

        public string Comprobante { get; set; }
        public string ComprobanteTesoreria { get; set; }
        public string ArchivoBancos { get; set; }
        public string ComprobanteAutorizacion { get; set; }

        public int CargaFactura { get; set; }

        public int CargaRep { get; set; }
        public string Facturadora { get; set; }

        public string ConceptoFacturacion{ get; set; }

        public string Razonsocial { get; set; }

        public int IdCancelacion { get; set; }

        public string FacturaPDF { get; set; }

        public string FacturaXML { get; set; }

        public string REP_PDF { get; set; }

        public string REP_XML { get; set; }

        public string ObservacionesParaTesoreria { get; set; }


        public decimal Saldo { get; set; } = 0;


        [Required]
        public decimal? Monto { get; set; } = 0;

        
        public int? validacion { get; set; } = null;

        public bool habilitar { get; set; } = true;

    }
}