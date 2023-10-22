using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class ModelPolizaGral
    {
        public int? IdPoliza { get; set; }
        [Required]
        [Display(Name = "Periodo Nómina")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }
        public List<mReporteCuentas> reporte { get; set; }
        public List<mReporteCuentasWS> reporteWS { get; set; }
        public int? IdRegistroPatronal { get; set; }
        [Display(Name = "Registro Patronal")]
        public string RFC { get; set; }
        public List<SelectListItem> lRegistros { get; set; }
    }
}