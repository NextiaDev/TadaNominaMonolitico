using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class SumaSaldos
    {
        public int idCliente { get; set; }


        [Display(Name = "Unidad Negocio")]
        public int Idunidad { get; set; }
        public List<SelectListItem> LUnidad { get; set; }

        [Display(Name = "Facturadora")]

        public int idFacturadora { get; set; }

        public List<SelectListItem> LFacturadora { get; set; }

        public string Monto { get; set; }
        public string TipoMovimiento { get; set; }

        public string IdFacturaContabilidad { get; set; }

        public List<SelectListItem> LCostep { get; set; }

        public string Imagen { get; set; }

        public bool BanderaFactura { get; set; }

        public int Idestatus { get; set; }

        public int Idcaptura { get; set; }
    }
}