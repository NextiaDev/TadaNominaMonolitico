using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class ImpuestoSatController : BaseController
    {
        // GET: ImpuestosSat
        /// <summary>
        /// Acción que lista los impuestos del SAT.
        /// </summary>
        /// <returns>Regresa la vista con la lista de los impuestos del SAT.</returns>
        public ActionResult Index()
        {
            int IdCliente = 0;
            int IdUnidadNegocio = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }

            if (IdCliente == 0 || IdUnidadNegocio == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
                ModelImpuestoSat model = clsImpuestoSat.LlenaListaTipoNomina();
                ViewBag.SeleccionarTipoNomina = model.LTipoNomina;
                return View(clsImpuestoSat.GetImpuestoSat());
            }
        }

        /// <summary>
        /// Acción que lista los impuestos del SAT con un tipo nómina específico.
        /// </summary>
        /// <param name="IdTipoNomina">Recibe el identificador del tipo nómina.</param>
        /// <returns>Regresa la vista con la lista de impuestos.</returns>
        [HttpPost]
        public ActionResult Index(int IdTipoNomina)
        {
            ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
            ModelImpuestoSat model = clsImpuestoSat.LlenaListaTipoNomina();
            ViewBag.SeleccionarTipoNomina = model.LTipoNomina;
            return View(clsImpuestoSat.GetImpuestoSat(IdTipoNomina)); 
        }

        /// <summary>
        /// Acción que genera la vista con el detalle del impuesto del SAT.
        /// </summary>
        /// <param name="id">Recibe el identificador del impuesto.</param>
        /// <returns>Regresa la vista con el detalle del impuesto del SAT</returns>
        public ActionResult Details(int id)
        {
            ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
            return PartialView(clsImpuestoSat.GetModelImpuestoSat(id));
        }

        /// <summary>
        /// Acciónn que agrega un impuesto nuevo.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de impuestos.</returns>
        public ActionResult Create()
        {

            ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
            ModelImpuestoSat model = clsImpuestoSat.LlenaListaTipoNomina();
            return View(model);

        }

        /// <summary>
        /// Acción que guarda el impuesto nuevo.
        /// </summary>
        /// <param name="collection">Recibe el modelo de impuestos.</param>
        /// <returns>Regresa la vista con los impuestos del SAT.</returns>
        [HttpPost]
        public ActionResult Create(ModelImpuestoSat collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
                    clsImpuestoSat.AddImpuestoSat(collection);
                    ModelImpuestoSat model = clsImpuestoSat.LlenaListaTipoNomina();
                    return RedirectToAction("Index", "ImpuestoSat");
                }
                else
                {
                    ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
                    ModelImpuestoSat model = clsImpuestoSat.LlenaListaTipoNomina();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que edita un impuesto específico.
        /// </summary>
        /// <param name="id">Recibe el identificador del impuesto.</param>
        /// <returns>Regresa la vista con el modelo de impuestos.</returns>
        public ActionResult Edit(int id)
        {
            ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
            ModelImpuestoSat registro = clsImpuestoSat.GetModelImpuestoSat(id);
            ModelImpuestoSat modelo = clsImpuestoSat.LlenaListaTipoNomina(registro);
            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda el impuesto modificado.
        /// </summary>
        /// <param name="id">Recibe el identificador del impuesto.</param>
        /// <param name="collection">Recibe el modelo de impuestos del SAT.</param>
        /// <returns>Regresa la vista con los impuestos del SAT.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelImpuestoSat collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
                    clsImpuestoSat.UpdateImpuestoSat(collection, id);
                    return RedirectToAction("Index");
                }
                else
                {
                    ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
                    ModelImpuestoSat model = clsImpuestoSat.LlenaListaTipoNomina();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina un impuesto.
        /// </summary>
        /// <param name="id">Recibe el identificador del impuesto.</param>
        /// <returns>Regresa la vista con la confirmación de la eliminación del impuesto.</returns>
        public ActionResult Delete(int id)
        {
            ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
            return PartialView(clsImpuestoSat.GetModelImpuestoSat(id));

        }

        /// <summary>
        /// Acción que confirma la eliminación del impuesto.
        /// </summary>
        /// <param name="id">Recibe el identificador del impuesto.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista con los impuestos del SAT.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
                clsImpuestoSat.DeleteImpuestoSAT(id);
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
        /// <returns>Regresa la vista con el modelo de los impuestos del SAT.</returns>
        public ActionResult CreateLayout()
        {
            ModelImpuestoSat modelo = new ModelImpuestoSat();
            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda el archivo para carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo de impuestos del SAT.</param>
        /// <returns>Regresa el archivo.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelImpuestoSat model)
        {
            Debug.WriteLine("ModelState not Valid");
            ClassImpuestoSat clsImpuestoSat = new ClassImpuestoSat();
            ModelImpuestoSat modelo = new ModelImpuestoSat();
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Valid");
                if (model.Archivo.ContentLength > 0)
                {
                    Debug.WriteLine("Archivo");
                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);

                    ModelErroresImpuestoSat errores = clsImpuestoSat.GetImpuestoSatArchivo(_path);
                    return TextFile(errores);
                }
            }
            return View(modelo);
        }

        /// <summary>
        /// Acción que genera el archivo con el resumen de la carga masiva.
        /// </summary>
        /// <param name="model">Recibe el modelo de errores de impuestos del SAT.</param>
        /// <returns>Regresa el archivo de resumen.</returns>
        public ActionResult TextFile(ModelErroresImpuestoSat model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE LAYOUT DEL ARCHIVO: " + model.Path);
            tw.WriteLine("");
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("Numero de Registros Leidos: " + model.noRegistro);
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