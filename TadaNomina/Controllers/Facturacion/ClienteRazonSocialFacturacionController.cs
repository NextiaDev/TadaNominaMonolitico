using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.Facturacion;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Controllers.Facturacion
{
    public class ClienteRazonSocialFacturacionController : BaseController
    {
        // GET: ClienteRazonSocialFacturacion
        public ActionResult Index()
        {
            ClassClienteRazonSocialFacturacion ccrs = new ClassClienteRazonSocialFacturacion();

           return View(ccrs.listar());
        }


        public ActionResult Crud(string ID = "0")
        {

            ClassClienteRazonSocialFacturacion ccrsf = new ClassClienteRazonSocialFacturacion();

            int _id = 0;

            if (ID != "0") { _id = int.Parse(TadaNomina.Models.ClassCore.Statics.DesEncriptar(ID.ToString())); }

            ViewBag.lista = ccrsf.ComboGrupoFacturacion();
            ViewBag.IdClienteRazonSocialFacturacion = _id;


            if (_id != 0) { return View(ccrsf.ListarRSocial(_id)); }
            else { return View(); }

        }

        [HttpPost]
        public ActionResult Crud(ModelClienteRazonSocialFacturacion mrsf)
        {
            if (ModelState.IsValid)
            {
                mrsf.IdCliente = (int)Session["sIdCliente"];
                mrsf.IdCaptura = (int)Session["sIdUsuario"];

                ClassClienteRazonSocialFacturacion ccrs = new ClassClienteRazonSocialFacturacion();

                ccrs.Crud(mrsf);

                return RedirectToAction("Index");
            }

            return View();
        }



        [HttpPost]
        public ActionResult Eliminar(ModelClienteRazonSocialFacturacion mrsf)
        {
            ClassClienteRazonSocialFacturacion ccrs = new ClassClienteRazonSocialFacturacion();


            mrsf.IdModifica = (int)Session["sIdUsuario"];
            mrsf.FechaModifica = DateTime.Now; 

            ccrs.Eliminar(mrsf);
            return RedirectToAction("Index", "ClienteRazonSocialFacturacion");
        }


    }
}