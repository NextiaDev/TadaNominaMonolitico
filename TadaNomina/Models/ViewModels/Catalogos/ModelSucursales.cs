using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelSucursales
    {
        [Key]
        public int IdSucursal { get; set; }
        public int IdCliente { get; set; }
        [Display(Name = "Clave Sucursal:")]
        public string Clave { get; set; }
        [Display(Name = "Nombre Sucursal:")]
        public string Sucursal { get; set; }

        [Display(Name = "Seleccionar Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }

        [Display(Name = "Dirección Completa: ")]
        public string Direccion { get; set; }
        [Display(Name = "Código Postal: ")]
        public string CP { get; set; }
        [Display(Name = "País:")]
        public string Pais { get; set; }
        [Display(Name = "Entidad Federativa: ")]
        public string EntidadFederativa { get; set; }
        [Display(Name = "Municipio: ")]
        public string Municipio { get; set; }
        [Display(Name = "Colonia: ")]
        public string Colonia { get; set; }
        [Display(Name = "Calle: ")]
        public string Calle { get; set; }


        [Display(Name = "Numero Int: ")]
        public string Nunterior { get; set; }
        [Display(Name = "Numero Ext: ")]
        public string NuExterior { get; set; }

        public string Mensaje { get; set; }
        public bool validacion { get; set; }

    }
}