using ClosedXML.Excel;
using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Contabilidad;
using TadaNomina.Models.ViewModels.Contabilidad;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Contabilidad
{
    public class PolizaContableController : BaseController
    {
        public ActionResult Index()
        {
            var cc = new sContabilidad();
            var list = cc.GetCuentas((int)Session["sIdCliente"], 1, null, Session["sToken"].ToString());

            return View(list);
        }

        public ActionResult PolizaGral(int? IdPoliza)
        {
            var cp = new ClassPeriodoNomina();
            var crp = new ClassRegistroPatronal();
            var model = new ModelPolizaGral();
            model.IdPoliza = IdPoliza;
            model.lPeriodos = cp.GetSeleccionPeriodoAcumulado((int)Session["sIdUnidadNegocio"]);
            model.lRegistros = crp.getSelectRegistro((int)Session["sIdCliente"]);
            return View(model);
        }

        [HttpPost]
        public ActionResult PolizaGral(ModelPolizaGral poliza)
        {
            string token = Session["sToken"].ToString();
            int idCliente = (int)Session["sIdCliente"];
            int idUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            var cp = new ClassPeriodoNomina();
            var crp = new ClassRegistroPatronal();
            var sconta = new sContabilidad();
            
            if (poliza.IdPoliza == null) { poliza.IdPoliza = 0; }
            var periodo = cp.GetvPeriodoNominasId(poliza.IdPeriodoNomina);

            poliza.lPeriodos = cp.GetSeleccionPeriodoAcumulado(idUnidadNegocio);
            poliza.lRegistros = crp.getSelectRegistro(idCliente);

            // Si es Wingstop
            if (idCliente == 6)
                poliza.reporteWS = sconta.getReporteWS(token, periodo.FechaInicio.ToShortDateString(), periodo.FechaFin.ToShortDateString(), poliza.IdPeriodoNomina, poliza.IdRegistroPatronal, poliza.RFC);
            // Cualquier otro cliente
            else
                poliza.reporte = sconta.getReporte(token, idUnidadNegocio, poliza.IdPeriodoNomina, (int)poliza.IdPoliza);


            return View(poliza);
        }

        public FileResult Descargar(int IdPeriodoNomina, int IdPoliza)
        {

            var sconta = new sContabilidad();
            var reporte = sconta.getReporte(Session["sToken"].ToString(), (int)Session["sIdUnidadNegocio"], IdPeriodoNomina, IdPoliza);

            var dt = new DataTable();
            using (var reader = ObjectReader.Create(reporte))
            {
                dt.Load(reader);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Grid");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        [ValidateInput(false)]
        public FileResult DescargarWS(string GridHtml)
        {
            return File(Encoding.Default.GetBytes(GridHtml), "application/vnd.ms-excel", "poliza.xls");
        }

        [HttpPost]
        public JsonResult ActualizarInfo(int? IdPeriodoNomina)
        {
            try
            {
                if (IdPeriodoNomina != null)
                {                   
                    var cc = new ClassContabilidad();                    
                    cc.eliminaInfoPoliza((int)IdPeriodoNomina);                    
                    
                    return Json("Exito");
                }
                else
                    throw new Exception("NO se ha seleccionado ningun periodo.");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }            
        }
    }
}