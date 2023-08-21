using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Dispersion;

namespace TadaNomina.Models.ClassCore.Dispersion
{
    public class ClassDispersion
    {
        /// <summary>
        ///     Método que obtiene el modelo de dispersión
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad negocio</param>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="IdPeriodo">Variable que contiene el id del periodo</param>
        /// <returns>Modelo con la información de la dispersión </returns>
        public ModelDispersa getModel(int IdUnidadNegocio, int IdCliente, int IdPeriodo)
        {            
            var model = new ModelDispersa();
            var idsReg = getRegistroPatronalesNomina(IdPeriodo);
            model.Saldos = getSaldo(IdCliente, idsReg);
            model.TotalDispersar = 0;
            model.lPeriodos = getList(IdUnidadNegocio);
            model.ListEmpleadosSinCuenta = new List<Nomina>();
            
            return model;
        }

        /// <summary>
        ///     Método que obtiene la lista de los periodos acumulados
        /// </summary>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad negocio</param>
        /// <returns>Lista con los periodos de nómina acumulados</returns>
        public List<SelectListItem> getList(int IdUnidadNegocio)
        {
            var cpn = new ClassPeriodoNomina();
            var list = cpn.GetSeleccionPeriodoAcumulado(IdUnidadNegocio);

            return list;
        }

        /// <summary>
        ///     Método que regresa los saldos de los registros patronales seleccionados
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="Ids">Variable que contiene los id's de los registros patronales</param>
        /// <returns>lista de modelos con la información de cada patrona y sus saldos</returns>
        public List<ModelPatrona> getSaldo(int IdCliente, int[] Ids)
        {
            var reg = getRegistrosPatronales(IdCliente, Ids);
            var mreg = new List<ModelPatrona>();

            reg.ForEach(x => { mreg.Add(new ModelPatrona { registroPatronal = x, saldo = 0 }); });
            
            return mreg;
        }

        /// <summary>
        ///     Método que obtiene el monto total para dispersar 
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Monto total a dispersar</returns>
        public decimal getTotalDispersar(int IdPeriodo)
        {
            var cn = new ClassNomina();
            var nom = cn.GetDatosNomina(IdPeriodo).Sum(x=> x.Neto);

            return (decimal)nom;
        }

        /// <summary>
        ///     Método que obtiene el total de movimientos en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Total de movimientos realizados en el periodo</returns>
        public int getTotalMovimientos(int IdPeriodo)
        {
            var cn = new ClassNomina();
            var nom = cn.GetDatosNomina(IdPeriodo).Count();

            return nom;
        }

        /// <summary>
        ///     Método que obtiene los empleados que no tienen registrada la clabe interbancaria
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Lista de empleados</returns>
        public List<Nomina> getEmpleadosSinCuenta(int IdPeriodo)
        {
            var cn = new ClassNomina();
            var nom = cn.GetDatosNomina(IdPeriodo).Where(x => x.CuentaInterbancariaTrad == null && x.Neto > 0).ToList();

            return nom;
        }

        /// <summary>
        ///     Método que obtiene los registros patronales
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Lista de id's de los registros patronales activos</returns>
        public int[] getRegistroPatronalesNomina(int IdPeriodo)
        {
            var cn = new ClassNomina();
            var nom = cn.GetDatosNomina(IdPeriodo).Where(x=> x.IdRegistroPatronal != null).Select(x=> (int)x.IdRegistroPatronal).Distinct().ToArray();

            return nom;
        }

        /// <summary>
        ///     Método que obtiene los registros patronales activos de un cliente
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <param name="Ids">Variable que contiene una lista con los id's de los registros patronales</param>
        /// <returns>Lista de modelos con la información de cada registro patronal</returns>
        public List<Cat_RegistroPatronal> getRegistrosPatronales(int IdCliente, int[] Ids)
        {
            var crp = new ClassRegistroPatronal();
            var registros = crp.GetRegistroPatronalByIdCliente(IdCliente).Where(x=> Ids.Contains(x.IdRegistroPatronal)).ToList();
            return registros;
        }
    }
}