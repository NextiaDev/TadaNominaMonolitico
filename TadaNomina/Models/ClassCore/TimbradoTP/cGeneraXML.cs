using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ClassCore.PDF_CFDI;
using System.Web.UI.WebControls;
using TadaNomina.Services;
using Delva.AppCode.TimbradoTurboPAC;
using TadaNomina.Models.ClassCore.TimbradoTP.CFDI40;
using System.Net.NetworkInformation;
using System.IO;
using TadaNomina.Models.ClassCore.Timbrado;

namespace TadaNomina.Models.ClassCore.TimbradoTP
{
    public class cGeneraXML
    {
        SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);

        /// <summary>
        /// Metodo para generar los xml a timbrar por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="Id"></param>
        /// <param name="IdUsuario">Usuario</param>
        public string GeneraXMLTimbradoNomina(int IdPeriodo, int IdUnidadNegocio, int IdCliente, Guid Id, string tipoTimbrado, string[] Claves, int IdUsuario)
        {
            string result = string.Empty;
            var cunidad = new ClassUnidadesNegocio();
            var informacion = new List<DatosXML>();
            var timbrados = getAllTimbrados(IdPeriodo);

            switch (tipoTimbrado)
            {
                case "Timbrado CRC":
                    informacion = obtenDatosTimbrado(IdPeriodo).OrderBy(x => x.Receptor_Rfc).ToList();
                    break;
                case "Timbrado CR":                      
                    int?[] _timbrados = timbrados.Where(x=> x.IdEstatus == 1).Select(t => t.IdEmpleado).ToArray();
                    string[] _stimbrados = Array.ConvertAll(_timbrados, x=> x.Value.ToString());
                    if (timbrados.Count > 0)                    
                        informacion = obtenDatosTimbradoReTimbrar(IdPeriodo).Where(x => _stimbrados.Contains(x.IdEmpleado)).ToList();                   
                    else
                        throw new Exception("No se puede elegir la opción 'Timbrado con Relación y Cancelación' ya que no existen timbrados para este periodo.");
                    
                    break;
                case "Timbrado":
                    informacion = obtenDatosTimbrado(IdPeriodo).OrderBy(x => x.Receptor_Rfc).ToList();
                    break;
            }

            if (Claves.Count() > 0)
                informacion = informacion.Where(x => Claves.Contains(x.NumEmpleado)).ToList();

            if (informacion.Count <= 0)
                throw new Exception("No existen registros con los datos ingresados o ya fueron timbrados.");

            var unidad = cunidad.getUnidadesnegocioId(IdUnidadNegocio);

            foreach (var i in informacion)
            {
                int _idRegistro = 0;
                decimal _sdi = 0;

                var uuidRel = new List<string>();

                if (tipoTimbrado == "Timbrado CRC")
                    uuidRel = getUUIDTimbradobyIdEmpleadoyIdPeriodo(int.Parse(i.IdPeriodo), int.Parse(i.IdEmpleado), 2);

                if (tipoTimbrado == "Timbrado CR")
                    uuidRel = getUUIDTimbradobyIdEmpleadoyIdPeriodo(int.Parse(i.IdPeriodo), int.Parse(i.IdEmpleado), 1);

                var _IdEmpleado = int.Parse(i.IdEmpleado);
                try { _idRegistro = int.Parse(i.IdRegistroPatronal); } catch { _idRegistro = 0; }
                try { _sdi = decimal.Parse(i.SalarioDiarioIntegrado); } catch { _sdi = 0; }

                if (_sdi == 0) result += ("El empleado no tiene SDI. ref: " + i.IdEmpleado + " - " + i.NumEmpleado + " - " + i.Nombre + " | \n");
                if (_idRegistro == 0) result += ("El empleado no tiene Registro Patronal. ref: " + i.IdEmpleado + " - " + i.NumEmpleado + " - " + i.Nombre + " | \n");

                bool validacion = false;

                if (i.TipoContrato == "SIN RELACION LABORAL" && _sdi == 0)
                    validacion = true;

                if (i.TipoContrato != "SIN RELACION LABORAL" && _sdi > 0)
                    validacion = true;

                int IdEmpleado = int.Parse(i.IdEmpleado);
                var timbrado = timbrados.Where(x => x.IdEmpleado == IdEmpleado).ToList();

                if (_idRegistro != 0 && validacion && decimal.Parse(i.totalPercepciones) > 0)
                {
                    if (i.vercionCFDI == "3.3")
                        CrearXML33(i, IdUnidadNegocio, IdPeriodo, _IdEmpleado, _idRegistro, Id, unidad.FiniquitosFechasDiferentes, tipoTimbrado, IdUsuario);
                    else if (i.vercionCFDI == "4.0")                    
                        result += CrearXML40(i, IdUnidadNegocio, IdPeriodo, _IdEmpleado, _idRegistro, Id, unidad.FiniquitosFechasDiferentes, uuidRel, tipoTimbrado, timbrado, IdUsuario);                    
                    else
                        throw new Exception("No se ha especificado la version del cfdi.");
                }
            }

            return result;
        }

        /// <summary>
        /// Metodo que nos ayuda a generar el archivo xml version 3.3 para posteriormente ser timbrado
        /// </summary>
        /// <param name="dat">datos necesarios para crear el xml</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <param name="guid">identificador unico para dar seguimiento a estas solicitudes</param>
        /// <param name="tipoFechaFiniquito">tipo de fecha en caso de que sea un finiquito</param>
        /// <param name="IdUsuario">Identificador del usuario que esta ejecutando el proeceso</param>
        private void CrearXML33(DatosXML dat, int IdUnidadNegocio, int IdPeriodo, int IdEmpleado, int IdRegistro, Guid guid, string tipoFechaFiniquito, string UsoXML, int IdUsuario)
        {
            GeneraXML xml = new GeneraXML();
            string Comprobante = string.Empty;
            Comprobante = xml.GeneraXMLNomina12(dat, IdUnidadNegocio, tipoFechaFiniquito, IdPeriodo);

            if (Comprobante != string.Empty)
            {
                if (validaRegistro(IdPeriodo, IdEmpleado))
                    updateXmlDB(IdPeriodo, IdEmpleado, IdRegistro, Comprobante, dat.Leyenda, guid, UsoXML, IdUsuario);
                else
                    GuardarXmlDB(IdPeriodo, IdEmpleado, IdRegistro, Comprobante, dat.Leyenda, guid, UsoXML, IdUsuario);
            }
        }

        /// <summary>
        /// Metodo que nos ayuda a generar el archivo xml version 4.0 para posteriormente ser timbrado
        /// </summary>
        /// <param name="dat">datos necesarios para crear el xml</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <param name="guid">identificador unico para dar seguimiento a estas solicitudes</param>
        /// <param name="tipoFechaFiniquito">tipo de fecha en caso de que sea un finiquito</param>
        /// <param name="IdUsuario">Identificador del usuario que esta ejecutando el proeceso</param>
        private string CrearXML40(DatosXML dat, int IdUnidadNegocio, int IdPeriodo, int IdEmpleado, int IdRegistro, Guid guid, string tipoFechaFiniquito, List<string> UUIDRel, string UsoXML, List<vTimbradoNomina> timbrados, int IdUsuario)
        {
            string result = string.Empty;
            cCreaXML cxml = new cCreaXML();
            string Comprobante = string.Empty;
            try { Comprobante = cxml.GeneraXML40Nomina12(dat, IdUnidadNegocio, tipoFechaFiniquito, IdPeriodo, UUIDRel); } catch (Exception ex) { result += ex.Message + " | \n"; }
            
            if (Comprobante != string.Empty)
            {
                bool timbradoActivo = timbrados.Select(x => x.IdEstatus == 1).Any();
                if (validaRegistro(IdPeriodo, IdEmpleado) && (timbrados.Count > 0 || !timbradoActivo))                
                    updateXmlDB(IdPeriodo, IdEmpleado, IdRegistro, Comprobante, dat.Leyenda, guid, UsoXML, IdUsuario);                
                else                
                    GuardarXmlDB(IdPeriodo, IdEmpleado, IdRegistro, Comprobante, dat.Leyenda, guid, UsoXML, IdUsuario);                
            }

            return result;
        }

        /// <summary>
        /// Metodo que nos ayuda a identificar si un regustro ya fue insertado
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns></returns>
        private bool validaRegistro(int IdPeriodo, int IdEmpleado)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = entidad.XmlNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEmpleado == IdEmpleado && x.IdEstatus == 1).FirstOrDefault();

                if (registro != null)
                    return true;
                else
                    return false;
            }
        }

        public List<vTimbradoNomina> getTimbrados(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                return entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).ToList();
            }
        }

        public List<vTimbradoNomina> getAllTimbrados(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                return entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && (x.IdEstatus == 1 || x.IdEstatus == 2)).ToList();
            }
        }

        /// <summary>
        /// Metodo que nos ayuda a guardar la información del XML en la base de datos.
        /// </summary>
        /// <param name="IdPeriodo">>Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdRegistro">Identificador del registro patronal al que pertece el empleado</param>
        /// <param name="XML">Archivo XML creado</param>
        /// <param name="Leyenda">Leyenda que aparecera en la representación grafica del CFDI</param>
        /// <param name="IdUsuario">Identificador del usuario que realiza esta acción</param>
        private void GuardarXmlDB(int IdPeriodo, int IdEmpleado, int IdRegistro, string XML, string Leyenda, Guid Id, string UsoXML, int IdUsuario)
        {
            XmlNomina xml = new XmlNomina() {
                IdPeriodoNomina = IdPeriodo,
                IdEmpleado = IdEmpleado,
                IdRegistroPatronal = IdRegistro,
                XML = XML,
                Leyenda = Leyenda,
                IdEstatus = 1,
                IdCaptura = IdUsuario,
                FechaCaptura = DateTime.Now,
                Guid = Id,
                UsoXML = UsoXML,
            };

            using (TadaTimbradoEntities entidaad = new TadaTimbradoEntities())
            {
                entidaad.XmlNomina.Add(xml);
                entidaad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo que nos ayuda a actualizar la información del XML en la base de datos.
        /// </summary>
        /// <param name="IdPeriodo">>Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdRegistro">Identificador del registro patronal al que pertece el empleado</param>
        /// <param name="XML">Archivo XML creado</param>
        /// <param name="Leyenda">Leyenda que aparecera en la representación grafica del CFDI</param>
        /// <param name="IdUsuario">Identificador del usuario que realiza esta acción</param>
        private void updateXmlDB(int IdPeriodo, int IdEmpleado, int IdRegistro, string XML, string Leyenda, Guid Id, string UsoXML, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = entidad.XmlNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEmpleado == IdEmpleado && x.IdEstatus == 1).FirstOrDefault();

                if (registro != null)
                {
                    registro.IdPeriodoNomina = IdPeriodo;
                    registro.IdEmpleado = IdEmpleado;
                    registro.IdRegistroPatronal = IdRegistro;
                    registro.XML = XML;
                    registro.Leyenda = Leyenda;
                    registro.IdEstatus = 1;
                    registro.IdModifica = IdUsuario;
                    registro.FechaModifica = DateTime.Now;
                    registro.Guid = Id;
                    registro.UsoXML = UsoXML;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para obtener la información que se necesita para la creación de los XML
        /// </summary>
        /// <param name="IdPeriodoNomina"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DatosXML> obtenDatosTimbrado(int IdPeriodoNomina)
        {
            try
            {
                sqlconn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_InformacionXML_Nomina", sqlconn))
                {
                    cmd.CommandTimeout = 0;
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
                        dt.DifHoras = dr["DiferenciaHorasCP_E"].ToString();
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

        /// <summary>
        /// Metodo para obtener la información que se necesita para la creación de los XML
        /// </summary>
        /// <param name="IdPeriodoNomina"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DatosXML> obtenDatosTimbradoReTimbrar(int IdPeriodoNomina)
        {
            try
            {
                sqlconn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_InformacionXML_Nomina_Retimbrar", sqlconn))
                {
                    cmd.CommandTimeout = 0;
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
                        dt.DifHoras = dr["DiferenciaHorasCP_E"].ToString();
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
               
        public List<string> getUUIDTimbradobyIdEmpleadoyIdPeriodo(int IdPeriodoNomina, int IdEmpleado, int IdEstatus)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                return entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEmpleado == IdEmpleado && x.IdEstatus == IdEstatus)
                    .Select(x=> x.FolioUDDI).ToList();
            }
        }

        /// <summary>
        /// Metodo que nos ayuda a obtener todos los archivos generados y guardados en la DB
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns></returns>
        public List<XmlNomina> getRegistrosXMLPeriodo(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registros = entidad.XmlNomina.Where(x => x.IdEstatus == 1 && x.IdPeriodoNomina == IdPeriodo).ToList();

                return registros;
            }
        }

        public List<vXmlNomina> getRegistrosvXMLPeriodo(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registros = entidad.vXmlNomina.Where(x => x.IdEstatus == 1 && x.IdPeriodoNomina == IdPeriodo).ToList();

                return registros;
            }
        }

        /// <summary>
        /// Metodo que nos ayuda a limpiar(eliminar registros) la tabla de XML
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        public void deleteRegistrosXMl(int IdPeriodo, int IdUsuario) 
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {                
                var yaTimbrado = entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).Select(x=> x.IdXml).ToList();
                var registros = entidad.XmlNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && !yaTimbrado.Contains(x.IdXml)).ToList();

                foreach (var item in registros)
                {
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }
                
                entidad.SaveChanges();
            }
        }

        public void GetZip(int IdPeriodoNomina, int IdUnidad)
        {
            string ruta_CFDI_ZIP = @"D:\TadaNomina\DescargaCFDINominaPrevio";           
            var list = getRegistrosXMLPeriodo(IdPeriodoNomina);

            List<string> files = new List<string>();

            if (Directory.Exists(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip"))
                Directory.Delete(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip", true);

            if (list.Count > 0)
            {
                files = getFiles(list, string.Empty);
                CreateZipFile(files, ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip");
            }
        }

        public void GetZipXML(int IdPeriodoNomina, int IdUnidad)
        {
            string ruta_CFDI_ZIP = @"D:\TadaNomina\DescargaCFDINominaPrevio";
            var list = getRegistrosXMLPeriodo(IdPeriodoNomina);

            List<string> files = new List<string>();

            if (Directory.Exists(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip"))
                Directory.Delete(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip", true);

            if (list.Count > 0)
            {
                files = getFilesXML(list, string.Empty);
                CreateZipFile(files, ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip");
            }
        }

        public List<string> getFiles(List<XmlNomina> timbrado, string filtro)
        {
            List<string> files = new List<string>();
            
            files = GetPDFId(timbrado, filtro);
            
            return files;
        }

        public List<string> getFilesXML(List<XmlNomina> timbrado, string filtro)
        {
            List<string> files = new List<string>();

            files = GetXMLId(timbrado, filtro);

            return files;
        }

        public List<string> GetXMLId(List<XmlNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            foreach (var item in timbrado)
            {
                string xml = item.XML;
                string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "_" + item.IdEmpleado + ".xml";
                string ruta = @"D:\TadaNomina\DescargaCFDINominaPrevio\" + item.IdPeriodoNomina;
                if (filtro != string.Empty) { ruta += @"\" + filtro; }
                string rutaArchivo = ruta + @"\" + NombreArchivo;

                if (!Directory.Exists(ruta))
                    System.IO.Directory.CreateDirectory(ruta);

                CrearXML _xml = new CrearXML();
                _xml.crearXML(xml, rutaArchivo);
                lista.Add(rutaArchivo);
            }

            return lista;
        }

        public List<string> GetPDFId(List<XmlNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();

            foreach (var item in timbrado)
            {
                string xml = item.XML;
                string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "_" + item.IdXml + ".pdf";

                string ruta = @"D:\TadaNomina\DescargaCFDINominaPrevio\" + item.IdPeriodoNomina;
                if (filtro != string.Empty) { ruta += @"\" + filtro; }
                string rutaArchivo = ruta + @"\" + NombreArchivo;

                if (!Directory.Exists(ruta))
                    System.IO.Directory.CreateDirectory(ruta);

                WS_CFDI cga = new WS_CFDI();
                cga.guardaPDF(item.XML, item.Leyenda, rutaArchivo, null, null);
                lista.Add(rutaArchivo);
            }

            return lista;
        }

        public void CreateZipFile(List<string> items, string destination)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                foreach (string item in items)
                {
                    if (System.IO.File.Exists(item))
                    {
                        zip.AddFile(item, "");
                    }
                    else if (System.IO.Directory.Exists(item))
                    {
                        zip.AddDirectory(item, new System.IO.DirectoryInfo(item).Name);
                    }
                }
                zip.Save(destination);
            }
        }
    }
}
