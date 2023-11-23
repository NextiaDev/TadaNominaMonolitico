using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class ClassReportesDirectivos
    {
        public List<SelectListItem> GetLstPeriodos(int IdUnidadNegocio)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                lst = ctx.vPeriodoNomina.Where(x=>x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).Select(x=>new SelectListItem { Text = x.Periodo + "-"+x.TipoNomina, Value = x.IdPeriodoNomina.ToString()}).ToList();
            }
            return lst;
        }

        public List<ModelReportesDirectivos> GetReporteByPeriodo(string[] Periodos)
        {
            List<ModelReportesDirectivos> model = new List<ModelReportesDirectivos>();
            List<Nomina> datos = new List<Nomina>();
            string per = null;
            using(NominaEntities1 ctx = new NominaEntities1())
            {
                datos = ctx.Nomina.Where(x => Periodos.Contains(x.IdPeriodoNomina.ToString())).ToList();
            }
            foreach(var periodo in Periodos)
            {
                using (NominaEntities1 ctx = new NominaEntities1())
                {
                    per = ctx.PeriodoNomina.Where(x => x.IdPeriodoNomina.ToString() == periodo).Select(x => x.Periodo).FirstOrDefault();
                }
                    model.Add(new ModelReportesDirectivos
                    {
                        Filtro = per,
                        NumEmpleados = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Select(x => x.IdEmpleado).Distinct().Count(),
                        TotalPatron = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Sum(x => x.Total_Patron),
                        ISN = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Sum(x => x.ISN),
                        TotalPercep = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Sum(x => x.ER),
                        TotalApoyo = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Sum(x => x.Apoyo),
                        ISR = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Sum(x => x.ISR),
                        ImssObrero = datos.Where(x => x.IdPeriodoNomina == int.Parse(periodo)).Sum(x => x.IMSS_Obrero)
                    });
            }
            return model;
        }

        public byte[] ExcelByPeriodo(List<ModelReportesDirectivos> Datos)
        {
            List<DataColumn> columns = new List<DataColumn>();
            ModelReportesDirectivos Totales = new ModelReportesDirectivos
            {
                NumEmpleados = 0,
                TotalPatron = 0.00M,
                ISN = 0.00M,
                TotalPercep = 0.00M,
                TotalApoyo = 0.00M,
                ISR = 0.00M,
                ImssObrero = 0.00M
            };
            List<string> encabezados = new List<string> { "Periodo", "Sun Empleados", "Total_Patron", "ISN", "Total Percep", "Apoyo","ISR","IMSS Obrero" };

            foreach (var item in encabezados)
            {
                columns.Add(new DataColumn(item));
            }

            var colDB = columns.ToArray();

            DataTable dt = new DataTable("Periodos");
            dt.Columns.AddRange(colDB);

            foreach (var customer in Datos)
            {
                dt.Rows.Add(customer.Filtro, customer.NumEmpleados,customer.TotalPatron,customer.ISN, customer.TotalPercep, customer.TotalApoyo, customer.ISR, customer.ImssObrero);
                Totales.NumEmpleados += customer.NumEmpleados;
                Totales.TotalPatron += customer.TotalPatron;
                Totales.ISN += customer.ISN;
                Totales.TotalPercep += customer.TotalPercep;
                Totales.TotalApoyo += customer.TotalApoyo;
                Totales.ISR += customer.ISR;
                Totales.ImssObrero += customer.ImssObrero;
            }
            dt.Rows.Add("Totales Generales", Totales.NumEmpleados, Totales.TotalPatron, Totales.ISN, Totales.TotalPercep, Totales.TotalApoyo, Totales.ISR, Totales.ImssObrero);

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

        public List<ModelReportesDirectivos> GetReporteByEntidad(string[] Periodos)
        {
            List<ModelReportesDirectivos> model = new List<ModelReportesDirectivos>();
            List<int?> entidades = new List<int?>();
            List<Nomina> datos = new List<Nomina>();
            string entidad = null;
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                datos = ctx.Nomina.Where(x => Periodos.Contains(x.IdPeriodoNomina.ToString())).ToList();
            }
            entidades = datos.Select(x=>x.IdEntidad).Distinct().ToList();
            foreach (var ent in entidades)
            {
                if(ent!= null)
                {
                    using (TadaNominaEntities ctx = new TadaNominaEntities())
                    {
                        entidad = ctx.Cat_EntidadFederativa.Where(x => x.Id == ent).Select(x => x.Nombre).FirstOrDefault();
                    }
                    model.Add(new ModelReportesDirectivos
                    {
                        Filtro = entidad,
                        NumEmpleados = datos.Where(x => x.IdEntidad == ent).Select(x => x.IdEmpleado).Distinct().Count(),
                        TotalPercep = datos.Where(x => x.IdEntidad == ent).Sum(x => x.ER),
                        TotalPercepEsq = datos.Where(x=>x.IdEntidad == ent).Sum(x=>x.ERS),
                        ISR = datos.Where(x => x.IdEntidad == ent).Sum(x => x.ISR),
                        ImssObrero = datos.Where(x => x.IdEntidad == ent).Sum(x => x.IMSS_Obrero),
                        TotalDeduc = datos.Where(x=>x.IdEntidad == ent).Sum(x=>x.DD),
                        TotalPatron = datos.Where(x => x.IdEntidad == ent).Sum(x => x.Total_Patron)
                    });
                }
            }
            return model;
        }

        public byte[] ExcelByEntidad(List<ModelReportesDirectivos> Datos)
        {
            List<DataColumn> columns = new List<DataColumn>();
            List<string> encabezados = new List<string> { "Entidad", "Sun Empleados", "Total Percep", "Total Percep Esq", "ISR", "IMSS Obrero", "Total Deduc.", "Total_Patron" };
            ModelReportesDirectivos Totales = new ModelReportesDirectivos
            {
                NumEmpleados = 0,
                TotalPercep = 0.00M,
                TotalPercepEsq = 0.00M,
                ISR = 0.00M,
                ImssObrero = 0.00M,
                TotalDeduc = 0.00M,
                TotalPatron = 0.00M,
            };
            foreach (var item in encabezados)
            {
                columns.Add(new DataColumn(item));
            }

            var colDB = columns.ToArray();

            DataTable dt = new DataTable("Periodos");
            dt.Columns.AddRange(colDB);

            foreach (var customer in Datos)
            {
                dt.Rows.Add(customer.Filtro, customer.NumEmpleados, customer.TotalPercep, customer.TotalPercepEsq, customer.ISR, customer.ImssObrero, customer.TotalDeduc, customer.TotalPatron);
                Totales.NumEmpleados += customer.NumEmpleados;
                Totales.TotalPercep += customer.TotalPercep;
                Totales.TotalPercepEsq += customer.TotalPercepEsq;
                Totales.ISR += customer.ISR;
                Totales.ImssObrero += customer.ImssObrero;
                Totales.TotalDeduc += customer.TotalDeduc;
                Totales.TotalPatron += customer.TotalPatron;
            }
            dt.Rows.Add("Totales Generales", Totales.NumEmpleados, Totales.TotalPercep, Totales.TotalPercepEsq, Totales.ISR, Totales.ImssObrero, Totales.TotalDeduc, Totales.TotalPatron);
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

        public List<ModelReportesDirectivos> GetReporteByCC(string[] Periodos)
        {
            List<ModelReportesDirectivos> model = new List<ModelReportesDirectivos>();
            List<int?> CCs = new List<int?>();
            List<Nomina> datos = new List<Nomina>();
            string centro = null;
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                datos = ctx.Nomina.Where(x => Periodos.Contains(x.IdPeriodoNomina.ToString())).ToList();
            }
            CCs = datos.Select(x => x.IdCentroCostos).Distinct().ToList();
            foreach (var cc in CCs)
            {
                if (cc != null)
                {
                    using (TadaNominaEntities ctx = new TadaNominaEntities())
                    {
                        centro = ctx.Cat_CentroCostos.Where(x => x.IdCentroCostos == cc).Select(x => x.CentroCostos).FirstOrDefault();
                    }
                    model.Add(new ModelReportesDirectivos
                    {
                        Filtro = centro,
                        NumEmpleados = datos.Where(x => x.IdCentroCostos == cc).Select(x=>x.IdEmpleado).Distinct().Count(),
                        TotalPercep = datos.Where(x => x.IdCentroCostos == cc).Sum(x => x.ER),
                        TotalPercepEsq = datos.Where(x => x.IdCentroCostos == cc).Sum(x => x.ERS),
                        ISR = datos.Where(x => x.IdCentroCostos == cc).Sum(x => x.ISR),
                        ImssObrero = datos.Where(x => x.IdCentroCostos == cc).Sum(x => x.IMSS_Obrero),
                        TotalPatron = datos.Where(x => x.IdCentroCostos == cc).Sum(x => x.Total_Patron),
                        ISN = datos.Where(x=>x.IdCentroCostos == cc).Sum(x=>x.ISN)
                    });
                }
            }
            return model;
        }

        public byte[] ExcelByCC(List<ModelReportesDirectivos> Datos)
        {
            List<DataColumn> columns = new List<DataColumn>();
            List<string> encabezados = new List<string> { "CC", "Sun Empleados", "Total Percep", "Total Percep Esq", "ISR", "IMSS Obrero", "Total_Patron", "ISN" };
            ModelReportesDirectivos Totales = new ModelReportesDirectivos
            {
                NumEmpleados = 0,
                TotalPercep = 0.00M,
                TotalPercepEsq = 0.00M,
                ISR = 0.00M,
                ImssObrero = 0.00M,
                TotalPatron = 0.00M,
                ISN = 0.00M,
            };
            foreach (var item in encabezados)
            {
                columns.Add(new DataColumn(item));
            }

            var colDB = columns.ToArray();

            DataTable dt = new DataTable("Periodos");
            dt.Columns.AddRange(colDB);

            foreach (var customer in Datos)
            {
                dt.Rows.Add(customer.Filtro, customer.NumEmpleados, customer.TotalPercep, customer.TotalPercepEsq, customer.ISR, customer.ImssObrero, customer.TotalPatron, customer.ISN);
                Totales.NumEmpleados += customer.NumEmpleados;
                Totales.TotalPercep += customer.TotalPercep;
                Totales.TotalPercepEsq += customer.TotalPercepEsq;
                Totales.ISR += customer.ISR;
                Totales.ImssObrero += customer.ImssObrero;
                Totales.TotalPatron += customer.TotalPatron;
                Totales.ISN += customer.ISN;
            }
            dt.Rows.Add("Totales Generales", Totales.NumEmpleados, Totales.TotalPercep, Totales.TotalPercepEsq, Totales.ISR, Totales.ImssObrero, Totales.TotalPatron, Totales.ISN);

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

        public List<ModelReportesDirectivos> GetReporteByPatrona(string[] Periodos)
        {
            List<ModelReportesDirectivos> model = new List<ModelReportesDirectivos>();
            List<int?> patronas = new List<int?>();
            List<Nomina> datos = new List<Nomina>();
            string patronal = null;
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                datos = ctx.Nomina.Where(x => Periodos.Contains(x.IdPeriodoNomina.ToString())).ToList();
            }
            patronas = datos.Select(x => x.IdRegistroPatronal).Distinct().ToList();
            foreach (var patron in patronas)
            {
                if (patron != null)
                {
                    using (TadaNominaEntities ctx = new TadaNominaEntities())
                    {
                        patronal = ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == patron).Select(x => x.NombrePatrona).FirstOrDefault();
                    }
                    model.Add(new ModelReportesDirectivos
                    {
                        Filtro = patronal,
                        NumEmpleados = datos.Where(x => x.IdRegistroPatronal == patron).Select(x=>x.IdEmpleado).Distinct().Count(),
                        TotalPercep = datos.Where(x => x.IdRegistroPatronal == patron).Sum(x => x.ER),
                        TotalPercepEsq = datos.Where(x => x.IdRegistroPatronal == patron).Sum(x => x.ERS),
                        ImssObrero = datos.Where(x => x.IdRegistroPatronal == patron).Sum(x => x.IMSS_Obrero),
                        TotalPatron = datos.Where(x => x.IdRegistroPatronal == patron).Sum(x => x.Total_Patron),
                        ISN = datos.Where(x => x.IdRegistroPatronal == patron).Sum(x => x.ISN),
                        ISR = datos.Where(x => x.IdRegistroPatronal == patron).Sum(x => x.ISR)
                    });
                }
            }
            return model;
        }

        public byte[] ExcelByPatrona(List<ModelReportesDirectivos> Datos)
        {
            List<DataColumn> columns = new List<DataColumn>();
            List<string> encabezados = new List<string> { "Patrona", "Sun Empleados", "Total Percep", "Total Percep Esq", "IMSS Obrero", "Total_Patron", "ISN", "ISR" };
            ModelReportesDirectivos Totales = new ModelReportesDirectivos
            {
                NumEmpleados = 0,
                TotalPercep = 0.00M,
                TotalPercepEsq = 0.00M,
                ImssObrero = 0.00M,
                TotalPatron = 0.00M,
                ISN = 0.00M,
                ISR = 0.00M,
            };
            foreach (var item in encabezados)
            {
                columns.Add(new DataColumn(item));
            }

            var colDB = columns.ToArray();

            DataTable dt = new DataTable("Periodos");
            dt.Columns.AddRange(colDB);

            foreach (var customer in Datos)
            {
                dt.Rows.Add(customer.Filtro, customer.NumEmpleados, customer.TotalPercep, customer.TotalPercepEsq, customer.ImssObrero, customer.TotalPatron, customer.ISN, customer.ISR);
                Totales.NumEmpleados += customer.NumEmpleados;
                Totales.TotalPercep += customer.TotalPercep;
                Totales.TotalPercepEsq += customer.TotalPercepEsq;
                Totales.ImssObrero += customer.ImssObrero;
                Totales.TotalPatron += customer.TotalPatron;
                Totales.ISN += customer.ISN;
                Totales.ISR += customer.ISR;
            }
            dt.Rows.Add("Totales Generales", Totales.NumEmpleados, Totales.TotalPercep, Totales.TotalPercepEsq, Totales.ImssObrero, Totales.TotalPatron, Totales.ISN, Totales.ISR);

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