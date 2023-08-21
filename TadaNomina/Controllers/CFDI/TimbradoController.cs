using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.CFDI
{
    public class TimbradoController : BaseController
    {
        // GET: Timbrado
        public ActionResult Index() 
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassTimbradoNomina cperiodo = new ClassTimbradoNomina();
            ModelTimbradoNomina model = cperiodo.GetModeloTimbradoNomina(IdUnidadNegocio);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(int IdPeriodoNomina)
        {   
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassTimbradoNomina ctimbrado = new ClassTimbradoNomina();
            ModelTimbradoNomina model = ctimbrado.GetModeloTimbradoNomina(IdUnidadNegocio);
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                Guid Id = Guid.NewGuid();
                ctimbrado.TimbradoNomina(IdPeriodoNomina, Id, IdUsuario);

                var timbrados = ctimbrado.GetTimbrados(IdPeriodoNomina);
                var errores = ctimbrado.GetErrores(IdPeriodoNomina, Id);

                model.errores = errores;
                
                model.validacion = true;
                model.Mensaje = "El timbrado se realizo con los siguientes resultados: Total Timbrados; " + timbrados.Count + ", Errores: " + errores.Count;
            }
            catch (Exception ex)
            {
                model.validacion = false;
                model.Mensaje = "No se pudo timbrar, error: " + ex.Message;
            }

            return View(model);
        }
    }
}
