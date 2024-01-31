using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class ShiftModel
    {
        //***********************************Shift*************************************//
        //Insert~Token

        //Petición
        public string StartHour { get; set; } //Required
        public string MaxStartHour { get; set; } //Required
        public string EndHour { get; set; } //Required
        public string BreakStart { get; set; } //Required
        public string BreakEnd { get; set; } //Required
        public int? BreakMinutes { get; set; }
        public string ShiftHours { get; set; } //Required
        public string Custom { get; set; }
        public string ShiftDay { get; set; }

        //Respuesta
        //El método retorna el identificador del turno encriptado como cadena de texto.

        ////////////////////////////////////////////////////////////////////////////////
        //List~OAuth

        //Petición
        //La petición no contiene parámetros de entrada.

        //Respuesta
        public string ID_SHIFT { get; set; } //Required
        public string DESCRIPTION { get; set; } //Required
        public string TYPE_SHIFT { get; set; }
        public string START_HOUR { get; set; }
        public string END_HOUR { get; set; }
        public string START_BREAK { get; set; }
        public string END_BREAK { get; set; }
        public string BREAK_TYPE { get; set; }
        public List<UserModel> BREAK_MINUTES { get; set; }
        public int SHIFT_HOURS { get; set; }
        public int CUSTOM { get; set; }
        public int EXTERNAL_SHIFT_ID { get; set; }
        public bool ENABLED { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        //AttendanceBook
        public string StartTime { get; set; }
        public string ExitTime { get; set; }
        public string FixedShiftHours { get; set; }
        public string Ends { get; set; }
        public string Begins { get; set; }
        public string ShiftDisplay { get; set; }
        //public string BreakMinutes {get;set;} // Insert-Peticion
        //public string BreakStart {get;set;} // Insert-Peticion
        //public string BreakEnd {get;set;} // Insert-Peticion
        public string BreakDelay { get; set; }
        public string EarlyLeave { get; set; }
        public string Type { get; set; }
    }
}