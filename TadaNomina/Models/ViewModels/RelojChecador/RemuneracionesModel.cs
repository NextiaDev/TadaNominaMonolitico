using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class RemuneracionesModel
    {
        public int IncludeAll { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string EndDate { get; set; }
        public string UserIds { get; set; }
        public string Identifier { get; set; }
        public string WorkedHours { get; set; }
        public string NonWorkedHours { get; set; }
        public string TotalAuthorizedExtraTime { get; set; }
        public int Absent { get; set; }
        public Dictionary<string,string> AccomplishedExtraTime { get; set; }
    }
}