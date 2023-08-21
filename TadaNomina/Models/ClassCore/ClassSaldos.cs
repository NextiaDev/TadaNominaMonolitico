using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    public class ClassSaldos
    {
        /// <summary>
        /// Método para obtener los saldos por unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de saldos.</returns>

        public List<sp_NOMINA_IncidenciasFijasAgrupadas_Result> GetMovimientos(int idUnidad)
        {
            var f = DateTime.Now.ToShortDateString();
            var fecha = DateTime.Parse(f);
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                var query = (from a in ctx.sp_NOMINA_IncidenciasFijasAgrupadas(idUnidad) select a).ToList();
                return query;
            }
        }

        public ModeloSaldos LlenaListasIncidencias(int IdUnidad, int IdCliente)
        {

            List<SelectListItem> Tipo = new List<SelectListItem>();
            Tipo.Add(new SelectListItem { Text = "Periodo de Tiempo", Value = "Periodo de Tiempo" });
            Tipo.Add(new SelectListItem { Text = "Saldos", Value = "Saldos" });


            List<SelectListItem> _Periodo = new List<SelectListItem>();
            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            List<PeriodoNomina> periodos = cperiodo.GetPeriodoNominas(IdUnidad).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            periodos.ForEach(x => { _Periodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            List<SelectListItem> _empleados = new List<SelectListItem>();
            ClassEmpleado cempleado = new ClassEmpleado();
            List<Empleados> lEmpleados = cempleado.GetEmpleadoByUnidadNegocio(IdUnidad).OrderBy(x => x.ApellidoPaterno).ToList();
            lEmpleados.ForEach(x => { _empleados.Add(new SelectListItem { Text = x.ClaveEmpleado + " - " + x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, Value = x.IdEmpleado.ToString() }); });

            List<SelectListItem> _conceptos = new List<SelectListItem>();
            ClassConceptos cconceptos = new ClassConceptos();
            List<Cat_ConceptosNomina> lconceptos = cconceptos.GetConceptos(IdCliente).OrderBy(x => x.Concepto).Where(x=> x.TipoDato == "Pesos").ToList();
            lconceptos.ForEach(x => { _conceptos.Add(new SelectListItem { Text = x.ClaveConcepto + " - " + x.Concepto, Value = x.IdConcepto.ToString() }); });

            ModeloSaldos modelIncidencias = new ModeloSaldos();
            modelIncidencias.LPeriodo = _Periodo;
            modelIncidencias.LEmpleados = _empleados;
            modelIncidencias.LConcepto = _conceptos;
            modelIncidencias.LTipo = Tipo;


            return modelIncidencias;
        }

        /// <summary>
        /// Método para obtener el saldo por medio del Id.
        /// </summary>
        /// <param name="IdSaldo">Recibe el identificador del saldo.</param>
        /// <returns>Regresa el resultado de la búsqueda.</returns>
        public vSaldos getSaldo(int IdSaldo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return (from b in entidad.vSaldos.Where(x => x.IdSaldo == IdSaldo) select b)
                    .FirstOrDefault();
            }
        }

        public List<vSaldos> getSaldosList(int IdSaldo, int idunidad)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return (from b in entidad.vSaldos.Where(x => x.IdConcepto == IdSaldo && x.IdUnidadNegocio == idunidad && x.IdEstatus == 1 || x.IdEstatus == 2) select b)
                    .ToList();
            }
        }


        public void newSaldoPeriodo(int IdEmpleado, int IdCocepto, string Tipo, decimal DescuentoPeriodo, string FechaInicial, string FechaFinal, int Indefinido, string Observaciones, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var saldo = new DB.Saldos()
                {
                    IdEmpleado = IdEmpleado,
                    IdConcepto = IdCocepto,
                    Tipo = Tipo,
                    DescuentoPeriodo = DescuentoPeriodo,
                    FechaInicial = DateTime.Parse(FechaInicial),
                    FechaFinal = DateTime.Parse(FechaFinal),
                    Indefinido = Indefinido,
                    Observaciones = Observaciones,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.Saldos.Add(saldo);
                entidad.SaveChanges();
            }
        }

        public void newSaldo(int IdEmpleado, int IdCocepto, string Tipo, decimal SaldoI, decimal SaldoA, decimal DescuentoP, decimal NumeroP, string Observaciones, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var saldo = new DB.Saldos()
                {
                    IdEmpleado = IdEmpleado,
                    IdConcepto = IdCocepto,
                    Tipo = Tipo,
                    SaldoInicial = SaldoI,
                    SaldoActual = SaldoA,
                    DescuentoPeriodo = DescuentoP,
                    NumeroPeriodos = NumeroP,
                    IdEstatus = 1,
                    Observaciones = Observaciones,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.Saldos.Add(saldo);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar la información del saldo.
        /// </summary>
        /// <param name="IdSaldo">Recibe el identificador del saldo.</param>
        /// <param name="IdCocepto">Recibe el identificador del concepto.</param>
        /// <param name="SaldoInicial">Recibe la variable tipo decimal.</param>
        /// <param name="SaldoActual">Recibe la variable decimal.</param>
        /// <param name="SaldoTotal">Recibe la variable decimal.</param>
        /// <param name="DescuentoPP">Recibe la variable decimal.</param>
        /// <param name="NumePeriodos">Recibe la variable decimal.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void editSaldo(int idSaldo, int idEmpleado, string Tipo, int idConcepto, decimal saldoInicial, decimal saldoActual, decimal descuentoPeriodo, decimal numeroPeriodos, string observaciones)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var s = entidad.Saldos.Where(x => x.IdSaldo == idSaldo).FirstOrDefault();

                if (s != null)
                {
                    s.IdConcepto = idConcepto;
                    s.Tipo = Tipo;
                    s.SaldoInicial = saldoInicial;
                    s.SaldoActual = saldoActual;
                    s.DescuentoPeriodo = descuentoPeriodo;
                    s.NumeroPeriodos = numeroPeriodos;
                    s.IdModifica = idEmpleado;
                    s.Observaciones = observaciones;
                    s.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }
        public void editSaldoPeriodo(int idSaldo, int idEmpleado, string Tipo, int idConcepto, decimal descuentoPeriodo, string FechaInicial, string FechaFinal, string observaciones, int indefinidon)
        {


            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var s = entidad.Saldos.Where(x => x.IdSaldo == idSaldo).FirstOrDefault();

                if (s != null)
                {
                    s.IdConcepto = idConcepto;
                    s.Tipo = Tipo;
                    s.DescuentoPeriodo = descuentoPeriodo;
                    s.FechaInicial = DateTime.Parse(FechaInicial);
                    s.FechaFinal = DateTime.Parse(FechaFinal);
                    s.Indefinido = indefinidon;
                    s.IdModifica = idEmpleado;
                    s.Observaciones = observaciones;
                    s.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar el registro de un saldo.
        /// </summary>
        /// <param name="IdSaldo">Recibe el identificador del saldo.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void Suspender(int IdSaldo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var s = entidad.Saldos.Where(x => x.IdSaldo == IdSaldo).FirstOrDefault();

                if (s != null)
                {
                    s.IdEstatus = 2;
                    s.IdModifica = IdUsuario;
                    s.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        public void Activar(int IdSaldo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var s = entidad.Saldos.Where(x => x.IdSaldo == IdSaldo).FirstOrDefault();

                if (s != null)
                {
                    s.IdEstatus = 1;
                    s.IdModifica = IdUsuario;
                    s.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }
        public void deleteSaldo(int IdSaldo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var s = entidad.Saldos.Where(x => x.IdSaldo == IdSaldo).FirstOrDefault();

                if (s != null)
                {
                    s.IdEstatus = 4;
                    s.IdModifica = IdUsuario;
                    s.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para finiquitar el saldo.
        /// </summary>
        /// <param name="IdSaldo">Recibe el identificador del saldo.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void saldarSaldo(int IdSaldo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var s = entidad.Saldos.Where(x => x.IdSaldo == IdSaldo).FirstOrDefault();

                if (s != null)
                {
                    s.IdEstatus = 3;
                    s.IdModifica = IdUsuario;
                    s.FechaModifica = DateTime.Now;
                    entidad.SaveChanges();
                }
            }
        }

        public void ProcesaSaldosList(List<vSaldos> saldos, int IdEmpleado, int IdPeriodoNomina, int IdUsuario)
        {
            borraSaldosList(IdPeriodoNomina, IdEmpleado, saldos.Select(x => x.IdSaldo).ToList());
            List<Incidencias> linc = new List<Incidencias>();
            foreach (var item in saldos)
            {
                var ins = new Incidencias();
                
                ins = creaIncidenciaSaldo(IdEmpleado, IdPeriodoNomina, item.IdConcepto ?? 0, item.IdSaldo, item.DescuentoPeriodo ?? 0, 0, IdUsuario);

                if (ins.IdEmpleado != 0 && ins.IdConcepto != 0 && ins.Monto != 0)
                    linc.Add(ins);
            }

            guardaIncidenciasSaldo(linc);
        }

        public void borraSaldosList(int IdPeriiodoNomina, int IdEmpleado, List<int> IdsSaldos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var saldo = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriiodoNomina && x.IdEmpleado == IdEmpleado && IdsSaldos.Contains((int)x.BanderaSaldos)).ToList();

                entidad.Incidencias.RemoveRange(saldo);
                entidad.SaveChanges();
            }
                
        }

        public Incidencias creaIncidenciaSaldo(int IdEmpleado, int IdPeriodoNomina, int IdConcepto, int IdSaldo, decimal Monto, decimal MontoEsq, int IdUsuario)
        {
            Incidencias i = new Incidencias();
            i.IdEmpleado = IdEmpleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = IdConcepto;
            i.Cantidad = 0;
            i.Monto = Monto;
            i.Exento = 0;
            i.Gravado = 0;
            i.MontoEsquema = MontoEsq;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaSaldos = IdSaldo;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            return i;
        }

        public void guardaIncidenciasSaldo(List<Incidencias> lInc)
        {
            
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                entidad.Incidencias.AddRange(lInc);
                entidad.SaveChanges();
            }
        }


        public vIncidencias getincidencia(int IdSaldo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return (from b in entidad.vIncidencias.Where(x => x.BanderaSaldos == IdSaldo) select b)
                    .FirstOrDefault();
            }
        }
    }
}