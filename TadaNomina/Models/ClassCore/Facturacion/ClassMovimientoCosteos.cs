using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.CFDI;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ClassCore.CalculoNomina;

namespace TadaNomina.Models.ClassCore
{
    public class ClassMovimientoCosteos
    {
        /// <summary>
        ///     Método que lista los movimientos de costeos en un periodo de nómina
        /// </summary>
        /// <param name="_IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Lista de modelos con la información de los movimientos de costeos</returns>
        public List<ModelMovimientosCosteos> ListaMovimientosCosteos(int _IdPeriodoNomina)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Costeos_Movimientos
                        where a.IdPeriodoNomina == _IdPeriodoNomina && a.IdEstatus == 1
                        select new ModelMovimientosCosteos
                        {
                            IdCosteosMovimiento = a.IdCosteosMovimiento,
                            IdPeriodoNomina = a.IdPeriodoNomina,
                            Esquema = a.Esquema,
                            TipoMovimiento = a.TipoMovimiento,
                            MovimientoAfecta = a.MovimientoAfecta,
                            IdMovimientoAfecta = a.IdMovimientoAfecta,
                            IdCosteo = a.IdCosteo,
                            IdPatrona = a.IdPatrona,
                            IdDivision = a.IdDivision,
                            IdConcepto = a.IdConcepto,
                            Descripcion = a.Descripcion,
                            Monto = a.Monto,
                            Observaciones = a.Observaciones,
                            IdEstatus = a.IdEstatus
                        }).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene los registros patronales de un cliente
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <returns>Lista de modelos con la información de los registros patronales</returns>
        public List<Cat_RegistroPatronal> GetRegistrosPatronales(int IdCliente)
        {
            List<Cat_RegistroPatronal> _lista = new List<Cat_RegistroPatronal>();

            //using (TadaNominaEntities ctx = new TadaNominaEntities())
            //{
            //var ce = new ClassEmpleado();
            //var RegistroPatronal = ce.GetEmpleadoByUnidadNegocio(IdUnidadNEgocio).Select(x=> x.IdRegistroPatronal).Distinct();

            ClassProcesosNomina cpn = new ClassProcesosNomina();
            _lista = cpn.ObtenerRPByIdCliente(IdCliente);//.Where(x => RegistroPatronal.Contains(x.IdRegistroPatronal)).ToList();

            return _lista;
            //}
        }

        /// <summary>
        ///     Método que obtiene un movimiento de costeo
        /// </summary>
        /// <param name="_idMovimientoCosteos">Variable que contiene el id del movimiento</param>
        /// <returns>Modelo con la información del movimiento</returns>
        public ModelMovimientosCosteos GetMovimientoCosteos(int _idMovimientoCosteos)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Costeos_Movimientos
                        where a.IdCosteosMovimiento == _idMovimientoCosteos && a.IdEstatus == 1
                        select new ModelMovimientosCosteos
                        {
                            IdCosteosMovimiento = a.IdCosteosMovimiento,
                            IdPeriodoNomina = a.IdPeriodoNomina,
                            Esquema = a.Esquema,
                            TipoMovimiento = a.TipoMovimiento,
                            MovimientoAfecta = a.MovimientoAfecta,
                            IdMovimientoAfecta = a.IdMovimientoAfecta,
                            IdCosteo = a.IdCosteo,
                            IdPatrona = a.IdPatrona,
                            IdDivision = a.IdDivision,
                            IdConcepto = a.IdConcepto,
                            Descripcion = a.Descripcion,
                            Monto = a.Monto,
                            Observaciones = a.Observaciones,
                            IdEstatus = a.IdEstatus
                        }).FirstOrDefault();
            }
        }

        /// <summary>
        ///     Método que obtiene una lista de conceptos de costeos
        /// </summary>
        /// <param name="_IdCosteo">Variable que contiene el id del costeo</param>
        /// <returns>Lista de modelos con la información de los conceptos</returns>
        public List<Costeos_Conceptos> GetConceptosCosteos(int _IdCosteo)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Costeos_Conceptos.Where(x => x.IdEstatus == 1 && x.IdCosteo == _IdCosteo) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene una lista de costeos
        /// </summary>
        /// <param name="_idUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns>Lista de modelos con los costeos</returns>
        public List<TadaNomina.Models.DB.Costeos> GetCosteos(int _idUnidadNegocio)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Costeos.Where(x => x.IdEstatus == 1 && x.IdUnidadNegocio == _idUnidadNegocio) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que lista los costeos
        /// </summary>
        /// <param name="_PeriodoNomina">Variable que contiene una lista de periodos de nómina</param>
        /// <returns>Lista con información de costeos</returns>
        public List<SelectListItem> ListaCosteos(List<TadaNomina.Models.DB.Costeos> _PeriodoNomina)
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem
            {
                Value = "",
                Text = "Elegir...",
            });
            foreach (var item in _PeriodoNomina)
            {
                _lista.Add(new SelectListItem
                {
                    Value = item.IdCosteo.ToString(),
                    Text = item.Descripcion,
                });
            }
            return _lista;
        }

        /// <summary>
        ///     Método que obtiene una lista con los tipos de movimientos
        /// </summary>
        /// <returns>Lista de movimientos</returns>
        public List<SelectListItem> ListarTipoMovimiento()
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem { Text = "Elegir...", Value = "" });
            _lista.Add(new SelectListItem { Text = "Cargo", Value = "Cargo" });
            _lista.Add(new SelectListItem { Text = "Abono", Value = "Abono" });
            return _lista;
        }

        /// <summary>
        ///     Método que genera una lista vacía
        /// </summary>
        /// <returns>Lista vacía</returns>
        public List<SelectListItem> ListaVacia()
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem { Text = "Elegir...", Value = "0" });
            return _lista;
        }

        /// <summary>
        ///     Método que obtiene una lista de centros de costos de un cliente
        /// </summary>
        /// <param name="_idCliente">Variable que contiene el id del cliente</param>
        /// <returns>Lista de modelos con la información de los centros de costos</returns>
        public List<Cat_CentroCostos> GetCentroCostos(int _idCliente)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.Cat_CentroCostos.Where(x => x.IdCliente == _idCliente && x.IdEstatus == 1) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que convierte una lista de centros de costos en una lista de objetos
        /// </summary>
        /// <param name="_PeriodoNomina">Variable que contiene una lista de modelos con información de centros de costos</param>
        /// <returns>Lista con información de los centros de costos</returns>
        public List<SelectListItem> ListaCentroCostos(List<TadaNomina.Models.DB.Cat_CentroCostos> _PeriodoNomina)
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem
            {
                Value = "",
                Text = "Elegir...",
            });
            foreach (var item in _PeriodoNomina)
            {
                _lista.Add(new SelectListItem
                {
                    Value = item.IdCentroCostos.ToString(),
                    Text = item.CentroCostos,
                });
            }
            return _lista;
        }

        /// <summary>
        ///     Método que obtiene una lista de registros patronales
        /// </summary>
        /// <param name="_IdCliente">Variable que contiene el id del cliente</param>
        /// <returns>Lista de modelos con la información de los registros patronales</returns>
        public List<Cat_RegistroPatronal> GetRegistroPatronal(int _IdCliente)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return (from a in ctx.Cat_RegistroPatronal.Where(x => x.IdCliente == _IdCliente && x.IdEstatus == 1) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que convierte una lista de modelos de registros patronales en una lista de objetos
        /// </summary>
        /// <param name="_PeriodoNomina">Variable que contiene una lista de modelos de registros patronales</param>
        /// <returns>lista de registros patronales</returns>
        public List<SelectListItem> ListaRegistroPatronal(List<Cat_RegistroPatronal> _PeriodoNomina)
        {
            List<SelectListItem> _lista = new List<SelectListItem>();

            _lista.Add(new SelectListItem
            {
                Value = "",
                Text = "Elegir...",
            });
            foreach (var item in _PeriodoNomina)
            {
                _lista.Add(new SelectListItem
                {
                    Value = item.IdRegistroPatronal.ToString(),
                    Text = item.NombrePatrona,
                });
            }
            return _lista;
        }

        /// <summary>
        ///     Método que obtiene el id del cliente
        /// </summary>
        /// <param name="_idunidadnegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns>Id del cliente</returns>
        public int GetIdCliente(int _idunidadnegocio)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return (from a in ctx.Cat_UnidadNegocio.Where(x => x.IdUnidadNegocio == _idunidadnegocio) select a.IdCliente).FirstOrDefault();
            }
                
        }

        /// <summary>
        ///     Método que obtiene los costeos de una división
        /// </summary>
        /// <param name="_idDivision">Variable que contiene el id del costeo</param>
        /// <returns>Modelo con la información del costeo</returns>
        public TadaNomina.Models.DB.Costeos GetCosteoSeparador(int _idDivision)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Costeos.Where(x => x.IdCosteo == _idDivision) select a).First();
            }
        }

        /// <summary>
        ///     Método que obtiene una lista de centros de costos en una unidad de negocio
        /// </summary>
        /// <param name="_idUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns>Lista de modelos con la información de los centros de costos</returns>
        public List<Cat_CentroCostos> GetCentroCostosUN(int _idUnidadNegocio)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.Cat_CentroCostos.Where(x => x.IdUnidadNegocio == _idUnidadNegocio && x.IdEstatus == 1) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene los departamentos de una unidad de negocio
        /// </summary>
        /// <param name="_idUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns>Lista de modelos con la informción de los departamentos</returns>
        public List<Cat_Departamentos> GetDeptoIdUN(int _idUnidadNegocio)
        {
            using (NominaEntities1 ctx = new NominaEntities1())
            {
                return (from a in ctx.Cat_Departamentos.Where(x => x.IdUnidadNegocio == _idUnidadNegocio && x.IdEstatus == 1) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene los puestos de una unidad de negocio
        /// </summary>
        /// <param name="_idUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <returns>Lista de modelos con la información de los puestos</returns>
        public List<Cat_Puestos> GetPuestoIdUn(int _idUnidadNegocio)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                return (from a in ctx.Cat_Puestos.Where(x => x.IdUnidadNegocio == _idUnidadNegocio && x.IdEstatus == 1) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene una lista de conceptos de costeos
        /// </summary>
        /// <param name="IdCosteo">Variable que contiene el id del costeo</param>
        /// <returns>Lista de modelos con información de los conceptos</returns>
        public List<Costeos_Conceptos> ListarCosteoConceptos(int IdCosteo)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                return (from a in ctx.Costeos_Conceptos.Where(x => x.IdCosteo == IdCosteo && x.IdEstatus == 1) select a).ToList();
            }
        }

        /// <summary>
        ///     Método que crea un nuevo movimiento de costeo
        /// </summary>
        /// <param name="_MMC">Variable que contiene la información de un nuevo movimiento</param>
        public void Crear(ModelMovimientosCosteos _MMC)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                Costeos_Movimientos CM = new Costeos_Movimientos();


                CM.IdPeriodoNomina = _MMC.IdPeriodoNomina;
                CM.Esquema = _MMC.Esquema;
                CM.TipoMovimiento = _MMC.TipoMovimiento;
                CM.MovimientoAfecta = _MMC.MovimientoAfecta;
                CM.IdMovimientoAfecta = _MMC.IdMovimientoAfecta;
                CM.IdCosteo = _MMC.IdCosteo;
                CM.IdPatrona = _MMC.IdPatrona;
                CM.IdDivision = (int)_MMC.IdDivision;
                CM.IdConcepto = _MMC.IdConcepto;
                CM.Descripcion = _MMC.Descripcion;
                CM.Monto = _MMC.Monto;
                CM.Observaciones = _MMC.Observaciones;
                CM.IdEstatus = 1;
                CM.FechaCaptura = DateTime.Now;
                CM.IdCaptura = _MMC.IdCaptura;

                ctx.Costeos_Movimientos.Add(CM);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que edita un movimiento
        /// </summary>
        /// <param name="_MMC">Variable que contiene la información de un movimiento de costeo</param>
        public void Editar(ModelMovimientosCosteos _MMC)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var CM = (from a in ctx.Costeos_Movimientos.Where(x => x.IdCosteosMovimiento == _MMC.IdCosteosMovimiento) select a).First();

                CM.IdPeriodoNomina = _MMC.IdPeriodoNomina;
                CM.Esquema = _MMC.Esquema;
                CM.TipoMovimiento = _MMC.TipoMovimiento;
                CM.MovimientoAfecta = _MMC.MovimientoAfecta;
                CM.IdMovimientoAfecta = _MMC.IdMovimientoAfecta;
                CM.IdCosteo = _MMC.IdCosteo;
                CM.IdPatrona = _MMC.IdPatrona;
                CM.IdDivision = _MMC.IdDivision;
                CM.IdConcepto = _MMC.IdConcepto;
                CM.Descripcion = _MMC.Descripcion;
                CM.Monto = _MMC.Monto;
                CM.Observaciones = _MMC.Observaciones;
                CM.FechaModifica = DateTime.Now;
                CM.IdModifica = _MMC.IdCaptura;

                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que modifica el estado de un movimiento a eliminado
        /// </summary>
        /// <param name="_IdCosteosMovimiento">Variable que contiene el id del movimiento</param>
        /// <param name="_IdUsuario">Variable que contiene el id del usuario con la sesión abierta</param>
        public void Eliminar(int _IdCosteosMovimiento, int _IdUsuario)
        {
            using (TadaContabilidadEntities ctx = new TadaContabilidadEntities())
            {
                var query = (from a in ctx.Costeos_Movimientos.Where(x => x.IdCosteosMovimiento == _IdCosteosMovimiento) select a).First();
                query.IdModifica = _IdUsuario;
                query.IdEstatus = 2;
                query.FechaModifica = DateTime.Now;

                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que convierte una lista de modelos en una lista de objetos
        /// </summary>
        /// <param name="_MDPCC">Variable que contiene una lista de modelos</param>
        /// <returns>Lista de objetos</returns>
        public List<SelectListItem> ListarCC(List<ModelDeptoPuestoCC> _MDPCC )
        {
            List<SelectListItem> _Lista = new List<SelectListItem>();

            _Lista.Add(new SelectListItem { Value="0", Text="Elegir..."});

            foreach (var item in _MDPCC)
            {
                _Lista.Add(new SelectListItem
                {
                    Text = item.CentroCostos,
                    Value = item.IdCentroCostos.ToString(),
                });
            }
            return _Lista;
        }

        /// <summary>
        ///     Método que convierte una lista de modelos en una lista de objetos
        /// </summary>
        /// <param name="_MDPCC">Variable que contiene una lista de modelos</param>
        /// <returns>Lista de objetos</returns>
        public List<SelectListItem> ListarDD(List<ModelDeptoPuestoCC> _MDPCC )
        {
            List<SelectListItem> _Lista = new List<SelectListItem>();

            _Lista.Add(new SelectListItem { Value="0", Text="Elegir..."});

            foreach (var item in _MDPCC)
            {
                _Lista.Add(new SelectListItem
                {
                    Text = item.Departamento,
                    Value = item.IdDepartamento.ToString(),
                });
            }
            return _Lista;
        }

        /// <summary>
        ///     Método que convierte una lista de modelos en una lista de objetos
        /// </summary>
        /// <param name="_MDPCC">Variable que contiene una lista de modelos</param>
        /// <returns>Lista de objetos</returns>
        public List<SelectListItem> ListarPP(List<ModelDeptoPuestoCC> _MDPCC)
        {
            List<SelectListItem> _Lista = new List<SelectListItem>();

            _Lista.Add(new SelectListItem { Value = "0", Text = "Elegir..." });

            foreach (var item in _MDPCC)
            {
                _Lista.Add(new SelectListItem
                {
                    Text = item.Puesto,
                    Value = item.IdPuesto.ToString(),
                });
            }
            return _Lista;
        }

        /// <summary>
        ///     Método que convierte una lista de modelos en una lista de objetos
        /// </summary>
        /// <param name="_MDPCC">Variable que contiene una lista de modelos</param>
        /// <returns>Lista de objetos</returns>
        public List<SelectListItem> ListarCosteosConceptos(List<Costeos_Conceptos> _MDPCC)
        {
            List<SelectListItem> _Lista = new List<SelectListItem>();

            _Lista.Add(new SelectListItem { Value = "0", Text = "Elegir..." });

            foreach (var item in _MDPCC)
            {
                _Lista.Add(new SelectListItem
                {
                    Text = item.Descripcion,
                    Value = item.IdCosteosConcepto.ToString(),
                });
            }
            return _Lista;
        }

    }
}