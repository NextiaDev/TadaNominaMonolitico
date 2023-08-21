using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassSolicitudDispersion
    {
        public List<ModelSolicitudFacturas> listar()
        {

            var _IdUnidadNegocio = (int)System.Web.HttpContext.Current.Session["sIdUnidadNegocio"];
            var anio = DateTime.Now.Year;
            List<ModelSolicitudFacturas> nl = new List<ModelSolicitudFacturas>();

            try
            {
                using (var consql = new SqlConnection(ConfigurationManager.ConnectionStrings["ModelContabilidad"].ToString()))
                {
                    consql.Open();

                    string sql = "select ROW_NUMBER() OVER(ORDER BY IdfacturasContabilidad desc) AS Num,IdfacturasContabilidad, idUnidadNegocio, dbo.fu_NombresPeriodos(periodo) as Descripcion, TipoEsquema, SubTotal,IVA, Total, IdEstatusTesoreria,";
                    sql += "case idestatustesoreria when 1 then 'COSTEO GUARDADO' when 2 then 'SOLICITADO' WHEN 3 THEN 'FINALIZADO' WHEN 4 THEN 'RECHAZADO' END AS EstatusTesoreria, costeo_HTML,NTotal, AdjuntarComprobante, ArchivoBancos, ComprobanteAutorizacion ";
                    sql += " from FacturasContabilidad WHERE idestatustesoreria in (1,2) and IdUnidadNegocio = " + _IdUnidadNegocio + " and year(fechacaptura)=" + anio + " order by IdfacturasContabilidad desc";

                    var query = new SqlCommand(sql, consql);

                    var dr = query.ExecuteReader();

                    while (dr.Read())
                    {
                        ModelSolicitudFacturas dt = new ModelSolicitudFacturas();
                        dt.Num = int.Parse(dr["Num"].ToString());
                        dt.IdfacturasContabilidad = int.Parse(dr["IdfacturasContabilidad"].ToString());
                        dt.IdUnidadNegocio = int.Parse(dr["IdUnidadNegocio"].ToString());
                        dt.Descripcion = dr["Descripcion"].ToString();
                        dt.TipoEsquema = dr["TipoEsquema"].ToString();
                        dt.Estatus = dr["EstatusTesoreria"].ToString();
                        dt.SubTotal = decimal.Parse(dr["SubTotal"].ToString());
                        dt.IVA = decimal.Parse(dr["IVA"].ToString());
                        dt.Total = decimal.Parse(dr["Total"].ToString());
                        dt.NTotal = decimal.Parse(dr["NTotal"].ToString());
                        dt.Costeo_HTML = dr["Costeo_HTML"].ToString();
                        dt.IdEstatusTesoreria = (int)dr["IdEstatusTesoreria"];
                        try { dt.Comprobante = Path.GetFileName(dr["AdjuntarComprobante"].ToString()); } catch { }
                        try { dt.ArchivoBancos = Path.GetFileName(dr["ArchivoBancos"].ToString()); } catch { }
                        try { dt.ComprobanteAutorizacion = Path.GetFileName(dr["ComprobanteAutorizacion"].ToString()); } catch { }
                        nl.Add(dt);

                    }

                    return nl;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<v_Tesoreria_Solicitud> getSolicitudesRechazadas(int IdUnidadNegocio)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {  
                var query = ctx.v_Tesoreria_Solicitud.Where(x => x.IdEstatusTesoreria == 4 && x.IdUnidadNegocio == IdUnidadNegocio).OrderByDescending(x=> x.FechaCancelacionTesoreria).ToList();

                return query;
            }
        }

        public List<v_Tesoreria_Solicitud> getSolicitudesFinalizadas(int IdUnidadNegocio)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var query = ctx.v_Tesoreria_Solicitud.Where(x => x.IdEstatusTesoreria == 3 && x.IdUnidadNegocio == IdUnidadNegocio).OrderByDescending(x => x.FechaFinalizacionTesoreria).ToList();

                return query;
            }
        }

        public v_Tesoreria_Solicitud getSolicitud(int IdFacturasContabilidad)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var query = ctx.v_Tesoreria_Solicitud.Where(x => x.IdFacturasContabilidad == IdFacturasContabilidad).FirstOrDefault();

                return query;
            }
        }

        public bool Eliminar(ModelSolicitudFacturas modsf)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {  //Eliminar
                    var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == modsf.IdfacturasContabilidad).First();
                    query.IdEstatusTesoreria = (int)modsf.IdEstatus;
                    query.IdCancelacionTesoreria = (int)modsf.IdCancelacion;

                    ctx.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RegistraSolicitud(ModelSolicitudFacturas modsf)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {
                    var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == modsf.IdfacturasContabilidad).First();

                    string nuevoHtml = query.Costeo_HTML.Replace("*Facturadora*", modsf.Facturadora);
                    nuevoHtml = nuevoHtml.Replace("*ConceptoFacturacion*", modsf.ConceptoFacturacion);
                    nuevoHtml = nuevoHtml.Replace("*RazonCliente*", modsf.Razonsocial);

                    string nuevoJson = null;
                    if (query.Costeo_Json != null && query.Costeo_Json != string.Empty)
                    {
                        var nuevoModel = JsonConvert.DeserializeObject<ModelCosteo>(query.Costeo_Json);
                        nuevoModel.Facturadora = modsf.Facturadora;
                        nuevoModel.RazonCliente = modsf.Razonsocial;
                        nuevoModel.ConceptoFacturacion = modsf.ConceptoFacturacion;

                        nuevoJson = JsonConvert.SerializeObject(nuevoModel);
                    }

                    query.IdClienteRazonSocialFacturacion = modsf.IdClienteRazonSocialFacturacion;
                    query.IdEmpresaFacturadora = modsf.IdEmpresaFacturadora;
                    query.IdConceptoFacturacion = modsf.IdConceptoFacturacion;
                    query.IdEstatusTesoreria = 2;
                    query.IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"];
                    query.FechaSolicitudTesoreria = DateTime.Now;
                    query.ObservacionesParaTesoreria = modsf.ObservacionesParaTesoreria;
                    query.AdjuntarComprobante = modsf.Comprobante;
                    query.ArchivoBancos = modsf.ArchivoBancos;
                    if (nuevoHtml != string.Empty) { query.Costeo_HTML = nuevoHtml; }                    
                    query.Costeo_Json = nuevoJson;

                    ctx.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RegistraSolicitudD(ModelSolicitudDispersion modsf)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {
                    var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == modsf.IdfacturasContabilidad).First();

                    string nuevoHtml = query.Costeo_HTML;                    
                    nuevoHtml = nuevoHtml.Replace("*Facturadora*", modsf.Facturadora);
                    nuevoHtml = nuevoHtml.Replace("*ConceptoFacturacion*", modsf.ConceptoFacturacion);
                    nuevoHtml = nuevoHtml.Replace("*RazonCliente*", modsf.Razonsocial);

                    string nuevoJson = null;
                    if (query.Costeo_Json != null && query.Costeo_Json != string.Empty)
                    {
                        var nuevoModel = JsonConvert.DeserializeObject<ModelCosteo>(query.Costeo_Json);
                        nuevoModel.Facturadora = modsf.Facturadora;
                        nuevoModel.RazonCliente = modsf.Razonsocial;
                        nuevoModel.ConceptoFacturacion = modsf.ConceptoFacturacion;

                        nuevoJson = JsonConvert.SerializeObject(nuevoModel);
                    }

                    query.IdClienteRazonSocialFacturacion = modsf.IdClienteRazonSocialFacturacion;
                    query.IdEmpresaFacturadora = modsf.IdEmpresaFacturadora;
                    query.IdConceptoFacturacion = modsf.IdConceptoFacturacion;
                    query.IdEstatusTesoreria = 2;
                    query.IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"];
                    query.FechaSolicitudTesoreria = DateTime.Now;
                    query.ComprobanteAutorizacion = modsf.AutorizacionDispersion;
                    query.ObservacionesParaTesoreria = modsf.Observaciones;
                    query.AdjuntarComprobanteTesoreria = modsf.Comprobante;
                    query.ArchivoBancosTesoreria = modsf.ArchivoBancos;
                    if (nuevoHtml != string.Empty) { query.Costeo_HTML = nuevoHtml; }
                    query.Costeo_Json = nuevoJson;
                    try { query.FechaPago = DateTime.Parse(modsf.FechaPago); } catch { }

                    ctx.SaveChanges();
                }

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}