using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.RelojChecador;

namespace TadaNomina.Models.ClassCore.RelojChecador
{
    public class cConceptosChecador
    {
        public List<ConceptosChecadorModel> GetConceptos(int IdCliente)
        {
            TadaNominaEntities ctx = new TadaNominaEntities();
            using (ctx)
            {
                var conceptos = (from ccn in ctx.Cat_ConceptosNomina
                                 join ccc in ctx.Cat_ConceptosChecador on ccn.IdConcepto equals ccc.IdConceptoNomina
                                 where ccn.IdCliente == IdCliente && ccn.IdEstatus == 1 && ccc.IdEstatus == 1
                                 select new ConceptosChecadorModel
                                 {
                                     IdConceptoChecador = ccc.IdConceptoChecador,
                                     IdConceptoNomina = ccn.IdConcepto,
                                     ClaveConcepto = ccn.ClaveConcepto,
                                     ClaveSAT = ccn.ClaveSAT,
                                     Concepto = ccn.Concepto,
                                     DescripcionGV = ccc.DescripcionGV,
                                     Pagable = ccc.Pagable
                                 }).ToList();
                return conceptos;
            }
        }

        public List<SelectListItem> LstConceptosNomina(int IdCliente)
        {
            TadaNominaEntities ctx = new TadaNominaEntities();
            using (ctx)
            {
                var conceptos = (from ccn in ctx.Cat_ConceptosNomina
                                 where ccn.IdCliente == IdCliente && ccn.IdEstatus == 1
                                 select new SelectListItem
                                 {
                                     Value = ccn.IdConcepto.ToString(),
                                     Text = ccn.ClaveConcepto + " - " + ccn.Concepto
                                 }).ToList();
                return conceptos;
            }
        }

        public List<TimeOffModel> GetTimeOffs(string token)
        {
            List<TimeOffModel> GetTypes;
            var client = new RestClient(Statics.ServidorGeoVictoriaToken + "/api/v1/TimeOff/GetTypes");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", token);
            IRestResponse response = client.Execute(request);
            GetTypes = new JavaScriptSerializer().Deserialize<List<TimeOffModel>>(response.Content);
            GetTypes.Add(new TimeOffModel
            {
                Id = "1619",
                TranslatedDescription = "Retardo",
                IsPayable = "true",
                Status = "enabled"
            });
            return GetTypes;
        }

        public List<SelectListItem> LstTimeOffs(List<TimeOffModel> LstTO)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            foreach (var item in LstTO)
            {
                if (item.Status == "enabled")
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.Id,
                        Text = item.TranslatedDescription
                    });
                }
            }

            return lst;
        }

        public ConceptosChecadorModel concepto(int idConcepto)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var concepto = (from c in ctx.Cat_ConceptosChecador.Where(x => x.IdConceptoChecador == idConcepto && x.IdEstatus == 1) select new ConceptosChecadorModel
                {
                    IdConceptoChecador = c.IdConceptoChecador,
                    IdConceptoNomina = c.IdConceptoNomina,
                    IdConceptoGV = c.IdConceptoGV,
                    DescripcionGV = c.DescripcionGV
                }).First();
                return concepto;
            }
        }

        public bool Pagable(ConceptosChecadorModel ccm, string token)
        {
            bool r;
            List<TimeOffModel> lstConceptos = GetTimeOffs(token);
            string pagable = lstConceptos.Where(x => x.Id == ccm.IdConceptoGV && x.Status == "enabled").Select(x => x.IsPayable).FirstOrDefault();
            if (pagable == "True")
            {
                r = true;
            }
            else if (ccm.IdConceptoGV == "1619")
            {
                r = true;
            }
            else
            {
                r = false;
            }
            return r;
        }

        public void AddConceptosChecador(ConceptosChecadorModel ccm, int IdCaptura)
        {
            TadaNominaEntities ctx = new TadaNominaEntities();
            using (ctx)
            {
                Cat_ConceptosChecador ccc = new Cat_ConceptosChecador();
                {
                    ccc.IdConceptoNomina = ccm.IdConceptoNomina;
                    ccc.IdConceptoGV = ccm.IdConceptoGV;
                    ccc.DescripcionGV = ccm.DescripcionGV;
                    ccc.IdEstatus = 1;
                    ccc.IdCaptura = IdCaptura;
                    ccc.FechaCaptura = DateTime.Now;
                    ccc.Pagable = ccm.Pagable;

                    ctx.Cat_ConceptosChecador.Add(ccc);
                    ctx.SaveChanges();
                }
            }
        }

        public void EditConceptosChecador(ConceptosChecadorModel ccm, int IdModifica)
        {
            TadaNominaEntities ctx = new TadaNominaEntities();
            using (ctx)
            {
                var concepto = (from ccc in ctx.Cat_ConceptosChecador.Where(x=>x.IdConceptoChecador == ccm.IdConceptoChecador && x.IdEstatus == 1) select ccc).FirstOrDefault();
                if (concepto != null)
                {
                    concepto.IdConceptoNomina = ccm.IdConceptoNomina;
                    concepto.IdConceptoGV = ccm.IdConceptoGV;
                    concepto.DescripcionGV = ccm.DescripcionGV;
                    concepto.IdModifica = IdModifica;
                    concepto.FechaModifica = DateTime.Now;
                    concepto.Pagable = ccm.Pagable;

                    ctx.SaveChanges();
                }
            }
        }

        public void DeleteConceptosChecador(int IdConceptoChecador, int IdModifica)
        {
            TadaNominaEntities ctx = new TadaNominaEntities();
            using (ctx)
            {
                var concepto = (from ccc in ctx.Cat_ConceptosChecador.Where(x => x.IdConceptoChecador == IdConceptoChecador && x.IdEstatus == 1) select ccc).FirstOrDefault();
                if (concepto != null)
                {
                    concepto.IdEstatus = 2;
                    concepto.IdModifica = IdModifica;
                    concepto.FechaModifica= DateTime.Now;

                    ctx.SaveChanges();
                }
            }
        }
    }
}