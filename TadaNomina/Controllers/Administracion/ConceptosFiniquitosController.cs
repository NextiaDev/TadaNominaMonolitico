using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class ConceptosFiniquitosController : BaseController
    {
        // GET: ConceptosFiniquitos
        /// <summary>
        /// Acción que lista los conceptos de finiquitos.
        /// </summary>
        /// <returns>Regresa la vista con la lista de conceptos de finiquitos.</returns>
        /// <exception cref="Exception">ENvía mensaje de error.</exception>
        public ActionResult Index()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch(Exception ex) { throw new Exception(ex.Message, ex); }
            ClassConceptosFiniquitos cconceptos = new ClassConceptosFiniquitos();
            ModelConceptosFiniquito model = cconceptos.LLenaListasConceptosFinquitos(IdCliente);
            
            return View(model);
        }

        /// <summary>
        /// Acción que guarda el concepto de finiquito.
        /// </summary>
        /// <param name="model">Recibe el modelo del concepto de finiquito.</param>
        /// <returns>Regresa la vista con el mensaje del resultado de guardar el concepto del finiquito.</returns>
        /// <exception cref="Exception">Representa los errores que se producen durante la ejecución de una aplicación.</exception>
        [HttpPost]
        public ActionResult Index(ModelConceptosFiniquito model)
        {
            int IdCliente = 0;
            int IdUsuario = 0;
            try { IdCliente = (int)Session["sIdCliente"]; IdUsuario = (int)Session["sIdUsuario"]; } catch (Exception ex) { throw new Exception(ex.Message, ex); }

            ClassConceptosFiniquitos cconceptos = new ClassConceptosFiniquitos();
            
            try
            {
                cconceptos.GuardaConfiguracionConceptoFiniquito(model, IdUsuario);
                model = cconceptos.LLenaListasConceptosFinquitos(IdCliente);
                
                model.validacion = true;
                model.Mensaje = "Los datos se guardaron de forma correcta!";
            }
            catch (Exception ex)
            {
                model = cconceptos.LLenaListasConceptosFinquitos(IdCliente);
                model.validacion = false;
                model.Mensaje = "No se pudo guardar la información, Error: " + ex;
            }

            
            return View(model);
        }
    }
}
