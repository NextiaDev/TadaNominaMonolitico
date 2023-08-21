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
    public class IMSSController : BaseController
    {
        // GET: IMSS
        public ActionResult ActualizacionSDI()
        {
            mActualizacionSDI m = new mActualizacionSDI();
            return View(m);
        }

        [HttpPost]
        public ActionResult ActualizacionSDI(mActualizacionSDI m)
        {
            cIMSS movimienmtos = new cIMSS();
            int IdUsuario = (int)Session["sIdUsuario"];
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            try
            {
                if (ModelState.IsValid)
                {
                    string fileName = Path.GetFileName(m.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    m.Archivo.SaveAs(_path);

                    mRespuestaActualizacionSDI r = movimienmtos.UpdateEmpleadosSDI(_path, IdUsuario, m.Observaciones, IdUnidadNegocio);

                    return TextFile(r);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult TextFile(mRespuestaActualizacionSDI model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DEL PROCESO DE ACTUALIZACION DEL ARCHIVO: " + model.Path);

            tw.WriteLine("----------------------------------------");
            tw.WriteLine("Numero de Registros Leidos: " + model.noRegistros);
            tw.WriteLine("Actualizados correctamente: " + model.Correctos);
            tw.WriteLine("No Actualizados: " + model.Errores);
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("");

            if (list.Count > 0)
            {
                tw.WriteLine("Detalle de los errores:");
                tw.WriteLine("");
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
            else
            {
                tw.WriteLine("El archivo se cargo correctamente.");
            }

            tw.Flush();
            tw.Close();
            return File(memoryStream.GetBuffer(), "text/plain", "resultado.txt");
        }
    }
}