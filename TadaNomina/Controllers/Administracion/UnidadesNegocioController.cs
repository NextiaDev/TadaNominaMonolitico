using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Controllers.Administracion
{
    public class UnidadesNegocioController : BaseController
    {
        // GET: UnidadesNegocio
        /// <summary>
        /// Acción que muestra las unidades de negocio.
        /// </summary>
        /// <returns>Regresa la vista con las unidades de negocio.</returns>
        public ActionResult Index()
        {   
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { return RedirectToAction("Index", "Default"); }
                
            ClassUnidadesNegocio unidad = new ClassUnidadesNegocio();
            List<vUnidadesNegocio> lunidades = new List<vUnidadesNegocio>();
            lunidades = unidad.getUnidadesnegocio(IdCliente);

            return View(lunidades);
            
        }


        public ActionResult Config(int id)
        {
            Session["sAsignada"] = id;
            return View();
        }


        public JsonResult GetTipoInfoUnidad()
        {
            try
            {
                int IdUnidadNegocio = 0;
                try { IdUnidadNegocio = (int)Session["sAsignada"]; } catch { IdUnidadNegocio = 0; }

                var cs = new ClassUnidadesNegocio();
                var emp = cs.getUnidadesnegocioId(IdUnidadNegocio);
                return Json(emp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GuardarEspeciales(string SeptimoDia, string PrestacionesEntero, string CuotaSindical, string CargasSFaltas, string DiasEquiv, string CobroCops, string RetenciISRSMGV, string SubirArchivo, string GeneraIntegrado, string Isr74, string NCargaObrera, string NCargaPatronal, string FechaInicio, string FechaFin,  string CAA, string AEC, int? DImss, int? DImssB, string DaMas, string DaMenos, string DaMasF, string DaMenosF, string ISRM, string ISRC, string FFD, string FEC, string FS, string FTM)
        {

            int IdUnidadNegocio = 0;
            decimal isrms = 0;
            try { IdUnidadNegocio = (int)Session["sAsignada"]; } catch { IdUnidadNegocio = 0; }
            try { isrms = isrms = decimal.Parse(ISRC); } catch { isrms = 0; }
              ;
            try
            {
                if (ModelState.IsValid)
                {
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();
                    clsUnidad.UpdateUnidadNegocioEspeciales(IdUnidadNegocio, SeptimoDia, PrestacionesEntero, CuotaSindical, CargasSFaltas, DiasEquiv, CobroCops, RetenciISRSMGV, SubirArchivo, GeneraIntegrado, Isr74, NCargaObrera, NCargaPatronal, FechaInicio, FechaFin,  CAA, AEC, DImss, DImssB, DaMas, DaMenos, DaMasF, DaMenosF, ISRM, isrms, FFD, FEC, FS, FTM, idUsuario);

                    return Json("Exito", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Incorrecto", JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: UnidadesNegocio/Details/5
        /// <summary>
        /// Acción que muestra el detalle de una unidad de negocio.
        /// </summary>
        /// <param name="id">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa la vista con el detalle de la unidad negocio.</returns>
        public ActionResult Details(int id)
        {
            ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();

            List<SelectListItem> listTipoNomina = new List<SelectListItem>();
            List<SelectListItem> listConfiguracionSueldos = LlenaListConfiguracionSueldos();
            ModelUnidadesNegocio model = clsUnidad.GetModelUnidadesNegocioById(id);
            List<ModelTipoNomina> _tipoNomina = new List<ModelTipoNomina>();
            _tipoNomina = clsUnidad.getTipoNomina();

            _tipoNomina.ForEach(x => { listTipoNomina.Add(new SelectListItem { Text = x.TipoNomina, Value = x.IdTipoNomina.ToString() }); });

            model.TipoNomina = listTipoNomina;
            model.LConfiguracionSueldos = listConfiguracionSueldos;

            return PartialView(model);
        }

        // GET: UnidadesNegocio/Create
        /// <summary>
        /// Acción que genera una unidad de negocio nueva.
        /// </summary>
        /// <returns>Regresa la vista con el modelo de la unidad negocio.</returns>
        public ActionResult Create()
        {
            List<SelectListItem> listTipoNomina = new List<SelectListItem>();
            List<SelectListItem> listConfiguracionSueldos = LlenaListConfiguracionSueldos();
            ModelUnidadesNegocio model = new ModelUnidadesNegocio();
            ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();
            List<ModelTipoNomina> _tipoNomina = new List<ModelTipoNomina>();
            _tipoNomina = clsUnidad.getTipoNomina();

            _tipoNomina.ForEach(x => { listTipoNomina.Add(new SelectListItem { Text = x.TipoNomina, Value = x.IdTipoNomina.ToString() }); });

            model.TipoNomina = listTipoNomina;
            model.LConfiguracionSueldos = listConfiguracionSueldos;

            return View(model);
        }

        // POST: UnidadesNegocio/Create
        /// <summary>
        /// Acción que guarda la unidad negocio nueva.
        /// </summary>
        /// <param name="collection">Recibe el modelo de la unidad negocio.</param>
        /// <returns>Regresa la vista de las unidades de negocio.</returns>
        [HttpPost]
        public ActionResult Create(ModelUnidadesNegocio collection)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();
                    int idUsuario = (int)Session["sIdUsuario"];
                    int IdCliente = (int)Session["sIdCliente"];
                    clsUnidad.AddUnidadNegocio(collection, idUsuario, IdCliente); 
                    return RedirectToAction("Index");  
                }
                else
                {
                    List<SelectListItem> listTipoNomina = new List<SelectListItem>();
                    List<SelectListItem> listConfiguracionSueldos = LlenaListConfiguracionSueldos();
                    ModelUnidadesNegocio model = new ModelUnidadesNegocio();
                    ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();
                    List<ModelTipoNomina> _tipoNomina = new List<ModelTipoNomina>();
                    _tipoNomina = clsUnidad.getTipoNomina();

                    _tipoNomina.ForEach(x => { listTipoNomina.Add(new SelectListItem { Text = x.TipoNomina, Value = x.IdTipoNomina.ToString() }); });

                    model.TipoNomina = listTipoNomina;
                    model.LConfiguracionSueldos = listConfiguracionSueldos;

                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: UnidadesNegocio/Edit/5
        /// <summary>
        /// Acción que modifica la unidad de negocio.
        /// </summary>
        /// <param name="id">Recibe el identificador de la unidad de negocio.</param>
        /// <returns>Regresa la vista con el modelo de unidades de negocio.</returns>
        public ActionResult Edit(int id)
        {
            ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();

            List<SelectListItem> listTipoNomina = new List<SelectListItem>();
            List<SelectListItem> listConfiguracionSueldos = LlenaListConfiguracionSueldos();
            ModelUnidadesNegocio model = clsUnidad.GetModelUnidadesNegocioById(id);
            List<ModelTipoNomina> _tipoNomina = new List<ModelTipoNomina>();
            _tipoNomina = clsUnidad.getTipoNomina();
            _tipoNomina.ForEach(x => { listTipoNomina.Add(new SelectListItem { Text = x.TipoNomina, Value = x.IdTipoNomina.ToString() }); });
                        
            model.TipoNomina = listTipoNomina;
            model.LConfiguracionSueldos = listConfiguracionSueldos;
            return View(model);
        }

        // POST: UnidadesNegocio/Edit/5
        /// <summary>
        /// Acción que guarda la unidad de negocio modificada.
        /// </summary>
        /// <param name="id">Recibe el identificador de la unidad negocio.</param>
        /// <param name="collection">Recibe el modelo de la unidad negocio.</param>
        /// <returns>Regresa la vista con las unidades de negocio.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelUnidadesNegocio collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();
                    clsUnidad.UpdateUnidadNegocio(id, collection, idUsuario);

                    return RedirectToAction("Index");
                }
                else
                {
                    List<SelectListItem> listTipoNomina = new List<SelectListItem>();
                    List<SelectListItem> listConfiguracionSueldos = LlenaListConfiguracionSueldos();
                    ModelUnidadesNegocio model = new ModelUnidadesNegocio();
                    ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();
                    List<ModelTipoNomina> _tipoNomina = new List<ModelTipoNomina>();
                    _tipoNomina = clsUnidad.getTipoNomina();
                    _tipoNomina.ForEach(x => { listTipoNomina.Add(new SelectListItem { Text = x.TipoNomina, Value = x.IdTipoNomina.ToString() }); });

                    model.TipoNomina = listTipoNomina;

                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: UnidadesNegocio/Delete/5
        /// <summary>
        /// Acción que elimina una unidad de negocio.
        /// </summary>
        /// <param name="id">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa la vista con la unidad de negocio a eliminar.</returns>
        public ActionResult Delete(int id)
        {
            ClassUnidadesNegocio clsUnidad = new ClassUnidadesNegocio();

            List<SelectListItem> listTipoNomina = new List<SelectListItem>();
            ModelUnidadesNegocio model = clsUnidad.GetModelUnidadesNegocioById(id);
            List<ModelTipoNomina> _tipoNomina = new List<ModelTipoNomina>();
            _tipoNomina = clsUnidad.getTipoNomina();

            _tipoNomina.ForEach(x => { listTipoNomina.Add(new SelectListItem { Text = x.TipoNomina, Value = x.IdTipoNomina.ToString() }); });

            model.TipoNomina = listTipoNomina;
            return PartialView(model);
        }

        // POST: UnidadesNegocio/Delete/5
        /// <summary>
        /// Acción que confirma la eliminación de la unidad de negocio.
        /// </summary>
        /// <param name="id">Recibe el identificador de la unidad de negocio.</param>
        /// <param name="collection">Representa una colección de claves.</param>
        /// <returns>Regresa la vista de las unidades de negocio.</returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                int idUsuario = (int)Session["sIdUsuario"];
                ClassUnidadesNegocio clsUnidadNegocio = new ClassUnidadesNegocio();
                clsUnidadNegocio.DeleteUnidadNegocio(id, idUsuario);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        /// <summary>
        /// Acción que lista la configuración de sueldos.
        /// </summary>
        /// <returns>Regresa la vista con la lista de configuración de sueldos.</returns>
        private static List<SelectListItem> LlenaListConfiguracionSueldos()
        {
            List<SelectListItem> listConfiguracionSueldos = new List<SelectListItem>();
            listConfiguracionSueldos.Add(new SelectListItem { Text = "Brutos", Value = "Brutos" });
            listConfiguracionSueldos.Add(new SelectListItem { Text = "Neto a Pagar", Value = "NetosPagar" });
            listConfiguracionSueldos.Add(new SelectListItem { Text = "Netos(Real)", Value = "Netos(Real)" });
            listConfiguracionSueldos.Add(new SelectListItem { Text = "Netos(Impuestos)", Value = "Netos(Impuestos)" });            
            listConfiguracionSueldos.Add(new SelectListItem { Text = "Netos Tradicional(Piramida)", Value = "Netos Tradicional(Piramida)" });
            listConfiguracionSueldos.Add(new SelectListItem { Text = "Netos Tradicional(Piramida ART 93)", Value = "Netos Tradicional(Piramida ART 93)" });
            return listConfiguracionSueldos;
        }
    }
}
