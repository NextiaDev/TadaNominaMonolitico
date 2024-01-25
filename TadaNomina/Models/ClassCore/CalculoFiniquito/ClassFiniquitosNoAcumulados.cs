using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.CalculoFiniquito
{
    public class ClassFiniquitosNoAcumulados
    {
        /// <summary>
        ///     Obtiene los finiquitos que no se han acumulado
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene la unidad de negocio</param>
        /// <returns>Regresa los finiquitos que no han sido acumulados de esa unidad de negocio</returns>
        public List<vNominaSelectFiniquito> getFiniquitosNoAcumulados(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 2, 4 };
                var finiquitos = entidad.vNominaSelectFiniquito.Where(x => estatus.Contains((int)x.IdEstatus) && x.IdUnidadNegocio == IdUnidadNegocio && x.TipoNomina == "Finiquitos").ToList();

                return finiquitos;
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <param name="IdEmpleado"></param>
        /// <param name="IdNuevoPeriodo"></param>
        public void cambioPeriodoFiniquito(int IdPeriodo, int IdEmpleado, int IdNuevoPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var consulta = "sp_CambioPeriodoFiniquito " + IdPeriodo + ", " + IdEmpleado + ", " + IdNuevoPeriodo;
                entidad.Database.ExecuteSqlCommand(consulta);
                entidad.SaveChanges();
            }        
        }
    }
}