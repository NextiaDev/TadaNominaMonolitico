using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels;
using TadaNomina.Services;

namespace TadaNomina.Controllers
{
    public class CentralController : Controller
    {        
        public ActionResult Index(string Stoken)
        {
            var clog = new sAcceso();
            var _datos = Base64Decode(Stoken);
            var datos = _datos.Split('|').ToArray();
            var usuario = datos[0];
            var pass = datos[1];
            ModelLogin login = new ModelLogin();
            login.username = usuario.Trim();
            login.password = pass.Trim();
            ModelUser model = null;
            try { model = clog.sGetAcceso(login); } catch { }
            if (model != null)
            {
                string[] user = { "System", "Usuario" };
                if (user.Contains(model.User.Tipo))
                {
                    CreaVariablesSession(model);
                    return RedirectToAction("Index", "Default");
                }
            }
            ViewBag.Mensaje = "Datos no validos";
            return RedirectToAction("Index", "Default");
        }


        public void CreaVariablesSession(ModelUser user)
        {
            Session["sIdUsuario"] = user.User.Id;
            Session["sLogin"] = user.User.Username;
            Session["sNombre"] = user.User.Name;
            Session["sCorreo"] = user.User.Correo;
            Session["sIdClientes"] = user.User.IdCliente;
            Session["sIdUnidades"] = user.User.IdUnidades;
            Session["sTipoUsuario"] = user.User.Tipo;
            Session["sToken"] = user.Token;
            Session["sFoto"] = user.User.Foto;
            Session["sRelojChecador"] = user.User.relojChecador;
        }

        public static string Base64Decode(string Stoken)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(Stoken);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}