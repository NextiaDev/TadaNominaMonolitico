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
    public class ClassSubsidioEmpleoSat
    {
        /// <summary>
        /// Método que muestra el modelo del subsidio de los tipos de nómina.
        /// </summary>
        /// <returns>Regresa el modelo del subsidio.</returns>
        public ModelSubsidioEmpleoSat LlenaListaTipoNomina()
        {
            List<SelectListItem> _tipoNomina = new List<SelectListItem>();

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var a = (from b in entidad.Cat_TipoNomina select b).OrderBy(x => x.IdTipoNomina).ToList();
                //_tipoNomina.Add(new SelectListItem { Text = "Elegir...", Value = "0" });
                a.ForEach(x => { _tipoNomina.Add(new SelectListItem { Text = x.Observaciones, Value = x.IdTipoNomina.ToString() }); });
            }

            ModelSubsidioEmpleoSat ModelImpuestoSat = new ModelSubsidioEmpleoSat();
            ModelImpuestoSat.LTipoNomina = _tipoNomina;
            return ModelImpuestoSat;
        }

        /// <summary>
        /// Método que lista los subsidios activos.
        /// </summary>
        /// <returns>Regresa la lista con los subsidios activos.</returns>
        public List<SubsidioEmpleoSat> GetSubsidio()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Subsidio = from b in entidad.SubsidioEmpleoSat.Where(x => x.IdTipoNomina == 1) select b;

                return Subsidio.ToList();
            }
        }

        /// <summary>
        /// Método que lista los subsidios por cliente.
        /// </summary>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <returns></returns>
        public List<SubsidioEmpleoSat> GetSubsidioEmpleoSat(int pIdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Subsidio = from b in entidad.SubsidioEmpleoSat.Where(x => x.IdTipoNomina == pIdCliente) select b;

                return Subsidio.ToList();
            }
        }

        /// <summary>
        /// Método que agrega un subsidio.
        /// </summary>
        /// <param name="model">Recibe el modelo del subsidio.</param>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void AddSubsidioEmpleoSat(ModelSubsidioEmpleoSat model, int pIdCliente, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                SubsidioEmpleoSat subsi = new SubsidioEmpleoSat()
                {
                    IdTipoNomina = pIdCliente,
                    LimiteInferior = model.LimiteInferior,
                    LimiteSuperior = model.LimiteSuperior,

                    IdSubsidio = 1,
                    FechaInicio = DateTime.Now,
                    //IdCaptura = pIdUsuario,
                };

                entidad.SubsidioEmpleoSat.Add(subsi);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener los datos del subsidio.
        /// </summary>
        /// <param name="pIdSubsidioEmpleoCat">Recibe el identificador del subsidio</param>
        /// <returns>Regresa el modelo del subsidio.</returns>
        public ModelSubsidioEmpleoSat GetModelSubsidioEmpleoSat(int pIdSubsidioEmpleoCat)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelSubsidioEmpleoSat modelSubsidio = new ModelSubsidioEmpleoSat();

                var Subsi = (from b in entidad.SubsidioEmpleoSat
                             where b.IdTipoNomina == pIdSubsidioEmpleoCat
                             select b).FirstOrDefault();

                if (Subsi != null)
                {
                    modelSubsidio.IdTipoNomina = Subsi.IdTipoNomina;
                    modelSubsidio.LimiteInferior = Subsi.LimiteInferior;
                    modelSubsidio.LimiteSuperior = Subsi.LimiteSuperior;
                    modelSubsidio.CreditoSalario = Subsi.CreditoSalario;
                    modelSubsidio.FechaInicio = Subsi.FechaInicio;

                    return modelSubsidio;
                }
                else
                {
                    return modelSubsidio;
                }
            }
        }

        /// <summary>
        /// Método para modificar los datos del subsidio.
        /// </summary>
        /// <param name="model">Recibe el modelo del subsidio.</param>
        /// <param name="pIdTipoNomina">Recibe el identificador del tipo de nómina.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void UpdateSubsidioEmpleoSat(ModelSubsidioEmpleoSat model, int pIdTipoNomina, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Subsi = (from b in entidad.SubsidioEmpleoSat
                             where b.IdTipoNomina == pIdTipoNomina
                             select b).FirstOrDefault();

                if (Subsi != null)
                {
                    Subsi.IdTipoNomina = model.IdTipoNomina;
                    Subsi.LimiteInferior = model.LimiteInferior;

                    //Subsi.FechaModificacion = DateTime.Now;
                    Subsi.CreditoSalario = pIdUsuario;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar un subsidio.
        /// </summary>
        /// <param name="pIdTipoNomina">Recibe el identificador del tipo de nómina.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void DeleteSubsidioEmpleoSat(int pIdTipoNomina, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Subsi = (from b in entidad.SubsidioEmpleoSat
                             where b.IdTipoNomina == pIdTipoNomina
                             select b).FirstOrDefault();


                if (Subsi != null)
                {
                    //Subsi.IdEstatus = 2;
                    //Subsi.IdModificacion = pIdUsuario;
                    //Subsi.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para mostrar el modelo del subsidio con el identificador del tipo de nómina.
        /// </summary>
        /// <param name="pIdTipoNomina">Recibe el identificador del tipo de nómina.</param>
        /// <returns>Regresa el modelo del subsidio.</returns>
        public ModelSubsidioEmpleoSat GetModelSubsidio(int pIdTipoNomina)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelSubsidioEmpleoSat modelSubsi = new ModelSubsidioEmpleoSat();

                var Subsi = (from b in entidad.SubsidioEmpleoSat
                             where b.IdTipoNomina == pIdTipoNomina
                             select b).FirstOrDefault();

                if (Subsi != null)
                {
                    modelSubsi.IdTipoNomina = Subsi.IdTipoNomina;
                    modelSubsi.LimiteInferior = Subsi.LimiteInferior;
                    modelSubsi.LimiteSuperior = Subsi.LimiteSuperior;
                    modelSubsi.CreditoSalario = Subsi.CreditoSalario;

                    return modelSubsi;
                }
                else
                {
                    return modelSubsi;
                }
            }
        }

        /// <summary>
        /// Método que muestra el modelo de eroores en el subsidio.
        /// </summary>
        /// <param name="ruta">Recibe la variable tipo string.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns></returns>
        public ModelErroresSubsidioEmpleoSat GetSubsidio(string ruta, int IdCliente, int IdUsuario)
        {
            //Debug.WriteLine("NOMBRE DE RUTA" + ruta);
            ModelErroresSubsidioEmpleoSat errorSubsi = new ModelErroresSubsidioEmpleoSat();
            errorSubsi.listErrores = new List<string>();
            errorSubsi.Correctos = 0;
            errorSubsi.Errores = 0;
            errorSubsi.noRegistro = 0;
            errorSubsi.Path = Path.GetFileName(ruta);

            ArrayList array = GetArraySubsidioEmpleoSat(ruta);

            List<ModelSubsidioEmpleoSat> Subsi = new List<ModelSubsidioEmpleoSat>();


            foreach (var item in array)
            {
                errorSubsi.noRegistro++;
                AddRegidtroSubsidioEmpleoSat(errorSubsi, Subsi, item);
            }

            try { NewSubsidioEmpleoSat( Subsi, IdCliente, IdUsuario); } catch (Exception ex) { errorSubsi.listErrores.Add(ex.ToString()); }

            return errorSubsi;
        }

        /// <summary>
        /// Método que lee la ruta del subsidio.
        /// </summary>
        /// <param name="ruta">Recibe la variable tipo string.</param>
        /// <returns>Regresa la ruta del subsidio.</returns>
        private ArrayList GetArraySubsidioEmpleoSat(string ruta)
        {
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
           // throw new NotImplementedException();
        }

        /// <summary>
        /// Método para agregar un subsidio nuevo.
        /// </summary>
        /// <param name="model">Recibe el modelo del subsidio.</param>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void NewSubsidioEmpleoSat(List<ModelSubsidioEmpleoSat> model, int IdCliente, int IdUsuario)
        {
            foreach (var i in model)
            {
                AddSubsidioEmpleoSat(i, IdCliente, IdUsuario);
            }
        }

        /// <summary>
        /// Método para agregar registros a los subsidios.
        /// </summary>
        /// <param name="errores">Recibe el modelo de errores de subsidio.</param>
        /// <param name="subsidioEmpleoSats">Recibe el modelo del subsidio</param>
        /// <param name="item">Recibe una variable tipo object.</param>
        private void AddRegidtroSubsidioEmpleoSat(ModelErroresSubsidioEmpleoSat errores, List<ModelSubsidioEmpleoSat> subsidioEmpleoSats, object item)
        {
            string[] campos = item.ToString().Split(',');

            decimal LimiteInferior = 0 ;
            decimal LimiteSuperior = 0 ;
            //LimiteInferior = campos[0];
            //LimiteSuperior = campos[1];

            errores.listErrores.AddRange(ValidaCamposArchivo( LimiteInferior,  LimiteSuperior, campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelSubsidioEmpleoSat i = new ModelSubsidioEmpleoSat();
                i.LimiteInferior = LimiteInferior;
                i.LimiteSuperior = LimiteSuperior;

                subsidioEmpleoSats.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método para listar la validación de errores del registro del subsidio.
        /// </summary>
        /// <param name="LimiteInferior">Recibe la variable tipo decimal.</param>
        /// <param name="LimiteSuperior">Recibe la variable tipo decimal.</param>
        /// <param name="campos">Recibe un arreglo de variables tipo string.</param>
        /// <param name="NoRegistro">Recibe una variable tipo int.</param>
        /// <returns>Regresa una lista con la validación de los campos del archivo.</returns>
        public List<string> ValidaCamposArchivo(decimal LimiteInferior, decimal LimiteSuperior, string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            if (LimiteInferior == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - El valor de la clave, no es correcto.";
                errores.Add(Mensaje);
            }

            if (LimiteSuperior == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - El valor del Nombre de la Sucursal, no es correcto.";
                errores.Add(Mensaje);
            }



            return errores;
        }

    }
    
}