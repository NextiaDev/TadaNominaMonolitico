using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore.IMSS
{
    public class ClassRecuperacionesIMSS
    {
        //Busqueda empleado 
        public List<vEmpleados> GetEmpleados(int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<vEmpleados> empleados = new List<vEmpleados>();
                var query = (from b in entity.vEmpleados where b.IdUnidadNegocio == idUnidadNegocio && b.IdRegistroPatronal>0 && b.SDI>0 select b).ToList();
                query.ForEach(x => {
                    empleados.Add(new vEmpleados
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        NombrePatrona= x.NombrePatrona,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        Estatus= x.Estatus
                    });
                });

                return empleados;
            }
        }

        //Busqueda de empleados por Clave
        public List<vEmpleados> GetEmpleadosByClave(string clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<vEmpleados> empleados = new List<vEmpleados>();
                var query = (from b in entity.vEmpleados  where b.ClaveEmpleado == clave && b.IdUnidadNegocio == idUnidadNegocio && b.IdRegistroPatronal > 0 && b.SDI > 0 select b).ToList();

                query.ForEach(x => {
                    empleados.Add(new vEmpleados
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        NombrePatrona = x.NombrePatrona,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        Estatus = x.Estatus
                    });
                });
                return empleados;
            }
        }

        //Busqueda de Empleado por NOmbre
        public List<vEmpleados> GetEmpleadosByNombre(string nombre, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<vEmpleados> empleados = new List<vEmpleados>();
                var query = (from b in entity.vEmpleados where (b.Nombre + " " + b.ApellidoPaterno + " " + b.ApellidoMaterno).StartsWith(nombre) && b.IdUnidadNegocio == idUnidadNegocio && b.IdRegistroPatronal > 0 && b.SDI > 0 select b).ToList();

                query.ForEach(x => {
                    empleados.Add(new vEmpleados
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        NombrePatrona = x.NombrePatrona,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        Estatus = x.Estatus
                    });
                });
                return empleados;
            }
        }

        public ModelRecuperaciones GetModelRecuperaciones(int IdEmpleado, int IdUnidadNegocio)
        {
            ModelRecuperaciones m = new ModelRecuperaciones();

            m.IdEmpleado = IdEmpleado;
            m.IdRegistroPatronal= GetRegistroP(IdEmpleado);
            try { m.PeriodosList = GetPeriodos(IdUnidadNegocio); } catch { m.PeriodosList = new List<SelectListItem>(); }
            try { m.ConceptoList = GetConceptos(); } catch { m.ConceptoList = new List<SelectListItem>(); }

            return m;
        }

        private int GetRegistroP(int IdEmpleado)
        {
            using (TadaEmpleadosEntities entidad= new TadaEmpleadosEntities())
            {
                var IdRP = (from b in entidad.Empleados where b.IdEmpleado == IdEmpleado select b).FirstOrDefault();

                return (int)IdRP.IdRegistroPatronal;
            }
        }

        private List<SelectListItem> GetConceptos()
        {
            List<SelectListItem> conceptos = new List<SelectListItem>
            {
                new SelectListItem() { Text = "IMSS", Value = "IMSS" },
                new SelectListItem() { Text = "INFONAVIT", Value = "INFONAVIT" }
            };
            return conceptos;
        }

        private List<SelectListItem> GetPeriodos(int IdUnidadNegocio)
        {
            List<SelectListItem> periodos = new List<SelectListItem>();

            using (NominaEntities1 entity = new NominaEntities1())
            {
                var rp = (from b in entity.PeriodoNomina where b.IdUnidadNegocio == IdUnidadNegocio orderby b.IdPeriodoNomina descending select b).ToList();
                rp.ForEach(x => { periodos.Add(new SelectListItem { Text = x.Periodo.ToUpper(), Value = x.IdPeriodoNomina.ToString() }); });                
            }

            return periodos;
        }

        public bool NuevaRecuperacion(ModelRecuperaciones m)
        {
            bool resultado = false;
            try
            {
                RecuperacionIMSS recuperacion = new RecuperacionIMSS();

                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    recuperacion.IdEmpleado = m.IdEmpleado;
                    recuperacion.IdPeriodoNomina = m.IdPeriodoNomina;
                    recuperacion.IdRegistroPatronal = m.IdRegistroPatronal;
                    recuperacion.Monto = m.monto;
                    recuperacion.Observaciones = m.Observaciones;
                    recuperacion.TipoRecuperacion = m.ConceptoRecuperacion;
                    recuperacion.IdCaptura = m.Idusuario;

                    recuperacion.IdEstatus = 1;
                    recuperacion.FechaCreacion = DateTime.Now;

                    entidad.RecuperacionIMSS.Add(recuperacion);
                    entidad.SaveChanges();

                    resultado = true;
                }

                return resultado;
            }
            catch (Exception)
            {
                resultado = false;
                return resultado;
            }            
        }
    }
}