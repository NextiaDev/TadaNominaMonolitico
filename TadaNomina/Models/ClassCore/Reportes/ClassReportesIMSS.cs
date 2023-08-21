using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class ClassReportesIMSS
    {
        /// <summary>
        /// Crea una lista de tipo SelectListItem con 3 opciones (tipo de movimientos en IMSS) y las agrega al modelo ModelIDSE
        /// </summary>
        /// <returns></returns>
        public ModelIDSE getModelIDSE()
        {
            List<SelectListItem> listMov = new List<SelectListItem>();
            listMov.Add(new SelectListItem { Text="A-ALTAS", Value="A" });
            listMov.Add(new SelectListItem { Text = "B-BAJAS", Value = "B" });
            listMov.Add(new SelectListItem { Text = "C-CAMBIOS", Value = "C" });

            ModelIDSE midse = new ModelIDSE();
            midse.ltipoMovimiento = listMov;

            return midse;
        }
    }
}