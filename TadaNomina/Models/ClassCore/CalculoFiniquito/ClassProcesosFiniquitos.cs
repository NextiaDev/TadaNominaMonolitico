using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Nominas;

namespace TadaNomina.Models.ClassCore.CalculoFiniquito
{
    public class ClassProcesosFiniquitos : ClassCalculoImpuestos
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <returns></returns>
        public string GetClavesFiniquitosConfigurados(int IdPeriodoNomina)
        {
            string claves = null;
            List<vConfiguracionFiniquito> fin = GetFiniquitosConfigurados(IdPeriodoNomina);

            int cont = 1;
            int registros = fin.Count();
            foreach (var item in fin)
            {
                claves += item.ClaveEmpleado;
                if (cont != registros)
                    claves += ",";
                cont++;
            }

            return claves;
        }

        /// <summary>
        ///     Obtiene los Id's de los finiquitos configurados en un periodo
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el id del periodo de nómina</param>
        /// <returns>Lista de id's de los finiquitos activos dentro de un periodo</returns>
        public string GetIdsFiniquitosConfigurados(int IdPeriodoNomina)
        {
            string claves = null;
            List<vConfiguracionFiniquito> fin = GetFiniquitosConfigurados(IdPeriodoNomina);

            int cont = 1;
            int registros = fin.Count();
            foreach (var item in fin)
            {
                claves += item.IdEmpleado;
                if (cont != registros)
                    claves += ",";
                cont++;
            }

            return claves;
        }

        /// <summary>
        ///     Obtiene los id's de los empleados a los que se les ha configurado un finiquito dentro de un periodo
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <returns>Regresa una lista con los empleados que tienen finiquitos configurados en un periodo</returns>
        public int[] GetIdsEmpleadosFiniquitosConfigurados(int IdPeriodoNomina)
        {
            List<vConfiguracionFiniquito> fin = GetFiniquitosConfigurados(IdPeriodoNomina);
            int[] Ids = fin.Select(x => x.IdEmpleado).ToArray();

            return Ids;
        }

        /// <summary>
        ///     Método que obtiene los finiquits configurados en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <returns>Lista de finiquitos activos</returns>
        public List<vConfiguracionFiniquito> GetFiniquitosConfigurados(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var finiquitos = from b in entidad.vConfiguracionFiniquito.Where(x => x.IdEstatus == 1 && x.IdPeriodoNomina == IdPeriodoNomina) select b;

                return finiquitos.ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene los finiquitos configurados
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdEmpleado">Variable que contiene el Id del empleado</param>
        /// <returns>Datos de la vista ConfiguracionFiniquito</returns>
        public vConfiguracionFiniquito GetFiniquitoConfigurado(int IdPeriodoNomina, int IdEmpleado)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var finiquitos = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdEstatus == 1 && x.IdPeriodoNomina == IdPeriodoNomina && x.IdEmpleado == IdEmpleado) select b).FirstOrDefault();

                return finiquitos;
            }
        }

        /// <summary>
        ///     Método que guarda o modifica la configuración de los finiquitos
        /// </summary>
        /// <param name="model">Variable que contiene los datos del finiquito a configurar</param>
        /// <param name="IdUsuario">Variable que contiene el id del empleado que está firmado en la sesión</param>
        public void GuardaConfiguracionFiniquitos(ModelProcesaNominaGeneral model, int IdUsuario)
        {
            foreach (var item in model.empleados)
            {
                if (item.IdEstatus != 5)
                {
                    if (ValidaConfiguracionFiniquitos(item.IdEmpleado, item.IdPeriodoNomina))
                    {
                        UpdateConfiguracionFiniquitos(item, item.IdPeriodoNomina, IdUsuario);
                    }
                    else
                    {
                        AddConfiguracionFiniquitos(item, item.IdPeriodoNomina, IdUsuario);
                    }
                }
            }
        }

        /// <summary>
        ///     Método que guarda o modifica la configuración de los finiquitos
        /// </summary>
        /// <param name="model">Variable que contiene los datos del finiquito a configurar</param>
        /// <param name="IdUsuario">Variable que contiene el id del empleado que está firmado en la sesión</param>
        public void GuardaConfiguracionFiniquitos(List<vEmpleados> model, int IdPeriodoNomina, int IdUsuario)
        {
            foreach (var item in model)
            {
                if (item.IdEstatus != 5)
                {
                    if (ValidaConfiguracionFiniquitos(item.IdEmpleado, IdPeriodoNomina))
                    {
                        UpdateConfiguracionFiniquitos(item, IdPeriodoNomina, IdUsuario);
                    }
                    else
                    {
                        AddConfiguracionFiniquitos(item, IdPeriodoNomina, IdUsuario);
                    }
                }
            }
        }

        public void BorrarAllFiniquitos(int IdPeriodo)
        {
            DeleteAllFiniquitoNomina(IdPeriodo);
            DeleteAllConfiguracionFiniquitos(IdPeriodo);
        }

        /// <summary>
        ///     Método que borra los finiquitos configurados (cambian a estatus 2 = inactivo)
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodo">Variable que contiene  el periodo de nómina</param>
        public void BorrarFiniquito(int IdEmpleado, int IdPeriodo)
        {
            DeleteFiniquitoNomina(IdEmpleado, IdPeriodo);
            DeleteConfiguracionFiniquitos(IdEmpleado, IdPeriodo);
        }

        /// <summary>
        ///     Método que borra el registro del finiquito
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void DeleteFiniquitoNomina(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo) select b).FirstOrDefault();

                if (registro != null)
                {
                    entidad.NominaTrabajo.Remove(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que borra todos los finiquitos configurados en un periodo
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void DeleteAllFiniquitoNomina(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo) select b).ToList();

                if (registro != null)
                {
                    entidad.NominaTrabajo.RemoveRange(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método para cerrar el finiquito configurado en un periodo
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void CerrarFiniquito(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo) select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.IdEstatus = 1;                    
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que cierra todos los finiquitos configurados en un periodo
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void CerrarAllFiniquito( int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo) select b).ToList();

                foreach (var item in registro)
                {
                    item.IdEstatus = 1;                    
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que bloquea los finiquitos configurados en un periodo
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void BloquearFiniquito(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo) select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.IdEstatus = 2;
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que abre un finiquito dentro de un periodo
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empelado</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void AbrirFiniquito(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo) select b).FirstOrDefault();

                if (registro != null)
                {
                    registro.IdEstatus = 4;
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que borra el registro de un finiquito dentro de un periodo
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void DeleteConfiguracionFiniquitos(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo) select b).FirstOrDefault();

                if (registro != null)
                {
                    entidad.ConfiguracionFiniquito.Remove(registro);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método para borrar los registros de la configuración de finiquitos
        /// </summary>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        public void DeleteAllConfiguracionFiniquitos(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdPeriodoNomina == IdPeriodo) select b).ToList();
                
                entidad.ConfiguracionFiniquito.RemoveRange(registro);
                entidad.SaveChanges();
                
            }
        }

        /// <summary>
        ///     Método que valida que exista una configuración de finiquito creada para un usuario
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        /// <returns>Verdadero o falso en caso de que exista un finiquito configurado</returns>
        public bool ValidaConfiguracionFiniquitos(int IdEmpleado, int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodo) select b).FirstOrDefault();

                if (registro != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Método que agrega una configuración de finiquito nueva
        /// </summary>
        /// <param name="model">Variable que contiene los datos para configurar los finiquitos</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void AddConfiguracionFiniquitos(ModelEmpleadoFiniquito model, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ConfiguracionFiniquito cf = new ConfiguracionFiniquito();
                cf.IdPeriodoNomina = IdPeriodoNomina;
                cf.IdEmpleado = model.IdEmpleado;
                try { cf.FechaBaja = DateTime.Parse(model.FechaBaja); } catch { cf.FechaBaja = DateTime.Now; }
                if (model.Liquidacion) { cf.BanderaLiquidacion = 1; } else { cf.BanderaLiquidacion = 0; }
                cf.IdEstatus = 1;
                cf.IdCaptura = IdUsuario;
                cf.FechaCaptura = DateTime.Now;

                entidad.ConfiguracionFiniquito.Add(cf);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que agrega una configuración de finiquito nueva
        /// </summary>
        /// <param name="model">Variable que contiene los datos para configurar los finiquitos</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void AddConfiguracionFiniquitos(vEmpleados model, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ConfiguracionFiniquito cf = new ConfiguracionFiniquito();
                cf.IdPeriodoNomina = IdPeriodoNomina;
                cf.IdEmpleado = model.IdEmpleado;
                try { cf.FechaBaja = model.FechaBaja ?? DateTime.Now; } catch { cf.FechaBaja = DateTime.Now; }
                cf.BanderaLiquidacion = 0; 
                cf.IdEstatus = 1;
                cf.IdCaptura = IdUsuario;
                cf.FechaCaptura = DateTime.Now;

                entidad.ConfiguracionFiniquito.Add(cf);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que modifica la configuración del finiquito
        /// </summary>
        /// <param name="model">Variable que contiene los datos para configurar el finiquito</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateConfiguracionFiniquitos(ModelEmpleadoFiniquito model, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cf = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdEmpleado == model.IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (cf != null)
                {
                    try { cf.FechaBaja = DateTime.Parse(model.FechaBaja); } catch { cf.FechaBaja = DateTime.Now; }
                    if (model.Liquidacion) { cf.BanderaLiquidacion = 1; } else { cf.BanderaLiquidacion = 0; }
                    cf.IdEstatus = 1;
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;
                                     
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que modifica la configuración del finiquito
        /// </summary>
        /// <param name="model">Variable que contiene los datos para configurar el finiquito</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateConfiguracionFiniquitos(vEmpleados model, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cf = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdEmpleado == model.IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (cf != null)
                {
                    //try { cf.FechaBaja = model.FechaBaja ?? DateTime.Now; } catch { cf.FechaBaja = DateTime.Now; }
                    cf.BanderaLiquidacion = 0; 
                    cf.IdEstatus = 1;
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que modifica la fecha de baja de un empleado
        /// </summary>
        /// <param name="fechaBaja">Variable que contiene la fecha de baja del empleado</param>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateFechaBaja(string fechaBaja, int IdEmpleado, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cf = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (cf != null)
                {
                    try { cf.FechaBaja = DateTime.Parse(fechaBaja); } catch { cf.FechaBaja = DateTime.Now; }  
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que modifica la fecha de baja de todos los empleados que estan en un periodo de finiquitos.
        /// </summary>
        /// <param name="fechaBaja">Variable que contiene la fecha de baja del empleado</param>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateFechaBajaGral(string fechaBaja, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var _cf = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdPeriodoNomina == IdPeriodoNomina) select b).ToList();

                foreach (var cf in _cf)
                {
                    try { cf.FechaBaja = DateTime.Parse(fechaBaja); } catch { cf.FechaBaja = DateTime.Now; }
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;                    
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        ///     Método que modifica la liquidación del empleado que será baja
        /// </summary>
        /// <param name="Liquidacion">Variable que contiene a confiración de la liquidación (V-F)</param>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateLiquidacion(bool Liquidacion, int IdEmpleado, int IdPeriodoNomina, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cf = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdEmpleado == IdEmpleado && x.IdPeriodoNomina == IdPeriodoNomina) select b).FirstOrDefault();

                if (cf != null)
                {
                    if (Liquidacion)
                        cf.BanderaLiquidacion = 1;
                    else
                        cf.BanderaLiquidacion = null;

                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que modifica el sueldo real del empleado
        /// </summary>
        /// <param name="IdEmpleado">Variable que contiene el id del empleado</param>
        /// <param name="nuevoSueldo">Variable que contiene el nuevo sueldo del empleado</param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateSueldoReal(int IdEmpleado, decimal nuevoSueldo, int IdUsuario)
        {
            using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
            {
                var cf = (from b in entidad.Empleados.Where(x => x.IdEmpleado == IdEmpleado) select b).FirstOrDefault();

                if (cf != null)
                {
                    cf.SD = nuevoSueldo;
                    cf.IdModificacion = IdUsuario;
                    //cf.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que modifica una configuracion de un finiquito más detallada
        /// </summary>
        /// <param name="IdConfiguracionFiniquito">Variable que contiene el id del finiquito a configurar</param>
        /// <param name="banderaVac">Variable que contiene una vandera de vacaciones</param>
        /// <param name="banderaPV">Variable que contiene </param>
        /// <param name="banderaAgui">Variable que contiene </param>
        /// <param name="bandera90d">Variable que contiene </param>
        /// <param name="bandera20d">Variable que contiene </param>
        /// <param name="banderaPA">Variable que contiene </param>
        /// <param name="IdUsuario">Variable que contiene el id del usuario que está firmado en la sesión</param>
        public void UpdateConfiguracionAvanzada(int IdConfiguracionFiniquito, bool banderaVac, bool banderaPV, bool banderaAgui, bool bandera90d, bool bandera20d, bool banderaPA, bool LiquidacionSDI, int IdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cf = (from b in entidad.ConfiguracionFiniquito.Where(x => x.IdConfiguracionFiniquito == IdConfiguracionFiniquito) select b).FirstOrDefault();

                if (cf != null)
                {
                    if (banderaVac) { cf.BanderaVac = 1; } else { cf.BanderaVac = null; }
                    if (banderaPV) { cf.BanderaPV = 1; } else { cf.BanderaPV = null; }
                    if (banderaAgui) { cf.BanderaAguinaldo = 1; } else { cf.BanderaAguinaldo = null; }
                    if (bandera90d) { cf.Bandera90d = 1; } else { cf.Bandera90d = null; }
                    if (bandera20d) { cf.Bandera20d = 1; } else { cf.Bandera20d = null; }
                    if (banderaPA) { cf.BanderaPA = 1; } else { cf.BanderaPA = null; }
                    if (LiquidacionSDI) { cf.LiquidacionSDI = 1; } else { cf.LiquidacionSDI = null; }
                    cf.IdModifica = IdUsuario;
                    cf.FechaModifica = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Método que busca si un empleado tiene algún finiquito configurado
        /// </summary>
        /// <param name="claves">Variable que contiene las claves de los empleados</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        /// <param name="modificados">Variable que contiene una lista con los datos para la configuración de un finiquito</param>
        /// <returns></returns>
        public List<ModelEmpleadoFiniquito> SearchEmpleadosFiniquitos(string claves, int IdUnidadNegocio, int IdPeriodo, List<ModelEmpleadoFiniquito> modificados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                List<ModelEmpleadoFiniquito> _emp = new List<ModelEmpleadoFiniquito>();

                if (claves == null)
                {
                    var emp = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdConfiguracionFiniquito != 0 && x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).ToList();

                    emp.ForEach(x => {
                        _emp.Add(new ModelEmpleadoFiniquito
                        {
                            IdEmpleado = x.IdEmpleado,
                            IdPeriodoNomina = (int)x.IdPeriodoNomina,
                            ClaveEmpleado = x.ClaveEmpleado,
                            Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                            RFC = x.Rfc,
                            _Liquidacion = x.BanderaLiquidacion,
                            FechaBaja = x.FechaBaja.ToString(),
                            ER = string.Format("{0:C2}", x.ER),
                            DD = string.Format("{0:C2}", x.DD),
                            Neto = string.Format("{0:C2}", x.Neto)
                        });
                    });
                }
                else
                {
                    string[] _claves = claves.Split(',').ToArray();
                    int?[] _IdPeriodos = { IdPeriodo, null };
                    var emp = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && _claves.Contains(x.ClaveEmpleado) && _IdPeriodos.Contains(x.IdPeriodoNomina) ) select b).ToList();

                    foreach (var x in emp)
                    {
                        ModelEmpleadoFiniquito emp_ = new ModelEmpleadoFiniquito();

                        emp_.IdEmpleado = x.IdEmpleado;
                        emp_.IdPeriodoNomina = IdPeriodo;
                        emp_.ClaveEmpleado = x.ClaveEmpleado;
                        emp_.Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre;
                        emp_.RFC = x.Rfc;
                        emp_._Liquidacion = x.BanderaLiquidacion;
                        if (emp_._Liquidacion == 1)
                            emp_.Liquidacion = true;
                        else
                            emp_.Liquidacion = false;
                        try { emp_.FechaBaja = x.FechaBajaFin.Value.ToShortDateString(); } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        emp_.ER = string.Format("{0:C2}", x.ER);
                        emp_.DD = string.Format("{0:C2}", x.DD);
                        emp_.Neto = string.Format("{0:C2}", x.Neto);

                        ModelEmpleadoFiniquito registroConfigurado = null;
                        if (modificados != null)
                        {
                            registroConfigurado = (from b in modificados.Where(y => y.IdEmpleado == x.IdEmpleado && y.IdPeriodoNomina == x.IdPeriodoNomina && x.IdEstatus == 1) select b).FirstOrDefault();
                        }

                        if (registroConfigurado != null)
                        {
                            emp_.Liquidacion = registroConfigurado.Liquidacion;
                            if (emp_.Liquidacion)
                            {
                                emp_._Liquidacion = 1;
                            }
                            else
                            {
                                emp_._Liquidacion = 0;
                            }
                            try { emp_.FechaBaja = registroConfigurado.FechaBaja; } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        }

                        _emp.Add(emp_);
                    }
                }

                return _emp;
            }
        }

        /// <summary>
        ///     Método que busca a los empleados que tienen configurado un finiquito
        /// </summary>
        /// <param name="Ids">Variable que contiene una lista de id's de empleados</param>
        /// <param name="IdUnidadNegocio">Variable que contiene el id de la unidad de negocio</param>
        /// <param name="IdPeriodo">Variable que contiene el periodo de nómina</param>
        /// <param name="modificados">Variable que contiene una lista con los datos de un finiquito</param>
        /// <returns>Lista con la información de los finiquitos configurados</returns>
        public List<ModelEmpleadoFiniquito> SearchEmpleadosFiniquitosIds(string Ids, int IdUnidadNegocio, int IdPeriodo, List<ModelEmpleadoFiniquito> modificados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                List<ModelEmpleadoFiniquito> _emp = new List<ModelEmpleadoFiniquito>();

                if (Ids == null || Ids == string.Empty)
                {
                    var emp = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdConfiguracionFiniquito != 0 && x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).ToList();

                    emp.ForEach(x =>
                    {
                        _emp.Add(new ModelEmpleadoFiniquito
                        {
                            IdEmpleado = x.IdEmpleado,
                            IdPeriodoNomina = (int)x.IdPeriodoNomina,
                            Periodo = x.Periodo,
                            ClaveEmpleado = x.ClaveEmpleado,
                            Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                            RFC = x.Rfc,
                            _Liquidacion = x.BanderaLiquidacion,
                            FechaBaja = x.FechaBaja.ToString(),
                            ER = string.Format("{0:C2}", x.ER),
                            DD = string.Format("{0:C2}", x.DD),
                            Neto = string.Format("{0:C2}", x.Neto),
                            IdEstatus = x.IdEstatusNomina
                        });
                    });
                }                                                               
                else
                {
                    string[] _Ids = Ids.Split(',').ToArray();
                    int[] IdsInt = Array.ConvertAll(_Ids, int.Parse);
                    int?[] _IdPeriodos = { IdPeriodo, null };
                    var emp = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && IdsInt.Contains(x.IdEmpleado) && _IdPeriodos.Contains(x.IdPeriodoNomina)) select b).ToList();
                    var empAnt = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && IdsInt.Contains(x.IdEmpleado)  && !_IdPeriodos.Contains(x.IdPeriodoNomina)) select b).ToList();

                    foreach (var x in emp)
                    {
                        ModelEmpleadoFiniquito emp_ = new ModelEmpleadoFiniquito();

                        emp_.IdEmpleado = x.IdEmpleado;
                        emp_.IdPeriodoNomina = IdPeriodo;
                        emp_.Periodo = x.Periodo;
                        emp_.ClaveEmpleado = x.ClaveEmpleado;
                        emp_.Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre;
                        emp_.RFC = x.Rfc;
                        emp_._Liquidacion = x.BanderaLiquidacion;
                        if (emp_._Liquidacion == 1)
                            emp_.Liquidacion = true;
                        else
                            emp_.Liquidacion = false;
                        try { emp_.FechaBaja = x.FechaBajaFin.Value.ToShortDateString(); } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        emp_.ER = string.Format("{0:C2}", x.ER);
                        emp_.DD = string.Format("{0:C2}", x.DD);
                        emp_.Neto = string.Format("{0:C2}", x.Neto);
                        emp_.IdEstatus = x.IdEstatusNomina;

                        ModelEmpleadoFiniquito registroConfigurado = null;
                        if (modificados != null)
                        {
                            registroConfigurado = (from b in modificados.Where(y => y.IdEmpleado == x.IdEmpleado && y.IdPeriodoNomina == x.IdPeriodoNomina && x.IdEstatus == 1) select b).FirstOrDefault();
                        }

                        if (registroConfigurado != null)
                        {
                            emp_.Liquidacion = registroConfigurado.Liquidacion;
                            if (emp_.Liquidacion)
                            {
                                emp_._Liquidacion = 1;
                            }
                            else
                            {
                                emp_._Liquidacion = 0;
                            }
                            try { emp_.FechaBaja = registroConfigurado.FechaBaja; } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        }

                        _emp.Add(emp_);
                    }

                    foreach (var x in empAnt)
                    {
                        ModelEmpleadoFiniquito emp_ = new ModelEmpleadoFiniquito();

                        emp_.IdEmpleado = x.IdEmpleado;
                        emp_.IdPeriodoNomina = IdPeriodo;
                        emp_.Periodo = x.Periodo;
                        emp_.ClaveEmpleado = x.ClaveEmpleado;
                        emp_.Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre;
                        emp_.RFC = x.Rfc;
                        emp_._Liquidacion = x.BanderaLiquidacion;
                        if (emp_._Liquidacion == 1)
                            emp_.Liquidacion = true;
                        else
                            emp_.Liquidacion = false;
                        try { emp_.FechaBaja = x.FechaBajaFin.Value.ToShortDateString(); } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        emp_.ER = string.Format("{0:C2}", x.ER);
                        emp_.DD = string.Format("{0:C2}", x.DD);
                        emp_.Neto = string.Format("{0:C2}", x.Neto);
                        emp_.IdEstatus = 5;                        

                        _emp.Add(emp_);
                    }
                }

                return _emp.OrderBy(x => x.Nombre).ToList();
            }
        }

        public List<ModelEmpleadoFiniquito> SearchEmpleadosFiniquitosIds2(string Ids, int IdUnidadNegocio, int IdPeriodo, List<ModelEmpleadoFiniquito> modificados)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                List<ModelEmpleadoFiniquito> _emp = new List<ModelEmpleadoFiniquito>();

                if (Ids == null || Ids == string.Empty)
                {
                    var emp = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdConfiguracionFiniquito != 0 && x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).ToList();

                    emp.ForEach(x =>
                    {
                        _emp.Add(new ModelEmpleadoFiniquito
                        {
                            IdEmpleado = x.IdEmpleado,
                            IdPeriodoNomina = (int)x.IdPeriodoNomina,
                            Periodo = x.Periodo,
                            ClaveEmpleado = x.ClaveEmpleado,
                            Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre,
                            RFC = x.Rfc,
                            _Liquidacion = x.BanderaLiquidacion,
                            FechaBaja = x.FechaBaja.ToString(),
                            ER = string.Format("{0:C2}", x.ER),
                            DD = string.Format("{0:C2}", x.DD),
                            Neto = string.Format("{0:C2}", x.Neto),
                            IdEstatus = x.IdEstatusNomina
                        });
                    });
                }
                else
                {
                    string[] _Ids = Ids.Split(',').ToArray();
                    int[] IdsInt = Array.ConvertAll(_Ids, int.Parse);
                    int?[] _IdPeriodos = { IdPeriodo, null };
                    var emp = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && IdsInt.Contains(x.IdEmpleado) && _IdPeriodos.Contains(x.IdPeriodoNomina)) select b).ToList();
                    
                    foreach (var x in emp)
                    {
                        ModelEmpleadoFiniquito emp_ = new ModelEmpleadoFiniquito();

                        emp_.IdEmpleado = x.IdEmpleado;
                        emp_.IdPeriodoNomina = IdPeriodo;
                        emp_.Periodo = x.Periodo;
                        emp_.ClaveEmpleado = x.ClaveEmpleado;
                        emp_.Nombre = x.ApellidoPaterno + " " + x.ApellidoMaterno + " " + x.Nombre;
                        emp_.RFC = x.Rfc;
                        emp_._Liquidacion = x.BanderaLiquidacion;
                        if (emp_._Liquidacion == 1)
                            emp_.Liquidacion = true;
                        else
                            emp_.Liquidacion = false;
                        try { emp_.FechaBaja = x.FechaBajaFin.Value.ToShortDateString(); } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        emp_.ER = string.Format("{0:C2}", x.ER);
                        emp_.DD = string.Format("{0:C2}", x.DD);
                        emp_.Neto = string.Format("{0:C2}", x.Neto);
                        emp_.IdEstatus = x.IdEstatusNomina;

                        ModelEmpleadoFiniquito registroConfigurado = null;
                        if (modificados != null)
                        {
                            registroConfigurado = (from b in modificados.Where(y => y.IdEmpleado == x.IdEmpleado && y.IdPeriodoNomina == x.IdPeriodoNomina && x.IdEstatus == 1) select b).FirstOrDefault();
                        }

                        if (registroConfigurado != null)
                        {
                            emp_.Liquidacion = registroConfigurado.Liquidacion;
                            if (emp_.Liquidacion)
                            {
                                emp_._Liquidacion = 1;
                            }
                            else
                            {
                                emp_._Liquidacion = 0;
                            }
                            try { emp_.FechaBaja = registroConfigurado.FechaBaja; } catch { emp_.FechaBaja = DateTime.Now.ToShortDateString(); }
                        }

                        _emp.Add(emp_);
                    }                    
                }

                return _emp.OrderBy(x => x.Nombre).ToList();
            }
        }

        /// <summary>
        ///     Método que obtiene la configuración de los finiquitos en un periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Variable que contiene el periodo de nómina</param>
        public void GetConfiguracionFiniquitos(int IdPeriodoNomina)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registros = (from b in entidad.vConfiguracionFiniquito.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1 && x.IdEstatusNomina != 1) select b).ToList();                
                ListConfiguracionFiniquito = registros;
            }
        }

        public void ObtenConfiguracionFiniquitoGral()
        {
            foreach (var item in ListConfiguracionFiniquito)
            {
                item.BanderaVac = Periodo.BanderaNoVacaciones == "S" ? 1 : item.BanderaVac;
                item.BanderaPV = Periodo.BanderaNoPV == "S" ? 1 : item.BanderaPV;
                item.BanderaAguinaldo = Periodo.BanderaNoAguinaldo == "S" ? 1 : item.BanderaAguinaldo;
                item.Bandera90d = Periodo.BanderaNo90Dias == "S" ? 1 : item.Bandera90d;
                item.Bandera20d = Periodo.BanderaNo20Dias == "S" ? 1 : item.Bandera20d;
                item.BanderaPA = Periodo.BanderaNoPA == "S" ? 1 : item.BanderaPA;
            }
        }

        /// <summary>
        ///     Método que obtiene los conceptos para configurar un finiquito
        /// </summary>
        public void GetConceptosFiniquitos()
        {
            ClassConceptosFiniquitos cconceptos = new ClassConceptosFiniquitos();

            conceptosFiniquitos = cconceptos.GetvConfiguracionConceptosFiniquitos(UnidadNegocio.IdCliente);
        }

        /// <summary>
        ///     Método que obtiene los conceptos de los finiquitos
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <returns></returns>
        public vConfiguracionConceptosFiniquitos GetConceptosFiniquitos(int IdCliente)
        {
            ClassConceptosFiniquitos cconceptos = new ClassConceptosFiniquitos();

            return cconceptos.GetvConfiguracionConceptosFiniquitos(IdCliente);
        }

        /// <summary>
        ///     Método que obtiene la cantidad de días de vacaciones que corresponden con respecto a la antigüedad 
        /// </summary>
        /// <param name="Antiguedad">Variable que contiene los años de antigüedad</param>
        /// <returns>Días de vacaciones correspondientes</returns>
        /// <exception cref="Exception"></exception>
        public decimal ObtenDiasVacacionesPorFactorIntegracion(decimal Antiguedad, int? IdPrestaciones)
        {
            try
            {
                decimal _antiguedad = Math.Round(Antiguedad, 2);
                if (IdPrestaciones != null)
                {
                    int _idp = (int)IdPrestaciones;
                    return (decimal)(from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad && x.IdPrestaciones == _idp) select b.Vacaciones).First();
                }
                else
                {
                    return (decimal)(from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad) select b.Vacaciones).First();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Algo esta mal con la antiguedad del empleado para calculo." + ex.Message, ex);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="Antiguedad"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public decimal ObtenPorcentajePVPorFactorIntegracion(decimal Antiguedad, int? IdPrestacion)
        {
            try
            {
                decimal Dias = 0;
                decimal _antiguedad = Math.Round(Antiguedad, 2);
                if (IdPrestacion != null)
                {
                    int idp = (int)IdPrestacion;
                    Dias = (decimal)(from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad && x.IdPrestaciones == idp) select b.PrimaVacacional).First();
                }
                else
                {
                   Dias = (decimal)(from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad) select b.PrimaVacacional).First();
                }                

                return Dias * .1M;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Método que obtiene los días de aguinaldo correspondientes por el factor de integración
        /// </summary>
        /// <param name="Antiguedad">Variable que contiene los años de antigüedad</param>
        /// <returns>Cantidad de días de aguinaldo correspondientes</returns>
        /// <exception cref="Exception"></exception>
        public decimal ObtenDiasAguinaldoPorFactorIntegracion(decimal Antiguedad, int? IdPrestacion )
        {
            try
            {
                decimal Dias = 0;
                decimal _antiguedad = Math.Round(Antiguedad, 2);
                if (IdPrestacion != null)
                {
                    int idp = (int)IdPrestacion;
                    Dias = (decimal)(from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad && x.IdPrestaciones == idp) select b.Aguinaldo).First();
                }
                else
                {
                    Dias = (decimal)(from b in prestaciones.Where(x => x.Limite_Superior >= _antiguedad && x.Limite_Inferior <= _antiguedad) select b.Aguinaldo).First();
                }               

                return Dias;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Método que hace la validación de los campos de los finiquitos
        /// </summary>
        /// <param name="IdCliente">Variable que contiene el id del cliente</param>
        /// <returns></returns>
        public bool ValidaCamposFiniquitos(int IdCliente)
        {
            bool validacion = false;
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var registros = (from b in entidad.ConfiguracionConceptosFiniquito.Where(x => x.IdCliente == IdCliente && x.IdEstatus == 1) select b).FirstOrDefault();

                if(registros != null)
                {
                    if (registros.IdConceptoAguinaldo == null || registros.IdConceptoPV == null || registros.IdConceptoVacaciones == null || registros.IdConceptoIndem20D == null || registros.IdConceptoIndem3M == null || registros.IdConceptoIndemPA == null)
                        return false;
                    else
                        return true;
                }

                return validacion;
            }
        }
    }
}