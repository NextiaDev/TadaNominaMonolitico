using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class AcumularPeriodosController : BaseController
    {
        // GET: AcumularPeriodos
        public ActionResult Index()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                var uni = cun.getUnidadesnegocioId(IdUnidadNegocio);
                if(uni.ValidacionAcumulaPeriodo == "S")
                {
                    var usuariosConAcceso = uni.IdsValidacionAcumulaPeriodo != null && uni.IdsValidacionAcumulaPeriodo.Length > 0 ?
                        uni.IdsValidacionAcumulaPeriodo.Replace(" ", "").Trim().Split(',').ToArray() : new string[0];
                    if (usuariosConAcceso.Contains(Session["sIdUsuario"].ToString()))
                    {
                        ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                        List<ModelPeriodoNomina> model = cperiodo.GetModelPeriodoNominas(IdUnidadNegocio);

                        return View(model);
                    }
                    else
                        return RedirectToAction("Index", "Periodos");
                }
                else
                    return RedirectToAction("Index", "Periodos");
            }
        }

        [HttpGet]
        public JsonResult Detail(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);

            var detalles = cperiodos.getDatosNominaByIdPeriodo(Id);
            
            model.ISR = detalles.Select(x => x.isr).Sum();
            model.CargaObrera = detalles.Select(x => x.cargaObrera).Sum();
            model.CargaPatronal = detalles.Select(x => x.cargaPatronal).Sum();
            model.TotalPercepciones = detalles.Select(x => x.totalPerc).Sum();
            model.TotalDeducciones = detalles.Select(x => x.totalDed).Sum();
            model.NetoPagar = detalles.Select(x => x.neto).Sum();
            

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}