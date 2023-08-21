using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelNomina
    {
        [Display(Name = "Periodo de Nómina")]
        public int IdPeriodoNomina { get; set; }

        [Display(Name = "Registro patronal")]
        public int? IdRegistroPatronal { get; set; }

        [Display(Name = "Número de empleado")]
        public int IdEmpleado { get; set; }

        [Display(Name = "Banco de la patrona")]
        public int? IdBancoTrad { get; set; }

        //Dato de la tabla de Cat_RegistroPatronal
        public int IdBancoRegistroPatronal { get; set; }

        public string NombrePatrona { get; set; }

        public string NombreCorto { get; set; }


        //Datos para generar archivo txt.
        public string CuentaCargo { get; set; }

        public string CuentaAbono { get; set; }

        public int BancoReceptor { get; set; }

        public string Beneficiario { get; set; }

        public string Sucursal { get; set; }

        public string Importe { get; set; }

        public string Plaza { get; set; }

        public string PlazaBanxico { get; set; }

        public string Concepto { get; set; }

        public string EstadoCuenta { get; set; }

        public string RFC { get; set; }

        public string IVA { get; set; }

        public string ReferenciaOrdenante { get; set; }

        public string FormaAplicacion { get; set; }

    }
}