using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ClassCore.TimbradoTP.CFDI40;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Services.CFDI40;

namespace TadaNomina.Controllers.CFDI
{
    public class CargaCertificadosController : BaseController
    {
        // GET: CargaCertificados
        public ActionResult Index()
        {
            int IdCliente = (int)Session["sIdCliente"];
            ClassRegistroPatronal clsRegistroPatronal = new ClassRegistroPatronal();
            return View(clsRegistroPatronal.GetRegistroPatronalByIdCliente(IdCliente));            
        }

        public ActionResult CargarSellos(int Id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargarSellos(int Id, ModelCSD m)
        {
            if (m.ArchivoCer.ContentLength > 0 &&  m.ArchivoKey.ContentLength > 0)
            {
                string rutaPrincipal = @"D:\SistemaTada\Sellos\" + Id;
                if (!Directory.Exists(rutaPrincipal))
                {
                    Directory.CreateDirectory(rutaPrincipal);
                }

                string rutaCer = rutaPrincipal + @"\CSD.cer";
                m.ArchivoCer.SaveAs(rutaCer);

                string rutaKey = rutaPrincipal + @"\CSD.key";
                m.ArchivoKey.SaveAs(rutaKey);

                cCancelar cc = new cCancelar();
                string rutaPfx = rutaPrincipal + @"\cert.pfx";
                cc.creaPfx(rutaCer, rutaKey, rutaPfx, m.pass);

                ClassRegistroPatronal crp = new ClassRegistroPatronal();
                crp.updateRutasCerKey(Id, rutaCer, rutaKey, rutaPfx, m.pass);
            }

            return RedirectToAction("Index");
        }

        public JsonResult enviarCSDalPAC(int IdRegistroPatronal)
        {
            try
            {
                ClassRegistroPatronal crp = new ClassRegistroPatronal();
                var regPat = crp.GetRegistroPatronalById(IdRegistroPatronal);

                sGetToken sgt = new sGetToken();
                var acceso = sgt.sGetAcceso();
                var cerCSD = ObtenArchivoB64(regPat.rutaCer);
                var keyCSD = ObtenArchivoB64(regPat.rutaKey);

                sEmisores se = new sEmisores();
                var resultado = se.sCargaCertificado(regPat.RFC, cerCSD, keyCSD, regPat.KeyPass, acceso.Access_Token);

                return Json(new { result = "Ok" });
            }
            catch (Exception ex)
            {
;               return Json(new { result = "error", mensaje = ex.Message });
            }
        }        

        public ActionResult CargarFIEL(int Id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargarFIEL(int Id, ModelCSD m)
        {
            if (m.ArchivoCer.ContentLength > 0 && m.ArchivoKey.ContentLength > 0)
            {
                string rutaPrincipal = @"D:\SistemaTada\FIEL\" + Id;
                if (!Directory.Exists(rutaPrincipal))
                {
                    Directory.CreateDirectory(rutaPrincipal);
                }

                string rutaCer = rutaPrincipal + @"\FIEL.cer";
                m.ArchivoCer.SaveAs(rutaCer);

                string rutaKey = rutaPrincipal + @"\FIEL.key";
                m.ArchivoKey.SaveAs(rutaKey);
                               

                ClassRegistroPatronal crp = new ClassRegistroPatronal();
                crp.updateRutasCerKeyFIEL(Id, rutaCer, rutaKey, m.pass);
            }

            return RedirectToAction("Index");
        }

        public JsonResult enviarFIELalPAC(int IdRegistroPatronal)
        {
            try
            {
                ClassRegistroPatronal crp = new ClassRegistroPatronal();
                var regPat = crp.GetRegistroPatronalById(IdRegistroPatronal);

                sGetToken sgt = new sGetToken();
                var acceso = sgt.sGetAcceso();
                var cerFiel = ObtenArchivoB64(regPat.rutaFielCer);
                var keyFiel = ObtenArchivoB64(regPat.rutaFielKey);

                sEmisores se = new sEmisores();
                var resultado = se.sCargarCartaConsentimiento(regPat.RFC, regPat.NombreRepresentante, cerFiel, keyFiel, regPat.KeyPassFiel, acceso.Access_Token);

                return Json(new { result = "Ok" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", mensaje = ex.Message });
            }
        }

        private static string ObtenArchivoB64(string ruta)
        {
            string b64Result = string.Empty;
            try
            {
                var cerfiel = System.IO.File.ReadAllBytes(ruta);
                b64Result = WebUtility.UrlEncode( Convert.ToBase64String(cerfiel));

                return b64Result;
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo cargar el archivo: " + ruta + ", " + ex.Message);
            }
        }
    }
}