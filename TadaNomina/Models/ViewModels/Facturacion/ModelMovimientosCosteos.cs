using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelMovimientosCosteos
    {
        public int IdCosteosMovimiento { get; set;}
        public int? IdPeriodoNomina { get; set;}
        public string Esquema{ get; set;}
        public string TipoMovimiento{ get; set;}
        public string MovimientoAfecta{ get; set;}
        public int? IdMovimientoAfecta{ get; set;}
        public int? IdCosteo{ get; set;}
        public int? IdPatrona{ get; set;}
        public int? IdDivision{ get; set;}
        public int? IdConcepto{ get; set;}
        public string Descripcion{ get; set;}
        public decimal? Monto{ get; set;}
        public string Observaciones{ get; set;}
        public int? IdEstatus{ get; set;}
        public int? IdCaptura{ get; set;}
        public DateTime? FechaCaptura{ get; set;}
        public int? IdModifica { get; set;}
        public DateTime? FechaModifica{ get; set;}
    }
}