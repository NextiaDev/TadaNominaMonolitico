//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TadaNomina.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class vCreditoInfonavit
    {
        public int IdCreditoInfonavit { get; set; }
        public Nullable<int> IdEmpleado { get; set; }
        public string NumeroCredito { get; set; }
        public string Tipo { get; set; }
        public Nullable<decimal> CantidadUnidad { get; set; }
        public Nullable<int> IdEstatus { get; set; }
        public Nullable<int> IdCaptura { get; set; }
        public Nullable<System.DateTime> FechaCaptura { get; set; }
        public Nullable<int> IdModifica { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
        public string ClaveEmpleado { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public Nullable<decimal> SDIMSS { get; set; }
        public Nullable<decimal> SDI { get; set; }
        public Nullable<int> IdUnidadNegocio { get; set; }
        public Nullable<int> IdCentroCostos { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
        public Nullable<int> IdPuesto { get; set; }
        public Nullable<int> IdSucursal { get; set; }
        public string Curp { get; set; }
        public string Rfc { get; set; }
        public string Imss { get; set; }
        public Nullable<int> IdRegistroPatronal { get; set; }
        public Nullable<int> IdCliente { get; set; }
        public string UnidadNegocio { get; set; }
        public string Cliente { get; set; }
        public string CveSucursal { get; set; }
        public string Sucursal { get; set; }
        public string NombrePatrona { get; set; }
        public string RegistroPatronal { get; set; }
        public string Expr1 { get; set; }
        public string CveCC { get; set; }
        public string CentroCostos { get; set; }
        public string CveDepto { get; set; }
        public string Departamento { get; set; }
        public string CvePto { get; set; }
        public string Puesto { get; set; }
        public Nullable<decimal> PorcentajeTradicional { get; set; }
        public string BanderaSeguroVivienda { get; set; }
        public string Activo { get; set; }
    }
}
