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
    
    public partial class Cat_Clientes
    {
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string Telefono { get; set; }
        public string Contacto { get; set; }
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        public string Correo { get; set; }
        public int ClienteAdministrado { get; set; }
        public Nullable<int> IdCordinador { get; set; }
        public Nullable<int> Estructura { get; set; }
        public string Usuario { get; set; }
        public byte[] Password { get; set; }
        public string Acceso { get; set; }
        public string Logo { get; set; }
        public string TituloPortal { get; set; }
        public string AnimacionPortal { get; set; }
        public Nullable<int> Nom_035 { get; set; }
        public Nullable<int> ComentariosPublicaciones { get; set; }
        public string VersionCFDI { get; set; }
        public Nullable<int> Analitica { get; set; }
        public string CorreoAnalitica { get; set; }
        public Nullable<System.DateTime> FechaBaja { get; set; }
        public string MotivoBaja { get; set; }
        public Nullable<int> IdEstatus { get; set; }
        public Nullable<int> IdCaptura { get; set; }
        public Nullable<System.DateTime> FechaCaptura { get; set; }
        public Nullable<int> IdModificacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<System.DateTime> FechaInicioProduccion { get; set; }
        public Nullable<int> RecibosTotalesRegPatronal { get; set; }
        public Nullable<int> IdPac { get; set; }
    }
}
