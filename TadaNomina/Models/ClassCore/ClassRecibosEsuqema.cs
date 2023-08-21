using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassRecibosEsuqema
    {

        SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);

        /// <summary>
        /// Método que lista periodos de nómina con sus distintos tipos de nómina.
        /// </summary>
        /// <param name="_IdUnidadNegocio">Recibe una variable tipo int.</param>
        /// <returns>Regresa la lista de los periodos de nómina.</returns>
        public List<ModelPeriodoNomina> GetPeriodosNomina(int _IdUnidadNegocio)
        {
            using (NominaEntities1 _ctx = new NominaEntities1())
            {
                return (from x in _ctx.PeriodoNomina
                        where x.IdUnidadNegocio == _IdUnidadNegocio && x.IdEstatus == 2 && (x.TipoNomina == "Nomina" || x.TipoNomina == "Aguinaldo" || x.TipoNomina == "Complemento" || x.TipoNomina == "Finiquitos")
                        select new ModelPeriodoNomina
                        {
                            IdPeriodoNomina = x.IdPeriodoNomina,
                            IdUnidadNegocio = x.IdUnidadNegocio,
                            Periodo = x.Periodo,
                            TipoNomina = x.TipoNomina,
                            FechaInicio = x.FechaInicio.ToString(),
                            FechaFin = x.FechaFin.ToString(),
                            AjusteImpuestos = x.AjusteDeImpuestos,
                            IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                            Observaciones = x.Observaciones
                        }).ToList();
            }
        }

        /// <summary>
        /// Método que lista los periodos de nómina por unidad de negocio.
        /// </summary>
        /// <param name="_IdUnidadNegocio">Recibe la variable de tipo int.</param>
        /// <returns>Regresa la lista de los periodos de nómina por unidad de negocio.</returns>
        public List<ModelPeriodoNomina> GetPeriodos(int _IdUnidadNegocio)
        {
            using (NominaEntities1 _ctx = new NominaEntities1())
            {
                return (from x in _ctx.PeriodoNomina
                        where x.IdUnidadNegocio == _IdUnidadNegocio && x.IdEstatus == 2
                        select new ModelPeriodoNomina
                        {
                            IdPeriodoNomina = x.IdPeriodoNomina,
                            IdUnidadNegocio = x.IdUnidadNegocio,
                            Periodo = x.Periodo,
                            TipoNomina = x.TipoNomina,
                            FechaInicio = x.FechaInicio.ToString(),
                            FechaFin = x.FechaFin.ToString(),
                            AjusteImpuestos = x.AjusteDeImpuestos,
                            IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                            Observaciones = x.Observaciones
                        }).ToList();
            }
        }

        /// <summary>
        /// Método que lista los valores del combo box de los periodos de nómina.
        /// </summary>
        /// <param name="_PeriodoNomina">Recibe el modelo del periodo de nómina.</param>
        /// <returns>Regresa la lista de los valores del periodo de nómina.</returns>
        public List<SelectListItem> ListaPeriodosnomina(List<ModelPeriodoNomina> _PeriodoNomina)
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem
            {
                Value = "",
                Text = "Elegir...",
            });
            foreach (var item in _PeriodoNomina)
            {
                _lista.Add(new SelectListItem
                {
                    Value = item.IdPeriodoNomina.ToString(),
                    Text = item.Periodo,
                });
            }
            return _lista;
        }

        /// <summary>
        /// Método que lista los formatos de impresion de los recibos esquema.
        /// </summary>
        /// <returns>Regresa la lista con los valores del formato del recibo esquema.</returns>
        public List<SelectListItem> ListarFormatos()
        {
            List<SelectListItem> _lista = new List<SelectListItem>();
            _lista.Add(new SelectListItem { Value = "1", Text = "2 recibos en una hoja." });
            _lista.Add(new SelectListItem { Value = "2", Text = "1 Recibo en una hoja." });

            return _lista;
        }

        /// <summary>
        /// Método que lista los valores del combo box del tipo de archivo.
        /// </summary>
        /// <returns>Regresa los valores de los tipos de archivo.</returns>
        public List<SelectListItem> ListarTipoArchivos()
        {
            List<SelectListItem> _lista = new List<SelectListItem>();
            _lista.Add(new SelectListItem { Value = "1", Text = "Un solo archivo" });
            _lista.Add(new SelectListItem { Value = "2", Text = "Varios archivos" });

            return _lista;
        }

        /// <summary>
        /// Método para obtener la información del periodo de nómina.
        /// </summary>
        /// <param name="idPeriodoNomina">Recibe la variable tipo int.</param>
        /// <returns>Regresa el resultado de la búsqueda.</returns>
        /// <exception cref="Exception">Envía mensaje de error.</exception>
        public PeriodoNomina ObtenerDatosPerido(int idPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                try
                {
                    var query = (from b in entidad.PeriodoNomina where b.IdPeriodoNomina == idPeriodoNomina select b).FirstOrDefault();

                    return query;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Método para borrar recibos esquema.
        /// </summary>
        /// <param name="nomina">Recibe el modelo del periodo de nómina.</param>
        private void BorraRegistrosAnteriores(PeriodoNomina nomina)
        {

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var Delete = entidad.RecibosEsquema.Where(x => x.IdPeriodoNomina == nomina.IdPeriodoNomina);
                entidad.RecibosEsquema.RemoveRange(Delete);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para iniciar la conexión a la base de datos de la nómina.
        /// </summary>
        /// <param name="query">Recibe la variable tipo string.</param>
        /// <returns>Regresa el resultado de la conexión.</returns>
        /// <exception cref="Exception">Envía mensaje de eroor.</exception>
        public string ObtenerQuery(string query)
        {
            SqlConnection sqlconnLocal = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);
            try
            {
                sqlconnLocal.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlconnLocal))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                sqlconnLocal.Close();
            }
        }

        /// <summary>
        /// Método para obtener el encabezado del archivo XML.
        /// </summary>
        /// <param name="idPeriodoNomina">Recibe la variable tipo int.</param>
        /// <param name="idEmpleado">Recibe la variable tipo int.</param>
        /// <param name="nombre">Recibe la variable tipo string.</param>
        /// <param name="RFC">Recibe la variable tipo string.</param>
        /// <param name="claveEmpleado">Recibe la variable tipo string.</param>
        /// <param name="Periodo">Recibe la variable tipo string.</param>
        /// <returns>Recibe los valores enviados en el encabezado del recibo.</returns>
        /// <returns>Recibe los valores enviados en el encabezado del recibo.</returns>
        private string ObtenEncabezadoXML(int idPeriodoNomina, int idEmpleado, string nombre, string RFC, string claveEmpleado, string Periodo)
        {
            string strEncabezadoXML = "<?xml version='1.0' encoding='UTF-8' standalone='yes'?>" +
                                "<ReciboPago>" +
                                "<DatosGenerales>" +
                                "<NumeroEmpleado>" + claveEmpleado + "</NumeroEmpleado>" +
                                "<Nombre>" + nombre + "</Nombre>" +
                                "<RFC>" + RFC + "</RFC>" +
                                "<Periodo>" + Periodo + "</Periodo>" +
                                "</DatosGenerales>";
            return strEncabezadoXML;
        }

        /// <summary>
        /// Método para obtener las percepciones en el archivo XML.
        /// </summary>
        /// <param name="idPeriodoNomina">Recibe la variable tipo int.</param>
        /// <param name="idEmpleado">Recibe la variable tipo int.</param>
        /// <param name="nombre">Recibe la variable tipo string.</param>
        /// <param name="RFC">Recibe la variable tipo string.</param>
        /// <param name="claveEmpleado">Recibe la variable tipo string.</param>
        /// <param name="idUnidadNegocio">Recibe la variable tipo int.</param>
        /// <param name="pConsulta">Recibe la variable tipo string.</param>
        /// <returns>Regresa el cálculo de las percepciones.</returns>
        /// <exception cref="Exception">Envía mensaje de error.</exception>
        private string ObtenPercepcionesXML(int idPeriodoNomina, int idEmpleado, string nombre, string RFC, string claveEmpleado, int idUnidadNegocio, string pConsulta)
        {
            string strXMLPercepciones = " ";
            SqlConnection sqlconnAuxiliarPercepciones = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);

            pConsulta = pConsulta.Replace("IDIDPERIODONOMINA", idPeriodoNomina.ToString());
            pConsulta = pConsulta.Replace("IDIDEMPLEADO", idEmpleado.ToString());

            try
            {
                sqlconnAuxiliarPercepciones.Open();
                using (SqlCommand cmd = new SqlCommand(pConsulta, sqlconnAuxiliarPercepciones))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        strXMLPercepciones = strXMLPercepciones + "<Percepcion><Clave>" + dr.GetString(0) + "</Clave><Concepto>" + dr.GetString(1) + "</Concepto><Importe>" + dr.GetDecimal(2) + "</Importe></Percepcion>";
                    }
                    return "<Percepciones>" + strXMLPercepciones + "</Percepciones>";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                sqlconnAuxiliarPercepciones.Close();
            }
        }

        /// <summary>
        /// Método para obtener las deducciones en el archivo XML.
        /// </summary>
        /// <param name="idPeriodoNomina">Recibe la variable tipoint.</param>
        /// <param name="idEmpleado">Recibe la variable tipo int.</param>
        /// <param name="nombre">Recibe la variable tipo string.</param>
        /// <param name="RFC">Recibe la variable tipo string.</param>
        /// <param name="claveEmpleado">Recibe la variable tipo string.</param>
        /// <param name="idUnidadNegocio">Recibe la variable tipo int.</param>
        /// <param name="pConsulta">Recibe la variable tipo string.</param>
        /// <returns>Regresavlas deducciones para el archivo XML.</returns>
        /// <exception cref="Exception">Envía mensaje de error.</exception>
        private string ObtenDeduccionesXML(int idPeriodoNomina, int idEmpleado, string nombre, string RFC, string claveEmpleado, int idUnidadNegocio, string pConsulta)
        {
            string strXMLDeducciones = " ";
            SqlConnection sqlconnAuxiliarPercepciones = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);

            pConsulta = pConsulta.Replace("IDIDPERIODONOMINA", idPeriodoNomina.ToString());
            pConsulta = pConsulta.Replace("IDIDEMPLEADO", idEmpleado.ToString());

            try
            {
                sqlconnAuxiliarPercepciones.Open();
                using (SqlCommand cmd = new SqlCommand(pConsulta, sqlconnAuxiliarPercepciones))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        strXMLDeducciones = strXMLDeducciones + "<Deduccion><Clave>" + dr.GetString(0) + "</Clave><Concepto>" + dr.GetString(1) + "</Concepto><Importe>" + dr.GetDecimal(2) + "</Importe></Deduccion>";
                    }
                    return "<Deducciones>" + strXMLDeducciones + "</Deducciones>";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                sqlconnAuxiliarPercepciones.Close();
            }
        }

        /// <summary>
        /// Método para realizar los recibos complementarios.
        /// </summary>
        /// <param name="idPeriodoNomina">Recibe la variable tipo int.</param>
        /// <param name="idUnidadNegocio">Recibe la variable tipo int.</param>
        /// <exception cref="Exception"></exception>
        public void GeneraRecibosComplementarios(int idPeriodoNomina, int idUnidadNegocio)
        {
            string ConsultaPercepciones = string.Empty;
            string ConsultaDeducciones = string.Empty;

            PeriodoNomina datosPerido = new PeriodoNomina();
            try
            {
                datosPerido = ObtenerDatosPerido(idPeriodoNomina);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            // BORRAMOS TODOS LOS REGISTROS
            BorraRegistrosAnteriores(datosPerido);

            // Obtenemos los Querys para los recibos          

            ConsultaPercepciones = ObtenerQuery("select dbo.fu_ObtenQueryRecibosComplementarios(" + idUnidadNegocio + ",'ER'" + ")");
            ConsultaDeducciones = ObtenerQuery("select dbo.fu_ObtenQueryRecibosComplementarios(" + idUnidadNegocio + ",'DD'" + ")");


            // PROCEDEMOS A INSERTAR REGISTROS
            try
            {
                string query = "select a.IdEmpleado, b.apellidopaterno + ' ' + b.apellidomaterno + ' ' + b.nombre , b.rfc , b.claveempleado, a.ers, a.dds, a.netos," +
                                " 'Del '+ cast (c.FechaInicio as varchar(10))+' al '+cast(c.FechaFin as varchar(10)) " +
                                " from nomina a " +
                                " inner join empleados b " +
                                " on a.idempleado=b.idempleado " +
                                " inner join PeriodoNomina c" +
                                " on a.idperiodonomina= c.idperiodonomina " +
                                " where a.idestatus=1 and a.netos>0 and a.idperiodonomina=" + idPeriodoNomina;

                sqlconn.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlconn))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader dr = cmd.ExecuteReader();



                    while (dr.Read())
                    {
                        decimal dTotalPercepciones = dr.GetDecimal(4);
                        decimal dTotalDeducciones = dr.GetDecimal(5);
                        string strXMLEncabezado = ObtenEncabezadoXML(idPeriodoNomina, dr.GetInt32(0), dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(7));
                        string strPercepciones = ObtenPercepcionesXML(idPeriodoNomina, dr.GetInt32(0), dr.GetString(1), dr.GetString(2), dr.GetString(3), idUnidadNegocio, ConsultaPercepciones);
                        string strDeducciones = ObtenDeduccionesXML(idPeriodoNomina, dr.GetInt32(0), dr.GetString(1), dr.GetString(2), dr.GetString(3), idUnidadNegocio, ConsultaDeducciones);

                        string strXML = strXMLEncabezado + strPercepciones + strDeducciones + "</ReciboPago>";

                        string queryInserta = "INSERT INTO recibosesquema(IdPeriodoNomina, IdEmpleado, RFC, Total_Percepciones, Total_Deducciones, CadenaXML, Periodo, FechaInicio, FechaFin, FechaCaptura, IdEstatus) " +
                            "VALUES (@IdPeriodo,@IdEmpleado,@RFC,@Percepciones,@Deducciones,@cadenaXML,@Periodo, @FechaInicio, @FechaFin,@FechaCaptura,@IdEstatus)";

                        SqlConnection sqlconnAuxiliar = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString);
                        sqlconnAuxiliar.Open();
                        using (SqlCommand cmdInserta = new SqlCommand(queryInserta, sqlconnAuxiliar))
                        {
                            cmdInserta.Parameters.Add(new SqlParameter("@IdPeriodo", idPeriodoNomina));
                            cmdInserta.Parameters.Add(new SqlParameter("@IdEmpleado", dr.GetInt32(0)));
                            cmdInserta.Parameters.Add(new SqlParameter("@RFC", dr.GetString(2)));
                            cmdInserta.Parameters.Add(new SqlParameter("@Percepciones", dTotalPercepciones));
                            cmdInserta.Parameters.Add(new SqlParameter("@Deducciones", dTotalDeducciones));
                            cmdInserta.Parameters.Add(new SqlParameter("@cadenaXML", strXML));
                            cmdInserta.Parameters.Add(new SqlParameter("@Periodo", datosPerido.Periodo));
                            cmdInserta.Parameters.Add(new SqlParameter("@FechaInicio", datosPerido.FechaInicio));
                            cmdInserta.Parameters.Add(new SqlParameter("@FechaFin", datosPerido.FechaFin));
                            cmdInserta.Parameters.Add(new SqlParameter("@FechaCaptura", DateTime.Now));
                            cmdInserta.Parameters.Add(new SqlParameter("@IdEstatus", 1));

                            cmdInserta.CommandType = CommandType.Text;
                            cmdInserta.ExecuteNonQuery();
                        }
                        sqlconnAuxiliar.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                sqlconn.Close();
            }
        }




        ///////////////////////////////////////////// reación de PDF //////////////////////////////

        iTextSharp.text.Font _negrita = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

        /// <summary>
        /// Método para generar en texto una cantidad decimal expresada en pesos.
        /// </summary>
        /// <param name="num">Recibe la variable tipo string.</param>
        /// <returns>Regresa el valor expresado en texto.</returns>
        public string enletras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;

            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));
            if (decimales > 0)
            {
                dec = " CON " + decimales.ToString() + "/100";
            }

            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }

        /// <summary>
        /// Método para generar en texto una cantidad entera expresada en pesos.
        /// </summary>
        /// <param name="value">Recibe la variable tipo decimal.</param>
        /// <returns>Regresa el valor expresado en texto.</returns>
        private string toText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {
                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            return Num2Text;

        }

        /// <summary>
        /// Método para construir el PDF.
        /// </summary>
        /// <param name="cadenaXML">Recibe la variable tipo string.</param>
        /// <param name="path">Recibe la variable tipo string.</param>
        /// <param name="formato">Recibe la variable tipo string.</param>
        /// <param name="monto">Recibe la variable tipo string.</param>
        public void generaPDFTodosObses(string cadenaXML, string path, string formato, string monto)
        {
            //Cargamos el XML para leerlo
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(cadenaXML);


            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc,
                                        new FileStream(path, FileMode.Create));

            // Definimos los tipos de fuente
            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 9);

            // Para el cuerpo del recibo
            // Para el encabezado
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesEncabezado, 9);

            // Para la Leyenda Final
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontLeyenda = new Font(bfTimes, 7);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            // Abrimos el archivo
            doc.Open();

            //PARA OBSES INSERTAMOS EL ENCABEZADO CON IMAGEN Y LEYENDAS

            //IMAGEN/////////////////////////////////////////////////
            string imagepath = HttpContext.Current.Server.MapPath("~/Imagenes");
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imagepath + "/Encabezado_Obses.jpg");
            //Resize image depend upon your need
            jpg.ScaleToFit(350f, 100f);
            //Give space before image
            jpg.SpacingBefore = 10f;
            //Give some space after the image
            jpg.SpacingAfter = 1f;
            jpg.Alignment = Element.ALIGN_CENTER;
            ///////////////////////////////////////////////////////  

            ////////////////////////////LEYENDA/////////////////////////////////////
            XmlNodeList listaDatosAuxiliar = xmlDoc.SelectNodes("ReciboPago/DatosGenerales");
            XmlNode datosAuxiliar;
            string nombreEmpleado = string.Empty;
            for (int i = 0; i < listaDatosAuxiliar.Count; i++)
            {
                datosAuxiliar = listaDatosAuxiliar.Item(i);
                nombreEmpleado = datosAuxiliar.SelectSingleNode("Nombre").InnerText;
            }

            Chunk frase1 = new Chunk("YO, " + nombreEmpleado + ", ", _negrita);
            Chunk frase2 = new Chunk("en mi carácter de agremiado del ", _standardFont);
            Chunk frase3 = new Chunk("SINDICATO UNICO DE EMPLEADOS Y TRABAJADORES DE HOTELES, MOTELES, RESTAURANTES, BARES, CENTROS NOCTURNOS Y SIMILARES DEL DISTRITO FEDERAL ", _negrita);
            Chunk frase4 = new Chunk("recibo la cantidad de ", _standardFont);
            Chunk frase5 = new Chunk("$ " + monto + " M.N. (" + enletras(monto) + "), ", _negrita);
            Chunk frase6 = new Chunk(" por concepto de todos y cada uno de los apoyos y ayudas sociales que se desprenden del Contrato Colectivo de Trabajo, en base a lo contenido en las cláusulas que a continuación se detallan: ", _standardFont);


            // Creamos las frases
            Phrase p1 = new Phrase();
            p1.Add(frase1);
            p1.Add(frase2);
            p1.Add(frase3);
            p1.Add(frase4);
            p1.Add(frase5);
            p1.Add(frase6);

            Paragraph parrafo1 = new Paragraph();
            parrafo1.Alignment = Element.ALIGN_JUSTIFIED;
            parrafo1.Add(p1);
            ///////////////////////////////////////////////////////////////////////



            //RECIBO NORMAL 
            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = 500f;
            table.LockedWidth = true;


            //SECCION DE ENCABEZADO DEL RECIBO
            PdfPCell NoEmpleado = new PdfPCell(new Phrase("NO.", fontEncabezado));
            NoEmpleado.Colspan = 1;
            table.AddCell(NoEmpleado);

            PdfPCell Nombre = new PdfPCell(new Phrase("NOMBRE", fontEncabezado));
            Nombre.Colspan = 5;
            table.AddCell(Nombre);

            PdfPCell rfc = new PdfPCell(new Phrase("RFC", fontEncabezado));
            rfc.Colspan = 2;
            table.AddCell(rfc);

            PdfPCell periodo = new PdfPCell(new Phrase("PERIODO", fontEncabezado));
            periodo.Colspan = 2;
            table.AddCell(periodo);

            //DATOS DEL ENCABEZADO
            XmlNodeList listaDatos = xmlDoc.SelectNodes("ReciboPago/DatosGenerales");
            XmlNode datos;
            for (int i = 0; i < listaDatos.Count; i++)
            {
                datos = listaDatos.Item(i);
                PdfPCell NoEmpleadoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("NumeroEmpleado").InnerText, fontEncabezado));
                NoEmpleadoDato.Colspan = 1;
                table.AddCell(NoEmpleadoDato);

                PdfPCell NombreDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Nombre").InnerText, fontEncabezado));
                NombreDato.Colspan = 5;
                table.AddCell(NombreDato);

                PdfPCell rfcDato = new PdfPCell(new Phrase(datos.SelectSingleNode("RFC").InnerText, fontEncabezado));
                rfcDato.Colspan = 2;
                table.AddCell(rfcDato);

                PdfPCell periodoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Periodo").InnerText, fontEncabezado));
                periodoDato.Colspan = 2;
                table.AddCell(periodoDato);
            }

            PdfPCell celPercepciones = new PdfPCell(new Phrase("PERCEPCIONES", fontEncabezado));
            celPercepciones.Colspan = 5;
            table.AddCell(celPercepciones);

            PdfPCell celDeducciones = new PdfPCell(new Phrase("DEDUCCIONES", fontEncabezado));
            celDeducciones.Colspan = 5;
            table.AddCell(celDeducciones);

            //DATOS DE LAS PERCEPCIONES
            PdfPTable PercepcionesTable = new PdfPTable(5);
            XmlNodeList listaPercepciones = xmlDoc.SelectNodes("ReciboPago/Percepciones/Percepcion");
            XmlNode percepciones;
            int auxiliarPercepciones = 6 - listaPercepciones.Count;
            decimal dtotalPercepciones = 0;

            for (int i = 0; i < listaPercepciones.Count; i++)
            {
                percepciones = listaPercepciones.Item(i);

                PdfPCell ClaveDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                ClaveDato.Colspan = 1;
                ClaveDato.BorderWidthBottom = 0;
                ClaveDato.BorderWidthLeft = 0;
                ClaveDato.BorderWidthTop = 0;
                ClaveDato.BorderWidthRight = 0.1F;
                PercepcionesTable.AddCell(ClaveDato);

                PdfPCell ConceptoDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                ConceptoDato.Colspan = 3;
                ConceptoDato.BorderWidthBottom = 0;
                ConceptoDato.BorderWidthLeft = 0;
                ConceptoDato.BorderWidthTop = 0;
                ConceptoDato.BorderWidthRight = 0.1F;
                PercepcionesTable.AddCell(ConceptoDato);

                PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                ImporteDato.Colspan = 1;
                ImporteDato.BorderWidthBottom = 0;
                ImporteDato.BorderWidthLeft = 0;
                ImporteDato.BorderWidthTop = 0;
                ImporteDato.BorderWidthRight = 0;
                ImporteDato.HorizontalAlignment = Element.ALIGN_RIGHT;
                PercepcionesTable.AddCell(ImporteDato);

                dtotalPercepciones = dtotalPercepciones + decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText);
            }

            if (auxiliarPercepciones > 0)
            {
                for (int i = 0; i < auxiliarPercepciones; i++)
                {
                    PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    PercepcionesTable.AddCell(ImporteDato);
                }
            }

            PdfPCell columasPercepciones = new PdfPCell(PercepcionesTable);
            columasPercepciones.Colspan = 5;
            columasPercepciones.Padding = 0f;
            table.AddCell(columasPercepciones);

            //DATOS DE LAS DEDUCCIONES
            PdfPTable DeduccionesTable = new PdfPTable(5);
            XmlNodeList listaDeducciones = xmlDoc.SelectNodes("ReciboPago/Deducciones/Deduccion");
            XmlNode deducciones;
            int auxiliarDeducciones = 6 - listaDeducciones.Count;
            decimal dtotalDeducciones = 0;

            for (int i = 0; i < listaDeducciones.Count; i++)
            {
                deducciones = listaDeducciones.Item(i);

                PdfPCell ClaveDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                ClaveDato.Colspan = 1;
                ClaveDato.BorderWidthBottom = 0;
                ClaveDato.BorderWidthLeft = 0;
                ClaveDato.BorderWidthTop = 0;
                ClaveDato.BorderWidthRight = 0.1F;
                DeduccionesTable.AddCell(ClaveDato);

                PdfPCell ConceptoDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                ConceptoDato.Colspan = 3;
                ConceptoDato.BorderWidthBottom = 0;
                ConceptoDato.BorderWidthLeft = 0;
                ConceptoDato.BorderWidthTop = 0;
                ConceptoDato.BorderWidthRight = 0.1F;
                DeduccionesTable.AddCell(ConceptoDato);

                PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                ImporteDato.Colspan = 1;
                ImporteDato.BorderWidthBottom = 0;
                ImporteDato.BorderWidthLeft = 0;
                ImporteDato.BorderWidthTop = 0;
                ImporteDato.BorderWidthRight = 0;
                DeduccionesTable.AddCell(ImporteDato);

                dtotalDeducciones = dtotalDeducciones + decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText); ;
            }

            if (auxiliarDeducciones > 0)
            {
                for (int i = 0; i < auxiliarDeducciones; i++)
                {
                    PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    DeduccionesTable.AddCell(ImporteDato);
                }
            }
            PdfPCell columasDeducciones = new PdfPCell(DeduccionesTable);
            columasDeducciones.Padding = 0f;
            columasDeducciones.Colspan = 5;
            table.AddCell(columasDeducciones);

            PdfPCell totalPercepciones = new PdfPCell(new Phrase("TOTAL PERCEPCIONES", fontEncabezado));
            totalPercepciones.Colspan = 4;
            totalPercepciones.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell totalPercepcionesImporte = new PdfPCell(new Phrase(dtotalPercepciones.ToString("c"), fontEncabezado));
            totalPercepcionesImporte.Colspan = 1;
            totalPercepcionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

            table.AddCell(totalPercepciones);
            table.AddCell(totalPercepcionesImporte);

            PdfPCell totalDeducciones = new PdfPCell(new Phrase("TOTAL DEDUCCIONES", fontEncabezado));
            totalDeducciones.Colspan = 4;
            totalDeducciones.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell totalDeduccionesImporte = new PdfPCell(new Phrase(dtotalDeducciones.ToString("c"), fontEncabezado));
            totalDeduccionesImporte.Colspan = 1;
            totalDeduccionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

            table.AddCell(totalDeducciones);
            table.AddCell(totalDeduccionesImporte);

            PdfPCell netoVacio = new PdfPCell(new Phrase(" "));
            netoVacio.Colspan = 7;
            netoVacio.BorderWidthBottom = 0;
            netoVacio.BorderWidthLeft = 0;
            netoVacio.BorderWidthTop = 0;
            netoVacio.BorderWidthRight = 0;
            table.AddCell(netoVacio);

            PdfPCell neto = new PdfPCell(new Phrase("NETO  " + (dtotalPercepciones - dtotalDeducciones).ToString("c"), fontEncabezado));
            neto.Colspan = 3;
            neto.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(neto);

            //SECCION PARA LA LEYENDA DE FIRMA
            PdfPTable tableLeyenda = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.LockedWidth = true;

            PdfPCell celdaLeyenda = new PdfPCell(new Phrase("ACORDE AL ESTATUTO DEL SINDICATO LA CANTIDAD QUE ME CORRESPONDE POR APOYO/AYUDA SINDICAL SIN GENERAR RELACION LABORAL OTORGANDO FINIQUITO TOTAL.", fontLeyenda));
            celdaLeyenda.Colspan = 4;
            celdaLeyenda.BorderWidthBottom = 0;
            celdaLeyenda.BorderWidthLeft = 0;
            celdaLeyenda.BorderWidthTop = 0;
            celdaLeyenda.BorderWidthRight = 0;
            tableLeyenda.AddCell(celdaLeyenda);

            PdfPTable tableFirma = new PdfPTable(1);

            PdfPCell raya = new PdfPCell(new Phrase("_____________________", fontLeyenda));
            raya.BorderWidthBottom = 0;
            raya.BorderWidthLeft = 0;
            raya.BorderWidthTop = 0;
            raya.BorderWidthRight = 0;

            PdfPCell firmaFinal = new PdfPCell(new Phrase("FIRMA", fontLeyenda));
            firmaFinal.BorderWidthBottom = 0;
            firmaFinal.BorderWidthLeft = 0;
            firmaFinal.BorderWidthTop = 0;
            firmaFinal.BorderWidthRight = 0;
            firmaFinal.HorizontalAlignment = Element.ALIGN_CENTER;

            tableFirma.AddCell(raya);
            tableFirma.AddCell(firmaFinal);

            PdfPCell celdaFirma = new PdfPCell(tableFirma);
            celdaFirma.Colspan = 1;
            celdaFirma.BorderWidthBottom = 0;
            celdaFirma.BorderWidthLeft = 0;
            celdaFirma.BorderWidthTop = 0;
            celdaFirma.BorderWidthRight = 0;
            tableLeyenda.AddCell(celdaFirma);

            //SI LA FECHA DE CUMPLEAÑOS DEL EMPLEADO ESTA DENTRO DEL PERIDO DE NOMINA
            //if (bLeyendaCumple == true)
            //{
            //    PdfPCell celCumple = new PdfPCell(new Phrase("TE DESEAMOS QUE CADA UNA DE LAS COSAS, GRANDES O PEQUEÑAS QUE TU CORAZÓN ANHELE, PUEDAN HACERSE REALIDAD. ¡FELIZ CUMPLEAÑOS!", fontLeyenda));
            //    celCumple.Colspan = 5;
            //    celCumple.BorderWidthBottom = 0;
            //    celCumple.BorderWidthLeft = 0;
            //    celCumple.BorderWidthTop = 0;
            //    celCumple.BorderWidthRight = 0;
            //    celCumple.HorizontalAlignment = Element.ALIGN_CENTER;
            //    tableLeyenda.AddCell(celCumple);
            //}


            if (formato == "1")
            {
                doc.Add(jpg);
                doc.Add(parrafo1);
                doc.Add(espacio);
                doc.Add(table);
                doc.Add(espacio);
                doc.Add(tableLeyenda);

                doc.Add(espacio);

                doc.Add(jpg);
                doc.Add(parrafo1);
                doc.Add(espacio);
                doc.Add(table);
                doc.Add(espacio);
                doc.Add(tableLeyenda);

            }
            else
            {
                doc.Add(jpg);
                doc.Add(parrafo1);
                doc.Add(table);
                doc.Add(espacio);
                doc.Add(tableLeyenda);
            }

            doc.Close();
            writer.Close();
        }

        /// <summary>
        /// Método para construir el recibo en formato PDF.
        /// </summary>
        /// <param name="recibos">Recibe el modelo de los recibos esquema.</param>
        /// <param name="path">Recibe la variable tipo string.</param>
        /// <param name="formato">Recibe la variable tipo string.</param>
        public void GeneraRecibosUnArchivoObses(List<RecibosEsquema> recibos, string path, string formato)
        {
            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER, 20, 20, 20, 20);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc,
                                        new FileStream(path, FileMode.Create));

            // Definimos los tipos de fuente
            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 9);

            // Para el cuerpo del recibo
            // Para el encabezado
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesEncabezado, 9);

            // Para la Leyenda Final
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontLeyenda = new Font(bfTimes, 7);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            // Abrimos el archivo
            doc.Open();


            foreach (var item in recibos)
            {
                //Cargamos el XML para leerlo
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(item.CadenaXML);
                string monto = (item.Total_Percepciones - item.Total_Deducciones).ToString();

                //PARA OBSES INSERTAMOS EL ENCABEZADO CON IMAGEN Y LEYENDAS

                //IMAGEN/////////////////////////////////////////////////
                string imagepath = HttpContext.Current.Server.MapPath("~/Imagenes");
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imagepath + "/Encabezado_Obses.jpg");
                //Resize image depend upon your need
                jpg.ScaleToFit(350f, 100f);
                //Give space before image
                jpg.SpacingBefore = 10f;
                //Give some space after the image
                jpg.SpacingAfter = 1f;
                jpg.Alignment = Element.ALIGN_CENTER;
                ///////////////////////////////////////////////////////  

                ////////////////////////////LEYENDA/////////////////////////////////////
                XmlNodeList listaDatosAuxiliar = xmlDoc.SelectNodes("ReciboPago/DatosGenerales");
                XmlNode datosAuxiliar;
                string nombreEmpleado = string.Empty;
                for (int i = 0; i < listaDatosAuxiliar.Count; i++)
                {
                    datosAuxiliar = listaDatosAuxiliar.Item(i);
                    nombreEmpleado = datosAuxiliar.SelectSingleNode("Nombre").InnerText;
                }

                Chunk frase1 = new Chunk("YO, " + nombreEmpleado + ", ", _negrita);
                Chunk frase2 = new Chunk("en mi carácter de agremiado del ", _standardFont);
                Chunk frase3 = new Chunk("SINDICATO UNICO DE EMPLEADOS Y TRABAJADORES DE HOTELES, MOTELES, RESTAURANTES, BARES, CENTROS NOCTURNOS Y SIMILARES DEL DISTRITO FEDERAL ", _negrita);
                Chunk frase4 = new Chunk("recibo la cantidad de ", _standardFont);
                Chunk frase5 = new Chunk("$ " + monto + " M.N. (" + enletras(monto) + "), ", _negrita);
                Chunk frase6 = new Chunk(" por concepto de todos y cada uno de los apoyos y ayudas sociales que se desprenden del Contrato Colectivo de Trabajo, en base a lo contenido en las cláusulas que a continuación se detallan: ", _standardFont);

                // Creamos las frases
                Phrase p1 = new Phrase();
                p1.Add(frase1);
                p1.Add(frase2);
                p1.Add(frase3);
                p1.Add(frase4);
                p1.Add(frase5);
                p1.Add(frase6);

                Paragraph parrafo1 = new Paragraph();
                parrafo1.Alignment = Element.ALIGN_JUSTIFIED;
                parrafo1.Add(p1);
                ///////////////////////////////////////////////////////////////////////



                //RECIBO NORMAL 
                PdfPTable table = new PdfPTable(10);
                table.TotalWidth = 500f;
                table.LockedWidth = true;


                //SECCION DE ENCABEZADO DEL RECIBO
                PdfPCell NoEmpleado = new PdfPCell(new Phrase("NO.", fontEncabezado));
                NoEmpleado.Colspan = 1;
                table.AddCell(NoEmpleado);

                PdfPCell Nombre = new PdfPCell(new Phrase("NOMBRE", fontEncabezado));
                Nombre.Colspan = 5;
                table.AddCell(Nombre);

                PdfPCell rfc = new PdfPCell(new Phrase("RFC", fontEncabezado));
                rfc.Colspan = 2;
                table.AddCell(rfc);

                PdfPCell periodo = new PdfPCell(new Phrase("PERIODO", fontEncabezado));
                periodo.Colspan = 2;
                table.AddCell(periodo);

                //DATOS DEL ENCABEZADO
                XmlNodeList listaDatos = xmlDoc.SelectNodes("ReciboPago/DatosGenerales");
                XmlNode datos;
                for (int i = 0; i < listaDatos.Count; i++)
                {
                    datos = listaDatos.Item(i);
                    PdfPCell NoEmpleadoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("NumeroEmpleado").InnerText, fontEncabezado));
                    NoEmpleadoDato.Colspan = 1;
                    table.AddCell(NoEmpleadoDato);

                    PdfPCell NombreDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Nombre").InnerText, fontEncabezado));
                    NombreDato.Colspan = 5;
                    table.AddCell(NombreDato);

                    PdfPCell rfcDato = new PdfPCell(new Phrase(datos.SelectSingleNode("RFC").InnerText, fontEncabezado));
                    rfcDato.Colspan = 2;
                    table.AddCell(rfcDato);

                    PdfPCell periodoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Periodo").InnerText, fontEncabezado));
                    periodoDato.Colspan = 2;
                    table.AddCell(periodoDato);
                }

                PdfPCell celPercepciones = new PdfPCell(new Phrase("PERCEPCIONES", fontEncabezado));
                celPercepciones.Colspan = 5;
                table.AddCell(celPercepciones);

                PdfPCell celDeducciones = new PdfPCell(new Phrase("DEDUCCIONES", fontEncabezado));
                celDeducciones.Colspan = 5;
                table.AddCell(celDeducciones);

                //DATOS DE LAS PERCEPCIONES
                PdfPTable PercepcionesTable = new PdfPTable(5);
                XmlNodeList listaPercepciones = xmlDoc.SelectNodes("ReciboPago/Percepciones/Percepcion");
                XmlNode percepciones;
                int auxiliarPercepciones = 6 - listaPercepciones.Count;
                decimal dtotalPercepciones = 0;

                for (int i = 0; i < listaPercepciones.Count; i++)
                {
                    percepciones = listaPercepciones.Item(i);

                    PdfPCell ClaveDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    ImporteDato.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PercepcionesTable.AddCell(ImporteDato);

                    dtotalPercepciones = dtotalPercepciones + decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText);
                }

                if (auxiliarPercepciones > 0)
                {
                    for (int i = 0; i < auxiliarPercepciones; i++)
                    {
                        PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                        ClaveDato.Colspan = 1;
                        ClaveDato.BorderWidthBottom = 0;
                        ClaveDato.BorderWidthLeft = 0;
                        ClaveDato.BorderWidthTop = 0;
                        ClaveDato.BorderWidthRight = 0.1F;
                        PercepcionesTable.AddCell(ClaveDato);

                        PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                        ConceptoDato.Colspan = 3;
                        ConceptoDato.BorderWidthBottom = 0;
                        ConceptoDato.BorderWidthLeft = 0;
                        ConceptoDato.BorderWidthTop = 0;
                        ConceptoDato.BorderWidthRight = 0.1F;
                        PercepcionesTable.AddCell(ConceptoDato);

                        PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                        ImporteDato.Colspan = 1;
                        ImporteDato.BorderWidthBottom = 0;
                        ImporteDato.BorderWidthLeft = 0;
                        ImporteDato.BorderWidthTop = 0;
                        ImporteDato.BorderWidthRight = 0;
                        PercepcionesTable.AddCell(ImporteDato);
                    }
                }

                PdfPCell columasPercepciones = new PdfPCell(PercepcionesTable);
                columasPercepciones.Colspan = 5;
                columasPercepciones.Padding = 0f;
                table.AddCell(columasPercepciones);

                //DATOS DE LAS DEDUCCIONES
                PdfPTable DeduccionesTable = new PdfPTable(5);
                XmlNodeList listaDeducciones = xmlDoc.SelectNodes("ReciboPago/Deducciones/Deduccion");
                XmlNode deducciones;
                int auxiliarDeducciones = 6 - listaDeducciones.Count;
                decimal dtotalDeducciones = 0;

                for (int i = 0; i < listaDeducciones.Count; i++)
                {
                    deducciones = listaDeducciones.Item(i);

                    PdfPCell ClaveDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    DeduccionesTable.AddCell(ImporteDato);

                    dtotalDeducciones = dtotalDeducciones + decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText); ;
                }

                if (auxiliarDeducciones > 0)
                {
                    for (int i = 0; i < auxiliarDeducciones; i++)
                    {
                        PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                        ClaveDato.Colspan = 1;
                        ClaveDato.BorderWidthBottom = 0;
                        ClaveDato.BorderWidthLeft = 0;
                        ClaveDato.BorderWidthTop = 0;
                        ClaveDato.BorderWidthRight = 0.1F;
                        DeduccionesTable.AddCell(ClaveDato);

                        PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                        ConceptoDato.Colspan = 3;
                        ConceptoDato.BorderWidthBottom = 0;
                        ConceptoDato.BorderWidthLeft = 0;
                        ConceptoDato.BorderWidthTop = 0;
                        ConceptoDato.BorderWidthRight = 0.1F;
                        DeduccionesTable.AddCell(ConceptoDato);

                        PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                        ImporteDato.Colspan = 1;
                        ImporteDato.BorderWidthBottom = 0;
                        ImporteDato.BorderWidthLeft = 0;
                        ImporteDato.BorderWidthTop = 0;
                        ImporteDato.BorderWidthRight = 0;
                        DeduccionesTable.AddCell(ImporteDato);
                    }
                }
                PdfPCell columasDeducciones = new PdfPCell(DeduccionesTable);
                columasDeducciones.Padding = 0f;
                columasDeducciones.Colspan = 5;
                table.AddCell(columasDeducciones);

                PdfPCell totalPercepciones = new PdfPCell(new Phrase("TOTAL PERCEPCIONES", fontEncabezado));
                totalPercepciones.Colspan = 4;
                totalPercepciones.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell totalPercepcionesImporte = new PdfPCell(new Phrase(dtotalPercepciones.ToString("c"), fontEncabezado));
                totalPercepcionesImporte.Colspan = 1;
                totalPercepcionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

                table.AddCell(totalPercepciones);
                table.AddCell(totalPercepcionesImporte);

                PdfPCell totalDeducciones = new PdfPCell(new Phrase("TOTAL DEDUCCIONES", fontEncabezado));
                totalDeducciones.Colspan = 4;
                totalDeducciones.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell totalDeduccionesImporte = new PdfPCell(new Phrase(dtotalDeducciones.ToString("c"), fontEncabezado));
                totalDeduccionesImporte.Colspan = 1;
                totalDeduccionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

                table.AddCell(totalDeducciones);
                table.AddCell(totalDeduccionesImporte);

                PdfPCell netoVacio = new PdfPCell(new Phrase(" "));
                netoVacio.Colspan = 7;
                netoVacio.BorderWidthBottom = 0;
                netoVacio.BorderWidthLeft = 0;
                netoVacio.BorderWidthTop = 0;
                netoVacio.BorderWidthRight = 0;
                table.AddCell(netoVacio);

                PdfPCell neto = new PdfPCell(new Phrase("NETO  " + (dtotalPercepciones - dtotalDeducciones).ToString("c"), fontEncabezado));
                neto.Colspan = 3;
                neto.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(neto);

                //SECCION PARA LA LEYENDA DE FIRMA
                PdfPTable tableLeyenda = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.LockedWidth = true;

                PdfPCell celdaLeyenda = new PdfPCell(new Phrase("ACORDE AL ESTATUTO DEL SINDICATO LA CANTIDAD QUE ME CORRESPONDE POR APOYO/AYUDA SINDICAL SIN GENERAR RELACION LABORAL OTORGANDO FINIQUITO TOTAL.", fontLeyenda));
                celdaLeyenda.Colspan = 4;
                celdaLeyenda.BorderWidthBottom = 0;
                celdaLeyenda.BorderWidthLeft = 0;
                celdaLeyenda.BorderWidthTop = 0;
                celdaLeyenda.BorderWidthRight = 0;
                tableLeyenda.AddCell(celdaLeyenda);

                PdfPTable tableFirma = new PdfPTable(1);

                PdfPCell raya = new PdfPCell(new Phrase("_____________________", fontLeyenda));
                raya.BorderWidthBottom = 0;
                raya.BorderWidthLeft = 0;
                raya.BorderWidthTop = 0;
                raya.BorderWidthRight = 0;

                PdfPCell firmaFinal = new PdfPCell(new Phrase("FIRMA", fontLeyenda));
                firmaFinal.BorderWidthBottom = 0;
                firmaFinal.BorderWidthLeft = 0;
                firmaFinal.BorderWidthTop = 0;
                firmaFinal.BorderWidthRight = 0;
                firmaFinal.HorizontalAlignment = Element.ALIGN_CENTER;

                tableFirma.AddCell(raya);
                tableFirma.AddCell(firmaFinal);

                PdfPCell celdaFirma = new PdfPCell(tableFirma);
                celdaFirma.Colspan = 1;
                celdaFirma.BorderWidthBottom = 0;
                celdaFirma.BorderWidthLeft = 0;
                celdaFirma.BorderWidthTop = 0;
                celdaFirma.BorderWidthRight = 0;
                tableLeyenda.AddCell(celdaFirma);

                //SI LA FECHA DE CUMPLEAÑOS DEL EMPLEADO ESTA DENTRO DEL PERIDO DE NOMINA
                //if (bLeyendaCumple == true)
                //{
                //    PdfPCell celCumple = new PdfPCell(new Phrase("TE DESEAMOS QUE CADA UNA DE LAS COSAS, GRANDES O PEQUEÑAS QUE TU CORAZÓN ANHELE, PUEDAN HACERSE REALIDAD. ¡FELIZ CUMPLEAÑOS!", fontLeyenda));
                //    celCumple.Colspan = 5;
                //    celCumple.BorderWidthBottom = 0;
                //    celCumple.BorderWidthLeft = 0;
                //    celCumple.BorderWidthTop = 0;
                //    celCumple.BorderWidthRight = 0;
                //    celCumple.HorizontalAlignment = Element.ALIGN_CENTER;
                //    tableLeyenda.AddCell(celCumple);
                //}


                if (formato == "1")
                {
                    doc.Add(jpg);
                    doc.Add(parrafo1);
                    doc.Add(espacio);
                    doc.Add(table);
                    doc.Add(espacio);
                    doc.Add(tableLeyenda);

                    doc.Add(espacio);

                    doc.Add(jpg);
                    doc.Add(parrafo1);
                    doc.Add(espacio);
                    doc.Add(table);
                    doc.Add(espacio);
                    doc.Add(tableLeyenda);

                    doc.NewPage();
                }
                else
                {
                    doc.Add(jpg);
                    doc.Add(parrafo1);
                    doc.Add(espacio);
                    doc.Add(table);
                    doc.Add(espacio);
                    doc.Add(tableLeyenda);
                    doc.NewPage();
                }

            }

            //FINALIZAMOS EL DOCUEMNTO
            doc.Close();
            writer.Close();
        }

        /// <summary>
        /// Método para generar archivo PDF.
        /// </summary>
        /// <param name="cadenaXML">Recibe la variable tipo string.</param>
        /// <param name="path">Recibe la variable tipo string.</param>
        /// <param name="formato">Recibe la variable tipo string.</param>
        /// <param name="idUnidadNegocio">Recibe la variable tipo int.</param>
        public void generaPDFTodos(string cadenaXML, string path, string formato, int idUnidadNegocio)
        {
            // Para saber que pintamos en la leyenda segun la unidad de negocio
            string leyenda = string.Empty;

            leyenda = "ACORDE AL ESTATUTO DEL SINDICATO LA CANTIDAD QUE ME CORRESPONDE POR APOYO/AYUDA SINDICAL SIN GENERAR RELACION LABORAL OTORGANDO FINIQUITO TOTAL.";

            //if (ExcluyeLeyendaSindicato(idUnidadNegocio))
            //{
            //    leyenda = "";
            //}

            //Cargamos el XML para leerlo
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(cadenaXML);


            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc,
                                        new FileStream(path, FileMode.Create));

            // Definimos los tipos de fuente
            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 10);

            // Para el cuerpo del recibo
            // Para el encabezado
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesEncabezado, 10);

            // Para la Leyenda Final
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontLeyenda = new Font(bfTimes, 7);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            // Abrimos el archivo
            doc.Open();

            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = 500f;
            table.LockedWidth = true;


            //SECCION DE ENCABEZADO DEL RECIBO
            PdfPCell NoEmpleado = new PdfPCell(new Phrase("NO.", fontEncabezado));
            NoEmpleado.Colspan = 1;
            table.AddCell(NoEmpleado);

            PdfPCell Nombre = new PdfPCell(new Phrase("NOMBRE", fontEncabezado));
            Nombre.Colspan = 5;
            table.AddCell(Nombre);

            PdfPCell rfc = new PdfPCell(new Phrase("RFC", fontEncabezado));
            rfc.Colspan = 2;
            table.AddCell(rfc);

            PdfPCell periodo = new PdfPCell(new Phrase("PERIODO", fontEncabezado));
            periodo.Colspan = 2;
            table.AddCell(periodo);

            //DATOS DEL ENCABEZADO
            XmlNodeList listaDatos = xmlDoc.SelectNodes("ReciboPago/DatosGenerales");
            XmlNode datos;
            for (int i = 0; i < listaDatos.Count; i++)
            {
                datos = listaDatos.Item(i);
                PdfPCell NoEmpleadoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("NumeroEmpleado").InnerText, fontEncabezado));
                NoEmpleadoDato.Colspan = 1;
                table.AddCell(NoEmpleadoDato);

                PdfPCell NombreDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Nombre").InnerText, fontEncabezado));
                NombreDato.Colspan = 5;
                table.AddCell(NombreDato);

                PdfPCell rfcDato = new PdfPCell(new Phrase(datos.SelectSingleNode("RFC").InnerText, fontEncabezado));
                rfcDato.Colspan = 2;
                table.AddCell(rfcDato);

                PdfPCell periodoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Periodo").InnerText, fontEncabezado));
                periodoDato.Colspan = 2;
                table.AddCell(periodoDato);
            }

            PdfPCell celPercepciones = new PdfPCell(new Phrase("PERCEPCIONES", fontEncabezado));
            celPercepciones.Colspan = 5;
            table.AddCell(celPercepciones);

            PdfPCell celDeducciones = new PdfPCell(new Phrase("DEDUCCIONES", fontEncabezado));
            celDeducciones.Colspan = 5;
            table.AddCell(celDeducciones);

            //DATOS DE LAS PERCEPCIONES
            PdfPTable PercepcionesTable = new PdfPTable(5);
            XmlNodeList listaPercepciones = xmlDoc.SelectNodes("ReciboPago/Percepciones/Percepcion");
            XmlNode percepciones;
            int auxiliarPercepciones = 10 - listaPercepciones.Count;
            decimal dtotalPercepciones = 0;

            for (int i = 0; i < listaPercepciones.Count; i++)
            {
                percepciones = listaPercepciones.Item(i);

                PdfPCell ClaveDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                ClaveDato.Colspan = 1;
                ClaveDato.BorderWidthBottom = 0;
                ClaveDato.BorderWidthLeft = 0;
                ClaveDato.BorderWidthTop = 0;
                ClaveDato.BorderWidthRight = 0.1F;
                PercepcionesTable.AddCell(ClaveDato);

                PdfPCell ConceptoDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                ConceptoDato.Colspan = 3;
                ConceptoDato.BorderWidthBottom = 0;
                ConceptoDato.BorderWidthLeft = 0;
                ConceptoDato.BorderWidthTop = 0;
                ConceptoDato.BorderWidthRight = 0.1F;
                PercepcionesTable.AddCell(ConceptoDato);

                PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                ImporteDato.Colspan = 1;
                ImporteDato.BorderWidthBottom = 0;
                ImporteDato.BorderWidthLeft = 0;
                ImporteDato.BorderWidthTop = 0;
                ImporteDato.BorderWidthRight = 0;
                ImporteDato.HorizontalAlignment = Element.ALIGN_RIGHT;
                PercepcionesTable.AddCell(ImporteDato);

                dtotalPercepciones = dtotalPercepciones + decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText);
            }

            if (auxiliarPercepciones > 0)
            {
                for (int i = 0; i < auxiliarPercepciones; i++)
                {
                    PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    PercepcionesTable.AddCell(ImporteDato);
                }
            }

            PdfPCell columasPercepciones = new PdfPCell(PercepcionesTable);
            columasPercepciones.Colspan = 5;
            columasPercepciones.Padding = 0f;
            table.AddCell(columasPercepciones);

            //DATOS DE LAS DEDUCCIONES
            PdfPTable DeduccionesTable = new PdfPTable(5);
            XmlNodeList listaDeducciones = xmlDoc.SelectNodes("ReciboPago/Deducciones/Deduccion");
            XmlNode deducciones;
            int auxiliarDeducciones = 10 - listaDeducciones.Count;
            decimal dtotalDeducciones = 0;

            for (int i = 0; i < listaDeducciones.Count; i++)
            {
                deducciones = listaDeducciones.Item(i);

                PdfPCell ClaveDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                ClaveDato.Colspan = 1;
                ClaveDato.BorderWidthBottom = 0;
                ClaveDato.BorderWidthLeft = 0;
                ClaveDato.BorderWidthTop = 0;
                ClaveDato.BorderWidthRight = 0.1F;
                DeduccionesTable.AddCell(ClaveDato);

                PdfPCell ConceptoDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                ConceptoDato.Colspan = 3;
                ConceptoDato.BorderWidthBottom = 0;
                ConceptoDato.BorderWidthLeft = 0;
                ConceptoDato.BorderWidthTop = 0;
                ConceptoDato.BorderWidthRight = 0.1F;
                DeduccionesTable.AddCell(ConceptoDato);

                PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                ImporteDato.Colspan = 1;
                ImporteDato.BorderWidthBottom = 0;
                ImporteDato.BorderWidthLeft = 0;
                ImporteDato.BorderWidthTop = 0;
                ImporteDato.BorderWidthRight = 0;
                DeduccionesTable.AddCell(ImporteDato);

                dtotalDeducciones = dtotalDeducciones + decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText); ;
            }

            if (auxiliarDeducciones > 0)
            {
                for (int i = 0; i < auxiliarDeducciones; i++)
                {
                    PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    DeduccionesTable.AddCell(ImporteDato);
                }
            }
            PdfPCell columasDeducciones = new PdfPCell(DeduccionesTable);
            columasDeducciones.Padding = 0f;
            columasDeducciones.Colspan = 5;
            table.AddCell(columasDeducciones);

            PdfPCell totalPercepciones = new PdfPCell(new Phrase("TOTAL PERCEPCIONES", fontEncabezado));
            totalPercepciones.Colspan = 4;
            totalPercepciones.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell totalPercepcionesImporte = new PdfPCell(new Phrase(dtotalPercepciones.ToString("c"), fontEncabezado));
            totalPercepcionesImporte.Colspan = 1;
            totalPercepcionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

            table.AddCell(totalPercepciones);
            table.AddCell(totalPercepcionesImporte);

            PdfPCell totalDeducciones = new PdfPCell(new Phrase("TOTAL DEDUCCIONES", fontEncabezado));
            totalDeducciones.Colspan = 4;
            totalDeducciones.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell totalDeduccionesImporte = new PdfPCell(new Phrase(dtotalDeducciones.ToString("c"), fontEncabezado));
            totalDeduccionesImporte.Colspan = 1;
            totalDeduccionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

            table.AddCell(totalDeducciones);
            table.AddCell(totalDeduccionesImporte);

            PdfPCell netoVacio = new PdfPCell(new Phrase(" "));
            netoVacio.Colspan = 7;
            netoVacio.BorderWidthBottom = 0;
            netoVacio.BorderWidthLeft = 0;
            netoVacio.BorderWidthTop = 0;
            netoVacio.BorderWidthRight = 0;
            table.AddCell(netoVacio);

            PdfPCell neto = new PdfPCell(new Phrase("NETO  " + (dtotalPercepciones - dtotalDeducciones).ToString("c"), fontEncabezado));
            neto.Colspan = 3;
            neto.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(neto);

            //SECCION PARA LA LEYENDA DE FIRMA
            PdfPTable tableLeyenda = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.LockedWidth = true;

            PdfPCell celdaLeyenda = new PdfPCell(new Phrase(leyenda, fontLeyenda));
            celdaLeyenda.Colspan = 4;
            celdaLeyenda.BorderWidthBottom = 0;
            celdaLeyenda.BorderWidthLeft = 0;
            celdaLeyenda.BorderWidthTop = 0;
            celdaLeyenda.BorderWidthRight = 0;
            tableLeyenda.AddCell(celdaLeyenda);

            PdfPTable tableFirma = new PdfPTable(1);

            PdfPCell raya = new PdfPCell(new Phrase("_____________________", fontLeyenda));
            raya.BorderWidthBottom = 0;
            raya.BorderWidthLeft = 0;
            raya.BorderWidthTop = 0;
            raya.BorderWidthRight = 0;

            PdfPCell firmaFinal = new PdfPCell(new Phrase("FIRMA", fontLeyenda));
            firmaFinal.BorderWidthBottom = 0;
            firmaFinal.BorderWidthLeft = 0;
            firmaFinal.BorderWidthTop = 0;
            firmaFinal.BorderWidthRight = 0;
            firmaFinal.HorizontalAlignment = Element.ALIGN_CENTER;

            tableFirma.AddCell(raya);
            tableFirma.AddCell(firmaFinal);

            PdfPCell celdaFirma = new PdfPCell(tableFirma);
            celdaFirma.Colspan = 1;
            celdaFirma.BorderWidthBottom = 0;
            celdaFirma.BorderWidthLeft = 0;
            celdaFirma.BorderWidthTop = 0;
            celdaFirma.BorderWidthRight = 0;
            tableLeyenda.AddCell(celdaFirma);

            //SI LA FECHA DE CUMPLEAÑOS DEL EMPLEADO ESTA DENTRO DEL PERIDO DE NOMINA
            //if (bLeyendaCumple == true)
            //{
            //    PdfPCell celCumple = new PdfPCell(new Phrase("TE DESEAMOS QUE CADA UNA DE LAS COSAS, GRANDES O PEQUEÑAS QUE TU CORAZÓN ANHELE, PUEDAN HACERSE REALIDAD. ¡FELIZ CUMPLEAÑOS!", fontLeyenda));
            //    celCumple.Colspan = 5;
            //    celCumple.BorderWidthBottom = 0;
            //    celCumple.BorderWidthLeft = 0;
            //    celCumple.BorderWidthTop = 0;
            //    celCumple.BorderWidthRight = 0;
            //    celCumple.HorizontalAlignment = Element.ALIGN_CENTER;
            //    tableLeyenda.AddCell(celCumple);
            //}


            if (formato == "1")
            {
                doc.Add(table);
                doc.Add(espacio);
                doc.Add(tableLeyenda);

                doc.Add(espacio);
                doc.Add(espacio);
                doc.Add(espacio);
                doc.Add(espacio);
                doc.Add(espacio);

                doc.Add(table);
                doc.Add(espacio);
                doc.Add(tableLeyenda);

            }
            else
            {
                doc.Add(table);
                doc.Add(espacio);
                doc.Add(tableLeyenda);
            }

            doc.Close();
            writer.Close();
        }
        
        /// <summary>
        /// Método para generar un recibo de finiquito en formato PDF.
        /// </summary>
        /// <param name="recibos"></param>
        /// <param name="path"></param>
        /// <param name="formato"></param>
        /// <param name="idUnidadNegocio"></param>
        /// <param name="pRecibosEspeciales"></param>
        public void GeneraRecibosUnArchivo(List<vRecibosEsquema> recibos, string path, string formato, int idUnidadNegocio, bool pRecibosEspeciales)
        {
            // Para saber que pintamos en la leyenda segun la unidad de negocio
            string leyenda = string.Empty;
            leyenda = "ACORDE AL ESTATUTO DEL SINDICATO LA CANTIDAD QUE ME CORRESPONDE POR APOYO/AYUDA SINDICAL SIN GENERAR RELACION LABORAL OTORGANDO FINIQUITO TOTAL.";

            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc,
                                        new FileStream(path, FileMode.Create));

            // Definimos los tipos de fuente
            // Para el encabezado
            BaseFont bfTimesEncabezado = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontEncabezado = new Font(bfTimesEncabezado, 10);

            // Para el cuerpo del recibo
            // Para el encabezado
            BaseFont bfTimesCuerpo = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontCuerpo = new Font(bfTimesEncabezado, 10);

            // Para la Leyenda Final
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font fontLeyenda = new Font(bfTimes, 7);

            //Definimos el espacio
            Paragraph espacio = new Paragraph(new Phrase(" "));

            // Abrimos el archivo
            doc.Open();


            foreach (var item in recibos)
            {///////Comienza ciclo para el archivo//////////////////
                //Cargamos el XML para leerlo
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(item.CadenaXML);

                PdfPTable table = new PdfPTable(10);
                table.TotalWidth = 500f;
                table.LockedWidth = true;


                //SECCION DE ENCABEZADO DEL RECIBO
                PdfPCell NoEmpleado = new PdfPCell(new Phrase("NO.", fontEncabezado));
                NoEmpleado.Colspan = 1;
                table.AddCell(NoEmpleado);

                PdfPCell Nombre = new PdfPCell(new Phrase("NOMBRE", fontEncabezado));
                Nombre.Colspan = 5;
                table.AddCell(Nombre);

                PdfPCell rfc = new PdfPCell(new Phrase("RFC", fontEncabezado));
                rfc.Colspan = 2;
                table.AddCell(rfc);

                PdfPCell periodo = new PdfPCell(new Phrase("PERIODO", fontEncabezado));
                periodo.Colspan = 2;
                table.AddCell(periodo);

                //DATOS DEL ENCABEZADO
                XmlNodeList listaDatos = xmlDoc.SelectNodes("ReciboPago/DatosGenerales");
                XmlNode datos;
                for (int i = 0; i < listaDatos.Count; i++)
                {
                    datos = listaDatos.Item(i);
                    PdfPCell NoEmpleadoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("NumeroEmpleado").InnerText, fontEncabezado));
                    NoEmpleadoDato.Colspan = 1;
                    table.AddCell(NoEmpleadoDato);

                    PdfPCell NombreDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Nombre").InnerText, fontEncabezado));
                    NombreDato.Colspan = 5;
                    table.AddCell(NombreDato);

                    PdfPCell rfcDato = new PdfPCell(new Phrase(datos.SelectSingleNode("RFC").InnerText, fontEncabezado));
                    rfcDato.Colspan = 2;
                    table.AddCell(rfcDato);

                    PdfPCell periodoDato = new PdfPCell(new Phrase(datos.SelectSingleNode("Periodo").InnerText, fontEncabezado));
                    periodoDato.Colspan = 2;
                    table.AddCell(periodoDato);
                }

                PdfPCell celPercepciones = new PdfPCell(new Phrase("PERCEPCIONES", fontEncabezado));
                celPercepciones.Colspan = 5;
                table.AddCell(celPercepciones);

                PdfPCell celDeducciones = new PdfPCell(new Phrase("DEDUCCIONES", fontEncabezado));
                celDeducciones.Colspan = 5;
                table.AddCell(celDeducciones);

                //DATOS DE LAS PERCEPCIONES
                PdfPTable PercepcionesTable = new PdfPTable(5);
                XmlNodeList listaPercepciones = xmlDoc.SelectNodes("ReciboPago/Percepciones/Percepcion");
                XmlNode percepciones;
                int auxiliarPercepciones = 10 - listaPercepciones.Count;
                decimal dtotalPercepciones = 0;

                for (int i = 0; i < listaPercepciones.Count; i++)
                {
                    percepciones = listaPercepciones.Item(i);
                    PdfPCell ConceptoDato;

                    PdfPCell ClaveDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ClaveDato);

                    if (pRecibosEspeciales)
                    {
                        ConceptoDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Concepto").InnerText.Replace("REMBOLSO_TRASPORTE S", "REMBOLSO_TRASPORTE S ( Bonos , Incentivos , Gratificaciones )").Replace("AYUDA_VESTIDO S", "AYUDA_VESTIDO S ( Horas Extras )"), fontCuerpo));
                    }
                    else
                    {
                        ConceptoDato = new PdfPCell(new Phrase(percepciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                    }

                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    PercepcionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    ImporteDato.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PercepcionesTable.AddCell(ImporteDato);

                    dtotalPercepciones = dtotalPercepciones + decimal.Parse(percepciones.SelectSingleNode("Importe").InnerText);
                }

                if (auxiliarPercepciones > 0)
                {
                    for (int i = 0; i < auxiliarPercepciones; i++)
                    {
                        PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                        ClaveDato.Colspan = 1;
                        ClaveDato.BorderWidthBottom = 0;
                        ClaveDato.BorderWidthLeft = 0;
                        ClaveDato.BorderWidthTop = 0;
                        ClaveDato.BorderWidthRight = 0.1F;
                        PercepcionesTable.AddCell(ClaveDato);

                        PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                        ConceptoDato.Colspan = 3;
                        ConceptoDato.BorderWidthBottom = 0;
                        ConceptoDato.BorderWidthLeft = 0;
                        ConceptoDato.BorderWidthTop = 0;
                        ConceptoDato.BorderWidthRight = 0.1F;
                        PercepcionesTable.AddCell(ConceptoDato);

                        PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                        ImporteDato.Colspan = 1;
                        ImporteDato.BorderWidthBottom = 0;
                        ImporteDato.BorderWidthLeft = 0;
                        ImporteDato.BorderWidthTop = 0;
                        ImporteDato.BorderWidthRight = 0;
                        PercepcionesTable.AddCell(ImporteDato);
                    }
                }

                PdfPCell columasPercepciones = new PdfPCell(PercepcionesTable);
                columasPercepciones.Colspan = 5;
                columasPercepciones.Padding = 0f;
                table.AddCell(columasPercepciones);

                //DATOS DE LAS DEDUCCIONES
                PdfPTable DeduccionesTable = new PdfPTable(5);
                XmlNodeList listaDeducciones = xmlDoc.SelectNodes("ReciboPago/Deducciones/Deduccion");
                XmlNode deducciones;
                int auxiliarDeducciones = 10 - listaDeducciones.Count;
                decimal dtotalDeducciones = 0;

                for (int i = 0; i < listaDeducciones.Count; i++)
                {
                    deducciones = listaDeducciones.Item(i);

                    PdfPCell ClaveDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Clave").InnerText, fontCuerpo));
                    ClaveDato.Colspan = 1;
                    ClaveDato.BorderWidthBottom = 0;
                    ClaveDato.BorderWidthLeft = 0;
                    ClaveDato.BorderWidthTop = 0;
                    ClaveDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ClaveDato);

                    PdfPCell ConceptoDato = new PdfPCell(new Phrase(deducciones.SelectSingleNode("Concepto").InnerText, fontCuerpo));
                    ConceptoDato.Colspan = 3;
                    ConceptoDato.BorderWidthBottom = 0;
                    ConceptoDato.BorderWidthLeft = 0;
                    ConceptoDato.BorderWidthTop = 0;
                    ConceptoDato.BorderWidthRight = 0.1F;
                    DeduccionesTable.AddCell(ConceptoDato);

                    PdfPCell ImporteDato = new PdfPCell(new Phrase(decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText).ToString("c"), fontCuerpo));
                    ImporteDato.Colspan = 1;
                    ImporteDato.BorderWidthBottom = 0;
                    ImporteDato.BorderWidthLeft = 0;
                    ImporteDato.BorderWidthTop = 0;
                    ImporteDato.BorderWidthRight = 0;
                    DeduccionesTable.AddCell(ImporteDato);

                    dtotalDeducciones = dtotalDeducciones + decimal.Parse(deducciones.SelectSingleNode("Importe").InnerText); ;
                }

                if (auxiliarDeducciones > 0)
                {
                    for (int i = 0; i < auxiliarDeducciones; i++)
                    {
                        PdfPCell ClaveDato = new PdfPCell(new Phrase(" "));
                        ClaveDato.Colspan = 1;
                        ClaveDato.BorderWidthBottom = 0;
                        ClaveDato.BorderWidthLeft = 0;
                        ClaveDato.BorderWidthTop = 0;
                        ClaveDato.BorderWidthRight = 0.1F;
                        DeduccionesTable.AddCell(ClaveDato);

                        PdfPCell ConceptoDato = new PdfPCell(new Phrase(" "));
                        ConceptoDato.Colspan = 3;
                        ConceptoDato.BorderWidthBottom = 0;
                        ConceptoDato.BorderWidthLeft = 0;
                        ConceptoDato.BorderWidthTop = 0;
                        ConceptoDato.BorderWidthRight = 0.1F;
                        DeduccionesTable.AddCell(ConceptoDato);

                        PdfPCell ImporteDato = new PdfPCell(new Phrase(" "));
                        ImporteDato.Colspan = 1;
                        ImporteDato.BorderWidthBottom = 0;
                        ImporteDato.BorderWidthLeft = 0;
                        ImporteDato.BorderWidthTop = 0;
                        ImporteDato.BorderWidthRight = 0;
                        DeduccionesTable.AddCell(ImporteDato);
                    }
                }
                PdfPCell columasDeducciones = new PdfPCell(DeduccionesTable);
                columasDeducciones.Padding = 0f;
                columasDeducciones.Colspan = 5;
                table.AddCell(columasDeducciones);

                PdfPCell totalPercepciones = new PdfPCell(new Phrase("TOTAL PERCEPCIONES", fontEncabezado));
                totalPercepciones.Colspan = 4;
                totalPercepciones.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell totalPercepcionesImporte = new PdfPCell(new Phrase(dtotalPercepciones.ToString("c"), fontEncabezado));
                totalPercepcionesImporte.Colspan = 1;
                totalPercepcionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

                table.AddCell(totalPercepciones);
                table.AddCell(totalPercepcionesImporte);

                PdfPCell totalDeducciones = new PdfPCell(new Phrase("TOTAL DEDUCCIONES", fontEncabezado));
                totalDeducciones.Colspan = 4;
                totalDeducciones.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell totalDeduccionesImporte = new PdfPCell(new Phrase(dtotalDeducciones.ToString("c"), fontEncabezado));
                totalDeduccionesImporte.Colspan = 1;
                totalDeduccionesImporte.HorizontalAlignment = Element.ALIGN_RIGHT;

                table.AddCell(totalDeducciones);
                table.AddCell(totalDeduccionesImporte);

                PdfPCell netoVacio = new PdfPCell(new Phrase(" "));
                netoVacio.Colspan = 7;
                netoVacio.BorderWidthBottom = 0;
                netoVacio.BorderWidthLeft = 0;
                netoVacio.BorderWidthTop = 0;
                netoVacio.BorderWidthRight = 0;
                table.AddCell(netoVacio);

                PdfPCell neto = new PdfPCell(new Phrase("NETO  " + (dtotalPercepciones - dtotalDeducciones).ToString("c"), fontEncabezado));
                neto.Colspan = 3;
                neto.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(neto);

                //SECCION PARA LA LEYENDA DE FIRMA
                PdfPTable tableLeyenda = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.LockedWidth = true;

                PdfPCell celdaLeyenda = new PdfPCell(new Phrase(leyenda, fontLeyenda));
                celdaLeyenda.Colspan = 4;
                celdaLeyenda.BorderWidthBottom = 0;
                celdaLeyenda.BorderWidthLeft = 0;
                celdaLeyenda.BorderWidthTop = 0;
                celdaLeyenda.BorderWidthRight = 0;
                tableLeyenda.AddCell(celdaLeyenda);

                PdfPTable tableFirma = new PdfPTable(1);

                PdfPCell raya = new PdfPCell(new Phrase("_____________________", fontLeyenda));
                raya.BorderWidthBottom = 0;
                raya.BorderWidthLeft = 0;
                raya.BorderWidthTop = 0;
                raya.BorderWidthRight = 0;

                PdfPCell firmaFinal = new PdfPCell(new Phrase("FIRMA", fontLeyenda));
                firmaFinal.BorderWidthBottom = 0;
                firmaFinal.BorderWidthLeft = 0;
                firmaFinal.BorderWidthTop = 0;
                firmaFinal.BorderWidthRight = 0;
                firmaFinal.HorizontalAlignment = Element.ALIGN_CENTER;

                tableFirma.AddCell(raya);
                tableFirma.AddCell(firmaFinal);

                PdfPCell celdaFirma = new PdfPCell(tableFirma);
                celdaFirma.Colspan = 1;
                celdaFirma.BorderWidthBottom = 0;
                celdaFirma.BorderWidthLeft = 0;
                celdaFirma.BorderWidthTop = 0;
                celdaFirma.BorderWidthRight = 0;
                tableLeyenda.AddCell(celdaFirma);

                //SI LA FECHA DE CUMPLEAÑOS DEL EMPLEADO ESTA DENTRO DEL PERIDO DE NOMINA
                //if (bLeyendaCumple == true)
                //{
                //    PdfPCell celCumple = new PdfPCell(new Phrase("TE DESEAMOS QUE CADA UNA DE LAS COSAS, GRANDES O PEQUEÑAS QUE TU CORAZÓN ANHELE, PUEDAN HACERSE REALIDAD. ¡FELIZ CUMPLEAÑOS!", fontLeyenda));
                //    celCumple.Colspan = 5;
                //    celCumple.BorderWidthBottom = 0;
                //    celCumple.BorderWidthLeft = 0;
                //    celCumple.BorderWidthTop = 0;
                //    celCumple.BorderWidthRight = 0;
                //    celCumple.HorizontalAlignment = Element.ALIGN_CENTER;
                //    tableLeyenda.AddCell(celCumple);
                //}


                if (formato == "1")
                {
                    doc.Add(espacio);
                    doc.Add(table);
                    doc.Add(espacio);
                    doc.Add(tableLeyenda);

                    doc.Add(espacio);
                    doc.Add(espacio);
                    doc.Add(espacio);
                    doc.Add(espacio);
                    doc.Add(espacio);

                    doc.Add(table);
                    doc.Add(espacio);
                    doc.Add(tableLeyenda);
                    doc.NewPage();
                }
                else
                {
                    doc.Add(espacio);
                    doc.Add(table);
                    doc.Add(espacio);
                    doc.Add(tableLeyenda);
                    doc.NewPage();
                }

            }

            //FINALIZAMOS EL DOCUEMNTO
            doc.Close();
            writer.Close();
        }

        /// <summary>
        /// Método para obtener los recibos esquema.
        /// </summary>
        /// <param name="_idPeriodoNomina">Recibe la variable tipo int.</param>
        /// <returns>Regresa la lista de los recibos esquema.</returns>
        public List<vRecibosEsquema> GetvRecibosEsquema(int _idPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                var query = (from a in ctx.vRecibosEsquema.Where(x => x.IdPeriodoNomina == _idPeriodoNomina) select a).ToList();

                return query;
            }
        }

        /// <summary>
        /// Método para generar un archivo en formato ZIP.
        /// </summary>
        /// <param name="items">Recibe una lista de strings.</param>
        /// <param name="destination">Recibe una variable tipo string.</param>
        public void CreateZipFile(List<string> items, string destination)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                foreach (string item in items)
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
                zip.Save(destination);
            }
        }

        /// <summary>
        /// Método para obtener el nombre del periodo de la nómina.
        /// </summary>
        /// <param name="_idPeriodoNomina">Recibe una variable tipo string.</param>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public string GetNamePeriodo(int _idPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.PeriodoNomina.Where(x => x.IdPeriodoNomina == _idPeriodoNomina) select a.Periodo).First();
            }
        }

        /// <summary>
        /// Método para listar los periodos de nómina con sus distintos estatus.
        /// </summary>
        /// <param name="_IdUnidadNegocio">Recibe una variable tipo int.</param>
        /// <returns>Regresa la lista de los periodos de nómina con los status.</returns>
        public List<ModelPeriodoNomina> GetPeriodosDifStatus(int _IdUnidadNegocio)
        {
            using (NominaEntities1 _ctx = new NominaEntities1())
            {
                return (from x in _ctx.PeriodoNomina
                        where x.IdUnidadNegocio == _IdUnidadNegocio && (x.IdEstatus == 2 || x.IdEstatus == 1 || x.IdEstatus == 3)
                        select new ModelPeriodoNomina
                        {
                            IdPeriodoNomina = x.IdPeriodoNomina,
                            IdUnidadNegocio = x.IdUnidadNegocio,
                            Periodo = x.Periodo,
                            TipoNomina = x.TipoNomina,
                            FechaInicio = x.FechaInicio.ToString(),
                            FechaFin = x.FechaFin.ToString(),
                            AjusteImpuestos = x.AjusteDeImpuestos,
                            IdsPeriodosAjuste = x.SeAjustaraConPeriodo,
                            Observaciones = x.Observaciones
                        }).ToList();
            }
        }

    }
}
  