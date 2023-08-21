using API_Nomors.Core.CFDI40;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using XSDToXML.Utils;

namespace TadaNomina.Models.ClassCore.TimbradoTP.CFDI40
{
    public class cAux
    {
        public cTipos tipos = new cTipos();

        //public List<Cat_ConceptosNomina> conceptos;
        public decimal TotalOtrasDeducciones { get; set; }
        public decimal TotalImpuestosRetenidos { get; set; }
        public string XMLTotalDeducciones { get; set; }
        public decimal TotalGravadoPercepciones { get; set; }
        public decimal TotalExcentoPercepciones { get; set; }
        public string XMLTotalPercepciones { get; set; }
        protected string _KeyPass;
        protected string _cer;
        protected string _key;
        protected string _noCert;
        protected string aa, b, c;

        protected void ObtenCertificadosCfdi(string rutaCer, string rutaKey, string Keypass)
        {
            _cer = rutaCer;
            _key = rutaKey;
            _KeyPass = Keypass;

            SelloDigital.leerCER(_cer, out aa, out b, out c, out _noCert);
        }

        /// <summary>
        /// Metodo para obtener las incidencias consolidadas de un empleado por periodo de nomina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nomina</param>
        /// <param name="IdEmpleado">Clave del empleado</param>
        /// <returns></returns>
        public List<vIncidencias_Consolidadas> GetVInsidencias_Consolidadas(int IdPeriodo, int IdEmpleado)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.Monto > 0) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Metodo para obtener las percepciones de un empleado en un periodo de nomina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="IdEmpleado">Clave del empleado</param>
        /// <param name="Sueldo">Sueldo bruto del empleado</param>
        /// <param name="SubsidioEmpleo">Subsidio</param>
        /// <param name="Vacaciones">Vacaciones</param>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <param name="incidencias">Lista de incidencias</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NominaPercepcionesPercepcion[] ObtenPercepciones(int IdPeriodo, int IdEmpleado, string Sueldo, string SubsidioEmpleo, string Vacaciones, int IdUnidadNegocio, List<vIncidencias_Consolidadas> incidencias)
        {
            NominaPercepcionesPercepcion psueldo = null;
            NominaPercepcionesPercepcion pVacaciones = null;

            int cantidad = 0;
            int _cantidad = 0;

            decimal _sueldo = decimal.Parse(Sueldo);
            decimal _vacaciones = decimal.Parse(Vacaciones);

            if (_sueldo > 0)
            {
                psueldo = new NominaPercepcionesPercepcion();
                psueldo.TipoPercepcion = tipos.ObtenTipoPercepcion("001");
                psueldo.Clave = "001";
                psueldo.Concepto = "SUELDO";
                psueldo.ImporteGravado = _sueldo;
                psueldo.ImporteExento = 0;

                cantidad++;
            }

            var cantidadIncidencias = (from b in incidencias.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.TipoConcepto == "ER" && x.Monto > 0) select b);

            int _cantidadIncidencias = cantidadIncidencias.Count();
            List<NominaPercepcionesPercepcion> listaConceptos = new List<NominaPercepcionesPercepcion>();
            if (_cantidadIncidencias > 0)
            {
                _cantidad = cantidad;
                foreach (var item in cantidadIncidencias)
                {
                    var claveSAT = item.ClaveSAT;

                    if (claveSAT == null || claveSAT == "0" || claveSAT == string.Empty) { throw new Exception("El concepto con Id: " + item.IdConcepto + ": " + item.ClaveConcepto + " - " + item.Concepto + " no tiene definida una clave SAT."); }
                    var suma = (decimal)item.Exento + (decimal)item.Gravado;
                    if (claveSAT.Length == 3 && suma > 0)
                    {
                        if (claveSAT.Equals("019"))
                        {
                            NominaPercepcionesPercepcion pVariable = new NominaPercepcionesPercepcion();
                            pVariable.TipoPercepcion = tipos.ObtenTipoPercepcion(claveSAT);
                            pVariable.Clave = item.ClaveConcepto;
                            pVariable.Concepto = item.Concepto;
                            pVariable.ImporteExento = (decimal)item.Exento;
                            pVariable.ImporteGravado = (decimal)item.Gravado;
                            pVariable.HorasExtra = ObtenHorasExtra(item, incidencias);

                            listaConceptos.Add(pVariable);
                        }
                        else
                        {
                            NominaPercepcionesPercepcion pVariable = new NominaPercepcionesPercepcion();
                            pVariable.TipoPercepcion = tipos.ObtenTipoPercepcion(claveSAT);
                            pVariable.Clave = item.ClaveConcepto;
                            pVariable.Concepto = item.Concepto;
                            pVariable.ImporteExento = (decimal)item.Exento;
                            pVariable.ImporteGravado = (decimal)item.Gravado;

                            listaConceptos.Add(pVariable);
                        }

                        cantidad++;
                    }
                }
            }

            NominaPercepcionesPercepcion[] mnpp = new NominaPercepcionesPercepcion[cantidad];
            if (_sueldo > 0) { mnpp[0] = psueldo; }
            try { if (_vacaciones > 0) { mnpp[1] = pVacaciones; } } catch { mnpp[0] = pVacaciones; }
            if (_sueldo == 0 && _vacaciones != 0) { mnpp[0] = pVacaciones; }
            //if (_sueldo == 0 && _vacaciones == 0) { _cantidad = cantidad; }
            foreach (var item in listaConceptos)
            {
                mnpp[_cantidad] = item;
                _cantidad++;
            }

            return mnpp;
        }

        /// <summary>
        /// Metodo para obtener horas extras por empleado en un periodo de nómina
        /// </summary>
        /// <param name="item">Datos empleado y periodo nomina</param>
        /// <param name="incidencias">Incidencias</param>
        /// <returns></returns>
        private NominaPercepcionesPercepcionHorasExtra[] ObtenHorasExtra(vIncidencias_Consolidadas item, List<vIncidencias_Consolidadas> incidencias)
        {
            NominaPercepcionesPercepcionHorasExtra[] _horasExtra = new NominaPercepcionesPercepcionHorasExtra[1];
            NominaPercepcionesPercepcionHorasExtra horasExtra = new NominaPercepcionesPercepcionHorasExtra();

            horasExtra.Dias = 1;
            horasExtra.TipoHoras = tipos.ObtenTipoHorasExtra(item.ClaveConcepto);
            horasExtra.HorasExtra = (int)ObtenCantidadHorasExtra((int)item.IdEmpleado, (int)item.IdPeriodoNomina, (item.ClaveConcepto + "C"), incidencias);
            horasExtra.ImportePagado = (decimal)item.Monto;
            _horasExtra[0] = horasExtra;
            return _horasExtra;
        }

        /// <summary>
        /// Metodo para sumar la contidad de horas extras por empleado en un periodo de nómina
        /// </summary>
        /// <param name="IdEmpleado">Clave empleado</param>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="ClaveConcepto">Concepto</param>
        /// <param name="incidencias">Lista de incidencias</param>
        /// <returns></returns>
        public decimal ObtenCantidadHorasExtra(int IdEmpleado, int IdPeriodo, string ClaveConcepto, List<vIncidencias_Consolidadas> incidencias)
        {
            decimal Cantidad = 1;
            var cantidad = (from b in incidencias.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.ClaveConcepto == ClaveConcepto && x.IdEstatus == 1) select b).Sum(x => x.Cantidad);

            if (cantidad != 0)
                Cantidad = (decimal)cantidad;

            return Cantidad;
        }

        /// <summary>
        /// Metodo para obtener deducciones aplicadas a un empleado por su periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo nómina</param>
        /// <param name="IdEmpleado">Clave empleado</param>
        /// <param name="IMSS">Cantidad IMSS</param>
        /// <param name="ISR">Cantidad ISR</param>
        /// <param name="Incidencias">Lista de incidencias consolidadas</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NominaDeduccionesDeduccion[] ObtenDeducciones(int IdPeriodo, int IdEmpleado, string IMSS, string ISR, List<vIncidencias_Consolidadas> Incidencias)
        {
            NominaDeduccionesDeduccion dImss = null;
            NominaDeduccionesDeduccion dISR = null;
            int cantidad = 0;
            int _cantidad = 0;

            decimal _imss = decimal.Parse(IMSS);
            decimal _isr = decimal.Parse(ISR);

            if (_imss > 0)
            {
                dImss = new NominaDeduccionesDeduccion();
                dImss.TipoDeduccion = tipos.ObtenTipoDeduccion("001");
                dImss.Clave = "022";
                dImss.Concepto = "SEGURIDAD SOCIAL";
                dImss.Importe = _imss;

                cantidad++;
            }

            if (_isr > 0)
            {
                dISR = new NominaDeduccionesDeduccion();
                dISR.TipoDeduccion = tipos.ObtenTipoDeduccion("002");
                dISR.Clave = "021";
                dISR.Concepto = "ISR";
                dISR.Importe = _isr;

                cantidad++;
            }

            var cantidadIncidencias = (from b in Incidencias.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.TipoConcepto == "DD" && x.Monto > 0) select b);
            int _cantidadIncidencias = cantidadIncidencias.Count();
            List<NominaDeduccionesDeduccion> listaConceptos = new List<NominaDeduccionesDeduccion>();
            if (_cantidadIncidencias > 0)
            {
                _cantidad = cantidad;
                foreach (var item in cantidadIncidencias)
                {                    
                    var claveSAT = item.ClaveGpo == "501" ? "006" : item.ClaveSAT;
                    if (claveSAT == null || claveSAT == "0" || claveSAT == string.Empty) { throw new Exception("El concepto con Id: " + item.IdConcepto + ": " + item.ClaveConcepto + " - " + item.Concepto + " no tiene definida una clave SAT."); }
                    if (claveSAT.Length == 3)
                    {
                        NominaDeduccionesDeduccion dVariable = new NominaDeduccionesDeduccion();
                        dVariable.TipoDeduccion = tipos.ObtenTipoDeduccion(claveSAT);
                        dVariable.Clave = item.ClaveConcepto;
                        dVariable.Concepto = item.Concepto;
                        dVariable.Importe = (decimal)item.Monto;

                        listaConceptos.Add(dVariable);

                        cantidad++;
                    }
                }
            }

            NominaDeduccionesDeduccion[] mndd = new NominaDeduccionesDeduccion[cantidad];
            if (_imss > 0) { mndd[0] = dImss; }
            try { if (_isr > 0) { mndd[1] = dISR; } } catch { mndd[0] = dISR; }
            if (_imss == 0 && _isr != 0) { mndd[0] = dISR; }
            foreach (var item in listaConceptos)
            {
                mndd[_cantidad] = item;
                _cantidad++;
            }

            return mndd;
        }

        /// <summary>
        /// Metodo para dar un tamaño de logitud a la clave del empleado
        /// </summary>
        /// <param name="ClaveEmpleado">Clave del empleado</param>
        /// <returns>Regresa clave con logitud 6</returns>
        public string DarFormatoClaveEmpleado(string ClaveEmpleado)
        {
            string Clave = string.Empty;
            if (ClaveEmpleado.Length == 1)
                Clave = "00000" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 2)
                Clave = "0000" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 3)
                Clave = "000" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 4)
                Clave = "00" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 5)
                Clave = "0" + ClaveEmpleado;
            if (ClaveEmpleado.Length == 6)
                Clave = ClaveEmpleado;

            if (Clave == string.Empty)
                Clave = ClaveEmpleado;

            return Clave;
        }

        /// <summary>
        /// Metodo para obtener las deducciones de un periodo de nómina para un empleado
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="IdEmpleado">Clave del empleado</param>
        /// <param name="IMSS">Cantidad IMSS</param>
        /// <param name="ISPT">Descuento ISPT</param>
        /// <exception cref="Exception">Metodo para cachar cualquier error y poder mostrarlo al usuario</exception>
        public void ObtenTotalesDeducciones(int IdPeriodo, int IdEmpleado, decimal IMSS, decimal ISPT)
        {
            using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                try
                {
                    sqlconn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenTotalesDeducciones_Nom12", sqlconn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("IdPeriodo", SqlDbType.Int).Value = IdPeriodo;
                        cmd.Parameters.Add("IdEmpleado", SqlDbType.Int).Value = IdEmpleado;
                        cmd.Parameters.Add("IMSS", SqlDbType.Decimal).Value = IMSS;
                        cmd.Parameters.Add("ISPT", SqlDbType.Decimal).Value = ISPT;
                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            TotalOtrasDeducciones = decimal.Parse(dr["TotalOtrasDeducciones"].ToString());
                            TotalImpuestosRetenidos = decimal.Parse(dr["TotalImpuestosRetenidos"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    sqlconn.Close();
                }
            }
        }

        /// <summary>
        /// Metodo para obtener las percepciones por empleado en un perido de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de Nómina</param>
        /// <param name="IdEmpleado">Clave del empleado</param>
        /// <param name="Sueldo">Sueldo del empleado</param>
        /// <param name="SubsidioEmpleo">Subsidio del empleado</param>
        /// <param name="ReintegroISR"></param>
        /// <param name="Vacaciones"></param>
        /// <param name="IdUnidadNegocio">Unidad de negocio</param>
        /// <exception cref="Exception">Metodo para cachar cualquier error y poder mostrarlo al usuario</exception>
        public void ObtenTotalesPercepciones(int IdPeriodo, int IdEmpleado, decimal Sueldo, decimal SubsidioEmpleo, decimal ReintegroISR, decimal Vacaciones, int IdUnidadNegocio)
        {
            using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString))
            {
                try
                {
                    sqlconn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenTotalesPercepciones", sqlconn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("IdUnidadNegocio", SqlDbType.Int).Value = IdUnidadNegocio;
                        cmd.Parameters.Add("IdPeriodo", SqlDbType.Int).Value = IdPeriodo;
                        cmd.Parameters.Add("IdEmpleado", SqlDbType.Int).Value = IdEmpleado;
                        cmd.Parameters.Add("Sueldo", SqlDbType.Decimal).Value = Sueldo;
                        cmd.Parameters.Add("SubsidioEmpleo", SqlDbType.Decimal).Value = 0;
                        cmd.Parameters.Add("Vacaciones", SqlDbType.Decimal).Value = Vacaciones;

                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            TotalGravadoPercepciones = decimal.Parse(dr["Gravado"].ToString());
                            TotalExcentoPercepciones = decimal.Parse(dr["Excento"].ToString());
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    sqlconn.Close();
                }
            }

        }

        /// <summary>
        /// Metodo para obtener la antiguedad tomando en cuenta fecha de reconocimiento antiguedad y la fecha de baja
        /// </summary>
        /// <param name="FechaInicio">Fecha reconocimiento antiguedad</param>
        /// <param name="FechaFin">Fecha baja</param>
        /// <returns>Años de antiguedad</returns>
        public string ObtenAntiguedadAños(DateTime FechaInicio, DateTime FechaFin)
        {
            string Antiguedad = string.Empty;
            decimal dato = Math.Round((decimal)((FechaFin.Subtract(FechaInicio).TotalDays) / 365), 2);
            if (dato > 1)
                dato = Math.Truncate(dato);
            else
                dato = 1;

            Antiguedad = dato.ToString();

            return Antiguedad;
        }

        /// <summary>
        /// Metodo para obtener el ingreso acumulable
        /// </summary>
        /// <param name="IngGravado">Ingreso gravado</param>
        /// <param name="SMO"></param>
        /// <returns></returns>
        public decimal ObtenIngresoAcumulable(decimal IngGravado, decimal SMO)
        {
            decimal IngAcumulable = 0;
            if (IngGravado >= SMO)
                IngAcumulable = SMO;
            else
                IngAcumulable = IngGravado;

            return Math.Round(IngAcumulable, 2);
        }

        /// <summary>
        /// Metodo para regresar clave de tipo de contrato
        /// </summary>
        /// <param name="tipoContrato">Tipo de contrato</param>
        /// <returns>Clave del tipo de contrato</returns>
        public string GetTipoContrato(string tipoContrato)
        {
            string TipoContrato = string.Empty;

            if (tipoContrato.ToUpper().Equals("INDETERMINADO"))
                TipoContrato = "01";

            if (tipoContrato.ToUpper().Equals("DETERMINADO"))
                TipoContrato = "02";

            if (tipoContrato.ToUpper().Equals("SIN RELACION LABORAL"))
                TipoContrato = "09";

            return TipoContrato;
        }

        public List<vIncidencias_Consolidadas> GetVInsidencias_Consolidadas_Incapacidades(int IdPeriodo, int IdEmpleado)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.ClaveGpo == "501") select b).ToList();

                return incidencias;
            }
        }
    }
}