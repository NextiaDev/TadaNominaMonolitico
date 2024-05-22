using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TadaNomina.Models.ViewModels
{
    public class ModelLogin
    {
        [Required]
        public string username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string mensaje { get; set; }
    }
}
