using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ClassProcesosTimbrado: cCreaXMLCancelacion
    {
        public string Token { get; set; }
        public string URI { get; set; }

        /// <summary>
        /// Metodo para obtener una lista información para XML por periodo de nomina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nomina</param>
        /// <returns>Lista de información de XML</returns>
        public List<sp_InformacionXML_Nomina1_Result> GetInformacionXML(int IdPeriodo) 
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var datos = (from b in entidad.sp_InformacionXML_Nomina1(IdPeriodo) select b).ToList();

                return datos;
            }
        }
        
        /// <summary>
        /// Metodo para obtener TOken y URI y guardas en variables globales
        /// </summary>
        /// <param name="Tipo"></param>
        public void Get_Token(string Tipo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var cat_token = (from b in entidad.Cat_Token_MasNegocio.Where(x => x.IdEstatus == 1 && x.Tipo == Tipo) select b).FirstOrDefault();
                Token = cat_token.Token;
                URI = cat_token.URI;
            }
        }

        /// <summary>
        /// Metodo para guardar error de timbrado
        /// </summary>
        /// <param name="i">Informacion XML</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="IdPeriodoNomina">Periodo de nomina</param>
        /// <param name="Id"></param>
        /// <param name="Codigo">Codigo de error</param>
        /// <param name="Texto">Informacion error</param>
        /// <param name="Observaciones"></param>
        public void GuardaError(sp_InformacionXML_Nomina1_Result i, int IdUsuario, int IdPeriodoNomina, Guid Id, string Codigo, string Texto, string Observaciones)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores le = new LogErrores();
                le.Guid = Id;
                le.IdPeriodoNomina = IdPeriodoNomina;
                le.Modulo = "Timbrado";
                le.Referencia = i.Rfc;
                le.Descripcion = "Error: " + ((char)13) + Codigo + " - " + Texto + " - " + Observaciones;
                le.Fecha = DateTime.Now;
                le.IdUsuario = IdUsuario;
                le.IdEstatus = 1;

                entidad.LogErrores.Add(le);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para guardar error de timbrado
        /// </summary>
        /// <param name="i">Informacion XML</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="IdPeriodoNomina">Periodo de nomina</param>
        /// <param name="Id"></param>
        /// <param name="Codigo">Codigo de error</param>
        /// <param name="Texto">Informacion error</param>
        /// <param name="Observaciones"></param>
        public void GuardaError(string Rfc, int IdUsuario, int IdPeriodoNomina, Guid Id, string Codigo, string Texto, string Observaciones)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores le = new LogErrores();
                le.Guid = Id;
                le.IdPeriodoNomina = IdPeriodoNomina;
                le.Modulo = "Timbrado";
                le.Referencia = Rfc;
                le.Descripcion = "Error: " + ((char)13) + Codigo + " - " + Texto + " - " + Observaciones;
                le.Fecha = DateTime.Now;
                le.IdUsuario = IdUsuario;
                le.IdEstatus = 1;

                entidad.LogErrores.Add(le);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para guardar error de timbrado
        /// </summary>
        /// <param name="i">Informacion XML</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="IdPeriodoNomina">Periodo de nomina</param>
        /// <param name="Id"></param>
        /// <param name="Codigo">Codigo de error</param>
        /// <param name="Texto">Informacion error</param>
        /// <param name="Observaciones"></param>
        public void GuardaError(vXmlNomina i, int IdUsuario, int IdPeriodoNomina, Guid Id, string Codigo, string Texto, string Observaciones)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores le = new LogErrores();
                le.Guid = Id;
                le.IdPeriodoNomina = IdPeriodoNomina;
                le.Modulo = "Timbrado";
                le.Referencia = i.Rfc;
                le.Descripcion = "Error: " + ((char)13) + Codigo + " - " + Texto + " - " + Observaciones;
                le.Fecha = DateTime.Now;
                le.IdUsuario = IdUsuario;
                le.IdEstatus = 1;

                entidad.LogErrores.Add(le);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Guarda comprobante de timbrado
        /// </summary>
        /// <param name="i">Informacion timbrado</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="IdPeriodoNomina">Periodo nómina</param>
        /// <param name="uuid"></param>
        /// <param name="fechaTimbrado">Fecha timbrado</param>
        /// <param name="anioMes">Año mes</param>
        /// <param name="FacturaTimbrada">CDFI</param>
        /// <param name="Leyenda">Leyenda timbrado</param>
        public void GuardaTablaTimbrado(sp_InformacionXML_Nomina1_Result i, int IdUsuario, int IdPeriodoNomina, string uuid, string fechaTimbrado, int anioMes, string FacturaTimbrada, string Leyenda)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                TimbradoNomina tn = new TimbradoNomina();
                tn.IdPeriodoNomina = IdPeriodoNomina;
                tn.IdEmpleado = (i.IdEmpleado);
                tn.IdRegistroPatronal = (i.IdRegistroPatronal);
                tn.RegistroPatronal = i.RegistroPatronal;
                tn.NombrePatrona = i.NombrePatrona;
                tn.RFC = i.Rfc;
                tn.FechaTimbrado = fechaTimbrado;
                tn.FolioUDDI = uuid;
                tn.AnioMes = anioMes;
                tn.Mensaje = "Comprobante timbrado exitosamente";
                tn.IdEstatus = 1;
                tn.IdCaptura = IdUsuario;
                tn.FechaCaptura = DateTime.Now;
                tn.FechaInicioPeriodo = i.FechaInicio;
                tn.FechaFinPeriodo = i.FechaFin;
                tn.CFDI_Timbrado = FacturaTimbrada;
                tn.Leyenda = Leyenda;

                entidad.TimbradoNomina.Add(tn);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Guarda comprobante de timbrado
        /// </summary>
        /// <param name="i">Informacion timbrado</param>
        /// <param name="IdUsuario">Usuario</param>
        /// <param name="IdPeriodoNomina">Periodo nómina</param>
        /// <param name="uuid"></param>
        /// <param name="fechaTimbrado">Fecha timbrado</param>
        /// <param name="anioMes">Año mes</param>
        /// <param name="FacturaTimbrada">CDFI</param>
        /// <param name="Leyenda">Leyenda timbrado</param>
        public void GuardaTablaTimbrado(vXmlNomina i, int IdUsuario, int IdPeriodoNomina, string uuid, string fechaTimbrado, int anioMes, string FacturaTimbrada, string Leyenda)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                TimbradoNomina tn = new TimbradoNomina();
                tn.IdPeriodoNomina = IdPeriodoNomina;
                tn.IdEmpleado = (i.IdEmpleado);
                tn.IdRegistroPatronal = (i.IdRegistroPatronal);
                tn.RegistroPatronal = i.RegistroPatronal;
                tn.NombrePatrona = i.NombrePatrona;
                tn.RFC = i.Rfc;
                tn.FechaTimbrado = fechaTimbrado;
                tn.FolioUDDI = uuid;
                tn.AnioMes = anioMes;
                tn.Mensaje = "Comprobante timbrado exitosamente";
                tn.IdEstatus = 1;
                tn.IdCaptura = IdUsuario;
                tn.FechaCaptura = DateTime.Now;
                tn.FechaInicioPeriodo = i.FechaInicio;
                tn.FechaFinPeriodo = i.FechaFin;
                tn.CFDI_Timbrado = FacturaTimbrada;
                tn.Leyenda = Leyenda;

                entidad.TimbradoNomina.Add(tn);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para obtener lista de timbrados por periodo de nomina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <returns>Lista de timbrados</returns>
        public List<TimbradoNomina> GetTimbrados(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = (from b in entidad.TimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).ToList();

                return timbrado;
            }
        }

        /// <summary>
        /// Metodo para obtener lista de timbrados cancelados por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <returns>Lista de timbrados cancelados</returns>
        public List<TimbradoNomina> GetCancelados(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = (from b in entidad.TimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 2) select b).ToList();

                return timbrado;
            }
        }

        /// <summary>
        /// Metodo para obtener lista de timbrados por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <returns>Lista de timbrados</returns>
        public List<vTimbradoNomina> GetvTimbrados(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = (from b in entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b).ToList();

                return timbrado;
            }
        }

        /// <summary>
        /// Metodo para obtener la lista de errores generados al timbrar por un determinado periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Perido de nómina</param>
        /// <param name="guid"></param>
        /// <returns>Lista de errores</returns>
        public List<LogErrores> GetErrores(int IdPeriodo, Guid guid)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = (from b in entidad.LogErrores.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1 && x.Guid == guid) select b).ToList();

                return timbrado;
            }
        }

        /// <summary>
        /// Lista con datos de timbrados por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodoNomina">Periodo de nómina</param>
        /// <returns>Lista de timbrados</returns>
        public List<vTimbradoNomina> ObtenDatosTimbradoNominaPeriodo(int IdPeriodoNomina)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var periodo = (from b in entidad.vTimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdEstatus == 1) select b);

                return periodo.ToList();
            }
        }

        /// <summary>
        /// Metodo para guardar error de cancelacion de timbrado de nómina
        /// </summary>
        /// <param name="datos">Datos del timbrado</param>
        /// <param name="id"></param>
        /// <param name="IdUsuario">usuario</param>
        public void GuardaErrorCancelacion(vTimbradoNomina datos, Guid id, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                LogErrores logErrores = new LogErrores();
                logErrores.Guid = id;
                logErrores.IdPeriodoNomina = datos.IdPeriodoNomina;
                logErrores.Modulo = "Cancelacion Timbrado";
                logErrores.Referencia = datos.IdTimbradoNomina.ToString();
                logErrores.Descripcion = "No se pudo cancelar el timbre";
                logErrores.Fecha = DateTime.Now;
                logErrores.IdUsuario = IdUsuario;
                logErrores.IdEstatus = 1;

                entidad.LogErrores.Add(logErrores);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para actualizar registro de trimbrado
        /// </summary>
        /// <param name="UUID">Folio fiscal</param>
        /// <param name="IdUsuario">usuario</param>
        public void ActualizaRegistroTimbraado(string UUID, int IdUsuario)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var timbrado = from b in entidad.TimbradoNomina.Where(x => x.FolioUDDI == UUID && x.IdEstatus == 1) select b;

                foreach (var item in timbrado)
                {
                    item.IdEstatus = 2;
                    item.IdModifica = IdUsuario;
                    item.FechaModifica = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para obtener timbrados de nomina por folio fiscal
        /// </summary>
        /// <param name="FolioUUID">Identificador folio fiscal</param>
        /// <returns>Lista de datos de timbrados</returns>
        public List<vTimbradoNomina> ObtenDatosTimbradoNominaSinPeriodo(string[] FolioUUID)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                var periodo = (from b in entidad.vTimbradoNomina.Where(x => x.IdEstatus == 1 && FolioUUID.Contains(x.FolioUDDI)) select b);

                return periodo.ToList();
            }
        }

        /// <summary>
        /// Metodo que obtiene cantidad de timbrados por periodo de nómina
        /// </summary>
        /// <param name="IdPeriodo">Periodo de nómina</param>
        /// <returns>Número de registros</returns>
        public int GetCantidadTimbresPeriodoNomina(int IdPeriodo)
        {
            using (TadaTimbradoEntities entidad = new TadaTimbradoEntities())
            {
                int cantidad = 0;
                var registros = from b in entidad.TimbradoNomina.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1) select b;

                cantidad = registros.Count();

                return cantidad;
            }
        }
    }
}