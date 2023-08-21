using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    /// <summary>
    /// Controlador para el control de los ausentismos.
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 23/05/2022, Razón: documentar el codigo
    /// </summary>
    public class AusenController : BaseController
    {

        /// <summary>
        /// Acción que obtiene el listado de los ausentimos capturados y vigentes.
        /// </summary>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult Index()
        {
            int idunidadnegocio = 0;
            try { idunidadnegocio = (int)Session["sidunidadnegocio"]; } catch { idunidadnegocio = 0; }
            if (idunidadnegocio == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                Ausen clsAusentismo = new Ausen();
                return View(clsAusentismo.getAusentismoUnidadn(idunidadnegocio));
            }
        }

        /// <summary>
        /// Acción para obtener el detalle de un ausentismo en base a su identificador.
        /// </summary>
        /// <param name="id">Identificador del ausentismo.</param>
        /// <returns>Resultados del ausentismo a la vista.</returns>
        public ActionResult Details(int id)
        {
            Ausen clsAusen = new Ausen();
            return PartialView(clsAusen.GetModelAusenB(id));
        }



        public ActionResult Editar(int id)
        {
            Ausen clas = new Ausen();
            string nuevo = string.Empty;
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

                var ause = clas.GetIsAusentismose(id);
                int idEmpleado = ause.IdEmpleado.Value;
                ModelAusentismos model = clas.FindListAusen(IdCliente);

                model = clas.GetausenById(model, idEmpleado, IdUnidadNegocio);
                if (idEmpleado > 0)
                {

                    return View(model);
                }
                else
                {
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

        /// <summary>
        /// Acción para crear un ausentismo.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
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

        /// <summary>
        /// Acción que obtiene la información del empleado.
        /// </summary>
        /// <returns>Resultados a la vista.</returns>
        [HttpPost]
        public JsonResult GetEmpleados()
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosAusen(IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Acción que obtiene la información del empleado mediante su clave.
        /// </summary>
        /// <param name="clave">Clave del empleado.</param>
        /// <returns>Resultados a la vista.</returns>
        [HttpPost]
        public JsonResult GetEmpleadosByClave(string clave)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosByClaveAusen(clave, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Acción que obtiene la información del empleado mediante el nombre del mismo.
        /// </summary>
        /// <param name="name">Nombre del empleado.</param>
        /// <returns>Resultados a la vista.</returns>
        [HttpPost]
        public JsonResult GetEmpleadosByNombre(string name)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosByNombreAusen(name, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Acción para agregar un nuevo ausentismo.
        /// </summary>
        /// <param name="data">Datos que se cargan a la vista para el usuario del sistema.</param>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult AddAusen(string data)

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
                Ausen clas = new Ausen();
                ModelAusentismos model = clas.FindListAusen(IdCliente);

                model = clas.GetausenById(model, idEmpleado, IdUnidadNegocio);
                if (idEmpleado > 0)
                {

                    return View(model);
                }
                else
                {
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

        /// <summary>
        /// Acción para agregar un nuevo ausentismo.
        /// </summary>
        /// <param name="data">Datos proporcionados por el usuario del sistema.</param>
        /// <returns>Resultados del proceso a la vista.</returns>
        [HttpPost]
        public ActionResult AddAusen(ModelAusentismos model)
        {

            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            int idcliente = 0;
            string img = string.Empty;
            Ausen clas = new Ausen();
            ModelAusentismos ausen = clas.FindListAusen(idcliente);
            ClassConceptosFiniquitos cfiniquitos = new ClassConceptosFiniquitos();
            model.Incapacidad = ausen.Incapacidad;
            model.LAusentismos = ausen.LAusentismos;
            model.Subsecuente = ausen.Subsecuente;
            foreach (string filename in Request.Files)
            {
                try
                {
                    IdUsuario = (int)Session["sIdUsuario"];
                    IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                    idcliente = (int)Session["sIdCliente"];
                }
                catch
                {
                    return RedirectToAction("Index", "Login");
                }

                //Si es Subsecuente
                if (model.IdSubsecuente == "Si")
                {
                    var result = clas.getfolioR(model.FolioSub);
                    model.unidadnegocio = IdUnidadNegocio;
                    HttpPostedFileBase file = Request.Files[filename];
                    string ruta = @"D:\TadaNomina\Ausentismo\";
                    img = file.FileName;


                    if (result != null)
                    {
                        ausen.Validacion = true;
                        ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismos', 'Este folio no es subsecuente!!', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");
                    }

                    if (file != null && file.ContentLength > 0)
                    {
                        if (!Directory.Exists(ruta))
                        {
                            Directory.CreateDirectory(ruta);
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;
                            clas.AddAusenCons(model, IdUsuario);
                            ausen.Validacion = true;
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");

                        }
                        else
                        {
                            if (file != null)
                            {
                                file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                                model.Imagen = img;
                            }
                            clas.AddAusenCons(model, IdUsuario);

                            ausen.Validacion = true;
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                        }
                    }

                    else
                    {

                        model.Imagen = img;
                        if (model.idAusentismos == null || model.IdIncapacidad == null)
                        {
                            try
                            {

                                clas.AddAusen(model, IdUsuario);
                                ausen.Validacion = true;
                                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                                return RedirectToAction("Index", "Ausen");
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }
                }

                else
                {

                    //Obtiene folio
                    var result = clas.getfolioR(model.Folio);
                    model.unidadnegocio = IdUnidadNegocio;
                    model.idCliente = idcliente;
                    HttpPostedFileBase file = Request.Files[filename];
                    string ruta = @"D:\TadaNomina\Ausentismo\";
                    img = file.FileName;
                    //if (result == "Si")
                    //{
                    //    ModelAusentismos models = clas.FindListAusen(idcliente);

                    //    ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismos', 'Ese folio ya contiene un registro!!', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");
                    //    return View(models);
                    //}


                    if (file != null && file.ContentLength > 0)
                    {
                        if (!Directory.Exists(ruta))
                        {
                            Directory.CreateDirectory(ruta);
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;

                            if (model.idAusentismos == null || model.IdIncapacidad == null)
                            {
                                try
                                {

                                    clas.AddAusen(model, IdUsuario);
                                    ausen.Validacion = true;
                                    ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                                    return RedirectToAction("Index", "Ausen");

                                }
                                catch (Exception)
                                {

                                    throw;
                                }

                            }



                        }
                        else
                        {

                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;
                            if (model.idAusentismos == null || model.IdIncapacidad == null)
                            {
                                try
                                {

                                    clas.AddAusen(model, IdUsuario);
                                    ausen.Validacion = true;
                                    ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                                    return RedirectToAction("Index", "Ausen");

                                }
                                catch (Exception)
                                {

                                    throw;
                                }

                            }
                        }
                    }

                    else
                    {

                        model.Imagen = img;
                        if (model.idAusentismos == null || model.IdIncapacidad == null)
                        {
                            try
                            {

                                clas.AddAusen(model, IdUsuario);
                                ausen.Validacion = true;
                                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                                return RedirectToAction("Index", "Ausen");
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }


                }

            }
            return View(model);
        }

        /// <summary>
        /// Accion para codificar URL  
        /// </summary>
        /// <param name="param">Url sin condificar.</param>
        /// <returns>Url codificada.</returns>
        [HttpPost]

        public string EncodeParam(string param)
        {
            byte[] array = Encoding.ASCII.GetBytes(param);
            return Server.UrlTokenEncode(array);
        }

        /// <summary>
        /// Acción para decodificar la Url.
        /// </summary>
        /// <param name="param">Url codificada.</param>
        /// <returns>Url decodificada.</returns>
        private string DecodeParam(string param)
        {
            byte[] array = Server.UrlTokenDecode(param);
            return Encoding.UTF8.GetString(array);
        }


        /// <summary>
        /// Acción para cargar los datos necesarios para editar un ausentismo.
        /// </summary>
        /// <param name="id">Identificador del ausentismo.</param>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult Edit(int id)
        {

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];


            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }


            string iausentismo = string.Empty;
            int idEmpleado = 0;
            Ausen clas = new Ausen();
            ModelAusentismos nuevo = new ModelAusentismos();
            nuevo = clas.FindListAusen(IdCliente);
            nuevo = clas.GeteditAusentismossById(nuevo, id);
            idEmpleado = nuevo.IdEmpleado.Value;

            nuevo = clas.GetausenById(nuevo, idEmpleado, IdUnidadNegocio);





            return View(nuevo);
        }

        /// <summary>
        /// Acción resultante de la edicion de un ausentismo.
        /// </summary>
        /// <param name="id">Identificador del ausentismo.</param>
        /// <param name="model">Datos necesarios para la edicion del ausentismo.</param>
        /// <returns>Resultados de la operación a la vista.</returns>
        [HttpPost]
        public ActionResult Edit(string id, ModelAusentismos model)
        {
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            string img = string.Empty;
            int IdCliente = (int)Session["sIdCliente"];


            Ausen clas = new Ausen();
            ModelAusentismos ausen = clas.FindListAusen(IdCliente);
            model.Incapacidad = ausen.Incapacidad;
            model.Subsecuente = ausen.Subsecuente;
            foreach (string filename in Request.Files)
            {
                try
                {
                    IdUsuario = (int)Session["sIdUsuario"];
                    IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                }
                catch
                {
                    return RedirectToAction("Index", "Login");
                }


                if (model.Imagen == null)
                {
                    ausen.Validacion = true;
                    ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismos', 'Registre una imagen!!', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");
                }


                else if (model.IdSubsecuente == "Si")
                {
                    var result = clas.getfolioR(model.FolioSub);

                    model.unidadnegocio = IdUnidadNegocio;
                    HttpPostedFileBase file = Request.Files[filename];
                    string ruta = @"D:\TadaNomina\Ausentismo\";
                    img = file.FileName;

                    if (result != null)
                    {
                        ausen.Validacion = true;
                        ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismos', 'Este folio no es subsecuente!!', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");
                    }
                    else if (file != null && file.ContentLength > 0)
                    {
                        if (!Directory.Exists(ruta))
                        {
                            Directory.CreateDirectory(ruta);
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;
                            clas.AddAusenedit(model, id);

                            ausen.Validacion = true;
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");

                        }
                        else
                        {

                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;
                            clas.AddAusenCons(model, IdUsuario);

                            ausen.Validacion = true;
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                        }
                    }

                }
                else
                {

                    model.unidadnegocio = IdUnidadNegocio;
                    HttpPostedFileBase file = Request.Files[filename];
                    string ruta = @"D:\TadaNomina\Ausentismo\";
                    img = file.FileName;

                    if (file != null && file.ContentLength > 0)
                    {
                        if (!Directory.Exists(ruta))
                        {
                            Directory.CreateDirectory(ruta);
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;
                            clas.AddAusen(model, IdUsuario);

                            ausen.Validacion = true;
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                        }
                        else
                        {
                            file.SaveAs(ruta + "/" + Path.GetFileName(file.FileName));
                            model.Imagen = img;
                            clas.AddAusenedit(model, id);

                            ausen.Validacion = true;
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Ausentismo', 'Se registro Correctamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                        }
                    }
                }

            }
            return View(model);

        }

        /// <summary>
        /// Acción que obtiene la información necesaria para borrar un ausentismo.
        /// </summary>
        /// <param name="id">Identificador del ausentismo.</param>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult Delete(int id)
        {
            Ausen clsAusen = new Ausen();
            return PartialView(clsAusen.GetModelAusenB(id));
        }

        /// <summary>
        /// Accion para borrar un ausentismo.
        /// </summary>
        /// <param name="id">Identificador del ausentismo.</param>
        /// <param name="collection">Datos necesarios para eliminar el ausentismo.</param>
        /// <returns>Resusltados de la operación a la vista.</returns>
        [HttpPost]
        public JsonResult DeleteAusen(int idAusentimos)
        {
            try
            {
                Ausen clsCC = new Ausen();
                int idUsuario = (int)Session["sIdUsuario"];
                var buscar = clsCC.BuscarIdIncidenciasC(idAusentimos);
                if (buscar == "S")
                {
                    clsCC.DeleteAusentism(idAusentimos, idUsuario);
                    clsCC.EliminaIncidenciasNominaAbierta(idAusentimos);
                    return Json("OK");


                }
                else
                {
                    clsCC.DeleteAusentism(idAusentimos, idUsuario);
                    clsCC.EliminaIncidenciasNominaAbierta(idAusentimos);
                    return Json("Delete");

                }


            }
            catch
            {
                return Json("Delete");
            }
        }
    }
}