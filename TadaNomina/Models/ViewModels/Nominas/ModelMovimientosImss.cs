using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelMovimientosImss
    {


        public int IdEmpleado { get; set; }
        public Nullable<System.DateTime> FechaCaptura { get; set; }
        public string Nombre { get; set; }
        public string NombrePatrona { get; set; }
        public string RegistroPatronal { get; set; }
        public string NombreCliente { get; set; }
        public string UnidadNegocio { get; set; }
        public string Entidad { get; set; }
        public string TipoMovimiento { get; set; }
        public string FechaAplicacionIMSS { get; set; }
        public string Imss { get; set; }
        public string Rfc { get; set; }
        public string NombreEmpleado { get; set; }
        public Nullable<decimal> SD { get; set; }
        public Nullable<decimal> SDI { get; set; }
        public string MotivoBaja { get; set; }
        public string Puesto { get; set; }
        public int IdentificadorEmpleado { get; set; }

        public string Reenvio { get; set; }
        public string Observaciones { get; set; }
        public int IdCliente { get; set; }
        public int IdUnidadNegocio { get; set; }
        public Nullable<int> IdCentroCostos { get; set; }
        public string CentroCostos { get; set; }
        public Nullable<int> IdModificacionSueldo { get; set; }
        public int TipoSalario { get; set; }


        public bool Seleccionado { get; set; }


    }

    public class RecibirEmpleadoModel
    {
        public int[] idempleado { get; set; }
    }


}