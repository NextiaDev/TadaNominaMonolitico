﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class InfonavitController : BaseController
    {
        // GET: Infonavit
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassInfonavit ci = new ClassInfonavit();
            var model = ci.getModelInfonavit(IdUnidadNegocio);
            return View(model);
        }

        public ActionResult Create()
        {
            ClassInfonavit ci = new ClassInfonavit();
            var model = new ModelInfonavit();
            model.lTipo = ci.getTipos();
            model.Activo = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelInfonavit model)
        {
            ClassInfonavit ci = new ClassInfonavit();
            model.lTipo = ci.getTipos();
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                if (model.IdEmpleado != null)
                {
                    ci.newCredito(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información del credito!";
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
            ClassInfonavit ci = new ClassInfonavit();
            var model = ci.getModelInfonavitId(id);
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            ClassInfonavit ci = new ClassInfonavit();
            var model = ci.getModelInfonavitId(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(ModelInfonavit model)
        {
            try
            {
                ClassInfonavit ci = new ClassInfonavit();
                ci.DeleteCredito(model.IdCreditoInfonavit, (int)Session["sIdUsuario"]);
                ci.DeleteIncidenciasCreditoVivienda(model.IdCreditoInfonavit, (int)Session["sIdUsuario"]);


            }
            catch { }

            return RedirectToAction("Index");
        }

        public ActionResult UpdatePorcentaje(int id)
        {
            ClassInfonavit ci = new ClassInfonavit();
            var model = ci.getModelInfonavitId(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdatePorcentaje(ModelInfonavit model)
        {
            try
            {
                ClassInfonavit ci = new ClassInfonavit();
                ci.UpdatePorcentaje(model.IdCreditoInfonavit, (int)Session["sIdUsuario"], model.PorcentajeTradicional, model.CantidadUnidad, model.BanderaSeguroVivienda, model.Activo);
            }
            catch { }

            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Método que modifica el estado del crédito, en caso de que sea inactivo no se considerará en el cálculo de la nómina
        /// </summary>
        /// <param name="IdCredito">Id del crédito</param>
        /// <returns>estatus del movimiento</returns>
        [HttpPost]
        public JsonResult CambiaStatusCredito(int IdCredito)
        {
            try
            {
                ClassInfonavit cI = new ClassInfonavit();
                int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());
                var res = cI.CambiaEstatus(IdCredito, IdUsuario);
                if(res == 1)
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                    return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Método que desactiva todos los créditos INFONAVIT
        /// </summary>
        /// <returns>Respuesta del movimiento</returns>
        [HttpPost]
        public JsonResult Desactivacreditos(int tipoMov)
        {
            try
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassInfonavit cI = new ClassInfonavit();
                var res = cI.Desactivacreditos(IdUnidadNegocio, IdUsuario, tipoMov);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }
    }
}