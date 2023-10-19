using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class SindicatosClientesController : BaseController
    {
        // GET: SindicatosClientes
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
                var clsArea = new ClassCatSindicatosClientes();
                return View(clsArea.getSindicatosCliente(idcliente));
            }
        }

        public ActionResult Create()
        {
            var sindicato = new ModelCatSindicatosClientes();
            return View(sindicato);
        }

        [HttpPost]

        public ActionResult Create(ModelCatSindicatosClientes collection)
        {
            int idCliente = (int)Session["sIdCliente"];
            try
            {
                if (ModelState.IsValid)
                {
                    var ClassCatSindicatosClientes = new ClassCatSindicatosClientes();
                    ClassCatSindicatosClientes.addSindicatos(collection, idCliente, (int)Session["sIdUsuario"]);
                    return RedirectToAction("Index");
                }
                else
                {
                    var sindicatos = new ClassCatSindicatosClientes();
                    return View(sindicatos);
                }
            }
            catch
            {
                return View();
            }

        }


        public ActionResult Edit(int id)
        {
            var clsArea = new ClassCatSindicatosClientes();
            return View(clsArea.GetModelSindicato(id));
        }

        [HttpPost]
        public ActionResult Edit(int id, ModelCatSindicatosClientes collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idusuario = (int)Session["sIdUsuario"];
                    var cls = new ClassCatSindicatosClientes();
                    cls.UpdateSindicatos(id, idusuario, collection);

                    return RedirectToAction("Index");
                }
                else
                {
                    var clsArea = new ClassCatSindicatosClientes();
                    return View(clsArea.getSindicatosCliente(id));
                }
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Details(int id)
        {
            var cls = new ClassCatSindicatosClientes();
            return PartialView(cls.GetModelSindicato(id));
        }


        public ActionResult Delete(int id)
        {
            var cls = new ClassCatSindicatosClientes();
            return PartialView(cls.GetModelSindicato(id));
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var cls = new ClassCatSindicatosClientes();
                int idUsuario = (int)Session["sIdUsuario"];
                cls.DeleteSindicatos(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}