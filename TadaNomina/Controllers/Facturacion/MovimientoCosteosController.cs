using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Controllers.CFDI
{
    public class MovimientoCosteosController : BaseController
    {
        // GET: MovimientoCosteos
        public ActionResult Index()
        {

            ClassPeriodoNomina CPN = new ClassPeriodoNomina();
            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());

            ViewBag.LstPeriodos = CPN.ListaPeriodosnomina(CPN.GetPeriodosDifStatus(_IdUN));


            return View();
        }

        [HttpPost]
        public ActionResult Consultar (int NewValue)
        {
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();
            var v = CMC.ListaMovimientosCosteos(NewValue);
            ClassPeriodoNomina CPN = new ClassPeriodoNomina();
            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ViewBag.LstPeriodos = CPN.ListaPeriodosnomina(CPN.GetPeriodosDifStatus(_IdUN));

            return View(v);
        }
        
        public ActionResult Crear()
        {
            ModelMovimientosCosteos MMC = new ModelMovimientosCosteos();
            ClassPeriodoNomina CPN = new ClassPeriodoNomina();
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();
            

            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            int _IdCli = (int)Session["sIdCliente"];
            int IdCliente = (int)CMC.GetIdCliente(_IdUN);
            

            //Periodos de nomina
            ViewBag.LstPeriodos = CPN.ListaPeriodosnomina(CPN.GetPeriodosDifStatus(_IdUN));

           //Listar Costeos
            ViewBag.lstCosteos = CMC.ListaCosteos(CMC.GetCosteos(_IdUN));

            //Listar Tipo Movimiento
            ViewBag.lstTipoMovimientos = CMC.ListarTipoMovimiento();

            //Lista Vacia
            ViewBag.ListaVacia = CMC.ListaVacia();

            ViewBag.lstRegistroPatronal = CMC.ListaRegistroPatronal(CMC.GetRegistrosPatronales(_IdCli));

            return View(MMC);
        }
        [HttpPost]
        public ActionResult Crear(ModelMovimientosCosteos _ModelMovimiento)
        {
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();
            ClassPeriodoNomina CPN = new ClassPeriodoNomina();

            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            int IdCliente = (int)CMC.GetIdCliente(_IdUN);
            _ModelMovimiento.IdCaptura = int.Parse(Session["sIdUsuario"].ToString());
            if(_ModelMovimiento.IdDivision == null)
            {
                _ModelMovimiento.IdDivision = 0;
            }

            CMC.Crear(_ModelMovimiento);


            //Periodos de nomina
            ViewBag.LstPeriodos = CPN.ListaPeriodosnomina(CPN.GetPeriodosDifStatus(_IdUN));
            //Listar Costeos
            ViewBag.lstCosteos = CMC.ListaCosteos(CMC.GetCosteos(_IdUN));

            //Listar Tipo Movimiento
            ViewBag.lstTipoMovimientos = CMC.ListarTipoMovimiento();

            //Lista Vacia
            ViewBag.ListaVacia = CMC.ListaVacia();

            ViewBag.lstRegistroPatronal = CMC.ListaRegistroPatronal(CMC.GetRegistroPatronal(IdCliente));



            return View();
        }

        public ActionResult Editar(int _Id)
        {
            ModelMovimientosCosteos MMC = new ModelMovimientosCosteos();
            ClassPeriodoNomina CPN = new ClassPeriodoNomina();
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();
            ModelDeptoPuestoCC MDPCC = new ModelDeptoPuestoCC();

            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            int IdCliente = (int)CMC.GetIdCliente(_IdUN);
            var _Modelo = CMC.GetMovimientoCosteos(_Id);



            //Periodos de nomina
            ViewBag.LstPeriodos = CPN.ListaPeriodosnomina(CPN.GetPeriodosDifStatus(_IdUN));

            //Listar Costeos
            ViewBag.lstCosteos = CMC.ListaCosteos(CMC.GetCosteos(_IdUN));

            //Listar Tipo Movimiento
            ViewBag.lstTipoMovimientos = CMC.ListarTipoMovimiento();

            //Lista Vacia
            ViewBag.ListaVacia = CMC.ListaVacia();
            //Lista Registro Patronal
            ViewBag.lstRegistroPatronal = CMC.ListaRegistroPatronal(CMC.GetRegistroPatronal(IdCliente));


            var SeparadoPor = new TadaNomina.Models.DB.Costeos();

            if (_Modelo.IdCosteo != null)
            {
                SeparadoPor = CMC.GetCosteoSeparador((int)_Modelo.IdCosteo);

                ViewBag.ListaCosteoConceptos = CMC.ListarCosteosConceptos(CMC.ListarCosteoConceptos((int)_Modelo.IdCosteo));
            }
            else {
                ViewBag.ListaCosteoConceptos = CMC.ListaVacia();
            }


            if (_Modelo.IdDivision == null)
            {
                _Modelo.IdDivision = 0;
            }

            if (SeparadoPor.SeparadoPor != "General")
            {
                switch (SeparadoPor.SeparadoPor)
                {
                    case "CENTRO DE COSTOS":
                        ViewBag.Division = CMC.ListarCC(MDPCC.ListarCentroCostos(IdCliente));
                        break;

                    case "DEPARTAMENTOS":
                        ViewBag.Division = CMC.ListarDD(MDPCC.ListarDeptos(IdCliente));
                        break;

                    case "PUESTOS":
                        ViewBag.Division = CMC.ListarPP(MDPCC.ListarPuestos(IdCliente));
                        break;
                    default:
                        ViewBag.Division = CMC.ListaVacia();
                        break;
                }
            }else
            {
                ViewBag.Division = CMC.ListaVacia();
            }

            
            return View(_Modelo);
        }

        [HttpPost]
        public ActionResult Editar(ModelMovimientosCosteos _MMC)
        {
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();


            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            _MMC.IdModifica = (int)CMC.GetIdCliente(_IdUN);

            CMC.Editar(_MMC);

            return RedirectToAction("Index", "MovimientoCosteos");
        }

        [HttpPost]
        public ActionResult Eliminar(int _Id)
        {
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();
            int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());


            CMC.Eliminar(_Id, IdUsuario);

            return RedirectToAction("Index", "MovimientoCosteos");

        }



        [HttpPost]
        public JsonResult ListarDivision(int _IdCosteo)
        {
            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();
            ModelDeptoPuestoCC MDPCC = new ModelDeptoPuestoCC();

            var SeparadoPor = CMC.GetCosteoSeparador(_IdCosteo);
            int IdCliente = CMC.GetIdCliente(_IdUN);

            if (SeparadoPor.SeparadoPor != "GENERAL")
            {
                switch (SeparadoPor.SeparadoPor)
                {
                    case "CENTRO DE COSTOS":
                        var CC = MDPCC.ListarCentroCostos(IdCliente);
                        return Json(CC, JsonRequestBehavior.AllowGet);

                    case "DEPARTAMENTOS":
                        var Depto = MDPCC.ListarDeptos(IdCliente);
                        return Json(Depto, JsonRequestBehavior.AllowGet);

                    case "PUESTOS":
                        var Puestos = MDPCC.ListarPuestos(IdCliente);
                        return Json(Puestos, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("GENERAL", JsonRequestBehavior.AllowGet);
            }
            return Json("GENERAL", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarPatronas(int _IdCosteo)
        {

            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();

            var DividirPatronal = CMC.GetCosteoSeparador(_IdCosteo);
            if (DividirPatronal.DividirPatronal == "SI")
            {
                return Json("SI", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NO", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ListarConcepto(int _IdCosteo)
        {
            int _IdUN = int.Parse(Session["sIdUnidadNegocio"].ToString());
            ClassMovimientoCosteos CMC = new ClassMovimientoCosteos();

            var CosteoConcepto = CMC.ListarCosteoConceptos(_IdCosteo);

            
                return Json(CosteoConcepto, JsonRequestBehavior.AllowGet);
        }
    }
}