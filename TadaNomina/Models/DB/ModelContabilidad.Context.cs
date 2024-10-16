﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class TadaContabilidadEntities : DbContext
    {
        public TadaContabilidadEntities()
            : base("name=TadaContabilidadEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Costeos> Costeos { get; set; }
        public virtual DbSet<Costeos_Conceptos> Costeos_Conceptos { get; set; }
        public virtual DbSet<Costeos_Conceptos_Configuracion> Costeos_Conceptos_Configuracion { get; set; }
        public virtual DbSet<Costeos_Movimientos> Costeos_Movimientos { get; set; }
        public virtual DbSet<ClienteRazonSocialFacturacion> ClienteRazonSocialFacturacion { get; set; }
        public virtual DbSet<ConceptosFacturacion> ConceptosFacturacion { get; set; }
        public virtual DbSet<Facturadoras> Facturadoras { get; set; }
        public virtual DbSet<GruposFacturacion> GruposFacturacion { get; set; }
        public virtual DbSet<v_Costeos_ClienteGrupo> v_Costeos_ClienteGrupo { get; set; }
        public virtual DbSet<Facturadoras_Grupos> Facturadoras_Grupos { get; set; }
        public virtual DbSet<v_Costeos_Grupos_Facturadoras> v_Costeos_Grupos_Facturadoras { get; set; }
        public virtual DbSet<Costeos_Fondeos> Costeos_Fondeos { get; set; }
        public virtual DbSet<vCosteos_Fondeos> vCosteos_Fondeos { get; set; }
        public virtual DbSet<ArchivoAltaCuentas> ArchivoAltaCuentas { get; set; }
        public virtual DbSet<vw_ArchivosAltaCuentas> vw_ArchivosAltaCuentas { get; set; }
        public virtual DbSet<SolicitudCheque> SolicitudCheque { get; set; }
        public virtual DbSet<v_Tesoreria_SolicitudCheque> v_Tesoreria_SolicitudCheque { get; set; }
        public virtual DbSet<PagosAdicionales> PagosAdicionales { get; set; }
        public virtual DbSet<v_Tesoreria_PagosAdicionales> v_Tesoreria_PagosAdicionales { get; set; }
        public virtual DbSet<v_saldosclientes> v_saldosclientes { get; set; }
        public virtual DbSet<vSolicitudFactura> vSolicitudFactura { get; set; }
        public virtual DbSet<FacturasContabilidad> FacturasContabilidad { get; set; }
        public virtual DbSet<v_Tesoreria_Solicitud> v_Tesoreria_Solicitud { get; set; }
    
        public virtual int SP_InsertaSaldosCosteo(Nullable<int> idCaptura, Nullable<int> idCliente, Nullable<int> idUnidadnegocio, Nullable<int> idFacturadora, Nullable<decimal> importe)
        {
            var idCapturaParameter = idCaptura.HasValue ?
                new ObjectParameter("IdCaptura", idCaptura) :
                new ObjectParameter("IdCaptura", typeof(int));
    
            var idClienteParameter = idCliente.HasValue ?
                new ObjectParameter("IdCliente", idCliente) :
                new ObjectParameter("IdCliente", typeof(int));
    
            var idUnidadnegocioParameter = idUnidadnegocio.HasValue ?
                new ObjectParameter("IdUnidadnegocio", idUnidadnegocio) :
                new ObjectParameter("IdUnidadnegocio", typeof(int));
    
            var idFacturadoraParameter = idFacturadora.HasValue ?
                new ObjectParameter("IdFacturadora", idFacturadora) :
                new ObjectParameter("IdFacturadora", typeof(int));
    
            var importeParameter = importe.HasValue ?
                new ObjectParameter("Importe", importe) :
                new ObjectParameter("Importe", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_InsertaSaldosCosteo", idCapturaParameter, idClienteParameter, idUnidadnegocioParameter, idFacturadoraParameter, importeParameter);
        }
    
        public virtual int SP_ValidaMontoFacturados(Nullable<int> idCaptura, Nullable<int> idCliente, Nullable<int> idFacturadora, Nullable<int> idcosteo, Nullable<decimal> importe, ObjectParameter texto)
        {
            var idCapturaParameter = idCaptura.HasValue ?
                new ObjectParameter("IdCaptura", idCaptura) :
                new ObjectParameter("IdCaptura", typeof(int));
    
            var idClienteParameter = idCliente.HasValue ?
                new ObjectParameter("IdCliente", idCliente) :
                new ObjectParameter("IdCliente", typeof(int));
    
            var idFacturadoraParameter = idFacturadora.HasValue ?
                new ObjectParameter("IdFacturadora", idFacturadora) :
                new ObjectParameter("IdFacturadora", typeof(int));
    
            var idcosteoParameter = idcosteo.HasValue ?
                new ObjectParameter("idcosteo", idcosteo) :
                new ObjectParameter("idcosteo", typeof(int));
    
            var importeParameter = importe.HasValue ?
                new ObjectParameter("Importe", importe) :
                new ObjectParameter("Importe", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_ValidaMontoFacturados", idCapturaParameter, idClienteParameter, idFacturadoraParameter, idcosteoParameter, importeParameter, texto);
        }
    
        public virtual ObjectResult<sp_SaldosClientesDetalle_Result> sp_SaldosClientesDetalle(Nullable<int> idCliente, Nullable<int> idFacturadora)
        {
            var idClienteParameter = idCliente.HasValue ?
                new ObjectParameter("IdCliente", idCliente) :
                new ObjectParameter("IdCliente", typeof(int));
    
            var idFacturadoraParameter = idFacturadora.HasValue ?
                new ObjectParameter("IdFacturadora", idFacturadora) :
                new ObjectParameter("IdFacturadora", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SaldosClientesDetalle_Result>("sp_SaldosClientesDetalle", idClienteParameter, idFacturadoraParameter);
        }
    
        public virtual ObjectResult<sp_SaldosPorClientes_Result> sp_SaldosPorClientes(Nullable<int> idUsuario)
        {
            var idUsuarioParameter = idUsuario.HasValue ?
                new ObjectParameter("IdUsuario", idUsuario) :
                new ObjectParameter("IdUsuario", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SaldosPorClientes_Result>("sp_SaldosPorClientes", idUsuarioParameter);
        }
    }
}
