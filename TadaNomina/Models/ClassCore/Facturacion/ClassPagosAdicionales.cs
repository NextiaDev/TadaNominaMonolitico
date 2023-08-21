using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.Facturacion
{
    public class ClassPagosAdicionales
    {
        public List<v_Tesoreria_PagosAdicionales> getPagosAdicionales(int IdUnidadNegocio)
        {
            using (var entidad = new TadaContabilidadEntities())
            {
                var pagos = entidad.v_Tesoreria_PagosAdicionales.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1).OrderByDescending(x=> x.IdPagoAdicional).Take(500).ToList();

                return pagos;
            }
        }

        public List<v_Tesoreria_PagosAdicionales> getPagosAdicionalesFinalizados(int IdUnidadNegocio)
        {
            using (var entidad = new TadaContabilidadEntities())
            {
                var pagos = entidad.v_Tesoreria_PagosAdicionales.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).OrderByDescending(x => x.IdPagoAdicional).Take(500).ToList();

                return pagos;
            }
        }

        public List<v_Tesoreria_PagosAdicionales> getPagosAdicionalesRechazados(int IdUnidadNegocio)
        {
            using (var entidad = new TadaContabilidadEntities())
            {
                var pagos = entidad.v_Tesoreria_PagosAdicionales.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 3).OrderByDescending(x => x.IdPagoAdicional).Take(500).ToList();

                return pagos;
            }
        }

        public v_Tesoreria_PagosAdicionales getPagoAdicional(int IdPagoAdicional)
        {
            using (var entidad = new TadaContabilidadEntities())
            {
                var pagos = entidad.v_Tesoreria_PagosAdicionales.Where(x => x.IdPagoAdicional == IdPagoAdicional).FirstOrDefault();

                return pagos;
            }
        }

        public int NewPagoAdicional(int IdCliente, int IdUnidadNegocio, string Comentarios, string Archivo, int IdUsuario)
        {
            using (var entidad = new TadaContabilidadEntities())
            {
                var pa = new PagosAdicionales() 
                {
                    IdCliente = IdCliente,
                    IdUnidadNegocio = IdUnidadNegocio,
                    Archivo = Archivo,
                    ObservacionesNomina = Comentarios,
                    IdEstatus = 1,
                    IdCaptura = IdUsuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.PagosAdicionales.Add(pa);
                entidad.SaveChanges();

                return pa.IdPagoAdicional;
            }
        }

        public void guardarArchivo(int IdSolicitud, HttpPostedFileBase archivo)
        {
            var ruta = Statics.rutaGralArchivos + @"PagosAdicionales\" + IdSolicitud;
            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);

            archivo.SaveAs(ruta + @"\" + archivo.FileName);
        }
    }
}