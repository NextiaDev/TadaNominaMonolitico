using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Microsoft.Ajax.Utilities;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.CalculoFiniquito;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    /// Nomina
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassNomina
    {
        /// <summary>
        /// Método para obter la información para presentar las cantidades generales y representativas de un periodo de la nomina 
        /// </summary>
        /// <param name="periodo">Información del periodo de nómina</param>
        /// <returns>información para presentar las cantidades generales y representativas de un periodo de la nomina</returns>
        public ModelProcesaNominaGeneral GetModelProcesaNominaGeneral( vPeriodoNomina periodo)
        {
            ModelProcesaNominaGeneral model = new ModelProcesaNominaGeneral();
                        
            model.Cliente = periodo.Cliente;
            model.IdPeriodoNomina = periodo.IdPeriodoNomina;
            model.Periodo = periodo.Periodo;
            model.TipoNomina = periodo.TipoNomina;
            model.FechaInicial = periodo.FechaInicio.ToShortDateString();
            model.FechaFinal = periodo.FechaFin.ToShortDateString();
            model.UnidadNegocio = periodo.UnidadNegocio;
            model.AjusteImpuestos = periodo.AjusteDeImpuestos;
            model.Periodicidad = periodo.Periodicidad;
            model.AJusteAnual = periodo.AjusteAnual;
            model.ConfiguracionSueldos = periodo.ConfiguracionSueldos;
            
            try { model.PorcentajeISN = (decimal)periodo.PorcentajeISN; } catch { model.PorcentajeISN = 0; }

            if (periodo.TipoNomina == "Aguinaldo")
            {
                model.IncidenciasAguinaldoAutomaticas = periodo.IncidenciasAguinaldoAutomaticas;
                if (model.IncidenciasAguinaldoAutomaticas == 1)
                    model.AguinaldoSINO = true;
                else
                    model.AguinaldoSINO = false;                
            }

            ClassPeriodoNomina cper = new ClassPeriodoNomina();
            model.PeriodoAjuste = cper.GetDescripcionPeriodos(periodo.SeAjustaraConPeriodo);
            
            List<ModelEmpleadosNetos> empNetos = new List<ModelEmpleadosNetos>();
           
            if (periodo.IdEstatus == 1)
            {
                ClassEmpleado classEmpleado = new ClassEmpleado();
                var empleados = classEmpleado.GetEmpleadoByUnidadNegocio(periodo.IdUnidadNegocio).Where(x => x.IdEstatus == 1);
                model.TotalEmpleados = empleados.Count();

                if (model.TotalEmpleados <= 50)
                {
                    empleados.ForEach(x => empNetos.Add(new ModelEmpleadosNetos
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                        Neto = x.NetoPagar
                    }));
                }

                model.lempleadosNetos = empNetos;
                model.EmpleadosSinSDI = empleados.Where(x => x.IdEstatus == 1 && (x.IdRegistroPatronal != null && x.IdRegistroPatronal != 0) && (x.SDIMSS != null && x.SDIMSS != 0) && (x.SDI == null || x.SDI == 0)).Count();
                model.LEmpleadosSinSDI = new Dictionary<string, string>();
                empleados.Where(x => x.IdEstatus == 1 && (x.IdRegistroPatronal != null && x.IdRegistroPatronal != 0) && (x.SDIMSS != null && x.SDIMSS != 0) && (x.SDI == null || x.SDI == 0)).ForEach(x => { model.LEmpleadosSinSDI.Add(x.ClaveEmpleado, x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre); });

                List<NominaTrabajo> dataNomina = GetDataNominaTrabajo(periodo.IdPeriodoNomina);
                if (periodo.TipoNomina == "Finiquitos")
                    dataNomina = GetDataNominaTrabajoFiniquitos(periodo.IdPeriodoNomina);

                model.TotalPagarSueldos = string.Format("{0:C2}", dataNomina.Select(x => x.Neto).Sum());
                model.TotalISR = string.Format("{0:C2}", dataNomina.Select(x => x.ImpuestoRetener).Sum());
                model.TotalIMSS = string.Format("{0:C2}", dataNomina.Select(x => x.IMSS_Obrero).Sum());
                model.TotalIMSS_P = string.Format("{0:C2}", dataNomina.Select(x => x.Total_Patron).Sum());
                model.TotalISN = string.Format("{0:C2}", dataNomina.Select(x => x.ISN).Sum());
            }

            if (periodo.IdEstatus == 2)
            {
                List<Nomina> dataNomina = GetDatosNomina(periodo.IdPeriodoNomina);
                if (periodo.TipoNomina == "Finiquitos")
                    dataNomina = GetDataNominaFiniquitos(periodo.IdPeriodoNomina);

                try { model.FechaCierre = ((DateTime)periodo.FechaCierre).ToShortDateString(); } catch { }
                try { model.FechaPago = ((DateTime)periodo.FechaDispersion).ToShortDateString(); } catch { }
                model.TotalEmpleados = dataNomina.Count();
                model.TotalPagarSueldos = string.Format("{0:C2}", dataNomina.Select(x => x.Neto).Sum());
                model.TotalISR = string.Format("{0:C2}", dataNomina.Select(x => x.ImpuestoRetener).Sum());
                model.TotalIMSS = string.Format("{0:C2}", dataNomina.Select(x => x.IMSS_Obrero).Sum());
                model.TotalIMSS_P = string.Format("{0:C2}", dataNomina.Select(x => x.Total_Patron).Sum());
                model.TotalISN = string.Format("{0:C2}", dataNomina.Select(x => x.ISN).Sum());
            }

            return model;
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina d el atabla NominaTrabajo 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina de la tabla NominaTrabajo</returns>
        public List<NominaTrabajo> GetDataNominaTrabajo(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).ToList();
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo</returns>
        public List<NominaTrabajo> GetDataNominaTrabajoFiniquitos(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 1, 2, 4 };
                var nomina = (from b in entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo && estatus.Contains((int)x.IdEstatus)) select b).ToList();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo</returns>
        public List<Nomina> GetDataNominaFiniquitos(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 1, 2, 4 };
                var nomina = (from b in entidad.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodo && estatus.Contains((int)x.IdEstatus)) select b).ToList();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo</returns>
        public List<vNominaTrabajo> GetDatavNominaTrabajoFiniquitos(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 1, 2, 4 };
                var nomina = (from b in entidad.vNominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo && estatus.Contains((int)x.IdEstatus)) select b).ToList();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información consolidada de todo lo que esta procesado en el periodo de nómina de la tabla Nomina
        /// </summary>
        /// <param name="IdPeriodo"></param>
        /// <returns>información consolidada de todo lo que esta procesado en el periodo de nómina de la tabla Nomina</returns>
        public List<Nomina> GetDatosNomina( int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).ToList();
            }
        }

        /// <summary>
        /// Método que extrae el conjunto de incidencias en base a un empledo y un periodo de nómina para presentarlas en la pantalla de cálculo individual
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empledo</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>conjunto de incidencias en base a un empledo y un periodo de nómina para presentarlas en la pantalla de cálculo individual</returns>
        public List<ModelIncidenciaIndividual> GetIncidenciasIndividuales(int IdEmpleado, int IdPeriodo, int IdCliente)
        {
            var model = new List<ModelIncidenciaIndividual>();            

            var conceptos = GetIncidneciasConceptos(IdEmpleado, IdPeriodo, IdCliente);

            conceptos.ForEach(x => { model.Add(new ModelIncidenciaIndividual { Id = x.IdIncidencia, ClaveConcepto = x.ClaveConcepto, Concepto = x.Concepto, TipoConcepto = x.TipoConcepto,
                Agrupador = x.Agrupador, TipoDato = x.TipoDato, Cantidad = x.Cantidad, CantidadEsq = x.CantidadEsquema, Monto = x.MontoTrad, MontoEsq = x.MontoEsq, IdConcepto = x.IdConcepto, TipoEsquema = x.TipoEsquema,
                BanderaFiniquitos = x.BanderaFiniquitos, BanderaAguinaldo = x.BanderaAguinaldos, MultiplicaDT = x.MultiplicaDT, BanderaIncidenciasFijas = x.BanderaIncidenciasFijas,
                BanderaConceptoEspecial = x.BanderaConceptoEspecial, BanderaFonacot = x.BanderaFonacot, BanderaInfonavit = x.BanderaInfonavit, BanderaPensionAlimenticia = x.BanderaPensionAlimenticia, 
                BanderaVacaciones = x.BanderaVacaciones, BanderaAusentismos = x.BanderaAusentismos, BanderaAdelantoPULPI = x.BanderaAdelantoPULPI, BanderaPiramidacion = x.BanderaPiramidacion, BanderaSaldos = x.BanderaSaldos, 
                BanderaCompensaciones = x.BanderaCompensaciones, BanderaIncidencia = x.BanderaIncidencia }); });

            foreach (var item in model)
            {
                if (item.BanderaIncidenciasFijas != null)
                    item.ModuloCaptura = "Inc.Fijas"; 
                if (item.BanderaConceptoEspecial != null)
                    item.ModuloCaptura = "Especial";
                if (item.BanderaFonacot != null)
                    item.ModuloCaptura = "Fonacot";
                if (item.BanderaInfonavit != null)
                    item.ModuloCaptura = "Vivienda";
                if (item.BanderaPensionAlimenticia != null)
                    item.ModuloCaptura = "P.Alimenticia";
                if (item.BanderaVacaciones != null)
                    item.ModuloCaptura = "Vacaciones";
                if (item.BanderaAusentismos != null)
                    item.ModuloCaptura = "Ausentismos";
                if (item.BanderaPiramidacion != null)
                    item.ModuloCaptura = "Piramidación";
                if (item.BanderaSaldos != null)
                    item.ModuloCaptura = "Saldos";
                if (item.BanderaCompensaciones != null)
                    item.ModuloCaptura = "Comp. RH";
                if (item.BanderaIncidencia != null)
                    item.ModuloCaptura = "Incidencia";
                if (item.BanderaConceptoEspecial != null)
                    item.ModuloCaptura = "Automatica";
            }

            return model;
        }

        /// <summary>
        /// Método que extrae el conjunto de incidencias en base a un empledo y un periodo de nómina para presentarlas en la pantalla de cálculo individual
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>conjunto de incidencias en base a un empledo y un periodo de nómina para presentarlas en la pantalla de cálculo individual</returns>
        public List<sp_RegresaIncidenciasCalculoIndividual_Result> GetIncidneciasConceptos(int IdEmpleado, int IdPeriodo, int IdCliente)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var conceptos = (from b in entidad.sp_RegresaIncidenciasCalculoIndividual(IdEmpleado, IdPeriodo, IdCliente) select b).ToList();

                return conceptos;
            }
        }

        /// <summary>
        /// Método para obter la información para presentar las cantidades generales y representativas de un periodo de la nomina por empleado
        /// </summary>
        /// <param name="pIdEmpleado">Identificador del empleado</param>
        /// <param name="pIdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="TipoNomina">Tipo de nómina</param>
        /// <returns>información para presentar las cantidades generales y representativas de un periodo de la nomina por empleado</returns>
        public ModelNominaIndividual GetModelNominaIndividual(int pIdEmpleado, int pIdPeriodoNomina, string TipoNomina)
        {
            ModelNominaIndividual model = new ModelNominaIndividual();

            ClassPeriodoNomina cperiod = new ClassPeriodoNomina();
            ClassEmpleado classEmpleado = new ClassEmpleado(); 
            vEmpleados empleado = classEmpleado.GetvEmpleado(pIdEmpleado);

            var periodo = cperiod.GetvPeriodoNominasId(pIdPeriodoNomina);

            model.IdPeriodoNomina = pIdPeriodoNomina;

            model.NombrePeriodo = periodo.Periodo;
            model.FechasPeriodo = periodo.FechaInicio.ToShortDateString() + " - " + periodo.FechaFin.ToShortDateString();
            model.IdEmpleado = empleado.IdEmpleado;
            model.claveEmpleado = empleado.ClaveEmpleado;
            model.NombreCompletoEmpleado = empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + " " + empleado.Nombre;
            try { model.SueldoDiario = string.Format("{0:C2}", empleado.SD); } catch { model.SueldoDiario = "$0.00"; }
            try { model.SueldoDiarioIMSS = string.Format("{0:C2}", empleado.SDIMSS); } catch { model.SueldoDiarioIMSS = "$0.00"; }
            try { model.SDI = string.Format("{0:C2}", empleado.SDI); } catch { model.SDI = "$0.00"; }
            try { model.SueldoDiarioEsq = empleado.Esquema.Trim() != "100% TRADICIONAL" ? string.Format("{0:C2}", (empleado.SD - empleado.SDIMSS)) : "$0.00"; } catch { model.SueldoDiarioEsq = "$0.00"; }
            try { model.NetoPagar = empleado.NetoPagar.ToString(); } catch { model.NetoPagar = "0.00"; }
            try { model.FechaAltaImss = empleado.FechaAltaIMSS.Value.ToShortDateString(); } catch { model.FechaAltaImss = ""; }
            try { model.FechaReconocimientoAntiguedad = empleado.FechaReconocimientoAntiguedad.Value.ToShortDateString(); } catch { model.FechaReconocimientoAntiguedad = ""; }
            model.IdEstatus = empleado.IdEstatus;
            model.ConfiguracionSueldos = periodo.ConfiguracionSueldos;

            model.ReciboTradicional = new ModelReciboTradicional();
            model.ReciboEsquema = new ModelReciboEsquema();
            model.ReciboReal = new ModelReciboReal();
            model.ReciboTradicional.CURP = empleado.Curp;
            model.ReciboTradicional.RFC = empleado.Rfc;
            model.ReciboTradicional.NSS = empleado.Imss;
            model.ReciboTradicional.SD = empleado.SDIMSS ?? 0;
            model.ReciboTradicional.SDI = empleado.SDI ?? 0;

            int IdCliente = empleado.IdCliente;

            NominaTrabajo nom = GetNominaTrabajo(pIdEmpleado, pIdPeriodoNomina);
            if (TipoNomina == "Finiquitos")
                nom = GetNominaTrabajoFiniquitos(pIdEmpleado, pIdPeriodoNomina);

            if (nom != null)
            {
                model.ER = string.Format("{0:C2}", nom.ER);
                model.ERS = string.Format("{0:C2}", nom.ERS);
                model.ERR = string.Format("{0:C2}", (nom.ER + nom.ERS));
                model.DD = string.Format("{0:C2}", nom.DD);
                model.DDS = string.Format("{0:C2}", nom.DDS);
                model.DDR = string.Format("{0:C2}", (nom.DDS + nom.DD));
                model.Neto = string.Format("{0:C2}", nom.Neto);
                model.NetoS = string.Format("{0:C2}", nom.Netos);
                model.NetoR = string.Format("{0:C2}", (nom.Netos + nom.Neto));
                model.TotalRecibir = string.Format("{0:C2}", (decimal)nom.Neto + (decimal)nom.Netos); 

                try { model.ReciboTradicional.DiasLaborados = (decimal)nom.DiasTrabajados; } catch { model.ReciboTradicional.DiasLaborados = 0; }
                try { model.ReciboTradicional.DiasVacaciones = (decimal)nom.Dias_Vacaciones; } catch { model.ReciboTradicional.DiasVacaciones = 0; }
                try { model.ReciboTradicional.Incapacidades = (decimal)nom.Incapacidades; } catch { model.ReciboTradicional.Incapacidades = 0; }
                try { model.ReciboTradicional.Faltas = (decimal)nom.Faltas; } catch { model.ReciboTradicional.Faltas = 0; }
                try { model.ReciboTradicional.DiasLaborados = (decimal)nom.DiasTrabajados; } catch { model.ReciboTradicional.DiasLaborados = 0; }
                try { model.ReciboTradicional.BaseGravada = (decimal)nom.BaseGravada; } catch { model.ReciboTradicional.BaseGravada = 0; }
                try { model.ReciboTradicional.TotalPatron = (decimal)nom.Total_Patron; } catch { model.ReciboTradicional.TotalPatron = 0; }
            }
            else
            {
                model.ER = "$0.00";
                model.ERS = "$0.00";
                model.DD = "$0.00";
                model.DDS = "$0.00";
                model.Neto = "$0.00";
                model.NetoS = "$0.00";

                model.ReciboTradicional.DiasLaborados = 0;
                model.ReciboTradicional.DiasVacaciones = 0;
                model.ReciboTradicional.Incapacidades = 0;
                model.ReciboTradicional.Faltas = 0;
                model.ReciboTradicional.DiasLaborados = 0;
                model.ReciboTradicional.BaseGravada = 0;
                model.ReciboTradicional.TotalPatron = 0;
            }

            model.ListIncidencias = GetIncidenciasIndividuales(pIdEmpleado, pIdPeriodoNomina, IdCliente).Where(x=>x.Agrupador != "003").ToList();
            model.ReciboTradicional.IncidenciasRecibo = GetIncidenciasRecibo(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboTradicional.IncidenciasReciboDec = GetIncidenciasReciboDeduc(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboEsquema.IncidenciasReciboEsquema = GetIncidenciasReciboEsquema(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboEsquema.IncidenciasReciboDecEsquema = GetIncidenciasReciboDeducEsquema(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboReal.IncidenciasReciboReal = GetIncidenciasReciboReal(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboReal.IncidenciasReciboDecReal = GetIncidenciasReciboDeducReal(pIdEmpleado, pIdPeriodoNomina);

            if (TipoNomina == "Finiquitos")
            {
                model.ListIncidencias = GetIncidenciasIndividuales(pIdEmpleado, pIdPeriodoNomina, IdCliente);
                ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
                vConfiguracionFiniquito conf = cfiniquitos.GetFiniquitoConfigurado(pIdPeriodoNomina, pIdEmpleado);                
                try { model.FechaBaja = conf.FechaBajaFin.Value.ToShortDateString(); } catch { model.FechaBaja = null; }
                                
                try { model.IdConfiguracionFiniquito = (int)conf.IdConfiguracionFiniquito; } catch { }                
                try { if (conf.BanderaVac == 1) { model.banderaVac = true; } } catch { }
                try { if (conf.BanderaPV == 1) { model.banderaPV = true; } } catch { }
                try { if (conf.BanderaAguinaldo == 1) { model.banderaAgui = true; } } catch { }
                try { if (conf.BanderaAguinaldoEsq == 1) { model.banderaAguiEsq = true; } } catch { }
                try { if (conf.Bandera90d == 1) { model.bandera90d = true; } } catch { }
                try { if (conf.Bandera20d == 1) { model.bandera20d = true; } } catch { }
                try { if (conf.BanderaPA == 1) { model.banderaPA = true; } } catch { }
                try { if (conf.BanderaLiquidacion == 1) { model.Liquidacion = true; } } catch { }
                try { if (conf.LiquidacionSDI == 1) { model.LiquidacionSDI = true; } } catch { }
            }
            
            return model;
        }

        public ModelNominaIndividual GetModelNominaIndividualAcumulado(int pIdEmpleado, int pIdPeriodoNomina, string TipoNomina)
        {
            ModelNominaIndividual model = new ModelNominaIndividual();

            ClassPeriodoNomina cperiod = new ClassPeriodoNomina();
            ClassEmpleado classEmpleado = new ClassEmpleado();
            vEmpleados empleado = classEmpleado.GetvEmpleado(pIdEmpleado);

            var periodo = cperiod.GetvPeriodoNominasId(pIdPeriodoNomina);

            model.IdPeriodoNomina = pIdPeriodoNomina;

            model.NombrePeriodo = periodo.Periodo;
            model.FechasPeriodo = periodo.FechaInicio.ToShortDateString() + " - " + periodo.FechaFin.ToShortDateString();
            model.IdEmpleado = empleado.IdEmpleado;
            model.claveEmpleado = empleado.ClaveEmpleado;
            model.NombreCompletoEmpleado = empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + " " + empleado.Nombre;
            try { model.SueldoDiario = string.Format("{0:C2}", empleado.SD); } catch { model.SueldoDiario = "$0.00"; }
            try { model.SueldoDiarioIMSS = string.Format("{0:C2}", empleado.SDIMSS); } catch { model.SueldoDiarioIMSS = "$0.00"; }
            try { model.SDI = string.Format("{0:C2}", empleado.SDI); } catch { model.SDI = "$0.00"; }
            try { model.SueldoDiarioEsq = empleado.Esquema.Trim() != "100% TRADICIONAL" ? string.Format("{0:C2}", (empleado.SD - empleado.SDIMSS)) : "$0.00"; } catch { model.SueldoDiarioEsq = "$0.00"; }
            try { model.NetoPagar = empleado.NetoPagar.ToString(); } catch { model.NetoPagar = "0.00"; }
            try { model.FechaAltaImss = empleado.FechaAltaIMSS.Value.ToShortDateString(); } catch { model.FechaAltaImss = ""; }
            try { model.FechaReconocimientoAntiguedad = empleado.FechaReconocimientoAntiguedad.Value.ToShortDateString(); } catch { model.FechaReconocimientoAntiguedad = ""; }
            model.IdEstatus = empleado.IdEstatus;
            model.ConfiguracionSueldos = periodo.ConfiguracionSueldos;

            model.ReciboTradicional = new ModelReciboTradicional();
            model.ReciboEsquema = new ModelReciboEsquema();
            model.ReciboReal = new ModelReciboReal();
            model.ReciboTradicional.CURP = empleado.Curp;
            model.ReciboTradicional.RFC = empleado.Rfc;
            model.ReciboTradicional.NSS = empleado.Imss;
            model.ReciboTradicional.SD = empleado.SDIMSS ?? 0;
            model.ReciboTradicional.SDI = empleado.SDI ?? 0;

            int IdCliente = empleado.IdCliente;

            Nomina nom = GetNomina(pIdEmpleado, pIdPeriodoNomina);
            if (TipoNomina == "Finiquitos")
                nom = GetNominaFiniquitos(pIdEmpleado, pIdPeriodoNomina);

            if (nom != null)
            {
                model.ER = string.Format("{0:C2}", nom.ER);
                model.ERS = string.Format("{0:C2}", nom.ERS);
                model.ERR = string.Format("{0:C2}", (nom.ER + nom.ERS));
                model.DD = string.Format("{0:C2}", nom.DD);
                model.DDS = string.Format("{0:C2}", nom.DDS);
                model.DDR = string.Format("{0:C2}", (nom.DDS + nom.DD));
                model.Neto = string.Format("{0:C2}", nom.Neto);
                model.NetoS = string.Format("{0:C2}", nom.Netos);
                model.NetoR = string.Format("{0:C2}", (nom.Netos + nom.Neto));
                model.TotalRecibir = string.Format("{0:C2}", (decimal)nom.Neto + (decimal)nom.Netos);

                try { model.ReciboTradicional.DiasLaborados = (decimal)nom.DiasTrabajados; } catch { model.ReciboTradicional.DiasLaborados = 0; }
                try { model.ReciboTradicional.DiasVacaciones = (decimal)nom.Dias_Vacaciones; } catch { model.ReciboTradicional.DiasVacaciones = 0; }
                try { model.ReciboTradicional.Incapacidades = (decimal)nom.Incapacidades; } catch { model.ReciboTradicional.Incapacidades = 0; }
                try { model.ReciboTradicional.Faltas = (decimal)nom.Faltas; } catch { model.ReciboTradicional.Faltas = 0; }
                try { model.ReciboTradicional.DiasLaborados = (decimal)nom.DiasTrabajados; } catch { model.ReciboTradicional.DiasLaborados = 0; }
                try { model.ReciboTradicional.BaseGravada = (decimal)nom.BaseGravada; } catch { model.ReciboTradicional.BaseGravada = 0; }
                try { model.ReciboTradicional.TotalPatron = (decimal)nom.Total_Patron; } catch { model.ReciboTradicional.TotalPatron = 0; }
            }
            else
            {
                model.ER = "$0.00";
                model.ERS = "$0.00";
                model.DD = "$0.00";
                model.DDS = "$0.00";
                model.Neto = "$0.00";
                model.NetoS = "$0.00";

                model.ReciboTradicional.DiasLaborados = 0;
                model.ReciboTradicional.DiasVacaciones = 0;
                model.ReciboTradicional.Incapacidades = 0;
                model.ReciboTradicional.Faltas = 0;
                model.ReciboTradicional.DiasLaborados = 0;
                model.ReciboTradicional.BaseGravada = 0;
                model.ReciboTradicional.TotalPatron = 0;
            }

            model.ListIncidencias = GetIncidenciasIndividuales(pIdEmpleado, pIdPeriodoNomina, IdCliente).Where(x => x.Agrupador != "003").ToList();
            model.ReciboTradicional.IncidenciasRecibo = GetIncidenciasRecibo(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboTradicional.IncidenciasReciboDec = GetIncidenciasReciboDeduc(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboEsquema.IncidenciasReciboEsquema = GetIncidenciasReciboEsquema(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboEsquema.IncidenciasReciboDecEsquema = GetIncidenciasReciboDeducEsquema(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboReal.IncidenciasReciboReal = GetIncidenciasReciboReal(pIdEmpleado, pIdPeriodoNomina);
            model.ReciboReal.IncidenciasReciboDecReal = GetIncidenciasReciboDeducReal(pIdEmpleado, pIdPeriodoNomina);

            if (TipoNomina == "Finiquitos")
            {
                model.ListIncidencias = GetIncidenciasIndividuales(pIdEmpleado, pIdPeriodoNomina, IdCliente);
                ClassProcesosFiniquitos cfiniquitos = new ClassProcesosFiniquitos();
                vConfiguracionFiniquito conf = cfiniquitos.GetFiniquitoConfigurado(pIdPeriodoNomina, pIdEmpleado);
                try { model.FechaBaja = conf.FechaBajaFin.Value.ToShortDateString(); } catch { model.FechaBaja = null; }

                try { model.IdConfiguracionFiniquito = (int)conf.IdConfiguracionFiniquito; } catch { }
                try { if (conf.BanderaVac == 1) { model.banderaVac = true; } } catch { }
                try { if (conf.BanderaPV == 1) { model.banderaPV = true; } } catch { }
                try { if (conf.BanderaAguinaldo == 1) { model.banderaAgui = true; } } catch { }
                try { if (conf.BanderaAguinaldoEsq == 1) { model.banderaAguiEsq = true; } } catch { }
                try { if (conf.Bandera90d == 1) { model.bandera90d = true; } } catch { }
                try { if (conf.Bandera20d == 1) { model.bandera20d = true; } } catch { }
                try { if (conf.BanderaPA == 1) { model.banderaPA = true; } } catch { }
                try { if (conf.BanderaLiquidacion == 1) { model.Liquidacion = true; } } catch { }
                try { if (conf.LiquidacionSDI == 1) { model.LiquidacionSDI = true; } } catch { }
            }

            return model;
        }

        /// <summary>
        /// Método para obter la información del recibo del empleado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <returns></returns>
        public List<sp_ReciboTradicionalPercepciones_Result> GetIncidenciasRecibo(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Database.SqlQuery<sp_ReciboTradicionalPercepciones_Result>("sp_ReciboTradicionalPercepciones " + IdPeriodo + ", " + IdEmpleado).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Método para obtener las deducciones que se aplicaran al sueldo del empleado en el periodo de nómina
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <returns>educciones que se aplicaran al sueldo del empleado en el periodo de nómina</returns>
        public List<sp_ReciboTradicionalDeducciones_Result> GetIncidenciasReciboDeduc(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.sp_ReciboTradicionalDeducciones(IdPeriodo, IdEmpleado) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Método para obtener las deducciones que se aplicaran al sueldo del empleado en el periodo de nómina
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <returns>educciones que se aplicaran al sueldo del empleado en el periodo de nómina</returns>
        public List<sp_ReciboEsquemaPercepciones_Result> GetIncidenciasReciboEsquema(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.sp_ReciboEsquemaPercepciones(IdPeriodo, IdEmpleado) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Método para obtener las deducciones que se aplicaran al sueldo del empleado en el periodo de nómina
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <returns>educciones que se aplicaran al sueldo del empleado en el periodo de nómina</returns>
        public List<sp_ReciboRealPercepciones_Result> GetIncidenciasReciboReal(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.sp_ReciboRealPercepciones(IdPeriodo, IdEmpleado) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Método para obtener las deducciones que se aplicaran al sueldo del empleado en el periodo de nómina
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <returns>educciones que se aplicaran al sueldo del empleado en el periodo de nómina</returns>
        public List<sp_ReciboEsquemaDeducciones_Result> GetIncidenciasReciboDeducEsquema(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.sp_ReciboEsquemaDeducciones(IdPeriodo, IdEmpleado) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Método para obtener las deducciones que se aplicaran al sueldo del empleado en el periodo de nómina
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo</param>
        /// <returns>educciones que se aplicaran al sueldo del empleado en el periodo de nómina</returns>
        
        public List<sp_ReciboRealDeducciones_Result> GetIncidenciasReciboDeducReal(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = (from b in entidad.sp_ReciboRealDeducciones(IdPeriodo, IdEmpleado) select b).ToList();

                return incidencias;
            }
        }

        /// <summary>
        /// Método para obtener información de manera individual del empleado por clave y unidad de negocio
        /// </summary>
        /// <param name="pClave">Clave del empleado</param>
        /// <param name="pIdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>información de manera individual del empleado por clave y unidad de negocio</returns>
        public List<vEmpleados> GetvEmpeladosByClaveByIdUnidadnegocio(string pClave, int pIdUnidadNegocio)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                int[] estatus = { 1, 2, 3 };
                var empleado = (from b in entidad.vEmpleados
                                       where b.ClaveEmpleado == pClave && b.IdUnidadNegocio == pIdUnidadNegocio && estatus.Contains(b.IdEstatus)
                                       select b).OrderBy(x=> x.IdEstatus).ToList();

                return empleado;
            }
        }

        /// <summary>
        /// Método para obtener información de manera individual del empleado por el identificador del empleado
        /// </summary>
        /// <param name="IdEmpleado"></param>
        /// <returns>Información de manera individual del empleado por el identificador del empleado</returns>
        public vEmpleados GetvEmpeladosByClaveByIdUnidadnegocio(int IdEmpleado)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                vEmpleados empleado = (from b in entidad.vEmpleados.Where(x=> x.IdEmpleado == IdEmpleado)
                                       select b).FirstOrDefault();

                return empleado;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina de la tabla NominaTrabajo por empleado 
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina de la tabla NominaTrabajo por empleado 
        /// </summary></returns>
        public NominaTrabajo GetNominaTrabajo(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var nomina = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).FirstOrDefault();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina de la tabla NominaTrabajo por empleado 
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina de la tabla NominaTrabajo por empleado 
        /// </summary></returns>
        public Nomina GetNomina(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var nomina = (from b in entidad.Nomina.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).FirstOrDefault();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo por empleado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo por empleado</returns>
        public NominaTrabajo GetNominaTrabajoFiniquitos(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 1, 2, 4 };
                var nomina = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && estatus.Contains((int)x.IdEstatus)) select b).FirstOrDefault();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo por empleado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo por empleado</returns>
        public Nomina GetNominaFiniquitos(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int[] estatus = { 1, 2, 4 };
                var nomina = (from b in entidad.Nomina.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && estatus.Contains((int)x.IdEstatus)) select b).FirstOrDefault();

                return nomina;
            }
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo por empleado
        /// </summary>
        /// <param name="ClaveEmpleado">Clave empleado</param>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina considerando diferentes estatus especiales de los finiquitos de la tabla NominaTrabajo por empleado</returns>
        public vNominaTrabajo GetvNominaTrabajo(string ClaveEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {                
                var nomina = (from b in entidad.vNominaTrabajo.Where(x => x.ClaveEmpleado == ClaveEmpleado && x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).FirstOrDefault();

                return nomina;
            }
        }

        /// <summary>
        /// Metódo para calcular la nómina de manera individual
        /// </summary>
        /// <param name="model">ModelNominaIndividual que viene de la pantalla de ProcesaNominaIndividual</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <exception cref="Exception">Error al calcular la nómina</exception>
        public void Proceso_Nomina_Individual(ModelNominaIndividual model, int IdUsuario)
        {
            ClassEmpleado cemp = new ClassEmpleado();
            try { cemp.ProcesoNetoPagar(model.IdEmpleado, model.NetoPagar); } catch(Exception ex) { throw new Exception(ex.Message, ex); }

            if (model.ListIncidencias != null)
            {
                ClassIncidencias cincidencias = new ClassIncidencias();
                try { cincidencias.Proceso_Incidencias_Individuales(model.ListIncidencias, model.IdEmpleado, model.IdPeriodoNomina, IdUsuario); } catch (Exception ex) { throw new Exception(ex.Message, ex); }
            }

            ClassCalculoNomina cnomina = new ClassCalculoNomina();
            try { cnomina.Procesar(model.IdPeriodoNomina, model.IdEmpleado, IdUsuario); } catch(Exception ex) { throw new Exception(ex.Message, ex); }
        }

        /// <summary>
        /// Metódo para calcular el finiquito de manera individual
        /// </summary>
        /// <param name="model">ModelNominaIndividual que viene de l apantalla de ProcesaNominaIndividual</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <exception cref="Exception">Error al calcular l anómina</exception>
        public void Proceso_Finiquito_Individual(ModelNominaIndividual model, int IdUsuario)
        {
            ClassIncidencias cincidencias = new ClassIncidencias();
            try { cincidencias.Proceso_Incidencias_Individuales(model.ListIncidencias, model.IdEmpleado, model.IdPeriodoNomina, IdUsuario); } catch (Exception ex) { throw new Exception(ex.Message, ex); }

            ClassCalculoFiniquitos cfiniquito = new ClassCalculoFiniquitos();
            try { cfiniquito.Procesar(model.IdPeriodoNomina, model.IdEmpleado, IdUsuario); }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }


        /// <summary>
        /// Método que en base al sueldo del empleado y a las prestaciones del patron nos regresa el sueldo diario integrado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <param name="sdimss">Sueldo base de cotización</param>
        /// <param name="fechaReconocimientoAntiguedad">Fecha de reconocimiento de antiguedad</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>sueldo diario integrado calculado</returns>
        public double ObtenSDI(int IdEmpleado, decimal sdimss, string fechaReconocimientoAntiguedad, int IdCliente, int? IdPrestaciones)
        {
            decimal _factorIntegracion = 0;

            int prestaciones = validaPrestaciones(IdCliente, IdEmpleado, IdPrestaciones);

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                if (string.IsNullOrEmpty(fechaReconocimientoAntiguedad))
                {
                    var query = (from b in entidad.FactorIntegracion
                                 where b.IdPrestaciones == prestaciones && b.IdEstatus == 1
                                 && b.FechaInicioVigencia == (entidad.FactorIntegracion.Where(y => y.FechaInicioVigencia <= DateTime.Now && y.IdEstatus == 1).Select(c=> c.FechaInicioVigencia).OrderByDescending(z=> z).FirstOrDefault())
                                 orderby b.Limite_Inferior
                                 select b).First();

                    _factorIntegracion = (decimal)query.FactorIntegracion1;
                }
                else
                {
                    DateTime fra = DateTime.Parse(fechaReconocimientoAntiguedad);

                    decimal AntDias = (DateTime.Now.Subtract(fra).Days) + 1;
                    decimal AntAños = Math.Round(AntDias / 365, 2);

                    if (AntAños <= 0)
                    {
                        AntAños = 0.01M;
                    }

                    using (TadaNominaEntities ctx = new TadaNominaEntities())
                    {
                        var query = (from b in ctx.FactorIntegracion
                                     where b.Limite_Superior >= AntAños && b.Limite_Inferior <= AntAños && b.IdPrestaciones == prestaciones && b.IdEstatus == 1
                                     && b.FechaInicioVigencia == (ctx.FactorIntegracion.Where(y => y.FechaInicioVigencia <= DateTime.Now && y.IdEstatus == 1).Select(c => c.FechaInicioVigencia).OrderByDescending(z => z).FirstOrDefault())
                                     select b).First();

                        _factorIntegracion = (decimal)query.FactorIntegracion1; 
                    }
                }
            }

            return (double)ValidaTopeSDI(sdimss * _factorIntegracion);
        }

        /// <summary>
        /// Método que en base a la uma nos regresa el sueldo diario interado que no rebase las 25 umas
        /// </summary>
        /// <returns>ueldo diario interado que no rebase las 25 umas</returns>
        public double RegresaTopeSDI()
        {
            double uma = (double)ObtenUMA();

            return uma * 25;
        }

        /// <summary>
        /// Método que obtiene el valor de la uma
        /// </summary>
        /// <returns>valor de la uma</returns>
        private static decimal ObtenUMA()
        {
            NominaEntities1 context = new NominaEntities1();
            decimal _uma = 0;
            DateTime fecha = DateTime.Now;
            var resultado = (from b in context.Sueldos_Minimos
                             where fecha >= b.FechaInicio
                             select b).OrderByDescending(x => x.FechaInicio).FirstOrDefault();

            if (resultado != null)
            {
                _uma = (decimal)resultado.UMA;
            }

            return _uma;
        }

        /// <summary>
        /// Método que valida que el salario diario integrado no rebase las 25 umas
        /// </summary>
        /// <param name="sdi">Sakario diario integrado</param>
        /// <returns>alario diario integrado que no rebase las 25 umas</returns>
        public double ValidaTopeSDI(decimal sdi)
        {
            double TopeSDI = RegresaTopeSDI();

            if ((double)sdi >= TopeSDI)
            {
                return TopeSDI;
            }
            else
            {
                return (double)sdi;
            }
        }

        /// <summary>
        /// Método que valia las prestaciones del patron
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns>Identificador de las prestaciones</returns>
        private int validaPrestaciones(int IdCliente, int IdEmpleado, int? IdPrestaciones)
        {
            int res = 1;

            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                // Si es un empleado ya existente
                if (IdEmpleado > 0)
                {
                    int _aux = validaPrestacionesByEmpleado(IdEmpleado);
                    if (_aux > 0)
                    {
                        res = _aux;
                    }
                }
                else
                {
                    if (IdPrestaciones == null)
                    {
                        List<Prestaciones> prestacionesCliente = (from b in entidad.Prestaciones
                                                                  where b.IdCliente == IdCliente && b.IdEstatus == 1
                                                                  select b).ToList();

                        if (prestacionesCliente.Count == 1)
                        {
                            res = prestacionesCliente[0].IdPrestaciones;
                        }
                    }
                    else
                    {
                        res = int.Parse(IdPrestaciones.ToString());
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Método que valia las prestaciones del patron por empleado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns>Identificador de la prestación</returns>
        private int validaPrestacionesByEmpleado(int IdEmpleado)
        {
            int res = 0;

            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                int? prestacionEmpleado = (from b in entidad.Empleados
                                           where b.IdEmpleado == IdEmpleado
                                           select b.IdPrestaciones).FirstOrDefault();

                if (prestacionEmpleado != null)
                {
                    res = (int)prestacionEmpleado;
                }
            }

            return res;
        }

        public int? GeneraClaveEmpleado(string Paterno, int UN)
        {
            string primeraLetra = Paterno.Substring(0, 1).ToUpper();
            // Buscamos el valor MAX del campo ClaveEmpleado donde la primer letra
            // del apellido paterno inicie con primeraLetra
            int? maxClaveEmpleado = ObtenerMaxClaveEmpleado(primeraLetra, UN);
            if (maxClaveEmpleado != null)
            {
                // El valor obtenido le sumamos 1
                return maxClaveEmpleado + 1;
            }
            else
            {
                // Si no hay valor máximo, se utiliza el primer valor de la siguiente escala numérica:
                switch (primeraLetra)
                {
                    case "A":
                        return 1; //rangoFin = 99;
                    case "B":
                        return 100; //rangoFin = 199;
                    case "C":
                        return 200; // rangoFin = 299;
                    case "D":
                        return 300; // rangoFin = 399;
                    case "E":
                        return 400; // rangoFin = 499;
                    case "F":
                        return 500; // rangoFin = 599;
                    case "G":
                        return 600; // rangoFin = 699;
                    case "H":
                        return 700; // rangoFin = 799;
                    case "I":
                        return 800; // rangoFin = 899;
                    case "J":
                        return 900; // rangoFin = 999;
                    case "K":
                        return 1000; // rangoFin = 1099;
                    case "L":
                        return 1100; // rangoFin = 1199;
                    case "M":
                        return 1200; // rangoFin = 1299;
                    case "N":
                        return 1300; // rangoFin = 1399;
                    case "Ñ":
                        return 1400; // rangoFin = 1499;
                    case "O":
                        return 1500; // rangoFin = 1599;
                    case "P":
                        return 1600; // rangoFin = 1699;
                    case "Q":
                        return 1700; // rangoFin = 1799;
                    case "R":
                        return 1800; // rangoFin = 1899;
                    case "S":
                        return 1900; // rangoFin = 1999;
                    case "T":
                        return 2000; // rangoFin = 2099;
                    case "U":
                        return 2100; // rangoFin = 2199;
                    case "V":
                        return 2200; // rangoFin = 2299;
                    case "W":
                        return 2300; // rangoFin = 2399;
                    case "Z":
                        return 2400; // rangoFin = 2499;
                    // Agregar casos para las demás letras
                    // ...

                    default:
                        // Manejar el caso de la letra no encontrada
                        return null;
                }
            }
        }

        /// <summary>
        /// Devuelve la clave de empleado máxima según la primera letra
        /// del apellido paterno y unidad de negocio especificados
        /// </summary>
        /// <param name="letra">Primera letra del apellido paterno</param>
        /// <param name="UN">El ID de la unidad de negocio</param>
        /// <returns>La máxima clave de empleado</returns>
        private int? ObtenerMaxClaveEmpleado(string letra, int UN)
        {
            ClassEmpleado classEmpleado= new ClassEmpleado();
            var ultimoEmpleado = classEmpleado.ObtenUltimoEmpleadoPorAP(letra, UN);
            if (ultimoEmpleado != null && !string.IsNullOrEmpty(ultimoEmpleado.ClaveEmpleado))
            {
                bool success = int.TryParse(ultimoEmpleado.ClaveEmpleado, out int maxClaveEmpleado);
                if (success && maxClaveEmpleado > 0)
                    return maxClaveEmpleado;
                else
                    return null;
            }
            else
                return null;
        }
    }
}
