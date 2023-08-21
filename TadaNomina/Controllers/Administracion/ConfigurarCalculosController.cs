using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class ConfigurarCalculosController : BaseController
    {
        // GET: ConfigurarCalculos
        /// <summary>
        /// Acción que muestra el modelo para configurar las fechas para los cálculos del sistema.
        /// </summary>
        /// <returns>Regresa la vista con el modelo para configurar las fechas de los cálculos.</returns>
        public ActionResult Index()
        {
            ClassFechasCalculos ccalculos = new ClassFechasCalculos();
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ModelConfiguracionFechasCalculos model = ccalculos.GetModel(IdUnidadNegocio);
            
            return View(model);
        }

        /// <summary>
        /// Acción que guarda el modelo que configura las fechas para los cálculos del sistema.
        /// </summary>
        /// <param name="model">Recibe el modelo de configuración de fechas de cálculos.</param>
        /// <returns>Regresa la vista con el resumen del resultado del guardado de la configuración de las fechas de los cálculos.</returns>
        [HttpPost]
        public ActionResult Index(ModelConfiguracionFechasCalculos model)
        {
            try
            {
                ClassFechasCalculos cf = new ClassFechasCalculos();
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                int IdUsuario = (int)Session["sIdUsuario"];

                cf.AgregaFechasCalculos(model, IdUnidadNegocio, IdUsuario);
                cf.GetListas(model);

                model.validacion = true;
                model.Mensaje = "La configuración de fechas se guardo de forma correcta.";
            }
            catch (Exception ex)
            {
                model.validacion = false;
                model.Mensaje = "La configuración de fechas NO se pudo guardar:" + ex.Message;
            }
            return View(model);
        }
    }
}