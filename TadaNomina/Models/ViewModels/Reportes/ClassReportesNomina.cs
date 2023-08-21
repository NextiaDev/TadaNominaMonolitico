using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Reportes
{
    public class ClassReportesNomina
    {
        public List<sp_ReporteNominaGeneral_Temporal_Result> GetReporteNomina(int IdPeriodo)
        {
            using (TadaReportesEntities entidad = new TadaReportesEntities())
            {
                var registros = from b in entidad.sp_ReporteNominaGeneral_Temporal(IdPeriodo) select b;

                return registros.ToList();
            }
        }

        public DataTable GetTableNomina(int IdPeriodoNomina)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteNominaGeneral";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }
    }
}