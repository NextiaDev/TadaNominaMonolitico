using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.ServiceReferenceTP;

namespace TadaNomina.Models.ClassCore.TimbradoTP
{
    public class ClassProcesosTimbradoTP
    {
        SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);
        public string Token { get; set; }
        public string URI { get; set; }
        public List<sp_InformacionXML_Nomina1_Result> GetInformacionXML(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var datos = (from b in entidad.sp_InformacionXML_Nomina1(IdPeriodo) select b).ToList();

                return datos;
            }
        }

        public void Get_Token(string Tipo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var cat_token = (from b in entidad.Cat_Token_MasNegocio.Where(x => x.IdEstatus == 1 && x.Tipo == Tipo) select b).FirstOrDefault();
                Token = cat_token.Token;
                URI = cat_token.URI;
            }
        }

        public void GuardaTablaTimbrado(vXmlNomina dat, RespuestaTimbrado respuesta, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                TimbradoNomina tim = new TimbradoNomina();
                tim.IdPeriodoNomina = dat.IdPeriodoNomina;
                tim.IdEmpleado = dat.IdEmpleado;
                tim.IdRegistroPatronal = dat.IdRegistroPatronal;
                tim.RegistroPatronal = dat.RegistroPatronal;
                tim.NombrePatrona = dat.NombrePatrona;
                tim.RFC = dat.Rfc; 
                tim.FolioUDDI = respuesta.Uuid;
                tim.Mensaje = "Comprobante timbrado exitosamente";
                tim.IdEstatus = 1;
                tim.IdCaptura = IdUsuario;
                tim.FechaInicioPeriodo = dat.FechaInicio;
                tim.FechaFinPeriodo = dat.FechaFin;
                tim.FechaCaptura = DateTime.Now;
                tim.CFDI_Timbrado = respuesta.Cfdi;
                tim.Leyenda = dat.Leyenda;
                tim.IdXml = dat.IdXml;

                entidad.TimbradoNomina.Add(tim);
                entidad.SaveChanges();
            }
        }

        public void GuardaLogError(vXmlNomina dat, RespuestaTimbrado respuesta, Guid guid, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores error = new LogErrores();
                error.Guid = guid;
                error.IdPeriodoNomina = dat.IdPeriodoNomina;
                error.Modulo = "Timbrado";
                error.Referencia = dat.Rfc;
                error.Descripcion = "Errores: " + string.Join("", respuesta.DescripcionError);
                error.Fecha = DateTime.Now;
                error.IdUsuario = IdUsuario;
                error.IdEstatus = 1;

                entidad.LogErrores.Add(error);
                entidad.SaveChanges();
            }
        }

        public List<DatosXML> obtenDatosTimbrado(int IdPeriodoNomina)
        {
            try
            {
                sqlconn.Open(); 

                using (SqlCommand cmd = new SqlCommand("sp_InformacionXML_Nomina", sqlconn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("IdPeriodoNomina", SqlDbType.Int).Value = IdPeriodoNomina;
                    SqlDataReader dr = cmd.ExecuteReader();
                    List<DatosXML> datos = new List<DatosXML>();

                    while (dr.Read())
                    {
                        DatosXML dt = new DatosXML();
                        dt.IdEmpleado = dr["IdEmpleado"].ToString();
                        dt.IdRegistroPatronal = dr["IdRegistroPatronal"].ToString();
                        dt.Nombre = dr["Nombre"].ToString();
                        dt.noCertificado = dr["SelloDigital"].ToString();
                        dt.CURP = dr["CURP"].ToString();
                        dt.total = dr["SueldoPagado"].ToString();
                        dt.totalImpuestosRetenidos = dr["ISR"].ToString();
                        dt.retencionesImporte = dr["ISR"].ToString();
                        dt.SalarioBaseCotApor = dr["SueldoDiario"].ToString();
                        dt.SalarioDiarioIntegrado = dr["SDI"].ToString();
                        dt.PeriodicidadPago = dr["TipoNomina"].ToString();
                        dt.FechaInicioRelLaboral = dr["FechaAltaDelva"].ToString();
                        dt.Puesto = dr["Puesto"].ToString();
                        dt.Departamento = dr["Departamento"].ToString();
                        dt.NumDiasPagados = dr["DiasTrabajados"].ToString();
                        dt.NumSeguridadSocial = dr["Imss"].ToString();
                        dt.NumEmpleado = dr["ClaveEmpleado"].ToString();
                        dt.RegistroPatronal = dr["RegistroPatronal"].ToString();
                        dt.Emisor_Rfc = dr["RFC_P"].ToString().ToUpper();
                        dt.Emisor_Nombre = dr["NombrePatrona"].ToString();
                        dt.codigoPostalEmisor = dr["CP_E"].ToString();
                        dt.paisEmisor = dr["Pais_E"].ToString();
                        dt.estadoEmisor = dr["Entidad_E"].ToString();
                        dt.municipioEmisor = dr["Municipio_E"].ToString();
                        dt.calleEmisor = dr["Calle_E"].ToString();
                        dt.Receptor_Nombre = dr["Nombre"].ToString();
                        dt.Receptor_Rfc = dr["Rfc"].ToString().ToUpper();                        
                        dt.Subsidio = dr["Subsidio"].ToString();
                        dt.SubsidioPagar = dr["SubsidioPagar"].ToString();                        
                        dt.totalPercepciones = dr["ER"].ToString();
                        dt.ISPT = dr["ImpuestoRetener"].ToString();
                        dt.IMSS = dr["IMSS_Obrero"].ToString();
                        dt.IdPeriodo = dr["IdPeriodoNomina"].ToString();
                        dt.DiaFestivo = dr["DiaFestivo"].ToString();
                        dt.FechaInicio = dr["FechaInicio"].ToString();
                        dt.FechaFin = dr["FechaFin"].ToString();                        
                        dt.Vacaciones = dr["Sueldo_Vacaciones"].ToString();
                        dt.DiasVacaciones = dr["Dias_Vacaciones"].ToString();                        
                        dt.TipoContrato = dr["TipoContrato"].ToString();
                        dt.Esquema = dr["Esquema"].ToString();
                        dt.ClaveEntFed = dr["ClaveEntidad"].ToString();
                        dt.Clase = dr["Clase"].ToString();
                        dt.Antiguedad = dr["Antiguedad"].ToString();
                        dt.FechaReconocimientoAntiguedad = dr["FechaReconocimientoAntiguedad"].ToString();
                        dt.Neto = dr["Neto"].ToString();
                        dt.ClaveBanco = dr["ClaveBanco"].ToString();
                        dt.CuentaBancaria = dr["CuentaBancariaTrad"].ToString();
                        dt.CuentaInterbancariaTrad = dr["cuentainterbancariatrad"].ToString();
                        dt.Leyenda = dr["Leyenda"].ToString();                        
                        dt.SueldoMensual = dr["SueldoMensualEmp"].ToString();
                        dt.RFCSubcontratacion = dr["RFCSubContratacion"].ToString();
                        dt.rutaCer = dr["rutaCer"].ToString();
                        dt.rutaKey = dr["rutaKey"].ToString();
                        dt.keyPass = dr["KeyPass"].ToString();
                        dt._TipoNomina = dr["TipoCalculo"].ToString();
                        dt.Liquidacion_Gravado = dr["LIQUIDACION_GRAVADO"].ToString();
                        dt.Liquidacion_Exento = dr["LIQUIDACION_EXENTO"].ToString();
                        dt.FechaBaja = dr["FechaBaja"].ToString();
                        dt.SueldosPagados = dr["Dias_Pagados"].ToString();
                        dt.FechaDispersion = dr["FechaDispersion"].ToString();
                        dt.ReintegroISR = dr["ReintegroISR"].ToString();
                        dt.FechaBajaEmpleado = dr["FechaBajaEmpleado"].ToString();
                        dt.PersonaFisica = dr["PersonaFisica"].ToString();
                        dt.CurpPersonaFisica = dr["CurpPersonaFisica"].ToString();
                        dt.vercionCFDI = dr["VersionCFDI"].ToString();
                        dt.CodigoPostalEmpleado = dr["CodigoPostalEmpleado"].ToString();
                        datos.Add(dt);
                    }
                    return datos;
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

        public ModelDetalleTimbradoInicio getDetalleInicio(int IdUnidadNegocio)
        {
            string consulta = @"select a.*, isnull(b.Timbrados, 0) as Timbrados, a.nomina-isnull(b.Timbrados, 0) as NoTimbrados
                                from(
                                select UnidadNegocio, count(*) as Nomina
                                from Nomina a
                                inner join PeriodoNomina b
                                on a.IdPeriodoNomina= b.IdPeriodoNomina
                                inner join vUnidadesNegocio c
                                on b.IdUnidadNegocio=c.IdUnidadNegocio
                                where SDI>0 and a.IdEstatus=1 and idregistropatronal is not null and idregistropatronal >0 and er>0
                                and a.IdPeriodoNomina in(select IdPeriodoNomina from PeriodoNomina where TipoNomina in('Nomina','Bonos','Aguinaldo','Finiquitos') and IdEstatus=2 and IdUnidadNegocio=" + IdUnidadNegocio + @")
                                and b.FechaFin>='" + DateTime.Now.Year + "0101" + @"' 
                                group by UnidadNegocio) a
                                left join (
                                select c.UnidadNegocio, count(*) as Timbrados
                                from TimbradoNomina a
                                inner join PeriodoNomina b
                                on a.IdPeriodoNomina= b.IdPeriodoNomina
                                inner join Cat_UnidadNegocio c
                                on b.IdUnidadNegocio=c.IdUnidadNegocio
                                where a.IdEstatus=1
                                and a.IdPeriodoNomina in(select IdPeriodoNomina from PeriodoNomina where TipoNomina in('Nomina','Bonos','Aguinaldo','Finiquitos') and IdEstatus=2 and IdUnidadNegocio=" + IdUnidadNegocio + @")
                                and b.FechaFin>='" + DateTime.Now.Year + "0101" + @"' 
                                group by c.UnidadNegocio)b
                                on a.UnidadNegocio=b.UnidadNegocio";

            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                ModelDetalleTimbradoInicio mdi = new ModelDetalleTimbradoInicio();
                mdi.Timbrados = 0;
                mdi.NoTimbrados = 0;

                var result = entidad.Database.SqlQuery<ModelDetalleTimbradoInicio>(consulta).FirstOrDefault();

                if (result != null)
                    return result;

                return mdi;
            }
        }
    }
}