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
    public class AreasController : BaseController
    {
        // GET: Areas
        // GET: Areas
        /// <summary>
        /// Acción que lista las áreas por cliente específico.
        /// </summary>
        /// <returns>Regresa la vista con las áreas por cliente específico.</returns>
        public ActionResult Area()
        {

            int idcliente = 0;
            try { idcliente = (int)Session["sIdCliente"]; } catch { idcliente = 0; }

            if (idcliente == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                var clsArea = new ClassArea();
                return View(clsArea.getAreasbyidCliente(idcliente));
            }
        }

        /// <summary>
        /// Acción que muestra los detalles de las áreas.
        /// </summary>
        /// <param name="id">Recibe el identificador de el área.</param>
        /// <returns>Regresa la vista parcial con los detalles de las áreas por identificador de área.</returns>
        public ActionResult Details(int id)
        {
            var clsArea = new ClassArea();
            return PartialView(clsArea.GetModelAreas(id));
        }

        /// <summary>
        /// Acción que agrega un área.
        /// </summary>
        /// <returns>Regresa la vista con el área a agregar.</returns>
        public ActionResult Create()
        {
            var areas = new ModelAreas();
            return View(areas);
        }

        /// <summary>
        /// Acción que agrega el área.
        /// </summary>
        /// <param name="collection">Recibe el modelo de áreas.</param>
        /// <returns>Regresa la vista de áreas.</returns>
        [HttpPost]

        public ActionResult Create(ModelAreas collection)
        {
            int idCliente = (int)Session["sIdCliente"];
            try
            {
                if (ModelState.IsValid)
                {
                    var clsArea = new ClassArea();
                    clsArea.addAreas(collection, idCliente, (int)Session["sIdUsuario"]);
                    return RedirectToAction("Area");
                }
                else
                {
                    var centrocostos = new ModelCentroCostos();
                    return View(centrocostos);
                }
            }
            catch
            {
                return View();
            }

        }

        /// <summary>
        /// Acción para modificar la área.
        /// </summary>
        /// <param name="id">Recibe el identificador de la área.</param>
        /// <returns>Regresa la vista del área a modificar.</returns>
        public ActionResult Edit(int id)
        {
            var clsArea = new ClassArea();
            return View(clsArea.GetModelAreas(id));
        }

        /// <summary>
        /// Acción para modificar el área.
        /// </summary>
        /// <param name="id">Recibe el identificador del área.</param>
        /// <param name="collection">Recibe el modelo de las áreas.</param>
        /// <returns>Regresa la vista con la lista de áreas.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelAreas collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idusuario = (int)Session["sIdUsuario"];
                    var clsArea = new ClassArea();
                    clsArea.UpdateAreas(id, idusuario, collection);

                    return RedirectToAction("Area");
                }
                else
                {
                    var clsArea = new ClassArea();
                    return View(clsArea.GetModelAreas(id));
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina un registro de área.
        /// </summary>
        /// <param name="id">Recibe el identificador del área.</param>
        /// <returns>Regresa una vista con la confirmación de la eliminación.</returns>
        public ActionResult Delete(int id)
        {
            var clsArea = new ClassArea();
            var model = clsArea.ValidaArea(id);
            return PartialView(model);
        }

        /// <summary>
        /// Acción que elimina un registro de área.
        /// </summary>
        /// <param name="id">Recibe el identificador del área.</param>
        /// <param name="collection">Contiene los proveedores de valor de formulario para la aplicación.</param>
        /// <returns>Regresa a la vista de las áreas.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var clsArea = new ClassArea();
                int idUsuario = (int)Session["sIdUsuario"];
                clsArea.DeleteAreas(id, idUsuario);

                return RedirectToAction("Area");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción para subir las áreas de forma masiva.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de áreas.</returns>
        public ActionResult CreateLayout()
        {
            //Debug.WriteLine("Nombre de Ruta");
            var modelo = new ModelAreas();
            return View(modelo);
        }

        /// <summary>
        /// Acción que permite subir el archivo con las áreas a guardar.
        /// </summary>
        /// <param name="model">Recibe el modelo de áreas.</param>
        /// <returns>Regresa la vista con el modelo de áreas.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelAreas model)
        {
            Debug.WriteLine("NOMBRE DE RUTAhchcvhchcv");
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            var cc = new ClassArea();
            var modelo = new ModelAreas();

            if (ModelState.IsValid)
            {
                //Debug.WriteLine("NOMBRE DE RUTA");
                if (model.Archivo.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);
                    //Debug.WriteLine("NOMBRE DE RUTA" + _path.ToString() );
                    var errores = cc.GetAteas(_path, IdCliente, IdUsuario);

                    return TextFile(errores);
                }
            }

            return View(modelo);
        }

        /// <summary>
        /// Acción que genera un archivo con el resumen de los registros cargados.
        /// </summary>
        /// <param name="model">Recibe el modelo de errores de áreas.</param>
        /// <returns>Regresa el archivo con el resumen de la carga de los registros de áreas. </returns>
        public ActionResult TextFile(ModelErroresAreas model)
        {
            var memoryStream = new MemoryStream();
            var tw = new StreamWriter(memoryStream);
            var list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE AREAS DEL ARCHIVO: " + model.Path);
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
        /// Acción para descargar el archivo que contiene las áreas.
        /// </summary>
        /// <returns>Regresa el archivo.</returns>
        [HttpPost]
        public JsonResult Descarga()
        {
            int IdCliente = (int)Session["sIdCliente"];

            var ccc = new ClassArea();
            var cc = ccc.getAreasbyidCliente(IdCliente).Select(x => new { x.IdEstatus, x.Clave, x.Area });
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(cc);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}