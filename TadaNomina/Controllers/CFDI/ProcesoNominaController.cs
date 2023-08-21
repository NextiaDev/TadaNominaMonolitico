using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class ProcesoNominaController : BaseController
    {
        // GET: ProcesoNomina
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            ClassRecibosEsuqema classPeriodoNomina = new ClassRecibosEsuqema();
            List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetPeriodosNomina(IdUnidadNegocio);

            ViewBag.Periodos = classPeriodoNomina.ListaPeriodosnomina(periodos);

            return View(periodos);
        }


        public ActionResult DescargarRecibos()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];

            ClassRecibosEsuqema classPeriodoNomina = new ClassRecibosEsuqema();

            List<ModelPeriodoNomina> periodos = classPeriodoNomina.GetPeriodosNomina(IdUnidadNegocio);
            ViewBag.Periodos = classPeriodoNomina.ListaPeriodosnomina(periodos);

            return View();
        }

        [HttpPost]
        public JsonResult GenerarXML(int IdPeriodoNomina)
        {
            string Result = "";
            int IdUnidadNegocio =  (int)Session["sIdUnidadNegocio"];
            ClassProcesoNominal nomina = new ClassProcesoNominal();

            ClassRecibosEsuqema repository = new ClassRecibosEsuqema();

            repository.GeneraRecibosComplementarios(IdPeriodoNomina, IdUnidadNegocio);

            nomina.IdPeriodoNomina =IdPeriodoNomina;
            nomina.CambiaEstatusRecibosEsquema(IdPeriodoNomina);


            return Json(Result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DescargarPDF(int IdPeriodoNomina)
        {
            
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassRecibosEsuqema repository = new ClassRecibosEsuqema();
            string formato = "1";

            string _tipoFormato = "";

            
            _tipoFormato = "2 recibos en una hoja";
            
            string _TipoPeriodo = repository.GetNamePeriodo(IdPeriodoNomina);

            string _formato = "-FORMATO-VariosArchivos" + _tipoFormato;
            string ruta_Principal = @"D:\TadaNomina\XML\" + IdUnidadNegocio.ToString() + _formato + @"\";
            string ruta_PDFS = @"D:\TadaNomina\XML\" + IdUnidadNegocio.ToString() +  @"\" + _TipoPeriodo + @"\PDFS\";
            string ruta_PDFS_ZIP = @"D:\TadaNomina\XML\" + IdUnidadNegocio.ToString() + @"\" + _TipoPeriodo;

            if (!Directory.Exists(ruta_Principal))
            {
                System.IO.Directory.CreateDirectory(ruta_Principal);
            }
            else
            {
                System.IO.Directory.Delete(ruta_Principal, true);
                System.IO.Directory.CreateDirectory(ruta_Principal);
            }


            if (!Directory.Exists(ruta_PDFS))
            {
                System.IO.Directory.CreateDirectory(ruta_PDFS);
            }
            else
            {
                System.IO.Directory.Delete(ruta_PDFS, true);
                System.IO.Directory.CreateDirectory(ruta_PDFS);
            }

            var query = repository.GetvRecibosEsquema(IdPeriodoNomina);

            List<string> files = new List<string>();

                foreach (var item in query)
                {
                    string archivoFisico = ruta_PDFS + item.ApellidoPaterno + " " + item.ApellidoMaterno + " " + item.Nombre + "_" + item.RFC + "_" + item.IdEmpleado + ".pdf";
                    //Genera dos formatos y varios archivos 
                    repository.generaPDFTodos(item.CadenaXML, archivoFisico, formato, IdUnidadNegocio);

                    files.Add(archivoFisico);
                }


            repository.CreateZipFile(files, ruta_PDFS_ZIP + @"\" + _TipoPeriodo + @".zip");
            string RutaZipCompleta = ruta_PDFS_ZIP + @"\" + _TipoPeriodo + @".zip";

            byte[] fileByte = System.IO.File.ReadAllBytes(RutaZipCompleta);
            return File(fileByte, System.Net.Mime.MediaTypeNames.Application.Octet, _TipoPeriodo + @".zip");
        }
    }
}