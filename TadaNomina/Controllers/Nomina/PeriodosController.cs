using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.CalculoFiniquito;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PeriodosController : BaseController
    {    
        public ActionResult Index()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model =  cperiodo.GetModelPeriodoNominas(IdUnidadNegocio);
                return View(model);
            }
        }

        public ActionResult Create()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio);
            return View(model);
        }

        /// <summary>
        /// Método para crear un periodo de nómina en la base de datos
        /// </summary>
        /// <param name="collection">Modelo de la vista</param>
        /// <returns>Redirección a la vista de periodos de nómina</returns>
        [HttpPost]
        public ActionResult Create(ModelPeriodoNomina collection)
        {
            try // Iniciamos el control de excepciones
            {
                if (ModelState.IsValid) // Validamos si el modelo es válido
                {
                    ClassPeriodoNomina cperiodos = new ClassPeriodoNomina(); // Instanciamos la clase de periodos de nómina
                    int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; // Obtenemos el identificador de la unidad de negocio del usuario
                    int IdCliente_PTU = (int)Session["sIdCliente"]; // Obtenemos el identificador del cliente PTU del usuario
                    int IdUsuario = (int)Session["sIdUsuario"]; // Obtenemos el identificador del usuario
                    string token = (string)Session["sToken"]; // Obtenemos el token del usuario
                    collection.IdUnidadNegocio = IdUnidadNegocio; // Asignamos el identificador de la unidad de negocio al modelo
                    collection.IdCliente_PTU = IdCliente_PTU; // Asignamos el identificador del cliente PTU al modelo
                    if (DateTime.Parse(collection.FechaFin) < DateTime.Parse(collection.FechaInicio)) { throw new Exception("Las fecha final no puede ser menor a la fecha inicial."); } // Validamos que la fecha final no sea menor a la fecha inicial

                    // Validamos si existen pendientes por timbrar
                    if (cperiodos.ValidaTimbrado(IdUnidadNegocio) > 0)
                    {
                        ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio); // Obtenemos los periodos de nómina de la unidad de negocio del usuario
                        model.Validacion = false; // Asignamos el valor de falso a la validación del modelo de la vista de periodos de nómina 
                        model.Mensaje = "Existen periodos de nómina con recibos pendientes por timbrar, favor de validar!"; // Asignamos el mensaje de error al modelo de la vista de periodos de nómina 
                        return View(model); // Retornamos la vista de periodos de nómina con el modelo de la vista de periodos de nómina
                    }

                    cperiodos.AddPeriodoNomina(collection, token); // Agregamos el periodo de nómina a la base de datos con el modelo de la vista de periodos de nómina y el token del usuario

                    return RedirectToAction("Index", "Periodos"); // Redireccionamos a la vista de periodos de nómina
                }
                else // Si el modelo no es válido
                {
                    int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; // Obtenemos el identificador de la unidad de negocio del usuario 
                    ClassPeriodoNomina cperiodos = new ClassPeriodoNomina(); // Instanciamos la clase de periodos de nómina
                    ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio); // Obtenemos los periodos de nómina de la unidad de negocio del usuario
                    model.Validacion = false; // Asignamos el valor de falso a la validación del modelo de la vista de periodos de nómina
                    model.Mensaje = "Faltan datos por capturar, favor de validar!"; // Asignamos el mensaje de error al modelo de la vista de periodos de nómina
                    return View(model); // Retornamos la vista de periodos de nómina con el modelo de la vista de periodos de nómina
                }
            }
            catch (Exception ex) // Capturamos la excepción
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; // Obtenemos el identificador de la unidad de negocio del usuario
                ClassPeriodoNomina cperiodos = new ClassPeriodoNomina(); // Instanciamos la clase de periodos de nómina 
                ModelPeriodoNomina model = cperiodos.FindListPeriodos(IdUnidadNegocio); // Obtenemos los periodos de nómina de la unidad de negocio del usuario

<<<<<<< HEAD
                model.Validacion = false; // Asignamos el valor de falso a la validación del modelo de la vista de periodos de nómina
                model.Mensaje = "Error: " + ex.Message; // Asignamos el mensaje de error al modelo de la vista de periodos de nómina
                return View(model); // Retornamos la vista de periodos de nómina con el modelo de la vista de periodos de nómina
=======
                model.Validacion = false;
                model.Mensaje = "ERROR: " + ex.Message;
                return View(model);
>>>>>>> master
            }
        }

        public ActionResult Edit(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModelPeriodoNomina collection)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            try
            {                
                if (ModelState.IsValid)
                {                    
                    int IdUsuario = (int)Session["sIdUsuario"];                    
                    int IdCliente_PTU = (int)Session["sIdCliente"];                    
                    cperiodos.EditPeriodoNomina(collection, IdUsuario);

                    return RedirectToAction("Index", "Periodos");
                }
                else
                {
                    int IdUnidad = (int)Session["sIdUnidadNegocio"];
                    ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(collection.IdPeriodoNomina);
                    modelo.IdsPeriodosAjuste.Replace("0", "");
                    ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);                    
                    return View(model);
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(collection.IdPeriodoNomina);
                ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);
                return View(model);
            }
        }
       
        [HttpPost]
        public JsonResult Delete(int IdPeriodoNomina)
        {
            try
            {
                ClassPeriodoNomina periodo = new ClassPeriodoNomina();
                int IdUsuario = (int)Session["sIdUsuario"];
                periodo.DeletePeriodoNomina(IdPeriodoNomina, IdUsuario);

                ClassProcesosFiniquitos cpf = new ClassProcesosFiniquitos();
                cpf.DeleteAllConfiguracionFiniquitos(IdPeriodoNomina);

                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detail(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AcumularNomina(int IdPeriodoNomina, DateTime FechaDispersion)
        {
            try
            {                
                ClassPeriodoNomina periodo = new ClassPeriodoNomina();
                ClassFonacot fonacot = new ClassFonacot();
                int IdUsuario = (int)Session["sIdUsuario"];
                int IdUnidad = (int)Session["sIdUnidadNegocio"];
                var detalles = periodo.getDatosNominaByIdPeriodo(IdPeriodoNomina);
                periodo.AcumularPeriodoNomina(IdPeriodoNomina, FechaDispersion, IdUsuario);
                fonacot.consolidaIncidenciasFonacot(IdPeriodoNomina);

                ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
                var uni = cun.getUnidadesnegocioId(IdUnidad);

                if (uni.NotificarAcumular == "S" && uni.CorreosNotificacion != null && uni.CorreosNotificacion.Length > 0)
                {
                    ClassNotificaciones cn = new ClassNotificaciones();
                    ModelPeriodoNomina modelo = periodo.GetModelPeriodoNominasId(IdPeriodoNomina);
                    var correos = uni.CorreosNotificacion.Replace(" ", "").Trim().Split(',').ToArray();
                                        
                    string html = "<html><body>¡Atencion! Se Acumulo el Periodo de Nómina: <h4> " + modelo.Periodo +
                        "</h4> <br /><br /> <table border='1'><tr><th>Unidad</th><th>Periodo</th><th>F. Inicial</th><th>F. Final</th><th>F. Dispersión</th><th>ISR</th><th>Carga Obrera</th><th>Carga Patronal</th><th>Total Percepciones</th><th>Total Deducciones</th><th>Neto</th></tr>";
                    html += "<tr><td>" + modelo.UnidaNegocio + "</td><td>" + modelo.Periodo + "</td><td>" + modelo.FechaInicio + "</td><td>" + modelo.FechaFin + "</td><td>" + FechaDispersion.ToShortDateString() + 
                        "</td><td>" + detalles.Select(x => x.isr).Sum().Value.ToString("C") + "</td><td>" + detalles.Select(x => x.cargaObrera).Sum().Value.ToString("C") + 
                        "</td><td>" + detalles.Select(x => x.cargaPatronal).Sum().Value.ToString("C") + "</td><td>" + detalles.Select(x => x.totalPerc).Sum().Value.ToString("C") + 
                        "</td><td>" + detalles.Select(x => x.totalDed).Sum().Value.ToString("C") + "</td><td>" + detalles.Select(x => x.neto).Sum().Value.ToString("C") + "</td></tr>";
                    html += "</table></body></html>";

                    cn.EnviarCorreoTada(html, "Acumulación de Periodo", correos, "Nómina");
                }

                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Acumulados()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model = cperiodo.GetModelPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();
                return View(model);
            }
        }

        public ActionResult DesAcumularNomina(int Id)
        {
            ClassPeriodoNomina cperiodos = new ClassPeriodoNomina();
            int IdUnidad = (int)Session["sIdUnidadNegocio"];
            ModelPeriodoNomina modelo = cperiodos.GetModelPeriodoNominasId(Id);
            ModelPeriodoNomina model = cperiodos.FindListPeriodos(modelo, IdUnidad);

            return PartialView("DesAcumularNomina", model);
        }

        [HttpPost]
        public ActionResult DesAcumularNomina(ModelPeriodoNomina modelo, int Id)
        {
            try
            {
                ClassPeriodoNomina periodo = new ClassPeriodoNomina();
                ClassFonacot fonacot = new ClassFonacot();
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassTimbradoNomina ctimbrado = new ClassTimbradoNomina();
                int cantidad = ctimbrado.GetCantidadTimbresPeriodoNomina(Id);
                if (cantidad == 0)
                {
                    periodo.DesAcumularPeriodoNomina(Id, IdUsuario);
                    fonacot.desacumulaIncidenciasFonacot(Id);
                }
                
                return RedirectToAction("Acumulados");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public ActionResult NominasAcumuladas()
        {
            int IdUnidadNegocio = 0;
            try { IdUnidadNegocio = (int)Session["sIdUnidadNegocio"]; } catch { IdUnidadNegocio = 0; }
            if (IdUnidadNegocio == 0) { return RedirectToAction("Index", "Default"); }
            else
            {
                ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
                List<ModelPeriodoNomina> model = cperiodo.GetModelPeriodoNominasAcumuladas(IdUnidadNegocio);
                return View(model);
            }
        }
    }
}
