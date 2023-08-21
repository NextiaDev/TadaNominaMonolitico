using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelV_SaldosClientes
    {

        public string Cliente { get; set; }
        public string Facturadora { get; set; }
        public int IdCliente { get; set; }
        public int IdFacturadora { get; set; }
        public decimal Saldo { get; set; }

    }
}