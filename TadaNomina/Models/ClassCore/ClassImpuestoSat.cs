using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Impuestos sat
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassImpuestoSat
    {
        /// <summary>
        /// Método para obtener los tipos de nómina
        /// </summary>
        /// <returns>Lista de los tipos de nómina</returns>
        public ModelImpuestoSat LlenaListaTipoNomina()
        {
            List<SelectListItem> _tipoNomina = new List<SelectListItem>();

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var a = (from b in entidad.Cat_TipoNomina  select b).OrderBy(x => x.IdTipoNomina).ToList();
                //_tipoNomina.Add(new SelectListItem { Text = "Elegir...", Value = "0" });
                a.ForEach(x => { _tipoNomina.Add(new SelectListItem { Text = x.Observaciones, Value = x.IdTipoNomina.ToString() }); });
            }

            ModelImpuestoSat ModelImpuestoSat = new ModelImpuestoSat();
            ModelImpuestoSat.LTipoNomina = _tipoNomina;
            return ModelImpuestoSat;
        }

        /// <summary>
        /// Método para obtener un listado con los tipos de nómina
        /// </summary>
        /// <param name="modelImpuestoSat">ModelImpuestoSat</param>
        /// <returns>Listado con los tipos de nómina</returns>
        public ModelImpuestoSat LlenaListaTipoNomina(ModelImpuestoSat modelImpuestoSat)
        {
            List<SelectListItem> _tipoNomina = new List<SelectListItem>();

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var a = (from b in entidad.Cat_TipoNomina select b).OrderBy(x => x.IdTipoNomina).ToList();
                //_tipoNomina.Add(new SelectListItem { Text = "Elegir...", Value = "0" });
                a.ForEach(x => { _tipoNomina.Add(new SelectListItem { Text = x.Observaciones, Value = x.IdTipoNomina.ToString() }); });
            }
            modelImpuestoSat.LTipoNomina = _tipoNomina;
            return modelImpuestoSat;
        }

        /// <summary>
        /// Método para obterne rla información del impuesto del sat por su identificador
        /// </summary>
        /// <param name="pIdImpuesto">Identificador del impuesto</param>
        /// <returns>Información sobre el impuesto del sat</returns>
        public ModelImpuestoSat GetModelImpuestoSat(int pIdImpuesto)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelImpuestoSat modelImpuestoSat = new ModelImpuestoSat();

                var registro = (from b in entidad.ImpuestoSat
                                where b.IdImpuesto == pIdImpuesto
                                select b).FirstOrDefault();
                var tiponomina = (from b in entidad.Cat_TipoNomina where b.IdTipoNomina == registro.IdTipoNomina select b).FirstOrDefault();

                if (registro != null)
                {
                    modelImpuestoSat.IdTipoNomina = (int)registro.IdTipoNomina;
                    modelImpuestoSat.TipoNomina = tiponomina.Observaciones;
                    modelImpuestoSat.LimiteInferior = (decimal)registro.LimiteInferior;
                    modelImpuestoSat.LimiteSuperior = (decimal)registro.LimiteSuperior;
                    modelImpuestoSat.CuotaFija = (decimal)registro.CuotaFija;
                    modelImpuestoSat.Porcentaje = (decimal)registro.Porcentaje;
                    modelImpuestoSat.FechaInicio = registro.FechaInicio.Value.ToShortDateString();                        
                    return modelImpuestoSat;
                }
                else
                {
                    return modelImpuestoSat;
                }
            }
        }

        /// <summary>
        /// Método par aobtener un listado con los impuestos del sat
        /// </summary>
        /// <returns>Listado con los impuestós del sat</returns>
        public List<ImpuestoSat> GetImpuestoSat()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var Impuesto = from b in entidad.ImpuestoSat.Where(x =>  x.EstatusId == 1) select b;

                return Impuesto.ToList();
            }
        }

        public List<ImpuestoSat> GetImpuestoSat(int IdTipoNomina)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
               
                if (IdTipoNomina != 0)
                {
                     var Impuesto = from b in entidad.ImpuestoSat.Where(x => x.IdTipoNomina == IdTipoNomina && x.EstatusId == 1) select b;
                    return Impuesto.ToList();
                }
                else {
                     var Impuesto = from b in entidad.ImpuestoSat.Where(x => x.EstatusId == 1) select b;
                    return Impuesto.ToList();
                }
                //return Impuesto.ToList();
            }
        }

        /// <summary>
        /// Método para agregar un impuesto
        /// </summary>
        /// <param name="model">ModelImpuestoSat</param>
        /// <exception cref="Exception">Error al guardar el impuesto</exception>
        public void AddImpuestoSat(ModelImpuestoSat model)
        {
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {                    
                    ImpuestoSat impuesto = new ImpuestoSat
                    {
                        IdTipoNomina = model.IdTipoNomina,
                        LimiteInferior = model.LimiteInferior,
                        LimiteSuperior = model.LimiteSuperior,
                        CuotaFija = model.CuotaFija,
                        Porcentaje = model.Porcentaje,
                        FechaInicio =  DateTime.Parse(model.FechaInicio, new CultureInfo("en-ES")), 
                        EstatusId = 1
                    };
                    entidad.ImpuestoSat.Add(impuesto);
                    entidad.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }         
        }

        /// <summary>
        /// Método para modificar la información del impuesto
        /// </summary>
        /// <param name="model">ModelImpuestoSat</param>
        /// <param name="pIdImpuesto">Identificador del impuesto</param>
        public void UpdateImpuestoSat(ModelImpuestoSat model, int pIdImpuesto)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var impuesto = (from b in entidad.ImpuestoSat
                              where b.IdImpuesto == pIdImpuesto
                              select b).FirstOrDefault();

                if (impuesto != null)
                {
                    impuesto.IdTipoNomina = model.IdTipoNomina;
                    impuesto.LimiteInferior = model.LimiteInferior;
                    impuesto.LimiteSuperior = model.LimiteSuperior;
                    impuesto.CuotaFija = model.CuotaFija;
                    impuesto.Porcentaje = model.Porcentaje;
                    impuesto.FechaInicio = DateTime.Parse(model.FechaInicio, new CultureInfo("en-ES"));

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliominar el impuesto
        /// </summary>
        /// <param name="pIdImpuesto">Identificador del impuesto</param>
        public void DeleteImpuestoSAT(int pIdImpuesto)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var impuesto = (from b in entidad.ImpuestoSat
                              where b.IdImpuesto == pIdImpuesto
                              select b).FirstOrDefault();

                if (impuesto != null)
                {
                    impuesto.EstatusId = 2; 
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para agregar impuestos por layout
        /// </summary>
        /// <param name="ruta">ruta del archivo</param>
        /// <returns>Errores al cargar los impuestos</returns>
        public ModelErroresImpuestoSat GetImpuestoSatArchivo(string ruta)
        {
            ModelErroresImpuestoSat modelErrores = new ModelErroresImpuestoSat();
            modelErrores.listErrores = new List<string>();
            modelErrores.Correctos = 0;
            modelErrores.Errores = 0;
            modelErrores.noRegistro = 0;
            modelErrores.Path = Path.GetFileName(ruta);

            ArrayList array = GetArrayImpuesto(ruta);

            List<ModelImpuestoSat> impuestos = new List<ModelImpuestoSat>();

            foreach (var item in array)
            {
                modelErrores.noRegistro++;
                AddRegistroImpuesto(modelErrores, impuestos, item);
            }

            try { NewImpuesto(impuestos); } catch (Exception ex) { modelErrores.listErrores.Add(ex.ToString()); }

            return modelErrores;
        }

        /// <summary>
        /// Método para recorrer la lista de los impuestos por agregar
        /// </summary>
        /// <param name="model">Listado del ModelImpuestoSat</param>
        public void NewImpuesto(List<ModelImpuestoSat> model)
        {
            foreach (var i in model)
            {
                AddImpuestoSat(i);
            }
        }

        /// <summary>
        /// Método para llenar una lista con los impuestos a agregar
        /// </summary>
        /// <param name="ruta">ruta del archivo</param>
        /// <returns>Listado con los impuestos para agregar</returns>
        public ArrayList GetArrayImpuesto(string ruta)
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
        /// Método para agregar el registro del impuesto a la base de datos
        /// </summary>
        /// <param name="errores">Errores encontrados en el archivo de los impuestos a cargar</param>
        /// <param name="impuestos">Listado de impuestos a cargar</param>
        /// <param name="item">texto contenido en el archivo</param>
        private void AddRegistroImpuesto(ModelErroresImpuestoSat errores, List<ModelImpuestoSat> impuestos, object item)
        {
            string[] campos = item.ToString().Split(',');

            int Tipo_Nomina = 0;
            decimal Limite_Inferior = 0;
            decimal Limite_Superior = 0;
            decimal Cuota_Fija = 0;
            decimal Porcentaje = 0;
            string Fecha_Inicio = null;

            try { Tipo_Nomina = int.Parse(campos[0]); } catch { Tipo_Nomina = 0; }
            try { Limite_Inferior = decimal.Parse(campos[1]); } catch { Limite_Inferior = 0; }
            try { Limite_Superior = decimal.Parse(campos[2]); } catch { Limite_Superior = 0; }
            try { Cuota_Fija = decimal.Parse(campos[3]); } catch { Cuota_Fija = 0; }
            try { Porcentaje = decimal.Parse(campos[4]); } catch { Porcentaje = 0; }
            try { Fecha_Inicio = campos[5]; } catch { Fecha_Inicio = null; }

            errores.listErrores.AddRange(ValidaCamposArchivo(campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelImpuestoSat i = new ModelImpuestoSat();
                i.IdTipoNomina = Tipo_Nomina;
                i.LimiteInferior = Limite_Inferior;
                i.LimiteSuperior = Limite_Superior;
                i.CuotaFija = Cuota_Fija;
                i.Porcentaje = Porcentaje;
                i.FechaInicio = Fecha_Inicio;
                impuestos.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método par avalidar los campos del archivo
        /// </summary>
        /// <param name="campos">Listado de los campos a validar</param>
        /// <param name="NoRegistro">Numero de registro</param>
        /// <returns>Listado con los errores encontrados</returns>
        public List<string> ValidaCamposArchivo(string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            int Tipo_Nomina = 0;
            decimal Limite_Inferior = 0;
            decimal Limite_Superior = 0;
            decimal Cuota_Fija = 0;
            decimal Porcentaje = 0;
            string Fecha_Inicio = null;

            try { Tipo_Nomina = int.Parse(campos[0]); } catch { Tipo_Nomina = 0; }
            try { Limite_Inferior = decimal.Parse(campos[1]); } catch { Limite_Inferior = 0; }
            try { Limite_Superior = decimal.Parse(campos[2]); } catch { Limite_Superior = 0; }
            try { Cuota_Fija = decimal.Parse(campos[3]); } catch { Cuota_Fija = 0; }
            try { Porcentaje = decimal.Parse(campos[4]); } catch { Porcentaje = 0; }
            try { Fecha_Inicio = campos[5]; } catch { Fecha_Inicio = null; }


            if (Tipo_Nomina == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[0] + " - El valor del Tipo de Nomina, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Limite_Inferior == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[1] + " - El valor del Límete Inferior, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Limite_Superior == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[2] + " - El valor del Límite Superior, no es correcto.";
                errores.Add(Mensaje);
            }       

            if (Cuota_Fija == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[3] + " - El valor de la Cuota Fija, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Porcentaje == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[4] + " - El valor del Porcentaje, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Fecha_Inicio == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[5] + " - El valor de la Fecha de Inicio, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Tipo_Nomina != 0)
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    var Id = (from b in entidad.Cat_TipoNomina where b.IdTipoNomina == Tipo_Nomina select b).FirstOrDefault();
                    if (Id == null)
                    {
                        Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[0] + " - El valor del Tipo de Nómina, no existe en sistema.";
                        errores.Add(Mensaje);
                    }
                }
            }

            if (Fecha_Inicio != null)
            {
                if (Regex.IsMatch(Fecha_Inicio, "^(?:[012]?[0-9]|3[01])[./-](?:0?[1-9]|1[0-2])[./-](?:[0-9]{2}){1,2}$") == false) {
                    Mensaje = "Referencia: Registro - " + NoRegistro.ToString() + ", Error - " + campos[5] + " - El valor no corresponde a una fecha válida.";
                    errores.Add(Mensaje);
                }
            }


            return errores;
        }

    }
}