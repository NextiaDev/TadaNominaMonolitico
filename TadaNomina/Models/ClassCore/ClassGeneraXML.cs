using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class ClassGeneraXML
    {
        public decimal TotalGravadoPercepciones { get; set; }
        public decimal TotalExcentoPercepciones { get; set; }
        public string XMLTotalPercepciones { get; set; }



    }
}