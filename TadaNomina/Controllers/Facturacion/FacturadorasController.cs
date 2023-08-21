using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ClassCore;



namespace TadaNomina.Controllers.CFDI
{
    public class FacturadorasController : BaseController
    {
        // GET: Facturacion
        public ActionResult Index()
        {
            ClassFacturadoras Facturadoras = new ClassFacturadoras();
            //var prueba = Facturadoras.Listnasd();
            
            
            var fac = Facturadoras.ListaFacturadoras();
            
            return View(Facturadoras.ListaFacturadoras());
        }

        public ActionResult Crear()
        {
            ClassFacturadoras _CLSFacturadoras = new ClassFacturadoras();
            ModelFacturadoras _Facturadoras = null;

            ViewBag.lstBancos = _CLSFacturadoras.ListaBancos();

            return View(_Facturadoras);
        }

        [HttpPost]
        public ActionResult Crear(ModelFacturadoras _MF)
        {
            ClassFacturadoras _Facturadoras = new ClassFacturadoras();

            _MF.IdCaptura = int.Parse(Session["sIdUsuario"].ToString());
            
            _Facturadoras.Crear(_MF);

            return RedirectToAction("Index","Facturadoras");
        }


        public ActionResult Editar(int _Id)
        {

            ClassFacturadoras _CLSFacturadoras = new ClassFacturadoras();

            ViewBag.lstBancos = _CLSFacturadoras.ListaBancos();
            ClassFacturadoras _CF = new ClassFacturadoras();

            var _Fac = _CF.GetFacturadora(_Id);

            _Fac.FechaOperacionString = _Fac.FechaOperacion.ToString().Substring(0,10);

            return View(_Fac);
        }


        [HttpPost]
        public ActionResult Editar(ModelFacturadoras _MF)
        {
            ClassFacturadoras _CF = new ClassFacturadoras();
            _MF.FechaOperacion = DateTime.Parse(_MF.FechaOperacionString);
            _CF.Modifica(_MF);

            return RedirectToAction("Index", "Facturadoras");
        }


        [HttpPost]
        public ActionResult Eliminar(int _id)
        {
            ClassFacturadoras Facturadoras = new ClassFacturadoras();
            int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());

            Facturadoras.Eliminar(_id, IdUsuario);

            return RedirectToAction("Index","Facturadoras");
        }

    }

}