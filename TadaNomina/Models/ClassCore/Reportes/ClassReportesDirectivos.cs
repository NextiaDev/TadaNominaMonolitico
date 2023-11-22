using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class ClassReportesDirectivos
    {
        public List<SelectListItem> GetLstPeriodos(int IdUnidadNegocio)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                lst = ctx.vPeriodoNomina.Where(x=>x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).Select(x=>new SelectListItem { Text = x.Periodo + "-"+x.TipoNomina, Value = x.IdPeriodoNomina.ToString()}).ToList();
            }
            return lst;
        }
    }
}