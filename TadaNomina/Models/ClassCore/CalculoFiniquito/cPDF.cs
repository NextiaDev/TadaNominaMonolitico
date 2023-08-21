using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.CalculoFiniquito
{
    public class cPDF
    {
        /// <summary>
        ///     Método que genera el PDF para un periodo de nómina tradicional
        /// </summary>
        /// <param name="IdPeriodonomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <returns>Arreglo de bytes con el PDF generado</returns>
        public byte[] getPDFTrad(int IdPeriodonomina, int IdEmpleado)
        {
            string _Departamento = string.Empty;
            string _Puesto = string.Empty;
            string _CentroCostos = string.Empty;
            string _EmpresaPatrona = string.Empty;
            string _UnidadNegocio = string.Empty;

            ClassNomina cn = new ClassNomina();
            ClassIncidencias ci = new ClassIncidencias();
            ClassEmpleado ce = new ClassEmpleado();
            ClassProcesosFiniquitos cpf = new ClassProcesosFiniquitos();
            ClassUnidadesNegocio cuni = new ClassUnidadesNegocio();

            var datos = ce.GetvEmpleado(IdEmpleado);
            var NominaTrab = cn.GetNominaTrabajoFiniquitos(IdEmpleado, IdPeriodonomina);
            var incidencias = ci.GetvIncindencias(IdPeriodonomina, IdEmpleado);
            var confFiniquito = cpf.GetFiniquitoConfigurado(IdPeriodonomina, IdEmpleado);
            var unidad = cuni.getUnidadesnegocioId(datos.IdUnidadNegocio);
            var tipoNomina = cuni.getTipoNominaById(unidad.IdTipoNomina);

            if (datos != null)
            {
                _Departamento = datos.Departamento;
                _Puesto = datos.Puesto;
                _CentroCostos = datos.CentroCostos;
                _EmpresaPatrona = datos.NombrePatrona;
                _UnidadNegocio = datos.UnidadNegocio;
            }

            ///Crea el documento PDF
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            #region fuentes
            ///////////////////////////////////////////////// Definimos los tipos de fuente////////////////////////////////////////////////////////
            BaseFont bfDetallePago = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontDetallePago = new Font(bfDetallePago, 11);

            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 10);

            // Para el cuerpo del recibo
            // Para el encabezado
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesCuerpo, 8);

            BaseFont bfTimesCuerpo10 = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo10 = new Font(bfTimesCuerpo10, 10);

            BaseFont bfTimesCuerpoNegrita = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerponegrita = new Font(bfTimesCuerpoNegrita, 8, Font.BOLD);

            // Para la Leyenda Final
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontLeyenda = new Font(bfTimes, 7);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #endregion fuentes

            ///Para los espacios///
            Paragraph espacio = new Paragraph(" ");

            ///Se abre el documento///
            doc.Open();

            PdfPTable table = new PdfPTable(100);
            table.TotalWidth = 500f;
            table.LockedWidth = true;

            //////////////////////////////////////////////////////////////////////////////////////
            string _TipoCalculo = confFiniquito.BanderaLiquidacion == 1 && unidad.MostrarLiquidacionFormato == "S" ? "LIQUIDACIÓN" : "FINIQUITO";

            PdfPCell detallePago = new PdfPCell(new Phrase(_TipoCalculo, fontDetallePago));
            detallePago.Colspan = 100;
            detallePago.HorizontalAlignment = Element.ALIGN_CENTER;
            detallePago.BorderWidthBottom = 0;
            detallePago.BorderWidthLeft = 0;
            detallePago.BorderWidthTop = 0;
            detallePago.BorderWidthRight = 0;

            table.AddCell(detallePago);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell vacia = new PdfPCell(new Phrase(" "));
            vacia.Colspan = 100;
            vacia.BorderWidthBottom = 0;
            vacia.BorderWidthLeft = 0;
            vacia.BorderWidthTop = 0;
            vacia.BorderWidthRight = 0;

            table.AddCell(vacia);
            table.AddCell(vacia);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell espacio1 = new PdfPCell(new Phrase(""));

            espacio1.Colspan = 4;
            espacio1.BorderWidthBottom = 0;
            espacio1.BorderWidthLeft = 0;
            espacio1.BorderWidthTop = 0;
            espacio1.BorderWidthRight = 0;

            //////////////////////////////////////////////////////////////////////////////////

            PdfPCell NoEmText = new PdfPCell(new Phrase("Num Empleado", fontCuerponegrita));
            NoEmText.BackgroundColor = BaseColor.LIGHT_GRAY;
            NoEmText.Colspan = 19;
            table.AddCell(NoEmText);

            PdfPCell NoEmp = new PdfPCell(new Phrase(datos.ClaveEmpleado, fontCuerpo));
            NoEmp.HorizontalAlignment = Element.ALIGN_RIGHT;
            NoEmp.Colspan = 29;
            table.AddCell(NoEmp);

            table.AddCell(espacio1);

            PdfPCell SMIMSSText = new PdfPCell(new Phrase("Sueldo Mensual:", fontCuerponegrita));
            SMIMSSText.BackgroundColor = BaseColor.LIGHT_GRAY;
            SMIMSSText.Colspan = 19;
            table.AddCell(SMIMSSText);

            decimal sueldoMensual = 0;
            sueldoMensual = getSueldo(NominaTrab, tipoNomina, sueldoMensual);

            PdfPCell SMIMSS = new PdfPCell(new Phrase(sueldoMensual.ToString("C2"), fontCuerpo));
            SMIMSS.HorizontalAlignment = Element.ALIGN_RIGHT;
            SMIMSS.Colspan = 29;
            table.AddCell(SMIMSS);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell NombreText = new PdfPCell(new Phrase("Nombre:", fontCuerponegrita));
            NombreText.BackgroundColor = BaseColor.LIGHT_GRAY;
            NombreText.Colspan = 19;
            table.AddCell(NombreText);

            PdfPCell Nombre = new PdfPCell(new Phrase(datos.ApellidoPaterno + " " + datos.ApellidoMaterno + " " + datos.Nombre, fontCuerpo));
            Nombre.HorizontalAlignment = Element.ALIGN_RIGHT;
            Nombre.Colspan = 29;
            table.AddCell(Nombre);

            table.AddCell(espacio1);

            PdfPCell FechaRecAntiguedadText = new PdfPCell(new Phrase("Fecha Ingreso:", fontCuerponegrita));
            FechaRecAntiguedadText.BackgroundColor = BaseColor.LIGHT_GRAY;
            FechaRecAntiguedadText.Colspan = 19;
            table.AddCell(FechaRecAntiguedadText);

            PdfPCell FechaRecAntiguedad = new PdfPCell(new Phrase(((NominaTrab.FechaReconocimientoAntiguedad)).Value.ToShortDateString(), fontCuerpo));
            FechaRecAntiguedad.HorizontalAlignment = Element.ALIGN_RIGHT;
            FechaRecAntiguedad.Colspan = 29;
            table.AddCell(FechaRecAntiguedad);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell rfcText = new PdfPCell(new Phrase("RFC:", fontCuerponegrita));
            rfcText.BackgroundColor = BaseColor.LIGHT_GRAY;
            rfcText.Colspan = 19;
            table.AddCell(rfcText);

            PdfPCell rfc = new PdfPCell(new Phrase(datos.Rfc.ToUpper().Trim(), fontCuerpo));
            rfc.HorizontalAlignment = Element.ALIGN_RIGHT;
            rfc.Colspan = 29;
            table.AddCell(rfc);

            table.AddCell(espacio1);

            PdfPCell FechaSalidaText = new PdfPCell(new Phrase("Fecha Salida:", fontCuerponegrita));
            FechaSalidaText.BackgroundColor = BaseColor.LIGHT_GRAY;
            FechaSalidaText.Colspan = 19;
            table.AddCell(FechaSalidaText);

            string fechaBaja = "N/A";
            try { fechaBaja = (NominaTrab.FechaBaja).Value.ToShortDateString(); } catch { }
            PdfPCell FechaSalida = new PdfPCell(new Phrase(fechaBaja, fontCuerpo));
            FechaSalida.HorizontalAlignment = Element.ALIGN_RIGHT;
            FechaSalida.Colspan = 29;
            table.AddCell(FechaSalida);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell PuestoText = new PdfPCell(new Phrase("Puesto:", fontCuerponegrita));
            PuestoText.BackgroundColor = BaseColor.LIGHT_GRAY;
            PuestoText.Colspan = 19;
            table.AddCell(PuestoText);

            PdfPCell Puesto = new PdfPCell(new Phrase(_Puesto, fontCuerpo));
            Puesto.HorizontalAlignment = Element.ALIGN_RIGHT;
            Puesto.Colspan = 29;
            table.AddCell(Puesto);

            table.AddCell(espacio1);

            PdfPCell AntiguedadText = new PdfPCell(new Phrase("Años Antiguedad:", fontCuerponegrita));
            AntiguedadText.BackgroundColor = BaseColor.LIGHT_GRAY;
            AntiguedadText.Colspan = 19;
            table.AddCell(AntiguedadText);

            PdfPCell Antiguedad = new PdfPCell(new Phrase(((NominaTrab.Anios)).ToString(), fontCuerpo));
            Antiguedad.HorizontalAlignment = Element.ALIGN_RIGHT;
            Antiguedad.Colspan = 29;
            table.AddCell(Antiguedad);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell DepartamentoText = new PdfPCell(new Phrase("Departamento:", fontCuerponegrita));
            DepartamentoText.BackgroundColor = BaseColor.LIGHT_GRAY;
            DepartamentoText.Colspan = 19;
            table.AddCell(DepartamentoText);

            PdfPCell Departamento = new PdfPCell(new Phrase(_Departamento, fontCuerpo));
            Departamento.HorizontalAlignment = Element.ALIGN_RIGHT;
            Departamento.Colspan = 29;
            table.AddCell(Departamento);

            table.AddCell(espacio1);

            PdfPCell GrupoPagoText = new PdfPCell(new Phrase("Grupo de Pago:", fontCuerponegrita));
            GrupoPagoText.BackgroundColor = BaseColor.LIGHT_GRAY;
            GrupoPagoText.Colspan = 19;
            table.AddCell(GrupoPagoText);

            PdfPCell GrupoPago = new PdfPCell(new Phrase(((_UnidadNegocio)).ToString(), fontCuerpo));
            GrupoPago.HorizontalAlignment = Element.ALIGN_RIGHT;
            GrupoPago.Colspan = 29;
            table.AddCell(GrupoPago);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell CentroCostosText = new PdfPCell(new Phrase("Centro de Costos:", fontCuerponegrita));
            CentroCostosText.BackgroundColor = BaseColor.LIGHT_GRAY;
            CentroCostosText.Colspan = 19;
            table.AddCell(CentroCostosText);

            PdfPCell CentroCostos = new PdfPCell(new Phrase(_CentroCostos, fontCuerpo));
            CentroCostos.HorizontalAlignment = Element.ALIGN_RIGHT;
            CentroCostos.Colspan = 29;
            table.AddCell(CentroCostos);

            table.AddCell(espacio1);

            PdfPCell sdiText = new PdfPCell(new Phrase("Salario Diario Integrado", fontCuerponegrita));
            sdiText.BackgroundColor = BaseColor.LIGHT_GRAY;
            sdiText.Colspan = 19;
            table.AddCell(sdiText);

            PdfPCell sdi = new PdfPCell(new Phrase((((decimal)NominaTrab.SDI)).ToString("C2"), fontCuerpo));
            sdi.HorizontalAlignment = Element.ALIGN_RIGHT;
            sdi.Colspan = 29;
            table.AddCell(sdi);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell EmpresaPatronaText = new PdfPCell(new Phrase("Empresa Patrona:", fontCuerponegrita));
            EmpresaPatronaText.BackgroundColor = BaseColor.LIGHT_GRAY;
            EmpresaPatronaText.Colspan = 19;
            table.AddCell(EmpresaPatronaText);

            PdfPCell EmpresaPatrona = new PdfPCell(new Phrase(_EmpresaPatrona, fontCuerpo));
            EmpresaPatrona.HorizontalAlignment = Element.ALIGN_RIGHT;
            EmpresaPatrona.Colspan = 29;
            table.AddCell(EmpresaPatrona);

            table.AddCell(espacio1);

            PdfPCell SalarioDiarioText = new PdfPCell(new Phrase("Salario Diario", fontCuerponegrita));
            SalarioDiarioText.BackgroundColor = BaseColor.LIGHT_GRAY;
            SalarioDiarioText.Colspan = 19;
            table.AddCell(SalarioDiarioText);

            PdfPCell salarioDiario = new PdfPCell(new Phrase((((decimal)NominaTrab.SueldoDiario)).ToString("C2"), fontCuerpo));
            salarioDiario.HorizontalAlignment = Element.ALIGN_RIGHT;
            salarioDiario.Colspan = 29;
            table.AddCell(salarioDiario);

            ////////////////////////////////////////////////////////////////////////////////////

            table.AddCell(vacia);
            table.AddCell(vacia);

            ////////////////////////////////////////////////////////////////////////////////////
            PdfPCell TituloPercepciones = new PdfPCell(new Phrase("Percepciones", fontCuerponegrita));
            TituloPercepciones.BackgroundColor = BaseColor.LIGHT_GRAY;
            TituloPercepciones.Colspan = 100;
            table.AddCell(TituloPercepciones);
            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell EncClaveConcepto = new PdfPCell(new Phrase("CVE", fontCuerponegrita));
            EncClaveConcepto.Colspan = 10;
            EncClaveConcepto.BorderWidthBottom = 0;
            EncClaveConcepto.BorderWidthLeft = 0;
            EncClaveConcepto.BorderWidthTop = 0;
            EncClaveConcepto.BorderWidthRight = 0;
            table.AddCell(EncClaveConcepto);

            PdfPCell EncDescripcionConcepto = new PdfPCell(new Phrase("DESC. CONCEPTO", fontCuerponegrita));
            EncDescripcionConcepto.Colspan = 30;
            EncDescripcionConcepto.BorderWidthBottom = 0;
            EncDescripcionConcepto.BorderWidthLeft = 0;
            EncDescripcionConcepto.BorderWidthTop = 0;
            EncDescripcionConcepto.BorderWidthRight = 0;
            table.AddCell(EncDescripcionConcepto);

            PdfPCell EncTradicional = new PdfPCell(new Phrase("", fontCuerponegrita));
            EncTradicional.Colspan = 20;
            EncTradicional.BorderWidthBottom = 0;
            EncTradicional.BorderWidthLeft = 0;
            EncTradicional.BorderWidthTop = 0;
            EncTradicional.BorderWidthRight = 0;
            table.AddCell(EncTradicional);

            PdfPCell EncFactores = new PdfPCell(new Phrase("FACTORES", fontCuerponegrita));
            EncFactores.Colspan = 20;
            EncFactores.BorderWidthBottom = 0;
            EncFactores.BorderWidthLeft = 0;
            EncFactores.BorderWidthTop = 0;
            EncFactores.BorderWidthRight = 0;
            table.AddCell(EncFactores);

            PdfPCell EncTotales = new PdfPCell(new Phrase("TOTALES", fontCuerponegrita));
            EncTotales.Colspan = 20;
            EncTotales.BorderWidthBottom = 0;
            EncTotales.BorderWidthLeft = 0;
            EncTotales.BorderWidthTop = 0;
            EncTotales.BorderWidthRight = 0;
            table.AddCell(EncTotales);

            ////////////////////////////////////////////////////////////////////////////////////
            //sueldo pagado
            if (NominaTrab.SueldoPagado != 0)
            {
                PdfPCell ClvSueldo = new PdfPCell(new Phrase("101", fontCuerpo));
                ClvSueldo.Colspan = 10;
                ClvSueldo.BorderWidthBottom = 0;
                ClvSueldo.BorderWidthLeft = 0;
                ClvSueldo.BorderWidthTop = 0;
                ClvSueldo.BorderWidthRight = 0;
                table.AddCell(ClvSueldo);

                PdfPCell descSueldo = new PdfPCell(new Phrase("Sueldo", fontCuerpo));
                descSueldo.Colspan = 30;
                descSueldo.BorderWidthBottom = 0;
                descSueldo.BorderWidthLeft = 0;
                descSueldo.BorderWidthTop = 0;
                descSueldo.BorderWidthRight = 0;
                table.AddCell(descSueldo);

                PdfPCell sueldoMontoTrad = new PdfPCell(new Phrase("", fontCuerpo));
                sueldoMontoTrad.Colspan = 20;
                sueldoMontoTrad.BorderWidthBottom = 0;
                sueldoMontoTrad.BorderWidthLeft = 0;
                sueldoMontoTrad.BorderWidthTop = 0;
                sueldoMontoTrad.BorderWidthRight = 0;
                table.AddCell(sueldoMontoTrad);

                decimal _sueldoFactores = 0;
                try { _sueldoFactores = Math.Round((decimal)incidencias.Where(x => x.ClaveConcepto == "001").Select(x => x.Cantidad).Sum(), 2); } catch { _sueldoFactores = 0; }
                PdfPCell sueldoFactores = new PdfPCell(new Phrase(_sueldoFactores.ToString(), fontCuerpo));
                sueldoFactores.Colspan = 20;
                sueldoFactores.BorderWidthBottom = 0;
                sueldoFactores.BorderWidthLeft = 0;
                sueldoFactores.BorderWidthTop = 0;
                sueldoFactores.BorderWidthRight = 0;
                table.AddCell(sueldoFactores);

                PdfPCell sueldoTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.SueldoPagado), fontCuerpo));
                sueldoTotales.Colspan = 20;
                sueldoTotales.BorderWidthBottom = 0;
                sueldoTotales.BorderWidthLeft = 0;
                sueldoTotales.BorderWidthTop = 0;
                sueldoTotales.BorderWidthRight = 0;
                table.AddCell(sueldoTotales);
            }

            //subsidio
            if (NominaTrab.SubsidioPagar != 0)
            {
                PdfPCell CveSubsidio = new PdfPCell(new Phrase("102", fontCuerpo));
                CveSubsidio.Colspan = 10;
                CveSubsidio.BorderWidthBottom = 0;
                CveSubsidio.BorderWidthLeft = 0;
                CveSubsidio.BorderWidthTop = 0;
                CveSubsidio.BorderWidthRight = 0;
                table.AddCell(CveSubsidio);

                PdfPCell descSubsidio = new PdfPCell(new Phrase("SUBSIDIO AL EMPLEO", fontCuerpo));
                descSubsidio.Colspan = 30;
                descSubsidio.BorderWidthBottom = 0;
                descSubsidio.BorderWidthLeft = 0;
                descSubsidio.BorderWidthTop = 0;
                descSubsidio.BorderWidthRight = 0;
                table.AddCell(descSubsidio);

                PdfPCell subsidioMonto = new PdfPCell(new Phrase("", fontCuerpo));
                subsidioMonto.Colspan = 20;
                subsidioMonto.BorderWidthBottom = 0;
                subsidioMonto.BorderWidthLeft = 0;
                subsidioMonto.BorderWidthTop = 0;
                subsidioMonto.BorderWidthRight = 0;
                table.AddCell(subsidioMonto);

                decimal _sbsidioFactores = 0;

                PdfPCell subsidioFactores = new PdfPCell(new Phrase(_sbsidioFactores.ToString(), fontCuerpo));
                subsidioFactores.Colspan = 20;
                subsidioFactores.BorderWidthBottom = 0;
                subsidioFactores.BorderWidthLeft = 0;
                subsidioFactores.BorderWidthTop = 0;
                subsidioFactores.BorderWidthRight = 0;
                table.AddCell(subsidioFactores);

                PdfPCell subsidioTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.SubsidioPagar), fontCuerpo));
                subsidioTotales.Colspan = 20;
                subsidioTotales.BorderWidthBottom = 0;
                subsidioTotales.BorderWidthLeft = 0;
                subsidioTotales.BorderWidthTop = 0;
                subsidioTotales.BorderWidthRight = 0;
                table.AddCell(subsidioTotales);
            }

            ////////consulta otros finiquitos ////////////

            foreach (var item in incidencias.Where(x => x.TipoConcepto == "ER" && x.IdEstatus == 1).ToList())
            {
                if (item.Monto != 0)
                {
                    PdfPCell ClaveConcepto = new PdfPCell(new Phrase(item.ClaveConcepto, fontCuerpo));
                    ClaveConcepto.Colspan = 10;
                    ClaveConcepto.BorderWidthBottom = 0;
                    ClaveConcepto.BorderWidthLeft = 0;
                    ClaveConcepto.BorderWidthTop = 0;
                    ClaveConcepto.BorderWidthRight = 0;
                    table.AddCell(ClaveConcepto);

                    PdfPCell descConcepto = new PdfPCell(new Phrase(item.Concepto, fontCuerpo));
                    descConcepto.Colspan = 30;
                    descConcepto.BorderWidthBottom = 0;
                    descConcepto.BorderWidthLeft = 0;
                    descConcepto.BorderWidthTop = 0;
                    descConcepto.BorderWidthRight = 0;
                    table.AddCell(descConcepto);


                    PdfPCell MontoTradicional = new PdfPCell(new Phrase("", fontCuerpo));
                    MontoTradicional.Colspan = 20;
                    MontoTradicional.BorderWidthBottom = 0;
                    MontoTradicional.BorderWidthLeft = 0;
                    MontoTradicional.BorderWidthTop = 0;
                    MontoTradicional.BorderWidthRight = 0;
                    table.AddCell(MontoTradicional);

                    PdfPCell ConceptoFactores = new PdfPCell(new Phrase(item.Cantidad.ToString(), fontCuerpo));
                    ConceptoFactores.Colspan = 20;
                    ConceptoFactores.BorderWidthBottom = 0;
                    ConceptoFactores.BorderWidthLeft = 0;
                    ConceptoFactores.BorderWidthTop = 0;
                    ConceptoFactores.BorderWidthRight = 0;
                    table.AddCell(ConceptoFactores);

                    PdfPCell ConceptoTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", item.Monto), fontCuerpo));
                    ConceptoTotales.Colspan = 20;
                    ConceptoTotales.BorderWidthBottom = 0;
                    ConceptoTotales.BorderWidthLeft = 0;
                    ConceptoTotales.BorderWidthTop = 0;
                    ConceptoTotales.BorderWidthRight = 0;
                    table.AddCell(ConceptoTotales);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////
            table.AddCell(vacia);
            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell TOTALPercepciones = new PdfPCell(new Phrase(" SUBTOTAL PERCEPCIONES", fontCuerponegrita));
            TOTALPercepciones.Colspan = 40;
            TOTALPercepciones.BorderWidthRight = 0;
            table.AddCell(TOTALPercepciones);

            PdfPCell TOTALPercepciones1 = new PdfPCell(new Phrase("", fontCuerponegrita));
            TOTALPercepciones1.Colspan = 40;
            TOTALPercepciones1.BorderWidthRight = 0;
            TOTALPercepciones1.BorderWidthLeft = 0;
            table.AddCell(TOTALPercepciones1);

            PdfPCell TOTALPercepciones2 = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.ER), fontCuerponegrita));
            TOTALPercepciones2.Colspan = 20;
            TOTALPercepciones2.BorderWidthLeft = 0;
            table.AddCell(TOTALPercepciones2);

            ////////////////////////////////////////////////////////////////////////////////////
            table.AddCell(vacia);
            table.AddCell(vacia);
            ////////////////////////////////////////////////////////////////////////////////////
            PdfPCell TituloDeducciones = new PdfPCell(new Phrase("Deducciones", fontCuerponegrita));
            TituloDeducciones.BackgroundColor = BaseColor.LIGHT_GRAY;
            TituloDeducciones.Colspan = 100;
            table.AddCell(TituloDeducciones);
            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell EncClaveConcepto1 = new PdfPCell(new Phrase("CVE", fontCuerponegrita));
            EncClaveConcepto1.Colspan = 10;
            EncClaveConcepto1.BorderWidthBottom = 0;
            EncClaveConcepto1.BorderWidthLeft = 0;
            EncClaveConcepto1.BorderWidthTop = 0;
            EncClaveConcepto1.BorderWidthRight = 0;
            table.AddCell(EncClaveConcepto1);

            PdfPCell EncDescripcionConcepto1 = new PdfPCell(new Phrase("DESC. CONCEPTO", fontCuerponegrita));
            EncDescripcionConcepto1.Colspan = 30;
            EncDescripcionConcepto1.BorderWidthBottom = 0;
            EncDescripcionConcepto1.BorderWidthLeft = 0;
            EncDescripcionConcepto1.BorderWidthTop = 0;
            EncDescripcionConcepto1.BorderWidthRight = 0;
            table.AddCell(EncDescripcionConcepto1);

            PdfPCell EncTradicional1 = new PdfPCell(new Phrase("", fontCuerponegrita));
            EncTradicional1.Colspan = 20;
            EncTradicional1.BorderWidthBottom = 0;
            EncTradicional1.BorderWidthLeft = 0;
            EncTradicional1.BorderWidthTop = 0;
            EncTradicional1.BorderWidthRight = 0;
            table.AddCell(EncTradicional1);

            PdfPCell EncFactores1 = new PdfPCell(new Phrase("FACTORES", fontCuerponegrita));
            EncFactores1.Colspan = 20;
            EncFactores1.BorderWidthBottom = 0;
            EncFactores1.BorderWidthLeft = 0;
            EncFactores1.BorderWidthTop = 0;
            EncFactores1.BorderWidthRight = 0;
            table.AddCell(EncFactores1);

            PdfPCell EncTotales1 = new PdfPCell(new Phrase("TOTALES", fontCuerponegrita));
            EncTotales1.Colspan = 20;
            EncTotales1.BorderWidthBottom = 0;
            EncTotales1.BorderWidthLeft = 0;
            EncTotales1.BorderWidthTop = 0;
            EncTotales1.BorderWidthRight = 0;
            table.AddCell(EncTotales1);

            ////////////////////////////////////////////////////////////////////////////////////

            if ((NominaTrab.ImpuestoRetener + NominaTrab.ISRLiquidacion) != 0)
            {
                PdfPCell ClvISR = new PdfPCell(new Phrase("001", fontCuerpo));
                ClvISR.Colspan = 10;
                ClvISR.BorderWidthBottom = 0;
                ClvISR.BorderWidthLeft = 0;
                ClvISR.BorderWidthTop = 0;
                ClvISR.BorderWidthRight = 0;
                table.AddCell(ClvISR);

                PdfPCell descISR = new PdfPCell(new Phrase("ISR", fontCuerpo));
                descISR.Colspan = 30;
                descISR.BorderWidthBottom = 0;
                descISR.BorderWidthLeft = 0;
                descISR.BorderWidthTop = 0;
                descISR.BorderWidthRight = 0;
                table.AddCell(descISR);

                PdfPCell ISRTrad = new PdfPCell(new Phrase("", fontCuerpo));
                ISRTrad.Colspan = 20;
                ISRTrad.BorderWidthBottom = 0;
                ISRTrad.BorderWidthLeft = 0;
                ISRTrad.BorderWidthTop = 0;
                ISRTrad.BorderWidthRight = 0;
                table.AddCell(ISRTrad);

                PdfPCell ISRFactor = new PdfPCell(new Phrase("0", fontCuerpo));
                ISRFactor.Colspan = 20;
                ISRFactor.BorderWidthBottom = 0;
                ISRFactor.BorderWidthLeft = 0;
                ISRFactor.BorderWidthTop = 0;
                ISRFactor.BorderWidthRight = 0;
                table.AddCell(ISRFactor);

                PdfPCell ISRTotal = new PdfPCell(new Phrase(String.Format("{0:C2}", (NominaTrab.ImpuestoRetener + NominaTrab.ISRLiquidacion)), fontCuerpo));
                ISRTotal.Colspan = 20;
                ISRTotal.BorderWidthBottom = 0;
                ISRTotal.BorderWidthLeft = 0;
                ISRTotal.BorderWidthTop = 0;
                ISRTotal.BorderWidthRight = 0;
                table.AddCell(ISRTotal);
            }

            ////////////////////////////////////////////////////////////////////////////////////

            if (NominaTrab.IMSS_Obrero != 0)
            {
                PdfPCell ClvIMSS = new PdfPCell(new Phrase("002", fontCuerpo));
                ClvIMSS.Colspan = 10;
                ClvIMSS.BorderWidthBottom = 0;
                ClvIMSS.BorderWidthLeft = 0;
                ClvIMSS.BorderWidthTop = 0;
                ClvIMSS.BorderWidthRight = 0;
                table.AddCell(ClvIMSS);

                PdfPCell descIMSS = new PdfPCell(new Phrase("CARGAS SOCIALES", fontCuerpo));
                descIMSS.Colspan = 30;
                descIMSS.BorderWidthBottom = 0;
                descIMSS.BorderWidthLeft = 0;
                descIMSS.BorderWidthTop = 0;
                descIMSS.BorderWidthRight = 0;
                table.AddCell(descIMSS);

                PdfPCell IMSSTrad = new PdfPCell(new Phrase("", fontCuerpo));
                IMSSTrad.Colspan = 20;
                IMSSTrad.BorderWidthBottom = 0;
                IMSSTrad.BorderWidthLeft = 0;
                IMSSTrad.BorderWidthTop = 0;
                IMSSTrad.BorderWidthRight = 0;
                table.AddCell(IMSSTrad);

                PdfPCell IMSSFactor = new PdfPCell(new Phrase("0", fontCuerpo));
                IMSSFactor.Colspan = 20;
                IMSSFactor.BorderWidthBottom = 0;
                IMSSFactor.BorderWidthLeft = 0;
                IMSSFactor.BorderWidthTop = 0;
                IMSSFactor.BorderWidthRight = 0;
                table.AddCell(IMSSFactor);

                PdfPCell IMSSTotal = new PdfPCell(new Phrase(string.Format("{0:C2}", (NominaTrab.IMSS_Obrero)), fontCuerpo));
                IMSSTotal.Colspan = 20;
                IMSSTotal.BorderWidthBottom = 0;
                IMSSTotal.BorderWidthLeft = 0;
                IMSSTotal.BorderWidthTop = 0;
                IMSSTotal.BorderWidthRight = 0;
                table.AddCell(IMSSTotal);
            }

            ////////////////////////////////////////////////////////////////////////////////////
            ////////consulta otros finiquitos ////////////

            foreach (var item in incidencias.Where(x => x.TipoConcepto == "DD" && x.IdEstatus == 1))
            {
                if (item.Monto != 0)
                {
                    PdfPCell ClaveConcepto = new PdfPCell(new Phrase(item.ClaveConcepto, fontCuerpo));
                    ClaveConcepto.Colspan = 10;
                    ClaveConcepto.BorderWidthBottom = 0;
                    ClaveConcepto.BorderWidthLeft = 0;
                    ClaveConcepto.BorderWidthTop = 0;
                    ClaveConcepto.BorderWidthRight = 0;
                    table.AddCell(ClaveConcepto);

                    PdfPCell descConcepto = new PdfPCell(new Phrase(item.Concepto, fontCuerpo));
                    descConcepto.Colspan = 30;
                    descConcepto.BorderWidthBottom = 0;
                    descConcepto.BorderWidthLeft = 0;
                    descConcepto.BorderWidthTop = 0;
                    descConcepto.BorderWidthRight = 0;
                    table.AddCell(descConcepto);

                    PdfPCell MontoTradicional = new PdfPCell(new Phrase("", fontCuerpo));
                    MontoTradicional.Colspan = 20;
                    MontoTradicional.BorderWidthBottom = 0;
                    MontoTradicional.BorderWidthLeft = 0;
                    MontoTradicional.BorderWidthTop = 0;
                    MontoTradicional.BorderWidthRight = 0;
                    table.AddCell(MontoTradicional);

                    PdfPCell ConceptoFactores = new PdfPCell(new Phrase(item.Cantidad.ToString(), fontCuerpo));
                    ConceptoFactores.Colspan = 20;
                    ConceptoFactores.BorderWidthBottom = 0;
                    ConceptoFactores.BorderWidthLeft = 0;
                    ConceptoFactores.BorderWidthTop = 0;
                    ConceptoFactores.BorderWidthRight = 0;
                    table.AddCell(ConceptoFactores);

                    PdfPCell ConceptoTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", item.Monto), fontCuerpo));
                    ConceptoTotales.Colspan = 20;
                    ConceptoTotales.BorderWidthBottom = 0;
                    ConceptoTotales.BorderWidthLeft = 0;
                    ConceptoTotales.BorderWidthTop = 0;
                    ConceptoTotales.BorderWidthRight = 0;
                    table.AddCell(ConceptoTotales);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////
            table.AddCell(vacia);
            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell TOTALDeducciones = new PdfPCell(new Phrase("SUBTOTAL DEDUCCIONES", fontCuerponegrita));
            TOTALDeducciones.Colspan = 40;
            TOTALDeducciones.BorderWidthRight = 0;
            table.AddCell(TOTALDeducciones);

            PdfPCell TOTALDeducciones1 = new PdfPCell(new Phrase("", fontCuerponegrita));
            TOTALDeducciones1.Colspan = 40;
            TOTALDeducciones1.BorderWidthRight = 0;
            TOTALDeducciones1.BorderWidthLeft = 0;
            table.AddCell(TOTALDeducciones1);

            PdfPCell TOTALDeducciones2 = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.DD), fontCuerponegrita));
            TOTALDeducciones2.Colspan = 20;
            TOTALDeducciones2.BorderWidthLeft = 0;
            table.AddCell(TOTALDeducciones2);

            ////////////////////////////////////////////////////////////////////////////////////

            table.AddCell(vacia);
            table.AddCell(vacia);
            ////////////////////////////////////////////////////////////////////////////////

            PdfPCell _TOTALPercepciones = new PdfPCell(new Phrase("TOTAL PERCEPCIONES", fontCuerponegrita));
            _TOTALPercepciones.Colspan = 40;
            _TOTALPercepciones.BorderWidthRight = 0;
            table.AddCell(_TOTALPercepciones);

            PdfPCell TOTALPercepciones12 = new PdfPCell(new Phrase("", fontCuerponegrita));
            TOTALPercepciones12.Colspan = 40;
            TOTALPercepciones12.BorderWidthRight = 0;
            TOTALPercepciones12.BorderWidthLeft = 0;
            table.AddCell(TOTALPercepciones12);

            PdfPCell TOTALPercepciones23 = new PdfPCell(new Phrase(((float)NominaTrab.ER).ToString("C2"), fontCuerponegrita));
            TOTALPercepciones23.Colspan = 20;
            TOTALPercepciones23.BorderWidthLeft = 0;
            table.AddCell(TOTALPercepciones23);

            ////////////////////////////////////////////////////////////////////////////////////

            PdfPCell _TOTALDeducciones = new PdfPCell(new Phrase("TOTAL DEDUCCIONES", fontCuerponegrita));
            _TOTALDeducciones.Colspan = 40;
            _TOTALDeducciones.BorderWidthRight = 0;
            table.AddCell(_TOTALDeducciones);

            PdfPCell TOTALDeducciones12 = new PdfPCell(new Phrase("", fontCuerponegrita));
            TOTALDeducciones12.Colspan = 40;
            TOTALDeducciones12.BorderWidthRight = 0;
            TOTALDeducciones12.BorderWidthLeft = 0;
            table.AddCell(TOTALDeducciones12);

            PdfPCell TOTALDeducciones23 = new PdfPCell(new Phrase(((float)NominaTrab.DD).ToString("C2"), fontCuerponegrita));
            TOTALDeducciones23.Colspan = 20;
            TOTALDeducciones23.BorderWidthLeft = 0;
            table.AddCell(TOTALDeducciones23);

            ////////////////////////////////////////////////////////////////////////////////////

            decimal Neto = (decimal)NominaTrab.ER - (decimal)NominaTrab.DD;

            PdfPCell NETO = new PdfPCell(new Phrase("NETO A PAGAR", fontCuerponegrita));
            NETO.BackgroundColor = BaseColor.LIGHT_GRAY;
            NETO.Colspan = 40;
            NETO.BorderWidthRight = 0;
            table.AddCell(NETO);

            PdfPCell NETO1 = new PdfPCell(new Phrase("", fontCuerponegrita));
            NETO1.BackgroundColor = BaseColor.LIGHT_GRAY;
            NETO1.Colspan = 40;
            NETO1.BorderWidthRight = 0;
            NETO1.BorderWidthLeft = 0;
            table.AddCell(NETO1);

            PdfPCell NETO2 = new PdfPCell(new Phrase(Neto.ToString("C2"), fontCuerponegrita));
            NETO2.BackgroundColor = BaseColor.LIGHT_GRAY;
            NETO2.Colspan = 20;
            NETO2.BorderWidthLeft = 0;
            table.AddCell(NETO2);

            ///////////////////////////////////////////////////////////////////////////////////////
            //Leyenda           

            if (unidad.LeyendaReciboFiniquitos != null && unidad.LeyendaReciboFiniquitos.Length > 0)
            {
                table.AddCell(vacia);
                PdfPCell leyenda = new PdfPCell(new Phrase(unidad.LeyendaReciboFiniquitos, fontCuerpo10));
                leyenda.HorizontalAlignment = Element.ALIGN_CENTER;
                leyenda.BorderWidthBottom = 0;
                leyenda.Colspan = 100;
                table.AddCell(leyenda);
            }
            else
            {
                table.AddCell(vacia);
                table.AddCell(vacia);
                table.AddCell(vacia);
            }

            ////////////////////////////////////////////////////////////////////////////////////////
            PdfPCell Firma1 = new PdfPCell(new Phrase(" ", fontCuerponegrita));
            Firma1.BorderWidthBottom = 0;
            Firma1.Colspan = 100;
            table.AddCell(Firma1);

            PdfPCell Firma2 = new PdfPCell(new Phrase(" ", fontCuerponegrita));
            Firma2.BorderWidthBottom = 0;
            Firma2.BorderWidthTop = 0;
            Firma2.Colspan = 100;
            table.AddCell(Firma2);

            PdfPCell Firma3 = new PdfPCell(new Phrase(" ", fontCuerponegrita));
            Firma3.BorderWidthBottom = 0;
            Firma3.BorderWidthTop = 0;
            Firma3.Colspan = 100;
            table.AddCell(Firma3);

            PdfPCell Firma = new PdfPCell(new Phrase("Nombre y Firma", fontCuerponegrita));
            Firma.BackgroundColor = BaseColor.LIGHT_GRAY;
            Firma.HorizontalAlignment = Element.ALIGN_CENTER;
            Firma.Colspan = 100;
            table.AddCell(Firma);

            ////////////////////////////////////////////////////////////////////////////////////

            ///// se agrega la tabla al documento///////
            doc.Add(table);

            ///// se cierra el documento////////
            doc.Close();
            writer.Close();

            return ms.ToArray();
        }

        private static decimal getSueldo(NominaTrabajo NominaTrab, Cat_TipoNomina tipoNomina, decimal sueldoMensual)
        {
            if (tipoNomina.Clave_Sat == "02" || tipoNomina.Clave_Sat == "03")
                sueldoMensual = (decimal)NominaTrab.SueldoDiario * 30;

            if (tipoNomina.Clave_Sat == "04")
                sueldoMensual = (decimal)NominaTrab.SueldoDiario * (tipoNomina.DiasPago * 2);

            if (tipoNomina.Clave_Sat == "05")
                sueldoMensual = (decimal)NominaTrab.SueldoDiario * (tipoNomina.DiasPago);

            return sueldoMensual;
        }
    }
}