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
    
    public partial class Cat_Puestos
    {
        public int IdPuesto { get; set; }
        public Nullable<int> IdCliente { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
        public string Clave { get; set; }
        public string Puesto { get; set; }
        public Nullable<int> IdEstatus { get; set; }
        public Nullable<int> IdCaptura { get; set; }
        public Nullable<System.DateTime> FechaCaptura { get; set; }
        public Nullable<int> IdModificacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<decimal> Compensacion_Dia_Trabajado { get; set; }
        public Nullable<int> IdUnidadNegocio { get; set; }
        public string ClaveRH { get; set; }
    }
}
