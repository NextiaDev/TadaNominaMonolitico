using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class SucursalesController : BaseController
    {
        /// <summary>
        /// Acción que muestra las sucursales.
        /// </summary>
        /// <returns>Regresa la vista de sucursales.</returns>
        public ActionResult Index()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }

            if (IdCliente == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ClassSucursales clsSucursales = new ClassSucursales();
                return View(clsSucursales.GetSucursalesByIdCliente(IdCliente));
            }
        }

        //GET: Sucursales/Details
        /// <summary>
        /// Acción que muestra el detalle de una sucursal.
        /// </summary>
        /// <param name="id">Recibe el identificador de la sucursal.</param>
        /// <returns>Regresa la vista con el detalle de la sucursal.</returns>
        public ActionResult Details(int id)
        {
            ClassSucursales classSucursales = new ClassSucursales();
            return PartialView(classSucursales.GetModelSucursal(id));
        }

        /// <summary>
        /// acción que genera una sucursal.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de sucursales.</returns>
        public ActionResult Create()
        {
            ModelSucursales model = new ModelSucursales();
            return View(model);
        }

        /// <summary>
        /// Acción que guarda la sucursal nueva.
        /// </summary>
        /// <param name="collection">Recibe el modelo de sucursales.</param>
        /// <returns>Regresa la vista de las sucursales.</returns>
        [HttpPost]
        public ActionResult Create(ModelSucursales collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idCliente = (int)Session["sIdCliente"];
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassSucursales classSucursales = new ClassSucursales();
                    classSucursales.AddSucursales(collection, idCliente, idUsuario);

                    return RedirectToAction("Index", "Sucursales");
                }
                else
                {
                    ModelSucursales model = new ModelSucursales();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que modifica una sucursal.
        /// </summary>
        /// <param name="id">Recibe el identificador de la sucursal.</param>
        /// <returns>Regresa la vista de la sucursal a modificar.</returns>
        public ActionResult Edit(int id)
        {
            ClassSucursales classSucursales = new ClassSucursales();
            return View(classSucursales.GetModelSucursal(id));

        }

        /// <summary>
        /// Acción que guarda la sucursal modificada.
        /// </summary>
        /// <param name="id">Recibe el identificador de la sucursal.</param>
        /// <param name="collection">Recibe el modelo de sucursales.</param>
        /// <returns>Regresa la vista con las sucursales.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelSucursales collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassSucursales classSucursales = new ClassSucursales();
                    int idusuario = (int)Session["sIdUsuario"];
                    classSucursales.UpdateSucursales(collection, id, idusuario);

                    return RedirectToAction("Index");
                }
                else
                {
                    ClassSucursales classSucursales = new ClassSucursales();
                    return View(classSucursales.GetModelSucursal(id));
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina una sucursal.
        /// </summary>
        /// <param name="id">Recibe el identificador de la sucursal.</param>
        /// <returns>Regresa la vista de la sucursal.</returns>
        public ActionResult Delete(int id)
        {
            ClassSucursales classSucursales = new ClassSucursales();
            return PartialView(classSucursales.GetModelSucursal(id));

        }

        /// <summary>
        /// Acción que confirma la eliminación de la sucursal.
        /// </summary>
        /// <param name="id">Recibe el identificador de la sucursal.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista de las sucursales.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                //ClassSucursales classSucursales = new ClassSucursales();
                int idUsuario = (int)Session["sIdUsuario"];
                ClassSucursales classSucursales = new ClassSucursales();
                classSucursales.DeleteSucursales(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que genera el archivo de carga masiva.
        /// </summary>
        /// <returns>Regresa el modelo de las sucursales.</returns>
        public ActionResult CreateLayout()
        {

            ModelSucursales modelo = new ModelSucursales();
            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda el archivo de carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo de sucursales.</param>
        /// <returns>Regresa el archivo de carga masiva.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelSucursales model)
        {
            //Debug.WriteLine("NOMBRE DE RUTA");
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassSucursales csucursales = new ClassSucursales();
            ModelSucursales modelo = new ModelSucursales();

            if (ModelState.IsValid)
            {
                //Debug.WriteLine("NOMBRE DE RUTA");
                if (model.Archivo.ContentLength > 0)
                {

                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);
                    //Debug.WriteLine("NOMBRE DE RUTA" + _path.ToString() );
                    ModelErroresSucursales errores = csucursales.GetSucursales(_path, IdCliente, IdUsuario);

                    return TextFile(errores);
                }
            }

            return View(modelo);
        }

        /// <summary>
        /// Acción que genera el archivo resumen de la carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo de errores de sucursales.</param>
        /// <returns>Regresa el archivo de resumen.</returns>
        public ActionResult TextFile(ModelErroresSucursales model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE SUCURSALES DEL ARCHIVO: " + model.Path);
            tw.WriteLine("");
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("Numero de Registros Leidos: " + model.Errores);
            tw.WriteLine("Insertados correctamente: " + model.Correctos);
            tw.WriteLine("No Insertados: " + model.Errores);
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("");

            if (list.Count > 0)
            {
                tw.WriteLine("Detalle de los errores:");
                tw.WriteLine("");
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
            else
            {
                tw.WriteLine("El archivo se cargo correctamente.");
            }

            tw.Flush();
            tw.Close();
            return File(memoryStream.GetBuffer(), "text/plain", "errores.txt");
        }

        /// <summary>
        /// Acción que descarga el archivo de carga masiva.
        /// </summary>
        /// <returns>Regresa un Json con la información.</returns>
        [HttpPost]
        public JsonResult Descarga()
        {
            int IdCliente = (int)Session["sIdCliente"];

            ClassSucursales ccc = new ClassSucursales();
            var cc = ccc.GetSucursalesByIdCliente(IdCliente).Select(x => new { x.IdSucursal, x.Clave, x.Sucursal });
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(cc);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }

}