using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Nomina
{
    public class NominasAcumuladasController : BaseController
    {
        // GET: NominasAcumuladas
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();

            return View(periodos);           
        }

        public ActionResult NominaGeneral(int IdPeriodoNomina)
        {
            ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
            ClassNomina classNomina = new ClassNomina();

            ModelProcesaNominaGeneral modelo = classNomina.GetModelProcesaNominaGeneral(classPeriodoNomina.GetvPeriodoNominasId(IdPeriodoNomina));
            return View(modelo);
        }
    }
}
