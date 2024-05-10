using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    /// Entidades Federativas
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassEntidadFederativa
    {
        /// <summary>
        /// Método para obtener listado d elas entidades federativas
        /// </summary>
        /// <returns>Listado de entidades federativas</returns>
        public List<Cat_EntidadFederativa> getEntidades()
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                var list = (from b in entidad.Cat_EntidadFederativa select b).ToList();

                return list;
            }
        }

        /// <summary>
        /// Método para obtener la tabala de las entidades federativas
        /// </summary>
        /// <returns>Tabla de eentidades federativas</returns>
        public DataTable GetTableEntidades()
        {
            DataTable dt = new DataTable();
            var bancos = getEntidades();

            using (var reader = ObjectReader.Create(bancos))
            {
                dt.Load(reader);
            }

            return dt;
        }
    }
}
