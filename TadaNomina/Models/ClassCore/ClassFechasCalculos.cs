using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Fechas de cálculo
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassFechasCalculos
    {
        /// <summary>
        /// Método para obtener informacíon sobre fechas
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>ModelConfiguracionFechasCalculos</returns>
        public ModelConfiguracionFechasCalculos GetModel(int IdUnidadNegocio)
        {
            ModelConfiguracionFechasCalculos model = new ModelConfiguracionFechasCalculos();
            var configuracion = GetConfiguracionFechas(IdUnidadNegocio);

            if (configuracion != null)
            {
                model.FechaVacacionesReal = configuracion.FVacacionesReal;
                model.FechaVacaciones = configuracion.FVacaciones;
                model.FechaVacacionesEsq = configuracion.FVacacionesEsq;
                model.FechaPVReal = configuracion.FPVReal;
                model.FechaPV = configuracion.FPV;
                model.FechaPVEsq = configuracion.FPVEsq;
                model.FechaAguinaldoReal = configuracion.FAguinaldoReal;
                model.FechaAguinaldo = configuracion.FAguinaldo;
                model.FechaAguinaldoEsq = configuracion.FAguinaldoEsq;
                model.FechaLiquidacionReal = configuracion.FLiquidacionReal;
                model.FechaLiquidacion = configuracion.FLiquidacion;
                model.FechaLiquidacionEsq = configuracion.FLiquidacionEsq;
            }

            GetListas(model);

            return model;
        }

        /// <summary>
        /// Metodo para obtener listas de tipo SelectListItem para llenar la información 
        /// </summary>
        /// <param name="model">ModelConfiguracionFechasCalculos</param>
        public void GetListas(ModelConfiguracionFechasCalculos model)
        {
            model.lFechasVR = LlenaListas();
            model.lFechasV = LlenaListas();
            model.lFechasVE = LlenaListas();
            model.lFechasPVR = LlenaListas();
            model.lFechasPV = LlenaListas();
            model.lFechasPVE = LlenaListas();
            model.lFechasAR = LlenaListas();
            model.lFechasA = LlenaListas();
            model.lFechasAE = LlenaListas();
            model.lFechasLR = LlenaListas();
            model.lFechasL = LlenaListas();
            model.lFechasLE = LlenaListas();
        }

        /// <summary>
        /// Método para llenar las listas para las opciones de fecha
        /// </summary>
        /// <returns>Listado de tipo SelectListItem</returns>
        public List<SelectListItem> LlenaListas()
        {
            List<SelectListItem> lfechas = new List<SelectListItem>();
            lfechas.Add(new SelectListItem { Text = "Reconocimiento Antiguedad", Value = "Reconocimiento Antiguedad" }); ;
            lfechas.Add(new SelectListItem { Text = "Alta SS", Value = "Alta IMSS" });

            return lfechas;
        }

        /// <summary>
        /// Método para obtener la configuracón de las fechas de la unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Confguración de las fechas de la unidad de negocio</returns>
        public ConfiguracionFechasCalculos GetConfiguracionFechas(int IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cf = (from b in entidad.ConfiguracionFechasCalculos.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio) select b).FirstOrDefault();

                return cf;
            }
        }

        /// <summary>
        /// Método para agregar o editar la fecha de cálculo
        /// </summary>
        /// <param name="m">ModelConfiguracionFechasCalculos</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void AgregaFechasCalculos(ModelConfiguracionFechasCalculos m, int IdUnidadNegocio, int IdUsuario)
        {
            if (ValidaFechasCalculoUnidad(IdUnidadNegocio))
                EditFechasCalculos(m, IdUnidadNegocio, IdUsuario);
            else
                AddFechasCalculos(m, IdUnidadNegocio, IdUsuario);
        }

        /// <summary>
        /// Método para validar la fecha de cálculo
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>true/false</returns>
        private bool ValidaFechasCalculoUnidad(int IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cf = (from b in entidad.ConfiguracionFechasCalculos.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio) select b).FirstOrDefault();

                if (cf != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Método para agregar fechas de cálculo
        /// </summary>
        /// <param name="m">ModelConfiguracionFechasCalculos</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        private void AddFechasCalculos(ModelConfiguracionFechasCalculos m, int IdUnidadNegocio, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ConfiguracionFechasCalculos cf = new ConfiguracionFechasCalculos() {
                    IdUnidadNegocio = IdUnidadNegocio,
                    FVacacionesReal = m.FechaVacacionesReal,
                    FVacaciones = m.FechaVacaciones,
                    FVacacionesEsq = m.FechaVacacionesEsq,
                    FPVReal = m.FechaPVReal,
                    FPV = m.FechaPV,
                    FPVEsq = m.FechaPVEsq,
                    FAguinaldoReal = m.FechaAguinaldoReal,
                    FAguinaldo = m.FechaAguinaldo,
                    FAguinaldoEsq = m.FechaAguinaldoEsq,
                    FLiquidacionReal = m.FechaLiquidacionReal,
                    FLiquidacion = m.FechaLiquidacion,
                    FLiquidacionEsq = m.FechaLiquidacionEsq,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.ConfiguracionFechasCalculos.Add(cf);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para editar las fechas de cálculo
        /// </summary>
        /// <param name="m">ModelConfiguracionFechasCalculos</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        private void EditFechasCalculos(ModelConfiguracionFechasCalculos m, int IdUnidadNegocio, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cf = (from b in entidad.ConfiguracionFechasCalculos.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio) select b).FirstOrDefault();

                if (cf != null)
                {   
                    cf.FVacacionesReal = m.FechaVacacionesReal;
                    cf.FVacaciones = m.FechaVacaciones;
                    cf.FVacacionesEsq = m.FechaVacacionesEsq;
                    cf.FPVReal = m.FechaPVReal;
                    cf.FPV = m.FechaPV;
                    cf.FPVEsq = m.FechaPVEsq;
                    cf.FAguinaldoReal = m.FechaAguinaldoReal;
                    cf.FAguinaldo = m.FechaAguinaldo;
                    cf.FAguinaldoEsq = m.FechaAguinaldoEsq;
                    cf.FLiquidacionReal = m.FechaLiquidacionReal;
                    cf.FLiquidacion = m.FechaLiquidacion;
                    cf.FLiquidacionEsq = m.FechaLiquidacionEsq;
                    
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;
                                        
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar fechas de cálculo
        /// </summary>
        /// <param name="m">ModelConfiguracionFechasCalculos</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void DeleteFechasCalculos(ModelConfiguracionFechasCalculos m, int IdUnidadNegocio, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cf = (from b in entidad.ConfiguracionFechasCalculos.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio) select b).FirstOrDefault();

                if (cf != null)
                {
                    cf.IdEstatus = 2;
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;
                                        
                    entidad.SaveChanges();
                }
            }
        }
    }
}