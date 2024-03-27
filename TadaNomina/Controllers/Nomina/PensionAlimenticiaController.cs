﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class PensionAlimenticiaController : BaseController
    {
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticia(IdUnidadNegocio);
            return View(model);
        }

        public ActionResult BaseAlimenticia()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }

            if (IdCliente == 0)
            {
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ClassPensionAlimenticia clsPension = new ClassPensionAlimenticia();
                return View(clsPension.getBasePensiones(IdCliente));
            }
        }

        public ActionResult CreateBasePension()
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            return View(ci.FindListConceptos(IdCliente));
        }


        [HttpPost]
        public ActionResult CreateBasePension(ModelBasePensionAlimenticia model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idCliente = (int)Session["sIdCliente"];
                    int idUsuario = (int)Session["sIdUsuario"];
                    ClassPensionAlimenticia classs = new ClassPensionAlimenticia();
                    classs.AddBasePension(model, idCliente, idUsuario);
                    return RedirectToAction("BaseAlimenticia", "PensionAlimenticia");
                }
                else
                {
                    int IdCliente = 0;
                    try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
                    ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
                    return View(ci.FindListConceptos(IdCliente));
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditBase(int id)
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            ModelBasePensionAlimenticia modelo = ci.getModelBasePensionAlimenticiaId(id);
            ModelBasePensionAlimenticia modelos = ci.FindListConceptosbase(IdCliente, modelo);
            return View(modelos);
        }




        [HttpPost]
        public ActionResult EditBase(ModelBasePensionAlimenticia model)
        {
            int IdCliente = 0;
            try { IdCliente = (int)Session["sIdCliente"]; } catch { IdCliente = 0; }
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                if (model != null)
                {
                    ci.editPensionBase(model, IdUsuario);
                    return RedirectToAction("BaseAlimenticia", "PensionAlimenticia");

                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        public JsonResult getPalabrasReservadas()
        {
            try
            {
                ClassConceptos cc = new ClassConceptos();
                var conceptos = cc.getConceptosFormulacion((int)Session["sIdCliente"]);
                return Json(conceptos);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public ActionResult Create()
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = new ModelPensionAlimenticia();
            model.lTipo = ci.getTipos();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticiaId(id);
            model.lTipo = ci.getTipos();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelPensionAlimenticia model)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            model.lTipo = ci.getTipos();
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                if (model != null)
                {
                    ci.createPension(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información de la pensión!";
                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModelPensionAlimenticia model)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                if (model != null)
                {
                    ci.editPension(model, IdUsuario);
                    model.lTipo = ci.getTipos();
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información de la pensión!";
                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        public JsonResult BuscaEmpleado(string clave)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado cemp = new ClassEmpleado();
            var emp = cemp.GetEmpleadosByClave(clave, IdUnidadNegocio).FirstOrDefault();

            if (emp != null)
                return Json(emp, JsonRequestBehavior.AllowGet);
            else
                return Json("El Empleado con la clave que ingreso no existe!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticiaId(id);
            return PartialView(model);
        }

        public ActionResult Detailsbase(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelBasePensionAlimenticiaId(id);
            return PartialView(model);
        }

        public ActionResult DeleteBase(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelBasePensionAlimenticiaId(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult DeleteBase(ModelBasePensionAlimenticia model)
        {
            try
            {
                ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
                ci.DeletePensionBase(model.IdBasePensionAlimenticia, (int)Session["sIdUsuario"]);
            }
            catch { }

            return RedirectToAction("BaseAlimenticia");
        }



        public ActionResult Delete(int id)
        {
            ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
            var model = ci.getModelPensionAlimenticiaId(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Delete(ModelPensionAlimenticia model)
        {
            try
            {
                ClassPensionAlimenticia ci = new ClassPensionAlimenticia();
                ci.DeletePension(model.IdPensionAlimenticia, (int)Session["sIdUsuario"]);
                ci.DeleteIncidenciasPension(model.IdPensionAlimenticia, (int)Session["sIdUsuario"]);
            }
            catch { }

            return RedirectToAction("Index");
        }
    }
}