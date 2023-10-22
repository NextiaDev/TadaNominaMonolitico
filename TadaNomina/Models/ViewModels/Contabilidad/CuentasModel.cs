using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class CuentasModel
    {
        public int idCuenta { get; set; }
        public int? IdReferencia { get; set; }
        public int idCliente { get; set; }
        public int Nivel { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public int? idTipoNomina { get; set; }
        public string conceptoNomina { get; set; }
        public int? IdTipoCuenta { get; set; }
        public int idEstatus { get; set; }
        public int idCaptura { get; set; }
        public DateTime fechaCaptura { get; set; }
        public int? idModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public string TipoCuenta { get; set; }
        public string TipoNomina { get; set; }
        public int? IdRegistroPatronal { get; set; }
        public string RFCPatrona { get; set; }
    }
}