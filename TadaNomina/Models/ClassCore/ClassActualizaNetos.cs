using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassActualizaNetos
    {
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

                    var emp = (from b in entidad.Empleados.Where(x => x.ClaveEmpleado == ClaveEmp && x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).FirstOrDefault();

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