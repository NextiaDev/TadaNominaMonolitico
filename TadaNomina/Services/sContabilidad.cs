using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TadaNomina.Models.ViewModels.Contabilidad;

namespace TadaNomina.Services
{
    public class sContabilidad
    {
        /// <summary>
        ///     Método que obtiene las cuentas de un cliente
        /// </summary>
        /// <param name="_IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="_IdNivel">Variable que contiene el id del nivel</param>
        /// <param name="IdReferencia">Variable que contiene el id de la referencia</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información de las cuentas</returns>
        public List<CuentasModel> GetCuentas(int _IdCliente, int _IdNivel, int? IdReferencia, string Token)
        {
            string cadenaUrl = sStatics.ServidorContabilidad + "api/CuentasContables/getCuentas?IdCliente=" + _IdCliente + "&IdNivel=" + _IdNivel;
            if (IdReferencia != null)
                cadenaUrl += "&IdReferencia=" + IdReferencia;

            Uri url = new Uri(cadenaUrl);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;
            var result = wc.DownloadString(url);

            var datos = JsonConvert.DeserializeObject<List<CuentasModel>>(result);

            return datos;
        }

        /// <summary>
        ///     Método que obtiene una cuenta
        /// </summary>
        /// <param name="IdCuenta">Variable que contiene el id de la cuenta</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Modelo con la información de una cuenta</returns>
        public CuentasModel GetCuenta(int IdCuenta, string Token)
        {

            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/getCuenta?IdCuenta=" + IdCuenta);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;
            var result = wc.DownloadString(url);

            var datos = JsonConvert.DeserializeObject<CuentasModel>(result);

            return datos;
        }

        /// <summary>
        ///     Método que obtiene los tipos de cuentas
        /// </summary>
        /// <param name="Token">Variable que contiene un JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información de las cuentas</returns>
        public List<ModelTipoCuenta> GetTipoCuenta(string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/getTipoCuenta");

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.DownloadString(url);

            var datos = JsonConvert.DeserializeObject<List<ModelTipoCuenta>>(result);

            return datos;
        }

        /// <summary>
        ///     Método que obtiene los tipos de nómina
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de modelos con la información de los tipos de nómina</returns>
        public List<Cat_TipoNominaModel> GetTipoNomina(string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/getTipoNomina");

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.DownloadString(url);

            var datos = JsonConvert.DeserializeObject<List<Cat_TipoNominaModel>>(result);

            return datos;
        }

        /// <summary>
        ///     Método que obtiene 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public List<Cat_NominaModel> GetTipoCatNomina(string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/getTipoCatNomina");

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.DownloadString(url);

            var datos = JsonConvert.DeserializeObject<List<Cat_NominaModel>>(result);

            return datos;
        }

        /// <summary>
        ///     Método que elimina el registro de una cuenta
        /// </summary>
        /// <param name="_IdCuenta">Variable que contiene el id de la cuenta</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string removeCuentas(int _IdCuenta, string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/removeCuentas?IdCuenta=" + _IdCuenta);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, "");
            
            return result;
        }

        /// <summary>
        ///     Método que obtiene el tipo de conceptos
        /// </summary>
        /// <param name="Token">Variable que contieneel JWT para consumir una API</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <returns>Lista de modelos con la información de los conceptos</returns>
        public List<ModelCat_Conceptos> GetTipoCatConceptos(string Token, int IdCliente)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/getConceptos?IdCliente=" + IdCliente);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.DownloadString(url);

            var datos = JsonConvert.DeserializeObject<List<ModelCat_Conceptos>>(result);

            return datos;
        }

        /// <summary>
        ///     Método que agrega una nueva cuenta contable
        /// </summary>
        /// <param name="Token">Variable que contiene un JWT para consumir la API</param>
        /// <param name="IdRefencia">Variable que contiene el id de referencia</param>
        /// <param name="_IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdRegistroPatronal">Variable que contieneid del registro patronal</param>
        /// <param name="Nivel">Variable que contiene el nivel</param>
        /// <param name="IdCuentaCliente">Variable que contiene id de la cuenta del cliente</param>
        /// <param name="_Clave">Variable que contiene la clave de la cuenta</param>
        /// <param name="_descripcion">Variable que contiene la descripción</param>
        /// <param name="_IdTipoNomina">Variable que contiene el id del tipo de nómina</param>
        /// <param name="_Concepto">Variable que contiene el concepto</param>
        /// <param name="IdTipoCuenta">Variable que contiene el tipo de cuenta</param>
        /// <returns>Respuesta del movimiento</returns>
        public string AddCuentaContable(string Token, int? IdRefencia, int _IdCliente, int? IdRegistroPatronal, int Nivel, int? IdCuentaCliente, string _Clave, string _descripcion, int? _IdTipoNomina, string _Concepto, int? IdTipoCuenta)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/addCuentas");

                var data = new cAgregarCuentaContable()
                {
                    idReferencia = IdRefencia,
                    idCliente = _IdCliente,
                    Nivel = Nivel,
                    IdCuentaCliente = IdCuentaCliente,
                    clave = _Clave.Trim(),
                    descripcion = _descripcion.Trim().Replace("\n", ""),
                    idTipoNomina = _IdTipoNomina,
                    concepto = _Concepto,
                    IdTipoCuenta = IdTipoCuenta,
                    IdRegistroPatronal = IdRegistroPatronal
                };

                var _data = JsonConvert.SerializeObject(data);

                var wc = new WebClient();
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;

                var result = wc.UploadString(url, _data);

                return result;
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }            
        }

        /// <summary>
        ///     Método que edita una cuenta contable
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="IdCuenta">Variable que contiene id de la cuenta</param>
        /// <param name="_Clave">Variable que contiene la clave de la cuenta</param>
        /// <param name="_descripcion">Variable que contiene la descripción</param>
        /// <param name="_IdTipoNomina">Variable que contiene el id del tipo de nómina</param>
        /// <param name="_Concepto">Variable que contiene el concepto</param>
        /// <param name="IdTipoCuenta">Variable que contiene el tipo de cuenta</param>
        /// <returns>Respuesta del movimiento</returns>
        public string editCuentaContable(string Token, int IdCuenta, string _Clave, string _descripcion, int? _IdTipoNomina, string _Concepto, int? IdTipoCuenta)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/editCuentas?IdCuenta=" + IdCuenta);

            var data = new cAgregarCuentaContable()
            {                
                clave = _Clave, //Clave contable
                descripcion = _descripcion,
                idTipoNomina = _IdTipoNomina,
                concepto = _Concepto,
                IdTipoCuenta = IdTipoCuenta
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que obtiene un reporte 
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <param name="CuentaFiltro">Variable que contiene el filtro de la cuenta</param>
        /// <returns>Lista de modelos con la información de los reportes</returns>
        public List<mReporteCuentas> getReporte(string Token, int IdCliente, int IdPeriodoNomina, int CuentaFiltro)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CuentasContables/getReporte?IdCliente=" + IdCliente + "&IdPeriodoNomina=" + IdPeriodoNomina + "&CuentaFiltro=" + CuentaFiltro);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.DownloadString(url);
            var dataResult = JsonConvert.DeserializeObject<List<mReporteCuentas>>(result);

            return dataResult;
        }

        /// <summary>
        ///     Método que obtiene 
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una api</param>
        /// <param name="FechaInicio">Variable que contiene la fecha de inicio</param>
        /// <param name="FechaFin">Variable que contiene la fecha en que finaliza</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <param name="IdRegistroPatronal">Variable que contiene el id del registro patronal</param>
        /// <returns></returns>
        public List<mReporteCuentasWS> getReporteWS(string Token, string FechaInicio, string FechaFin, int IdPeriodoNomina, int? IdRegistroPatronal)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/ObtenerCuenta/getPolizaIndividualWS");

                var datos = new mGetReporte()
                {
                    FechaInicio = FechaInicio,
                    FechaFin = FechaFin,
                    IdPeriodoNomina = IdPeriodoNomina,
                    IdRegistroPatronal = IdRegistroPatronal
                };

                var _datos = JsonConvert.SerializeObject(datos);

                var wc = new MyWebClient();
                
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;

                var result = wc.UploadString(url, _datos);
                var dataResult = JsonConvert.DeserializeObject<List<mReporteCuentasWS>>(result);

                return dataResult;
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }
            
        }

        /// <summary>
        ///     Método que actualiza
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="FechaInicio">Variable que contiene la fecha de inicio</param>
        /// <param name="FechaFin">Variable que contiene la fecha final</param>
        /// <param name="IdRegistroPatronal">Variable que contiene el id del registro patronal</param>
        public void actualizaInfoWS(string Token, string FechaInicio, string FechaFin, int? IdRegistroPatronal)
        {
            try
            {
                Uri url = new Uri(sStatics.ServidorContabilidad + "api/ObtenerCuenta/actualizaInfoPoliza");

                var datos = new mGetReporte()
                {
                    FechaInicio = FechaInicio,
                    FechaFin = FechaFin,
                    IdRegistroPatronal = IdRegistroPatronal
                };

                var _datos = JsonConvert.SerializeObject(datos);

                var wc = new WebClient();
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;

                var result = wc.UploadString(url, _datos);                
            }
            catch (WebException ex)
            {
                throw sException.GetException(ex);
            }

        }
    }    
}