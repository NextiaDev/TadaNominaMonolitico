using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels
{
    public class ModelPerfil
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int IdCliente { get; set; }
        public int IdUnidadNegocio { get; set; }
        public List<SelectListItem> sLClientes { get; set; } 
        public List<SelectListItem> sLUnidades { get; set; }
        public List<Cat_Clientes> lClientes { get; set; }
        public List<Cat_UnidadNegocio> lUnidad { get; set; }
      
    }
}