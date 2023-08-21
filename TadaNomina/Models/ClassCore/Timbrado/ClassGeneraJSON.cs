using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.Timbrado.ModelosJSON;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ClassGeneraJSON : ClassGeneraJSONCancelacion
    {
        /// <summary>
        /// Metodo para regresar la informacion de nomina en formato JSON
        /// </summary>
        /// <param name="i">Informacion de XML</param>
        /// <returns>Información de XML en formato JSON</returns>
        /// <exception cref="Exception">Regresa el tipo de error en caso de haber</exception>
        public string GetJSON(sp_InformacionXML_Nomina1_Result i)
        {
            string json = string.Empty;
            decimal Subtotal = (decimal)i.ER;
            decimal TotalPercepciones = (decimal)i.TotalPercepciones;
            decimal Descuento = (decimal)i.TotalDeducciones;
            decimal ValorUnitario = (decimal)i.ER;
            string FecharRelLab = i.FechaAltaDelva.Value.ToShortDateString();
            decimal DiasPagados = (decimal)(i.DiasTrabajados + i.Dias_Vacaciones);

            ModeloComprobanteJSON_SCB compJ = new ModeloComprobanteJSON_SCB();

            compJ.Emisor = new ModeloEmisor
            {
                Rfc = i.RFC_P.ToUpper(),
                Nombre = i.NombrePatrona.ToUpper(),
                RegimenFiscal = "601"
            };

            compJ.Receptor = new ModeloReceptor
            {
                Rfc = i.Rfc.ToUpper(),
                Nombre = i.Nombre.ToUpper(),
                UsoCFDI = "P01"
            };

            compJ.Conceptos = new ModeloConceptos[1];

            ModeloConceptos conceptos = new ModeloConceptos();
            conceptos.ClaveProdServ = "84111505";
            conceptos.Cantidad = "1";
            conceptos.ClaveUnidad = "ACT";
            conceptos.Descripcion = "Pago de nómina";
            conceptos.ValorUnitario = ValorUnitario.ToString();
            conceptos.Importe = ValorUnitario.ToString("F");
            conceptos.Descuento = Descuento.ToString("F");
            conceptos.DescuentoSpecified = true;

            compJ.Conceptos[0] = conceptos;

            ModeloImpuestos modeloImpuestos = new ModeloImpuestos();

            ModeloComplementoSCB Complemento = new ModeloComplementoSCB();

            Complemento.Items = new ModeloItemsSCB[1];
            ModeloItemsSCB item = new ModeloItemsSCB();
            item.ObjectType = "Nomina";
            item.Version = "1.2";
            
            if (i.TipoCalculo == "Aguinaldo")
            {
                item.TipoNomina = "E";
                item.FechaPago = Statics.obtenFechaXML(DateTime.Parse("19/" + i.FechaFin.Value.Month + "/" + i.FechaFin.Value.Year).ToString());
            }
            else
            {
                item.TipoNomina = "O";
                item.FechaPago = Statics.obtenFechaXML(i.FechaFin.Value.ToShortDateString());
            }

            item.FechaInicialPago = Statics.obtenFechaXML(i.FechaInicio.Value.ToShortDateString());
            item.FechaFinalPago = Statics.obtenFechaXML(i.FechaFin.Value.ToShortDateString());
            
            if((decimal)i.DiasTrabajados == 0)
                item.NumDiasPagados = 0.001M;
            else
                item.NumDiasPagados = (decimal)i.DiasTrabajados;

            if (TotalPercepciones > 0)
            {
                item.TotalPercepciones = TotalPercepciones;
                item.TotalPercepcionesSpecified = true;
            }
            else
            {
                item.TotalPercepcionesSpecified = false;
            }

            if (Descuento != 0)
            {
                item.TotalDeducciones = Descuento;
                item.TotalDeduccionesSpecified = true;
            }
            else
            {
                item.TotalDeduccionesSpecified = false;
            }

            decimal SubsidioPagado1 = 0;
            if (i.SubsidioPagar > 0)
            {
                SubsidioPagado1 = (decimal)i.SubsidioPagar;
                if (SubsidioPagado1 == 0)
                    SubsidioPagado1 = 0.01M;
            }

            decimal reintegro = 0;
            try { reintegro = i.ReintegroISR; } catch { reintegro = 0; }

            item.TotalOtrosPagos = SubsidioPagado1 + reintegro;
            item.TotalOtrosPagosSpecified = true;
            ModeloNominaEmisor nominaEmisor = new ModeloNominaEmisor();
            nominaEmisor.RegistroPatronal = i.RegistroPatronal;
            item.Emisor = nominaEmisor;
            ModeloNominaReceptorSBC nreceptor = new ModeloNominaReceptorSBC();
            nreceptor.Curp = i.Curp.ToUpper();
            nreceptor.NumSeguridadSocial = i.Imss;
            nreceptor.FechaInicioRelLaboral = FecharRelLab;
            nreceptor.FechaInicioRelLaboralSpecified = true;
            nreceptor.Antiguedad = i.Antiguedad;
            nreceptor.TipoContrato = GetTipoContrato(i.TipoContrato);
            nreceptor.TipoJornada = "01";
            nreceptor.TipoRegimen = "02";
            nreceptor.NumEmpleado = Statics.DarFormatoClaveEmpleado(i.ClaveEmpleado);
            nreceptor.Puesto = i.Puesto;
            nreceptor.RiesgoPuesto = i.Clase.ToString();
            if (i.TipoCalculo == "Aguinaldo")
            {
                nreceptor.PeriodicidadPago = "99";
            }
            else
            {
                nreceptor.PeriodicidadPago = i.TipoNomina;
            }
            
            nreceptor.Banco = null;
            nreceptor.SalarioBaseCotApor = (decimal)i.SueldoDiario;
            nreceptor.SalarioBaseCotAporSpecified = true;
            nreceptor.SalarioDiarioIntegrado = (decimal)i.SDI;
            nreceptor.SalarioDiarioIntegradoSpecified = true;
            nreceptor.ClaveEntFed = i.ClaveEntidad;
            if (i.RFCSubContratacion != null && i.RFCSubContratacion != string.Empty)
            {
                ModeloNominaSubcontratacion modeloNominaSubcontratacion = new ModeloNominaSubcontratacion();
                modeloNominaSubcontratacion.RfcLabora = i.RFCSubContratacion;
                modeloNominaSubcontratacion.PorcentajeTiempo = 100;
                nreceptor.SubContratacion = new ModeloNominaSubcontratacion[1];
                nreceptor.SubContratacion[0] = modeloNominaSubcontratacion;
            }
            else
            {
                nreceptor.SubContratacion = null;
            }
            item.Receptor = nreceptor;
            ModeloNominaPercepciones npercepciones = new ModeloNominaPercepciones();
            npercepciones.TotalSueldos = TotalPercepciones;
            npercepciones.TotalSueldosSpecified = true;
            npercepciones.TotalSeparacionIndemnizacion = null;
            npercepciones.TotalSeparacionIndemnizacionSpecified = false;
            npercepciones.TotalJubilacionPensionRetiro = null;
            npercepciones.TotalJubilacionPensionRetiroSpecified = false;
            npercepciones.TotalGravado = (decimal)i.IMPORTE_GRAVADO;
            npercepciones.TotalExento = i.IMPORTE_EXCENTO;

            try
            {
                if (TotalPercepciones > 0)
                {
                    npercepciones.Percepcion = ObtenPercepciones(i.IdPeriodoNomina, i.IdEmpleado, i.SueldoPagado, i.Sueldo_Vacaciones);
                    item.Percepciones = npercepciones;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            if (Descuento != 0)
            {
                ModeloNominaDeducciones ndeducciones = new ModeloNominaDeducciones();
                ndeducciones.TotalOtrasDeducciones = (decimal)i.OtrasDeducciones;
                ndeducciones.TotalOtrasDeduccionesSpecified = true;
                if (i.ImpuestoRetener > 0)
                {
                    ndeducciones.TotalImpuestosRetenidos = (decimal)i.ImpuestoRetener;
                    ndeducciones.TotalImpuestosRetenidosSpecified = true;
                }
                else
                {
                    ndeducciones.TotalImpuestosRetenidosSpecified = false;
                }

                try { ndeducciones.Deduccion = ObtenDeducciones(i.IdPeriodoNomina, i.IdEmpleado, (decimal)i.IMSS_Obrero, (decimal)i.ImpuestoRetener); } catch (Exception ex) { throw new Exception(ex.Message); }
                item.Deducciones = ndeducciones;
            }

            int indiceOtros = 0;
            decimal reintegroISR = 0;
            decimal subsidio = 0;
            try { reintegroISR = i.ReintegroISR; } catch { reintegroISR = 0; }
            try { subsidio = (decimal)i.Subsidio; } catch { subsidio = 0; }
            int cantIndices = 1;
            if (reintegroISR > 0) { cantIndices++; }            

            if (cantIndices > 0)
            {
                item.OtrosPagos = new ModeloNominaOtrosPagos[cantIndices];
            }
            else
            {
                item.OtrosPagos = null;
            }            
            
            ModeloNominaOtrosPagos nOtrosPagos = new ModeloNominaOtrosPagos();
            nOtrosPagos.TipoOtroPago = "002";
            nOtrosPagos.Clave = "002";
            nOtrosPagos.Concepto = "Subsidio para el empleo(efectivamente entregado al trabajador)";

            decimal SubsidioPagado = (decimal)i.SubsidioPagar;
            //if (SubsidioPagado == 0)
            //    SubsidioPagado = 0.01M;

            nOtrosPagos.Importe = SubsidioPagado;
            nOtrosPagos.SubsidioAlEmpleo = new ModeloNominaOtrosPagosSubsidioEmpleo
            {
                SubsidioCausado = (decimal)i.Subsidio
            };
            item.OtrosPagos[indiceOtros] = nOtrosPagos;

            if (reintegroISR > 0)
            {
                ModeloNominaOtrosPagos nOtrosPagos2 = new ModeloNominaOtrosPagos();
                nOtrosPagos2.TipoOtroPago = "001";
                nOtrosPagos2.Clave = "001";
                nOtrosPagos2.Concepto = "Reintegro de ISR pagado en exceso (siempre que no haya sido enterado al SAT).";

                nOtrosPagos.Importe = reintegroISR;
                item.OtrosPagos[indiceOtros] = nOtrosPagos2;
                indiceOtros++;
            }

            item.Incapacidades = null;

            Complemento.Items[0] = item;
            compJ.Complemento = Complemento;
            compJ.Addenda = null;
            compJ.Version = "3.3";
            compJ.Serie = "ATF";
            compJ.Folio = "101";
            compJ.Fecha = Statics.obtenFechaXML(DateTime.Now.ToShortDateString()) + "T" + DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2") + ":" + DateTime.Now.Second.ToString("D2"); ;
            compJ.Sello = string.Empty;
            compJ.FormaPago = "99";
            compJ.NoCertificado = i.SelloDigital;
            compJ.Certificado = string.Empty;
            compJ.CondicionesDePago = null;
            compJ.SubTotal = Subtotal.ToString("F");
            compJ.Descuento = Descuento.ToString("F");
            compJ.DescuentoSpecified = true;
            compJ.Moneda = "MXN";
            compJ.TipoCambio = null;
            compJ.TipoCambioSpecified = false;
            compJ.Total = ((Subtotal - Descuento)).ToString("F");
            compJ.TipoDeComprobante = "N";
            compJ.MetodoPago = "PUE";
            compJ.LugarExpedicion = i.CP_E;
            compJ.Confirmacion = null;

            json = JsonConvert.SerializeObject(compJ);

            return json;
        }

        /// <summary>
        /// Metodo que regresa el tipo la clave de tipo de contrato
        /// </summary>
        /// <param name="tipoContrato">Tipo de contrato</param>
        /// <returns>Clave de tipo de contrato</returns>
        private string GetTipoContrato(string tipoContrato)
        {
            string TipoContrato = string.Empty;

            if (tipoContrato.ToUpper().Equals("INDETERMINADO"))
                TipoContrato = "01";

            if (tipoContrato.ToUpper().Equals("DETERMINADO"))
                TipoContrato = "02";

            return TipoContrato;
        }

        /// <summary>
        /// Metodo para obtener percepciones de un empleado en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="IdEmpleado">Empleado</param>
        /// <param name="Sueldo">Sueldo</param>
        /// <param name="Vacaciones">Vaciones</param>
        /// <returns></returns>
        public ModeloNominaPercepcionesPercepcion[] ObtenPercepciones(int IdPeriodo, int IdEmpleado, decimal? Sueldo, decimal? Vacaciones)
        {
            NominaEntities1 entidad = new NominaEntities1();
            ModeloNominaPercepcionesPercepcion psueldo = null;
            ModeloNominaPercepcionesPercepcion pVacaciones = null;
            int cantidad = 0;
            int _cantidad = 0;

            decimal _sueldo = (decimal)Sueldo;
            decimal _vacaciones = (decimal)Vacaciones;

            if (_sueldo > 0)
            {
                psueldo = new ModeloNominaPercepcionesPercepcion();
                psueldo.TipoPercepcion = "001";
                psueldo.Clave = "001";
                psueldo.Concepto = "SUELDO";
                psueldo.ImporteGravado = _sueldo;
                psueldo.ImporteExento = 0;

                cantidad++;
            }

            if (_vacaciones > 0)
            {
                pVacaciones = new ModeloNominaPercepcionesPercepcion();
                pVacaciones.TipoPercepcion = "001";
                pVacaciones.Clave = "001";
                pVacaciones.Concepto = "VACACIONES";
                pVacaciones.ImporteGravado = _vacaciones;
                pVacaciones.ImporteExento = 0;

                cantidad++;
            }

            string[] gpsExcluidos = { "001", "002" };
            string[] esquema = { "Tradicional", "Mixto" };
            var cantidadIncidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && esquema.Contains( x.TipoEsquema) && x.TipoConcepto == "ER" && x.Monto > 0 && !gpsExcluidos.Contains(x.ClaveGpo)) select b);

            int _cantidadIncidencias = cantidadIncidencias.Count();
            List<ModeloNominaPercepcionesPercepcion> listaConceptos = new List<ModeloNominaPercepcionesPercepcion>();
            if (_cantidadIncidencias > 0)
            {
                _cantidad = cantidad;
                foreach (var item in cantidadIncidencias)
                {
                    if (item.ClaveSAT.Length == 3)
                    {   
                        ModeloNominaPercepcionesPercepcion pVariable = new ModeloNominaPercepcionesPercepcion();
                        pVariable.TipoPercepcion = item.ClaveSAT;
                        pVariable.Clave = item.ClaveConcepto;
                        pVariable.Concepto = item.Concepto;
                        pVariable.ImporteExento = (decimal)item.Exento;
                        pVariable.ImporteGravado = (decimal)item.Gravado;
                        if (item.ClaveSAT.Equals("019"))
                            pVariable.HorasExtra = ObtenHorasExtra(item);

                        listaConceptos.Add(pVariable);
                        cantidad++;
                    }
                }
            }

            ModeloNominaPercepcionesPercepcion[] mnpp = new ModeloNominaPercepcionesPercepcion[cantidad];
            if (_sueldo > 0) { mnpp[0] = psueldo; }
            try { if (_vacaciones > 0) { mnpp[1] = pVacaciones; } } catch { mnpp[0] = pVacaciones; }
            if (_sueldo == 0 && _vacaciones != 0) { mnpp[0] = pVacaciones; }
            
            foreach (var item in listaConceptos)
            {
                mnpp[_cantidad] = item;
                _cantidad++;
            }

            return mnpp;
        }

        /// <summary>
        /// Metodo para obtener deducciones de un empleado en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <param name="IdEmpleado">Empleado</param>
        /// <param name="IMSS">imss</param>
        /// <param name="ISR">isr</param>
        /// <returns>Deducciones</returns>
        public ModeloNominaDeduccionesDeduccion[] ObtenDeducciones(int IdPeriodo, int IdEmpleado, decimal IMSS, decimal ISR)
        {

            NominaEntities1 entidad = new NominaEntities1();
            ModeloNominaDeduccionesDeduccion dImss = null;
            ModeloNominaDeduccionesDeduccion dISR = null;
            int cantidad = 0;
            int _cantidad = 0;

            decimal _imss = IMSS;
            decimal _isr = ISR;

            if (_imss > 0)
            {
                dImss = new ModeloNominaDeduccionesDeduccion();
                dImss.TipoDeduccion = "001";
                dImss.Clave = "022";
                dImss.Concepto = "SEGURIDAD SOCIAL";
                dImss.Importe = _imss;

                cantidad++;
            }

            if (_isr > 0)
            {
                dISR = new ModeloNominaDeduccionesDeduccion();
                dISR.TipoDeduccion = "002";
                dISR.Clave = "021";
                dISR.Concepto = "ISR";
                dISR.Importe = _isr;

                cantidad++;
            }

            string[] esquema = { "Tradicional", "Mixto" };
            var cantidadIncidencias = (from b in entidad.vIncidencias_Consolidadas.Where(x => x.IdEstatus == 1 && x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo && esquema.Contains(x.TipoEsquema) && x.TipoConcepto == "DD" && x.Monto > 0) select b);
            int _cantidadIncidencias = cantidadIncidencias.Count();
            List<ModeloNominaDeduccionesDeduccion> listaConceptos = new List<ModeloNominaDeduccionesDeduccion>();
            if (_cantidadIncidencias > 0)
            {
                _cantidad = cantidad;
                foreach (var item in cantidadIncidencias)
                {                    

                    if (item.ClaveSAT.Length == 3)
                    {
                        ModeloNominaDeduccionesDeduccion dVariable = new ModeloNominaDeduccionesDeduccion();
                        dVariable.TipoDeduccion = item.ClaveSAT;
                        dVariable.Clave = item.ClaveConcepto;
                        dVariable.Concepto = item.Concepto;
                        dVariable.Importe = (decimal)item.Monto;

                        listaConceptos.Add(dVariable);
                        cantidad++;
                    }
                }
            }

            ModeloNominaDeduccionesDeduccion[] mndd = new ModeloNominaDeduccionesDeduccion[cantidad];
            if (_imss > 0) { mndd[0] = dImss; }
            try { if (_isr > 0) { mndd[1] = dISR; } } catch { mndd[0] = dISR; }
            if (_imss == 0 && _isr != 0) { mndd[0] = dISR; }
            foreach (var item in listaConceptos)
            {
                mndd[_cantidad] = item;
                _cantidad++;
            }

            return mndd;

        }

        /// <summary>
        /// Metodo para obtener horas extras
        /// </summary>
        /// <param name="item">Información (Dias, horas extras, tipo horas, importe)</param>
        /// <returns>Horas extra</returns>
        private ModeloHorasExtra[] ObtenHorasExtra(vIncidencias_Consolidadas item)
        {
            ModeloHorasExtra[] _horasExtra = new ModeloHorasExtra[1];
            ModeloHorasExtra horasExtra = new ModeloHorasExtra();

            horasExtra.Dias = "1";
            horasExtra.TipoHoras = item.ClaveGpo.Substring(1);
            horasExtra.HorasExtra = (int)item.Cantidad;
            horasExtra.ImportePagado = item.Monto.ToString();
            _horasExtra[0] = horasExtra;
            return _horasExtra;
        }

        /// <summary>
        /// Metodo para agregar una leyenda al modelo TextoCFDI
        /// </summary>
        /// <param name="Leyenda">Leyenda</param>
        /// <returns>Modelo con el texto</returns>
        public ModeloTextoCFDI ObtenLeyendaCFDI(string Leyenda)
        {
            ModeloTextoCFDI texto = new ModeloTextoCFDI();
            texto.v_titulo = "Nota";
            texto.v_texto = Leyenda;

            return texto;
        }
    }
}