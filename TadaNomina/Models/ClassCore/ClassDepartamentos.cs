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
    /// <summary>
    /// Departamentos
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
	public class ClassDepartamentos
	{
        /// <summary>
        /// Método para obtener un listado de los departamentos activos
        /// </summary>
        /// <returns>Listado de tipo vDepartamentos</returns>
        public List<vDepartamentos> GetDepartamentos()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var deptos = from b in entidad.vDepartamentos.Where(x => x.IdEstatus == 1) select b;

                return deptos.ToList();
            }
        }

        /// <summary>
        /// Método para obtener los departamentos por cliente
        /// </summary>
        /// <param name="pIdCliente"></param>
        /// <returns>Lista de tipo vDepartamentos</returns>
        public List<vDepartamentos> GetDepartamentosByIdCliente(int pIdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var deptos = from b in entidad.vDepartamentos.Where(x => x.IdEstatus == 1 && x.IdCliente== pIdCliente) select b;

                return deptos.ToList();
            }
        }

        /// <summary>
        /// Método para agregar un departamento
        /// </summary>
        /// <param name="model">ModelDepartamentos</param>
        /// <param name="pIdCliente">Identificador del cliente</param>
        /// <param name="pIdUsuario">Identificador del usuario</param>
        public void AddDepartamento(ModelDepartamentos model, int pIdCliente, int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                Cat_Departamentos depto = new Cat_Departamentos
                {
                    IdCliente = pIdCliente,
                    Clave = model.Clave,
                    Departamento = model.Departamento,

                    IdEstatus = 1,
                    FechaCaptura = DateTime.Now,
                    IdCaptura= pIdUsuario
                };

                entidad.Cat_Departamentos.Add(depto);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar un departamento
        /// </summary>
        /// <param name="model">ModelDepartamentos</param>
        /// <param name="pIdDepartamento">Identificador del departamento</param>
        /// <param name="pIdUsuario">Identificador del usuario</param>
        public void UpdateDepartamento(ModelDepartamentos model, int pIdDepartamento, int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                var depto = (from b in entidad.Cat_Departamentos
                             where b.IdDepartamento == pIdDepartamento
                             select b).FirstOrDefault();

                if (depto != null)
                {
                    depto.Clave = model.Clave;
                    depto.Departamento = model.Departamento;

                    depto.FechaModificacion = DateTime.Now;
                    depto.IdModificacion = pIdUsuario;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar el departamento
        /// </summary>
        /// <param name="pIdDepartamento">Identificador del departamento</param>
        /// <param name="pIdUsuario">Identificador del usuario</param>
        public void DeleteDepartamento(int pIdDepartamento, int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                var depto = (from b in entidad.Cat_Departamentos
                             where b.IdDepartamento == pIdDepartamento
                             select b).FirstOrDefault();


                if (depto != null)
                {
                    depto.IdEstatus = 2;
                    depto.IdModificacion = pIdUsuario;
                    depto.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para llenar el ModelDepartamento buscando la informacion del departamento por su identificador
        /// </summary>
        /// <param name="pIdDepartamento">Identificador del departamento</param>
        /// <returns>ModelDepartamento</returns>
        public ModelDepartamentos GetModelDepartamentos(int pIdDepartamento)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                ModelDepartamentos modelDepto = new ModelDepartamentos();

                var depto = (from b in entidad.vDepartamentos
                             where b.IdDepartamento == pIdDepartamento
                             select b).FirstOrDefault();

                if (depto != null)
                {
                    modelDepto.IdDepartamento = depto.IdDepartamento;
                    modelDepto.Cliente = depto.Cliente;
                    modelDepto.Clave = depto.Clave;
                    modelDepto.Departamento = depto.Departamento;

                    return modelDepto;
                }
                else
                {
                    return modelDepto;
                }
            }
        }

        public ModelDepartamentos ValidaDepto(int IdDepartamento)
        {
            ModelDepartamentos MA = new ModelDepartamentos();
            MA = GetModelDepartamentos(IdDepartamento);
            MA.ValidaDepto = ValidaEstructura(IdDepartamento);
            MA.ValidaEmpleado = ValidaEmpleado(IdDepartamento);

            return MA;
        }

        public int? ValidaEstructura(int IdDepartamento)
        {
            ModelDepartamentos MA = new ModelDepartamentos();
            int? res = null;
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    var ValidaArea = entidad.DepartamentoPuesto.Where(x => x.IdDepartamento == IdDepartamento && x.IdEstatus == 1).ToList();

                    if (ValidaArea != null && ValidaArea.Count > 0)
                    {
                        res = ValidaArea.Count;
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

        public int? ValidaEmpleado(int IdDepartamento)
        {
            List<Empleados> Emp = new List<Empleados>();
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    Emp = entidad.Empleados.Where(x => x.IdDepartamento == IdDepartamento && x.IdEstatus == 1).ToList();
                }

                if (Emp != null && Emp.Count > 0)
                    return Emp.Count;
                else
                    return null;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Método para cargar los departamentos por layout
        /// </summary>
        /// <param name="ruta">ruta del archivo cargado</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="IdUsuario">Identificador del Usuario</param>
        /// <returns>MoelErroresDepartamentos en caso de existir inconsistencias en la información del documento</returns>
        public ModelErroresDepartamentos GetDepartamentos(string ruta, int IdCliente, int IdUsuario)
        {
            //Debug.WriteLine("NOMBRE DE RUTA" + ruta);
            ModelErroresDepartamentos errorDepartamentos = new ModelErroresDepartamentos();
            errorDepartamentos.listErrores = new List<string>();
            errorDepartamentos.Correctos = 0;
            errorDepartamentos.Errores = 0;
            errorDepartamentos.noRegistro = 0;
            errorDepartamentos.Path = Path.GetFileName(ruta);

            ArrayList array = GetArraySucursales(ruta);

            List<ModelDepartamentos> departamento = new List<ModelDepartamentos>();


            foreach (var item in array)
            {
                errorDepartamentos.noRegistro++;
                AddRegidtroDepartamentos(errorDepartamentos, departamento, item);
            }

            try { NewDepartamentos(departamento, IdCliente, IdUsuario); } catch (Exception ex) { errorDepartamentos.listErrores.Add(ex.ToString()); }

            return errorDepartamentos;
        }

        /// <summary>
        /// Método para recorrer la lista de los departamentos a cargar por layout
        /// </summary>
        /// <param name="model">ModelDepartamentos</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void NewDepartamentos(List<ModelDepartamentos> model, int IdCliente, int IdUsuario)
        {
            foreach (var i in model)
            {
                AddDepartamento(i, IdCliente, IdUsuario);
            }
        }

        /// <summary>
        /// Método para la incersion de los departamentos en la base de datos
        /// </summary>
        /// <param name="errores">Lista de las inconsistencias encontradas en el archivo</param>
        /// <param name="departamentos">lista de los departamnetos a insertar</param>
        /// <param name="item">Sucursal</param>
        private void AddRegidtroDepartamentos(ModelErroresDepartamentos errores, List<ModelDepartamentos> departamentos, object item)
        {
            string[] campos = item.ToString().Split(',');

            string Clave = null;
            string Departamentos = null;
            Clave = campos[0];
            Departamentos = campos[1];



            errores.listErrores.AddRange(ValidaCamposArchivo(Clave, Departamentos, campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelDepartamentos i = new ModelDepartamentos();
                i.Clave = Clave;
                i.Departamento = Departamentos;

                departamentos.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método para validar los campos del departamento
        /// </summary>
        /// <param name="Clave">Clave del departamento</param>
        /// <param name="Departamentos">Nombre del departamento</param>
        /// <param name="campos">Departamentos</param>
        /// <param name="NoRegistro">Numero de registro</param>
        /// <returns>Lista de inconsistencias encontradas en los registros</returns>
        public List<string> ValidaCamposArchivo(string Clave, string Departamentos, string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            if (Clave == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - El valor de la clave, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Departamentos == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - El valor del Nombre de la Sucursal, no es correcto.";
                errores.Add(Mensaje);
            }

            return errores;
        }

        /// <summary>
        /// Métodd para obtener un listado de las sucursales
        /// </summary>
        /// <param name="ruta">Rura del archivo</param>
        /// <returns>Listado de las sucursales</returns>
        public ArrayList GetArraySucursales(string ruta)
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
        /// Método para obtener una lista de tipo SlectListItem de los departamentos por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns>Lista d etipo SelectListItem de los departamentos del cliente </returns>
        public List<SelectListItem> getSelectDepartamento(int IdCliente)
        {
            var dep = GetDepartamentosByIdCliente(IdCliente);
            var list = new List<SelectListItem>();

            dep.ForEach(x=> { list.Add(new SelectListItem { Text = x.Departamento, Value = x.IdDepartamento.ToString() }); });

            return list;
        }
    }
}