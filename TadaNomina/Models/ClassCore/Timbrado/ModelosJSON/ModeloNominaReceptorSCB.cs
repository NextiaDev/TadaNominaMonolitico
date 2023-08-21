using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.Timbrado.ModelosJSON
{
    public class ModeloNominaReceptorSBC
    {        
        public string Curp { get; set; }
        public string NumSeguridadSocial { get; set; }
        public string FechaInicioRelLaboral { get; set; }
        public bool FechaInicioRelLaboralSpecified { get; set; }
        public string Antiguedad { get; set; }
        public string TipoContrato { get; set; }
        //public string Sindicalizado { get; set; }
        //public bool SindicalizadoSpecified { get; set; }
        public string TipoJornada { get; set; }
        public string TipoRegimen { get; set; }
        public string NumEmpleado { get; set; }
        //public string Departamento { get; set; }
        public string Puesto { get; set; }
        public string RiesgoPuesto { get; set; }
        public string PeriodicidadPago { get; set; }
        public string Banco { get; set; }
        //public string CuentaBancaria { get; set; }
        public decimal SalarioBaseCotApor { get; set; }
        public bool SalarioBaseCotAporSpecified { get; set; }
        public decimal SalarioDiarioIntegrado { get; set; }
        public bool SalarioDiarioIntegradoSpecified { get; set; }
        public string ClaveEntFed { get; set; }
        public ModeloNominaSubcontratacion[] SubContratacion { get; set; }
    }
}