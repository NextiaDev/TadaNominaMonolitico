using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ClassCore.TimbradoTP;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Services;

namespace TadaNomina.Controllers.CFDI
{
    public class TimbradoTPController : BaseController
    {
        // GET: TimbradoTP
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassTimbradoNomina cperiodo = new ClassTimbradoNomina();
            ModelTimbradoNomina model = cperiodo.GetModeloTimbradoNomina(IdUnidadNegocio);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ModelTimbradoNomina timbrado)
        {
            int IdCliente = (int)Session["sIdCliente"];
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            int IdUsuario = (int)Session["sIdUsuario"];

            ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
            var cliente = cun.getClienteById(IdCliente);

            ClassTimbradoNomina cperiodo = new ClassTimbradoNomina();
            ModelTimbradoNomina model = cperiodo.GetModeloTimbradoNomina(IdUnidadNegocio);

            timbrado.lPeriodos = model.lPeriodos;

            try
            {
                Guid Id = Guid.NewGuid();

                if (cliente.IdPAC == 1)
                {
                    ClassTimbrado ct = new ClassTimbrado();
                    ct.TimbradoNomina(timbrado.IdPeriodoNomina, IdUnidadNegocio, IdCliente, Id, IdUsuario);
                }

                if (cliente.IdPAC == 2)
                {
                    cperiodo.TimbradoPeriodoNomina(timbrado.IdPeriodoNomina, IdUnidadNegocio, IdCliente, Id, IdUsuario);
                }

                var timbrados = cperiodo.GetTimbrados(timbrado.IdPeriodoNomina);
                var errores = cperiodo.GetErrores(timbrado.IdPeriodoNomina, Id);

                timbrado.errores = errores;

                timbrado.validacion = true;
                timbrado.Mensaje = "El timbrado se realizo con los siguientes resultados: Total Timbrados; " + timbrados.Count + ", Errores: " + errores.Count;
            }
            catch (Exception ex)
            {
                timbrado.validacion = false;
                timbrado.Mensaje = "No se pudo timbrar, error: " + ex.Message;
            }

            return View(timbrado);
        }
    }
}