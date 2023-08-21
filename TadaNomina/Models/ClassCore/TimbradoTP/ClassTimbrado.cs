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
        public void TimbradoNomina(int IdPeriodo, int IdUnidadNegocio, int IdCliente, Guid Id, int IdUsuario)
        {            
            var cunidad = new  ClassUnidadesNegocio();
            var cgxml = new cGeneraXML();
            
            var yaTimbrados = getYaTimbradosPeriodo(IdPeriodo).Select(y => y.IdEmpleado).ToList();
            var informacion = cgxml.getRegistrosvXMLPeriodo(IdPeriodo).Where(x=> !yaTimbrados.Contains(x.IdEmpleado)).ToList();
            var unidad = cunidad.getUnidadesnegocioId(IdUnidadNegocio);
            var cliente = cunidad.getClienteById(IdCliente);

            foreach (var i in informacion)
            {                
                if (cliente.VersionCFDI == "3.3")
                    TimbraTP(i, IdUnidadNegocio, IdPeriodo, Id, unidad.FiniquitosFechasDiferentes, IdUsuario);
                else if (cliente.VersionCFDI == "4.0")
                    TimbraTP40(i, IdUnidadNegocio, IdPeriodo, Id, unidad.FiniquitosFechasDiferentes, IdUsuario);
                else
                    throw new Exception("No se ha especificado la version del cfdi.");                                
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
                //Comprobante = xml.GeneraXMLNomina12(dat, IdUnidadNegocio, tipoFechaFiniquito, IdPeriodo);
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

        public void TimbraTP40(vXmlNomina dat, int IdUnidadNegocio, int IdPeriodo, Guid guid, string tipoFechaFiniquito, int IdUsuario)
        {  
            string Comprobante = string.Empty;
            bool timbra = false;
            mTimbradoResult respuesta = null;

            sGetToken sgt = new sGetToken();
            var acceso = sgt.sGetAcceso();

            try
            {
                //Comprobante = cxml.GeneraXML40Nomina12(dat, IdUnidadNegocio, tipoFechaFiniquito, IdPeriodo);
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

            if (respuesta.Facturas[0].XMLTimbrado != null)
            {
                var xml = Statics.Base64Decode(respuesta.Facturas[0].XMLTimbrado).Replace("\n", "").Replace("\r", "");

                GuardaTablaTimbrado(dat, xml, respuesta.Facturas[0].UUID, respuesta.Facturas[0].FechaSello, IdUsuario);
            }
            else
                GuardaLogError(dat, respuesta.Facturas[0], guid, IdUsuario);
        }

        public void GuardaTablaTimbrado(vXmlNomina dat, string xml, string UUID, string FechaTimbrado, int IdUsuario)
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