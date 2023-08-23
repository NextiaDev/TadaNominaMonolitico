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
            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.A4, 10f, 10f, 140f, 10f);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc, new System.IO.FileStream(ruta, System.IO.FileMode.Create));

            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 8);

            // Para titulos pequeños
            BaseFont bfTimesTitulo = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTitulo = new Font(bfTimesTitulo, 7);
            //fontEncabezado.Color = BaseColor.GRAY;
            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesCuerpo, 7);


            //writer.PageEvent = new HeaderFooter();
            writer.PageEvent = new ITextEvents();
            // Abrimos el archivo
            doc.Open();

            //for (int i = 0; i < 100; i++)
            //{
            //    Paragraph para = new Paragraph("Hello world. Checking Header Footer", new Font(Font.FontFamily.HELVETICA, 22));
            //    para.Alignment = Element.ALIGN_CENTER;
            //    doc.Add(para);
            //    doc.NewPage();
            //}


            PdfPTable table = new PdfPTable(10);
            table.WidthPercentage = 100f;

            string[] valor = { "Tradicional", "Mixto" };



            string puesto = String.Empty;
            int primerPuesto = 1;
            foreach (var item in datos)
            {



                var _ins = incidencias.Where(x => x.IdEmpleado == item.IdEmpleado && x.Monto > 0 && x.IdEstatus == 1 && valor.Contains(x.TipoEsquema)).ToList();

                if (primerPuesto == 1)
                {
                    puesto = item.Puesto;
                    AgregarCelda(fontEncabezado, table, puesto, 10, 1);
                    primerPuesto = 0;
                }
                else
                {
                    if (!(puesto ?? "").Equals(item.Puesto.ToString()))
                    {
                        AgregarCelda(fontEncabezado, table, puesto, 10, 1);
                        puesto = item.Puesto.ToString();
                    }
                }

                AgregarCelda(fontEncabezado, table, " * RAZON SOCIAL : " + item.NombrePatrona, 7, 1);
                AgregarCelda(fontEncabezado, table, " R.F.C. : " + item.RFCPATRONA, 3, 1);



                AgregarCelda(fontEncabezado, table, item.ClaveEmpleado.ToString().PadLeft(8, '0'), 1, 1);
                AgregarCelda(fontEncabezado, table, item.Rfc.ToString(), 2, 1);
                AgregarCelda(fontEncabezado, table, item.Imss, 1, 1);
                AgregarCelda(fontEncabezado, table, item.Curp.ToString(), 2, 1);
                AgregarCelda(fontEncabezado, table, item.Nombre.ToString(), 3, 1);
                AgregarCelda(fontEncabezado, table, item.FechaAltaIMSS.Value.ToShortDateString(), 1, 1);
                string depto = " ";
                if (item.Departamento != null) { depto = item.Departamento; }
                AgregarCelda(fontEncabezado, table, depto, 2, 1);
                AgregarCelda(fontEncabezado, table, item.Puesto ?? "", 3, 1);
                AgregarCelda(fontEncabezado, table, item.sueldoDiario.ToString(), 3, 1);
                AgregarCelda(fontEncabezado, table, item.SDI.ToString(), 2, 1);

                AgregarCelda(fontEncabezado, table, " -  Percepciones y Deducciones ", 10, 1);

                PdfPTable tablePercep = new PdfPTable(5);
                tablePercep.WidthPercentage = 100f;

                //Percepciones
                string fechaDispersion = " ";

                //
                if (periodo.FechaDispersion != null)
                {
                    fechaDispersion = periodo.FechaDispersion.Value.ToShortDateString();
                    AgregarCelda(fontEncabezado, tablePercep, "400 SUELDO", 3, 1);
                    AgregarCelda(fontEncabezado, tablePercep, fechaDispersion, 1, 1); //fecha de pago
                    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", item.SueldoPagado), 1, 1);
                }
                else
                {
                    AgregarCelda(fontEncabezado, tablePercep, "400 SUELDO", 4, 1);
                    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", item.SueldoPagado), 1, 1);
                }

                if (periodo.FechaDispersion != null)
                {
                    AgregarCelda(fontEncabezado, tablePercep, "402 SUBSIDIO EMPLEO PAGADO", 3, 1);
                    AgregarCelda(fontEncabezado, tablePercep, fechaDispersion, 1, 1);
                    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", item.SubsidioPagar), 1, 1);
                }
                else
                {
                    AgregarCelda(fontEncabezado, tablePercep, "402 SUBSIDIO EMPLEO PAGADO", 4, 1);
                    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", item.SubsidioPagar), 1, 1);
                }


                //fecha de pago



                var insper = _ins.Where(x => x.TipoConcepto == "ER" && x.Monto > 0).ToList();
                foreach (var ins in insper)
                {
                    //if (ins.Cantidad > 0)
                    //{
                    //    AgregarCelda(fontEncabezado, tablePercep, ins.ClaveConcepto + " " + ins.Concepto, 4, 1);//clave concepto
                    //    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", ins.Cantidad), 1, 1); //revisar como lo mandan(Concepto)
                    //}
                    //else
                    //{

                    AgregarCelda(fontEncabezado, tablePercep, ins.ClaveConcepto + " " + ins.Concepto, 4, 1);//clave concepto
                    AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", ins.Monto), 1, 1); //revis
                    //}

                }

                AgregarCelda(fontEncabezado, tablePercep, "Percepciones: ", 4, 3);
                AgregarCelda(fontEncabezado, tablePercep, string.Format("{0:C2}", item.ER), 1, 1);

                PdfPTable tableDeduc = new PdfPTable(5);
                tableDeduc.WidthPercentage = 100f;
                //For de Deducciones.
                AgregarCelda(fontEncabezado, tableDeduc, "601 IMSS", 4, 1);
                AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", item.IMSS_Obrero), 1, 1);
                AgregarCelda(fontEncabezado, tableDeduc, " ISR", 4, 1);
                AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", item.ISR), 1, 1);


                string[] valord = { "Tradicional", "Mixto" };
                var insdeduc = _ins.Where(x => x.TipoConcepto == "DD" && x.Monto >= 0).ToList();
                foreach (var ins in insdeduc)
                {
                    //if (ins.Cantidad > 0)
                    //{
                    //    AgregarCelda(fontEncabezado, tableDeduc, ins.ClaveConcepto + " " + ins.Concepto, 4, 1);//clave concepto
                    //    AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", ins.Cantidad), 1, 1); //rev

                    //}
                    //else
                    //{
                    AgregarCelda(fontEncabezado, tableDeduc, ins.ClaveConcepto + " " + ins.Concepto, 4, 1);//clave concepto
                    AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", ins.Monto), 1, 1); //revis



                }

                AgregarCelda(fontEncabezado, tableDeduc, "Deducciones: ", 4, 3);
                AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", item.DD).ToString(), 1, 1);
                AgregarCelda(fontEncabezado, tableDeduc, "Total de Pago: ", 4, 3);
                AgregarCelda(fontEncabezado, tableDeduc, string.Format("{0:C2}", item.Neto), 1, 1);


                PdfPCell columas = new PdfPCell(tablePercep);
                columas.Colspan = 5;
                columas.Padding = 0f;
                columas.Border = 0;
                table.AddCell(columas);

                columas = new PdfPCell(tableDeduc);
                columas.Colspan = 5;
                columas.Padding = 0f;
                columas.Border = 0;
                table.AddCell(columas);
            }

            //Final de PDF
            //            Totales Generales por Conceptos Concepto Descripción                      Dato Percepción             Deducción Gravados              Exento ________________________________________________________________________________________________________________________________ 400 SUELDO 1800.0 $357,500.00 $357,500.00 $0.00 402 SUBSIDIO EMPLEO PAGA 0.00 $8,330.24 $0.00 $0.00 601 IMSS 0.00 $10,911.05 $0.00 $0.00 602 IMPUESTO RETENIDO 0.00 $23,639.84 $0.00 $0.00 605 FALTA 3.00 $399.00 -$399.00 $0.00 607 INCAP ACC TRAYECTO 15.00 $2,000.00 -$2,000.00 $0.00 612 CREDITO INFONAVIT 0.00 $40,163.79 $0.00 $0.00 630 CRED FONACOT 1 0.00 $1,668.65 $0.00 $0.00
            //Total de Percepciones: $365,830.24 Total de Deducciones: $78,782.33 <<<<<< Total Neto General: >>>>>> $287,047.91 $355,101.00 $0.00 <<<<<< Total de Trabajadores: >>>>>> 120
            PdfPCell DatosEmpresa = new PdfPCell(new Phrase("", fontEncabezado));
            DatosEmpresa.Colspan = 8;
            DatosEmpresa.BorderWidthBottom = 0;
            DatosEmpresa.BorderWidthLeft = 0;
            DatosEmpresa.BorderWidthTop = 0.1F;
            DatosEmpresa.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);
            DatosEmpresa = new PdfPCell(new Phrase("", fontEncabezado));
            DatosEmpresa.Colspan = 8;
            DatosEmpresa.BorderWidthBottom = 0;
            DatosEmpresa.BorderWidthLeft = 0;
            DatosEmpresa.BorderWidthTop = 0.1F;
            DatosEmpresa.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);


            PdfPTable totales = new PdfPTable(7);
            totales.WidthPercentage = 100f;
            AgregarCelda(fontEncabezado, totales, " Totales Por Centro de Trabajo ", 10, 2);
            //AgregarCelda(fontEncabezado, totales, "1 : CENTRO PENSION ISSSTE", 10, 1);

            AgregarCelda(fontEncabezado, totales, "Concepto", 1, 2);
            AgregarCelda(fontEncabezado, totales, "Descripción", 1, 2);
            AgregarCelda(fontEncabezado, totales, "Dato", 1, 2);
            AgregarCelda(fontEncabezado, totales, "Percepción", 1, 2);
            AgregarCelda(fontEncabezado, totales, "Deducción", 1, 2);
            AgregarCelda(fontEncabezado, totales, "Gravados", 1, 2);
            AgregarCelda(fontEncabezado, totales, "Exento ", 1, 2);

            //FIJO SUELDO
            AgregarCelda(fontEncabezado, totales, "400", 1, 1);
            AgregarCelda(fontEncabezado, totales, "Sueldo", 1, 1);
            AgregarCelda(fontEncabezado, totales, "", 1, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.SueldoPagado).ToString(), 1, 1);
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.SueldoPagado).ToString(), 1, 1); //Gravados
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)

            //FIJO SUBSIDIO EMPLEO PAGA 
            AgregarCelda(fontEncabezado, totales, "402", 1, 1);
            AgregarCelda(fontEncabezado, totales, "SUBSIDIO EMPLEO PAGA", 1, 1);
            AgregarCelda(fontEncabezado, totales, "0.00", 1, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.SubsidioPagar).ToString(), 1, 1);
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1); //Gravados
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)

            AgregarCelda(fontEncabezado, totales, "602", 1, 1);
            AgregarCelda(fontEncabezado, totales, "Impuesto Retenido", 1, 1);
            AgregarCelda(fontEncabezado, totales, "0.00", 1, 1);
            AgregarCelda(fontEncabezado, totales, "0.00", 1, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.ImpuestoRetener).ToString(), 1, 1);
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)


            AgregarCelda(fontEncabezado, totales, "601", 1, 1);
            AgregarCelda(fontEncabezado, totales, "IMSS", 1, 1);
            AgregarCelda(fontEncabezado, totales, "0.00", 1, 1);
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)    
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.IMSS_Obrero).ToString(), 1, 1);
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)            
            AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);//EXENTo de la primera fila(SUELDO)

            //Aqui va incidencias
            var listaClaveConcepto = (from a in incidencias
                                      where a.TipoConcepto == "ER" || a.TipoConcepto == "DD"
                                      group a by new
                                      {
                                          a.ClaveConcepto,
                                          a.Concepto,
                                          a.TipoConcepto

                                      } into b
                                      select new { b.Key.ClaveConcepto, b.Key.Concepto, b.Key.TipoConcepto }).ToList();

            foreach (var clave in listaClaveConcepto)
            {

                var Cantidad = incidencias.Where(n => n.ClaveConcepto == clave.ClaveConcepto && n.TipoConcepto == clave.TipoConcepto).Sum(m => m.Cantidad);
                var Monto = incidencias.Where(n => n.ClaveConcepto == clave.ClaveConcepto && n.TipoConcepto == clave.TipoConcepto).Sum(m => m.Monto);
                var ER = incidencias.Where(n => n.ClaveConcepto == clave.ClaveConcepto && n.TipoConcepto == clave.TipoConcepto).Sum(m => m.Monto);
                var DD = incidencias.Where(n => n.ClaveConcepto == clave.ClaveConcepto && n.TipoConcepto == clave.TipoConcepto).Sum(m => m.Monto);
                var Gravado = incidencias.Where(n => n.ClaveConcepto == clave.ClaveConcepto && n.TipoConcepto == clave.TipoConcepto).Sum(m => m.Gravado);
                var Excento = incidencias.Where(n => n.ClaveConcepto == clave.ClaveConcepto && n.TipoConcepto == clave.TipoConcepto).Sum(m => m.Exento);








                if (clave.TipoConcepto == "ER")
                {
                    AgregarCelda(fontEncabezado, totales, clave.ClaveConcepto, 1, 1);
                    AgregarCelda(fontEncabezado, totales, clave.Concepto, 1, 1);
                    AgregarCelda(fontEncabezado, totales, "0.00", 1, 1);
                    AgregarCelda(fontEncabezado, totales, ER == 0 ? "$0.00" : string.Format("{0:C2}", ER), 1, 1);
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                }

                else if (clave.TipoConcepto == "DD")
                {
                    AgregarCelda(fontEncabezado, totales, clave.ClaveConcepto, 1, 1);
                    AgregarCelda(fontEncabezado, totales, clave.Concepto, 1, 1);
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                    AgregarCelda(fontEncabezado, totales, DD == 0 ? "$0.00" : string.Format("{0:C2}", DD), 1, 1);
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);

                }


                if (Gravado > 0 && Excento > 0)
                {
                    AgregarCelda(fontEncabezado, totales, Gravado == 0 ? "$0.00" : string.Format("{0:C2}", Gravado), 1, 1);
                    AgregarCelda(fontEncabezado, totales, Excento == 0 ? "$0.00" : string.Format("{0:C2}", Excento), 1, 1);

                }
                else if (Gravado > 0 && Excento == 0)
                {

                    AgregarCelda(fontEncabezado, totales, Gravado == 0 ? "$0.00" : string.Format("{0:C2}", Gravado), 1, 1);
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                }
                else if (Gravado == 0 && Excento > 0)
                {
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                    AgregarCelda(fontEncabezado, totales, Excento == 0 ? "$0.00" : string.Format("{0:C2}", Excento), 1, 1);

                }
                else
                {
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);

                }


                //if (PercepcionIncidencia > 0)
                //{
                //    AgregarCelda(fontEncabezado, totales, string.Format("{0:C2}", PercepcionIncidencia), 1, 1);
                //}
                //else
                //{
                //    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                //}
                //if (DeduccionIncidencia > 0)
                //{
                //    AgregarCelda(fontEncabezado, totales, string.Format("{0:C2}", DeduccionIncidencia), 1, 1);
                //}
                //else
                //{
                //    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                //}

                //if (ExcentoIncidencia > 0)
                //{
                //    AgregarCelda(fontEncabezado, totales, string.Format("{0:C2}", ExcentoIncidencia), 1, 1);
                //}
                //else
                //{
                //    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                //}

                //if (ExcentoIncidencia > 0)
                //{
                //    AgregarCelda(fontEncabezado, totales, string.Format("{0:C2}", ExcentoIncidencia), 1, 1);
                //}
                //else
                //{
                //    AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                //}
                //AgregarCelda(fontEncabezado, totales, "$0.00", 1, 1);
                //AgregarCelda(fontEncabezado, totales, CantidadIncidenciaER == null ? "$0.00" : string.Format("{0:C2}", CantidadIncidenciaER), 1, 1);

            }

            PdfPCell DatosEmpresauno = new PdfPCell(new Phrase("", fontEncabezado));
            DatosEmpresauno.Colspan = 8;
            DatosEmpresauno.BorderWidthBottom = 0;
            DatosEmpresauno.BorderWidthLeft = 0;
            DatosEmpresauno.BorderWidthTop = 0.1F;
            DatosEmpresauno.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);
            DatosEmpresauno = new PdfPCell(new Phrase("", fontEncabezado));
            DatosEmpresauno.Colspan = 8;
            DatosEmpresauno.BorderWidthBottom = 0;
            DatosEmpresauno.BorderWidthLeft = 0;
            DatosEmpresauno.BorderWidthTop = 0.1F;
            DatosEmpresauno.BorderWidthRight = 0;
            table.AddCell(DatosEmpresa);



            //TOTALES GENERALES
            AgregarCelda(fontEncabezado, totales, "Total de Percepciones:", 1, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.ER).ToString(), 6, 1);

            AgregarCelda(fontEncabezado, totales, "Total de Deducciones:", 1, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.DD).ToString(), 6, 1);
            // <<<<<< Total Neto General: >>>>>> 
            AgregarCelda(fontEncabezado, totales, "<<<<<< Total Neto General: >>>>>> ", 6, 1);
            AgregarCelda(fontEncabezado, totales, datos.Sum(x => x.Neto).ToString(), 1, 1);

            AgregarCelda(fontEncabezado, totales, " <<<<<< Total de Trabajadores: >>>>>>  ", 6, 1);
            AgregarCelda(fontEncabezado, totales, datos.Count().ToString(), 1, 1);



            PdfPCell colum = new PdfPCell(totales);
            colum.Colspan = 10;
            colum.Padding = 0f;
            colum.Border = 0;
            table.AddCell(colum);

            doc.Add(table);
            doc.NewPage();
            //FINALIZAMOS EL DOCUEMNTO
            doc.Close();

            writer.Close();

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
