using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models;
using TadaNomina.Models.ClassCore;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Reportes
{
    public class ReporteGVController : BaseController
    {
        // GET: ReporteGV
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase Archivo)
        {
            try
            {
                int IdCliente = int.Parse(Session["sIdCliente"].ToString());
                int IdUnidadNegocio = int.Parse(Session["sIdUnidadNegocio"].ToString());
                string filePath = string.Empty;
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(Archivo.FileName);
                Archivo.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath, Encoding.UTF7);

                FileImportGV file = new FileImportGV();
                var f = file.ProcesoArchivo(csvData);
                var exel = file.Excel(f);
                return File(exel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UBICACIONES.xlsx");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}