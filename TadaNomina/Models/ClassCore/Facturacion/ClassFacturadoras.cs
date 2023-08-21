using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ClassCore
{
    public class ClassFacturadoras
    {
        /// <summary>
        ///     Método que obtiene la lista las facturadoras
        /// </summary>
        /// <returns>Lista de modelos con la información de las facturadoras</returns>
        public List<ModelFacturadoras> ListaFacturadoras()
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {

                return (from a in ctx.Facturadoras
                        where a.IdEstatus == 1
                        select new ModelFacturadoras
                        {
                            IdFacturadora = a.IdFacturadora,
                            IdBanco = a.IdBanco,
                            IdBanco2 = a.IdBanco2,
                            Facturadora = a.Facturadora,
                            Domicilio = a.Domicilio,
                            RFC = a.RFC,
                            Observaciones = a.Observaciones,
                            FechaOperacion = a.FechaIncioOperacion,
                            ClaveFacturadora = a.ClaveFacturadora,
                            TipoEsquema = a.TipoEsquema,
                            FacturacionMensual = a.FacturacionMensual,
                            FacturacionAnual = a.FacturacionAnual,
                            UsuarioMail = a.UsuarioMail,
                            PasswordMail = a.PasswordMail,
                            PuertoMail = a.PuertoMail,
                            HostMail = a.HostMail,
                        }).ToList();
            }
        }
        /// <summary>
        ///     Método que obtiene la lista de facturadoras
        /// </summary>
        /// <returns>Losta de modelos con la inforamción de las facturadoras</returns>
        public List<Facturadoras> Listnasd()
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var query = (from a in ctx.Facturadoras.Where(x => x.IdEstatus == 1) select a).ToList();

                return query;
            }
        }

        /// <summary>
        ///     Método que obtiene la lista de los bancos
        /// </summary>
        /// <returns>Lista de bancos</returns>
        public List<SelectListItem> ListaBancos()
        {
            ClassBancos Bancos = new ClassBancos();

            List<SelectListItem> _Lista = new List<SelectListItem>();

            _Lista.Add(new SelectListItem { Value = "0", Text = "Elegir... " });

            foreach(var item in Bancos.getBancos())
            {
                _Lista.Add(new SelectListItem
                {
                    Value = item.IdBanco.ToString(),
                    Text = item.NombreCorto
                });
            }
            return _Lista;
        }

        /// <summary>
        ///     Método que crea una nueva facturadora
        /// </summary>
        /// <param name="_MF">Variable que contiene la información de la facturadora</param>
        public void Crear (ModelFacturadoras _MF)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                Facturadoras _fac = new Facturadoras();
                _fac.IdBanco = _MF.IdBanco;
                _fac.IdBanco2 = _MF.IdBanco2;
                _fac.Facturadora = _MF.Facturadora;
                _fac.Domicilio = _MF.Domicilio;
                _fac.RFC = _MF.RFC;
                _fac.Observaciones = _MF.Observaciones;
                _fac.FechaIncioOperacion = _MF.FechaOperacion;
                _fac.ClaveFacturadora = _MF.ClaveFacturadora;
                _fac.TipoEsquema = _MF.TipoEsquema;
                _fac.FacturacionMensual = _MF.FacturacionMensual;
                _fac.FacturacionAnual = _MF.FacturacionAnual;
                _fac.UsuarioMail = _MF.UsuarioMail;
                _fac.PasswordMail = _MF.PasswordMail;
                _fac.PuertoMail = _MF.PuertoMail;
                _fac.HostMail = _MF.HostMail;
                _fac.IdEstatus = 1;
                _fac.IdCaptura = _MF.IdCaptura;
                _fac.FechaCaptura = DateTime.Now;

                ctx.Facturadoras.Add(_fac);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que obtiene la información de una facturadora
        /// </summary>
        /// <param name="_IdFacturadora">Variable que contiene el id de la facturadora</param>
        /// <returns>Modelo con la información de la facturadora</returns>
        public ModelFacturadoras GetFacturadora(int _IdFacturadora)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Facturadoras
                        where a.IdFacturadora == _IdFacturadora
                        select new ModelFacturadoras
                        {
                            IdFacturadora = a.IdFacturadora,
                            IdBanco = a.IdBanco,
                            IdBanco2 = a.IdBanco2,
                            Facturadora = a.Facturadora,
                            Domicilio = a.Domicilio,
                            RFC = a.RFC,
                            Observaciones = a.Observaciones,
                            FechaOperacion = a.FechaIncioOperacion,
                            ClaveFacturadora = a.ClaveFacturadora,
                            TipoEsquema = a.TipoEsquema,
                            FacturacionMensual = a.FacturacionMensual,
                            FacturacionAnual = a.FacturacionAnual,
                            UsuarioMail = a.UsuarioMail,
                            PasswordMail = a.PasswordMail,
                            PuertoMail = a.PuertoMail,
                            HostMail = a.HostMail,
                        }).First();
            }
        }

        /// <summary>
        ///     Método que modifica una facturadora
        /// </summary>
        /// <param name="_MF">Variable que contiene la información de una facturadora</param>
        public void Modifica(ModelFacturadoras _MF)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var _fac = (from a in ctx.Facturadoras.Where(x => x.IdFacturadora == _MF.IdFacturadora) select a).First();

                _fac.IdBanco = _MF.IdBanco;
                _fac.IdBanco2 = _MF.IdBanco2;
                _fac.Facturadora = _MF.Facturadora;
                _fac.Domicilio = _MF.Domicilio;
                _fac.RFC = _MF.RFC;
                _fac.Observaciones = _MF.Observaciones;
                _fac.FechaIncioOperacion = _MF.FechaOperacion;
                _fac.ClaveFacturadora = _MF.ClaveFacturadora;
                _fac.TipoEsquema = _MF.TipoEsquema;
                _fac.FacturacionMensual = _MF.FacturacionMensual;
                _fac.FacturacionAnual = _MF.FacturacionAnual;
                _fac.UsuarioMail = _MF.UsuarioMail;
                //_fac.PasswordMail = _MF.PasswordMail;
                _fac.PuertoMail = _MF.PuertoMail;
                _fac.HostMail = _MF.HostMail;
                _fac.IdEstatus = 1;
                _fac.IdCaptura = _MF.IdCaptura;
                _fac.FechaCaptura = DateTime.Now;

                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que modifica la facturadora cambiándola a "eliminado"
        /// </summary>
        /// <param name="_IdFacturadora">Variable que contiene el id de la facturadora</param>
        /// <param name="_IdUsuario">Variable que contiene el id del usuario con la sesión abierta</param>
        public void Eliminar(int _IdFacturadora, int _IdUsuario)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var a = ctx.Facturadoras.Where(x => x.IdFacturadora == _IdFacturadora).First();
                a.IdEstatus = 2;
                a.IdModifica = _IdUsuario;
                a.FechaModifica = DateTime.Now;
                ctx.SaveChanges();
            }
        }


    }
}