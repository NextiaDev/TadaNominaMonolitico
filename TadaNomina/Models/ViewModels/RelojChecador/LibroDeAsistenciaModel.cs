using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class LibroDeAsistenciaModel
    {
        //***********************************Libro de asistencia*************************************//
        //AttendanceBook~Token

        //Peticion
        public string StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string EndDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string UserIds { get; set; }

        //Respuesta
        public List<UserModel> Users { get; set; }
        public List<LAExtraTimeValuesModel> ExtraTimeValues { get; set; }
    }
}