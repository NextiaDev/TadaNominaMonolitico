using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassSolicitudCheques
    {
        public List<v_Tesoreria_SolicitudCheque> getSolicitudesActivas(int IdUnidadNegocio)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var solicitudes = entidad.v_Tesoreria_SolicitudCheque.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio).OrderByDescending(x => x.IdSolicitudCheque).ToList();

                return solicitudes;
            }
        }

        public List<v_Tesoreria_SolicitudCheque> getSolicitudesFinalizadas(int IdUnidadNegocio)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var solicitudes = entidad.v_Tesoreria_SolicitudCheque.Where(x => x.IdEstatus == 2 && x.IdUnidadNegocio == IdUnidadNegocio).OrderByDescending(x=> x.IdSolicitudCheque).ToList();

                return solicitudes;
            }
        }

        public List<v_Tesoreria_SolicitudCheque> getSolicitudesRechazadas(int IdUnidadNegocio)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var solicitudes = entidad.v_Tesoreria_SolicitudCheque.Where(x => x.IdEstatus == 3 && x.IdUnidadNegocio == IdUnidadNegocio).OrderByDescending(x => x.IdSolicitudCheque).ToList();

                return solicitudes;
            }
        }

        public v_Tesoreria_SolicitudCheque getSolicitud(int IdSolicitudCheque)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var solicitudes = entidad.v_Tesoreria_SolicitudCheque.Where(x => x.IdSolicitudCheque == IdSolicitudCheque).FirstOrDefault();

                return solicitudes;
            }
        }

        public int newSolicitudCheque(int IdPeriodoNomina, string rutaArch, string Observaciones, int IdUsuario)
        {
            using (TadaContabilidadEntities entidad = new TadaContabilidadEntities())
            {
                var s = new SolicitudCheque()
                {
                    IdPeriodoNomina = IdPeriodoNomina,
                    rutaArchivo = rutaArch,
                    ObservacionesNominista = Observaciones,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.SolicitudCheque.Add(s);
                entidad.SaveChanges();

                return s.IdSolicitudCheque;
            }
        }

        public void guardarArchivo(int IdArchivo, HttpPostedFileBase archivo)
        {
            var ruta = Statics.rutaGralArchivos + @"SolicitdCheques\" + IdArchivo;
            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);

            archivo.SaveAs(ruta + @"\" + archivo.FileName);
        }
    }
}