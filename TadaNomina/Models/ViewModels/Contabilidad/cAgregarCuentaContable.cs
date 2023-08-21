using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class cAgregarCuentaContable
    {
        public int? idReferencia { get; set; }
        public int idCliente { get; set; }
        public int Nivel { get; set; }
        public int? IdCuentaCliente { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public int? idTipoNomina { get; set; }
        public string concepto { get; set; }
        public int? IdTipoCuenta { get; set; }
        public int? IdRegistroPatronal { get; set; }
    }
}