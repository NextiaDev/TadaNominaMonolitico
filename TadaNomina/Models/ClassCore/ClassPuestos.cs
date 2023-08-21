using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    public class ClassPuestos
    {        
        /// <summary>
        /// Método para listar los puestos.
        /// </summary>
        /// <returns>Regresa la lista de los puestos.</returns>
        public List<Cat_Puestos> GetPuestos()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var puesto = from b in entidad.Cat_Puestos.Where(x => x.IdEstatus == 1) select b;

                return puesto.ToList();
            }
        }

        /// <summary>
        /// Método para listar los puestos por cliente específico.
        /// </summary>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de los puestos por cliente específico.</returns>
        public List<Cat_Puestos> GetPuestosByIdCliente(int pIdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var puesto   = from b in entidad.Cat_Puestos.Where(x => x.IdCliente == pIdCliente && x.IdEstatus == 1) select b;

                return puesto.ToList();
            }
        }

        /// <summary>
        /// Método para agregar un puesto nuevo.
        /// </summary>
        /// <param name="model">Recibe el modelo del puesto.</param>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void AddPuestos(ModelPuestos model, int pIdCliente, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                Cat_Puestos puesto = new Cat_Puestos
                {
                    IdCliente = pIdCliente,
                    Clave = model.Clave,
                    Puesto = model.Puesto,

                    IdEstatus = 1,
                    FechaCaptura = DateTime.Now,
                    IdCaptura = pIdUsuario
                };

                entidad.Cat_Puestos.Add(puesto);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar un puesto.
        /// </summary>
        /// <param name="model">Recibe el modelo del puesto.</param>
        /// <param name="pIdPuesto">Recibe el identificador del puesto</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void UpdatePuesto(ModelPuestos model, int pIdPuesto, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var puesto = (from b in entidad.Cat_Puestos
                             where b. IdPuesto == pIdPuesto
                             select b).FirstOrDefault();

                if (puesto != null)
                {
                    puesto.Clave = model.Clave;
                    puesto.Puesto = model.Puesto;

                    puesto.FechaModificacion = DateTime.Now;
                    puesto.IdModificacion = pIdUsuario;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar el registro de un puesto.
        /// </summary>
        /// <param name="pIdPuesto">Recibe el identificador del puesto.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void DeletePuesto(int pIdPuesto, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var puesto = (from b in entidad.Cat_Puestos
                             where b.IdPuesto == pIdPuesto
                             select b).FirstOrDefault();


                if (puesto != null)
                {
                    puesto.IdEstatus = 2;
                    puesto.IdModificacion = pIdUsuario;
                    puesto.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para obtener un puesto específico.
        /// </summary>
        /// <param name="pIdPuesto">Recibe el identificador del puesto.</param>
        /// <returns>Regresa el puesto que se obtiene de la consulta.</returns>
        public ModelPuestos GetModelPuestos(int pIdPuesto)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelPuestos modelPuesto = new ModelPuestos();

                var puesto = (from b in entidad.Cat_Puestos
                             where b.IdPuesto == pIdPuesto
                             select b).FirstOrDefault();

                if (puesto != null)
                {
                    modelPuesto.IdPuesto = puesto.IdPuesto;
                    modelPuesto.IdCliente = (int)puesto.IdCliente;
                    modelPuesto.Clave = puesto.Clave;
                    modelPuesto.Puesto = puesto.Puesto;

                    return modelPuesto;
                }
                else
                {
                    return modelPuesto;
                }
            }
        }

        public ModelPuestos ValidaPuestos(int IdPuesto)
        {
            ModelPuestos MA = new ModelPuestos();
            MA = GetModelPuestos(IdPuesto);
            MA.validaPuesto = ValidaEstructura(IdPuesto);
            MA.ValidaEmpleado = ValidaEmpleado(IdPuesto);

            return MA;
        }

        public int? ValidaEstructura(int IdPuesto)
        {
            ModelPuestos MA = new ModelPuestos();
            int? res = null;
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    var ValidaPuesto = entidad.DepartamentoPuesto.Where(x => x.IdPuesto == IdPuesto && x.IdEstatus == 1).ToList();

                    if (ValidaPuesto != null && ValidaPuesto.Count > 0)
                    {
                       res = ValidaPuesto.Count;
                    }
                    else
                    {
                       res = null;
                    }
                }
                return res;
            }
            catch
            {
                res = 0;
                return res;
            }
        }

        public int? ValidaEmpleado(int IdPuesto)
        {
            List<Empleados> Emp = new List<Empleados>();
            try
            {
                using(TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    Emp = entidad.Empleados.Where(x => x.IdPuesto == IdPuesto && x.IdEstatus == 1).ToList();
                }

                if (Emp != null && Emp.Count > 0)
                    return Emp.Count;
                else
                    return null;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Método para llenar el modelo del puestos.
        /// </summary>
        /// <param name="IdUnidad">Recibe la variable int.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="modelPuesto">Recibe el modelo del puesto.</param>
        /// <returns>Regresa el modelo del puesto.</returns>
        public ModelPuestos LlenaListasPuestos(int IdUnidad, int IdCliente, ModelPuestos modelPuesto)
        {

            return modelPuesto;
        }

        /// <summary>
        /// Método para subir archivo con los puestos con carga por layout.
        /// </summary>
        /// <param name="ruta">Recibe la variable que contiene la ruta del archivo.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el informe de la carga de los registros.</returns>
        public ModelErroresPuestos GetPuestos(string ruta, int IdCliente, int IdUsuario)
        {
            ModelErroresPuestos erroresPuestos = new ModelErroresPuestos();
            erroresPuestos.listErrores = new List<string>();
            erroresPuestos.Correctos = 0;
            erroresPuestos.Errores = 0;
            erroresPuestos.noRegistro = 0;
            erroresPuestos.Path = Path.GetFileName(ruta);

            ArrayList array = GetArrayPuestos(ruta);

            List<ModelPuestos> puestos = new List<ModelPuestos>();


            foreach (var item in array)
            {
                erroresPuestos.noRegistro++;
                AddRegidtroPuestos( erroresPuestos, puestos,  item);
            }

            try { NewPuestos( puestos, IdCliente, IdUsuario); } 
            catch (Exception ex) { erroresPuestos.listErrores.Add(ex.ToString()); }

            return erroresPuestos;
        }

        /// <summary>
        /// Método para generar un puesto.
        /// </summary>
        /// <param name="model">Recibel el modelo de los puestos.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void NewPuestos(List<ModelPuestos> model, int IdCliente, int IdUsuario)  
        {
            foreach (var i in model)
            {
                AddPuestos(i, IdCliente, IdUsuario);
            }
        }

        /// <summary>
        /// Método para subir archivo con puestos con carga por layout.
        /// </summary>
        /// <param name="errores">Recibe el modelo de los errores de los puestos.</param>
        /// <param name="puestos">Recibe el modelo de puestos.</param>
        /// <param name="item">Recibe la variable object.</param>
        private void AddRegidtroPuestos(ModelErroresPuestos errores, List<ModelPuestos> puestos, object item)
        {
            string[] campos = item.ToString().Split(',');

            string Clave = null;
            string Puesto = null;
            Clave = campos[0];
            Puesto = campos[1];


            errores.listErrores.AddRange(ValidaCamposArchivo(Clave, Puesto, campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelPuestos i = new ModelPuestos();
                i.Clave = Clave;
                i.Puesto = Puesto;

                puestos.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método para listar la validación de los campos del archivo.
        /// </summary>
        /// <param name="Clave">Recibe la variable string.</param>
        /// <param name="Puesto">Recible la variable string.</param>
        /// <param name="campos">Recibe el arreglo de strings.</param>
        /// <param name="NoRegistro">Recibe la variable int.</param>
        /// <returns>Regresa mensaje con la validación del archivo.</returns>
        public List<string> ValidaCamposArchivo(string Clave, string Puesto, string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            if (Clave == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - El valor de la clave, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Puesto == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - El valor del Nombre de la Sucursal, no es correcto.";
                errores.Add(Mensaje);
            }



            return errores;
        }

        /// <summary>
        /// Método para leer la ruta de un archivo para cargar puestos de forma masiva.
        /// </summary>
        /// <param name="ruta">Recibe la ruta del archivo.</param>
        /// <returns>Regresa el arreglo de los puestos.</returns>
        public ArrayList GetArrayPuestos(string ruta)
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
        /// Método que lista los puestos de un cliente específico.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de los puestos de un cliente específico.</returns>
        public List<SelectListItem> getSelectPuestos(int IdCliente)
        {
            var puestos = GetPuestosByIdCliente(IdCliente);
            var list = new List<SelectListItem>();

            puestos.ForEach(x=> { list.Add(new SelectListItem { Text = x.Puesto, Value = x.IdPuesto.ToString() }); });

            return list;
        }

        /// <summary>
        /// Método que lista los departamentos de un cliente específico.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de departamentos por cliente específico.</returns>
        public List<SelectListItem> getListDepartamentos(int IdCliente)
        {
            ClassDepartamentos cdtos = new ClassDepartamentos();
            var deptos = cdtos.GetDepartamentosByIdCliente(IdCliente);
            List<SelectListItem> ldtos = new List<SelectListItem>();
            deptos.ForEach(x=> { ldtos.Add(new SelectListItem { Value = x.IdDepartamento.ToString(), Text = x.Departamento }); });

            return ldtos;
        }
    }
}