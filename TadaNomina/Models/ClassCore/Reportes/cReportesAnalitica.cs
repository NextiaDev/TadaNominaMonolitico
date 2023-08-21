using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Reportes;

namespace TadaNomina.Models.ClassCore.Reportes
{
    public class cReportesAnalitica
    {
        public ModelAnalitica GetAnaliticaByIdCliente(int IdCliente)
        {
            ModelAnalitica m = new ModelAnalitica();

            using (TadaNominaEntities entities = new TadaNominaEntities())
            {
                var model = (from b in entities.Cat_Clientes
                             where b.IdEstatus == 1 && b.IdCliente == IdCliente
                             select b).FirstOrDefault();

                if (model != null)
                {
                    m.IdCliente = model.IdCliente;
                    m.Analitica = model.Analitica;
                }

                return m;
            }
        }
    }
}