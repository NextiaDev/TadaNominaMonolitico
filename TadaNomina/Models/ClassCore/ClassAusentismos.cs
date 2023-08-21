using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassAusentismos
    {
        public List<vAusentismos> getAusentismos(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var ausentismos = (from b in entidad.vAusentismos.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == IdUnidadNegocio) select b).ToList();

                return ausentismos;
            }
        }

        //public List<ModelAusentismos> getModelAusentismos(int IdUnidadNegocio)
        //{
        //    List<ModelAusentismos> lAu = new List<ModelAusentismos>();
        //    var au = getAusentismos(IdUnidadNegocio);

        //    au.ForEach(x=> { lAu.Add(new ModelAusentismos { 
        //        IdAusentismo = x.IdAusentismo,
        //        IdTipoAusentismo = x.IdTipoAusentismo,
        //        IdTipoIncapacidad = x.IdTipoIncapacidad,
        //        IdEmpleado = x.IdEmpleado,
        //        ClaveEmp =  x.ClaveEmpleado,
        //        Empleado = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
        //        Ausentismo = x.TipoAusentismo,
        //        FechaInicial = x.FechaInicial,
        //        FechaFinal = x.FechaFinal,
        //        Folio = x.Folio,
        //        Observaciones = x.Observaciones
        //    }); });

        //    return lAu;
        //}
    }
}