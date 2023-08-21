using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;

namespace TadaNomina.Controllers.CFDI
{
    public class ConceptosFacturacionController : BaseController
    {
        // GET: ConceptosFacturacion
        public ActionResult Index()
        {

            ClassConceptosFacturacion _CCF = new ClassConceptosFacturacion();

            var _ListaCCF = _CCF.ListarConceptosFact();

            return View(_ListaCCF);
        }

        public ActionResult Crear()
        {
            ClassFacturadoras _CLSFacturadoras = new ClassFacturadoras();
            ClassConceptosFacturacion _CLFConceptosFac = new ClassConceptosFacturacion();
            ModelConceptosFacturacion _ModFConceptosFac = new ModelConceptosFacturacion();


            ViewBag._lstFacturadoras = _CLFConceptosFac.ListarFacturadoras(_CLSFacturadoras.ListaFacturadoras());

            return View(_ModFConceptosFac);
        }

        [HttpPost]
        public ActionResult Crear(ModelConceptosFacturacion _MCF)
        {
            ClassConceptosFacturacion _CCF = new ClassConceptosFacturacion();
            _MCF.IdCaptura = int.Parse(Session["sIdUsuario"].ToString());

            _CCF.Crear(_MCF);

            return RedirectToAction("Index", "ConceptosFacturacion");
        }

        public ActionResult Editar(int _id)
        {
            ModelConceptosFacturacion _MCF = new ModelConceptosFacturacion();
            ClassFacturadoras _CLSFacturadoras = new ClassFacturadoras();
            ClassConceptosFacturacion _CLFConceptosFac = new ClassConceptosFacturacion();

            var lst_ConceoptoFac = _CLFConceptosFac.ListarMCF(_id);

            ViewBag._lstFacturadoras = _CLFConceptosFac.ListarFacturadoras(_CLSFacturadoras.ListaFacturadoras());

            return View(lst_ConceoptoFac);
        }


        [HttpPost]
        public ActionResult Editar(ModelConceptosFacturacion _MCF)
        {
            ClassConceptosFacturacion _CCF = new ClassConceptosFacturacion();
            _MCF.IdCaptura = int.Parse(Session["sIdUsuario"].ToString());

            _CCF.Editar(_MCF);
            return RedirectToAction("Index", "ConceptosFacturacion");
        }

        public ActionResult Eliminar(int _Id)
        {
            ClassConceptosFacturacion ConceptoFacturadoras = new ClassConceptosFacturacion();
            int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());

            ConceptoFacturadoras.Eliminar(_Id, IdUsuario);

            return RedirectToAction("Index", "ConceptosFacturacion");
        }
    }
}