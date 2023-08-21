using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore.Contabilidad
{
    public class ClassContabilidad
    {
        /// <summary>
        ///     Método que obtiene la información de los reportes contables
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de unidad negocio</param>
        /// <param name="pFechaInicial">Variable que contiene la fecha inicial del periodo</param>
        /// <param name="pFechaFinal">Variable que contiene la fecha final del periodo</param>
        /// <returns></returns>
        public DataTable GetDataTableForReporteConatble(int IdUnidadNegocio, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteContable";
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

        /// <summary>
        ///     Método que obtiene los nomres 
        /// </summary>
        /// <param name="pFechaInicial"></param>
        /// <param name="pFechaFinal"></param>
        /// <returns></returns>
        public string RegresaNombreReporteContable(DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string res = "ReporteContable_";

            string fi = pFechaInicial.ToShortDateString().Replace("/", string.Empty);
            string ff = pFechaFinal.ToShortDateString().Replace("/", string.Empty);

            return res += fi + '_' + ff + ".xlsx";
        }
        public string RegresaNombreReporteContable(int IdPeriodoNomina)
        {
            string res = "ReporteContable_";

            return res += IdPeriodoNomina.ToString() + ".xlsx";
        }

        /// <summary>
        ///     Elimina el registro de la poliza contable
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        public void eliminaInfoPoliza(int IdPeriodoNomina)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                string consulta = "delete from PolizasContables where IdPeriodoNomina=" + IdPeriodoNomina;
                entidad.Database.ExecuteSqlCommand(consulta);
                entidad.SaveChanges();
            }
        }
        /// <summary>
        /// Metodo para obtener un modelo con un çombo con los periodos de nomina acumulados
        /// </summary>
        /// <param name="IdUnidadNegocio">Id de la Unidad de Negocio</param>
        /// <returns></returns>
        public ModelReporteByIdPeriodo GetModelReporteByIdPeriodo(int IdUnidadNegocio)
        {
            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            ModelReporteByIdPeriodo m = new ModelReporteByIdPeriodo();
            List<SelectListItem> lperiodos = new List<SelectListItem>();
            List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoNominasAcumuladas(IdUnidadNegocio)
                .OrderByDescending(x => x.IdPeriodoNomina).Take(250).ToList();

            lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });

            m.lPeriodos = lperiodos;

            return m;
        }
        /// <summary>
        /// Obtiene un Data Table con el reporte contable por periodo de nomina
        /// </summary>
        /// <param name="IdPeriodoNomina">Id del Periodo de nomina a consultar</param>
        /// <returns></returns>
        public DataTable GetDataTableForReporteContableByIdPeriodoNomina(int IdPeriodoNomina)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteContableByIdPeriodoNomina";
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