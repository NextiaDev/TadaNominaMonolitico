using Delva.AppCode.TimbradoTurboPAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ClassCore.TimbradoTP.CFDI40;
using TadaNomina.Models.DB;
using TadaNomina.ServiceReferenceTP;
using TadaNomina.ServiceReferenceTP40;
using TadaNomina.Services.CFDI40;
using TadaNomina.Services.CFDI40.Models;

namespace TadaNomina.Models.ClassCore.TimbradoTP
{
    public class ClassTimbrado: ClassProcesosTimbradoTP
    {
        GeneraXML xml = new GeneraXML();
        cCreaXML cxml = new cCreaXML();
        public string FolioUUIDNuevoTimbrado;

        /// <summary>
        /// Metodo para timbrar un periodo de nomina con el Id PAC 1
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina.</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio.</param>
        /// <param name="IdCliente"></param>
        /// <param name="Id"></param>
        /// <param name="IdUsuario"></param>
        /// <exception cref="Exception"></exception>
        public void TimbradoNomina(int IdPeriodo, int IdUnidadNegocio, int IdCliente, Guid Id, int IdPAC, int IdUsuario)
        {            
            var cunidad = new  ClassUnidadesNegocio();
            var cgxml = new cGeneraXML();
            
            var yaTimbrados = getYaTimbradosPeriodo(IdPeriodo).Select(y => y.IdXml).ToList();
            var informacion = cgxml.getRegistrosvXMLPeriodo(IdPeriodo).Where(x=> !yaTimbrados.Contains(x.IdXml)).ToList();
            var unidad = cunidad.getUnidadesnegocioId(IdUnidadNegocio);
            var cliente = cunidad.getClienteById(IdCliente);

            foreach (var i in informacion)
            {
                if (cliente.VersionCFDI == "3.3")                
                    TimbraTP(i, IdUnidadNegocio, IdPeriodo, Id, unidad.FiniquitosFechasDiferentes, IdUsuario);
                else if (cliente.VersionCFDI == "4.0")
                
                    TimbraTP40(i, Id, IdPAC, IdUsuario);                
                else
                    throw new Exception("No se ha especificado la version del cfdi.");

                //FolioUUIDNuevoTimbrado = "274458ef-5bfa-0d00-06ab-6090f59083cf" // se puede poner un UUID directo para cancelar;
                if (i.UsoXML == "Timbrado CR" && FolioUUIDNuevoTimbrado != null && FolioUUIDNuevoTimbrado != string.Empty)
                {                    
                    cCancelar cc = new cCancelar();
                    cc.CancelarTimbradoNominaRelacion(IdPeriodo, i.FoliosUUIDRelacionados, FolioUUIDNuevoTimbrado, IdUsuario);
                }
            }
        }

        public void TimbraTP(vXmlNomina dat, int IdUnidadNegocio, int IdPeriodo, Guid guid, string tipoFechaFiniquito, int IdUsuario)
        {
            TurboPacWsClient servicio = new TurboPacWsClient();

            string Comprobante = string.Empty;
            bool timbra = false;
            RespuestaTimbrado respuesta = null;

            try 
            {                
                Comprobante = dat.XML;
                if (Comprobante != string.Empty) { timbra = true; }
            } 
            catch (Exception ex)
            {
                string[] error = { ex.Message };
                respuesta = new RespuestaTimbrado() {
                    DescripcionError = error
                };
            }
            
            if(timbra)
                respuesta = servicio.TimbraCfdi33("lavore", "02F7E5C197C54C9EB867A7387F6245CB5434330D", Comprobante);

            servicio.Close();

            if (respuesta.Valido)            
                GuardaTablaTimbrado(dat, respuesta, IdUsuario);            
            else            
                GuardaLogError(dat, respuesta, guid, IdUsuario);
        }

        public void TimbraTP40(vXmlNomina dat, Guid guid, int IdPAC, int IdUsuario)
        {
            string Comprobante = string.Empty;
            bool timbra = false;
            mTimbradoResult respuesta = null;

            sGetToken sgt = new sGetToken();
            var acceso = sgt.sGetAcceso();

            try
            {                
                Comprobante = dat.XML;
                if (Comprobante != string.Empty) { timbra = true; }
            }
            catch (Exception ex)
            {
                List<mFacturasResult> mef = new List<mFacturasResult>();
                mReporteIncidentes mri = new mReporteIncidentes();
                List<string> incidentes = new List<string>();
                incidentes.Add(ex.Message);
                mri.Incidentes = incidentes;
                mef.Add(new mFacturasResult { Contador = 1, ReporteIncidentes = mri });

                respuesta = new mTimbradoResult() {
                    Facturas = mef
                };
            }

            if (timbra)
            {
                sTimbrador st = new sTimbrador();
                var comprobanteB64 = Statics.Base64Encode(Comprobante);
                List<mFacturas> fac = new List<mFacturas>();
                fac.Add(new mFacturas { Base64 = comprobanteB64 });
                mTimbrar mTimbrar = new mTimbrar() {
                    Referencia = guid.ToString(),
                    Fecha = DateTime.Now.ToShortDateString(),
                    Facturas = fac
                };

                respuesta = st.sTimbrado(mTimbrar, acceso.Access_Token); 
            }

            try
            {
                if (respuesta.Facturas[0].XMLTimbrado != null)
                {
                    var xml = Statics.Base64Decode(respuesta.Facturas[0].XMLTimbrado).Replace("\n", "").Replace("\r", "");
                    FolioUUIDNuevoTimbrado = respuesta.Facturas[0].UUID;
                    GuardaTablaTimbrado(dat, xml, respuesta.Facturas[0].UUID, respuesta.Facturas[0].FechaSello, IdPAC, IdUsuario);
                }
                else
                    GuardaLogError(dat, respuesta.Facturas[0], guid, IdUsuario);
            }
            catch { }
        }

        public void GuardaTablaTimbrado(vXmlNomina dat, string xml, string UUID, string FechaTimbrado, int IdPAC, int IdUsuario)
        {            
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                TimbradoNomina tim = new TimbradoNomina();
                tim.IdPeriodoNomina = dat.IdPeriodoNomina;
                tim.IdEmpleado = dat.IdEmpleado;
                tim.IdRegistroPatronal = dat.IdRegistroPatronal;
                tim.RegistroPatronal = dat.RegistroPatronal;
                tim.NombrePatrona = dat.NombrePatrona;
                tim.RFC = dat.Rfc;
                tim.FolioUDDI = UUID;
                tim.Mensaje = "Comprobante timbrado exitosamente";
                tim.IdEstatus = 1;
                tim.IdCaptura = IdUsuario;
                tim.FechaInicioPeriodo = dat.FechaInicio;
                tim.FechaFinPeriodo = dat.FechaFin;
                tim.FechaCaptura = DateTime.Now;
                tim.CFDI_Timbrado = xml;
                tim.Leyenda = dat.Leyenda;
                tim.FechaTimbrado = FechaTimbrado;
                tim.IdXml = dat.IdXml;
                tim.IdPAC = IdPAC;

                entidad.TimbradoNomina.Add(tim);
                entidad.SaveChanges();
            }
        }

        public void GuardaLogError(vXmlNomina dat, mFacturasResult respuesta, Guid guid, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores error = new LogErrores();
                error.Guid = guid;
                error.IdPeriodoNomina = dat.IdPeriodoNomina;
                error.Modulo = "Timbrado";
                error.Referencia = dat.Rfc + " - " + dat.ClaveEmpleado + " - " + dat.ApellidoPaterno + " " + dat.ApellidoMaterno + " " + dat.Nombre;
                error.Descripcion = "Errores: " + string.Join("", respuesta.ReporteIncidentes.Incidentes);
                error.Fecha = DateTime.Now;
                error.IdUsuario = IdUsuario;
                error.IdEstatus = 1;

                entidad.LogErrores.Add(error);
                entidad.SaveChanges();
            }
        }

        public List<vTimbradoNomina> getYaTimbradosPeriodo(int IdPeriodoNomina)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrados = entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1).ToList();

                return timbrados;
            }
        }
    }
}
