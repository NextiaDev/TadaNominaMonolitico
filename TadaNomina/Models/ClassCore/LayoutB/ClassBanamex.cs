using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.LayoutBancos;

namespace TadaNomina.Models.ClassCore.LayoutB
{
    public class ClassBanamex
    {
        public string GetNombrePeriodoNomina(string IdPeriodoNomina)
        {
            int IdPeriodoN = int.Parse(IdPeriodoNomina);
            string query = "";
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = ctx.PeriodoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoN).Select(x => x.Periodo).FirstOrDefault();
            }
            return query;
        }

        public List<SelectListItem> GetPeriodosN(int IdUnidadNegocio)
        {
            var lista = listaPeriodosAcumulados(IdUnidadNegocio);

            List<SelectListItem> result = new List<SelectListItem>();

            foreach (var item in lista)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Periodo,
                    Value = item.IdPeriodoNomina.ToString()
                });
            }
            return result;
        }

        public List<PeriodoNomina> listaPeriodosAcumulados(int IdUnidadNegocio)
        {
            List<PeriodoNomina> query = new List<PeriodoNomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = ctx.PeriodoNomina.Where(p => p.IdEstatus == 2 && p.IdUnidadNegocio == IdUnidadNegocio).ToList();
            }
            return query;
        }

        public string GeneraTxtBanamex(int IdPeriodoNomina, int IdUnidadNegocio, string NumCliente, string ClvSucursal, string RefNum, string RefAlfaNum, string NombreEmpresa)
        {
            var listado = GetListaBanamex(IdPeriodoNomina, IdUnidadNegocio);
            var cuentaCargo = GetCuentaDispersion(listado[0].IdRegistroPatronal);

            DateTime FechaDispersion = (DateTime)listaPeriodosAcumulados(IdUnidadNegocio).Where(x => x.IdPeriodoNomina == IdPeriodoNomina).Select(x => x.FechaDispersion).FirstOrDefault();
            int linea = 1;
            string textoLinea1 = string.Empty;
            string textoEmpleado = string.Empty;
            string textoEmpleado2 = string.Empty;
            string textoFinal = string.Empty;
            decimal montoTotalDispersar = 0;
            listado.ForEach(p => {montoTotalDispersar = montoTotalDispersar + decimal.Parse(p.NetoPagar); }) ;
            string lineaFinal = string.Empty;

            textoLinea1 = linea + RellenaCadenaCeros(NumCliente, 12) + FechaDispersion.ToString("yy/MM/dd").Replace("/", "") + "0001" + RellenaCadenaEspacios(NombreEmpresa, 36) + RellenaCadenaEspacios("NOMINA", 20) + RellenaCadenaEspacios("15D01", 5);

            textoEmpleado = "2" + "1" + "001" + RellenaCadenaCeros(montoTotalDispersar.ToString(), 19).Replace(".", "") + "01000000000" + ClvSucursal + cuentaCargo + RellenaCadenaCeros(listado.Count.ToString(), 6);
            textoFinal += textoLinea1 + Environment.NewLine + textoEmpleado + Environment.NewLine;


            foreach (var item in listado)
            {
                string tipoDispersion = item.IdBancoEmpleado == 1 ? "001" : "002";
                string tipotransf = item.IdBancoEmpleado == 1 ? "03" : "40";
                string valAlfa = item.IdBancoEmpleado == 1 ? RefAlfaNum : "";
                linea++;
                textoEmpleado2 = "3" + "0" + tipoDispersion + "01001" + RellenaCadenaCeros(item.NetoPagar, 19).Replace(".", "") + tipotransf + RellenaCadenaCeros(item.NumeroCuenta, 20).Replace(".", "") + RellenaCadenaCeros(RefNum, 7) + RellenaCadenaEspacios(valAlfa, 9) + RellenaCadenaEspacios(item.Nombre + "," + item.ApellidoPaterno + "/" + item.ApellidoMaterno, 195).Replace("Ñ", "@") + "0" + item.ClaveBanco + "00" + RellenaCadenaEspacios("", 152);
                textoFinal += textoEmpleado2 + Environment.NewLine;
            }
            lineaFinal = "4" + "001" + RellenaCadenaCeros(listado.Count.ToString(), 6) + RellenaCadenaCeros(montoTotalDispersar.ToString(), 19).Replace(".", "") + "000001" + RellenaCadenaCeros(montoTotalDispersar.ToString(), 19).Replace(".", "") + Environment.NewLine;
            textoFinal += lineaFinal + Environment.NewLine;
            return textoFinal;
        }

        public List<ModelBanamex> GetListaBanamex(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            List<ModelBanamex> listaBanamex = new List<ModelBanamex>();
            var tipoNomina = GetTipoNomina(IdPeriodoNomina);
            var listaNom = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeriodoNomina, null) : GetNominaFiniquitos(IdPeriodoNomina);
            List<int> listaIdEmpleados = listaNom.Select(p => p.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            List<vEmpleados> listaInfoEmp = cemp.getvEmpleadosByListIds(listaIdEmpleados, IdUnidadNegocio);
            List<int> listaIdsInfoEmp = listaInfoEmp.Select(p => p.IdEmpleado).ToList();
            List<Nomina> listaNomsinInfo = listaNom.Where(p => p.CuentaBancariaTrad == null && p.CuentaInterbancariaTrad == null).ToList();
            listaNomsinInfo.ForEach(p => listaNom.Remove(p));
            List<vEmpleados> listaEmopleadossinInfo = listaInfoEmp.Where(p => p.CuentaBancariaTrad == null && p.CuentaInterbancariaTrad == null).ToList();
            listaEmopleadossinInfo.ForEach(p => listaInfoEmp.Remove(p));
            List<int?> GetIdBancoByEmpleados = listaInfoEmp.Select(p => p.IdBancoTrad).ToList();
            List<Cat_Bancos> listaBancosEmpleados = GetClaveBanco(GetIdBancoByEmpleados);
            List<Nomina> listaNominaEmpleadosUnidad = listaNom.Where(p => listaIdsInfoEmp.Contains(p.IdEmpleado)).ToList();
            foreach (var item in listaNominaEmpleadosUnidad)
            {
                var infoEmp = listaInfoEmp.Where(p => p.IdEmpleado == item.IdEmpleado).First();
                var infoBanco = listaBancosEmpleados.Where(p => p.IdBanco == infoEmp.IdBancoTrad).First();
                ModelBanamex model = new ModelBanamex();
                model.IdBancoEmpleado = infoBanco.IdBanco;
                model.NetoPagar = tipoNomina == "Finiquitos" ? item.Neto.ToString() : item.TotalEfectivo.ToString();
                model.NumeroCuenta = infoEmp.IdBancoTrad == 1  && infoEmp.CuentaBancariaTrad != null ? infoEmp.CuentaBancariaTrad : infoEmp.CuentaInterbancariaTrad;
                model.Nombre = infoEmp.Nombre;
                model.ApellidoPaterno = !string.IsNullOrEmpty(infoEmp.ApellidoPaterno) ? infoEmp.ApellidoPaterno : " ";
                model.ApellidoMaterno = !string.IsNullOrEmpty(infoEmp.ApellidoMaterno) ? infoEmp.ApellidoMaterno : " ";
                model.IdRegistroPatronal = (int)item.IdRegistroPatronal;
                model.FechaDispersion = (DateTime)item.FechaCaptura;
                model.ClaveBanco = infoEmp.IdBancoTrad == 1 ? "000" : infoBanco.ClaveBanco;
                listaBanamex.Add(model);
            }
            return listaBanamex;
        }

        public List<Cat_Bancos> GetClaveBanco(List<int?> GetIdBancoByEmpleados)
        {
            List<Cat_Bancos> listado = new List<Cat_Bancos>();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
              listado = ctx.Cat_Bancos.Where(p => GetIdBancoByEmpleados.Contains(p.IdBanco)).ToList();
            }
            return listado;
        }

        public string GetTipoNomina(int IdPeriodoNomina)
        {
            string query = string.Empty;
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = (from b in ctx.PeriodoNomina
                         where b.IdPeriodoNomina == IdPeriodoNomina && b.IdEstatus == 2
                         select b.TipoNomina).FirstOrDefault();
            }
            return query;
        }

        public List<Nomina> GetNominaNormal(int IdPeridoNomina, int? IdBanco)
        {
            List<Nomina> listado = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                if (IdBanco == null)
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeridoNomina && x.TotalEfectivo > 0).ToList();
                }
                else
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeridoNomina && x.TotalEfectivo > 0 && x.IdBancoTrad != IdBanco).ToList();
                }
            }
            return listado;
        }

        public List<Nomina> GetNominaFiniquitos(int IdPeridoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeridoNomina && x.CuentaBancariaTrad != null && x.Neto > 0 && x.IdEstatus == 1).ToList();
            }
        }

        public vEmpleados GetInfoEmpleado(int IdEmpleado)
        {
            vEmpleados query = new vEmpleados();
            using (TadaEmpleadosEntities ctx = new TadaEmpleadosEntities())
            {
                query = ctx.vEmpleados.Where(x => x.IdEmpleado == IdEmpleado).First();
            }
            return query;
        }

        public string GetCuentaDispersion(int IdRegistroPatronal)
        {
            string cuentaDispersa = string.Empty;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal).First();
                cuentaDispersa = query.CuentaBancaria;
            }
            return cuentaDispersa;
        }

        public Cat_RegistroPatronal GetInfoRP(int IdRegistroPatronal)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal && x.IdEstatus == 1).First();
            }
        }

        public int GetIdBancoByRP(int IdRegistroPatronal)
        {
            int idBanco = 0;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(x => x.IdBanco == IdRegistroPatronal && x.IdEstatus == 1).First();
                idBanco = int.Parse(query.IdBanco.ToString());
            }
            return idBanco;
        }

        private string RellenaCadenaEspacios(string pCadena, int pPosiciones)
        {
            if (pCadena.Length <= pPosiciones)
            {
                while (pCadena.Length < pPosiciones)
                {
                    pCadena += " ";
                }
                return pCadena;
            }
            else if (pCadena.Length > pPosiciones)
            {
                return pCadena.Substring(0, pPosiciones);
            }
            else
            {
                return pCadena.Substring(1, pPosiciones);
            }
        }

        private string RellenaCadenaCeros(string pCadena, int pPosiciones)
        {
            if (pCadena.Length <= pPosiciones)
            {
                while (pCadena.Length < pPosiciones)
                {
                    pCadena = "0" + pCadena;
                }
                return pCadena;
            }
            else if (pCadena.Length > pPosiciones)
            {
                return pCadena.Substring(0, pPosiciones);
            }
            else
            {
                return pCadena.Substring(1, pPosiciones);
            }
        }
    }
}