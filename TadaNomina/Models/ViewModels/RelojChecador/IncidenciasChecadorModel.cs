using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class IncidenciasChecadorModel
    {
        public int idPeriodoNomina { get; set; }
        public int idEmpleado { get; set; }
        public int idConcepto { get; set; }
        public double cantidad { get; set; }
        public double? cantidadEsquema { get; set; }
        public double? monto { get; set; }
        public double? montoEsquema { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public string folio { get; set; }
        public string observaciones { get; set; }
        public int? banderaFiniquitos { get; set; }
        public int? banderaAguinaldos { get; set; }
        public int banderaChecadores { get; set; }
        public int? banderaIncidenciasFijas { get; set; }
        public double? factorDiasTrabajadosPeriodo { get; set; }
        public double? porcentaje { get; set; }
        public double? sdCalculo { get; set; }
        public double? sdCalculoReal { get; set; }
    }
}