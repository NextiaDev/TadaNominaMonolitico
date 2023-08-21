using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion.Costeo;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassSolicitudFacturas
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

                    string sql = "select ROW_NUMBER() OVER(ORDER BY IdfacturasContabilidad desc) AS Num,IdfacturasContabilidad, idUnidadNegocio, dbo.fu_NombresPeriodos(periodo) as Descripcion, TipoEsquema, SubTotal,IVA, Total,";
                    sql += "case idestatus when 1 then 'COSTEO GUARDADO' when 2 then 'FACTURA SOLICITADA' WHEN 3 THEN 'PENDIENTE DE REP' WHEN 4 THEN 'FINALIZADA' WHEN 5 THEN 'RECHAZADA' WHEN 6 THEN 'CANCELADO' END AS Estatus, costeo_HTML,NTotal";
                    sql += " from FacturasContabilidad WHERE IdEstatus in (1,2,3,5) and IdUnidadNegocio = " + _IdUnidadNegocio + " and year(fechacaptura)=" + anio + " order by IdfacturasContabilidad desc";

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
                        dt.Estatus = dr["Estatus"].ToString();
                        dt.SubTotal = decimal.Parse(dr["SubTotal"].ToString());
                        dt.IVA = decimal.Parse(dr["IVA"].ToString());
                        dt.Total = decimal.Parse(dr["Total"].ToString());
                        dt.NTotal = decimal.Parse(dr["NTotal"].ToString());
                        dt.Costeo_HTML = dr["Costeo_HTML"].ToString();
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

        public List<ModelSolicitudFacturas> listarfinalizadas()
        {
            var _IdUnidadNegocio = (int)System.Web.HttpContext.Current.Session["sIdUnidadNegocio"];
            var anio = DateTime.Now.Year;
            List<ModelSolicitudFacturas> nl = new List<ModelSolicitudFacturas>();

            try
            {
                using (var consql = new SqlConnection(ConfigurationManager.ConnectionStrings["ModelContabilidad"].ToString()))
                {
                    consql.Open();
                    string sql = "select ROW_NUMBER() OVER(ORDER BY IdfacturasContabilidad desc) AS Num,IdfacturasContabilidad, idUnidadNegocio, dbo.fu_NombresPeriodos(periodo) as Descripcion, TipoEsquema, SubTotal,IVA, Total,";
                    sql += "case idestatus when 1 then 'COSTEO GUARDADO' when 2 then 'FACTURA SOLICITADA' WHEN 3 THEN 'PENDIENTE DE REP' WHEN 4 THEN 'FINALIZADA' WHEN 5 THEN 'RECHAZADA' WHEN 6 THEN 'CANCELADO' END AS Estatus, costeo_HTML,NTotal";
                    sql += " from FacturasContabilidad WHERE IdEstatus= 4 and IdUnidadNegocio = " + _IdUnidadNegocio +  " order by IdfacturasContabilidad desc";

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
                        dt.Estatus = dr["Estatus"].ToString();
                        dt.SubTotal = decimal.Parse(dr["SubTotal"].ToString());
                        dt.IVA = decimal.Parse(dr["IVA"].ToString());
                        dt.Total = decimal.Parse(dr["Total"].ToString());
                        dt.NTotal = decimal.Parse(dr["NTotal"].ToString());
                        dt.Costeo_HTML = dr["Costeo_HTML"].ToString();

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


        public List<ModelSolicitudFacturas> listarPendientesTesoreria()
        {

            var _IdUnidadNegocio = (int)System.Web.HttpContext.Current.Session["sIdUnidadNegocio"];
            var anio = DateTime.Now.Year;
            List<ModelSolicitudFacturas> nl = new List<ModelSolicitudFacturas>();

            try
            {
                using (var consql = new SqlConnection(ConfigurationManager.ConnectionStrings["ModelContabilidad"].ToString()))
                {
                    consql.Open();

                    string sql = "select  IdFacturasContabilidad,IdUnidadNegocio,Periodo,TipoEsquema,EstatusTesoreria,Subtotal,IVA,Total,NTotal,Total,Costeo_HTML from v_Tesoreria_Solicitud WHERE IdEstatusTesoreria in (2,4,6) and AdjuntarComprobanteTesoreria is null and Total != 0 and IdUnidadNegocio = " + _IdUnidadNegocio + " and year(fechacaptura)=" + anio + " order by IdfacturasContabilidad desc";

                    var query = new SqlCommand(sql, consql);

                    var dr = query.ExecuteReader();

                    while (dr.Read())
                    {
                        ModelSolicitudFacturas dt = new ModelSolicitudFacturas();
                        dt.IdfacturasContabilidad = int.Parse(dr["IdfacturasContabilidad"].ToString());
                        dt.IdUnidadNegocio = int.Parse(dr["IdUnidadNegocio"].ToString());
                        dt.Periodo = dr["Periodo"].ToString();
                        dt.TipoEsquema = dr["TipoEsquema"].ToString();
                        dt.Estatus = dr["EstatusTesoreria"].ToString();
                        dt.SubTotal = decimal.Parse(dr["SubTotal"].ToString());
                        dt.IVA = decimal.Parse(dr["IVA"].ToString());
                        dt.Total = decimal.Parse(dr["Total"].ToString());
                        dt.NTotal = decimal.Parse(dr["NTotal"].ToString());
                        dt.Costeo_HTML = dr["Costeo_HTML"].ToString();
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


        public List<SelectListItem> ComboRSFacturacion()
        {
            List<SelectListItem> listaRSFacturacion = new List<SelectListItem>();

            using (var bd = new TadaContabilidadEntities())
            {
                int _IdCliente = (int)System.Web.HttpContext.Current.Session["sIdCliente"];

                listaRSFacturacion = (from crsf in bd.ClienteRazonSocialFacturacion
                                      where crsf.IdEstatus == 1 && crsf.IdCliente == _IdCliente
                                      group crsf by new
                                      {
                                          crsf.IdClienteRazonSocialFacturacion,
                                          crsf.RazonSocial,
                                      } into vemgb

                                      select new SelectListItem()
                                      {
                                          Text = vemgb.Key.RazonSocial,
                                          Value = vemgb.Key.IdClienteRazonSocialFacturacion.ToString()

                                      }).ToList();

                listaRSFacturacion.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            }

            return listaRSFacturacion;
        }

        public List<SelectListItem> ComboTipoPago()
        {
            List<SelectListItem> listaTipoPago = new List<SelectListItem>();

            listaTipoPago.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            listaTipoPago.Insert(1, new SelectListItem { Text = "Pago en una unica exibicion", Value = "Unico" });
            listaTipoPago.Insert(2, new SelectListItem { Text = "Pago en parcialidades", Value = "Parcialidades" });

            return listaTipoPago;
        }

        public List<SelectListItem> ComboMetodoPago()
        {
            List<SelectListItem> listaMetodoPago = new List<SelectListItem>();
            listaMetodoPago.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            listaMetodoPago.Insert(1, new SelectListItem { Text = "Transferencia electrónica", Value = "03" });
            listaMetodoPago.Insert(2, new SelectListItem { Text = "Cheque nominativo", Value = "02" });
            listaMetodoPago.Insert(3, new SelectListItem { Text = "Por definir", Value = "99" });

            return listaMetodoPago;
        }

        public List<SelectListItem> ComboCFDI()
        {

            List<SelectListItem> listaCFDI = new List<SelectListItem>();

            listaCFDI.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            listaCFDI.Insert(1, new SelectListItem { Text = "Gastos generales", Value = "G03" });
            listaCFDI.Insert(2, new SelectListItem { Text = "Por definir", Value = "99" });


            return listaCFDI;

        }

        public List<SelectListItem> ComboFacturadoras()
        {
            List<SelectListItem> listaFacturadoras = new List<SelectListItem>();

            using (var bd = new TadaContabilidadEntities())
            {
                int _IdCliente = (int)System.Web.HttpContext.Current.Session["sIdCliente"];
                var listGrupos = bd.v_Costeos_ClienteGrupo.Where(x => x.IdEstatus == 1 && x.IdCliente == _IdCliente).Select(x => x.IdGrupoFacturacion).ToList();
                var listFact = bd.v_Costeos_Grupos_Facturadoras.Where(x => x.IdEstatus == 1 && listGrupos.Contains(x.IdGrupoFacturacion)).ToList();
                listFact.ForEach(x => { listaFacturadoras.Add(new SelectListItem { Text = x.Facturadora, Value = x.IdFacturadora.ToString() }); });
                listaFacturadoras.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            }

            return listaFacturadoras;
        }

        public List<SelectListItem> ComboConceptos(int IdFacturadora)
        {
            List<SelectListItem> listaConceptos = new List<SelectListItem>();

            using (var bd = new TadaContabilidadEntities())
            {
                listaConceptos = (from cf in bd.ConceptosFacturacion
                                  where cf.IdEstatus == 1 && cf.IdFacturadora == IdFacturadora
                                  group cf by new
                                  {
                                      cf.IdConceptoFacturacion,
                                      cf.Concepto,
                                      cf.DescripcionClaveSat,
                                      cf.ClaveSat,
                                  } into vemgb
                                  select new SelectListItem()
                                  {
                                      Text = "(" + vemgb.Key.ClaveSat + ") " + vemgb.Key.DescripcionClaveSat + " -- " + vemgb.Key.Concepto,
                                      Value = vemgb.Key.IdConceptoFacturacion.ToString()

                                  }).OrderBy(x => x.Text).ToList();


                listaConceptos.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            }

            return listaConceptos;
        }

        public List<SelectListItem> ComboConceptos()
        {
            List<SelectListItem> listaConceptos = new List<SelectListItem>();
            listaConceptos.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });

            return listaConceptos;
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
                    query.MetodoPago = modsf.MetodoPago;
                    query.UsoCFDI = modsf.UsoCFDI;
                    query.FormaPago = modsf.FormaPago;
                    query.IdEmpresaFacturadora = modsf.IdEmpresaFacturadora;
                    query.IdConceptoFacturacion = modsf.IdConceptoFacturacion;
                    query.Observaciones = modsf.Observaciones;
                    query.CargaFactura = 0;
                    query.CargaREP = 0;
                    query.IdEstatus = 2;
                    query.IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"];
                    query.FechaCaptura = DateTime.Now;
                    query.AdjuntarComprobante = modsf.Comprobante;
                    query.ArchivoBancos = modsf.ArchivoBancos;
                    if (nuevoHtml != string.Empty) { query.Costeo_HTML = nuevoHtml; }
                    query.Costeo_Json = nuevoJson;
                    if (modsf.Monto != 0) {

                        var saplicado = (decimal)modsf.Total - (decimal)modsf.Monto;

                        query.SaldoAplicado = (decimal)modsf.Monto;
                        query.NTotal = (decimal)saplicado;

                    }



                    ctx.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Eliminar(ModelSolicitudFacturas modsf)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {  //Eliminar
                    var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == modsf.IdfacturasContabilidad).First();
                    query.IdEstatus = (int)modsf.IdEstatus;
                    query.IdCancelacion = (int)modsf.IdCancelacion;

                    ctx.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
                
        public ModelSolicitudFacturas listarArchivos( int _IdFacturaC)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from b in ctx.FacturasContabilidad                       
                        where b.IdFacturasContabilidad == _IdFacturaC
                        select new ModelSolicitudFacturas
                        {
                            IdfacturasContabilidad = b.IdFacturasContabilidad,
                            IdClienteRazonSocialFacturacion = b.IdClienteRazonSocialFacturacion,
                            IdConceptoFacturacion = b.IdConceptoFacturacion,
                            IdEmpresaFacturadora = b.IdEmpresaFacturadora,
                            FacturaPDF = b.FacturaPDF,
                            FacturaXML = b.FacturaXML,
                            REP_PDF = b.REP_PDF,
                            REP_XML = b.REP_XML,
                            IdEstatus = (int)b.IdEstatus,
                            ArchivoBancos  = b.ArchivoBancos,
                            Comprobante = b.AdjuntarComprobante,
                            ComprobanteTesoreria = b.AdjuntarComprobanteTesoreria,
                            Total = (decimal)b.Total,
                            
                        }).First();
            }            
        }

        public string TraeCosteo(int IdFactura)
        {            
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == IdFactura).First();

                string costeo = query.Costeo_HTML;                   

                return costeo;
            }
         }

        public void newSolicitud(int IdCliente, string Cliente, int IdUnidadNegocio, string Unidad, string Descripcion, string Esquema, decimal Importe, decimal Honorario, decimal Subtotal, decimal iva, decimal total, int IdUsuario)
        {
            var IdPer = newPeriodoNomina(IdUnidadNegocio, Descripcion, IdUsuario);

            if (IdPer != 0)
            {
                using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
                {
                    var sol = new FacturasContabilidad();
                    sol.IdCliente = IdCliente;
                    sol.IdUnidadNegocio = IdUnidadNegocio;
                    sol.Periodo = IdPer.ToString();
                    sol.Importe = Importe;
                    sol.Honorario = Honorario;
                    sol.Subtotal = Subtotal;
                    sol.IVA = iva;
                    sol.Total = total;
                    sol.NTotal = total;
                    sol.SaldoAplicado = 0;
                    sol.SaldoFavor = 0;
                    sol.TipoEsquema = Esquema;
                    sol.IdEstatus = 1;
                    sol.IdEstatusTesoreria = 1;
                    sol.IdCaptura = IdUsuario;
                    sol.FechaCaptura = DateTime.Now;
                    sol.Costeo_HTML = getCadenaCosteoSN(Importe, Honorario, Subtotal, iva, total, Esquema, Cliente, Unidad);
                    sol.Costeo_Json = string.Empty;

                    entidad.FacturasContabilidad.Add(sol);
                    entidad.SaveChanges();
                }                                  
            }
        }

        public int newPeriodoNomina(int IdUnidadNegocio, string Descripcion, int IdUsuario)
        {
            var cp = new ClassPeriodoNomina();
            var per = new ModelPeriodoNomina();
            per.IdUnidadNegocio = IdUnidadNegocio;
            per.Periodo = Descripcion;
            per.FechaInicio = DateTime.Now.ToString("yyyy-MM-dd");
            per.FechaFin = DateTime.Now.ToString("yyyy-MM-dd");
            per.Observaciones = "Periodo especial para solicitud de factura";
            per.TipoNomina = "Especial";
            per.OmitirDescuentosFijos = true;
            per.AjusteImpuestos = "NO";

            var id = cp.AddPeriodoNominaId(per, IdUsuario);

            return id;
        }

        public string getCadenaCosteoSN(decimal Importe, decimal Honorario, decimal Subtotal, decimal IVA, decimal Total, string Esquema, string Cliente, string Unidad)
        {
            string tabla = string.Empty;


            tabla += "<div class='row'>";
            //Columna 1
            tabla += "<div class='col-md-6'>";
            //detalle del costeo
            tabla += "<table class='table table-bordered table-condensed table-sm input-sm'>";
            tabla += "<thead style = 'background: rgba(0, 0, 150, 0.1); border: 1px solid rgba(0, 0, 200, 0.2);'>";
            tabla += "<tr><th width='50%' colspan='2'><center> MOVIMIENTO / " + Esquema + "</center></th></tr>";
            tabla += "<tr><th width='50%'>Cliente: </th><th>" + Cliente + " - " + Unidad + "</th></tr>";
            tabla += "<tr><th width='50%'>Depositan en: </th><th>*Facturadora*</th></tr>";
            tabla += "<tr><th width='50%'>Razon Social Cliente: </th><th>*RazonCliente*</th></tr>";
            tabla += "<tr><th width='50%'>Concepto de Factura: </th><th>*ConceptoFacturacion*</th></tr>";
            tabla += "<tr><th width='50%'>Concepto</th><th>Monto</th></tr>";
            tabla += "</thead>";
            tabla += "<tbody>";

            //Importe
            tabla += "<tr>";
            tabla += "<td width='50%'>Importe</td>";
            tabla += "<td style='text-align:right'>" + Importe.ToString("C") + "</td>";
            tabla += "</tr>";

            //Honorario
            tabla += "<tr>";
            tabla += "<td width='50%'>Honorario</td>";
            tabla += "<td style='text-align:right'>" + Honorario.ToString("C") + "</td>";
            tabla += "</tr>";

            //Subtotal
            tabla += "<tr>";
            tabla += "<td width='50%'>Subtotal</td>";
            tabla += "<td style='text-align:right'>" + Subtotal.ToString("C") + "</td>";
            tabla += "</tr>";

            //IVA
            tabla += "<tr>";
            tabla += "<td width='50%'>IVA</td>";
            tabla += "<td style='text-align:right'>" + IVA.ToString("C") + "</td>";
            tabla += "</tr>";

            //TOTAL
            tabla += "<tr>";
            tabla += "<td width='50%'>TOTAL</td>";
            tabla += "<td style='text-align:right'>" + Total.ToString("C") + "</td>";
            tabla += "</tr>";

            tabla += "</tbody>";
            tabla += "</table>";
            tabla += "</div>";

            //Columna 2
            tabla += "<div class='col-md-6'>";
            tabla += "</div>";
            tabla += "</div>";
            tabla += "<br/>";

            return tabla;
        }



        
        public List<ModelSolicitudFacturas> listarPendientes()
        {
            var _IdUnidadNegocio = (int)System.Web.HttpContext.Current.Session["sIdUnidadNegocio"];
            var anio = DateTime.Now.Year;
            List<ModelSolicitudFacturas> nl = new List<ModelSolicitudFacturas>(); try
            {
                using (var consql = new SqlConnection(ConfigurationManager.ConnectionStrings["ModelContabilidad"].ToString()))
                {
                    consql.Open(); string sql = "select ROW_NUMBER() OVER(ORDER BY IdfacturasContabilidad desc) AS Num,IdfacturasContabilidad, idUnidadNegocio, dbo.fu_NombresPeriodos(periodo) as Descripcion, TipoEsquema, SubTotal,IVA, Total,";
                    sql += "case idestatus when 1 then 'COSTEO GUARDADO' when 2 then 'FACTURA SOLICITADA' WHEN 3 THEN 'PENDIENTE DE REP' WHEN 4 THEN 'FINALIZADA' WHEN 5 THEN 'RECHAZADA' WHEN 6 THEN 'CANCELADO' END AS Estatus, costeo_HTML,NTotal";
                    sql += " from FacturasContabilidad WHERE IdEstatus in (2,3,4,5) and AdjuntarComprobante is null and Total != 0 and IdUnidadNegocio = " + _IdUnidadNegocio + " and year(fechacaptura)=" + anio + " order by IdfacturasContabilidad desc"; var query = new SqlCommand(sql, consql); var dr = query.ExecuteReader(); while (dr.Read())
                    {
                        ModelSolicitudFacturas dt = new ModelSolicitudFacturas();
                        dt.Num = int.Parse(dr["Num"].ToString());
                        dt.IdfacturasContabilidad = int.Parse(dr["IdfacturasContabilidad"].ToString());
                        dt.IdUnidadNegocio = int.Parse(dr["IdUnidadNegocio"].ToString());
                        dt.Descripcion = dr["Descripcion"].ToString();
                        dt.TipoEsquema = dr["TipoEsquema"].ToString();
                        dt.Estatus = dr["Estatus"].ToString();
                        dt.SubTotal = decimal.Parse(dr["SubTotal"].ToString());
                        dt.IVA = decimal.Parse(dr["IVA"].ToString());
                        dt.Total = decimal.Parse(dr["Total"].ToString());
                        dt.NTotal = decimal.Parse(dr["NTotal"].ToString());
                        dt.Costeo_HTML = dr["Costeo_HTML"].ToString();
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


        
        public bool RegistraSolicitudPendiente(ModelSolicitudFacturas modsf)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {
                    var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == modsf.IdfacturasContabilidad).First();
                    query.ObservacionesRep = modsf.ObservacionesRep;
                    query.IdModificacion = (int)System.Web.HttpContext.Current.Session["sIdUsuario"];
                    query.FechaModificacion = DateTime.Now;
                    query.FechaComprobantePago = DateTime.Now;
                    query.AdjuntarComprobante = modsf.Comprobante;
                    ctx.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RegistraSolicitudPendienteTesoreria(ModelSolicitudFacturas modsf)
        {
            try
            {
                using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
                {
                    var query = ctx.FacturasContabilidad.Where(x => x.IdFacturasContabilidad == modsf.IdfacturasContabilidad).First();                   
                    query.IdModificacion = (int)System.Web.HttpContext.Current.Session["sIdUsuario"];
                    query.FechaModificacion = DateTime.Now;
                    query.FechaComprobantePago = DateTime.Now;
                    query.AdjuntarComprobanteTesoreria = modsf.Comprobante;
                    ctx.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ModelV_SaldosClientes VerificaSaldo(int IdFacturadora) 
        {
            int _IdCliente = (int)System.Web.HttpContext.Current.Session["sIdCliente"];

            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {


                return (from b in ctx.v_saldosclientes
                        where b.IdFacturadora == IdFacturadora && b.IdCliente == _IdCliente
                        select new ModelV_SaldosClientes
                        {
                            Cliente = b.Cliente,
                            Facturadora = b.Facturadora,
                            IdCliente = (int)b.IdCliente,
                            IdFacturadora = b.IdFacturadora,
                            Saldo = (decimal)b.Saldo
                        }).FirstOrDefault();
            }

        }

    }
}