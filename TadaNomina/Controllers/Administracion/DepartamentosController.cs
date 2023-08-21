using System;
using System.Collections;
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
    public class DepartamentosController : BaseController
    {
        // GET: Departamentos
        /// <summary>
        /// Acción que muestra la lista de los departamentos.
        /// </summary>
        /// <returns>Regresa la vista con el listado de los departamentos.</returns>
        public ActionResult Index()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }

            if (IdCliente==0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ClassDepartamentos clsDepartamentos = new ClassDepartamentos();                
                return View(clsDepartamentos.GetDepartamentosByIdCliente(IdCliente));
            }            
        }

        // GET: Departamentos/Details/5
        /// <summary>
        /// Acción que genera la vista del detalle del departamento.
        /// </summary>
        /// <param name="id">Recibe el identificador del departamento.</param>
        /// <returns>Regresa la vista con el detalle del departamento.</returns>
        public ActionResult Details(int id)
        {
            ClassDepartamentos classDepartamentos = new ClassDepartamentos();
            return PartialView(classDepartamentos.GetModelDepartamentos(id));
        }

        // GET: Departamentos/Create
        /// <summary>
        /// Acción que genera la vista para agregar un departamento.
        /// </summary>
        /// <returns>Regresa la vista con el modelo del departamento.</returns>
        public ActionResult Create()
        {
            ModelDepartamentos model = new ModelDepartamentos();
            return View(model);
        }

        // POST: Departamentos/Create
        /// <summary>
        /// Acción que guarda el departamento.
        /// </summary>
        /// <param name="collection">Recibe el modelo del departamento.</param>
        /// <returns>Regresa la vista con el listado de los departamentos.</returns>
        [HttpPost]
        public ActionResult Create(ModelDepartamentos collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idCliente = (int)Session["sIdCliente"];
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassDepartamentos classDepartamentos = new ClassDepartamentos();
                    classDepartamentos.AddDepartamento(collection, idCliente, idUsuario);

                    return RedirectToAction("Index", "Departamentos");
                }
                else
                {
                    ModelDepartamentos model = new ModelDepartamentos();
                    return View(model);
                }                
            }
            catch
            {
                return View();
            }
        }

        // GET: Departamentos/Edit/5
        /// <summary>
        /// Acción que genera la vista para modificar el departamento.
        /// </summary>
        /// <param name="id">Recibe el identificador del departamento.</param>
        /// <returns>Regresa la vista con la información del departamento.</returns>
        public ActionResult Edit(int id)
        {
            ClassDepartamentos classDepartamentos = new ClassDepartamentos();
            return View(classDepartamentos.GetModelDepartamentos(id));
        }

        // POST: Departamentos/Edit/5
        /// <summary>
        /// Acción que guarda la modificación del departamento. 
        /// </summary>
        /// <param name="id">Recibe el identificador del departamento.</param>
        /// <param name="collection">Recibe el modelo del departamento</param>
        /// <returns>Regresa la vista con el listado de los departamentos.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelDepartamentos collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassDepartamentos classDepartamentos = new ClassDepartamentos();
                    int idusuario = (int)Session["sIdUsuario"];
                    classDepartamentos.UpdateDepartamento(collection, id, idusuario);

                    return RedirectToAction("Index");
                }
                else
                {
                    ClassDepartamentos classDepartamentos = new ClassDepartamentos();
                    return View(classDepartamentos.GetModelDepartamentos(id));
                }                
            }
            catch
            {
                return View();
            }
        }

        // GET: Departamentos/Delete/5
        /// <summary>
        /// Acción que genera la vista para eliminar el departamento.
        /// </summary>
        /// <param name="id">Recibe el identificador del departamento.</param>
        /// <returns>Regresa la vista con la confirmación para eliminar el departamento.</returns>
        public ActionResult Delete(int id)
        {
            ClassDepartamentos classDepartamentos = new ClassDepartamentos();

            var model = classDepartamentos.ValidaDepto(id);
            return PartialView(model);            
        }

        // POST: Departamentos/Delete/5
        /// <summary>
        /// Acción que ejecuta la eliminación del departamento.
        /// </summary>
        /// <param name="id">Recibe el identificador del departamento.</param>
        /// <param name="collection">Contiene los proveedores de valor de formulario para la aplicación.</param>
        /// <returns>Regresa la vista con el listado de los departamentos.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                int idUsuario = (int)Session["sIdUsuario"];
                ClassDepartamentos classDepartamentos = new ClassDepartamentos();
                classDepartamentos.DeleteDepartamento(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que genera la vista para hacer carga de departamentos por layout.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de carga por layout de los departamentos.</returns>
        public ActionResult CreateLayout()
        {

            ModelDepartamentos modelo = new ModelDepartamentos();
            return View(modelo);
        }

        /// <summary>
        /// Acción para subir el archivo con los departamentos.
        /// </summary>
        /// <param name="model">Recibe el modelo de departamentos.</param>
        /// <returns>Regresa la vista con un resumen de la información del archivo de departamentos.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelDepartamentos model)
        {
            //Debug.WriteLine("NOMBRE DE RUTA");
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassDepartamentos cdepartamentos = new ClassDepartamentos();
            ModelDepartamentos modelo = new ModelDepartamentos();

            
            //Debug.WriteLine("NOMBRE DE RUTA");
            if (model.Archivo.ContentLength > 0)
            {

                string fileName = Path.GetFileName(model.Archivo.FileName);
                string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                model.Archivo.SaveAs(_path);
                
                ModelErroresDepartamentos errores = cdepartamentos.GetDepartamentos(_path, IdCliente, IdUsuario);

                return TextFile(errores);
            }
           
            return View(modelo);
        }

        /// <summary>
        /// Archivo que guarda el archivo con los departamentos.
        /// </summary>
        /// <param name="model">Recibe el modelo de errores de los departamentos.</param>
        /// <returns>Regresa la vista con la información sobre los registros de los departamentos.</returns>
        public ActionResult TextFile(ModelErroresDepartamentos model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE DEPARTAMENTOS DEL ARCHIVO: " + model.Path);
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
        /// Acción que descarga el archivo con el formato de los departamentos.
        /// </summary>
        /// <returns>Regresa el archivo descargado.</returns>
        [HttpPost]
        public JsonResult Descarga()
        {
            int IdCliente = (int)Session["sIdCliente"];

            ClassDepartamentos ccc = new ClassDepartamentos();
            var cc = ccc.GetDepartamentosByIdCliente(IdCliente).Select(x => new { x.IdDepartamento, x.Clave, x.Departamento });
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(cc);

            return Json(json, JsonRequestBehavior.AllowGet);
        }

    }
}
