using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class mReporteCuentas
    {
        public int IdPoliza { get; set; }
        public int IdPeriodoNomina { get; set; }
        public int IdTipoCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public string Cuenta { get; set; }
        public string DescripcionCuenta { get; set; }
        public decimal ImporteDebito { get; set; }
        public decimal ImporteCredito { get; set; }
        public string Ubicacion { get; set; }
        public int IdCuentaRaiz { get; set; }
        public int IdEstatus { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public string Periodo { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }
}