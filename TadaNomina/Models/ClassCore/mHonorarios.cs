using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore
{
    public class mHonorarios
    {
        public int idempleadoH { get; set; }

        public int IdEmpleado { get; set; }
        public int idHonorarioF { get; set; }
        public int IdPeriodoNomina { get; set; }
        public int? IdRegistroPatronal { get; set; }
        public decimal? HonorariosN { get; set; }
        public decimal? HonorariosB { get; set; }
        public string Observaciones { get; set; }
        public double? subtotal { get; set; }
        public double? iva { get; set; }
        public double? totalfactura { get; set; }
        public double? retencionisr { get; set; }
        public double? retencioniva { get; set; }
        public double? totalRetencion { get; set; }




    }
}