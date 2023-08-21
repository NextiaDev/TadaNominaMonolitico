using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mAuth
    {

        public string Access_Token { get; set; }
        public string Type_Token { get; set; }
        public string Expires_In { get; set; }
        public string Cliente { get; set; }
        public string Start_In { get; set; }
        public List<mErrores> Errores { get; set; }

    }
}