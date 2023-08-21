using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelSaldos
    {
        public int IdSaldo { get; set; }
        public int? IdEmpleado { get; set; }        
        public decimal? SaldoInicial { get; set; }        
        public decimal? SaldoActual { get; set; }        
        public decimal? SaldoTotal { get; set; }        
        public decimal? DescuentoPeriodo { get; set; }        
        public decimal? NumeroPeriodos { get; set; }
        public int? IdEstatus { get; set; }
        public int? IdCaptura { get; set; }        
        public DateTime? FechaCaptura { get; set; }
        public int? IdModifica { get; set; }        
        public DateTime? FechaModifica { get; set; }        
        public string ClaveEmpleado { get; set; }        
        public string Nombre { get; set; }        
        public string ApellidoPaterno { get; set; }        
        public string ApellidoMaterno { get; set; }        
        public string Curp { get; set; }        
        public string Rfc { get; set; }        
        public string Imss { get; set; }
        public int IdUnidadNegocio { get; set; }
        public int IdConcepto { get; set; }
        public string ClaveConcepto { get; set; }
        public string ClaveSAT { get; set; }
        public string Concepto { get; set; }
    }
}