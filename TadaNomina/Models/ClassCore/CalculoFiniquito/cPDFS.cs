using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.CalculoFiniquito
{
    public class cPDFS
    {
        /// <summary>
        ///     Método que genera el PDF 
        /// </summary>
        /// <param name="IdPeriodonomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdEmpleado">Variable que contiene el id de empleado</param>
        /// <returns>Arreglo de bytes con el PDF generado</returns>
        public byte[] getPDFEsq(int IdPeriodonomina, int IdEmpleado)
        {
            string _Departamento = string.Empty;
            string _Puesto = string.Empty;
            string _CentroCostos = string.Empty;
            string _EmpresaPatrona = string.Empty;
            string _UnidadNegocio = string.Empty;

            ClassNomina cn = new ClassNomina();
            ClassIncidencias ci = new ClassIncidencias();
            ClassEmpleado ce = new ClassEmpleado();

            var datos = ce.GetvEmpleado(IdEmpleado);
            var NominaTrab = cn.GetNominaTrabajoFiniquitos(IdEmpleado, IdPeriodonomina);
            var incidencias = ci.GetvIncindencias(IdPeriodonomina, IdEmpleado);

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
            string _TipoCalculo = "CÁLCULO COMPLEMENTO";

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

            PdfPCell NoEmp = new PdfPCell(new Phrase("N/A", fontCuerpo));
            NoEmp.HorizontalAlignment = Element.ALIGN_RIGHT;
            NoEmp.Colspan = 29;
            table.AddCell(NoEmp);

            table.AddCell(espacio1);

            PdfPCell NombreText = new PdfPCell(new Phrase("Nombre:", fontCuerponegrita));
            NombreText.BackgroundColor = BaseColor.LIGHT_GRAY;
            NombreText.Colspan = 19;
            table.AddCell(NombreText);

            PdfPCell Nombre = new PdfPCell(new Phrase(datos.ApellidoPaterno + " " + datos.ApellidoMaterno + " " + datos.Nombre, fontCuerpo));
            Nombre.HorizontalAlignment = Element.ALIGN_RIGHT;
            Nombre.Colspan = 29;
            table.AddCell(Nombre);
            
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

            PdfPCell GrupoPagoText = new PdfPCell(new Phrase("Grupo de Pago:", fontCuerponegrita));
            GrupoPagoText.BackgroundColor = BaseColor.LIGHT_GRAY;
            GrupoPagoText.Colspan = 19;
            table.AddCell(GrupoPagoText);

            PdfPCell GrupoPago = new PdfPCell(new Phrase(((_UnidadNegocio)).ToString(), fontCuerpo));
            GrupoPago.HorizontalAlignment = Element.ALIGN_RIGHT;
            GrupoPago.Colspan = 29;
            table.AddCell(GrupoPago);

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

            if (NominaTrab.Apoyo != 0)
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

                PdfPCell sueldoTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.Apoyo), fontCuerpo));
                sueldoTotales.Colspan = 20;
                sueldoTotales.BorderWidthBottom = 0;
                sueldoTotales.BorderWidthLeft = 0;
                sueldoTotales.BorderWidthTop = 0;
                sueldoTotales.BorderWidthRight = 0;
                table.AddCell(sueldoTotales);
            }

            ////////consulta otros finiquitos ////////////

            foreach (var item in incidencias.Where(x => x.TipoConcepto == "ER" && x.IdEstatus == 1).ToList())
            {
                if (item.MontoEsquema != 0)
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

                    PdfPCell ConceptoTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", item.MontoEsquema), fontCuerpo));
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

            PdfPCell TOTALPercepciones2 = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.ERS), fontCuerponegrita));
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

            ////////consulta otros finiquitos ////////////

            foreach (var item in incidencias.Where(x => x.TipoConcepto == "DD" && x.IdEstatus == 1))
            {
                if (item.MontoEsquema != 0)
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

                    PdfPCell ConceptoTotales = new PdfPCell(new Phrase(string.Format("{0:C2}", item.MontoEsquema), fontCuerpo));
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

            PdfPCell TOTALDeducciones2 = new PdfPCell(new Phrase(string.Format("{0:C2}", NominaTrab.DDS), fontCuerponegrita));
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

            PdfPCell TOTALPercepciones23 = new PdfPCell(new Phrase(((float)NominaTrab.ERS).ToString("C2"), fontCuerponegrita));
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

            PdfPCell TOTALDeducciones23 = new PdfPCell(new Phrase(((float)NominaTrab.DDS).ToString("C2"), fontCuerponegrita));
            TOTALDeducciones23.Colspan = 20;
            TOTALDeducciones23.BorderWidthLeft = 0;
            table.AddCell(TOTALDeducciones23);

            ////////////////////////////////////////////////////////////////////////////////////

            decimal Netos = (decimal)NominaTrab.ERS - (decimal)NominaTrab.DDS;

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

            PdfPCell NETO2 = new PdfPCell(new Phrase(Netos.ToString("C2"), fontCuerponegrita));
            NETO2.BackgroundColor = BaseColor.LIGHT_GRAY;
            NETO2.Colspan = 20;
            NETO2.BorderWidthLeft = 0;
            table.AddCell(NETO2);
            /////////////////////////////////////////////////////////////////////////////////////////
            
            table.AddCell(vacia);
            table.AddCell(vacia);
            table.AddCell(vacia);

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
    }
}