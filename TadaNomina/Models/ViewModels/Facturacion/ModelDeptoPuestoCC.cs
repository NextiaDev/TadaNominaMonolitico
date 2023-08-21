using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ViewModels.CFDI
{
    public class ModelDeptoPuestoCC
    {

        //tabla Centro de Costos
        public int IdCentroCostos { get; set; }
        public string CentroCostos { get; set; }


        //tabla Puestos
        public int IdPuesto { get; set; }
        public string Puesto { get; set; }

        //Tabla Departamentos

        public int IdDepartamento { get; set; }
        public string Departamento { get; set; }

        public int IdRegistroPatronal { get; set; }
        public string NombrePatrona { get; set; }


        public string Division { get; set; }
        public string DivisionPatronal { get; set; }


        public List<ModelDeptoPuestoCC> ListarPuestos(int IdCliente)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return (from a in ctx.Cat_Puestos
                        where a.IdCliente == IdCliente && a.IdEstatus == 1
                        select new ModelDeptoPuestoCC
                        {
                            IdPuesto = a.IdPuesto,
                            Puesto = a.Puesto,
                            Division = "PP"
                        }).ToList();
            }
        }
        public List<ModelDeptoPuestoCC> ListarCentroCostos(int IdCliente)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.Cat_CentroCostos
                        where a.IdCliente == IdCliente && a.IdEstatus == 1
                        select new ModelDeptoPuestoCC
                        {
                            IdCentroCostos = a.IdCentroCostos,
                            CentroCostos = a.CentroCostos,
                            Division = "CC"
                        }).ToList();
            }
        }

        public List<ModelDeptoPuestoCC> ListarDeptos(int IdCliente)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.Cat_Departamentos
                        where a.IdCliente == IdCliente && a.IdEstatus == 1
                        select new ModelDeptoPuestoCC
                        {
                           IdDepartamento = a.IdDepartamento,
                           Departamento = a.Departamento,
                            Division = "DD"
                        }).ToList();
            }
        }

        public List<ModelDeptoPuestoCC> ListarPatronas(int IdCliente)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return (from a in ctx.Cat_RegistroPatronal
                        where a.IdCliente == IdCliente && a.IdEstatus == 1
                        select new ModelDeptoPuestoCC
                        {
                            IdRegistroPatronal = a.IdRegistroPatronal,
                            NombrePatrona = a.NombrePatrona,
                            Division = "DD"
                        }).ToList();
            }
        }
    }
}