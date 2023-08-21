using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelConfiguracionFechasCalculos
    {
        [Display(Name = "Fecha Vacaciones Real:")]
        public string FechaVacacionesReal { get; set; }
        public List<SelectListItem> lFechasVR { get; set; }

        [Display(Name="Fecha Vacaciones:")]
        public string FechaVacaciones { get; set; }
        public List<SelectListItem> lFechasV { get; set; }

        [Display(Name = "Fecha Vacaciones Esquema:")]        
        public string FechaVacacionesEsq { get; set; }
        public List<SelectListItem> lFechasVE { get; set; }

        [Display(Name = "Fecha Prima Vacacional Real:")]        
        public string FechaPVReal { get; set; }
        public List<SelectListItem> lFechasPVR { get; set; }

        [Display(Name = "Fecha Prima Vacacional:")]
        public string FechaPV { get; set; }
        public List<SelectListItem> lFechasPV { get; set; }

        [Display(Name = "Fecha Prima Vacacional Esquema:")]
        public string FechaPVEsq { get; set; }
        public List<SelectListItem> lFechasPVE { get; set; }

        [Display(Name = "Fecha Aguinaldo Real:")]
        public string FechaAguinaldoReal { get; set; }
        public List<SelectListItem> lFechasAR { get; set; }

        [Display(Name = "Fecha Aguinaldo:")]
        public string FechaAguinaldo { get; set; }
        public List<SelectListItem> lFechasA { get; set; }

        [Display(Name = "Fecha Aguinado Esquema:")]
        public string FechaAguinaldoEsq { get; set; }
        public List<SelectListItem> lFechasAE { get; set; }

        [Display(Name = "Fecha Liquidación Real:")]
        public string FechaLiquidacionReal { get; set; }
        public List<SelectListItem> lFechasLR { get; set; }

        [Display(Name = "Fecha Liquidación:")]
        public string FechaLiquidacion { get; set; }
        public List<SelectListItem> lFechasL { get; set; }

        [Display(Name = "Fecha Liquidación Esquema:")]
        public string FechaLiquidacionEsq { get; set; }
        public List<SelectListItem> lFechasLE { get; set; }
        

        public string Mensaje { get; set; }
        public bool validacion { get; set; }
    }
}