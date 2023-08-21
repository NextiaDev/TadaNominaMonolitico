using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Facturacion
{
    public class ModelClienteRazonSocialFacturacion
    {

        public int IdClienteRazonSocialFacturacion { get; set; }
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public string RFC { get; set; }
        public string Correo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public int IdGrupoFacturacion { get; set; }
        public int IdEstatus { get; set; }
        public int IdCaptura { get; set; }
        public DateTime FechaCaptura { get; set; }
        public int IdModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public string Grupo { get; set; }
    }
}