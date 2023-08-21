using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class ConsultaMovimientosAfiliatoriosController : BaseController
    {
        // GET: ConsultaMovimientosAfiliatorios
        public ActionResult Index(string mensaje)
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        public JsonResult GetEmpledoByIMSS(string imss)
        {
            cMovimientosAfiliatorios cma = new cMovimientosAfiliatorios();
            var IdCliete = int.Parse(Session["sIdCliente"].ToString());
            var lista = cma.GetMovimientosByImss(imss, IdCliete);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAfilIndividual(string lote, string Imss)
        {
            try
            {
                cMovimientosAfiliatorios cma = new cMovimientosAfiliatorios();
                var info = cma.GetInfoAfilIndividual(lote, Imss);
                switch (info.Movimiento)
                {
                    case "Reingreso":
                        var p = cma.GetPdfAlta(info);
                        return File(p, ".pdf");
                        
                    case "Baja":
                        var p2 = cma.GetPdfBaja(info);
                        return File(p2, ".pdf");
                        
                    case "Modificación":
                        var p3 = cma.GetPdfModificacion(info);
                        return File(p3, ".pdf");
                       
                    default:
                        return RedirectToAction("Index");
                        
                }          
            }
            catch(Exception ex)
            {
                string mnje = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mnje });
            }
        }

        public ActionResult DescargaAcuseIndividual(string lote, string origen)
        {
            try
            {
                byte[] bytes = null;
                string ruta = string.Empty;
                string tipoArch = string.Empty;
                switch (origen)
                {
                    case "HISTORICO":
                        ruta = "D:\\SistemaTada\\LotesIMSS\\Historial\\" + lote + ".pdf";
                        bytes = System.IO.File.ReadAllBytes(ruta);
                        tipoArch = ".pdf";
                        break;
                    case "TADA":
                        ruta = "D:\\SistemaTada\\LotesIMSS\\Nuevos\\" + lote + ".zip";
                        bytes = System.IO.File.ReadAllBytes(ruta);
                        tipoArch = ".zip";
                        break;

                }
                return File(bytes, "*"+tipoArch, "Acuse_Lote_" + lote + tipoArch);
            }
            catch
            {
                string mens = "No se encontro el archivo";
                return RedirectToAction("Index", new { mensaje = mens });
            }
        }

        public ActionResult DescargaMasivaAcuses(string IMSS)
        {
            try
            {
                var IdCliete = int.Parse(Session["sIdCliente"].ToString());
                cMovimientosAfiliatorios cma = new cMovimientosAfiliatorios();
                var listado = cma.GetListalotes(IMSS, IdCliete);
                string mensaje = cma.GetZip(listado, IMSS);
                if (mensaje == string.Empty)
                {
                    string pathZip = @"D:\SistemaTada\LotesIMSS\" + IMSS + ".zip";
                    byte[] fileByte = System.IO.File.ReadAllBytes(pathZip);
                    return File(fileByte, System.Net.Mime.MediaTypeNames.Application.Octet, IMSS + ".zip");
                }
                else
                {
                    return RedirectToAction("Index", new { mensaje = mensaje });
                }
            }
            catch(Exception ex)
            {
                string msj = ex.Message;
                return RedirectToAction("Index", new { mensaje = msj });
            }
        }

        public ActionResult DescargaAfilesIndividuales(string IMSS)
        {
            cMovimientosAfiliatorios cma = new cMovimientosAfiliatorios();
            string path = cma.CreatePDFs(IMSS);
            cma.CreateExcel(IMSS);
            string pathZip = @"D:\SistemaTada\LotesIMSS\AfilesIndividuals" + IMSS + ".zip";
            if (System.IO.File.Exists(pathZip))
            {
                System.IO.File.Delete(pathZip);
                ZipFile.CreateFromDirectory(path, pathZip);
            }
            else
            {
                ZipFile.CreateFromDirectory(path, pathZip);
            }
            byte[] fileByte = System.IO.File.ReadAllBytes(pathZip);
            return File(fileByte, System.Net.Mime.MediaTypeNames.Application.Octet, "AfilesIndividuals" + IMSS + ".zip");
        }

        public ActionResult DescargaPDFRespuestaGeneral(int IdRegistroPatronal, string Lote)
        {
            try
            {
                cConsultas cc = new cConsultas();
                var zip = cc.GetPDFRespuestaGeneral(IdRegistroPatronal, Lote);
                return File(zip, "*.zip", "Constancia de presentación de movimientos afiliatorios Lote_" + Lote + ".zip");
            }
            catch(Exception ex)
            {
                string msj = "Error proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = msj });
            }
        }
    }
}