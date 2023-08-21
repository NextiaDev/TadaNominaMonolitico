using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore
{
    public class DatosXML
    {
        public static string LugarExpedicion = "Distrito Federal";
        public static string MetodoDePago = "03";
        public static string tipoDeComprobante = "egreso";
        public static string motivoDescuento = "Deducciones nomina";
        public static string formaDePago = "Pago en una sola exhibición";
        public string fecha { get; set; }
        public static string version = "3.2";
        public static string paisExpedicion = "México";
        public static string RegimenFiscal = "Regimen General de Ley de Personas Morales";
        public static string paisReceptor = "México";
        public static string conceptoNoIdentificacion = "Recibo de nomina";
        public static string conceptoDescripcion = "Pago de nomina";
        public static string conceptoUnidad = "Servicio";
        public static string conceptoCantidad = "1";
        public static string retencionesImpuesto = "ISR";
        public static string RiesgoPuesto = "1";
        public static string TipoJornada = "Diurna";
        public string TipoContrato = "Base";
        public static string TipoRegimen = "2";
        public static string nominaVersion = "1.1";
        public string IdEmpleado { get; set; }
        public string IdRegistroPatronal { get; set; }
        public string Nombre { get; set; }
        public string Certificado { get; set; }
        public string noCertificado { get; set; }
        public string Sello { get; set; }
        public string total { get; set; }
        public string Descuento { get; set; }
        public string subTotal { get; set; }
        public string Emisor_Rfc { get; set; }
        public string razonSocial { get; set; }
        public string codigoPostalEmisor { get; set; }
        public string paisEmisor { get; set; }
        public string estadoEmisor { get; set; }
        public string municipioEmisor { get; set; }
        public string calleEmisor { get; set; }
        public string Emisor_Nombre { get; set; }
        public string Receptor_Rfc { get; set; }
        public string Receptor_Nombre { get; set; }
        public string conceptoImporte { get; set; }
        public string conceptoValorUnitario { get; set; }
        public string totalImpuestosRetenidos { get; set; }
        public string retencionesImporte { get; set; }
        public string SalarioDiarioIntegrado { get; set; }
        public string SalarioBaseCotApor { get; set; }
        public string PeriodicidadPago { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public string FechaInicioRelLaboral { get; set; }
        public string NumDiasPagados { get; set; }
        public string FechaPago { get; set; }
        public string NumSeguridadSocial { get; set; }
        public string CURP { get; set; }
        public string NumEmpleado { get; set; }
        public string RegistroPatronal { get; set; }
        public string Periodo { get; set; }
        public string totalPercepciones { get; set; }
        public string totalDeducciones { get; set; }
        public string Subsidio { get; set; }
        public string SubsidioPagar { get; set; }
        public string ReintegroISR { get; set; }
        public string PrimaVacacional { get; set; }
        public string ISPT { get; set; }
        public string IMSS { get; set; }
        public string IdPeriodo { get; set; }
        public string DiaFestivo { get; set; }
        public decimal DeduccionesTotalExcento { get; set; }
        public decimal DeduccionesTotalGravado { get; set; }
        public decimal PercepcionTotalExcento { get; set; }
        public decimal PercepcionTotalGravado { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string IdServicioPAC { get; set; }
        public string UsuarioPAC { get; set; }
        public string ContrasenaPAC { get; set; }
        public string DescInfonavit { get; set; }
        public string Vacaciones { get; set; }
        public string DiasVacaciones { get; set; }
        public string Prima_Vacacional_Esquema { get; set; }
        public string Esquema { get; set; }
        public string ClaveEntFed { get; set; }
        public string Clase { get; set; }
        public string Antiguedad { get; set; }
        public string FechaReconocimientoAntiguedad { get; set; }
        public string Neto { get; set; }
        public string ClaveBanco { get; set; }
        public string CuentaBancaria { get; set; }
        public string CuentaInterbancariaTrad { get; set; }
        public string Leyenda { get; set; }
        public string ImporteVales { get; set; }
        public string ImporteValesPensionadas { get; set; }
        public string SueldoMensual { get; set; }
        public string RFCSubcontratacion { get; set; }
        public string rutaCer { get; set; }
        public string rutaKey { get; set; }
        public string keyPass { get; set; }
        public string Liquidacion_Gravado { get; set; }
        public string Liquidacion_Exento { get; set; }
        public string SueldosPagados { get; set; }
        public string FechaBaja { get; set; }
        public string _TipoNomina { get; set; }
        public string FechaDispersion { get; set; }

        //cambios sat
        public string ISR_AjustadoPorSubsidio { get; set; }
        public string SubsidioEntregadoNoCorrespondia { get; set; }
        public string AjusteAlSubsidioCausado { get; set; }
        public string ISR_PeriodoAjuste { get; set; }
        public string AjusteAlSubsidioEntregado { get; set; }
    }
}