using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassPensionAlimenticia
    {

        /// <summary>
        /// Método para listar a los empleados que dan pensión alimenticia.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el parámetro de la unidad negocio.</param>
        /// <returns>Regresa el listado de los empleados con pensión.</returns>
        public List<BasePensionAlimenticia> getBasePensiones(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var pensiones = (from b in entidad.BasePensionAlimenticia.Where(x => x.idCliente == IdCliente && x.IdEstatus == 1) select b).ToList();

                return pensiones;
            }
        }

        /// <summary>
        /// Método para listar a los empleados que dan pensión alimenticia.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el parámetro de la unidad negocio.</param>
        /// <returns>Regresa el listado de los empleados con pensión.</returns>
        public List<vPensionAlimenticia> getPensiones(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var pensiones = (from b in entidad.vPensionAlimenticia.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).ToList();

                return pensiones;
            }
        }

        /// <summary>
        /// Método para buscar a un empleado que da pensión alimenticia.
        /// </summary>
        /// <param name="IdPensionAlimenticia">Recibe el parámetro del Id de la pensión alimenticia.</param>
        /// <returns>Regresa al empleado con el detalle del modelo.</returns>
        public vPensionAlimenticia getPension(int IdPensionAlimenticia)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var pensiones = (from b in entidad.vPensionAlimenticia.Where(x => x.IdPensionAlimenticia == IdPensionAlimenticia) select b).FirstOrDefault();

                return pensiones;
            }
        }


        public BasePensionAlimenticia getPensionbase(int IdPensionAlimenticia)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var pensiones = (from b in entidad.BasePensionAlimenticia.Where(x => x.IdBasePensionAlimenticia == IdPensionAlimenticia) select b).FirstOrDefault();

                return pensiones;
            }
        }

        /// <summary>
        /// Método para llenar la lista de las pensiones alimneticias utilizando el modelo de pension alimenticia.
        /// </summary>
        /// <param name="IdUnidadNegocio">Parámetro que se recibe para llenar la lista por medio del identificador de la unidad negocio.</param>
        /// <returns>Regresa la lista con la información de la consulta.</returns>
        public List<ModelPensionAlimenticia> getModelPensionAlimenticia(int IdUnidadNegocio)
        {
            var pensiones = getPensiones(IdUnidadNegocio);
            var mpensiones = new List<ModelPensionAlimenticia>();

            pensiones.ForEach(x =>
            {
                mpensiones.Add(new ModelPensionAlimenticia
                {
                    IdPensionAlimenticia = x.IdPensionAlimenticia,
                    IdEmpleado = x.IdEmpleado,
                    ClaveEmp = x.ClaveEmpleado,
                    Empleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                    TipoPension = x.TipoPension,
                    Valor = x.Valor,
                    ValorEsq = x.ValorEsq,
                    NombreBeneficiario = x.NombreBeneficiario,
                    IdEstatus = x.IdEstatus
                });
            });

            return mpensiones;
        }

        /// <summary>
        /// Método que muestra en pantalla los datos de la pensión alimenticia.
        /// </summary>
        /// <param name="IdPensionAlimenticia">Recibe el parámetro del identificador de la pensión alimenticia.</param>
        /// <returns>Regresa el modelo de la pensión alimenticia.</returns>
        public ModelPensionAlimenticia getModelPensionAlimenticiaId(int IdPensionAlimenticia)
        {
            var x = getPension(IdPensionAlimenticia);
            var mPension = new ModelPensionAlimenticia();

            mPension.IdPensionAlimenticia = x.IdPensionAlimenticia;
            mPension.IdEmpleado = x.IdEmpleado;
            mPension.ClaveEmp = x.ClaveEmpleado;
            mPension.Empleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre;
            mPension.TipoPension = x.TipoPension;
            mPension.Valor = x.Valor;
            mPension.ValorEsq = x.ValorEsq;
            mPension.NombreBeneficiario = x.NombreBeneficiario;
            mPension.IdEstatus = x.IdEstatus;
            if (x.IdEstatus == 1) { mPension.Estatus = true; } else { mPension.Estatus = false; }
            mPension.fechaCaptura = x.FechaCaptura;

            return mPension;
        }

        public ModelBasePensionAlimenticia getModelBasePensionAlimenticiaId(int IdPensionAlimenticia)
        {

            var x = getPensionbase(IdPensionAlimenticia);
            var mPension = new ModelBasePensionAlimenticia();

            mPension.IdBasePensionAlimenticia = x.IdBasePensionAlimenticia;
            mPension.Nombre = x.Nombre;
            mPension.Descripcion = x.Descripcion;
            mPension.Formula = x.Formula;
            mPension.IdEstatus = x.IdEstatus;
            if (x.IdEstatus == 1) { mPension.Estatus = true; } else { mPension.Estatus = false; }
            mPension.fechaCaptura = x.FechaCaptura;



            return mPension;
        }



        /// <summary>
        /// Método con el combo box para seleccionar el tipo de cuota de la pensión alimenticia.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getTipos()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Porcentaje", Value = "Porcentaje" });
            list.Add(new SelectListItem { Text = "Cuota Fija", Value = "CuotaFija" });

            return list;
        }

        /// <summary>
        /// Método para agregar un empleado que va a dar pensión alimenticia.
        /// </summary>
        /// <param name="inf">Recibe el modelo de la pensión alimenticia.</param>
        /// <param name="IdUsuario">Recibe el parámetro del identificador del empleado</param>
        public void createPension(ModelPensionAlimenticia inf, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                PensionAlimenticia ci = new PensionAlimenticia();
                ci.IdEmpleado = inf.IdEmpleado;
                ci.Tipo = inf.TipoPension;
                ci.Valor = inf.Valor;
                ci.ValorEsq = inf.ValorEsq;
                ci.NombreBeneficiario = inf.NombreBeneficiario;
                ci.IdEstatus = 1;
                ci.IdCaptura = IdUsuario;
                ci.FechaCaptura = DateTime.Now;

                entidad.PensionAlimenticia.Add(ci);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar la información del empleado que está dando pensión alimenticia.
        /// </summary>
        /// <param name="inf">Recibe el modelo de la pensión alimenticia.</param>
        /// <param name="IdUsuario">Recibe el parámetro del identificador del empleado.</param>
        public void editPension(ModelPensionAlimenticia inf, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var pension = (from b in entidad.PensionAlimenticia where b.IdPensionAlimenticia == inf.IdPensionAlimenticia select b).FirstOrDefault();

                if (pension != null)
                {
                    pension.Tipo = inf.TipoPension;
                    pension.Valor = inf.Valor;
                    pension.ValorEsq = inf.ValorEsq;
                    pension.NombreBeneficiario = inf.NombreBeneficiario;
                    pension.IdModifica = IdUsuario;
                    pension.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }


        public void editPensionBase(ModelBasePensionAlimenticia inf, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var pension = (from b in entidad.BasePensionAlimenticia where b.IdBasePensionAlimenticia == inf.IdBasePensionAlimenticia select b).FirstOrDefault();

                if (pension != null)
                {
                    pension.Nombre = inf.Nombre;
                    pension.Descripcion = inf.Descripcion;
                    pension.Formula = inf.Formula;
                    pension.IdModifica = IdUsuario;
                    pension.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para borrar el registro del empleado que está dando pensión alimenticia.
        /// </summary>
        /// <param name="IdPensionAlimenticia">Recibe el parámetro del identificador de la pensión alimenticia.</param>
        /// <param name="IdUsuario">Recibe el parámetro del identificador del empleado</param>
        public void DeletePension(int IdPensionAlimenticia, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var pension = (from b in entidad.PensionAlimenticia.Where(x => x.IdPensionAlimenticia == IdPensionAlimenticia) select b).FirstOrDefault();

                if (pension != null)
                {
                    pension.IdEstatus = 2;
                    pension.IdModifica = IdUsuario;
                    pension.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }


        /// <param name="IdUsuario">Recibe el parámetro del identificador del empleado</param>
        public void DeletePensionBase(int IdPensionAlimenticia, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var pension = (from b in entidad.BasePensionAlimenticia.Where(x => x.IdBasePensionAlimenticia == IdPensionAlimenticia) select b).FirstOrDefault();

                if (pension != null)
                {
                    pension.IdEstatus = 2;
                    pension.IdModifica = IdUsuario;
                    pension.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina las incidencias que se crean de la carga de las pensiones alimenticias
        /// </summary>
        /// <param name="IdCreditoInfonavit">Identificador de la pension alimenticia</param>
        /// <param name="IdUsuario">Identificador del usuario que elimina la incidencia</param>
        public void DeleteIncidenciasPension(int IdPension, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.BanderaPensionAlimenticia == IdPension && x.IdEstatus == 1).ToList();

                foreach (var item in incidencias)
                {
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }



        public ModelBasePensionAlimenticia FindListConceptos(int idcliente)
        {
            ClassConceptos classConceptos = new ClassConceptos();
            List<SelectListItem> _TipoNomima = new List<SelectListItem>();
            var tipoPeriodo = classConceptos.GetvConceptos(idcliente);
            tipoPeriodo.ForEach(x => _TipoNomima.Add(new SelectListItem { Text = x.ClaveConcepto + '-' + x.Concepto, Value = x.IdConcepto.ToString() }));
            ModelBasePensionAlimenticia model = new ModelBasePensionAlimenticia
            {

                LTipoNomina = _TipoNomima,
            };
            return model;
        }

        public ModelBasePensionAlimenticia FindListConceptosbase(int idcliente, ModelBasePensionAlimenticia model)
        {
            ClassConceptos classConceptos = new ClassConceptos();
            List<SelectListItem> _TipoNomima = new List<SelectListItem>();
            var tipoPeriodo = classConceptos.GetvConceptos(idcliente);
            tipoPeriodo.ForEach(x => _TipoNomima.Add(new SelectListItem { Text = x.ClaveConcepto + '-' + x.Concepto, Value = x.IdConcepto.ToString() }));
            model.LTipoNomina = _TipoNomima;
            return model;
        }

        public void AddBasePension(ModelBasePensionAlimenticia model, int pIdCliente, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                BasePensionAlimenticia depto = new BasePensionAlimenticia
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Formula = model.Formula,
                    idCliente = pIdCliente,
                    IdEstatus = 1,
                    FechaCaptura = DateTime.Now,
                    IdCaptura = pIdUsuario
                };

                entidad.BasePensionAlimenticia.Add(depto);
                entidad.SaveChanges();
            }
        }
    }
}