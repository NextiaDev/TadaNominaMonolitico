using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class ClassReportesIncidencias
    {
        /// <summary>
        /// Muestra las incidencias de un periodo en especifico
        /// </summary>
        /// <param name="IdPeriodoNomina">Parámetro de referencia para buscar las incidencias </param>
        /// <returns></returns>
        public List<vIncidencias> GetvIncidencias(int IdPeriodoNomina)
        {
            using (TadaReportesEntities entidad = new TadaReportesEntities())
            {
                var incidencias = (from b in entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Aterriza en una lista las incidencias de la base que son de un periodo especifico al modelo ModelReporteIncidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Parámetro utilizado para enviar a método y traer incidencias generadas en un periodo de nómina.</param>
        /// <returns></returns>
        public List<ModelReporteIncidencias> GetReporteIncidencias(int IdPeriodoNomina)
        {
            List<vIncidencias> incidencias = GetvIncidencias(IdPeriodoNomina);
            List<ModelReporteIncidencias> lista = new List<ModelReporteIncidencias>();

            incidencias.ForEach(x=> { lista.Add(new ModelReporteIncidencias { Periodo = x.Periodo, ClaveEmpleado = x.ClaveEmpleado, ApellidoPaterno = x.ApellidoPaterno, ApellidoMaterno = x.ApellidoMaterno, Nombre = x.Nombre,
                ClaveConcepto = x.ClaveConcepto, Concepto = x.Concepto, Cantidad = (decimal)x.Cantidad, Monto = (decimal)x.Monto, Exento = (decimal)x.Exento, Gravado = (decimal)x.Gravado }); });

            return lista;
            
        }
    }
}