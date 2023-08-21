using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class CargaCertificadosIMSSController : BaseController
    {
        // GET: CargaCertificados
        public ActionResult Index()
        {
            cCargaCertificados ccc = new cCargaCertificados();
            var IdCliete = int.Parse(Session["sIdCliente"].ToString());
            var listado = ccc.GetRegistroPatronalByIdCliente(IdCliete);
            return View(listado);
        }

        public ActionResult CargaCerIMSS(int IdReg)
        {
            mCredencialesMovimientosIMSS mcm = new mCredencialesMovimientosIMSS();
            mcm.IdReg = IdReg;
            return View(mcm);
        }

        [HttpPost]
        public ActionResult CargaCerIMSS(mCredencialesMovimientosIMSS m)
        {
            int IdReg = m.IdReg;
            int IdUser = (int)Session["sIdUsuario"];
            string nombrearchivo = m.Archivo.FileName;
            if (m.Archivo.ContentLength > 0)
            {
                string rutaPrincipal = @"D:\SistemaTada\Sellos\" + IdReg;
                if (!Directory.Exists(rutaPrincipal))
                {
                    Directory.CreateDirectory(rutaPrincipal);
                }

                if (m.Key != null)
                {
                    string nombrearchivoKey = m.Key.FileName;
                    string rutaCer = rutaPrincipal + @"\" + nombrearchivo;
                    m.Archivo.SaveAs(rutaCer);

                    string rutaKey = rutaPrincipal + @"\" + nombrearchivoKey;
                    m.Key.SaveAs(rutaKey);
                    cCargaCertificados crp = new cCargaCertificados();
                    crp.AddCertificadoIMSS(m, rutaCer, rutaKey, IdUser);
                }
                else
                {
                    string rutaCer = rutaPrincipal + @"\" + nombrearchivo;
                    m.Archivo.SaveAs(rutaCer);
                    string rutaKey = null;
                    cCargaCertificados crp = new cCargaCertificados();
                    crp.AddCertificadoIMSS(m, rutaCer, rutaKey, IdUser);
                }
                
            }
            return RedirectToAction("Index");
        }
    }
}