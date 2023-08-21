using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.DB;
using System.IO;
using TadaNomina.Models;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class FactorIntegracionController : BaseController
    {
        /// <summary>
        /// Acción que genera el listado de prestaciones.
        /// </summary>
        /// <returns>Regresa la vista con las prestaciones.</returns>
        public ActionResult Prestaciones()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            if (IdCliente == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
                List<ModelPrestaciones> model = cfactor.GetModelPrestaciones(IdCliente);

                return View(model);
            }
        }

        /// <summary>
        /// Acción que genera una prestación nueva.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de prestaciones.</returns>
        public ActionResult NuevaPrestacion()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            if (IdCliente == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ModelPrestaciones mprestacion = new ModelPrestaciones();
                mprestacion.IdCliente = (int)Session["sIdCliente"];
                mprestacion.Cliente = Session["sCliente"].ToString();

                return View(mprestacion);
            }
        }

        /// <summary>
        /// Acción que guarda la prestación nueva.
        /// </summary>
        /// <param name="model">Recibe el modelo de prestaciones.</param>
        /// <returns>Regresa la vista con las prestaciones.</returns>
        [HttpPost]
        public ActionResult NuevaPrestacion(ModelPrestaciones model)
        {
            try
            {
                ModelPrestaciones mprestacion = new ModelPrestaciones();
                mprestacion.IdCliente = (int)Session["sIdCliente"];
                mprestacion.Cliente = Session["sCliente"].ToString();

                ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
                int IdUsuario = (int)Session["sIdUsuario"];
                model.IdCliente = mprestacion.IdCliente;
                cfactor.AddPrestacion(model, IdUsuario);

                mprestacion.validacion = true;
                mprestacion.Mensaje = "La nueva prestacion se guardo de forma correcta";

                return View(mprestacion);
            }
            catch (Exception ex)
            {
                ModelPrestaciones mprestacion = new ModelPrestaciones();
                mprestacion.IdCliente = (int)Session["sIdCliente"];
                mprestacion.Cliente = Session["sCliente"].ToString();
                mprestacion.validacion = false;
                mprestacion.Mensaje = "No se pudieron guardar los datos" + ex;

                return View();
            }
        }

        /// <summary>
        /// Acción que modifica una prestación específica.
        /// </summary>
        /// <param name="Id">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la vista con el modelo de prestaciones.</returns>
        public ActionResult EditPrestacion(int Id)
        {
            ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
            ModelPrestaciones mprestacion = cfactor.GetModelPrestacion(Id);

            return View(mprestacion);
        }

        /// <summary>
        /// Acción que guarda la edición de la prestación.
        /// </summary>
        /// <param name="model">Recibe el modelo de las prestaciones.</param>
        /// <returns>Regresa la vista con las prestaciones.</returns>
        [HttpPost]
        public ActionResult EditPrestacion(ModelPrestaciones model)
        {
            try
            {
                ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
                int IdUsuario = (int)Session["sIdUsuario"];
                cfactor.EditPrestacion(model, IdUsuario);

                model.validacion = true;
                model.Mensaje = "Los cambios se guardaron de forma correcta";

                return View(model);
            }
            catch (Exception)
            {
                return View();
            }

        }

        /// <summary>
        /// Acción que muestra el detalle de la prestación específica.
        /// </summary>
        /// <param name="Id">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la vista con el detalle de la prestación.</returns>
        public ActionResult DetailsPrestacion(int Id)
        {
            ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
            ModelPrestaciones mprestacion = cfactor.GetModelPrestacion(Id);
            return PartialView(mprestacion);
        }

        /// <summary>
        /// Acción para eliminar una prestación específica.
        /// </summary>
        /// <param name="Id">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la vista con el detalle de la prestación.</returns>
        public ActionResult DeletePrestacion(int Id)
        {
            ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
            ModelPrestaciones mprestacion = cfactor.GetModelPrestacion(Id);

            return PartialView(mprestacion);
        }

        /// <summary>
        /// Acción que elimina una prestación.
        /// </summary>
        /// <param name="model"Recibe el modelo de prestaciones.></param>
        /// <returns>Regresa la vista con las prestaciones.</returns>
        [HttpPost]
        public ActionResult DeletePrestacion(ModelPrestaciones model)
        {
            try
            {
                ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
                int IdUsuario = (int)Session["sIdUsuario"];
                cfactor.DeletePrestacion(model.IdPrestacion, IdUsuario);
                return RedirectToAction("Prestaciones", "FactorIntegracion", new { Id = model.IdPrestacion });
            }
            catch (Exception)
            {
                return RedirectToAction("Prestaciones", "FactorIntegracion", new { Id = model.IdPrestacion });
            }

        }

        /// <summary>
        /// Acción que muestra los factores de integración.
        /// </summary>
        /// <param name="id">Recibe el identificador del factor de integración.</param>
        /// <returns>Regresa la vista con los factores de integración.</returns>
        public ActionResult Index(int id)
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
                ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
                ViewBag.IdPrestaciones = id;
                var model = clsFactorIntegracion.GetFactorByIdUnidadNegocio(id);
                return View(model);
            }
        }

        /// <summary>
        /// Acción que genera el detalle del factor de integración.
        /// </summary>
        /// <param name="id">Recibe el identificador del factor de integración.</param>
        /// <returns>Regresa la vista con el detalle del factor de integración.</returns>
        public ActionResult Details(int id)
        {
            ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
            return PartialView(clsFactorIntegracion.GetModelFactorIntegracion(id));
        }

        /// <summary>
        /// Acción que genera un factor de integración.
        /// </summary>
        /// <param name="Id">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la vista con el modelo del factor de integración.</returns>
        public ActionResult Create(int Id)
        {
            ModelFactorIntegracion model = new ModelFactorIntegracion();
            model.IdPrestaciones = Id;
            return View(model);
        }

        /// <summary>
        /// Acción que guarda el factor de integración.
        /// </summary>
        /// <param name="collection">Recibe el modelo de factor de integración.</param>
        /// <returns>Regresa la vista con los factores de integración.</returns>
        [HttpPost]
        public ActionResult Create(ModelFactorIntegracion collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                    int idUsuario = (int)Session["sIdUsuario"];

                    ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
                    clsFactorIntegracion.AddFactorIntegracion(collection, idUsuario);

                    return RedirectToAction("Index", "FactorIntegracion", new { Id = collection.IdPrestaciones });
                }
                else
                {
                    ModelFactorIntegracion model = new ModelFactorIntegracion();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que modifica un factor de integración.
        /// </summary>
        /// <param name="id">Recibe el identificador del factor de integración.</param>
        /// <returns>Regresa la vista con el factor de integración específico.</returns>
        public ActionResult Edit(int id)
        {
            ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
            return View(clsFactorIntegracion.GetModelFactorIntegracion(id));

        }

        /// <summary>
        /// Acción que guarda la modificación del factor de integración.
        /// </summary>
        /// <param name="id">Recibe el identificador del factor de integración.</param>
        /// <param name="collection">Recibe el modelo de factor de integración.</param>
        /// <returns>Regresa la vista con los factores de integración.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelFactorIntegracion collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
                    int idusuario = (int)Session["sIdUsuario"];
                    clsFactorIntegracion.UpdateFactorIntegracion(collection, id, idusuario);

                    return RedirectToAction("Index", "FactorIntegracion", new { Id = collection.IdPrestaciones });
                }
                else
                {
                    Debug.WriteLine("ModelState not Valid");
                    ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
                    return View(clsFactorIntegracion.GetModelFactorIntegracion(id));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("debuger " + ex);
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina un factor de integración.
        /// </summary>
        /// <param name="id">Recibe el identificador del factor de integración.</param>
        /// <returns>Regresa la vista de la eliminación del factor de integración.</returns>
        public ActionResult Delete(int id)
        {
            ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
            return PartialView(clsFactorIntegracion.GetModelFactorIntegracion(id));

        }

        /// <summary>
        /// Acción que guarda la eliminación de un factor de integración.
        /// </summary>
        /// <param name="id">Recibe el identificador del factor de integración.</param>
        /// <param name="collection"></param>
        /// <returns>Regresa la vista con los factores de integración.</returns>
        [HttpPost]
        public ActionResult Delete(int id, ModelFactorIntegracion collection)
        {
            try
            {
                int idUsuario = (int)Session["sIdUsuario"];
                ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
                clsFactorIntegracion.DeleteFactorIntegracion(id, idUsuario);
            }
            catch
            {

            }

            return RedirectToAction("Index", "FactorIntegracion", new { Id = collection.IdPrestaciones });
        }

        /// <summary>
        /// Acción que genera archivo para carga masiva de registros.
        /// </summary>
        /// <param name="Id">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la vista con el modelo del factor de integración.</returns>
        public ActionResult CreateLayout(int Id)
        {
            ModelFactorIntegracion modelo = new ModelFactorIntegracion();
            modelo.IdPrestaciones = Id;
            ViewBag.IdPrestaciones = Id;
            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda el archivo de carga masiva de registros.
        /// </summary>
        /// <param name="model">Recibe el modelo de factor de integración.</param>
        /// <returns>Regresa la vista del modelo de factor de integración.</returns>
        [HttpPost]
        public ActionResult CreateLayout(ModelFactorIntegracion model)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassFactorIntegracion clsFactorIntegracion = new ClassFactorIntegracion();
            ModelFactorIntegracion modelo = new ModelFactorIntegracion();

            if (ModelState.IsValid)
            {
                if (model.Archivo.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);

                    ModelErroresFactorIntegracion errores = clsFactorIntegracion.GetFactorIntegracionArchivo(_path, model.IdPrestaciones, IdUnidad, IdUsuario);
                    return TextFile(errores);
                }
            }

            return View(modelo);
        }

        /// <summary>
        /// Acción que genera archivo con resumen de la carga masivo de registros.
        /// </summary>
        /// <param name="model">Recibe el modelo del factor de integración.</param>
        /// <returns>Regresa el archivo.</returns>
        public ActionResult TextFile(ModelErroresFactorIntegracion model)
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

        /// <summary>
        /// Acción que carga las prestaciones por ley.
        /// </summary>
        /// <param name="Id">Recibe el identificador de la prestación.</param>
        /// <returns>Regresa la vista con las prestaciones.</returns>
        public ActionResult PrestacionesLey(int Id)
        {
            try
            {
                ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
                int IdUsuario = (int)Session["sIdUsuario"];
                cfactor.CargarPrestacionesLey(Id, IdUsuario);
            }
            catch (Exception)
            {


            }

            return RedirectToAction("Index", "FactorIntegracion", new { id = Id });

        }

        /// <summary>
        /// Acción que elimina todo lo de prestación.
        /// </summary>
        /// <param name="Id">Acción que elimina todo lo de prestación. de la prestación.</param>
        /// <returns>Regresa la vista de los factores de integración.</returns>
        public ActionResult EliminarTOdo(int Id)
        {
            try
            {
                ClassFactorIntegracion cfactor = new ClassFactorIntegracion();
                int IdUsuario = (int)Session["sIdUsuario"];
                cfactor.DeletePrestaciones(Id);
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index", "FactorIntegracion", new { Id });
        }
    }
}