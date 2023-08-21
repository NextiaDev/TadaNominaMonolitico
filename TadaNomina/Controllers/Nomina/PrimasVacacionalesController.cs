using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PrimasVacacionalesController : BaseController
    {
        // GET: PrimasVacacionales
        public ActionResult Index()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                if (!cpv.validaConfiguracion(IdUnidadNegocio))
                    ViewBag.MensajeValidacion = "Nota: El sistema no puede realizar el calculo debido a: Falta configurar las fechas con las que se calcularan las primas vacacionales.";

                ClassPeriodoNomina classPeriodoNomina = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetModelPeriodoNominas(IdUnidadNegocio);

                return View(periodos);
            }
        }

        public ActionResult CalculaPrimas(int pIdPeriodoNomina, string pNombrePeriodo)
        {
            try
            {
                ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                ViewBag.NombrePeriodo = pNombrePeriodo;
                ViewBag.pIdPeriodoNomina = pIdPeriodoNomina;
                var model = cpv.GetPrimasVacacionalesPeriodo(pIdPeriodoNomina);

                return View(model);
            }
            catch (Exception ex)
            {                
                ViewBag.MensajeValidacion = ex.Message;
                return RedirectToAction("Index");
            }            
        }

        public ActionResult AplicarPrimas(int pIdPeriodoNomina)
        {
            try
            {
                int IdCliente = int.Parse(Session["sIdCliente"].ToString());
                int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());
                ClassPrimasVacacionales cpv = new ClassPrimasVacacionales();
                var model = cpv.GetPrimasVacacionalesPeriodo(pIdPeriodoNomina);

                if (cpv.ValidaConfiguracionConcepto(IdCliente) != null)
                {
                    cpv.AplicaPrimasVacacionales(model, (int)cpv.ValidaConfiguracionConcepto(IdCliente), IdUsuario, pIdPeriodoNomina);
                    ViewBag.MensajeValidacion = "Las incidencias se ingresaron correctamente";
                    return View();
                }
                else
                {
                    ViewBag.MensajeValidacion = "Nota: El sistema no puede realizar el calculo debido a: Falta configurar el Concepto de Prima Vacacional.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.MensajeValidacion = ex.Message;
                return View();
            }


            

           
        }
    }
}
