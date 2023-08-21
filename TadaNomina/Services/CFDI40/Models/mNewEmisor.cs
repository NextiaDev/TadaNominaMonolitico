using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Services.CFDI40.Models
{
    public class mNewEmisor
    {        
        public string RFCEmisor { get; set; }
        public string RazonSocial { get; set; }
        public string RegimenFiscal { get; set; }
        public string FacAtribAdquiriente { get; set; }
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public string Colonia { get; set; }
        public string DelegacionMunicipio { get; set; }
        public string LugarExpedicion { get; set; }
        public string Estado { get; set; }
        public string Telefono { get; set; }
        public string EmailEmisor { get; set; }    
    }
}