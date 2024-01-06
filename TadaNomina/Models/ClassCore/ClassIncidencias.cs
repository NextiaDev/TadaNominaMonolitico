using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Incidencias
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassIncidencias
    {
        public decimal montoReal { get; set; }
        public decimal montoTradicional { get; set; }
        public decimal montoEsquema { get; set; }
        public decimal Exento { get; set; }
        public decimal Gravado { get; set; }
        public decimal UMA { get; set; }
        public decimal SMGV { get; set; }

        /// <summary>
        /// Método para obtener un listado con las incidencias activas del periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <returns>Listado de las incidencias del periodo de nómina</returns>
        public List<Incidencias> GetIncindencias(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1) select b;

                return incidencias.ToList();
            }
        }

        /// <summary>
        /// Método para obtener la información de la incidencia
        /// </summary>
        /// <param name="IdIncidencia">Identificador de la incidencia</param>
        /// <returns>Información de la incidencia</returns>
        public Incidencias GetIncindencia(int IdIncidencia)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.Incidencias.Where(x => x.IdIncidencia == IdIncidencia && x.IdEstatus == 1) select b).FirstOrDefault();

                return incidencias;
            }
        }        

        /// <summary>
        /// Método para obtene run listado con la información de las incidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns>Listado de la información de las incidencias</returns>
        public List<vIncidencias> GetvIncindencias(int IdPeriodoNomina, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = from b in entidad.vIncidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado) select b;

                return incidencias.ToList();
            }
        }

        /// <summary>
        /// Método para obtener un listado de tipo ModelIncidencias
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodeo de nómina</param>
        /// <param name="token">Token para acceso de la api</param>
        /// <returns>Listado de tipo ModelIncidencias</returns>
        public List<ModelIncidencias> GetModelIncidencias(int IdPeriodoNomina, string token)
        {
            var si = new sIncidencias();
            var modelIncidencias = new List<ModelIncidencias>();
            var vIncidencias = si.GetvIncindencias(IdPeriodoNomina, token);

            vIncidencias.ForEach(x => { modelIncidencias.Add(new ModelIncidencias {
                IdIncidencia = x.IdIncidencia,
                IdEmpleado = (int)x.IdEmpleado,
                ClaveEmpleado = x.ClaveEmpleado,
                NombreEmpleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                IdPeriodoNomina = (int)x.IdPeriodoNomina,
                PeriodoNomina = x.Periodo,
                IdConcepto = (int)x.IdConcepto,
                ClaveConcepto = x.ClaveConcepto,
                Concepto = x.Concepto,
                Cantidad = (decimal)x.Cantidad,
                Monto = (decimal)x.Monto,
                Exento = (decimal)x.Exento,
                Gravado = (decimal)x.Gravado,
                MontoEsquema = x.MontoEsquema,
                ExentoEsquema = x.ExentoEsquema,
                GravadoEsquema = x.GravadoEsquema,
                FechaIncio = x.FechaInicio,
                FechaFinal = x.FechaFin,
                Folio = x.Folio,
                Observaciones = x.Observaciones,
                TipoEsquema = x.TipoEsquema
            }); });

            return modelIncidencias;
        }

        /// <summary>
        /// Método para llenar el ModelIncidencias
        /// </summary>
        /// <param name="IdIncidencia">Identificador de la incidencia</param>
        /// <param name="token">Token para acceso de la api</param>
        /// <returns>ModelIncidencias</returns>
        public ModelIncidencias GetModelIncidencia(int IdIncidencia, string token)
        {
            var si = new sIncidencias();
            var vIncidencias = si.GetvIncindencia(IdIncidencia, token);
            var modelIncidencias = new ModelIncidencias();
            
            modelIncidencias.IdIncidencia = vIncidencias.IdIncidencia;
            modelIncidencias.IdEmpleado = (int)vIncidencias.IdEmpleado;
            modelIncidencias.ClaveEmpleado = vIncidencias.ClaveEmpleado;
            modelIncidencias.NombreEmpleado = vIncidencias.ApellidoPaterno + " " + vIncidencias.ApellidoMaterno + " " + vIncidencias.Nombre;
            modelIncidencias.IdPeriodoNomina = (int)vIncidencias.IdPeriodoNomina;
            modelIncidencias.PeriodoNomina = vIncidencias.Periodo;
            modelIncidencias.IdConcepto = (int)vIncidencias.IdConcepto;
            modelIncidencias.ClaveConcepto = vIncidencias.ClaveConcepto;
            modelIncidencias.Concepto = vIncidencias.Concepto;
            modelIncidencias.Cantidad = (decimal)vIncidencias.Cantidad;
            modelIncidencias.Monto = (decimal)vIncidencias.Monto;
            modelIncidencias.Exento = (decimal)vIncidencias.Exento;
            modelIncidencias.Gravado = (decimal)vIncidencias.Gravado;
            try { modelIncidencias.MontoEsquema = (decimal)vIncidencias.MontoEsquema; } catch { }
            try { modelIncidencias.ExentoEsquema = (decimal)vIncidencias.ExentoEsquema; } catch { }
            try { modelIncidencias.GravadoEsquema = (decimal)vIncidencias.GravadoEsquema; } catch { }
            modelIncidencias.FechaIncio = vIncidencias.FechaInicio;
            modelIncidencias.FechaFinal = vIncidencias.FechaFin;
            modelIncidencias.Folio = vIncidencias.Folio;
            modelIncidencias.Observaciones = vIncidencias.Observaciones;            

            return modelIncidencias;
        }

        /// <summary>
        /// Método para agregar incidencias
        /// </summary>
        /// <param name="i">ModelIncidencias</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public int AddIncidencias(ModelIncidencias i, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                Incidencias ins = new Incidencias();

                ins.IdEmpleado = i.IdEmpleado;
                ins.IdPeriodoNomina = i.IdPeriodoNomina;
                ins.IdConcepto = i.IdConcepto;
                ins.Cantidad = Math.Round(i.Cantidad, 2);
                try { ins.CantidadEsquema = i.CantidadEsq; } catch { }
                ins.Monto = Math.Round(i.Monto, 2);
                ins.Exento = Math.Round(i.Exento, 2);
                ins.Gravado = Math.Round(i.Gravado, 2);
                try { ins.MontoEsquema = Math.Round((decimal)i.MontoEsquema, 2); } catch { ins.MontoEsquema = 0; }
                 try {   ins.ExentoEsquema = Math.Round((decimal)i.ExentoEsquema, 2); } catch { ins.ExentoEsquema = 0; }
                try { ins.GravadoEsquema = Math.Round((decimal)i.GravadoEsquema, 2); } catch { ins.GravadoEsquema = 0; }
                ins.FechaInicio = i.FechaIncio;
                ins.FechaFin = i.FechaFinal;
                ins.Folio = i.Folio;
                ins.Observaciones = i.Observaciones;
                ins.FormulaEjecutada = i.FormulaEjecutada;
                ins.BanderaFiniquitos = i.BanderaFiniquitos;
                ins.BanderaAguinaldos = i.BanderaAguinaldo;
                ins.BanderaIncidenciasFijas = i.BanderaIncidenciasFijas;
                ins.BanderaAdelantoPULPI = i.BanderaAdelantoNominaPULPI;
                ins.BanderaVacaciones = i.BanderaVacaciones;
                ins.BanderaCompensaciones = i.BanderaCompensaciones;
                ins.BanderaIncidencia = i.BanderaIncidencia;
                ins.BanderaConceptoEspecial = i.BanderaConceptoEspecial;
                ins.BanderaChecadores = i.BanderaChecadores;
                ins.IdEstatus = 1;
                ins.IdCaptura = IdUsuario;
                ins.FechaCaptura = DateTime.Now;               
                                
                entidad.Incidencias.Add(ins);
                entidad.SaveChanges();

                return ins.IdIncidencia;
            }
        }

        /// <summary>
        /// Metodo par allenar un listado de las incidencias
        /// </summary>
        /// <param name="IdUnidad">Identificador de la unidad de negocio</param>
        /// <param name="IdCliente">Identificador de cliente</param>
        /// <returns>Listado de las incidencias</returns>
        public ModelIncidencias LlenaListasIncidencias(int IdUnidad, int IdCliente)
        {
            List<SelectListItem> _Periodo = new List<SelectListItem>();
            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            List<PeriodoNomina> periodos = cperiodo.GetPeriodoNominas(IdUnidad).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            periodos.ForEach(x => { _Periodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            List<SelectListItem> _empleados = new List<SelectListItem>();
            ClassEmpleado cempleado = new ClassEmpleado();
            List<Empleados> lEmpleados = cempleado.GetEmpleadoByUnidadNegocio(IdUnidad).OrderBy(x => x.ApellidoPaterno).ToList();
            lEmpleados.ForEach(x => { _empleados.Add(new SelectListItem { Text = x.ClaveEmpleado + " - " + x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, Value = x.IdEmpleado.ToString() }); });

            List<SelectListItem> _conceptos = new List<SelectListItem>();
            ClassConceptos cconceptos = new ClassConceptos();
            List<Cat_ConceptosNomina> lconceptos = cconceptos.GetConceptos(IdCliente).OrderBy(x => x.Concepto).ToList();
            lconceptos.ForEach(x => { _conceptos.Add(new SelectListItem { Text = x.ClaveConcepto + " - " + x.Concepto, Value = x.IdConcepto.ToString() }); });

            ModelIncidencias modelIncidencias = new ModelIncidencias();
            modelIncidencias.LPeriodo = _Periodo;
            modelIncidencias.LEmpleados = _empleados;
            modelIncidencias.LConcepto = _conceptos;


            return modelIncidencias;
        }

        /// <summary>
        /// Método par aobtener los listados de Peridos de nomina, Empleados y Conceptos
        /// </summary>
        /// <param name="IdUnidad">Identificador de la unidad de negocio</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="modelIncidencias">ModelIncidencias</param>
        /// <returns>Listados de Peridos de nomina, Empleados y Conceptos</returns>
        public ModelIncidencias LlenaListasIncidencias(int IdUnidad, int IdCliente, ModelIncidencias modelIncidencias)
        {
            List<SelectListItem> _Periodo = new List<SelectListItem>();
            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            List<PeriodoNomina> periodos = cperiodo.GetPeriodoNominas(IdUnidad).OrderByDescending(x => x.IdPeriodoNomina).ToList();
            periodos.ForEach(x => { _Periodo.Add(new SelectListItem { Text = x.Periodo, Value = x.IdPeriodoNomina.ToString() }); });

            List<SelectListItem> _empleados = new List<SelectListItem>();
            ClassEmpleado cempleado = new ClassEmpleado();
            List<Empleados> lEmpleados = cempleado.GetEmpleadoByUnidadNegocio(IdUnidad).Where(x=>x.IdEstatus == 1).OrderBy(x => x.ApellidoPaterno).ToList();
            lEmpleados.ForEach(x => { _empleados.Add(new SelectListItem { Text = x.ClaveEmpleado + " - " + x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, Value = x.IdEmpleado.ToString() }); });

            List<SelectListItem> _conceptos = new List<SelectListItem>();
            ClassConceptos cconceptos = new ClassConceptos();
            List<Cat_ConceptosNomina> lconceptos = cconceptos.GetConceptos(IdCliente).OrderBy(x => x.Concepto).ToList();
            lconceptos.ForEach(x => { _conceptos.Add(new SelectListItem { Text = x.ClaveConcepto + " - " + x.Concepto, Value = x.IdConcepto.ToString() }); });
                        
            modelIncidencias.LPeriodo = _Periodo;
            modelIncidencias.LEmpleados = _empleados;
            modelIncidencias.LConcepto = _conceptos;

            return modelIncidencias;
        }

        /// <summary>
        /// Método para procesar las incidencias por empleado
        /// </summary>
        /// <param name="incidencias">List<ModelIncidenciaIndividual></param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void Proceso_Incidencias_Individuales(List<ModelIncidenciaIndividual> incidencias, int IdEmpleado, int IdPeriodoNomina, int IdUsuario)
        {
            var _incidencias = incidencias.Where(x => x.BanderaFiniquitos != 1).ToList();
            _incidencias = _incidencias.Where(x => x.BanderaConceptoEspecial == null && x.BanderaInfonavit == null && x.BanderaFonacot == null && x.BanderaPensionAlimenticia == null && x.BanderaIncidenciasFijas == null && x.BanderaAguinaldo == null && x.BanderaAusentismos == null && x.BanderaAdelantoPULPI == null && x.BanderaSaldos == null && x.BanderaCompensaciones == null && x.BanderaIncidencia == null).ToList();
            DeleteIncidencia(_incidencias.Select(x=>x.Id).ToArray());

            foreach (var i in _incidencias)
            {
                ModelIncidencias inc = new ModelIncidencias();
                inc.IdEmpleado = IdEmpleado;
                inc.IdPeriodoNomina = IdPeriodoNomina;
                inc.IdConcepto = i.IdConcepto;
                inc.Cantidad = i.Cantidad;
                inc.CantidadEsq = i.CantidadEsq;
                inc.Monto = i.Monto;
                inc.MontoEsquema = i.MontoEsq;                

                NewIncindencia(inc, IdUsuario);
            }
        }

        /// <summary>
        /// Método para agregar nuevas incidencias recibiendo un modelo
        /// </summary>
        /// <param name="i">ModelIncidencias</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void NewIncindencia(ModelIncidencias i, int IdUsuario)
        {
            ClassConceptos cconceptos = new ClassConceptos();
            vConceptos concepto = cconceptos.GetvConcepto(i.IdConcepto);

            ClassEmpleado cempleado = new ClassEmpleado();
            vEmpleados vEmp = cempleado.GetvEmpleado(i.IdEmpleado);

            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            vPeriodoNomina periodo = cperiodo.GetvPeriodoNominasId(i.IdPeriodoNomina);

            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            Cat_UnidadNegocio catUnidad = cu.getUnidadesnegocioId(periodo.IdUnidadNegocio);

            ClassConceptosFiniquitos ccf = new ClassConceptosFiniquitos();
            ConfiguracionConceptosFiniquito configuracion = ccf.GetConfiguracionConceptosFiniquitos(periodo.IdCliente);

            decimal cantidadAnt = 0;

            if (configuracion != null)
            {
                if (configuracion.IdConceptoPV != null && configuracion.IdConceptoPV == i.IdConcepto)
                {                    
                    ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                    ClassFechasCalculos cf = new ClassFechasCalculos();
                    var conf = cf.GetConfiguracionFechas(periodo.IdUnidadNegocio);
                    if (conf != null)
                    {                       
                        int IdPrestaciones = vEmp.IdPrestaciones ?? 1;
                        DateTime? FechaIngreso = null;
                        FechaIngreso = cpv.ObtenFechaCalculo(vEmp, conf, "Real");
                        decimal Antiguedad = periodo.FechaFin.Subtract((DateTime)FechaIngreso).Days / 365M;
                        cpv.GetDias(IdPrestaciones, Antiguedad);

                        if (concepto.SDPor != 0 && concepto.SDPor != null)
                        {
                            if (concepto.FactoryValor == "SI")
                            {
                                cantidadAnt = i.Cantidad;
                                i.Cantidad = i.Cantidad * (decimal)concepto.SDPor;
                                concepto.SDPor = cpv.Porcentaje;
                            }
                        }
                        else
                            concepto.SDPor = cpv.Porcentaje;
                        
                    }
                }
            }

            if (concepto.MultiplicaDT == "SI")
            {
                concepto.TipoDato = "Cantidades";
                concepto.CalculaMontos = "NO";
            }

            bool guardar = false;
            Exento = 0;
            Gravado = 0;
            switch (concepto.TipoDato)
            {
                case "Cantidades":
                    if (i.Cantidad > 0 || i.CantidadEsq > 0)
                    {
                        decimal cantidadTemporal = i.Cantidad;
                        decimal? cantidadTemporalEsq = i.CantidadEsq;

                        if (catUnidad.DiasFraccioandos == "S" && catUnidad.FactorDiasFraccionados > 0)
                        {
                            if (catUnidad.IdsConceptosFraccionados != null && catUnidad.IdsConceptosFraccionados.Length > 0)
                            {
                                var clavesFraccionadas = catUnidad.IdsConceptosFraccionados.Replace(" ", "").Split(',').ToArray();

                                if (clavesFraccionadas.Contains(i.ClaveConcepto))
                                {
                                    i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                    i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                                }
                            }
                            else
                            {
                                i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                            }
                        }
                                                
                        i.Monto = 0;
                        i.MontoEsquema = 0;

                        if (concepto.CalculaMontos == "SI")
                        {
                            ObtenMontos(concepto, vEmp, i, null, null);
                            ObtenExentosGravados(concepto, periodo.FechaFin, null, i.Cantidad, vEmp.SDIMSS);

                            i.Monto = montoTradicional;
                            i.MontoEsquema = montoEsquema;
                        }

                        i.Cantidad = cantidadTemporal;
                        i.CantidadEsq = cantidadTemporalEsq;
                        guardar = true;
                    }
                    break;
                case "Pesos":
                    i.Cantidad = 0;

                    if (i.Monto > 0)
                    {
                        montoTradicional = i.Monto;
                        ObtenExentosGravados(concepto, periodo.FechaFin, null, null, vEmp.SDIMSS);
                        guardar = true;
                    }

                    if (i.MontoEsquema > 0)
                    {
                        if (concepto.TipoEsquema == "Esquema" || concepto.TipoEsquema == "Mixto")
                        {
                            i.MontoEsquema = i.MontoEsquema;
                        }
                        else
                        {
                            i.MontoEsquema = 0;
                        }
                        guardar = true;
                    }
                    break;
                default:
                    break;
            }
                        
            i.Exento = Exento;
            i.Gravado = Gravado;

            if (concepto.ClaveSAT == "999")
            {
                i.Exento = 0;
                i.Gravado = 0;
            }

            if (cantidadAnt > 0)
                i.Cantidad = cantidadAnt;

            if (guardar)
            {
                int IdIncidencia = AddIncidencias(i, IdUsuario);
                if (concepto.CreaConceptoAdicional == "SI" && concepto.IdConceptoAdicional != null && concepto.IdConceptoAdicional != 0)
                    IncindenciaSecundaria(i, IdIncidencia, (int)concepto.IdConceptoAdicional, IdUsuario);
            }
        }
        
        /// <summary>
        /// Método para agregar nuevas incidencias recibiendo todos los parametros
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="Cantidad">Cantidad de la incidencia</param>
        /// <param name="CantidadEsquema">Cantidad de la incidencia tipo esquema</param>
        /// <param name="Monto">Monto de la incidencia</param>
        /// <param name="MontoEsquema">Montpo de la incidencia tipo esquema</param>
        /// <param name="FechaInicio">Fecha de inicio</param>
        /// <param name="FechaFin">Fecha de termino</param>
        /// <param name="Folio">Folio de la incidencia</param>
        /// <param name="Observaciones">Observaciones de la incidencia</param>
        /// <param name="BanderaFiniquitos">Identificador de finiquito</param>
        /// <param name="BanderaAguinaldos">Identificador de aginaldo</param>
        /// <param name="BanderaIncidenciasFijas">Identificador de incidencia fija</param>
        /// <param name="FactorDiasTrabajadosPeriodo">Valor proporcional de los dias trabajados durante el periodo por el empleado siendo 1 por los trabajar todos los días del periodo</param>
        /// <param name="Porcentaje"></param>
        /// <param name="SDCalculo">Salario diario</param>
        /// <param name="SDCalculoReal">Salario diario real</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void NewIncindencia(int IdPeriodoNomina, int IdConcepto, int IdEmpleado, decimal Cantidad, decimal CantidadEsquema, decimal Monto, decimal MontoEsquema, DateTime? FechaInicio, DateTime? FechaFin, string Folio, string Observaciones, int? BanderaFiniquitos, int? BanderaAguinaldos, int? BanderaIncidenciasFijas, decimal? FactorDiasTrabajadosPeriodo, decimal? Porcentaje, decimal? SDCalculo, decimal? SDCalculoReal, int IdUsuario)
        {
            var i = new ModelIncidencias();
            var cconceptos = new ClassConceptos();
            var concepto = cconceptos.GetvConcepto(IdConcepto);

            var cempleado = new ClassEmpleado();
            var vEmp = cempleado.GetvEmpleado(IdEmpleado);

            var cperiodo = new ClassPeriodoNomina();
            var periodo = cperiodo.GetvPeriodoNominasId(IdPeriodoNomina);

            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            Cat_UnidadNegocio catUnidad = cu.getUnidadesnegocioId(periodo.IdUnidadNegocio);

            ClassConceptosFiniquitos ccf = new ClassConceptosFiniquitos();
            ConfiguracionConceptosFiniquito configuracion = ccf.GetConfiguracionConceptosFiniquitos(periodo.IdCliente);

            decimal cantidadAnt = 0;

            if (configuracion != null)
            {
                if (configuracion.IdConceptoPV != null && configuracion.IdConceptoPV == i.IdConcepto)
                {
                    ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                    ClassFechasCalculos cf = new ClassFechasCalculos();
                    var conf = cf.GetConfiguracionFechas(periodo.IdUnidadNegocio);
                    if (conf != null)
                    {
                        int IdPrestaciones = vEmp.IdPrestaciones ?? 1;
                        DateTime? FechaIngreso = null;
                        FechaIngreso = cpv.ObtenFechaCalculo(vEmp, conf, "Real");
                        decimal Antiguedad = periodo.FechaFin.Subtract((DateTime)FechaIngreso).Days / 365M;
                        cpv.GetDias(IdPrestaciones, Antiguedad);

                        if (concepto.SDPor != 0 && concepto.SDPor != null)
                        {
                            if (concepto.FactoryValor == "SI")
                            {
                                cantidadAnt = i.Cantidad;
                                i.Cantidad = i.Cantidad * (decimal)concepto.SDPor;
                                concepto.SDPor = cpv.Porcentaje;
                            }
                        }
                        else
                            concepto.SDPor = cpv.Porcentaje;
                    }
                }
            }

            i.IdConcepto = IdConcepto;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdEmpleado = IdEmpleado;
            i.FechaIncio = FechaInicio;
            i.FechaFinal = FechaFin;
            i.Folio = Folio;
            i.Observaciones = Observaciones;
            i.BanderaFiniquitos = BanderaFiniquitos;
            i.BanderaAguinaldo = BanderaAguinaldos;
            i.BanderaIncidenciasFijas = BanderaIncidenciasFijas;

            bool guardar = false;
            switch (concepto.TipoDato)
            {
                case "Cantidades":
                    i.Cantidad = Cantidad;
                    i.CantidadEsq = CantidadEsquema;

                    if (Cantidad > 0 || CantidadEsquema > 0)
                    {
                        decimal cantidadTemporal = i.Cantidad;
                        decimal? cantidadTemporalEsq = i.CantidadEsq;

                        if (catUnidad.DiasFraccioandos == "S" && catUnidad.FactorDiasFraccionados > 0 )
                        {
                            if (catUnidad.IdsConceptosFraccionados != null && catUnidad.IdsConceptosFraccionados.Length > 0)
                            {
                                var clavesFraccionadas = catUnidad.IdsConceptosFraccionados.Replace(" ", "").Split(',').ToArray();

                                if (clavesFraccionadas.Contains(i.ClaveConcepto))
                                {
                                    i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                    i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                                }
                            }
                            else
                            {
                                i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                            }
                        }

                        i.Monto = 0;
                        i.MontoEsquema = 0;

                        if (concepto.CalculaMontos == "SI")
                        {
                            ObtenMontos(concepto, vEmp, Cantidad, CantidadEsquema, Porcentaje, SDCalculo, SDCalculoReal);
                            ObtenExentosGravados(concepto, periodo.FechaFin, FactorDiasTrabajadosPeriodo, Cantidad, vEmp.SDIMSS);
                            if (Porcentaje != null && Porcentaje > 0) { i.Cantidad = Cantidad * (decimal)Porcentaje; i.CantidadEsq = CantidadEsquema * (decimal)Porcentaje; }                            
                            i.Monto = montoTradicional;
                            i.MontoEsquema = montoEsquema;
                        }

                        i.Cantidad = cantidadTemporal;
                        i.CantidadEsq = cantidadTemporalEsq;
                        guardar = true;
                    }
                    break;
                case "Pesos":
                    Cantidad = 0;

                    if (Monto > 0)
                    {
                        i.Monto = Monto;
                        montoTradicional = Monto;
                        ObtenExentosGravados(concepto, periodo.FechaFin, FactorDiasTrabajadosPeriodo, null, vEmp.SDIMSS);
                        guardar = true;
                    }

                    if (MontoEsquema > 0)
                    {
                        if (concepto.TipoEsquema == "Esquema" || concepto.TipoEsquema == "Mixto")
                        {
                            i.MontoEsquema = MontoEsquema;
                        }
                        else
                        {
                            i.MontoEsquema = 0;
                        }
                        guardar = true;
                    }
                    break;
                default:
                    break;
            }

            i.Exento = Exento;
            i.Gravado = Gravado;

            if (concepto.ClaveSAT == "999")
            {
                i.Exento = 0;
                i.Gravado = 0;
            }

            if (cantidadAnt > 0)
                i.Cantidad = cantidadAnt;

            if (guardar)
            {
                int IdIncidencia = AddIncidencias(i, IdUsuario);
                if (concepto.CreaConceptoAdicional == "SI" && concepto.IdConceptoAdicional != null && concepto.IdConceptoAdicional != 0)
                    IncindenciaSecundaria(i, IdIncidencia, (int)concepto.IdConceptoAdicional, IdUsuario);
            }
        }

        /// <summary>
        /// Método para agregar incidencias recibiendo listas
        /// </summary>
        /// <param name="model">List<ModelIncidencias></param>
        /// <param name="conceptos">List<vConceptos></param>
        /// <param name="empleados">List<vEmpleados></param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void NewIncindencia(List<ModelIncidencias> model, List<vConceptos> conceptos, List<vEmpleados> empleados, int IdUsuario)
        {
            foreach (var i in model)
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                vPeriodoNomina periodo = cperiodo.GetvPeriodoNominasId(i.IdPeriodoNomina);

                ClassConceptos cconceptos = new ClassConceptos();
                vConceptos concepto = cconceptos.GetvConcepto(i.IdConcepto);
                                
                vEmpleados vEmp = empleados.Where(x => x.IdEmpleado == i.IdEmpleado).FirstOrDefault();

                ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
                Cat_UnidadNegocio catUnidad = cu.getUnidadesnegocioId(periodo.IdUnidadNegocio);

                ClassConceptosFiniquitos ccf = new ClassConceptosFiniquitos();
                ConfiguracionConceptosFiniquito configuracion = ccf.GetConfiguracionConceptosFiniquitos(periodo.IdCliente);

                decimal cantidadAnt = 0;

                if (configuracion != null)
                {
                    if (configuracion.IdConceptoPV != null && configuracion.IdConceptoPV == i.IdConcepto)
                    {
                        ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                        ClassFechasCalculos cf = new ClassFechasCalculos();
                        var conf = cf.GetConfiguracionFechas(periodo.IdUnidadNegocio);
                        if (conf != null)
                        {
                            int IdPrestaciones = vEmp.IdPrestaciones ?? 1;
                            DateTime? FechaIngreso = null;
                            FechaIngreso = cpv.ObtenFechaCalculo(vEmp, conf, "Real");
                            decimal Antiguedad = periodo.FechaFin.Subtract((DateTime)FechaIngreso).Days / 365M;
                            cpv.GetDias(IdPrestaciones, Antiguedad);

                            if (concepto.SDPor != 0 && concepto.SDPor != null)
                            {
                                if (concepto.FactoryValor == "SI")
                                {
                                    cantidadAnt = i.Cantidad;
                                    i.Cantidad = i.Cantidad * (decimal)concepto.SDPor;
                                    concepto.SDPor = cpv.Porcentaje;
                                }
                            }
                            else
                                concepto.SDPor = cpv.Porcentaje;
                        }
                    }
                }

                bool guardar = false;
                switch (concepto.TipoDato)
                {
                    case "Cantidades":
                        if (i.Cantidad > 0)
                        {
                            decimal cantidadTemporal = i.Cantidad;
                            decimal? cantidadTemporalEsq = i.CantidadEsq;

                            if (catUnidad.DiasFraccioandos == "S" && catUnidad.FactorDiasFraccionados > 0 )
                            {
                                if (catUnidad.IdsConceptosFraccionados != null && catUnidad.IdsConceptosFraccionados.Length > 0)
                                {
                                    var clavesFraccionadas = catUnidad.IdsConceptosFraccionados.Replace(" ", "").Split(',').ToArray();

                                    if (clavesFraccionadas.Contains(i.ClaveConcepto))
                                    {
                                        i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                        i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                                    }
                                }
                                else
                                {
                                    i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                    i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                                }
                            }

                            if (concepto.CalculaMontos == "SI")
                            {
                                ObtenMontos(concepto, vEmp, i, null, null);
                                ObtenExentosGravados(concepto, periodo.FechaFin, null, i.Cantidad, vEmp.SDIMSS);

                                i.Monto = montoTradicional;
                                i.MontoEsquema = montoEsquema;
                            }

                            i.Cantidad = cantidadTemporal;
                            i.CantidadEsq = cantidadTemporalEsq;
                            guardar = true;
                        }
                        break;
                    case "Pesos":
                        try { montoEsquema = (decimal)i.MontoEsquema; } catch { montoEsquema = 0; }
                        if (i.Monto > 0 || montoEsquema > 0)
                        {
                            montoTradicional = i.Monto;                            
                            ObtenExentosGravados(concepto, periodo.FechaFin, null, null, vEmp.SDIMSS);
                            guardar = true;
                        }
                        break;
                    default:
                        break;
                }

                i.Exento = Exento;
                i.Gravado = Gravado;

                if (concepto.ClaveSAT == "999")
                {
                    i.Exento = 0;
                    i.Gravado = 0;
                }

                if (cantidadAnt > 0)
                    i.Cantidad = cantidadAnt;

                if (guardar)
                {
                    int IdIncidencia = AddIncidencias(i, IdUsuario);
                    if (concepto.CreaConceptoAdicional == "SI" && concepto.IdConceptoAdicional != null && concepto.IdConceptoAdicional != 0)
                        IncindenciaSecundaria(i, IdIncidencia, (int)concepto.IdConceptoAdicional, IdUsuario);
                }
            }
        }

        /// <summary>
        /// Metodo para agregar incidencias de manera paraetrizada para el proceso particular de vacaciones solictadas
        /// </summary>
        /// <param name="IdPeriodoNomina">Id del periodo que se esta calculando</param>
        /// <param name="IdConcepto">id del concepto de vacaciones asigndao al cliente</param>
        /// <param name="IdEmpleado">Id del empleado</param>
        /// <param name="Cantidad">Cantidad en dias de vacacioens para la incidencia</param>
        /// <param name="CantidadEsquema">Cantidad para dias de complemento</param>
        /// <param name="BanderaVacaciones">Bandera para identificar que son vacaciones</param>
        /// <param name="IdUsuario">Id del usuario</param>
        public void NewIncindencia(int IdPeriodoNomina, int IdConcepto, int IdEmpleado, decimal Cantidad, decimal CantidadEsquema, int BanderaVacaciones, int IdUsuario)
        {
            var i = new ModelIncidencias();
            var cconceptos = new ClassConceptos();
            var concepto = cconceptos.GetvConcepto(IdConcepto);

            var cempleado = new ClassEmpleado();
            var vEmp = cempleado.GetvEmpleado(IdEmpleado);

            var cperiodo = new ClassPeriodoNomina();
            var periodo = cperiodo.GetvPeriodoNominasId(IdPeriodoNomina);

            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            Cat_UnidadNegocio catUnidad = cu.getUnidadesnegocioId(periodo.IdUnidadNegocio);

            ClassConceptosFiniquitos ccf = new ClassConceptosFiniquitos();
            ConfiguracionConceptosFiniquito configuracion = ccf.GetConfiguracionConceptosFiniquitos(periodo.IdCliente);

            decimal cantidadAnt = 0;

            if (configuracion != null)
            {
                if (configuracion.IdConceptoPV != null && configuracion.IdConceptoPV == i.IdConcepto)
                {
                    ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                    ClassFechasCalculos cf = new ClassFechasCalculos();
                    var conf = cf.GetConfiguracionFechas(periodo.IdUnidadNegocio);
                    if (conf != null)
                    {
                        int IdPrestaciones = vEmp.IdPrestaciones ?? 1;
                        DateTime? FechaIngreso = null;
                        FechaIngreso = cpv.ObtenFechaCalculo(vEmp, conf, "Real");
                        decimal Antiguedad = periodo.FechaFin.Subtract((DateTime)FechaIngreso).Days / 365M;
                        cpv.GetDias(IdPrestaciones, Antiguedad);

                        if (concepto.SDPor != 0 && concepto.SDPor != null)
                        {
                            if (concepto.FactoryValor == "SI")
                            {
                                cantidadAnt = i.Cantidad;
                                i.Cantidad = i.Cantidad * (decimal)concepto.SDPor;
                                concepto.SDPor = cpv.Porcentaje;
                            }
                        }
                        else
                            concepto.SDPor = cpv.Porcentaje;
                    }
                }
            }

            i.IdConcepto = IdConcepto;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdEmpleado = IdEmpleado;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaVacaciones = BanderaVacaciones;

            bool guardar = false;
            switch (concepto.TipoDato)
            {
                case "Cantidades":
                    i.Cantidad = Cantidad;
                    i.CantidadEsq = CantidadEsquema;

                    if (Cantidad > 0 || CantidadEsquema > 0)
                    {
                        decimal cantidadTemporal = i.Cantidad;
                        decimal? cantidadTemporalEsq = i.CantidadEsq;

                        if (catUnidad.DiasFraccioandos == "S" && catUnidad.FactorDiasFraccionados > 0 )
                        {
                            if (catUnidad.IdsConceptosFraccionados != null && catUnidad.IdsConceptosFraccionados.Length > 0)
                            {
                                var clavesFraccionadas = catUnidad.IdsConceptosFraccionados.Replace(" ", "").Split(',').ToArray();

                                if (clavesFraccionadas.Contains(i.ClaveConcepto))
                                {
                                    i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                    i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                                }
                            }
                            else
                            {
                                i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                            }
                        }

                        i.Monto = 0;
                        i.MontoEsquema = 0;

                        if (concepto.CalculaMontos == "SI")
                        {
                            ObtenMontos(concepto, vEmp, i, null, null);
                            ObtenExentosGravados(concepto, periodo.FechaFin, null, Cantidad, vEmp.SDIMSS);

                            i.Monto = montoTradicional;
                            i.MontoEsquema = montoEsquema;
                        }

                        i.Cantidad = cantidadTemporal;
                        i.CantidadEsq = cantidadTemporalEsq;
                        guardar = true;
                    }
                    break;

                default:
                    break;
            }

            i.Exento = Exento;
            i.Gravado = Gravado;

            if (concepto.ClaveSAT == "999")
            {
                i.Exento = 0;
                i.Gravado = 0;
            }

            if (cantidadAnt > 0)
                i.Cantidad = cantidadAnt;

            if (guardar)
            {
                int IdIncidencia = AddIncidencias(i, IdUsuario);
                if (concepto.CreaConceptoAdicional == "SI" && concepto.IdConceptoAdicional != null && concepto.IdConceptoAdicional != 0)
                    IncindenciaSecundaria(i, IdIncidencia, (int)concepto.IdConceptoAdicional, IdUsuario);
            }
        }


        /// <summary>
        /// Metodo para borrar las incidencias que se crearon por otra incidencia.
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <param name="IdEmpleado"></param>
        /// <param name="IdConcepto"></param>        
        public void DeleteIncidenciaSecundaria(int IdPeriodo, int IdEmpleado, int IdConcepto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var inc = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEmpleado == IdEmpleado && x.IdConcepto == IdConcepto && x.BanderaIncidencia != null).ToList();

                entidad.Incidencias.RemoveRange(inc);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para borrar las incidencias que se crearon por otra incidencia.
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <param name="IdEmpleado"></param>
        /// <param name="IdConcepto"></param>        
        public void DeleteIncidenciaSecundaria(int IdBandera)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var inc = entidad.Incidencias.Where(x => x.BanderaIncidencia == IdBandera).ToList();

                entidad.Incidencias.RemoveRange(inc);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para agregar nuevas incidencias que se generan de conceptos configurados para la creacion de otra incidencia
        /// </summary>
        /// <param name="i">ModelIncidencias</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void IncindenciaSecundaria(ModelIncidencias i, int IdIncidencia, int IdConcepto, int IdUsuario)
        {
            DeleteIncidenciaSecundaria(i.IdPeriodoNomina, i.IdEmpleado, IdConcepto);
            i.IdConcepto = IdConcepto;
            ClassConceptos cconceptos = new ClassConceptos();
            vConceptos concepto = cconceptos.GetvConcepto(i.IdConcepto);

            ClassEmpleado cempleado = new ClassEmpleado();
            vEmpleados vEmp = cempleado.GetvEmpleado(i.IdEmpleado);

            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            vPeriodoNomina periodo = cperiodo.GetvPeriodoNominasId(i.IdPeriodoNomina);

            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            Cat_UnidadNegocio catUnidad = cu.getUnidadesnegocioId(periodo.IdUnidadNegocio);

            ClassConceptosFiniquitos ccf = new ClassConceptosFiniquitos();
            ConfiguracionConceptosFiniquito configuracion = ccf.GetConfiguracionConceptosFiniquitos(periodo.IdCliente);

            decimal cantidadAnt = 0;

            if (configuracion != null)
            {
                if (configuracion.IdConceptoPV != null && configuracion.IdConceptoPV == i.IdConcepto)
                {
                    ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                    ClassFechasCalculos cf = new ClassFechasCalculos();
                    var conf = cf.GetConfiguracionFechas(periodo.IdUnidadNegocio);
                    if (conf != null)
                    {
                        int IdPrestaciones = vEmp.IdPrestaciones ?? 1;
                        DateTime? FechaIngreso = null;
                        FechaIngreso = cpv.ObtenFechaCalculo(vEmp, conf, "Real");
                        decimal Antiguedad = periodo.FechaFin.Subtract((DateTime)FechaIngreso).Days / 365M;
                        cpv.GetDias(IdPrestaciones, Antiguedad);

                        if (concepto.SDPor != 0 && concepto.SDPor != null)
                        {
                            if (concepto.FactoryValor == "SI")
                            {
                                cantidadAnt = i.Cantidad;
                                i.Cantidad = i.Cantidad * (decimal)concepto.SDPor;
                                concepto.SDPor = cpv.Porcentaje;
                            }
                        }
                        else
                            concepto.SDPor = cpv.Porcentaje;

                    }
                }
            }

            if (concepto.MultiplicaDT == "SI")
            {
                concepto.TipoDato = "Cantidades";
                concepto.CalculaMontos = "NO";
            }

            bool guardar = false;
            Exento = 0;
            Gravado = 0;
            switch (concepto.TipoDato)
            {
                case "Cantidades":
                    if (i.Cantidad > 0 || i.CantidadEsq > 0)
                    {
                        decimal cantidadTemporal = i.Cantidad;
                        decimal? cantidadTemporalEsq = i.CantidadEsq;

                        if (catUnidad.DiasFraccioandos == "S" && catUnidad.FactorDiasFraccionados > 0)
                        {
                            if (catUnidad.IdsConceptosFraccionados != null && catUnidad.IdsConceptosFraccionados.Length > 0)
                            {
                                var clavesFraccionadas = catUnidad.IdsConceptosFraccionados.Replace(" ", "").Split(',').ToArray();

                                if (clavesFraccionadas.Contains(i.ClaveConcepto))
                                {
                                    i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                    i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                                }
                            }
                            else
                            {
                                i.Cantidad = i.Cantidad * (decimal)catUnidad.FactorDiasFraccionados;
                                i.CantidadEsq = i.CantidadEsq = (decimal)catUnidad.FactorDiasFraccionados;
                            }
                        }

                        i.Monto = 0;
                        i.MontoEsquema = 0;

                        if (concepto.CalculaMontos == "SI")
                        {
                            ObtenMontos(concepto, vEmp, i, null, null);
                            ObtenExentosGravados(concepto, periodo.FechaFin, null, i.Cantidad, vEmp.SDIMSS);

                            i.Monto = montoTradicional;
                            i.MontoEsquema = montoEsquema;
                        }

                        i.Cantidad = cantidadTemporal;
                        i.CantidadEsq = cantidadTemporalEsq;
                        guardar = true;
                    }
                    break;
                case "Pesos":
                    i.Cantidad = 0;

                    if (i.Monto > 0)
                    {
                        montoTradicional = i.Monto;
                        ObtenExentosGravados(concepto, periodo.FechaFin, null, null, vEmp.SDIMSS);
                        guardar = true;
                    }

                    if (i.MontoEsquema > 0)
                    {
                        if (concepto.TipoEsquema == "Esquema" || concepto.TipoEsquema == "Mixto")
                        {
                            i.MontoEsquema = i.MontoEsquema;
                        }
                        else
                        {
                            i.MontoEsquema = 0;
                        }
                        guardar = true;
                    }
                    break;
                default:
                    break;
            }

            i.Exento = Exento;
            i.Gravado = Gravado;

            if (concepto.ClaveSAT == "999")
            {
                i.Exento = 0;
                i.Gravado = 0;
            }

            if (cantidadAnt > 0)
                i.Cantidad = cantidadAnt;

            if (guardar)
            {
                i.BanderaIncidencia = IdIncidencia;
                AddIncidencias(i, IdUsuario);
            }
        }

        /// <summary>
        /// Método para modificar el campo de exento y gravado de las incidencias considerando el Factor de los dias trabajados
        /// </summary>
        /// <param name="Incidencia">Indentificador de la incidencia</param>
        /// <param name="FactorDiasTrabajadosEjercicio">Valor proporcional de los dias trabajados durante el periodo por el empleado siendo 1 por los trabajar todos los días del periodo</param>
        /// <param name="FechaFinPeriodo">Fecha del fin del periodo de nómina</param>
        public void UpdateExentosGravados(int Incidencia, decimal FactorDiasTrabajadosEjercicio, DateTime FechaFinPeriodo, decimal? SDIMSS)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.Incidencias.Where(x => x.IdIncidencia == Incidencia) select b).FirstOrDefault();

                if(registro != null)
                {
                    ClassConceptos cconceptos = new ClassConceptos();
                    try { montoTradicional = (decimal)registro.Monto; } catch { montoTradicional = 0; }
                    ObtenExentosGravados(cconceptos.GetvConcepto((int)registro.IdConcepto), FechaFinPeriodo, FactorDiasTrabajadosEjercicio, registro.Cantidad, SDIMSS);

                    registro.Exento = Exento;
                    registro.Gravado = Gravado;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para modificar el campo de exento y gravado de las incidencias pudiendo no considerar el Factor de los dias trabajados
        /// </summary>
        /// <param name="Incidencia">Identificador de la incidencia</param>
        /// <param name="Monto">Monto</param>
        /// <param name="FactorDiasTrabajadosEjercicio">Valor proporcional de los dias trabajados durante el periodo por el empleado siendo 1 por los trabajar todos los días del periodo</param>
        /// <param name="FechaFinPeriodo">Fecha del fin del periodo de nómina</param>
        public void UpdateExentosGravados(int Incidencia, decimal Monto, decimal? FactorDiasTrabajadosEjercicio, DateTime FechaFinPeriodo, decimal? SDIMSS)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.Incidencias.Where(x => x.IdIncidencia == Incidencia) select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.Monto = Monto;
                    ClassConceptos cconceptos = new ClassConceptos();
                    montoTradicional = Monto;
                    ObtenExentosGravados(cconceptos.GetvConcepto((int)registro.IdConcepto), FechaFinPeriodo, FactorDiasTrabajadosEjercicio, registro.Cantidad, SDIMSS);

                    registro.Exento = Exento;
                    registro.Gravado = Gravado;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para obtener el monto del Salario diario
        /// </summary>
        /// <param name="concepto">Información del concepto</param>
        /// <param name="vEmp">Información del empleado</param>
        /// <param name="i">Información de la incidencia</param>
        /// <param name="Porcentaje"></param>
        /// <param name="SDCalculo">Salario diario</param>
        private void ObtenMontos(vConceptos concepto, vEmpleados vEmp, ModelIncidencias i, decimal? Porcentaje, decimal? SDCalculo)
        {
            decimal Por = 1;
            decimal Entre = 1;
            decimal SD = 0;
            decimal SDIMSS = 0;
            try
            {
                if (concepto.SDPor > 0)
                {
                    Por = (decimal)concepto.SDPor;
                }

                if (Porcentaje != null && Porcentaje > 0)
                {
                    Por = (decimal)Porcentaje;
                }
            }
            catch { Por = 1; }
            try 
            { 
                if (concepto.SDEntre > 0) { Entre = (decimal)concepto.SDEntre; }

                if (concepto.ClaveSAT == "019" && vEmp.Horas > 0 )
                    Entre = (decimal)vEmp.Horas;
                
            } 
            catch { Entre = 1; }
            try { SD = (decimal)vEmp.SD; } catch { SD = 0; }
            try { SDIMSS = (decimal)vEmp.SDIMSS; } catch { SDIMSS = 0; }
            if (SDCalculo != null)
                SDIMSS = (decimal)SDCalculo;

            if (SD == 0)
            {
                SD = SDIMSS;
            }
            
            montoReal = SD * Por;
            montoReal = montoReal / Entre;
            montoReal = montoReal * i.Cantidad;

            montoTradicional = SDIMSS * Por;
            montoTradicional = montoTradicional / Entre;

            if (concepto.TipoEsquema == "Mixto" || concepto.TipoEsquema == "Tradicional")
            {
                montoTradicional = montoTradicional * i.Cantidad;
            }
            else
            {
                montoTradicional = 0;
            }

            if (concepto.TipoEsquema == "Mixto" || concepto.TipoEsquema == "Esquema")
            {
                if (montoTradicional == 0)
                {
                    decimal SDEsquema = SD - SDIMSS;

                    if (SDEsquema < 0) { SDEsquema = 0; }
                    if (concepto.TipoEsquema == "Esquema") { SDEsquema = SD; }
                    montoEsquema = SDEsquema * Por;
                    montoEsquema = montoEsquema / Entre;

                    montoEsquema = montoEsquema * i.Cantidad;
                }
                else
                {
                    montoEsquema = montoReal - montoTradicional;
                    if (montoEsquema < 0) { montoEsquema = 0; }
                }
            }
            else
            {
                montoEsquema = 0;
            }
        }

        /// <summary>
        /// Método para obtener el monto del Salario diario real
        /// </summary>
        /// <param name="concepto">Información del concepto</param>
        /// <param name="vEmp">Información del empleado</param>
        /// <param name="Cantidad">cantidad del concepto</param>
        /// <param name="cantidadEsquema">cantidad del concepto esquema</param>
        /// <param name="Porcentaje"></param>
        /// <param name="SDCalculo">salrio diario</param>
        /// <param name="SDCalculoReal">salario diario real</param>
        private void ObtenMontos(vConceptos concepto, vEmpleados vEmp, decimal Cantidad, decimal? cantidadEsquema, decimal? Porcentaje, decimal? SDCalculo, decimal? SDCalculoReal)
        {
            decimal Por = 1;
            decimal Entre = 1;
            decimal SD = 0;
            decimal SDIMSS = 0;
            decimal _cantidadEsquema = Cantidad;
            try { _cantidadEsquema = (decimal)cantidadEsquema; } catch { }
            if(Cantidad > 0 && _cantidadEsquema == 0)
                _cantidadEsquema = Cantidad;

            try
            {   
                if(Porcentaje != null && Porcentaje > 0)                
                    Por = (decimal)Porcentaje;                

                if (concepto.SDPor > 0)                
                    Por = (decimal)concepto.SDPor;                
            }
            catch { Por = 1; }
            try
            {
                if (concepto.SDEntre > 0)
                    Entre = (decimal)concepto.SDEntre;

                if (concepto.ClaveSAT == "019" && vEmp.Horas > 0)
                    Entre = (decimal)vEmp.Horas;
            }
            catch { Entre = 1; }
            try { SD = (decimal)vEmp.SD; } catch { SD = 0; }
            try { SDIMSS = (decimal)vEmp.SDIMSS; } catch { SDIMSS = 0; }

            if (SDCalculo != null)
                SDIMSS = (decimal)SDCalculo;

            if (SD == 0)            
                SD = SDIMSS;            

            if (SDCalculoReal != null)
                SD = (decimal)SDCalculoReal;

            montoReal = SD * Por;
            montoReal = montoReal / Entre;
            montoReal = montoReal * _cantidadEsquema;

            montoTradicional = SDIMSS * Por;
            montoTradicional = montoTradicional / Entre;

            if (concepto.TipoEsquema == "Mixto" || concepto.TipoEsquema == "Tradicional")            
                montoTradicional = montoTradicional * Cantidad;            
            else            
                montoTradicional = 0;            

            if (concepto.TipoEsquema == "Mixto" || concepto.TipoEsquema == "Esquema")
            {
                if (montoTradicional == 0)
                {
                    decimal SDEsquema = SD - SDIMSS;
                    if (concepto.TipoEsquema == "Esquema") { SDEsquema = SD; }
                    montoEsquema = SDEsquema * Por;
                    montoEsquema = montoEsquema / Entre;

                    montoEsquema = montoEsquema * _cantidadEsquema;
                }
                else                
                    montoEsquema = montoReal - montoTradicional;                
            }
            else            
                montoEsquema = 0;            
        }

        /// <summary>
        /// Método para obtener la parte extenta y la parte gravada de los conceptos de nómina 
        /// </summary>
        /// <param name="concepto">Información del concepto de nómina</param>
        /// <param name="fechaFinPeriodo">Fecha din del periodeo de nómina</param>
        /// <param name="FactorDiasTrabajadosEjercicio">Valor proporcional de los dias trabajados durante el periodo por el empleado siendo 1 por los trabajar todos los días del periodo</param>
        private void ObtenExentosGravados(vConceptos concepto, DateTime fechaFinPeriodo, decimal? FactorDiasTrabajadosEjercicio, decimal? Cantidad, decimal? SDIMSS)
        {
            if (concepto.TipoConcepto != "DD")
            {
                if (concepto.Exenta == "SI")
                {
                    GetSueldosMinimos(fechaFinPeriodo);
                    decimal unidadExenta = 0;
                    decimal exentoConcepto = 0;
                    decimal porcentajeGravado = 0;
                    decimal montoEvaluado = montoTradicional;
                    decimal _exento = 0;
                    decimal _gravado = 0;
                    decimal aux = 0;
                    decimal _exentoConcepto = 0;

                    unidadExenta = GetUnidadExenta(concepto);
                    decimal cantidad = Cantidad != null && Cantidad != 0 && concepto.ExentaPorUnidad == "SI" ? (decimal)Cantidad : 1;
                    _exentoConcepto = ((decimal)concepto.CantidadExenta * unidadExenta) * cantidad;
                    if (FactorDiasTrabajadosEjercicio != null && FactorDiasTrabajadosEjercicio > 0)                    
                        exentoConcepto = _exentoConcepto * (decimal)FactorDiasTrabajadosEjercicio;                    
                    else
                        exentoConcepto = _exentoConcepto;

                    porcentajeGravado = (decimal)concepto.PorcentajeGravado * .01M;

                    if (porcentajeGravado > 0)
                    {
                        _gravado = montoEvaluado * porcentajeGravado;
                        montoEvaluado = montoEvaluado = _gravado;
                    }

                    if (exentoConcepto >= montoEvaluado)
                    {
                        _exento = montoEvaluado;
                        _gravado = montoTradicional - _exento;
                    }
                    else
                    {
                        aux = montoEvaluado - exentoConcepto;
                        _gravado += aux;
                        _exento = exentoConcepto;
                    }

                    if (SDIMSS <= SMGV && concepto.ExentoPorSueldoMinimo == "SI")
                    {
                        _exento = montoTradicional;
                        _gravado = 0;
                    }

                    Exento = _exento;
                    Gravado = _gravado;
                }
                else
                {
                    Exento = 0;
                    Gravado = montoTradicional;
                }
            }
            else
            {
                Exento = montoTradicional;
                Gravado = 0;
            }
        }

        /// <summary>
        /// Método para obtener el valor que excenta el concepto según el sat
        /// </summary>
        /// <param name="concepto">Información del concepto</param>
        /// <returns>valor que excenta el concepto según el sat</returns>
        private decimal GetUnidadExenta(vConceptos concepto)
        {
            decimal unidadExenta;
            switch (concepto.UnidadExenta.Trim())
            {
                case "UMA":
                    unidadExenta = UMA;
                    break;
                case "SMGV":
                    unidadExenta = SMGV;
                    break;
                default:
                    unidadExenta = 0;
                    break;
            }

            return unidadExenta;
        }

        /// <summary>
        /// Método para sueldos minimos vigentes
        /// </summary>
        /// <param name="fechaFinPeriodo"></param>
        private void GetSueldosMinimos(DateTime fechaFinPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int año = fechaFinPeriodo.Year;
                var sueldos_minimos = (from b in entidad.Sueldos_Minimos.Where(x => x.Ano == año && fechaFinPeriodo >= x.FechaInicio)
                                       select b).OrderByDescending(x => x.FechaInicio).FirstOrDefault();

                if (sueldos_minimos != null)
                {
                    UMA = (decimal)sueldos_minimos.UMA;
                    SMGV = (decimal)sueldos_minimos.SalarioMinimoGeneral;
                }
                else
                {
                    UMA = 0;
                    SMGV = 0;
                }
            }
        }

        /// <summary>
        /// Método para eliminar la incidencia de la base de datos
        /// </summary>
        /// <param name="Id">Identificador de la incidencia</param>
        /// <param name="token">token para acceso a los servicios de la api</param>
        public void DeleteIncidencia(int Id, string token)
        {
            var si = new sIncidencias();
            si.DeleteIncindencia(Id, token);
            DeleteIncidenciaSecundaria(Id);
        }

        /// <summary>
        /// Método para eliminar varias incidencias
        /// </summary>
        /// <param name="Ids">Listado de los identificadores de las incidencias</param>
        public void DeleteIncidencia(int[] Ids)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = (from b in entidad.Incidencias.Where(x => Ids.Contains(x.IdIncidencia) ) select b).ToList();

                if (incidencia.Count > 0)
                {
                    entidad.Incidencias.RemoveRange(incidencia);
                    entidad.SaveChanges();
                }
            }
        }

        public void DeleteIncidencia(int IdConcepto, int IdEmpleado, int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = entidad.Incidencias.Where(x => x.IdConcepto == IdConcepto && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina).ToList();

                if (incidencia.Count > 0)
                {
                    entidad.Incidencias.RemoveRange(incidencia);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para obtener la sincidencias por empleado presentadas en el periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        public void DeleteAllEmpleado(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEmpleado == IdEmpleado).ToList();

                entidad.Incidencias.RemoveRange(incidencia);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener las incidencias del empleado par acalcular su finiquito 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <param name="IdEmpleado">Idnetificador del empleado</param>
        public void DeleteAllEmpleadoFinquitos(int IdPeriodo, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEmpleado == IdEmpleado && x.BanderaFiniquitos == 1) select b).ToList();

                entidad.Incidencias.RemoveRange(incidencia);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener el detalle de todas las incidencias del periódo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador delperiodo de nómina</param>
        /// <param name="token">token para acceder a los servicios de la api</param>
        public void DeleteAll(int IdPeriodoNomina, string token)
        {
            var si = new sIncidencias();
            si.DeleteAllIncindencia(IdPeriodoNomina, token);
        }

        /// <summary>
        /// Método para eliminar todos las incidencias de un concepto en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdConcepto">Identificador del concepto</param>
        public void DeleteAllIdConcepto(int IdPeriodoNomina, int IdConcepto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdConcepto == IdConcepto) select b);

                entidad.Incidencias.RemoveRange(incidencia);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para eliminar todas las incidencias de un periodd de nomina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        public void DeleteAllFiniquitos(int IdPeriodoNomina, List<int> IdsEmpleados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencia = (entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.BanderaFiniquitos == 1 && IdsEmpleados.Contains((int)x.IdEmpleado))).ToList();

                entidad.Incidencias.RemoveRange(incidencia);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener un listado de las incidencias de un archivo con formato .csv
        /// </summary>
        /// <param name="ruta">Ruta del archivo</param>
        /// <returns>Listado de las incidencias capturadas en el archivo</returns>
        public ArrayList GetArrayIncidencias(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;
            int contador = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (contador > 0)
                    {
                        arrText.Add(sLine);
                    }
                    contador++;
                }
            }

            objReader.Close();
            return arrText;
        }

        /// <summary>
        /// Método para validar que los campos de incidencias capturados en el archivo sean corretos
        /// </summary>
        /// <param name="ruta">ruta del archivo</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <returns>listados con los campos correctos e incorrectos</returns>
        public ModelErroresIncidencias GetIncidenciasArchivo(string ruta, int IdCliente, int IdUnidadNegocio, int IdPeriodoNomina, string TipoPeriodo, int IdUsuario)
        {
            ModelErroresIncidencias modelErrores = new ModelErroresIncidencias();
            modelErrores.listErrores = new List<string>();
            modelErrores.Correctos = 0;
            modelErrores.Errores = 0;
            modelErrores.noRegistro = 0;
            modelErrores.Path = Path.GetFileName(ruta);
            
            ArrayList array = GetArrayIncidencias(ruta);
            List<ModelIncidencias> incidencias = new List<ModelIncidencias>();

            ClassConceptos cconceptos = new ClassConceptos();
            List<vConceptos> lconcepto = cconceptos.GetvConceptos(IdCliente);

            ClassEmpleado cempleado = new ClassEmpleado();
            List<vEmpleados> vEmp = cempleado.GetAllvEmpleados(IdUnidadNegocio);

            if (TipoPeriodo != "Complemento")
                vEmp = vEmp.Where(x => x.IdEstatus != 2).ToList();
            
            foreach (var item in array)
            {
                modelErrores.noRegistro++;
                AddRegidtroIncidencia(IdPeriodoNomina, modelErrores, incidencias, lconcepto, vEmp, item);
            }

            try { NewIncindencia(incidencias, lconcepto, vEmp, IdUsuario); } catch(Exception ex) { modelErrores.listErrores.Add(ex.ToString()); }
                        
            return modelErrores;
        }

        /// <summary>
        /// Método para validar que los campos de incidencias capturados en el archivo sean corretos
        /// </summary>
        /// <param name="ruta">ruta del archivo</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <returns>listados con los campos correctos e incorrectos</returns>
        public ModelErroresIncidencias GetIncidenciasArchivodos(string ruta, int IdCliente, int IdUnidadNegocio, int IdPeriodoNomina, string TipoPeriodo, int IdUsuario) 
        {
            ModelErroresIncidencias modelErrores = new ModelErroresIncidencias();
            modelErrores.listErrores = new List<string>();
            modelErrores.Correctos = 0;
            modelErrores.Errores = 0;
            modelErrores.noRegistro = 0;
            modelErrores.Path = Path.GetFileName(ruta);
            ArrayList array = GetArrayIncidencias(ruta);
            List<ModelIncidencias> incidencias = new List<ModelIncidencias>();
            ClassConceptos cconceptos = new ClassConceptos();
            List<vConceptos> lconcepto = cconceptos.GetvConceptos(IdCliente);
            ArrayList arrayConcepto = GetArrayConceptos(ruta);
            object itemConcepto = string.Empty;
            if (arrayConcepto.Count > 0)
            {
                itemConcepto = arrayConcepto[0];
            }
            ClassEmpleado cempleado = new ClassEmpleado();
            List<vEmpleados> vEmp = cempleado.GetAllvEmpleados(IdUnidadNegocio);

            if (TipoPeriodo != "Complemento")
                vEmp = vEmp.Where(x=> x.IdEstatus != 2).ToList();

            foreach (var item in array)
            {
                modelErrores.noRegistro++;
                AddRegidtroIncidenciados(IdPeriodoNomina, modelErrores, incidencias, lconcepto, vEmp, item, itemConcepto);
            }

            try
            {
                NewIncindencia  (incidencias, lconcepto, vEmp, IdUsuario);
            }
            catch (Exception ex)
            {
                modelErrores.listErrores.Add(ex.ToString());
            }

            return modelErrores;
        }

        public class Mapeo
        {
            public string Campo { get; set; }
            public string Concepto { get; set; }
            public string Valor { get; set; }
        }

        /// <summary>
        /// Método para incertar las incidencias a la base de datos
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="errores">listado de errores encontrados</param>
        /// <param name="incidencias">listado de las incidencias por cargar</param>
        /// <param name="lconcepto">listado de conceptos</param>
        /// <param name="vEmp">listado de la información de los empleados</param>
        /// <param name="item">contenido del archivo</param>
        /// <param name="itemConcepto">listado de la información de los conceptos</param>
        private void AddRegidtroIncidenciados(int IdPeriodoNomina, ModelErroresIncidencias errores, List<ModelIncidencias> incidencias, List<vConceptos> lconcepto, List<vEmpleados> vEmp, object item, object itemConcepto)
        {
            string[] campos = item.ToString().Split(',');
            List<string> conceptos = itemConcepto.ToString().Split(',').ToList();
            List<Mapeo> valores = new List<Mapeo>();
            for (int i = 3; i < campos.Length; i++)
            {
                valores.Add(new Mapeo { Campo = campos[i], Concepto = conceptos[i], Valor = campos[i] });
            }

            decimal Cantidad = 0;
            var datos1 = valores.AsEnumerable().Where(x => x.Valor != "").ToList();
            foreach (Mapeo datos in valores.AsEnumerable().Where(x => x.Valor != "").ToList())
            {
                ModelIncidencias i = new ModelIncidencias();

                decimal Monto = 0;
                decimal MontoEsq = 0;
                int IdEmpleado = vEmp.Where(x => x.ClaveEmpleado.Trim() == campos[0].Trim()).Select(x => x.IdEmpleado).FirstOrDefault();
                int IdConcepto = lconcepto.Where(x => x.IdConcepto.ToString() == datos.Concepto.Split('-')[0]).Select(x => x.IdConcepto).FirstOrDefault();
                string TipoDato = lconcepto.Where(x => x.IdConcepto.ToString() == datos.Concepto.Split('-')[0]).Select(x => x.TipoDato).FirstOrDefault();
                string TipoEsquema = lconcepto.Where(x => x.IdConcepto.ToString() == datos.Concepto.Split('-')[0]).Select(x => x.TipoEsquema).FirstOrDefault();
                i.IdEmpleado = IdEmpleado;
                i.IdPeriodoNomina = IdPeriodoNomina;
                i.IdConcepto = IdConcepto;
                if (IdEmpleado != 0)
                {
                    if (TipoDato == "Cantidades")
                    {
                        try { Cantidad = decimal.Parse(datos.Valor); } catch { Cantidad = 0; }
                        i.Cantidad = Cantidad;

                    }
                    else if (TipoDato == "Pesos")
                    {
                        if (datos.Concepto.Contains("T"))
                        {
                            try { Monto = decimal.Parse(datos.Valor); } catch { Monto = 0; }
                            i.Monto = Monto;
                        }
                        else if (datos.Concepto.Contains("S"))
                        {
                            try { MontoEsq = decimal.Parse(datos.Valor); } catch { MontoEsq = 0; }
                            i.MontoEsquema = MontoEsq;
                        }
                    }
                    incidencias.Add(i);

                }
                else
                {


                }
            }

        }

        /// <summary>
        /// Método para obtener un listado de los conceptos de un archivo con formato .csv
        /// </summary>
        /// <param name="ruta">Ruta del archivo</param>
        /// <returns>Listado de los conceptos capturadas en el archivo</returns>
        public ArrayList GetArrayConceptos(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;

            sLine = objReader.ReadLine();
            arrText.Add(sLine);
            objReader.Close();
            return arrText;
        }

        /// <summary>
        /// Método para agregar una incidencia en el periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="errores">listado de errores encontrados en el archivo</param>
        /// <param name="incidencias">listado de incidencias por cargar</param>
        /// <param name="lconcepto">listado de la información de los conceptos</param>
        /// <param name="vEmp">lisyado de la información de os empleados</param>
        /// <param name="item"></param>
        private void AddRegidtroIncidencia(int IdPeriodoNomina, ModelErroresIncidencias errores, List<ModelIncidencias> incidencias, List<vConceptos> lconcepto, List<vEmpleados> vEmp, object item)
        {
            string[] campos = item.ToString().Split(',');

            int IdEmpleado = vEmp.Where(x => x.ClaveEmpleado.Trim() == campos[0].Trim()).Select(x => x.IdEmpleado).FirstOrDefault();
            int IdConcepto = lconcepto.Where(x => x.ClaveConcepto.Trim() == campos[1].Trim()).Select(x => x.IdConcepto).FirstOrDefault();
            string TipoDato = lconcepto.Where(x => x.ClaveConcepto.Trim() == campos[1].Trim()).Select(x => x.TipoDato).FirstOrDefault(); 
            string TipoEsquema = lconcepto.Where(x => x.ClaveConcepto.Trim() == campos[1].Trim()).Select(x => x.TipoEsquema).FirstOrDefault();
            decimal Cantidad = 0;
            decimal Monto = 0;
            decimal MontoEsq = 0;
            DateTime? FechaInicio = null;
            DateTime? FechaFin = null;
            string Folio = string.Empty;

            if (TipoDato == "Cantidades")
                try { Cantidad = decimal.Parse(campos[2]); } catch { Cantidad = 0; }
            else if (TipoDato == "Pesos")
            {
                try { Monto = decimal.Parse(campos[3]); } catch { Monto = 0; }
                try { MontoEsq = decimal.Parse(campos[4]); } catch { MontoEsq = 0; }
            }

            try { FechaInicio = DateTime.Parse(campos[5]); } catch { FechaInicio = null; }
            try { FechaFin = DateTime.Parse(campos[6]); } catch { FechaFin = null; }
            Folio = campos[7];

            errores.listErrores.AddRange(ValidaCamposArchivo(IdEmpleado, IdConcepto, campos, errores.noRegistro, TipoDato, TipoEsquema));

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                ModelIncidencias i = new ModelIncidencias();
                i.IdEmpleado = IdEmpleado;
                i.IdPeriodoNomina = IdPeriodoNomina;
                i.IdConcepto = IdConcepto;
                i.Cantidad = Cantidad;
                i.Monto = Monto;
                i.MontoEsquema = MontoEsq;
                i.FechaIncio = FechaInicio;
                i.FechaFinal = FechaFin;
                i.Folio = Folio;

                incidencias.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

        /// <summary>
        /// Método para validar los campos necesarios para cargar las incidencias en el archivo
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdConcepto">Identificador del empleado</param>
        /// <param name="campos">Compos a validar</param>
        /// <param name="NoRegistro">Numero de registro</param>
        /// <param name="tipoDato">tipo de dato a validar</param>
        /// <param name="TipoEsquema">Tipo de esquema de pago</param>
        /// <returns></returns>
        public List<string> ValidaCamposArchivo(int IdEmpleado, int IdConcepto, string[] campos, int NoRegistro, string tipoDato, string TipoEsquema)
        {
            List<string> errores = new List<string>();
            string Mensaje = string.Empty;            

            if (IdEmpleado == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[0] + " - La clave de empleado que ingreso no corresponde a ningun empleados para esta nómina.";
                errores.Add(Mensaje);
            }
            
            if (IdConcepto == 0)
            {
                Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[1] + " - La clave de concepto que ingreso no corresponde a ningun concepto para esta nómina.";
                errores.Add(Mensaje);
            }

            if (tipoDato == "Cantidades")
            {
                decimal cantidad = 0;
                try { cantidad = decimal.Parse(campos[2]); } catch { }

                if (cantidad == 0)
                {
                    Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[2] + " - El valor que ingreso, no es correcto.";
                    errores.Add(Mensaje);
                }
            }

            if (tipoDato == "Pesos")
            {
                decimal mnto = 0;
                decimal mntoEsq = 0;
                try { mnto = decimal.Parse(campos[3]); } catch { }
                try { mntoEsq = decimal.Parse(campos[4]); } catch { }

                if (mnto == 0 && mntoEsq == 0)
                {
                    Mensaje = "Referencia: Registro - " + NoRegistro + ", Error - " + campos[3] + " | " + campos[4] + " - El valor que ingreso, no es correcto.";
                    errores.Add(Mensaje);
                }
            }           

            return errores;
        }

        /// <summary>
        /// Método para eliminar incidencias de los empleados del periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="IdsEmpleados">Identificadores de los empleados</param>
        public void DeleteIncidenciasAguinaldo(int IdPeriodoNomina, int[] IdsEmpleados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registros = (entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.BanderaAguinaldos == 1 && IdsEmpleados.Contains((int)x.IdEmpleado))).ToList();

                entidad.Incidencias.RemoveRange(registros);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener los tipos de nómina por unidad de negocío
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>listado de los tipos de nómina</returns>
        public ModelIncidencias FindListIncidencias(int IdUnidadNegocio)
        {
            List<SelectListItem> _TipoNomima = new List<SelectListItem>();
            _TipoNomima.Add(new SelectListItem { Text = "Formato 1", Value = "1" });
            _TipoNomima.Add(new SelectListItem { Text = "Formato 2", Value = "2" });


            ModelIncidencias model = new ModelIncidencias
            {
                Lformato = _TipoNomima,

            };

            return model;
        }

        /// <summary>
        /// Método para modificar los montos de las incidencias
        /// </summary>
        /// <param name="IdIncidencia">Identificador de la incidencia</param>
        /// <param name="NuevoMonto">valor del nuevo monto</param>
        public void updateMontoEsquemaAguinado(int IdIncidencia, decimal NuevoMonto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var inc = entidad.Incidencias.Where(x => x.IdIncidencia == IdIncidencia).FirstOrDefault();

                if (inc != null)
                {
                    inc.MontoEsquema = NuevoMonto;

                    entidad.SaveChanges();
                }
            }
        }

        public void EliminaMontosAusentismos(int IdIncidencia)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var inc = entidad.Incidencias.Where(x => x.IdIncidencia == IdIncidencia).FirstOrDefault();

                if (inc != null)
                {
                    inc.Monto = 0;
                    inc.Exento = 0;
                    inc.Gravado = 0;

                    entidad.SaveChanges();
                }
            }
        }

        public ModelErroresIncidencias GetIncidenciasArchivoHonorarios(string ruta, int IdUnidadNegocio, int IdPeriodoNomina, int IdUsuario)
        {

            cHonorarios cl = new cHonorarios();

            ModelErroresIncidencias modelErrores = new ModelErroresIncidencias();
            modelErrores.listErrores = new List<string>();
            modelErrores.Correctos = 0;
            modelErrores.Errores = 0;
            modelErrores.noRegistro = 0;
            modelErrores.Path = Path.GetFileName(ruta);

            ArrayList array = GetArrayIncidencias(ruta);
            List<mHonorarios> incidencias = new List<mHonorarios>();

            ClassEmpleado cempleado = new ClassEmpleado();
            List<vEmpleados> vEmp = cempleado.GetAllvEmpleados(IdUnidadNegocio);



            foreach (var item in array)
            {
                modelErrores.noRegistro++;
                AddRegidtroIncidenciaHonorarios(IdPeriodoNomina, modelErrores, incidencias, vEmp, item);
            }

            try { cl.GuardarHonorariosLista(incidencias, IdUsuario); } catch (Exception ex) { modelErrores.listErrores.Add(ex.ToString()); }

            return modelErrores;
        }

        private void AddRegidtroIncidenciaHonorarios(int IdPeriodoNomina, ModelErroresIncidencias errores, List<mHonorarios> incidencias, List<vEmpleados> vEmp, object item)
        {
            string[] campos = item.ToString().Split(',');

            int IdEmpleado = vEmp.Where(x => x.ClaveEmpleado.Trim() == campos[0].Trim().ToUpper()).Select(x => x.IdEmpleado).FirstOrDefault();
            int IdregistroPatronal = 0;
            int IdFactura = 0;
            string Observaciones = string.Empty;
            decimal HonorariosN = 0;
            decimal HonorariosB = 0;

            try { IdregistroPatronal = vEmp.Where(x => x.ClaveEmpleado.Trim() == campos[0].Trim().ToUpper()).Select(x => x.IdRegistroPatronal).FirstOrDefault().Value; ; } catch { IdregistroPatronal = 0; }

            if (IdregistroPatronal == 0)
            {
                try { IdregistroPatronal = int.Parse(campos[1]); } catch { IdregistroPatronal = 0; }
            }


            try { IdFactura = int.Parse(campos[2]); } catch { IdFactura = 0; }
            try { HonorariosN = decimal.Parse(campos[3]); } catch { HonorariosN = 0; }
            try { HonorariosB = decimal.Parse(campos[4]); } catch { HonorariosB = 0; }
            try { Observaciones = campos[5]; } catch { Observaciones = string.Empty; }

            if (errores.listErrores.Count == 0)
            {
                errores.Correctos++;
                mHonorarios i = new mHonorarios();
                i.IdEmpleado = IdEmpleado;
                i.idHonorarioF = IdFactura;
                i.IdPeriodoNomina = IdPeriodoNomina;
                i.Observaciones = Observaciones;
                i.IdRegistroPatronal = IdregistroPatronal;
                i.HonorariosN = HonorariosN; ;
                i.HonorariosB = HonorariosB;



                incidencias.Add(i);
            }
            else
            {
                errores.Errores++; ;
            }
        }

    }
}