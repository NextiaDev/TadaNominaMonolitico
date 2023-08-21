using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Controllers.Facturacion
{
    public class MovimientosSaldosController : BaseController
    {
        // GET: MovimientosSaldos
        public ActionResult Index()
        {

            try
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                if (IdUnidadNegocio > 0)
                {

                    Models.ClassCore.Saldos cl = new Models.ClassCore.Saldos();
                    SumaSaldos model = cl.ListaSaldos(IdUnidadNegocio.ToString());
                    return View(model);
                }


            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(SumaSaldos model)
        {
            int IdUsuario = (int)Session["sIdUsuario"];
            int Cliente = (int)Session["sIdCliente"];
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            model.idCliente = Cliente;

            string resultado = "0";
            try
            {
                if (model.IdFacturaContabilidad != resultado)
                {
                    using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
                    {

                        int Idc = Convert.ToInt32(model.IdFacturaContabilidad);

                        var notify = (from s in entidad.FacturasContabilidad
                                      where s.IdFacturasContabilidad == Idc
                                      select s.Total).FirstOrDefault();


                        if (Convert.ToDecimal(model.Monto) >= notify)
                        {
                            
                                ObjectParameter param = new ObjectParameter("Texto", "");

                                entidad.SP_ValidaMontoFacturados(IdUsuario, Cliente, int.Parse(model.IdFacturaContabilidad), Idc, decimal.Parse(model.Monto), param);
                                Models.ClassCore.Saldos cls = new Models.ClassCore.Saldos();
                                SumaSaldos modelos = cls.ListaSaldos(IdUnidadNegocio.ToString());
                                ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Saldos', 'Registrar completo!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");

                                return View(modelos);
                            

                        }
                        else
                        {
                            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Saldos', 'Su monto es menor!!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");

                        }
                    }

                }
                else
                {

                    AddMovimiento(model, IdUsuario);
                }



            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }

            Models.ClassCore.Saldos cl = new Models.ClassCore.Saldos();
            SumaSaldos modelo = cl.ListaSaldos(IdUnidadNegocio.ToString());
            ViewBag.JavaScriptFunction = string.Format("mensajeAlerta('Saldos', 'Registrar completo!', 'mint', 'bounceInRight', 'bounceOutLeft', 4500);");


            return View(modelo);
        }




        public void AddMovimiento(SumaSaldos modelo, int IdUsuario)
        {
            using (Models.DB.TadaContabilidadEntities entidad = new Models.DB.TadaContabilidadEntities())
            {
                try
                {
                    entidad.SP_InsertaSaldosCosteo(IdUsuario, modelo.idCliente, modelo.Idunidad, modelo.idFacturadora, Convert.ToDecimal(modelo.Monto));
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}