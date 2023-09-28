using DocumentFormat.OpenXml.EMMA;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.LayoutBancos;

namespace TadaNomina.Models.ClassCore.LayoutB
{
    public class ClassBajio
    {
        public string GeneraTxtBajio(int IdPeridoNomina, int IdUnidadNegocio)
        {
            var listado = GetListaBajio(IdPeridoNomina, IdUnidadNegocio);

            if (listado.Count <= 0) throw new Exception("No esposible genear el archivo. Validar el banco de la patrona y/o el banco del empleado.");


            var cuentaCargo = GetCuentaBancariaDispersion(listado[0].IdRegistroPatronal);

            int registro = 1;
            string linea1 = string.Empty;
            decimal totalNomina = 0;
            string linea2 = string.Empty;
            string linea3 = string.Empty;
            string textoFinal = string.Empty;
            totalNomina = netoNomina(listado);
            DateTime fechadispersion = (DateTime)listaPeriodosAcumulados(IdUnidadNegocio).Where(x => x.IdPeriodoNomina == IdPeridoNomina).Select(x => x.FechaDispersion).FirstOrDefault();

            if (IdUnidadNegocio == 208)
            {
                linea1 = "01" + RellenaCadenaCeros(registro.ToString(), 7) + "030" + "S" + "90" + "0" + "0001SGJ" + fechadispersion.ToString("yyyy/MM/dd").Replace("/", "") + RellenaCadenaCeros(cuentaCargo, 20) + RellenaCadenaEspacios("", 130) + "\n";

                foreach (var item in listado)
                {
                    totalNomina = netoNomina(listado);

                    registro++;
                    linea2 += "02" + RellenaCadenaCeros(registro.ToString(), 7) + "90" + fechadispersion.ToString("yyyy/MM/dd").Replace("/", "") + "000" + "030" + RellenaCadenaCeros(item.NetoPagar.ToString().Replace(".", ""), 15) + fechadispersion.ToString("yyyy/MM/dd").Replace("/", "") + "00" + RellenaCadenaCeros(cuentaCargo.ToString(), 20) + " " + "00" + RellenaCadenaCeros(item.CuentaEmpleado.ToString(), 20) + " " + RellenaCadenaCeros(registro.ToString(), 7) + RellenaCadenaEspacios("DEPOSITO DE NOMINA", 40) + RellenaCadenaCeros("", 40) + "\n";

                }
                var registrofinal = registro - 2;

                if (registrofinal == 0)
                {
                    registrofinal = registrofinal + 1;
                }

                var registroultimalinea = registro + 1;
                linea3 = "09" + RellenaCadenaCeros(registroultimalinea.ToString(), 7) + "90" + RellenaCadenaCeros(registrofinal.ToString(), 7) + RellenaCadenaCeros(totalNomina.ToString().Replace(".", ""), 18) + RellenaCadenaEspacios("", 145);
                textoFinal += linea1 + linea2 + linea3;
                return textoFinal;
            }
            else
            {
                linea1 = "01" + RellenaCadenaCeros(registro.ToString(), 7) + "030" + "S" + "90" + "0" + "0001UD6" + fechadispersion.ToString("yyyy/MM/dd").Replace("/", "") + RellenaCadenaCeros(cuentaCargo, 20) + RellenaCadenaEspacios("", 130) + "\n";

                foreach (var item in listado)
                {
                    totalNomina = netoNomina(listado);

                    registro++;
                    linea2 += "02" + RellenaCadenaCeros(registro.ToString(), 7) + "90" + fechadispersion.ToString("yyyy/MM/dd").Replace("/", "") + "000" + "030" + RellenaCadenaCeros(item.NetoPagar.ToString().Replace(".", ""), 15) + fechadispersion.ToString("yyyy/MM/dd").Replace("/", "") + "00" + RellenaCadenaCeros(cuentaCargo.ToString(), 20) + " " + "00" + RellenaCadenaCeros(item.CuentaEmpleado.ToString(), 20) + " " + RellenaCadenaCeros(registro.ToString(), 7) + RellenaCadenaEspacios("DEPOSITO DE NOMINA", 40) + RellenaCadenaCeros("", 40) + "\n";

                }
                var registrofinal = registro - 2;

                if (registrofinal == 0)
                {
                    registrofinal =  registrofinal + 1;
                }

                var registroultimalinea = registro + 1;
                linea3 = "09" + RellenaCadenaCeros(registroultimalinea.ToString(), 7) + "90" + RellenaCadenaCeros(registrofinal.ToString(), 7) + RellenaCadenaCeros(totalNomina.ToString().Replace(".", ""), 18) + RellenaCadenaEspacios("", 145);
                textoFinal += linea1 + linea2 + linea3;
                return textoFinal;
            }
            
        }

        public string GeneraTxtBajioInter(int IdPeriodNomina, int IdUnidadNegocio)
        {
            return null;
        }

        public List<mBajio> GetListaBajio(int IdPeriodoNomina, int IdUnidadNegocio)
        {
            List<mBajio> listaBajio = new List<mBajio>();
            var tipoNomina = GetTipoNomina(IdPeriodoNomina);
            var listaNomina = tipoNomina != "Finiquitos" ? GetNominaNormal(IdPeriodoNomina, 8) : GetNominaFiniquitos(IdPeriodoNomina);
            List<int> listaEmpleados = listaNomina.Select(x => x.IdEmpleado).ToList();
            ClassEmpleado cemp = new ClassEmpleado();
            ClassBancomer cb = new ClassBancomer();
            var cuentaNominaEmpleado = cemp.getCuentaEmpleadoByNomina(IdPeriodoNomina, listaEmpleados);

            List<vEmpleados> listaInfoEmpleados = cemp.getvEmpleadosByListIds(listaEmpleados, IdUnidadNegocio).Where(x => x.IdBancoTrad == 8 && x.CuentaBancariaTrad != null).ToList();

            foreach (var item in listaInfoEmpleados)
            {
                try
                {
                    Cat_Bancos claveBanco = cb.GetDatosBanco((int)item.IdBancoTrad);

                    Nomina infoNom = listaNomina.Where(x => x.IdEmpleado == item.IdEmpleado).First();
                    mBajio model = new mBajio();
                    model.NetoPagar = tipoNomina == "Finiquitos" ? infoNom.Neto : infoNom.TotalEfectivo;
                    model.IdEmpleado = item.IdEmpleado;
                    model.CuentaEmpleado = cuentaNominaEmpleado.Where(x => x.IdEmpleado == item.IdEmpleado).Select(x => x.CuentaBancariaTrad).First();
                    model.Nombre = item.Nombre;
                    model.ApellidoPaterno = item.ApellidoPaterno;
                    model.ApellidoMaterno = item.ApellidoMaterno;
                    model.IdRegistroPatronal = (int)item.IdRegistroPatronal;
                    model.ClaveBanco = claveBanco.ClaveBanco;
                    listaBajio.Add(model);
                }
                catch (Exception ex)
                {
                    throw new Exception("No esposible genear el archivo. No esposible genear el archivo. Validar el banco de la patrona y/o el banco del empleado.");
                }
            }
            return listaBajio;
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

        public List<Nomina> GetNominaNormal(int IdPeriodNomina, int? IdBanco)
        {
            List<Nomina> listado = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                if (IdBanco == null)
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodNomina && x.CuentaBancariaTrad != null && x.TotalEfectivo > 0).ToList();
                }
                else
                {
                    listado = ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodNomina && x.CuentaInterbancariaTrad != null && x.TotalEfectivo > 0 && x.IdBancoTrad == IdBanco).ToList();
                }
            }
            return listado;
        }

        public List<Nomina> GetNominaFiniquitos(int IdPeriodoNomina)
        {
            using (NominaEntities1 ctx= new NominaEntities1())
            {
                return ctx.Nomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null && x.Neto > 0 && x.IdEstatus == 1).ToList();
            }
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

        public List<PeriodoNomina> listaPeriodosAcumulados(int IdUnidadNegocio)
        {
            List<PeriodoNomina> query = new List<PeriodoNomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                query = ctx.PeriodoNomina.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 2).ToList();
            }
            return query;
        }

        public decimal netoNomina(List<mBajio> model)
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

        public string GetCuentaBancariaDispersion(int IdRegistroPatronal)
        {
            try
            {
                string cuentaDispersa = string.Empty;
                using (TadaNominaEntities ctx = new TadaNominaEntities())
                {
                    var query = ctx.Cat_RegistroPatronal.Where(x => x.IdRegistroPatronal == IdRegistroPatronal).First();
                    cuentaDispersa = query.CuentaBancaria;
                }
                return cuentaDispersa;
            }
            catch (Exception ex)
            {
                throw new Exception("No esposible genear el archivo. Validar el banco de la patrona y/o el banco del empleado.");
            }
        }

        public string GetClaveBanco(int IdBanco)
        {
            string claveBanco = string.Empty;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_Bancos.Where(x => x.IdBanco == IdBanco).Select(x => x.ClaveBanco).First();
            }
            return claveBanco;
        }
    }
}