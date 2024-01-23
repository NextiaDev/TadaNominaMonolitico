using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services.CFDI40;
using TadaNomina.Services.CFDI40.Models;

namespace TadaNomina.Controllers.Administracion
{
    public class RegistroPatronalController :  BaseController
    {
        // GET: RegistroPatronal
        /// <summary>
        /// Acción que muestra los registros patronales.
        /// </summary>
        /// <returns>Regresa la vista con los registros patronales.</returns>
        public ActionResult Index()
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
                ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
                ListadoDeBancos();
                var h = clsRegistroPatronal.GetRegistroPatronalByIdCliente(IdCliente);
                return View(h);
            }
        }

        /// <summary>
        /// Acción para generar un registro patronal.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de registros patronales.</returns>
        public ActionResult Create()
        {
            int IdCliente = (int)Session["sIdCliente"];
            ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
            ModelRegistroPatronal model = clsRegistroPatronal.LlenaListaActividadEconomica();
            model.RegimenesFiscales = clsRegistroPatronal.GetRegimenesToSelect();
            ListadoDeBancos();
            ViewBag.LA = model.LActividadEconomica;
            return View(model);
        }

        /// <summary>
        /// Acción que guarda el registro patronal.
        /// </summary>
        /// <param name="collection">Recibe el modelo de registro patronal.</param>
        /// <returns>Regresa la vista con los registros patronales.</returns>
        [HttpPost]
        public ActionResult Create(ModelRegistroPatronal collection)
        {
            ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
            try
            {
                int IdCliente = (int)Session["sIdCliente"];
                int idUsuario = (int)Session["sIdUsuario"];

                if (ModelState.IsValid)
                {
                    clsRegistroPatronal.AddRegistroPatronal(collection, idUsuario, IdCliente);
                   
                    return RedirectToAction("Index", "RegistroPatronal");
                }
                else
                    throw new Exception("Falta datos por capturar");                
            }
            catch(Exception ex)
            {                
                var model = clsRegistroPatronal.LlenaListaActividadEconomica();
                model.RegimenesFiscales = clsRegistroPatronal.GetRegimenesToSelect();
                ListadoDeBancos();
                ViewBag.LA = model.LActividadEconomica;
                model.validacion = false;
                model.Mensaje = "El Registro Patronal no se pudo insertar!" + ex.Message;
                return View(model);
            }
        }

        /// <summary>
        /// Acción que genera la vista del detalle patronal específico.
        /// </summary>
        /// <param name="id">Recibe el identificador del registro patronal.</param>
        /// <returns>Regresa la vista con el detalle del tegistro patronal.</returns>
        public ActionResult Details(int id)
        {
            ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
            return PartialView(clsRegistroPatronal.GetModelRegistroPatronal(id));
        }

        /// <summary>
        /// Acción que modifica un registro patronal.
        /// </summary>
        /// <param name="id">Recibe el identificador del registro patronal.</param>
        /// <returns>Regresa la vista con el modelo de registro patronal.</returns>
        public ActionResult Edit(int id)
        {

            int IdCliente = (int)Session["sIdCliente"];
            ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();

            ModelRegistroPatronal registro = clsRegistroPatronal.GetModelRegistroPatronal(id);
            ModelRegistroPatronal modelo = clsRegistroPatronal.LlenaListaActividadEconomica(registro);
            modelo.RegimenesFiscales = clsRegistroPatronal.GetRegimenesToSelect();
            ListadoDeBancos();
            return View(modelo);
        }

        /// <summary>
        /// Acción que guarda la modificación del registro patronal.
        /// </summary>
        /// <param name="id">Recibe el identificador del registro patronal.</param>
        /// <param name="collection">Recibe el modelo de registro patronal.</param>
        /// <returns>Regresa la vista con los registros patronales.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelRegistroPatronal collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
                    int idusuario = (int)Session["sIdUsuario"];

                    clsRegistroPatronal.UpdateRegistroPatronal(collection, id, idusuario);

                    return RedirectToAction("Index");
                }
                else
                {
                    ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
                    return View(clsRegistroPatronal.GetModelRegistroPatronal(id));
                }
            }
            catch (Exception )
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que elimina un registro patronal específico.
        /// </summary>
        /// <param name="id">Recibe el identificador del registro patronal.</param>
        /// <returns>Regresa la vista con los registros patronales.</returns>
        public ActionResult Delete(int id)
        {
            ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
            return PartialView(clsRegistroPatronal.GetModelRegistroPatronal(id));
        }

        /// <summary>
        /// Acción que confirma la eliminación del registro patronal.
        /// </summary>
        /// <param name="id">Recibe el identificador del registro patronal.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista con los registros patronales.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                int idUsuario = (int)Session["sIdUsuario"];
                ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
                clsRegistroPatronal.DeleteRegistroPatrona(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción que lista los bancos.
        /// </summary>
        public void ListadorDeBancosIndex()
        {
            ClassBancos _bancos = new ClassBancos();
            ViewBag.ListadorDeBancos = _bancos.getBancos();
        }

        /// <summary>
        /// Acción que muestra los bancos por medio de un cambo box.
        /// </summary>
        public void ListadoDeBancos()
        {
            List<SelectListItem> _listaB = new List<SelectListItem>();

            ClassBancos _bancos = new ClassBancos();

            foreach (var item in _bancos._LBancos())
            {
                _listaB.Add(new SelectListItem
                {
                    Value = item.IdBanco.ToString(),
                    Text = item.NombreCorto,
                });
                ViewBag.ListaNombreBanco = _listaB;
            }
        }

        /// <summary>
        /// Acción para registrar la empresa patrona ante el PAC
        /// </summary>
        /// <param name="IdRegistroPatronal"></param>
        /// <returns>La respuesta de la API</returns>
        [HttpPost]
        public JsonResult registrarPatrona(int IdRegistroPatronal)
        {
            try
            {
                ClassRegistroPatronal crp = new ClassRegistroPatronal();
                var ddatosRegistro = crp.GetRegistroPatronalById(IdRegistroPatronal);

                sGetToken sgt = new sGetToken();
                var token = sgt.sGetAcceso();

                sEmisores se = new sEmisores();
                mNewEmisor mne = new mNewEmisor() 
                {
                    RFCEmisor = ddatosRegistro.RFC,
                    RazonSocial = ddatosRegistro.NombrePatrona,
                    RegimenFiscal = "601",
                    Calle = ddatosRegistro.Calle,
                    Colonia = ddatosRegistro.Colonia,
                    DelegacionMunicipio = ddatosRegistro.Municipio,
                    LugarExpedicion = ddatosRegistro.CP,
                    Estado = ddatosRegistro.EntidadFederativa,
                    Telefono = ddatosRegistro.Telefono,
                    EmailEmisor = ddatosRegistro.CorreoElectronico
                };

                var resultado = se.sAltaEmisor(mne, token.Access_Token);

                return Json(new { result = "Ok", resultado });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Acción para actualizar los datos de la empresa patrona ante el PAC
        /// </summary>
        /// <param name="IdRegistroPatronal"></param>
        /// <returns>El resultado de la API</returns>
        [HttpPost]
        public JsonResult actualizarPatrona(int IdRegistroPatronal)
        {
            try
            {
                ClassRegistroPatronal crp = new ClassRegistroPatronal();
                var ddatosRegistro = crp.GetRegistroPatronalById(IdRegistroPatronal);

                sGetToken sgt = new sGetToken();
                var token = sgt.sGetAcceso();

                sEmisores se = new sEmisores();
                mNewEmisor mne = new mNewEmisor()
                {
                    RFCEmisor = ddatosRegistro.RFC,
                    RazonSocial = ddatosRegistro.NombrePatrona,
                    RegimenFiscal = "601",
                    Calle = ddatosRegistro.Calle,
                    Colonia = ddatosRegistro.Colonia,
                    DelegacionMunicipio = ddatosRegistro.Municipio,
                    LugarExpedicion = ddatosRegistro.CP,
                    Estado = ddatosRegistro.EntidadFederativa,
                    Telefono = ddatosRegistro.Telefono,
                    EmailEmisor = ddatosRegistro.CorreoElectronico
                };

                var resultado = se.sEditarEmisor(mne, token.Access_Token);

                return Json(new { result = "Ok", resultado });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Error", mensaje = ex.Message });
            }
        }

    }
}