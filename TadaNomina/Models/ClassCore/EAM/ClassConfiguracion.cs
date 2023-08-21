using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore.EAM
{
    public class ClassConfiguracion
    {
        /// <summary>
        ///     Método que obtiene las horas que trabajó un empleado
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad negocio</param>
        /// <returns>Modelo con la información del empleado</returns>
        public ModelConfEmpleadoHoras GetModel(int IdEmpleado, int IdUnidadNegocio)
        {
            ClassEmpleado ce = new ClassEmpleado();
            var empleado = ce.GetvEmpleado(IdEmpleado);  
            var materias = GetModelMaterias();
            var empleados = getPersonalCargo(IdUnidadNegocio, IdEmpleado);

            List<SelectListItem> ltipo = new List<SelectListItem>();
            ltipo.Add(new SelectListItem { Text = "Porcentaje", Value = "Porcentaje" });
            ltipo.Add(new SelectListItem { Text = "Monto", Value = "Monto" });

            ModelConfEmpleadoHoras model = new ModelConfEmpleadoHoras();
            model.empleado = empleado;
            model.empleados = empleados;
            model.Materias = materias;
            model.lTipoBono = ltipo;

            return model;
        }

        /// <summary>
        ///     Método que obtiene a los empleados de una unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <returns>Lista con la información de todos los empleados excepto el seleccionado</returns>
        public List<ModePersonalACargo> getPersonalCargo(int IdUnidadNegocio, int IdEmpleado)
        {
            ClassEmpleado ce = new ClassEmpleado();
            var empleados = ce.GetvEmpleados(IdUnidadNegocio).Where(x => x.IdEmpleado != IdEmpleado).ToList();

            List<ModePersonalACargo> model = new List<ModePersonalACargo>();

            empleados.ForEach(x=> { model.Add(new ModePersonalACargo { IdEmpleado = x.IdEmpleado, Cve = x.ClaveEmpleado, Nombre = x.NombreCompleto }); });

            return model;
        }

        /// <summary>
        ///     Método que obtiene las materias
        /// </summary>
        /// <returns>Listado del catálogo de materias</returns>
        public List<EAM_Materias> getMaterias()
        {
            using (EAMEntities entidad = new EAMEntities())
            {
                var mat = (from b in entidad.EAM_Materias.Where(x => x.IdEstatus == 1) select b).ToList();

                return mat;
            }
        }

        /// <summary>
        ///     Método que convierte el catálogo de materias en un modelo de materias
        /// </summary>
        /// <returns>Lista de modelos con la inforamción de las materias</returns>
        public List<ModelMaterias> GetModelMaterias()
        {
            var mat = getMaterias();
            List<ModelMaterias> meterias = new List<ModelMaterias>();

            mat.ForEach(x=> { meterias.Add(new ModelMaterias { IdMateria = x.IdMateria, Materia = x.Materia }); });

            return meterias;
        }
    }
}