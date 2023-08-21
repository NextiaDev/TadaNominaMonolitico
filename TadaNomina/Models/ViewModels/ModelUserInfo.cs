using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels
{
    public class ModelUserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string IdCliente { get; set; }
        public string IdUnidades { get; set; }
        public string Foto { get; set; }
        public string Tipo { get; set; }
        public string Correo { get; set; }
        public int? Confidencial { get; set; }
        public string relojChecador { get; set; }
    }
}