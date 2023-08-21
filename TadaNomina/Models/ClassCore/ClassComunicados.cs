using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    /// Comunicados
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassComunicados
    {
        /// <summary>
        /// Método para obtener los comunicados por Unidad de Negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la Unidad de Negocio</param>
        /// <returns></returns>
        public List<vComunicados> getComunicados(int IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var com = (from b in entidad.vComunicados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio) select b).ToList();

                return com;
            }
        }
    }
}