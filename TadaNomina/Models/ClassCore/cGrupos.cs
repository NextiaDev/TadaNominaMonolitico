using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class cGrupos
    {
        public List<Cat_Grupos> getGrupos()
        {
            using (TadaNominaEntities bd = new TadaNominaEntities())
            {
                var grupos = (from b in bd.Cat_Grupos
                              where b.IdEstatus == 1 
                            select b);

                return grupos.ToList();
            }
        }
    }
}