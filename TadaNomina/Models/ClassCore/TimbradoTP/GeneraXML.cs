using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using TadaNomina.Models.ClassCore.TimbradoTP;
using Core.Sat;
using Core.XmlUtils;
using CadenasXslt.Sat;
using System.Linq;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using System.IO;

namespace Delva.AppCode.TimbradoTurboPAC
{
    public class GeneraXML : ClassAux
    {
        public string GeneraXMLNomina12(DatosXML dat, int IdUnidadNegocio, string TipoFechaFiniquito, int IdPeriodo)
        {
            try
            {
                dat.TipoContrato = GetTipoContrato(dat.TipoContrato);
                int contador = 0;
                var incidencias = GetVInsidencias_Consolidadas(int.Parse(dat.IdPeriodo), int.Parse(dat.IdEmpleado));
                ObtenTotalesPercepciones(int.Parse(dat.IdPeriodo), int.Parse(dat.IdEmpleado), decimal.Parse(dat.total), decimal.Parse(dat.SubsidioPagar), decimal.Parse(dat.ReintegroISR), decimal.Parse(dat.Vacaciones), IdUnidadNegocio);
                ObtenTotalesDeducciones(int.Parse(dat.IdPeriodo), int.Parse(dat.IdEmpleado), decimal.Parse(dat.IMSS), decimal.Parse(dat.ISPT));

                decimal otrosPagos = 0; 
                try { otrosPagos = (decimal)incidencias.Where(x => x.ClaveSAT == "999").Select(x => x.Monto).Sum(); } catch { }

                decimal Subtotal = (TotalExcentoPercepciones + TotalGravadoPercepciones) + decimal.Parse(dat.SubsidioPagar) + decimal.Parse(dat.ReintegroISR) + otrosPagos;                
                var tp = decimal.Parse(dat.totalPercepciones);
                
                validaCFDI(Subtotal, tp);

                decimal TotalPercepciones = (TotalExcentoPercepciones + TotalGravadoPercepciones);
                decimal Descuento = (TotalOtrasDeducciones + TotalImpuestosRetenidos);
                decimal ValorUnitario = TotalPercepciones + decimal.Parse(dat.SubsidioPagar) + decimal.Parse(dat.ReintegroISR) + otrosPagos;

                DateTime? FEchaRelLab = null; try { FEchaRelLab = DateTime.Parse(dat.FechaInicioRelLaboral); } catch { FEchaRelLab = DateTime.Parse(dat.FechaReconocimientoAntiguedad); }
                if (dat._TipoNomina == "Finiquitos")
                    try { FEchaRelLab = DateTime.Parse(dat.FechaReconocimientoAntiguedad); } catch { new Exception("El empleado no tiene fecha de reconocimiento de antiguedad"); }

                decimal DIasPagados = 1.000M;

                decimal _diasPagados = decimal.Parse(dat.NumDiasPagados + "0");                

                if (Math.Round(_diasPagados + decimal.Parse(dat.DiasVacaciones), 3) > 0)
                    DIasPagados = Math.Round(_diasPagados + decimal.Parse(dat.DiasVacaciones), 3);

                if (dat._TipoNomina == "Finiquitos")
                    try { _diasPagados = decimal.Parse(dat.SueldosPagados + "0"); } catch { _diasPagados = 0; }

                //Conceptos de liquidacion de finiquitos.
                decimal TotalLiquidacion = 0;
                try { TotalLiquidacion = Math.Round(decimal.Parse(dat.Liquidacion_Gravado) + decimal.Parse(dat.Liquidacion_Exento), 2); } catch { TotalLiquidacion = 0; }
                decimal SMO = 0;
                string AntiguedadLiquidacion = string.Empty;
                decimal IngAcumulable = 0;
                decimal IngNoAcumulable = 0;
                if (TotalLiquidacion > 0)
                {
                    try { SMO = Math.Round(decimal.Parse(dat.SueldoMensual), 2); } catch { SMO = Math.Round(decimal.Parse(dat.SalarioBaseCotApor) * 30, 2); }
                    AntiguedadLiquidacion = ObtenAntiguedadAños(DateTime.Parse(dat.FechaReconocimientoAntiguedad), DateTime.Parse(dat.FechaBaja));
                    IngAcumulable = ObtenIngresoAcumulable(decimal.Parse(dat.Liquidacion_Gravado), SMO);
                    IngNoAcumulable = Math.Round(decimal.Parse(dat.Liquidacion_Gravado) - IngAcumulable, 2);
                }

                decimal TotalSueldos = Math.Round((TotalGravadoPercepciones + TotalExcentoPercepciones) - (TotalLiquidacion), 2);

                var comprobante = new Comprobante33();
                
                ComprobanteEmisor33 emisor = new ComprobanteEmisor33();
                emisor.Rfc = dat.Emisor_Rfc;
                emisor.Nombre = dat.Emisor_Nombre;
                if (dat.PersonaFisica == "1")
                {
                    emisor.RegimenFiscal = c_RegimenFiscal.Item612;

                }
                else
                {
                    emisor.RegimenFiscal = c_RegimenFiscal.Item601;
                }

                comprobante.Emisor = emisor;

                ComprobanteReceptor33 receptor = new ComprobanteReceptor33();
                receptor.Rfc = dat.Receptor_Rfc;
                receptor.UsoCFDI = c_UsoCFDI.P01;
                receptor.Nombre = dat.Receptor_Nombre;

                comprobante.Receptor = receptor;

                var Conceptos = new List<ComprobanteConcepto33>
                {
                    new ComprobanteConcepto33()
                    {
                        ClaveProdServ = "84111505",
                        Cantidad = 1,
                        ClaveUnidad = c_ClaveUnidad.ACT,
                        Descripcion = "Pago de nómina" ,
                        ValorUnitario = ValorUnitario,
                        Importe = ValorUnitario,
                        Descuento = Descuento,
                        DescuentoSpecified = true
                    }
                };

                comprobante.Conceptos = Conceptos.ToArray();

                var nomina = new Core.Sat.Nomina();
                var nominaNameSpaces = new XmlSerializerNamespaces();
                nominaNameSpaces.Add(nomina.GetXmlRootShortNamespace(), nomina.GetXmlRootNamespace());

                nomina.TipoNomina = c_TipoNomina.O;
                nomina.Version = "1.2";
                nomina.FechaPago = DateTime.Parse(dat.FechaDispersion);
                if (dat._TipoNomina == "Finiquitos")
                {
                    string TipoFecha = string.Empty;
                    try { TipoFecha = TipoFechaFiniquito; } catch {  }

                    if (TipoFecha == "S")
                        try { nomina.FechaInicialPago = DateTime.Parse(dat.FechaInicioRelLaboral); } catch { nomina.FechaInicialPago = DateTime.Parse(dat.FechaReconocimientoAntiguedad); }
                    else
                        try { nomina.FechaInicialPago = DateTime.Parse(dat.FechaReconocimientoAntiguedad); } catch { nomina.FechaInicialPago = DateTime.Parse(dat.FechaInicioRelLaboral); }
                }
                else
                    nomina.FechaInicialPago = DateTime.Parse(dat.FechaInicio);

                if (dat._TipoNomina == "Finiquitos" && dat.FechaBaja != "" && dat.FechaBaja != null)
                    nomina.FechaFinalPago = DateTime.Parse(dat.FechaBaja);
                else if(dat._TipoNomina == "Finiquitos"  && dat.FechaBajaEmpleado != null)
                    nomina.FechaFinalPago = DateTime.Parse(dat.FechaBajaEmpleado);
                else
                    nomina.FechaFinalPago = DateTime.Parse(dat.FechaFin);

                if (dat._TipoNomina == "Aguinaldo")
                {
                    nomina.FechaInicialPago = DateTime.Parse(dat.FechaDispersion);
                    nomina.FechaFinalPago = DateTime.Parse(dat.FechaDispersion);
                }

                nomina.NumDiasPagados = (decimal)DIasPagados;

                if (TotalPercepciones > 0)
                {
                    nomina.TotalPercepciones = TotalPercepciones;
                    nomina.TotalPercepcionesSpecified = true;
                }
                else
                {
                    nomina.TotalPercepcionesSpecified = false;
                }

                if (Descuento != 0)
                {
                    nomina.TotalDeducciones = Descuento;
                    nomina.TotalDeduccionesSpecified = true;
                }
                else
                {
                    nomina.TotalDeduccionesSpecified = false;
                }

                decimal reintegro = 0;
                try { reintegro = decimal.Parse(dat.ReintegroISR); } catch { }

                decimal SubsidioPagado1 = 0;
                try { SubsidioPagado1 = decimal.Parse(dat.SubsidioPagar); } catch { }

                if (dat.TipoContrato != "09")
                {
                    nomina.TotalOtrosPagos = SubsidioPagado1 + reintegro + otrosPagos;
                    nomina.TotalOtrosPagosSpecified = true;

                    nomina.Emisor = new NominaEmisor();
                    nomina.Emisor.RegistroPatronal = dat.RegistroPatronal;

                    if (dat.PersonaFisica == "1")
                        nomina.Emisor.Curp = dat.CurpPersonaFisica;
                }

                NominaReceptor nominaReceptor = new NominaReceptor();

                nominaReceptor.Curp = dat.CURP;
                if (dat.TipoContrato != "09")
                {
                    nominaReceptor.NumSeguridadSocial = dat.NumSeguridadSocial;
                    nominaReceptor.FechaInicioRelLaboral = (DateTime)FEchaRelLab;
                    nominaReceptor.FechaInicioRelLaboralSpecified = true;

                    if (dat._TipoNomina == "Finiquitos")
                    {
                        var ant = ((nomina.FechaFinalPago.Subtract(nominaReceptor.FechaInicioRelLaboral).Days) + 1);
                        string ant1 = string.Empty;
                        if (ant < 7)                        
                            ant1 = "P" + (ant -1) + "D";                        
                        else
                            ant1 = "P" + Math.Truncate((ant / 7M)) + "W";                        
                        
                        nominaReceptor.Antigüedad = ant1;
                    }                        
                    else
                        nominaReceptor.Antigüedad = dat.Antiguedad;

                    nominaReceptor.TipoJornada = c_TipoJornada.Item01;
                    nominaReceptor.TipoRegimen = c_TipoRegimen.Item02;
                }
                else
                {
                    nominaReceptor.TipoJornada = c_TipoJornada.Item04;
                    nominaReceptor.TipoRegimen = c_TipoRegimen.Item09;
                }

                nominaReceptor.TipoContrato = tipos.ObtenTipoContrato(dat.TipoContrato);
                nominaReceptor.TipoJornadaSpecified = true;
                nominaReceptor.NumEmpleado = DarFormatoClaveEmpleado(dat.NumEmpleado);
                nominaReceptor.Puesto = dat.Puesto.Trim() != "" ? dat.Puesto.Trim() : null;
                nominaReceptor.RiesgoPuesto = tipos.ObtenRiesgoPuesto(dat.Clase);
                nominaReceptor.RiesgoPuestoSpecified = true;
                nominaReceptor.PeriodicidadPago = tipos.ObtenPeriodicidadPago(dat.PeriodicidadPago);
                nominaReceptor.SalarioBaseCotApor = Math.Round(decimal.Parse(dat.SalarioBaseCotApor), 2);
                nominaReceptor.SalarioBaseCotAporSpecified = true;
                nominaReceptor.SalarioDiarioIntegrado = Math.Round(decimal.Parse(dat.SalarioDiarioIntegrado), 2);
                nominaReceptor.SalarioDiarioIntegradoSpecified = true;
                nominaReceptor.ClaveEntFed = tipos.ObtenEntidadFederativa(dat.ClaveEntFed);
                nominaReceptor.Departamento = dat.Departamento != null && dat.Departamento != string.Empty ? dat.Departamento : null;
                
                if (dat.RFCSubcontratacion != null && dat.RFCSubcontratacion != string.Empty)
                {
                    NominaReceptorSubContratacion subContratacion = new NominaReceptorSubContratacion();
                    subContratacion.RfcLabora = dat.RFCSubcontratacion;
                    subContratacion.PorcentajeTiempo = 100;
                    nominaReceptor.SubContratacion = new NominaReceptorSubContratacion[1];
                    nominaReceptor.SubContratacion[0] = subContratacion;
                }
                else
                {
                    nominaReceptor.SubContratacion = null;
                }

                nomina.Receptor = nominaReceptor;

                NominaPercepciones nominaPercepciones = new NominaPercepciones();
                nominaPercepciones.TotalSueldos = TotalSueldos;
                nominaPercepciones.TotalSueldosSpecified = true;
                if (TotalLiquidacion > 0)
                {
                    nominaPercepciones.TotalSeparacionIndemnizacion = TotalLiquidacion;
                    nominaPercepciones.TotalSeparacionIndemnizacionSpecified = true;
                }
                else
                {
                    nominaPercepciones.TotalSeparacionIndemnizacion = 0;
                    nominaPercepciones.TotalSeparacionIndemnizacionSpecified = false;
                }
                nominaPercepciones.TotalJubilacionPensionRetiro = 0;
                nominaPercepciones.TotalJubilacionPensionRetiroSpecified = false;
                nominaPercepciones.TotalGravado = TotalGravadoPercepciones;
                nominaPercepciones.TotalExento = TotalExcentoPercepciones;

                try
                {
                    if (TotalPercepciones > 0)
                    {
                        nominaPercepciones.Percepcion = ObtenPercepciones(IdPeriodo, int.Parse(dat.IdEmpleado), dat.total, dat.SubsidioPagar, dat.Vacaciones, IdUnidadNegocio, incidencias);
                        nomina.Percepciones = nominaPercepciones;
                    }

                    if (TotalLiquidacion > 0)
                    {
                        NominaPercepcionesSeparacionIndemnizacion modeloPercepcionesSeparacionIndemnizacion = new NominaPercepcionesSeparacionIndemnizacion();
                        modeloPercepcionesSeparacionIndemnizacion.TotalPagado = TotalLiquidacion;
                        modeloPercepcionesSeparacionIndemnizacion.NumAñosServicio = int.Parse(AntiguedadLiquidacion);
                        modeloPercepcionesSeparacionIndemnizacion.UltimoSueldoMensOrd = SMO;
                        modeloPercepcionesSeparacionIndemnizacion.IngresoAcumulable = IngAcumulable;
                        modeloPercepcionesSeparacionIndemnizacion.IngresoNoAcumulable = IngNoAcumulable;

                        nominaPercepciones.SeparacionIndemnizacion = modeloPercepcionesSeparacionIndemnizacion;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                var incapDeduc = incidencias.Where(x => x.TipoConcepto == "DD" && x.Monto > 0 && x.ClaveGpo == "501").Sum(x => x.Monto);

                if (Descuento != 0)
                {
                    NominaDeducciones ndeducciones = new NominaDeducciones();
                    ndeducciones.TotalOtrasDeducciones = TotalOtrasDeducciones;
                    ndeducciones.TotalOtrasDeduccionesSpecified = true;
                    if (TotalImpuestosRetenidos > 0)
                    {
                        ndeducciones.TotalImpuestosRetenidos = TotalImpuestosRetenidos;
                        ndeducciones.TotalImpuestosRetenidosSpecified = true;
                    }
                    else
                    {
                        ndeducciones.TotalImpuestosRetenidos = TotalImpuestosRetenidos;
                        ndeducciones.TotalImpuestosRetenidosSpecified = false;
                    }

                    if (incapDeduc > 0)
                    { 
                        //ndeducciones.TOtal
                    }

                    try { ndeducciones.Deduccion = ObtenDeducciones(IdPeriodo, int.Parse(dat.IdEmpleado), dat.IMSS, dat.ISPT, incidencias); } catch (Exception ex) { throw new Exception(ex.Message); }
                    nomina.Deducciones = ndeducciones;
                }
                                
                if (dat.TipoContrato != "09")
                {
                    int cantidadIncidenciasOtrosPagos = incidencias.Where(x => x.ClaveSAT == "999").Count();

                    int indiceOtros = 0;
                    int cantIndices = 1;
                    if (reintegro > 0) { cantIndices++; }
                    cantIndices += cantidadIncidenciasOtrosPagos;

                    nomina.OtrosPagos = new NominaOtroPago[cantIndices];

                    NominaOtroPago nOtrosPagos = new NominaOtroPago();
                    nOtrosPagos.TipoOtroPago = tipos.ObtenTipoOtrosPagos("002");
                    nOtrosPagos.Clave = "002";
                    nOtrosPagos.Concepto = "Subsidio para el empleo(efectivamente entregado al trabajador)";
                    nOtrosPagos.Importe = SubsidioPagado1;
                    nOtrosPagos.SubsidioAlEmpleo = new NominaOtroPagoSubsidioAlEmpleo
                    {
                        SubsidioCausado = decimal.Parse(dat.Subsidio)
                    };
                    nomina.OtrosPagos[indiceOtros] = nOtrosPagos;

                    if (reintegro > 0)
                    {
                        indiceOtros++;
                        NominaOtroPago nOtrosPagosR = new NominaOtroPago();
                        nOtrosPagosR.TipoOtroPago = tipos.ObtenTipoOtrosPagos("001");
                        nOtrosPagosR.Clave = "001";
                        nOtrosPagosR.Concepto = "Reintegro de ISR pagado en exceso (siempre que no haya sido enterado al SAT).";
                        nOtrosPagosR.Importe = reintegro;
                        nomina.OtrosPagos[indiceOtros] = nOtrosPagosR;
                    }

                    foreach (var oPagos in incidencias.Where(x => x.ClaveSAT == "999"))
                    {
                        indiceOtros++;
                        NominaOtroPago nOtrosPagos3 = new NominaOtroPago();                        
                        nOtrosPagos3.TipoOtroPago = oPagos.ClaveConcepto == "003" ? tipos.ObtenTipoOtrosPagos("003") : tipos.ObtenTipoOtrosPagos("999");
                        nOtrosPagos3.Clave = oPagos.ClaveConcepto;
                        nOtrosPagos3.Concepto = oPagos.Concepto;

                        nOtrosPagos3.Importe = (decimal)oPagos.Monto;
                        nomina.OtrosPagos[indiceOtros] = nOtrosPagos3;
                    }
                }

                nomina.Incapacidades = null;
                var subsInc = incidencias.Where(x => x.ClaveSAT == "014" && x.TipoConcepto == "ER").Sum(x => x.Monto);
                
                if (subsInc > 0 || incapDeduc > 0)
                {
                    //nodo de incapacidades
                    List<NominaIncapacidad> nIncapacidades = new List<NominaIncapacidad>();

                    //obtiene las incidencias de incapacidades
                    var Inc = GetVInsidencias_Consolidadas_Incapacidades(IdPeriodo, int.Parse(dat.IdEmpleado));

                    foreach (var iInc in Inc)
                    {
                        NominaIncapacidad nIncapacidad = new NominaIncapacidad();
                        nIncapacidad.DiasIncapacidad = (int)iInc.Cantidad;
                        nIncapacidad.ImporteMonetario = (decimal)subsInc + (decimal)incapDeduc;
                        nIncapacidad.ImporteMonetarioSpecified = true;

                        if (iInc.ClaveSAT == "01")
                            nIncapacidad.TipoIncapacidad = c_TipoIncapacidad.Item01;

                        if (iInc.ClaveSAT == "02")
                            nIncapacidad.TipoIncapacidad = c_TipoIncapacidad.Item02;

                        if (iInc.ClaveSAT == "03")
                            nIncapacidad.TipoIncapacidad = c_TipoIncapacidad.Item03;

                        nIncapacidades.Add(nIncapacidad);
                    }

                    nomina.Incapacidades = nIncapacidades.Count > 0 ? nIncapacidades.ToArray() : null;
                }

                comprobante.Addenda = null;
                comprobante.Version = "3.3";
                comprobante.Serie = "ATF";
                comprobante.Folio = "101";

                var localTimezone = TimeZoneInfo.Local;
                if (localTimezone.DaylightName.Equals("Hora de verano central (México)") && dat.codigoPostalEmisor.Equals("33920"))
                    comprobante.Fecha = DateTime.Now.AddHours(-1).AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm:ss");
                else if (dat.codigoPostalEmisor.Equals("22750"))
                    comprobante.Fecha = DateTime.Now.AddHours(-2).AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm:ss");
                else if (dat.codigoPostalEmisor.Equals("22226") || dat.codigoPostalEmisor.Equals("01460") || dat.codigoPostalEmisor.Equals("03570") || dat.codigoPostalEmisor.Equals("42185") || dat.codigoPostalEmisor.Equals("54000"))
                    comprobante.Fecha = DateTime.Now.AddHours(-1).AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:ss");                
                else                
                    comprobante.Fecha = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm:ss");
                

                try { ObtenCertificadosCfdi(dat.rutaCer, dat.rutaKey, dat.keyPass); } catch { throw new Exception("No se pudo obtener el certificado."); }
                comprobante.FormaPago = tipos.ObtenFormaPago("99");
                comprobante.FormaPagoSpecified = true;
                comprobante.Certificado = _firmaSoftware256.CertToBase64String();
                comprobante.NoCertificado = _noCert;
                comprobante.CondicionesDePago = null;
                comprobante.SubTotal = Subtotal;
                comprobante.Descuento = Descuento;
                comprobante.DescuentoSpecified = true;
                comprobante.Moneda = tipos.ObtenTipoMoneda("MXN");
                comprobante.TipoCambio = 0;
                comprobante.TipoCambioSpecified = false;
                comprobante.Total = Subtotal - Descuento;
                comprobante.TipoDeComprobante = tipos.ObtenTipoComprobante("N");
                comprobante.MetodoPago = tipos.ObtenMetodoPago("PUE");
                comprobante.MetodoPagoSpecified = true;
                comprobante.LugarExpedicion = dat.codigoPostalEmisor;
                comprobante.Confirmacion = null;

                const string schemaNomina = "http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";
                if (!comprobante.xsiSchemaLocation.Contains(schemaNomina))
                    comprobante.xsiSchemaLocation += " " + schemaNomina;
                comprobante.xmlns.AddRange(nominaNameSpaces);
                comprobante.Complemento = new ComprobanteComplemento33();

                var acumulado = new List<XmlElement>();

                acumulado.Add(nomina.ToXmlElement(nominaNameSpaces));
                comprobante.Complemento.Any = acumulado.ToArray();


                GeneradorCadenasCfd33 gen = new GeneradorCadenasCfd33();
                string cadena = gen.CadenaOriginal(comprobante.Serialize(comprobante.xmlns));
                string Sello = string.Empty;                
                try { Sello = _firmaSoftware256.Sellar(cadena); } catch (Exception ex) { Sello = string.Empty; throw new Exception("Error al sellar. " + ex.Message); }
                comprobante.Sello = Sello;

                contador++;

                File.WriteAllText(Path.Combine(@"D:\TadaNomina\cfdi\Entrada", dat.Receptor_Rfc + "_" + IdPeriodo + "_" + IdUnidadNegocio + ".xml"), comprobante.Serialize(comprobante.xmlns));

                return comprobante.Serialize(comprobante.xmlns);

                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void validaCFDI(decimal subtotal, decimal totalPercepciones)
        {
            var diferencia = subtotal - totalPercepciones;
            diferencia = Math.Abs(diferencia);

            if(diferencia > 1.0M)
                throw new Exception("Error: La suma de los conceptos no es igual al subtotal, no se puede crear el XML.");
        }
    }
}