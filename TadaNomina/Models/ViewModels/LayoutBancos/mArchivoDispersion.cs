using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.LayoutBancos
{
    public class mArchivoDispersion
    {
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> selectListPeriodoNomina { get; set; }
        public int IdBanco { get; set; }
        public List<SelectListItem> selectListBancos { get; set; }
        public int TipoArchivo { get; set; }
        public List<SelectListItem> selectListTipoArchivos { get; set; }
        public string NumCliente { get; set; }
        public string ClvSucursal { get; set; }
        public string RefNumerica { get; set; }
        public string RefAlfaNum { get; set; }
        public string NombreEmpresa { get; set; }
        public string Empresa { get; set; }
        public int TipoArchivoBnte { get; set; }
        public int TipoArchivoBajio { get; set; }
    }
}