using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore
{
    public class ClassBancos
    {
        public List<Cat_Bancos> getBancos()
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                var list = (from b in entidad.Cat_Bancos.Where(x => x.IdEstatus == 1) select b).ToList();

                return list;
            }
        }

        public List<Cat_Bancos> getBancosByIdCliente(int IdCliente)
        {
            List<Cat_Bancos> result = new List<Cat_Bancos>();
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                List<int?> IdBancosCliente = ctx.Cat_RegistroPatronal.Where(p => p.IdCliente == IdCliente && p.IdEstatus == 1).Select(p => p.IdBanco).ToList();
                result = ctx.Cat_Bancos.Where(p => IdBancosCliente.Contains(p.IdBanco)).ToList();
            }
            return result;
        }

        public DataTable GetTableBancos()
        {
            DataTable dt = new DataTable();
            var bancos = getBancos();

            using (var reader = ObjectReader.Create(bancos))
            {
                dt.Load(reader);
            }

            return dt;
        }

        public List<ModelRegistroPatronal> _LBancos()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                return (from b in entidad.Cat_Bancos
                        where b.IdEstatus == 1
                        select new ModelRegistroPatronal
                        {
                            IdBanco = b.IdBanco,
                            NombreCorto = b.NombreCorto,
                        }).ToList();
            }
        }

        public List<ModelPeriodoNomina> LPeriodoNomina()
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return (from b in entidad.PeriodoNomina
                        where b.IdEstatus == 2
                        select new ModelPeriodoNomina
                        {
                            IdPeriodoNomina = b.IdPeriodoNomina,
                            Periodo = b.Periodo,

                        }).ToList();
            }
        }

        public List<SelectListItem> getSelectBancos()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            List<Cat_Bancos> listBancos = getBancos();
            listBancos.ForEach(p => { result.Add(new SelectListItem { Text = p.NombreCorto, Value = p.IdBanco.ToString() }); });
            return result;
        }

        public List<SelectListItem> getSelectBancosCliente(int IdCliente)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            List<Cat_Bancos> listBancos = getBancosByIdCliente(IdCliente);
            listBancos.ForEach(p => { result.Add(new SelectListItem { Text = p.NombreCorto, Value = p.IdBanco.ToString() }); });
            return result;
        }
    }
}