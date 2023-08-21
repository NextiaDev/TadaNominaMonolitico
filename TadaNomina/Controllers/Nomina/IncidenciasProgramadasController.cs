using iTextSharp.text.pdf.qrcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class IncidenciasProgramadasController : BaseController
    {
        // GET: IncidenciasProgramadas
        public ActionResult Index()
        {
            ClassIncidenciasProgramadas cip = new ClassIncidenciasProgramadas();
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            var inc = cip.getModelIncidenciasProgramadas(IdUnidadNegocio);

            return View(inc);
        }

        public ActionResult Create()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            ClassIncidenciasProgramadas cip = new ClassIncidenciasProgramadas();
            var model = cip.LlenaListasIncidencias(IdUnidadNegocio, IdCliente);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelIncidenciasProgramadas m)
        {
            try
            {
                int IdCliente = (int)Session["sIdCliente"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassIncidenciasProgramadas cip = new ClassIncidenciasProgramadas();
                cip.newIncidenciaProgramada(m, IdUsuario);
                var model = cip.LlenaListasIncidencias(IdUnidadNegocio, IdCliente);
                m.LEmpleados = model.LEmpleados;
                m.LConcepto = model.LConcepto;

                m.Validacion = true;
                m.Mensaje = "Se configuro la incidencia de forma correcta";
            }
            catch (Exception ex)
            {
                m.Validacion = false;
                m.Mensaje = "Error: " + ex.Message;
            }

            return View(m);
        }

        public ActionResult Details(int id)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassIncidenciasProgramadas cincidencias = new ClassIncidenciasProgramadas();
            ModelIncidenciasProgramadas incidencia = cincidencias.getModelIncidenciasProgramadas(IdUnidadNegocio, id);

            return PartialView(incidencia);
        }

        public ActionResult Delete(int id)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassIncidenciasProgramadas cincidencias = new ClassIncidenciasProgramadas();
            ModelIncidenciasProgramadas incidencia = cincidencias.getModelIncidenciasProgramadas(IdUnidadNegocio, id);            

            return PartialView(incidencia);
        }
                
        [HttpPost]
        public ActionResult Delete(ModelIncidenciasProgramadas m)
        {
            try
            {
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassIncidenciasProgramadas cIncidencias = new ClassIncidenciasProgramadas();
                cIncidencias.deleteIncidenciaProgramada(m.IdIncidenciaProgramada, IdUsuario);
            }
            catch
            {
                
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassIncidenciasProgramadas cincidencias = new ClassIncidenciasProgramadas();
            ModelIncidenciasProgramadas incidencia = cincidencias.getModelIncidenciasProgramadas(IdUnidadNegocio, Id);

            return View(incidencia);   
        }

        [HttpPost]
        public ActionResult Edit(ModelIncidenciasProgramadas m)
        {
            try
            {
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassIncidenciasProgramadas cip = new ClassIncidenciasProgramadas();
                cip.updateIncidenciaProgramada(m, IdUsuario);

                m.Validacion = true;
                m.Mensaje = "La informacion se actualizo de forma correcta.";
            }
            catch (Exception ex)
            {
                m.Validacion = false;
                m.Mensaje = "Error: " + ex.Message;
            }
            return View(m);
        }
    }
}