using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Web;
using System.Xml;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class cCreaXMLCancelacion
    {
        public string getXml(string RfcEmisor, string UUID, string CveMotivo, string UUIDSustitucion)
        {
            string xml = "<Cancelacion xmlns='http://cancelacfd.sat.gob.mx' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'" +
                " Fecha='" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "' RfcEmisor='" + RfcEmisor + "'>" +
                " <Folios>";

            xml += " <Folio UUID='" + UUID + "' Motivo='" + CveMotivo + "' FolioSustitucion='" + UUIDSustitucion + "' />";

            xml += " </Folios>" +
                " </Cancelacion>";

            return xml;
        }

        public string getXMLCancelacionSignature(string RfcEmisor, string UUID, string CveMotivo, string UUIDSustitucion, byte[] archvivopfx, string pfxPassword)
        {
            XmlDocument originalXmlDocument = new XmlDocument();
            try { originalXmlDocument.LoadXml(getXml(RfcEmisor, UUID, CveMotivo, UUIDSustitucion)); } catch (Exception ex) { throw new Exception("No se pudo crear el xml principal. " + ex.Message); }

            XmlElement signatureElement;
            try { signatureElement = GenerateXmlSignature(originalXmlDocument, archvivopfx, pfxPassword); } catch (Exception ex) { throw new Exception("No se pudo crear el XML Signature Error en el proceso GenerateXmlSignature." + ex.Message); }

            originalXmlDocument.DocumentElement.AppendChild(originalXmlDocument.ImportNode(signatureElement, true));

            var res = originalXmlDocument.OuterXml;

            return res;
        }

        public static XmlElement GenerateXmlSignature(XmlDocument originalXmlDocument, byte[] pfx, String pfxPassword)
        {
            if (pfx.Length == 0) { throw new Exception("El archivo PFX no es valido."); }
            X509Certificate2 cert = new X509Certificate2(pfx, pfxPassword);
            RSA pKey = cert.GetRSAPrivateKey();
            SignedXml signedXml;
            try { signedXml = new SignedXml(originalXmlDocument) { SigningKey = pKey }; } catch (Exception ex) { throw new Exception("No se pudo cargar el documento original dentro del proceso GenerateXmlSignature." + ex.Message); }
            Reference reference = new Reference() { Uri = String.Empty };
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            KeyInfoX509Data kdata = new KeyInfoX509Data(cert);
            kdata.AddIssuerSerial(cert.Issuer, cert.SerialNumber);
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(kdata);
            signedXml.KeyInfo = keyInfo;
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            return signedXml.GetXml();
        }
    }
}