using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Contabilidad;
using TadaNomina.Models.ViewModels.LayoutBancos;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore.LayoutB
{
    public class ClassBanorte
    {
        public string GeneraTxtBanorte(int IdPeriodoNomina, int IdunidadNegocio, string Empresa)
        {
            var listado = GetListaBanorte(IdPeriodoNomina, IdunidadNegocio);
            try
            {
                var cuentaCargo = GetCuentaBancariaDispersion(listado[0].IdRegistroPatronal);
            }
            catch (Exception ex)
            {
                throw new Exception("No hay datos de la cuenta de dispersión");
            }

            DateTime fechaDispersa = (DateTime)listaPeriodosAcumulados(IdunidadNegocio).Where(x => x.IdPeriodoNomina == IdPeriodoNomina).Select(x => x.FechaDispersion).FirstOrDefault();
            string linea1 = string.Empty;
            decimal totalNomina = 0;
            string linea2 = string.Empty;
            string textoFinal = string.Empty;
            var yearDispersa = fechaDispersa.Year;
            var monthDispersa = fechaDispersa.Month;
            var dayDispersa = fechaDispersa.Day;


            totalNomina = netoNomina(listado);
            linea1 = "HNE" + Empresa + yearDispersa.ToString() + monthDispersa.ToString() + "/" + dayDispersa.ToString() + "01" + RellenaCadenaCeros(listado.Count().ToString(), 6) + RellenaCadenaCeros(totalNomina.ToString().Replace(".", ""), 15) + RellenaCadenaCeros("", 49) + "\n";

            foreach (var item in listado)
            {
                linea2 += "D";
                linea2 += yearDispersa.ToString() + monthDispersa.ToString() + "/" + dayDispersa.ToString() + RellenaCadenaCeros(item.IdEmpleado.ToString(), 10) + RellenaCadenaEspacios("", 80) + RellenaCadenaCeros(item.NetoPagar.ToString().Replace(".", ""), 15) + item.ClaveBanco + "01" + RellenaCadenaCeros(item.NumeroCuenta.ToString(), 18) + "0" + RellenaCadenaEspacios("", 1) + RellenaCadenaCeros("", 9) + "\n";

            }
            textoFinal += linea1 + linea2;
            return textoFinal;
        }

        public string GeneraTxtBanorteInter(int IdPeriodoNomina, int IdUnidadNegocio, string Empresa)
        {
            DateTime fechaDispersa = (DateTime)listaPeriodosAcumulados(IdUnidadNegocio).Where(x => x.IdPeriodoNomina == IdPeriodoNomina).Select(x => x.FechaDispersion).FirstOrDefault();
            var listado = GetListaBanorte(IdPeriodoNomina, IdUnidadNegocio).Where(x => x.IdBanco != 18 && x.NumeroCuentaInter != null && x.NetoPagar != null).Select(x => new MBanorte
            {
                NetoPagar = x.NetoPagar,
                IdEmpleado = x.IdEmpleado,
                IdBanco = x.IdBanco,
                ClaveBanco = x.ClaveBanco,
                NumeroCuenta = x.NumeroCuenta,
                NumeroCuentaInter = x.NumeroCuentaInter,
                FechaDispersion = x.FechaDispersion,
            }).ToList();
            listaPeriodosAcumulados(IdPeriodoNomina);
            string linea1 = string.Empty;
            decimal totalNomina = 0;
            string linea2 = string.Empty;
            string textoFinal = string.Empty;
            var yearDispersa = fechaDispersa.Year;
            var monthDispersa = fechaDispersa.Month;
            var dayDispersa = fechaDispersa.Day;

            totalNomina = netoNomina(listado);
            linea1 = "HNE" + Empresa + yearDispersa.ToString() + monthDispersa.ToString() + "/" + dayDispersa.ToString() + "01" + RellenaCadenaCeros(listado.Count().ToString(), 6) + RellenaCadenaCeros(totalNomina.ToString().Replace(".", ""), 15) + RellenaCadenaCeros("", 49);
            foreach (var item in listado)
            {
                linea2 += "D";
                linea2 += yearDispersa.ToString() + monthDispersa.ToString() + "/" + dayDispersa.ToString() + RellenaCadenaCeros(item.IdEmpleado.ToString(), 10) + RellenaCadenaEspacios("", 80) + RellenaCadenaCeros(item.NetoPagar.ToString().Replace(".", ""), 15) + item.ClaveBanco + "40" + RellenaCadenaCeros(item.NumeroCuentaInter.ToString(), 18) + "0" + RellenaCadenaEspacios("", 1) + RellenaCadenaCeros("", 9);
                linea1 = "";
            }
            textoFinal = linea1 + linea2 + Environment.NewLine;
            return textoFinal;
        }

        public List<MBanorte> GetListaBanorte(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            List<MBanorte> listaBanorte = new List<MBanorte>();
            var tipoNomina = GetTipoNomina(IdPeriodoNomina);
            var listaNomina = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeriodoNomina, null) : GetNominaFiniquitos(IdPeriodoNomina);
            List<int> listaEmpleados = listaNomina.Select(x => x.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            ClassBancomer cb = new ClassBancomer();
            var cuentaNominaEmpleado = cemp.getCuentaEmpleadoByNomina(IdPeriodoNomina, listaEmpleados);


            List<vEmpleados> listaInfoEmp = cemp.getvEmpleadosByListIds(listaEmpleados, IdUnidadNegocio).Where(x => x.IdBancoTrad == 18 && x.CuentaBancariaTrad != null).ToList();
            //List<vEmpleados> listaInfoEmp = cemp.getvEmpleadosByListIds(listaEmpleados, IdUnidadNegocio).Where(x => x.CuentaInterbancariaTrad != null || x.CuentaBancariaTrad != null && x.NetoPagar > 0).ToList();
            foreach (var item in listaInfoEmp)
            {
                Cat_Bancos claveBanco = cb.GetDatosBanco((int)item.IdBancoTrad);

                Nomina infoNom = listaNomina.Where(x => x.IdEmpleado == item.IdEmpleado).First();
                MBanorte model = new MBanorte();
                model.NetoPagar = tipoNomina == "Finiquitos" ? infoNom.Neto : infoNom.TotalEfectivo;
                model.IdEmpleado = item.IdEmpleado;
                model.NumeroCuenta = cuentaNominaEmpleado.Where(x => x.IdEmpleado == item.IdEmpleado).Select(x => x.CuentaBancariaTrad).First();
                model.Nombre = item.Nombre;
                model.ApellidoPaterno = item.ApellidoPaterno;
                model.ApellidoMaterno = item.ApellidoMaterno;
                model.IdRegistroPatronal = (int)item.IdRegistroPatronal;
                model.ClaveBanco = claveBanco.ClaveBanco;
                listaBanorte.Add(model);
            }
            return listaBanorte;
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

        public List<Nomina> GetNominaNormal(int IdPeriodoNomina, int? Idbanco)
        {
            List<Nomina> listado = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                if (Idbanco == null)
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null && x.TotalEfectivo > 0).ToList();
                }
                else
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaInterbancariaTrad == null && x.TotalEfectivo > 0 && x.IdBancoTrad != Idbanco).ToList();
                }
            }
            return listado;
        }

        public List<Nomina> GetNominaFiniquitos(int IdPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null & x.Neto > 0 && x.IdEstatus == 1).ToList();
            }
        }

        public string GetCuentaBancariaDispersion(int IdRegistroPatronal)
        {
            string cuentaDispersa = string.Empty;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal).First();
                cuentaDispersa = query.CuentaBancaria;
            }
            return cuentaDispersa;
        }

        public List<PeriodoNomina> listaPeriodosAcumulados(int IdUnidadNegocio)
        {
            List<PeriodoNomina> query = new List<PeriodoNomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = ctx.PeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).ToList();
            }
            return query;
        }

        public decimal netoNomina(List<MBanorte> model)
        {
            decimal netoT = 0;
            foreach (var item in model)
            {
                netoT += item.NetoPagar ?? 0;
            }
            return netoT;
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