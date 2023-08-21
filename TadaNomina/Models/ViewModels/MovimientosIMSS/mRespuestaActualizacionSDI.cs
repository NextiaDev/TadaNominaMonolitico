using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.MovimientosIMSS
{
    public class mRespuestaActualizacionSDI
    {
        public string Path { get; set; }
        public int noRegistros { get; set; }
        public int Correctos { get; set; }
        public int Errores { get; set; }
        public List<string> listErrores { get; set; }
    }
}