using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cMovimientosProcesados
    {
        public List<SelectListItem> GetTiposMovientos()
        {
            List<SelectListItem> lista = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "TODOS", Value = "0"},
                new SelectListItem(){ Text = "EXITOSOS", Value = "1"},
                new SelectListItem(){ Text = "ERRONEOS", Value = "2"},
            };
            return lista;
        }

        public List<mMovimientosProcesados> GetMovimientosProcesados(mMovimientosProcesados modelo, int IdCliente)
        {
            var lista = new List<mMovimientosProcesados>();
            cMovimientosSinRespuesta cmsr = new cMovimientosSinRespuesta();
            var listadoIDs = cmsr.GetListadoRPbyCliente(IdCliente);
            DateTime fi = DateTime.Parse(modelo.fecha1);
            DateTime ff = DateTime.Parse(modelo.fecha2);
            switch (modelo.tipomov)
            {
                case "0":
                    lista = GetMovxFecha(listadoIDs, fi, ff);
                    break;
                case "1":
                    lista = GetMovExitosos(listadoIDs, fi, ff);
                    break;
                case "2":
                    lista = GetMovError(listadoIDs, fi, ff);
                    break;
            }
            return lista;
        }

        public List<mMovimientosProcesados> GetMovxFecha(List<int> ListadoIds, DateTime Fecha1, DateTime Fecha2)
        {
            List<mMovimientosProcesados> listaMov = new List<mMovimientosProcesados>();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var lista = (from a in ctx.MovimientosIMSS
                             from b in ctx.Empleados
                             where b.IdEmpleado == a.IdEmpleado
                             && a.Envio == 1
                             && a.Fecha >= Fecha1
                             && a.Fecha <= Fecha2
                             && ListadoIds.Contains(a.IdRegistroPatronal)
                             select new
                             {
                                 IdEmpleado = a.IdEmpleado,
                                 lote = a.Lote,
                                 RespuestaLote = a.RespuestaLote,
                                 RegistroPatronal = a.IdRegistroPatronal,
                                 NSS = b.Imss,
                                 ApellidoPaterno = b.ApellidoPaterno,
                                 ApellidoMaterno = b.ApellidoMaterno,
                                 Nombre = b.Nombre,
                                 FechaMovimiento = a.Fecha,
                                 FechaTransmision = a.FechaEnvio,
                                 TipoMovimiento = a.TipoMovimiento
                             }).ToList();
                lista.ForEach(p =>
                {
                    string Error = null;
                    string RP = null;
                    RP = GetNombrePatrona(p.RegistroPatronal);
                    if (p.RespuestaLote != 0 && p.RespuestaLote != null)
                    {
                        Error = GetNombreError(p.RespuestaLote);
                    }

                    mMovimientosProcesados mm = new mMovimientosProcesados();
                    mm.IdEmpleado = p.IdEmpleado.ToString();
                    mm.lote = p.lote;
                    mm.RespuestaLote = p.RespuestaLote;
                    mm.RegistroPatronal = p.RegistroPatronal.ToString();
                    mm.NSS = p.NSS;
                    mm.ApellidoPaterno = p.ApellidoPaterno.ToString();
                    if (p.ApellidoMaterno != null)
                    { mm.ApellidoMaterno = p.ApellidoMaterno.ToString(); }
                    mm.Nombre = p.Nombre.ToString();
                    mm.FechaMovimiento = p.FechaMovimiento.ToShortDateString();
                    mm.FechaTransmision = p.FechaTransmision.ToString();
                    mm.TipoMovimiento = p.TipoMovimiento;
                    mm.NombreERROR = Error;
                    mm.NombrePatrona = RP;

                    listaMov.Add(mm);
                });
                return listaMov;
            }
        }

        public List<mMovimientosProcesados> GetMovExitosos(List<int> ListadoIds, DateTime Fecha1, DateTime Fecha2)
        {
            List<mMovimientosProcesados> listaMov = new List<mMovimientosProcesados>();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var lista = (from a in ctx.MovimientosIMSS
                             from b in ctx.Empleados
                             where b.IdEmpleado == a.IdEmpleado
                             && a.Envio == 1
                             && a.RespuestaLote == 1
                             && a.Fecha >= Fecha1
                             && a.Fecha <= Fecha2
                             && ListadoIds.Contains(a.IdRegistroPatronal)
                             select new
                             {
                                 IdEmpleado = a.IdEmpleado,
                                 lote = a.Lote,
                                 RespuestaLote = a.RespuestaLote,
                                 RegistroPatronal = a.IdRegistroPatronal,
                                 NSS = b.Imss,
                                 ApellidoPaterno = b.ApellidoPaterno,
                                 ApellidoMaterno = b.ApellidoMaterno,
                                 Nombre = b.Nombre,
                                 FechaMovimiento = a.Fecha,
                                 FechaTransmision = a.FechaEnvio,
                                 TipoMovimiento = a.TipoMovimiento
                             }).ToList();
                lista.ForEach(p =>
                {
                    string Error = null;
                    string RP = null;
                    RP = GetNombrePatrona(p.RegistroPatronal);

                    mMovimientosProcesados mm = new mMovimientosProcesados();
                    mm.IdEmpleado = p.IdEmpleado.ToString();
                    mm.lote = p.lote;
                    mm.RespuestaLote = p.RespuestaLote;
                    mm.RegistroPatronal = p.RegistroPatronal.ToString();
                    mm.NSS = p.NSS;
                    mm.ApellidoPaterno = p.ApellidoPaterno.ToString();
                    if (p.ApellidoMaterno != null)
                    { mm.ApellidoMaterno = p.ApellidoMaterno.ToString(); }
                    mm.Nombre = p.Nombre.ToString();
                    mm.FechaMovimiento = p.FechaMovimiento.ToShortDateString();
                    mm.FechaTransmision = p.FechaTransmision.ToString();
                    mm.TipoMovimiento = p.TipoMovimiento;
                    mm.NombreERROR = Error;
                    mm.NombrePatrona = RP;

                    listaMov.Add(mm);
                });
                return listaMov;
            }
        }

        public List<mMovimientosProcesados> GetMovError(List<int> ListadoIds, DateTime Fecha1, DateTime Fecha2)
        {
            List<mMovimientosProcesados> listaMov = new List<mMovimientosProcesados>();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var lista = (from a in ctx.MovimientosIMSS
                             from b in ctx.Empleados
                             where b.IdEmpleado == a.IdEmpleado
                             && a.Envio == 1
                             && a.CodigoERRORRespuesta != null
                             && a.Fecha >= Fecha1
                             && a.Fecha <= Fecha2
                             && ListadoIds.Contains(a.IdRegistroPatronal)
                             select new
                             {
                                 IdEmpleado = a.IdEmpleado,
                                 lote = a.Lote,
                                 RespuestaLote = a.RespuestaLote,
                                 RegistroPatronal = a.IdRegistroPatronal,
                                 NSS = b.Imss,
                                 ApellidoPaterno = b.ApellidoPaterno,
                                 ApellidoMaterno = b.ApellidoMaterno,
                                 Nombre = b.Nombre,
                                 FechaMovimiento = a.Fecha,
                                 FechaTransmision = a.FechaEnvio,
                                 TipoMovimiento = a.TipoMovimiento
                             }).ToList();
                lista.ForEach(p =>
                {
                    string Error = null;
                    string RP = null;
                    RP = GetNombrePatrona(p.RegistroPatronal);

                    mMovimientosProcesados mm = new mMovimientosProcesados();
                    mm.IdEmpleado = p.IdEmpleado.ToString();
                    mm.lote = p.lote;
                    mm.RespuestaLote = p.RespuestaLote;
                    mm.RegistroPatronal = p.RegistroPatronal.ToString();
                    mm.NSS = p.NSS;
                    mm.ApellidoPaterno = p.ApellidoPaterno.ToString();
                    if (p.ApellidoMaterno != null)
                    { mm.ApellidoMaterno = p.ApellidoMaterno.ToString(); }
                    mm.Nombre = p.Nombre.ToString();
                    mm.FechaMovimiento = p.FechaMovimiento.ToShortDateString();
                    mm.FechaTransmision = p.FechaTransmision.ToString();
                    mm.TipoMovimiento = p.TipoMovimiento;
                    mm.NombreERROR = Error;
                    mm.NombrePatrona = RP;

                    listaMov.Add(mm);
                });
                return listaMov;
            }
        }

        public string GetNombrePatrona(int IdRegistroPatronal)
        {
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.IdRegistroPatronal == IdRegistroPatronal).First();
                return query.NombrePatrona;
            }
        }

        public string GetNombreError(int? codigo)
        {
            string CE = codigo.ToString();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_ErroresIMSS.Where(p => p.CodigoERROR == CE).FirstOrDefault();
                return query.Descripcion;
            }
        }
    }
}