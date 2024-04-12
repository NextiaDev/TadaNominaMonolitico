using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.LayoutBancos;

namespace TadaNomina.Models.ClassCore.LayoutB
{
    public class ClassSantander
    {
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

        public string GetNombrePeriodoNomina(string IdPeriodoNomina)
        {
            int IdPeriodoN = int.Parse(IdPeriodoNomina);
            string query = "";
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = ctx.PeriodoNomina.Where(p => p.IdPeriodoNomina == IdPeriodoN).Select(p => p.Periodo).FirstOrDefault();
            }
            return query;
        }

        public List<ModelSantander> GetListaSantander(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            List<ModelSantander> listaSantander = new List<ModelSantander>();
            var tipoNomina = GetTipoNomina(IdPeriodoNomina);
            var listaNom = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeriodoNomina, null) : GetNominaFiniquitos(IdPeriodoNomina, null);
            List<int> listaIdEmpleados = listaNom.Select(p => p.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            List<vEmpleados> listaInfoEmp = cemp.getvEmpleadosByListIds(listaIdEmpleados, IdUnidadNegocio).Where(p => p.IdBancoTrad == 5 && p.CuentaBancariaTrad != null).ToList();
            foreach (var item in listaInfoEmp)
            {
                Nomina infoNom = listaNom.Where(p => p.IdEmpleado == item.IdEmpleado).First();
                ModelSantander model = new ModelSantander();
                model.NetoPagar = tipoNomina == "Finiquitos" ? infoNom.Neto.ToString() : infoNom.TotalEfectivo.ToString();
                model.NumeroCuenta = item.CuentaBancariaTrad;
                model.Nombre = item.Nombre;
                model.ApellidoPaterno = item.ApellidoPaterno;
                model.ApellidoMaterno = item.ApellidoMaterno;
                model.FechaHoy = DateTime.Now.ToString();
                model.IdRegistroPatronal = (int)infoNom.IdRegistroPatronal;
                listaSantander.Add(model);
            }
            return listaSantander;
        }

        public List<Nomina> GetNominaNormal(int IdPeriodoNomina, int? IdBanco)
        {
            List<Nomina> listado = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                if (IdBanco == null)
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null && x.TotalEfectivo > 0).ToList();
                }
                else
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null && x.TotalEfectivo > 0 && x.IdBancoTrad != IdBanco).ToList();
                }
            }
            return listado;
        }

        public List<Nomina> GetNominaFiniquitos(int IdPeriodoNomina, int? IdBanco)
        {
            var result = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                if (IdBanco == null)
                {
                    result = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null && x.Neto > 0 && x.IdEstatus == 1).ToList();
                }
                else
                {
                    result = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null && x.Neto > 0 && x.IdEstatus == 1 && x.IdBancoTrad != IdBanco).ToList();
                }
            }
            return result;
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


        public string GeneraTxtSantander(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            var listado = GetListaSantander(IdPeriodoNomina, IdUnidadNegocio);
            var cuentaCargo = GetCuentaBancariaDispersion(listado[0].IdRegistroPatronal);

            DateTime FechaDispersion = (DateTime)listaPeriodosAcumulados(IdUnidadNegocio).Where(x => x.IdPeriodoNomina == IdPeriodoNomina).Select(x => x.FechaDispersion).FirstOrDefault();
            int registro = 1;
            string textoempleado2 = string.Empty;
            string textoFinal = string.Empty;
            decimal totalsalario = 0;
            string textoempleado1 = "1" + RellenaCadenaCeros(registro.ToString(), 5) + "E" + DateTime.Now.ToString("MM/dd/yyyy").Replace("/", "") + cuentaCargo + RellenaCadenaEspacios("", 5) + FechaDispersion.ToString("MM/dd/yyyy").Replace("/", "");
            textoFinal = textoempleado1 + Environment.NewLine;
            foreach (var item in listado)
            {
                registro++;
                textoempleado2 = "2" + RellenaCadenaCeros(registro.ToString(), 5) + RellenaCadenaEspacios("", 7) + RellenaCadenaEspacios(item.ApellidoPaterno.Replace("Ñ", "N"), 30) + RellenaCadenaEspacios(item.ApellidoMaterno.Replace("Ñ", "N"), 20) + RellenaCadenaEspacios(item.Nombre.Replace("Ñ", "N"), 30) + RellenaCadenaEspacios(item.NumeroCuenta, 16) + RellenaCadenaCeros(item.NetoPagar.ToString().Replace(".", ""), 18) + "01" + Environment.NewLine;
                textoFinal += textoempleado2;
                totalsalario = totalsalario + decimal.Parse(item.NetoPagar);
            }
            string ImporteTotal = totalsalario.ToString().Replace(".", "");
            string textoempleado3 = "3" + RellenaCadenaCeros((registro + 1).ToString(), 5) + RellenaCadenaCeros((registro - 1).ToString(), 5) + RellenaCadenaCeros(ImporteTotal, 18);
            textoFinal = textoFinal + textoempleado3 + Environment.NewLine;
            return textoFinal;

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

        public string GeneraTxtSantanderInter(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            ClassBancomer cb = new ClassBancomer();
            int registro = 1;
            string textoFinal = string.Empty;
            decimal totalsalario = 0;
            var tipoNomina = GetTipoNomina(IdPeriodoNomina);
            var listadoNomina = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeriodoNomina, 5) : GetNominaFiniquitos(IdPeriodoNomina, 5);
            var ListadoNominaxRP = listadoNomina.GroupBy(p => p.IdRegistroPatronal);
            foreach (var item in ListadoNominaxRP)
            {
                var listaEmpleadosxRP = listadoNomina.Where(p => p.IdRegistroPatronal == item.Key).ToList();
                var listaxFecha = listaEmpleadosxRP.GroupBy(p => p.FechaCaptura);
                foreach (var item2 in listaxFecha)
                {
                    var infoRP = GetInfoRP(int.Parse(item.Key.ToString()));
                    string textoempleado1 = "1" + RellenaCadenaCeros(registro.ToString(), 5) + "E" + DateTime.Now.ToString("MM/dd/yyyy").Replace("/", "") + infoRP.CuentaBancaria + RellenaCadenaEspacios("", 5) + DateTime.Parse(item2.Key.ToString()).ToString("MM/dd/yyyy").Replace("/", "");
                    textoFinal = textoempleado1 + Environment.NewLine;
                    foreach (var item3 in listaEmpleadosxRP)
                    {
                        try
                        {                           
                            Cat_Bancos claveBanco = cb.GetDatosBanco(int.Parse(item3.IdBancoTrad.ToString()));
                            string textoempleado2 = string.Empty;
                            var InfoEmpleado = GetInfoEmpleado(item3.IdEmpleado);
                            if (InfoEmpleado.CuentaInterbancariaTrad != null || string.IsNullOrEmpty(InfoEmpleado.CuentaInterbancariaTrad))
                            {
                                registro++;
                                textoempleado2 += "2";
                                textoempleado2 += RellenaCadenaCeros(registro.ToString(), 5);
                                textoempleado2 += RellenaCadenaEspacios(InfoEmpleado.NombreCompleto.Replace("Ñ", "N"), 50);
                                textoempleado2 += RellenaCadenaEspacios("40", 5);
                                textoempleado2 += RellenaCadenaEspacios(InfoEmpleado.CuentaInterbancariaTrad, 20);
                                textoempleado2 += tipoNomina != "Finiquitos" ? RellenaCadenaCeros(item3.TotalEfectivo.ToString().Replace(".", ""), 18) : RellenaCadenaCeros(item3.Neto.ToString().Replace(".", ""), 18);
                                textoempleado2 += RellenaCadenaEspacios(claveBanco.ClaveTransfer, 5);
                                textoempleado2 += "01001";
                                textoFinal += textoempleado2 + Environment.NewLine;
                                decimal valorSumar = tipoNomina != "Finiquitos" ? decimal.Parse(item3.TotalEfectivo.ToString()) : decimal.Parse(item3.Neto.ToString());
                                totalsalario = totalsalario + valorSumar;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("No se pudo generar el registro para el layout del empleado: Id - " + item3.IdEmpleado);
                        }
                        
                    }
                    string ImporteTotal = totalsalario.ToString().Replace(".", "");
                    string textoempleado3 = "3" + RellenaCadenaCeros((registro + 1).ToString(), 5) + RellenaCadenaCeros((registro - 1).ToString(), 5) + RellenaCadenaCeros(ImporteTotal, 18);
                    textoFinal = textoFinal + textoempleado3 + Environment.NewLine;
                }

            }
            return textoFinal;
        }

        public List<Nomina> GetEmpleadosPeriodo(int IdPeriodoNomina)
        {
            List<Nomina> listadoNomina = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                listadoNomina = ctx.Nomina.Where(p => p.IdPeriodoNomina == IdPeriodoNomina && p.IdEstatus == 1).ToList();
            }
            return listadoNomina;
        }

        public List<ModelSantander> GetListaSantanderInter(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            List<ModelSantander> listaSantanderInter = new List<ModelSantander>();
            var tipoNomina = GetTipoNomina(IdPeriodoNomina);
            var listaNom = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeriodoNomina, 5) : GetNominaFiniquitos(IdPeriodoNomina, 5);
            List<int> listaIdEmpleados = listaNom.Select(p => p.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            List<vEmpleados> listaInfoEmp = cemp.getvEmpleadosByListIds(listaIdEmpleados, IdUnidadNegocio).Where(p => p.IdBancoTrad != 5 && p.CuentaBancariaTrad != null).ToList();
            foreach (var item in listaInfoEmp)
            {
                var infoNom = listaNom.Where(p => p.IdEmpleado == item.IdEmpleado).First();
                ModelSantander model = new ModelSantander();
                model.NetoPagar = tipoNomina == "Finiquitos" ? infoNom.Neto.ToString() : infoNom.TotalEfectivo.ToString();
                model.NumeroCuentaInter = infoNom.CuentaInterbancariaTrad;
                model.NombreCompleto = item.NombreCompleto;
                model.NetoPagar = infoNom.TotalEfectivo.ToString();
                model.FechaHoy = DateTime.Now.ToString();
                model.IdRegistroPatronal = (int)infoNom.IdRegistroPatronal;
                int idbanco = GetIdBancoByRP(model.IdRegistroPatronal);
                model.ClaveTransfer = GetDatosBanco(idbanco);
                listaSantanderInter.Add(model);
            }
            return listaSantanderInter;
        }

        public int GetIdBancoByRP(int IdRegistroPatronal)
        {
            int idBanco = 0;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal && x.IdEstatus == 1).First();
                idBanco = int.Parse(query.IdBanco.ToString());
            }
            return idBanco;
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

        public string GetDatosBanco(int IdBanco)
        {
            string claveTransfer = string.Empty;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_Bancos.Where(x => x.IdBanco == IdBanco && x.IdEstatus == 1).First();
                claveTransfer = query.ClaveTransfer;
            }
            return claveTransfer;
        }

        public Cat_RegistroPatronal GetInfoRP(int IdRegistroPatronal)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal && x.IdEstatus == 1).First();
            }
        }
    }
}