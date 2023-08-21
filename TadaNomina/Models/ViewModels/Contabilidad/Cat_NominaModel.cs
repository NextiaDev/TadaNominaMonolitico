using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Contabilidad
{
    public class Cat_NominaModel
    {
        public int idCatTipoNomina { get; set; }
        public string nombre { get; set; }
        public string campoDb { get; set; }
        public int idEstatus { get; set; }
    }
}