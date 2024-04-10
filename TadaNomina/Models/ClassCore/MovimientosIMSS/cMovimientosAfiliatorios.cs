using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cMovimientosAfiliatorios
    {
        public List<mMovimientosAfiliatorios> GetMovimientosByImss(string IMSS, int IdCliente)
        {
            List< mMovimientosAfiliatorios> lista = new List<mMovimientosAfiliatorios>();
            var rp = GetNombrePatronas(IdCliente);
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = (from a in ctx.v_IMSS_MovimientosCESS
                                  where a.Imss == IMSS
                                   && rp.Contains(a.RegistroPatronal)
                                  select new
                                  {
                                      Lote = a.Lote,
                                      NombrePatrona = a.NombrePatrona,
                                      ActividadEconomica = a.ActividadEconomica,
                                      ApellidoPaterno = a.ApellidoPaterno,
                                      ApellidoMaterno = a.ApellidoMaterno,
                                      Nombre = a.Nombre,
                                      Imss = a.Imss,
                                      FechaMovimiento = a.FechaMovimiento,
                                      Movimiento = a.Movimiento,
                                      FechaEnvio = a.FechaTransmision,
                                      Origen = a.Origen
                                  }).ToList();
                query.ForEach(p =>
                {
                    mMovimientosAfiliatorios mma = new mMovimientosAfiliatorios();
                    mma.Lote = p.Lote.ToString();
                    mma.NombrePatrona = p.NombrePatrona;
                    mma.ActividadEconomica = p.ActividadEconomica;
                    mma.ApellidoMaterno = p.ApellidoMaterno;
                    mma.ApellidoPaterno = p.ApellidoPaterno;
                    mma.Nombre = p.Nombre;
                    mma.Imss = p.Imss;
                    mma.FechaMovimiento = p.FechaMovimiento.ToString().Substring(0, 10);
                    mma.TipoMovimiento = p.Movimiento;
                    mma.FechaEnvio = p.FechaEnvio.ToString().Substring(0, 10);
                    mma.Origen = p.Origen;
                    lista.Add(mma);
                });
                return lista;
            }
        }

        public int GetIdRP(string RegistroPatronal)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.RegistroPatronal == RegistroPatronal && p.IdEstatus == 1).First();
                return query.IdRegistroPatronal;
            }
        }

        public List<string> GetNombrePatronas(int IdCliente)
        {
            List<string> listado = new List<string>();
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.IdCliente == IdCliente).ToList();
                foreach(var item in query)
                {
                    var nom = item.RegistroPatronal;
                    listado.Add(nom);
                }
                return listado;
            }
        }

        public v_IMSS_MovimientosCESS GetInfoAfilIndividual(string lote, string imss)
        {
            cConsultas cc = new cConsultas();
            double? L = double.Parse(lote.ToString());
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.v_IMSS_MovimientosCESS.Where(p => p.Lote == L && p.Imss == imss).First();
                if (query.Origen == "TADA")
                {
                    string Reg = query.RegistroPatronal;
                    int IdReg = GetIdByRegPatronal(Reg);
                    var respuesta = cc.GetRespuestaDetalleLote(IdReg, query.Lote.ToString());
                    foreach (var item in respuesta.respuestaWebService.movimientosLote)
                    {
                        if (item.nss == query.Imss)
                        {
                            string sdi = item.salarioBase;
                            query.SDI = double.Parse(sdi);
                        }
                    }
                }
                return query;
            }
        }

        public int GetIdByRegPatronal(string RP)
        {
            int id = 0;
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.RegistroPatronal == RP && p.IdEstatus==1).First();
                id = query.IdRegistroPatronal;
                return id;
            }
        }

        public byte[] GetPdfAlta(v_IMSS_MovimientosCESS model)
        {
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 8);

            // Para titulos pequeños
            BaseFont bfTimesTituloSub = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTituloSub = new Font(bfTimesTituloSub, 11, Font.UNDERLINE);

            // Para titulos pequeños
            BaseFont bfTimesTitulo = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTitulo = new Font(bfTimesTitulo, 11);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesCuerpo, 11);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpoSub = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpoSub = new Font(bfTimesCuerpoSub, 11, Font.UNDERLINE);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpoLineas = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpoLineas = new Font(bfTimesCuerpoLineas, 18);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            // Abrimos el archivo
            doc.Open();

            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = 500f;
            table.LockedWidth = true;

            PdfPCell Cell = new PdfPCell(new Phrase("AFIL IDSE", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            PdfPCell espacioCell = new PdfPCell(new Phrase(" "));
            espacioCell.Colspan = 10;
            espacioCell.Border = 0;
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("IMSS DESDE SU EMPRESA", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Acuse Notarial de confirmación de procesamiento de movimientos afiliatorios de la empresa" + " " + model.NombrePatrona, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Lote número:" + "                                       " + model.Lote, fontCuerpo));  //39 espacios
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Razón social:" + "                                       " + model.NombrePatrona, fontCuerpo)); //39 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("RFC:" + "                                                    " + model.RFC_Patrona, fontCuerpo));  //52 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Denominación del Tramite:" + "                 " + "AFILIACION CONSULTA DE LOTES PROCESADOS", fontCuerpo));   //17 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Actividad Económica:" + "                         " + model.ActividadEconomica, fontCuerpo));    //25 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Registro Patronal:" + "                                " + model.RegistroPatronal, fontCuerpo));    //32 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Fecha de Transacción:" + "                         " + model.FechaMovimiento.ToString().Substring(0, 10), fontCuerpo));   //25 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("PATRON" + " " + model.RegistroPatronal, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(model.NombrePatrona, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Fecha de Transacción:" + " " + model.FechaMovimiento.ToString().Substring(0, 10), fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("AFIL06:" + " " + "RELACION DE MOVIMIENTOS OPERADOS", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("NSS:" + "          " + model.Imss, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("NOMBRE DEL ASEGURADO:" + "          " + model.Nombre + " " + model.ApellidoPaterno + " " + model.ApellidoMaterno, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(" ", fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("MOVIMIENTO" + "     SAL BASE" + "    ID-EX" + "     UMF" + "       SJR" + "                       FECHA MOV" + "          TIPO TRAB", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("        " + model.TipoMovimiento + "                 " + model.SDI.ToString() + "            " + "0" + "               " + "0" + "              " + model.TipoSemana + "     " + model.FechaMovimiento.ToString().Substring(0, 10) + "                 " + "Permanente", fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //agrega las tablas al documento
            doc.Add(espacio);
            doc.Add(table);

            doc.NewPage();
            doc.Close();
            writer.Close();

            //regresamos el formato
            return ms.ToArray();
        }

        public byte[] GetPdfBaja(v_IMSS_MovimientosCESS model)
        {
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 8);

            // Para titulos pequeños
            BaseFont bfTimesTituloSub = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTituloSub = new Font(bfTimesTituloSub, 11, Font.UNDERLINE);

            // Para titulos pequeños
            BaseFont bfTimesTitulo = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTitulo = new Font(bfTimesTitulo, 11);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesCuerpo, 11);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpoSub = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpoSub = new Font(bfTimesCuerpoSub, 11, Font.UNDERLINE);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpoLineas = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpoLineas = new Font(bfTimesCuerpoLineas, 18);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            //Abrimos el archivo
            doc.Open();

            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = 500f;
            table.LockedWidth = true;

            PdfPCell Cell = new PdfPCell(new Phrase("AFIL IDSE 03", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            PdfPCell espacioCell = new PdfPCell(new Phrase(" "));
            espacioCell.Colspan = 10;
            espacioCell.Border = 0;
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("IMSS DESDE SU EMPRESA", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Acuse Notarial de confirmación de procesamiento de movimientos afiliatorios de la empresa" + " " + model.NombrePatrona, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Lote número:" + "                                       " + model.Lote, fontCuerpo));  //39 espacios
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Razón social:" + "                                       " + model.NombrePatrona, fontCuerpo)); //39 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("RFC:" + "                                                    " + model.RFC_Patrona, fontCuerpo));  //52 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Denominación del Tramite:" + "                 " + "BAJA CONSULTA DE LOTES PROCESADOS", fontCuerpo));   //17 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Actividad Económica:" + "                         " + model.ActividadEconomica, fontCuerpo));    //25 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Registro Patronal:" + "                                " + model.RegistroPatronal, fontCuerpo));    //32 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Fecha de Transacción:" + "                         " + model.FechaTransmision.ToString().Substring(0, 10), fontCuerpo));   //25 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(model.NombrePatrona, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Fecha de Transacción:" + " " + model.FechaTransmision.ToString().Substring(0, 10), fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("AFIL06:" + " " + "RELACION DE MOVIMIENTOS OPERADOS", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("NSS:" + "          " + model.Imss, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("NOMBRE DEL ASEGURADO:" + "          " + model.Nombre + " " + model.ApellidoPaterno + " " + model.ApellidoMaterno, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(" ", fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("MOVIMIENTO" + "    ID-EX" + "     UMF" + "       SJR" + "                       FECHA MOV" + "          TIPO TRAB", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(model.Movimiento + "                         " + "0" + "                  " + "0" + "             " + model.TipoSemana + "       " + model.FechaMovimiento.ToString().Substring(0, 10) + "                     " + "N/A", fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Causa de la baja:" + "          " + model.CausaBaja, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega las tablas al documento
            doc.Add(espacio);
            doc.Add(table);

            doc.NewPage();
            doc.Close();

            writer.Close();

            return ms.ToArray();
        }

        public byte[] GetPdfModificacion(v_IMSS_MovimientosCESS model)
        {
            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 8);

            // Para titulos pequeños
            BaseFont bfTimesTituloSub = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTituloSub = new Font(bfTimesTituloSub, 11, Font.UNDERLINE);

            // Para titulos pequeños
            BaseFont bfTimesTitulo = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            Font fontTitulo = new Font(bfTimesTitulo, 11);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesCuerpo, 11);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpoSub = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpoSub = new Font(bfTimesCuerpoSub, 11, Font.UNDERLINE);

            // Para el cuerpo del recibo
            BaseFont bfTimesCuerpoLineas = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpoLineas = new Font(bfTimesCuerpoLineas, 18);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            // Abrimos el archivo
            doc.Open();

            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = 500f;
            table.LockedWidth = true;

            PdfPCell Cell = new PdfPCell(new Phrase("AFIL IDSE 03", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            PdfPCell espacioCell = new PdfPCell(new Phrase(" "));
            espacioCell.Colspan = 10;
            espacioCell.Border = 0;
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("IMSS DESDE SU EMPRESA", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Acuse Notarial de confirmación de procesamiento de movimientos afiliatorios de la empresa" + " " + model.NombrePatrona, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Lote número:" + "                                       " + model.Lote, fontCuerpo));  //39 espacios
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Razón social:" + "                                       " + model.NombrePatrona, fontCuerpo)); //39 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("RFC:" + "                                                    " + model.RFC_Patrona, fontCuerpo));  //52 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Denominación del Tramite:" + "                 " + "MODIFICACION CONSULTA DE LOTES PROCESADOS", fontCuerpo));   //17 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Actividad Económica:" + "                         " + model.ActividadEconomica, fontCuerpo));    //25 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Registro Patronal:" + "                                " + model.RegistroPatronal, fontCuerpo));    //32 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("Fecha de Transacción:" + "                         " + model.FechaTransmision.ToString().Substring(0, 10), fontCuerpo));   //25 esp
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("PATRON" + " " + model.RegistroPatronal, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(model.NombrePatrona, fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("Fecha de Transacción:" + " " + model.FechaTransmision.ToString().Substring(0, 10), fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("AFIL06:" + " " + "RELACION DE MOVIMIENTOS OPERADOS", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("NSS:" + "          " + model.Imss, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase("NOMBRE DEL ASEGURADO:" + "          " + model.Nombre + " " + model.ApellidoPaterno + " " + model.ApellidoMaterno, fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(" ", fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            table.AddCell(Cell);

            Cell = new PdfPCell(new Phrase("MOVIMIENTO" + "     SAL BASE" + "    ID-EX" + "     UMF" + "      SJR" + "                      FECHA MOV" + "          TIPO TRAB", fontTitulo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //Agrega una línea en vacía
            table.AddCell(espacioCell);

            Cell = new PdfPCell(new Phrase(model.Movimiento + "            " + model.SDI.ToString() + "            " + "0" + "               " + "0" + "         " + model.TipoSemana + "     " + model.FechaMovimiento.ToString().Substring(0, 10) + "                 " + "N/A", fontCuerpo));
            Cell.Colspan = 10;
            Cell.Border = 0;
            Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(Cell);

            //agrega las tablas al documento
            doc.Add(espacio);
            doc.Add(table);

            doc.NewPage();
            doc.Close();
            writer.Close();

            //regresamos el formato
            return ms.ToArray();
        }

        public List<string> GetListalotes(string imss, int IdCliente)
        {
            List<string> list = new List<string>();
            var lisatado = GetMovimientosByImss(imss, IdCliente);
            foreach (var item in lisatado)
             {
                switch (item.Origen)
                {
                    case "TADA":
                        string path = @"D:\SistemaTada\LotesIMSS\Nuevos\" + item.Lote + ".zip";
                        if(list.Contains(path))
                        {
                            list.Remove(path);
                        }
                        list.Add(path);
                        break;
                    case "HISTORICO":
                        string path2 = @"D:\SistemaTada\LotesIMSS\Historial\" + item.Lote + ".pdf";
                        if (list.Contains(path2))
                        {
                            list.Remove(path2);
                        }
                        list.Add(path2);
                        break;
                }
            }
            return list;
        }

        public string GetZip(List<string> lista, string imss)
        {
            string mensaje = string.Empty;
            string rutaZip = @"D:\SistemaTada\LotesIMSS\" + imss + ".zip";
            if (Directory.Exists(rutaZip))
            {
                Directory.Delete(rutaZip, true);
            }

            if (lista.Count > 0)
            {
                mensaje = CreateZip(lista, rutaZip);
            }
            return mensaje;
        }

        public string CreateZip(List<string> lista, string destino)
        {
            try
            {
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    foreach (var item in lista)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            zip.AddFile(item, "");
                        }
                        else if (System.IO.Directory.Exists(item))
                        {
                            zip.AddDirectory(item, new System.IO.DirectoryInfo(item).Name);
                        }
                    }
                    zip.Save(destino);
                }
                return string.Empty;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreatePDFs(string Imss)
        {
            var listado = GetInfoToPDF(Imss);
            string path = @"D:\SistemaTada\LotesIMSS\AfilesIndividuales" + Imss;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }

            foreach (var item in listado)
            {
                string ruta = path + "\\" + item.Lote + ".pdf";
                switch (item.Movimiento)
                {
                    case "Reingreso":
                        byte[] archivoA = GetPdfAlta(item);
                        if (!File.Exists(ruta))
                        {
                            File.WriteAllBytes(ruta, archivoA);
                        }
                        else
                        {
                            File.Delete(ruta);
                            File.WriteAllBytes(ruta, archivoA);
                        }
                        break;
                    case "Baja":
                        byte[] archivoB = GetPdfBaja(item);
                        if (!File.Exists(ruta))
                        {
                            File.WriteAllBytes(ruta, archivoB);
                        }
                        else
                        {
                            File.Delete(ruta);
                            File.WriteAllBytes(ruta, archivoB);
                        }
                        break;
                    case "Modificación":
                        byte[] archivoM = GetPdfModificacion(item);
                        if (!File.Exists(ruta))
                        {
                            File.WriteAllBytes(ruta, archivoM);
                        }
                        else
                        {
                            File.Delete(ruta);
                            File.WriteAllBytes(ruta, archivoM);
                        }
                        break;
                }
            }
            return path;
        }

        public List<v_IMSS_MovimientosCESS> GetInfoToPDF(string IMSS)
        {
            cConsultas cc = new cConsultas();
            List<v_IMSS_MovimientosCESS> listado = new List<v_IMSS_MovimientosCESS>();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = (from a in ctx.v_IMSS_MovimientosCESS
                             where a.Imss == IMSS
                             select new
                             {
                                 Lote = a.Lote,
                                 NombrePatrona = a.NombrePatrona,
                                 RFC_Patrona = a.RFC_Patrona,
                                 RegistroPatronal = a.RegistroPatronal,
                                 ActividadEconomica = a.ActividadEconomica,
                                 ApellidoPaterno = a.ApellidoPaterno,
                                 ApellidoMaterno = a.ApellidoMaterno,
                                 Nombre = a.Nombre,
                                 SDI = a.SDI,
                                 Imss = a.Imss,
                                 TipoSemana = a.TipoSemana,
                                 FechaMovimiento = a.FechaMovimiento,
                                 FechaTransmision = a.FechaTransmision,
                                 TipoMovimiento = a.TipoMovimiento,
                                 Movimiento = a.Movimiento,
                                 CausaBaja = a.CausaBaja,
                                 Origen = a.Origen
                             }).ToList();
                foreach (var item in query)
                {
                    v_IMSS_MovimientosCESS model = new v_IMSS_MovimientosCESS();
                    model.Lote = item.Lote;
                    model.NombrePatrona = item.NombrePatrona;
                    model.RFC_Patrona = item.RFC_Patrona;
                    model.RegistroPatronal = item.RegistroPatronal;
                    model.ActividadEconomica = item.ActividadEconomica;
                    model.ApellidoPaterno = item.ApellidoPaterno;
                    model.ApellidoMaterno = item.ApellidoMaterno;
                    model.Nombre = item.Nombre;
                    if (item.Origen == "TADA")
                    {
                        var idreg = GetIdByRegPatronal(item.RegistroPatronal);
                        var respuesta = cc.GetRespuestaDetalleLote(idreg, item.Lote.ToString());
                        foreach (var item2 in respuesta.respuestaWebService.movimientosLote)
                        {
                            if (item2.nss == item.Imss)
                            {
                                var sdi = item2.salarioBase;
                                model.SDI = double.Parse(sdi);
                            }
                        }
                    }
                    else
                    {
                        model.SDI = item.SDI;
                    }
                    model.Imss = item.Imss;
                    model.TipoSemana = item.TipoSemana;
                    model.FechaMovimiento = item.FechaMovimiento;
                    model.FechaTransmision = item.FechaTransmision;
                    model.TipoMovimiento = item.TipoMovimiento;
                    model.Movimiento = item.Movimiento;
                    model.CausaBaja = item.CausaBaja;
                    model.Origen = item.Origen;
                    listado.Add(model);
                }
                return listado;
            }
        }

        public void CreateExcel(string Imss)
        {
            var listado = GetInfoToPDF(Imss);
            string path = @"D:\SistemaTada\LotesIMSS\AfilesIndividuales" + Imss + @"\" + "AfilesIndividuales.xlsx";
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Lote");
            tabla.Columns.Add("Nombre Patrona");
            tabla.Columns.Add("RFC_Patrona");
            tabla.Columns.Add("Registro Patronal");
            tabla.Columns.Add("Actividad Economica");
            tabla.Columns.Add("Apellido Paterno");
            tabla.Columns.Add("Apellido Materno");
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("SDI");
            tabla.Columns.Add("IMSS");
            tabla.Columns.Add("Tipo Semana");
            tabla.Columns.Add("Fecha Movimiento");
            tabla.Columns.Add("Fecha Transmisión");
            tabla.Columns.Add("Tipo Movimiento");
            tabla.Columns.Add("Movimiento");
            tabla.Columns.Add("Causa Baja");
            tabla.Columns.Add("Origen");
            foreach (var item in listado)
            {
                DataRow row = tabla.NewRow();
                row["Lote"] = item.Lote;
                row["Nombre Patrona"] = item.NombrePatrona;
                row["RFC_Patrona"] = item.RFC_Patrona;
                row["Registro Patronal"] = item.RegistroPatronal;
                row["Actividad Economica"] = item.ActividadEconomica;
                row["Apellido Paterno"] = item.ApellidoPaterno;
                row["Apellido Materno"] = item.ApellidoMaterno;
                row["Nombre"] = item.Nombre;
                row["SDI"] = item.SDI;
                row["IMSS"] = item.Imss;
                row["Tipo Semana"] = item.TipoSemana;
                row["Fecha Movimiento"] = item.FechaMovimiento;
                row["Fecha Transmisión"] = item.FechaTransmision;
                row["Tipo Movimiento"] = item.TipoMovimiento;
                row["Movimiento"] = item.Movimiento;
                row["Causa Baja"] = item.CausaBaja;
                tabla.Rows.Add(row);
            }
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(tabla, "Afiles Individuales");
                    wb.SaveAs(path);
                }
            }
            else
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(tabla, "Afiles Individuales");
                    wb.SaveAs(path);
                }
            }

        }
    }
}