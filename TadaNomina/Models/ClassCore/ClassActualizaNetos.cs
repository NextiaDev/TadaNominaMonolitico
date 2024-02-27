using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    public class ClassActualizaNetos
    {
        public void ActualizaNetosString(string valores)
        {
            valores = valores.Trim();
            if (valores != string.Empty)
            {
                valores.Replace(" ", "");
                List<string> _datos = valores.Trim().Split(',').ToList();

                foreach (var item in _datos)
                {
                    if (item != string.Empty)
                    {
                        string[] datos = item.Split(':');
                        if (datos[0] != string.Empty)
                        {
                            int IdEmpleado = int.Parse(datos[0]);
                            decimal? netoPagar = null;
                            try { netoPagar = decimal.Parse(datos[1]); } catch { }
                            ActualizaNetosByIdEmpleado(IdEmpleado, netoPagar);
                        }
                    }
                }
            }
        }

        public void ActualizaNetosByIdEmpleado(int IdEmpleado, decimal? NetoPagar)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var emp = entidad.Empleados.Where(x => x.IdEmpleado == IdEmpleado).FirstOrDefault();

                if (emp != null)
                { 
                    emp.NetoPagar = NetoPagar;
                    entidad.SaveChanges();
                }
            }
        }

        public void ActualizaNetos(string path, int IdUnidadNegocio)
        {
            ClassEmpleado cemp = new ClassEmpleado();
            ArrayList array = GetArrayNetos(path);

            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                foreach (var item in array)
                {
                    string[] campos = item.ToString().Split(',');
                    decimal neto = 0;
                    string ClaveEmp = campos[0].Trim();
                    try { neto = decimal.Parse(campos[1].Trim()); } catch { }                    

                    var emp = entidad.Empleados.Where(x => x.ClaveEmpleado == ClaveEmp && x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1).FirstOrDefault();

                    if (emp != null)
                    {       
                        emp.NetoPagar = neto;
                        entidad.SaveChanges();                    
                    }
                }
            }
        }

        public ArrayList GetArrayNetos(string ruta)
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
