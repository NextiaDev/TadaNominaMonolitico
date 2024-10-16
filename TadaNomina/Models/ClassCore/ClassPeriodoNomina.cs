﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Windows.Input;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    public class ClassPeriodoNomina
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlApiNom;

        public ClassPeriodoNomina()
        {
            _httpClient = new HttpClient();
            _urlApiNom = sStatics.servidor;
        }

        /// <summary>
        /// Método que lista los periodos de nómina por unidad de negocio que estén activas y ordenadas por el periodo de nómina en forma descendente.
        /// </summary>
        /// <param name="IdUnidadNegocio"></param>
        /// <returns></returns>
        public List<PeriodoNomina> GetPeriodoNominas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodos = (from b in entidad.PeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).OrderByDescending(x=>x.IdPeriodoNomina);

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Método que lista los registros patronales por unidad de negocio que estén activos, ordenados por el nombre de la patrona en forma descendente.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los registros patronales activos por unidad de negocio.</returns>
        public List<vRegistroPatronal> GetRegistrosPatronales(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registros = (from b in entidad.vRegistroPatronal.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).OrderByDescending(x => x.NombrePatrona);

                return registros.ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodos de nómina acumulados por unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los periodos de nómina acumuladas por unidad de negocio.</returns>
        public List<PeriodoNomina> GetPeriodoNominasAcumulados(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.PeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).ToList();
            }
        }

        /// <summary>
        /// Obtiene los tipos de periodo que se pueden ejecutar en el sistema.
        /// </summary>
        /// <returns></returns>
        public List<Cat_TipoPeriodo> getTipoPeriodoNomina()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.Cat_TipoPeriodo.Where(x => x.IdEstatus == 1).ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodo de nómina por unidad de negocio que contenga los estatus del arreglo.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa la lista de los periodos con el estatus 1 y 2.</returns>
        public List<PeriodoNomina> GetAllPeriodoNominas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 1, 2 };
                var periodos = from b in entidad.PeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && estatus.Contains(x.IdEstatus)) select b;

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodos de nómina que contenga el arreglo. 
        /// </summary>
        /// <param name="IdsPeriodos">Recibe el identificador de los periodos.</param>
        /// <returns>Regresa la lista con los identificadores del periodo de npomina.</returns>
        public List<PeriodoNomina> getPeriodosIds(int[] IdsPeriodos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {                
                var periodos = from b in entidad.PeriodoNomina.Where(x => IdsPeriodos.Contains(x.IdPeriodoNomina)) select b;

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodos de nómina por unidad de negocio que estén activos y el tipo de nómina específico.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los periodos de nómina activas con el tipo de nómina específico.</returns>
        public List<vPeriodoNomina> GetvPeriodoNominas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {                
                return entidad.vPeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1 && x.TipoNomina != "Especial").ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodos de nómina acumuladas por unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista con los periodos de nómina acumuladas.</returns>
        public List<vPeriodoNomina> GetvPeriodoNominasAcumuladas(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vPeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).ToList();
            }
        }

        /// <summary>
        /// Método que muestra el modelo un periodo de nómina específico.
        /// </summary>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <returns>Regresa el modelo del periodo de nómina con datos.</returns>
        public vPeriodoNomina GetvPeriodoNominasId(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vPeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina).FirstOrDefault();
            }
        }


        /// <summary>
        /// Método que lista el modelo del periodo de nómina con los datos.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista del modelo del periodo de nómina.</returns>
        public List<ModelPeriodoNomina> GetModelPeriodoNominas(int IdUnidadNegocio)
        {
            List<ModelPeriodoNomina> _periodos = new List<ModelPeriodoNomina>();
            List<vPeriodoNomina> lperiodos = GetvPeriodoNominas(IdUnidadNegocio);
            lperiodos.ForEach(x => { _periodos.Add(new ModelPeriodoNomina {
                IdPeriodoNomina = x.IdPeriodoNomina,
                IdUnidadNegocio = x.IdUnidadNegocio,
                UnidaNegocio = x.UnidadNegocio,
                Periodo = x.Periodo,
                TipoNomina = x.TipoNomina,
                FechaInicio = x.FechaInicio.ToShortDateString(),
                FechaFin = x.FechaFin.ToShortDateString(),
                AjusteImpuestos = x.AjusteDeImpuestos,
                IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                Observaciones = x.Observaciones,
                ValidacionAcumulaPeriodo = x.ValidacionAcumulaPeriodo,             
                idsValidacion = x.IdsValidacionAcumulaPeriodo != null && x.IdsValidacionAcumulaPeriodo.Length > 0 ? x.IdsValidacionAcumulaPeriodo.Split(',').ToArray() : new string[0]
            });; });

            return _periodos;
        }


        /// <summary>
        /// Método que lista el modelo del periodo de nómina con los datos de los periodos de nómina acumuladas.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa el listado del modelo de los periodos de nóminas acumuladas.</returns>
        public List<ModelPeriodoNomina> GetModelPeriodoNominasAcumuladas(int IdUnidadNegocio)
        {
            ClassTimbradoNomina ctimbrado = new ClassTimbradoNomina();
            List<ModelPeriodoNomina> _periodos = new List<ModelPeriodoNomina>();
            List<vPeriodoNomina> lperiodos = GetvPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();
            lperiodos.ForEach(x => {
                _periodos.Add(new ModelPeriodoNomina
                {
                    IdPeriodoNomina = x.IdPeriodoNomina,
                    IdUnidadNegocio = x.IdUnidadNegocio,
                    UnidaNegocio = x.UnidadNegocio,
                    Periodo = x.Periodo,
                    TipoNomina = x.TipoNomina,
                    FechaInicio = x.FechaInicio.ToShortDateString(),
                    FechaFin = x.FechaFin.ToShortDateString(),
                    FechaDispersion_ = ((DateTime)x.FechaDispersion).ToShortDateString(),
                    AjusteImpuestos = x.AjusteDeImpuestos,
                    IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                    Observaciones = x.Observaciones,
                    timbradosPeriodo = ctimbrado.GetCantidadTimbresPeriodoNomina(x.IdPeriodoNomina)
                });
            });

            return _periodos;
        }


        /// <summary>
        /// Método que muestra el modelo perido de nómina con los datos de un periodo de nómina en específico.
        /// </summary>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <returns>Regresa el modelo del periodo de nómina especificado.</returns>
        public ModelPeriodoNomina GetModelPeriodoNominasId(int IdPeriodoNomina)
        {            
            vPeriodoNomina lperiodos = GetvPeriodoNominasId(IdPeriodoNomina);
            ModelPeriodoNomina modelo = new ModelPeriodoNomina();

            modelo.IdPeriodoNomina = lperiodos.IdPeriodoNomina;
            modelo.IdUnidadNegocio = lperiodos.IdUnidadNegocio;
            modelo.UnidaNegocio = lperiodos.UnidadNegocio;
            modelo.Periodo = lperiodos.Periodo;
            modelo.TipoNomina = lperiodos.TipoNomina;
            modelo.FechaInicio = lperiodos.FechaInicio.ToShortDateString();
            modelo.FechaFin = lperiodos.FechaFin.ToShortDateString();
            modelo.AjusteImpuestos = lperiodos.AjusteDeImpuestos;
            modelo.IdsPeriodosAjuste = lperiodos.SeAjustaraConPeriodo;
            modelo.Observaciones = lperiodos.Observaciones;
            if (lperiodos.AjusteAnual == "S")
                modelo.AjusteAnual = true;
            if (lperiodos.TablaDiaria == "S")
                modelo.TablaIDiaria = true;

            if (lperiodos.FechaFinChecador != null)
            {
                modelo.FechaFinChecador = lperiodos.FechaFinChecador.ToString().Substring(0, 10);
            }
            if (lperiodos.FechaInicioChecador != null)
            {
                modelo.FechaInicioChecador = lperiodos.FechaInicioChecador.ToString().Substring(0, 10);
            }

            if (lperiodos.DescuentosFijos == "NO")
                modelo.OmitirDescuentosFijos = true;

            //Para PTU
            if (modelo.TipoNomina=="PTU")
            {
                try { modelo.IdCliente_PTU= (int)lperiodos.IdCliente_PTU; } catch { };
                try { modelo.IdRegistroPatronal_PTU=(int)lperiodos.IdRegistroPatronal_PTU; } catch { };
                try { modelo.Monto_PTU= (decimal)lperiodos.Monto_PTU; } catch { };
                try { modelo.Año_PTU= (int)lperiodos.Año_PTU; } catch { };
            }            

            return modelo;
        }

        public dynamic GetModelPeriodoNominasIdConf(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            vPeriodoNomina lperiodos = GetvPeriodoNominasId(IdPeriodoNomina);
            var lSPeriodos = GetSeleccionPeriodoAcumulado(IdUnidadNegocio);
            var descPeriodos = GetDescripcionPeriodos(lperiodos.PeriodosAjusteSecundario);

            return new { lperiodos.IdPeriodoNomina, lperiodos.EmpleadosSinAjuste, lperiodos.PeriodosAjusteSecundario, lSPeriodos, descPeriodos };            
        }

        /// <summary>
        /// Método para agregar un periodo de nómina con pago de utilidades.
        /// </summary>
        /// <param name="modelo">Recibe el modelo del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void AddPeriodoNomina(ModelPeriodoNomina modelo, string token)
        {
            string servicio = "/api/PeriodosNomina/CreatePeriodoNomina"; //Servicio de la API
            Uri apiUrl = new Uri(_urlApiNom + servicio); //URL de la API

            //Se crea el objeto de la petición
            var request = new ModelCreatePeriodoNomina
            (
                (int)modelo.IdUnidadNegocio, //Identificador de la unidad de negocio
                (string)modelo.Periodo, //Periodo de la nómina
                DateTime.Parse(modelo.FechaInicio), //Fecha de inicio del periodo
                DateTime.Parse(modelo.FechaFin), //Fecha de fin del periodo
                (string)modelo.AjusteImpuestos, //Ajuste de impuestos
                modelo.IdsPeriodosAjuste != null ? (string)modelo.IdsPeriodosAjuste : null, //Ids de los periodos de ajuste
                modelo.Observaciones != null ? (string)modelo.Observaciones : null, //Observaciones
                (string)modelo.TipoNomina, //Tipo de nómina
                (bool)modelo.TablaIDiaria, //Tabla I Diaria
                (bool)modelo.AjusteAnual, //Ajuste anual
                (bool)modelo.OmitirDescuentosFijos, //Omitir descuentos fijos
                (int?)modelo.IdCliente_PTU, //Id del cliente PTU
                (int?)modelo.IdRegistroPatronal_PTU, //Id del registro patronal PTU
                (decimal?)modelo.Monto_PTU, // Monto de la PTU
                (int?)modelo.Año_PTU, //Año de la PTU
                (bool?)modelo.CalculoPatronaPtu, // Cálculo de la patrona PTU
                modelo.FechaInicioChecador != null ? (string)modelo.FechaInicioChecador : null, //Fecha de inicio del checador
                modelo.FechaFinChecador != null ? (string)modelo.FechaFinChecador : null //Fecha de fin del checador
            );

            var json = JsonConvert.SerializeObject(request); //Se serializa el objeto de la petición a JSON
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"); //Se convierte a contenido JSON para la petición POST

            _httpClient.DefaultRequestHeaders.Authorization = null; //Se limpia la autorización por si se tiene alguna configurada previamente en el cliente HTTP
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); //Se agrega la autorización con el token JWT en el cliente HTTP

            var response = _httpClient.PostAsync(apiUrl, content).Result; //Se ejecuta la petición POST a la API con el contenido JSON de la petición y se obtiene la respuesta

            response.EnsureSuccessStatusCode(); // Se verifica que la respuesta sea exitosa (200)
        }

        /// <summary>
        /// Método que agrega un identificador de periodo de nómina.
        /// </summary>
        /// <param name="modelo">Recibe el modelo del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el identificador periodo de nómina agregado.</returns>
        public int AddPeriodoNominaId(ModelPeriodoNomina modelo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                PeriodoNomina periodoNomina = new PeriodoNomina();

                periodoNomina.IdUnidadNegocio = modelo.IdUnidadNegocio;
                periodoNomina.Periodo = modelo.Periodo;
                periodoNomina.FechaInicio = DateTime.Parse(modelo.FechaInicio);
                periodoNomina.FechaFin = DateTime.Parse(modelo.FechaFin);
                periodoNomina.AjusteDeImpuestos = modelo.AjusteImpuestos;
                periodoNomina.SeAjustaraConPeriodo = modelo.IdsPeriodosAjuste + "0";
                periodoNomina.Observaciones = modelo.Observaciones;
                periodoNomina.NominaTimbrada = "NO";
                periodoNomina.RecibosComplemento = "NO";
                periodoNomina.TipoNomina = modelo.TipoNomina;

                if (modelo.OmitirDescuentosFijos)
                    periodoNomina.DescuentosFijos = "NO";
                else
                    periodoNomina.DescuentosFijos = "SI";

                periodoNomina.NuevaIncidencia = 0;
                periodoNomina.TotalEmpleados = 0;
                periodoNomina.TotalISR = 0;
                periodoNomina.TotalIMSSObrero = 0;
                periodoNomina.TotalIMSSPatronal = 0;
                periodoNomina.TotalISN = 0;
                periodoNomina.ImporteNeto = 0;
                periodoNomina.IdEstatus = 1;
                periodoNomina.IdCaptura = IdUsuario;
                periodoNomina.FechaCaptura = DateTime.Now;


                entidad.PeriodoNomina.Add(periodoNomina);
                entidad.SaveChanges();

                return periodoNomina.IdPeriodoNomina;
            }
        }

        /// <summary>
        /// Método para listar los periodos de nómina por una unidad de negocio específica.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de de los periodos de nómina.</returns>
        public List<SelectListItem> GetSeleccionPeriodo(int IdUnidadNegocio)
        {
            List<PeriodoNomina> pNomina = GetPeriodoNominas(IdUnidadNegocio).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            List<SelectListItem> lPeriodo = new List<SelectListItem>();
            pNomina.ForEach(x => { lPeriodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            return lPeriodo;
        }

        public Dictionary<int, string> GetSeleccionPeriodoDict(int IdUnidadNegocio)
        {
            List<PeriodoNomina> pNomina = GetPeriodoNominas(IdUnidadNegocio).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            Dictionary<int, string> lPeriodo = new Dictionary<int, string>();
            pNomina.ForEach(x => { lPeriodo.Add(x.IdPeriodoNomina, x.Periodo); });

            return lPeriodo;
        }

        /// <summary>
        /// Método para listar los periodos de nómina acumuladas por una unidad de negocio específica.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los periodos de nómina acumulados.</returns>
        public List<SelectListItem> GetSeleccionPeriodoAcumulado(int IdUnidadNegocio)
        {
            List<PeriodoNomina> pNomina = GetPeriodoNominasAcumulados(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();
            List<SelectListItem> lPeriodo = new List<SelectListItem>();
            pNomina.ForEach(x => { lPeriodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            return lPeriodo;
        }

        /// <summary>
        /// Método para listar los periodos de nómina acumuladas por una unidad de negocio específica, unicamente periodos ordinarios.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los periodos de nómina acumulados.</returns>
        public List<SelectListItem> GetSeleccionPeriodoAcumuladoNominaOrdinaria(int IdUnidadNegocio)
        {
            List<PeriodoNomina> pNomina = GetPeriodoNominasAcumulados(IdUnidadNegocio).Where(x=> x.TipoNomina == "Nomina").OrderByDescending(x => x.IdPeriodoNomina).ToList();
            List<SelectListItem> lPeriodo = new List<SelectListItem>();
            pNomina.ForEach(x => { lPeriodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            return lPeriodo;
        }


        /// <summary>
        /// Método para listar los periodos de nómina de finiquitos por una unidad de negocio específica.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de los periodos de nómina de finiquitos.</returns>
        public List<SelectListItem> GetSeleccionPeriodoFiniquitos(int IdUnidadNegocio)
        {
            List<PeriodoNomina> pNomina = GetPeriodoNominas(IdUnidadNegocio).Where(x=>x.TipoNomina == "Finiquitos").OrderByDescending(x => x.IdPeriodoNomina).ToList();
            List<SelectListItem> lPeriodo = new List<SelectListItem>();
            pNomina.ForEach(x => { lPeriodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            return lPeriodo;
        }

        /// <summary>
        /// Método para listar todos los periodos de nómina.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista con losperiodos de nómina.</returns>
        public List<SelectListItem> GetSeleccionAllPeriodo(int IdUnidadNegocio)
        {
            List<PeriodoNomina> pNomina = GetAllPeriodoNominas(IdUnidadNegocio).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            List<SelectListItem> lPeriodo = new List<SelectListItem>();
            pNomina.ForEach(x => { lPeriodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            return lPeriodo;
        }

        /// <summary>
        /// Método para mostrar los valores del combo box de los periodos de nómina por unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa los valores del modelo del periodo de nómina.</returns>
        public ModelPeriodoNomina FindListPeriodos(int IdUnidadNegocio)
        {
            List<SelectListItem> _TipoNomima = new List<SelectListItem>();
            var tipoPeriodo = getTipoPeriodoNomina();
            tipoPeriodo.ForEach(x => _TipoNomima.Add(new SelectListItem { Text = x.TipoPeriodo, Value = x.TipoPeriodo }));
            
            List<SelectListItem> _AjusteImp = new List<SelectListItem>();
            _AjusteImp.Add(new SelectListItem { Text = "SI", Value = "SI" });
            _AjusteImp.Add(new SelectListItem { Text = "NO", Value = "NO" });

            List<SelectListItem> _periodos = new List<SelectListItem>();
            List<PeriodoNomina> lperiodos = GetPeriodoNominasAcumulados(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();
            lperiodos.ForEach(x => { _periodos.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            List<SelectListItem> _registros = new List<SelectListItem>();
            List<vRegistroPatronal> lregistros = GetRegistrosPatronales(IdUnidadNegocio);
            lregistros.ForEach(x=> { _registros.Add(new SelectListItem { Text= x.NombrePatrona, Value= x.IdRegistroPatronal.ToString() }); });

            ModelPeriodoNomina model = new ModelPeriodoNomina
            {
                LAjuste = _AjusteImp,
                LPAjuste = _periodos,
                LTipoNomina = _TipoNomima,
                listRegistrosPatronalesCliente= _registros
            };

            return model;
        }

        /// <summary>
        /// Método para mostrar los valores del combo box de los periodos de nómina con ajustes y por unidad de negocio específico.
        /// </summary>
        /// <param name="model">Recibe el modelo del periodo de nómina.</param>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa los valores del modelo del periodo de nómina.</returns>
        public ModelPeriodoNomina FindListPeriodos(ModelPeriodoNomina model, int IdUnidadNegocio)
        {
            List<SelectListItem> _TipoNomima = new List<SelectListItem>();
            var tipoPeriodo = getTipoPeriodoNomina();
            tipoPeriodo.ForEach(x => _TipoNomima.Add(new SelectListItem { Text = x.TipoPeriodo, Value = x.TipoPeriodo }));
            
            List<SelectListItem> _AjusteImp = new List<SelectListItem>();
            _AjusteImp.Add(new SelectListItem { Text = "SI", Value = "SI" });
            _AjusteImp.Add(new SelectListItem { Text = "NO", Value = "NO" });

            List<SelectListItem> _periodos = new List<SelectListItem>();
            List<PeriodoNomina> lperiodos = GetPeriodoNominasAcumulados(IdUnidadNegocio).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            lperiodos.ForEach(x => { _periodos.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            List<SelectListItem> _registros = new List<SelectListItem>();
            List<vRegistroPatronal> lregistros = GetRegistrosPatronales(IdUnidadNegocio);
            lregistros.ForEach(x => { _registros.Add(new SelectListItem { Text= x.NombrePatrona, Value= x.IdRegistroPatronal.ToString() }); });

            model.PeriodosAjuste = GetDescripcionPeriodos(model.IdsPeriodosAjuste);
            model.LAjuste = _AjusteImp;
            model.LPAjuste = _periodos;
            model.LTipoNomina = _TipoNomima;
            model.listRegistrosPatronalesCliente= _registros;

            return model;
        }

        /// <summary>
        /// Método que obtiene la descripción de los periodos de nómina.
        /// </summary>
        /// <param name="idsPeriodos">Recibe el identificador del periodo de nómina.</param>
        /// <returns>Regresa la descripción de los periodos de nómina.</returns>
        public string GetDescripcionPeriodos(string idsPeriodos)
        {
            string desc = string.Empty;
            if (idsPeriodos != null)
            {
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    string[] ids = idsPeriodos.Split(',').ToArray();

                    var periodos = (from b in entidad.PeriodoNomina.Where(x => ids.Contains(x.IdPeriodoNomina.ToString())) select b).ToList();

                    periodos.ForEach(x => { desc += x.Periodo + ","; });
                }
            }

            return desc;
        }

        /// <summary>
        /// Método para modificar la información del periodo de nómina.
        /// </summary>
        /// <param name="modelo">Recibe el modelo del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void EditPeriodoNomina(ModelPeriodoNomina modelo, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = (from b in entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == modelo.IdPeriodoNomina) select b).FirstOrDefault();

                periodo.Periodo = modelo.Periodo;
                periodo.TipoNomina = modelo.TipoNomina;
                periodo.FechaInicio = DateTime.Parse(modelo.FechaInicio);
                periodo.FechaFin = DateTime.Parse(modelo.FechaFin);
                periodo.Observaciones = modelo.Observaciones;
                periodo.AjusteDeImpuestos = modelo.AjusteImpuestos;
                if (modelo.AjusteImpuestos == "NO")
                {
                    modelo.AjusteAnual = false;

                }
                

                periodo.SeAjustaraConPeriodo = modelo.IdsPeriodosAjuste;
                periodo.IdModifica = IdUsuario;
                periodo.FechaModifica = DateTime.Now;

                try { periodo.FechaFinChecador = DateTime.Parse(modelo.FechaFinChecador); } catch { }
                try { periodo.FechaInicioChecador = DateTime.Parse(modelo.FechaInicioChecador); } catch { }

                if (modelo.OmitirDescuentosFijos)
                    periodo.DescuentosFijos = "NO";
                else
                    periodo.DescuentosFijos = "SI";
                if (modelo.AjusteAnual)
                    periodo.AjusteAnual = "S";
                else
                    periodo.AjusteAnual = "N";

                if (modelo.TablaIDiaria)
                    periodo.TablaDiaria = "S";
                else
                    periodo.TablaDiaria = "N";

                if (modelo.TipoNomina == "PTU")
                {
                    periodo.IdRegistroPatronal_PTU = modelo.IdRegistroPatronal_PTU;
                    periodo.Monto_PTU = modelo.Monto_PTU;
                    periodo.Año_PTU = modelo.Año_PTU;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para modificar la información del periodo de nómina, solo modifica los conceptos que se calcularan automaticamente.
        /// </summary>
        /// <param name="modelo">Recibe el modelo del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void EditPeriodoNominaConceptosAut(int IdPeriodoNomina, bool BanderaNoVacaciones, bool BanderaNoPV, bool BanderaNoAguinaldo, bool BanderaNo90Dias, bool BanderaNo20Dias, bool BanderaNoPA)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina).FirstOrDefault();

                if (periodo != null)
                {
                    periodo.BanderaNoVacaciones = BanderaNoVacaciones ? "S" : null;
                    periodo.BanderaNoPV = BanderaNoPV ? "S" : null;
                    periodo.BanderaNoAguinaldo = BanderaNoAguinaldo ? "S" : null;
                    periodo.BanderaNo90Dias = BanderaNo90Dias ? "S" : null;
                    periodo.BanderaNo20Dias = BanderaNo20Dias ? "S" : null;
                    periodo.BanderaNoPA = BanderaNoPA? "S" : null;

                    entidad.SaveChanges();
                }                
            }
        }

        /// <summary>
        /// Método para modificar la información del periodo de nómina, guarda los periodos con los que se integra el SDI de liquidación.
        /// </summary>
        /// <param name="modelo">Recibe el modelo del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void EditPeriodoNominaDatosSDI(int IdPeriodoNomina, string periodos)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina).FirstOrDefault();

                if (periodo != null)
                {
                    periodo.PeriodosIntegracionSDI = periodos;

                    entidad.SaveChanges();
                }
            }
        }
                

        public void guardarPeriodoNominaConfAjuste(int IdPeriodoNomina, string EmpleadosSinAjuste, string PeriodosAjuste, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = (from b in entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (periodo != null)
                {
                    periodo.EmpleadosSinAjuste = EmpleadosSinAjuste != null ? EmpleadosSinAjuste.Replace(" ", "") : null;
                    periodo.PeriodosAjusteSecundario= PeriodosAjuste;
                    periodo.IdModifica = IdUsuario;

                    entidad.SaveChanges();
                }                
            }
        }

        /// <summary>
        /// Método para eliminar un periodo de nómina.
        /// </summary>
        /// <param name="Id">Recibe el identificador del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        public void DeletePeriodoNomina(int Id, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodo = (from b in entidad.PeriodoNomina.Where(x => x.IdPeriodoNomina == Id) select b).FirstOrDefault();

                periodo.IdEstatus = 3;
                periodo.IdModifica = IdUsuario;
                periodo.FechaModifica = DateTime.Now;

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para acumular el periodo de nómina.
        /// </summary>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <param name="FechaDispersion">Recibe la fecha de dispersión.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <exception cref="Exception">Envía mensaje de error.</exception>
        public void AcumularPeriodoNomina(int IdPeriodoNomina, DateTime FechaDispersion, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                try
                {
                    string fecha = FechaDispersion.ToString("yyyyMMdd");
                    string consulta = "sp_AcumulaPeriodoNomina " + IdPeriodoNomina + ", " + IdUsuario + ", '" + fecha + "'";
                    entidad.Database.ExecuteSqlCommand(consulta);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Método para desacumular un periodo de nómina.
        /// </summary>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <exception cref="Exception">Envía mensaje de eroor.</exception>
        public void DesAcumularPeriodoNomina(int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                try
                {
                    string consulta = "sp_DesAcumulaPeriodoNomina " + IdPeriodoNomina;
                    entidad.Database.ExecuteSqlCommand(consulta);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Método para listar los periodos de nómina con sus respectivos status.
        /// </summary>
        /// <param name="_IdUnidadNegocio">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista con los periodos de nómina y los status que les corresponden.</returns>
        public List<ModelPeriodoNomina> GetPeriodosDifStatus(int _IdUnidadNegocio)
        {
            using (NominaEntities1 _ctx = new NominaEntities1())
            {
                return (from x in _ctx.PeriodoNomina
                        where x.IdUnidadNegocio == _IdUnidadNegocio && (x.IdEstatus == 2 || x.IdEstatus == 1 || x.IdEstatus == 3)
                        select new ModelPeriodoNomina
                        {
                            IdPeriodoNomina = x.IdPeriodoNomina,
                            IdUnidadNegocio = x.IdUnidadNegocio,
                            Periodo = x.Periodo,
                            TipoNomina = x.TipoNomina,
                            FechaInicio = x.FechaInicio.ToString(),
                            FechaFin = x.FechaFin.ToString(),
                            AjusteImpuestos = x.AjusteDeImpuestos,
                            IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                            Observaciones = x.Observaciones
                        }).ToList();
            }
        }

        /// <summary>
        /// Método para listar los valores del combo box de los periodos de nómina.
        /// </summary>
        /// <param name="_PeriodoNomina">Recibe el modelo del periodo de nómina.</param>
        /// <returns>Regresa la lista con los valores del modelo.</returns>
        public List<SelectListItem> ListaPeriodosnomina(List<ModelPeriodoNomina> _PeriodoNomina)
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem
            {
                Value = "",
                Text = "Elegir...",
            });
            foreach (var item in _PeriodoNomina)
            {
                _lista.Add(new SelectListItem
                {
                    Value = item.IdPeriodoNomina.ToString(),
                    Text = item.Periodo,
                });
            }
            return _lista;
        }


        /// <summary>
        /// Método para obtener el periodo de nómina por el identificador del periodo.
        /// </summary>
        /// <param name="_IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <returns>Regresa el periodo de nómina.</returns>
        public PeriodoNomina GetPeriodo(int _IdPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.PeriodoNomina.Where(x => x.IdPeriodoNomina == _IdPeriodoNomina) select a).FirstOrDefault();
            }
        }

        public List<IGrouping<int, ModelAcumularPeriodo>> getDatosNominaByPeriodo(int[] IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vNominaTrabajo.Where(x => x.IdEstatus == 1 && IdPeriodoNomina.Contains(x.IdPeriodoNomina))
                    .Select(x => new ModelAcumularPeriodo { Id = x.IdPeriodoNomina, isr = x.ISR, cargaObrera = x.IMSS_Obrero, cargaPatronal = x.Total_Patron, 
                        totalPerc = x.ER, totalDed = x.DD, neto = x.Neto })                    
                    .GroupBy(x => x.Id).ToList();
            }
        }

        public List<ModelAcumularPeriodo> getDatosNominaByIdPeriodo(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vNominaTrabajo.Where(x => x.IdEstatus == 1 && x.IdPeriodoNomina == IdPeriodoNomina)
                    .Select(x => new ModelAcumularPeriodo { Id = x.IdPeriodoNomina, isr = x.ISR, cargaObrera = x.IMSS_Obrero, cargaPatronal = x.Total_Patron, 
                        totalPerc = x.ER, totalDed = x.DD, neto = x.Neto })                    
                    .ToList();
            }
        }

        public List<vPeriodoNomina> GetvPeriodoHonorarios(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodos = from b in entidad.vPeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1 && x.TipoNomina == "Honorarios") select b;

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Método para validar dentro de las nominas administradas que unidades tienen recibos pendientes por timbrar
        /// </summary>
        /// <param name="IdUnidadNegocio">Es el Id de la Unidad de Negocio con la que se va a ejecutar la validación</param>
        /// <returns></returns>
        public int ValidaTimbrado(int IdUnidadNegocio)
        {
            SqlConnection sqlconnLocal = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);
            string query = "select dbo.fu_ValidaTimbrado(" + IdUnidadNegocio.ToString() + ")";

            try
            {
                sqlconnLocal.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlconnLocal))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    return int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                sqlconnLocal.Close();
            }
        }

        public List<ModelPeriodoNomina> GetModelPeriodoNominasHonorarios(int IdUnidadNegocio)
        {
            List<ModelPeriodoNomina> _periodos = new List<ModelPeriodoNomina>();
            List<vPeriodoNomina> lperiodos = GetvPeriodoNominasHonorarios(IdUnidadNegocio);
            lperiodos.ForEach(x => {
                _periodos.Add(new ModelPeriodoNomina
                {
                    IdPeriodoNomina = x.IdPeriodoNomina,
                    IdUnidadNegocio = x.IdUnidadNegocio,
                    UnidaNegocio = x.UnidadNegocio,
                    Periodo = x.Periodo,
                    TipoNomina = x.TipoNomina,
                    FechaInicio = x.FechaInicio.ToShortDateString(),
                    FechaFin = x.FechaFin.ToShortDateString(),
                    AjusteImpuestos = x.AjusteDeImpuestos,
                    IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                    Observaciones = x.Observaciones,
                    ValidacionAcumulaPeriodo = x.ValidacionAcumulaPeriodo,
                    IdEstatus = x.IdEstatus,
                    idsValidacion = x.IdsValidacionAcumulaPeriodo != null && x.IdsValidacionAcumulaPeriodo.Length > 0 ? x.IdsValidacionAcumulaPeriodo.Split(',').ToArray() : new string[0]
                }); ;
            });

            return _periodos;
        }


        public List<vPeriodoNomina> GetvPeriodoNominasHonorarios(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {

                var periodos = from b in entidad.vPeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.TipoNomina == "Honorarios" ) select b;

                return periodos.ToList();
            }
        }

        /// <summary>
        /// Se valida que los totales de nómina e incidencias antes de cerrar el periodo
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo a validar</param>
        public List<string> ValidaTotales(int IdPeriodo)
        {
            List<string> diferencias = new List<string>();
            var nomina = GetNominaTrabajoValidacion(IdPeriodo);
            var incidencias = GetIncidenciasValidacion(IdPeriodo);

            foreach (var item in nomina)
            {
                decimal? incidenciasER = incidencias.Where(x => x.IdEmpleado == item.IdEmpleado && x.TipoConcepto == "ER" && x.Monto != 0).Select(s=> s.Monto).Sum();
                incidenciasER += incidencias.Where(x => x.IdEmpleado == item.IdEmpleado && x.TipoConcepto == "OTRO" && x.Monto != 0).Select(s=> s.Monto).Sum();
                decimal nominaER = (item.SueldoPagado ?? 0) + (item.SubsidioPagar ?? 0) + (item.ReintegroISR ?? 0);

                decimal ER = (incidenciasER ?? 0) + nominaER;

                if (ER != item.ER)
                    diferencias.Add(item.ClaveEmpleado + "-" + item.ApellidoPaterno + " " + item.ApellidoMaterno + " " + item.Nombre + " - Diferencia Total Percepciones: " + string.Format("{0:C2}", item.ER - ER));

                decimal? incidenciasDD = incidencias.Where(x => x.IdEmpleado == item.IdEmpleado && x.TipoConcepto == "DD" && x.Monto != 0).Select(s => s.Monto).Sum();
                decimal nominaDD = (item.ImpuestoRetener ?? 0) + (item.IMSS_Obrero ?? 0);

                decimal DD = (incidenciasDD ?? 0) + nominaDD;

                if (DD != item.DD)
                    diferencias.Add(item.ClaveEmpleado + "-" + item.Nombre + " - Diferencia Total Deducciones: " + string.Format("{0:C2}", item.DD - DD));
            }

            return diferencias;
        }

        /// <summary>
        /// Obtiene los datos del periodo de nomina para validar antes de acumualar
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina.</param>
        /// <returns></returns>
        public List<vNominaTrabajo> GetNominaTrabajoValidacion(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vNominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).ToList();
            }
        }

        /// <summary>
        /// Obtiene los datos de incidencias para validar antes de timbrar.
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina.</param>
        /// <returns></returns>
        public List<vIncidencias> GetIncidenciasValidacion(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).ToList();
            }
        }
    }
}
