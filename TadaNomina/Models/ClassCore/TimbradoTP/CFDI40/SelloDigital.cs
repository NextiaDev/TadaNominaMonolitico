using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace XSDToXML.Utils
{
    public class SelloDigital
    {
        /// <summary>
        /// Metodo para regresar sello digital con Claveprivada en String
        /// </summary>
        /// <param name="CadenaOriginal">Cadena</param>
        /// <param name="ArchivoClavePrivada">Clave privada</param>
        /// <param name="lPassword">Clave</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string Sellar(string CadenaOriginal, string ArchivoClavePrivada, string lPassword, int IdRegistro, string NombrePatrona)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;

            SecureString lSecStr = new SecureString();
            SHA256Managed sham = new SHA256Managed();
            
            // SHA1Managed sham = new SHA1Managed(); version 3.2
            lSecStr.Clear();

            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);

            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            try
            {
                bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);
            }
            catch (Exception ex)
            {
                throw new NullReferenceException("Clave privada incorrecta (" + NombrePatrona + "), IdR=" + IdRegistro + " revisa que la contraseña corresponde a los sellos digitales cargados: " + ex.Message);
            }
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }

        /// <summary>
        /// metodo que realiza el sello reciviendo el archivo key como matriaz de bytes
        /// </summary>
        /// <param name="CadenaOriginal">Cadena</param>
        /// <param name="ArchivoClavePrivada">Clave privada</param>
        /// <param name="lPassword">Clave</param>
        /// <returns></returns>
        public string Sellar(string CadenaOriginal, byte[] ArchivoClavePrivada, string lPassword)
        {
            byte[] ClavePrivada = ArchivoClavePrivada;
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;

            SecureString lSecStr = new SecureString();
            SHA256Managed sham = new SHA256Managed();
            lSecStr.Clear();

            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);

            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            try
            {
                bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Clave privada incorrecta.");
            }
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }

        /// <summary>
        /// Metodo para verificar sello digital con seguridad SHA1
        /// </summary>
        /// <param name="CadenaOriginal"></param>
        /// <param name="ArchivoClavePrivada"></param>
        /// <param name="lPassword"></param>
        /// <param name="ArchivoClavePublica"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public bool verificarSello(string CadenaOriginal, string ArchivoClavePrivada, string lPassword, string ArchivoClavePublica)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;

            SecureString lSecStr = new SecureString();
            SHA1Managed sham = new SHA1Managed();
            lSecStr.Clear();

            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);

            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            try
            {
                bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Clave privada incorrecta.");
            }

            string sellodigital = Convert.ToBase64String(bytesFirmados);

            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            //rsaCSP.ImportParameters(rsaParams);
            //rsaCSP = JavaScience.opensslkey.(File.ReadAllBytes(ArchivoClavePublica));
            bool dataOK = rsaCSP.VerifyData(Encoding.UTF8.GetBytes(CadenaOriginal), CryptoConfig.MapNameToOID("SHA1"), bytesFirmados);
            hashedData = hash.ComputeHash(bytesFirmados);
            return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), Encoding.UTF8.GetBytes(CadenaOriginal));
        }//*/

        /// <summary>
        /// Metodo para comprovar la integridad del sello digital con algoritmo MD5
        /// </summary>
        /// <param name="CadenaOriginal"></param>
        /// <param name="ArchivoClavePrivada"></param>
        /// <param name="lPassword"></param>
        /// <returns></returns>
        public string SellarMD5(string CadenaOriginal, string ArchivoClavePrivada, string lPassword)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;
            SecureString lSecStr = new SecureString();
            lSecStr.Clear();
            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);
            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            hasher.ComputeHash(bCadenaOriginal);
            bytesFirmados = lrsa.SignData(bCadenaOriginal, hasher);
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }

        /// <summary>
        /// Metodo para convertir a base 64 la cadena del archivo CER
        /// </summary>
        /// <param name="ArchivoCER">Cadena del archivo CER</param>
        /// <returns></returns>
        public string Certificado(string ArchivoCER)
        {
            byte[] Certificado = File.ReadAllBytes(ArchivoCER);
            return Base64_Encode(Certificado);
        }

        /// <summary>
        /// Metodo para convertir en base 64 el arreglo de bytes del archivo CER
        /// </summary>
        /// <param name="ArchivoCER">Arreglo de bytes del archivo CER</param>
        /// <returns></returns>
        public string Certificado(byte[] ArchivoCER)
        {
            return Base64_Encode(ArchivoCER);
        }

        /// <summary>
        /// Metodo para convertir en base 64 una cadena de bytes
        /// </summary>
        /// <param name="str">cadena de bytes</param>
        /// <returns></returns>
        string Base64_Encode(byte[] str)
        {
            return Convert.ToBase64String(str);
        }

        /// <summary>
        /// Metodo para decodificar de base 64 un string
        /// </summary>
        /// <param name="str">Parámetro a decodificar</param>
        /// <returns></returns>
        byte[] Base64_Decode(string str)
        {
            try
            {
                byte[] decbuff = Convert.FromBase64String(str);
                return decbuff;
            }
            catch
            {
                { return null; }
            }
        }

        /// <summary>
        /// Metodo para transformar un archivo xml en base al xslt y se almacena en un string
        /// </summary>
        /// <param name="NombreXML">XML</param>
        /// <returns></returns>
        public static string getCadenaOriginal(string NombreXML)
        {
            System.Xml.Xsl.XslCompiledTransform transformer = new System.Xml.Xsl.XslCompiledTransform();
            //Encoding utf8 = Encoding.UTF8;
            //byte[] encodedBytes;
            StringWriter strwriter = new StringWriter();
            if (File.Exists("cadenaoriginal_3_3.xslt"))
            {
                //cargamos el xslt transformer
                try
                {
                    transformer.Load("cadenaoriginal_3_3.xslt");
                    //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
                    transformer.Transform(NombreXML, null, strwriter);
                    //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
                    //encodedBytes = utf8.GetBytes(strwriter.ToString());
                    return strwriter.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (File.Exists("cadenaoriginal_3_3.xslt"))
            {
                //cargamos el xslt transformer
                try
                {
                    transformer.Load("cadenaoriginal_3_3.xslt");
                    //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
                    transformer.Transform(NombreXML, null, strwriter);
                    //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
                    //encodedBytes = utf8.GetBytes(strwriter.ToString());
                    return strwriter.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw e;
                }
            }
            else return "Error al cargar el validador.";


        }

        /// <summary>
        /// Metodo para comprovar la integridad de los datos con algoritmo MD5
        /// </summary>
        /// <param name="Value">Valor a comprovar</param>
        /// <returns></returns>
        public static string md5(string Value)
        {
          

            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = Encoding.UTF8.GetBytes(Value);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            string ret = "";
            for (int i = 0; i < encodedBytes.Length; i++)
                ret += encodedBytes[i].ToString("x2").ToLower();
            return ret;

        }

        /// <summary>
        /// Metodo para leer el codigo del certificado enviado
        /// </summary>
        /// <param name="NombreArchivo"></param>
        /// <param name="Inicio"></param>
        /// <param name="Final"></param>
        /// <param name="Serie"></param>
        /// <param name="Numero"></param>
        /// <returns></returns>
        public static bool leerCER(string NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero)
        {

            if (NombreArchivo.Length < 1)
            {
                Inicio = "";
                Final = "";
                Serie = "INVALIDO";
                Numero = "";
                return false;
            }
            X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
            StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

            //Detalle
            objSB.AppendLine("Persona = " + objCert.Subject);
            objSB.AppendLine("Emisor = " + objCert.Issuer);
            objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
            Inicio = objCert.NotBefore.ToString();
            objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
            Final = objCert.NotAfter.ToString();
            objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
            objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
            Serie = objCert.SerialNumber.ToString();

            objSB.AppendLine("Hash = " + objCert.Thumbprint);
            //Numero = "?";
            string tNumero = "", rNumero = "", tNumero2 = "";

            int X;
            if (Serie.Length < 2)
                Numero = "";
            else
            {
                foreach (char c in Serie)
                {
                    switch (c)
                    {
                        case '0': tNumero += c; break;
                        case '1': tNumero += c; break;
                        case '2': tNumero += c; break;
                        case '3': tNumero += c; break;
                        case '4': tNumero += c; break;
                        case '5': tNumero += c; break;
                        case '6': tNumero += c; break;
                        case '7': tNumero += c; break;
                        case '8': tNumero += c; break;
                        case '9': tNumero += c; break;
                    }
                }
                for (X = 1; X < tNumero.Length; X++)
                {
                    //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
                    X += 1;
                    //rNumero = rNumero + 
                    tNumero2 = tNumero.Substring(0, X);
                    rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
                }
                Numero = rNumero;

            }

            if (DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore)
            {
                return true;
            }


         
            return false;
        }

        /// <summary>
        /// lee el codigo del certificado enviado este como matriz de bytes
        /// </summary>
        /// <param name="NombreArchivo">certificado en matriz de bytes</param>
        /// <param name="Inicio"></param>
        /// <param name="Final"></param>
        /// <param name="Serie"></param>
        /// <param name="Numero"></param>
        /// <returns></returns>
        public static bool leerCER(byte[] NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero)
        {

            if (NombreArchivo.Length < 1)
            {
                Inicio = "";
                Final = "";
                Serie = "INVALIDO";
                Numero = "";
                return false;
            }
            X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
            StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

            //Detalle
            objSB.AppendLine("Persona = " + objCert.Subject);
            objSB.AppendLine("Emisor = " + objCert.Issuer);
            objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
            Inicio = objCert.NotBefore.ToString();
            objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
            Final = objCert.NotAfter.ToString();
            objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
            objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
            Serie = objCert.SerialNumber.ToString();

            objSB.AppendLine("Hash = " + objCert.Thumbprint);
            //Numero = "?";
            string tNumero = "", rNumero = "", tNumero2 = "";

            int X;
            if (Serie.Length < 2)
                Numero = "";
            else
            {
                foreach (char c in Serie)
                {
                    switch (c)
                    {
                        case '0': tNumero += c; break;
                        case '1': tNumero += c; break;
                        case '2': tNumero += c; break;
                        case '3': tNumero += c; break;
                        case '4': tNumero += c; break;
                        case '5': tNumero += c; break;
                        case '6': tNumero += c; break;
                        case '7': tNumero += c; break;
                        case '8': tNumero += c; break;
                        case '9': tNumero += c; break;
                    }
                }
                for (X = 1; X < tNumero.Length; X++)
                {
                    //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
                    X += 1;
                    //rNumero = rNumero + 
                    tNumero2 = tNumero.Substring(0, X);
                    rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
                }
                Numero = rNumero;

            }

            if (DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Metodo para validar la clave del CER
        /// </summary>
        /// <param name="NombreArchivoCER">Nombre del archivo</param>
        /// <param name="NombreArchivoKEY">Nombre del archivo KEY</param>
        /// <param name="ClavePrivada">Clave privada</param>
        /// <returns></returns>
        public static bool validarCERKEY(string NombreArchivoCER, string NombreArchivoKEY, string ClavePrivada)
        {
            X509Certificate2 certificado = new X509Certificate2(File.ReadAllBytes(NombreArchivoCER));
            //initialze the byte arrays to the public key information.
            byte[] pk = certificado.GetPublicKey();
            X509Certificate2 certPrivado = new X509Certificate2(File.ReadAllBytes(NombreArchivoKEY));
            return false;
        }
    }
}
