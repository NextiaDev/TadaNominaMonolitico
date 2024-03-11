using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Infonavit
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassInfonavit
    {
        public decimal montoCredito { get; set; }
        public decimal montoCreditoEsq { get; set; }

        /// <summary>
        /// Método para obtener los creditos por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de los creditos del la unidad de negocio</returns>
        public List<vCreditoInfonavit> getCreditos(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var creditos = (from b in entidad.vCreditoInfonavit.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).ToList();

                return creditos;
            }
        }

        /// <summary>
        /// Método para obtener la información de un credito por su identificador
        /// </summary>
        /// <param name="IdCreditoInfonavit">Identificador del credito de infonavit</param>
        /// <returns>Información del credito</returns>
        public vCreditoInfonavit getCredito(int IdCreditoInfonavit)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var creditos = (from b in entidad.vCreditoInfonavit.Where(x => x.IdCreditoInfonavit == IdCreditoInfonavit) select b).FirstOrDefault();

                return creditos;
            }
        }

        /// <summary>
        /// Método para obtener la informacón de los creditos de infonavir de la unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de los creditos de infonavit de la unidad de negocio</returns>
        public List<ModelInfonavit> getModelInfonavit(int IdUnidadNegocio)
        {
            var creditos = getCreditos(IdUnidadNegocio);
            var mCreditos = new List<ModelInfonavit>();

            creditos.ForEach(x =>
            {
                mCreditos.Add(new ModelInfonavit
                {
                    IdCreditoInfonavit = x.IdCreditoInfonavit,
                    IdEmpleado = x.IdEmpleado,
                    ClaveEmp = x.ClaveEmpleado,
                    Empleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                    Tipo = x.Tipo,
                    NoCredito = x.NumeroCredito,
                    CantidadUnidad = x.CantidadUnidad,
                    IdEstatus = x.IdEstatus
                });
            });

            return mCreditos;
        }

        /// <summary>
        /// Método para obtener la información del credito de infonavit por su identificador 
        /// </summary>
        /// <param name="IdCreditoInfonavit">Identificador del credito de infonavit</param>
        /// <returns>Información del credito de infonavit</returns>
        public ModelInfonavit getModelInfonavitId(int IdCreditoInfonavit)
        {
            var x = getCredito(IdCreditoInfonavit);
            var mCreditos = new ModelInfonavit();

            mCreditos.IdCreditoInfonavit = x.IdCreditoInfonavit;
            mCreditos.IdEmpleado = x.IdEmpleado;
            mCreditos.ClaveEmp = x.ClaveEmpleado;
            mCreditos.Empleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre;
            mCreditos.Tipo = x.Tipo;
            mCreditos.NoCredito = x.NumeroCredito;
            mCreditos.CantidadUnidad = x.CantidadUnidad;
            mCreditos.PorcentajeTradicional = (decimal)x.PorcentajeTradicional;
            mCreditos.IdEstatus = x.IdEstatus;
            if (x.IdEstatus == 1) { mCreditos.Estatus = true; } else { mCreditos.Estatus = false; }
            mCreditos.fechaCaptura = x.FechaCaptura;

            return mCreditos;
        }

        /// <summary>
        /// Método para obtener los tipos de creditos
        /// </summary>
        /// <returns>Lista de los tipos de creditos</returns>
        public List<SelectListItem> getTipos()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "VSM", Value="VSM" } );
            list.Add(new SelectListItem { Text = "Couta Fija", Value = "Couta Fija" });
            list.Add(new SelectListItem { Text = "Porcentaje", Value = "Porcentaje" });

            return list;
        }

        /// <summary>
        /// Método para validar que no exista un registro del mismo credito
        /// </summary>
        /// <param name="inf">Información del credito</param>
        /// <param name="IdUsuario">Identificar del usuario</param>
        /// <exception cref="Exception">Error en caso de existir un registro repetido</exception>
        public void newCredito(ModelInfonavit inf, int IdUsuario)
        {
            if (getCreditoNumero(inf.NoCredito) == null)
                createCredito(inf, IdUsuario);
            else
                throw new Exception("El numero de credito que desea ingresar ya existe");
        }

        /// <summary>
        /// Método para obtener información del credito por el numero de credito
        /// </summary>
        /// <param name="NumeroCredito">Numero del credito</param>
        /// <returns></returns>
        public CreditosInfonavit getCreditoNumero(string NumeroCredito)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var credito = (from b in entidad.CreditosInfonavit.Where(x => x.NumeroCredito == NumeroCredito && x.IdEstatus == 1) select b).FirstOrDefault();

                return credito;
            }
        }

        /// <summary>
        /// Método par acrear un credito
        /// </summary>
        /// <param name="inf">Información del credito</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        private void createCredito(ModelInfonavit inf, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                CreditosInfonavit ci = new CreditosInfonavit();
                ci.IdEmpleado = inf.IdEmpleado;
                ci.NumeroCredito = inf.NoCredito;
                ci.Tipo = inf.Tipo;
                ci.CantidadUnidad = inf.CantidadUnidad;
                ci.PorcentajeTradicional = 100;  // valor por default
                ci.IdEstatus = 1;
                ci.IdCaptura = IdUsuario;
                ci.FechaCaptura = DateTime.Now;

                entidad.CreditosInfonavit.Add(ci);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para borrar un credito
        /// </summary>
        /// <param name="IdCreditoInfonavit">Identificador del credito</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void DeleteCredito(int IdCreditoInfonavit, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var credito = (from b in entidad.CreditosInfonavit.Where(x => x.IdCreditoInfonavit == IdCreditoInfonavit) select b).FirstOrDefault();

                if (credito != null)
                {
                    credito.IdEstatus = 2;
                    credito.IdModifica = IdUsuario;
                    credito.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina las incidencias que se crean de la carga de los creditos de vivienda
        /// </summary>
        /// <param name="IdCreditoInfonavit">Identificador del credito de vivienda</param>
        /// <param name="IdUsuario">Identificador del usuario que elimina la incidencia</param>
        public void DeleteIncidenciasCreditoVivienda(int IdCreditoInfonavit, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.BanderaInfonavit == IdCreditoInfonavit && x.IdEstatus == 1).ToList();

                foreach (var item in incidencias)
                {
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar el porcentaje tradicional
        /// </summary>
        /// <param name="IdCreditoInfonavit">Identificador del credito</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <param name="porcentaje">Nuevo porcentaje</param>
        public void UpdatePorcentaje(int IdCreditoInfonavit, int IdUsuario, decimal porcentaje, decimal? cantidadUnidad)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var credito = (from b in entidad.CreditosInfonavit.Where(x => x.IdCreditoInfonavit == IdCreditoInfonavit) select b).FirstOrDefault();

                if (credito != null && porcentaje>0 && porcentaje<=100)
                {
                    credito.PorcentajeTradicional = porcentaje;
                    
                    if(cantidadUnidad != null && cantidadUnidad > 0)
                        credito.CantidadUnidad = cantidadUnidad;

                    credito.IdModifica = IdUsuario;
                    credito.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método par acalcular el monto al descontar del salario del empleado
        /// </summary>
        /// <param name="credito">Información del credito</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="UMI">Unidad dada por el sat</param>
        /// <param name="diasPeriodo">Dias del periodo de nómina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <param name="SDI">Salario diario integrado</param>
        /// <param name="TipoNomina">Tipo de nomina</param>
        /// <param name="ffPeriodo">Dias del bimestre</param>
        public void procesaCreditosInfonavit(vCreditoInfonavit credito, int IdPeriodoNomina, decimal UMI, decimal diasPeriodo, int IdConcepto, int IdUsuario, decimal SDI, string TipoNomina, DateTime ffPeriodo)
        {
            eliminaIncidenciasCredito(IdPeriodoNomina, IdConcepto, credito.IdCreditoInfonavit);
            decimal descuentoBimestral = 0;
            int DiasBimestre = 0;

            var ins = new Incidencias();

            montoCredito = 0;
            switch (credito.Tipo)
            {
                case "VSM":
                    //montoCredito = Math.Round(((((((decimal)credito.CantidadUnidad * UMI) * 2) + 15) * 6.25M) / 365) * diasPeriodo, 2);
                    descuentoBimestral = ((decimal)credito.CantidadUnidad * (decimal)UMI * 2) + 15;
                    montoCredito = CalculaDescuentoPorPeriodo(descuentoBimestral, TipoNomina);
                    break;
                case "Couta Fija":
                    //montoCredito = Math.Round((((((decimal)credito.CantidadUnidad * 2) + 15) * 6.25M) / 365) * diasPeriodo, 2);
                    descuentoBimestral = ((decimal)credito.CantidadUnidad * 2) + 15;
                    montoCredito = CalculaDescuentoPorPeriodo(descuentoBimestral, TipoNomina);
                    break;
                case "Cuota Fija":
                    //montoCredito = Math.Round((((((decimal)credito.CantidadUnidad * 2) + 15) * 6.25M) / 365) * diasPeriodo, 2);
                    descuentoBimestral = ((decimal)credito.CantidadUnidad * 2) + 15;
                    montoCredito = CalculaDescuentoPorPeriodo(descuentoBimestral, TipoNomina);
                    break;
                case "Porcentaje":
                    //montoCredito = Math.Round((((((decimal)credito.CantidadUnidad * (decimal)credito.SDI) * 365.25M) + 93.75M) / 365) * diasPeriodo, 2);
                    DiasBimestre = ObtenDiasBimestre(ffPeriodo);
                    descuentoBimestral = ((decimal)credito.CantidadUnidad * DiasBimestre * SDI) + 15;
                    montoCredito = CalculaDescuentoPorPeriodo(descuentoBimestral, TipoNomina);
                    break;
                default:
                    montoCredito = 0;
                    break;
            }

            if (montoCredito != 0)
            {
                ins = creaIncidenciaCredito((int)credito.IdEmpleado, IdPeriodoNomina, IdConcepto, credito.IdCreditoInfonavit, montoCredito, IdUsuario, (decimal)credito.PorcentajeTradicional);
                guardaIncidenciasCredito(ins);
            }
        }

        /// <summary>
        /// Método para eliminar incidencia del credito
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdCredito">Identificador del credito</param>
        public void eliminaIncidenciasCredito(int IdPeriodoNomina, int IdConcepto, int IdCredito)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidenccias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdConcepto == IdConcepto && x.BanderaInfonavit == IdCredito) select b).ToList();  //VALIDAR

                entidad.Incidencias.RemoveRange(incidenccias);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para crer una incidenci ade credito
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdCredito">Identificador del credito</param>
        /// <param name="Monto">Monto del credito</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <param name="porcentaje">Porcentaje sobre el total de percepciones del empleado</param>
        /// <returns>Información de la incidencia</returns>
        public Incidencias creaIncidenciaCredito(int IdEmpleado, int IdPeriodoNomina, int IdConcepto, int IdCredito, decimal Monto, int IdUsuario,decimal porcentaje)
        {
            montoCreditoEsq=0;

            if (porcentaje!=100)
            {
                montoCreditoEsq = Monto * (100-porcentaje) / 100;
                Monto = Monto - montoCreditoEsq;
            }


            Incidencias i = new Incidencias();
            i.IdEmpleado = IdEmpleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = IdConcepto;
            i.Cantidad = 0;
            i.Monto = Monto;
            i.Exento = 0;
            i.Gravado = 0;
            i.MontoEsquema = montoCreditoEsq;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaInfonavit = IdCredito;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            return i;
        }

        /// <summary>
        /// Método para guardar la incidencia de crédito
        /// </summary>
        /// <param name="lInc">Información de la incidencia</param>
        public void guardaIncidenciasCredito(Incidencias lInc)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                entidad.Incidencias.Add(lInc);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método par acalcular el porcentaje a descontar del credito al empleado segun sus ingresos
        /// </summary>
        /// <param name="pDescuentoBimestral">Valor del porcentaje</param>
        /// <param name="tipoNomina">Tipo de nómina</param>
        /// <returns>Valor del porcentaje a descontar del credito al empleado segun sus ingresos</returns>
        private decimal CalculaDescuentoPorPeriodo(decimal pDescuentoBimestral, string tipoNomina)
        {
            decimal resultado = 0;
            // Calculo del decuento por periodo
            if (tipoNomina == "Mensual")
            {
                resultado = pDescuentoBimestral / 2;
            }
            else if (tipoNomina == "Quincenal")
            {
                resultado = pDescuentoBimestral / 4;
            }
            else if (tipoNomina == "Catorcenal")
            {
                resultado = ((pDescuentoBimestral / 4) / 15) * 14;
            }
            else if (tipoNomina == "Semanal")
            {
                resultado = ((pDescuentoBimestral / 4) / 15) * 7;
            }

            return resultado;
        }

        /// <summary>
        /// Método para obtener los dias del bimestre
        /// </summary>
        /// <param name="ffPeriodo">Mes</param>
        /// <returns>dias del bimestre</returns>
        private int ObtenDiasBimestre(DateTime ffPeriodo)
        {
            int resultado = 0;
            int mes = ffPeriodo.Month;

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int bimestre = (int)entidad.Bimestres.Where(x => x.IdMes == mes).Select(x => x.Id_bimestre).FirstOrDefault();
                resultado = (int)entidad.Bimestres.Where(x => x.Id_bimestre == bimestre).Sum(x => x.Dias);
            }

            return resultado;
        }
    }
}