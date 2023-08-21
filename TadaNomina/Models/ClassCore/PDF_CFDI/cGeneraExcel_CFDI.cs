
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TadaNomina.Models.ClassCore.Timbrado;
using static ClosedXML.Excel.XLPredefinedFormat;


namespace TadaNomina.Models.ClassCore.PDF_CFDI
{
    public class cGeneraExcel_CFDI
    {       
        public System.Data.DataTable generaExcel(int IdPeriodoNomina)
        {
            ClassTimbradoNomina ct = new ClassTimbradoNomina();
            var info = ct.GetTimbrados(IdPeriodoNomina);

            System.Data.DataTable tabla = new System.Data.DataTable();
            DataColumn column;           
            DataRow row;

            column = new DataColumn("NombreEmisor");
            tabla.Columns.Add(column);

            column = new DataColumn("RFCEmisor");
            tabla.Columns.Add(column);

            column = new DataColumn("RegimenFiscalEmisor");
            tabla.Columns.Add(column);

            column = new DataColumn("RegistroPatronal");
            tabla.Columns.Add(column);

            column = new DataColumn("NumeroEmpleado");
            tabla.Columns.Add(column);

            column = new DataColumn("Nombre");
            tabla.Columns.Add(column);

            column = new DataColumn("CURP");
            tabla.Columns.Add(column);

            column = new DataColumn("RFC");
            tabla.Columns.Add(column);

            column = new DataColumn("NSS");
            tabla.Columns.Add(column);
            
            column = new DataColumn("FechaInicial");
            tabla.Columns.Add(column);
            
            column = new DataColumn("FechaFinal");
            tabla.Columns.Add(column);
            
            column = new DataColumn("FechaPago");
            tabla.Columns.Add(column);            
            
            column = new DataColumn("UUID");
            tabla.Columns.Add(column);
            
            column = new DataColumn("LugarExpedicion");
            tabla.Columns.Add(column);
            
            column = new DataColumn("FechaCertificación");
            tabla.Columns.Add(column);
            
            column = new DataColumn("FechaTimbrado");
            tabla.Columns.Add(column);
            
            column = new DataColumn("NoCertificadoSAT");
            tabla.Columns.Add(column);
            
            column = new DataColumn("NoCertificado");
            tabla.Columns.Add(column);
            
            column = new DataColumn("FormaPago");
            tabla.Columns.Add(column);
            
            column = new DataColumn("Serie");
            tabla.Columns.Add(column);
            
            column = new DataColumn("SelloCFD");
            tabla.Columns.Add(column);
            
            column = new DataColumn("SelloSAT");
            tabla.Columns.Add(column);            

            column = new DataColumn("TotalPercepciones");
            tabla.Columns.Add(column);

            column = new DataColumn("TotalOtrosPagos");
            tabla.Columns.Add(column);

            column = new DataColumn("TotalDeducciones");
            tabla.Columns.Add(column);
            
            column = new DataColumn("SubTotal");
            tabla.Columns.Add(column);
            
            column = new DataColumn("Descuento");
            tabla.Columns.Add(column);
            
            column = new DataColumn("Total");
            tabla.Columns.Add(column);
            
            column = new DataColumn("Moneda");
            tabla.Columns.Add(column);
            
            column = new DataColumn("TipoDeComprobante");
            tabla.Columns.Add(column);
            
            column = new DataColumn("Version");
            tabla.Columns.Add(column);

            column = new DataColumn("DiasPago");
            tabla.Columns.Add(column);

            foreach (var item in info)
            {
                string xml = item.CFDI_Timbrado;
                XElement l = XElement.Parse(xml.Trim());
                byte[] byteArray = Encoding.UTF8.GetBytes(xml.Trim());

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(l.ToString());

                XDocument xmlInput = null;
                XNamespace df;

                xmlInput = XDocument.Parse(l.ToString());
                df = xmlInput.Root.Name.Namespace;
                XNamespace nomin = XNamespace.Get("http://www.sat.gob.mx/nomina12");
                XNamespace timbra = XNamespace.Get("http://www.sat.gob.mx/TimbreFiscalDigital");

                row = tabla.NewRow();
                               
                //Emisor
                row["NombreEmisor"] = xmlInput.Root.Element(df + "Emisor").Attribute("Nombre").Value;
                row["RFCEmisor"] = xmlInput.Root.Element(df + "Emisor").Attribute("Rfc").Value;
                row["RegimenFiscalEmisor"] = xmlInput.Root.Element(df + "Emisor").Attribute("RegimenFiscal").Value;
                row["RegistroPatronal"] = xmlInput.Descendants(nomin + "Emisor").Attributes("RegistroPatronal").ToList()[0].Value;

                ////Receptor
                row["NumeroEmpleado"] = xmlInput.Descendants(nomin + "Receptor").Attributes("NumEmpleado").ToList()[0].Value;
                row["Nombre"] = xmlInput.Root.Element(df + "Receptor").Attribute("Nombre").Value;
                row["CURP"] = xmlInput.Descendants(nomin + "Receptor").Attributes("Curp").ToList()[0].Value;
                row["RFC"] = xmlInput.Root.Element(df + "Receptor").Attribute("Rfc").Value;
                try { row["NSS"] = xmlInput.Descendants(nomin + "Receptor").Attributes("NumSeguridadSocial").ToList()[0].Value; } catch { row["NSS"] = "N/A"; }
                row["FechaInicial"] = xmlInput.Descendants(nomin + "Nomina").Attributes("FechaInicialPago").ToList()[0].Value;
                row["FechaFinal"] = xmlInput.Descendants(nomin + "Nomina").Attributes("FechaFinalPago").ToList()[0].Value;
                row["FechaPago"] = xmlInput.Descendants(nomin + "Nomina").Attributes("FechaPago").ToList()[0].Value;                

                //datos timbrado
                row["UUID"] = xmlInput.Descendants(timbra + "TimbreFiscalDigital").Attributes("UUID").ToList()[0].Value;
                row["LugarExpedicion"] = xmlInput.Descendants(df + "Comprobante").Attributes("LugarExpedicion").ToList()[0].Value;
                row["FechaCertificación"] = xmlInput.Descendants(df + "Comprobante").Attributes("Fecha").ToList()[0].Value;
                row["FechaTimbrado"] = xmlInput.Descendants(timbra + "TimbreFiscalDigital").Attributes("FechaTimbrado").ToList()[0].Value;
                row["NoCertificadoSAT"] = xmlInput.Descendants(timbra + "TimbreFiscalDigital").Attributes("NoCertificadoSAT").ToList()[0].Value;
                row["NoCertificado"] = xmlInput.Descendants(df + "Comprobante").Attributes("NoCertificado").ToList()[0].Value;
                row["FormaPago"] = xmlInput.Descendants(df + "Comprobante").Attributes("FormaPago").ToList()[0].Value;
                row["Serie"] = xmlInput.Descendants(df + "Comprobante").Attributes("Serie").ToList()[0].Value;
                row["SelloCFD"] = xmlInput.Descendants(timbra + "TimbreFiscalDigital").Attributes("SelloCFD").ToList()[0].Value;
                row["SelloSAT"] = xmlInput.Descendants(timbra + "TimbreFiscalDigital").Attributes("SelloSAT").ToList()[0].Value; row["SubTotal"] = xmlInput.Descendants(df + "Comprobante").Attributes("SubTotal").ToList()[0].Value;
                row["Descuento"] = xmlInput.Descendants(df + "Comprobante").Attributes("Descuento").ToList()[0].Value;
                row["Total"] = xmlInput.Descendants(df + "Comprobante").Attributes("Total").ToList()[0].Value;
                row["Moneda"] = xmlInput.Descendants(df + "Comprobante").Attributes("Moneda").ToList()[0].Value;
                row["TipoDeComprobante"] = xmlInput.Descendants(df + "Comprobante").Attributes("TipoDeComprobante").ToList()[0].Value;
                row["Version"] = xmlInput.Descendants(df + "Comprobante").Attributes("Version").ToList()[0].Value;

                //Percepciones y Deducciones
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("ab", "http://www.sat.gob.mx/nomina12");
                XmlNodeList listaPercepciones = xmlDoc.SelectNodes("//ab:Percepcion", nsmgr);
                XmlNodeList listaOtrosPagos = xmlDoc.SelectNodes("//ab:OtroPago", nsmgr);
                XmlNodeList listaOtrosPagosSubsidio = xmlDoc.SelectNodes("//ab:SubsidioAlEmpleo", nsmgr);
                XmlNodeList listaDeducciones = xmlDoc.SelectNodes("//ab:Deduccion", nsmgr);
                nodosOrdenados no = new nodosOrdenados();

                row["DiasPago"] = xmlInput.Descendants(nomin + "Nomina").Attributes("NumDiasPagados").ToList()[0].Value;

                foreach (var iper in no.ordenaNodosXML(listaPercepciones, "Percepciones"))
                {
                    if (!tabla.Columns.Contains(iper.TipoConcepto + "-" + iper.Concepto))
                    {
                        column = new DataColumn(iper.TipoConcepto + "-" + iper.Concepto);
                        tabla.Columns.Add(column);
                    }                    

                    decimal totalPercepcion = decimal.Parse(iper.ImporteExento) + decimal.Parse(iper.ImporteGravado);
                    row[iper.TipoConcepto + "-" + iper.Concepto] = totalPercepcion.ToString();
                }                

                row["TotalPercepciones"] = xmlInput.Descendants(nomin + "Nomina").Attributes("TotalPercepciones").ToList()[0].Value;

                foreach (var iOtros in no.ordenaNodosXML(listaOtrosPagos, "OtrosPagos"))
                {
                    decimal totalOtros = decimal.Parse(iOtros.Importe);
                    if (!tabla.Columns.Contains(iOtros.TipoConcepto + "-" + iOtros.Concepto))
                    {
                        column = new DataColumn(iOtros.TipoConcepto + "-" + iOtros.Concepto);
                        tabla.Columns.Add(column);
                    }
                                        
                    row[iOtros.TipoConcepto + "-" + iOtros.Concepto] = totalOtros.ToString();
                }
                                
                row["TotalOtrosPagos"] = xmlInput.Descendants(nomin + "Nomina").Attributes("TotalOtrosPagos").ToList()[0].Value;

                foreach (var idec in no.ordenaNodosXML(listaDeducciones, "Deducciones"))
                {
                    decimal totalDeduccionesSinIsr = 0;
                    decimal totalDeduccion = decimal.Parse(idec.Importe);
                    decimal isr = 0;
                    if (idec.TipoConcepto != "002")
                        totalDeduccionesSinIsr += totalDeduccion;
                    else
                        isr += totalDeduccion;

                    if (!tabla.Columns.Contains(idec.TipoConcepto + "-" + idec.Concepto))
                    {
                        column = new DataColumn(idec.TipoConcepto + "-" + idec.Concepto);
                        tabla.Columns.Add(column);
                    }
                                        
                    row[idec.TipoConcepto + "-" + idec.Concepto] = totalDeduccion.ToString();
                }
                               
                row["TotalDeducciones"] = xmlInput.Descendants(nomin + "Nomina").Attributes("TotalDeducciones").ToList()[0].Value;   

                tabla.Rows.Add(row);
            }

            return tabla;
        }

        public dynamic getDataRow(XElement ElemtoLinq)
        {
            XDocument xmlInput = null;
            XNamespace df;

            xmlInput = XDocument.Parse(ElemtoLinq.ToString());
            df = xmlInput.Root.Name.Namespace;
            XNamespace nomin = XNamespace.Get("http://www.sat.gob.mx/nomina12");
            XNamespace timbra = XNamespace.Get("http://www.sat.gob.mx/TimbreFiscalDigital");

            var result = new { 
                //Emisor
                NombreEmisor = xmlInput.Root.Element(df + "Emisor").Attribute("Nombre").Value,
                RFCEmisor = xmlInput.Root.Element(df + "Emisor").Attribute("Rfc").Value,
                RegimenFiscalEmisor = xmlInput.Root.Element(df + "Emisor").Attribute("RegimenFiscal").Value,
                RegistroPatronal = xmlInput.Descendants(nomin + "Emisor").Attributes("RegistroPatronal").ToList()[0].Value ?? "N/A",
                //Receptor
                NumeroEmpleado = xmlInput.Descendants(nomin + "Receptor").Attributes("NumEmpleado").ToList()[0].Value,
                Nombre = xmlInput.Root.Element(df + "Receptor").Attribute("Nombre").Value,
                CURP = xmlInput.Descendants(nomin + "Receptor").Attributes("Curp").ToList()[0].Value,
            };

            return result;
        }
    }
}