using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.DB;
using System.Diagnostics;
using TadaNomina.Models.ViewModels.Catalogos;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Data.Linq.SqlClient;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    /// <summary>
    /// Empleados
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class ClassEmpleado
    {

        /// <summary>
        /// Método para llenar el modelo Empleado con las listas desplegables requeridas para usar el el formulario
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>modelo Empleados</returns>
        public Empleado Init(int IdCliente, int idUnidadNegocio)
        {
            Empleado empleado = new Empleado();

            empleado.IdUnidadNegocio = idUnidadNegocio;
            empleado.SexoList = GetSexo();
            empleado.EstadoCivilList = GetEstadoCivil();
            empleado.BancosList = GetBancos();
            empleado.EsquemasList = GetEsquema();

            try { empleado.DepartamentoList = GetDepartamentos(IdCliente); } catch { empleado.DepartamentoList = new List<SelectListItem>(); }
            try { empleado.PuestosList = GetPuestos(IdCliente); } catch { empleado.PuestosList = new List<SelectListItem>(); }
            try { empleado.SucursalList = GetSucursales(IdCliente); } catch { empleado.SucursalList = new List<SelectListItem>(); }
            try { empleado.CentrosCostosList = GetCentrosCostos(IdCliente); } catch { empleado.CentrosCostosList = new List<SelectListItem>(); }
            try { empleado.EntidadFederativaList = GetEntidadesFederativas(); } catch { empleado.EntidadFederativaList = new List<SelectListItem>(); }
            try { empleado.TiposContratoList = GetTIpoContrato(); } catch { empleado.TiposContratoList = new List<SelectListItem>(); }
            try { empleado.RegistrosPatronalesList = GetRegistrosPatronales(IdCliente); } catch { empleado.RegistrosPatronalesList = new List<SelectListItem>(); }
            try { empleado.AreaList = GetAreas(IdCliente); } catch { empleado.AreaList = new List<SelectListItem>(); }
            try { empleado.SindicatoList = GetSindicatos(); } catch { empleado.SindicatoList = new List<SelectListItem>(); }
            try { empleado.PrestacionesList = GetPrestaciones(IdCliente); } catch { empleado.PrestacionesList = new List<SelectListItem>(); }
            try { empleado.JornadaList = GetJornadas(IdCliente); } catch { empleado.JornadaList = new List<SelectListItem>(); }

            return empleado;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con los estatus
        /// </summary>
        /// <returns>lisa tipo SelectListItem con los estatus </returns>
        public List<SelectListItem> GetEstatus()
        {
            List<SelectListItem> estatus = new List<SelectListItem>
            {
                new SelectListItem() { Text = "ACTIVO", Value = "1" },
                new SelectListItem() { Text = "BAJA", Value = "2" },
                new SelectListItem() { Text = "BAJA(Cargas/Sueldos)", Value = "3" }
            };
            return estatus;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el catalogo de los motivos de baja para clientes externos
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el catalogo de los motivos de baja</returns>
        public List<SelectListItem> GetMotivosBajaExternos()
        {
            List<SelectListItem> externos = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "TERMINO DE CONTRATO", Value = "TERMINO DE CONTRATO" /*Value = "1"*/ },
                new SelectListItem() { Text = "SEPARACION VOLUNTARIA", Value = "SEPARACION VOLUNTARIA" /*Value = "2"*/ },
                new SelectListItem() { Text = "ABANDONO DE EMPLEO", Value = "ABANDONO DE EMPLEO" /*Value = "3"*/ },
                new SelectListItem() { Text = "DEFUNCION", Value = "DEFUNCION" /*Value = "4"*/ },
                new SelectListItem() { Text = "CLAUSURA", Value = "CLAUSURA" /*Value = "5"*/ },
                new SelectListItem() { Text = "OTRA", Value = "OTRA" /*Value = "6"*/ },
                new SelectListItem() { Text = "AUSENTISMO", Value = "AUSENTISMO" /*Value = "7"*/ },
                new SelectListItem() { Text = "RESCISION DE CONTRATO", Value = "RESCISION DE CONTRATO" /*Value = "8"*/ },
                new SelectListItem() { Text = "JUBILACION", Value = "JUBILACION" /*Value = "9"*/ },
                new SelectListItem() { Text = "PENSION", Value = "PENSION" /*Value = "A"*/ },
            };
            return externos;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el catalogo motivos de baja para internos
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el catalogo motivos de baja para internos</returns>
        public List<SelectListItem> GetMotivosBajaInternos()
        {
            List<SelectListItem> internos = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "SEPARACION VOLUNTARIA", Value = "SEPARACION VOLUNTARIA" /*Value = "2"*/ },
                new SelectListItem() { Text = "AUSENTISMO", Value = "AUSENTISMO" /*Value = "7"*/ },
                new SelectListItem() { Text = "OTRA", Value = "OTRA" /*Value = "6"*/ },
                new SelectListItem() { Text = "DEFUNCION", Value = "DEFUNCION" /*Value = "4"*/ },
                new SelectListItem() { Text = "PENSION", Value = "PENSION" /*Value = "A"*/ },
            };
            return internos;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el catalogo sexo
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el catalogo sexo</returns>
        public List<SelectListItem> GetSexo()
        {
            List<SelectListItem> sexo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "FEMENINO", Value = "FEMENINO" },
                new SelectListItem() { Text = "MASCULINO", Value = "MASCULINO" }
            };
            return sexo;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo estado civil
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el Catalogo estado civil</returns>
        public List<SelectListItem> GetEstadoCivil()
        {
            List<SelectListItem> sexo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "SOLTERO", Value = "SOLTERO" },
                new SelectListItem() { Text = "CASADO", Value = "CASADO" }
            };
            return sexo;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo Recontraatable
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el Catalogo Recontraatable</returns>
        public List<SelectListItem> GetRecontratable()
        {
            List<SelectListItem> sexo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "SI", Value = "SI" },
                new SelectListItem() { Text = "NO", Value = "NO" }
            };
            return sexo;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catagolo bancos
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el Catagolo bancos</returns>
        public List<SelectListItem> GetBancos()
        {
            List<SelectListItem> Cat_Bancos = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var bancos = (from b in entity.Cat_Bancos select b).OrderBy(x => x.NombreBanco).ToList();

                bancos.ForEach(x => { Cat_Bancos.Add(new SelectListItem { Text = x.NombreBanco.ToUpper(), Value = x.IdBanco.ToString() }); });
            }

            return Cat_Bancos;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo de departamentos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo de departamentos</returns>
        public List<SelectListItem> GetDepartamentos(int idCliente)
        {
            List<SelectListItem> departamentos = new List<SelectListItem>();


            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var departaments = (from b in entity.Cat_Departamentos where b.IdCliente == idCliente select b).OrderBy(x => x.Departamento).ToList();

                departaments.ForEach(x => { departamentos.Add(new SelectListItem { Text = x.Departamento.ToUpper(), Value = x.IdDepartamento.ToString() }); });
            }

            return departamentos;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo puesto potr cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo puesto por cliente</returns>
        public List<SelectListItem> GetPuestos(int idCliente)
        {
            List<SelectListItem> Puestos = new List<SelectListItem>();


            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var p = (from b in entity.Cat_Puestos where b.IdCliente == idCliente select b).OrderBy(x => x.Puesto).ToList();

                p.ForEach(x => { Puestos.Add(new SelectListItem { Text = x.Puesto.ToUpper(), Value = x.IdPuesto.ToString() }); });
            }

            return Puestos;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo sucursales por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo sucursales por cliente</returns>
        public List<SelectListItem> GetSucursales(int idCliente)
        {
            List<SelectListItem> sucursales = new List<SelectListItem>();


            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var p = (from b in entity.Cat_Sucursales where b.IdCliente == idCliente select b).OrderBy(x => x.Sucursal).ToList();

                p.ForEach(x => { sucursales.Add(new SelectListItem { Text = x.Sucursal.ToUpper(), Value = x.IdSucursal.ToString() }); });
            }

            return sucursales;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo Centro de costos por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo Centro de costos por cliente</returns>
        public List<SelectListItem> GetCentrosCostos(int idCliente)
        {
            List<SelectListItem> CentrosCostos = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var cc = (from b in entity.Cat_CentroCostos where b.IdCliente == idCliente select b).OrderBy(x => x.CentroCostos).ToList();

                cc.ForEach(x => { CentrosCostos.Add(new SelectListItem { Text = x.CentroCostos.ToUpper(), Value = x.IdCentroCostos.ToString() }); });
            }

            return CentrosCostos;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo Entidades federativas
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el Catalogo Entidades federativas</returns>
        public List<SelectListItem> GetEntidadesFederativas()
        {
            List<SelectListItem> EntidadesFederetivas = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var ef = (from b in entity.Cat_EntidadFederativa select b).ToList();

                ef.ForEach(x => { EntidadesFederetivas.Add(new SelectListItem { Text = x.Nombre.ToUpper(), Value = x.Id.ToString() }); });
            }

            return EntidadesFederetivas;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo Esquema
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el Catalogo Esquema</returns>
        public List<SelectListItem> GetEsquema()
        {
            List<SelectListItem> Esquema = new List<SelectListItem>
            {
                new SelectListItem() { Text = "100% ESQUEMA", Value = "100% ESQUEMA" },
                new SelectListItem() { Text = "100% TRADICIONAL", Value = "100% TRADICIONAL" },
                new SelectListItem() { Text = "MIXTO", Value = "MIXTO" }
            };
            return Esquema;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo de Areas
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo de Areas</returns>
        public List<SelectListItem> GetAre(int idCliente)
        {
            List<SelectListItem> Area = new List<SelectListItem>();

            using (TadaNominaEntities entity = new TadaNominaEntities())
            {
                var ef = (from b in entity.Cat_Areas where b.IdCliente == idCliente select b).ToList();

                ef.ForEach(x => { Area.Add(new SelectListItem { Text = x.Area.ToUpper(), Value = x.IdArea.ToString() }); });
            }

            return Area;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con Sindicatos
        /// </summary>
        /// <returns>lisa tipo SelectListItem con Sindicatos</returns>
        public List<SelectListItem> GetSindicato()
        {
            List<SelectListItem> sindi = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var ef = (from b in entity.Sindicatos select b).ToList();

                ef.ForEach(x => { sindi.Add(new SelectListItem { Text = x.NombreCorto.ToUpper(), Value = x.IdSindicato.ToString() }); });
            }

            return sindi;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo Tipo contrato
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el Catalogo Tipo contrato</returns>
        public List<SelectListItem> GetTIpoContrato()
        {
            List<SelectListItem> TipoContrato = new List<SelectListItem>
            {
                new SelectListItem() { Text = "DETERMINADO", Value = "DETERMINADO" },
                new SelectListItem() { Text = "INDETERMINADO", Value = "INDETERMINADO" },
                new SelectListItem() { Text = "HONORARIOS", Value = "HONORARIOS" },
                new SelectListItem() { Text = "SIN CONTRATO", Value = "SIN CONTRATO" },
                new SelectListItem() { Text = "RESICO", Value = "RESICO" }

            };
            return TipoContrato;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo Registros patronales
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo Registros patronales</returns>
        public List<SelectListItem> GetRegistrosPatronales(int idCliente)
        {
            List<SelectListItem> RegistrosPatronales = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var rp = (from b in entity.Cat_RegistroPatronal where b.IdCliente == idCliente select b).ToList();
                rp.ForEach(x => { RegistrosPatronales.Add(new SelectListItem { Text = x.NombrePatrona.ToUpper() + " - " + x.RegistroPatronal.ToUpper(), Value = x.IdRegistroPatronal.ToString() }); });

                var es = (from b in entity.vClienteEmpresaEspecializada where b.IdCliente == idCliente select b).ToList();
                es.ForEach(x => { RegistrosPatronales.Add(new SelectListItem { Text = x.NombrePatrona.ToUpper() + " - " + x.RegistroPatronal.ToUpper(), Value = x.IdRegistroPatronal.ToString() }); });
            }

            return RegistrosPatronales;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con las Prestaciones
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con las Prestaciones</returns>
        public List<SelectListItem> GetPrestaciones(int IdCliente)
        {
            List<SelectListItem> Prestaciones = new List<SelectListItem>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var pres = (from b in entity.Prestaciones.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();
                pres.ForEach(x => { Prestaciones.Add(new SelectListItem { Text = x.Descripcion.ToUpper(), Value = x.IdPrestaciones.ToString() }); });
            }

            return Prestaciones;
        }

        /// <summary>
        /// Método para obtener un listado de empleados por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de empleados por unidad de negocio</returns>
        public List<Empleados> GetEmpleadoByUnidadNegocio(int IdUnidadNegocio)
        {
            List<Empleados> empleados = new List<Empleados>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                empleados = (from b in entity.Empleados where b.IdUnidadNegocio.Equals(IdUnidadNegocio) && b.IdEstatus == 1 select b).ToList();
            }

            return empleados;
        }

        /// <summary>
        /// Método para obtener los empleados dados de baja por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad d enegocio</param>
        /// <returns>Listado de empleados dados de baja por unidad de negocio</returns>
        public List<Empleados> GetEmpleadoByUnidadNegocioBajas(int IdUnidadNegocio)
        {
            List<Empleados> empleados = new List<Empleados>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                int[] estatus = { 2 };
                empleados = (from b in entity.Empleados where b.IdUnidadNegocio.Equals(IdUnidadNegocio) && estatus.Contains(b.IdEstatus) select b).ToList();
            }

            return empleados;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo de Area
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo de Area</returns>
        public List<SelectListItem> GetAreas(int idCliente)
        {
            List<SelectListItem> Are = new List<SelectListItem>();


            using (TadaNominaEntities entity = new TadaNominaEntities())
            {
                var Areas = (from b in entity.Cat_Areas where b.IdCliente == idCliente && b.IdEstatus == 1 select b).OrderBy(x => x.Area).ToList();

                Areas.ForEach(x => { Are.Add(new SelectListItem { Text = x.Area.ToUpper(), Value = x.IdArea.ToString() }); });
            }

            return Are;
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el catalogo Sindicatos
        /// </summary>
        /// <returns>lisa tipo SelectListItem con el catalogo Sindicatoso</returns>
        public List<SelectListItem> GetSindicatos()
        {
            List<SelectListItem> Sindicatos = new List<SelectListItem>();


            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var Areas = (from b in entity.Sindicatos where b.IdEstatus == 1 select b).OrderBy(x => x.NombreCorto).ToList();

                Areas.ForEach(x => { Sindicatos.Add(new SelectListItem { Text = x.NombreCorto.ToUpper() + " " + "Grupo" + x.Grupo, Value = x.IdSindicato.ToString() }); });
            }

            return Sindicatos;
        }

        /// <summary>
        /// Método para obtener los empleados activos por unidad de negocio y claves de empleado
        /// </summary>
        /// <param name="claves">Claves de los empleados</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de empleados activos por unidad de negocio y claves de empleado</returns>
        public List<Empleados> GetEmpleadosActivosByUnidadNegocioYClave(string[] claves, int IdUnidadNegocio)
        {
            List<Empleados> empleados = new List<Empleados>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                empleados = (from b in entity.Empleados
                             where b.IdUnidadNegocio.Equals(IdUnidadNegocio) && claves.Contains(b.ClaveEmpleado) && b.IdEstatus == 1
                             select b).ToList();
            }

            return empleados;
        }

        /// <summary>
        /// Método para agregar un empleado
        /// </summary>
        /// <param name="empleado">Información del empleado</param>
        /// <returns>1/0 dependiendo si el registro se logro con exito o no</returns>
        public int AddEmpleado(Empleado empleado, int IdCliente, string token)
        {
            int value = 0;
            if (empleado.PremioP == true)
            {
                empleado.Premio = "SI";
            }
            else
            {
                empleado.Premio = "";
            }

            DateTime? fechaTerminoContrato = null;
            try { fechaTerminoContrato = DateTime.Parse(empleado.FechaTerminoContrato); } catch { }

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                Empleados emp = new Empleados()
                {
                    IdUnidadNegocio = empleado.IdUnidadNegocio,
                    IdCentroCostos = empleado.IdCentroCostos,
                    IdDepartamento = empleado.IdDepartamento,
                    IdPuesto = empleado.IdPuesto,
                    IdSucursal = empleado.IdSucursal,
                    IdRegistroPatronal = empleado.IdRegistroPatronal,
                    IdEntidad = empleado.IdEntidad,
                    ClaveEmpleado = empleado.ClaveEmpleado,
                    Nombre = empleado.Nombre.ToUpper(),
                    ApellidoPaterno = empleado.ApellidoPaterno.ToUpper(),
                    ApellidoMaterno = empleado.ApellidoMaterno.ToUpper(),
                    Sexo = empleado.Sexo.ToUpper(),
                    EstadoCivil = empleado.EstadoCivil,
                    FechaNacimiento = Convert.ToDateTime(empleado.FechaNacimiento),
                    SD = empleado.SD,
                    SDIMSS = empleado.SDIMSS,
                    SDI = empleado.SDI,
                    IdBancoTrad = empleado.IdBancoTrad,
                    CuentaBancariaTrad = empleado.CuentaBancariaTrad,
                    CuentaInterbancariaTrad = empleado.CuentaInterbancariaTrad,
                    Curp = empleado.Curp.ToUpper(),
                    Rfc = empleado.Rfc.ToUpper(),
                    Imss = empleado.Imss,
                    CorreoElectronico = empleado.CorreoElectronico,
                    FechaReconocimientoAntiguedad = Convert.ToDateTime(empleado.FechaReconocimientoAntiguedad),
                    FechaAltaIMSS = Convert.ToDateTime(empleado.FechaAltaIMSS),
                    Esquema = empleado.Esquema,
                    TipoContrato = empleado.TipoContrato,
                    FechaTerminoContrato = fechaTerminoContrato,
                    RFCSubContratacion = empleado.RFCSubContratacion,
                    idSindicato = empleado.Idsindicato,
                    IdArea = empleado.idArea,
                    PremioP = empleado.Premio,
                    Telefono = empleado.NumeroTelefonico,
                    IdEstatus = 1,
                    IdCaptura = empleado.IdCaptura,
                    FechaCaptura = DateTime.Now,
                    IdPrestaciones = empleado.IdPrestaciones,
                    IdJornada= empleado.IdJornada,
                    IdBancoViaticos = empleado.IdBancoViaticos,
                    CuentaBancariaViaticos = empleado.CuentaBancariaViaticos,
                    CuentaInterbancariaViaticos = empleado.CuentaInterBancariaViaticos,
                    IdCodigoPostal = empleado.IdCodigoPostalFiscal,
                    Calle = empleado.CalleFiscal,
                    noExt = empleado.NumeroExtFiscal,
                    noInt = empleado.NumeroIntFiscal,
                    CP = empleado.CodigoPostalFiscal
                };

                if (empleado.IdRegistroPatronal != null)
                {
                    decimal salariominimo = ObtenSMV();
                    if (empleado.SDIMSS > 0 && empleado.SDIMSS >= salariominimo)
                    {
                        DateTime FechaFinal = DateTime.Today;
                        DateTime FechaInicial = Convert.ToDateTime(empleado.FechaAltaIMSS).Date;
                        if (FechaInicial <= FechaFinal)
                        {
                            if (GetDiasHabiles(FechaInicial, FechaFinal) <= 5)
                            {
                                entity.Empleados.Add(emp);
                                value = entity.SaveChanges();
                                bool i = SetPassEmpleado(emp.IdEmpleado, IdCliente, emp.CorreoElectronico, token);
                            }
                            else
                            {
                                value = -1;
                            }
                        }
                        else
                        {
                            if (GetDiasAdelatados(FechaInicial, FechaFinal) == 1)
                            {
                                entity.Empleados.Add(emp);
                                value = entity.SaveChanges();
                            }
                            else
                            {
                                value = -2;
                            }
                        }
                    }
                    else
                    {
                        value = -3;
                    }
                }
                if (empleado.IdRegistroPatronal == null)
                {
                    if(empleado.Imss == null && empleado.SDIMSS==null && empleado.SDI==null && empleado.FechaAltaIMSS==null)
                    {
                        entity.Empleados.Add(emp);
                        value=entity.SaveChanges();
                        bool i = SetPassEmpleado(emp.IdEmpleado, IdCliente, emp.CorreoElectronico, token);
                    }
                    else
                    {
                        value = -4;
                    }
                }
                ///Para evitar que den de alta a empleados sin registro patronal y con SDI
                //else
                //{
                //    entity.Empleados.Add(emp);
                //    value = entity.SaveChanges();
                //}
                if (value == 0)
                {
                    return value;
                }

                if (value > 0)
                {
                    empleado.IdEmpleado = emp.IdEmpleado;
                    empleado.FechaCaptura = emp.FechaCaptura;
                    value = AddEmpleadoComplementaria(empleado);
                }

                return value;
            }

        }

        /// <summary>
        /// Método para obtener los 3 días de atraso habiles para el alata del empleado ante el IMSS 
        /// </summary>
        /// <param name="FechaInicial">Fecha del día de lata del empleado</param>
        /// <param name="FechaFinal">Fecha del día de hoy</param>
        /// <returns>Numero de días de diferencia</returns>
        public static int GetDiasHabiles(DateTime FechaInicial, DateTime FechaFinal)
        {
            int diashabiles = 0;
            while (FechaInicial < FechaFinal)
            {
                int numerodia = Convert.ToInt32(FechaInicial.DayOfWeek.ToString("d"));
                if (numerodia == 1 || numerodia == 2 || numerodia == 3 || numerodia == 4 || numerodia == 5)
                {
                    diashabiles++;
                }
                FechaInicial = FechaInicial.AddDays(1);
            }
            return diashabiles;
        }

        /// <summary>
        /// Método par aobtener los dias adelantados permitidos por el IMSS
        /// </summary>
        /// <param name="FechaInicial">Fecha del día de lata del empleado</param>
        /// <param name="FechaFinal">Fecha del día de hoy</param>
        /// <returns>Numero de días de diferencia</returns>
        public static int GetDiasAdelatados(DateTime FechaInicial, DateTime FechaFinal)
        {
            int diasadelantados = 0;
            while (FechaInicial > FechaFinal)
            {
                int numerodia = Convert.ToInt32(FechaInicial.DayOfWeek.ToString("d"));
                if (numerodia == 1 || numerodia == 2 || numerodia == 3 || numerodia == 4 || numerodia == 5)
                {
                    diasadelantados++;
                }
                FechaInicial = FechaInicial.AddDays(-1);
            }
            return diasadelantados;
        }

        /// <summary>
        /// Método par agregar l ainformación complementaria del empleado
        /// </summary>
        /// <param name="empleado">Información del empleado</param>
        /// <returns>Valor para rectificar que el registro se realizo con exito</returns>
        private int AddEmpleadoComplementaria(Empleado empleado)
        {
            int value = 0;
            if (empleado.IdCodigoPostal != 0)
            {
                using (TadaEmpleados entity = new TadaEmpleados())
                {
                    EmpleadoInformacionComplementaria emic = new EmpleadoInformacionComplementaria()
                    {
                        IdEmpleado = empleado.IdEmpleado,
                        Calle = empleado.Calle,
                        NumeroExt = empleado.NumeroExt,
                        NumeroInt = empleado.NumeroExt,
                        IdCodigoPostal = empleado.IdCodigoPostal,
                        IdEstatus = 1,
                        IdCaptura = empleado.IdCaptura,
                        FechaCaptura = DateTime.Now
                    };

                    entity.EmpleadoInformacionComplementaria.Add(emic);
                    value = entity.SaveChanges();
                }
            }

            return value;
        }

        /// <summary>
        /// Método para obtener los empleador por unidad de negocio
        /// </summary>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado con la informacion del emplado</returns>
        public List<Empleado> GetEmpleados(int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();
                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        FechaBaja = x.FechaBaja.Value.ToShortDateString(),
                    });
                });

                return empleados;
            }
        }

        //Busqueda de empleados por Clave
        /// <summary>
        /// Método para obtener a los empledos por su clave de empleado
        /// </summary>
        /// <param name="clave">Clave del empleado</param>
        /// <param name="idUnidadNegocio">Identificador de la unodad de negocio</param>
        /// <returns>Listado con la información del empleado</returns>
        public List<Empleado> GetEmpleadosByClave(string clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();                
                var query = (from b in entity.Empleados where b.ClaveEmpleado == clave && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        FechaBaja = x.FechaBaja != null ? x.FechaBaja.Value.ToShortDateString() : null,
                    });
                });
                return empleados;
            }
        }

        public List<Empleado> GetEmpleadosByClaveAllEstatus(string clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                int[] status = { 1, 2, 3 };
                var query = (from b in entity.Empleados where b.ClaveEmpleado == clave && b.IdUnidadNegocio == idUnidadNegocio && status.Contains(b.IdEstatus) select b).OrderBy(x => x.IdEstatus).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        FechaBaja = x.FechaBaja != null ? x.FechaBaja.Value.ToShortDateString() : null,
                    });
                });
                return empleados;
            }
        }

        /// <summary>
        /// Método para obtener a los empledos por diferentes claves de empleado
        /// </summary>
        /// <param name="clave">Claves de los empleados</param>
        /// <param name="idUnidadNegocio">Identificador d ela unidad de negocio</param>
        /// <returns>Listado con la información de los empleados</returns>
        public List<Empleado> GetEmpleadosByClave(string[] clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where clave.Contains(b.ClaveEmpleado) && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,                       
                        FechaBaja = x.FechaBaja != null ? x.FechaBaja.Value.ToShortDateString() : null,
                    });
                });
                return empleados;
            }
        }

        /// <summary>
        /// Método para obtener a la información de los empleados por su nombre
        /// </summary>
        /// <param name="nombre">Nombre del empleado</param>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado con la información d elos empleados</returns>
        public List<Empleado> GetEmpleadosByNombre(string nombre, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where (b.Nombre + " " + b.ApellidoPaterno + " " + b.ApellidoMaterno).StartsWith(nombre) && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        FechaBaja = x.FechaBaja != null ? x.FechaBaja.Value.ToShortDateString() : null,
                    });
                });
                return empleados;
            }
        }

        /// <summary>
        /// Método para emplear la información del emplado para ser editada
        /// </summary>
        /// <param name="idEmpleado">Identificador del empleado</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Información del empleado</returns>
        public Empleado GetEmpleadoToEdit(int idEmpleado, int IdUnidadNegocio, int IdCliente)
        {

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var emp = entity.Empleados.Where(b=> b.IdEmpleado.Equals(idEmpleado) && b.IdUnidadNegocio == IdUnidadNegocio && b.IdEstatus == 1).FirstOrDefault();

                var reg = (from a in entity.Cat_RegistroPatronal//unificar el motivo ed bajas DRR
                           where a.IdRegistroPatronal == emp.IdRegistroPatronal && a.IdEstatus == 1//unificar el motivo ed bajas DRR
                           select a).FirstOrDefault();//unificar el motivo ed bajas DRR

                Empleado empleado = new Empleado();

                if (emp.PremioP == "SI")
                {
                    empleado.PremioP = true;
                }
                else
                {
                    empleado.PremioP = false;
                }

                if (reg != null)
                {
                    empleado.idcliente = reg.IdCliente;
                }
                else
                {
                    empleado.idcliente = IdCliente;//unificar el motivo ed bajas DRR
                }

                empleado.MotivoBajaExterno = GetMotivosBajaExternos();//unificar el motivo ed bajas DRR
                empleado.MotivoBajaInterno = GetMotivosBajaInternos();//unificar el motivo ed bajas DRR

                empleado.IdUnidadNegocio = emp.IdUnidadNegocio;
                empleado.IdCentroCostos = emp.IdCentroCostos;
                empleado.CentrosCostosList = GetCentrosCostos(IdCliente);

                empleado.IdDepartamento = emp.IdDepartamento;
                empleado.DepartamentoList = GetDepartamentos(IdCliente);
                empleado.idArea = emp.IdArea;
                empleado.AreaList = GetAre(IdCliente);
                empleado.Idsindicato = emp.idSindicato;
                empleado.SindicatoList = GetSindicato();
                empleado.IdPuesto = emp.IdPuesto;
                empleado.PuestosList = GetPuestos(IdCliente);

                empleado.IdSucursal = emp.IdSucursal;
                empleado.SucursalList = GetSucursales(IdCliente);

                empleado.IdRegistroPatronal = emp.IdRegistroPatronal;
                empleado.RegistrosPatronalesList = GetRegistrosPatronales(IdCliente);

                empleado.IdEntidad = emp.IdEntidad;
                empleado.EntidadFederativaList = GetEntidadesFederativas();

                empleado.IdBancoTrad = emp.IdBancoTrad;
                empleado.BancosList = GetBancos();

                empleado.IdEmpleado = emp.IdEmpleado;

                empleado.ClaveEmpleado = emp.ClaveEmpleado;
                empleado.Nombre = emp.Nombre;
                empleado.ApellidoPaterno = emp.ApellidoPaterno;
                empleado.ApellidoMaterno = emp.ApellidoMaterno;

                empleado.Sexo = emp.Sexo;
                empleado.SexoList = GetSexo();

                empleado.EstadoCivil = emp.EstadoCivil;
                empleado.EstadoCivilList = GetEstadoCivil();

                empleado.NumeroTelefonico = emp.Telefono;
                empleado.SD = emp.SD;
                empleado.SDI = emp.SDI;
                empleado.SDIMSS = emp.SDIMSS;
                empleado.CuentaBancariaTrad = emp.CuentaBancariaTrad;
                empleado.CuentaInterbancariaTrad = emp.CuentaInterbancariaTrad;
                empleado.Curp = emp.Curp;
                empleado.Rfc = emp.Rfc;
                empleado.Imss = emp.Imss;
                empleado.CorreoElectronico = emp.CorreoElectronico;
                empleado.PremioP = empleado.PremioP;

                try
                {
                    empleado.FechaNacimiento = emp.FechaNacimiento.Value.ToShortDateString();
                }
                catch
                {
                    empleado.FechaNacimiento = null;
                }

                try
                {
                    empleado.FechaAltaIMSS = emp.FechaAltaIMSS.Value.ToShortDateString();
                }
                catch
                {
                    empleado.FechaAltaIMSS = null;
                }

                try
                {
                    empleado.FechaReconocimientoAntiguedad = emp.FechaReconocimientoAntiguedad.Value.ToShortDateString();
                }
                catch
                {
                    empleado.FechaReconocimientoAntiguedad = null;
                }

                try
                {
                    empleado.FechaBaja = emp.FechaBaja.Value.ToShortDateString();
                }
                catch
                {
                    empleado.FechaBaja = null;
                }

                empleado.Recontratable = emp.Recontratable;
                empleado.RecontratableList = GetRecontratable();
                empleado.Esquema = emp.Esquema;
                empleado.EsquemasList = GetEsquema();

                empleado.IdEstatus = emp.IdEstatus;
                empleado.EstatusList = GetEstatus();

                empleado.TipoContrato = emp.TipoContrato;
                empleado.TiposContratoList = GetTIpoContrato();
                empleado.FechaTerminoContrato = emp.FechaTerminoContrato != null ? ((DateTime)emp.FechaTerminoContrato).ToShortDateString() : null;

                try { empleado.IdPrestaciones = (int)emp.IdPrestaciones; } catch { empleado.IdPrestaciones = 0; }
                empleado.PrestacionesList = GetPrestaciones(IdCliente);

                empleado.IdJornada = emp.IdJornada;
                empleado.JornadaList = GetJornadas(IdCliente);

                if (empleado != null)
                {
                    empleado = GetInfoComEmpleado(empleado);
                }
                if (empleado != null)
                {
                    empleado.CalleFiscal = emp.Calle;
                    empleado.NumeroExtFiscal = emp.noExt;
                    empleado.NumeroIntFiscal = emp.noInt;
                    empleado.IdCodigoPostalFiscal = emp.IdCodigoPostal;
                    empleado.CodigoPostalFiscal = emp.CP;

                    if (empleado.IdCodigoPostalFiscal != null && empleado.IdCodigoPostalFiscal != 0)
                    {
                        int idCodigoPostalFiscal = (int)empleado.IdCodigoPostalFiscal;
                        CP cp = GetCodigoPostalesById(idCodigoPostalFiscal);
                        empleado.CodigosPostalesListFiscales = new List<SelectListItem>();
                        empleado.CodigosPostalesListFiscales.Add(new SelectListItem { Value = cp.Id.ToString(), Text = cp.Colonia });

                        empleado.CodigoPostalFiscal = cp.Codigo;
                        empleado.EntidadFiscal = cp.Municipio;
                        empleado.MunicipioFiscal = cp.Entidad;
                    }
                    else
                    {
                        empleado.CodigosPostalesListFiscales = new List<SelectListItem>();
                        empleado.CodigosPostalesListFiscales.Add(new SelectListItem { Value = "0", Text = "---" });
                    }
                }
                empleado.IdBancoViaticos = emp.IdBancoViaticos;
                empleado.CuentaBancariaViaticos = emp.CuentaBancariaViaticos;
                empleado.CuentaInterBancariaViaticos = emp.CuentaInterbancariaViaticos;

                return empleado;
            }
        }

        /// <summary>
        /// Método para obtener la información complementaria del empleado para ser editada
        /// </summary>
        /// <param name="empleado">Informacón del empleado</param>
        /// <returns>Información complentaria del empleado</returns>
        public Empleado GetInfoComEmpleado(Empleado empleado)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                int idEmpleado = empleado.IdEmpleado;
                var query = (from b in entity.EmpleadoInformacionComplementaria
                             where b.IdEmpleado == idEmpleado
                             select b).FirstOrDefault();

                if (query != null)
                {
                    empleado.Calle = query.Calle;
                    empleado.NumeroExt = query.NumeroExt;
                    empleado.NumeroInt = query.NumeroInt;
                    empleado.IdCodigoPostal = query.IdCodigoPostal;

                    if (query.IdCodigoPostal != null && query.IdCodigoPostal != 0)
                    {
                        int idCodigoPostal = (int)query.IdCodigoPostal;
                        CP cp = GetCodigoPostalesById(idCodigoPostal);
                        empleado.CodigosPostalesList = new List<SelectListItem>();
                        empleado.CodigosPostalesList.Add(new SelectListItem { Value = cp.Id.ToString(), Text = cp.Colonia });
                        empleado.CodigoPostal = cp.Codigo;
                        empleado.Municipio = cp.Municipio;
                        empleado.Entidad = cp.Entidad;
                    }
                    else
                    {
                        empleado.CodigosPostalesList = new List<SelectListItem>();
                        empleado.CodigosPostalesList.Add(new SelectListItem { Value = "0", Text = "---" });
                    }

                }
                else
                {
                    empleado.CodigosPostalesList = new List<SelectListItem>();
                    empleado.CodigosPostalesList.Add(new SelectListItem { Value = "0", Text = "---" });
                }
                return empleado;
            }
        }

        /// <summary>
        /// Método para guardar la información editada del empleado
        /// </summary>
        /// <param name="empleado">Información editada del empleado</param>
        /// <param name="ClienteAdministrado">Identificador de cliente administardo</param>
        /// <returns>Valor para rectificar que el registro se realizó con exito</returns>
        public int EditEmpleado(Empleado empleado, int ClienteAdministrado)
        {
            if (empleado.PremioP == true)
            {
                empleado.Premio = "SI";
            }
            else
            {
                empleado.Premio = "";
            }
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                try
                {
                    var emp = (from b in entity.Empleados where b.IdEmpleado.Equals(empleado.IdEmpleado) select b).FirstOrDefault();


                    //Validamos si el cliente es administrado para no permitir modificacions sobre campos IMSS

                    if (ClienteAdministrado != 1)
                    {
                        emp.IdRegistroPatronal = empleado.IdRegistroPatronal;
                        emp.SDI = empleado.SDI;
                        emp.Imss = empleado.Imss;

                        if (empleado.FechaAltaIMSS != null)
                        {
                            emp.FechaAltaIMSS = Convert.ToDateTime(empleado.FechaAltaIMSS).Date;
                        }
                        else
                        {
                            emp.FechaAltaIMSS = null;
                        }
                    }

                    emp.IdUnidadNegocio = empleado.IdUnidadNegocio;
                    emp.IdCentroCostos = empleado.IdCentroCostos;
                    emp.IdDepartamento = empleado.IdDepartamento;
                    emp.IdPuesto = empleado.IdPuesto;
                    emp.IdSucursal = empleado.IdSucursal;
                    emp.IdPrestaciones = empleado.IdPrestaciones;
                    emp.IdEntidad = empleado.IdEntidad;
                    emp.ClaveEmpleado = empleado.ClaveEmpleado;
                    emp.Nombre = empleado.Nombre.ToUpper();
                    emp.ApellidoPaterno = empleado.ApellidoPaterno.ToUpper();
                    emp.ApellidoMaterno = empleado.ApellidoMaterno != null ? empleado.ApellidoMaterno.ToUpper() : "";
                    //emp.Sexo = empleado.Sexo.ToUpper();
                    //emp.EstadoCivil = empleado.EstadoCivil.ToUpper();                    
                    emp.SD = empleado.SD;
                    emp.SDIMSS = empleado.SDIMSS;
                    emp.IdBancoTrad = empleado.IdBancoTrad;
                    emp.CuentaBancariaTrad = empleado.CuentaBancariaTrad;
                    emp.CuentaInterbancariaTrad = empleado.CuentaInterbancariaTrad;
                    emp.Curp = empleado.Curp;
                    emp.Rfc = empleado.Rfc;
                    emp.CorreoElectronico = empleado.CorreoElectronico;
                    emp.FechaNacimiento = null;
                    emp.idSindicato = empleado.Idsindicato;
                    emp.IdArea = empleado.idArea;
                    emp.PremioP = empleado.Premio;
                    emp.Telefono = empleado.NumeroTelefonico;

                    if (empleado.FechaNacimiento != null)
                    {
                        emp.FechaNacimiento = DateTime.Parse(empleado.FechaNacimiento);
                    }
                    else
                    {
                        emp.FechaNacimiento = null;
                    }


                    if (empleado.FechaReconocimientoAntiguedad != null)
                    {
                        emp.FechaReconocimientoAntiguedad = Convert.ToDateTime(empleado.FechaReconocimientoAntiguedad).Date;
                    }
                    else
                    {
                        emp.FechaReconocimientoAntiguedad = null;
                    }

                    if (empleado.FechaBaja != null)                                 
                    {
                        emp.FechaBaja = Convert.ToDateTime(empleado.FechaBaja);     
                    }
                    else                                                            
                    {
                        emp.FechaBaja = null;                                       
                    }

                    emp.Esquema = empleado.Esquema;
                    emp.TipoContrato = empleado.TipoContrato;

                    if (empleado.FechaTerminoContrato != null)
                    {
                        emp.FechaTerminoContrato = Convert.ToDateTime(empleado.FechaTerminoContrato).Date;
                    }
                    else
                    {
                        emp.FechaTerminoContrato = null;
                    }
                    
                    emp.IdEstatus = empleado.IdEstatus;
                    emp.RFCSubContratacion = empleado.RFCSubContratacion;
                    emp.IdJornada= empleado.IdJornada;
                    emp.IdBancoViaticos = empleado.IdBancoViaticos;
                    emp.CuentaBancariaViaticos = empleado.CuentaBancariaViaticos;
                    emp.CuentaInterbancariaViaticos = empleado.CuentaInterBancariaViaticos;

                    emp.IdCodigoPostal = empleado.IdCodigoPostalFiscal;
                    emp.Calle = empleado.CalleFiscal;
                    emp.noExt = empleado.NumeroExtFiscal;
                    emp.noInt = empleado.NumeroIntFiscal;
                    emp.CP = empleado.CodigoPostalFiscal;

                    if (empleado.IdEstatus == 2 || empleado.IdEstatus == 3)
                    {

                        string resultado = string.Empty;//Validacion de los 5 dias habiles para bajas DRR
                        DateTime FechaFinal = DateTime.Today;//Validacion de los 5 dias habiles para bajas DRR
                        DateTime FechaInicial = Convert.ToDateTime(empleado.FechaBaja);//Validacion de los 5 dias habiles para bajas DRR

                        if (FechaInicial <= FechaFinal)
                        {
                            if (GetDiasHabiles(FechaInicial, FechaFinal) <= 5)//Validacion de los 5 dias habiles para bajas DRR
                            {

                                emp.IdEstatus = empleado.IdEstatus;
                                emp.FechaBaja = Convert.ToDateTime(empleado.FechaBaja).Date;
                                emp.MotivoBaja = empleado.MotivoBaja;
                                emp.Recontratable = empleado.Recontratable;
                            }
                            else //Validacion de los 5 dias habiles para bajas DRR
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            if (GetDiasAdelatados(FechaInicial, FechaFinal) <= 1)
                            {
                                emp.IdEstatus = empleado.IdEstatus;
                                emp.FechaBaja = Convert.ToDateTime(empleado.FechaBaja).Date;
                                emp.MotivoBaja = empleado.MotivoBaja;
                                emp.Recontratable = empleado.Recontratable;
                            }
                            else
                            {
                                return -2;
                            }
                        }
                    }
                    emp.IdModificacion = empleado.IdModificacion;
                    emp.FechaModificacion = DateTime.Now;
                    int value = entity.SaveChanges();
                    EditEmpleadIC(empleado);
                    return value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Aqui la ex" + ex.ToString());
                    return 0;
                }
            }
        }

        /// <summary>
        /// Método para guardar la información complementaria editada del empleado
        /// </summary>
        /// <param name="empleado">Información complementaria editada del empleado</param>
        /// <returns>Valor para rectificar que el registro se realizó con exito</returns>
        public int EditEmpleadIC(Empleado empleado)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var emp = (from e in entity.EmpleadoInformacionComplementaria where e.IdEmpleado == empleado.IdEmpleado select e).FirstOrDefault();

                if (emp != null)
                {
                    emp.IdCodigoPostal = empleado.IdCodigoPostal;
                    emp.Calle = empleado.Calle;
                    emp.NumeroExt = empleado.NumeroExt;
                    emp.NumeroInt = empleado.NumeroInt;
                    emp.IdEstatus = empleado.IdEstatus;
                    emp.IdModificacion = empleado.IdModificacion;
                    emp.FechaModificacion = DateTime.Now;
                    int value = entity.SaveChanges();
                    return value;
                }
                else
                {
                    return AddEmpleadoComplementaria(empleado);
                }
            }
        }

        /// <summary>
        /// Método para parsear de string a decimal del neto a pagar
        /// </summary>
        /// <param name="IdEmpleado">Identificador del Empleado</param>
        /// <param name="NetoPagar">Neto a pagar</param>
        public void ProcesoNetoPagar(int IdEmpleado, string NetoPagar)
        {
            decimal _neto = 0;
            try { _neto = decimal.Parse(NetoPagar); } catch { }

            ActualizaNetoPagarEmpleado(IdEmpleado, _neto);
        }

        /// <summary>
        /// Método para actualizar el neto a pagar
        /// </summary>
        /// <param name="IdEmpleado">Identidficador del empleado</param>
        /// <param name="NetoPagar">Neto a pagar</param>
        public void ActualizaNetoPagarEmpleado(int IdEmpleado, decimal NetoPagar)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var emp = (from e in entity.Empleados where e.IdEmpleado == IdEmpleado select e).FirstOrDefault();

                if (emp != null)
                {
                    emp.NetoPagar = NetoPagar;
                    entity.SaveChanges();
                }
            }

        }

        /// <summary>
        /// Método que retorna codigos postales por codigo postal string con busqueda coincidente
        /// </summary>
        /// <param name="cp">Codigo postal</param>
        /// <returns>Informaión relacionada con el codigo postal</returns>
        public List<CodigoPostal> GetCodigoPostales(string cp)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var codigosList = (from b in entity.CodigoPostal where b.Codigo.StartsWith(cp) select b).Distinct().Take(15).ToList();
                return codigosList;
            }
        }

        /// <summary>
        /// Método que retorna codigos postales por codigo postal string con busqueda exacta
        /// </summary>
        /// <param name="cp">Codigo postal</param>
        /// <returns>Informacón relacionada con el codigo postal</returns>
        public List<CP> GetCodigoPostalesByString(string cp)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var query = (from b in entity.CodigoPostal
                             join c in entity.Cat_EntidadFederativa
                             on b.EntidadFederativaId equals c.Id
                             where b.Codigo.Equals(cp)
                             select new
                             {
                                 b.Id,
                                 b.Codigo,
                                 b.Colonia,
                                 b.Municipio,
                                 c.Nombre
                             }).ToList();

                List<CP> cps = new List<CP>();
                query.ForEach(x => { cps.Add(new CP { Id = x.Id, Codigo = x.Codigo, Colonia = x.Colonia, Municipio = x.Municipio, Entidad = x.Nombre }); });
                return cps;
            }
        }

        /// <summary>
        /// Método que retorna codigos postales por codigo postal string con busqueda exacta    
        /// </summary>
        /// <param name="IdCodigoPostal">Identificador del codigo postal</param>
        /// <returns>Información relacionada con el codigo postal</returns>
        public CP GetCodigoPostalesById(int IdCodigoPostal)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var query = (from b in entity.CodigoPostal
                             join c in entity.Cat_EntidadFederativa
                             on b.EntidadFederativaId equals c.Id
                             where b.Id == IdCodigoPostal
                             select new
                             {
                                 b.Id,
                                 b.Codigo,
                                 b.Colonia,
                                 b.Municipio,
                                 c.Nombre
                             }).FirstOrDefault();

                CP cp = new CP()
                {
                    Id = query.Id,
                    Codigo = query.Codigo,
                    Colonia = query.Colonia,
                    Municipio = query.Municipio,
                    Entidad = query.Nombre
                };
                return cp;
            }
        }

        //Insercion Empleado en batch
        /// <summary>
        /// Método para la caraga masiva de empleados
        /// </summary>
        /// <param name="empleados">Lista con la información de los empleados</param>
        /// <returns>Valor para rectificar que los registros se realizaron con exito</returns>
        public int AddEmpleadoBatch(List<Empleado> empleados, decimal SMGV)
        {
            int value = 0;
            decimal _SDI_Minimo = SMGV * 1.0493M;
            _SDI_Minimo = Math.Round(_SDI_Minimo, 2);

            List<Empleados> empSave = new List<Empleados>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                try
                {
                    foreach (Empleado empleado in empleados)
                    {
                        var emp = new Empleados();

                        emp.IdUnidadNegocio = empleado.IdUnidadNegocio;
                        emp.IdCentroCostos = empleado.IdCentroCostos;
                        emp.IdDepartamento = empleado.IdDepartamento;
                        emp.IdPuesto = empleado.IdPuesto;
                        emp.IdRegistroPatronal = empleado.IdRegistroPatronal;
                        emp.IdEntidad = empleado.IdEntidad;
                        emp.ClaveEmpleado = empleado.ClaveEmpleado;
                        emp.Nombre = empleado.Nombre;
                        emp.ApellidoPaterno = empleado.ApellidoPaterno;
                        emp.ApellidoMaterno = empleado.ApellidoMaterno;
                        emp.Sexo = empleado.Sexo;
                        emp.EstadoCivil = empleado.EstadoCivil;
                        emp.FechaNacimiento = Convert.ToDateTime(empleado.FechaNacimiento);
                        if (empleado.SD != null) { emp.SD = empleado.SD; } else { emp.SD = 0; }
                        if (empleado.SDIMSS != null) { emp.SDIMSS = empleado.SDIMSS; } else { emp.SDIMSS = 0; }
                        if (empleado.SDI != null) { emp.SDI = empleado.SDI; } else { emp.SDI = 0; }
                        emp.IdBancoTrad = empleado.IdBancoTrad;
                        emp.CuentaBancariaTrad = empleado.CuentaBancariaTrad;
                        emp.CuentaInterbancariaTrad = empleado.CuentaInterbancariaTrad;
                        emp.Curp = empleado.Curp;
                        emp.Rfc = empleado.Rfc;
                        emp.Imss = empleado.Imss;
                        emp.CorreoElectronico = empleado.CorreoElectronico;
                        emp.FechaReconocimientoAntiguedad = Convert.ToDateTime(empleado.FechaReconocimientoAntiguedad);
                        emp.FechaAltaIMSS = Convert.ToDateTime(empleado.FechaAltaIMSS);
                        emp.Esquema = empleado.Esquema;
                        emp.TipoContrato = empleado.TipoContrato;
                        emp.RFCSubContratacion = empleado.RFCSubContratacion;
                        emp.IdEstatus = 1;
                        emp.IdCaptura = empleado.IdCaptura;
                        emp.FechaCaptura = DateTime.Now;
                        emp.IdSucursal = empleado.IdSucursal;
                        emp.Calle = empleado.Calle;
                        emp.noExt = empleado.NumeroExt;
                        emp.noInt = empleado.NumeroInt;
                        emp.CP = empleado.CodigoPostal;
                        emp.Nacionalidad = empleado.Nacionalidad;

                        // Validamos si el SDI viene en 0
                        if (emp.SDIMSS > 0 && emp.SDI == 0)
                        {
                            emp.SDI = emp.SDIMSS * 1.0493M;
                            emp.SDI = Math.Round((decimal)emp.SDI, 2);
                        }

                        // Validamos que el SDI no sea menor al minimo
                        if (emp.SDIMSS > 0 && emp.SDI < _SDI_Minimo)
                        {
                            emp.SDI = _SDI_Minimo;
                        }

                        entity.Empleados.Add(emp);
                        empSave.Add(emp);
                    }

                    value = entity.SaveChanges();

                    if (value > 0)
                    {
                        foreach (Empleados e in empSave)
                        {
                            empleados.Find(x => x.ClaveEmpleado.Equals(e.ClaveEmpleado) && x.Nombre.Equals(e.Nombre)).IdEmpleado = e.IdEmpleado;

                            if (e.Rfc != null && e.Rfc.Length > 0)
                            {
                                SetPassword(e.IdEmpleado);
                            }
                        }

                        AddEmplieadoInfoComBatch(empleados);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Excepcion del Batch" + ex.ToString());
                }
            }

            return value;
        }

        /// <summary>
        /// Método para la caraga masiva de la información complementaria del empleado
        /// </summary>
        /// <param name="empleados">Información complementaria de los empleados</param>
        /// <returns>Valor para rectificar que los registros se realizaron con exito</returns>
        private int AddEmplieadoInfoComBatch(List<Empleado> empleados)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                foreach (Empleado e in empleados)
                {
                    EmpleadoInformacionComplementaria empics = new EmpleadoInformacionComplementaria()
                    {
                        IdEmpleado = e.IdEmpleado,
                        IdCodigoPostal = e.IdCodigoPostal,
                        Calle = e.Calle,
                        NumeroExt = e.NumeroExt,
                        NumeroInt = e.NumeroInt,
                        IdEstatus = e.IdEstatus,
                        IdCaptura = e.IdCaptura,
                        FechaCaptura = e.FechaCaptura
                    };

                    entity.EmpleadoInformacionComplementaria.Add(empics);
                }
                return entity.SaveChanges();
            }
        }

        /// <summary>
        /// Método para la baja masiva de empleados
        /// </summary>
        /// <param name="empleados">Información de los empleados</param>
        /// <returns>Valor para rectificar que los registros se realizaron con exito</returns>
        public int DismissBatch(List<Empleado> empleados)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                foreach (var e in empleados)
                {
                    var emp = (from em in entity.Empleados where em.ClaveEmpleado == e.ClaveEmpleado && em.IdUnidadNegocio == e.IdUnidadNegocio && em.IdEstatus == 1 select em).FirstOrDefault();
                    emp.FechaBaja = Convert.ToDateTime(e.FechaBaja);
                    emp.MotivoBaja = e.MotivoBaja;
                    emp.Recontratable = e.Recontratable;
                    emp.IdModificacion = e.IdModificacion;
                    emp.IdEstatus = 2;
                    emp.FechaModificacion = DateTime.Now;
                }

                return entity.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener los Centros de costos por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Listado de los centros de costos</returns>
        public List<Cat_CentroCostos> GetCentrosCostosByClient(int idCliente)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var CentrosCostos = (from cc in entity.Cat_CentroCostos where cc.IdCliente == idCliente select cc).ToList();
                return CentrosCostos;
            }
        }

        /// <summary>
        /// Método para obtener los departamentos por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Listado de los departamentos</returns>
        public List<Cat_Departamentos> GetDepartamentosByCliente(int idCliente)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var Departamentos = (from cc in entity.Cat_Departamentos where cc.IdCliente == idCliente select cc).ToList();
                return Departamentos;
            }
        }

        /// <summary>
        /// Método para obtener los puestos por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Listado de los puestos</returns>
        public List<Cat_Puestos> GetPuestosByCliente(int idCliente)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var Puestos = (from cc in entity.Cat_Puestos where cc.IdCliente == idCliente select cc).ToList();
                return Puestos;
            }
        }

        /// <summary>
        /// Método para obtener los registros patronales por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Listado d elos registros patronales</returns>
        public List<Cat_RegistroPatronal> GetRegistrosPatronalesByCliente(int idCliente)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var Puestos = (from cc in entity.Cat_RegistroPatronal where cc.IdCliente == idCliente select cc).ToList();
                return Puestos;
            }
        }

        /// <summary>
        /// Método para obtener el listado de bancos
        /// </summary>
        /// <returns>Listado de bancos</returns>
        public List<Cat_Bancos> GetBancosGral()
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var Bancos = (from cc in entity.Cat_Bancos select cc).ToList();
                return Bancos;
            }
        }

        /// <summary>
        /// Método para obtene rlas areas por cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Listado de las areas</returns>
        public List<Cat_Areas> GetAreasByClient(int idCliente)
        {
            using (NominaEntities1 entity = new NominaEntities1())
            {
                var LstAreas = (from cc in entity.Cat_Areas where cc.IdCliente == idCliente select cc).ToList();
                return LstAreas;
            }
        }

        /// <summary>
        /// Método para obtener los sindicatos
        /// </summary>
        /// <returns>Listado de los sindicatos</returns>
        public List<Sindicatos> GetAreasBySindicatos()
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var LstAreas = (from cc in entity.Sindicatos where cc.IdEstatus == 1 select cc).ToList();
                return LstAreas;
            }
        }

        /// <summary>
        /// Método para obtener las sucursales del cliente
        /// </summary>
        /// <param name="IdCliente">Idnetificador del cliente</param>
        /// <returns>Listado de las sucursales</returns>
        public List<Cat_Sucursales> GetAllSucursales(int IdCliente)
        {
            using(TadaEmpleados ctx = new TadaEmpleados())
            {
                var lista = ctx.Cat_Sucursales.Where(p => p.IdEstatus==1 && p.IdCliente==IdCliente).ToList();
                return lista;
            }
        }

        /// <summary>
        /// Metodo para obtener la información del empleado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns>Información del empleado</returns>
        public vEmpleados GetvEmpleado(int IdEmpleado)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var query = (from b in entity.vEmpleados.Where(x => x.IdEmpleado == IdEmpleado) select b).FirstOrDefault();

                return query;
            }
        }

        /// <summary>
        /// Método para obtener la información de los empleados por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de la informacion de los empleado</returns>
        public List<vEmpleados> GetvEmpleados(int IdUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var query = (from b in entity.vEmpleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).ToList();

                return query;
            }
        }

        /// <summary>
        /// Método para obtene rtodo slos empleado sactivos por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de los empleados activos</returns>
        public List<vEmpleados> GetAllvEmpleados(int IdUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                int[] IdEstatus = { 1, 2, 3 };
                var query = (from b in entity.vEmpleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && IdEstatus.Contains(x.IdEstatus)) select b).OrderBy(x=> x.IdEstatus).ToList();

                return query;
            }
        }

        /// <summary>
        /// Método para obtener los periodos de nomina en los que haya participado el empleado
        /// </summary>
        /// <param name="IdEmpleado">Identificador del empleado</param>
        /// <returns>Listado de los periodos de nomina en los que haya participado el empleado</returns>
        public List<EmpleadoPeriodo> getPeriodosProcesados(int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                int?[] estatus = { 1, 4 };
                var procesados = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && estatus.Contains(x.IdEstatus)) select b.IdPeriodoNomina).ToArray();

                var periodos = (from b in entidad.PeriodoNomina.Where(x => x.IdEstatus == 1 && procesados.Contains(x.IdPeriodoNomina)) select b).ToList();

                List<EmpleadoPeriodo> empPer = new List<EmpleadoPeriodo>();
                periodos.ForEach(x => { empPer.Add(new EmpleadoPeriodo { IdPeriodoNomina = x.IdPeriodoNomina, Periodo = x.Periodo, Validacion = false }); });

                return empPer;
            }
        }

        /// <summary>
        /// Método para obtener los catalogos necesarios para las altas de los empleados 
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Catalogos para el alta de los empleados</returns>
        public ModelCatalogosAltas getCatalogos(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cc = (from b in entidad.Cat_CentroCostos.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();
                var dep = (from b in entidad.Cat_Departamentos.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();
                var suc = (from b in entidad.Cat_Sucursales.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();
                var ptos = (from b in entidad.Cat_Puestos.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();
                var ban = (from b in entidad.Cat_Bancos.Where(x => x.IdEstatus == 1) select b).ToList();
                var reg = ObtenerRPByIdCliente(IdCliente);
                var ent = (from b in entidad.Cat_EntidadFederativa select b).ToList();
                var are = (from b in entidad.Cat_Areas.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();
                var sin = (from b in entidad.Sindicatos.Where(x => x.IdEstatus == 1) select b).ToList();
                var jor = (from b in entidad.Cat_Jornadas.Where(x => x.IdEstatus == 1) select b).ToList();
                var hono = (from b in entidad.Cat_HonorariosFacturas.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).ToList();


                ModelCatalogosAltas model = new ModelCatalogosAltas();
                model.bancos = ban;
                model.centroCostos = cc;
                model.puestos = ptos;
                model.departamentos = dep;
                model.registroPat = reg;
                model.entidades = ent;
                model.sucursales = suc;
                model.areasPat = are;
                model.Sindicatos = sin;
                model.Jornadas=jor;
                model.Cat_Hono = hono;


                return model;
            }
        }

        /// <summary>
        /// Método para obtene rel reporte en excel de los empleados activos 
        /// </summary>
        /// <param name="idunidadnegocio">Identificador de la unidad de negocio</param>
        /// <returns>Archivo Excel con lo empleados acticos</returns>
        public byte[] Excel(int idunidadnegocio)
        {

            TadaEmpleados entities = new TadaEmpleados();

            var emp = (from b in entities.Empleados.Where(x => x.IdUnidadNegocio == idunidadnegocio && x.IdEstatus == 1) select b).ToList();
            var campos = (from b in entities.Cat_CamposModificaEmp.Where(x => x.IdEstatus == 1) select b).ToList();

            List<DataColumn> camposDB = new List<DataColumn>();
            foreach (var item in campos)
            {
                camposDB.Add(new DataColumn(item.Descripcion));
            }

            var colDB = camposDB.ToArray();

            DataTable dt = new DataTable("Empleados");
            dt.Columns.AddRange(colDB);

            foreach (var customer in emp)
            {
                dt.Rows.Add(customer.ClaveEmpleado, customer.Nombre, customer.ApellidoPaterno, customer.ApellidoMaterno);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// Método para obtener el salario minimo vigente
        /// </summary>
        /// <returns>Valor del salario minimo vigente</returns>
        public decimal ObtenSMV()
        {
            NominaEntities1 context = new NominaEntities1();
            decimal _smv = 0;
            DateTime fecha = DateTime.Now;
            var resultado = (from b in context.Sueldos_Minimos
                             where fecha >= b.FechaInicio
                             select b).OrderByDescending(x => x.FechaInicio).FirstOrDefault();

            if (resultado != null)
            {
                _smv = (decimal)resultado.SalarioMinimoGeneral;
            }

            return _smv;
        }

        /// <summary>
        /// Método para obtener el salario del empleado
        /// </summary>
        /// <param name="idEmpleado">Identificador del empleado</param>
        /// <returns>Informacion del salario del empleado</returns>
        public ModelModificacionSueldos GetEmpleadoParaMS(int idEmpleado)
        {

            using (TadaEmpleadosEntities entity = new TadaEmpleadosEntities())
            {
                var emp = (from b in entity.Empleados
                           where b.IdEmpleado.Equals(idEmpleado) && b.IdEstatus == 1
                           select b).FirstOrDefault();

                ModelModificacionSueldos empleado = new ModelModificacionSueldos();

                empleado.IdEmpleado = emp.IdEmpleado;
                empleado.IdUnidadNegocio = emp.IdUnidadNegocio;

                empleado.SD = emp.SD;
                empleado.SDI = emp.SDI;
                empleado.SDIMSS = emp.SDIMSS;
                empleado.FechaReconocimientoAntiguedad = emp.FechaReconocimientoAntiguedad.ToString();

                return empleado;
            }
        }

        /// <summary>
        /// Método para cambiar d eunidad de negocio al empleado
        /// </summary>
        /// <param name="emp">Información del empleado</param>
        /// <param name="IdNuevaUnidad">Identificador de la unidad de negocio</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        public void CambiaUnidadNegocio(List<Empleados> emp, int IdNuevaUnidad, int IdUsuario)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                List<int> ids = emp.Select(x => x.IdEmpleado).ToList();
                var empleados = (from b in entidad.Empleados.Where(x => ids.Contains(x.IdEmpleado)) select b).ToList();

                foreach (var item in empleados)
                {
                    item.IdUnidadNegocio = IdNuevaUnidad;
                    item.IdModificacion = IdUsuario;
                    item.FechaModificacion = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para buscar empleados con ausentismos
        /// </summary>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de los empleados con usentismos</returns>
        public List<Empleado> GetEmpleadosAusen(int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();
                query.ForEach(x => { empleados.Add(new Empleado { IdEmpleado = x.IdEmpleado, ClaveEmpleado = x.ClaveEmpleado, Nombre = x.Nombre, ApellidoPaterno = x.ApellidoPaterno, ApellidoMaterno = x.ApellidoMaterno, Rfc = x.Rfc, Curp = x.Curp, }); });

                return empleados;
            }
        }

        /// <summary>
        /// Método para obtener empleados por clave 
        /// </summary>
        /// <param name="clave">Clave del empleado</param>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de emopleados</returns>
        public List<Empleado> GetEmpleadosByClaveAusen(string clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where b.ClaveEmpleado == clave && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                    });
                });
                return empleados;
            }
        }

        /// <summary>
        /// Método para obtener a los empleados por nombre 
        /// </summary>
        /// <param name="nombre">Nombre del empleado</param>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de los empleados</returns>
        public List<Empleado> GetEmpleadosByNombreAusen(string nombre, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where (b.Nombre + " " + b.ApellidoPaterno + " " + b.ApellidoMaterno).StartsWith(nombre) && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss
                    });
                });
                return empleados;
            }
        }

        //Retorna empleado para editar empleado
        /// <summary>
        /// Método para obtener al empleado para editar
        /// </summary>
        /// <param name="idEmpleado">Identificador del empleado</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Empleado a editar</returns>
        public Empleado GetEmpleadoToEditBaja(int idEmpleado, int IdUnidadNegocio, int IdCliente)
        {

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                int[] ids = { 2, 3 };
                var emp = (from b in entity.Empleados
                           where b.IdEmpleado.Equals(idEmpleado) && b.IdUnidadNegocio == IdUnidadNegocio && ids.Contains(b.IdEstatus)
                           select b).FirstOrDefault();

                Empleado empleado = new Empleado();

                empleado.IdUnidadNegocio = emp.IdUnidadNegocio;
                empleado.IdCentroCostos = emp.IdCentroCostos;
                empleado.CentrosCostosList = GetCentrosCostos(IdCliente);

                empleado.IdDepartamento = emp.IdDepartamento;
                empleado.DepartamentoList = GetDepartamentos(IdCliente);
                empleado.idArea = emp.IdArea;
                empleado.AreaList = GetAreas(IdCliente);

                empleado.IdPuesto = emp.IdPuesto;
                empleado.PuestosList = GetPuestos(IdCliente);

                empleado.IdSucursal = emp.IdSucursal;
                empleado.SucursalList = GetSucursales(IdCliente);

                empleado.RegistrosPatronalesList = GetRegistrosPatronales(IdCliente);

                empleado.IdEntidad = emp.IdEntidad;
                empleado.EntidadFederativaList = GetEntidadesFederativas();

                empleado.IdBancoTrad = emp.IdBancoTrad;

                empleado.BancosList = GetBancos();

                empleado.IdEmpleado = emp.IdEmpleado;

                empleado.ClaveEmpleado = emp.ClaveEmpleado;
                empleado.Nombre = emp.Nombre;
                empleado.ApellidoPaterno = emp.ApellidoPaterno;
                empleado.ApellidoMaterno = emp.ApellidoMaterno;

                empleado.Sexo = emp.Sexo;
                empleado.SexoList = GetSexo();

                empleado.EstadoCivil = emp.EstadoCivil;
                empleado.EstadoCivilList = GetEstadoCivil();



                empleado.CuentaBancariaTrad = emp.CuentaBancariaTrad;
                empleado.CuentaInterbancariaTrad = emp.CuentaInterbancariaTrad;
                empleado.Curp = emp.Curp;
                empleado.Rfc = emp.Rfc;
                empleado.Imss = emp.Imss;
                empleado.CorreoElectronico = emp.CorreoElectronico;

                try { empleado.FechaAltaIMSS = ((DateTime)emp.FechaAltaIMSS).ToShortDateString(); } catch { empleado.FechaAltaIMSS = string.Empty; }
                try { empleado.FechaReconocimientoAntiguedad = ((DateTime)emp.FechaReconocimientoAntiguedad).ToShortDateString(); } catch { empleado.FechaReconocimientoAntiguedad = string.Empty; }
                empleado.CodigoPostalFiscal = emp.CP;

                try
                {
                    empleado.FechaNacimiento = emp.FechaNacimiento.Value.ToShortDateString();
                }
                catch
                {
                    empleado.FechaNacimiento = null;
                }

                try
                {
                    empleado.FechaBaja = emp.FechaBaja.Value.ToShortDateString();
                }
                catch (Exception)
                {

                    empleado.FechaBaja = null;
                }



                empleado.Recontratable = emp.Recontratable;
                empleado.RecontratableList = GetRecontratable();
                empleado.Esquema = emp.Esquema;
                empleado.EsquemasList = GetEsquema();

                empleado.IdEstatus = emp.IdEstatus;
                empleado.EstatusList = GetEstatus();

                empleado.TipoContrato = emp.TipoContrato;
                empleado.TiposContratoList = GetTIpoContrato();

                try { empleado.IdPrestaciones = (int)emp.IdPrestaciones; } catch { empleado.IdPrestaciones = 0; }
                empleado.PrestacionesList = GetPrestaciones(IdCliente);

                if (empleado != null)
                {
                    empleado = GetInfoComEmpleado(empleado);
                }
                return empleado;
            }
        }

        /// <summary>
        /// Método par aobtener loa empleados para editar por su nombre
        /// </summary>
        /// <param name="nombre">Nombre del empleado</param>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de empleados</returns>
        public List<Empleado> GetEmpleadosByNombreBaja(string nombre, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var query = (from b in entity.Empleados where (b.Nombre + " " + b.ApellidoPaterno + " " + b.ApellidoMaterno).StartsWith(nombre) && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 2 select b).ToList();

                List<Empleado> emp = new List<Empleado>();

                Empleado empleados = new Empleado();
                foreach (var item in query)
                {
                    empleados.IdEmpleado = item.IdEmpleado;
                    empleados.ClaveEmpleado = item.ClaveEmpleado;
                    empleados.Nombre = item.Nombre;
                    empleados.ApellidoPaterno = item.ApellidoPaterno;
                    empleados.ApellidoMaterno = item.ApellidoMaterno;
                    empleados.Rfc = item.Rfc;
                    empleados.Curp = item.Curp;
                    empleados.Imss = item.Imss;
                    empleados.SD = item.SD;
                    empleados.SDIMSS = item.SDIMSS;
                    empleados.SDI = item.SDI;
                    empleados.FechaBaja = item.FechaBaja != null ? item.FechaBaja.Value.ToShortDateString() : null;

                    emp.Add(empleados);
                }
                return emp;
            }
        }

        /// <summary>
        /// Método para obtener a los empleados por clave, nombre ó rfc 
        /// </summary>
        /// <param name="clave">Clave empleado</param>
        /// <param name="nombre">Nombre del empleado</param>
        /// <param name="rfc">RFC del empleado</param>
        /// <param name="claves">Claves de los empleados</param>
        /// <param name="IdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Listado de empleados</returns>
        public List<vEmpleados> GetEmpleadosByCamposBusqueda(string clave, string nombre, string rfc, string claves, int IdUnidadNegocio)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                var result = new List<vEmpleados>();

                int[] estatus = { 1, 2, 3 };
                if (claves != null && claves.Length > 0)
                {
                    var _claves = claves.Split(',').ToArray();
                    var emp = entidad.vEmpleados.Where(x => estatus.Contains(x.IdEstatus) && x.IdUnidadNegocio == IdUnidadNegocio && _claves.Contains(x.ClaveEmpleado)).ToList();

                    result = emp;
                }
                else
                {
                    var emp = entidad.vEmpleados.Where(x => estatus.Contains(x.IdEstatus) && x.IdUnidadNegocio == IdUnidadNegocio).ToList();

                    if (clave != null && clave.Length > 0)
                        result = emp.Where(x => x.ClaveEmpleado == clave).ToList();

                    if (nombre != null && nombre.Length > 0)
                        result = emp.Where(x => x.NombreCompleto.ToUpper().Contains(nombre.ToUpper())).ToList();

                    if (rfc != null && rfc.Length > 0)
                        result = emp.Where(x => x.Rfc == rfc).ToList();
                }

                return result;
            }
        }

        /// <summary>
        /// Método para obtene al empleado por su clave para modificar
        /// </summary>
        /// <param name="clave">Clave empleado</param>
        /// <param name="idUnidadNegocio">Identificador d ela unidad de negocio</param>
        /// <returns>Listado de empleados</returns>
        public List<Empleado> GetEmpleadosByClaveBaja(string clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {

                var query = (from b in entity.Empleados where b.ClaveEmpleado == clave && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 2 select b).OrderBy(x => x.FechaModificacion).ToList();

                List<Empleado> emp = new List<Empleado>();


                foreach (var item in query)
                {
                    Empleado empleados = new Empleado();
                    empleados.IdEmpleado = item.IdEmpleado;
                    empleados.ClaveEmpleado = item.ClaveEmpleado;
                    empleados.Nombre = item.Nombre;
                    empleados.ApellidoPaterno = item.ApellidoPaterno;
                    empleados.ApellidoMaterno = item.ApellidoMaterno;
                    empleados.Rfc = item.Rfc;
                    empleados.Curp = item.Curp;
                    empleados.Imss = item.Imss;
                    empleados.SD = item.SD;
                    empleados.SDIMSS = item.SDIMSS;
                    empleados.SDI = item.SDI;                    
                    empleados.FechaBaja = item.FechaBaja.ToString();

                    emp.Add(empleados);

                }


                return emp;
            }
        }

        /// <summary>
        /// Método para agregar un empleado
        /// </summary>
        /// <param name="empleado">Información del empleado</param>
        /// <returns>Valor para rectificar que el registro se realizó con exito</returns>
        public int AddEmpleadoBaja(Empleado empleado)
        {
            int value = 0;


            List<Empleados> empleados = new List<Empleados>();

            using (TadaEmpleados entity = new TadaEmpleados())
            {

                var query = (from b in entity.Empleados where b.IdEmpleado == empleado.IdEmpleado && b.IdEstatus == 2 select b).FirstOrDefault();

                Empleados emp = new Empleados()
                {
                    IdUnidadNegocio = empleado.IdUnidadNegocio,
                    TipoContrato = query.TipoContrato,
                    Esquema = query.Esquema,
                    IdCentroCostos = query.IdCentroCostos,
                    IdDepartamento = query.IdDepartamento,
                    IdArea = query.IdArea,
                    IdPuesto = query.IdPuesto,
                    IdSucursal = query.IdSucursal,
                    IdRegistroPatronal = empleado.IdRegistroPatronal,
                    IdEntidad = query.IdEntidad,
                    ClaveEmpleado = empleado.ClaveEmpleado,
                    Nombre = empleado.Nombre.ToUpper(),
                    ApellidoPaterno = empleado.ApellidoPaterno.ToUpper(),
                    ApellidoMaterno = empleado.ApellidoMaterno.ToUpper(),
                    Sexo = query.Sexo,
                    EstadoCivil = query.EstadoCivil,
                    FechaNacimiento = Convert.ToDateTime(empleado.FechaNacimiento),
                    SD = empleado.SD,
                    SDIMSS = empleado.SDIMSS,
                    SDI = empleado.SDI,
                    IdBancoTrad = query.IdBancoTrad,
                    CuentaBancariaTrad = query.CuentaBancariaTrad,
                    CuentaInterbancariaTrad = query.CuentaInterbancariaTrad,
                    Curp = query.Curp.ToUpper(),
                    Rfc = query.Rfc.ToUpper(),
                    Imss = empleado.Imss,
                    CorreoElectronico = empleado.CorreoElectronico,
                    FechaReconocimientoAntiguedad = Convert.ToDateTime(empleado.FechaReconocimientoAntiguedad),
                    FechaAltaIMSS = Convert.ToDateTime(empleado.FechaAltaIMSS),
                    IdEstatus = 1,
                    IdCaptura = empleado.IdCaptura,
                    FechaCaptura = DateTime.Now,
                    FechaBaja = Convert.ToDateTime(empleado.FechaBaja),
                    IdCodigoPostal = query.IdCodigoPostal,
                    CP = query.CP,
                    RutaCSF = query.RutaCSF,
                    Entidad = query.Entidad,
                    Calle = query.Calle,
                    noExt = query.noExt,
                    noInt = query.noInt,
                    Alcaldia = query.Alcaldia,
                    Nacionalidad = query.Nacionalidad,
                    
                };

                entity.Empleados.Add(emp);
                value = entity.SaveChanges();



                return value;
            }

        }

        public int EditEmpleadoBaja(Empleado empleado)
        {
            int value = 0;

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var query = (from b in entity.Empleados where b.IdEmpleado == empleado.IdEmpleado && b.IdEstatus == 2 select b).FirstOrDefault();

                if(query != null)
                {                    
                    query.Nombre = empleado.Nombre.ToUpper();
                    query.ApellidoPaterno = empleado.ApellidoPaterno.ToUpper();
                    query.ApellidoMaterno = empleado.ApellidoMaterno.ToUpper();                                
                    query.FechaNacimiento = Convert.ToDateTime(empleado.FechaNacimiento);                   
                    query.IdBancoTrad = empleado.IdBancoTrad;
                    query.CuentaBancariaTrad = empleado.CuentaBancariaTrad;
                    query.CuentaInterbancariaTrad = empleado.CuentaInterbancariaTrad;
                    query.Curp = empleado.Curp.ToUpper();
                    query.Rfc = empleado.Rfc.ToUpper();
                    query.Imss = empleado.Imss;
                    query.CorreoElectronico = empleado.CorreoElectronico;
                    query.FechaReconocimientoAntiguedad = Convert.ToDateTime(empleado.FechaReconocimientoAntiguedad);
                    query.FechaAltaIMSS = Convert.ToDateTime(empleado.FechaAltaIMSS);
                    query.CP = empleado.CodigoPostalFiscal;
                    query.IdModificacion = empleado.IdCaptura;
                    query.FechaBaja = Convert.ToDateTime(empleado.FechaBaja);
                    //query.FechaModificacion = DateTime.Now;  Para que no se afecten los movimientos de IMSS
                };

                
                value = entity.SaveChanges();

                return value;
            }

        }

        /// <summary>
        /// Método para validar que no se repita la clave de empleado
        /// </summary>
        /// <param name="claveempleado">Clave del empleado</param>
        /// <param name="idunidadN">Identificadoe de la unidad de negocio</param>
        /// <returns>true/false si es que existe una clave igual o no</returns>
        public bool RevisaCLaverepetida(string claveempleado, int idunidadN)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                var empleados = (from b in entidad.Empleados.Where(x => x.ClaveEmpleado == claveempleado && x.IdUnidadNegocio == idunidadN && x.IdEstatus == 1) select b).FirstOrDefault();

                if (empleados != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Método para obtener los registros patronales por cliente
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <returns>Listado de los registros patronales</returns>
        public List<Cat_RegistroPatronal> ObtenerRPByIdCliente(int IdCliente)
        {
            List<Cat_RegistroPatronal> listRegistros = new List<Cat_RegistroPatronal>();

            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                var rpPropios = (from b in entidad.Cat_RegistroPatronal where b.IdCliente == IdCliente && b.IdEstatus == 1 select b).ToList();
                foreach (var item in rpPropios)
                {
                    listRegistros.Add(item);
                }

                var rpAsignados = (from b in entidad.vClienteEmpresaEspecializada
                                   join c in entidad.Cat_RegistroPatronal on b.IdRegistroPatronal equals c.IdRegistroPatronal
                                   where b.IdCliente == IdCliente && b.IdEstatus == 1 && c.IdEstatus == 1
                                   select c).ToList();

                foreach (var item in rpAsignados)
                {
                    listRegistros.Add(item);
                }
            }

            return listRegistros;
        }

        /// <summary>
        /// Método para obtner el reporte de las incidencias por cliente
        /// </summary>
        /// <param name="idunidadnegocio">Identificador de la unidad de negocio</param>
        /// <param name="idcliente">Identificador del cliente</param>
        /// <returns>Reporte de las incidencias</returns>
        public byte[] ExcelIncidencias(int idunidadnegocio, int idcliente)
        {

            TadaEmpleados entities = new TadaEmpleados();

            var emp = (from b in entities.Empleados.Where(x => x.IdUnidadNegocio == idunidadnegocio && x.IdEstatus == 1) select b).ToList();
            var insi = (from b in entities.Cat_ConceptosNomina.Where(x => x.IdEstatus == 1 && x.IdCliente == idcliente) select b).ToList();

            List<string> ListFirstRow = new List<string>();
            List<string> ListSecondRow = new List<string>();

            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");
            ListFirstRow.Add(" ");

            ListSecondRow.Add("ClaveEmpleado");
            ListSecondRow.Add("Nombre");
            ListSecondRow.Add("RFC");

            foreach (var item in insi)
            {
                for (int i = 0; i < 2; i++)
                {

                    if (i == 0)
                    {
                        ListFirstRow.Add(item.IdConcepto.ToString() + "-" + "T");
                        ListSecondRow.Add(item.Concepto + " " + "Tradicional");

                    }
                    else
                    {
                        ListFirstRow.Add(item.IdConcepto.ToString() + "-" + "S");
                        ListSecondRow.Add(item.Concepto + " " + "Esquema");

                    }




                }

            }

            List<TadaNomina.Models.ClassCore.Excel> ListDatos = new List<Excel>();

            foreach (var customer in emp)
            {
                TadaNomina.Models.ClassCore.Excel data = new Excel();
                data.ClaveEmpleado = customer.ClaveEmpleado;
                data.Nombre = customer.Nombre + " " + customer.ApellidoMaterno + " " + customer.ApellidoPaterno;
                data.Rfc = customer.Rfc;
                ListDatos.Add(data);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var xl = wb.Worksheets.Add("Incidencias");
                int i = 1;
                foreach (string column in ListFirstRow)
                {
                    xl.Cell(1, i).Value = column;

                    if (i <= 3)
                    {
                        xl.Cell(1, i).Style.Fill.BackgroundColor = XLColor.Red;
                    }
                    else
                    {
                        xl.Cell(1, i).Style.Fill.BackgroundColor = XLColor.BlueBell;
                    }
                    xl.Cell(1, i).Style.Font.FontColor = XLColor.White;
                    xl.Cell(1, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    i++;
                }

                i = 1;
                foreach (string column in ListSecondRow)
                {
                    xl.Cell(2, i).Value = column;
                    if (i <= 3)
                    {
                        xl.Cell(2, i).Style.Fill.BackgroundColor = XLColor.Red;

                    }
                    else
                    {
                        xl.Cell(2, i).Style.Fill.BackgroundColor = XLColor.BlueBell;
                    }
                    xl.Cell(2, i).Style.Font.FontColor = XLColor.White;
                    xl.Cell(2, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    i++;
                }

                xl.Cell(3, 1).InsertData(ListDatos);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }


        public bool SetPassEmpleado(int IdEmpleado, int IdCliente, string correo, string token)
        {
            string email = "";
            email = correo;

            int flag = 0;
            flag = SetPassword(IdEmpleado);

            if (email == "")
                flag = 1;

            Uri url = new Uri(sStatics.servidor + "/api/Empleados/setPassword?IdEmpleado=" + IdEmpleado + "&IdCliente=" + IdCliente);

            if (flag == 2)
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Clear();
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers["Content-type"] = "application/json";
                    wc.Headers["Authorization"] = "Bearer " + token;

                    try
                    {
                        var resul = wc.DownloadString(url);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public int SetPassword(int IdEmpleado)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return ctx.sp_Crea_Pass_LaNube(IdEmpleado);
            }
        }

        public void ModificaDatosProy(int IdEmpleado, decimal SD, decimal SD_Real, decimal SDI, decimal Neto, DateTime? FechaIMSS, DateTime? FechaRecAnt)
        {
            using (TadaEmpleados ctx = new TadaEmpleados())
            {
                var emp = ctx.Empleados.Where(x => x.IdEmpleado == IdEmpleado).FirstOrDefault();

                if (emp != null)
                {
                    emp.SD_Proyeccion = SD_Real;
                    emp.SDIMSS_Proyeccion = SD;
                    emp.SDI_Proyeccion = SDI;
                    emp.Neto_Proyeccion = Neto;
                    emp.FechaAltaIMSS_Proyeccion = FechaIMSS;
                    emp.FechaReconocimientoAntiguedad_Proyeccion = FechaRecAnt;

                    ctx.SaveChanges();
                }
            }
        }

        public void EliminaDatosProyección(int IdUnidadNegocio)
        {
            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                var emp = entidad.Empleados.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1).ToList();

                foreach (var item in emp)
                {
                    item.SD_Proyeccion = null;
                    item.SDI_Proyeccion= null;
                    item.SDIMSS_Proyeccion = null;
                    item.Neto_Proyeccion = null;
                    item.FechaAltaIMSS_Proyeccion = null;
                    item.FechaReconocimientoAntiguedad_Proyeccion = null;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obter una lisa tipo SelectListItem con el Catalogo de Jornadas
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>lisa tipo SelectListItem con el Catalogo de departamentos</returns>
        public List<SelectListItem> GetJornadas(int idCliente)
        {
            List<SelectListItem> jornadas = new List<SelectListItem>();


            using (NominaEntities1 entity = new NominaEntities1())
            {
                var _jornadas = (from b in entity.Cat_Jornadas where b.IdCliente == idCliente select b).OrderBy(x => x.Jornada).ToList();

                _jornadas.ForEach(x => { jornadas.Add(new SelectListItem { Text = x.Jornada.ToUpper(), Value = x.IdJornada.ToString() }); });
            }

            return jornadas;
        }


        public List<Empleado> GetEmpleadosHonorarios(int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 && (b.TipoContrato == "HONORARIOS" || b.TipoContrato == "RESICO") select b).ToList();
                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        Contrato = x.TipoContrato
                    });
                });

                return empleados;
            }
        }


        public vHonorarios GetEmpleadosHonorariosv(int idhonorario)
        {
            using (NominaEntities1 entity = new NominaEntities1())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.vHonorarios where b.IdHonorarios == idhonorario select b).FirstOrDefault();
                return query;
            }
        }


        public List<vEmpleados> getvEmpleadosByListIds(List<int> IdsEmpleados, int IdUnidadNegocio)
        {
            List<vEmpleados> list = new List<vEmpleados>();
            using (TadaEmpleadosEntities ctx = new TadaEmpleadosEntities())
            {
                list = ctx.vEmpleados.Where(p => IdsEmpleados.Contains(p.IdEmpleado) && p.IdUnidadNegocio == IdUnidadNegocio).ToList();
            }
            return list;
        }

        public List<Nomina> getCuentaEmpleadoByNomina(int IdPeriodoNomina, List<int> IdsEmpleados)
        {
            List<Nomina> lista = new List<Nomina>();
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                lista = ctx.Nomina.Where(x => IdsEmpleados.Contains(x.IdEmpleado) && x.IdPeriodoNomina == IdPeriodoNomina && x.CuentaBancariaTrad != null).ToList();
            }
            return lista;
        }


        public Empleado GetEmpleadoToEditHono(int idEmpleado, int IdUnidadNegocio, int IdCliente)
        {

            using (TadaEmpleados entity = new TadaEmpleados())
            {
                var emp = entity.vEmpleados.Where(b => b.IdEmpleado.Equals(idEmpleado) && b.IdUnidadNegocio == IdUnidadNegocio && b.IdEstatus == 1).FirstOrDefault();

                Empleado empleado = new Empleado();

                empleado.IdUnidadNegocio = emp.IdUnidadNegocio;
                empleado.IdCentroCostos = emp.IdCentroCostos;
                empleado.CentrosCostosList = GetCentrosCostos(IdCliente);

                empleado.IdDepartamento = emp.IdDepartamento;
                empleado.DepartamentoList = GetDepartamentos(IdCliente);
                empleado.idArea = emp.IdArea;
                empleado.AreaList = GetAre(IdCliente);
                empleado.Idsindicato = emp.idSindicato;
                empleado.SindicatoList = GetSindicato();
                empleado.IdPuesto = emp.IdPuesto;
                empleado.PuestosList = GetPuestos(IdCliente);

                empleado.IdSucursal = emp.IdSucursal;
                empleado.SucursalList = GetSucursales(IdCliente);

                if (emp.NombrePatrona != null)
                {
                    empleado.Patrona = emp.NombrePatrona + "-" + emp.RFC_Patrona;

                }
                else
                {
                    empleado.Patrona = "No tiene registro";

                }


                empleado.IdRegistroPatronal = emp.IdRegistroPatronal;
                empleado.RegistrosPatronalesList = GetRegistrosPatronales(IdCliente);

                empleado.IdEntidad = emp.IdEntidad;
                empleado.EntidadFederativaList = GetEntidadesFederativas();

                empleado.IdBancoTrad = emp.IdBancoTrad;
                empleado.BancosList = GetBancos();

                empleado.IdEmpleado = emp.IdEmpleado;

                empleado.ClaveEmpleado = emp.ClaveEmpleado;
                empleado.Nombre = emp.Nombre;
                empleado.ApellidoPaterno = emp.ApellidoPaterno;
                empleado.ApellidoMaterno = emp.ApellidoMaterno;

                empleado.Sexo = emp.Sexo;
                empleado.SexoList = GetSexo();

                empleado.EstadoCivil = emp.EstadoCivil;
                empleado.EstadoCivilList = GetEstadoCivil();

                empleado.NumeroTelefonico = emp.Telefono;
                empleado.SD = emp.SD;
                empleado.SDI = emp.SDI;
                empleado.SDIMSS = emp.SDIMSS;
                empleado.CuentaBancariaTrad = emp.CuentaBancariaTrad;
                empleado.CuentaInterbancariaTrad = emp.CuentaInterbancariaTrad;
                empleado.Curp = emp.Curp;
                empleado.Rfc = emp.Rfc;
                empleado.Imss = emp.Imss;
                empleado.CorreoElectronico = emp.CorreoElectronico;
                empleado.PremioP = empleado.PremioP;




                return empleado;
            }
        }


        public List<Empleado> GetEmpleadosByNombreHonorarios(string nombre, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where (b.Nombre + " " + b.ApellidoPaterno + " " + b.ApellidoMaterno).StartsWith(nombre) && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 && (b.TipoContrato == "HONORARIOS" || b.TipoContrato == "RESICO") select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        FechaBaja = x.FechaBaja != null ? x.FechaBaja.Value.ToShortDateString() : null,
                    });
                });
                return empleados;
            }
        }

        public List<Empleado> GetEmpleadosByClaveHonorarios(string clave, int idUnidadNegocio)
        {
            using (TadaEmpleados entity = new TadaEmpleados())
            {
                List<Empleado> empleados = new List<Empleado>();
                var query = (from b in entity.Empleados where b.ClaveEmpleado == clave && b.IdUnidadNegocio == idUnidadNegocio && b.IdEstatus == 1 && (b.TipoContrato == "HONORARIOS" || b.TipoContrato == "RESICO") select b).ToList();

                query.ForEach(x =>
                {
                    empleados.Add(new Empleado
                    {
                        IdEmpleado = x.IdEmpleado,
                        ClaveEmpleado = x.ClaveEmpleado,
                        Nombre = x.Nombre,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        Rfc = x.Rfc,
                        Curp = x.Curp,
                        Imss = x.Imss,
                        SD = x.SD,
                        SDIMSS = x.SDIMSS,
                        SDI = x.SDI,
                        FechaBaja = x.FechaBaja != null ? x.FechaBaja.Value.ToShortDateString() : null,
                    });
                });
                return empleados;
            }
        }


        public List<HistoricoHonorarios> Historial(int idhonorario)
        {
            using (NominaEntities1 entity = new NominaEntities1())
            {
                var query = (from b in entity.HistoricoHonorarios where b.IdHonorarios == idhonorario select b).ToList();
                return query;
            }
        }


    }
}