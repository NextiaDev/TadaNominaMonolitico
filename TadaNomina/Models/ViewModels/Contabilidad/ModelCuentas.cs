using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class ModelCuentas
    {
        public int? IdCuenta { get; set; }
        public int? Nivel { get; set; }
        public string _Descripcion { get; set; }        
        public List<CuentasModel> cuentas { get; set; }
    }
}