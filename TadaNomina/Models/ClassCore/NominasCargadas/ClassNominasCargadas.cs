using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;
using static TadaNomina.Models.ClassCore.ClassIncidencias;

namespace TadaNomina.Models.ClassCore.NominasCargadas
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassNominasCargadas
    {
        /// <summary>
        /// Método para obtener el modelo de la vista de Nominas Cargadas
        /// </summary>
        /// <param name="periodo">Es un objero del tipo vPeriodoNomina</param>
        /// <returns></returns>
        public ModelNominasCargadas GetModelNominasCargadas(vPeriodoNomina periodo)
        {
            ModelNominasCargadas m = new ModelNominasCargadas();

            m.Cliente = periodo.Cliente;
            m.IdPeriodoNomina = periodo.IdPeriodoNomina;
            m.Periodo = periodo.Periodo;
            m.TipoNomina = periodo.TipoNomina;
            m.FechaInicial = periodo.FechaInicio.ToShortDateString();
            m.FechaFinal = periodo.FechaFin.ToShortDateString();
            m.UnidadNegocio = periodo.UnidadNegocio;
            m.Periodicidad = periodo.Periodicidad;

            // Datos relacionados con la carga
            List<NominaTrabajo> dataNomina = GetDataNominaTrabajo(periodo.IdPeriodoNomina);

            m.TotalEmpleadosCargados = dataNomina.Count();
            m.TotalSueldo = string.Format("{0:C2}", dataNomina.Select(x => x.SueldoPagado).Sum());
            m.TotalSubsidio = string.Format("{0:C2}", dataNomina.Select(x => x.SubsidioPagar).Sum());
            m.TotalPercepciones= string.Format("{0:C2}", dataNomina.Select(x => x.ER).Sum());
            m.TotalISR = string.Format("{0:C2}", dataNomina.Select(x => x.ImpuestoRetener).Sum());
            m.TotalIMSS = string.Format("{0:C2}", dataNomina.Select(x => x.IMSS_Obrero).Sum());
            m.TotalDeducciones = string.Format("{0:C2}", dataNomina.Select(x => x.DD).Sum());
            m.Neto = string.Format("{0:C2}", dataNomina.Select(x => x.Neto).Sum());
            m.TotalISN = string.Format("{0:C2}", dataNomina.Select(x => x.ISN).Sum());
            m.BaseGravada= string.Format("{0:C2}", dataNomina.Select(x => x.BaseGravada).Sum());

            return m;
        }

        /// <summary>
        /// Método para extraer la información de todo lo que esta procesado en el periodo de nómina d el atabla NominaTrabajo 
        /// </summary>
        /// <param name="IdPeriodo">Identificador del periodo de nómina</param>
        /// <returns>información de todo lo que esta procesado en el periodo de nómina de la tabla NominaTrabajo</returns>
        public List<NominaTrabajo> GetDataNominaTrabajo(int IdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                return entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == IdPeriodo && x.IdEstatus == 1).ToList();
            }
        }

        /// <summary>
        /// Método para cargar tanto en NominaTrabajop como en Incidencias por medio del layout los valores para la nomina proporcionada
        /// </summary>
        /// <param name="ruta">Ruta del layout</param>
        /// <param name="IdCliente">Iddentificador del cliente</param>
        /// <param name="IdUnidadNegocio">Idebtificador de la unidad de negocio</param>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        /// <param name="TipoPeriodo">Tipo de Periodo de Nomina</param>
        /// <param name="IdUsuario">Identificador del usuario que esta ejecutando el proceso</param>
        /// <exception cref="Exception"></exception>
        public void GetIncidenciasNominasCargadas(string ruta, int IdCliente, int IdUnidadNegocio, int IdPeriodoNomina, string TipoPeriodo, int IdUsuario)
        {
            try
            {
                ArrayList array = GetArrayIncidencias1(ruta);
                List<ModelIncidencias> incidencias = new List<ModelIncidencias>();
                ClassConceptos cconceptos = new ClassConceptos();
                ClassIncidencias inci = new ClassIncidencias();
                List<vConceptos> lconcepto = cconceptos.GetvConceptos(IdCliente);
                ArrayList arrayConcepto = GetArrayConceptos(ruta);
                object itemConcepto = string.Empty;
                if (arrayConcepto.Count > 0)
                {
                    itemConcepto = arrayConcepto[0];
                }
                ClassEmpleado cempleado = new ClassEmpleado();
                List<vEmpleados> vEmp = cempleado.GetAllvEmpleados(IdUnidadNegocio);

                //Primero realizamos la insercion en la tabla de NominaTrabajo

                // Eliminamos registros cargados por el mismo metodo
                DeleteNominaTrabajo(IdPeriodoNomina);

                foreach (var item in array)
                {
                    AddRegistroNominaCargada(IdPeriodoNomina, vEmp, item, IdUsuario, IdUnidadNegocio);
                }


                // Ahora pasamos a insertar las incidencias
                array = GetArrayIncidencias2(ruta);
                vEmp = cempleado.GetAllvEmpleados(IdUnidadNegocio);  // Recargamos esta lista porque se pudieron haber insertado nuevos empleados en el proceso

                foreach (var item in array)
                {
                    AddRegistrosIncidencias(IdPeriodoNomina, incidencias, lconcepto, vEmp, item, itemConcepto);
                }

                inci.NewIncindencia(incidencias, lconcepto, vEmp, IdUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene en un formato los registros del layout
        /// </summary>
        /// <param name="ruta">Ruta del layout</param>
        /// <returns></returns>
        public ArrayList GetArrayIncidencias1(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;
            int contador = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (contador > 1)
                    {
                        arrText.Add(sLine);
                    }
                    contador++;
                }
            }

            objReader.Close();
            return arrText;
        }

        /// <summary>
        /// Obtiene en un formato los registros del layout
        /// </summary>
        /// <param name="ruta">Ruta del layout</param>
        /// <returns></returns>
        public ArrayList GetArrayIncidencias2(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;
            int contador = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (contador > 0)
                    {
                        arrText.Add(sLine);
                    }
                    contador++;
                }
            }

            objReader.Close();
            return arrText;
        }

        /// <summary>
        /// Método para obtener un listado de los conceptos de un archivo con formato .csv
        /// </summary>
        /// <param name="ruta">Ruta del archivo</param>
        /// <returns>Listado de los conceptos capturadas en el archivo</returns>
        public ArrayList GetArrayConceptos(string ruta)
        {
            StreamReader objReader = new StreamReader(ruta);
            ArrayList arrText = new ArrayList();
            string sLine = string.Empty;

            sLine = objReader.ReadLine();
            arrText.Add(sLine);
            objReader.Close();
            return arrText;
        }

        /// <summary>
        /// Método para incertar las incidencias a la base de datos
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nómina</param>
        /// <param name="errores">listado de errores encontrados</param>
        /// <param name="incidencias">listado de las incidencias por cargar</param>
        /// <param name="lconcepto">listado de conceptos</param>
        /// <param name="vEmp">listado de la información de los empleados</param>
        /// <param name="item">contenido del archivo</param>
        /// <param name="itemConcepto">listado de la información de los conceptos</param>
        private void AddRegistrosIncidencias(int IdPeriodoNomina, List<ModelIncidencias> incidencias, List<vConceptos> lconcepto, List<vEmpleados> vEmp, object item, object itemConcepto)
        {
            string[] campos = item.ToString().Split(',');
            List<string> conceptos = itemConcepto.ToString().Split(',').ToList();
            List<Mapeo> valores = new List<Mapeo>();
            for (int i = 14; i < campos.Length; i++)
            {
                valores.Add(new Mapeo { Campo = campos[i], Concepto = conceptos[i], Valor = campos[i] });
            }

            var datos1 = valores.AsEnumerable().Where(x => x.Valor != "").ToList();
            foreach (Mapeo datos in valores.AsEnumerable().Where(x => x.Valor != "").ToList())
            {
                ModelIncidencias i = new ModelIncidencias();

                decimal Monto = 0;

                int IdEmpleado = vEmp.Where(x => x.ClaveEmpleado.Trim() == campos[0].Trim()).Select(x => x.IdEmpleado).FirstOrDefault();
                int IdConcepto = lconcepto.Where(x => x.IdConcepto.ToString() == datos.Concepto.Split('-')[0]).Select(x => x.IdConcepto).FirstOrDefault();
                string TipoDato = lconcepto.Where(x => x.IdConcepto.ToString() == datos.Concepto.Split('-')[0]).Select(x => x.TipoDato).FirstOrDefault();
                string TipoEsquema = lconcepto.Where(x => x.IdConcepto.ToString() == datos.Concepto.Split('-')[0]).Select(x => x.TipoEsquema).FirstOrDefault();
                i.IdEmpleado = IdEmpleado;
                i.IdPeriodoNomina = IdPeriodoNomina;
                i.IdConcepto = IdConcepto;
                if (IdEmpleado != 0)
                {
                    if (TipoDato == "Pesos")
                    {
                        try { Monto = decimal.Parse(datos.Valor); } catch { Monto = 0; }
                        i.Monto = Monto;
                        i.MontoEsquema=0;
                    }
                    incidencias.Add(i);
                }
            }

        }

        /// <summary>
        /// Método para insertar registros de nominas proporcionadas
        /// </summary>
        /// <param name="IdPeriodoNomina">Identificador del periodo de nomina</param>
        /// <param name="vEmp">Lista de objetos tipo vEmpleados</param>
        /// <param name="item">Elemento con los encabezados del layout</param>
        /// <param name="IdUsuario">Identificador del usuario que ejecuta el proceso</param>
        private void AddRegistroNominaCargada(int IdPeriodoNomina, List<vEmpleados> vEmp, object item, int IdUsuario, int IdUnidadNegocio)
        {
            string[] datos = item.ToString().Split(',');
            int _IdEmpleado = 0;

            NominaTrabajo nt = new NominaTrabajo();

            int IdEmpleado = vEmp.Where(x => x.Rfc.Trim() == datos[1].Trim()).Select(x => x.IdEmpleado).FirstOrDefault();

            // Si el empleado existe
            if (IdEmpleado != 0)
            {
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    nt.IdEmpleado=IdEmpleado;
                    try { nt.SueldoPagado= decimal.Parse(datos[5]); } catch { nt.SueldoPagado=0; }
                    try { nt.SubsidioPagar= decimal.Parse(datos[6]); } catch { nt.SubsidioPagar=0; }
                    try { nt.ER= decimal.Parse(datos[7]); } catch { nt.ER=0; }
                    try { nt.ImpuestoRetener= decimal.Parse(datos[8]); } catch { nt.ImpuestoRetener=0; }
                    try { nt.IMSS_Obrero= decimal.Parse(datos[9]); } catch { nt.IMSS_Obrero = 0; }
                    try { nt.DD= decimal.Parse(datos[10]); } catch { nt.DD= 0; }
                    try { nt.Neto= decimal.Parse(datos[11]); } catch { nt.Neto= 0; }
                    try { nt.ISN= decimal.Parse(datos[12]); } catch { nt.ISN= 0; }
                    try { nt.BaseGravada= decimal.Parse(datos[13]); } catch { nt.BaseGravada= 0; }
                    try { nt.BaseGravadaP= decimal.Parse(datos[13]); } catch { nt.BaseGravadaP= 0; }

                    // Estos campos se insertan para que sean consideradas estas nominas para ajustes                    
                    nt.ISR=nt.ImpuestoRetener;

                    nt.Subsidio=nt.SubsidioPagar;

                    if (nt.BaseGravada==0)
                    {
                        nt.BaseGravada= nt.ER;
                        nt.BaseGravadaP=nt.ER;
                    }


                    nt.IdPeriodoNomina=IdPeriodoNomina;
                    nt.IdCaptura= IdUsuario;
                    nt.FechaCaptura= DateTime.Now;
                    nt.IdEstatus=1;

                    entidad.NominaTrabajo.Add(nt);
                    entidad.SaveChanges();
                }
            }
            else
            {
                // Si el empleado no existe lo damos de alta con un estatus especial.
                using (TadaEmpleados entidad = new TadaEmpleados())
                {
                    Empleados emp = new Empleados()
                    {
                        IdUnidadNegocio=IdUnidadNegocio,
                        ClaveEmpleado= datos[0],
                        Rfc= datos[1],
                        Nombre= datos[2],
                        ApellidoPaterno= datos[3],
                        ApellidoMaterno= datos[4],
                        SDIMSS=decimal.Parse(datos[5]) / 15,

                        IdEntidad=0,
                        Esquema="100% TRADICIONAL",
                        TipoContrato="DETERMINADO",

                        IdCaptura=IdUsuario,
                        FechaCaptura= DateTime.Now,
                        IdEstatus=5,
                    };

                    entidad.Empleados.Add(emp);
                    entidad.SaveChanges();

                    _IdEmpleado=emp.IdEmpleado;
                }

                // Ahora con el nuevo valor del IdEmpleado gravamos el registro
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    nt.IdEmpleado=_IdEmpleado;
                    try { nt.SueldoPagado= decimal.Parse(datos[5]); } catch { nt.SueldoPagado=0; }
                    try { nt.SubsidioPagar= decimal.Parse(datos[6]); } catch { nt.SubsidioPagar=0; }
                    try { nt.ER= decimal.Parse(datos[7]); } catch { nt.ER=0; }
                    try { nt.ImpuestoRetener= decimal.Parse(datos[8]); } catch { nt.ImpuestoRetener=0; }
                    try { nt.IMSS_Obrero= decimal.Parse(datos[9]); } catch { nt.IMSS_Obrero = 0; }
                    try { nt.DD= decimal.Parse(datos[10]); } catch { nt.DD= 0; }
                    try { nt.Neto= decimal.Parse(datos[11]); } catch { nt.Neto= 0; }
                    try { nt.ISN= decimal.Parse(datos[12]); } catch { nt.ISN= 0; }
                    try { nt.BaseGravada= decimal.Parse(datos[13]); } catch { nt.BaseGravada= 0; }
                    try { nt.BaseGravadaP= decimal.Parse(datos[13]); } catch { nt.BaseGravadaP= 0; }

                    // Estos campos se insertan para que sean consideradas estas nominas para ajustes                    
                    nt.ISR=nt.ImpuestoRetener;

                    nt.Subsidio=nt.SubsidioPagar;

                    if (nt.BaseGravada==0)
                    {
                        nt.BaseGravada= nt.ER;
                        nt.BaseGravadaP=nt.ER;
                    }

                    nt.IdPeriodoNomina=IdPeriodoNomina;
                    nt.IdCaptura= IdUsuario;
                    nt.FechaCaptura= DateTime.Now;
                    nt.IdEstatus=1;

                    entidad.NominaTrabajo.Add(nt);
                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Metodo para borrar todos los registros de la tabla de nómina de un periodo de nómina.
        /// </summary>
        /// <param name="pIdPeriodo">Identificador del periodo de nómina.</param>
        public void DeleteNominaTrabajo(int pIdPeriodo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var registro = (from b in entidad.NominaTrabajo.Where(x => x.IdPeriodoNomina == pIdPeriodo) select b);

                if (registro != null)
                {
                    entidad.NominaTrabajo.RemoveRange(registro);
                    entidad.SaveChanges();
                }
            }
        }
    }
}
