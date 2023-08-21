using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.MovimientosIMSS;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Controllers.MovimientosIMSS
{
    public class ConsultasController : BaseController
    {
        // GET: Consultas
        public ActionResult Index(string mensaje)
        {
            List<Cat_RegistroPatronal> listado = new List<Cat_RegistroPatronal>();
            cConsultas cc = new cConsultas();
            int IdCliente = int.Parse(Session["sIdCliente"].ToString());
            listado = cc.GetRegistrosPatronales(IdCliente);
            ViewBag.Mensaje = mensaje;
            return View(listado);
        }

        public ActionResult ListadoLotes(int IdRegistroPatronal, string mensaje)
        {
            try
            {
                RespuestaLotesIMSS mrl = new RespuestaLotesIMSS();
                cConsultas cc = new cConsultas();
                mrl = cc.GetLotesByRegistroPatronal(IdRegistroPatronal);
                if (mrl.mensaje == "OPERACION REALIZADA EXITOSAMENTE")
                {
                    ViewBag.RegistroPatronal = IdRegistroPatronal;
                    ViewBag.Mensaje = mensaje;
                    return View(mrl);
                }
                else
                {
                    return RedirectToAction("Index", new { mensaje = mrl.mensaje });
                }
            }
            catch (Exception ex)
            {
                string mensaje2 = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mensaje2 });
            }
        }

        public ActionResult ConsultaRespuestaLote(int IdRegistroPatronal, string Lote)
        {
            try
            {
                mDetalleLote mdl = new mDetalleLote();
                cConsultas cc = new cConsultas();
                mdl = cc.GetDetalleLote(IdRegistroPatronal, Lote);
                if (mdl.RespuestaGeneral.mensaje == "OPERACION REALIZADA EXITOSAMENTE")
                {
                    ViewBag.Registro = IdRegistroPatronal;
                    ViewBag.Lote = Lote;
                    return View(mdl);
                }
                else
                {
                    return RedirectToAction("ListadoLotes", new { IdRegistroPatronal = IdRegistroPatronal, mensaje = mdl.RespuestaGeneral.mensaje });
                }
            }
            catch (Exception ex)
            {
                string mensaje2 = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mensaje2 });
            }
        }

        public ActionResult DescargaPDFRespuestaGeneral(int IdRegistroPatronal, string Lote)
        {
            try
            {
                cConsultas cc = new cConsultas();
                var zip = cc.GetPDFRespuestaGeneral(IdRegistroPatronal, Lote);
                return File(zip, "*.zip", "Constancia de presentación de movimientos afiliatorios Lote_" + Lote + ".zip");
            }
            catch (Exception ex)
            {
                string mensaje2 = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mensaje2 });
            }
        }

        public ActionResult DescargaPDFDetalleRespuesta(int IdRegistroPatronal, string Lote)
        {
            try
            {
                cConsultas cc = new cConsultas();
                var zip = cc.GetPDFDetalleRespuesta(IdRegistroPatronal, Lote);
                return File(zip, "*.zip", "Acuse de recibo electrónico Lote_" + Lote + ".zip");
            }
            catch (Exception ex)
            {
                string mensaje2 = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mensaje2 });
            }
        }

        public ActionResult Emisiones(int IdRegistroPatronal, string mensaje)
        {
            try
            {
                cConsultas cc = new cConsultas();
                var modelo = cc.GetmEmisiones(IdRegistroPatronal);
                ViewBag.id = IdRegistroPatronal;
                ViewBag.Mensaje = mensaje;
                return View(modelo);
            }
            catch (Exception ex)
            {
                string mensaje2 = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mensaje2 });
            }
        }

        public ActionResult DescargaEmision(string idregistro, mEmisiones model)
        {
            try
            {
                string _tipoEmision = model.tipoemision;
                string resultado = string.Empty;
                cConsultas cc = new cConsultas();
                int IdRegistroPatronal = int.Parse(idregistro);
                var respuesta = cc.GetRespuestaEmision(IdRegistroPatronal, _tipoEmision);
                if (respuesta.respuestaWebService == null)
                {
                    resultado = "No Existe el tipo de archivo";
                    return RedirectToAction("Emisiones", new { IdRegistroPatronal = IdRegistroPatronal, mensaje = resultado });
                }
                else
                {
                    try
                    {
                        resultado = respuesta.respuestaWebService.descargaDisco.archivoEmisionDisco;
                        byte[] bytes = Convert.FromBase64String(resultado);
                        return File(bytes, "*.zip");
                    }
                    catch
                    {
                        resultado = "Pruebe con" + model.tipoemision + "_PDF";
                        return RedirectToAction("Emisiones", new { IdRegistroPatronal = IdRegistroPatronal, mensaje = resultado });
                    }
                }
            }
            catch (Exception ex)
            {
                string mensaje2 = "Error de proveedor" + ex.Message;
                return RedirectToAction("Index", new { mensaje = mensaje2 });
            }
        }
    }
}