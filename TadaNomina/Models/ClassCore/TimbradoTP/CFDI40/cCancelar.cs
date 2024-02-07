using DocumentFormat.OpenXml.Office.CustomUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.ServiceReferenceTPCancelacion20;

namespace TadaNomina.Models.ClassCore.TimbradoTP.CFDI40
{
    public class cCancelar
    {
        public List<vTimbradoNomina> errores { get; set; }
        public List<vTimbradoNomina> correctos { get; set; }

        private string _KeyPass;
        private string _cer;
        private string _key;

        public void CancelarPeriodoNomina(int IdPeriodo, Guid id, int IdUnidadNegocio, string MotivoCancelacion, int IdUsuario)
        {
            List<vTimbradoNomina> timbrado = ObtendatosTimbradoNomina(IdPeriodo);
            correctos = new List<vTimbradoNomina>();
            errores = new List<vTimbradoNomina>();
            foreach (var item in timbrado)
            {
                Cancelar40(id, IdUsuario, item, MotivoCancelacion);
            }
        }

        public void CancelarPeriodoNominaClaves(int IdPeriodo, Guid id, int IdunidadNegocio, string[] Claves, string MotivoCancelacion, int IdUsuario)
        {
            List<vTimbradoNomina> timbrado = ObtendatosTimbradoNomina(IdPeriodo).Where(x => x.IdEstatus == 1 && Claves.Contains(x.ClaveEmpleado)).ToList();
            correctos = new List<vTimbradoNomina>();
            errores = new List<vTimbradoNomina>();
            foreach (var item in timbrado)
            {
                Cancelar40(id, IdUsuario, item, MotivoCancelacion);
            }
        }

        public void Cancelar40(Guid id, int IdUsuario, vTimbradoNomina item, string MotivoCancelacion)
        {
            creaPfx(item.rutaCer, item.rutaKey, item.KeyPass.Trim(), item.PFXCancelacionTimbrado);
            var rutabytesPfx = item.PFXCancelacionTimbrado;
            var bytesPfx = File.ReadAllBytes(rutabytesPfx);
            var respuesta = CancelarTimbrado40(item.RFC_Patronal, item.FolioUDDI, bytesPfx, item.KeyPass);
            //var respuesta = CancelarTimbrado_02Test(item.RFC_Patronal, item.FolioUDDI, bytesPfx, item.KeyPass);

            var _codigoRespuesta = respuesta.Code;
            item.keyRespuesta = respuesta.Message;
            var result = respuesta.Success;
            var acuse = respuesta.Recib;
            string mensaje = "";
            try { mensaje = respuesta.Message; } catch { }
            int[] _codigos = { 201, 202 };

            if (result || mensaje == "Folio Fiscal Previamente Cancelado")
            {
                ActualizaRegistroTimbraado(item.FolioUDDI, MotivoCancelacion, acuse, _codigoRespuesta, item.keyRespuesta, IdUsuario);
                correctos.Add(item);
            }
            else
            {
                GuardaErrorCancelacion(item, _codigoRespuesta + ":" + item.keyRespuesta, id, IdUsuario);
                errores.Add(item);
            }
        }
        
        public List<vTimbradoNomina> ObtendatosTimbradoNomina(int IdPeriodoNomina)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = (from b in entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1) select b).ToList();

                return timbrado;
            }
        }

        public void creaPfx(string rutaCer, string rutaKey, string pass, string ruta)
        {
            if (!File.Exists(ruta))
            {
                var oPFX = new Pfx(rutaCer, rutaKey, pass, ruta, Path.GetDirectoryName(ruta) + "/");

                oPFX.creaPFX();
            }
        }

        public ServiceReferenceTPCancelado31082023.ResponseCancel CancelarTimbrado40(string rfc_Emisor, string UUID, byte[] pfx, string pass)
        {
            ServiceReferenceTPCancelado31082023.Cancela4Client serviceCancel = new ServiceReferenceTPCancelado31082023.Cancela4Client();

            var response = serviceCancel.SolicitudCancelacion_02(rfc_Emisor, UUID, string.Empty, pfx, pass);

            return response;
        }

        public ServiceReferenceCancelacionTPTest2.ResponseCancel CancelarTimbrado_02Test(string rfc_Emisor, string UUID, byte[] pfx, string pass)
        {
            ServiceReferenceCancelacionTPTest2.Cancela4Client serviceCancel = new ServiceReferenceCancelacionTPTest2.Cancela4Client();

            var response = serviceCancel.SolicitudCancelacion_02(rfc_Emisor, UUID, string.Empty, pfx, pass);

            return response;
        }

        private static void GuardaErrorCancelacion(vTimbradoNomina datos, string Error, Guid id, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores logErrores = new LogErrores();
                logErrores.Guid = id;
                logErrores.IdPeriodoNomina = datos.IdPeriodoNomina;
                logErrores.Modulo = "Cancelacion Timbrado";
                logErrores.Referencia = "IdT:" + datos.IdTimbradoNomina.ToString() + " | IdR:" + datos.IdRegistroPatronal + " | " + datos.ClaveEmpleado + " - " + datos.Nombre + " " + datos.ApellidoPaterno + " " + datos.ApellidoMaterno + " - " + datos.RFC;
                logErrores.Descripcion = "No se pudo cancelar el timbre: " + Error;
                logErrores.Fecha = DateTime.Now;
                logErrores.IdUsuario = IdUsuario;
                logErrores.IdEstatus = 1;

                entidad.LogErrores.Add(logErrores);
                entidad.SaveChanges();
            }
        }
        public void ActualizaRegistroTimbraado(string UUID, string claveMotivoCancelacion, string xmlAcuse, string CodigoRespuesta, string Mensaje, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = from b in entidad.TimbradoNomina.Where(x => x.FolioUDDI == UUID && x.IdEstatus == 1) select b;

                foreach (var item in timbrado)
                {
                    item.ClaveMotivoCancelacion = claveMotivoCancelacion;
                    item.AcuseCancelacion = xmlAcuse;
                    item.CodigoRespuestaCancelacion = CodigoRespuesta;
                    item.Mensaje = Mensaje;
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }
    }
}
