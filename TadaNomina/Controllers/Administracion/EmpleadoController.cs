using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels;
using System.IO;
using TadaNomina.Models;
using System.Data;
using System.Web.Script.Serialization;
using TadaNomina.Models.DB;
using LumenWorks.Framework.IO.Csv;
using TadaNomina.Models.ClassCore.RelojChecador;
using TadaNomina.Models.ViewModels.RelojChecador;

namespace TadaNomina.Controllers.Administracion
{
    public class EmpleadoController : BaseController
    {
        // GET: Empleado
        /// <summary>
        /// Acción que genera la vista para registrar un empleado.
        /// </summary>
        /// <returns>Regresa la vista con el modelo para hacer el registro del empleado.</returns>
        public ActionResult RegistraEmpleado()
        {
            try
            {
                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado initEmpleado = new ClassEmpleado();
                Empleado empleado = initEmpleado.Init(IdCliente, IdUnidadNegocio);
                return View(empleado);
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Acción que genera la vista de la búsqueda del empleado.
        /// </summary>
        /// <returns>Regresa la vista de la búsqueda del empleado.</returns>
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

        /// <summary>
        /// Acción que modifica al empleado.
        /// </summary>
        /// <param name="data">Recibe la variable tipo string.</param>
        /// <returns>Regresa la vista de modificación del empleado.</returns>
        public ActionResult Edit(string data)
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

            try
            {
                int idEmpleado = Convert.ToInt32(DecodeParam(data));

                if (idEmpleado > 0)
                {
                    Debug.WriteLine("Entra con idempleado " + idEmpleado);
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    Empleado empleado = classEmpleado.GetEmpleadoToEdit(idEmpleado, IdUnidadNegocio, IdCliente);
                    empleado.lPeriodosProcesados = classEmpleado.getPeriodosProcesados(idEmpleado);
                    if (Session["sRelojChecador"].ToString() == "SI")
                    {
                        cUsers cu = new cUsers();
                        int estatus = 0;
                        string clave;
                        if (Session["sIdCliente"].ToString() == "141")
                        {
                            if (empleado.ClaveEmpleado.Substring(0, 1) == "H")
                            {
                                clave = empleado.ClaveEmpleado.Substring(1, empleado.ClaveEmpleado.Length - 1);
                            }
                            else
                            {
                                clave = empleado.Imss;
                            }
                        }
                        else
                        {
                            clave = empleado.Imss;
                        }
                        estatus = cu.ListarUsuariosGV().Where(x => x.Identifier == clave).Select(x => x.Enabled).FirstOrDefault();
                        if (estatus == 0)
                        {
                            empleado.RelojChecador = false;
                        }
                        else
                        {
                            empleado.RelojChecador = true;
                        }
                    }
                    return View(empleado);
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

        /// <summary>
        /// Acción que genera la vista del archivo de carga por layout.
        /// </summary>
        /// <returns>Regresa la vista con el archivo de carga por layout.</returns>
        public ActionResult Batch()
        {
            try
            {
                ClassEmpleado ce = new ClassEmpleado();

                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

                var model = ce.getCatalogos(IdCliente);
                return View(model);
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Acción que guarda el archivo con varios registros de empleados.
        /// </summary>
        /// <param name="postedFile">Actúa como clase base para las clases que proporcionan acceso a los archivos individuales que ha cargado un cliente.</param>
        /// <returns>Regresa la vista con un resumen del archivo con varios registros de empleados.</returns>
        [HttpPost]
        public ActionResult Batch(HttpPostedFileBase postedFile)
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

            //Configuracion del archivo csv
            string filePath = string.Empty;
            var extension = Path.GetExtension(postedFile.FileName);
            //Validacion del csv
            if (postedFile != null && extension == ".csv")
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath, Encoding.UTF7);
                FileImport file = new FileImport(IdCliente, IdUnidadNegocio);
                file.Init(csvData, postedFile.FileName, IdUsuario, IdUnidadNegocio);
                return File(file);
            }

            ViewBag.confirmation = false;
            ViewBag.title = "Inserción Empleados";
            ViewBag.alert = "¡Inserción Errónea!";
            ViewBag.message = "Error de lectura de archivo " + postedFile.FileName + ".";
            return View("Response");
        }

        //Vista BajaBatch
        /// <summary>
        /// Acción que descarga el archivo con el formato de empleados.
        /// </summary>
        /// <returns>Regresa la vista con el archivo descargado.</returns>
        public ActionResult BajaBatch()
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
        /// Acción que descarga el archivo de layout de empleados.
        /// </summary>
        /// <param name="postedFile"> Actúa como clase base para las clases que proporcionan acceso a los archivos individuales que ha cargado un cliente.</param>
        /// <returns>Regresa la vista del archivo descargado.</returns>
        [HttpPost]
        public ActionResult BajaBatch(HttpPostedFileBase postedFile)
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

            //Configuracion del archivo csv
            string filePath = string.Empty;
            //Validacion del csv
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath);
                FileImport file = new FileImport(IdCliente, IdUnidadNegocio);
                file.InitBaja(csvData, postedFile.FileName, IdUsuario, IdUnidadNegocio);
                return FileD(file);
            }

            ViewBag.confirmation = false;
            ViewBag.title = "Inserción Empleados";
            ViewBag.alert = "¡Inserción Errónea!";
            ViewBag.message = "Error de lectura de archivo.";
            return View("Response");
        }

        /// <summary>
        /// Acción que genera archivo con el resumen de la carga por layout de empleados.
        /// </summary>
        /// <param name="file">Recibe el modelo.</param>
        /// <returns>Regresa el archivo generado con el resumen de la carga.</returns>
        [HttpPost]
        public ActionResult File(FileImport file)
        {
            DateTime date = DateTime.Now;
            return File(file.DetailUpload().GetBuffer(), "text/plain", "DetalleCargaEmpleados" + date.ToString("yyyyMMddhhmm") + ".txt");
        }

        /// <summary>
        /// Acción que genera archivo con el resumen de la carga por layout de empleados.
        /// </summary>
        /// <param name="file">Recibe el modelo.</param>
        /// <returns>Regresa el archivo generado.</returns>
        [HttpPost]
        public ActionResult FileD(FileImport file)
        {
            DateTime date = DateTime.Now;
            return File(file.DetailDismiss().GetBuffer(), "text/plain", "DetalleBajaEmpleados" + date.ToString("yyyyMMddhhmm") + ".txt");
        }

        /// <summary>
        /// Acción que genera la vista para la respuesta del CRUD.
        /// </summary>
        /// <returns>Regresa la vista principal.</returns>
        public new ActionResult Response()
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

        public ActionResult ResponseBaja()
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

        //Data
        /// <summary>
        /// Acción para editar al empleado.
        /// </summary>
        /// <param name="empleado">Recibe el modelo empleado.</param>
        /// <param name="RelojChecador">Recibe la variable tipo bool.</param>
        /// <returns>Regresa la vista del empleado modificado.</returns>
        [HttpPost]
        public ActionResult RegistrarEmpleado(Empleado empleado)
        {
            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            string tokenNom = "";

            try
            {
                IdCliente = (int)Session["sIdCliente"];
                IdUsuario = (int)Session["sIdUsuario"];
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                tokenNom = Session["sToken"].ToString();

                if (ModelState.IsValid)
                {
                    empleado.IdCaptura = IdUsuario;
                    ClassEmpleado registraEmpleado = new ClassEmpleado();
                    if (registraEmpleado.RevisaCLaverepetida(empleado.ClaveEmpleado, IdUnidadNegocio) == false)
                    {
                        //registraEmpleado.AddEmpleado(empleado);
                        int value = registraEmpleado.AddEmpleado(empleado, IdCliente, tokenNom);
                        if (value == 1)
                        {
                            ViewBag.confirmation = true;
                            ViewBag.title = "Registro Empleado";
                            ViewBag.alert = "¡Registro Exitoso!";
                            ViewBag.message = "Se ha guardado la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + " correctamente.";


                            ////////////Reloj Checador ////////////////
                            if (Session["sRelojChecador"].ToString() != "SI")
                            {
                                return View("Response");
                            }
                            else
                            {
                                ClassAccesosGV CAGV = new ClassAccesosGV();
                                cRegistroAsistencias cRegistro = new cRegistroAsistencias();
                                cUsers cu = new cUsers();
                                var Accesos = CAGV.DatosGV(IdCliente);
                                if (empleado.RelojChecador  && Accesos != null)
                                {
                                    var r = cu.Add(empleado, Accesos);
                                    if (r.Success == false)
                                    {
                                        cu.Enable(empleado, Accesos);
                                    }
                                    else
                                    {
                                        ViewBag.confirmation = false;
                                        ViewBag.title = "Registro Empleado";
                                        ViewBag.alert = "¡Registro en checador erróneo!";
                                        ViewBag.message = "Verificar datos ingresados";
                                    }
                                }
                            }
                            ////////////Reloj Checador ////////////////

                            return View("Response");
                        }
                        if (value == -1)
                        {
                            ViewBag.confirmation = false;
                            ViewBag.title = "Registro Empleado";
                            ViewBag.alert = "¡Registro Erróneo!";
                            ViewBag.message = "La fecha de alta ante el IMSS excede los 5 dias habiles extemporaneos";//La fecha de alta ante el IMSS excede el dia anticipado
                            return View("Response");
                        }
                        if (value == -2)
                        {
                            ViewBag.confirmation = false;
                            ViewBag.title = "Registro Empleado";
                            ViewBag.alert = "¡Registro Erróneo!";
                            ViewBag.message = "La fecha de alta ante el IMSS excede el dia anticipado";
                            return View("Response");
                        }
                        if (value == -3)
                        {
                            ViewBag.confirmation = false;
                            ViewBag.title = "Registro Empleado";
                            ViewBag.alert = "¡Registro Erróneo!";
                            ViewBag.message = "Verifique que el valor de Sueldo Diario Base sea mayor a 0 y mayor o igual al Salario Minimo General vigente";
                            return View("Response");
                        }
                        if (value == -4)
                        {
                            ViewBag.confirmation = false;
                            ViewBag.title = "Registro Empleado";
                            ViewBag.alert = "¡Registro Erróneo!";
                            ViewBag.message = "Se detecto que el registro contiene algun dato relativo a IMSS y no tiene ningun registro patronal asociado";
                            return View("Response");
                        }
                        else
                        {
                            ViewBag.confirmation = false;
                            ViewBag.title = "Registro Empleado";
                            ViewBag.alert = "¡Registro Erróneo!";
                            ViewBag.message = "No se logró guardar la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + ".";
                            return View("Response");
                        }
                    }
                    else
                    {
                        ViewBag.confirmation = false;
                        ViewBag.title = "Registro Empleado";
                        ViewBag.alert = "¡Registro Erróneo!";
                        ViewBag.message = "La clave del empleado ya esta registrada";
                        return View("Response");
                    }

                }
                else
                {
                    ViewBag.confirmation = false;
                    ViewBag.title = "Registro Empleado";
                    ViewBag.alert = "¡Registro Erróneo!";
                    ViewBag.message = "No se logró guardar la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + ".";
                    return View("Response");
                }
            }
            catch (Exception ex)
            {
                ViewBag.confirmation = false;
                ViewBag.message = "No se logró guardar la información " + ex.Message;
                return View("Response");
            }
        }

        /// <summary>
        /// Acción para obtener empleados en general.
        /// </summary>
        /// <returns>Regresa un Json de los empleados.</returns>
        [HttpPost]
        public JsonResult GetEmpleados()
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleados(IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Accion para obtener empleados por nombre.
        /// </summary>
        /// <param name="name">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con los empleados por nombre.</returns>
        [HttpPost]
        public JsonResult GetEmpleadosByNombre(string name)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosByNombre(name, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Accion para obtener empleados por clave.
        /// </summary>
        /// <param name="clave">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con el empleado.</returns>
        [HttpPost]
        public JsonResult GetEmpleadosByClave(string clave)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                var data = empleados.GetEmpleadosByClave(clave, IdUnidadNegocio);
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Accion para editar empleado.
        /// </summary>
        /// <param name="empleado">Recibe el modelo empleado.</param>
        /// <returns>Regresa la vista del empleado modificado.</returns>
        [HttpPost]
        public ActionResult EditEmpleado(Empleado empleado)
        {

            int IdUsuario = 0;

            try
            {
                IdUsuario = (int)Session["sIdUsuario"];
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["sRelojChecador"].ToString() == "SI")
            {
                try
                {
                    var cRegistro = new cRegistroAsistencias();
                    ClassAccesosGV CAGV = new ClassAccesosGV();

                    int? IdCliente = int.Parse(Session["sIdCliente"].ToString());
                    var Accesos = CAGV.DatosGV((int)IdCliente);
                    cUsers CU = new cUsers();
                    string clave;
                    if (Session["sIdCliente"].ToString() == "141")
                    {
                        if (empleado.ClaveEmpleado.Substring(0, 1) == "H")
                        {
                            clave = empleado.ClaveEmpleado.Substring(1, empleado.ClaveEmpleado.Length - 1);
                        }
                        else
                        {
                            clave = empleado.Imss;
                        }
                    }
                    else
                    {
                        clave = empleado.Imss;
                    }
                    int estatus = CU.ListarUsuariosGV().Where(x => x.Identifier == clave).Select(x => x.Enabled).FirstOrDefault();

                    mResultEdit res = new mResultEdit();
                    if (empleado.RelojChecador)
                    {
                        if (estatus == 1)
                        {
                            res = CU.Edit(empleado, Accesos);
                            if (empleado.IdEstatus != 1)
                            {
                                res = CU.Disable(empleado, Accesos);
                            }
                        }
                        else
                        {
                            res = CU.Enable(empleado, Accesos);
                            if (res.Success == false)
                            {
                                res = CU.Add(empleado, Accesos);
                            }
                        }
                    }
                    else
                    {
                        if (estatus == 1)
                        {
                            res = CU.Disable(empleado, Accesos);
                        }
                    }
                }
                catch
                {
                }
            }

            if (ModelState.IsValid == true)
            {
                empleado.IdModificacion = IdUsuario;
                int ClienteAdministrado = int.Parse(Session["sClienteAdministrado"].ToString());
                ClassEmpleado emp = new ClassEmpleado();
                int value = emp.EditEmpleado(empleado, ClienteAdministrado);

                if (value > 0)
                {
                    ViewBag.confirmation = true;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "¡Modificación Exitosa!";
                    ViewBag.message = "Se ha modificado la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + " correctamente.";
                    return View("Response");
                }
                else
                {
                    switch (value)
                    {
                        case -1:
                            ViewBag.confirmation = false;
                            ViewBag.title = "Modicación Empleado";
                            ViewBag.alert = "¡Modificación Errónea!";
                            ViewBag.message = "No se logró modificar la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + ", por favor verifica que la fecha no exceda los 5 dias habiles.";
                            return View("Response");

                        case -2:
                            ViewBag.confirmation = false;
                            ViewBag.title = "Modicación Empleado";
                            ViewBag.alert = "¡Modificación Errónea!";
                            ViewBag.message = "No se logró modificar la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + ", por favor verifica que la fecha no exceda los 1 dias adelantados.";
                            return View("Response");

                    }
                    ViewBag.confirmation = false;
                    ViewBag.title = "Modicación Empleado";
                    ViewBag.alert = "¡Modificación Errónea!";
                    ViewBag.message = "No se logró modificar la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + ", por favor verifica los datos.";
                    return View("Response");
                }
            }
            else
            {

                ViewBag.confirmation = false;
                ViewBag.title = "Modicación Empleado";
                ViewBag.alert = "¡Modificación Errónea!";
                ViewBag.message = "No se logró modificar la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + ". Fallo al validar el modelo completo de datos.";
                return View("Response");
            }
        }

        /// <summary>
        /// Accion para codificar URL.
        /// </summary>
        /// <param name="param">Recibe la variable tipo string.</param>
        /// <returns>Regresa la cadena que contiene la matriz codificada.</returns>
        [HttpPost]
        public string EncodeParam(string param)
        {
            byte[] array = Encoding.ASCII.GetBytes(param);
            return Server.UrlTokenEncode(array);
        }

        //[HttpPost]
        //public JsonResult getPeriodosProcesados(int IdEmpleado)
        //{
        //    ClassEmpleado ce = new ClassEmpleado();
        //    var list = ce.getPeriodosProcesados(IdEmpleado);

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Accion para decoficar url
        /// </summary>
        /// <param name="param">Recibe la variable tipo string.</param>
        /// <returns>Regresa la cadena que contiene el resultado de la descodificación.</returns>
        private string DecodeParam(string param)
        {
            byte[] array = Server.UrlTokenDecode(param);
            return Encoding.UTF8.GetString(array);
        }

        /// <summary>
        /// Accion para obtener codigos postales
        /// </summary>
        /// <param name="cp">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con los códigos postales.</returns>
        [HttpPost]
        public JsonResult GetCodigosPostales(string cp)
        {
            ClassEmpleado cps = new ClassEmpleado();
            return Json(cps.GetCodigoPostales(cp), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Accion para obtener detalle del codigo postal
        /// </summary>
        /// <param name="cp">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con el detalle de códigos postales.</returns>
        [HttpPost]
        public JsonResult GetDetailCP(string cp)
        {
            ClassEmpleado cps = new ClassEmpleado();
            return Json(cps.GetCodigoPostalesByString(cp), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Accion para obtener CP por id
        /// </summary>
        /// <param name="data">Recibe la variable tipo int.</param>
        /// <returns>Regresa un Json con el CP específico.</returns>
        [HttpPost]
        public JsonResult GetCPByID(int data)
        {
            ClassEmpleado cps = new ClassEmpleado();
            return Json(cps.GetCodigoPostalesById(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Acción para descargar el área.
        /// </summary>
        /// <param name="tipo">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con los datos del área</returns>
        [HttpPost]
        public JsonResult DescargarCC(string tipo)
        {
            int IdCliente = (int)Session["sIdCliente"];

            var json = string.Empty;
            if (tipo == "CC")
            {
                ClassCentrosCostos ccc = new ClassCentrosCostos();
                var cc = ccc.getCentrosCostosByIdCliente(IdCliente).Select(x => new { x.IdCentroCostos, x.CentroCostos });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "ptos")
            {
                ClassPuestos ccc = new ClassPuestos();
                var cc = ccc.GetPuestosByIdCliente(IdCliente).Select(x => new { x.IdPuesto, x.Puesto });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "deptos")
            {
                ClassDepartamentos ccc = new ClassDepartamentos();
                var cc = ccc.GetDepartamentosByIdCliente(IdCliente).Select(x => new { x.IdDepartamento, x.Departamento });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "Suc")
            {
                ClassSucursales ccc = new ClassSucursales();
                var cc = ccc.GetSucursalesByIdCliente(IdCliente).Select(x => new { x.IdSucursal, x.Sucursal });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "regPat")
            {
                ClassEmpleado ccc = new ClassEmpleado();
                var cc = ccc.ObtenerRPByIdCliente(IdCliente).Select(x => new { x.IdRegistroPatronal, x.NombrePatrona });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "Ban")
            {
                ClassBancos ccc = new ClassBancos();
                var cc = ccc.getBancos().Select(x => new { x.IdBanco, x.NombreBanco });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "Ent")
            {
                ClassEntidadFederativa ccc = new ClassEntidadFederativa();
                var cc = ccc.getEntidades().Select(x => new { x.Id, x.Nombre });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "Are")
            {
                ClassAreas ccc = new ClassAreas();
                var cc = ccc.getAreas().Select(x => new { x.IdArea, x.Area });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }
            if (tipo == "Jor")
            {
                ClassJornadaL ccc = new ClassJornadaL();
                var cc = ccc.GetJornadas(IdCliente).Select(x => new { x.IdJornada, x.Jornada });
                var jsonSerializer = new JavaScriptSerializer();
                json = jsonSerializer.Serialize(cc);
            }


            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Acción pata editar al empleado.
        /// </summary>
        /// <returns>Regresa la vista con los empleados.</returns>
        public ActionResult EditarEmpleados()
        {
            try
            {
                ClassEmpleado ce = new ClassEmpleado();

                int IdCliente = (int)Session["sIdCliente"];
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

                var model = ce.getCatalogos(IdCliente);


                return View(model);
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Acción para generar archivo de excel.
        /// </summary>
        /// <returns>Regresa el archivo de excel.</returns>
        public FileResult Excel()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado initEmpleado = new ClassEmpleado();

            var arch = initEmpleado.Excel(IdUnidadNegocio);

            return File(arch, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Empleados.xlsx");
        }

        /// <summary>
        /// Acción que guarda la modificación del empleado.
        /// </summary>
        /// <param name="postedFile">Actúa como clase base para las clases que proporcionan acceso a los archivos individuales que ha cargado un cliente.</param>
        /// <returns>Regresa la vista del empleado modificado.</returns>
        [HttpPost]
        public ActionResult EditarEmpleados(HttpPostedFileBase postedFile)
        {
            ClassEmpleado ce = new ClassEmpleado();
            int IdCliente = (int)Session["sIdCliente"];

            var model = ce.getCatalogos(IdCliente);

            if (postedFile == null)
            {
                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Iniciaste Sesión en:', 'Se actualizo correctamente la informacion.', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");

                return View(model);
            }

            ClassEmpleado clas = new ClassEmpleado();
            int idUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            var csvTable = new DataTable();
            using (var csvReader = new CsvReader(postedFile.InputStream, true, Encoding.UTF8))
            {
                csvTable.Load(csvReader);
            }

            decimal SMV = clas.ObtenSMV();
            //var csvTable = new DataTable();
            List<Empleados> ListEmpleado = new List<Empleados>();

            try
            {
                using (TadaEmpleados tada = new TadaEmpleados())
                {
                    ListEmpleado = tada.Empleados.Where(n => n.IdUnidadNegocio == idUnidadNegocio && n.IdEstatus == 1).ToList();

                    foreach (DataRow dr in csvTable.Rows)
                    {

                        string IdBancoViaticos = null;
                        try { IdBancoViaticos = dr["[Id_BancoViaticos]"].ToString(); } catch { IdBancoViaticos = null; }
                        string Cuenta_Bancaria_Viaticos = null;
                        try { Cuenta_Bancaria_Viaticos = dr["[Cuenta_Bancaria_Viaticos]"].ToString(); } catch { Cuenta_Bancaria_Viaticos = null; }
                        string Cuenta_InterBancaria_Viaticos = null;
                        try { Cuenta_InterBancaria_Viaticos = dr["[Cuenta_InterBancaria_Viaticos]"].ToString(); } catch { Cuenta_InterBancaria_Viaticos = null; }

                        Empleados emp = ListEmpleado.Where(n => n.ClaveEmpleado == dr["[Cve.]"].ToString()).FirstOrDefault();

                        if (emp != null)
                        {
                            if (!string.IsNullOrEmpty(dr["[Sexo]"].ToString()))
                            {
                                emp.Sexo = dr["[Sexo]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[Fecha_Nacimiento]"].ToString()))
                            {
                                emp.FechaNacimiento = DateTime.Parse(dr["[Fecha_Nacimiento]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Fecha_Reconocimiento_Antiguedad]"].ToString()))
                            {
                                emp.FechaReconocimientoAntiguedad = DateTime.Parse(dr["[Fecha_Reconocimiento_Antiguedad]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Fecha_Alta_SS]"].ToString()))
                            {
                                emp.FechaAltaIMSS = DateTime.Parse(dr["[Fecha_Alta_SS]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Rfc]"].ToString()))
                            {
                                emp.Rfc = dr["[Rfc]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[Curp]"].ToString()))
                            {
                                emp.Curp = dr["[Curp]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[NSS]"].ToString()))
                            {
                                emp.Imss = dr["[NSS]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[Sueldo_Diario_Base]"].ToString()))
                            {
                                if (decimal.Parse(dr["[Sueldo_Diario_Base]"].ToString()) < SMV && decimal.Parse("[Sueldo_Diario_Base]") > 0)
                                {
                                    emp.SDIMSS = SMV;
                                }
                                else
                                {
                                    emp.SDIMSS = decimal.Parse(dr["[Sueldo_Diario_Base]"].ToString());
                                }
                            }
                            if (!string.IsNullOrEmpty(dr["[Id_Banco_Tradicional]"].ToString()))
                            {
                                emp.IdBancoTrad = int.Parse(dr["[Id_Banco_Tradicional]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Id_Registro_Patronal]"].ToString()))
                            {
                                emp.IdRegistroPatronal = int.Parse(dr["[Id_Registro_Patronal]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Cuenta_Bancaria]"].ToString()))
                            {
                                emp.CuentaBancariaTrad = dr["[Cuenta_Bancaria]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[Cuenta_Interbancaria]"].ToString()))
                            {
                                emp.CuentaInterbancariaTrad = dr["[Cuenta_Interbancaria]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[Id_entidad]"].ToString()))
                            {
                                emp.IdEntidad = int.Parse(dr["[Id_entidad]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Sueldo_Diario_Real]"].ToString()))
                            {
                                emp.SD = decimal.Parse(dr["[Sueldo_Diario_Real]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Tipo_Contrato]"].ToString()))
                            {
                                emp.TipoContrato = dr["[Tipo_Contrato]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[Fecha_Termino_Contrato]"].ToString()))
                            {
                                emp.FechaTerminoContrato = DateTime.Parse(dr["[Fecha_Termino_Contrato]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Id_Departamento]"].ToString()))
                            {
                                emp.IdDepartamento = int.Parse(dr["[Id_Departamento]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Id_Puesto]"].ToString()))
                            {
                                emp.IdPuesto = int.Parse(dr["[Id_Puesto]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Id_Centro_de_Costos]"].ToString()))
                            {
                                emp.IdCentroCostos = int.Parse(dr["[Id_Centro_de_Costos]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[Correo_Electronico]"].ToString()))
                            {
                                emp.CorreoElectronico = dr["[Correo_Electronico]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[IdArea]"].ToString()))
                            {
                                emp.IdArea = int.Parse(dr["[IdArea]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[IdSindicato]"].ToString()))
                            {
                                emp.idSindicato = int.Parse(dr["[IdSindicato]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[IdSucursal]"].ToString()))
                            {
                                emp.IdSucursal = int.Parse(dr["[IdSucursal]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[IdJornada]"].ToString()))
                            {
                                emp.IdJornada = int.Parse(dr["[IdJornada]"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["[CalleFiscal]"].ToString()))
                            {
                                emp.Calle = dr["[CalleFiscal]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[noExtFiscal]"].ToString()))
                            {
                                emp.noExt = dr["[noExtFiscal]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[noIntFiscal]"].ToString()))
                            {
                                emp.noInt = dr["[noIntFiscal]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[CPFiscal]"].ToString()))
                            {
                                emp.CP = dr["[CPFiscal]"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["[IdPrestaciones]"].ToString()))
                            {
                                emp.IdPrestaciones = int.Parse(dr["[IdPrestaciones]"].ToString());
                            }

                            //Viaticos
                            if (!string.IsNullOrEmpty(IdBancoViaticos))
                            {
                                emp.IdBancoViaticos = int.Parse(IdBancoViaticos);
                            }
                            if (!string.IsNullOrEmpty(Cuenta_Bancaria_Viaticos))
                            {
                                emp.CuentaBancariaViaticos = Cuenta_Bancaria_Viaticos;
                            }
                            if (!string.IsNullOrEmpty(Cuenta_InterBancaria_Viaticos))
                            {
                                emp.CuentaInterbancariaViaticos = Cuenta_InterBancaria_Viaticos;
                            }

                            if (!string.IsNullOrEmpty(dr["[Telefono]"].ToString()))
                            {
                                emp.Telefono = dr["[Telefono]"].ToString().Replace("/", "").Replace(" ", "").Replace("-", "");
                            }
                            if (!string.IsNullOrEmpty(dr["[Nacionalidad]"].ToString()))
                            {
                                try
                                {
                                    if (dr["[Nacionalidad]"].ToString().ToUpper().Contains("MEX"))
                                        emp.Nacionalidad = "MEXICANA";
                                    else if (dr["[Nacionalidad]"].ToString().ToUpper().Contains("EXT"))
                                        emp.Nacionalidad = "EXTRANJERA";
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    tada.SaveChanges();
                    ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Actualiza Empleados', 'Se actualizo correctamente la informacion.', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");


                }
            }
            catch (Exception ex)
            {
                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Actualiza Empleados', 'No se actualizo la información: " + ex.Message + ", 'danger', 'bounceInRight', 'bounceOutLeft', 4500);");

            }

            return View(model);
        }

        /// <summary>
        /// Acción para actualizar el sueldo.
        /// </summary>
        /// <returns>Regresa la vista principal.</returns>
        public ActionResult ActualizaSueldos()
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
        /// Acción para modificar el sueldo.
        /// </summary>
        /// <param name="data">Recibe la variable tipo string.</param>
        /// <returns>Regresa la vista con el modelo de la edición de sueldo.</returns>
        public ActionResult EditSueldos(string data)

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
                    Debug.WriteLine("Entra con idempleado " + idEmpleado);
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    ModelModificacionSueldos empleado = classEmpleado.GetEmpleadoParaMS(idEmpleado);
                    return View(empleado);
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

        /// <summary>
        /// Accion para guardar la informacion.
        /// </summary>
        /// <param name="em">Recibe el modelo de modificación de sueldo.</param>
        /// <returns>Regresa la vista de sueldo modificado.</returns>
        [HttpPost]
        public ActionResult EditSueldos(ModelModificacionSueldos em)
        {

            int IdCliente = 0;
            int IdUsuario = 0;
            int IdUnidadNegocio = 0;
            string data = em.IdEmpleado.ToString();
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

            int resul = 0;
            try
            {
                using (NominaEntities1 tada = new NominaEntities1())
                {
                    resul = tada.SP_CambioSueldos(IdUsuario, IdUnidadNegocio, em.IdEmpleado, IdCliente, Convert.ToDateTime(em.FechaMovimiento), em.SDIMSSSueldos, em.SDISueldos, em.SDSueldos, em.Observaciones);
                }
                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Actualiza Saldos', 'Se actualizo su saldo exitosamente!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");
                return View();
            }
            catch
            {
                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Actualiza Saldos', 'No se actualizo la informacion', 'warning', 'bounceInRight', 'bounceOutLeft', 4500);");
                return View();
            }
        }

        /// <summary>
        /// Metodo para calcular el SDI
        /// </summary>
        /// <param name="IdEmpleado">Recibe la variable tipo int.</param>
        /// <param name="SDIMSS">Recibe la variable tipo decimal.</param>
        /// <param name="FechaReconocimientoAntiguedad">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json del cálculo de SDI.</returns>
        [HttpPost]
        public JsonResult CalcularSdI(int IdEmpleado, decimal SDIMSS, string FechaReconocimientoAntiguedad, int? IdPrestaciones)
        {
            ClassNomina cs = new ClassNomina();
            int idCliente = int.Parse(Session["sIdCliente"].ToString());

            var ml = Math.Round(cs.ObtenSDI(IdEmpleado, SDIMSS, FechaReconocimientoAntiguedad, idCliente, IdPrestaciones), 2);
            return Json(ml, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Acción para buscar a un empleado reactivado.
        /// </summary>
        /// <returns>Regresa la vista con el modelo.</returns>
        public ActionResult ReactivarEmpleadoSearch()
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
        /// Acción para reactivar a un empleado.
        /// </summary>
        /// <param name="data">Recibe la variable tipo string.</param>
        /// <returns>Regresa la vista con la información.</returns>
        public ActionResult EmpleadosReactivacion(string data)

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


            try
            {
                int idEmpleado = Convert.ToInt32(DecodeParam(data));

                if (idEmpleado > 0)
                {
                    Debug.WriteLine("Entra con idempleado " + idEmpleado);
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    Empleado empleado = classEmpleado.GetEmpleadoToEditBaja(idEmpleado, IdUnidadNegocio, IdCliente);
                    return View(empleado);
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

        public ActionResult EmpleadosEditarInfoBaja(string data)

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


            try
            {
                int idEmpleado = Convert.ToInt32(DecodeParam(data));

                if (idEmpleado > 0)
                {
                    Debug.WriteLine("Entra con idempleado " + idEmpleado);
                    ClassEmpleado classEmpleado = new ClassEmpleado();
                    Empleado empleado = classEmpleado.GetEmpleadoToEditBaja(idEmpleado, IdUnidadNegocio, IdCliente);
                    return View(empleado);
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

        /// <summary>
        /// Acción que obtiene a un empleado dado de baja.
        /// </summary>
        /// <param name="name">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con el resultado de la consulta.</returns>
        [HttpPost]
        public JsonResult GetEmpleadosByNombreBaja(string name)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosByNombreBaja(name, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Accion para obtener empleados por clave
        /// </summary>
        /// <param name="clave">Recibe la variable tipo string.</param>
        /// <returns>Regresa un Json con el empleado específico.</returns>
        [HttpPost]
        public JsonResult GetEmpleadosByClaveBaja(string clave)
        {
            try
            {
                int IdUnidadNegocio = 0;
                IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                ClassEmpleado empleados = new ClassEmpleado();
                return Json(new { data = empleados.GetEmpleadosByClaveBaja(clave, IdUnidadNegocio) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Acción que guarda la reactivación del empleado.
        /// </summary>
        /// <param name="empleado">Recibe el modelo de empleado.</param>
        /// <returns>Regresa la vista con la información de la reactivación del empleado.</returns>
        [HttpPost]
        public ActionResult EmpleadosReactivacion(Empleado empleado)
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
            empleado.IdCaptura = IdUsuario;
            ClassEmpleado registraEmpleado = new ClassEmpleado();
            registraEmpleado.AddEmpleadoBaja(empleado);
            ViewBag.confirmation = true;
            ViewBag.title = "Reactivacion Empleado";
            ViewBag.alert = "¡Reactivación Exitosa!";
            ViewBag.message = "Se ha guardado la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + " correctamente.";
            return View("ResponseBaja");
        }

        [HttpPost]
        public ActionResult EmpleadosEditarInfoBaja(Empleado empleado)
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
            empleado.IdCaptura = IdUsuario;
            ClassEmpleado registraEmpleado = new ClassEmpleado();
            registraEmpleado.EditEmpleadoBaja(empleado);
            ViewBag.confirmation = true;
            ViewBag.title1 = "Actualización Empleado";
            ViewBag.alert = "¡Actualización Exitosa!";
            ViewBag.message = "Se ha guardado la información de " + empleado.Nombre + " " + empleado.ApellidoPaterno + " " + empleado.ApellidoMaterno + " correctamente.";
            return View("ResponseBaja");
        }
    }
}