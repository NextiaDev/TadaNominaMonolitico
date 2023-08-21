using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ClassCore;
using System.ComponentModel.DataAnnotations;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelFacturadoras
    {
        public int IdFacturadora { get; set; }
        public int? IdBanco { get; set; }
        public int? IdBanco2 { get; set; }
        public string Facturadora { get; set; }
        public string Domicilio { get; set; }
        public string RFC { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public string FechaOperacionString { get; set; }

        [StringLength(10,ErrorMessage ="Longitud máxima de 10 caracteres")]
        public string ClaveFacturadora { get; set; }
        public string TipoEsquema { get; set; }

        public decimal? FacturacionMensual { get; set; }
        public decimal? FacturacionAnual { get; set; }
        public string UsuarioMail { get; set; }
        public string PasswordMail { get; set; }
        public int? PuertoMail { get; set; }
        public string HostMail { get; set; }
        public int? IdEstatus { get; set; }
        public int? IdCaptura { get; set; }
        public int? IdModifica { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public DateTime? FechaModifica { get; set; }




    }
}