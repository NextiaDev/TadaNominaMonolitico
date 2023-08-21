using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class JornadaLaboralController : Controller
    {
        // GET: JornadaLaboral
        public ActionResult Index()
        {
            int idcliente = 0;
            try { idcliente = (int)Session["sIdCliente"]; } catch { idcliente = 0; }

            if (idcliente == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                var clsArea = new ClassJornadaL();
                return View(clsArea.GetJornadas(idcliente));
            }
        }

        public ActionResult Details(int id)
        {
            var claJornada = new ClassJornadaL();
            return PartialView(claJornada.GetJornadaid(id));
        }


        public ActionResult Create()
        {
            var areas = new ModelJornada();
            return View(areas);
        }

        [HttpPost]

        public ActionResult Create(ModelJornada collection)
        {
            int idCliente = (int)Session["sIdCliente"];
            try
            {
                if (ModelState.IsValid)
                {
                    var claJornada = new ClassJornadaL();
                    claJornada.addJornada(collection, idCliente, (int)Session["sIdUsuario"]);
                    return RedirectToAction("Index");
                }
                else
                {
                    var ce = new ModelJornada();
                    return View(ce);
                }
            }
            catch
            {
                return View();
            }

        }


        public ActionResult Edit(int id)
        {
            var cls = new ClassJornadaL();
            return View(cls.GetJornadaid(id));
        }


        [HttpPost]
        public ActionResult Edit(int id, ModelJornada collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idusuario = (int)Session["sIdUsuario"];
                    var clsArea = new ClassJornadaL();
                    clsArea.UpdateJornada(id, idusuario, collection);

                    return RedirectToAction("Index");
                }
                else
                {
                    var clsArea = new ClassJornadaL();
                    return View(clsArea.GetJornadaid(id));
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            var cls = new ClassJornadaL();
            return PartialView(cls.GetJornadaid(id));
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var cls = new ClassJornadaL();
                int idUsuario = (int)Session["sIdUsuario"];
                cls.DeleteJornada(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


    }
}