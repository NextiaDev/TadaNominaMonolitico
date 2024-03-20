using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelNominasCargadas
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
        public string FechaInicial { get; set; }
        [Display(Name = "Fecha Fin Periodo:")]
        public string FechaFinal { get; set; }

        [Display(Name = "Nombre de la Unidad de Negocio:")]
        public string UnidadNegocio { get; set; }

        [Display(Name = "Total de empleados Cargados:")]
        public int TotalEmpleadosCargados;

        [Display(Name = "Total de Sueldo:")]
        public string TotalSueldo { get; set; }

        [Display(Name = "Total de Subsidio:")]
        public string TotalSubsidio { get; set; }

        [Display(Name = "Total de Percepciones:")]
        public string TotalPercepciones { get; set; }

        [Display(Name = "Total de ISR:")]
        public string TotalISR { get; set; }

        [Display(Name = "Total de IMSS:")]
        public string TotalIMSS { get; set; }

        [Display(Name = "Total de Deducciones:")]
        public string TotalDeducciones { get; set; }

        [Display(Name = "Neto:")]
        public string Neto { get; set; }

        [Display(Name = "Total de ISN:")]
        public string TotalISN { get; set; }


        public bool validacion { get; set; }
        public string Mensaje { get; set; }
    }
}