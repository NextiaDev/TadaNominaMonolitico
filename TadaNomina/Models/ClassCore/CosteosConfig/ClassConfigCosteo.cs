using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.CosteosConfig
{
    public class ClassConfigCosteo
    {

        /// <summary>
        ///     Método que genera la lista de conceptos
        /// </summary>
        /// <returns>Modelo con dos listas de conceptos</returns>
        public CosteoConceptosM FindLisConceptos()
        {
            List<SelectListItem> _Visible = new List<SelectListItem>();
            _Visible.Add(new SelectListItem { Text = "SI", Value = "SI" });
            _Visible.Add(new SelectListItem { Text = "NO", Value = "NO" });


            List<SelectListItem> _TipoFac = new List<SelectListItem>();
            _TipoFac.Add(new SelectListItem { Text = "TOTAL PERCEPCIONES", Value = "TOTAL_PERCEPCIONES" });
            _TipoFac.Add(new SelectListItem { Text = "TOTAL PERCEPCIONES_ESQ", Value = "TOTAL_PERCEPCIONES_ESQ" });
            _TipoFac.Add(new SelectListItem { Text = "CARGA SOCIAL", Value = "CARGA_SOCIAL" });
            _TipoFac.Add(new SelectListItem { Text = "ISN", Value = "ISN" });
            _TipoFac.Add(new SelectListItem { Text = "HONORARIO", Value = "HONORARIO" });
            _TipoFac.Add(new SelectListItem { Text = "HONORARIO MAQUILA", Value = "HONORARIO_MAQUILA" });
            _TipoFac.Add(new SelectListItem { Text = "SUBTOTAL", Value = "SUBTOTAL" });
            _TipoFac.Add(new SelectListItem { Text = "IVA", Value = "IVA" });
            _TipoFac.Add(new SelectListItem { Text = "TOTAL", Value = "TOTAL" });
            _TipoFac.Add(new SelectListItem { Text = "OTROS", Value = "OTROS" });

            CosteoConceptosM model = new CosteoConceptosM
            {
                lVisible = _Visible,
                Lfacturacion = _TipoFac,

            };

            return model;
        }

        /// <summary>
        ///     Método que genera cinco listas de conceptos para carga de información
        /// </summary>
        /// <returns>Modelo con las listas de los conceptos</returns>
        public CosteosModel FindListPeriodos()
        {
            List<SelectListItem> _TipoNomima = new List<SelectListItem>();
            _TipoNomima.Add(new SelectListItem { Text = "Nomina", Value = "Nomina" });
            _TipoNomima.Add(new SelectListItem { Text = "Finiquitos", Value = "Finiquitos" });
            _TipoNomima.Add(new SelectListItem { Text = "Ambos", Value = "Ambos" });

            List<SelectListItem> _TipoEsquema = new List<SelectListItem>();
            _TipoEsquema.Add(new SelectListItem { Text = "TRADICIONAL", Value = "TRADICIONAL" });
            _TipoEsquema.Add(new SelectListItem { Text = "ESQUEMA", Value = "ESQUEMA" });
            _TipoEsquema.Add(new SelectListItem { Text = "AMBOS", Value = "AMBOS" });

            List<SelectListItem> _RegistroP = new List<SelectListItem>();
            _RegistroP.Add(new SelectListItem { Text = "SI", Value = "SI" });
            _RegistroP.Add(new SelectListItem { Text = "NO", Value = "NO" });

            List<SelectListItem> _ObtenerCosteo = new List<SelectListItem>();
            _ObtenerCosteo.Add(new SelectListItem { Text = "GENERAL", Value = "GENERAL" });
            _ObtenerCosteo.Add(new SelectListItem { Text = "CENTRO DE COSTOS", Value = "CENTRO DE COSTOS" });
            _ObtenerCosteo.Add(new SelectListItem { Text = "DEPARTAMENTOS", Value = "DEPARTAMENTOS" });
            _ObtenerCosteo.Add(new SelectListItem { Text = "PUESTOS", Value = "PUESTOS" });
            _ObtenerCosteo.Add(new SelectListItem { Text = "ESQUEMA DE PAGO", Value = "ESQUEMAPAGO" });

            List<SelectListItem> _CostearporDesc = new List<SelectListItem>();
            _CostearporDesc.Add(new SelectListItem { Text = "SI", Value = "1" });
            _CostearporDesc.Add(new SelectListItem { Text = "NO", Value = "0" });



            CosteosModel model = new CosteosModel
            {
                CostearporDesc = _CostearporDesc,
                ObtenerCosteo = _ObtenerCosteo,
                RegistroP = _RegistroP,
                LEsquema = _TipoEsquema,
                LTipoNomina = _TipoNomima
            };

            return model;
        }

        /// <summary>
        ///     Método que genera las listas de conceptos para la configuración de un empleado
        /// </summary>
        /// <returns>Modelo con las listas</returns>
        public ConfiguraConceptosM FindLisConceptosConfigur()
        {
            List<SelectListItem> Tipo = new List<SelectListItem>();
            Tipo.Add(new SelectListItem { Text = "CUOTA FIJA", Value = "CUOTA FIJA" });
            Tipo.Add(new SelectListItem { Text = "EMPLEADO", Value = "EMPLEADO" });
            Tipo.Add(new SelectListItem { Text = "INCIDENCIAS", Value = "INCIDENCIAS" });
            Tipo.Add(new SelectListItem { Text = "NOMINA", Value = "NOMINA" });
            Tipo.Add(new SelectListItem { Text = "YA CONFIGURADO", Value = "YA CONFIGURADO" });
            Tipo.Add(new SelectListItem { Text = "OTRO", Value = "OTRO" });


            List<SelectListItem> oper = new List<SelectListItem>();
            oper.Add(new SelectListItem { Text = "+", Value = "+" });
            oper.Add(new SelectListItem { Text = "-", Value = "-" });
            oper.Add(new SelectListItem { Text = "*", Value = "*" });
            oper.Add(new SelectListItem { Text = "/", Value = "/" });


            List<SelectListItem> opergene = new List<SelectListItem>();
            opergene.Add(new SelectListItem { Text = "+", Value = "+" });
            opergene.Add(new SelectListItem { Text = "-", Value = "-" });
            opergene.Add(new SelectListItem { Text = "*", Value = "*" });
            opergene.Add(new SelectListItem { Text = "/", Value = "/" });
            ConfiguraConceptosM model = new ConfiguraConceptosM
            {
                lTipoConcepto = Tipo,
                lOperador = oper,
                loperadorGral = opergene

            };

            return model;
        }

        /// <summary>
        ///     Método que obtiene una lista de costeos en una unidad de negocio
        /// </summary>
        /// <param name="_UnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns></returns>
        public List<CosteoModel> GetListCosteos(int _UnidadNegocio, string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costeos/getCosteos?IdUndadNegocio=" + _UnidadNegocio);

            using (var wc = new WebClient())
            {
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;
                var result = wc.DownloadString(url);

                var datos = JsonConvert.DeserializeObject<List<CosteoModel>>(result);

                return datos;
            }
        }

        /// <summary>
        ///     Método que obtiene la información de un costeo
        /// </summary>
        /// <param name="idCosteo">Variable que contiene el id del costeo a consultar</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns></returns>
        public CosteoModel GetListCosteo(int idCosteo, string Token)
        {

            Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costeos/getCosteo?IdCosteo=" + idCosteo);

            using (var wc = new WebClient())
            {
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;
                var result = wc.DownloadString(url);

                var datos = JsonConvert.DeserializeObject<CosteoModel>(result);

                return datos;
            }
        }

        /// <summary>
        ///     Método que agrega la configuración de un costeo
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="_IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="idunidadnegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="descripcion">Variable que contiene la descripción del costeo nuevo</param>
        /// <param name="tipoNomina">Variable que contiene el tipo de nómina</param>
        /// <param name="tipoEsquema">Variable que contiene el tipo de esquema</param>
        /// <param name="dividirPatronal">Variable que contiene </param>
        /// <param name="separadoPor">Variable que contiene </param>
        /// <param name="agruparPorDescripcion">Variable que contiene </param>
        /// <returns>Respuesta de la alta de la configuración del costeo</returns>
        public string AddConfiguracionCosteo(string Token, int _IdCliente, int idunidadnegocio, string descripcion, string tipoNomina, string tipoEsquema, string dividirPatronal, string separadoPor, int agruparPorDescripcion)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costeos/newCosteo");

            var data = new CosteosModel()
            {

                idCliente = _IdCliente,
                idUnidadNegocio = idunidadnegocio,
                descripcion = descripcion,
                tipoNomina = tipoNomina,
                tipoEsquema = tipoEsquema,
                dividirPatronal = dividirPatronal,
                separadoPor = separadoPor,
                agruparPorDescripcion = agruparPorDescripcion
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que edita un costeo
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="Idcosteo">Variable que contiene el id del costeo</param>
        /// <param name="_IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="idunidadnegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="descripcion">Variable que contiene la descripción del costeo nuevo</param>
        /// <param name="tipoNomina">Variable que contiene el tipo de nómina</param>
        /// <param name="tipoEsquema">Variable que contiene el tipo de esquema</param>
        /// <param name="dividirPatronal">Variable que contiene </param>
        /// <param name="separadoPor">Variable que contiene </param>
        /// <param name="agruparPorDescripcion">Variable que contiene </param>
        /// <returns>Respuesta de la alta de la configuración del costeo</returns>
        public string Editar(string Token, int Idcosteo, int _IdCliente, int idunidadnegocio, string descripcion, string tipoNomina, string tipoEsquema, string dividirPatronal, string separadoPor, int agruparPorDescripcion)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costeos/updateCosteo?Idcosteo=" + Idcosteo);

            var data = new CosteoModel()
            {

                idCliente = _IdCliente,
                idUnidadNegocio = idunidadnegocio,
                descripcion = descripcion,
                tipoNomina = tipoNomina,
                tipoEsquema = tipoEsquema,
                dividirPatronal = dividirPatronal,
                separadoPor = separadoPor,
                agruparPorDescripcion = agruparPorDescripcion
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que borra el registro del costeo
        /// </summary>
        /// <param name="IdCosteo">Variable que contiene el id del costeo</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta de la alta de la configuración del costeo</returns>
        public string removeCosteo(int IdCosteo, string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/Costeos/deleteCosteo?IdCosteo=" + IdCosteo);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, "");

            return result;
        }

        //Conceptos
        /// <summary>
        ///     Método que obtiene una lista de costeos
        /// </summary>
        /// <param name="IdCosteo">Variable que contiene el id del costeo</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de un modelo con los datos de los conceptos</returns>
        public List<CosteoConceptos> GetListConceptos(int IdCosteo, string Token)
        {

            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptos/getCosteosConceptos?IdCosteo=" + IdCosteo);

            using (var wc = new WebClient())
            {
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;
                var result = wc.DownloadString(url);

                var datos = JsonConvert.DeserializeObject<List<CosteoConceptos>>(result);

                return datos;
            }
        }

        /// <summary>
        ///     Método que agrega un concepto
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="idCosteo">Variable que contiene el id del costeo</param>
        /// <param name="descripcion">Variable que contiene la descripción del costeo</param>
        /// <param name="tipoDatoFacturacion">Variable que contiene </param>
        /// <param name="observaciones">Variable que contiene las observaciones</param>
        /// <param name="orden">Variable que contiene </param>
        /// <param name="visible">Variable que contiene </param>1
        /// <returns>Respuesta del alta de la información</returns>
        public string AddConceptos(string Token, int idCosteo, string descripcion, string tipoDatoFacturacion, string observaciones, int orden, string visible)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptos/newCosteoConcepto");

            var data = new CosteoConceptosM()
            {

                idCosteo = idCosteo,
                descripcion = descripcion,
                tipoDatoFacturacion = tipoDatoFacturacion,
                observaciones = observaciones,
                orden = orden,
                visible = visible
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que obtiene una lista de conceptos
        /// </summary>
        /// <param name="idCosteoConcepto">Variable que contiene el id del costeo por concepto</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Modelo con la información de un costeo por concepto</returns>
        public CosteoConceptos GetListConcepto(int idCosteoConcepto, string Token)
        {

            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptos/getCosteosConcepto?idCosteoConcepto=" + idCosteoConcepto);

            using (var wc = new WebClient())
            {
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;
                var result = wc.DownloadString(url);

                var datos = JsonConvert.DeserializeObject<CosteoConceptos>(result);

                return datos;
            }
        }

        /// <summary>
        ///     Método que edita el concepto
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="IdcosteoConcepto">Variable que contiene el id del costeo por concepto</param>
        /// <param name="descripcion">Variable que contiene la descripción</param>
        /// <param name="tipoDatoFacturacion">Variable que contiene el tipo de facturación</param>
        /// <param name="observaciones">Variable que contiene las observaciones del concepto</param>
        /// <param name="orden">Variable que contiene</param>
        /// <param name="visible">Variable que contiene</param>
        /// <returns>Respuesta del movimiento</returns>
        public string EditarConceptos(string Token, int IdcosteoConcepto, string descripcion, string tipoDatoFacturacion, string observaciones, int orden, string visible)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptos/editCosteoConcepto?IdCosteoConcepto=" + IdcosteoConcepto);

            var data = new CosteoConceptos()
            {
                descripcion = descripcion,
                tipoDatoFacturacion = tipoDatoFacturacion,
                observaciones = observaciones,
                orden = orden,
                visible = visible,
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que elimina el registro del concepto
        /// </summary>
        /// <param name="idCosteosConcepto">Variable que contiene el id del costeo por concepto</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Respuesta del movimineto</returns>
        public string removeConcepto(int idCosteosConcepto, string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptos/deleteCosteoConcepto?IdCosteoConcepto=" + idCosteosConcepto);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, "");

            return result;
        }

        /// <summary>
        ///     Método que obtiene una lista de configuración de conceptos
        /// </summary>
        /// <param name="IdCosteosConcepto">Variable que contiene el id del costeo por concepto</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Lista de configuraciones de los conceptos</returns>
        public List<ConfiguraConceptos> GetListConceptosConfiguracion(int IdCosteosConcepto, string Token)
        {

            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptosConfiguracion/getCosteoConceptoConfigs?IdCosteosConcepto=" + IdCosteosConcepto);

            using (var wc = new WebClient())
            {
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;
                var result = wc.DownloadString(url);

                var datos = JsonConvert.DeserializeObject<List<ConfiguraConceptos>>(result);

                return datos;
            }
        }

        /// <summary>
        ///     Método que obtiene los tipos de conceptos
        /// </summary>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente </param>
        /// <returns>Lista del catálogo de conceptos</returns>
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
        ///     
        /// </summary>
        /// <param name="Token">Variable que contiene un JWT para consumir una API</param>
        /// <param name="idUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="idCosteosConcepto">Variable que contiene el id del concepto del costeo</param>
        /// <param name="concepto">Variable que contiene  el concepto del costeo</param>
        /// <param name="descripcion">Variable que contiene la descripción del costeo</param>
        /// <param name="operador">Variable que contiene</param>
        /// <param name="tipoConcepto">Variable que contiene el tipo de concepto</param>
        /// <param name="descripcionValor">Variable que contiene la descripción del costeo</param>
        /// <param name="valor">Variable que contiene el valor del costeo</param>
        /// <param name="operadorGral">Variable que contiene</param>
        /// <returns>Respuesta del movimiento</returns>
        public string AddConceptosConfiguracion(string Token, int idUnidadNegocio, int idCosteosConcepto, string concepto, string descripcion, string operador, string tipoConcepto, string descripcionValor, decimal valor, string operadorGral)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptosConfiguracion/newCosteoConceptoConfig");

            var data = new ConfiguraConceptosM()
            {

                idUnidadNegocio = idUnidadNegocio,
                idCosteosConcepto = idCosteosConcepto,
                concepto = concepto,
                descripcion = descripcion,
                operador = operador,
                tipoConcepto = tipoConcepto,
                descripcionValor = descripcionValor,
                valor = valor,
                operadorGral = operadorGral
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que obtiene la información del concepto configurado
        /// </summary>
        /// <param name="idCosteoConcepto">Variable que contiene id del concepto</param>
        /// <param name="Token">Variable que contiene el JWT para consumir una API</param>
        /// <returns>Modelo con la información de los conceptos</returns>
        public ConfiguraConceptos GetListConceptoConfiguracion(int idCosteoConcepto, string Token)
        {

            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptosConfiguracion/getCosteoConceptoConfig?idCosteoConceptoConfiguracion=" + idCosteoConcepto);

            using (var wc = new WebClient())
            {
                wc.Headers["Content-type"] = "application/json";
                wc.Headers["Authorization"] = "Bearer " + Token;
                var result = wc.DownloadString(url);

                var datos = JsonConvert.DeserializeObject<ConfiguraConceptos>(result);

                return datos;
            }
        }

        /// <summary>
        ///     Método que edita la configuración de los conceptos
        /// </summary>
        /// <param name="Token">Variable que contiene el token para consumir una API</param>
        /// <param name="Idunidadnegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="idCosteoConceptoConfiguracion">Variable que contiene  el id de la configuración del costeo que se va a modificar</param>
        /// <param name="idCosteosConcepto">Variable que contiene el id del costeo</param>
        /// <param name="concepto">Variable que contiene el concepto del costeo</param>
        /// <param name="descripcion">Variable que contiene la descripción</param>
        /// <param name="operador">Variable que contiene </param>
        /// <param name="tipoConcepto">Variable que contiene el tipo de conepto</param>
        /// <param name="descripcionValor">Variable que contiene la descripción del valor del costeo</param>
        /// <param name="valor">Variable que contiene el valor del costeo</param>
        /// <param name="operadorGral">Variable que contiene </param>
        /// <returns>Respuesta del movimiento</returns>
        public string EditarConceptosConfiguracion(string Token, int Idunidadnegocio, int idCosteoConceptoConfiguracion, int idCosteosConcepto, string concepto, string descripcion, string operador, string tipoConcepto, string descripcionValor, decimal valor, string operadorGral)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptosConfiguracion/editCosteoConceptoConfig?idCosteoConceptoConfiguracion=" + idCosteoConceptoConfiguracion);

            var data = new ConfiguraConceptos()
            {
                idUnidadNegocio = Idunidadnegocio,
                descripcion = descripcion,
                idCosteosConcepto = idCosteosConcepto,
                concepto = concepto,
                operador = operador,
                tipoConcepto = tipoConcepto,
                descripcionValor = descripcionValor,
                valor = valor,
                operadorGral = operadorGral,
            };

            var _data = JsonConvert.SerializeObject(data);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, _data);

            return result;
        }

        /// <summary>
        ///     Método que elimina un registro de configuración del concepto
        /// </summary>
        /// <param name="idCosteosConcepto">Variable que contiene el id del concepto</param>
        /// <param name="Token">Variable que contiene el token para consumir una API</param>
        /// <returns>Respuesta del movimiento</returns>
        public string removeConceptoConfiguracion(int idCosteosConcepto, string Token)
        {
            Uri url = new Uri(sStatics.ServidorContabilidad + "api/CosteosConceptosConfiguracion/deleteCosteoConceptoConfig?idCosteoConceptoConfiguracion=" + idCosteosConcepto);

            var wc = new WebClient();
            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer " + Token;

            var result = wc.UploadString(url, "");

            return result;
        }

    }
}