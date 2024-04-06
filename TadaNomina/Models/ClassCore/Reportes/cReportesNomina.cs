using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class cReportesNomina
    {
        /// <summary>
        /// Metodo para listar los periodos acumulados por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <returns></returns>
        public List<ModelReporteNomina> GetListaPeriodosAcumulados(int IdUnidadNegocio)
        {
            List<ModelReporteNomina> resultado= new List<ModelReporteNomina>(); 

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] status = { 1, 2 };
                string[] TipoNomina = { "Nomina", "Finiquitos" };
                var periodos = (from b in entidad.vPeriodoNomina
                                where b.IdUnidadNegocio == IdUnidadNegocio && status.Contains((int)b.IdEstatus) && b.TipoNomina.Contains((string)b.TipoNomina)
                                orderby b.FechaFin descending
                                select b);

                foreach (var item in periodos)
                {
                    ModelReporteNomina reporteNomina = new ModelReporteNomina()
                    {
                        Id=item.IdPeriodoNomina,
                        Periodo= item.Periodo,
                        FechaInicio= item.FechaInicio.ToShortDateString(),
                        FechaFin= item.FechaFin.ToShortDateString(),
                        Cliente= item.Cliente,
                        UnidadNegocio= item.UnidadNegocio,
                        IdEstatus = item.IdEstatus
                    };

                    resultado.Add(reporteNomina);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Metodo para obtener información de una nómina en especifico
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <returns>Información de la nómina</returns>
        public DataTable GetDataTableForNomina(int IdPeriodoNomina)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteNominaGeneral";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();                
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;                    
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        /// <summary>
        /// Metodo para obtener información de la incidencias en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <returns>Incidencias en un periodo de nómina</returns>
        public DataTable GetDataTableForIncidencias(int IdPeriodoNomina)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteIncidencias";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }


        /// <summary>
        /// Metodo para regresar un lista de los periodos acumulados por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <returns>Lista de periodos acumulados</returns>
        public List<ModelReporteNomina> GetListaPeriodosAcumuladosActivos(int IdUnidadNegocio)
        {
            List<ModelReporteNomina> resultado = new List<ModelReporteNomina>();

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] status = { 1, 2 };
                var periodos = (from b in entidad.vPeriodoNomina
                                where b.IdUnidadNegocio == IdUnidadNegocio && status.Contains((int)b.IdEstatus) && b.TipoNomina == "Nomina"
                                orderby b.FechaFin descending
                                select b);

                foreach (var item in periodos)
                {
                    ModelReporteNomina reporteNomina = new ModelReporteNomina()
                    {
                        Id = item.IdPeriodoNomina,
                        Periodo = item.Periodo,
                        FechaInicio = item.FechaInicio.ToShortDateString(),
                        FechaFin = item.FechaFin.ToShortDateString(),
                        Cliente = item.Cliente,
                        UnidadNegocio = item.UnidadNegocio
                    };

                    resultado.Add(reporteNomina);
                }
            }

            return resultado;
        }




        public List<ModelReporteNomina> GetListaPeriodosAcumuladosActivosReportes(int IdUnidadNegocio)
        {
            List<ModelReporteNomina> resultado = new List<ModelReporteNomina>();

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] status = { 1, 2 };
                string[] Tipo = { "Nomina", "Aguinaldo" };
                var periodos = (from b in entidad.vPeriodoNomina
                                where b.IdUnidadNegocio == IdUnidadNegocio && status.Contains((int)b.IdEstatus) && Tipo.Contains((string)b.TipoNomina)
                                orderby b.FechaFin descending
                                select b); ;

                foreach (var item in periodos)
                {
                    ModelReporteNomina reporteNomina = new ModelReporteNomina()
                    {
                        Id = item.IdPeriodoNomina,
                        Periodo = item.Periodo,
                        FechaInicio = item.FechaInicio.ToShortDateString(),
                        FechaFin = item.FechaFin.ToShortDateString(),
                        Cliente = item.Cliente,
                        UnidadNegocio = item.UnidadNegocio
                    };

                    resultado.Add(reporteNomina);
                }
            }

            return resultado;
        }
        /// <summary>
        /// Metodo que obtiene los periodos abiertos y cerrados de una unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <returns>Lista de periodos</returns>
        public List<PeriodoNomina> GetPeriodosActivosyCerrados(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] idestatus = { 1, 2, 3 };
                var periodos = (from b in entidad.PeriodoNomina.Where(x => idestatus.Contains((int)x.IdEstatus) && x.IdUnidadNegocio == IdUnidadNegocio) select b).ToList();

                return periodos;
            }
        }

        /// <summary>
        /// Metodo que obtiene datos del registro patronal por unidad de negocio
        /// </summary>
        /// <param name="idunidadnegocio">Unidad de negocio</param>
        /// <returns>Registro patronal</returns>
        public List<vRegistroPatronal> GetRegistroPatronal(int idunidadnegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var Reg = (from b in entidad.vRegistroPatronal.Where(x => x.IdUnidadNegocio == idunidadnegocio && x.IdEstatus == 1) select b).ToList();

                return Reg;
            }
        }

        /// <summary>
        /// Metodo que obtiene incidencias por periodos de nómina y centros de costos
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodos de nómina</param>
        /// <param name="CC">Centro de costos</param>
        /// <returns>Lista de incidencias consolidadas</returns>
        public List<vIncidencias_Consolidadas> GetvInsidencias(int[] IdPeriodoNomina, int CC)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.Monto > 0 && x.IdCentroCostos == CC && IdPeriodoNomina.Contains((int)x.IdPeriodoNomina)) select b).ToList();

                List<vIncidencias_Consolidadas> incidenciasp = new List<vIncidencias_Consolidadas>();
                var incidenciasPrev = GetvInsidenciasPrev(IdPeriodoNomina);

                incidenciasPrev.ForEach(x => { incidenciasp.Add(new vIncidencias_Consolidadas { IdIncidencia = x.IdIncidencia, IdPeriodoNomina = x.IdPeriodoNomina, TipoConcepto = x.TipoConcepto, Concepto = x.Concepto, IdEmpleado = x.IdEmpleado, ClaveConcepto = x.ClaveConcepto, Monto = x.Monto, Cantidad = x.Cantidad, Gravado = x.Gravado, Exento = x.Exento, FechaModifica = x.FechaModifica, TipoEsquema = x.TipoEsquema, Observaciones = x.Observaciones, IdEstatus = x.IdEstatus, IdCaptura = x.IdCaptura, FechaCaptura = x.FechaCaptura, IdModifica = x.IdModifica, TipoDato = x.TipoDato }); });

                incidencias.AddRange(incidenciasp);

                return incidencias;
            }
        }

        /// <summary>
        /// Metodo para obtener incidencias por periodo y depapartamento
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nomina/param>
        /// <param name="dep">Departamento</param>
        /// <returns></returns>
        public List<vIncidencias_Consolidadas> GetvInsidenciasdepar(int[] IdPeriodoNomina, int dep)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdDepartamento == dep && x.Monto > 0 && IdPeriodoNomina.Contains((int)x.IdPeriodoNomina)) select b).ToList();

                List<vIncidencias_Consolidadas> incidenciasp = new List<vIncidencias_Consolidadas>();
                var incidenciasPrev = GetvInsidenciasPrev(IdPeriodoNomina);

                incidenciasPrev.ForEach(x => { incidenciasp.Add(new vIncidencias_Consolidadas { IdIncidencia = x.IdIncidencia, IdPeriodoNomina = x.IdPeriodoNomina, TipoConcepto = x.TipoConcepto, Concepto = x.Concepto, IdEmpleado = x.IdEmpleado, ClaveConcepto = x.ClaveConcepto, Monto = x.Monto, Cantidad = x.Cantidad, Gravado = x.Gravado, Exento = x.Exento, FechaModifica = x.FechaModifica, TipoEsquema = x.TipoEsquema, Observaciones = x.Observaciones, IdEstatus = x.IdEstatus, IdCaptura = x.IdCaptura, FechaCaptura = x.FechaCaptura, IdModifica = x.IdModifica, TipoDato = x.TipoDato }); });

                incidencias.AddRange(incidenciasp);

                return incidencias;
            }
        }

        /// <summary>
        /// Metdos para obtener información de las incidencias Previsualizadas por periodo de nómina
        /// </summary>periodo de nomina
        /// <param name="IdPeriodoNomina">Arreglo de periodos de nomina</param>
        /// <returns>lista de incidencias</returns>
        public List<vIncidencias> GetvInsidenciasPrev(int[] IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.vIncidencias.Where(x => x.IdEstatus == 1 && x.Monto > 0 && IdPeriodoNomina.Contains((int)x.IdPeriodoNomina)) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Metodo para obtener información del costeo mensual en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <returns>Información de costeo mensual</returns>
        public DataTable GetDataTableForNominaCC(int IdPeriodoNomina)
        {
            

            DataTable dt = new DataTable();
            string sp = "sp_CosteoResumenMensualCCySucursal";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdsPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        /// <summary>
        /// Metodo para agregar lista de movimientos altas o bajas a un modelo
        /// </summary>
        /// <returns>Lista de movimientos</returns>
        public ModelRepByFechasTM getModelAltasBajasEmpleados()
        {
            List<SelectListItem> listMov = new List<SelectListItem>();
            listMov.Add(new SelectListItem { Text = "ALTAS", Value = "A" });
            listMov.Add(new SelectListItem { Text = "BAJAS", Value = "B" });

            ModelRepByFechasTM m = new ModelRepByFechasTM();
            m.ltipoMovimiento = listMov;

            return m;
        }

        /// <summary>
        /// Metodo para asignar los tipos de movimientos de timbrados
        /// </summary>
        /// <returns>Modelo con lista de tipo de movimientos</returns>
        public ModelRepByFechasTM GetModelReporteTimbrado()
        {
            List<SelectListItem> listMov = new List<SelectListItem>();
            listMov.Add(new SelectListItem { Text = "TIMBRADOS", Value = "S" });
            listMov.Add(new SelectListItem { Text = "NO TIMBRADOS", Value = "N" });
            listMov.Add(new SelectListItem { Text = "CANCELADOS", Value = "C" });
            
            ModelRepByFechasTM m = new ModelRepByFechasTM();
            m.ltipoMovimiento = listMov;

            return m;
        }

        /// <summary>
        /// Metodo que regresa el nombre de un archvo con cierta estructura
        /// </summary>
        /// <param name="prefijo">Prefijo</param>
        /// <param name="nomina">Nomina</param>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <returns></returns>
        public string RegresaNombreArchivo(string prefijo, string nomina, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            string fi = pFechaInicial.ToShortDateString().Replace("/", string.Empty);
            string ff = pFechaFinal.ToShortDateString().Replace("/", string.Empty);

            switch (prefijo)
            {
                case "A":
                    prefijo = "ALTAS_";
                    break;

                case "B":
                    prefijo = "BAJAS_";
                    break;

                case "C":
                    prefijo = "MS_";
                    break;

                case "ST":
                    prefijo = "TIMBRADOS_";
                    break;
                case "NT":

                    prefijo = "NO_TIMBRADOS_";
                    break;

            }

            return prefijo+nomina+"_"+ fi+"-"+ff+".xlsx";
        }

        /// <summary>
        /// Metodo para obtener las altas o bajas por unidad de negocio en un rango de fechas
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <param name="seleccion">Altas o bajas</param>
        /// <returns>Daos de altas o bajas</returns>
        public DataTable GetDataTableAltasBajasEmpleados(int IdUnidadNegocio, DateTime pFechaInicial, DateTime pFechaFinal, string seleccion)
        {
            DataTable dt = new DataTable();
            string sp = "sp_AltasBajasEmpleadosByFechas";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    cmd.Parameters.Add("Seleccion", SqlDbType.VarChar).Value = seleccion;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        /// <summary>
        /// Metodo para listar los saldos por clientes
        /// </summary>
        /// <param name="idusuario">Usuario</param>
        /// <returns>Lista de saldos</returns>
        public List<ModelReporteSaldosPorCliente> ListaSaldosPorCliente(int idusuario)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from b in ctx.sp_SaldosPorClientes(idusuario)
                        select new ModelReporteSaldosPorCliente
                        {
                            Cliente = b.Cliente,
                            IdFacturadora = b.IdFacturadora,
                            Facturadora = b.Facturadora,
                            Saldo = (decimal)b.Saldo,
                        }).ToList();
            }
        }

        /// <summary>
        /// Metodo para listar los saldos con detalle por clientes
        /// </summary>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="IdFacturadora">Facturadora</param>
        /// <returns>Lista de saldos con detalle</returns>
        public List<ModelReporteSaldosPorCliente> DetalleSaldosPorCliente(int IdCliente, int IdFacturadora)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from b in ctx.sp_SaldosClientesDetalle(IdCliente, IdFacturadora)
                        select new ModelReporteSaldosPorCliente
                        {
                            Cliente = b.Cliente,
                            IdCliente = (int)b.IdCliente,
                            IdFacturadora = b.IdFacturadora,
                            Facturadora = b.Facturadora,
                            Importe = (decimal)b.Importe,
                            TipoMovimiento = b.TipoMovimiento,
                            FechaCaptura = (DateTime)b.FechaCaptura,
                        }).ToList();
            }
        }

        /// <summary>
        /// Metodo para obtener un consentrado del ISN por rango de fechas de acuerdo a la unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <returns></returns>
        public DataTable GetDataTableReporteISN(int IdUnidadNegocio, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_Nominas_ReporteISN";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
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
        /// Metodo para obtener información de incidencias generadas por cliente en un rango de fechas
        /// </summary>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="pFechaInicial">Fecha incial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <returns>Información de incidencias</returns>
        public DataTable GetDataTableReporteIncidenciasByClienteFechas(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_Nominas_IncidenciasByClienteFechas";
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
                }
                return dt;
            }
        }

        /// <summary>
        /// Metodo para obtener información de nominas acumuladas por empleado
        /// </summary>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <returns></returns>
        public DataTable GetDataTableReporteNominaAcumuladoPorEmpleado(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteNominaTrad_Esq_AcumuladoPorEmpleado";
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
                }
                return dt;
            }
        }

        /// <summary>
        /// Metodo para gener reporte de nomina por centro de costos 
        /// </summary>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <returns>Informacion por centro de costos</returns>
        public DataTable GetDataTableReporteEspecialAgrupadoPorCC(DateTime pFechaInicial, DateTime pFechaFinal, int IdUnidadNegocio)
        {
            DataTable dt = new DataTable();
            string sp = "sp_Nominas_ReporteAcumuladoByCC";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;

                    dt.Load(cmd.ExecuteReader());
                }
                return dt;
            }
        }

        /// <summary>
        /// Metodo para obtener la información acumulada por cliente
        /// </summary>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <returns>Información por cliente</returns>
        public DataTable GetDataTableReporteAcumualdoByCliente(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteNominaTrad_Esq_ByIdCliente";
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

        /// <summary>
        /// Metodo para obtener información de facturación por clientes
        /// </summary>
        /// <param name="IdCliente">Cliente</param>
        /// <param name="pFechaInicial">Fecha Inicial</param>
        /// <param name="pFechaFinal">Fecha Final</param>
        /// <returns>Informacion de facturación</returns>
        public DataTable GetDataTableReporteFacturacionByCliente(int IdCliente, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteFacturacionByIdCliente";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelContabilidad"].ConnectionString))
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

        /// <summary>
        /// Metodo para generar información de empleados administrados por usuario
        /// </summary>
        /// <param name="IdUsuario">Usuario</param>
        /// <returns>Información de empleados administrados</returns>
        public DataTable GetDataTableReporteEmpleadosAdministradosByUsuario(int IdUsuario)
        {
            DataTable dt = new DataTable();
            string sp = "sp_EmpleadosAdministrados";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUsuario", SqlDbType.Int).Value = IdUsuario;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        /// <summary>
        /// Metodo que genera informacion de timbrados por rango de fechas
        /// </summary>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <param name="pFechaInicial">Fecha inicial</param>
        /// <param name="pFechaFinal">Fecha final</param>
        /// <param name="seleccion"></param>
        /// <returns>información de timbrados</returns>
        public DataTable GetDataTableReporteTimbrado(int IdUnidadNegocio, DateTime pFechaInicial, DateTime pFechaFinal, string seleccion)
        {
            DataTable dt = new DataTable();
            string sp = "sp_Nominas_Timbrado";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    cmd.Parameters.Add("Seleccion", SqlDbType.VarChar).Value = seleccion;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
            }
        }

        public DataTable DatosProcesados(DataTable DatosBD)
        {
            List<ModelReporteNominaTotal> mrnt = new List<ModelReporteNominaTotal>();
            DataTable dt = new DataTable();
            object[] a = new object[DatosBD.Columns.Count];

            for (int i = 0; i < DatosBD.Columns.Count; i++)
            {
                dt.Columns.Add(DatosBD.Columns[i].ColumnName);
                switch (DatosBD.Columns[i].ColumnName)
                {
                    case "SD":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "SDI":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "SueldoMensual":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Faltas":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Incapacidades":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DiasIMSS":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DiasTrabajados":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Sueldo":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Sub. Empleo":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "ReintegroISR":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Vacaciones":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "APOYO_BONO_S":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "BONO_POR_REFERIDO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "COMISIONES":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "COMPLEMENTO_SUELDO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DESCANSOS_TRABAJADOS":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Devolucion_INFONAVIT":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DIA_FESTIVO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DIAS_DE_RETROACTIVO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "GRATIFICACIÓN_ANUAL_AGUINALDO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "GRATIFICACIONES":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "HORAS_EXTRA_DOBLES":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "INDEMNIZACIÓN_20_DÍAS_":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "INDEMNIZACIÓN_3_MESES":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "OTRAS_PERCEPCIONES":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PRIMA_DE_ANTIGÜEDAD":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PRIMA_DOMINICAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PRIMA_VACACIONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PRIMA_VACACIONAL_PENDIENTE":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PTU":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "REEMBOLSOS":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Retroactivo_Sueldo":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "SUELDOS":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Total Percep":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "ISR":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS Obrero":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS AJUSTE_INFONAVIT":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Descuento_Equipo":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DESCUENTO_FONACOT":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DESCUENTO_INFONAVIT":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "DESCUENTOS_DIVERSOS":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "FALTA":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "INCAPACIDAD_ENFERMEDAD_GENERAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "INCAPACIDAD_MATERNIDAD":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "OTRAS_DEDUCCIONES":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PENSION_ALIMENTICIA":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "PERMISO_SIN_GOCE_DE_SUELDO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "SERVICIO_MONTO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "SGM_90":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Total Deduc.":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Neto":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_ENFERMEDAD Y MATERNIDAD":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_INVALIDEZ Y VIDA":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_CESANTIA Y VEJEZ":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_OBRERO":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_ENFERMEDAD Y MATERNIDAD PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_INVALIDEZ Y VIDA PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_CESANTIA Y VEJEZ PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_RIESGO DE TRABAJO PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_GUARDERIA PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "IMSS_RETIRO PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "INFONAVIT PATRONAL":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Total_Patron":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "ProvisionAguinaldo":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "ProvisionVacaciones":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "ProvisionPrimaVacacional":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "BaseGravada":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "Exento":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    case "ISN":
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = true, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                    default:
                        mrnt.Add(new ModelReporteNominaTotal { Bandera = false, Nombre = DatosBD.Columns[i].ColumnName, Total = 0 });
                        break;
                }
            }

            if (DatosBD.Rows.Count > 0)
            {
                for (int i = 0; i < DatosBD.Rows.Count; i++)
                {
                    dt.Rows.Add(DatosBD.Rows[i].ItemArray);

                    for (int j = 0; j < mrnt.Count; j++)
                    {
                        if (mrnt[j].Bandera == true)
                        {
                            var datos = DatosBD.Rows[i].ItemArray[j].ToString();
                            if (datos != "")
                                mrnt[j].Total += decimal.Parse(datos);
                        }
                    }
                }
                for (int i = 0; i < DatosBD.Columns.Count; i++)
                {
                    if (mrnt[i].Total > 0)
                    {
                        a[i] = mrnt[i].Total;
                    }
                    else
                    {

                        a[i] = null;
                    }
                    if (mrnt[i].Nombre == "Nombre")
                    {
                        a[i] = "Total empleados: " + DatosBD.Rows.Count;
                    }
                }
                dt.Rows.Add(a);
            }

            return dt;
        }

        public DataTable InfoDatosProcesados(int IdPeriodo, int IdUN)
        {
            DataTable dt = new DataTable();
            string NombrePatrona;
            string Nomina;
            DateTime fFin;
            DateTime fInicio;

            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                NombrePatrona = ctx.Cat_UnidadNegocio.Where(x => x.IdUnidadNegocio == IdUN).Select(x => x.UnidadNegocio).FirstOrDefault();
            }
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                Nomina = ctx.vPeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo).Select(x => x.Periodo).FirstOrDefault();
                fInicio = ctx.vPeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo).Select(x => x.FechaInicio).FirstOrDefault();
                fFin = ctx.vPeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo).Select(x => x.FechaFin).FirstOrDefault();
            }

            dt.Columns.Add("Información");
            dt.Rows.Add(NombrePatrona);
            dt.Rows.Add("Reporte " + Nomina);
            dt.Rows.Add("Período: " + fInicio.ToShortDateString() + " al " + fFin.ToShortDateString());

            return dt;
        }


        public DataTable GetDataTableReporteHonorarios(DateTime pFechaInicial, DateTime pFechaFinal, string Estatus, int idcliente)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteHonorarios";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    cmd.Parameters.Add("BanderaEstatus", SqlDbType.NVarChar).Value = Estatus;
                    cmd.Parameters.Add("IdCliente", SqlDbType.Int).Value = idcliente;

                    dt.Load(cmd.ExecuteReader());
                }
                return dt;
            }
        }

        public List<vIncidencias_Consolidadas> GetvInsidenciasPeriodo(int[] IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.Monto > 0 && IdPeriodoNomina.Contains((int)x.IdPeriodoNomina)) select b).ToList();

                List<vIncidencias_Consolidadas> incidenciasp = new List<vIncidencias_Consolidadas>();
                var incidenciasPrev = GetvInsidenciasPrev(IdPeriodoNomina);

                incidenciasPrev.ForEach(x => { incidenciasp.Add(new vIncidencias_Consolidadas { IdIncidencia = x.IdIncidencia, IdPeriodoNomina = x.IdPeriodoNomina, TipoConcepto = x.TipoConcepto, Concepto = x.Concepto, IdEmpleado = x.IdEmpleado, ClaveConcepto = x.ClaveConcepto, Monto = x.Monto, Cantidad = x.Cantidad, Gravado = x.Gravado, Exento = x.Exento, FechaModifica = x.FechaModifica, TipoEsquema = x.TipoEsquema, Observaciones = x.Observaciones, IdEstatus = x.IdEstatus, IdCaptura = x.IdCaptura, FechaCaptura = x.FechaCaptura, IdModifica = x.IdModifica, TipoDato = x.TipoDato }); });

                incidencias.AddRange(incidenciasp);

                return incidencias;
            }
        }


        public DataTable GetDataTableAusentimos(int IdUnidadNegocio, DateTime pFechaInicial, DateTime pFechaFinal)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteAusentismos";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("FechaInicial", SqlDbType.Date).Value = pFechaInicial;
                    cmd.Parameters.Add("FechaFinal", SqlDbType.Date).Value = pFechaFinal;
                    cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                    dt.Load(cmd.ExecuteReader());
                    con.Close();
                }

                return dt;
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

        public string RegresaNombreReporte(int IdPeriodoNomina)
        {
            string res = "ReporteNomina_P_";

            return res += IdPeriodoNomina.ToString() + ".xlsx";
        }

        public DataTable GetDataTableForReporteByIdPeriodoNomina(int IdPeriodoNomina)
        {
            DataTable dt = new DataTable();
            string sp = "sp_ReporteByIdPeriodoNominaAcumulado";
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