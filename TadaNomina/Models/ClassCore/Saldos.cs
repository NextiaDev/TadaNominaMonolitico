using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Models.ClassCore
{
    public class Saldos
    {

        public SumaSaldos ListaSaldos(string idunidadNegocio)
        {
            List<SelectListItem> Unidades = new List<SelectListItem>();
            List<SelectListItem> listaFacturadoras = new List<SelectListItem>();
            List<SelectListItem> ListaCosteos = new List<SelectListItem>();

            List<Cat_UnidadNegocio> Lunidadnes = GeTUnidad(idunidadNegocio);
            Lunidadnes.ForEach(x => { Unidades.Add(new SelectListItem { Text = x.UnidadNegocio, Value = x.IdUnidadNegocio.ToString() }); });

            using (var bd = new TadaContabilidadEntities())
            {
                int _IdCliente = (int)System.Web.HttpContext.Current.Session["sIdCliente"];

                var listGrupos = bd.v_Costeos_ClienteGrupo.Where(x => x.IdEstatus == 1 && x.IdCliente == _IdCliente).Select(x => x.IdGrupoFacturacion).ToList();

                var listFact = bd.v_Costeos_Grupos_Facturadoras.Where(x => x.IdEstatus == 1 && listGrupos.Contains(x.IdGrupoFacturacion)).ToList();

                listFact.ForEach(x => { listaFacturadoras.Add(new SelectListItem { Text = x.Facturadora, Value = x.IdFacturadora.ToString() }); });

                listaFacturadoras.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });

                var listCost = bd.vSolicitudFactura.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio.ToString() == idunidadNegocio).ToList();

                listCost.ForEach(x => { ListaCosteos.Add(new SelectListItem { Text = x.Periodo + " " + x.Total, Value = x.IdFacturasContabilidad.ToString() }); });

                ListaCosteos.Insert(0, new SelectListItem { Text = "--Sin Costeo--", Value = "0" });


            }

            SumaSaldos model = new SumaSaldos
            {
                LUnidad = Unidades,
                LFacturadora = listaFacturadoras,
                LCostep = ListaCosteos
            };

            return model;
        }



        public List<Cat_UnidadNegocio> GeTUnidad(string IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var periodos = from b in entidad.Cat_UnidadNegocio.Where(x => x.IdUnidadNegocio.ToString() == IdUnidadNegocio && x.IdEstatus == 1) select b;

                return periodos.ToList();
            }
        }
    }
}