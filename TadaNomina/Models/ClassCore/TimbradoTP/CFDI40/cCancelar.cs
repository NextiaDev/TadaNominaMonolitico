using DocumentFormat.OpenXml.Office.CustomUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
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

        public void CancelarTimbradoNominaRelacion(int IdPeriodoNomina, Guid id, string FolioUUIDSeparadoComas, string FolioRelacion, int IdUsuario)
        {
            List<string> folios = FolioUUIDSeparadoComas.Split(',').ToList();
            List<vTimbradoNomina> timbrado = ObtendatosTimbradoNominaByFoliosUUID(IdPeriodoNomina, folios);
            correctos = new List<vTimbradoNomina>();
            errores = new List<vTimbradoNomina>();
            foreach (var item in timbrado)
            {
                Cancelar40Relacion(IdUsuario, item, FolioRelacion, id);
            }
        }

        /// <summary>
        /// Metodo para cancelar el timbrado con relación o sustitución de CFDI
        /// </summary>
        /// <param name="IdUsuario">Identificador del usuario que realiza la operación</param>
        /// <param name="item">datos del timbrado de nómina</param>
        /// <param name="FolioRelacion">folio del nuevo timbrado que sutituye al anterior.</param>
        public void Cancelar40Relacion(int IdUsuario, vTimbradoNomina item, string FolioRelacion, Guid id)
        {
            creaPfx(item.rutaCer, item.rutaKey, item.KeyPass.Trim(), item.PFXCancelacionTimbrado);
            var rutabytesPfx = item.PFXCancelacionTimbrado;
            var bytesPfx = File.ReadAllBytes(rutabytesPfx);
            var respuesta = CancelarTimbrado40ConRelacion(item.RFC_Patronal, item.FolioUDDI, bytesPfx, item.KeyPass, FolioRelacion);
           
            var _codigoRespuesta = respuesta.Code;
            item.keyRespuesta = respuesta.Message;
            var result = respuesta.Success;
            var acuse = respuesta.Recib;
            string mensaje = "";
            try { mensaje = respuesta.Message; } catch { }
            int[] _codigos = { 201, 202 };

            if (result || mensaje == "Folio Fiscal Previamente Cancelado")
            {
                ActualizaRegistroTimbraado(item.FolioUDDI, "01", acuse, _codigoRespuesta, item.keyRespuesta, IdUsuario);
                correctos.Add(item);
            }
            else
            {               
                GuardaErrorCancelacion(item, _codigoRespuesta + ":" + item.keyRespuesta, id, IdUsuario);
                errores.Add(item);
            }
        }

        /// <summary>
        /// Metodo para cancelar un registro timbrado sin relación o sin sustitución de CFDI.
        /// </summary>
        /// <param name="id">nuemro de identificación para todos los cancelados del periodo.</param>
        /// <param name="IdUsuario">Identificador de usuario que realiza la operación</param>
        /// <param name="item">datos del timbrado de nómina</param>
        /// <param name="MotivoCancelacion">Motivo por el cual se cancela el CFDI.</param>
        public void Cancelar40(Guid id, int IdUsuario, vTimbradoNomina item, string MotivoCancelacion)
        {
            creaPfx(item.rutaCer, item.rutaKey, item.KeyPass.Trim(), item.PFXCancelacionTimbrado);
            var rutabytesPfx = item.PFXCancelacionTimbrado;
            var bytesPfx = File.ReadAllBytes(rutabytesPfx);
            var respuesta = CancelarTimbrado40(item.RFC_Patronal, item.FolioUDDI, bytesPfx, item.KeyPass);
            
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

        /// <summary>
        /// Obtiene los datos de los registros timbrados de todo el periodo de nómina.
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <returns></returns>
        public List<vTimbradoNomina> ObtendatosTimbradoNomina(int IdPeriodoNomina)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = (from b in entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1) select b).ToList();

                return timbrado;
            }
        }

        /// <summary>
        /// Obtiene los registros de timbrado en base a los folios UUID
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="FoliosUUID">listado de folios UUID a buscar</param>
        /// <returns></returns>
        public List<vTimbradoNomina> ObtendatosTimbradoNominaByFoliosUUID(int IdPeriodoNomina, List<string> FoliosUUID)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                return entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && FoliosUUID.Contains(x.FolioUDDI)).ToList();
            }
        }

        /// <summary>
        /// Crea el archivo PFX necesario para poder sellar la solicitud de cancelación.
        /// </summary>
        /// <param name="rutaCer">Ruta donde se tomara el archivo .cer</param>
        /// <param name="rutaKey">Ruta donde se tomara el archivo .key</param>
        /// <param name="pass">Contraseña de los CSD</param>
        /// <param name="ruta">Ruta donde se almacena el archivo PFX</param>
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

        public ServiceReferenceTPCancelado31082023.ResponseCancel CancelarTimbrado40ConRelacion(string rfc_Emisor, string UUID, byte[] pfx, string pass, string FolioRelacion)
        {
            ServiceReferenceTPCancelado31082023.Cancela4Client serviceCancel = new ServiceReferenceTPCancelado31082023.Cancela4Client();

            var response = serviceCancel.SolicitarCancelacion_01(rfc_Emisor, UUID, FolioRelacion, "información Incorrecta", pfx, pass);

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

        public void cancelaCFDISRelacionadosPrevios(int IdPeriodoNomina, Guid id, int IdUsuario)
        {
            var relacionados = getRelacionados(IdPeriodoNomina);
            
            foreach (var item in relacionados)
            { 
                if(item.cantidad >= 2)
                {
                    var cfdis = getTimbradosPeriodoRFC(IdPeriodoNomina, item.rfc);
                    var idXMl = cfdis[0].IdXml ?? 0;
                    var uuidNuevo = cfdis[0].FolioUDDI;
                    var CFDIRel = getXMlNomina(idXMl).FoliosUUIDRelacionados;
                    
                    if(CFDIRel != null && CFDIRel != string.Empty)
                        CancelarTimbradoNominaRelacion(IdPeriodoNomina, id, CFDIRel, uuidNuevo, IdUsuario);
                }                
            }
        }

        public List<modelRelacionados> getRelacionados(int IdPeriodo)
        {
            var consulta = "select count(rfc) as cantidad, rfc from TimbradoNomina where IdPeriodoNomina = " + IdPeriodo + " and IdEstatus = 1 group by rfc";

            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var result = entidad.Database.SqlQuery<modelRelacionados>(consulta);

                return result.ToList();
            }
        }

        public List<TimbradoNomina> getTimbradosPeriodoRFC(int IdPeriodo, string RFC)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var cfdis = entidad.TimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.RFC == RFC && x.IdEstatus == 1).OrderByDescending(x => x.FechaTimbrado);

                return cfdis.ToList();
            }
        }

        public XmlNomina getXMlNomina(int IdXMl)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var registro = entidad.XmlNomina.Where(x => x.IdXml == IdXMl).FirstOrDefault();

                return registro;
            }
        }
    }
}
