using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class ClassReportesEmpleados
    {
        
        public List<ModelReportesEmpleados> GetListReportes(string Cliente, string UnidadNegocio)
        {
            List<ModelReportesEmpleados> rempleados = new List<ModelReportesEmpleados>();

            using (TadaReportesEntities entidad= new TadaReportesEntities())
            {
                var reportes = (from b in entidad.Cat_Reportes.Where(x=> x.TipoReporte=="Empleados" && x.IdEstatus==1) select b);

                foreach (var item in reportes)
                {
                    ModelReportesEmpleados rep = new ModelReportesEmpleados()
                    {
                        Id= item.IdReporte,
                        Cliente= Cliente,
                        UnidadNegocio= UnidadNegocio,
                        NombreReporte= item.NombreReporte
                    };

                    rempleados.Add(rep);
                }
            }

            return rempleados;
        }

       
        public DataTable GetDataTableForEmpleados(int IdUnidadNegocio, int IdReporte)
        {
            DataTable dt = new DataTable();
            string sp = "sp_Empleados_Rep";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("IdReporte", SqlDbType.Int).Value = IdReporte;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }
    }
}