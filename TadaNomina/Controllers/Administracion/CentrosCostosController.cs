using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class CentrosCostosController : BaseController
    {
        // GET: CentrosCostos
        /// <summary>
        /// Acción que muestra los centros de costos.
        /// </summary>
        /// <returns>Regresa la vista de los centros de costos.</returns>
        public ActionResult Index()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0;}

            if (IdCliente==0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ClassCentrosCostos clsCentrosCostos = new ClassCentrosCostos();
                return View(clsCentrosCostos.getCentrosCostosByIdCliente(IdCliente));
            }            
        }

        // GET: CentrosCostos/Details/5
        /// <summary>
        /// Acción que muestra el detalle del centro de costos.
        /// </summary>
        /// <param name="id">Recibe el identificador del centro de costos.</param>
        /// <returns>Regresa la vista parcial con el detalle del centro de costos específico.</returns>
        public ActionResult Details(int id)
        {
            ClassCentrosCostos clsCentroCostos = new ClassCentrosCostos();
            return PartialView(clsCentroCostos.GetModelCentroCostosById(id));
        }

        // GET: CentrosCostos/Create
        /// <summary>
        /// Acción que genera un centro de costos nuevo.
        /// </summary>
        /// <returns>Regresa la vista con el modelo del centro de costos.</returns>
        public ActionResult Create()
        {
            ModelCentroCostos centrocostos = new ModelCentroCostos();
            return View(centrocostos);
        }

        // POST: CentrosCostos/Create
        /// <summary>
        /// Acción que guarda un centro de costos nuevo.
        /// </summary>
        /// <param name="collection">Recibe el modelo del centro de costos.</param>
        /// <returns>Valida el modelo y regresa la vista con los centros de costos.</returns>
        [HttpPost]
        public ActionResult Create(ModelCentroCostos collection)
        {
            int idCliente = (int)Session["sIdCliente"];
            try
            {
                if (ModelState.IsValid)
                {
                    ClassCentrosCostos clsCentroCostos = new ClassCentrosCostos();
                    clsCentroCostos.AddCentroCostos(collection, idCliente, (int)Session["sIdUsuario"]);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelCentroCostos centrocostos = new ModelCentroCostos();
                    return View(centrocostos);
                }                
            }
            catch
            {
                return View();
            }
        }

        // GET: CentrosCostos/Edit/5
        /// <summary>
        /// Acción que modifica el centro de costos.
        /// </summary>
        /// <param name="id">Recibe el identificador del centro de costos.</param>
        /// <returns>Regresa la vista del centro de costos a modificar.</returns>
        public ActionResult Edit(int id)
        {
            ClassCentrosCostos clsCentroCostos = new ClassCentrosCostos();
            return View(clsCentroCostos.GetModelCentroCostosById(id));
        }

        // POST: CentrosCostos/Edit/5
        /// <summary>
        /// Acción para guardar el centro de costos a modificar.
        /// </summary>
        /// <param name="id">Recibe el identificador del centro de costos.</param>
        /// <param name="collection">Recibe el modelo del centro de costos.</param>
        /// <returns>Valida el modelo del centro de costos y regresa la vista del centro de costos.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelCentroCostos collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idusuario = (int)Session["sIdUsuario"];
                    ClassCentrosCostos clsCentroCostos = new ClassCentrosCostos();
                    clsCentroCostos.UpdateCentroCostos(id, idusuario, collection);

                    return RedirectToAction("Index");
                }
                else
                {
                    ClassCentrosCostos clsCentroCostos = new ClassCentrosCostos();
                    return View(clsCentroCostos.GetModelCentroCostosById(id));
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: CentrosCostos/Delete/5
        /// <summary>
        /// Acción para eliminar el registro del centro de costos.
        /// </summary>
        /// <param name="id">Recibe el identificador del centro de costos.</param>
        /// <returns>Regresa una vista parcial de la eliminación del registro del centro de costos.</returns>
        public ActionResult Delete(int id)
        {
            ClassCentrosCostos clsCentroCostos = new ClassCentrosCostos();
            var model = clsCentroCostos.ValidaCC(id);
            return PartialView(model);
        }

        // POST: CentrosCostos/Delete/5
        /// <summary>
        /// Acción que elimina el registro del centro de costos.
        /// </summary>
        /// <param name="id">Recibe el identificador del centro de costos.</param>
        /// <param name="collection">Contiene los proveedores de valor de formulario para la aplicación.</param>
        /// <returns>Regresa la vista del centro de costos.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                ClassCentrosCostos clsCC = new ClassCentrosCostos();
                int idUsuario = (int)Session["sIdUsuario"];
                clsCC.DeleteCentroCostos(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción para generar un archivo y subir varios registros de centros de costos.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de centro de costos.</returns>
        public ActionResult CreateLayout()
        {
            //Debug.WriteLine("Nombre de Ruta");
            ModelCentroCostos modelo = new ModelCentroCostos();
            return View(modelo);           
        }

        /// <summary>
        /// Acción para guardar el archivo con los registros de centros de costos.
        /// </summary>
        /// <param name="model">Recibe el modelo del centro de costos.</param>
        /// <returns>Valida el modelo y regresa el resumen con información de los centros de costos.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelCentroCostos model)
        {
            Debug.WriteLine("NOMBRE DE RUTAhchcvhchcv");
            int IdCliente = (int)Session["sIdCliente"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassCentrosCostos cc = new ClassCentrosCostos();
            ModelCentroCostos modelo = new ModelCentroCostos();

            if (ModelState.IsValid)
            {
                //Debug.WriteLine("NOMBRE DE RUTA");
                if (model.Archivo.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);
                    //Debug.WriteLine("NOMBRE DE RUTA" + _path.ToString() );
                    ModelErroresCentroCostos errores = cc.GetCentrosCostos(_path, IdCliente, IdUsuario);

                    return TextFile(errores);
                }
            }

            return View(modelo);
        }

        /// <summary>
        /// Acción para generar un archivo con el informe de los registros que se subieron al sistema.
        /// </summary>
        /// <param name="model">Recibe el modelo de errores del centro de costos.</param>
        /// <returns>Regresa el archivo generado.</returns>
        public ActionResult TextFile(ModelErroresCentroCostos model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE CENTROS DE COSTOS DEL ARCHIVO: " + model.Path);
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
        /// Acción para descargar el archivo de los centros de costos.
        /// </summary>
        /// <returns>Regresa el archivo de los centros de costos.</returns>
        [HttpPost]
        public JsonResult Descarga()
        {
            int IdCliente = (int)Session["sIdCliente"];

            ClassCentrosCostos ccc = new ClassCentrosCostos();
            var cc = ccc.getCentrosCostosByIdCliente(IdCliente).Select(x => new { x.IdCentroCostos, x.Clave, x.CentroCostos });
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(cc);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}
