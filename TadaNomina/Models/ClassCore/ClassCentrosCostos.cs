using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    public class ClassCentrosCostos
    {
        public List<Cat_CentroCostos> getCentrosCostosByIdCliente(int pIdCliente)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                var cc = (from b in entidad.Cat_CentroCostos
                          where b.IdEstatus == 1 && b.IdCliente== pIdCliente
                          select b);

                return cc.ToList();
            }
        }

        public Cat_CentroCostos GetCentroCostosById(int pIdCentroCostos)
        {
            using (TadaNominaEntities entdad= new TadaNominaEntities())
            {
                Cat_CentroCostos cc = (from b in entdad.Cat_CentroCostos
                                       where b.IdCentroCostos == pIdCentroCostos
                                       select b).FirstOrDefault();

                return cc;
            }
        }

        public void AddCentroCostos(ModelCentroCostos model, int pIdCliente, int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                Cat_CentroCostos cc = new Cat_CentroCostos()
                {
                    IdCliente = pIdCliente,
                    Clave= model.Clave,
                    CentroCostos= model.CentroCostos,

                    IdEstatus=1,
                    IdCaptura= pIdUsuario,
                    FechaCaptura= DateTime.Now
                };

                entidad.Cat_CentroCostos.Add(cc);
                entidad.SaveChanges();                
            }
        }

        public ModelCentroCostos GetModelCentroCostosById(int pIdCentroCostos)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelCentroCostos model = new ModelCentroCostos();
                var cc = (from b in entidad.Cat_CentroCostos
                          where b.IdCentroCostos == pIdCentroCostos
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    model.Clave = cc.Clave;
                    model.CentroCostos = cc.CentroCostos;
                }

                return model;
            }
        }

        public ModelCentroCostos ValidaCC(int IdCC)
        {
            ModelCentroCostos mCC = new ModelCentroCostos();
            mCC = GetModelCentroCostosById(IdCC);
            mCC.ValidaEmpleado = ValidaEmpleado(IdCC);

            return mCC;
        }

        public int? ValidaEmpleado(int IdCC)
        {
            List<Empleados> Emp = new List<Empleados>();
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    Emp = entidad.Empleados.Where(x => x.IdCentroCostos == IdCC && x.IdEstatus == 1).ToList();
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

        public void UpdateCentroCostos(int pIdCentroCostos, int pIdusuario, ModelCentroCostos model)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                var cc = (from b in entidad.Cat_CentroCostos
                          where b.IdCentroCostos == pIdCentroCostos
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    cc.Clave = model.Clave;
                    cc.CentroCostos = model.CentroCostos;

                    cc.IdModificacion = pIdusuario;
                    cc.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }

            }
        }

        public void DeleteCentroCostos(int pIdCentroCostos, int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                var cc = (from b in entidad.Cat_CentroCostos
                          where b.IdCentroCostos == pIdCentroCostos
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    cc.IdModificacion = pIdUsuario;
                    cc.FechaModificacion = DateTime.Now;
                    cc.IdEstatus = 2;

                    entidad.SaveChanges();
                }
            }
        }

        public ModelCentroCostos GetModelCentrosCostos(int pIdCentroCostos)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelCentroCostos modelcc = new ModelCentroCostos();

                var cc = (from b in entidad.Cat_CentroCostos
                             where b.IdCentroCostos == pIdCentroCostos
                             select b).FirstOrDefault();

                if (cc != null)
                {
                    modelcc.IdCentroCostos = cc.IdCentroCostos;
                    modelcc.IdCliente = (int)cc.IdCliente;
                    modelcc.Clave = cc.Clave;
                    modelcc.CentroCostos = cc.CentroCostos;

                    return modelcc;
                }
                else
                {
                    return modelcc;
                }
            }
        }

        public ModelErroresCentroCostos GetCentrosCostos(string ruta, int IdCliente, int IdUsuario)
        {
            Debug.WriteLine("NOMBRE DE RUTA" + ruta);
            ModelErroresCentroCostos errorCentroCostos = new ModelErroresCentroCostos();
            errorCentroCostos.listErrores = new List<string>();
            errorCentroCostos.Correctos = 0;
            errorCentroCostos.Errores = 0;
            errorCentroCostos.noRegistro = 0;
            errorCentroCostos.Path = Path.GetFileName(ruta);

            string extension = Path.GetExtension(ruta);

            if (extension == ".csv")
            {
                ArrayList array = GetArrayCentroCostos(ruta);

                List<ModelCentroCostos> centroCostos = new List<ModelCentroCostos>();


                foreach (var item in array)
                {
                    errorCentroCostos.noRegistro++;
                    AddRegidtroCentroCostos(errorCentroCostos, centroCostos, item);
                }

                try { NewCentrosCostos(centroCostos, IdCliente, IdUsuario); } catch (Exception ex) { errorCentroCostos.listErrores.Add(ex.ToString()); }
            }
            else
            {
                errorCentroCostos.listErrores.Add("El archivo no tiene el formato correcto, debe tener extrension .csv(delimitado por comas)");
            }

            return errorCentroCostos;
        }

        public void NewCentrosCostos(List<ModelCentroCostos> model, int IdCliente, int IdUsuario)
        {
            foreach (var i in model)
            {
                AddCentroCostos(i, IdCliente, IdUsuario);
            }
        }

        private void AddRegidtroCentroCostos(ModelErroresCentroCostos errores, List<ModelCentroCostos> centroCostos, object item)
        {
            string[] campos = item.ToString().Split(',');

            string Clave = null;
            string CentroCostos = null;
            Clave = campos[0];
            CentroCostos = campos[1];



            errores.listErrores.AddRange(ValidaCamposArchivo(Clave, CentroCostos, campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelCentroCostos i = new ModelCentroCostos();
                i.Clave = Clave;
                i.CentroCostos = CentroCostos;

                centroCostos.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        public List<string> ValidaCamposArchivo(string Clave, string CentroCostos, string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            if (Clave == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - El valor de la clave, no es correcto.";
                errores.Add(Mensaje);
            }

            if (CentroCostos == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - El valor del Nombre de la Sucursal, no es correcto.";
                errores.Add(Mensaje);
            }



            return errores;
        }

        public ArrayList GetArrayCentroCostos(string ruta)
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

        public List<SelectListItem> getSelectCentroCostos(int IdCliente)
        {
            var cc = getCentrosCostosByIdCliente(IdCliente);
            var list = new List<SelectListItem>();

            cc.ForEach(x=> { list.Add(new SelectListItem { Text = x.CentroCostos, Value = x.IdCentroCostos.ToString() }); });

            return list;
        }
    }
}