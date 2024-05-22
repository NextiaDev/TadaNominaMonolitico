using Antlr.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TadaNomina.Models.ClassCore;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TadaNomina.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["sNombre"] == null)
            {
                var IP = filterContext.HttpContext.Request.UserHostAddress;
                generaLogSinDatosSession("Error", "Caduco la sesión", IP);
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Index"
                }));
            }

            try { ViewBag.NombreUsuario = Session["sNombre"].ToString(); } catch  { }
            try { ViewBag.CorreoUsuario = Session["sCorreo"].ToString(); } catch  { }
            try { ViewBag.NominaSeleccionada = Session["sNomina"].ToString(); } catch { ViewBag.NominaSeleccionada = "Ninguna"; }
            try { ViewBag.ClienteSeleccionado = Session["sCliente"].ToString(); } catch { ViewBag.ClienteSeleccionado = "Ninguno"; }
            try { ViewBag.IdNomia = Session["sIdUnidadNegocio"].ToString(); } catch { ViewBag.IdNomina = null; }
            try { ViewBag.AñoActual = DateTime.Now.Year; } catch { ViewBag.AñoActual = null; }
            try { ViewBag.APIConectada = ConfigurationManager.AppSettings.Get("API_Connection"); } catch { ViewBag.APIConectada = "N/A"; }
            try { ViewBag.APIConectadaConta = ConfigurationManager.AppSettings.Get("API_ConnectionCont"); } catch { ViewBag.APIConectadaConta = "N/A"; }

            var connectionString = ConfigurationManager.ConnectionStrings["ModelNomina"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connectionString);

            var connectionStringC = ConfigurationManager.ConnectionStrings["ModelContabilidad"].ConnectionString;
            var builderC = new SqlConnectionStringBuilder(connectionStringC);

            string database = builder.InitialCatalog;
            string databaseC = builderC.InitialCatalog;
            try { ViewBag.Base = database; } catch { ViewBag.Base = "N/A"; }
            try { ViewBag.BaseC = databaseC; } catch { ViewBag.BaseC = "N/A"; }

            base.OnActionExecuting(filterContext);
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var accion = filterContext.ActionDescriptor.ActionName;
            var method = filterContext.HttpContext.Request.HttpMethod;
            var IP = filterContext.HttpContext.Request.UserHostAddress;
            var model = filterContext.Controller.ViewData;
            var viewbag = filterContext.Controller.ViewBag;
            
            string mensaje = string.Empty;
            try { mensaje += model.GetViewDataInfo("Mensaje").Value; } catch { }
            try { mensaje += viewbag.GetViewDataInfo("Mensaje").Value; } catch { }
            
            if (Session["sLogin"] != null)
                generaLog("Normal", controlador, accion, method, mensaje, IP);
            else
                generaLogSinDatosSession("Error", "No se encontraron datos de sesión.", IP);

            base.OnActionExecuted(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = filterContext.Exception.Message;
            var controlador = filterContext.RouteData.Values["controller"] as string;
            var accion = filterContext.RouteData.Values["action"] as string;
            var method = filterContext.HttpContext.Request.HttpMethod;
            var IP = filterContext.HttpContext.Request.UserHostAddress;
            
            if(Session["sLogin"] != null)
                generaLog("Error", controlador, accion, method, error, IP);
            else
                generaLogSinDatosSession("Error", error, IP);

            base.OnException(filterContext);
        }

        private void generaLog(string estatus, string controlador, string accion, string method, string error, string ip)
        {   
            string path = Session["sLogin"].ToString() + ".txt";
            cLog oLog = new cLog();            
            var cliente = Session["sCliente"] != null ? Session["sCliente"].ToString() : "N/A";
            var nomina = Session["sNomina"] != null ? Session["sNomina"].ToString() : "N/A";

            oLog.Add("Estatus:" + estatus + " | Usuario: " + Session["sLogin"] + " ID:" + Session["sIdUsuario"] + " Nombre:" + Session["sNombre"] + " | Cliente: " + cliente 
                + " - Nomina: " + nomina + " | Ruta: " + controlador + "/" + accion + " - " + method + " | IP: " + ip + " | Mensaje: " + error, path);
        }

        private void generaLogSinDatosSession(string estatus, string error, string ip)
        {
            string path = "sinSession.txt";
            cLog oLog = new cLog();           
            oLog.Add("Estatus:" + estatus + " | Usuario:  ID:  Nombre: | Cliente: - Nomina:  | Ruta:  - | IP: " + ip + " | Mensaje: " + error, path);
        }

        public JsonResult ValidaSession()
        {
            int idusuario = 0;
            try { idusuario = (int)Session["sIdUsuario"]; }
            catch {  }

            if (idusuario != 0)
                return Json("Ok");
            else
            {                
                return Json("Error");
            }
        }
    }
}
