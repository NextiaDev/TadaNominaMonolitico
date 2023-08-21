using Delva.AppCode.TimbradoTurboPAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ClassCore.TimbradoTP.CFDI40;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Controllers.CFDI
{
    public class CancelaTimbradoTPController : BaseController
    {
        // GET: CancelaTimbradoTP
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
            ClassCancelarTimbrado cperiodo = new ClassCancelarTimbrado();
            cCancelar cc = new cCancelar();
            ModelCancelarTimbrado model = cperiodo.GetModeloTimbradoNomina(IdUnidadNegocio);
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                Guid Id = Guid.NewGuid();
                string empleados = string.Empty;
                string[] _empleados = { };
                try 
                { 
                    empleados = m.ClavesEmpleado.Trim();
                    _empleados = empleados.Replace(" ", "").Split(',');
                } catch { }
                
                if (_empleados.Count() > 0)
                    cc.CancelarPeriodoNominaClaves(m.IdPeriodoNomina, Id, IdUnidadNegocio, _empleados, m.motivoCancelacion, IdUsuario);                
                else
                    cc.CancelarPeriodoNomina(m.IdPeriodoNomina, Id, IdUnidadNegocio, m.motivoCancelacion, IdUsuario);

                var timbrados = cperiodo.GetCancelados(m.IdPeriodoNomina);
                var errores = cperiodo.GetErrores(m.IdPeriodoNomina, Id);

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