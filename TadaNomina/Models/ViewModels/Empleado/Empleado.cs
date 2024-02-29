using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public int IdUnidadNegocio { get; set; }

        [Display(Name = "Centro de Costos")]
        public int? IdCentroCostos { get; set; }
        public List<SelectListItem> CentrosCostosList { get; set; }

        [Display(Name = "Departamento")]
        public int? IdDepartamento { get; set; }
        public List<SelectListItem> DepartamentoList { get; set; }

        [Display(Name = "Puesto")]
        public int? IdPuesto { get; set; }        
        public List<SelectListItem> PuestosList { get; set; }
        [Display(Name = "Sucursal")]
        public int? IdSucursal { get; set; }
        public List<SelectListItem> SucursalList { get; set; }

        [Display(Name = "Registro Patronal")]
        public int? IdRegistroPatronal { get; set; }
        public List<SelectListItem> RegistrosPatronalesList { get; set; }

        [Display(Name = "Entidad Donde Labora")]
        public int IdEntidad { get; set; }
        public List<SelectListItem> EntidadFederativaList { get; set; }

        [Display(Name = "Clave Empleado")]
        [Required(ErrorMessage = "{0} es campo requerido")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "{0} no valido, solo debe de contener números y letras")]
        public string ClaveEmpleado { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        //[RegularExpression("^[a-zA-ZñÑ ]*$", ErrorMessage = "{0} no valido, solo debe de contener letras")]
        [Display(Name = "Nombre(s)")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        //[RegularExpression("^[a-zA-ZñÑ ]*$", ErrorMessage = "{0} no valido, solo debe de contener letras")]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }

        [Display(Name = "Apellido Materno")]
        //[RegularExpression("^[a-zA-ZñÑ ]*$", ErrorMessage = "{0} no valido, solo debe de contener letras")]
        public string ApellidoMaterno { get; set; }

        [Display(Name = "Sexo")]
        public string Sexo { get; set; }
        public List<SelectListItem> SexoList { get; set; }

        [Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }
        public List<SelectListItem> EstadoCivilList { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [RegularExpression("^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", ErrorMessage = "{0} no cumple con el formaro de fecha \"dd/mm/aaaa\"")]
        public string FechaNacimiento { get; set; }

        [Display(Name = "Sueldo Diario")]
        public decimal? SD { get; set; }

        [Display(Name = "Sueldo Diario Base")]
        public decimal? SDIMSS { get; set; }

        [Display(Name = "Sueldo Diario Integrado")]
        public decimal? SDI { get; set; }

        [Display(Name = "Banco Sueldo Tradicional")]
        public int? IdBancoTrad { get; set; }
        public List<SelectListItem> BancosList { get; set; }

        [Display(Name = "Área")]
        public int? idArea { get; set; }

        [Display(Name = "Premio de puntualidad")]
        public bool PremioP { get; set; }
        public string Premio { get; set; }
        public bool RelojChecador { get; set; }
        public List<SelectListItem> AreaList { get; set; }

        [Display(Name = "S. Grupo")]
        public int? Idsindicato { get; set; }
        public List<SelectListItem> SindicatoList { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} no valido, solo debe de contener números")]
        [Display(Name = "Cuenta Bancaria Tradicional")]
        public string CuentaBancariaTrad { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} no valido, solo debe de contener números")]
        [Display(Name = "Cuenta Interbancaria Tradicional")]
        public string CuentaInterbancariaTrad { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        [MaxLength(18, ErrorMessage = "{0} debe contener 11 digitos")]
        [MinLength(18, ErrorMessage = "{0} debe contener 11 digitos")]
        //[RegularExpression("^[a-zA-Z]{4,4}[0-9]{6}[a-zA-Z]{6,6}[0-9]{2}$", ErrorMessage = "{0} no valido, no cumple con el formato establecido")]
        [Display(Name = "CURP")]
        public string Curp { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        [RegularExpression("^([A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?)?$", ErrorMessage = "{0} no valido, no cumple con el formato establecido")]
        [Display(Name = "RFC")]
        public string Rfc { get; set; }

        [MaxLength(11, ErrorMessage = "{0} debe contener 11 digitos")]
        [MinLength(11, ErrorMessage = "{0} debe contener 11 digitos")]
        [Display(Name = "NSS")]
        public string Imss { get; set; }

        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string CorreoElectronico { get; set; }

        [Display(Name = "Fecha de Alta SS")]
        [RegularExpression("^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", ErrorMessage = "{0} no cumple con el formaro de fecha \"dd/mm/aaaa\"")]
        public string FechaAltaIMSS { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        [Display(Name = "Fecha de Reconocimiento Antigüedad")]
        [RegularExpression("^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", ErrorMessage = "{0} no cumple con el formaro de fecha \"dd/mm/aaaa\"")]
        public string FechaReconocimientoAntiguedad { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        [Display(Name = "Empleado con Esquema")]
        public string Esquema { get; set; }
        public List<SelectListItem> EsquemasList { get; set; }

        [Required(ErrorMessage = "{0} es campo requerido")]
        [Display(Name = "Tipo de contrato")]
        public string TipoContrato { get; set; }
        public List<SelectListItem> TiposContratoList { get; set; }

        [Display(Name = "Estatus")]
        public int IdEstatus { get; set; }
        public List<SelectListItem> EstatusList { get; set; }

        public int IdCaptura { get; set; }

        public DateTime FechaCaptura { get; set; }

        [RegularExpression("^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", ErrorMessage = "{0} no cumple con el formaro de fecha \"dd/mm/aaaa\"")]
        [Display(Name = "Fecha de Baja")]
        public string FechaBaja { get; set; }

        [Display(Name = "RFC Subcontratación")]
        [RegularExpression("^([A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?)?$", ErrorMessage = "{0} no valido, no cumple con el formato establecido")]
        public string RFCSubContratacion { get; set; }

        public int IdModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Código Postal")]
        public string CodigoPostalFiscal { get; set; }
        [Display(Name = "Colonia")]
        public int? IdCodigoPostalFiscal { get; set; }
        [Display(Name = "Alcaldía / Municipio")]
        public string MunicipioFiscal { get; set; }
        [Display(Name = "Entidad Federativa")]
        public string EntidadFiscal { get; set; }
        [Display(Name = "Calle")]
        public string CalleFiscal { get; set; }

        [Display(Name = "Número Exterior")]
        public string NumeroExtFiscal { get; set; }

        [Display(Name = "Número Interior")]
        public string NumeroIntFiscal { get; set; }

        [Display(Name = "Nacionalidad")]
        public string Nacionalidad { get; set; }

        //Informacion complementaria
        [Display(Name = "Código Postal")]
        public string CodigoPostal { get; set; }

        [Display(Name = "Colonia")]
        public int? IdCodigoPostal { get; set; }
        public List<SelectListItem> CodigosPostalesList { get; set; }
        public List<SelectListItem> CodigosPostalesListFiscales { get; set; }

        [Display(Name = "Alcaldía / Municipio")]
        public string Municipio { get; set; }

        [Display(Name = "Entidad Federativa")]
        public string Entidad { get; set; }

        [Display(Name = "Calle")]
        public string Calle { get; set; }

        [Display(Name = "Número Telefónico")]
        public string NumeroTelefonico { get; set; }

        [Display(Name = "Número Exterior")]
        public string NumeroExt { get; set; }

        [Display(Name = "Número Interior")]
        public string NumeroInt { get; set; }

        [Display(Name = "Motivo de Baja")]
        public string MotivoBaja { get; set; }
        public List<SelectListItem> MotivoBajaExterno { get; set; }
        public List<SelectListItem> MotivoBajaInterno { get; set; }

        [Display(Name = "Recontratable")]
        public string Recontratable { get; set;  }
        public List<SelectListItem> RecontratableList { get; set; }

      
        [Display(Name = "Prestaciones")]
        public int? IdPrestaciones { get; set; }
        public string Prestaciones { get; set; }
        public List<SelectListItem> PrestacionesList { get; set; }

        public List<EmpleadoPeriodo> lPeriodosProcesados { get; set; }
        public int idcliente { get; set; }

        [Display(Name = "Jornada Laboral")]
        public int? IdJornada { get; set; }
        public List<SelectListItem> JornadaList { get; set; }

        [Display(Name = "Concepto de Pago")]

        public string ConceptoPago { get; set; }

        [Display(Name = "Honorarios Brutos")]
        public decimal? HonorariosB { get; set; }

        [Display(Name = "Honorarios Netos")]
        public decimal? HonorariosN { get; set; }


        [Display(Name = "Subtotal")]

        public decimal Subtotal { get; set; }

        [Display(Name = "IVA")]

        public decimal iva { get; set; }

        [Display(Name = "Total factura")]

        public decimal totalfactura { get; set; }

        [Display(Name = "Retencion ISR")]

        public decimal retencionisr { get; set; }

        [Display(Name = "Retencion IVA")]

        public decimal retencioniva { get; set; }

        [Display(Name = "Total")]
        public decimal totalRetencion { get; set; }

        [Required]
        [Display(Name = "Seleccione el Periodo:")]
        public int IdPeriodoNomina { get; set; }
        public List<SelectListItem> lPeriodos { get; set; }


        [Required]
        [Display(Name = "Seleccione el Empleado:")]
        public int idempleadoH { get; set; }
        public List<SelectListItem> LEmpleados { get; set; }

        public int? IdBancoViaticos { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} no valido, solo debe de contener números")]
        public string CuentaBancariaViaticos { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} no valido, solo debe de contener números")]
        public string CuentaInterBancariaViaticos { get; set; }
        [Display(Name = "Fecha Termino Contrato:")]
        [RegularExpression("^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", ErrorMessage = "{0} no cumple con el formaro de fecha \"dd/mm/aaaa\"")]
        public string FechaTerminoContrato { get; set; }

        [Display(Name = "Sindicatos")]
        public int? IdSindicatos { get; set; }


        [Display(Name = "Cuota Sindical")]
        public decimal NSindicato { get; set; }


        [Display(Name = "Observaciones Cliente:")]

        public string ObservacionesCliente { get; set; }

        [Display(Name = "Observaciones Usuario:")]
        public string ObservacionesUsuario { get; set; }

        public List<SelectListItem> HonorarioFactura { get; set; }

        [Display(Name = "Observaciones :")]
        public string ObservacionesH { get; set; }

        [Display(Name = "Patrona")]

        public string Patrona { get; set; }

        [Display(Name = "Periodo")]

        public string Periodo { get; set; }

        [Display(Name = "Contrato")]

        public string Contrato { get; set; }

   

        [Display(Name = "Descripcion Facturacion")]
        public int? idHonorarioF { get; set; }


        [Display(Name = "Metodo de pago")]

        public string MetodoP { get; set; }

        [Display(Name = "Forma de pago")]

        public string Formap { get; set; }

        [Display(Name = "Uso de CFDI")]

        public string UsoCFDI { get; set; }


        [Display(Name = "Registro Patronal")]
        public string idreg { get; set; }

        public string Rfc_Emisor { get; set; }
        public string Rfc_Receptor { get; set; }
        public decimal TotalXML { get; set; }
        public string UUID { get; set; }

        public int idHono { get; set; }
        public List<SelectListItem> NacionalidadList { get; set; }
        public List<SelectListItem> SindicatosoList { get; set; }

        [Display(Name = "Participa en el Timbrado")]
        public string IdTimbrado { get; set; }
        public List<SelectListItem> TimbradoList { get; set; }


    }
}

