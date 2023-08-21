using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class SubsidioEmpleoSatController : BaseController
    {
        // GET: SubsidioEmpleoSat
        /// <summary>
        /// Acción que muestra los subsidios.
        /// </summary>
        /// <returns>Regresa la vista de los subsidios.</returns>
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
                ClassSubsidioEmpleoSat clsSubsidio = new ClassSubsidioEmpleoSat();
                return View(clsSubsidio.GetSubsidioEmpleoSat(IdCliente));
            }
        }

        /// <summary>
        /// Acción que muestra los subsidios por tipo de nómina.
        /// </summary>
        /// <param name="IdTipoNomina">Recibe el identificador del tipo de nómina.</param>
        /// <returns>Regresa la vista con los subsidios.</returns>
        [HttpPost]
        public ActionResult Index(int IdTipoNomina)
        {
            ClassSubsidioEmpleoSat clsSubsidio = new ClassSubsidioEmpleoSat();
            ModelSubsidioEmpleoSat model = clsSubsidio.LlenaListaTipoNomina();
            ViewBag.SeleccionarTipoNomina = model.LTipoNomina;
            return View(clsSubsidio.GetSubsidioEmpleoSat(IdTipoNomina));
        }

        /// <summary>
        /// Acción que muestra el detalle del subsidio.
        /// </summary>
        /// <param name="id">Recibe el identificador del subsidio.</param>
        /// <returns>Regresa la vista con el detalle del subsidio.</returns>
        public ActionResult Details(int id)
        {
            ClassSubsidioEmpleoSat classSubsidio = new ClassSubsidioEmpleoSat();
            return PartialView(classSubsidio.GetModelSubsidioEmpleoSat(id));
        }

        /// <summary>
        /// Acción que genera un subsidio nuevo.
        /// </summary>
        /// <returns>Regresa la vista con el modelo del subsidio.</returns>
        public ActionResult Create()
        {
            ModelSubsidioEmpleoSat model = new ModelSubsidioEmpleoSat();
            return View(model);
        }

        /// <summary>
        /// Acción que guarda el subsidio nuevo.
        /// </summary>
        /// <param name="collection">Recibe el modelo del subsidio.</param>
        /// <returns>Regresa la vista de los subsidios.</returns>
        [HttpPost]
        public ActionResult Create(ModelSubsidioEmpleoSat collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idCliente = (int)Session["sIdCliente"];
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassSubsidioEmpleoSat classSubsidio = new ClassSubsidioEmpleoSat();
                    classSubsidio.AddSubsidioEmpleoSat(collection, idCliente, idUsuario);

                    return RedirectToAction("Index", "SubsidioEmpleoSat");
                }
                else
                {
                    ModelSubsidioEmpleoSat model = new ModelSubsidioEmpleoSat();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que modifica el subsidio.
        /// </summary>
        /// <param name="id">Recibe el identificador del subsidio.</param>
        /// <returns>Regresa la vista del subsidio a modificar.</returns>
        public ActionResult Edit(int id)
        {
            ClassSubsidioEmpleoSat classSubsidio = new ClassSubsidioEmpleoSat();
            return View(classSubsidio.GetModelSubsidioEmpleoSat(id));

        }

        /// <summary>
        /// Acción que guarda la modificación del subsidio.
        /// </summary>
        /// <param name="id">Recibe el identificador del subsidio.</param>
        /// <param name="collection">Recibe el modelo del subsidio.</param>
        /// <returns>Regresa la vista de los subsidios.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelSubsidioEmpleoSat collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassSubsidioEmpleoSat classSubsidio = new ClassSubsidioEmpleoSat();
                    int idusuario = (int)Session["sIdUsuario"];
                    classSubsidio.UpdateSubsidioEmpleoSat(collection, id, idusuario);

                    return RedirectToAction("Index");
                }
                else
                {
                    ClassSubsidioEmpleoSat classSubsidio = new ClassSubsidioEmpleoSat();
                    return View(classSubsidio.GetModelSubsidioEmpleoSat(id));
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina un subsidio.
        /// </summary>
        /// <param name="id">Recibe el identificador del subsidio.</param>
        /// <returns>Regresa la vista con el subsidio a eliminar.</returns>
        public ActionResult Delete(int id)
        {
            ClassSubsidioEmpleoSat classSubsidio = new ClassSubsidioEmpleoSat();
            return PartialView(classSubsidio.GetModelSubsidioEmpleoSat(id));

        }

        /// <summary>
        /// Acción que guarda la eliminación del subsidio.
        /// </summary>
        /// <param name="id">Recibe el identificador del subsidio.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista con los subsidios.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                //ClassSucursales classSucursales = new ClassSucursales();
                int idUsuario = (int)Session["sIdUsuario"];
                ClassSubsidioEmpleoSat classSucursales = new ClassSubsidioEmpleoSat();
                classSucursales.DeleteSubsidioEmpleoSat(id, idUsuario);

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
        /// <returns>Regresa la vista con el modelo del subsidio.</returns>
        public ActionResult CreateLayout()
        {

            ModelSubsidioEmpleoSat modelo = new ModelSubsidioEmpleoSat();
            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda el archivo de carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo del subsidio.</param>
        /// <returns>Regresa la vista del modelo de subsidio.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelSubsidioEmpleoSat model)
        {
            //Debug.WriteLine("NOMBRE DE RUTA");
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassSubsidioEmpleoSat csubsi = new ClassSubsidioEmpleoSat();
            ModelSubsidioEmpleoSat modelo = new ModelSubsidioEmpleoSat();

            if (ModelState.IsValid)
            {
                //Debug.WriteLine("NOMBRE DE RUTA");
                if (model.Archivo.ContentLength > 0)
                {

                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);
                    //Debug.WriteLine("NOMBRE DE RUTA" + _path.ToString() );
                    ModelErroresSubsidioEmpleoSat errores = csubsi.GetSubsidio(_path, IdCliente, IdUsuario);

                    return TextFile(errores);
                }
            }

            return View(modelo);
        }

        /// <summary>
        /// Acción que genera el archivo de resumen del subsidio.
        /// </summary>
        /// <param name="model">Regresa el modelo de errores de subsidio.</param>
        /// <returns>Regresa el archivo de resumen.</returns>
        public ActionResult TextFile(ModelErroresSubsidioEmpleoSat model)
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
    }

}