using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;
using System.Runtime.InteropServices;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Factor de integración
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassFactorIntegracion
    {
        /// <summary>
        /// Método para obtener las prestaciones
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Listado de las prestaciones</returns>
        public List<Prestaciones> GetPrestaciones(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestaciones = (from b in entidad.Prestaciones.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();

                return prestaciones;
            }
        }

        /// <summary>
        /// Método para obtener las preataciones con informacion complementaria
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Listado de las preatciones con información complementaria</returns>
        public List<vPrestaciones> GetvPrestaciones(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestaciones = (from b in entidad.vPrestaciones.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();

                return prestaciones;
            }
        }

        /// <summary>
        /// Método para obtener la información de la prestación
        /// </summary>
        /// <param name="IdPrestacion">Identificador de la prestación</param>
        /// <returns>Información de la prestación</returns>
        public Prestaciones GetPrestacion(int IdPrestacion)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestaciones = (from b in entidad.Prestaciones.Where(x => x.IdPrestaciones == IdPrestacion) select b).FirstOrDefault();

                return prestaciones;
            }
        }

        /// <summary>
        /// Método para obtener la prestación con información complementaria
        /// </summary>
        /// <param name="IdPrestacion">Identificador e la prestación</param>
        /// <returns>Prestacion con información complementaria</returns>
        public vPrestaciones GetvPrestacion(int IdPrestacion)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestaciones = (from b in entidad.vPrestaciones.Where(x => x.IdPrestaciones == IdPrestacion) select b).FirstOrDefault();

                return prestaciones;
            }
        }

        /// <summary>
        /// Método para obtener un listado de tipo ModelPrestaciones
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>ModelPrestaciones</returns>
        public List<ModelPrestaciones> GetModelPrestaciones(int IdCliente)
        {
            List<vPrestaciones> prestaciones = GetvPrestaciones(IdCliente);
            List<ModelPrestaciones> mprestaciones = new List<ModelPrestaciones>();

            prestaciones.ForEach(x => { mprestaciones.Add(new ModelPrestaciones { IdCliente = (int)x.IdCliente, IdPrestacion = x.IdPrestaciones, Prestacion = x.Descripcion, Cliente = x.Cliente }); });

            return mprestaciones;
        }

        /// <summary>
        /// Método paara llenar el ModelPrestaciones
        /// </summary>
        /// <param name="IdPrestacion">Identificador de la prestación</param>
        /// <returns>ModelPrestaciones</returns>
        public ModelPrestaciones GetModelPrestacion(int IdPrestacion)
        {
            vPrestaciones prestaciones = GetvPrestacion(IdPrestacion);
            ModelPrestaciones mprestaciones = new ModelPrestaciones()
            {
                IdPrestacion = prestaciones.IdPrestaciones,
                Prestacion = prestaciones.Descripcion,
                IdCliente = (int)prestaciones.IdCliente,
                Cliente = prestaciones.Cliente
            };
            return mprestaciones;
        }

        /// <summary>
        /// Método para agregar un aprestación
        /// </summary>
        /// <param name="model">ModelPrestaciones</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void AddPrestacion(ModelPrestaciones model, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                Prestaciones p = new Prestaciones()
                {
                    IdCliente = model.IdCliente,
                    Descripcion = model.Prestacion,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.Prestaciones.Add(p);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para editar un aprestación
        /// </summary>
        /// <param name="model">ModelPrestaciones</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void EditPrestacion(ModelPrestaciones model, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestacion = (from b in entidad.Prestaciones.Where(x => x.IdPrestaciones == model.IdPrestacion) select b).FirstOrDefault();

                if (prestacion != null)
                {
                    prestacion.Descripcion = model.Prestacion;
                    prestacion.IdModifica = IdUsuario;
                    prestacion.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eleiminar una prestación
        /// </summary>
        /// <param name="IdPrestacion">Identificadotr de la prestación</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void DeletePrestacion(int IdPrestacion, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestacion = (from b in entidad.Prestaciones.Where(x => x.IdPrestaciones == IdPrestacion) select b).FirstOrDefault();

                if (prestacion != null)
                {
                    prestacion.IdEstatus = 2;
                    prestacion.IdModifica = IdUsuario;
                    prestacion.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para ontener un listado de los factores de integración
        /// </summary>
        /// <returns>Listado de los factores de integración</returns>
        public List<FactorIntegracion> GetFactores()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var factor = from b in entidad.FactorIntegracion.Where(x => x.IdEstatus == 1) select b;

                return factor.ToList();
            }
        }

        /// <summary>
        /// Método para obtener un listado de los factores de integración por su identificador
        /// </summary>
        /// <param name="IdPrestaciones">Listado de los identificadores de las prestaciones</param>
        /// <returns>Listado de los factores de integración</returns>
        public List<FactorIntegracion> GetFactorByIdUnidadNegocio(int IdPrestaciones)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var factor = from b in entidad.FactorIntegracion.Where(x => x.IdPrestaciones == IdPrestaciones && x.IdEstatus == 1) select b;

                return factor.ToList();
            }
        }

        /// <summary>
        /// Método para agaregar un Factor de integración
        /// </summary>
        /// <param name="model">ModelFactorIntegracion</param>
        /// <param name="pIdUsuario">Identificador del usuario</param>
        public void AddFactorIntegracion(ModelFactorIntegracion model, int pIdUsuario)
        {
            decimal _fi = Math.Round((365 + (model.Aguinaldo) + (model.Vacaciones * (model.PrimaVacacional / 10))) / 365, 4);

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                FactorIntegracion factor = new FactorIntegracion
                {
                    Limite_Inferior = model.Limite_Inferior,
                    Limite_Superior = model.Limite_Superior,
                    Aguinaldo = model.Aguinaldo,
                    Vacaciones = model.Vacaciones,
                    PrimaVacacional = model.PrimaVacacional,
                    PrimaVacacionalSDI = model.PrimaVacacionalSDI,
                    FactorIntegracion1 = _fi,
                    FechaInicioVigencia = model.FechaInicioVigencia,
                    IdPrestaciones = model.IdPrestaciones,
                    IdEstatus = 1,
                    FechaCaptura = DateTime.Now,
                    IdCaptura = pIdUsuario
                };

                entidad.FactorIntegracion.Add(factor);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para editar el factoe de integración
        /// </summary>
        /// <param name="model">ModelFactorIntegracion</param>
        /// <param name="pIdFactorIntegracion">Identificador del factor de integración</param>
        /// <param name="pIdUsuario">Identificador del usuario</param>
        public void UpdateFactorIntegracion(ModelFactorIntegracion model, int pIdFactorIntegracion, int pIdUsuario)
        {
            decimal _fi = Math.Round((365 + (model.Aguinaldo) + (model.Vacaciones * (model.PrimaVacacional / 10))) / 365, 4);

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var factor = (from b in entidad.FactorIntegracion
                              where b.IdFactorIntegracion == pIdFactorIntegracion
                              select b).FirstOrDefault();

                if (factor != null)
                {
                    factor.Limite_Inferior = model.Limite_Inferior;
                    factor.Limite_Superior = model.Limite_Superior;
                    factor.Aguinaldo = model.Aguinaldo;
                    factor.Vacaciones = model.Vacaciones;
                    factor.PrimaVacacional = model.PrimaVacacional;
                    factor.PrimaVacacionalSDI = model.PrimaVacacionalSDI;
                    factor.FactorIntegracion1 = _fi;
                    factor.FechaCaptura = DateTime.Now;
                    factor.IdCaptura = pIdUsuario;
                    factor.FechaInicioVigencia = model.FechaInicioVigencia;
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar un factor de integración
        /// </summary>
        /// <param name="pIdFactorIntegracion">Identificador del factor de integración</param>
        /// <param name="pIdUsuario">Identificador del usuario</param>
        public void DeleteFactorIntegracion(int pIdFactorIntegracion, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var factor = (from b in entidad.FactorIntegracion
                              where b.IdFactorIntegracion == pIdFactorIntegracion
                              select b).FirstOrDefault();


                if (factor != null)
                {
                    factor.IdEstatus = 2;
                    factor.FechaCaptura = DateTime.Now;
                    factor.IdCaptura = pIdUsuario;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para llenar el ModelFactorIntegracion
        /// </summary>
        /// <param name="pIdFactorIntegracion">Identificador del factor de integración</param>
        /// <returns>ModelFactorIntegracion</returns>
        public ModelFactorIntegracion GetModelFactorIntegracion(int pIdFactorIntegracion)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelFactorIntegracion modelFactorIntegracion = new ModelFactorIntegracion();

                var factor = (from b in entidad.FactorIntegracion
                              where b.IdFactorIntegracion == pIdFactorIntegracion
                              select b).FirstOrDefault();

                if (factor != null)
                {
                    modelFactorIntegracion.IdFactorIntegracion = factor.IdFactorIntegracion;
                    modelFactorIntegracion.IdPrestaciones = (int)factor.IdPrestaciones;
                    modelFactorIntegracion.Limite_Inferior = (decimal)factor.Limite_Inferior;
                    modelFactorIntegracion.Limite_Superior = (decimal)factor.Limite_Superior;
                    modelFactorIntegracion.Aguinaldo = (int)factor.Aguinaldo;
                    modelFactorIntegracion.Vacaciones = (int)factor.Vacaciones;
                    modelFactorIntegracion.PrimaVacacional = (decimal)factor.PrimaVacacional;
                    modelFactorIntegracion.PrimaVacacionalSDI = (decimal)factor.PrimaVacacionalSDI;
                    modelFactorIntegracion.FactorIntegracion = (decimal)factor.FactorIntegracion1;
                    modelFactorIntegracion.FechaInicioVigencia = factor.FechaInicioVigencia;

                    return modelFactorIntegracion;
                }
                else
                {
                    return modelFactorIntegracion;
                }
            }
        }

        /// <summary>
        /// Método para gregar factores de integración por layout
        /// </summary>
        /// <param name="ruta">Ruat del archivo</param>
        /// <param name="IdPrestaciones">Identificador de la prestación</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <returns>ModelErroresFactorIntegracion</returns>
        public ModelErroresFactorIntegracion GetFactorIntegracionArchivo(string ruta, int IdPrestaciones, int IdUnidadNegocio, int IdUsuario)
        {
            ModelErroresFactorIntegracion modelErrores = new ModelErroresFactorIntegracion();
            modelErrores.listErrores = new List<string>();
            modelErrores.Correctos = 0;
            modelErrores.Errores = 0;
            modelErrores.noRegistro = 0;
            modelErrores.Path = Path.GetFileName(ruta);

            ArrayList array = GetArrayFactor(ruta);

            List<ModelFactorIntegracion> factores = new List<ModelFactorIntegracion>();

            foreach (var item in array)
            {
                modelErrores.noRegistro++;
                AddRegidtroFactor(modelErrores, factores, IdPrestaciones, item);
            }

            try { NewFactor(factores, IdUsuario); } catch (Exception ex) { modelErrores.listErrores.Add(ex.ToString()); }

            return modelErrores;
        }

        /// <summary>
        /// Método par aobtener los errores al caragr los factores d eintegración
        /// </summary>
        /// <param name="ruta">Ruta del archivo</param>
        /// <returns>Arreglo de errores</returns>
        public ArrayList GetArrayFactor(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;
            int contador = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (contador > 0)
                    {
                        arrText.Add(sLine);
                    }
                    contador++;
                }
            }
            objReader.Close();
            return arrText;
        }

        /// <summary>
        /// Método para insertar en l abase datos los nuevos factores de integración
        /// </summary>
        /// <param name="errores">listado de errores</param>
        /// <param name="factores">listado de factores</param>
        /// <param name="IdPrestaciones">Identificador de la prestación</param>
        /// <param name="item">información</param>
        private void AddRegidtroFactor(ModelErroresFactorIntegracion errores, List<ModelFactorIntegracion> factores, int IdPrestaciones, object item)
        {
            string[] campos = item.ToString().Split(',');

            decimal Limite_Inferior = 0;
            decimal Limite_Superior = 0;
            int Aguinaldo = 0;
            int Vacaciones = 0;
            decimal PrimaVacacional = 0;
            decimal PrimaVacacionalSDI = 0;
            decimal FactorIntegracion = 0;

            try { Limite_Inferior = decimal.Parse(campos[0]); } catch { Limite_Inferior = 0; }
            try { Limite_Superior = decimal.Parse(campos[1]); } catch { Limite_Superior = 0; }
            try { Aguinaldo = int.Parse(campos[2]); } catch { Aguinaldo = 0; }
            try { Vacaciones = int.Parse(campos[3]); } catch { Vacaciones = 0; }
            try { PrimaVacacional = decimal.Parse(campos[4]); } catch { PrimaVacacional = 0; }
            try { PrimaVacacionalSDI = decimal.Parse(campos[5]); } catch { PrimaVacacionalSDI = 0; }
            try { FactorIntegracion = decimal.Parse(campos[6]); } catch { FactorIntegracion = 0; }

            decimal _fi = Math.Round((365 + (Aguinaldo) + (Vacaciones * (PrimaVacacional / 10))) / 365, 4);

            errores.listErrores.AddRange(ValidaCamposArchivo(campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelFactorIntegracion i = new ModelFactorIntegracion();
                i.IdPrestaciones = IdPrestaciones;
                i.Limite_Inferior = Limite_Inferior;
                i.Limite_Superior = Limite_Superior;
                i.Aguinaldo = Aguinaldo;
                i.Vacaciones = Vacaciones;
                i.PrimaVacacional = PrimaVacacional;
                i.PrimaVacacionalSDI = PrimaVacacionalSDI;
                i.FactorIntegracion = _fi;
                factores.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método para validar los campos del archivo
        /// </summary>
        /// <param name="campos">información de los archivos</param>
        /// <param name="NoRegistro">numero del registro</param>
        /// <returns>lista de la nformación</returns>
        public List<string> ValidaCamposArchivo(string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            decimal Limite_Inferior = 0;
            decimal Limite_Superior = 0;
            int Aguinaldo = 0;
            int Vacaciones = 0;
            decimal PrimaVacacional = 0;
            decimal PrimaVacacionalSDI = 0;
            decimal FactorIntegracion = 0;

            try { Limite_Inferior = decimal.Parse(campos[0]); } catch { Limite_Inferior = 0; }
            try { Limite_Superior = decimal.Parse(campos[1]); } catch { Limite_Superior = 0; }
            try { Aguinaldo = int.Parse(campos[2]); } catch { Aguinaldo = 0; }
            try { Vacaciones = int.Parse(campos[3]); } catch { Vacaciones = 0; }
            try { PrimaVacacional = decimal.Parse(campos[4]); } catch { PrimaVacacional = 0; }
            try { PrimaVacacionalSDI = decimal.Parse(campos[5]); } catch { PrimaVacacionalSDI = 0; }
            try { FactorIntegracion = decimal.Parse(campos[6]); } catch { FactorIntegracion = 0; }


            if (Limite_Inferior == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[0] + " - El valor del Límete Inferior, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Limite_Superior == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[1] + " - El valor del Límite Superior, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Aguinaldo == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[2] + " - El valor del Aguinaldo, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Vacaciones == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[3] + " - El valor de los Días de Vaciones, no es correcto.";
                errores.Add(Mensaje);
            }

            if (PrimaVacacional == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[4] + " - El valor de la Prima Vacacional, no es correcto.";
                errores.Add(Mensaje);
            }

            if (PrimaVacacionalSDI == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[5] + " - El valor de la Prima Vacional SDI, no es correcto.";
                errores.Add(Mensaje);
            }

            if (FactorIntegracion == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[6] + " - El valor del Factor de Integración, no es correcto.";
                errores.Add(Mensaje);
            }
            return errores;
        }

        /// <summary>
        /// Método para recorrer la lista de factores por agregar
        /// </summary>
        /// <param name="model">ModelFactorIntegracion</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void NewFactor(List<ModelFactorIntegracion> model, int IdUsuario)
        {
            foreach (var i in model)
            {
                AddFactorIntegracion(i, IdUsuario);
            }
        }

        /// <summary>
        /// Método par aobtener los factores de integración por prestación
        /// </summary>
        /// <returns>Listado de factor de integración</returns>
        public List<FactorIntegracion> GetPrestacionesLey()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestaciones = (from b in entidad.FactorIntegracion.Where(x => x.IdPrestaciones == 1) select b).ToList();

                return prestaciones;
            }
        }

        /// <summary>
        /// Método para agregar prestaciónes
        /// </summary>
        /// <param name="IdPrestacion">Identificador de la prestacón</param>
        /// <param name="IdUsuario">Identidifacor del usuario</param>
        public void AddPrestacionesLey(int IdPrestacion, int IdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                List<FactorIntegracion> pley = GetPrestacionesLey();
                List<FactorIntegracion> nPrestacion = new List<FactorIntegracion>();

                pley.ForEach(x => {
                    nPrestacion.Add(new FactorIntegracion
                    {
                        IdPrestaciones = IdPrestacion,
                        Limite_Inferior = x.Limite_Inferior,
                        Limite_Superior = x.Limite_Superior,
                        Aguinaldo = x.Aguinaldo,
                        Vacaciones = x.Vacaciones,
                        PrimaVacacional = x.PrimaVacacional,
                        PrimaVacacionalSDI = x.PrimaVacacionalSDI,
                        FactorIntegracion1 = x.FactorIntegracion1,
                        IdEstatus = 1,
                        IdCaptura = IdUsuario,
                        FechaCaptura = DateTime.Now,
                        FechaInicioVigencia = x.FechaInicioVigencia
                    });
                });

                entidad.FactorIntegracion.AddRange(nPrestacion);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para eleiminar una preatción
        /// </summary>
        /// <param name="IdPrestacion">Identificador de la prestación</param>
        public void DeletePrestaciones(int IdPrestacion)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var prestaciones = (from b in entidad.FactorIntegracion.Where(x => x.IdPrestaciones == IdPrestacion) select b);

                entidad.FactorIntegracion.RemoveRange(prestaciones);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para agregar o eliminar un aprestacón
        /// </summary>
        /// <param name="IdPrestacion">Identificador de la prestación</param>
        /// <param name="IdUsuario">Idebtificador del usuario</param>
        public void CargarPrestacionesLey(int IdPrestacion, int IdUsuario)
        {
            DeletePrestaciones(IdPrestacion);
            AddPrestacionesLey(IdPrestacion, IdUsuario);
        }
    }
}