using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    /// <summary>
    /// Controlador del Actualización de Netos.
    /// Autor: Carlos Alavez
    /// Fecha Ultima Modificación: 23/05/2022, Razón: documentar el codigo
    /// </summary>
    public class ActualizaNetosController : BaseController
    {
        /// <summary>
        /// Acción que carga los datos necesarios a la vista para el proceso de actualización de netos.
        /// </summary>
        /// <returns>Resultados a la vista.</returns>
        public ActionResult Index()
        {
            ModelActualizaNetos model = new ModelActualizaNetos();

            return View(model);
        }

        /// <summary>
        /// Acción resultante del proceso de actualiza netos.
        /// </summary>
        /// <param name="model">Datos capturados por el usuario.</param>
        /// <returns>Resultados del proceso a la vista.</returns>
        [HttpPost]
        public ActionResult Index(ModelActualizaNetos model)
        {
            if (ModelState.IsValid)
            {
                string FileName = Path.GetFileName(model.Archivo.FileName);
                string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), FileName);
                model.Archivo.SaveAs(_path);

                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

                ClassActualizaNetos cnetos = new ClassActualizaNetos();
                try 
                {
                    cnetos.ActualizaNetos(_path, IdUnidadNegocio);
                    model.validacion = true;
                    model.Mensaje = "Los netos se actualizaron de forma correcta.";
                }
                catch(Exception ex)
                {
                    model.validacion = false;
                    model.Mensaje = "No se pudieron actualizar los netos: " + ex.Message;
                }

            }

            return View(model);
        }
    }
}