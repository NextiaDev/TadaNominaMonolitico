using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class TimeOffModel
    {
        ////***********************************TimeOff*************************************//
        //GetTypes~Token

        //Peticion
        //La petición no contiene parámetros de entrada.

        //Respuesta
        public string Id { get; set; }
        [Display(Name = "Tipo de Permiso")]
        public string TranslatedDescription { get; set; }
        public string Status { get; set; }
        public string IsPayable { get; set; }

        /////////////////////////////////////////////////////////////////////////////////////
        //Get~Token

        //Peticion
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public string StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de termino")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public string EndDate { get; set; }
        public List<int> UserIds { get; set; }

        //Respuesta
        public string TimeOffTypeId { get; set; }
        public string Description { get; set; }
        public string Starts { get; set; }
        public string Ends { get; set; }
        public string TimeOffTypeDescription { get; set; }
        public string TimeOffOrigin { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora de inicio")]
        public string StartTime { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora de termino")]
        public string EndTime { get; set; }
        public string HolidayPeriod { get; set; }
        public string HolidayTypeCode { get; set; }

        /////////////////////////////////////////////////////////////////////////////////////
        //Upsert~Token

        //Peticion
        public string UserIdentifier { get; set; } //Requerido
        //public string TimeOffTypeId {get;set;} //Requerido || Get-Respuesta
        //public string StartDate {get;set;} //Requerido || Get-Peticion
        //public string EndDate {get;set;} //Requerido || Get-Peticion 
        public string CreatedByIdentifier { get; set; } //Requerido || Get-Respuesta
        //public string Description {get;set;} // Get-Respuesta
        //public string StartTime {get;set;} //Requerido || Get-Respuesta 
        //public string EndTime {get;set;} //Requerido || Get-Respuesta
        public string Origin { get; set; }

        //Respuesta
        //La respuesta que entrega el método si la acción ha sido exitosa es true, en caso que no se haya llevado a cabo la modificación el método responderá con información del error.

        /////////////////////////////////////////////////////////////////////////////////////
        //Delete~Token

        //Peticion
        //public string UserIdentifier {get;set;} //Required
        public string Start { get; set; } //Required
        public string End { get; set; } //Required
        public string TypeIdentifier { get; set; } //Required

        //Respuesta
        //La respuesta que entrega el método si la acción ha sido exitosa es true, en caso que no se haya llevado a cabo la modificación el método responderá con información del error.

        /////////////////////////////////////////////////////////////////////////////////////
        //AttendanceBook
        //public string TimeOffTypeId {get;set;} // Get-Respuesta
        //public string Starts {get;set;} // Get-Respuesta
        //public string Ends {get;set;} // Get-Respuesta
        //public string TimeOffTypeDescription {get;set;} // Get-Respuesta
        //public string TimeOffOrigin {get;set;} // Get-Respuesta
        //public string StartTime {get;set;} // Get-Respuesta
        //public string EndTime {get;set;} // Get-Respuesta


    }
}