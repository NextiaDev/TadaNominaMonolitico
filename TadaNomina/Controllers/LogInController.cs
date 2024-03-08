using System.Web.Mvc;
using TadaNomina.Models.ViewModels;
using TadaNomina.Services;
using System.Linq;
using System;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers
{
    public class LogInController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var accion = filterContext.ActionDescriptor.ActionName;
            var method = filterContext.HttpContext.Request.HttpMethod;
            var IP = filterContext.HttpContext.Request.UserHostAddress;
            string mensaje;
            mensaje = ViewBag.mensaje_ ?? "N/A";

            generaLog("Normal", controlador, accion, method, mensaje, IP);

            base.OnActionExecuted(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = filterContext.Exception.Message;
            var controlador = filterContext.RouteData.Values["controller"] as string;
            var accion = filterContext.RouteData.Values["action"] as string;
            var method = filterContext.HttpContext.Request.HttpMethod;
            var IP = filterContext.HttpContext.Request.UserHostAddress;

            generaLog("Error", controlador, accion, method, error, IP);

            base.OnException(filterContext);
        }

        private void generaLog(string estatus, string controlador, string accion, string method, string error, string ip)
        {
            string path = DateTime.Now.ToLongDateString();
            cLog oLog = new cLog();
            var cliente = Session["sCliente"] != null ? Session["sCliente"].ToString() : "N/A";
            var nomina = Session["sNomina"] != null ? Session["sNomina"].ToString() : "N/A";

            oLog.Add("Estatus:" + estatus + " | Usuario: ID:" + Session["sIdUsuario"] + " Nombre:" + Session["sNombre"] + " | Cliente: " + cliente
                + " - Nomina: " + nomina + " | Ruta: " + controlador + "/" + accion + " - " + method + " | IP: " + ip + " | Mensaje: " + error, path);
        }
             
        public ActionResult Index(string Usuario, string contraseña)
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Index(ModelLogin login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var clog = new sAcceso();
                    ModelUser model = null;
                    try { model = clog.sGetAcceso(login); } catch (Exception ex) { throw new Exception(ex.Message); }
                    if (model != null)
                    {
                        string[] user = { "System", "Usuario", "Administrador" };
                        if (user.Contains(model.User.Tipo))
                        {
                            if (model.Modulo.Contains("NOMINA") )
                            {
                                CreaVariablesSession(model);
                                return RedirectToAction("Index", "Default");
                            }
                            else
                                throw new Exception("Tipo de usuario no valido.");
                        }
                        else
                            throw new Exception("Tipo de usuario no valido.");
                    }
                    else
                        throw new Exception("No se encontro infomracion para este usuario.");
                }
                else
                    throw new Exception("Faltan datos por capturar.");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje_ = "Error de acceso: " + ex.Message + " datos(" + login.username + "," + login.password + ")";
                ViewBag.Mensaje = "Datos no validos: " + ex.Message;                
            }

            return View();
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
            Session["sRelojChecador"] = user.User.relojChecador;
            Session["sFoto"] = user.User.Foto;
        }
    }
}
