using API_Nomors.Core.CFDI40;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.TimbradoTP.CFDI40
{
    public class cTipos
    {
        /// <summary>
        /// Metodo para convertir el tipo de jornada de String a Enum
        /// </summary>
        /// <param name="tipoJornada">Clave de tipo de Jornada</param>
        /// <returns>Enum clave jornada</returns>
        public c_TipoJornada getTipoJornada(string tipoJornada)
        {
            c_TipoJornada cTipoJornada;
            switch (tipoJornada)
            {
                case "01":
                    cTipoJornada = c_TipoJornada.Item01;
                    break;
                case "02":
                    cTipoJornada = c_TipoJornada.Item02;
                    break;
                case "03":
                    cTipoJornada = c_TipoJornada.Item03;
                    break;
                case "04":
                    cTipoJornada = c_TipoJornada.Item04;
                    break;
                case "05":
                    cTipoJornada = c_TipoJornada.Item05;
                    break;
                case "06":
                    cTipoJornada = c_TipoJornada.Item06;
                    break;
                case "07":
                    cTipoJornada = c_TipoJornada.Item07;
                    break;
                case "08":
                    cTipoJornada = c_TipoJornada.Item08;
                    break;
                case "99":
                    cTipoJornada = c_TipoJornada.Item99;
                    break;
                default:
                    cTipoJornada = c_TipoJornada.Item99;
                    break;
            }

            return cTipoJornada;
        }

        /// <summary>
        /// Metodo para convertir la clave del regimen de String a Enum 
        /// </summary>
        /// <param name="tipoRegimen">Clave del tipo de regimen</param>
        /// <returns>Enum de clave del regimen</returns>
        public c_TipoRegimen getTipoRegimen(string tipoRegimen)
        {
            c_TipoRegimen ctipoRegimen;
            switch (tipoRegimen)
            {
                case "02":
                    ctipoRegimen = c_TipoRegimen.Item02;
                    break;
                case "03":
                    ctipoRegimen = c_TipoRegimen.Item03;
                    break;
                case "04":
                    ctipoRegimen = c_TipoRegimen.Item04;
                    break;
                case "05":
                    ctipoRegimen = c_TipoRegimen.Item05;
                    break;
                case "06":
                    ctipoRegimen = c_TipoRegimen.Item06;
                    break;
                case "07":
                    ctipoRegimen = c_TipoRegimen.Item07;
                    break;
                case "08":
                    ctipoRegimen = c_TipoRegimen.Item08;
                    break;
                case "09":
                    ctipoRegimen = c_TipoRegimen.Item09;
                    break;
                case "10":
                    ctipoRegimen = c_TipoRegimen.Item10;
                    break;
                case "11":
                    ctipoRegimen = c_TipoRegimen.Item11;
                    break;
                case "12":
                    ctipoRegimen = c_TipoRegimen.Item12;
                    break;
                case "13":
                    ctipoRegimen = c_TipoRegimen.Item13;
                    break;
                case "99":
                    ctipoRegimen = c_TipoRegimen.Item99;
                    break;
                default:
                    ctipoRegimen = c_TipoRegimen.Item99;
                    break;
            }

            return ctipoRegimen;
        }

        /// <summary>
        /// Metodo para convertir la clave del tipo de contrato de String a Enum
        /// </summary>
        /// <param name="TipoContrato">Clave del tipo de contrato</param>
        /// <returns>Enum de clave del tipo de contrato</returns>
        public c_TipoContrato getTipoContrato(string TipoContrato)
        {
            c_TipoContrato contrato;
            switch (TipoContrato)
            {
                case "01":
                    contrato = c_TipoContrato.Item01;
                    break;
                case "02":
                    contrato = c_TipoContrato.Item02;
                    break;
                case "03":
                    contrato = c_TipoContrato.Item03;
                    break;
                case "04":
                    contrato = c_TipoContrato.Item04;
                    break;
                case "05":
                    contrato = c_TipoContrato.Item05;
                    break;
                case "06":
                    contrato = c_TipoContrato.Item06;
                    break;
                case "07":
                    contrato = c_TipoContrato.Item07;
                    break;
                case "08":
                    contrato = c_TipoContrato.Item08;
                    break;
                case "09":
                    contrato = c_TipoContrato.Item09;
                    break;
                default:
                    contrato = c_TipoContrato.Item99;
                    break;
            }

            return contrato;
        }

        /// <summary>
        /// Metodo para convertir el riesgo del puesto de String a Enum
        /// </summary>
        /// <param name="_Riesgo">Número de rieso del puesto</param>
        /// <returns>Enum del número del puesto</returns>
        public c_RiesgoPuesto ObtenRiesgoPuesto(string _Riesgo)
        {
            c_RiesgoPuesto riesgo = c_RiesgoPuesto.Item99;
            switch (_Riesgo)
            {
                case "1":
                    riesgo = c_RiesgoPuesto.Item1;
                    break;
                case "2":
                    riesgo = c_RiesgoPuesto.Item2;
                    break;
                case "3":
                    riesgo = c_RiesgoPuesto.Item3;
                    break;
                case "4":
                    riesgo = c_RiesgoPuesto.Item4;
                    break;
                case "5":
                    riesgo = c_RiesgoPuesto.Item5;
                    break;
                default:
                    riesgo = c_RiesgoPuesto.Item99;
                    break;
            }

            return riesgo;
        }

        /// <summary>
        /// metodo para convertir la clave de periodicidad del pago de String a Enum
        /// </summary>
        /// <param name="_Periodicidad">Clave de periodicidad</param>
        /// <returns>Enum de clave de periodicidad</returns>
        public c_PeriodicidadPago ObtenPeriodicidadPago(string _Periodicidad)
        {
            c_PeriodicidadPago periodicidad = c_PeriodicidadPago.Item99;
            switch (_Periodicidad)
            {
                case "02":
                    periodicidad = c_PeriodicidadPago.Item02;
                    break;
                case "03":
                    periodicidad = c_PeriodicidadPago.Item03;
                    break;
                case "04":
                    periodicidad = c_PeriodicidadPago.Item04;
                    break;
                case "05":
                    periodicidad = c_PeriodicidadPago.Item05;
                    break;
                default:
                    periodicidad = c_PeriodicidadPago.Item99;
                    break;
            }

            return periodicidad;
        }

        /// <summary>
        /// Metodo que convierte clave federativa de String a Enum
        /// </summary>
        /// <param name="ClaveEntidad">Clave entidad federativa</param>
        /// <returns>Enum de clave entidad</returns>
        public c_Estado ObtenEntidadFederativa(string ClaveEntidad)
        {
            c_Estado entidad = c_Estado.DIF;

            switch (ClaveEntidad)
            {
                case "AGU":
                    entidad = c_Estado.AGU;
                    break;
                case "BCN":
                    entidad = c_Estado.BCN;
                    break;
                case "BCS":
                    entidad = c_Estado.BCS;
                    break;
                case "CAM":
                    entidad = c_Estado.CAM;
                    break;
                case "COA":
                    entidad = c_Estado.COA;
                    break;
                case "COL":
                    entidad = c_Estado.COL;
                    break;
                case "CHP":
                    entidad = c_Estado.CHP;
                    break;
                case "CHH":
                    entidad = c_Estado.CHH;
                    break;
                case "DIF":
                    entidad = c_Estado.DIF;
                    break;
                case "CMX":
                    entidad = c_Estado.CMX;
                    break;
                case "DUR":
                    entidad = c_Estado.DUR;
                    break;
                case "GUA":
                    entidad = c_Estado.GUA;
                    break;
                case "GRO":
                    entidad = c_Estado.GRO;
                    break;
                case "HID":
                    entidad = c_Estado.HID;
                    break;
                case "JAL":
                    entidad = c_Estado.JAL;
                    break;
                case "MEX":
                    entidad = c_Estado.MEX;
                    break;
                case "MIC":
                    entidad = c_Estado.MIC;
                    break;
                case "MOR":
                    entidad = c_Estado.MOR;
                    break;
                case "NAY":
                    entidad = c_Estado.NAY;
                    break;
                case "NLE":
                    entidad = c_Estado.NLE;
                    break;
                case "OAX":
                    entidad = c_Estado.OAX;
                    break;
                case "PUE":
                    entidad = c_Estado.PUE;
                    break;
                case "QUE":
                    entidad = c_Estado.QUE;
                    break;
                case "ROO":
                    entidad = c_Estado.ROO;
                    break;
                case "SLP":
                    entidad = c_Estado.SLP;
                    break;
                case "SIN":
                    entidad = c_Estado.SIN;
                    break;
                case "SON":
                    entidad = c_Estado.SON;
                    break;
                case "TAB":
                    entidad = c_Estado.TAB;
                    break;
                case "TAM":
                    entidad = c_Estado.TAM;
                    break;
                case "TLA":
                    entidad = c_Estado.TLA;
                    break;
                case "VER":
                    entidad = c_Estado.VER;
                    break;
                case "YUC":
                    entidad = c_Estado.YUC;
                    break;
                case "ZAC":
                    entidad = c_Estado.ZAC;
                    break;
                default:
                    entidad = c_Estado.CMX;
                    break;
            }

            return entidad;
        }

        /// <summary>
        /// Metodo que convierte la clave de tipo de percepción de String a Enum
        /// </summary>
        /// <param name="tipoPercepcion">Clave de tipo de percepción</param>
        /// <returns>Enum de clave de percepción</returns>
        public c_TipoPercepcion ObtenTipoPercepcion(string tipoPercepcion)
        {
            c_TipoPercepcion percepcion = c_TipoPercepcion.Item038;
            switch (tipoPercepcion)
            {
                case "001":
                    percepcion = c_TipoPercepcion.Item001;
                    break;
                case "002":
                    percepcion = c_TipoPercepcion.Item002;
                    break;
                case "003":
                    percepcion = c_TipoPercepcion.Item003;
                    break;
                case "004":
                    percepcion = c_TipoPercepcion.Item004;
                    break;
                case "005":
                    percepcion = c_TipoPercepcion.Item005;
                    break;
                case "006":
                    percepcion = c_TipoPercepcion.Item006;
                    break;
                case "009":
                    percepcion = c_TipoPercepcion.Item009;
                    break;
                case "010":
                    percepcion = c_TipoPercepcion.Item010;
                    break;
                case "011":
                    percepcion = c_TipoPercepcion.Item011;
                    break;
                case "012":
                    percepcion = c_TipoPercepcion.Item012;
                    break;
                case "013":
                    percepcion = c_TipoPercepcion.Item013;
                    break;
                case "014":
                    percepcion = c_TipoPercepcion.Item014;
                    break;
                case "015":
                    percepcion = c_TipoPercepcion.Item015;
                    break;
                case "019":
                    percepcion = c_TipoPercepcion.Item019;
                    break;
                case "020":
                    percepcion = c_TipoPercepcion.Item020;
                    break;
                case "021":
                    percepcion = c_TipoPercepcion.Item021;
                    break;
                case "022":
                    percepcion = c_TipoPercepcion.Item022;
                    break;
                case "023":
                    percepcion = c_TipoPercepcion.Item023;
                    break;
                case "024":
                    percepcion = c_TipoPercepcion.Item024;
                    break;
                case "025":
                    percepcion = c_TipoPercepcion.Item025;
                    break;
                case "026":
                    percepcion = c_TipoPercepcion.Item026;
                    break;
                case "027":
                    percepcion = c_TipoPercepcion.Item027;
                    break;
                case "028":
                    percepcion = c_TipoPercepcion.Item028;
                    break;
                case "029":
                    percepcion = c_TipoPercepcion.Item029;
                    break;
                case "030":
                    percepcion = c_TipoPercepcion.Item030;
                    break;
                case "031":
                    percepcion = c_TipoPercepcion.Item031;
                    break;
                case "032":
                    percepcion = c_TipoPercepcion.Item032;
                    break;
                case "033":
                    percepcion = c_TipoPercepcion.Item033;
                    break;
                case "034":
                    percepcion = c_TipoPercepcion.Item034;
                    break;
                case "035":
                    percepcion = c_TipoPercepcion.Item035;
                    break;
                case "036":
                    percepcion = c_TipoPercepcion.Item036;
                    break;
                case "037":
                    percepcion = c_TipoPercepcion.Item037;
                    break;
                case "038":
                    percepcion = c_TipoPercepcion.Item038;
                    break;
                case "039":
                    percepcion = c_TipoPercepcion.Item039;
                    break;
                case "044":
                    percepcion = c_TipoPercepcion.Item044;
                    break;
                case "045":
                    percepcion = c_TipoPercepcion.Item045;
                    break;
                case "046":
                    percepcion = c_TipoPercepcion.Item046;
                    break;
                case "047":
                    percepcion = c_TipoPercepcion.Item047;
                    break;
                case "048":
                    percepcion = c_TipoPercepcion.Item048;
                    break;
                case "049":
                    percepcion = c_TipoPercepcion.Item049;
                    break;
                case "050":
                    percepcion = c_TipoPercepcion.Item050;
                    break;
                default:
                    percepcion = c_TipoPercepcion.Item038;
                    break;
            }
            return percepcion;
        }

        /// <summary>
        /// Metodo que convierte la clave de tipo de horas extras de String a Enum
        /// </summary>
        /// <param name="tipoHoraExtra">Clave de tipo de horas extras</param>
        /// <returns>Enum de clave de horas extras</returns>
        public c_TipoHoras ObtenTipoHorasExtra(string tipoHoraExtra)
        {
            c_TipoHoras TipoHoras = c_TipoHoras.Item01;
            switch (tipoHoraExtra)
            {
                case "03":
                    TipoHoras = c_TipoHoras.Item03;
                    break;
                case "02":
                    TipoHoras = c_TipoHoras.Item02;
                    break;
                case "01":
                    TipoHoras = c_TipoHoras.Item01;
                    break;

            }

            return TipoHoras;
        }

        /// <summary>
        /// Metodo que convierte clave de tipo de deducción de String a Enum
        /// </summary>
        /// <param name="tipoDeduccion">Clave de tipo de deducción</param>
        /// <returns>Enum de tipo de decucción</returns>
        public c_TipoDeduccion ObtenTipoDeduccion(string tipoDeduccion)
        {
            c_TipoDeduccion deduccion = c_TipoDeduccion.Item004;
            switch (tipoDeduccion)
            {
                case "001":
                    deduccion = c_TipoDeduccion.Item001;
                    break;
                case "002":
                    deduccion = c_TipoDeduccion.Item002;
                    break;
                case "003":
                    deduccion = c_TipoDeduccion.Item003;
                    break;
                case "004":
                    deduccion = c_TipoDeduccion.Item004;
                    break;
                case "005":
                    deduccion = c_TipoDeduccion.Item005;
                    break;
                case "006":
                    deduccion = c_TipoDeduccion.Item006;
                    break;
                case "007":
                    deduccion = c_TipoDeduccion.Item007;
                    break;
                case "008":
                    deduccion = c_TipoDeduccion.Item008;
                    break;
                case "009":
                    deduccion = c_TipoDeduccion.Item009;
                    break;
                case "010":
                    deduccion = c_TipoDeduccion.Item010;
                    break;
                case "011":
                    deduccion = c_TipoDeduccion.Item011;
                    break;
                case "012":
                    deduccion = c_TipoDeduccion.Item012;
                    break;
                case "013":
                    deduccion = c_TipoDeduccion.Item013;
                    break;
                case "014":
                    deduccion = c_TipoDeduccion.Item014;
                    break;
                case "015":
                    deduccion = c_TipoDeduccion.Item015;
                    break;
                case "016":
                    deduccion = c_TipoDeduccion.Item016;
                    break;
                case "017":
                    deduccion = c_TipoDeduccion.Item017;
                    break;
                case "018":
                    deduccion = c_TipoDeduccion.Item018;
                    break;
                case "019":
                    deduccion = c_TipoDeduccion.Item019;
                    break;
                case "020":
                    deduccion = c_TipoDeduccion.Item020;
                    break;
                case "021":
                    deduccion = c_TipoDeduccion.Item021;
                    break;
                case "022":
                    deduccion = c_TipoDeduccion.Item022;
                    break;
                case "023":
                    deduccion = c_TipoDeduccion.Item023;
                    break;
                case "024":
                    deduccion = c_TipoDeduccion.Item024;
                    break;
                case "025":
                    deduccion = c_TipoDeduccion.Item025;
                    break;
                case "026":
                    deduccion = c_TipoDeduccion.Item026;
                    break;
                case "027":
                    deduccion = c_TipoDeduccion.Item027;
                    break;
                case "028":
                    deduccion = c_TipoDeduccion.Item028;
                    break;
                case "029":
                    deduccion = c_TipoDeduccion.Item029;
                    break;
                case "030":
                    deduccion = c_TipoDeduccion.Item030;
                    break;
                case "031":
                    deduccion = c_TipoDeduccion.Item031;
                    break;
                case "032":
                    deduccion = c_TipoDeduccion.Item032;
                    break;
                case "033":
                    deduccion = c_TipoDeduccion.Item033;
                    break;
                case "034":
                    deduccion = c_TipoDeduccion.Item034;
                    break;
                case "035":
                    deduccion = c_TipoDeduccion.Item035;
                    break;
                case "036":
                    deduccion = c_TipoDeduccion.Item036;
                    break;
                case "037":
                    deduccion = c_TipoDeduccion.Item037;
                    break;
                case "038":
                    deduccion = c_TipoDeduccion.Item038;
                    break;
                case "039":
                    deduccion = c_TipoDeduccion.Item039;
                    break;
                case "040":
                    deduccion = c_TipoDeduccion.Item040;
                    break;
                case "041":
                    deduccion = c_TipoDeduccion.Item041;
                    break;
                case "042":
                    deduccion = c_TipoDeduccion.Item042;
                    break;
                case "043":
                    deduccion = c_TipoDeduccion.Item043;
                    break;
                case "044":
                    deduccion = c_TipoDeduccion.Item044;
                    break;
                case "045":
                    deduccion = c_TipoDeduccion.Item045;
                    break;
                case "046":
                    deduccion = c_TipoDeduccion.Item046;
                    break;
                case "047":
                    deduccion = c_TipoDeduccion.Item047;
                    break;
                case "048":
                    deduccion = c_TipoDeduccion.Item048;
                    break;
                case "049":
                    deduccion = c_TipoDeduccion.Item049;
                    break;
                case "050":
                    deduccion = c_TipoDeduccion.Item050;
                    break;
                case "051":
                    deduccion = c_TipoDeduccion.Item051;
                    break;
                case "052":
                    deduccion = c_TipoDeduccion.Item052;
                    break;
                case "053":
                    deduccion = c_TipoDeduccion.Item053;
                    break;
                case "054":
                    deduccion = c_TipoDeduccion.Item054;
                    break;
                case "055":
                    deduccion = c_TipoDeduccion.Item055;
                    break;
                case "056":
                    deduccion = c_TipoDeduccion.Item056;
                    break;
                case "057":
                    deduccion = c_TipoDeduccion.Item057;
                    break;
                case "058":
                    deduccion = c_TipoDeduccion.Item058;
                    break;
                case "059":
                    deduccion = c_TipoDeduccion.Item059;
                    break;
                case "060":
                    deduccion = c_TipoDeduccion.Item060;
                    break;
                case "061":
                    deduccion = c_TipoDeduccion.Item061;
                    break;
                case "062":
                    deduccion = c_TipoDeduccion.Item062;
                    break;
                case "063":
                    deduccion = c_TipoDeduccion.Item063;
                    break;
                case "064":
                    deduccion = c_TipoDeduccion.Item064;
                    break;
                case "065":
                    deduccion = c_TipoDeduccion.Item065;
                    break;
                case "066":
                    deduccion = c_TipoDeduccion.Item066;
                    break;
                case "067":
                    deduccion = c_TipoDeduccion.Item067;
                    break;
                case "068":
                    deduccion = c_TipoDeduccion.Item068;
                    break;
                case "069":
                    deduccion = c_TipoDeduccion.Item069;
                    break;
                case "070":
                    deduccion = c_TipoDeduccion.Item070;
                    break;
                case "071":
                    deduccion = c_TipoDeduccion.Item071;
                    break;
                case "072":
                    deduccion = c_TipoDeduccion.Item072;
                    break;
                case "073":
                    deduccion = c_TipoDeduccion.Item073;
                    break;
                case "074":
                    deduccion = c_TipoDeduccion.Item074;
                    break;
                case "075":
                    deduccion = c_TipoDeduccion.Item075;
                    break;
                case "076":
                    deduccion = c_TipoDeduccion.Item076;
                    break;
                case "077":
                    deduccion = c_TipoDeduccion.Item077;
                    break;
                case "078":
                    deduccion = c_TipoDeduccion.Item078;
                    break;
                case "079":
                    deduccion = c_TipoDeduccion.Item079;
                    break;
                case "080":
                    deduccion = c_TipoDeduccion.Item080;
                    break;
                case "081":
                    deduccion = c_TipoDeduccion.Item081;
                    break;
                case "082":
                    deduccion = c_TipoDeduccion.Item082;
                    break;
                case "083":
                    deduccion = c_TipoDeduccion.Item083;
                    break;
                case "084":
                    deduccion = c_TipoDeduccion.Item084;
                    break;
                case "085":
                    deduccion = c_TipoDeduccion.Item085;
                    break;
                case "086":
                    deduccion = c_TipoDeduccion.Item086;
                    break;
                case "087":
                    deduccion = c_TipoDeduccion.Item087;
                    break;
                case "088":
                    deduccion = c_TipoDeduccion.Item088;
                    break;
                case "089":
                    deduccion = c_TipoDeduccion.Item089;
                    break;
                case "090":
                    deduccion = c_TipoDeduccion.Item090;
                    break;
                case "091":
                    deduccion = c_TipoDeduccion.Item091;
                    break;
                case "092":
                    deduccion = c_TipoDeduccion.Item092;
                    break;
                case "093":
                    deduccion = c_TipoDeduccion.Item093;
                    break;
                case "094":
                    deduccion = c_TipoDeduccion.Item094;
                    break;
                case "095":
                    deduccion = c_TipoDeduccion.Item095;
                    break;
                case "096":
                    deduccion = c_TipoDeduccion.Item096;
                    break;
                case "097":
                    deduccion = c_TipoDeduccion.Item097;
                    break;
                case "098":
                    deduccion = c_TipoDeduccion.Item098;
                    break;
                case "099":
                    deduccion = c_TipoDeduccion.Item099;
                    break;
                case "100":
                    deduccion = c_TipoDeduccion.Item100;
                    break;
                default:
                    deduccion = c_TipoDeduccion.Item004;
                    break;
            }

            return deduccion;
        }

        /// <summary>
        /// Metodo que convierte la clave de tipo (otros) de pagos de String a Enum
        /// </summary>
        /// <param name="TipoOtroPago">Calve de tipos de otros pagos</param>
        /// <returns>Enum declave de tipo de pago</returns>
        public c_TipoOtroPago ObtenTipoOtrosPagos(string TipoOtroPago)
        {
            c_TipoOtroPago otroPago = c_TipoOtroPago.Item999;
            switch (TipoOtroPago)
            {
                case "001":
                    otroPago = c_TipoOtroPago.Item001;
                    break;
                case "002":
                    otroPago = c_TipoOtroPago.Item002;
                    break;
                case "003":
                    otroPago = c_TipoOtroPago.Item003;
                    break;
                case "004":
                    otroPago = c_TipoOtroPago.Item004;
                    break;
                case "005":
                    otroPago = c_TipoOtroPago.Item005;
                    break;
                case "006":
                    otroPago = c_TipoOtroPago.Item006;
                    break;
                case "007":
                    otroPago = c_TipoOtroPago.Item007;
                    break;
                case "008":
                    otroPago = c_TipoOtroPago.Item008;
                    break;
                case "009":
                    otroPago = c_TipoOtroPago.Item009;
                    break;
                default:
                    otroPago = c_TipoOtroPago.Item999;
                    break;
            }

            return otroPago;
        }
    }
}
