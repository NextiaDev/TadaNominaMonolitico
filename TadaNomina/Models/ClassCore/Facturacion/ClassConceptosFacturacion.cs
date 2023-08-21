using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ClassCore
{
    public class ClassConceptosFacturacion
    {
        /// <summary>
        ///     Método que obtiene la lista de conceptos para facturación
        /// </summary>
        /// <returns>Lista de conceptos</returns>
        public List<ModelConceptosFacturacion> ListarConceptosFact()
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.ConceptosFacturacion
                        where a.IdEstatus == 1
                        select new ModelConceptosFacturacion
                        {
                            IdConceptoFacturacion = a.IdConceptoFacturacion,
                            IdFacturadora = a.IdFacturadora,
                            DescripcionClaveSat = a.DescripcionClaveSat,
                            Concepto = a.Concepto,
                            ClaveSat = a.ClaveSat,
                            IdEstatus = a.IdEstatus,
                            IdCaptura = a.IdCaptura,
                            FechaCaptura = a.FechaCaptura,
                            IdModifica = a.IdModifica,
                            FechaModifica = a.FechaModifica,
                        }).ToList();
            }
        }

        /// <summary>
        ///     Método que lista a las facturadoras
        /// </summary>
        /// <param name="_listMF">Variable que contiene una lista de modelos con la información de las facturadoras</param>
        /// <returns>Lista de facturadoras</returns>
        public List<SelectListItem> ListarFacturadoras(List<ModelFacturadoras> _listMF)
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem { Value = "0", Text = "Elegir... " });

            foreach (var item in _listMF)
            {
                _lista.Add(new SelectListItem
                {
                    Text = item.Facturadora,
                    Value = item.IdFacturadora.ToString()
                });
            }
            return _lista;
        }

        /// <summary>
        ///     Método que crea un nuevo concepto de facturación
        /// </summary>
        /// <param name="_MCF">Variable que contiene la información de un concepto</param>
        public void Crear(ModelConceptosFacturacion _MCF)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                ConceptosFacturacion ConceptosFacturacion = new ConceptosFacturacion();
                
                ConceptosFacturacion.IdFacturadora = _MCF.IdFacturadora;
                ConceptosFacturacion.DescripcionClaveSat = _MCF.DescripcionClaveSat;
                ConceptosFacturacion.Concepto = _MCF.Concepto;
                ConceptosFacturacion.ClaveSat = _MCF.ClaveSat;
                ConceptosFacturacion.IdEstatus = 1;
                ConceptosFacturacion.IdCaptura = _MCF.IdCaptura;
                ConceptosFacturacion.FechaCaptura = DateTime.Now;

                ctx.ConceptosFacturacion.Add(ConceptosFacturacion);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que obtiene la inforamción de un concepto de facturación
        /// </summary>
        /// <param name="_idConceptoFacturacion">Variable que contiene el id del concepto</param>
        /// <returns>Modelo con la información del concepto seleccionado</returns>
        public ModelConceptosFacturacion ListarMCF(int _idConceptoFacturacion)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.ConceptosFacturacion
                        where a.IdConceptoFacturacion == _idConceptoFacturacion
                        select new ModelConceptosFacturacion 
                        {
                            IdConceptoFacturacion = a.IdConceptoFacturacion,
                            IdFacturadora = a.IdFacturadora,
                            DescripcionClaveSat = a.DescripcionClaveSat,
                            Concepto = a.Concepto,
                            ClaveSat = a.ClaveSat,
                            IdEstatus = a.IdEstatus,
                            IdCaptura = a.IdCaptura,
                            FechaCaptura = a.FechaCaptura,
                            IdModifica = a.IdModifica,
                            FechaModifica = a.FechaModifica,
                        }).First();
            }
        }

        /// <summary>
        ///     Método que edita un concepto
        /// </summary>
        /// <param name="_MCF">Variable que contiene la información del concepto</param>
        public void Editar(ModelConceptosFacturacion _MCF)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var query = (from a in ctx.ConceptosFacturacion.Where(x => x.IdConceptoFacturacion == _MCF.IdConceptoFacturacion) select a).First();

                query.IdFacturadora = _MCF.IdFacturadora;
                query.DescripcionClaveSat = _MCF.DescripcionClaveSat;
                query.Concepto = _MCF.Concepto;
                query.ClaveSat = _MCF.ClaveSat;
                query.IdModifica = _MCF.IdModifica;
                query.FechaModifica = DateTime.Now;

                ctx.SaveChanges();
            
            }
        }

        /// <summary>
        ///     Método que modifica un concepto a "eliminado"
        /// </summary>
        /// <param name="_idConceptoFacturacion">Variable que contiene el id del concepto</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario con la sesión abierta</param>
        public void Eliminar(int _idConceptoFacturacion, int IdUsuario)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var a = ctx.ConceptosFacturacion.Where(x => x.IdConceptoFacturacion == _idConceptoFacturacion).First();
                a.IdEstatus = 2;
                a.IdModifica = IdUsuario;
                a.FechaModifica = DateTime.Now;
                ctx.SaveChanges();
            }
        }
    }
}