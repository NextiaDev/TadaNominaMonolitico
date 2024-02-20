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
        public ActionResult Index(MResultCRUD inforequest)
        {
            string token = Session["sToken"].ToString();
            var model = new MIndexUsuario();
            CConfigUsuario cconfiguser = new CConfigUsuario();
            model.Usuarios = cconfiguser.GetUsuariosActive(token);
            model.ClientesToAsign = cconfiguser.GetCklientesToSelect(token);
            model.UnidadNegocioToAsign = cconfiguser.GetUnidadesNegocioToAsign(token);
            model.Result = string.IsNullOrEmpty(inforequest.Result) ? null : inforequest.Result;
            model.MensajeResult = string.IsNullOrEmpty(inforequest.Mensaje) ? null : inforequest.Mensaje;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUsuario(MaddUsuario request)
        {
            var result = new MResultCRUD();
            if (request.IdUsuario != 0)
            {
                result = EditUsuario(request);
            }
            else
            {
                string token = Session["sToken"].ToString();
                CConfigUsuario configuser = new CConfigUsuario();
                result = configuser.AddUsuario(token, request);
            }
            return RedirectToAction("Index", result);
        }

        private MResultCRUD EditUsuario(MaddUsuario request)
        {
            string token = Session["sToken"].ToString();
            CConfigUsuario configuser = new CConfigUsuario();
            var result = configuser.EditUsuario(token,request);
            return result;
        }

        [HttpPost]
        public ActionResult DeleteUsuario(int IdUsuario)
        {
            string token = Session["sToken"].ToString();
            CConfigUsuario cuser = new CConfigUsuario();
            var result = cuser.DeleteUsuario(token, IdUsuario);
            return RedirectToAction("Index", result);
        }

    }
}