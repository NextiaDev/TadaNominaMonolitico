using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    public class ClassSucursales
    {
        /// <summary>
        /// Método que lista las sucursales activas.
        /// </summary>
        /// <returns>Regresa la lista de sucursales.</returns>
        public List<Cat_Sucursales> GetSucursales()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Sucurs = from b in entidad.Cat_Sucursales.Where(x => x.IdEstatus == 1) select b;

                return Sucurs.ToList();
            }
        }

        /// <summary>
        /// Método que lista las sucursales por un cliente específico.
        /// </summary>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de sucursales por cliente específico.</returns>
        public List<Cat_Sucursales> GetSucursalesByIdCliente(int pIdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Sucur = from b in entidad.Cat_Sucursales.Where(x => x.IdCliente == pIdCliente && x.IdEstatus == 1) select b;

                return Sucur.ToList();
            }
        }

        /// <summary>
        /// Método para gregar una sucursal nueva.
        /// </summary>
        /// <param name="model">Recine el modelo de sucursales.</param>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void AddSucursales(ModelSucursales model, int pIdCliente, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {

                string DireccionC = model.Calle + " " + model.Colonia + " " + model.Nunterior + " " + model.NuExterior + " " + model.EntidadFederativa + " " + model.Municipio;

                Cat_Sucursales sucur = new Cat_Sucursales()
                {
                    IdCliente = pIdCliente,
                    Clave = model.Clave,
                    Sucursal = model.Sucursal,
                    Direccion = DireccionC,
                    CP = model.CP,
                    Pais = model.Pais,
                    EntidadFederativa = model.EntidadFederativa,
                    Municipio = model.Municipio,
                    Colonia = model.Colonia,
                    Calle = model.Calle,
                    NoExt = model.NuExterior,
                    NoInt = model.Nunterior,

                    IdEstatus = 1,
                    FechaCaptura = DateTime.Now,
                    IdCaptura = pIdUsuario,
                };

                entidad.Cat_Sucursales.Add(sucur);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar los datos de una sucursal.
        /// </summary>
        /// <param name="model">Recibe el modelo de sucursales.</param>
        /// <param name="pIdSucursal">Recibe el identificador de la sucursal.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void UpdateSucursales(ModelSucursales model, int pIdSucursal, int pIdUsuario)
        {
            string DireccionC = model.Calle + " " + model.Colonia + " " + model.Nunterior + " " + model.NuExterior + " " + model.EntidadFederativa + " " + model.Municipio;

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Sucur = (from b in entidad.Cat_Sucursales
                             where b.IdSucursal == pIdSucursal
                             select b).FirstOrDefault();

                if (Sucur != null)
                {
                    Sucur.Clave = model.Clave;
                    Sucur.Sucursal = model.Sucursal;
                    Sucur.Direccion = DireccionC;
                    Sucur.CP = model.CP;
                    Sucur.Pais = model.Pais;
                    Sucur.EntidadFederativa = model.EntidadFederativa;
                    Sucur.Municipio = model.Municipio;
                    Sucur.Colonia = model.Colonia;
                    Sucur.Calle = model.Calle;
                    Sucur.NoExt = model.NuExterior;
                    Sucur.NoInt = model.Nunterior;
                    Sucur.FechaModificacion = DateTime.Now;

                    Sucur.IdModificacion = pIdUsuario;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar el registro de una sucursal.
        /// </summary>
        /// <param name="pIdSucursal">Recibe el identificador de la sucursal.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void DeleteSucursales(int pIdSucursal, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Sucur = (from b in entidad.Cat_Sucursales
                             where b.IdSucursal == pIdSucursal
                             select b).FirstOrDefault();


                if (Sucur != null)
                {
                    Sucur.IdEstatus = 2;
                    Sucur.IdModificacion = pIdUsuario;
                    Sucur.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método que muestra el modelo de la sucursal.
        /// </summary>
        /// <param name="pIdSucursal">Recibe el identificador de la sucursal.</param>
        /// <returns>Regresa el modelo de la sucursal.</returns>
        public ModelSucursales GetModelSucursal(int pIdSucursal)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelSucursales modelSucu = new ModelSucursales();

                var Sucur = (from b in entidad.Cat_Sucursales
                             where b.IdSucursal == pIdSucursal
                             select b).FirstOrDefault();

                if (Sucur != null)
                {
                    modelSucu.IdSucursal = Sucur.IdSucursal;
                    modelSucu.IdCliente = Sucur.IdCliente;
                    modelSucu.Clave = Sucur.Clave;
                    modelSucu.Sucursal = Sucur.Sucursal;
                    modelSucu.CP = Sucur.CP;
                    modelSucu.Pais = Sucur.Pais;
                    modelSucu.EntidadFederativa = Sucur.EntidadFederativa;
                    modelSucu.Municipio = Sucur.Municipio;
                    modelSucu.Colonia = Sucur.Colonia;
                    modelSucu.Calle = Sucur.Calle;
                    modelSucu.NuExterior = Sucur.NoExt;
                    modelSucu.Nunterior = Sucur.NoInt;
                    modelSucu.Direccion = Sucur.Direccion;
                    return modelSucu;
                }
                else
                {
                    return modelSucu;
                }
            }
        }

        /// <summary>
        /// Método que muestra el modelo de los errores de la sucursal.
        /// </summary>
        /// <param name="ruta">Recibe la ruta del archivo a subir.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el modelo de los errores de las sucursales.</returns>
        public ModelErroresSucursales GetSucursales(string ruta, int IdCliente, int IdUsuario)
        {
            Debug.WriteLine("NOMBRE DE RUTA" + ruta);
            ModelErroresSucursales errorSucursales = new ModelErroresSucursales();
            errorSucursales.listErrores = new List<string>();
            errorSucursales.Correctos = 0;
            errorSucursales.Errores = 0;
            errorSucursales.noRegistro = 0;
            errorSucursales.Path = Path.GetFileName(ruta);

            ArrayList array = GetArraySucursales(ruta);

            List<ModelSucursales> sucursal = new List<ModelSucursales>();


            foreach (var item in array)
            {
                errorSucursales.noRegistro++;
                AddRegidtroSucursales(errorSucursales, sucursal, item);
            }

            try { NewSucursales(sucursal, IdCliente, IdUsuario); } catch (Exception ex) { errorSucursales.listErrores.Add(ex.ToString()); }

            return errorSucursales;
        }

        /// <summary>
        /// Método para agregar sucursales por cliente específico.
        /// </summary>
        /// <param name="model">Recibe el modelo de las sucursales.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void NewSucursales(List<ModelSucursales> model, int IdCliente, int IdUsuario)
        {
            foreach (var i in model)
            {
                AddSucursales(i, IdCliente, IdUsuario);
            }
        }

        /// <summary>
        /// Método para agregar los registros de las sucursales con carga por layout.
        /// </summary>
        /// <param name="errores">Recibe el modelo de los errores de sucursales.</param>
        /// <param name="sucursales">Recibe el modelo de sucursales.</param>
        /// <param name="item">Recibe una variable tipo object.</param>
        private void AddRegidtroSucursales(ModelErroresSucursales errores, List<ModelSucursales> sucursales, object item)
        {
            string[] campos = item.ToString().Split(',');

            string Clave = null;
            string Sucursales = null;
            string CP = null;
            string Pais = null;
            string EntidadFederativa = null;
            string Colonia = null;
            string Calle = null;
            string NoExt = null;
            string NoInt = null;
            string Municipio = null;
            Clave = campos[0];
            Sucursales = campos[1];
            CP = campos[2];
            Pais = campos[3];
            EntidadFederativa = campos[4];
            Municipio = campos[5];
            Colonia = campos[6];
            Calle = campos[7];
            NoExt = campos[8];
            NoInt = campos[9];
            errores.listErrores.AddRange(ValidaCamposArchivo(Clave, Sucursales, campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelSucursales i = new ModelSucursales();
                i.Clave = Clave;
                i.Sucursal = Sucursales;
                i.CP = CP;
                i.Pais = Pais;
                i.Colonia = Colonia;
                i.Calle = Calle;
                i.EntidadFederativa = EntidadFederativa;
                i.Municipio = Municipio;
                i.NuExterior = NoExt;
                i.Nunterior = NoInt;
                sucursales.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método para listar la validación de los campos de la sucursal.
        /// </summary>
        /// <param name="Clave">Recibe una variable tipo string.</param>
        /// <param name="Sucursales">Recibe una variable tipo string.</param>
        /// <param name="campos">Recibe un arreglo de strings.</param>
        /// <param name="NoRegistro">Recibe el identificador del registro.</param>
        /// <returns>Regresa la lista de errores de la validación.</returns>
        public List<string> ValidaCamposArchivo(string Clave, string Sucursales, string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            if (Clave == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - El valor de la clave, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Sucursales == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - El valor del Nombre de la Sucursal, no es correcto.";
                errores.Add(Mensaje);
            }

            return errores;
        }

        /// <summary>
        /// Método para subir el archivo de sucursales con carga por layout.
        /// </summary>
        /// <param name="ruta">Recibe la ruta del archivo.</param>
        /// <returns>Regresa el resultado de la carga del archivo.</returns>
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

    }
}