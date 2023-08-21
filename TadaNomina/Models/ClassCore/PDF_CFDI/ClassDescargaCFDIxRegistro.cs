using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TadaNomina.Models.ClassCore;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ClassCore.Timbrado;

namespace TadaNomina.Models.ClassCore.PDF_CFDI
{
    public class ClassDescargaCFDIxRegistro
    {
        CrearXML _xml = new CrearXML();

        public ModelDescargaCFDI GetModel(int IdUnidadNegocio)
        {
            ModelDescargaCFDI model = new ModelDescargaCFDI();

            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();
            List<SelectListItem> lperiodos = new List<SelectListItem>();

            List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoNominasAcumuladas(IdUnidadNegocio);
            lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });

            List<SelectListItem> lFormato = new List<SelectListItem>() {
                new SelectListItem{ Text = "PDF", Value = "PDF" },
                new SelectListItem{ Text = "XML", Value = "XML" }
            };

            model.lPeriodos = lperiodos;
            model.lTipoArchivo = lFormato;

            return model;
        }

        public void GetZip(int IdPeriodoNomina, string formato, int IdUnidad)
        {
            string ruta_CFDI_ZIP = @"D:\TadaNomina\DescargaCFDINomina";
            ClassTimbradoNomina ct = new ClassTimbradoNomina();
            var list = ct.GetvTimbrados(IdPeriodoNomina);

            List<string> files = new List<string>();

            if (Directory.Exists(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip"))
                Directory.Delete(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip", true);

            if (list.Count > 0)
            {
                files = getFiles(list, formato, string.Empty);
                CreateZipFile(files, ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip");
            }
        }

        public List<string> getFiles(List<vTimbradoNomina> timbrado, string formato, string filtro)
        {
            List<string> files = new List<string>();
            if (formato == "PDF")
                files = GetPDFId(timbrado, filtro);
            else if (formato == "XML")
                files = GetXMLId(timbrado, filtro);


            return files;
        }

        public Cat_RegistroPatronal GetRegistro(string Registro)
        {
            using (var entidad = new TadaEmpleados())
            {
                return entidad.Cat_RegistroPatronal.Where(x => x.RegistroPatronal == Registro).FirstOrDefault();
            }
        }

        public Cat_CentroCostos GetCC(int IdCentroCostos)
        {
            using (var entidad = new TadaEmpleados())
            {
                return entidad.Cat_CentroCostos.Where(x => x.IdCentroCostos == IdCentroCostos).FirstOrDefault();
            }
        }

        public List<string> GetPDFId(List<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            ClassDescargaCFDIxRegistro cd = new ClassDescargaCFDIxRegistro();

            var listaregistro = timbrado.GroupBy(p => p.RegistroPatronal).Select(reg => new { RegistroPatronal = reg.Key, timbrado = reg.ToList() }).ToList();

            for (int i = 0; i < listaregistro.Count; i++)
            {
                var listadoxReg = timbrado.Where(x => x.RegistroPatronal.Equals(listaregistro[i].RegistroPatronal)).ToList();
                var patrona = cd.GetRegistro(listaregistro[i].RegistroPatronal.ToString());
                string carpeta = @"D:\TadaNomina\DescargaCFDINomina\" + patrona.NombrePatrona.ToString();

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }
                else
                {
                    Directory.Delete(carpeta, true);
                    Directory.CreateDirectory(carpeta);
                }

                foreach (var item in listadoxReg)
                {
                    string xml = item.CFDI_Timbrado;
                    string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "" + item.RFC + "" + item.ClaveEmpleado + "_" + item.IdEmpleado + ".pdf";

                    string ruta = carpeta + @"\" + item.IdPeriodoNomina;
                    if (filtro != string.Empty) { ruta += @"\" + filtro; }
                    string rutaArchivo = ruta + @"\" + NombreArchivo;

                    if (!Directory.Exists(ruta))
                        System.IO.Directory.CreateDirectory(ruta);

                    WS_CFDI cga = new WS_CFDI();
                    cga.guardaPDF(item.CFDI_Timbrado, item.Leyenda, rutaArchivo, item.Firma, item.SueldoMensual);
                }
                lista.Add(carpeta);
            }
            return lista;
        }

        public List<string> GetXMLId(IEnumerable<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            ClassDescargaCFDIxRegistro cd = new ClassDescargaCFDIxRegistro();

            var listaregistro = timbrado.GroupBy(p => p.RegistroPatronal).Select(reg => new { RegistroPatronal = reg.Key, timbrado = reg.ToList() }).ToList();

            for (int i = 0; i < listaregistro.Count; i++)
            {
                var listadoxReg = timbrado.Where(x => x.RegistroPatronal.Equals(listaregistro[i].RegistroPatronal)).ToList();
                var patrona = cd.GetRegistro(listaregistro[i].RegistroPatronal.ToString());
                string carpeta = @"D:\TadaNomina\DescargaCFDINomina\" + patrona.NombrePatrona.ToString();

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }
                else
                {
                    Directory.Delete(carpeta, true);
                    Directory.CreateDirectory(carpeta);
                }

                foreach (var item in listadoxReg)
                {
                    string xml = item.CFDI_Timbrado;
                    string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "" + item.RFC + "" + item.ClaveEmpleado + "_" + item.IdEmpleado + ".xml";
                    string ruta = carpeta + @"\" + item.IdPeriodoNomina;
                    if (filtro != string.Empty) { ruta += @"\" + filtro; }
                    string rutaArchivo = ruta + @"\" + NombreArchivo;

                    if (!Directory.Exists(ruta))
                        System.IO.Directory.CreateDirectory(ruta);

                    _xml.crearXML(xml, rutaArchivo);

                }
                lista.Add(carpeta);
            }

            return lista;
        }

        public void CreateZipFile(List<string> items, string destination)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                foreach (string item in items)
                {
                    if (System.IO.File.Exists(item))
                    {
                        zip.AddFile(item, "");
                    }
                    else if (System.IO.Directory.Exists(item))
                    {
                        zip.AddDirectory(item, new System.IO.DirectoryInfo(item).Name);
                    }
                }
                zip.Save(destination);
            }
        }
    }
}