using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.IMSS
{
    public class ClassIMSS
    {
        public string RegresaMovimientosIDSE(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal, string TipoMovimiento)
        {
            string res = string.Empty;

            switch (TipoMovimiento)
            {
                case "A":
                    res = obtenAltasPorCliente(IdCliente, pFechaInicial, pFechaFinal);
                    break;

                case "B":
                    res = obtenBajasPorCliente(IdCliente, pFechaInicial, pFechaFinal);
                    break;

                case "C":
                    res = obtenMSCliente(IdCliente, pFechaInicial, pFechaFinal);
                    break;
            }

            return res;
        }

        public string RegresaMovimientosSUA(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal, string TipoMovimiento)
        {
            string res = string.Empty;

            using (TadaEmpleadosEntities context= new TadaEmpleadosEntities())
            {
                res = context.sp_ArchivosSUA(IdCliente, pFechaInicial, pFechaFinal, TipoMovimiento).FirstOrDefault();
            }

            return res;
        }

        public string RegresaNombreArchivo(DateTime pFechaInicial, DateTime pFechaFinal, string TipoMovimiento)
        {
            string res = string.Empty;

            string fi = pFechaInicial.ToShortDateString().Replace("/", string.Empty);
            string ff = pFechaFinal.ToShortDateString().Replace("/", string.Empty);

            switch (TipoMovimiento)
            {
                case "A":
                    res = "ALTAS_" + fi + '_' + ff + ".txt";
                    break;

                case "B":
                    res = "BAJAS_" + fi + '_' + ff + ".txt";
                    break;

                case "C":
                    res = "MS_" + fi + '_' + ff + ".txt";
                    break;
            }

            return res;
        }

        public string RegresaNombreArchivoSUA(DateTime pFechaInicial, DateTime pFechaFinal, string TipoMovimiento)
        {
            string res = "General.txt";

            string fi = pFechaInicial.ToShortDateString().Replace("/", string.Empty);
            string ff = pFechaFinal.ToShortDateString().Replace("/", string.Empty);

            switch (TipoMovimiento)
            {
                case "A":
                    res = "ALTAS_" + fi + '_' + ff + ".txt";
                    break;

                case "B":
                    res = "BAJAS_" + fi + '_' + ff + ".txt";
                    break;

                case "C":
                    res = "MS_" + fi + '_' + ff + ".txt";
                    break;
            }

            return res;
        }

        public string RegresaNombreArchivoVariabilidad(DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string res = "Variabilidad_";

            string fi = pFechaInicial.ToShortDateString().Replace("/", string.Empty);
            string ff = pFechaFinal.ToShortDateString().Replace("/", string.Empty);

            return res += fi + '_' + ff + ".xlsx";
        }

        public string RegresaNombreArchivoRecuperaciones(DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string res = "RecuperacionesIMSS_";

            string fi = pFechaInicial.ToShortDateString().Replace("/", string.Empty);
            string ff = pFechaFinal.ToShortDateString().Replace("/", string.Empty);

            return res += fi + '_' + ff + ".xlsx";
        }
        private string obtenAltasPorCliente(int pIdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string cadenaFinal = string.Empty;
            string cadenaEmpleado = string.Empty;
            pFechaFinal = pFechaFinal.AddDays(1);

            using (TadaEmpleadosEntities context= new TadaEmpleadosEntities())
            {
                try
                {
                    var Altas = from b in context.vEmpleados                                                                
                                where b.IdEstatus == 1 && b.FechaCaptura >= pFechaInicial && b.FechaCaptura <= pFechaFinal && b.IdCliente == pIdCliente && b.IdRegistroPatronal != null
                                select b;

                    if (Altas != null)
                    {
                        foreach (var item in Altas)
                        {
                            cadenaEmpleado = string.Empty;
                            var empleado = (from b in context.vEmpleados
                                            where b.IdEmpleado == item.IdEmpleado
                                            select b).FirstOrDefault();

                            cadenaEmpleado += empleado.RegistroPatronal.Substring(0, 10);
                            cadenaEmpleado += empleado.RegistroPatronal.Substring(10, 1);
                            cadenaEmpleado += empleado.Imss.Substring(0, 10);
                            cadenaEmpleado += empleado.Imss.Substring(10, 1);
                            cadenaEmpleado += RellenaCadena(empleado.ApellidoPaterno, 27);
                            cadenaEmpleado += RellenaCadena(empleado.ApellidoMaterno, 27);
                            cadenaEmpleado += RellenaCadena(empleado.Nombre, 27);
                            cadenaEmpleado += RellenaCadena(empleado.SDI.ToString().Substring(0,6), 6);
                            cadenaEmpleado += RellenaCadena("", 6);
                            cadenaEmpleado += "1";
                            cadenaEmpleado += "0";
                            cadenaEmpleado += "0";
                            //Para le fecha de alta IMSS
                            cadenaEmpleado += empleado.FechaAltaIMSS.ToString().Substring(0, 2) + empleado.FechaAltaIMSS.ToString().Substring(3, 2) + empleado.FechaAltaIMSS.ToString().Substring(6, 4);
                            cadenaEmpleado += "   ";
                            cadenaEmpleado += RellenaCadena("", 2);
                            cadenaEmpleado += "08";
                            cadenaEmpleado += "     ";
                            cadenaEmpleado += RellenaCadena(empleado.ClaveEmpleado, 10);
                            cadenaEmpleado += " ";
                            cadenaEmpleado += empleado.Curp;
                            cadenaEmpleado += "9";

                            cadenaFinal += cadenaEmpleado + Environment.NewLine;
                        }

                    }

                    return cadenaFinal;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }           

        }

        private string obtenBajasPorCliente(int pIdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string cadenaFinal = string.Empty;
            string cadenaEmpleado = string.Empty;
            pFechaFinal = pFechaFinal.AddDays(1);

            using (TadaEmpleadosEntities context= new TadaEmpleadosEntities())
            {
                try
                {
                    int[] estatus = { 2, 3 };
                    var Bajas = from b in context.vEmpleados                                
                                where estatus.Contains(b.IdEstatus) && b.FechaModificacion >= pFechaInicial && b.FechaModificacion <= pFechaFinal && b.IdCliente == pIdCliente && b.FechaBaja != null
                                select b;

                    if (Bajas != null)
                    {
                        foreach (var item in Bajas)
                        {
                            cadenaEmpleado = string.Empty;
                            var empleado = (from b in context.vEmpleados
                                            where b.IdEmpleado == item.IdEmpleado
                                            select b).FirstOrDefault();

                            cadenaEmpleado += empleado.RegistroPatronal.Substring(0, 10);
                            cadenaEmpleado += empleado.RegistroPatronal.Substring(10, 1);
                            cadenaEmpleado += empleado.Imss.Substring(0, 10);
                            cadenaEmpleado += empleado.Imss.Substring(10, 1);
                            cadenaEmpleado += RellenaCadena(empleado.ApellidoPaterno, 27);
                            cadenaEmpleado += RellenaCadena(empleado.ApellidoMaterno, 27);
                            cadenaEmpleado += RellenaCadena(empleado.Nombre, 27);
                            cadenaEmpleado += RellenaCadena("", 15);
                            //Para le fecha de baja IMSS
                            cadenaEmpleado += empleado.FechaBaja.ToString().Substring(0, 2) + empleado.FechaBaja.ToString().Substring(3, 2) + empleado.FechaBaja.ToString().Substring(6, 4);
                            cadenaEmpleado += RellenaCadena("", 5);
                            cadenaEmpleado += "02";
                            cadenaEmpleado += "     ";
                            cadenaEmpleado += RellenaCadena(empleado.ClaveEmpleado, 10);
                            cadenaEmpleado += "2";
                            cadenaEmpleado += RellenaCadena("", 18);
                            cadenaEmpleado += "9";

                            cadenaFinal += cadenaEmpleado + Environment.NewLine;
                        }

                    }

                    return cadenaFinal;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


           
        }

        private string obtenMSCliente(int pIdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string cadenaFinal = string.Empty;
            string cadenaEmpleado = string.Empty;
            pFechaFinal = pFechaFinal.AddDays(1);

            using (TadaEmpleadosEntities context = new TadaEmpleadosEntities())
            {
                try
                {
                    var Altas = (from b in context.vEmpleados    
                                join c in context.ModificacionSueldos 
                                on b.IdEmpleado equals c.IdEmpleado
                                where b.IdEstatus == 1 && c.FechaCaptura >= pFechaInicial && c.FechaCaptura <= pFechaFinal && b.IdCliente == pIdCliente && b.IdRegistroPatronal != null
                                select new { b.IdEmpleado, b.RegistroPatronal, b.Imss, b.Nombre, b.ApellidoPaterno, b.ApellidoMaterno, c.SDI_Nuevo, c.FechaMovimiento, b.ClaveEmpleado, b.Curp }).ToList();

                    if (Altas != null)
                    {
                        foreach (var empleado in Altas)
                        {
                            cadenaEmpleado = string.Empty;

                            cadenaEmpleado += empleado.RegistroPatronal.Substring(0, 10);
                            cadenaEmpleado += empleado.RegistroPatronal.Substring(10, 1);
                            cadenaEmpleado += empleado.Imss.Substring(0, 10);
                            cadenaEmpleado += empleado.Imss.Substring(10, 1);
                            cadenaEmpleado += RellenaCadena(empleado.ApellidoPaterno, 27);
                            cadenaEmpleado += RellenaCadena(empleado.ApellidoMaterno, 27);
                            cadenaEmpleado += RellenaCadena(empleado.Nombre, 27);
                            cadenaEmpleado += RellenaCadena(empleado.SDI_Nuevo.ToString().Substring(0, 6), 6);
                            cadenaEmpleado += RellenaCadena("", 6);
                            cadenaEmpleado += RellenaCadena("", 1);
                            cadenaEmpleado += "0";
                            cadenaEmpleado += "0";
                            //Para le fecha de alta IMSS
                            cadenaEmpleado += empleado.FechaMovimiento.ToString().Substring(0, 2) + empleado.FechaMovimiento.ToString().Substring(3, 2) + empleado.FechaMovimiento.ToString().Substring(6, 4);
                            cadenaEmpleado += "   ";
                            cadenaEmpleado += RellenaCadena("", 2);
                            cadenaEmpleado += "08";
                            cadenaEmpleado += "     ";
                            cadenaEmpleado += RellenaCadena(empleado.ClaveEmpleado, 10);
                            cadenaEmpleado += " ";
                            cadenaEmpleado += empleado.Curp;
                            cadenaEmpleado += "9";

                            cadenaFinal += cadenaEmpleado + Environment.NewLine;
                        }

                    }

                    return cadenaFinal;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

        }

        private string RellenaCadena(string pCadena, int pPosiciones)
        {
            if (pCadena.Length <= pPosiciones)
            {
                while (pCadena.Length < pPosiciones)
                {
                    pCadena += " ";
                }
                return pCadena;
            }
            else
            {
                return pCadena.Substring(1, pPosiciones);
            }
        }

        public DataTable GetDataTableForVariabilidad(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_VariabilidadByIdCliente";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdCliente", SqlDbType.Int).Value = IdCliente;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        public DataTable GetDataTableRecuperacionesIMSS(int IdUnidadNegocio, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_IMSS_ReporteRecuperaciones";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

    }
}