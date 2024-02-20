using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelProcesaNominaGeneral
    {
        [Key]
        public int IdPeriodoNomina { get; set; }
        public int IdCliente { get; set; }
        [Display(Name = "Nombre del Cliente:")]
        public string Cliente { get; set; }
        [Display(Name = "Periodo de Nómina:")]        
        public string Periodo { get; set; }
        [Display(Name = "Tipo de Periodo:")]
        public string TipoNomina { get; set; }
        [Display(Name = "Periodicidad de Pago:")]
        public string Periodicidad { get; set; }
        [Display(Name = "Porcentaje para el Impuesto Sobre Nómina:")]
        public decimal PorcentajeISN { get; set; }
        [Display(Name = "Se procesa con ajuste de impuestos:")]
        public string AjusteImpuestos { get; set; }
        [Display(Name = "Periodos con los que ajusta:")]
        public string PeriodoAjuste { get; set; }
        [Display(Name = "Fecha Inicio Periodo:")]
        public string FechaInicial { get; set; }
        [Display(Name = "Fecha Fin Periodo:")]
        public string FechaFinal { get; set; }
        [Display(Name = "Nombre de la Unidad de Negocio:")]
        public string UnidadNegocio { get; set; }
        [Display(Name = "Total de empleados a Procesar:")]
        public int TotalEmpleados { get; set; }
        [Display(Name = "Total a pagar de sueldos:")]
        public string TotalPagarSueldos { get; set; }
        [Display(Name = "Total a pagar de ISR:")]
        public string TotalISR { get; set; }
        [Display(Name = "Total a pagar de Cuotas Obreras:")]
        public string TotalIMSS { get; set; }
        [Display(Name = "Total a pagar de Cuotas Patronales:")]
        public string TotalIMSS_P { get; set; }
        [Display(Name = "Total a pagar ISN:")]
        public string TotalISN { get; set; }
        [Display(Name = "Incidencias Aguinaldo Automaticas")]
        public int? IncidenciasAguinaldoAutomaticas { get; set; }
        public bool AguinaldoSINO { get; set; }
        [Display(Name = "Empleados a procesar(Claves):")]
        [DataType(DataType.MultilineText)]
        public string Claves { get; set; }
        public string Ids { get; set; }
        public List<ModelEmpleadoFiniquito> empleados { get; set; }
        public List<ModelEmpleadoFiniquito> empleadosOtros { get; set; }
        public string _empleadosOtros { get; set; }
        public string AJusteAnual { get; set; }
        [Display(Name = "Configuración de Sueldos:")]
        public string ConfiguracionSueldos {  get; set; }
        
        public bool validacion { get; set; }
        public string Mensaje { get; set; }

        //datos unicamente para finiquitos
        public bool banderaVacGral { get; set; }
        public bool banderaPVGral { get; set; }
        public bool banderaAguiGral { get; set; }       
        public bool bandera90dGral { get; set; }
        public bool bandera20dGral { get; set; }
        public bool banderaPAGral { get; set; }
        public bool LiquidacionSDIGral { get; set; }
        public string ConceptosSDI { get; set; }

        public int[] IdsPeriodos { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
        public int[] IdsPeriodosSelecionados { get; set; }
        public List<SelectListItem> lPeriodosSeleccionados { get; set; }

        public int EmpleadosSinSDI { get; set; }
        public Dictionary<string, string> LEmpleadosSinSDI { get; set; }
    }
}