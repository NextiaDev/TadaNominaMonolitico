using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class ClassAreas
    {

        public List<Cat_Areas> getAreas()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var list = (from b in entidad.Cat_Areas select b).ToList();

                return list;
            }
        }

        public List<Cat_Areas> getAreas(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var list = (from b in entidad.Cat_Areas where b.IdCliente == IdCliente select b).ToList();

                return list;
            }
        }

        public DataTable GetTableAreas()
        {
            DataTable dt = new DataTable();
            var ar = getAreas();

            using (var reader = ObjectReader.Create(ar))
            {
                dt.Load(reader);
            }

            return dt;
        }
    }
}
