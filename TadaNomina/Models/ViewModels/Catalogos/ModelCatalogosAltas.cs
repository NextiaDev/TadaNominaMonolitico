using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.Catalogos
{
    public class ModelCatalogosAltas
    {
        public List<Cat_CentroCostos> centroCostos { get; set; }
        public List<Cat_Departamentos> departamentos { get; set; }
        public List<Cat_Puestos> puestos { get; set; }
        public List<Cat_Sucursales> sucursales { get; set; }
        public List<Cat_Bancos> bancos { get; set; }
        public List<Cat_EntidadFederativa> entidades { get; set; }
        public List<Cat_RegistroPatronal> registroPat { get; set; }
        public List<Cat_Areas> areasPat { get; set; }
        public List<Sindicatos> Sindicatos { get; set; }
        public List<Cat_Jornadas> Jornadas { get; set; }

        public List<Cat_HonorariosFacturas> Cat_Hono { get; set; }

    }
}