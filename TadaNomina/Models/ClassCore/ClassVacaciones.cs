using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class ClassVacaciones
    {
        public List<vDesgloceVacaciones> GetVDesgloceVacaciones(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = (from b in entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                DateTime fi = periodo.FechaInicio;
                DateTime ff = periodo.FechaFin;

                var dv = (from b in entidad.vDesgloceVacaciones.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1 && x.IdEstatusEmpleado == 1
                          && x.Fecha >= fi && x.Fecha <= ff)
                          select b).ToList();

                return dv;
            }
        }
        public void ProcesaIncidenciasVacaciones(int IdUnidadNegocio, int IdPeriodoNomina, int IdUsuario, int IdConceptoV)
        {
            BorraIncidenciasVacaciones(IdPeriodoNomina);
            InsertaIncidenciasVacaciones(IdUnidadNegocio, IdPeriodoNomina, IdUsuario, IdConceptoV);
        }

        public void ProcesaIncidenciasVacaciones(int IdUnidadNegocio, int IdPeriodoNomina, int IdEmpleado, int IdUsuario, int IdConceptoV)
        {
            BorraIncidenciasVacaciones(IdPeriodoNomina, IdEmpleado);
            InsertaIncidenciasVacaciones(IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, IdUsuario, IdConceptoV);
        }

        private void InsertaIncidenciasVacaciones(int IdUnidadNegocio, int IdPeriodoNomina, int IdUsuario, int IdConceptoV)
        {
            ClassIncidencias cins = new ClassIncidencias();
            var insp = GetVDesgloceVacaciones(IdPeriodoNomina, IdUnidadNegocio);
            var insp_axiliar = insp.GroupBy(x => new { x.IdEmpleado, x.IdSolicitud }).ToList();

            if (insp_axiliar.Count > 0)
            {
                foreach (var ins in insp_axiliar)
                {
                    decimal cantidad = insp.Where(x => x.IdEmpleado == ins.Key.IdEmpleado && x.IdSolicitud == ins.Key.IdSolicitud).Count();
                    cins.NewIncindencia(IdPeriodoNomina, IdConceptoV, (int)ins.Key.IdEmpleado, cantidad, 0, ins.Key.IdSolicitud, IdUsuario);
                }
            }
        }

        private void InsertaIncidenciasVacaciones(int IdUnidadNegocio, int IdPeriodoNomina, int IdEmpleado, int IdUsuario, int IdConceptoV)
        {
            ClassIncidencias cins = new ClassIncidencias();
            var insp = GetVDesgloceVacaciones(IdPeriodoNomina, IdUnidadNegocio).Where(x => x.IdEmpleado == IdEmpleado).ToList();

            var insp_axiliar = insp.GroupBy(x => new { x.IdEmpleado, x.IdSolicitud }).ToList();

            if (insp_axiliar.Count > 0)
            {
                foreach (var ins in insp_axiliar)
                {
                    decimal cantidad = insp.Where(x => x.IdEmpleado == ins.Key.IdEmpleado && x.IdSolicitud == ins.Key.IdSolicitud).Count();
                    cins.NewIncindencia(IdPeriodoNomina, IdConceptoV, (int)ins.Key.IdEmpleado, cantidad, 0, ins.Key.IdSolicitud, IdUsuario);
                }
            }
        }

        private void BorraIncidenciasVacaciones(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaVacaciones != null) select b).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        private void BorraIncidenciasVacaciones(int IdPeriodoNomina, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaVacaciones != null && x.IdEmpleado == IdEmpleado) select b).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public int IdConceptoVacaciones(int IdUnidadNegocio)
        {
            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            int IdCliente = cu.getUnidadesnegocioId(IdUnidadNegocio).IdCliente;
            int _IdCV;

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cv = (from b in entidad.ConfiguracionConceptosFiniquito.Where(x => x.IdCliente == IdCliente) select b).FirstOrDefault();

                try { _IdCV = (int)cv.IdConceptoVacaciones; } catch { throw new Exception("Falta configurar el concepto de Vacaciones. "); }

                return _IdCV;
            }
        }
    }
}