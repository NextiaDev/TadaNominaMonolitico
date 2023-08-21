using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassPiramidados
    {
        internal List<ImpuestoSat> ListImpuestos;
        internal List<ImpuestoSat> ListImpuestos_Ajuste;
        internal List<ImpuestoSat> ListImpuestos_Auxiliar;
        internal List<SubsidioEmpleoSat> ListSubsidio;
        internal List<SubsidioEmpleoSat> ListSubsidio_Ajuste;
        internal List<SubsidioEmpleoSat> ListSubsidio_Auxiliar;

        public decimal piramida(decimal importe, DateTime FechaFin)
        {            
            var result = CalculaISRPiramidado(importe, DateTime.Now);

            return result;
        }

        public decimal CalculaISRPiramidado(decimal importe, DateTime FechaFin)
        {
            decimal ISR_Asimilado;
            decimal apoyo = 0;
            var DatoAlQueLlegar = importe;
            decimal VarialbeGravada = DatoAlQueLlegar;
            decimal datoCondicion = DatoAlQueLlegar;
            while (datoCondicion >= .005M)
            {
                var impuestoAsimilado = CalculaISR(VarialbeGravada, FechaFin, false);

                apoyo = VarialbeGravada;
                ISR_Asimilado = impuestoAsimilado;
                if (ISR_Asimilado < 0)
                    ISR_Asimilado = 0;

                var NetoAsimilado = apoyo - ISR_Asimilado;
                datoCondicion = importe - NetoAsimilado;
                VarialbeGravada += datoCondicion;
            }

            return apoyo;
        }

        public decimal CalculaISR(decimal BaseGravada, DateTime FechaFin, bool CalculaSubsidio)
        {
            decimal result = 0;
            decimal LimiteInferior = 0;
            decimal Porcentaje = 0;
            decimal CuotaFija = 0;
            decimal DiferenciaLimite = 0;
            decimal PorcentajeCalculado = 0;
            decimal ISR = 0;
            decimal CreditoSalario = 0;
            decimal Subsidio = 0;
            int IdTipoNomina = 4;

            GetImpuestosSAT_Auxiliar(IdTipoNomina, FechaFin);
            GetSubsidioSAT_Auxiliar(IdTipoNomina, FechaFin);

            if (BaseGravada > 0)
            {

                LimiteInferior += ListImpuestos_Auxiliar.Where(x => x.LimiteSuperior >= BaseGravada && x.LimiteInferior <= BaseGravada).FirstOrDefault().LimiteInferior;
                Porcentaje += ListImpuestos_Auxiliar.Where(x => x.LimiteSuperior >= BaseGravada && x.LimiteInferior <= BaseGravada).FirstOrDefault().Porcentaje;
                CuotaFija += ListImpuestos_Auxiliar.Where(x => x.LimiteSuperior >= BaseGravada && x.LimiteInferior <= BaseGravada).FirstOrDefault().CuotaFija;

                DiferenciaLimite = BaseGravada - LimiteInferior;

                decimal resultset = (DiferenciaLimite * Porcentaje) / 100;
                PorcentajeCalculado = decimal.Round(resultset, 2);

                ISR = CuotaFija + PorcentajeCalculado;

                if (CalculaSubsidio)
                {
                    var query = (from b in ListSubsidio_Auxiliar.Where(b => b.LimiteSuperior >= BaseGravada && b.LimiteInferior <= BaseGravada) select b).FirstOrDefault();

                    if (query == null)
                    {
                        CreditoSalario = 0;
                    }
                    else
                    {
                        CreditoSalario = decimal.Parse(query.CreditoSalario.ToString());
                    }

                    Subsidio = CreditoSalario;
                }

                result = ISR - CreditoSalario;
            }

            return result;
        }

        public void GetImpuestosSAT_Auxiliar(int IdTipoNomina, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _impuestos = (from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.EstatusId == 1
                                  && x.FechaInicio == (from c in entidad.ImpuestoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.EstatusId == 1 && y.IdTipoNomina == IdTipoNomina)
                                                       select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                  select b).ToList();

                ListImpuestos_Auxiliar = _impuestos;
            }
        }

        public void GetSubsidioSAT_Auxiliar(int IdTipoNomina, DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _subsidio = (from b in entidad.SubsidioEmpleoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.IdEstatus == 1
                                 && x.FechaInicio == (from c in entidad.SubsidioEmpleoSat.Where(y => y.FechaInicio <= fechaFinPeriodo && y.IdEstatus == 1 && y.IdTipoNomina == IdTipoNomina)
                                                      select c.FechaInicio).OrderByDescending(z => z).FirstOrDefault())
                                 select b).ToList();

                ListSubsidio_Auxiliar = _subsidio;
            }
        }

        public void AddConceptoPiramidado(int? IdPeriodoNomina, int? IdEmpleado, int? IdConcepto, decimal? DPago, decimal? SD, decimal? SMO, decimal? ISR_SMO, decimal? SMN, decimal? Importe, decimal? SMN_Imp, decimal? ISR_total, decimal? ISR_Cobrar, decimal? ImporteBruto, string ConsiderarSMO, string TipoCalculo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ConceptosPiramidados cp = new ConceptosPiramidados()
                {
                    IdPeriodo = IdPeriodoNomina,
                    IdEmpleado = IdEmpleado,
                    IdConcepto = IdConcepto,
                    DPago = DPago,
                    SD = SD,
                    SMO = SMO,
                    ISR_SMO = ISR_SMO,
                    SMN = SMN,
                    Importe = Importe,
                    SMN_Imp = SMN_Imp,
                    ISR_Total = ISR_total,
                    ISR_Cobrar = ISR_Cobrar,
                    ImporteBruto = ImporteBruto,
                    ConsiderarSMO = ConsiderarSMO,
                    TipoCalculo = TipoCalculo,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.ConceptosPiramidados.Add(cp);
                entidad.SaveChanges();
            }
        }

        public void DeleteConceptoPiramidado(int IdConceptoPiramidado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var concepto = entidad.ConceptosPiramidados.Where(x => x.IdConceptoPiramido == IdConceptoPiramidado).FirstOrDefault();

                entidad.ConceptosPiramidados.Remove(concepto);
                entidad.SaveChanges();
            }
        }

        public void DeleteConceptoPiramidadoByPeriodo(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var concepto = entidad.ConceptosPiramidados.Where(x => x.IdPeriodo == IdPeriodoNomina).ToList();

                entidad.ConceptosPiramidados.RemoveRange(concepto);
                entidad.SaveChanges();
            }
        }

        public List<ModelPiramidados> getCalculosPeriodo(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var calculos = entidad.vConceptosPiramidados.Where(x => x.IdPeriodo == IdPeriodoNomina)
                    .Select(x=>  new ModelPiramidados { IdConceptoPiramido = x.IdConceptoPiramido, IdEmpleado = x.IdEmpleado, ClaveEmpleado = x.ClaveEmpleado, 
                        Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, IdConcepto = x.IdConcepto, 
                        Concepto = x.ClaveConcepto + "-" + x.Concepto, Importe = x.Importe, ISR_SMO = x.ISR_SMO, ISR_Total = x.ISR_Total, SD = x.SD, 
                        DiasPago = x.DPago, ISR_Cobrar = x.ISR_Cobrar, Neto = x.ImporteBruto, SMB = x.SMO }).ToList();

                return calculos;
            }
        }

        public vConceptosPiramidados getConceptoById(int IdConceptoPiramidado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vConceptosPiramidados.Where(x => x.IdConceptoPiramido == IdConceptoPiramidado).FirstOrDefault();
            }
        }

        public List<vConceptosPiramidados> ProcesaPiramidados(int IdPeriodo, int? IdEmpleado, int IdUsuario)
        {
            var piramidados = new List<vConceptosPiramidados>();
            if (IdEmpleado != null)
            {
                EliminaIncidenciasPiramidados(IdPeriodo, (int)IdEmpleado);
                piramidados = getPiramidados(IdPeriodo, (int)IdEmpleado);
            }
            else
            {
                EliminaIncidenciasPiramidados(IdPeriodo);
                piramidados = getPiramidados(IdPeriodo);
            }

            List<Incidencias> li = new List<Incidencias>();
            foreach (var item in piramidados)
            {
                var incidencia = creaIncidenciaPiramidado((int)item.IdEmpleado, IdPeriodo, (int)item.IdConcepto, item.IdConceptoPiramido, (decimal)item.ImporteBruto, IdUsuario);
                li.Add(incidencia);                
            }

            guardaLista(li);

            return piramidados;
        }

        public void guardaLista(List<Incidencias> incidencias)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                entidad.Incidencias.AddRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public List<vConceptosPiramidados> getPiramidados(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vConceptosPiramidados
                    .Where(x => x.IdPeriodo == IdPeriodo && x.IdConcepto != null && x.IdEmpleado != null && x.IdEstatus == 1)
                    .ToList();               
            }
        }

        public List<vConceptosPiramidados> getPiramidados(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vConceptosPiramidados
                    .Where(x => x.IdPeriodo == IdPeriodo && x.IdConcepto != null && x.IdEmpleado != null && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado)
                    .ToList();
            }
        }

        public void EliminaIncidenciasPiramidados(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.BanderaPiramidacion != null && x.IdEstatus == 1).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public void EliminaIncidenciasPiramidados(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias
                    .Where(x => x.IdPeriodoNomina == IdPeriodo && x.BanderaPiramidacion != null && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado)
                    .ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public Incidencias creaIncidenciaPiramidado(int IdEmpleado, int IdPeriodoNomina, int IdConcepto, int IdConceptoPiramidados, decimal Monto, int IdUsuario)
        {
            Incidencias i = new Incidencias();
            i.IdEmpleado = IdEmpleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = IdConcepto;
            i.Cantidad = 0;
            i.Monto = Monto;
            i.Exento = 0;
            i.Gravado = Monto;
            i.MontoEsquema = 0;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaPiramidacion = IdConceptoPiramidados;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            return i;
        }
    }
}