using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.IMSS;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class RecuperacionesIMSSController : BaseController
    {
        public ActionResult Search()
        {
            try
            {
                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }

        }

        // Accion para obtener empleados general
        [HttpPost]
        public JsonResult GetEmpleados()
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassRecuperacionesIMSS empleados = new ClassRecuperacionesIMSS();
                return Json(new { data = empleados.GetEmpleados(IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }

        // Accion para obtener empleados por nombre
        [HttpPost]
        public JsonResult GetEmpleadosByNombre(string name)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassRecuperacionesIMSS empleados = new ClassRecuperacionesIMSS();
                return Json(new { data = empleados.GetEmpleadosByNombre(name, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        // Accion para obtener empleados por clave
        [HttpPost]
        public JsonResult GetEmpleadosByClave(string clave)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassRecuperacionesIMSS empleados = new ClassRecuperacionesIMSS();
                return Json(new { data = empleados.GetEmpleadosByClave(clave, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }

        public ActionResult Edit(string data)

        {

            string nuevo = string.Empty;

            nuevo = data;

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                Session["data"] = nuevo;

            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {
                int idEmpleado = Convert.ToInt32(DecodeParam(data));

                if (idEmpleado > 0)
                {                    
                    ClassRecuperacionesIMSS recuperacion = new ClassRecuperacionesIMSS();
                    ModelRecuperaciones m = recuperacion.GetModelRecuperaciones(idEmpleado, IdUnidadNegocio);
                    return View(m);
                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "1 ¡Modificacion Errónea!";
                    ViewBag.message = "No Fue posible encontrar al empleado solicitado.";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "2 ¡Modificacion Errónea!";
                ViewBag.message = "No Fue posible encontrar al empleado solictado." + ex.Message;
                return View("Response");
            }
        }
        
        [HttpPost]
        public ActionResult Edit(ModelRecuperaciones m)
        {
            int IdUsuario;
            int IdUnidadnegocio;
            ModelRecuperaciones model = new ModelRecuperaciones();
            try { IdUsuario = (int)Session["sIdUsuario"]; IdUnidadnegocio = (int)Session["sIdUnidadNegocio"]; } catch { return RedirectToAction("Index", "Login"); }

            try
            {
                if (ModelState.IsValid)
                {
                    m.Idusuario = IdUsuario;
                    ClassRecuperacionesIMSS recuperacion = new ClassRecuperacionesIMSS();                    

                    if (recuperacion.NuevaRecuperacion(m))
                    {
                        model = recuperacion.GetModelRecuperaciones(m.IdEmpleado, IdUnidadnegocio);
                        ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Recuperaciones IMSS', 'Se guardo la recuperación exitosamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                    }
                    else
                    {
                        model = recuperacion.GetModelRecuperaciones(m.IdEmpleado, IdUnidadnegocio);
                        ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Recuperaciones IMSS', 'No se guardo la informacion', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");
                    }

                    return View(model);
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Accion para codificar URL        
        public string EncodeParam(string param)
        {
            byte[] array = Encoding.ASCII.GetBytes(param);
            return Server.UrlTokenEncode(array);
        }      

        // Accion para decoficar url
        private string DecodeParam(string param)
        {
            byte[] array = Server.UrlTokenDecode(param);
            return Encoding.UTF8.GetString(array);
        }


    }
}
