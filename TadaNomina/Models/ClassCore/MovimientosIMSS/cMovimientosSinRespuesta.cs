using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cMovimientosSinRespuesta
    {
        public List<mMovimientosSinRespuesta> GetMovimientosSinRspuesta(int IdCliente)
        {
            List<mMovimientosSinRespuesta> listado = new List<mMovimientosSinRespuesta>();
            var listadoIDs = GetListadoRPbyCliente(IdCliente);
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.MovimientosIMSS.Where(p => p.Lote != null && p.RespuestaLote == null && p.IdEstatus == "1" && p.ClienteAdministrado == "N" && listadoIDs.Contains(p.IdRegistroPatronal)).ToList();
                foreach(var item in query)
                {
                    mMovimientosSinRespuesta mmsr = new mMovimientosSinRespuesta();
                    mmsr.IdMovimiento = item.IdMovimiento;
                    mmsr.IdEmpleado = item.IdEmpleado;
                    var infoEmp = GetInfoEmpleado(item.IdEmpleado);
                    mmsr.Nombre = infoEmp.Nombre;
                    mmsr.ApellidoPaterno = infoEmp.ApellidoPaterno;
                    mmsr.ApellidoMaterno = infoEmp.ApellidoMaterno;
                    mmsr.IdRegistroPatronal = item.IdRegistroPatronal;
                    var infoRP = GetInfoRP(item.IdRegistroPatronal);
                    mmsr.RegistroPatronal = infoRP.RegistroPatronal;
                    mmsr.NombrePatrona = infoRP.NombrePatrona;
                    mmsr.TipoMovimiento = item.TipoMovimiento;
                    mmsr.Fecha = item.Fecha;
                    mmsr.Lote = item.Lote;
                    listado.Add(mmsr);
                }
                return listado;
            }
        }

        public List<int> GetListadoRPbyCliente(int IdCliente)
        {
            List<int> listado = new List<int>();
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.IdCliente == IdCliente && p.IdEstatus == 1).ToList();
                foreach(var item in query)
                {
                    int id = item.IdRegistroPatronal;
                    listado.Add(id);
                }
                return listado;
            }
        }

        public Empleados GetInfoEmpleado(int IdEmpleado)
        {
            using(TadaEmpleadosEntities ctx = new TadaEmpleadosEntities())
            {
                var query = ctx.Empleados.Where(p => p.IdEmpleado == IdEmpleado).First();
                return query;
            }
        }

        public Cat_RegistroPatronal GetInfoRP(int IdRegistroPatronal)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.IdRegistroPatronal == IdRegistroPatronal).First();
                return query;
            }
        }

        public string AddRespuesta(int IdCliente)
        {
            try
            {
                var listado = GetMovimientosSinRspuesta(IdCliente);
                var listaxRegistro = listado.GroupBy(p => p.IdRegistroPatronal).ToList();
                foreach (var item in listaxRegistro)
                {
                    var listaxRegitroSelect = listado.Where(p => p.IdRegistroPatronal == item.Key).ToList();
                    var listadoxLote = listaxRegitroSelect.GroupBy(p => p.Lote).ToList();
                    foreach (var item2 in listadoxLote)
                    {
                        cConsultas cc = new cConsultas();
                        var respuestaAPI = cc.GetRespuestaDetalleLote(item.Key, item2.Key);
                        var listaxNSS = respuestaAPI.respuestaWebService.movimientosLote.GroupBy(p => p.nss).ToList();
                        foreach (var item3 in listaxNSS)
                        {
                            var NSSSelect = respuestaAPI.respuestaWebService.movimientosLote.Where(p => p.nss == item3.Key).First();
                            int IdMov = GetIdMov(NSSSelect.nss, item2.Key);
                            AddRespuestMov(IdMov, NSSSelect.codigoErrorMovimiento);
                        }
                    }
                }
                return "CORRECTO";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public int GetIdMov(string NSS, string Lote)
        {
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.MovimientosIMSS.Where(p => p.NSS == NSS && p.Lote == Lote).First();
                return query.IdMovimiento;
            }
        }

        public void AddRespuestMov(int IdMovimiento, string respuesta)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                DB.MovimientosIMSS movimiento = ctx.MovimientosIMSS.Where(p => p.IdMovimiento == IdMovimiento).FirstOrDefault();
                if (respuesta == "0")
                {
                    movimiento.RespuestaLote = int.Parse(respuesta);
                    movimiento.FechaRespuesta = DateTime.Now;
                    ctx.SaveChanges();
                }
                else
                {
                    movimiento.RespuestaLote = int.Parse(respuesta);
                    movimiento.CodigoERRORRespuesta = respuesta;
                    movimiento.FechaRespuesta = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
        }
    }
}