using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services;

namespace TadaNomina.Controllers.Indicadores
{
    public class ChartArrayBasicController : Controller
    {
        // GET: ChartArrayBasic
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetEmpleados()
        {
            List<object> chartData = new List<object>();

            chartData.Add(new object[] {
                "ShipCity", "TotalOrders"
            });

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        // GET: ChartArrayBasic/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChartArrayBasic/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChartArrayBasic/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ChartArrayBasic/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChartArrayBasic/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ChartArrayBasic/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChartArrayBasic/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
