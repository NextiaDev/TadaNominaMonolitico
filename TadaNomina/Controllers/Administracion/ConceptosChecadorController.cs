using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.RelojChecador;
using TadaNomina.Models.ClassCore.RelojChecador;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers.Administracion
{
    public class ConceptosChecadorController : Controller
    {
        // GET: ConceptosChecador
        /// <summary>
        /// Acción que lista los conceptos del checador.
        /// </summary>
        /// <returns>Regresa la vista con la lista de los conceptos del cjecador.</returns>
        public ActionResult Index()
        {
            try
            {
                cConceptosChecador ccc = new cConceptosChecador();
                int idCliente = int.Parse(Session["sIdCliente"].ToString());
                ViewBag.LstConceptos = ccc.GetConceptos(idCliente);
                return View();
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Acción que agrega un concepto de checador.
        /// </summary>
        /// <returns>Regresa la vista de conceptos de checador.</returns>
        public ActionResult AddConcepto()
        {
            cConceptosChecador ccc = new cConceptosChecador();
            cRegistroAsistencias cRegistro = new cRegistroAsistencias();
            ClassAccesosGV CAGV = new ClassAccesosGV();

            int idCliente = int.Parse(Session["sIdCliente"].ToString());
            var Accesos = CAGV.DatosGV(idCliente);
            string tokenGV = cRegistro.GetToken(Accesos).token;

            var lstCNomina = ccc.LstConceptosNomina(idCliente);
            var lstCGV = ccc.LstTimeOffs(ccc.GetTimeOffs(tokenGV));

            ViewBag.LstCNomina = lstCNomina;
            ViewBag.lstCGV = lstCGV;

            return View();
        }

        /// <summary>
        /// Accion que guarda el concepto de checador.
        /// </summary>
        /// <param name="ccm">Recibe el modelo de conceptos de checador.</param>
        /// <returns>Regresa la vista con la lista de conceptos de checador.</returns>
        [HttpPost]
        public ActionResult AddConcepto(ConceptosChecadorModel ccm)
        {
            if (ModelState.IsValid)
            {
                cConceptosChecador ccc = new cConceptosChecador();
                cRegistroAsistencias cRegistro = new cRegistroAsistencias();
                ClassAccesosGV CAGV = new ClassAccesosGV();

                int idCliente = int.Parse(Session["sIdCliente"].ToString());
                var Accesos = CAGV.DatosGV(idCliente);
                string tokenGV = cRegistro.GetToken(Accesos).token;

                int idCaptura = int.Parse(Session["sIdUsuario"].ToString());
                ccm.Pagable = ccc.Pagable(ccm, tokenGV);

                ccc.AddConceptosChecador(ccm, idCaptura);

                return RedirectToAction("Index");

            }
            return View(ccm);
        }

        /// <summary>
        /// Acción que modifica los datos del concepto de checador por identificador específico.
        /// </summary>
        /// <param name="Id">Recibe el identificador del concepto.</param>
        /// <returns>Regresa la vista con el modelo de conceptos de checador.</returns>
        public ActionResult EditConcepto(string Id)
        {
            int IdConcepto = int.Parse(Statics.DesEncriptar(Id));
            cConceptosChecador ccc = new cConceptosChecador();
            cRegistroAsistencias cRegistro = new cRegistroAsistencias();
            ClassAccesosGV CAGV = new ClassAccesosGV();

            int idCliente = int.Parse(Session["sIdCliente"].ToString());
            var Accesos = CAGV.DatosGV(idCliente);
            string tokenGV = cRegistro.GetToken(Accesos).token;

            var ccm = ccc.concepto(IdConcepto);

            var sliNomina = ccc.LstConceptosNomina(idCliente);
            var lstCGV = ccc.LstTimeOffs(ccc.GetTimeOffs(tokenGV));

            ViewBag.LstCNomina = sliNomina;
            ViewBag.lstCGV = lstCGV;

            return View(ccm);
        }

        /// <summary>
        /// Acción que guarda la modificación del concepto del checador.
        /// </summary>
        /// <param name="ccm">Recibe el modelo de conceptos de checador.</param>
        /// <returns>Regresa la vista con la lista de los conceptos de checador.</returns>
        [HttpPost]
        public ActionResult EditConcepto(ConceptosChecadorModel ccm)
        {
            if (ModelState.IsValid)
            {
                cConceptosChecador ccc = new cConceptosChecador();
                cRegistroAsistencias cRegistro = new cRegistroAsistencias();
                ClassAccesosGV CAGV = new ClassAccesosGV();

                int idCliente = int.Parse(Session["sIdCliente"].ToString());
                var Accesos = CAGV.DatosGV(idCliente);
                string tokenGV = cRegistro.GetToken(Accesos).token;

                int idCaptura = int.Parse(Session["sIdUsuario"].ToString());
                ccm.Pagable = ccc.Pagable(ccm, tokenGV);

                ccc.EditConceptosChecador(ccm, idCaptura);

                return RedirectToAction("Index");


            }
            return View(ccm);
        }

        /// <summary>
        /// Acción que elimina un registro de conceptos de checador.
        /// </summary>
        /// <param name="IdConcepto">Recibe el identificador del concepto del checador.</param>
        /// <returns>Regresa la vista con la lista de los conceptos de checador.</returns>
        [HttpPost]
        public ActionResult DeleteConcepto(int IdConcepto)
        {
            cConceptosChecador ccc = new cConceptosChecador();

            int idModifica = int.Parse(Session["sIdUsuario"].ToString());

            ccc.DeleteConceptosChecador(IdConcepto, idModifica);

            return RedirectToAction("Index");
        }
    }
}