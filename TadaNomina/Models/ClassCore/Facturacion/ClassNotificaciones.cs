using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassNotificaciones
    {
        public string ExtraeCorreos(string Modulo, int IdCliente, int IdUnidadNegocio, int? confidencial)
        {

            try
            {
                using (var consql = new SqlConnection(ConfigurationManager.ConnectionStrings["ModelContabilidad"].ToString()))
                {
                    consql.Open();

                    string sql = "SELECT IdCliente, IdUnidadNegocio, Correos = STUFF(";
                    sql += "(SELECT ',' + Correo";
                    sql += " FROM CorreosFacturacion";
                    sql += " WHERE Modulo ='" + Modulo + "' and IdEstatus = 1";
                    if (confidencial != null)
                        sql += " AND Confidencial = 1 AND IdCliente = " + IdCliente + " AND IdUnidadNegocio = " + IdUnidadNegocio ;

                    sql += " FOR XML PATH(''))";
                    sql += " , 1, 1, '') from CorreosFacturacion";
                    sql += " group by IdCliente,IdUnidadNegocio;";

                    var query = new SqlCommand(sql, consql);
                    var dr = query.ExecuteReader();
                    string correosnot = string.Empty;

                    while (dr.Read())
                    {
                        correosnot = dr["Correos"].ToString();
                    }
                    consql.Close();
                    return correosnot;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }            
        }
                        
        public void EnviarCorreo(string Message, string Titulo, string Correo, string Modulo)
        {           
            ClassSistema cs = new ClassSistema(); 
            var Send = cs.getSendCorreo();
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.To.Clear();

                
                if (Correo != string.Empty)
                {
                    msg.To.Add(Correo);

                    msg.From = new MailAddress(Send.Credentials, Modulo, System.Text.Encoding.UTF8);
                    msg.Subject = Titulo;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = Message;
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;

                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(Send.Credentials, Send.Contrasena);
                    client.Port = int.Parse(Send.Puerto);
                    client.Host = Send.ClientHost;
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(msg);

                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        throw new Exception("El correo electronico no pudo llegar a su destino debido a: " + ex.Message, ex);
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void EnviarCorreo(string Message, string Titulo, string Correo, string CorreoCC, string Modulo)
        {
            ClassSistema cs = new ClassSistema();
            var Send = cs.getSendCorreo();
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.To.Clear();


                if (Correo != string.Empty)
                {
                    msg.To.Add(Correo);
                    msg.CC.Add(CorreoCC);
                    msg.From = new MailAddress(Send.Credentials, Modulo, System.Text.Encoding.UTF8);
                    msg.Subject = Titulo;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = Message;
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;

                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(Send.Credentials, Send.Contrasena);
                    client.Port = int.Parse(Send.Puerto);
                    client.Host = Send.ClientHost;
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(msg);

                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        throw new Exception("El correo electronico no pudo llegar a su destino debido a: " + ex.Message, ex);
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void EnviarCorreoTada(string Message, string Titulo, string[] Correo, string Modulo)
        {
            ClassSistema cs = new ClassSistema();
            var Send = cs.getSendCorreoTada();
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.To.Clear();


                if (Correo.Count() > 0)
                {
                    foreach (var item in Correo)
                    {
                        msg.To.Add(item);
                    }
                    msg.From = new MailAddress(Send.Credentials, Modulo, System.Text.Encoding.UTF8);
                    msg.Subject = Titulo;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = Message;
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;

                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(Send.Credentials, Send.Contrasena);
                    client.Port = int.Parse(Send.Puerto);
                    client.Host = Send.ClientHost;
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(msg);

                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        throw new Exception("El correo electronico no pudo llegar a su destino debido a: " + ex.Message, ex);
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}