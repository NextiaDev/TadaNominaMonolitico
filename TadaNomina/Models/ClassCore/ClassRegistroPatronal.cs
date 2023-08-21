using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;
using System.Text;
using System.Web.Mvc;

namespace TadaNomina.Models.ClassCore
{
    public class ClassRegistroPatronal
    {
        /// <summary>
        /// Método para mostrar la lista de la actividad económica.
        /// </summary>
        /// <returns>Regresa la lista de la actividad económica.</returns>
        public ModelRegistroPatronal LlenaListaActividadEconomica()
        {           
            List<SelectListItem> _Actividad = new List<SelectListItem>();

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var a = (from b in entidad.ActividadEconomica where b.IdEstatus == 1 select b).OrderBy(x => x.IdActividadEconomica).ToList();

                a.ForEach(x => { _Actividad.Add(new SelectListItem { Text = x.ActividadEconomica1, Value = x.IdActividadEconomica.ToString() }); });
            }

            ModelRegistroPatronal modelRegistroPatronal = new ModelRegistroPatronal();
            modelRegistroPatronal.LActividadEconomica = _Actividad;
            return modelRegistroPatronal;
        }

        /// <summary>
        /// Método para mostrar la lista de actividad económica con un modelo de registro patronal.
        /// </summary>
        /// <param name="modelRegistroPatronal">Recibe el modelo del registro patronal.</param>
        /// <returns>Regresa el modelo del registro patronal.</returns>
        public ModelRegistroPatronal LlenaListaActividadEconomica(ModelRegistroPatronal modelRegistroPatronal)
        {
            List<SelectListItem> _Actividad = new List<SelectListItem>();

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var a = (from b in entidad.ActividadEconomica where b.IdEstatus == 1 select b).OrderBy(x => x.IdActividadEconomica).ToList();

                a.ForEach(x => { _Actividad.Add(new SelectListItem { Text = x.ActividadEconomica1, Value = x.IdActividadEconomica.ToString() }); });
            }
            modelRegistroPatronal.LActividadEconomica = _Actividad;
            return modelRegistroPatronal;
        }

        /// <summary>
        /// Método para listar los registros patronales por cliente.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public List<Cat_RegistroPatronal> GetRegistroPatronalByIdCliente(int IdCliente)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var Patrona = from b in entidad.Cat_RegistroPatronal.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b;
                return Patrona.ToList();
            }
        }

        /// <summary>
        /// Obtiene el registro patronal en base al identificador
        /// </summary>
        /// <param name="IdRegistroPatronal"></param>
        /// <returns>regresa el objeto de la tabla del registro patronal</returns>
        public Cat_RegistroPatronal GetRegistroPatronalById(int IdRegistroPatronal)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var Patrona = entidad.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal && x.IdEstatus == 1).FirstOrDefault();
                return Patrona;
            }
        }

        /// <summary>
        /// Método para listar los registros patronales.
        /// </summary>
        /// <param name="IdsReg">Recibe una lista de tipo int.</param>
        /// <returns>Regresa la lista de registros patronales.</returns>
        public List<Cat_RegistroPatronal> GetRegistroPatronalByIds(List<int?> IdsReg)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var Patrona = from b in entidad.Cat_RegistroPatronal.Where(x => IdsReg.Contains(x.IdRegistroPatronal)) select b;
                return Patrona.ToList();
            }
        }

        /// <summary>
        /// Método para agregar un registro patronal.
        /// </summary>
        /// <param name="model">Recibe el modelo del registro patronal.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        public void AddRegistroPatronal(ModelRegistroPatronal model, int pIdUsuario, int pIdCliente)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                Cat_RegistroPatronal Patrona = new Cat_RegistroPatronal
                {
                    IdCliente = pIdCliente,
                    NombrePatrona = model.NombrePatrona,
                    RegistroPatronal = model.RegistroPatronal,
                    RFC = model.RFC,
                    Clase = model.Clase,
                    RiesgoTrabajo = model.RiesgoTrabajo,
                    Direccion = model.Direccion,
                    CP = model.CP,
                    Pais = model.Pais,
                    EntidadFederativa = model.EntidadFederativa,
                    Municipio = model.Municipio,
                    Colonia = model.Colonia,
                    Calle = model.Calle,
                    IdActividadEconomica = model.IdActividadEconomica,
                    SelloDigital = model.SelloDigital,
                    NombreRepresentante = model.RepresentanteLegal,
                    IdEstatus = 1,
                    IdBanco = model.IdBanco,
                    CuentaBancaria = model.CuentaBancaria,
                    FechaCaptura = DateTime.Now,
                    IdCaptura = pIdUsuario
                };

                entidad.Cat_RegistroPatronal.Add(Patrona);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para llenar el modelo de registro patronal.
        /// </summary>
        /// <param name="pIdRegistroPatronal">Recibe una variable tipo int.</param>
        /// <returns>Regresa el modelo del registro patronal.</returns>
        public ModelRegistroPatronal GetModelRegistroPatronal(int pIdRegistroPatronal)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                ModelRegistroPatronal modelRegistroPatronal = new ModelRegistroPatronal();

                var registro = (from b in entidad.Cat_RegistroPatronal
                                where b.IdRegistroPatronal == pIdRegistroPatronal
                                select b).FirstOrDefault();

                if (registro != null)
                {
                    modelRegistroPatronal.IdActividadEconomica = registro.IdActividadEconomica;
                    modelRegistroPatronal.IdCliente = (int)registro.IdCliente;
                    modelRegistroPatronal.NombrePatrona = registro.NombrePatrona;
                    modelRegistroPatronal.RegistroPatronal = registro.RegistroPatronal;
                    modelRegistroPatronal.RFC = registro.RFC;
                    modelRegistroPatronal.Clase = (int)registro.Clase;
                    modelRegistroPatronal.RiesgoTrabajo = (decimal)registro.RiesgoTrabajo;
                    modelRegistroPatronal.RiesgoTrabajoS = registro.RiesgoTrabajo.ToString();
                    modelRegistroPatronal.Direccion = registro.Direccion;
                    modelRegistroPatronal.CP = registro.CP;
                    modelRegistroPatronal.Pais = registro.Pais;
                    modelRegistroPatronal.EntidadFederativa = registro.EntidadFederativa;
                    modelRegistroPatronal.Municipio = registro.Municipio;
                    modelRegistroPatronal.Colonia = registro.Colonia;
                    modelRegistroPatronal.Calle = registro.Calle;
                    modelRegistroPatronal.IdActividadEconomica = (int)registro.IdActividadEconomica;
                    modelRegistroPatronal.SelloDigital = registro.SelloDigital;
                    modelRegistroPatronal.KeyPass = registro.KeyPass;
                    modelRegistroPatronal.Logo = registro.Logo;
                    modelRegistroPatronal.IdBanco = registro.IdBanco;
                    modelRegistroPatronal.CuentaBancaria = registro.CuentaBancaria;
                    modelRegistroPatronal.RepresentanteLegal = registro.NombreRepresentante;

                    return modelRegistroPatronal;
                }
                else
                {
                    return modelRegistroPatronal;
                }
            }
        }

        /// <summary>
        /// Método para modificar la información del registro patronal.
        /// </summary>
        /// <param name="model">Recibe el modelo del registro patronal.</param>
        /// <param name="pIdRegistroPatronal">Recibe el identificador del registro patronal.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void UpdateRegistroPatronal(ModelRegistroPatronal model, int pIdRegistroPatronal, int pIdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = (from b in entidad.Cat_RegistroPatronal
                                where b.IdRegistroPatronal == pIdRegistroPatronal
                                select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.NombrePatrona = model.NombrePatrona;
                    registro.RegistroPatronal = model.RegistroPatronal;
                    registro.RFC = model.RFC;
                    registro.Clase = model.Clase;
                    registro.RiesgoTrabajo = decimal.Parse(model.RiesgoTrabajoS);
                    registro.Direccion = model.Direccion;
                    registro.CP = model.CP;
                    registro.Pais = model.Pais;
                    registro.EntidadFederativa = model.EntidadFederativa;
                    registro.Municipio = model.Municipio;
                    registro.Colonia = model.Colonia;
                    registro.Calle = model.Calle;
                    registro.IdActividadEconomica = model.IdActividadEconomica;
                    registro.SelloDigital = model.SelloDigital;
                    registro.NombreRepresentante = model.RepresentanteLegal;
                    registro.IdBanco = model.IdBanco;
                    registro.CuentaBancaria = model.CuentaBancaria;

                    registro.FechaCaptura = DateTime.Now;
                    registro.IdCaptura = pIdUsuario;
                    registro.IdModificacion = pIdUsuario;
                    registro.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar un registro patronal.
        /// </summary>
        /// <param name="Id">Recibe el identificador del registro patronal.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void DeleteRegistroPatrona(int Id, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = (from b in entidad.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == Id)
                                select b).FirstOrDefault();

                registro.IdEstatus = 2;
                registro.IdCaptura = IdUsuario;
                registro.FechaCaptura = DateTime.Now;

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para actualizar los cetificados de los sellos digitales.
        /// </summary>
        /// <param name="Id">Recibe el identificador del registro patronal.</param>
        /// <param name="rutaCer">Recibe la ruta del archivo .cer.</param>
        /// <param name="rutaKey">Recibe la ruta del archivo .key</param>
        /// <param name="pass">Recibe la contraseña de los archivos.</param>
        public void updateRutasCerKey(int Id, string rutaCer, string rutaKey, string rutaPfx, string pass)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = (from b in entidad.Cat_RegistroPatronal.Where(x=> x.IdRegistroPatronal == Id)                               
                                select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.rutaCer = rutaCer;
                    registro.rutaKey = rutaKey;
                    registro.KeyPass = pass;
                    registro.PFXCancelacionTimbrado = rutaPfx;

                    entidad.SaveChanges();
                }
            }
        }

        public void updateRutasCerKeyFIEL(int Id, string rutaCer, string rutaKey, string pass)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = (from b in entidad.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == Id)
                                select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.rutaFielCer = rutaCer;
                    registro.rutaFielKey = rutaKey;
                    registro.KeyPassFiel = pass;                    

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para listar los registros patronales.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de los regresos patronales.</returns>
        public List<SelectListItem> getSelectRegistro(int IdCliente)
        {
            var registros = GetRegistroPatronalByIdCliente(IdCliente);
            var idsRegistros = getIdsEspecializadas(IdCliente);
            var registrosIds = GetRegistroPatronalByIds(idsRegistros);
            var list = new List<SelectListItem>();

            registros.ForEach(x=> { list.Add(new SelectListItem { Text = x.NombrePatrona, Value = x.IdRegistroPatronal.ToString() }); });
            registrosIds.ForEach(x => { list.Add(new SelectListItem { Text = x.NombrePatrona, Value = x.IdRegistroPatronal.ToString() }); });

            return list;
        }

        /// <summary>
        /// Método para listar los Ids de los registros patronales con empresa especializada.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de los registros patronales especializados.</returns>
        public List<int?> getIdsEspecializadas(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var reg = entidad.Cliente_EmpresaEspecializada.Where(x => x.IdEstatus == 1 && x.IdCliente == IdCliente).Select(x=> x.IdRegistroPatronal).ToList();

                return reg;
            }
        }
    }
}