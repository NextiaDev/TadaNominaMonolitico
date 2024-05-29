using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Fonacot;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Controllers.Nomina
{
    public class FonacotController : BaseController
    {
        // GET: Fonacot
        public ActionResult Index()
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassFonacot ci = new ClassFonacot();
            var model = ci.GetModelFonacot(IdUnidadNegocio);
            return View(model);
        }

        public ActionResult CreateLayout()
        {
            


            return View();
        }

        [HttpPost]
        public ActionResult CreateLayout(ModelIncidencias model)
        {
            int IdUsuario = (int)Session["sIdUsuario"];

           
                if (model.Archivo.ContentLength > 0)
                {

                    string fileName = Path.GetFileName(model.Archivo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadesFiles"), fileName);
                    model.Archivo.SaveAs(_path);
                    ModelErroresIncidencias errores = GetIncidenciasFonacot(_path, IdUsuario);

                    return TextFile(errores);


                }
            
            return View(model);
        }

        public ActionResult TextFile(ModelErroresIncidencias model)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            List<string> list = model.listErrores;

            tw.WriteLine("DETALLE DE CARGA DE INCIDENCIAS DEL ARCHIVO: " + model.Path);
            tw.WriteLine("");
            tw.WriteLine("----------------------------------------");
            tw.WriteLine("Número de Registros Leidos: " + model.Errores);
            tw.WriteLine("Insertados correctamente: " + model.Correctos);
            tw.WriteLine("No Insertados: " + model.Errores);
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

        public ModelErroresIncidencias GetIncidenciasFonacot(string ruta, int IdUsuario)
        {
            int IdUnidad = (int)Session["sIdUnidadNegocio"];

            ModelErroresIncidencias modelErrores = new ModelErroresIncidencias();
            modelErrores.listErrores = new List<string>();
            modelErrores.Correctos = 0;
            modelErrores.Errores = 0;
            modelErrores.noRegistro = 0;
            modelErrores.Path = Path.GetFileName(ruta);
            ArrayList array = GetArrayIncidencias(ruta);
            List<CsvRecord> incidencias = new List<CsvRecord>();
            ClassEmpleado cempleado = new ClassEmpleado();
            List<vEmpleados> vEmp = cempleado.GetAllvEmpleados(IdUnidad);
            foreach (var item in array)
            {
                modelErrores.noRegistro++;
                AddRegidtroIncidenciaFonacot(modelErrores , incidencias, vEmp, item);
            }

            try { GuardarFonacotLista(incidencias, IdUsuario); } catch (Exception ex) { modelErrores.listErrores.Add(ex.ToString()); }

            return modelErrores;
        }


        private void AddRegidtroIncidenciaFonacot( ModelErroresIncidencias errores, List<CsvRecord> incidencias, List<vEmpleados> vEmp, object item)
        {
            ClassFonacot cl = new ClassFonacot();
            string[] campos = item.ToString().Split(',');
            int IdEmpleado = vEmp.Where(x => x.ClaveEmpleado.Trim() == campos[0].Trim().ToUpper()).Select(x => x.IdEmpleado).FirstOrDefault();

            if (IdEmpleado == 0)
            {
                errores.Errores++;
                errores.listErrores.Add($"Empleado con clave {campos[0]} no encontrado.");
                return;
            }


            string NoFonacot = null;
            string NoCredito = null;
            int Plazos = 0;
            decimal CuotasPagadas = 0;
            decimal RetencionMensual = 0;
            string Activo = "SI"; //


            if (!string.IsNullOrWhiteSpace(campos[1])) NoFonacot = campos[1].Trim();

            if (NoFonacot == null)
            {
                errores.Errores++;
                errores.listErrores.Add($"Error en campo NoFonacot del empleado con clave {campos[0]}.");
                return;
            }


            if (!string.IsNullOrWhiteSpace(campos[2])) NoCredito = campos[2].Trim();
            if (NoCredito == null)
            {
                errores.Errores++;
                errores.listErrores.Add($"Error en campo NoCredito del empleado con clave {campos[0]}.");
                return;

            }
            else
            {
                var validacion = cl.ValidaCreditoFonacot(NoCredito);

                if (validacion == true)
                {
                    errores.Errores++;
                    errores.listErrores.Add($"Error el credito esta repetido del empleado con clave {campos[0]}.");
                    return;
                }


            }

            if (!int.TryParse(campos[3], out Plazos))
            {
                errores.Errores++;
                errores.listErrores.Add($"Error en campo Plazos del empleado con clave {campos[0]}.");
                return;
            }
            if (!decimal.TryParse(campos[4], out CuotasPagadas))
            {
                errores.Errores++;
                errores.listErrores.Add($"Error en campo Cuotas Pagadas del empleado con clave {campos[0]}.");
                return;
            }
            if (!decimal.TryParse(campos[5], out RetencionMensual))
            {
                errores.Errores++;
                errores.listErrores.Add($"Error en campo Retención Mensual del empleado con clave {campos[0]}.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(campos[6])) Activo = campos[6].Trim();

            // Si no hay errores, agregar el registro a la lista de incidencias
            if (!errores.listErrores.Any())
            {
                errores.Correctos++;
                CsvRecord incidencia = new CsvRecord
                {
                    idEmpleado = IdEmpleado,
                    NoFonacot = NoFonacot,
                    NoCredito = NoCredito,
                    Plazos = Plazos,
                    CuotasPagadas = CuotasPagadas,
                    RetencionMensual = RetencionMensual,
                    Activo = Activo.ToUpper()
                };
                incidencias.Add(incidencia);
            }
        }



        public void GuardarFonacotLista(List<CsvRecord> modelo, int idcaptura)
        {

            cHonorarios cl = new cHonorarios();
            foreach (var i in modelo)
            {
                

                using (NominaEntities1 entity = new NominaEntities1())
                {
                    CreditosFonacot Cfonacot = new CreditosFonacot();

                    Cfonacot.IdEmpleado = i.idEmpleado;
                    Cfonacot.NoTrabajadorFonacot = i.NoFonacot;
                    Cfonacot.NumeroCredito = i.NoCredito;
                    Cfonacot.Plazos = i.Plazos;
                    Cfonacot.CuotasPagadas = i.CuotasPagadas;
                    Cfonacot.RetencionMensual = i.RetencionMensual; 
                    Cfonacot.Activo = i.Activo;
                    Cfonacot.IdEstatus = 1;
                    Cfonacot.IdCaptura = idcaptura;
                    Cfonacot.FechaCaptura = DateTime.Now;
                    entity.CreditosFonacot.Add(Cfonacot);
                    entity.SaveChanges();

                
                }
            }
        }


        public ArrayList GetArrayIncidencias(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;
            int contador = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (contador > 0)
                    {
                        arrText.Add(sLine);
                    }
                    contador++;
                }
            }

            objReader.Close();
            return arrText;
        }

        public ActionResult Create()
        {
            ClassFonacot cf = new ClassFonacot();
            var model = new ModelFonacot();
            model.Activo = true;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            try
            {
                ClassFonacot cf = new ClassFonacot();
                var model = cf.getCreditoFonacotById(id);
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult Edit(ModelFonacot model)
        {
            ClassFonacot ci = new ClassFonacot();
            int IdUsuario = (int)Session["sIdUsuario"];
            try
            {
                if (model != null)
                {
                    ci.editFonacot(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información del Credito!";
                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ModelFonacot model)
        {
            ClassFonacot cf = new ClassFonacot();
            int IdUsuario = (int)Session["sIdUsuario"];

            try
            {
                if (model.IdEmpleado > 0)
                {
                    cf.newCreditoFonacot(model, IdUsuario);
                    model.Validacion = true;
                    model.Mensaje = "Se guardo correctamente la información del credito!";
                }
                else
                {
                    model.Validacion = false;
                    model.Mensaje = "Error: No se encuentra al empleado, favor de verificar!";
                }
            }
            catch (Exception ex)
            {
                model.Validacion = false;
                model.Mensaje = "Error: " + ex.Message;
            }

            return View(model);
        }

        public JsonResult BuscaEmpleado(string clave)
        {
            int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
            ClassEmpleado cemp = new ClassEmpleado();
            var emp = cemp.GetEmpleadosByClave(clave, IdUnidadNegocio).FirstOrDefault();

            if (emp != null)
                return Json(emp, JsonRequestBehavior.AllowGet);
            else
                return Json("El Empleado con la clave que ingreso no existe!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            ClassFonacot cf = new ClassFonacot();
            var model = cf.getCreditoFonacotById(id);
            return PartialView(model);
        }

        public ActionResult Delete(int id)
        {
            ClassFonacot cf = new ClassFonacot();
            var model = cf.getCreditoFonacotById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Delete(ModelFonacot model)
        {
            try
            {
                ClassFonacot cf = new ClassFonacot();
                cf.DeleteCreditoFonacot(model.IdCreditoFonacot, (int)Session["sIdUsuario"]);
                cf.DeleteIncidenciasFonacot(model.IdCreditoFonacot, (int)Session["sIdUsuario"]);
            }
            catch { }

            return RedirectToAction("Index");
        }


        /// <summary>
        ///     Método que modifica el estdo del crédito, en caso de ser inactivo no se considerará en el cálculo de la nómina
        /// </summary>
        /// <param name="IdCredito">Id del crédito</param>
        /// <returns>Estatus del movimiento</returns>
        [HttpPost]
        public JsonResult CambiaStatusCredito(int IdCredito)
        {
            try
            {
                ClassFonacot cI = new ClassFonacot();
                int IdUsuario = int.Parse(Session["sIdUsuario"].ToString());
                var res = cI.CambiaEstatus(IdCredito, IdUsuario);
                if (res == 1)
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                    return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Método que desactiva todos los créditos FONACOT
        /// </summary>
        /// <returns>Respuesta del movimiento</returns>
        [HttpPost]
        public JsonResult Desactivacreditos(int tipoMov)
        {
            try
            {
                int IdUnidadNegocio = (int)Session["sIdUnidadNegocio"];
                int IdUsuario = (int)Session["sIdUsuario"];
                ClassFonacot cF = new ClassFonacot();
                var res = cF.Desactivacreditos(IdUnidadNegocio, IdUsuario, tipoMov);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }
    }
}