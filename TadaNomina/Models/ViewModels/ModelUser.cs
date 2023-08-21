using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels
{
    public class ModelUser
    {
        public ModelUserInfo User { get; set; }
        public List<string> Modulo { get; set; }
        public string Token { get; set; }
    }
}