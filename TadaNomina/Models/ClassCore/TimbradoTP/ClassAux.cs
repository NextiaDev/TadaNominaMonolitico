using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Sat.Utils;
using CryptoFirma;
using Core.Sat;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using TadaNomina.Models.DB;

namespace Delva.AppCode.TimbradoTurboPAC
{
    public class ClassAux
    {
        public ClassTipos tipos = new ClassTipos();

        //public List<Cat_ConceptosNomina> conceptos;
        public decimal TotalOtrasDeducciones { get; set; }
        public decimal TotalImpuestosRetenidos { get; set; }
        public string XMLTotalDeducciones { get; set; }
        public decimal TotalGravadoPercepciones { get; set; }
        public decimal TotalExcentoPercepciones { get; set; }
        public string XMLTotalPercepciones { get; set; }
        public string _KeyPass;
        public string _cer;
        public string _key;
        public string _noCert;
        public FirmaSoftware256 _firmaSoftware256;

        public void ObtenCertificadosCfdi(string rutaCer, string rutaKey, string Keypass)
        {
            _cer = rutaCer;
            _key = rutaKey;
            _KeyPass = Keypass;

            LoadCerts();
        }

        private void LoadCerts()
        {
            try
            {
                _firmaSoftware256 = new FirmaSoftware256(_cer, File.ReadAllBytes(_key), _KeyPass);
                _noCert = CertUtilsSat.NoCertificado(_firmaSoftware256.Cert.GetSerialNumberString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<vIncidencias_Consolidadas> GetVInsidencias_Consolidadas(int IdPeriodo, int IdEmpleado)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.Monto > 0) select b).ToList();

                return incidencias;
            }
        }

        public List<vIncidencias_Consolidadas> GetVInsidencias_Consolidadas_Incapacidades(int IdPeriodo, int IdEmpleado)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var incidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.ClaveGpo == "501") select b).ToList();

                return incidencias;
            }
        }

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
                        NominaPercepcionesPercepcion pVariable = new NominaPercepcionesPercepcion();
                        pVariable.TipoPercepcion = tipos.ObtenTipoPercepcion(claveSAT);
                        pVariable.Clave = item.ClaveConcepto;
                        pVariable.Concepto = item.Concepto;
                        pVariable.ImporteExento = (decimal)item.Exento;
                        pVariable.ImporteGravado = (decimal)item.Gravado;
                        
                        if (claveSAT.Equals("019"))                            
                            pVariable.HorasExtra = ObtenHorasExtra(item, incidencias);
                    
                        listaConceptos.Add(pVariable);
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

        public decimal ObtenCantidadHorasExtra(int IdEmpleado, int IdPeriodo, string ClaveConcepto, List<vIncidencias_Consolidadas> incidencias)
        {
            decimal Cantidad = 1;
            var cantidad = (from b in incidencias.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.ClaveConcepto == ClaveConcepto && x.IdEstatus == 1) select b).Sum(x => x.Cantidad);

            if (cantidad != 0)
                Cantidad = (decimal)cantidad;

            return Cantidad;
        }

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

        public decimal ObtenIngresoAcumulable(decimal IngGravado, decimal SMO)
        {
            decimal IngAcumulable = 0;
            if (IngGravado >= SMO)
                IngAcumulable = SMO;
            else
                IngAcumulable = IngGravado;

            return Math.Round(IngAcumulable, 2);
        }

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
    }
}