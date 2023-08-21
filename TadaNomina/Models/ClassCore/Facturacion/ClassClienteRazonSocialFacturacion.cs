using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassClienteRazonSocialFacturacion
    {
        /// <summary>
        ///     Método que obtiene la lista de razones sociales de un cliente
        /// </summary>
        /// <returns>Lista con la información de las razones sociales activas</returns>
        public List<ModelClienteRazonSocialFacturacion> listar()
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var _IdCliente = (int)System.Web.HttpContext.Current.Session["sIdCliente"];

                return (from b in ctx.ClienteRazonSocialFacturacion
                        join c in ctx.GruposFacturacion
                        on b.IdGrupoFacturacion equals c.IdGrupoFacturacion
                        where b.IdEstatus == 1 && b.IdCliente == _IdCliente

                        select new ModelClienteRazonSocialFacturacion
                        {
                            IdClienteRazonSocialFacturacion = b.IdClienteRazonSocialFacturacion,
                            RazonSocial = b.RazonSocial,
                            RFC = b.RFC,
                            Correo = b.Correo,
                            Grupo = c.Grupo
                        }).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene la información de una razón social
        /// </summary>
        /// <param name="_Id">Variable que contiene el id de la razón social</param>
        /// <returns>Modelo con la información de la razón social</returns>
        public ModelClienteRazonSocialFacturacion ListarRSocial(int _Id)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
              

                return (from b in ctx.ClienteRazonSocialFacturacion
                        join c in ctx.GruposFacturacion
                        on b.IdGrupoFacturacion equals c.IdGrupoFacturacion
                        where b.IdEstatus == 1 && b.IdClienteRazonSocialFacturacion == _Id

                        select new ModelClienteRazonSocialFacturacion
                        {
                            IdClienteRazonSocialFacturacion = b.IdClienteRazonSocialFacturacion,
                            IdGrupoFacturacion = (int)b.IdGrupoFacturacion,
                            RazonSocial = b.RazonSocial,
                            RFC = b.RFC,
                            Correo = b.Correo,
                            Grupo = c.Grupo
                        }).FirstOrDefault();
            }
        }

        /// <summary>
        ///     Método que Crea una nueva razón social o modifica uno existente
        /// </summary>
        /// <param name="modrs">Variable que contiene la información de una razón social</param>
        /// <returns>Estado del movimiento</returns>
        public bool Crud(ModelClienteRazonSocialFacturacion modrs)
        {
            if (modrs.IdClienteRazonSocialFacturacion == 0)
            {
                try
                {
                    ClienteRazonSocialFacturacion _crs = new ClienteRazonSocialFacturacion();

                    _crs.IdCliente = modrs.IdCliente;
                    _crs.RazonSocial = modrs.RazonSocial;
                    _crs.RFC = modrs.RFC;
                    _crs.Correo = modrs.Correo;
                    _crs.IdGrupoFacturacion = modrs.IdGrupoFacturacion;
                    _crs.IdEstatus = 1;
                    _crs.Idcaptura = modrs.IdCaptura;
                    _crs.FechaCaptura = DateTime.Now;

                    using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                    {
                        ctx.ClienteRazonSocialFacturacion.Add(_crs);
                        ctx.SaveChanges();
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                    {
                        var query = ctx.ClienteRazonSocialFacturacion.Where(x => x.IdClienteRazonSocialFacturacion == modrs.IdClienteRazonSocialFacturacion).First();
                        query.RazonSocial = modrs.RazonSocial;
                        query.RFC = modrs.RFC;
                        query.Correo = modrs.Correo;
                        query.IdGrupoFacturacion = modrs.IdGrupoFacturacion;
                        query.IdModifica = (int)System.Web.HttpContext.Current.Session["sIdUsuario"];
                        query.FechaModifica = DateTime.Now;
                        

                        ctx.SaveChanges();
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Método que obtiene una lista de grupos de facturación 
        /// </summary>
        /// <returns>Lista con la información del grupo de facturación</returns>
        public List<SelectListItem> ComboGrupoFacturacion()
        {

            List<SelectListItem> listaGrupoFacturacion = new List<SelectListItem>();

            using (var bd = new TadaContabilidadEntities())
            {
                int _IdCliente = (int)System.Web.HttpContext.Current.Session["sIdCliente"];

                var list = (from b in bd.v_Costeos_ClienteGrupo.Where(x => x.IdEstatus == 1 && x.IdCliente == _IdCliente) select b).ToList();
                list.ForEach(x => { listaGrupoFacturacion.Add(new SelectListItem { Text = x.Grupo, Value = x.IdGrupoFacturacion.ToString() }); }); 

                listaGrupoFacturacion.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            }

            return listaGrupoFacturacion;
        }

        /// <summary>
        ///     Método que cambia el estatus de una razón social a "eliminado"
        /// </summary>
        /// <param name="modrs">Variable que contiene la información de la razón social</param>
        /// <returns>Estado del movimiento</returns>
        public bool Eliminar(ModelClienteRazonSocialFacturacion modrs)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {
                    var query = ctx.ClienteRazonSocialFacturacion.Where(x => x.IdClienteRazonSocialFacturacion == modrs.IdClienteRazonSocialFacturacion).First();

                    query.IdEstatus = 2;
                    query.IdModifica = modrs.IdModifica;
                    query.FechaModifica = modrs.FechaModifica;

                    ctx.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}