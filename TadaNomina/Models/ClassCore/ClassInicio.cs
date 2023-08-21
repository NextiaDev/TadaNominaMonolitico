using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.TimbradoTP;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    ///Inicio
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassInicio
    {
        /// <summary>
        /// Obtiene la información que se muestra al inicio del proyecto de nómina 
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>información que se muestra al inicio del proyecto de nómina </returns>
        public ModelInicio getInfoInicio(int IdUnidadNegocio)
        {
            ModelInicio mi = new ModelInicio();

            ClassEmpleado cemp = new ClassEmpleado();
            ClassPeriodoNomina pn = new ClassPeriodoNomina();
            ClassNomina cn = new ClassNomina();
            ClassProcesosTimbradoTP pt = new ClassProcesosTimbradoTP();

            var periodos = pn.GetvPeriodoNominas(IdUnidadNegocio);
            var periodoNomTrab = periodos.LastOrDefault();
            int IdPeriodoNomAbierta = 0;
            try { IdPeriodoNomAbierta = periodoNomTrab.IdPeriodoNomina; } catch { }
            var nomTrabajo = cn.GetDataNominaTrabajo(IdPeriodoNomAbierta);

            var periodosAcum = pn.GetPeriodoNominasAcumulados(IdUnidadNegocio);
            var periodoNom = periodosAcum.LastOrDefault();
            int IdPeriodoNom = 0;
            try { IdPeriodoNom = periodoNom.IdPeriodoNomina; } catch { }
            var nom = cn.GetDatosNomina(IdPeriodoNom);

            mi.PeridosProcesados = 0;
            mi.PeridosProcesados = periodosAcum.Count();

            var emp = cemp.GetEmpleadoByUnidadNegocio(IdUnidadNegocio).Where(x=> x.IdEstatus == 1);
            mi.CantidadEmpleadosActivos = emp.Count();
            mi.empTradicionales = emp.Where(x => x.SDIMSS > 0).Count();
            mi.empMixto = emp.Where(x => x.SD > x.SDIMSS).Count();
            mi.empEsquema = emp.Where(x => x.SDIMSS == 0 && x.SD > 0).Count();

            mi.EmpleadosProcesadosC = nom.Count();
            try { mi.nombrePer = periodoNomTrab.Periodo; } catch { mi.nombrePer = string.Empty; }
            try { mi.NetoC = ((decimal)nom.Select(x => x.Neto).Sum()).ToString("C"); } catch { mi.NetoC = "0.00"; }
            try { mi.NetoSC = ((decimal)nom.Select(x => x.Netos).Sum()).ToString("C"); } catch { mi.NetoSC = "0.00"; }
            try { mi.IMSSC = ((decimal)nom.Select(x => x.Total_Patron).Sum()).ToString("C"); } catch { mi.IMSSC = "0.00"; }
            try { mi.ISRC = ((decimal)nom.Select(x => x.ImpuestoRetener).Sum()).ToString("C"); } catch { mi.IMSSC = "0.00"; }

            mi.EmpleadosProcesados = nomTrabajo.Count();
            try { mi.nombrePerC = periodoNom.Periodo; } catch { mi.nombrePerC = string.Empty; }
            try { mi.Neto = ((decimal)nomTrabajo.Select(x => x.Neto).Sum()).ToString("C"); } catch { mi.Neto = "0.00"; }
            try { mi.NetoS = ((decimal)nomTrabajo.Select(x => x.Netos).Sum()).ToString("C"); } catch { mi.NetoS = "0.00"; }
            try { mi.IMSS = ((decimal)nomTrabajo.Select(x => x.Total_Patron).Sum()).ToString("C"); } catch { mi.IMSS = "0.00"; }
            try { mi.ISR = ((decimal)nomTrabajo.Select(x => x.ImpuestoRetener).Sum()).ToString("C"); } catch { mi.IMSS = "0.00"; }

            mi.detalleTimbradoInicio = pt.getDetalleInicio(IdUnidadNegocio);

            return mi;
        }
    }
}