using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class PuestosController : BaseController
    {
        // GET: Puestos
        /// <summary>
        /// Acción que muestra los puestos por cliente.
        /// </summary>
        /// <returns>Regresa la vista con la lista de puestos.</returns>
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
                ClassPuestos clsPuestos = new ClassPuestos();
                return View(clsPuestos.GetPuestosByIdCliente(IdCliente));
            }
            
        }

        /// <summary>
        /// Acción que muestra el detalle del puesto.
        /// </summary>
        /// <param name="id">Recibe el identificador del puesto.</param>
        /// <returns>Regresa la vista con el detalle del puesto.</returns>
        public ActionResult Details(int id)
        {
            ClassPuestos classPuestos = new ClassPuestos();
            return PartialView(classPuestos.GetModelPuestos(id));
        }

        /// <summary>
        /// Acción que genera un puesto nuevo.
        /// </summary>
        /// <returns>Regresa la vista con el modelo del puesto.</returns>
        public ActionResult Create()
        {
            int IdCliente = (int)Session["sIdCliente"];
            ClassPuestos cp = new ClassPuestos();
            ModelPuestos model = new ModelPuestos();
            model.lDepartamentos = cp.getListDepartamentos(IdCliente);

            return View(model);
        }

        /// <summary>
        /// Acción que guarda el puesto nuevo.
        /// </summary>
        /// <param name="collection">Recibe el modelo del puesto.</param>
        /// <returns>Regresa la lista de puestos.</returns>
        [HttpPost]
        public ActionResult Create(ModelPuestos collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idCliente = (int)Session["sIdCliente"];
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassPuestos classPuestos = new ClassPuestos();
                    classPuestos.AddPuestos(collection, idCliente, idUsuario);

                    return RedirectToAction("Index", "Puestos");
                }
                else
                {
                    ModelPuestos model = new ModelPuestos();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que modifica el puesto.
        /// </summary>
        /// <param name="id">Recibe el identificador del puesto.</param>
        /// <returns>Regresa la vista del puesto específico.</returns>
        public ActionResult Edit(int id)
        {
            ClassPuestos classPuestos = new ClassPuestos();
            return View(classPuestos.GetModelPuestos(id));

        }

        /// <summary>
        /// Acción que guarda el puesto modificado.
        /// </summary>
        /// <param name="id">Recibe el identificador del puesto.</param>
        /// <param name="collection">Recibe el modelo del puesto.</param>
        /// <returns>Regresa la vista con los puestos.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelPuestos collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassPuestos classPuestos = new ClassPuestos();
                    int idusuario = (int)Session["sIdUsuario"];
                    classPuestos.UpdatePuesto(collection, id, idusuario);

                    return RedirectToAction("Index");
                }
                else
                {
                    ClassPuestos classPuestos = new ClassPuestos();
                    return View(classPuestos.GetModelPuestos(id));
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina el puesto.
        /// </summary>
        /// <param name="id">Recibe el identificador del puesto.</param>
        /// <returns>Regresa la vista con la confirmación de la eliminación del puesto.</returns>
        public ActionResult Delete(int id)
        {
            ClassPuestos classPuestos = new ClassPuestos();
            var model = classPuestos.ValidaPuestos(id);
            return PartialView(model);
        }

        /// <summary>
        /// Acción que confirma la eliminación del puesto.
        /// </summary>
        /// <param name="id">Recibe el identificador del puesto.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista con los puestos.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {               
                int idUsuario = (int)Session["sIdUsuario"];
                ClassPuestos classPuestos = new ClassPuestos();
                classPuestos.DeletePuesto(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que genera el archivo para carga masiva.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de puestos.</returns>
        public ActionResult CreateLayout()
        {
            //int IdUnidad = (int)Session["sIdUnidadNegocio"];
            //int IdCliente = (int)Session["sIdCliente"];
            //ClassPuestos cpuestos = new ClassPuestos();
            ModelPuestos modelo = new ModelPuestos();

            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda el archivo para carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo de puestos.</param>
        /// <returns>Regresa la vista con el modelo de puestos.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelPuestos model)
        {
            //int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassPuestos cpuestos = new ClassPuestos();
            ModelPuestos modelo = new ModelPuestos();
            
            if (model.Archivo.ContentLength > 0)
            {
                string fileName = Path.GetFileName(model.Archivo.FileName);
                string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                model.Archivo.SaveAs(_path);

                ModelErroresPuestos errores = cpuestos.GetPuestos(_path, IdCliente, IdUsuario);

                return TextFile(errores);
            }
            

                return View(modelo);
        }

        /// <summary>
        /// Acción que genera el archivo de resumen de la carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo de errores de puestos.</param>
        /// <returns>Regresa el archivo de resumen.</returns>
        public ActionResult TextFile(ModelErroresPuestos model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE PUESTOS DEL ARCHIVO: " + model.Path);
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
        /// Acción que descarga los puestos por cliente específico.
        /// </summary>
        /// <returns>Regresa un Json con la descarga.</returns>
        [HttpPost]
        public JsonResult Descarga()
        {
            int IdCliente = (int)Session["sIdCliente"];

            ClassPuestos ccc = new ClassPuestos();
            var cc = ccc.GetPuestosByIdCliente(IdCliente).Select(x => new { x.IdPuesto, x.Clave, x.Puesto });
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(cc);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}