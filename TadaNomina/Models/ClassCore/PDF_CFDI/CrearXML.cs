using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace TadaNomina.Models.ClassCore.PDF_CFDI
{
    public class CrearXML
    {
        public void crearXML(string xml, string ruta)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(ruta, settings))
            {
                doc.Save(writer);
            }
        }
    }
}