using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.LayoutBancos;
using TadaNomina.Services;
using static ClosedXML.Excel.XLPredefinedFormat;
using DateTime = System.DateTime;

namespace TadaNomina.Models.ClassCore.LayoutB
{
    public class ClassBancomer
    {
        public List<ModelBancomer> GetListaBancomer(int IdPeridoNomina, int IdUnidadNegocio)
        {
            List<ModelBancomer> listaBancomers = new List<ModelBancomer>();
            var tipoNomina = GetTipoNomina(IdPeridoNomina);
            var listadoNom = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeridoNomina) : GetNominaFiniquitos(IdPeridoNomina);
            List<int> listaIdEmpleados = listadoNom.Select(p => p.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            List<vEmpleados> listaInfoEmp = cemp.getvEmpleadosByListIds(listaIdEmpleados, IdUnidadNegocio);
            List<vEmpleados> listaEmpleadosMismoBanco = listaInfoEmp.Where(p => p.IdBancoTrad == 4).ToList();
            foreach (var item in listaEmpleadosMismoBanco)
            {
                Nomina infoNom = listadoNom.Where(p => p.IdEmpleado == item.IdEmpleado).First();
                ModelBancomer model = new ModelBancomer();
                if (item.CuentaBancariaTrad != null)
                {
                    if (tipoNomina == "Finiquitos")
                    {
                        model.NetoPagar = infoNom.Neto.ToString();
                    }
                    else
                    {
                        model.NetoPagar = infoNom.TotalEfectivo.ToString();
                    }
                    model.NumeroCuenta = item.CuentaBancariaTrad;
                    model.NombreCompleto = item.NombreCompleto;
                    listaBancomers.Add(model);
                }
            }
            return listaBancomers;
        }

        public List<ModelBancomer> GetListaBancomerInterBancaria(int IdPeridoNomina, int IdUnidadNegocio)
        {
            List<ModelBancomer> listaBancomers = new List<ModelBancomer>();
            var tipoNomina = GetTipoNomina(IdPeridoNomina);
            var listadoNom = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeridoNomina) : GetNominaFiniquitos(IdPeridoNomina);
            List<int> listaIdEmpleados = listadoNom.Select(p => p.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            List<vEmpleados> listaEmpleados = cemp.getvEmpleadosByListIds(listaIdEmpleados, IdUnidadNegocio);
            List<vEmpleados> EmpleadosOtrosBanco = listaEmpleados.Where(p => p.IdBancoTrad != 4).ToList();
            foreach (var item in EmpleadosOtrosBanco)
            {
                Nomina infoNomina = listadoNom.Where(p => p.IdEmpleado == item.IdEmpleado).First();
                ModelBancomer model = new ModelBancomer();
                if (item.CuentaInterbancariaTrad != null || string.IsNullOrEmpty(item.CuentaInterbancariaTrad))
                {
                    if (tipoNomina == "Finiquitos")
                    {
                        model.NetoPagar = infoNomina.Neto.ToString();
                    }
                    else
                    {
                        model.NetoPagar = infoNomina.TotalEfectivo.ToString();
                    }
                    model.NombreCompleto = item.NombreCompleto;
                    model.NumeroCuenta = infoNomina.CuentaInterbancariaTrad;
                    model.IdRegistroPatronal = (int)infoNomina.IdRegistroPatronal;
                    model.ClaveBanco = (int)infoNomina.IdBancoTrad;
                    listaBancomers.Add(model);
                }
            }
            return listaBancomers;
        }

        public List<Nomina> GetNominaFiniquitos(int IdPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return ctx.Nomina.Where(p => p.IdPeriodoNomina == IdPeriodoNomina && p.CuentaBancariaTrad != null && p.Neto > 0 && p.IdEstatus == 1).ToList();
            }
        }

        public List<Nomina> GetNominaNormal(int IdPeriodoNomina)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return ctx.Nomina.Where(p => p.IdPeriodoNomina == IdPeriodoNomina && p.CuentaBancariaTrad != null && p.TotalEfectivo > 0).ToList();
            }
        }

        public vEmpleados GetInfoEmpleado(int IdEmpleado)
        {
            vEmpleados query = new vEmpleados();
            using (TadaEmpleadosEntities ctx = new TadaEmpleadosEntities())
            {
                query = ctx.vEmpleados.Where(p => p.IdEmpleado == IdEmpleado).First();
            }
            return query;
        }

        public string GeneraTxtBBVA(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            Otros ot = new Otros();

            var listado = GetListaBancomer(IdPeriodoNomina, IdUnidadNegocio);
            int registro = 0;
            string txtfinal = string.Empty;

            foreach (var item in listado)
            {
                registro++;
                string textoempleado = string.Empty;
                textoempleado += RellenaCadenaCeros(registro.ToString(), 9);
                textoempleado += RellenaCadenaEspacios("", 16);
                textoempleado += ("99");
                textoempleado += RellenaCadenaEspacios(item.NumeroCuenta, 20);
                textoempleado += RellenaCadenaCeros(item.NetoPagar.Replace(".", ""), 15);
                item.NombreCompleto =  ot.RemueveAcentos(item.NombreCompleto);
                textoempleado += RellenaCadenaEspacios(item.NombreCompleto.Replace("Ñ", "N").Replace(".", " "), 40);
                textoempleado += RellenaCadenaEspacios("012", 3);
                textoempleado += RellenaCadenaEspacios("000", 3);

                txtfinal += textoempleado + Environment.NewLine;
            }
            return txtfinal;
        }

        public string GeneraTxtBBVAInterbancario(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            Otros ot = new Otros();

            var listado = GetListaBancomerInterBancaria(IdPeriodoNomina, IdUnidadNegocio);
            string txtfinal = string.Empty;
            var cuentaDispersion = GetCuentaBancariaDispersion(listado[0].IdRegistroPatronal);
            var nombrePeriodo = GetNombrePeriodoNomina(IdPeriodoNomina.ToString());
            DateTime FechaDispersion = (DateTime)listaPeriodosAcumulados(IdUnidadNegocio).Where(x => x.IdPeriodoNomina == IdPeriodoNomina).Select(x => x.FechaDispersion).FirstOrDefault();

            foreach (var item in listado)
            {
                Cat_Bancos claveBanco = GetDatosBanco(item.ClaveBanco);

                string textoempleado = string.Empty;
                textoempleado += RellenaCadenaCeros(item.NumeroCuenta.ToString(), 18);
                textoempleado += RellenaCadenaCeros(cuentaDispersion, 18);
                textoempleado += ("MXP");
                textoempleado += RellenaCadenaCeros(item.NetoPagar, 16);
                item.NombreCompleto = ot.RemueveAcentos(item.NombreCompleto);
                textoempleado += RellenaCadenaEspacios(item.NombreCompleto.Replace("Ñ", "N").Replace(".", " "), 28);
                textoempleado += ("  40");
                textoempleado += claveBanco.ClaveBanco;
                textoempleado += RellenaCadenaEspacios("Pago de Nomina", 30);
                textoempleado += ("0");
                textoempleado += FechaDispersion.ToString("dd-MM-yy").Replace("-", "");
                textoempleado += ("H");

                txtfinal += textoempleado + Environment.NewLine;
            }
            return txtfinal;
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


        public List<PeriodoNomina> listaPeriodosAcumulados(int IdUnidadNegocio)
        {
            List<PeriodoNomina> query = new List<PeriodoNomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = ctx.PeriodoNomina.Where(p => p.IdEstatus == 2 && p.IdUnidadNegocio == IdUnidadNegocio).ToList();
            }
            return query;
        }

        public string GetNombrePeriodoNomina(string IdPeriodoNomina)
        {
            int IdPeriodoN = int.Parse(IdPeriodoNomina);
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                var query = ctx.PeriodoNomina.Where(p => p.IdPeriodoNomina == IdPeriodoN).Select(p => p.Periodo).FirstOrDefault();
                return query;
            }
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
                    Value = item.IdPeriodoNomina.ToString(),
                });

            }
            return result;
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

        public Cat_Bancos GetDatosBanco(int IdBanco)
        {
            Cat_Bancos query = new Cat_Bancos();
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                query = ctx.Cat_Bancos.Where(x => x.IdBanco == IdBanco && x.IdEstatus == 1).First();
            }
            return query;
        }
    }
}