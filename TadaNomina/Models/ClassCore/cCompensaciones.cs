using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class cCompensaciones
    {
        public void ProcesarIncidenciasCompe(List<vCompensaciones> Ordenes, int IdPeriodoNomina, int IdUsuario)
        {

            var agrupador = Ordenes.GroupBy(x => new { x.IdEmpleado, x.IdConceptoNomina, }).Select(x => new
            {
                IdEmpleado = x.Key.IdEmpleado,
                IdConcepto = x.Key.IdConceptoNomina,

                Pago = x.Sum(c => c.Importe)
            });

            foreach (var ia in agrupador)
            {

                DeleteIncidenciaCompensacionesid(IdPeriodoNomina, ia.IdEmpleado, ia.IdConcepto);
                AgregaIncidenciasCompensaoEmpleadoNomina(ia.IdEmpleado, IdPeriodoNomina, ia.IdConcepto, ia.Pago, IdUsuario);
            }



        }

        public void DeleteIncidenciaCompensaciones(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaCompensaciones == 1).ToList();
                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public void DeleteIncidenciaCompensaciones(int IdPeriodoNomina, int idempleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaCompensaciones == 1 && x.IdEmpleado == idempleado).ToList();
                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public void DeleteIncidenciaCompensacionesid(int IdPeriodoNomina, int idempleado, int idConcepto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaCompensaciones == 1 && x.IdEmpleado == idempleado && x.IdConcepto == idConcepto).ToList();
                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }



        public void AgregaIncidenciasCompensaEmpleado(ModelIncidencias pagos, int IdUsuario)
        {
            ClassIncidencias cl = new ClassIncidencias();
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var ordenes = entidad.vCompensaciones.Where(x => x.IdEmpleado == pagos.IdEmpleado && x.IdPeridoNomina == pagos.IdPeriodoNomina).Where(x => x.IdEstatus == 1).ToList();
                var sum = ordenes.Select(c => c.Importe).Sum();
                var idconcepto = ordenes.Select(c => c.IdConceptoNomina).FirstOrDefault();
                var id = ordenes.Select(c => c.IdConceptoCompensacion).FirstOrDefault();
                ModelIncidencias model = new ModelIncidencias();

                model.IdEmpleado = pagos.IdEmpleado;
                model.IdPeriodoNomina = pagos.IdPeriodoNomina;
                model.IdConcepto = (int)idconcepto;
                model.Monto = sum;
                model.Observaciones = "PDUP SYSTEM COMPENSACIONES";
                model.MontoEsquema = 0;
                model.BanderaCompensaciones = id;
                cl.NewIncindencia(model, IdUsuario);



            }
        }


        public void AgregaIncidenciasCompensaoEmpleadoNomina(int idempleado, int idperiodo, int idconcepto, decimal monto, int IdUsuario)
        {
            ClassIncidencias cl = new ClassIncidencias();
            using (NominaEntities1 entidad = new NominaEntities1())
            {

                ModelIncidencias model = new ModelIncidencias();

                model.IdEmpleado = idempleado;
                model.IdPeriodoNomina = idperiodo;
                model.IdConcepto = (int)idconcepto;
                model.Monto = monto;
                model.Observaciones = "PDUP SYSTEM COMPENSACIONES";
                model.MontoEsquema = 0;
                model.BanderaCompensaciones = 1;
                cl.NewIncindencia(model, IdUsuario);



            }
        }
    }
}