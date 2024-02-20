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
using TadaNomina.Models.ClassCore.Timbrado;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;

namespace TadaNomina.Models.ClassCore.PDF_CFDI
{
    public class ClassDescargaCFDI
    {        
        CrearXML _xml = new CrearXML();

        public ModelDescargaCFDI GetModel(int IdUnidadNegocio)
        {
            ModelDescargaCFDI model = new ModelDescargaCFDI();

            ClassPeriodoNomina cperiodo = new ClassPeriodoNomina();            
            List<SelectListItem> lperiodos = new List<SelectListItem>();

            List<vPeriodoNomina> lvperiodos = cperiodo.GetvPeriodoNominasAcumuladas(IdUnidadNegocio).OrderByDescending(x=> x.IdPeriodoNomina).ToList();
            lvperiodos.ForEach(x => { lperiodos.Add(new SelectListItem { Value = x.IdPeriodoNomina.ToString(), Text = x.Periodo }); });

            List<SelectListItem> lFormato = new List<SelectListItem>() {
                new SelectListItem{ Text = "PDF", Value = "PDF" },
                new SelectListItem{ Text = "XML", Value = "XML" }
            };

            List<SelectListItem> lDividir = new List<SelectListItem>() {
                new SelectListItem{ Text = "General", Value = "General" },
                new SelectListItem{ Text = "Registro Patronal", Value = "RegistroPatronal" },
                new SelectListItem{ Text = "Centro de Costos", Value = "CentroCostos" }
            };

            model.lPeriodos = lperiodos;
            model.lTipoArchivo = lFormato;
            model.lDividir = lDividir;

            return model;
        }

        /// <summary>
        ///     Método que obtiene un zip con los CFDI o XML seleccionados
        /// </summary>
        /// <param name="IdPeriodoNomina">Id del periodo de nómina</param>
        /// <param name="formato">Tipo de formato que se va a descargar</param>
        /// <param name="IdCliente">Id del Cliente</param>
        public void GetZip(int IdPeriodoNomina, string formato, int IdCliente)
        {
            string ruta_CFDI_ZIP = @"D:\TadaNomina\DescargaCFDINomina";
            ClassTimbradoNomina ct = new ClassTimbradoNomina();
            var list = ct.GetvTimbrados(IdPeriodoNomina);

            if (list.Count <= 0) { throw new Exception("No existen registros timbrados para este periodo."); }

            List<string> files = new List<string>();

            if (Directory.Exists(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip"))
                Directory.Delete(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip", true);

            if (list.Count > 0)
            {
                //Se filtra el cliente BSD (grupo 2) y se agrega la leyenda para el PDF
                try { list = GetCliente(IdCliente, list); } catch { }

                files = getFiles(list, formato, string.Empty);
                CreateZipFile(files, ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip");
            }
        }

        /// <summary>
        ///     Método que obtiene la leyenda para el CFDI dependiendo del grupo al que pertenezca el cliente
        /// </summary>
        /// <param name="IdCliente">Id del cliente</param>
        /// <param name="list">Lista de empleados con los datos del timbrado</param>
        /// <returns>Lista de empleados con los datos del timbrado</returns>
        public List<vTimbradoNomina> GetCliente(int IdCliente, List<vTimbradoNomina> list)
        {
            string leyenda = "";
            List<vTimbradoNomina> res = new List<vTimbradoNomina>();
            res = list;
            try
            {
                using (TadaNominaEntities ctx = new TadaNominaEntities())
                {
                    var cl = ctx.Cat_Clientes.Where(x => x.IdCliente == IdCliente && x.IdGrupo == 2).FirstOrDefault();

                    if (cl != null)
                        leyenda = "Se puso a mi disposicion el archivo XML correspondiente y recibi de la empresa arriba mencionada la cantidad neta\r\na que este documento se refiere estando conforme con las percepciones y deducciones que en el aparecen\r\nespecificados";
                    else
                        leyenda = "";
                }

                if (leyenda != "")
                {
                    foreach (var item in res)
                        item.Leyenda = leyenda;

                    return res;
                }
            }
            catch
            {
                return list;
            }
            return list;
        }

        /// <summary>
        ///     Método que obtiene el CFDI de un registro patronal
        /// </summary>
        /// <param name="IdPeriodoNomina">Id del periodo de nómina</param>
        /// <param name="formato">Tipo de archivo que se generará PDF o XML</param>
        /// <param name="IdCliente">Id del cliente</param>
        public void GetZipReg(int IdPeriodoNomina, string formato, int IdCliente)
        {
            string ruta_CFDI_ZIP = @"D:\TadaNomina\DescargaCFDINomina";
            ClassTimbradoNomina ct = new ClassTimbradoNomina();
            var list = ct.GetvTimbrados(IdPeriodoNomina);

            List<string> files = new List<string>();

            if (Directory.Exists(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip"))
                Directory.Delete(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip", true);

            if (list.Count > 0)
            {
                //Se filtra el cliente BSD (grupo 2) y se agrega la leyenda para el PDF
                try { list = GetCliente(IdCliente, list); } catch { }

                files = getFilesReg(list, formato, string.Empty);
                CreateZipFile(files, ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip");
            }
        }

        /// <summary>
        ///     Método que genera los archivos por centro de costos
        /// </summary>
        /// <param name="IdPeriodoNomina">Id del periodo de nómina</param>
        /// <param name="formato">Tipo de archivo que se generará PDF o XML</param>
        /// <param name="IdCliente">Id del cliente</param>
        public void GetZipCC(int IdPeriodoNomina, string formato, int IdCliente)
        {
            string ruta_CFDI_ZIP = @"D:\TadaNomina\DescargaCFDINomina";
            ClassTimbradoNomina ct = new ClassTimbradoNomina();
            var list = ct.GetvTimbrados(IdPeriodoNomina);

            List<string> files = new List<string>();

            if (Directory.Exists(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip"))
                Directory.Delete(ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip", true);

            if (list.Count > 0)
            {
                //Se filtra el cliente BSD (grupo 2) y se agrega la leyenda para el PDF
                try { list = GetCliente(IdCliente, list); } catch { }

                files = getFilesCC(list, formato, string.Empty);
                CreateZipFile(files, ruta_CFDI_ZIP + @"\" + IdPeriodoNomina + @".zip");
            }
        }

        /// <summary>
        ///     Método que obtiene los archivos solicitados
        /// </summary>
        /// <param name="timbrado">Lista con la información de los empleados y del timbrado</param>
        /// <param name="formato">Formato que se va a descargar</param>
        /// <param name="filtro"></param>
        /// <returns>Archivos solicitados</returns>
        public List<string> getFiles(List<vTimbradoNomina> timbrado, string formato, string filtro)
        {  
            List<string> files = new List<string>();
            if (formato == "PDF")            
                files = GetPDFId(timbrado, filtro);            
            else if (formato == "XML")            
                files = GetXMLId(timbrado, filtro);
            

            return files;
        }

        /// <summary>
        ///     Método que obtiene los archivos solicitados por registro patronal
        /// </summary>
        /// <param name="timbrado">Lista con información del timbrado</param>
        /// <param name="formato">Formato que se va a descargar</param>
        /// <param name="filtro"></param>
        /// <returns>Erchivos solicitados</returns>
        public List<string> getFilesReg(List<vTimbradoNomina> timbrado, string formato, string filtro)
        {
            List<string> files = new List<string>();
            if (formato == "PDF")
                files = GetPDFIdReg(timbrado, filtro);
            else if (formato == "XML")
                files = GetXMLIdReg(timbrado, filtro);


            return files;
        }

        /// <summary>
        ///     Método que obtiene los archivos solicitados por centro de costos
        /// </summary>
        /// <param name="timbrado">Lista con información del timbrado</param>
        /// <param name="formato">Formato que se va a descargar</param>
        /// <param name="filtro"></param>
        /// <returns>Erchivos solicitados</returns>
        public List<string> getFilesCC(List<vTimbradoNomina> timbrado, string formato, string filtro)
        {
            List<string> files = new List<string>();
            if (formato == "PDF")
                files = GetPDFIdCC(timbrado, filtro);
            else if (formato == "XML")
                files = GetXMLIdCC(timbrado, filtro);


            return files;
        }

        /// <summary>
        ///     Método que guarda en una ruta física los archivos y posteriormente los descarga
        /// </summary>
        /// <param name="timbrado">Lista con información del timbrado uy de los empleados</param>
        /// <param name="filtro"></param>
        /// <returns>Lista con las rutas de los archivos</returns>
        public List<string> GetPDFId(List<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();

            foreach (var item in timbrado)
            {   
                string xml = item.CFDI_Timbrado;
                string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "_" + item.RFC + "_" + item.ClaveEmpleado + "_" + item.IdEmpleado + ".pdf";
                
                string ruta = @"D:\TadaNomina\DescargaCFDINomina\" + item.IdPeriodoNomina;
                if (filtro != string.Empty) { ruta += @"\" + filtro; }
                string rutaArchivo = ruta + @"\" + NombreArchivo;
                
                if (!Directory.Exists(ruta))                
                    System.IO.Directory.CreateDirectory(ruta);                

                WS_CFDI cga = new WS_CFDI();
                cga.guardaPDF(item.CFDI_Timbrado, item.Leyenda, rutaArchivo, item.Firma, item.SueldoMensual, item.Direccion, item.SD, item.idSincatosClientes, item.BanderaSindicalizados, item.IdGrupo);
                lista.Add(rutaArchivo);
            }
            return lista;
        }

        /// <summary>
        ///     Método que guarda en una ruta física los archivos y posteriormente los descarga por registor patronal
        /// </summary>
        /// <param name="timbrado">Lista con información del timbrado uy de los empleados</param>
        /// <param name="filtro"></param>
        /// <returns>Lista con las rutas de los archivos</returns>
        public List<string> GetPDFIdReg(List<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            
            var listaregistro = timbrado.GroupBy(p => p.RegistroPatronal).Select(reg => new { RegistroPatronal = reg.Key, timbrado = reg.ToList() }).ToList();

            for (int i = 0; i < listaregistro.Count; i++)
            {
                var listadoxReg = timbrado.Where(x => x.RegistroPatronal.Equals(listaregistro[i].RegistroPatronal)).ToList();
                var patrona = GetRegistro(listaregistro[i].RegistroPatronal.ToString());
                string carpeta = @"D:\TadaNomina\DescargaCFDINomina\" + patrona.NombrePatrona.ToString() + "_" + patrona.RegistroPatronal.ToString();

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
                        Directory.CreateDirectory(ruta);

                    WS_CFDI cga = new WS_CFDI(); 
                    cga.guardaPDF(item.CFDI_Timbrado, item.Leyenda, rutaArchivo, item.Firma, item.SueldoMensual, item.Direccion, item.SD, item.idSincatosClientes, item.BanderaSindicalizados, item.IdGrupo);
                }
                lista.Add(carpeta);
            }
            return lista;
        }

        /// <summary>
        ///     Método que guarda en una ruta física los archivos y posteriormente los descarga por centro de costos
        /// </summary>
        /// <param name="timbrado">Lista con información del timbrado uy de los empleados</param>
        /// <param name="filtro"></param>
        /// <returns>Lista con las rutas de los archivos</returns>
        public List<string> GetPDFIdCC(List<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
           
            var listaregistro = timbrado.GroupBy(p => p.IdCentroCostos).Select(reg => new { IdCentroCostos = reg.Key, timbrado = reg.ToList() }).ToList();

            for (int i = 0; i < listaregistro.Count; i++)
            {
                var listadoxReg = timbrado.Where(x => x.IdCentroCostos.Equals(listaregistro[i].IdCentroCostos)).ToList();
                var cc = GetCC((int)listaregistro[i].IdCentroCostos);
                string carpeta = @"D:\TadaNomina\DescargaCFDINomina\" + cc.IdCentroCostos + "_" + cc.Clave + "_" + cc.CentroCostos.ToString();

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
                    cga.guardaPDF(item.CFDI_Timbrado, item.Leyenda, rutaArchivo, item.Firma, item.SueldoMensual, item.Direccion, item.SD, item.idSincatosClientes, item.BanderaSindicalizados, item.IdGrupo);
                }
                lista.Add(carpeta);
            }
            return lista;
        }

        public List<string> GetXMLId(IEnumerable<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            foreach (var item in timbrado)
            {
                string xml = item.CFDI_Timbrado;
                string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "_" + item.RFC + "_" + item.ClaveEmpleado + "_" + item.IdEmpleado + ".xml";
                string ruta = @"D:\TadaNomina\DescargaCFDINomina\" + item.IdPeriodoNomina;
                if (filtro != string.Empty) { ruta += @"\" + filtro; }
                string rutaArchivo = ruta + @"\" + NombreArchivo;

                if (!Directory.Exists(ruta))                
                    System.IO.Directory.CreateDirectory(ruta);                

                _xml.crearXML(xml, rutaArchivo);
                lista.Add(rutaArchivo);
            }

            return lista;
        }               

        public List<string> GetXMLIdReg(IEnumerable<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            
            var listaregistro = timbrado.GroupBy(p => p.RegistroPatronal).Select(reg => new { RegistroPatronal = reg.Key, timbrado = reg.ToList() }).ToList();

            for (int i = 0; i < listaregistro.Count; i++)
            {
                var listadoxReg = timbrado.Where(x => x.RegistroPatronal.Equals(listaregistro[i].RegistroPatronal)).ToList();
                var patrona = GetRegistro(listaregistro[i].RegistroPatronal.ToString());
                string carpeta = @"D:\TadaNomina\DescargaCFDINomina\" + patrona.NombrePatrona.ToString() + "_" + patrona.RegistroPatronal.ToString();

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

        public List<string> GetXMLIdCC(IEnumerable<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            
            var listaregistro = timbrado.GroupBy(p => p.IdCentroCostos).Select(reg => new { IdCentroCostos = reg.Key, timbrado = reg.ToList() }).ToList();

            for (int i = 0; i < listaregistro.Count; i++)
            {
                var listadoxReg = timbrado.Where(x => x.IdCentroCostos.Equals(listaregistro[i].IdCentroCostos)).ToList();
                var cc = GetCC((int)listaregistro[i].IdCentroCostos);
                string carpeta = @"D:\TadaNomina\DescargaCFDINomina\" + cc.IdCentroCostos + "_" + cc.Clave + "_" + cc.CentroCostos.ToString();

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

        public void SendCorreo(int idPeriodoNomina, string filtro, bool Valida)
        {            
            string ruta = string.Empty, rutaArchivo = string.Empty, NombreArchivo = string.Empty;
            string Message = string.Empty, Titulo = string.Empty, Correo = string.Empty, Modulo = string.Empty, rutaCorreo = string.Empty, correosCCO = string.Empty;
            string fileName = string.Empty;
            List<vTimbradoNomina> listvTimbrado = new List<vTimbradoNomina>();
            //OBTENER LISTA DE PDF
            using (TadaTimbradoEntities tada = new TadaTimbradoEntities())
            {
                listvTimbrado = tada.vTimbradoNomina.Where(n => n.IdPeriodoNomina == idPeriodoNomina).ToList();
            }
            List<string> files = new List<string>();
            files = getFilesSend(listvTimbrado, filtro, string.Empty);
            //OBTENER LISTA DE CORREOS DE EMPLEADO
            foreach (vTimbradoNomina vt in listvTimbrado)
            {
                //VALIDAR CORREO EMPLEADO
                string CorreoEmpleado = ObtenCorreoEmpleados((int)vt.IdEmpleado);


                if (CorreoEmpleado != null && CorreoEmpleado != string.Empty)
                {

                    if (filtro == "PDF")
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(@"D:\TadaNomina\DescargaCFDINomina\" + idPeriodoNomina + @"\" + "CFDI_" + idPeriodoNomina + "_" + vt.RFC + "_" + vt.ClaveEmpleado + "_" + vt.IdEmpleado + ".pdf");
                        fileName = (@"D:\TadaNomina\DescargaCFDINomina\" + idPeriodoNomina + @"\" + "CFDI_" + idPeriodoNomina + "_" + vt.RFC + "_" + vt.ClaveEmpleado + "_" + vt.IdEmpleado + ".pdf");
                        String[] archivos = { fileName };
                        //Enviar PDF
                        Message = "Estimad@ ha sido enviado su recibo Correspondiente al periodo del  " + vt.FechaInicioPeriodo + "al" + vt.FechaFinPeriodo;
                        Titulo = "Recibos CFDI";
                        Correo = CorreoEmpleado;
                        Modulo = "Recibos CFDI";
                        EnviarCorreoDispercion(Message, Titulo, Correo, Modulo, archivos, Valida);

                    }
                    else
                    {
                        byte[] fileBytes = File.ReadAllBytes(@"D:\TadaNomina\DescargaCFDINomina\" + idPeriodoNomina + @"\" + "CFDI_" + idPeriodoNomina + "_" + vt.RFC + "_" + vt.ClaveEmpleado + "_" + vt.IdEmpleado + ".xml");
                        fileName = (@"D:\TadaNomina\DescargaCFDINomina\" + idPeriodoNomina + @"\" + "CFDI_" + idPeriodoNomina + "_" + vt.RFC + "_" + vt.ClaveEmpleado + "_" + vt.IdEmpleado + ".xml");
                        String[] archivos = { fileName };
                        //Enviar PDF
                        Message = "Estimad@ ha sido enviado su recibo Correspondiente al periodo del  " + vt.FechaInicioPeriodo + "al" + vt.FechaFinPeriodo;
                        Titulo = "Recibos CFDI";
                        Correo = CorreoEmpleado;
                        Modulo = "Recibos CFDI";
                        EnviarCorreoDispercion(Message, Titulo, Correo, Modulo, archivos, Valida);
                    }
                }
            }


        }

        public List<string> getFilesSend(List<vTimbradoNomina> timbrado, string formato, string filtro)
        {
            List<string> files = new List<string>();
            if (formato == "PDF")
                files = GetPDFIdSend(timbrado, filtro);
            else if (formato == "XML")
                files = GetXMLIdSend(timbrado, filtro);


            return files;
        }

        public List<string> GetPDFIdSend(List<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            foreach (var item in timbrado)
            {
                string xml = item.CFDI_Timbrado;
                string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "_" + item.RFC + "_" + item.ClaveEmpleado + "_" + item.IdEmpleado + ".pdf";

                string ruta = @"D:\TadaNomina\DescargaCFDINomina\" + item.IdPeriodoNomina;
                if (filtro != string.Empty) { ruta += @"\" + filtro; }
                string rutaArchivo = ruta + @"\" + NombreArchivo;

                if (!Directory.Exists(ruta))
                    System.IO.Directory.CreateDirectory(ruta);

                WS_CFDI cga = new WS_CFDI();
                cga.guardaPDF(item.CFDI_Timbrado, item.Leyenda, rutaArchivo, item.Firma, item.SueldoMensual);
                lista.Add(rutaArchivo);
            }

            return lista;
        }

        private string ObtenCorreoEmpleados(int IdEmpleado)
        {
            TadaEmpleados context = new TadaEmpleados();

            var CorreoEmpleado = (from b in context.Empleados.Where(x => x.IdEmpleado == IdEmpleado) select b.CorreoElectronico).FirstOrDefault();

            return CorreoEmpleado;
        }

        public List<string> GetXMLIdSend(IEnumerable<vTimbradoNomina> timbrado, string filtro)
        {
            List<string> lista = new List<string>();
            foreach (var item in timbrado)
            {
                string xml = item.CFDI_Timbrado;
                string NombreArchivo = "CFDI_" + item.IdPeriodoNomina + "_" + item.RFC + "_" + item.ClaveEmpleado + "_" + item.IdEmpleado + ".xml";
                string ruta = @"D:\TadaNomina\DescargaCFDINomina\" + item.IdPeriodoNomina;
                if (filtro != string.Empty) { ruta += @"\" + filtro; }
                string rutaArchivo = ruta + @"\" + NombreArchivo;

                if (!Directory.Exists(ruta))
                    System.IO.Directory.CreateDirectory(ruta);

                _xml.crearXML(xml, rutaArchivo);
                lista.Add(rutaArchivo);
            }

            return lista;
        }


        public void EnviarCorreo(string Message, string Titulo, string Correo, string Modulo, string[] fileName, bool Valida)
        {
            bool Val = Valida;

            ClassSistema cs = new ClassSistema();
            var Send = cs.getSendCorreo();
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.To.Clear();
                msg.To.Add(Correo);

                if (fileName != null)
                {
                    if (fileName.Count() > 0)
                    {
                        foreach (var item in fileName)
                        {
                            Attachment data = new Attachment(item, MediaTypeNames.Application.Octet);
                            msg.Attachments.Add(data);
                        }
                    }
                }

                Val = true;

                msg.From = new MailAddress(Send.Credentials, Modulo, System.Text.Encoding.UTF8);
                msg.Subject = Titulo;
                msg.SubjectEncoding = System.Text.Encoding.UTF8;
                msg.Body = Message;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(Send.Credentials, Send.Contrasena);
                client.Port = int.Parse(Send.Puerto);
                client.Host = Send.ClientHost;
                client.EnableSsl = true;

                try
                {
                    client.Send(msg);

                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    throw new Exception("El correo electronico no pudo llegar a su destino debido a: " + ex.Message, ex);
                }


            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }



        }
        public void EnviarCorreoDispercion(string Message, string Titulo, string Correo, string Modulo, string[] fileName, bool Valida)
        {
            bool Val = Valida;

            ClassSistema cs = new ClassSistema();
            var Send = cs.getSendCorreoTada();
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.To.Clear();
                msg.To.Add(Correo);

                if (fileName != null)
                {
                    if (fileName.Count() > 0)
                    {
                        foreach (var item in fileName)
                        {
                            Attachment data = new Attachment(item, MediaTypeNames.Application.Octet);
                            msg.Attachments.Add(data);
                        }
                    }
                }

                Val = true;

                msg.From = new MailAddress(Send.Credentials, Modulo, System.Text.Encoding.UTF8);
                msg.Subject = Titulo;
                msg.SubjectEncoding = System.Text.Encoding.UTF8;
                msg.Body = Message;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(Send.Credentials, Send.Contrasena);
                client.Port = int.Parse(Send.Puerto);
                client.Host = Send.ClientHost;
                client.EnableSsl = true;

                try
                {
                    client.Send(msg);

                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    throw new Exception("El correo electronico no pudo llegar a su destino debido a: " + ex.Message, ex);
                }


            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }



        }

        /// <summary>
        ///     Método que genera la carpeta en zip
        /// </summary>
        /// <param name="items">archivos en base 64</param>
        /// <param name="destination">Carpeta de destino</param>
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
