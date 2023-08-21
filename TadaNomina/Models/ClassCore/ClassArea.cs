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
    public class ClassArea
    {
        public List<Cat_Areas> getAreasbyidCliente(int pIdCliente)
        {
            using (TadaNominaEntities bd = new TadaNominaEntities())
            {
                var Area = (from b in bd.Cat_Areas
                            where b.IdEstatus == 1 && b.IdCliente == pIdCliente
                            select b);

                return Area.ToList();
            }
        }


        public void addAreas(ModelAreas model, int idcliente, int idusuario)
        {
            using (TadaNominaEntities bd = new TadaNominaEntities())
            {
                Cat_Areas ar = new Cat_Areas()
                {
                    IdCliente = idcliente,
                    Clave = model.Clave,
                    Area = model.Area,
                    IdEstatus = 1,
                    IdCaptura = idusuario,
                    FechaCaptura = DateTime.Now
                };

                bd.Cat_Areas.Add(ar);
                bd.SaveChanges();
            }
        }


        public ModelAreas GetModelAreas(int IdArea)
        {
            string fecha = (DateTime.Now).ToString();

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                ModelAreas model = new ModelAreas();
                var cc = (from b in entidad.Cat_Areas
                          where b.IdArea == IdArea
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    model.Clave = cc.Clave;
                    model.Area = cc.Area;
                }

                return model;
            }
        }

        public ModelAreas ValidaArea(int IdArea)
        {
            ModelAreas MA = new ModelAreas();
            MA = GetModelAreas(IdArea);
            MA.ValidaAreas = ValidaEstructura(IdArea);
            MA.ValidaEmpleado = ValidaEmpleado(IdArea);

            return MA;
        }


        public int? ValidaEstructura(int IdArea)
        {
            ModelAreas MA = new ModelAreas();
            int? res = null;

            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    var ValidaArea = entidad.DepartamentoPuesto.Where(x => x.IdArea == IdArea && x.IdEstatus == 1).ToList();

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

        public int? ValidaEmpleado(int IdArea)
        {
            List<Empleados> Emp = new List<Empleados>();
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    Emp = entidad.Empleados.Where(x => x.IdArea == IdArea && x.IdEstatus == 1).ToList();
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



        public void UpdateAreas(int idarea, int Idusuario, ModelAreas model)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cc = (from b in entidad.Cat_Areas
                          where b.IdArea == idarea
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    cc.Clave = model.Clave;
                    cc.Area = model.Area;

                    cc.IdModificacion = Idusuario;
                    cc.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }

            }
        }


        public void DeleteAreas(int idArea, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cc = (from b in entidad.Cat_Areas
                          where b.IdArea == idArea
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

        public ModelErroresAreas GetAteas(string ruta, int IdCliente, int IdUsuario)
        {
            Debug.WriteLine("NOMBRE DE RUTA" + ruta);
            ModelErroresAreas errorArea = new ModelErroresAreas();
            errorArea.listErrores = new List<string>();
            errorArea.Correctos = 0;
            errorArea.Errores = 0;
            errorArea.noRegistro = 0;
            errorArea.Path = Path.GetFileName(ruta);

            string extension = Path.GetExtension(ruta);

            if (extension == ".csv")
            {
                ArrayList array = GetArrayArea(ruta);

                List<ModelAreas> CAreas = new List<ModelAreas>();


                foreach (var item in array)
                {
                    errorArea.noRegistro++;
                    AddRegistroAreas(errorArea, CAreas, item);
                }

                try { NewAreas(CAreas, IdCliente, IdUsuario); } catch (Exception ex) { errorArea.listErrores.Add(ex.ToString()); }
            }
            else
            {
                errorArea.listErrores.Add("El archivo no tiene el formato correcto, debe tener extrension .csv(delimitado por comas)");
            }

            return errorArea;
        }

        public void NewAreas(List<ModelAreas> model, int IdCliente, int IdUsuario)
        {
            foreach (var i in model)
            {
                addAreas(i, IdCliente, IdUsuario);
            }
        }


        private void AddRegistroAreas(ModelErroresAreas errores, List<ModelAreas> Careas, object item)
        {
            string[] campos = item.ToString().Split(',');

            string Clave = null;
            string Area = null;
            Clave = campos[0];
            Area = campos[1];



            errores.listErrores.AddRange(ValidaCamposArchivo(Clave, Area, campos, errores.noRegistro));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelAreas i = new ModelAreas();
                i.Clave = Clave;
                i.Area = Area;

                Careas.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }


        public List<string> ValidaCamposArchivo(string Clave, string Area, string[] campos, int NoRegistro)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;

            if (Clave == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - El valor de la clave, no es correcto.";
                errores.Add(Mensaje);
            }

            if (Area == null)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - El valor del Nombre de la Area, no es correcto.";
                errores.Add(Mensaje);
            }



            return errores;
        }




        public ArrayList GetArrayArea(string ruta)
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