using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ViewModels.RelojChecador;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.RelojChecador
{
    public class ClassAccesosGV
    {
        public AccesosGVModel DatosGV(int _IdCliente)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.Cat_Accesos_GeoVictoria
                        where a.IdCliente == _IdCliente && a.IdEstatus == 1
                        select new AccesosGVModel
                        {
                            IdAccesosGeovictoria = a.IdAccesoGV,
                            ClaveAPI = a.ClaveAPI,
                            Secreto = a.Secreto,
                            IdEstatus = a.IdEstatus
                        }).FirstOrDefault();
            }
        }
    }
}