using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Controllers.Administracion
{
    public class SindicatosController : BaseController
    {
        // GET: Sindicatos
        /// <summary>
        /// Acción que muestra los sindicatos.
        /// </summary>
        /// <returns>Regresa la vista del listado de sindicatos.</returns>
        public ActionResult Index()
        {
            if (Session["sTipoUsuario"].ToString() != "System")
            {
                return RedirectToAction("Index", "Index");
            }
            ClassSindicatos csindicatos = new ClassSindicatos();
            var sindicatos = csindicatos.GetSindicatos();
            return View(sindicatos);
        }

        //GET: Sindicatos/Create
        /// <summary>
        /// Acción que genera un sindicato nuevo.
        /// </summary>
        /// <returns>Regresa la vista principal.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Acción que guarda el sindicato nuevo.
        /// </summary>
        /// <param name="s">Recibe el modelo de sindicatos.</param>
        /// <returns>Regresa la vista con los sindicatos.</returns>
        [HttpPost]
        public ActionResult Create(ModelSindicatos s)
        {
            int IdUsuario = (int)Session["sIdUsuario"];
            ClassSindicatos cs = new ClassSindicatos();

            if (!ModelState.IsValid)
            {
                return View(s);
            }
            else
            {
                cs.newSindicato(s, IdUsuario);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Acción que modifica un sindicato.
        /// </summary>
        /// <param name="id">Recibe el identificador del sindicato.</param>
        /// <returns>Regresa la vista con el sindicato específico.</returns>
        public ActionResult Edit(int id)
        {
            ClassSindicatos cs = new ClassSindicatos();
            var m = cs.GetModelSindicatos(id);
            return View(m);
        }

        /// <summary>
        /// Acción que guarda la modificación del sindicato.
        /// </summary>
        /// <param name="id">Recibe el identificador del sindicato.</param>
        /// <param name="s">Recibe el modelo del sindicato.</param>
        /// <returns>Regresa la vista de los sindicatos.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelSindicatos s)
        {
            int idUsuario = (int)Session["sIdUsuario"];
            ClassSindicatos cs = new ClassSindicatos();

            if (!ModelState.IsValid)
            {
                return View(s);
            }
            else
            {
                cs.EditSindicato(id, s, idUsuario);
            }
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Acciónn que elimina un sindicato.
        /// </summary>
        /// <param name="id">Recibe el identificador del sindicato.</param>
        /// <returns>Regresa el modelo del sindicato.</returns>
        public ActionResult Delete(int id)
        {
            ClassSindicatos cs = new ClassSindicatos();
            var m = cs.GetModelSindicatos(id);
            return View(m);
        }

        /// <summary>
        /// Acción que confirma la eliminación del sindicato.
        /// </summary>
        /// <param name="_id">Recibe el identificador del sindicato.</param>
        /// <returns>Regresa la vista de los sindicatos.</returns>
        public ActionResult Borrar(int _id)
        {
            int idUsuario = (int)Session["sIdUsuario"];
            ClassSindicatos cs = new ClassSindicatos();

            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                cs.DeleteUser(_id, idUsuario);
            }
            return RedirectToAction("Index");

        }

    }
}