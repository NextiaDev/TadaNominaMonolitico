using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.ConfigUsuarios.cs;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.ConfigUsuario;

namespace TadaNomina.Controllers.Administracion
{
    public class ConfigUsuarioController : BaseController
    {
        // GET: ConfigUsuario
        public ActionResult Index()
        {
            string token = Session["sToken"].ToString();
            var model = new MIndexUsuario();
            CConfigUsuario cconfiguser = new CConfigUsuario();
            model.Usuarios = cconfiguser.GetUsuariosActive(token);
            model.ClientesToAsign = cconfiguser.GetCklientesToSelect(token);
            model.UnidadNegocioToAsign = cconfiguser.GetUnidadesNegocioToAsign(token);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUsuario(MaddUsuario request)
        {

        }

        //public JsonResult GetUnidadesToAsign(string clientes)
        //{
        //    if (string.IsNullOrEmpty(clientes)) return Json(new List<Cat_UnidadNegocio>(), JsonRequestBehavior.AllowGet);
        //    string token = Session["sToken"].ToString();
        //    CConfigUsuario configuser = new CConfigUsuario();
        //    var result = configuser.GetUnidadesNegocioToAsign(token, clientes);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}