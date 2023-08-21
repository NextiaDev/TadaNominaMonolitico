using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class LAPunchesModel
    {
        //***********************************LAPunchesModel*************************************//
        public string Type { get; set; }
        public string Date { get; set; }
        public string Origin { get; set; }
        public string UploadDate { get; set; }
        public string GroupDescription { get; set; }
        public string ShiftPunchType { get; set; }
        public bool AssignedInBook { get; set; }
    }
}