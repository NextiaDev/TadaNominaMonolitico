using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class LAPlannedIntervalModel
    {
        //***********************************LAPlannedIntervalModel*************************************//
        public string Date { get; set; }
        public List<LAPunchesModel> Punches { get; set; }
        public List<ShiftModel> Shifts { get; set; }
        public string Delay { get; set; }
        public List<TimeOffModel> TimeOffs { get; set; }
        public string WorkedHours { get; set; }
        public string Absent { get; set; }
        public string Holiday { get; set; }
        public string Worked { get; set; }
        public string NonWorkedHours { get; set; }
        public Dictionary<string, string> AccomplishedExtraTimeBefore { get; set; }
        public Dictionary<string, string> AccomplishedExtraTimeAfter { get; set; }
        public Dictionary<string, string> AccomplishedExtraTime { get; set; }
        public Dictionary<string, string> AssignedExtraTimeBefore { get; set; }
        public Dictionary<string, string> AssignedExtraTimeAfter { get; set; }
        public Dictionary<string, string> AssignedExtraTime { get; set; }
    }
}