using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.CosteosConfig;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Costeos;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.Costeos
{
    public class ClassCosteos
    {
        /// <summary>
        /// Método que obtiene los costeos tradicionales
        /// </summary>
        /// <param name="IdsPeriodo">Variable que contiene los id's del periodo de nómina</param>
        /// <returns>Regresa la lista con los costeos tradicionales seleccionados</returns>
        public List<sp_Costeo_Result> getCosteoTrad(string IdsPeriodo)
        {
            using (TadaCosteosFacturacionEntities entidad = new TadaCosteosFacturacionEntities())
            {
                var costeo = (from b in entidad.sp_Costeo(IdsPeriodo) select b).ToList();

                return costeo;
            }
        }

        /// <summary>
        ///     Método que obtiene todos los costeos activos en una unidad de negocio
        /// </summary>
        /// <param name="IdUnidaNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de costeos</returns>
        public List<SelectListItem> getSelectCosteo(int IdUnidaNegocio, string Token)
        {
            var cc = new ClassConfigCosteo();
            var list = cc.GetListCosteos(IdUnidaNegocio, Token);
            var select = new List<SelectListItem>();

            list.ForEach(x=> { select.Add(new SelectListItem { Text = x.descripcion, Value = x.idCosteo.ToString() }); });

            return select;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="IdUnidadNegocio"></param>
        /// <returns></returns>
        public ModelCosteos getModelCosteo(int IdUnidadNegocio)
        {
            ClassPeriodoNomina cpn = new ClassPeriodoNomina();
            var lperiodos = new List<SelectListItem>();
            var model = new ModelCosteos();
            var lvperiodos = cpn.GetvPeriodoNominasAcumuladas(IdUnidadNegocio);

            lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });
            model.lPeriodos = lperiodos;

            return model;
        }

        /// <summary>
        ///     Método que obtiene un costeo en formato json
        /// </summary>
        /// <param name="model">Variable que contiene los datos de un costeo</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Los datos de un costoe en formato json</returns>
        public List<mResultCosteo> getCosteoString(ModelGetCosteo model, string Token)
        {
            mGetCosteo mdatos = new mGetCosteo() 
            {
                IdPeriodoNomina = model.IdsPeriodosSelecionados,
                DescripcionPeriodos = model.DescripcionPeriodos,
                IdCosteo = model.IdCosteo,
                IdCliente = model.IdCliente,
                IdUnidadNegocio = model.IdUnidadNegocio,
                ClienteUnidad = model.ClienteUnidad
            };

            var cost = new sCostear();
            var costeo = cost.GetCosteo(mdatos, Token);

            return costeo;
        }

        /// <summary>
        ///     Método que guarda un costeo nuevo
        /// </summary>
        /// <param name="model">Variable que contiene los datos de un costeo</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="Periodos">Variable que contiene una lista de periodos</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta de guardado</returns>
        public string guardarCosteos(ModelGetCosteo model, int IdCliente, int IdUnidadNegocio, string Periodos, string Token)
        {
            mGetCosteo mdatos = new mGetCosteo()
            {
                IdPeriodoNomina = model.IdsPeriodosSelecionados,
                DescripcionPeriodos = model.DescripcionPeriodos,
                IdCosteo = model.IdCosteo,
                IdCliente = model.IdCliente,
                IdUnidadNegocio = model.IdUnidadNegocio,
                ClienteUnidad = model.ClienteUnidad
            };

            var cost = new sCostear();
            var costeo = cost.GuardartCosteo(mdatos, IdCliente, IdUnidadNegocio, Periodos, Token);

            return costeo;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="IdsPeriodosSelecionados">Variable que contiene una lista de id's de costeos seleccionados</param>
        /// <param name="DescripcionPeriodos">Variable que contiene la descri´ción de,l periodo</param>
        /// <param name="IdCosteo">Variable que contiene el id del costeo</param>
        /// <param name="ClienteUnidad">Variable que contiene la unidad de negocio</param>
        /// <param name="fact">Variable que contiene </param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdUnidadNegocio">Variable que contiene la unidad de negocio</param>
        /// <param name="Periodos">Variable que contiene el periodo de nómina</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta de guardado</returns>
        public string guardarCosteosDif(int[] IdsPeriodosSelecionados, string DescripcionPeriodos, int IdCosteo, string ClienteUnidad, List<ModelFactDif> fact, int IdCliente, int IdUnidadNegocio, string Periodos, string Token)
        {
            mGetCosteo mdatos = new mGetCosteo()
            {
                IdPeriodoNomina = IdsPeriodosSelecionados,
                DescripcionPeriodos = DescripcionPeriodos,
                IdCosteo = IdCosteo,
                IdCliente = IdCliente,
                IdUnidadNegocio = IdUnidadNegocio,
                ClienteUnidad = ClienteUnidad,
                fact = fact
            };

            var cost = new sCostear();
            var costeo = cost.GuardartCosteoDif(mdatos, IdCliente, IdUnidadNegocio, Periodos, Token);

            return costeo;
        }
    }
}