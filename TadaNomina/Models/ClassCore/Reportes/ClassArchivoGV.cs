using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore
{
    public class FileImportGV
    {
        public List<ModelReporteGV> ProcesoArchivo(string Archivo)
        {
            List<ModelReporteGV> lst = new List<ModelReporteGV>();
            string imss = null;
            string fecha = null;
            string tipo = null;
            foreach (string row in Archivo.Split('\n'))
            {
                if (row != "" && row.Split(',')[12].Trim().ToUpper() != "" && row.Split(',')[0].Trim().ToUpper() != "APELLIDOS" && row.Split(',')[0].Trim().ToUpper() != "")
                {
                    if((imss != row.Split(',')[2].Trim() && fecha != DateTime.Parse(row.Split(',')[4]).ToShortDateString()) || (fecha != DateTime.Parse(row.Split(',')[4]).ToShortDateString() && tipo != row.Split(',')[5]))
                    {
                        ModelReporteGV aux = lst.Where(x => x.Ubicacion == row.Split(',')[12].Trim().ToUpper()).FirstOrDefault();
                        if (aux == null)
                        {
                            lst.Add(new ModelReporteGV
                            {
                                Ubicacion = row.Split(',')[12].Trim().ToUpper(),
                                Costo = 0.00M
                            });
                        }
                        imss = row.Split(',')[2].Trim();
                        Empleados empleado;
                        using (TadaNominaEntities ctx = new TadaNominaEntities())
                        {
                            empleado = ctx.Empleados.Where(x => x.Imss == imss && x.IdEstatus == 1).FirstOrDefault();
                        }
                        if (empleado != null)
                        {
                            aux = lst.Where(x => x.Ubicacion == row.Split(',')[12].Trim().ToUpper()).FirstOrDefault();
                            aux.Costo = empleado.SDI != null ? aux.Costo + (decimal)empleado.SDI : aux.Costo + 0.00M;
                        }
                        fecha = DateTime.Parse(row.Split(',')[4]).ToShortDateString();
                        empleado = null;
                        tipo = row.Split(',')[5];
                    }
                    else
                    {
                        tipo = row.Split(',')[5];
                        imss = row.Split(',')[2].Trim();
                        fecha = DateTime.Parse(row.Split(',')[4]).ToShortDateString();
                    }
                }
            }
            return lst;
        }

        public byte[] Excel(List<ModelReporteGV> Ubicaciones)
        {
            List<DataColumn> columns = new List<DataColumn>();
            List<string> encabezados = new List<string> { "Ubicación", "Costo" };
            decimal Total = 0.00M;

            foreach (var item in encabezados)
            {
                columns.Add(new DataColumn(item));
            }

            var colDB = columns.ToArray();

            DataTable dt = new DataTable("UBICACIONES");
            dt.Columns.AddRange(colDB);

            foreach (var customer in Ubicaciones)
            {
                dt.Rows.Add(customer.Ubicacion, customer.Costo);
                Total += customer.Costo;
            }
            dt.Rows.Add("","");
            dt.Rows.Add("TOTAL",Total);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}