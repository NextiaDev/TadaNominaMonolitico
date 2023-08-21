using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelErroresAreas
    {
        public string Path { get; set; }
        public int noRegistro { get; set; }
        public int Correctos { get; set; }
        public int Errores { get; set; }

        public List<string> listErrores { get; set; }
    }
}