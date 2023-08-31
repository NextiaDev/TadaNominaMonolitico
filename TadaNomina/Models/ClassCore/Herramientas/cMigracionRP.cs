using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Herramientas;

namespace TadaNomina.Models.ClassCore.Herramientas
{
    public class cMigracionRP
    {
        public mMigracionRP GetMigracionRP(int IdCliente)
        {
            mMigracionRP m = new mMigracionRP();
            ClassEmpleado classEmpleado = new ClassEmpleado();

            m.lregistros= classEmpleado.GetRegistrosPatronales(IdCliente);

            return m;
        }

        public void MigraEmpleadosRP(List<Empleados> empleados, int IdNuevoRegistroPatronal, DateTime FechaBaja, DateTime FechaAlta, bool ConservaAntiguedad, int IdUsuario)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                foreach (var item in empleados)
                {
                    MigraEmpleadoRP(item.IdEmpleado, IdNuevoRegistroPatronal, FechaBaja, FechaAlta, ConservaAntiguedad, IdUsuario);
                }
            }
        }

        public void MigraEmpleadoRP(int IdEmpleado, int IdNuevoRegistroPatronal, DateTime FechaBaja, DateTime FechaAlta, bool ConservaAntiguedad, int IdUsuario)
        {
            string sp = "sp_Nominas_MigracionRP";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdEmpleado", SqlDbType.Int).Value = IdEmpleado;
                    cmd.Parameters.Add("IdRegistroPatronal", SqlDbType.Int).Value = IdNuevoRegistroPatronal;
                    cmd.Parameters.Add("FechaBaja", SqlDbType.Date).Value = FechaBaja;
                    cmd.Parameters.Add("FechaAlta", SqlDbType.Date).Value = FechaAlta;
                    cmd.Parameters.Add("ConservaAntiguedad", SqlDbType.Bit).Value = ConservaAntiguedad;
                    cmd.Parameters.Add("IdUsuario", SqlDbType.Int).Value = IdUsuario;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

            }
        }

    }
}