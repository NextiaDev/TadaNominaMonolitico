using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class UsuariosController : BaseController
    {
        // GET: Usuarios
        /// <summary>
        /// Acción que muestra los usuarios.
        /// </summary>
        /// <returns>Regresa la vista con los usuarios.</returns>
        public ActionResult Index()
        {
            if (Session["sTipoUsuario"].ToString() != "System") { return RedirectToAction("Index", "Index"); }
            ClassUsuarios cuser = new ClassUsuarios();
            List<ModelUsuarios> user = cuser.GetModelUsuarios().OrderBy(x =>  x.IdUsuario ).ToList();

            return View(user);
        }

        // GET: Usuarios/Details/5
        /// <summary>
        /// Acción que muestra el detalle del un usuario.
        /// </summary>
        /// <param name="id">Recibe el identificador del usuario.</param>
        /// <returns>Regresa la vista con el detalle del usuario.</returns>
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Usuarios/Create
        /// <summary>
        /// Acción que genera un usuario nuevo.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de usuarios.</returns>
        public ActionResult Create()
        {
            //ViewBag.Ltipo = llenaTipoUsuario();
            return View();
        }

        /// <summary>
        /// Acción que muestra los valores del combo box del tipo de usuario.
        /// </summary>
        /// <returns>Regresa la vista del combo box del tipo de usuario.</returns>
        public List<SelectListItem> llenaTipoUsuario()
        {
            List<SelectListItem> tipoUser = new List<SelectListItem>();
            tipoUser.Add(new SelectListItem { Text = "Elegir...", Value = "0" });
            tipoUser.Add(new SelectListItem { Text = "Usuario", Value = "Usuario" });
            tipoUser.Add(new SelectListItem { Text = "System", Value = "System" });

            return tipoUser;
        }

        // POST: Usuarios/Create
        /// <summary>
        /// Acción que guarda al usuario nuevo.
        /// </summary>
        /// <param name="u">Recibe el modelo de usuarios.</param>
        /// <returns>Regresa la vista de usuarios.</returns>
        [HttpPost]
        public ActionResult Create(ModelUsuarios u)
        {
            try
            {
                // TODO: Add insert logic here
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassUsuarios cu = new ClassUsuarios();
                var user = cu.GetUsuarios();

                if (user.Select(x => x.Correo).Contains(u.Correo.Trim()))
                {
                    throw new Exception("El correo " + u.Correo + " ya esta registrado.");
                }
                else if (user.Select(x => x.Usuario).Contains(u.Usuario.Trim().ToUpper()))
                {
                    throw new Exception("El usuario " + u.Usuario + " ya esta registrado");
                }
                else
                {
                    cu.newUsuario(u, IdUsuario);
                    u.Validacion = true;
                    u.Mensaje = "El usuario se creo correctamente";
                }

                return View(u);
            }
            catch (Exception ex)
            {
                u.Validacion = false;
                u.Mensaje = "Error al crear usuario: " + ex.Message;
                return View(u);
            }
        }

        // GET: Usuarios/Edit/5
        /// <summary>
        /// Acción que modifica a un usuario.
        /// </summary>
        /// <param name="id">Recibe el identificador del usuario.</param>
        /// <returns>Regresa la vista del modelo de usuarios.</returns>
        public ActionResult Edit(int id)
        {
            ClassUsuarios cu = new ClassUsuarios();
            var model = cu.GetModelUsuarios(id);
            return View(model);
        }

        // POST: Usuarios/Edit/5
        /// <summary>
        /// Acción que guarda la modificación del usuario.
        /// </summary>
        /// <param name="id">Recibe el identificador del usuario.</param>
        /// <param name="u">Recibe el modelo de usuarios.</param>
        /// <returns>Regresa la vista de usuarios.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelUsuarios u)
        {
            try
            {
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassUsuarios cu = new ClassUsuarios();
                var user = cu.GetUsuarios().Where(x => x.IdUsuario != id);

                if (user.Select(x => x.Correo).Contains(u.Correo.Trim()))
                {
                    throw new Exception("El correo " + u.Correo + " ya esta registrado.");
                }
                else if (user.Select(x => x.Usuario).Contains(u.Usuario.Trim().ToUpper()))
                {
                    throw new Exception("El usuario " + u.Usuario + " ya esta registrado");
                }
                else
                {
                    cu.EditeUser(id, u, IdUsuario);
                    u.Validacion = true;
                    u.Mensaje = "El usuario se modifico correctamente";
                }

                return View(u);
            }
            catch (Exception ex)
            {
                u.Validacion = false;
                u.Mensaje = "Error al modificar usuario: " + ex.Message;
                return View(u);
            }
        }

        // GET: Usuarios/Delete/5
        /// <summary>
        /// Acción que elimina a un usuario.
        /// </summary>
        /// <param name="id">Recibe el identificador del usuario.</param>
        /// <returns>gresa la vista del modelo de usuarios.</returns>
        public ActionResult Delete(int id)
        {
            ClassUsuarios cu = new ClassUsuarios();
            var model = cu.GetModelUsuarios(id);
            return View(model);
        }

        // POST: Usuarios/Delete/5
        /// <summary>
        /// Acción que confirma la eliminación del usuario.
        /// </summary>
        /// <param name="id">Recibe el identificador del usuario.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista de los usuarios.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassUsuarios cu = new ClassUsuarios();
                cu.DeleteUser(id, IdUsuario);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que permite modificar la contraseña.
        /// </summary>
        /// <returns>Regresa la vista del modelo de cambio de contraseña.</returns>
        public ActionResult CambiaPass()
        {
            int IdUsuario = (int)Session["sIdUsuario"];
            ClassUsuarios cusuario = new ClassUsuarios();
            ModelCambioPass model = cusuario.GetModelCambio(IdUsuario);

            return View(model);
        }

        /// <summary>
        /// Acción que guarda el cambio de contraseña.
        /// </summary>
        /// <param name="collecction">Recibe el modelo de cambio de contraseña.</param>
        /// <returns>Regresa la vista con el modelo de cambio de contraseña.</returns>
        [HttpPost]
        public ActionResult CambiaPass(ModelCambioPass collecction)
        {
            ModelCambioPass model = new ModelCambioPass();
            try
            {
                ClassUsuarios cusuario = new ClassUsuarios();
                model = cusuario.CambiaPass(collecction);

                return View(model);
            }
            catch (Exception ex)
            {
                model.validacion = false;
                model.Mensaje = "Error! " + ex.Message;
                return View(model);
            }
        }

        /// <summary>
        /// Acción que muestra los perfiles de usuario.
        /// </summary>
        /// <param name="id">Recibe el identificador del perfil.</param>
        /// <returns>Regresa la vista del perfil del usuario.</returns>
        public ActionResult Perfiles(int id)
        {
            string token = Session["sToken"].ToString();
            ClassUsuarios cu = new ClassUsuarios();
            var user = cu.getPerfil(id, token);

            return View(user);
        }

        /// <summary>
        /// Acción que muestra las unidades de negocio.
        /// </summary>
        /// <param name="id">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa un Json con la información.</returns>
        [HttpPost]
        public JsonResult getUnidades(int id)
        {
            ClassUnidadesNegocio cu = new ClassUnidadesNegocio();           
            var unidades = cu.getUnidadesnegocio(id);

            return Json(unidades, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Acción que cambio la imagen del perfil de usuario.
        /// </summary>
        /// <returns>Regresa la vista principal.</returns>
        public ActionResult cambiaImgenPerfil()
        {
            return View();
        }

        /// <summary>
        /// Acción que permite cambiar la imagen del perfil.
        /// </summary>
        /// <param name="imagen">Actúa como clase base para las clases que proporcionan acceso a los archivos individuales que ha cargado un cliente.</param>
        /// <returns>Regresa la vista principal.</returns>
        [HttpPost]
        public ActionResult cambiaImgenPerfil(HttpPostedFileBase imagen)
        {
            return View();
        }

        public ActionResult Perfil()
        {
            ClassUsuarios cu = new ClassUsuarios();
            var datos = cu.GetModelUsuarios((int)Session["sIdUsuario"]);
            return View(datos);
        }

        public JsonResult UploadFoto()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    ClassUsuarios cu = new ClassUsuarios();

                    var files = Request.Files;

                    foreach (string str in files)
                    {
                        HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                        var ext = Path.GetExtension(file.FileName);
                        string[] extensions = { ".jpg", ".JPG", ".jpeg", ".JPEG" };

                        if (extensions.Contains(ext))
                        {
                            Image img = Image.FromStream(file.InputStream, true, true);

                            Session["sFoto"] = cu.GuardaFoto(img, (int)Session["sIdUsuario"]);
                        }
                        else
                            throw new Exception("El archivo que cargo no tiene la extension correcta.");
                    }

                    return Json(new { result = true });
                }
                else                 
                    throw new Exception("No se cargo ningun archivo.");
                
            }
            catch (Exception ex)
            {
                return Json(new { result = false, mensaje = ex.Message });
            }
        }

        public JsonResult quitarFoto()
        {
            try
            {
                ClassUsuarios cu = new ClassUsuarios();
                cu.deleteImage((int)Session["sIdUsuario"]);
                Session["sFoto"] = "";
                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, mensaje = ex.Message });
            }
        }
    }
}
