using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.IncidenciasPrenomina;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class IncidenciasPrenominaController : Controller
    {
        // GET: IncidenciasPrenomina
        public ActionResult SeleccionarPeriodo()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                cIncidenciasPrenomina classPeriodoNomina = new cIncidenciasPrenomina();
                List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominas(IdUnidadNegocio);

                return View(periodos);
            }
        }

        public ActionResult Index(int pIdPeriodoNomina, int? MostrarTodas, string TipoPeriodo)
        {
            ViewBag.pIdPeriodoNomina = pIdPeriodoNomina;
            ViewBag.TipoPeriodo = TipoPeriodo;

            cIncidenciasPrenomina cIncPre = new cIncidenciasPrenomina();

            PeriodoNomina periodo = cIncPre.GetPeriodo(pIdPeriodoNomina);

            if (periodo.TipoNomina == "Nomina")
            {
                ViewBag.TipoNomina = "Nomina";
            }
            else
            {
                ViewBag.TipoNomina = "ERROR";
            }

            string token = Session["sToken"].ToString();
            ModelIndexIncidencias mii = new ModelIndexIncidencias();
            List<PeriodoNomina> lperiodos = cIncPre.GetPeriodoNominas(pIdPeriodoNomina);
            List<ModelIncidencias> lincidencias = new List<ModelIncidencias>();
            try { lincidencias = cIncPre.GetModelIncidencias(pIdPeriodoNomina, token).OrderByDescending(x => x.IdIncidencia).ToList(); } catch { }

            ClassConceptos cc = new ClassConceptos();

            mii.LConcepto = cc.getSelectConceptos((int)Session["sIdCliente"]);

            ViewBag.Bandera = false;
            if (lincidencias.Count > 500)
            {
                if (MostrarTodas == null)
                {
                    ViewBag.Bandera = true;
                    lincidencias = lincidencias.Take(500).ToList();
                }
            }

            ViewBag.SeleccionarPeriodo = lperiodos;

            mii.Incidencias = lincidencias;
            mii.IdPeridoNomina = pIdPeriodoNomina;
            return View(mii);
        }

        public ActionResult ConfirmarIncidencia(int IdIncidencia, int pIdPeriodoNomina, string TipoPeriodo)
        {
            cIncidenciasPrenomina cIP = new cIncidenciasPrenomina();
            int IdModifica = int.Parse(Session["sIdUsuario"].ToString());
            cIP.ConfirmaIncidencia(IdIncidencia, IdModifica);
            return RedirectToAction("Index", new { pIdPeriodoNomina = pIdPeriodoNomina, TipoPeriodo = TipoPeriodo });
        }
    }
}