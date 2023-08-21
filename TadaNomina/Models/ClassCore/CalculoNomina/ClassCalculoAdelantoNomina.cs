using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.CalculoNomina
{
    public class ClassCalculoAdelantoNomina : ClassProcesosNomina
    {
        public decimal CalculaAdelantoNomina(vEmpleados vEmp, int IdPeriodoNomina, int IdUsuario)
        {
            decimal montoAdelantoPULPI = 0;
            var adelantos = GetAdelantoDeNomina(vEmp.IdEmpleado);

            if (adelantos != null && adelantos.Count > 0)
            {
                int _IdConceptoPULPI = GetConceptoAdelanto(vEmp.IdCliente);
                if (_IdConceptoPULPI != 0)
                {
                    EliminaIncidenciasAdelantoNomina(vEmp.IdEmpleado, _IdConceptoPULPI, IdPeriodoNomina);
                    foreach (var adel in adelantos)
                    {
                        montoAdelantoPULPI += InsertaIncidenciasAdelantos(adel, _IdConceptoPULPI, IdPeriodoNomina, IdUsuario);
                    }
                }
            }
            return montoAdelantoPULPI;
        }

        private List<AdelantoNomina> GetAdelantoDeNomina(int IdEmpleado)
        {
            List<AdelantoNomina> AN = new List<AdelantoNomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                AN = (from a in ctx.AdelantoNomina.Where(x => x.IdEmpleado == IdEmpleado && x.resp_estado == "PAID" && x.IdEstatus == 1) select a).ToList();
            }
            return AN;
        }

        private decimal InsertaIncidenciasAdelantos(AdelantoNomina _Adelanto, int _IdCondepto, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                Incidencias In = new Incidencias();
                In.IdEmpleado = _Adelanto.IdEmpleado;
                In.IdPeriodoNomina = IdPeriodoNomina;
                In.IdConcepto = _IdCondepto;
                In.Cantidad = 0;
                In.Monto = _Adelanto.total_amount;
                In.Exento = _Adelanto.total_amount;
                In.Gravado = 0;
                In.MontoEsquema = 0;
                In.ExentoEsquema = 0;
                In.GravadoEsquema = 0;
                In.BanderaAdelantoPULPI = _Adelanto.IdAdelantoNomina;
                In.IdEstatus = 1;
                In.IdCaptura = IdUsuario;
                In.FechaCaptura = DateTime.Now;
                
                ctx.Incidencias.Add(In);
                ctx.SaveChanges();
            }
            if (_Adelanto.total_amount != null)
                return (decimal)_Adelanto.total_amount;
            else
                return 0;
        }

        private int GetConceptoAdelanto(int IdCliente)
        {
            int idConcepto = 0;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                idConcepto = (from a in ctx.Cat_ConceptosNomina.Where(x => x.IdCliente == IdCliente && x.IdConceptoSistema == 1850) select a.IdConcepto).FirstOrDefault();
            }
            return idConcepto;
        }

        private void EliminaIncidenciasAdelantoNomina(int IdEmpleado, int IdConcepto, int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidenccias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdConcepto == IdConcepto && x.IdEmpleado == IdEmpleado && x.BanderaAdelantoPULPI != null) select b).ToList();

                if(incidenccias != null && incidenccias.Count > 0)
                    foreach(var item in incidenccias)
                    {
                        entidad.Incidencias.Remove(item);
                    }
                entidad.SaveChanges();
            }
        }
    }
}