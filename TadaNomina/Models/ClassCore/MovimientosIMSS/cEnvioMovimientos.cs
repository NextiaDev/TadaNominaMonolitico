using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cEnvioMovimientos
    {
        public List<sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS_Result> GetMovimientos(int IdCliente)
        {
            var f = DateTime.Now.ToShortDateString();
            var fecha = DateTime.Parse(f);
            var _fecha = fecha.ToString("yyyyMMdd");
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                string consulta = "sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS " + IdCliente + ", '" + _fecha + "', '" + _fecha + "'";
                var query = ctx.Database.SqlQuery<sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS_Result>(consulta).ToList();
                return query;
            }
        }

        public string EnviarMov(List<sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS_Result> listado)
        {
            try
            {
                var listadoxRP = listado.GroupBy(p => p.RegistroPatronal).ToList();
                foreach (var item in listadoxRP)
                {
                    var listadoRPSelect = listado.Where(p => p.RegistroPatronal == item.Key).ToList();
                    var listadoxTM = listadoRPSelect.GroupBy(p => p.TipoMovimiento).ToList();
                    foreach (var item2 in listadoxTM)
                    {
                        var reg = GetInfoRP(item.Key);
                        var listaIdEmp = listado.Where(p => p.RegistroPatronal == item.Key && p.TipoMovimiento == item2.Key).Select(p => p.IdEmpleado).ToList();
                        int IdRegistroPatronal = reg.IdRegistroPatronal;
                        switch (item2.Key)
                        {
                            case "ALTA":
                                var Altas = listado.Where(p => p.RegistroPatronal == item.Key && p.TipoMovimiento == item2.Key).ToList();
                                var dispmagA = DispmagAltas(Altas);
                                var RespuestaAPIAltas = GetRespuestaMov(item.Key, dispmagA);
                                //Agregar registro a la base de datos
                                foreach (var itemEmp in listaIdEmp)
                                {
                                    var infoemplaeado = GetEmpleadoBYId(itemEmp);
                                    var infoadicional = listado.Where(p => p.IdEmpleado == itemEmp && p.TipoMovimiento == "ALTA").First();
                                    decimal? sd = infoadicional.SD;
                                    decimal? sdi = infoadicional.SDI;
                                    DateTime? fai = DateTime.Parse(infoadicional.FechaAplicacionIMSS);
                                    int? idms = infoadicional.IdModificacionSueldo;
                                    AgregarRegistro(RespuestaAPIAltas, itemEmp, "ALTA", infoemplaeado, IdRegistroPatronal, sd, sdi, fai, idms);
                                }
                                break;
                            case "BAJA":
                                var Bajas = listado.Where(q => q.RegistroPatronal.Equals(item.Key) && q.TipoMovimiento.Equals(item2.Key)).ToList();
                                var dispmagB = DispmagBajas(Bajas);
                                var RespuestaAPIBajas = GetRespuestaMov(item.Key, dispmagB);
                                //Agregar registro a la base de datos
                                foreach (var itemEmp in listaIdEmp)
                                {
                                    var infoemplaeado = GetEmpleadoBYId(itemEmp);
                                    var infoadicional = listado.Where(p => p.IdEmpleado == itemEmp && p.TipoMovimiento == "BAJA").First();
                                    decimal? sd = infoadicional.SD;
                                    decimal? sdi = infoadicional.SDI;
                                    DateTime? fai = DateTime.Parse(infoadicional.FechaAplicacionIMSS);
                                    int? idms = infoadicional.IdModificacionSueldo;
                                    AgregarRegistro(RespuestaAPIBajas, itemEmp, "BAJA", infoemplaeado, IdRegistroPatronal, sd, sdi, fai, idms);
                                }
                                break;
                            case "CAMBIO":
                                var Modificacion = listado.Where(r => r.RegistroPatronal.Equals(item.Key) && r.TipoMovimiento.Equals(item2.Key)).ToList();
                                var dispmagM = DispmagModificaciones(Modificacion);
                                var RespuestaAPIModificacion = GetRespuestaMov(item.Key, dispmagM);
                                foreach (var itemEmp in listaIdEmp)
                                {
                                    var infoemplaeado = GetEmpleadoBYId(itemEmp);
                                    var infoadicional = listado.Where(p => p.IdEmpleado == itemEmp && p.TipoMovimiento == "CAMBIO").First();
                                    decimal? sd = infoadicional.SD;
                                    decimal? sdi = infoadicional.SDI;
                                    DateTime? fai = DateTime.Parse(infoadicional.FechaAplicacionIMSS);
                                    int? idms = infoadicional.IdModificacionSueldo;
                                    AgregarRegistro(RespuestaAPIModificacion, itemEmp, "CAMBIO", infoemplaeado, IdRegistroPatronal, sd, sdi, fai, idms);
                                }
                                break;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Cat_RegistroPatronal GetInfoRP(string RP)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var query = ctx.Cat_RegistroPatronal.Where(p => p.RegistroPatronal == RP).First();
                return query;
            }
        }

        public string DispmagAltas(List<sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS_Result> Altas)
        {
            string textofinal = string.Empty;
            string textoempleado = string.Empty;

            using (TadaNominaEntities context = new TadaNominaEntities())
            {
                if (Altas != null)
                {
                    foreach (var item in Altas)
                    {
                        textoempleado = string.Empty;
                        var empleado = (from b in context.vEmpleadosIMSS_NA
                                        where b.IdEmpleado == item.IdentificadorEmpleado
                                        select b).FirstOrDefault();
                        textoempleado += empleado.RegistroPatronal.Substring(0, 10);
                        textoempleado += empleado.RegistroPatronal.Substring(10, 1);
                        textoempleado += empleado.Imss.Substring(0, 10);
                        textoempleado += empleado.Imss.Substring(10, 1);
                        try { textoempleado += RellenaCadena(empleado.ApellidoPaterno.Replace("Ñ", "N"), 27); } catch { textoempleado += RellenaCadena("X", 27); }
                        try { textoempleado += RellenaCadena(empleado.ApellidoMaterno.Replace("Ñ", "N"), 27); } catch { textoempleado += RellenaCadena("X", 27); }
                        textoempleado += RellenaCadena(empleado.Nombre.Replace("Ñ", "N"), 27);
                        decimal menor = (decimal)999.99;
                        if (empleado.SDI <= menor)
                        {
                            string salario = empleado.SDI.ToString().Replace(".", "");
                            textoempleado += RellenaCadena(("0" + salario), 6);
                        }
                        else
                        {
                            string salario = empleado.SDI.ToString().Replace(".", "");
                            textoempleado += RellenaCadena(salario, 6);
                        }
                        textoempleado += RellenaCadena("", 6);
                        textoempleado += "1";
                        textoempleado += "0";
                        textoempleado += "0";
                        textoempleado += empleado.FechaAltaIMSS.ToString().Substring(0, 2) + empleado.FechaAltaIMSS.ToString().Substring(3, 2) + empleado.FechaAltaIMSS.ToString().Substring(6, 4);
                        textoempleado += "   ";
                        textoempleado += RellenaCadena("", 2);
                        textoempleado += "08";
                        textoempleado += "     ";
                        textoempleado += RellenaCadena(empleado.ClaveEmpleado, 10);
                        textoempleado += " ";
                        textoempleado += empleado.Curp;
                        textoempleado += "9";

                        textofinal += textoempleado + Environment.NewLine;
                    }
                }
                return textofinal;
            }
        }

        public string DispmagBajas(List<sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS_Result> Bajas)
        {
            string textofinal = string.Empty;
            string textoempleado = string.Empty;

            using (TadaNominaEntities context = new TadaNominaEntities())
            {
                if (Bajas != null)
                {
                    foreach (var item in Bajas)
                    {
                        textoempleado = string.Empty;
                        var empleado = (from b in context.vEmpleadosIMSS_NA
                                        where b.IdEmpleado == item.IdentificadorEmpleado
                                        select b).FirstOrDefault();
                        var motivobaja = (from a in context.Empleados
                                          where a.IdEmpleado == empleado.IdEmpleado
                                          select a).FirstOrDefault();
                        textoempleado += empleado.RegistroPatronal.Substring(0, 10);//
                        textoempleado += empleado.RegistroPatronal.Substring(10, 1);//
                        textoempleado += empleado.Imss.Substring(0, 10);//
                        textoempleado += empleado.Imss.Substring(10, 1);//
                        try { textoempleado += RellenaCadena(empleado.ApellidoPaterno.Replace("Ñ", "N"), 27); } catch { textoempleado += RellenaCadena("X", 27); }
                        try { textoempleado += RellenaCadena(empleado.ApellidoMaterno.Replace("Ñ", "N"), 27); } catch { textoempleado += RellenaCadena("X", 27); }
                        textoempleado += RellenaCadena(empleado.Nombre.Replace("Ñ", "N"), 27);//
                        textoempleado += "000000000000000";
                        textoempleado += empleado.FechaBaja.ToString().Substring(0, 2) + empleado.FechaBaja.ToString().Substring(3, 2) + empleado.FechaBaja.ToString().Substring(6, 4);
                        textoempleado += "     ";
                        textoempleado += "02";
                        textoempleado += "     ";
                        textoempleado += RellenaCadena(empleado.ClaveEmpleado, 10);
                        if (motivobaja.MotivoBaja == "TERMINO DE CONTRATO")
                        {
                            textoempleado += "1";
                        }
                        if (motivobaja.MotivoBaja == "SEPARACION VOLUNTARIA")
                        {
                            textoempleado += "2";
                        }
                        if (motivobaja.MotivoBaja == "ABANDONO DE EMPLEO")
                        {
                            textoempleado += "3";
                        }
                        if (motivobaja.MotivoBaja == "DEFUNCION")
                        {
                            textoempleado += "4";
                        }
                        if (motivobaja.MotivoBaja == "CLAUSURA")
                        {
                            textoempleado += "5";
                        }
                        if (motivobaja.MotivoBaja == "OTRA")
                        {
                            textoempleado += "6";
                        }
                        if (motivobaja.MotivoBaja == "AUSENTISMO")
                        {
                            textoempleado += "7";
                        }
                        if (motivobaja.MotivoBaja == "RESCISION DE CONTRATO")
                        {
                            textoempleado += "8";
                        }
                        if (motivobaja.MotivoBaja == "JUBILACION")
                        {
                            textoempleado += "9";
                        }
                        if (motivobaja.MotivoBaja == "PENSION")
                        {
                            textoempleado += "A";
                        }
                        textoempleado += RellenaCadena("", 18);
                        textoempleado += "9";

                        textofinal += textoempleado + Environment.NewLine;
                    }
                }
                return textofinal;
            }
        }

        public string DispmagModificaciones(List<sp_IMSS_MOVIMIENTOSIMSS_CLIENTES_NO_ADMINISTRADOS_Result> Modificaciones)
        {
            string textofinal = string.Empty;
            string textoempleado = string.Empty;

            using (TadaNominaEntities context = new TadaNominaEntities())
            {
                if (Modificaciones != null)
                {
                    foreach (var item in Modificaciones)
                    {
                        textoempleado = string.Empty;
                        var empleado = (from b in context.vEmpleadosIMSS_NA
                                        where b.IdEmpleado == item.IdentificadorEmpleado
                                        select b).FirstOrDefault();
                        textoempleado += empleado.RegistroPatronal.Substring(0, 10);
                        textoempleado += empleado.RegistroPatronal.Substring(10, 1);
                        textoempleado += empleado.Imss.Substring(0, 10);
                        textoempleado += empleado.Imss.Substring(10, 1);
                        try { textoempleado += RellenaCadena(empleado.ApellidoPaterno.Replace("Ñ", "N"), 27); } catch { textoempleado += RellenaCadena("X", 27); }
                        try { textoempleado += RellenaCadena(empleado.ApellidoMaterno.Replace("Ñ", "N"), 27); } catch { textoempleado += RellenaCadena("X", 27); }
                        textoempleado += RellenaCadena(empleado.Nombre.Replace("Ñ", "N"), 27);
                        decimal menor = (decimal)999.99;
                        if (empleado.SDI <= menor)
                        {
                            string salario = empleado.SDI.ToString().Replace(".", "");
                            textoempleado += RellenaCadena(("0" +salario), 6);
                        }
                        else
                        {
                            string salario = empleado.SDI.ToString().Replace(".", "");
                            textoempleado += RellenaCadena(salario, 6);
                        }
                        textoempleado += RellenaCadena("", 6);
                        textoempleado += "0";//Campo sugerido por CESS
                        textoempleado += item.TipoSalario.ToString();
                        textoempleado += "0";
                        textoempleado += item.FechaAplicacionIMSS.ToString().Substring(0, 2) + item.FechaAplicacionIMSS.ToString().Substring(3, 2) + item.FechaAplicacionIMSS.ToString().Substring(6, 4);
                        textoempleado += "   ";
                        textoempleado += RellenaCadena("", 2);
                        textoempleado += "07";
                        textoempleado += "     ";
                        textoempleado += RellenaCadena(empleado.ClaveEmpleado, 10);
                        textoempleado += " ";
                        textoempleado += empleado.Curp;
                        textoempleado += "9";

                        textofinal += textoempleado + Environment.NewLine;
                    }
                }
                return textofinal;
            }
        }

        private string RellenaCadena(string pCadena, int pPosiciones)
        {
            if (pCadena.Length <= pPosiciones)
            {
                while (pCadena.Length < pPosiciones)
                {
                    pCadena += " ";
                }
                return pCadena;
            }
            else
            {
                return pCadena.Substring(1, pPosiciones);
            }
        }

        public mRespuestaAfiliacion GetRespuestaMov(string RegistroPatronal, string dispmag)
        {

            var reg = GetInfoRP(RegistroPatronal);
            byte[] archivo;
            string certi = string.Empty;
            byte[] key;
            string keyIMSS = string.Empty;
            try
            {
                key = System.IO.File.ReadAllBytes(reg.KeyIMSS);
                keyIMSS = Convert.ToBase64String(key);
            }
            catch { }
            try
            {
                archivo = System.IO.File.ReadAllBytes(reg.CertificadoIMSS);
                certi = Convert.ToBase64String(archivo);
            }
            catch { }
            if (reg.KeyIMSS != null)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    key = keyIMSS,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 11),
                    dispmag = dispmag,
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/movtos");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mRespuestaAfiliacion>(response.Content);
                return responseDESERETI;
            }
            else
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    certificado = certi,
                    idUser = reg.UsuarioIMSS.ToString(),
                    password = reg.ContraseñaIMSS.ToString(),
                    regPatronal = reg.RegistroPatronal.ToString().Substring(0, 11),
                    dispmag = dispmag,
                });
                var client = new RestClient("http://www.desereti.com/tada/services/afiliacion/movtos");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responseDESERETI = Newtonsoft.Json.JsonConvert.DeserializeObject<mRespuestaAfiliacion>(response.Content);
                return responseDESERETI;
            }
        }

        public Empleados GetEmpleadoBYId(int empleado)
        {
            using (var entidad = new TadaNominaEntities())
            {
                return entidad.Empleados.Where(x => x.IdEmpleado == empleado).FirstOrDefault();
            }
        }

        public void AgregarRegistro(mRespuestaAfiliacion responseDESERETI, int empleado, string tipomovimiento, Empleados infoempleado, int idregistrop, decimal? sd, decimal? sdi, DateTime? fechamovimiento, int? idmodificacionsueldo)
        {
            using (TadaNominaEntities entity = new TadaNominaEntities())
            {
                if (responseDESERETI.estado == "0")
                {
                    if (tipomovimiento == "ALTA")
                    {
                        if (responseDESERETI.RespuestaWebService.movRechazado != null)
                        {
                            foreach (var item in responseDESERETI.RespuestaWebService.movRechazado)
                            {
                                if (item.nss == infoempleado.Imss)
                                {
                                    DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                                    {
                                        IdEmpleado = empleado,
                                        IdRegistroPatronal = idregistrop,
                                        TipoMovimiento = "A",
                                        Fecha = DateTime.Now.Date,
                                        FechaEnvio = DateTime.Now,
                                        Envio = 0,
                                        CodigoERROREnvio = item.codigoErrorMovimiento.ToString(),
                                        Lote = null,
                                        IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                                        FechaCaptura = DateTime.Now,
                                        IdEstatus = "1",
                                        FechaMovimiento = fechamovimiento,
                                        SDIMSS = sd,
                                        SDI = sdi,
                                        ClienteAdministrado = "N",
                                        NSS = infoempleado.Imss
                                    };
                                    entity.MovimientosIMSS.Add(mi);
                                    entity.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                            {
                                IdEmpleado = empleado,
                                IdRegistroPatronal = idregistrop,
                                TipoMovimiento = "A",
                                FechaEnvio = DateTime.Now,
                                Fecha = DateTime.Now.Date,
                                Envio = 1,
                                Lote = responseDESERETI.RespuestaWebService.reporteLote.idLote.ToString(),
                                IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                                FechaCaptura = DateTime.Now,
                                IdEstatus = "1",
                                FechaMovimiento = fechamovimiento,
                                SDIMSS = sd,
                                SDI = sdi,
                                ClienteAdministrado = "N",
                                NSS = infoempleado.Imss
                            };
                            entity.MovimientosIMSS.Add(mi);
                            entity.SaveChanges();
                        }
                    }
                    if (tipomovimiento == "BAJA")
                    {
                        if (responseDESERETI.RespuestaWebService.movRechazado != null)
                        {
                            foreach (var item in responseDESERETI.RespuestaWebService.movRechazado)
                            {
                                if (item.nss == infoempleado.Imss)
                                {
                                    DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                                    {
                                        IdEmpleado = empleado,
                                        IdRegistroPatronal = idregistrop,
                                        TipoMovimiento = "B",
                                        Fecha = DateTime.Now.Date,
                                        Envio = 0,
                                        FechaEnvio = DateTime.Now,
                                        CodigoERROREnvio = item.codigoErrorMovimiento.ToString(),
                                        Lote = null,
                                        IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                                        FechaCaptura = DateTime.Now,
                                        IdEstatus = "1",
                                        FechaMovimiento = fechamovimiento,
                                        SDIMSS = sd,
                                        SDI = sdi,
                                        ClienteAdministrado = "N",
                                        NSS = infoempleado.Imss
                                    };
                                    entity.MovimientosIMSS.Add(mi);
                                    entity.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                            {
                                IdEmpleado = empleado,
                                IdRegistroPatronal = idregistrop,
                                TipoMovimiento = "B",
                                Fecha = DateTime.Now.Date,
                                Envio = 1,
                                FechaEnvio = DateTime.Now,
                                Lote = responseDESERETI.RespuestaWebService.reporteLote.idLote.ToString(),
                                IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                                FechaCaptura = DateTime.Now,
                                IdEstatus = "1",
                                FechaMovimiento = fechamovimiento,
                                SDIMSS = sd,
                                SDI = sdi,
                                ClienteAdministrado = "N",
                                NSS = infoempleado.Imss
                            };
                            entity.MovimientosIMSS.Add(mi);
                            entity.SaveChanges();
                        }
                    }
                    if (tipomovimiento == "CAMBIO")
                    {
                        if (responseDESERETI.RespuestaWebService.movRechazado != null)
                        {
                            foreach (var item in responseDESERETI.RespuestaWebService.movRechazado)
                            {
                                if (item.nss == infoempleado.Imss)
                                {
                                    DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                                    {
                                        IdEmpleado = empleado,
                                        IdRegistroPatronal = idregistrop,
                                        TipoMovimiento = "C",
                                        Fecha = DateTime.Now.Date,
                                        FechaEnvio = DateTime.Now,
                                        Envio = 0,
                                        CodigoERROREnvio = item.codigoErrorMovimiento.ToString(),
                                        Lote = null,
                                        IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                                        FechaCaptura = DateTime.Now,
                                        IdEstatus = "1",
                                        FechaMovimiento = fechamovimiento,
                                        SDIMSS = sd,
                                        SDI = sdi,
                                        IdModificacionSueldo = idmodificacionsueldo,
                                        ClienteAdministrado = "N",
                                        NSS = infoempleado.Imss
                                    };
                                    entity.MovimientosIMSS.Add(mi);
                                    entity.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                            {
                                IdEmpleado = empleado,
                                IdRegistroPatronal = idregistrop,
                                TipoMovimiento = "C",
                                FechaEnvio = DateTime.Now,
                                Fecha = DateTime.Now.Date,
                                Envio = 1,
                                Lote = responseDESERETI.RespuestaWebService.reporteLote.idLote.ToString(),
                                IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                                FechaCaptura = DateTime.Now,
                                IdEstatus = "1",
                                FechaMovimiento = fechamovimiento,
                                SDIMSS = sd,
                                SDI = sdi,
                                IdModificacionSueldo = idmodificacionsueldo,
                                ClienteAdministrado = "N",
                                NSS = infoempleado.Imss
                            };
                            entity.MovimientosIMSS.Add(mi);
                            entity.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (tipomovimiento == "ALTA")
                    {
                        DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                        {
                            IdEmpleado = empleado,
                            IdRegistroPatronal = idregistrop,
                            TipoMovimiento = "A",
                            Fecha = DateTime.Now.Date,
                            Envio = 0,
                            FechaEnvio = DateTime.Now,
                            CodigoERROREnvio = responseDESERETI.estado,
                            IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                            FechaCaptura = DateTime.Now,
                            IdEstatus = "1",
                            FechaMovimiento = fechamovimiento,
                            SDIMSS = sd,
                            SDI = sdi,
                            ClienteAdministrado = "N",
                            NSS = infoempleado.Imss
                        };
                        entity.MovimientosIMSS.Add(mi);
                        entity.SaveChanges();
                    }
                    if (tipomovimiento == "BAJA")
                    {
                        DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                        {
                            IdEmpleado = empleado,
                            IdRegistroPatronal = idregistrop,
                            TipoMovimiento = "B",
                            Fecha = DateTime.Now.Date,
                            Envio = 0,
                            FechaEnvio = DateTime.Now,
                            CodigoERROREnvio = responseDESERETI.estado,
                            IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                            FechaCaptura = DateTime.Now,
                            IdEstatus = "1",
                            FechaMovimiento = fechamovimiento,
                            SDIMSS = sd,
                            SDI = sdi,
                            ClienteAdministrado = "N",
                            NSS = infoempleado.Imss
                        };
                        entity.MovimientosIMSS.Add(mi);
                        entity.SaveChanges();
                    }
                    if (tipomovimiento == "CAMBIO")
                    {
                        DB.MovimientosIMSS mi = new DB.MovimientosIMSS()
                        {
                            IdEmpleado = empleado,
                            IdRegistroPatronal = idregistrop,
                            TipoMovimiento = "C",
                            Fecha = DateTime.Now.Date,
                            Envio = 0,
                            FechaEnvio = DateTime.Now,
                            CodigoERROREnvio = responseDESERETI.estado,
                            IdCaptura = (int)System.Web.HttpContext.Current.Session["sIdUsuario"],
                            FechaCaptura = DateTime.Now,
                            IdEstatus = "1",
                            FechaMovimiento = fechamovimiento,
                            SDIMSS = sd,
                            SDI = sdi,
                            IdModificacionSueldo = idmodificacionsueldo,
                            ClienteAdministrado = "N",
                            NSS = infoempleado.Imss
                        };
                        entity.MovimientosIMSS.Add(mi);
                        entity.SaveChanges();
                    }
                }
            }
        }

        public List<mMovimientosIMSS> MovimientosError(int IdCliente)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                cMovimientosSinRespuesta cmsr = new cMovimientosSinRespuesta();
                var listadoIDs = cmsr.GetListadoRPbyCliente(IdCliente);
                var query = (from a in ctx.MovimientosIMSS where a.Fecha == DateTime.Today && a.Envio == 0 && listadoIDs.Contains(a.IdRegistroPatronal) && a.ClienteAdministrado=="N" select a);
                List<mMovimientosIMSS> ListadoMovenviadosErroneos = new List<mMovimientosIMSS>();
                foreach (var item in query)
                {
                    var x = new mMovimientosIMSS();
                    x.IdMovimiento = item.IdMovimiento;
                    x.IdEmpleado = item.IdEmpleado;
                    var nomre = (from e in ctx.Empleados where e.IdEmpleado == item.IdEmpleado select e);
                    foreach (var nom in nomre)
                    {
                        x.Nombre = nom.Nombre;
                        x.ApellidoPaterno = nom.ApellidoPaterno;
                        x.ApellidoMaterno = nom.ApellidoMaterno;
                    }
                    x.IdRegistroPatronal = item.IdRegistroPatronal;
                    var reg = (from r in ctx.Cat_RegistroPatronal where r.IdRegistroPatronal == item.IdRegistroPatronal select r);
                    foreach (var nomreg in reg)
                    {
                        x.RegistroPatronal = nomreg.RegistroPatronal;
                    }
                    x.TipoMovimiento = item.TipoMovimiento;
                    x.Fecha = item.Fecha;
                    x.Envio = item.Envio;
                    x.CodigoERROREnvio = item.CodigoERROREnvio;
                    var error = (from f in ctx.Cat_ErroresIMSS where f.CodigoERROR == item.CodigoERROREnvio select f);
                    foreach (var item2 in error)
                    {
                        x.ErrorEnvio = item2.Descripcion.ToString();
                    }
                    ListadoMovenviadosErroneos.Add(x);
                }
                return ListadoMovenviadosErroneos;
            }
        }

        public List<mMovimientosIMSS> MovimientosCorrectos(int IdCliente)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                cMovimientosSinRespuesta cmsr = new cMovimientosSinRespuesta();
                var listadoIDs = cmsr.GetListadoRPbyCliente(IdCliente);
                var query = (from a in ctx.MovimientosIMSS where a.Fecha == DateTime.Today && a.Envio == 1 && listadoIDs.Contains(a.IdRegistroPatronal) && a.ClienteAdministrado=="N" select a);
                List<mMovimientosIMSS> ListadoMovenviadosCorrectos = new List<mMovimientosIMSS>();
                foreach (var item in query)
                {
                    var x = new mMovimientosIMSS();
                    x.IdMovimiento = item.IdMovimiento;
                    x.IdEmpleado = item.IdEmpleado;
                    x.IdRegistroPatronal = item.IdRegistroPatronal;
                    x.TipoMovimiento = item.TipoMovimiento;
                    x.Fecha = item.Fecha;
                    x.Envio = item.Envio;
                    x.Lote = item.Lote;

                    ListadoMovenviadosCorrectos.Add(x);
                }
                return ListadoMovenviadosCorrectos;
            }
        }
    }
}