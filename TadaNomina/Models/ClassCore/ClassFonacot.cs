using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    /// Fonacot
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassFonacot
    {
        public decimal montoDescuentoPeriodo { get; set; }
        /// <summary>
        /// Método para obtener los creditos activos de fonacot de la unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Creditos activios</returns>
        public List<vCreditoFonacot> getCreditosFonacot(int IdUnidadNegocio)
        {
            using (NominaEntities1 context = new NominaEntities1())
            {
                var creditos = (from b in context.vCreditoFonacot.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus==1) select b).ToList();

                return creditos;
            }
        }

        /// <summary>
        /// Método para obtener información del credito fonacot
        /// </summary>
        /// <param name="IdCreditoFonacot">Identificador del credito fonacot</param>
        /// <returns>Información del credito fonacot</returns>
        public vCreditoFonacot GetCreditoFonacot(int IdCreditoFonacot)
        {
            using (NominaEntities1 context = new NominaEntities1())
            {
                var credito = (from b in context.vCreditoFonacot.Where(x => x.IdCreditoFonacot == IdCreditoFonacot) select b).FirstOrDefault();

                return credito;
            }
        }

        /// <summary>
        /// Método para obtener listado de la información de los creditos
        /// </summary>
        /// <param name="IdUnidadNegocio">Ientificador de la unidad de negocio</param>
        /// <returns>Listado de la informacion de los creditos</returns>
        public List<ModelFonacot> GetModelFonacot(int IdUnidadNegocio)
        {
            var creditosFonacot = getCreditosFonacot(IdUnidadNegocio);
            var mCreditos = new List<ModelFonacot>();

            creditosFonacot.ForEach(x =>
            {
                mCreditos.Add(new ModelFonacot
                {
                    IdCreditoFonacot = x.IdCreditoFonacot,
                    IdEmpleado = x.IdEmpleado,
                    ClaveEmp = x.ClaveEmpleado,
                    NombreEmpleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                    NumeroCredito = x.NumeroCredito,
                    NoTrabajadorFonacot = x.NoTrabajadorFonacot,
                    NombrePatrona = x.NombrePatrona,
                    Plazos = (int)x.Plazos,
                    CuotasPagadas = (decimal)x.CuotasPagadas,
                    RetencionMensual = (decimal)x.RetencionMensual,
                    IdEstatus = (int)x.IdEstatus,
                    Activo = x.Activo == "NO" ? false : true,
                }); ;
            });

            return mCreditos;
        }

        /// <summary>
        /// Método para la información del credito por su identificador
        /// </summary>
        /// <param name="IdCreditoFonacot">Identificador del credito fonacot</param>
        /// <returns>Información del credito fonacot</returns>
        public ModelFonacot getCreditoFonacotById(int IdCreditoFonacot)
        {
            vCreditoFonacot creditoFonacot = GetCreditoFonacot(IdCreditoFonacot);
            ModelFonacot model = new ModelFonacot();

            model.IdCreditoFonacot = creditoFonacot.IdCreditoFonacot;
            model.IdEmpleado = creditoFonacot.IdEmpleado;
            model.ClaveEmp = creditoFonacot.ClaveEmpleado;
            model.NombreEmpleado = creditoFonacot.Nombre;
            model.NombrePatrona = creditoFonacot.NombrePatrona;
            model.NoTrabajadorFonacot = creditoFonacot.NoTrabajadorFonacot;
            model.NumeroCredito = creditoFonacot.NumeroCredito;
            model.Plazos = (int)creditoFonacot.Plazos;
            model.RetencionMensual = (decimal)creditoFonacot.RetencionMensual;
            model.CuotasPagadas = (decimal)creditoFonacot.CuotasPagadas;
            model.RFC = creditoFonacot.Rfc;
            model.IdEstatus = (int)creditoFonacot.IdEstatus;
            model.Activo = creditoFonacot.Activo == "NO" ? false : true;

            return model;
        }

        /// <summary>
        /// Método para agregar un credito
        /// </summary>
        /// <param name="m">ModelFonacot</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <exception cref="Exception">Error en caso de ya existir el mismo credito</exception>
        public void newCreditoFonacot(ModelFonacot m, int IdUsuario)
        {
            if (!ValidaCreditoFonacot(m.NumeroCredito))
            {
                CreditosFonacot credito = new CreditosFonacot();
                using (NominaEntities1 context = new NominaEntities1())
                {
                    credito.IdEmpleado = m.IdEmpleado;                    
                    credito.NoTrabajadorFonacot = m.NoTrabajadorFonacot;
                    credito.NumeroCredito = m.NumeroCredito;
                    credito.Plazos = m.Plazos;
                    credito.CuotasPagadas = m.CuotasPagadas;
                    credito.RetencionMensual = m.RetencionMensual;
                    credito.Activo = m.Activo == true ? "SI" : "NO";

                    credito.IdEstatus = 1;
                    credito.FechaCaptura = DateTime.Now;
                    credito.IdCaptura = IdUsuario;

                    context.CreditosFonacot.Add(credito);
                    context.SaveChanges();
                }
            }
            else
            {
                throw new Exception("Ya existe el número de crédito");
            }
        }

        /// <summary>
        /// Método para validar el credito
        /// </summary>
        /// <param name="NumeroCreditoFonacot">Numero de credito</param>
        /// <returns>true/false</returns>
        public bool ValidaCreditoFonacot(string NumeroCreditoFonacot)
        {
            using (NominaEntities1 context= new NominaEntities1 ())
            {
                var credito = (from b in context.CreditosFonacot where b.NumeroCredito == NumeroCreditoFonacot && b.IdEstatus==1 select b).FirstOrDefault();

                if (credito!=null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        /// <summary>
        /// Método para eliminar el credito
        /// </summary>
        /// <param name="IdCreditoFonacot">Identificador del credito</param>
        /// <param name="IdUsario">Identificador del usuario</param>
        public void DeleteCreditoFonacot(int IdCreditoFonacot, int IdUsario)
        {
            using (NominaEntities1 context= new NominaEntities1())
            {
                var credito = (from b in context.CreditosFonacot where b.IdCreditoFonacot == IdCreditoFonacot select b).FirstOrDefault();

                if (credito!= null)
                {
                    credito.IdEstatus = 2;
                    credito.FechaModifica = DateTime.Now;
                    credito.IdModifica = IdUsario;

                    context.SaveChanges();
                }
            }        
        }

        /// <summary>
        /// Elimina las incidencias que se crean de la carga de los creditos fonacot
        /// </summary>
        /// <param name="IdCreditoFonacot">Identificador del credito fonacot</param>
        /// <param name="IdUsuario">Identificador del usuario que elimina la incidencia</param>
        public void DeleteIncidenciasFonacot(int IdCreditoFonacot, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.BanderaFonacot == IdCreditoFonacot && x.IdEstatus == 1).ToList();

                foreach(var item in incidencias)
                {
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para procesar el credito
        /// </summary>
        /// <param name="credito">vCreditoFonacot</param>
        /// <param name="IdPeriodoNomina">Identificador del credito</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <param name="ClaveTipoNomina">Clave para identificar el tipo de nomina</param>
        /// <exception cref="Exception">Error al procesar el credito</exception>
        public void ProcesaCreditoFonacot(vCreditoFonacot credito, int IdPeriodoNomina, int IdConcepto, int IdUsuario, string ClaveTipoNomina)
        {
            eliminaIncidenciasCredito(IdPeriodoNomina, IdConcepto, credito.IdCreditoFonacot);

            var ins = new Incidencias();
            if (credito.RetencionMensual == null) { throw new Exception("No se capturo correctamente el credito fonacot del empleado: " + credito.ClaveEmpleado + " - " + credito.Nombre); }
            montoDescuentoPeriodo = 0;
            switch (ClaveTipoNomina)
            {
                case "Semanal":  //Semmanal
                    montoDescuentoPeriodo = Math.Round(((decimal)credito.RetencionMensual / 4), 2);
                    break;
                case "Catorcenal":  //Catorcenal
                    montoDescuentoPeriodo = Math.Round(((decimal)credito.RetencionMensual / 2), 2);
                    break;
                case "Quincenal":  //Quincenal
                    montoDescuentoPeriodo = Math.Round(((decimal)credito.RetencionMensual / 2), 2);
                    break;
                case "Mensual":   //Mensual
                    montoDescuentoPeriodo = Math.Round(((decimal)credito.RetencionMensual), 2);
                    break;
            }

            if (montoDescuentoPeriodo != 0)
            {
                ins = creaIncidenciaCredito((int)credito.IdEmpleado, IdPeriodoNomina, IdConcepto, credito.IdCreditoFonacot, montoDescuentoPeriodo, IdUsuario);
                guardaIncidenciasCredito(ins);
            }
        }

        /// <summary>
        /// Método para eliminar las incidencias del credito
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdCredito">Identificador del credito</param>
        public void eliminaIncidenciasCredito(int IdPeriodoNomina, int IdConcepto, int IdCredito)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidenccias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdConcepto == IdConcepto && x.BanderaFonacot == IdCredito) select b).ToList();

                entidad.Incidencias.RemoveRange(incidenccias);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para crear incidencias del credito
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdCredito">Identificador del crédito</param>
        /// <param name="Monto">Monto de la incidencia</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <returns>Información de la incidencia</returns>
        public Incidencias creaIncidenciaCredito(int IdEmpleado, int IdPeriodoNomina, int IdConcepto, int IdCredito, decimal Monto, int IdUsuario)
        {
            Incidencias i = new Incidencias();
            i.IdEmpleado = IdEmpleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = IdConcepto;
            i.Cantidad = 0;
            i.Monto = Monto;
            i.Exento = 0;
            i.Gravado = 0;
            i.MontoEsquema = 0;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaFonacot = IdCredito;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            return i;
        }

        /// <summary>
        /// Método para guardar las incidencias del credito en la base de datos
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
        /// Método para agregar el monto pagado del credito
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        public void consolidaIncidenciasFonacot(int IdPeriodoNomina)
        {
            List<Incidencias_Consolidadas> incidencias = new List<Incidencias_Consolidadas>();

            using (NominaEntities1 entidad= new NominaEntities1())
            {
                incidencias = (from b in entidad.Incidencias_Consolidadas
                               where b.IdPeriodoNomina == IdPeriodoNomina && b.IdEstatus == 1 && b.BanderaFonacot != null
                               select b).ToList();

                if (incidencias != null)
                {
                    foreach (var item in incidencias)
                    {
                        CreditosFonacot credito = new CreditosFonacot();

                        credito = (from b in entidad.CreditosFonacot
                                   where b.IdCreditoFonacot == item.BanderaFonacot
                                   select b).FirstOrDefault();

                        credito.CuotasPagadas += item.Monto;

                        entidad.SaveChanges();
                    }
                }
            }

            
        }

        /// <summary>
        /// Método para eliminar cuotas pagadas
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        public void desacumulaIncidenciasFonacot(int IdPeriodoNomina)
        {
            List<Incidencias> incidencias = new List<Incidencias>();

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                incidencias = (from b in entidad.Incidencias
                               where b.IdPeriodoNomina == IdPeriodoNomina && b.IdEstatus == 1 && b.BanderaFonacot != null
                               select b).ToList();

                if (incidencias != null)
                {
                    foreach (var item in incidencias)
                    {
                        CreditosFonacot credito = new CreditosFonacot();

                        credito = (from b in entidad.CreditosFonacot
                                   where b.IdCreditoFonacot == item.BanderaFonacot
                                   select b).FirstOrDefault();

                        credito.CuotasPagadas -= item.Monto;

                        entidad.SaveChanges();
                    }
                }
            }


        }


        public void editFonacot(ModelFonacot inf, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                try
                {
                    var fonacot = (from b in entidad.CreditosFonacot where b.IdCreditoFonacot == inf.IdCreditoFonacot select b).FirstOrDefault();

                    if (fonacot != null)
                    {
                        fonacot.Activo = inf.Activo == true ? "SI" : "NO";

                        fonacot.RetencionMensual = inf.RetencionMensual;
                        fonacot.IdModifica = IdUsuario;
                        fonacot.FechaModifica = DateTime.Now;
                    }

                    entidad.SaveChanges();
                }
                catch (Exception ex)
                {

                    throw ex;
                }

               
            }
        }

        /// <summary>
        ///     Método que modifica el estatus del crédito
        /// </summary>
        /// <param name="IdCredito">Id del crédito</param>
        /// <param name="IdUsuario">Id del usuario</param>
        /// <returns>Estatus del movimiento</returns>
        public int CambiaEstatus(int IdCredito, int IdUsuario)
        {
            int res = 0;
            try
            {
                using (NominaEntities1 ctx = new NominaEntities1())
                {
                    var credito = ctx.CreditosFonacot.Where(x => x.IdCreditoFonacot == IdCredito).FirstOrDefault();

                    if (credito != null)
                    {
                        credito.Activo = credito.Activo == "NO" ? "SI" : "NO";
                        credito.FechaModifica = DateTime.Now;
                        credito.IdModifica = IdUsuario;

                        res = ctx.SaveChanges();
                    }
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
    }
}