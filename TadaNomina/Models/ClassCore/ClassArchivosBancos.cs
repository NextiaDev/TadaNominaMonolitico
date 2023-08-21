using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore
{
    public class ClassArchivosBancos
    {
        public DataTable GetTableTradicional(int IdUnidadNegocio, int IdPeriodo)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteArchivoBancos";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodo;
                    cmd.Parameters.Add("TipoEsquema", SqlDbType.NVarChar).Value = "T";
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        public DataTable GetTableEsquema(int IdUnidadNegocio, int IdPeriodo)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteArchivoBancos";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodo;
                    cmd.Parameters.Add("TipoEsquema", SqlDbType.NVarChar).Value = "S";
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        public DataTable GetTableHonorarios(int IdUnidadNegocio, int IdPeriodo)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteArchivoBancos";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodo;
                    cmd.Parameters.Add("TipoEsquema", SqlDbType.NVarChar).Value = "H";
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }
    }
}