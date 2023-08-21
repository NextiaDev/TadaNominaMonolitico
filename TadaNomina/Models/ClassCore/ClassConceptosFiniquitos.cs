using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    /// Conceptos Finiquitos
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassConceptosFiniquitos
    {
        /// <summary>
        /// Método para llenar las listas de tipo SelectListItem del modelo ModelConceptosFiniquitos 
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Modelo ModelConceptosFiniquito con información en las listas de tipos SelectListItem</returns>
        public ModelConceptosFiniquito LLenaListasConceptosFinquitos(int IdCliente)
        {
            ModelConceptosFiniquito model = new ModelConceptosFiniquito();

            ClassConceptos cconceptos = new ClassConceptos();
            List<vConceptos> lconceptosNomina = cconceptos.GetvConceptos(IdCliente);

            List<SelectListItem> lconceptosVac = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaVac = lconceptosNomina.Where(x => x.ClaveGpo == "002").ToList();
            lconceptosNominaVac.ForEach(x => { lconceptosVac.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });

            List<SelectListItem> lconceptosPrestaciones = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaPrestaciones = lconceptosNomina.Where(x => x.ClaveGpo == "100").ToList();
            lconceptosNominaPrestaciones.ForEach(x => { lconceptosPrestaciones.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });

            List<SelectListItem> lconceptosIndemizaciones = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaIndemnizaciones = lconceptosNomina.Where(x => x.ClaveGpo == "003").ToList();
            lconceptosNominaIndemnizaciones.ForEach(x => { lconceptosIndemizaciones.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });

            List<SelectListItem> lconceptosFonacot = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaFonacot = lconceptosNomina.Where(x => x.ClaveGpo == "200").ToList();
            lconceptosNominaFonacot.ForEach(x => { lconceptosFonacot.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });

            List<SelectListItem> lconceptosInfonavit = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaInfonavit = lconceptosNomina.Where(x => x.ClaveGpo == "200").ToList();
            lconceptosNominaInfonavit.ForEach(x => { lconceptosInfonavit.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });

            List<SelectListItem> lconceptosPension = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaPension = lconceptosNomina.Where(x => x.ClaveGpo == "200").ToList();
            lconceptosNominaPension.ForEach(x => { lconceptosPension.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });

            vConfiguracionConceptosFiniquitos lconceptosConfigurados = GetvConfiguracionConceptosFiniquitos(IdCliente);

            try { model.Id = lconceptosConfigurados.IdConfiguracion; } catch { model.Id = 0; }
            model.IdCliente = IdCliente;
            try { model.IdConceptoVacaciones = lconceptosConfigurados.IdConceptoVacaciones; } catch { model.IdConceptoVacaciones = null; }
            model.lConceptoVacaciones = lconceptosVac;
            try { model.IdConceptoPV = lconceptosConfigurados.IdConceptoPV; } catch { model.IdConceptoPV = null; }
            model.lConceptoPV = lconceptosPrestaciones;
            try { model.IdConceptoAguinaldo = lconceptosConfigurados.IdConceptoAguinaldo; } catch { model.IdConceptoAguinaldo = null; }
            model.lConceptoAguinaldo = lconceptosPrestaciones;
            try { model.IdConcepto3M = lconceptosConfigurados.IdConceptoIndem3M; } catch { model.IdConcepto3M = null; }
            model.lConcepto3M = lconceptosIndemizaciones;
            try { model.IdConcepto20D = lconceptosConfigurados.IdConceptoIndem20D; } catch { model.IdConcepto20D = null; }
            model.lConcepto20D = lconceptosIndemizaciones;
            try { model.IdConceptoPA = lconceptosConfigurados.IdConceptoIndemPA; } catch { model.IdConceptoPA = null; }
            model.lConceptoPA = lconceptosIndemizaciones;
            try { model.IdConceptoFonacot = lconceptosConfigurados.IdConceptoFonacot; } catch { model.IdConceptoFonacot = null; }
            model.lConceptoFonacot = lconceptosFonacot;
            try { model.IdConceptoInfonavit = lconceptosConfigurados.IdConceptoInfonavit; } catch { model.IdConceptoInfonavit = null; }
            model.lConceptoInfonavit = lconceptosInfonavit;
            try { model.IdConceptoPensionAlimenticia = lconceptosConfigurados.IdConceptoPensionAlimenticia; } catch { model.IdConceptoPensionAlimenticia = null; }
            model.lConceptoPensionAlimenticia = lconceptosPension;

            return model;
        }

        /// <summary>
        /// Método para obtener la configuración del finiquito del cliente
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Configuracion del finiquito del cliente</returns>
        public ConfiguracionConceptosFiniquito GetConfiguracionConceptosFiniquitos(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var configuracion = (from b in entidad.ConfiguracionConceptosFiniquito.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).FirstOrDefault();
                
                return configuracion;
            }
        }

        /// <summary>
        /// Método para obtener la configuración del finiquito e información complementaria del mismo
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Configuracion del finiquito con información complementaria</returns>
        public vConfiguracionConceptosFiniquitos GetvConfiguracionConceptosFiniquitos(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var configuracion = (from b in entidad.vConfiguracionConceptosFiniquitos.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).FirstOrDefault();

                return configuracion;
            }
        }

        /// <summary>
        /// Método para guardar la configuracíon del finiquito
        /// </summary>
        /// <param name="model">ModelConceptosFiniquitos</param>
        /// <param name="IdUsuario">Identificador usuario</param>
        public void GuardaConfiguracionConceptoFiniquito(ModelConceptosFiniquito model, int IdUsuario)
        {
            if (!ValidaConfiguracionConceptoFiniquito(model.Id))
                AddConfiguracionConceptoFiniquito(model, IdUsuario);
            else
                UpdateConfiguracionConceptoFiniquito(model, IdUsuario);
        }

        /// <summary>
        /// Método para validar de que la configuración de finiquitos exista
        /// </summary>
        /// <param name="Id">Identificador Configuración conceptos finiquitos</param>
        /// <returns>true/false si existe la configarion de los conceptos de finiquitos o no</returns>
        public bool ValidaConfiguracionConceptoFiniquito(int Id)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                bool validacion = false;

                var registro = (from b in entidad.ConfiguracionConceptosFiniquito.Where(x => x.IdConfiguracion == Id) select b).FirstOrDefault();

                if (registro != null)
                    validacion = true;

                return validacion;
            }
        }

        /// <summary>
        /// Método para agregar una configuración de conceptos para finiquitos
        /// </summary>
        /// <param name="model">ModelConceptosFiniquito</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void AddConfiguracionConceptoFiniquito(ModelConceptosFiniquito model, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ConfiguracionConceptosFiniquito cf = new ConfiguracionConceptosFiniquito() {
                    IdCliente = model.IdCliente,
                    IdConceptoVacaciones = model.IdConceptoVacaciones,
                    IdConceptoPV = model.IdConceptoPV,
                    IdConceptoAguinaldo = model.IdConceptoAguinaldo,
                    IdConceptoIndem3M = model.IdConcepto3M,
                    IdConceptoIndem20D = model.IdConcepto20D,
                    IdConceptoIndemPA = model.IdConceptoPA,
                    IdConceptoFonacot = model.IdConceptoFonacot,
                    IdConceptoInfonavit = model.IdConceptoInfonavit,
                    IdConceptoPensionAlimenticia = model.IdConceptoPensionAlimenticia,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.ConfiguracionConceptosFiniquito.Add(cf);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para editar la configuración de conceptos de finiquitos
        /// </summary>
        /// <param name="model">ModelConceptosFiniquito</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void UpdateConfiguracionConceptoFiniquito(ModelConceptosFiniquito model, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var registro = (from b in entidad.ConfiguracionConceptosFiniquito.Where(x => x.IdConfiguracion == model.Id) select b).FirstOrDefault();

                registro.IdConceptoVacaciones = model.IdConceptoVacaciones;
                registro.IdConceptoPV = model.IdConceptoPV;
                registro.IdConceptoAguinaldo = model.IdConceptoAguinaldo;
                registro.IdConceptoIndem3M = model.IdConcepto3M;
                registro.IdConceptoIndem20D = model.IdConcepto20D;
                registro.IdConceptoIndemPA = model.IdConceptoPA;
                registro.IdConceptoFonacot = model.IdConceptoFonacot;
                registro.IdConceptoInfonavit = model.IdConceptoInfonavit;
                registro.IdConceptoPensionAlimenticia = model.IdConceptoPensionAlimenticia;
                
                entidad.SaveChanges();
            }
        }
    }
}