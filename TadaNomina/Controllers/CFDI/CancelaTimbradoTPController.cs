using Delva.AppCode.TimbradoTurboPAC;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
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
            int IdCliente = (int)Session["sIdCliente"];
            ClassCancelarTimbrado cperiodo = new ClassCancelarTimbrado();
            cCancelar cc = new cCancelar();
            ModelCancelarTimbrado model = cperiodo.GetModeloTimbradoNomina(IdUnidadNegocio);
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
            var cliente = cun.getClienteById(IdCliente);

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

                if (cliente.IdPAC == 1)
                {
                    if (_empleados.Count() > 0)
                        cc.CancelarPeriodoNominaClaves(m.IdPeriodoNomina, Id, IdUnidadNegocio, _empleados, m.motivoCancelacion, IdUsuario);
                    else
                        cc.CancelarPeriodoNomina(m.IdPeriodoNomina, Id, IdUnidadNegocio, m.motivoCancelacion, IdUsuario);
                }

                if (cliente.IdPAC == 2)
                {
                    cperiodo.CancelaPeriodoNomina(m.IdPeriodoNomina, "02", _empleados, IdUsuario, Id);
                }

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

        [HttpPost]
        public JsonResult CancelaRelacionados(int? IdPeriodo)
        {
            try
            {
                if (IdPeriodo == null || IdPeriodo == 0) {throw new Exception("Debe elegir el periodo a cancelar."); }

                int IdUsuario = (int)Session["sIdUsuario"];
                cCancelar cc = new cCancelar();
                cc.cancelaCFDISRelacionadosPrevios((int)IdPeriodo, IdUsuario);

                ClassCancelarTimbrado cperiodo = new ClassCancelarTimbrado();
                var timbradosCancelados = cperiodo.GetCancelados((int)IdPeriodo);
                var timbrados = cperiodo.GetvTimbrados((int)IdPeriodo);
                string mensaje = "Timbres cancelados en el periodo: " + timbradosCancelados.Count + ", Timbres Activos en el periodo: " + timbrados.Count;
                return Json(new { result = "Ok", mensaje });
               
                
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", mensaje = ex.Message });
            }
            
        }
    }    
}
