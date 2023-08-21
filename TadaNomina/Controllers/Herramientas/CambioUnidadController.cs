using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Herramientas;

namespace TadaNomina.Controllers.Herramientas
{
    public class CambioUnidadController : BaseController
    {
        // GET: CambioUnidad
        public ActionResult Index()
        {
            int IdUnidadActual = (int)Session["sIdUnidadNegocio"];
            mCambioUnidad mc = new mCambioUnidad();
            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
            List<SelectListItem> lunidades = new List<SelectListItem>();
            var unidades = cu.getUnidadesnegocio((int)Session["sIdCliente"]).Where(x=> x.IdUnidadNegocio != IdUnidadActual).ToList();
            unidades.ForEach(x=> { lunidades.Add(new SelectListItem { Text = x.UnidadNegocio, Value = x.IdUnidadNegocio.ToString() }); });
            mc.lUnidadNegocio = lunidades;

            return View(mc);
        }

        [HttpPost]
        public ActionResult Index(mCambioUnidad m)
        {
            try
            {                
                int IdUnidadActual = (int)Session["sIdUnidadNegocio"];
                ClassUnidadesNegocio cu = new ClassUnidadesNegocio();
                List<SelectListItem> lunidades = new List<SelectListItem>();
                var unidades = cu.getUnidadesnegocio((int)Session["sIdCliente"]).Where(x => x.IdUnidadNegocio != IdUnidadActual).ToList();
                unidades.ForEach(x => { lunidades.Add(new SelectListItem { Text = x.UnidadNegocio, Value = x.IdUnidadNegocio.ToString() }); });
                m.lUnidadNegocio = lunidades;

                if (m.Claves == null || m.Claves == string.Empty) { throw new Exception("No se ingreso ninguna clave."); }
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassEmpleado ce = new ClassEmpleado();
                var _claves = m.Claves.Replace(" ", "").Split(',').ToArray();
                var model = ce.GetEmpleadosActivosByUnidadNegocioYClave(_claves, IdUnidadActual);
                ce.CambiaUnidadNegocio(model, m.NuevaUnidadNegocio, IdUsuario);

                m.Validacion = true;
                m.Mensaje = "Se migraron correctamente los empleados";
            }
            catch (Exception ex)
            {
                m.Validacion = false;
                m.Mensaje = "No se migraron los empleados: " + ex.Message;
            }

            return View(m);

        }

        public JsonResult getEmpleados( string Claves)
        {
            int IdUnidadActual = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado ce = new ClassEmpleado();
            var _claves = Claves.Replace(" ", "").Split(',').ToArray();
            var model = ce.GetEmpleadosByClave(_claves, IdUnidadActual).Select(x=> new { Clave = x.ClaveEmpleado, Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, x.Rfc }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}