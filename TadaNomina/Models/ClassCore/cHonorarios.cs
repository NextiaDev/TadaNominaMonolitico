using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class cHonorarios
    {

        public void DeleteHonorarios(int idHonorario, string Observaciones, int idUsuario)
        {
            bool Valida = false;

            string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var query = (from b in entidad.Honorarios.Where(x => x.IdHonorarios == idHonorario) select b).FirstOrDefault();

                query.IdEstatus = 2;
                query.IdModifica = idUsuario;
                query.ObservacionesCliente = Observaciones;
                query.FechaModifica = DateTime.Now;

                entidad.SaveChanges();



                try
                {
                    var obtenrid = getHonorariosporUsuario(idHonorario);
                    string obtener = ObtenCorreoEmpleados(obtenrid.IdEmpleado);
                    Message = "Estimad@ ha sido eliminado su Honorario" + Observaciones;
                    Titulo = "Honorario Eliminado";
                    Correo = obtener;
                    Modulo = "Honorarios";
                    EnviarCorreo(Message, Titulo, Correo, Modulo, Valida);
                }
                catch (Exception)
                {

                    throw;
                }

                try
                {
                    var obtenercorreoUsuario = ObtenCorreoUsuarios(idUsuario);
                    Message = "Estimad@ Elimino el Honorario  con el id " + idHonorario;
                    Titulo = "Honorario Eliminado";
                    Correo = obtenercorreoUsuario;
                    Modulo = "Honorarios";
                    EnviarCorreo(Message, Titulo, Correo, Modulo, Valida);
                }
                catch (Exception)
                {

                    throw;
                }

       



            }

        }

        public List<vHonorarios> getHonorariosporPeriodo(int idperiodo)
        {
            using (NominaEntities1 entity = new NominaEntities1())
            {

                var query = (from b in entity.vHonorarios.Where(x => x.IdPeriodoNomina == idperiodo && (x.IdEstatus == 1 || x.IdEstatus == 3)) select b).ToList();

                return query;
            }
        }


        public List<vHonorarios> getHonorariosporPeriodoEliminados(int idperiodo)
        {
            using (NominaEntities1 entity = new NominaEntities1())
            {

                var query = (from b in entity.vHonorarios.Where(x => x.IdPeriodoNomina == idperiodo && x.IdEstatus == 2) select b).ToList();

                return query;
            }
        }

        public Empleados getEmpleadosTipo(int idempelado)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {

                var query = (from b in entity.Empleados.Where(x => x.IdEmpleado == idempelado) select b).FirstOrDefault();

                return query;
            }
        }


        public vHonorarios getHonorariosporUsuario(int idHonorario)
        {
            using (NominaEntities1 entity = new NominaEntities1())
            {

                var query = (from b in entity.vHonorarios.Where(x => x.IdHonorarios == idHonorario) select b).FirstOrDefault();

                return query;
            }
        }


        public void GuardarHonorarios(mHonorarios modelo, int idcaptura)
        {



            using (NominaEntities1 entity = new NominaEntities1())
            {
                Honorarios honorarios = new Honorarios();

                honorarios.IdPeriodoNomina = modelo.IdPeriodoNomina;
                honorarios.IdFactura = modelo.idHonorarioF;
                honorarios.Observaciones = modelo.Observaciones;
                honorarios.IdEmpleado = modelo.IdEmpleado;
                honorarios.IVA = (decimal)modelo.iva;
                honorarios.TotalFactura = (decimal)modelo.totalfactura;
                honorarios.RetencionISR = (decimal)modelo.retencionisr;
                honorarios.RetencionIVA = (decimal)modelo.retencioniva;
                honorarios.SubTotal = (decimal)modelo.subtotal;
                honorarios.Total = (decimal)modelo.totalRetencion;
                honorarios.IdRegistroPatronal = modelo.IdRegistroPatronal;
                honorarios.MetodoPago = "Pago en una sola Exhibicion";
                honorarios.FormaPago = "Transferencia electrónica de fondos (incluye SPEI)";
                honorarios.UsoCFDI = "Gastos en general";
                honorarios.IdEstatus = 1;
                honorarios.IdCaptura = idcaptura;
                honorarios.FechaCaptura = DateTime.Now;

                entity.Honorarios.Add(honorarios);
                entity.SaveChanges();
            }

        }

        public void GuardarHistoricoHonorarios(int idHonorario , string EstatusSAT)
        {



            using (NominaEntities1 entity = new NominaEntities1())
            {
                HistoricoHonorarios honorarios = new HistoricoHonorarios();

                honorarios.IdHonorarios = idHonorario;
                honorarios.FechaCaptura = DateTime.Now;
               honorarios.EstatusSAT= EstatusSAT;

                entity.HistoricoHonorarios.Add(honorarios);
                entity.SaveChanges();
            }

        }


        public void GuardarHonorariosLista(List<mHonorarios> modelo, int idcaptura)
        {


            cHonorarios cl = new cHonorarios();
            bool Valida = false;
            double subtotal = 0;
            double iva = 0;
            double totalF = 0;
            double retencionisr = 0;
            double retencioniva = 0;
            double totalconretencion = 0;
            string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;

            foreach (var i in modelo)
            {
                if (i.HonorariosB != 0)
                {
                    var datos = cl.getEmpleadosTipo(i.IdEmpleado);


                    subtotal = Math.Round((double)i.HonorariosB);
                    iva = Math.Round(subtotal * 0.16, 2);
                    totalF = Math.Round(subtotal + iva, 2);
                    if (datos.TipoContrato == "RESICO")
                    {
                        retencionisr = Math.Round(subtotal * .0125, 2);

                    }
                    else
                    {
                        retencionisr = Math.Round(subtotal * 0.1, 2);

                    }

                    retencioniva = Math.Round(iva / 3 * 2, 2);
                    totalconretencion = Math.Round(totalF - retencionisr - retencioniva, 2);

                }
                else
                {
                    var datos = cl.getEmpleadosTipo(i.IdEmpleado);

                    subtotal = Math.Round((double)(i.HonorariosN) / 0.953333334, 2);
                    iva = Math.Round(subtotal * 0.16, 2);
                    totalF = Math.Round(subtotal + iva, 2);
                    if (datos.TipoContrato == "RESICO")
                    {
                        retencionisr = Math.Round(subtotal * .0125, 2);

                    }
                    else
                    {
                        retencionisr = Math.Round(subtotal * 0.1, 2);

                    }
                    retencioniva = Math.Round(iva / 3 * 2, 2);
                    totalconretencion = Math.Round(totalF - retencionisr - retencioniva, 2);

                }

                using (NominaEntities1 entity = new NominaEntities1())
                {
                    Honorarios honorarios = new Honorarios();

                    honorarios.IdPeriodoNomina = i.IdPeriodoNomina;
                    honorarios.Observaciones = i.Observaciones;
                    honorarios.IdEmpleado = i.IdEmpleado;
                    honorarios.IVA = (decimal)iva;
                    honorarios.TotalFactura = (decimal)totalF;
                    honorarios.RetencionISR = (decimal)retencionisr;
                    honorarios.RetencionIVA = (decimal)retencioniva;
                    honorarios.SubTotal = (decimal)subtotal;
                    honorarios.Total = (decimal)totalconretencion;
                    honorarios.IdRegistroPatronal = i.IdRegistroPatronal;
                    honorarios.IdFactura = i.idHonorarioF;
                    honorarios.MetodoPago = "Pago en una sola Exhibicion";
                    honorarios.FormaPago = "Transferencia electrónica de fondos (incluye SPEI)";
                    honorarios.UsoCFDI = "Gastos en general.";
                    honorarios.IdEstatus = 1;
                    honorarios.IdCaptura = idcaptura;
                    honorarios.FechaCaptura = DateTime.Now;

                    entity.Honorarios.Add(honorarios);
                    entity.SaveChanges();

                    string obtener = ObtenCorreoEmpleados(honorarios.IdEmpleado);
                    Message = "Estimad@ ha sido enviado su Solicitud de Honorarios consultarlo en la pagina https://servicios.tada.mx/lanube ";
                    Titulo = "Recibos Honorarios";
                    Correo = obtener;
                    Modulo = "Honorarios";
                    EnviarCorreo(Message, Titulo, Correo, Modulo, Valida);
                }
            }
        }


        private string ObtenCorreoEmpleados(int IdEmpleado)
        {
            TadaEmpleados context = new TadaEmpleados();

            var CorreoEmpleado = (from b in context.Empleados.Where(x => x.IdEmpleado == IdEmpleado) select b.CorreoElectronico).FirstOrDefault();

            return CorreoEmpleado;
        }


        private string ObtenCorreoUsuarios(int idusuario)
        {
            TadaAccesoEntities context = new TadaAccesoEntities();

            var CorreoEmpleado = (from b in context.Usuarios.Where(x => x.IdUsuario == idusuario) select b.Correo).FirstOrDefault();

            return CorreoEmpleado;
        }


        public void EnviarCorreo(string Message, string Titulo, string Correo, string Modulo, bool Valida)
        {
            bool Val = Valida;

            ClassSistema cs = new ClassSistema();
            var Send = cs.getSendCorreoTada();
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.To.Clear();
                msg.To.Add(Correo);

                Val = true;
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

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }



        }

    }
}