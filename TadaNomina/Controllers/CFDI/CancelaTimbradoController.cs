using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Controllers.CFDI
{
    public class CancelaTimbradoController : BaseController
    {
        // GET: CancelaTimbrado
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassCancelarTimbrado cperiodo = new ClassCancelarTimbrado();
            ModelCancelarTimbrado model = cperiodo.GetModeloTimbradoNomina(IdUnidadNegocio);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ModelCancelarTimbrado m)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassCancelarTimbrado ctimbrado = new ClassCancelarTimbrado();
            ModelCancelarTimbrado model = ctimbrado.GetModeloTimbradoNomina(IdUnidadNegocio);
            model.ClavesEmpleado = m.ClavesEmpleado;
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                Guid Id = Guid.NewGuid();
                string[] claves = null;
                try { claves = m.ClavesEmpleado.Replace(" ", "").Split(',').ToArray(); } catch { }
                
                if(claves != null)
                    ctimbrado.CancelacionPeriodoNominaClaves(m.IdPeriodoNomina, IdUsuario, Id, "Produccion", claves);
                else
                    ctimbrado.CancelacionPeriodoNomina(m.IdPeriodoNomina, IdUsuario, Id, "Produccion");

                var timbrados = ctimbrado.GetTimbrados(m.IdPeriodoNomina);
                var errores = ctimbrado.GetErrores(m.IdPeriodoNomina, Id);

                model.errores = errores;

                model.validacion = true;
                model.Mensaje = "La cancelacion del timbrado se realizo con los siguientes resultados: Total Cancelados; " + timbrados.Count + ", Errores: " + errores.Count;
            }
            catch (Exception ex)
            {
                model.validacion = false;
                model.Mensaje = "No se pudo cancelar el timbrado, error: " + ex.Message;
            }

            return View(model);
        }
    }
}