using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class ClassReportesPDF
    {
        public static PeriodoNomina periodoStatic { set; get; }
        public static vRegistroPatronal RegistroP { set; get; }

        public ClassReportesPDF(PeriodoNomina per, vRegistroPatronal reg)
        {
            periodoStatic = per;

            RegistroP = reg;
        }

        /// <summary>
        /// Metodo para crear PDF con lista de incidencias
        /// </summary>
        /// <param name="ruta">Ruta del PDF</param>
        /// <param name="incidencias">Lista de incidencias</param>
        /// <param name="datos">Datos por centros de costos</param>
        /// <param name="periodo">Periodo de nomina</param>
        public void CrearPdf(string ruta, List<vIncidencias_Consolidadas> incidencias, List<sp_PDFSunset1_Result> datos, PeriodoNomina periodo)
        {
            Document doc = new Document(PageSize.A4, 10f, 10f, 140f, 10f);
            PdfWriter writer = PdfWriter.GetInstance(doc, new System.IO.FileStream(ruta, System.IO.FileMode.Create));
            writer.PageEvent = new ITextEvents();
            doc.Open();

            Font fontEncabezado = new Font(BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false), 8);
            Font fontTitulo = new Font(BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false), 7);
            Font fontCuerpo = new Font(BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false), 7);

            PdfPTable table = new PdfPTable(10) { WidthPercentage = 100f };

            string puesto = String.Empty;
            foreach (var item in datos)
            {
                var _ins = incidencias.Where(x => x.IdEmpleado == item.IdEmpleado && x.Monto > 0 && x.IdEstatus == 1 && new string[] { "Tradicional", "Mixto" }.Contains(x.TipoEsquema)).ToList();

                if (puesto != item.Puesto)
                {
                    AgregarCelda(fontEncabezado, table, puesto ?? "No contiene este dato", 10, 1);
                    puesto = item.Puesto;
                }

                AgregarCelda(fontEncabezado, table, $" * RAZON SOCIAL : {item.NombrePatrona ?? "No contiene este dato"}", 7, 1);
                AgregarCelda(fontEncabezado, table, $" R.F.C. : {item.RFCPATRONA ?? "No contiene este dato"}", 3, 1);
                AgregarCelda(fontEncabezado, table, item.ClaveEmpleado.ToString().PadLeft(8, '0'), 1, 1);
                AgregarCelda(fontEncabezado, table, item.Rfc ?? "No contiene este dato", 2, 1);
                AgregarCelda(fontEncabezado, table, item.Imss ?? "No contiene este dato", 1, 1);
                AgregarCelda(fontEncabezado, table, item.Curp ?? "No contiene este dato", 2, 1);
                AgregarCelda(fontEncabezado, table, item.Nombre ?? "No contiene este dato", 3, 1);
                AgregarCelda(fontEncabezado, table, item.FechaAltaIMSS?.ToShortDateString() ?? "No contiene este dato", 1, 1);
                AgregarCelda(fontEncabezado, table, item.Departamento ?? "No contiene este dato", 2, 1);
                AgregarCelda(fontEncabezado, table, item.Puesto ?? "No contiene este dato", 3, 1);
                AgregarCelda(fontEncabezado, table, item.sueldoDiario?.ToString("C2") ?? "No contiene este dato", 3, 1);
                AgregarCelda(fontEncabezado, table, item.SDI?.ToString("C2") ?? "No contiene este dato", 2, 1);

                // Percepciones y Deducciones
                AgregarCelda(fontEncabezado, table, " -  Percepciones y Deducciones ", 10, 1);

                PdfPTable tablePercep = new PdfPTable(5) { WidthPercentage = 100f };
                string fechaDispersion = periodo.FechaDispersion?.ToShortDateString() ?? "No contiene este dato";

                AgregarCelda(fontEncabezado, tablePercep, "400 SUELDO", 3, 1);
                AgregarCelda(fontEncabezado, tablePercep, fechaDispersion, 1, 1);
                AgregarCelda(fontEncabezado, tablePercep, item.SueldoPagado?.ToString("C2") ?? "No contiene este dato", 1, 1);

                AgregarCelda(fontEncabezado, tablePercep, "402 SUBSIDIO EMPLEO PAGADO", 3, 1);
                AgregarCelda(fontEncabezado, tablePercep, fechaDispersion, 1, 1);
                AgregarCelda(fontEncabezado, tablePercep, item.SubsidioPagar?.ToString("C2") ?? "No contiene este dato", 1, 1);

                foreach (var ins in _ins.Where(x => x.TipoConcepto == "ER" && x.Monto > 0))
                {
                    AgregarCelda(fontEncabezado, tablePercep, $"{ins.ClaveConcepto} {ins.Concepto}", 4, 1);
                    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", ins.Monto), 1, 1); //revis

                }

                AgregarCelda(fontEncabezado, tablePercep, "Percepciones: ", 4, 3);
                AgregarCelda(fontEncabezado, tablePercep, item.ER?.ToString("C2") ?? "No contiene este dato", 1, 1);

                PdfPTable tableDeduc = new PdfPTable(5) { WidthPercentage = 100f };
                AgregarCelda(fontEncabezado, tableDeduc, "601 IMSS", 4, 1);
                AgregarCelda(fontEncabezado, tableDeduc, item.IMSS_Obrero?.ToString("C2") ?? "No contiene este dato", 1, 1);
                AgregarCelda(fontEncabezado, tableDeduc, "ISR", 4, 1);
                AgregarCelda(fontEncabezado, tableDeduc, item.ISR?.ToString("C2") ?? "No contiene este dato", 1, 1);

                foreach (var ins in _ins.Where(x => x.TipoConcepto == "DD" && x.Monto >= 0))
                {
                    AgregarCelda(fontEncabezado, tableDeduc, $"{ins.ClaveConcepto} {ins.Concepto}", 4, 1);
                    AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", ins.Monto), 1, 1); //revis
                }

                AgregarCelda(fontEncabezado, tableDeduc, "Deducciones: ", 4, 3);
                AgregarCelda(fontEncabezado, tableDeduc, item.DD?.ToString("C2") ?? "No contiene este dato", 1, 1);
                AgregarCelda(fontEncabezado, tableDeduc, "Total de Pago: ", 4, 3);
                AgregarCelda(fontEncabezado, tableDeduc, item.Neto?.ToString("C2") ?? "No contiene este dato", 1, 1);

                PdfPCell columas = new PdfPCell(tablePercep) { Colspan = 5, Padding = 0f, Border = 0 };
                table.AddCell(columas);
                columas = new PdfPCell(tableDeduc) { Colspan = 5, Padding = 0f, Border = 0 };
                table.AddCell(columas);
            }

            // Agregar totales por conceptos
            AgregarTotalesPorConceptos(fontEncabezado, table, incidencias, datos);

            // Agregar totales generales
            AgregarTotalesGenerales(fontEncabezado, table, datos);

            doc.Add(table);
            doc.Close();
        }


        private void AgregarTotalesPorConceptos(Font font, PdfPTable table, List<vIncidencias_Consolidadas> incidencias, List<sp_PDFSunset1_Result> datos)
        {
            PdfPTable totales = new PdfPTable(7) { WidthPercentage = 100f };
            AgregarCelda(font, totales, " Totales Por Centro de Trabajo ", 10, 2);
            AgregarCelda(font, totales, "Concepto", 1, 2);
            AgregarCelda(font, totales, "Descripción", 1, 2);
            AgregarCelda(font, totales, "Dato", 1, 2);
            AgregarCelda(font, totales, "Percepción", 1, 2);
            AgregarCelda(font, totales, "Deducción", 1, 2);
            AgregarCelda(font, totales, "Gravados", 1, 2);
            AgregarCelda(font, totales, "Exento ", 1, 2);

            // Agregar filas de totales
            var listaClaveConcepto = incidencias
                .Where(a => a.TipoConcepto == "ER" || a.TipoConcepto == "DD")
                .GroupBy(a => a.Concepto)
                .Select(g => g.First())
                .OrderBy(a => a.Concepto)
                .ToList();

            foreach (var concepto in listaClaveConcepto)
            {
                decimal? sumaMonto = incidencias.Where(a => a.Concepto == concepto.Concepto && a.TipoConcepto == concepto.TipoConcepto).Sum(a => a.Monto);
                decimal? sumaGravados = incidencias.Where(a => a.Concepto == concepto.Concepto && a.TipoConcepto == concepto.TipoConcepto).Sum(a => a.Gravado);
                decimal? sumaExento = incidencias.Where(a => a.Concepto == concepto.Concepto && a.TipoConcepto == concepto.TipoConcepto).Sum(a => a.Exento);

                AgregarCelda(font, totales, concepto.ClaveConcepto, 1, 1);
                AgregarCelda(font, totales, concepto.Concepto, 1, 1);
                AgregarCelda(font, totales, "", 1, 1);

                if (concepto.TipoConcepto == "ER")
                {
                    AgregarCelda(font, totales, sumaMonto?.ToString("C2") ?? "0", 1, 1);
                    AgregarCelda(font, totales, "", 1, 1);
                }
                else
                {
                    AgregarCelda(font, totales, "", 1, 1);
                    AgregarCelda(font, totales, sumaMonto?.ToString("C2") ?? "0", 1, 1);
                }

                AgregarCelda(font, totales, sumaGravados?.ToString("C2") ?? "0", 1, 1);
                AgregarCelda(font, totales, sumaExento?.ToString("C2") ?? "0", 1, 1);
            }

            // Agregar totales generales al final
            AgregarCelda(font, totales, "Totales:", 3, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.ER)?.ToString("C2") ?? "0", 1, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.DD)?.ToString("C2") ?? "0", 1, 1);
            AgregarCelda(font, totales, incidencias.Sum(a => a.Gravado)?.ToString("C2") ?? "0", 1, 1);
            AgregarCelda(font, totales, incidencias.Sum(a => a.Exento)?.ToString("C2") ?? "0", 1, 1);

            table.AddCell(new PdfPCell(totales) { Colspan = 10, Padding = 0f, Border = 0 });
        }


        private void AgregarTotalesGenerales(Font font, PdfPTable table, List<sp_PDFSunset1_Result> datos)
        {
            PdfPTable totales = new PdfPTable(5) { WidthPercentage = 100f };
            AgregarCelda(font, totales, " Totales Generales ", 10, 1);
            AgregarCelda(font, totales, "Dato", 3, 1);
            AgregarCelda(font, totales, "Cantidad", 1, 1);

            AgregarCelda(font, totales, "Sueldos Pagados", 3, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.SueldoPagado)?.ToString("C2") ?? "0", 1, 1);

            AgregarCelda(font, totales, "Subsisdio al Empleo Pagado", 3, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.SubsidioPagar)?.ToString("C2") ?? "0", 1, 1);

            AgregarCelda(font, totales, "Percepciones", 3, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.ER)?.ToString("C2") ?? "0", 1, 1);

            AgregarCelda(font, totales, "Deducciones", 3, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.DD)?.ToString("C2") ?? "0", 1, 1);

            AgregarCelda(font, totales, "Neto Pagado", 3, 1);
            AgregarCelda(font, totales, datos.Sum(a => a.Neto)?.ToString("C2") ?? "0", 1, 1);

            table.AddCell(new PdfPCell(totales) { Colspan = 10, Padding = 0f, Border = 0 });
        }

        /// <summary>
        /// Metodo para pintar celdas en un PDF
        /// </summary>
        /// <param name="fontEncabezado">Estilo encabezado</param>
        /// <param name="tableEnc">Tabla con número de columnas</param>
        /// <param name="NombreCampo">Nombre del campo a pintar</param>
        /// <param name="Span">Columnas a juntar</param>
        /// <param name="TipoAlineado">1 = left  2 = center 3 = right</param>
        public static void AgregarCelda(Font fontEncabezado, PdfPTable tableEnc, string NombreCampo, int Span, int TipoAlineado)
        {
            PdfPCell DatosEmpresa = new PdfPCell(new Phrase(NombreCampo ?? "N/A", fontEncabezado));

            DatosEmpresa.Border = Rectangle.NO_BORDER;
            DatosEmpresa.Colspan = Span;
            if (TipoAlineado == 1)
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_LEFT;
            if (TipoAlineado == 2)
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            if (TipoAlineado == 3)
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_RIGHT;
            tableEnc.AddCell(DatosEmpresa);
        }

        /// <summary>
        /// Metodo para pintar celdas en un PDF
        /// </summary>
        /// <param name="fontEncabezado">Tipo de encabezado</param>
        /// <param name="tableEnc">Tabla con número de celdas</param>
        /// <param name="NombreCampo">Nombre del campo a pintar</param>
        /// <param name="Span">Columnas a juntar</param>
        /// <param name="border">Con o sin borde</param>
        /// <param name="horizontal">Alineado horizontal</param>
        /// <param name="centrado">Centrado</param>
        public static void AgregarCelda(Font fontEncabezado, PdfPTable tableEnc, string NombreCampo, int Span, bool border, int horizontal, bool centrado = false)
        {
            PdfPCell DatosEmpresa = new PdfPCell(new Phrase(NombreCampo, fontEncabezado));
            if (border)
                DatosEmpresa.Border = Rectangle.NO_BORDER;
            if (horizontal == 2)
            {
                DatosEmpresa.BackgroundColor = BaseColor.LIGHT_GRAY;
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            }
            if (centrado)
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            DatosEmpresa.Colspan = Span;
            tableEnc.AddCell(DatosEmpresa);
        }
    }
    public class ITextEvents : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        /// <summary>
        /// Metodo para agregar una celda en un PDF
        /// </summary>
        /// <param name="fontEncabezado">Estilo encabezado</param>
        /// <param name="tableEnc">Tabla con número de celdas</param>
        /// <param name="NombreCampo">Nomber que se setea</param>
        /// <param name="Span">Colspam dependiendo el tamaño de la tabla</param>
        /// <param name="TipoAlineado">si es 1 left si es 2 center y si es 3 right</param>
        public static void AgregarCelda(Font fontEncabezado, PdfPTable tableEnc, string NombreCampo, int Span, int TipoAlineado)
        {
            PdfPCell DatosEmpresa = new PdfPCell(new Phrase(NombreCampo, fontEncabezado));

            DatosEmpresa.Border = Rectangle.NO_BORDER;
            DatosEmpresa.Colspan = Span;
            if (TipoAlineado == 1)
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_LEFT;
            DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            if (TipoAlineado == 3)
                DatosEmpresa.HorizontalAlignment = Element.ALIGN_RIGHT;
            tableEnc.AddCell(DatosEmpresa);
        }

        /// <summary>
        /// Metodo para pintar una linea en fondo
        /// </summary>
        /// <param name="fontEncabezado">Estilo encabezado</param>
        /// <param name="tableEnc">Tabla con número de celdas</param>
        public static void AgregarLinea(Font fontEncabezado, PdfPTable tableEnc)
        {
            PdfPCell DatosEmpresa = new PdfPCell(new Phrase("", fontEncabezado));
            DatosEmpresa.Border = Rectangle.BOTTOM_BORDER;
            tableEnc.AddCell(DatosEmpresa);
        }

        /// <summary>
        /// Metodo para crear un encabezado en un PDF
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="document"></param>
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(50, 50);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Metodo para agrgar pie de pagina a un PDF
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="document"></param>
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            iTextSharp.text.Font fontEncabezado = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font fontEncabezadoDoce = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            //Create PdfTable object
            PdfPTable table = new PdfPTable(10);
            PdfPCell DatosEmpresa;
            AgregarCelda(fontEncabezado, table, "REPORTE DE NOMINA", 2, 1);
            AgregarCelda(fontEncabezado, table, " Sin Incluir Trab.en Ceros", 5, 2);
            AgregarCelda(fontEncabezado, table, String.Format(" FECHA IMP.: {0}", DateTime.Now.ToString("dd/MM/yyyy - HHmmss")), 3, 3);

            //02 
            //AgregarCelda(fontEncabezado, table, ClassReportesPDF.RegistroP.NombrePatrona, 5, 1);
            //AgregarCelda(fontEncabezado, table, ClassReportesPDF.RegistroP.RFC, 5, 3);



            AgregarCelda(fontEncabezado, table, String.Format("Año", DateTime.Now.Year), 1, 3);
            AgregarCelda(fontEncabezado, table, "Periodo" + ClassReportesPDF.periodoStatic.FechaInicio + "AL" + ClassReportesPDF.periodoStatic.FechaFin, 5, 1);
            //AgregarCelda(fontEncabezado, table, "F. CORTE ADMIN.: 19/DIC/2020 AL: 04/ENE/2021", 4, 1);


            AgregarCelda(fontEncabezado, table, String.Format("REPORTE DE NOMINA GENERAL"), 10, 2);

            DatosEmpresa = new PdfPCell(new Phrase("    ", fontEncabezado));
            DatosEmpresa.Colspan = 10;
            DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            DatosEmpresa.BorderWidthBottom = 0;
            DatosEmpresa.BorderWidthLeft = 0;
            DatosEmpresa.BorderWidthTop = 0.1F;
            DatosEmpresa.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);





            //AgregarCelda(fontEncabezadoDoce, table, "Período:11:1:QUINCENAL -  NORMAL De: 01/06/2020 al 15/06/2020", 10, 2);
            //DatosEmpresa = new PdfPCell(new Phrase("Período: " + PensionesIsstePDF.periodoStatic.Periodo + " -  NORMAL De: " + PensionesIsstePDF.periodoStatic.FechaInicio.Value.ToString("DD/MM/yyyy") + " al " + PensionesIsstePDF.periodoStatic.FechaFin.Value.ToString("DD/MM/yyyy"), fontEncabezado));
            //DatosEmpresa.Colspan = 2;
            //DatosEmpresa.HorizontalAlignment = Element.ALIGN_LEFT;
            //DatosEmpresa.BorderWidthBottom = 0.1F;
            //DatosEmpresa.BorderWidthLeft = 0;
            //DatosEmpresa.BorderWidthTop = 0;
            //DatosEmpresa.BorderWidthRight = 0;
            //table.AddCell(DatosEmpresa);

            //DatosEmpresa = new PdfPCell(new Phrase("Período: " + PensionesIsstePDF.periodoStatic.Periodo + " -  NORMAL De: " + PensionesIsstePDF.periodoStatic.FechaInicio.Value.ToString("DD/MM/yyyy") + " al " + PensionesIsstePDF.periodoStatic.FechaFin.Value.ToString("DD/MM/yyyy"), fontEncabezado));
            //DatosEmpresa.Colspan = 8;
            //DatosEmpresa.HorizontalAlignment = Element.ALIGN_LEFT;
            //DatosEmpresa.BorderWidthBottom = 0.1F;
            //DatosEmpresa.BorderWidthLeft = 0.1F;
            //DatosEmpresa.BorderWidthTop = 0.1F;
            //DatosEmpresa.BorderWidthRight = 0.1F;
            //table.AddCell(DatosEmpresa);
            //Período:11:1:QUINCENAL -  NORMAL De: 01/06/2020 al 15/06/2020
            //AgregarCelda(fontEncabezado, table, "Período: " + periodo.Periodo + " -  NORMAL De: " + periodo.FechaInicio + " al " + periodo.FechaFin, 10, 2);
            //Agregar Linea correda
            //Linea 5 
            AgregarCelda(fontEncabezado, table, "Número", 1, 1);
            AgregarCelda(fontEncabezado, table, "RFC", 2, 1);
            AgregarCelda(fontEncabezado, table, "Afiliación", 1, 1);
            AgregarCelda(fontEncabezado, table, "CURP", 2, 1);

            AgregarCelda(fontEncabezado, table, "Nombre", 3, 1);
            AgregarCelda(fontEncabezado, table, "FechaIngreso", 1, 1);

            //Departamento Puesto Salario Diario Salario Diario Integrado
            AgregarCelda(fontEncabezado, table, "Departamento", 3, 1);
            AgregarCelda(fontEncabezado, table, "Puesto", 3, 1);
            AgregarCelda(fontEncabezado, table, "Salario Diario", 2, 1);
            AgregarCelda(fontEncabezado, table, "Salario Diario Integrado", 2, 1);


            //    IMSS Nomina 
            AgregarCelda(fontEncabezado, table, "Tipo Sal", 2, 1);
            AgregarCelda(fontEncabezado, table, "IMSS SAR", 1, 1);
            AgregarCelda(fontEncabezado, table, "Infonavit", 1, 1);
            AgregarCelda(fontEncabezado, table, "IMSS Nomina ", 6, 1);
            DatosEmpresa = new PdfPCell(new Phrase("    ", fontEncabezado));
            DatosEmpresa.Colspan = 10;
            DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            DatosEmpresa.BorderWidthBottom = 0;
            DatosEmpresa.BorderWidthLeft = 0;
            DatosEmpresa.BorderWidthTop = 0.1F;
            DatosEmpresa.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);






            //Departamento Puesto Salario Diario Salario Diario Integrado

            DatosEmpresa = new PdfPCell(new Phrase("    ", fontEncabezado));
            DatosEmpresa.Colspan = 10;
            DatosEmpresa.HorizontalAlignment = Element.ALIGN_CENTER;
            DatosEmpresa.BorderWidthBottom = 0;
            DatosEmpresa.BorderWidthLeft = 0;
            DatosEmpresa.BorderWidthTop = 0.1F;
            DatosEmpresa.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);




            //Add paging to header
            {
                //cb.BeginText();
                //cb.SetFontAndSize(bf, 12);
                //cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(45));
                //cb.EndText();
                //Adds "12" in Page 1 of 12
                cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200), document.PageSize.GetTop(45));
            }


            table.TotalWidth = document.PageSize.Width - 50f;
            table.WidthPercentage = 100;
            //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;    

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            table.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            //set pdfContent value

            //Move the pointer and draw line to separate header section from rest of page
            //cb.MoveTo(40, document.PageSize.Height - 100);
            //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
            //cb.Stroke();

            //cb.MoveTo(10, document.PageSize.Height - 50);
            //cb.LineTo(document.PageSize.Width - 10, document.PageSize.Height - 50);
            //cb.Stroke();
        }
    }


}
