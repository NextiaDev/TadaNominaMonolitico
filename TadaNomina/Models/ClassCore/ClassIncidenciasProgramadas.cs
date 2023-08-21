using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Incidencias programadas
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassIncidenciasProgramadas
    {
        /// <summary>
        /// Método para obtener todas las incidencias programadas
        /// </summary>
        /// <returns>Listado de las incidencias programadas</returns>
        public List<IncidenciasProgramadas> getIncidenciasProgramadas()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var inc = (from b in entidad.IncidenciasProgramadas.Where(x => x.IdEstatus == 1) select b).ToList();

                return inc;
            }
        }

        /// <summary>
        /// Método para obtener listado de la información complementaria de las incidencias por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidade de negocio</param>
        /// <returns> listado de la información complementaria de las incidencias por unidad de negocio</returns>
        public List<vIncidenciasProgramadas> getvIncidenciasProgramadas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var inc = (from b in entidad.vIncidenciasProgramadas.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio) select b).ToList();

                return inc;
            }
        }

        /// <summary>
        /// Método para agregar una incidencia programada
        /// </summary>
        /// <param name="m">ModelIncidenciasProgramadas</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void newIncidenciaProgramada(ModelIncidenciasProgramadas m, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                IncidenciasProgramadas ip = new IncidenciasProgramadas();
                ip.IdEmpleado = m.IdEmpleado;
                ip.IdConcepto = m.IdConcepto;
                ip.Cantidad = m.Cantidad;
                ip.Monto = m.Monto;
                ip.MontoEsq = m.MontoEsq;
                ip.Observaciones = m.Observaciones;
                if (m.Activo)
                    ip.Activo = 1;
                else
                    ip.Activo = 2;

                ip.IdEstatus = 1;
                ip.IdCaptura = IdUsuario;
                ip.FechaCaptura = DateTime.Now;

                entidad.IncidenciasProgramadas.Add(ip);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar una incidencia programada
        /// </summary>
        /// <param name="m">ModelIncidenciasProgramadas</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void updateIncidenciaProgramada(ModelIncidenciasProgramadas m, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var ip = (from b in entidad.IncidenciasProgramadas.Where(x => x.IdIncidenciasProgramadas == m.IdIncidenciaProgramada) select b).FirstOrDefault();

                if (ip != null)
                {                    
                    ip.Cantidad = m.Cantidad;
                    ip.Monto = m.Monto;
                    ip.MontoEsq = m.MontoEsq;
                    ip.Observaciones = m.Observaciones;
                    if (m.Activo)
                        ip.Activo = 1;
                    else
                        ip.Activo = 2;
                                        
                    ip.IdModifica = IdUsuario;
                    ip.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para borrar la incidencia programada
        /// </summary>
        /// <param name="IdIncidenciaProgramada">Identificador de la incidencia programada</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void deleteIncidenciaProgramada(int IdIncidenciaProgramada, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var ip = (from b in entidad.IncidenciasProgramadas.Where(x => x.IdIncidenciasProgramadas == IdIncidenciaProgramada) select b).FirstOrDefault();

                if (ip != null)
                {
                    ip.IdEstatus = 2;
                    ip.IdModifica = IdUsuario;
                    ip.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para obtener la información de las incidencias programadas por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de las incidencias programadas</returns>
        public List<ModelIncidenciasProgramadas> getModelIncidenciasProgramadas(int IdUnidadNegocio)
        {
            var inc = getvIncidenciasProgramadas(IdUnidadNegocio);
            var minc = new List<ModelIncidenciasProgramadas>();

            foreach (var i in inc)
            {
                ModelIncidenciasProgramadas m = new ModelIncidenciasProgramadas();
                m.IdIncidenciaProgramada = i.IdIncidenciasProgramadas;
                m.IdEmpleado = (int)i.IdEmpleado;
                m.NoEmp = i.ClaveEmpleado;
                m.Nombre = i.NombreCompleto;
                m.IdConcepto = (int)i.IdConcepto;
                m.CveConcepto = i.ClaveConcepto;
                m.Concepto = i.Concepto;
                m.Cantidad = (decimal)i.Cantidad;
                m.Monto = (decimal)i.Monto;
                m.MontoEsq = (decimal)i.MontoEsq;
                m.Observaciones = i.Observaciones;
                if (i.Activo == 1)
                    m.Activo = true;

                minc.Add(m);
            }

            return minc;
        }

        /// <summary>
        /// Método para obtener la información de una incidencia programada por su identificador
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdIncidenciaProgramada">Identifivador de la incidencia programada</param>
        /// <returns>Información de la incidencia programada</returns>
        public ModelIncidenciasProgramadas getModelIncidenciasProgramadas(int IdUnidadNegocio, int IdIncidenciaProgramada)
        {
            var inc = getvIncidenciasProgramadas(IdUnidadNegocio).Where(x=> x.IdIncidenciasProgramadas == IdIncidenciaProgramada).FirstOrDefault();
            
            ModelIncidenciasProgramadas m = new ModelIncidenciasProgramadas();
            m.IdIncidenciaProgramada = inc.IdIncidenciasProgramadas;
            m.IdEmpleado = (int)inc.IdEmpleado;
            m.NoEmp = inc.ClaveEmpleado;
            m.Nombre = inc.NombreCompleto;
            m.IdConcepto = (int)inc.IdConcepto;
            m.CveConcepto = inc.ClaveConcepto;
            m.Concepto = inc.Concepto;
            m.Cantidad = (decimal)inc.Cantidad;
            m.Monto = (decimal)inc.Monto;
            m.MontoEsq = (decimal)inc.MontoEsq;
            m.Observaciones = inc.Observaciones;
            m.TipoDato = inc.TipoDato;
            if (inc.Activo == 1)
                m.Activo = true;

            return m;
        }

        /// <summary>
        /// Metodo para obtener listado de los empleados por unidad de negocio y conceptos por unidad de negocio
        /// </summary>
        /// <param name="IdUnidad">Identificador de la unidad de negocio</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>listado de los empleados por unidad de negocio y conceptos por unidad de negocio</returns>
        public ModelIncidenciasProgramadas LlenaListasIncidencias(int IdUnidad, int IdCliente)
        {
            List<SelectListItem> _empleados = new List<SelectListItem>();
            ClassEmpleado cempleado = new ClassEmpleado();
            List<Empleados> lEmpleados = cempleado.GetEmpleadoByUnidadNegocio(IdUnidad).OrderBy(x => x.ApellidoPaterno).ToList();
            lEmpleados.ForEach(x => { _empleados.Add(new SelectListItem { Text = x.ClaveEmpleado + " - " + x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, Value = x.IdEmpleado.ToString() }); });

            List<SelectListItem> _conceptos = new List<SelectListItem>();
            ClassConceptos cconceptos = new ClassConceptos();
            List<Cat_ConceptosNomina> lconceptos = cconceptos.GetConceptos(IdCliente).OrderBy(x => x.Concepto).ToList();
            lconceptos.ForEach(x => { _conceptos.Add(new SelectListItem { Text = x.ClaveConcepto + " - " + x.Concepto, Value = x.IdConcepto.ToString() }); });

            ModelIncidenciasProgramadas modelIncidencias = new ModelIncidenciasProgramadas();           
            modelIncidencias.LEmpleados = _empleados;
            modelIncidencias.LConcepto = _conceptos;

            return modelIncidencias;
        }

        /// <summary>
        /// Método auxilir para validar si se desea incluir o no las incidencias programas al periodo de nómina
        /// </summary>
        /// <param name="IncluirDescuentos">si ó no</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void ProcesaIncidenciasProgramadas(string IncluirDescuentos, int IdUnidadNegocio, int IdPeriodoNomina, int IdUsuario)
        {
            deleteIncidenciasInsertadas(IdPeriodoNomina);
            if(IncluirDescuentos == "SI")
                InsertaIncidenciasPeriodo(IdUnidadNegocio, IdPeriodoNomina, IdUsuario);
        }

        /// <summary>
        /// Método auxilir para validar si se desea incluir o no las incidencias programas al periodo de nómina por empleado
        /// </summary>
        /// <param name="IncluirDescuentos">si ó no</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void ProcesaIncidenciasProgramadas(string IncluirDescuentos, int IdUnidadNegocio, int IdPeriodoNomina, int IdEmpleado, int IdUsuario)
        {
            deleteIncidenciasInsertadas(IdPeriodoNomina, IdEmpleado);
            if (IncluirDescuentos == "SI")
                InsertaIncidenciasPeriodo(IdUnidadNegocio, IdPeriodoNomina, IdEmpleado, IdUsuario);
        }

        /// <summary>
        /// Método para eliminar las incidencias programas repetidas del periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        public void deleteIncidenciasInsertadas(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaIncidenciasFijas != null) select b).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para eliminar las incidencias programas repetidas del periodo de nómina por empleado
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del periodo de nómina</param>
        public void deleteIncidenciasInsertadas(int IdPeriodoNomina, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.BanderaIncidenciasFijas != null && x.IdEmpleado == IdEmpleado) select b).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para insertas la sincidencias programas den el periodo de nómina
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del perido de nómina</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void InsertaIncidenciasPeriodo(int IdUnidadNegocio, int IdPeriodoNomina, int IdUsuario)
        {
            ClassIncidencias cins = new ClassIncidencias();
            var insp = getvIncidenciasProgramadas(IdUnidadNegocio).Where(x=> x.Activo == 1);
            
            foreach (var ins in insp)
            {                
                cins.NewIncindencia(IdPeriodoNomina, (int)ins.IdConcepto, (int)ins.IdEmpleado, (decimal)ins.Cantidad, (decimal)ins.Cantidad, (decimal)ins.Monto, (decimal)ins.MontoEsq, null, null, string.Empty, string.Empty, null, null, ins.IdIncidenciasProgramadas, null, null, null, null, IdUsuario);
            }
        }

        /// <summary>
        /// Método para insertas la sincidencias programas den el periodo de nómina por empleado
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void InsertaIncidenciasPeriodo(int IdUnidadNegocio, int IdPeriodoNomina, int IdEmpleado, int IdUsuario)
        {
            ClassIncidencias cins = new ClassIncidencias();
            var insp = getvIncidenciasProgramadas(IdUnidadNegocio).Where(x => x.Activo == 1 && x.IdEmpleado == IdEmpleado);

            foreach (var ins in insp)
            {
                cins.NewIncindencia(IdPeriodoNomina, (int)ins.IdConcepto, (int)ins.IdEmpleado, (decimal)ins.Cantidad, (decimal)ins.Cantidad, (decimal)ins.Monto, (decimal)ins.MontoEsq, null, null, string.Empty, string.Empty, null, null, ins.IdIncidenciasProgramadas, null, null, null, null, IdUsuario);
            }
        }
    }
}