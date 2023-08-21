using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ModelReporteSaldosPorCliente
    {
        public int IdCliente { get; set; }

        public int IdFacturadora { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Display(Name = "Facturadora")]
        public string Facturadora { get; set; }

        [Display(Name = "Saldo")]
        public decimal Saldo { get; set; }

        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        [Display(Name = "Tipo de movimiento")]
        public string TipoMovimiento { get; set; }

        [Display(Name = "Fecha de captura")]
        public DateTime FechaCaptura { get; set; }

        public string SaldoFavor { get; set; }

        public string SaldoAplicado { get; set; }

        public int IdUsuario { get; set; }
    }
}