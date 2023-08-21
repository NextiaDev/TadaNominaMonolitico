using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Facturacion;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassArchivosAltas
    {
        /// <summary>
        ///     Método que obtiene los archivos de las altas
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns></returns>
        public List<vw_ArchivosAltaCuentas> getArchivosAltas(int IdUnidadNegocio)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                int[] status = { 1, 2, 3, 4, 5 };
                var arch = entidad.vw_ArchivosAltaCuentas.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && status.Contains((int)x.IdEstatus)).OrderByDescending(x=> x.IdArchivoAltas).Take(500).ToList();

                return arch;
            }
        }

        /// <summary>
        ///     Método que obtiene los archivos de las altas
        /// </summary>
        /// <param name="IdArchivoAltas">Variable que contiene el id del archivo</param>
        /// <returns>Modelo con la información del archivo</returns>
        public vw_ArchivosAltaCuentas getArchivoAltas(int IdArchivoAltas)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var arch = entidad.vw_ArchivosAltaCuentas.Where(x => x.IdArchivoAltas == IdArchivoAltas).FirstOrDefault();

                return arch;
            }
        }

        /// <summary>
        ///     Método que genera un nuevo registro de alta
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="ruta">Variable que contiene la ruta donde se guardará el archivo</param>
        /// <param name="Observaciones">Variable que contiene las observaciones del alta</param>
        /// <param name="IdUsuario">Variable que contiene el id de la sesión del usuario</param>
        /// <returns>El id del archivo generado</returns>
        public int newArchivoAltas(int IdCliente, int IdUnidadNegocio, string ruta, string Observaciones, int IdUsuario)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var a = new ArchivoAltaCuentas()
                {
                    IdCliente = IdCliente,
                    IdUnidadNegocio = IdUnidadNegocio,
                    Ruta = ruta,
                    Observaciones = Observaciones,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.ArchivoAltaCuentas.Add(a);
                entidad.SaveChanges();

                return a.IdArchivoAltas;
            }
        }

        /// <summary>
        ///     Método que guarda el archivo en una rua del servidor
        /// </summary>
        /// <param name="IdArchivo">Variable que contiene el id del archivo</param>
        /// <param name="archivo">Variable que contiene el archivo</param>
        public void guardarArchivo(int IdArchivo, HttpPostedFileBase archivo)
        {
            var ruta = Statics.rutaGralArchivos + @"ArchivosAltas\" + IdArchivo;
            if (!Directory.Exists(ruta))            
                Directory.CreateDirectory(ruta);

            archivo.SaveAs(ruta + @"\" + archivo.FileName);
        }

        /// <summary>
        ///     Método que obtiene un archivo en base64
        /// </summary>
        /// <param name="IdArchivo">Variable que contiene el id del archivo</param>
        /// <param name="reg">Variable que contiene el archivo</param>
        /// <returns>Arreglo de bytes con el archivo</returns>
        public byte[] getFile(int IdArchivo, vw_ArchivosAltaCuentas reg)
        {
            var ruta = Statics.rutaGralArchivos + @"ArchivosAltas\" + IdArchivo + @"\";             
            var arch = File.ReadAllBytes(ruta + reg.Ruta);

            return arch;
        }        
    }
}