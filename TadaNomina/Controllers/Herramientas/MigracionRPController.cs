using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Herramientas;
using TadaNomina.Models.ViewModels.Herramientas;

namespace TadaNomina.Controllers.Herramientas
{
    public class MigracionRPController : BaseController
    {
        // GET: MigracionRP
        public ActionResult Index()
        {
            cMigracionRP cMigracion = new cMigracionRP();
            return View(cMigracion.GetMigracionRP(int.Parse(Session["sIdCliente"].ToString())));
        }

        [HttpPost]
        public ActionResult Index(mMigracionRP m)
        {
            try
            {
                cMigracionRP cMigracion = new cMigracionRP();
                DateTime _FechaBaja = DateTime.Parse(m.fBaja);
                DateTime _FechaAlta = DateTime.Parse(m.fAlta);

                if (m.Claves == null || m.Claves == string.Empty) { throw new Exception("No se ingreso ninguna clave."); }
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassEmpleado ce = new ClassEmpleado();
                var _claves = m.Claves.Replace(" ", "").Split(',').ToArray();
                var empleados = ce.GetEmpleadosActivosByUnidadNegocioYClave(_claves, int.Parse(Session["sIdUnidadNegocio"].ToString()));
                cMigracion.MigraEmpleadosRP(empleados, m.IdNuevoRegistroPatronal, _FechaBaja, _FechaAlta, m.ConservaAntiguedad, IdUsuario);

                m=cMigracion.GetMigracionRP(int.Parse(Session["sIdCliente"].ToString()));
                m.Validacion = true;
                m.Mensaje = "Se migraron correctamente los empleados";
            }
            catch (Exception ex)
            {
                cMigracionRP cMigracion = new cMigracionRP();
                m=cMigracion.GetMigracionRP(int.Parse(Session["sIdCliente"].ToString()));
                m.Validacion=false;
                m.Mensaje= ex.Message;
                return View(m);
            }

            return View(m);
        }

        public JsonResult getEmpleadosMigrar(string Claves)
        {
            int IdUnidadActual = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado ce = new ClassEmpleado();
            var _claves = Claves.Replace(" ", "").Split(',').ToArray();
            var model = ce.GetEmpleadosByClave(_claves, IdUnidadActual).Select(x => new { Clave = x.ClaveEmpleado, Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre, x.Rfc }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}