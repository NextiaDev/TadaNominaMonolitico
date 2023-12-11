using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.IncidenciasPrenomina
{
    public class cIncidenciasPrenomina
    {
        /// <summary>
        /// Método para obtener el periodo de nómina por el identificador del periodo.
        /// </summary>
        /// <param name="_IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <returns>Regresa el periodo de nómina.</returns>
        public PeriodoNomina GetPeriodo(int _IdPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.PeriodoNomina.Where(x => x.IdPeriodoNomina == _IdPeriodoNomina) select a).FirstOrDefault();
            }
        }

        /// <summary>
        /// Método que lista el modelo del periodo de nómina con los datos.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista del modelo del periodo de nómina.</returns>
        public List<ModelPeriodoNomina> GetModelPeriodoNominas(int IdUnidadNegocio)
        {
            List<ModelPeriodoNomina> _periodos = new List<ModelPeriodoNomina>();
            List<vPeriodoNomina> lperiodos = GetvPeriodoNominas(IdUnidadNegocio);
            lperiodos.ForEach(x => {
                _periodos.Add(new ModelPeriodoNomina
                {
                    IdPeriodoNomina = x.IdPeriodoNomina,
                    IdUnidadNegocio = x.IdUnidadNegocio,
                    UnidaNegocio = x.UnidadNegocio,
                    Periodo = x.Periodo,
                    TipoNomina = x.TipoNomina,
                    FechaInicio = x.FechaInicio.ToShortDateString(),
                    FechaFin = x.FechaFin.ToShortDateString(),
                    AjusteImpuestos = x.AjusteDeImpuestos,
                    IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                    Observaciones = x.Observaciones,
                    ValidacionAcumulaPeriodo = x.ValidacionAcumulaPeriodo,
                    idsValidacion = x.IdsValidacionAcumulaPeriodo != null && x.IdsValidacionAcumulaPeriodo.Length > 0 ? x.IdsValidacionAcumulaPeriodo.Split(',').ToArray() : new string[0]
                }); ;
            });

            return _periodos;
        }

        /// <summary>
        /// Método que lista los periodos de nómina por unidad de negocio que estén activos y el tipo de nómina específico.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los periodos de nómina activas con el tipo de nómina específico.</returns>
        public List<vPeriodoNomina> GetvPeriodoNominas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {

                var periodos = from b in entidad.vPeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1 && x.TipoNomina != "Especial" && x.TipoNomina != "Finiquitos") select b;

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodos de nómina por unidad de negocio que estén activas y ordenadas por el periodo de nómina en forma descendente.
        /// </summary>
        /// <param name="IdUnidadNegocio"></param>
        /// <returns></returns>
        public List<PeriodoNomina> GetPeriodoNominas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodos = (from b in entidad.PeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).OrderByDescending(x => x.IdPeriodoNomina);

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Método para obtener un listado de tipo ModelIncidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodeo de nómina</param>
        /// <param name="token">Token para acceso de la api</param>
        /// <returns>Listado de tipo ModelIncidencias</returns>
        public List<ModelIncidencias> GetModelIncidencias(int IdPeriodoNomina, string token)
        {
            var modelIncidencias = new List<ModelIncidencias>();
            var vIncidencias = GetvIncindencias(IdPeriodoNomina, token);

            var res = vIncidencias.Where(x => x.IdEstatus == 100).ToList();

            vIncidencias.ForEach(x => {
                modelIncidencias.Add(new ModelIncidencias
                {
                    IdIncidencia = x.IdIncidencia,
                    IdEmpleado = (int)x.IdEmpleado,
                    ClaveEmpleado = x.ClaveEmpleado,
                    NombreEmpleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                    IdPeriodoNomina = (int)x.IdPeriodoNomina,
                    PeriodoNomina = x.Periodo,
                    IdConcepto = (int)x.IdConcepto,
                    ClaveConcepto = x.ClaveConcepto,
                    Concepto = x.Concepto,
                    Cantidad = (decimal)x.Cantidad,
                    Monto = (decimal)x.Monto,
                    Exento = (decimal)x.Exento,
                    Gravado = (decimal)x.Gravado,
                    MontoEsquema = x.MontoEsquema,
                    ExentoEsquema = x.ExentoEsquema,
                    GravadoEsquema = x.GravadoEsquema,
                    FechaIncio = x.FechaInicio,
                    FechaFinal = x.FechaFin,
                    Folio = x.Folio,
                    Observaciones = x.Observaciones,
                    TipoEsquema = x.TipoEsquema
                });
            });

            return modelIncidencias;
        }

        /// <summary>
        ///     Método que obtiene una lista de incidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <param name="token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información de las incidencias</returns>
        public List<vIncidencias> GetvIncindencias(int IdPeriodoNomina, string token)
        {
            try
            {
                var listadpo = new List<vIncidencias>();
                using (NominaEntities1 ctx = new NominaEntities1())
                {
                    listadpo = ctx.vIncidencias.Where(i => i.IdPeriodoNomina == IdPeriodoNomina && i.IdEstatus == 100).ToList();
                }
                return listadpo;
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
        }

        public void ConfirmaIncidencia(int IdIncidencia, int IdModifica)
        {
            try
            {
                using (NominaEntities1 ctx = new NominaEntities1())
                {
                    var inc = ctx.Incidencias.Where(x => x.IdIncidencia == IdIncidencia).FirstOrDefault();

                    if(inc != null)
                    {
                        inc.IdEstatus = 1;
                        inc.IdModifica = IdModifica;
                        inc.FechaModifica = DateTime.Now;

                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}