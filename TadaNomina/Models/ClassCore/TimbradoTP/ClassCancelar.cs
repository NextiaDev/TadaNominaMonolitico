using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CryptoFirma;
using TadaNomina.Models.DB;
using TadaNomina.ServiceReferenceTP;
//using Core.Sat.Cancelacion.CFDi;
using TadaNomina.ServiceReferenceTPCancelacion20;

using System.IO;
using TadaNomina.ServiceReferenceTPCancelacionNuevo;

namespace Delva.AppCode.TimbradoTurboPAC
{
    public class ClassCancelar
    {
        public List<vTimbradoNomina> errores { get; set; }
        public List<vTimbradoNomina> correctos { get; set; }

        private string _KeyPass;
        private string _cer;
        private string _key;
        
        private void ObtenCertificadosCfdi(string rfc_Emisor)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = (from b in entidad.Cat_RegistroPatronal.Where(x => x.RFC == rfc_Emisor) select b).FirstOrDefault();

                _cer = registro.rutaCer;
                _key = registro.rutaKey;
                _KeyPass = registro.KeyPass;
            }
        }
        
        public TadaNomina.ServiceReferenceTPCancelacionNuevo.ResponseCancel CancelarTimbrado40(string rfc_Emisor, string UUID,  byte[] pfx, string pass )
        {
            TadaNomina.ServiceReferenceTPCancelacionNuevo.Cancela4Client serviceCancel = new TadaNomina.ServiceReferenceTPCancelacionNuevo.Cancela4Client();

            var response = serviceCancel.SolicitudCancelacion_02(rfc_Emisor, UUID, string.Empty, pfx, pass);

            return response;
        }

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

        public List<Cat_MotivosCancelacionSAT> getMotivosCancelacionSAT()
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var motivos = entidad.Cat_MotivosCancelacionSAT.Where(x => x.IdEstatus == 1).ToList();

                return motivos;
            }
        }

        private void Cancelar40(Guid id, int IdUsuario, vTimbradoNomina item, string MotivoCancelacion)
        {
            var rutabytesPfx = item.PFXCancelacionTimbrado;
            var bytesPfx = File.ReadAllBytes(rutabytesPfx);
            var respuesta = CancelarTimbrado40(item.RFC_Patronal, item.FolioUDDI, bytesPfx, item.KeyPass);
            
            var _codigoRespuesta = respuesta.Code;
            item.keyRespuesta = respuesta.Message;
            var result = respuesta.Success;
            var acuse = respuesta.Recib;
            int[] _codigos = { 201, 202 };

            if (result)
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

        public void ActualizaRegistroTimbraado(string UUID, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = from b in entidad.TimbradoNomina.Where(x => x.FolioUDDI == UUID && x.IdEstatus == 1) select b;

                foreach (var item in timbrado)
                {
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        public void ActualizaRegistroTimbraado(string UUID, string claveMotivoCancelacion, string xmlAcuse, string CodigoRespuesta, string Mensaje,  int IdUsuario)
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

        private static void GuardaErrorCancelacion(vTimbradoNomina datos, string Error, Guid id, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores logErrores = new LogErrores();
                logErrores.Guid = id;
                logErrores.IdPeriodoNomina = datos.IdPeriodoNomina;
                logErrores.Modulo = "Cancelacion Timbrado";
                logErrores.Referencia = datos.IdTimbradoNomina.ToString();
                logErrores.Descripcion = "No se pudo cancelar el timbre: " + Error;
                logErrores.Fecha = DateTime.Now;
                logErrores.IdUsuario = IdUsuario;
                logErrores.IdEstatus = 1;

                entidad.LogErrores.Add(logErrores);
                entidad.SaveChanges();
            }
        }
    }
}