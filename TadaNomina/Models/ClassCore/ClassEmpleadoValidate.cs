using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.DB;
using TadaNomina.Models.ClassCore;
using System.IO;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace TadaNomina.Models
{
    public enum Type
    {
        Warning,
        Error,
        Invalid,
        Success
    }

    public class ColumnFile
    {
        public int Column { get; set; }
        public string Field { get; set; }
        public string ColumnDetail { get; set; }
        public Type Type { get; set; }

    }

    public class RowFile
    {
        public int Row { get; set; }
        public Type RowValidation { get; set; }

        public string RowDetail { get; set; }

        public List<ColumnFile> Columns { get; set; }
    }
    /// <summary>
    /// Validación Empleados
    /// Autor: Diego Rodríguez
    /// Fecha Ultima Modificación: 17/05/2022, Razón: Documentación del código
    /// </summary>
    public class FileImport
    {
        private string FileName { get; set; }
        private List<RowFile> RowFiles { get; set; }
        private int InsertedCount { get; set; }
        private int ErrorCount { get; set; }
        private int TotalCount { get; set; }
        private List<Cat_CentroCostos> CentrosCostos { get; set; }
        private List<Cat_Departamentos> Departamentos { get; set; }
        private List<Cat_Puestos> Puestos { get; set; }
        private List<Cat_RegistroPatronal> RegistrosPatronales { get; set; }
        private List<Empleados> Empleados { get; set; }
        private List<Empleados> EmpleadosAlta { get; set; }
        private List<Cat_Bancos> Bancos { get; set; }
        private List<Cat_Areas> Areas { get; set; }
        private List<Sindicatos> Sindicatos { get; set; }
        private List<Cat_Sucursales> Sucursales { get; set; }
        private List<SelectListItem> Jornadas { get; set; }

        /// <summary>
        /// Método para ontener el archivo .csv
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="idunidadnegocio">Identificador de la unidad de negocio</param>
        public FileImport(int IdCliente, int idunidadnegocio)
        {
            ClassEmpleado config = new ClassEmpleado();
            RowFiles = new List<RowFile>();
            CentrosCostos = config.GetCentrosCostosByClient(IdCliente);
            Departamentos = config.GetDepartamentosByCliente(IdCliente);
            Puestos = config.GetPuestosByCliente(IdCliente);
            RegistrosPatronales = config.ObtenerRPByIdCliente(IdCliente);
            Empleados = config.GetEmpleadoByUnidadNegocio(idunidadnegocio);
            EmpleadosAlta = Empleados.FindAll(x => x.IdEstatus == 1).ToList();
            Bancos = config.GetBancosGral();
            Areas = config.GetAreasByClient(IdCliente);
            Sindicatos = config.GetAreasBySindicatos();
            Sucursales = config.GetAllSucursales(IdCliente);
            RowFiles = new List<RowFile>();
            Jornadas = config.GetJornadas(IdCliente);
        }

        /// <summary>
        /// Método validar la informacíon de los empleados para su alta masiva
        /// </summary>
        /// <param name="csvData">Archivo .csv con l ainformación de los los empleados</param>
        /// <param name="nameFile">Nombre del archivo</param>
        /// <param name="idUsuario">Identificador del usuario</param>
        /// <param name="pIdUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <returns>Contador de los errores detectados</returns>
        public int Init(string csvData, string nameFile, int idUsuario, int pIdUnidadNegocio, int idCliente)
        {
            FileName = nameFile;
            int index = 0;
            TotalCount = 0;
            InsertedCount = 0;
            ErrorCount = 0;

            List<Empleado> empleadosBatch = new List<Empleado>();

            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row) && index > 0)
                {
                    RowFile rowFile = new RowFile();
                    rowFile.Columns = new List<ColumnFile>();
                    rowFile.Row = index;

                    if (Validate(row, rowFile, idCliente, pIdUnidadNegocio).Equals(Type.Success))
                    {
                        int? IdBancoViatico = null;
                        try { IdBancoViatico = idBancoViaticos(row.Split(',')[41].Trim()); } catch { IdBancoViatico = null; }
                        string CuentaViatico = null;
                        try { CuentaViatico = Cuenta(row.Split(',')[42].Trim()); } catch { CuentaViatico = null; }
                        string CuentaInterViatico = null;
                        try { CuentaInterViatico = Cuenta(row.Split(',')[43].Trim()); } catch { CuentaInterViatico = null; }                        
                        empleadosBatch.Add(new Empleado
                        {
                            IdUnidadNegocio = pIdUnidadNegocio,
                            IdCentroCostos = CentroCostos(row.Split(',')[0].Trim()),
                            IdDepartamento = Departamento(row.Split(',')[1].Trim()),
                            IdPuesto = Puesto(row.Split(',')[2].Trim()),
                            IdRegistroPatronal = RegistroPatronal(row.Split(',')[3].Trim()),
                            IdEntidad = Convert.ToInt32(row.Split(',')[4].Trim()),
                            ClaveEmpleado = row.Split(',')[5].Trim().ToUpper(),
                            Nombre = Nombre(row.Split(',')[6].Trim().ToUpper()),
                            ApellidoPaterno = Nombre(row.Split(',')[7].Trim().ToUpper()),
                            ApellidoMaterno = Nombre(row.Split(',')[8].Trim().ToUpper()),
                            Sexo = Sexo(row.Split(',')[9].Trim().ToUpper()),
                            EstadoCivil = EstadoCivil(row.Split(',')[10].Trim().ToUpper()),
                            FechaNacimiento = Fecha(row.Split(',')[11].Trim()),
                            SD = NulableDecimal(row.Split(',')[12].Trim()),
                            SDIMSS = NulableDecimal(row.Split(',')[13].Trim()),
                            SDI = NulableDecimal(row.Split(',')[14].Trim()),
                            IdBancoTrad = IdBanco(row.Split(',')[15].Trim()),
                            CuentaBancariaTrad = Cuenta(row.Split(',')[16].Trim()),
                            CuentaInterbancariaTrad = Cuenta(row.Split(',')[17].Trim()),
                            Curp = row.Split(',')[18].Trim().ToUpper(),
                            Rfc = row.Split(',')[19].Trim().ToUpper(),
                            Imss = Imss(row.Split(',')[20].Trim()),
                            CorreoElectronico = CorreoElectronico(row.Split(',')[21].Trim().ToLower()),
                            FechaReconocimientoAntiguedad = Fecha(row.Split(',')[22].Trim()),
                            FechaAltaIMSS = Fecha(row.Split(',')[23].Trim()),
                            Esquema = row.Split(',')[24].Trim().ToUpper(),
                            TipoContrato = row.Split(',')[25].Trim().ToUpper(),
                            IdCaptura = idUsuario,
                            IdEstatus = 1,
                            FechaCaptura = DateTime.Now,
                            NumeroTelefonico = NulableString(row.Split(',')[26].Trim()),

                            CalleFiscal = NulableString(row.Split(',')[27].Trim().ToUpper()),
                            NumeroExtFiscal = NulableString(row.Split(',')[28].Trim().ToUpper()),
                            NumeroIntFiscal = NulableString(row.Split(',')[29].Trim().ToUpper()),
                            CodigoPostalFiscal = row.Split(',')[30].Trim(),
                            Calle = NulableString(row.Split(',')[31].Trim().ToUpper()),
                            NumeroExt = NulableString(row.Split(',')[32].Trim().ToUpper()),
                            NumeroInt = NulableString(row.Split(',')[33].Trim().ToUpper()),
                            CodigoPostal = row.Split(',')[34].Trim(),

                            idArea = IdArea(row.Split(',')[35].Trim().ToUpper()),
                            Idsindicato = idSindicato(row.Split(',')[36].Trim().ToUpper()),
                            IdSucursal = idSucursal(row.Split(',')[37].Trim().ToUpper()),
                            IdJornada = idJornada(row.Split(',')[38].Trim().ToUpper()),
                            IdBancoViaticos = IdBancoViatico,
                            CuentaBancariaViaticos = CuentaViatico,
                            CuentaInterBancariaViaticos = CuentaInterViatico,
                            Nacionalidad = ValidaNacionalidad(row.Split(',')[39].Trim()),
                            FechaTerminoContrato = Fecha(row.Split(',')[40].Trim()),
                            IdTimbrado = row.Split(',')[44].Trim().ToUpper(),

                        });

                        InsertedCount++;
                    }
                    else
                    {
                        ErrorCount++;
                    }

                    RowFiles.Add(rowFile);
                }
                index++;
            }

            TotalCount = (index - 1);
            ClassEmpleado empleado = new ClassEmpleado();
            decimal SMG_ = empleado.ObtenSMV();
            return empleado.AddEmpleadoBatch(empleadosBatch, SMG_, idCliente, pIdUnidadNegocio);
        }

        /// <summary>
        /// Método validar la informacíon de los empleados para su baja masiva
        /// </summary>
        /// <param name="csvData">Archivo .csv con l ainformación de los los empleados</param>
        /// <param name="nameFile">Nombre del archivo</param>
        /// <param name="IdUsuario">Identificador del usuario</param>
        /// <param name="IdUnidadNegocio">Identificador de la uni
        public int InitBaja(string csvData, string nameFile, int IdUsuario, int IdUnidadNegocio)
        {
            FileName = nameFile;
            int index = 0;
            TotalCount = 0;
            InsertedCount = 0;
            ErrorCount = 0;

            List<Empleado> empleadosBaja = new List<Empleado>();

            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    RowFile rowFile = new RowFile();
                    rowFile.Columns = new List<ColumnFile>();
                    rowFile.Row = index;

                    if (ValidateBaja(row, rowFile, IdUnidadNegocio).Equals(Type.Success))
                    {
                        empleadosBaja.Add(new Empleado
                        {
                            IdUnidadNegocio = IdUnidadNegocio,
                            ClaveEmpleado = row.Split(',')[0].Trim(),
                            FechaBaja = row.Split(',')[1].Trim(),
                            MotivoBaja = row.Split(',')[2].Trim(),
                            Recontratable = NulableString(row.Split(',')[3].Trim()),
                            IdModificacion = IdUsuario
                        });

                        InsertedCount++;
                    }
                    else
                    {
                        ErrorCount++;
                    }

                    RowFiles.Add(rowFile);
                }
                index++;
            }

            TotalCount = index;
            ClassEmpleado service = new ClassEmpleado();
            service.DismissBatch(empleadosBaja);
            return 0;
        }

        #region
        //retonar archivo txt
        /// <summary>
        /// Método para crear un archivo txt en donde se guardaran los archivos
        /// </summary>
        /// <returns>Archivo txt temporal</returns>
        public MemoryStream DetailUpload()
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine("DETALLE DE CARGA DEL ARCHIVO:  " + FileName);
            tw.WriteLine("--------------------------------");
            tw.WriteLine("TOTAL DE REGISTROS LEÍDOS:      " + (TotalCount - 1));
            tw.WriteLine("TOTAL DE REGISTROS INSERTADOS:  " + InsertedCount);
            tw.WriteLine("TOTAL DE REGISTROS ERRÓNEOS:    " + ErrorCount);
            tw.WriteLine("--------------------------------");


            //ERRONEOS            
            tw.WriteLine("");

            int rowLines = 0;

            foreach (var r in RowFiles)
            {

                if (r.RowValidation.Equals(Type.Error))
                {
                    if (rowLines == 0)
                    {
                        tw.WriteLine("DETALLE DE ERRORES DETECTADOS: (EMPLEADOS NO INSERTADOS)");
                    }

                    tw.WriteLine("-> Fila: " + (r.Row + 1) + ": " + " *Tipo: " + r.RowValidation + " *Detalle: " + r.RowDetail);
                    rowLines++;
                }
                else
                {
                    foreach (var c in r.Columns.FindAll(x => x.Type.Equals(Type.Error)))
                    {
                        if (rowLines == 0)
                        {
                            tw.WriteLine("DETALLE DE ERRORES DETECTADOS: (EMPLEADOS NO INSERTADOS)");
                        }

                        tw.WriteLine("-> Fila: " + (r.Row + 1));
                        tw.WriteLine("     -Columna:" + c.Column + "   *Campo: " + c.Field + "    *Detalle: " + c.ColumnDetail + " " + c.Type);
                    }
                    rowLines++;
                }

            }

            tw.WriteLine("");

            //WARNINGS
            int counterLine = 0;
            tw.WriteLine("");

            foreach (var r in RowFiles)
            {
                if (r.Columns.Exists(x => x.Type.Equals(Type.Warning) || x.Type.Equals(Type.Invalid)))
                {
                    if (counterLine.Equals(0))
                    {
                        tw.WriteLine("******************************************************************************************");
                        tw.WriteLine("DETALLE DE ALERTAS DETECTADAS: (VALORES INVALIDOS SUSTITUIDOS)");
                    }

                    tw.WriteLine("-> Fila: " + (r.Row + 1));

                    foreach (var c in r.Columns.FindAll(x => x.Type.Equals(Type.Warning) || x.Type.Equals(Type.Invalid)))
                    {
                        tw.WriteLine("     -Columna:" + c.Column + "   *Campo: " + c.Field + "   *Detalle: " + c.ColumnDetail + " " + c.Type);
                    }

                    tw.WriteLine("");
                    counterLine++;
                }
            }

            tw.Flush();
            tw.Close();
            return memoryStream;
        }

        /// <summary>
        /// Método par acrear la estructura del archivo txt
        /// </summary>
        /// <returns>Archivo txt con estructura</returns>
        public MemoryStream DetailDismiss()
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine("DETALLE DE CARGA DEL ARCHIVO:  " + FileName);
            tw.WriteLine("--------------------------------");
            tw.WriteLine("TOTAL DE REGISTROS LEIDOS:      " + TotalCount);
            tw.WriteLine("TOTAL DE REGISTROS MODIFICADOS:  " + InsertedCount);
            tw.WriteLine("TOTAL DE REGISTROS ERRÓNEOS:    " + ErrorCount);
            tw.WriteLine("--------------------------------");


            //ERRONEOS            
            tw.WriteLine("");

            int rowLines = 0;

            foreach (var r in RowFiles)
            {

                if (r.RowValidation.Equals(Type.Error))
                {
                    if (rowLines == 0)
                    {
                        tw.WriteLine("DETALLE DE ERRORES DETECTADOS: (EMPLEADOS NO MODIFICADOS)");
                    }

                    tw.WriteLine("-> Fila: " + r.Row + ": " + " *Tipo: " + r.RowValidation + " *Detalle: " + r.RowDetail);
                    rowLines++;
                }
                else
                {
                    foreach (var c in r.Columns.FindAll(x => x.Type.Equals(Type.Error)))
                    {
                        if (rowLines == 0)
                        {
                            tw.WriteLine("DETALLE DE ERRORES DETECTADOS: (EMPLEADOS NO MODIFICADOS)");
                        }

                        tw.WriteLine("-> Fila: " + r.Row);
                        tw.WriteLine("     -Columna:" + c.Column + "   *Campo: " + c.Field + "    *Detalle: " + c.ColumnDetail + " " + c.Type);
                    }
                    rowLines++;
                }
                tw.WriteLine("");
            }

            //WARNINGS
            int counterLine = 0;
            tw.WriteLine("");

            foreach (var r in RowFiles)
            {
                if (r.Columns.Exists(x => x.Type.Equals(Type.Warning) || x.Type.Equals(Type.Invalid)))
                {
                    if (counterLine.Equals(0))
                    {
                        tw.WriteLine("DETALLE DE ALERTAS DETECTADAS: (VALORES INVALIDOS SUSTITUIDOS)");
                    }

                    tw.WriteLine("-> Fila: " + r.Row);

                    foreach (var c in r.Columns.FindAll(x => x.Type.Equals(Type.Warning) || x.Type.Equals(Type.Invalid)))
                    {
                        tw.WriteLine("     -Columna:" + c.Column + "   *Campo: " + c.Field + "   *Detalle: " + c.ColumnDetail + " " + c.Type);
                    }

                    tw.WriteLine("");
                    counterLine++;
                }
            }

            tw.Flush();
            tw.Close();
            return memoryStream;
        }

        /// <summary>
        /// Método para validar toda la información para el alta de los empleados
        /// </summary>
        /// <param name="row">linea de texto a analizar</param>
        /// <param name="rowFile"></param>
        /// <param name="IdUnidadNegocio">Identificador unico de la Unidad de Negocio (debera ser obtenida de la variable de session: ["sIdUnidad"]</param>
        /// <returns>Validacíon del archivo</returns>
        public Type Validate(string row, RowFile rowFile, int idCliente, int IdUnidadNegocio)
        {
            if (ValidateRow(row, rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            ValidateIdCentroCostos(row.Split(',')[0].Trim(), rowFile);
            ValidateIdDepartamento(row.Split(',')[1].Trim(), rowFile);
            ValidateIdPuesto(row.Split(',')[2].Trim(), rowFile);
            if (ValidateIdRegistroPatronal(row.Split(',')[3].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateIdEntidad(row.Split(',')[4].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }
            // Se inhabilita validación de ClaveEmpleado para Grupo Marte para permitir autogeneración
            // en caso de campo vacio o nulo
            if (idCliente != 172 || idCliente != 286 || idCliente != 285 || idCliente != 287 || idCliente != 284 || idCliente != 282 || idCliente != 283)
            {
                if (ValidateClaveEmpleado(row.Split(',')[5].Trim(), rowFile).Equals(Type.Error))
                {
                    return Type.Error;
                }
            }

            if (ValidateNombre(row.Split(',')[6].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateApellidoPaterno(row.Split(',')[7].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            ValidateApellidoMaterno(row.Split(',')[8].Trim().ToUpper(), rowFile);
            ValidateSexo(row.Split(',')[9].Trim().ToUpper(), rowFile);
            ValidateEstadoCivil(row.Split(',')[10].Trim().ToUpper(), rowFile);
            ValidateFechaNacimiento(row.Split(',')[11].Trim(), rowFile);
            ValidateSD(row.Split(',')[12].Trim(), rowFile);
            if (ValidateSDIMMS(row.Split(',')[13].Trim(), rowFile, row.Split(',')[3].Trim()).Equals(Type.Error))
            {
                return Type.Error;
            }
            if (ValidateSDI(row.Split(',')[14].Trim(), rowFile, row.Split(',')[3].Trim()).Equals(Type.Error))
            {
                return Type.Error;
            }
            ValidateIdBancoTradicional(row.Split(',')[15].Trim(), rowFile);
            ValidateCuentaBancariaTrad(row.Split(',')[16].Trim(), rowFile);
            ValidateCuentaInterbancariaTrad(row.Split(',')[17].Trim(), rowFile);

            if (ValidateCURP(row.Split(',')[18].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateRFC(row.Split(',')[19].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateImss(row.Split(',')[20].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }
            ValidateCorreoElectronico(row.Split(',')[21].Trim(), rowFile);

            if (ValidateFechaReconocimientoAntiguedad(row.Split(',')[22].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateFechaAltaIMSS(row.Split(',')[23].Trim(), rowFile, IdUnidadNegocio).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateEsquema(row.Split(',')[24].Trim().ToUpper(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateTipoContrato(row.Split(',')[25].Trim().ToUpper(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }
            ValidateTelefono(row.Split(',')[26].Trim().ToUpper(), rowFile);

            ValidateCalle(row.Split(',')[27].Trim().ToUpper(), rowFile, "Fiscal", 27);
            ValidateNumeroExt(row.Split(',')[28].Trim().ToUpper(), rowFile, "Fiscal", 28);
            ValidateNumeroInt(row.Split(',')[29].Trim().ToUpper(), rowFile, "Fiscal", 29);
            ValidateCodigoPostal(row.Split(',')[30].Trim().ToUpper(), rowFile, "Fiscal", 30);

            ValidateCalle(row.Split(',')[31].Trim().ToUpper(), rowFile, "Personal", 31);
            ValidateNumeroExt(row.Split(',')[32].Trim().ToUpper(), rowFile, "Personal", 32);
            ValidateNumeroInt(row.Split(',')[33].Trim().ToUpper(), rowFile, "Personal", 33);
            ValidateCodigoPostal(row.Split(',')[34].Trim().ToUpper(), rowFile, "Personal", 34);
            ValidateTimbradoNomna(row.Split(',')[44].Trim(), rowFile);

            ValidateIdArea(row.Split(',')[35].Trim().ToUpper(), rowFile);
            ValidateSindicato(row.Split(',')[36].Trim().ToUpper(), rowFile);
            ValidateSucursal(row.Split(',')[37].Trim().ToUpper(), rowFile);
            ValidateJornada(row.Split(',')[38].Trim().ToUpper(), rowFile);
            ValidateNacionalidad(row.Split(',')[39].Trim(), rowFile);
            ValidateFechaTerinoContrato(row.Split(',')[40].Trim(), rowFile);

            return Type.Success;
        }

        /// <summary>
        /// Método par avalidar toda la información para la baja de los empleados
        /// </summary>
        /// <param name="row">linea de texto a analizar</param>
        /// <param name="rowFile"></param>
        /// <returns>Validacíon del archivo</returns>
        public Type ValidateBaja(string row, RowFile rowFile,int IdUnidadNegocio)
        {

            if (ValidateRowBaja(row, rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateClaveEmpleadoBaja(row.Split(',')[0].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            if (ValidateFechaBaja(row.Split(',')[1].Trim(), rowFile, IdUnidadNegocio).Equals(Type.Error))
            {
                return Type.Error;
            }


            if (ValidateMotivoBaja(row.Split(',')[2].Trim(), rowFile).Equals(Type.Error))
            {
                return Type.Error;
            }

            ValidateRecontratable(row.Split(',')[3].Trim(), rowFile);

            return Type.Success;
        }

        /// <summary>
        /// Método par avalidar que el archivo contenga la información necesaria para las bajas
        /// </summary>
        /// <param name="row">información del archivo</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateRowBaja(string row, RowFile rowFile)
        {
            int commas = row.Count(c => c.Equals(','));
            ColumnFile columnFile = new ColumnFile { Column = 1 };

            if (commas.Equals(3))
            {
                rowFile.RowValidation = Type.Success;
                rowFile.RowDetail = "Ok";
                return Type.Success;
            }
            else if (commas < 3)
            {

                rowFile.RowValidation = Type.Error;
                rowFile.RowDetail = "El registro no cumple con los campos requeridos, contiene menos campos, imposible leer línea";
                return Type.Error;
            }
            else
            {
                rowFile.RowValidation = Type.Error;
                rowFile.RowDetail = "El registro no cumple con los campos requeridos, contiene mas campos, imposible leer línea";
                return Type.Error;
            }

        }

        /// <summary>
        /// Método par avalidar que el archivo contenga la información necesaria para las altas
        /// </summary>
        /// <param name="row">información del archivo</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateRow(string row, RowFile rowFile)
        {
            int commas = row.Count(c => c.Equals(','));
            ColumnFile columnFile = new ColumnFile { Column = 1 };


            if (commas.Equals(44))
            {
                rowFile.RowValidation = Type.Success;
                rowFile.RowDetail = "Ok";
                return Type.Success;
            }
            else if (commas < 44)
            {

                rowFile.RowValidation = Type.Error;
                rowFile.RowDetail = "El registro no cumple con los campos requeridos, contiene menos campos, imposible leer línea";
                return Type.Error;
            }
            else
            {
                rowFile.RowValidation = Type.Error;
                rowFile.RowDetail = "El registro no cumple con los campos requeridos, contiene mas campos, imposible leer línea";
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar el Identificador del centro de costos
        /// </summary>
        /// <param name="IdCentroCostos">Identificador del centro de costos</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdCentroCostos(string IdCentroCostos, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 1 };


            if (string.IsNullOrEmpty(IdCentroCostos))
            {
                columnFile.Field = "Identificador Centro de costos";
                columnFile.ColumnDetail = "El campo Identificador Centro de costos es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdCentroCostos);
                    if (CentrosCostos.Exists(x => x.IdCentroCostos.Equals(value)))
                    {
                        columnFile.Field = "Identificador Centro de costos";
                        columnFile.ColumnDetail = "OK";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador Centro de costos";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador Centro de costos no existe en el catálogo de Centros de Costos se sustituye valor a nulo, valor leído: " + IdCentroCostos;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador Centro de Costos";
                    columnFile.ColumnDetail = "El campo Identificador Centro de Costos debe ser valor numérico se sustituye valor a nulo, valor leído: " + IdCentroCostos;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }

        }

        /// <summary>
        /// Método para validar el Identificador del departamento
        /// </summary>
        /// <param name="IdDepartamento">Identificador del departamento</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdDepartamento(string IdDepartamento, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 2 };

            if (string.IsNullOrEmpty(IdDepartamento))
            {
                columnFile.Field = "Identificador de Departamento ";
                columnFile.ColumnDetail = "El campo Identificador de Departamento es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdDepartamento);

                    if (Departamentos.Exists(x => x.IdDepartamento.Equals(value)))
                    {
                        columnFile.Field = "Identificador de Departamento ";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Departamento";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Departamento no existe en el catálogo de Departamentos se sustituye valor a nulo, valor leído: " + IdDepartamento;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Departamento";
                    columnFile.ColumnDetail = "El campo Identificador de Departamento debe ser valor numérico se sustituye valor a nulo, valor leído: " + IdDepartamento;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        //valida el campo IdPuesto
        /// <summary>
        /// Método para validar el Identificador del puesto
        /// </summary>
        /// <param name="IdPuesto">Identificador del puesto</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdPuesto(string IdPuesto, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 3 };

            if (string.IsNullOrEmpty(IdPuesto))
            {
                columnFile.Field = "Identificador de Puesto";
                columnFile.ColumnDetail = "El campo Identificador de Puesto es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdPuesto);

                    if (Puestos.Exists(x => x.IdPuesto.Equals(value)))
                    {
                        columnFile.Field = "Identificador de Puesto";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Puesto";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Puesto no existe en el catálogo de Puestos se sustituye valor a nulo, valor leído: " + IdPuesto;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Puesto";
                    columnFile.ColumnDetail = "El campo Identificador de Puesto debe ser valor numérico se sustituye valor a nulo, valor leído: " + IdPuesto;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        /// <summary>
        /// Método par avalidar el identificador del registro patronal
        /// </summary>
        /// <param name="IdRegistroPatronal">Identificador del registro patronal</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdRegistroPatronal(string IdRegistroPatronal, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 4 };

            if (string.IsNullOrEmpty(IdRegistroPatronal))
            {
                columnFile.Field = "Identificador de Registro Patronal ";
                columnFile.ColumnDetail = "El campo Identificador de Registro Patronal es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdRegistroPatronal);
                    if (RegistrosPatronales.Exists(x => x.IdRegistroPatronal.Equals(value)))
                    {
                        columnFile.Field = "Identificador de Registro Patronal";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Registro Patronal";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Registro Patronal no existe en el catálogo de Registros Patronales se sustituye valor a nulo, valor leído: " + IdRegistroPatronal;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Registro Patronal";
                    columnFile.ColumnDetail = "El campo Identificador de Registro Patronal debe ser valor numérico, valor leído: " + IdRegistroPatronal;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }

        }

        //Valida que IdEntidad no sea nula
        /// <summary>
        /// Método para validar que el Identificador de Entidad no sea nula
        /// </summary>
        /// <param name="IdEntidad">Identificador de Entidad </param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdEntidad(string IdEntidad, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 5 };

            if (string.IsNullOrEmpty(IdEntidad))
            {
                columnFile.Field = "Identificador de Entidad Federativa";
                columnFile.ColumnDetail = "El campo Identificador Entidad Federativa es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdEntidad);
                    if (value > 0 && value < 33)
                    {
                        columnFile.Field = "Identificador de Entidad Federativa";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Entidad Federativa";
                        columnFile.ColumnDetail = "El valor de la Entidad Federativa debe estar entre el rango 1 y 32 , empleado no insertado, valor leído: " + IdEntidad;
                        columnFile.Type = Type.Error;
                        rowFile.Columns.Add(columnFile);
                        return Type.Error;
                    }

                }
                catch
                {
                    columnFile.Field = "Identificador de Entidad Federativa";
                    columnFile.ColumnDetail = "El campo Identificador de Entidad Federativa debe ser valor numérico , empleado no insertado, valor leído: " + IdEntidad;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }

        }

        /// <summary>
        /// Método par avalidar que la clave del empledo no sea nula
        /// </summary>
        /// <param name="ClaveEmpleado">Clave del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateClaveEmpleado(string ClaveEmpleado, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 6 };



            if (string.IsNullOrEmpty(ClaveEmpleado))
            {
                columnFile.Field = "Clave Empleado";
                columnFile.ColumnDetail = "El campo Clave Empleado es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            Regex exp = new Regex(@"^[a-zA-Z0-9]*$");
            if (exp.IsMatch(ClaveEmpleado))
            {
                if (Empleados.Exists(x => x.ClaveEmpleado.Equals(ClaveEmpleado)))
                {
                    columnFile.Field = "Clave Empleado";
                    columnFile.ColumnDetail = "Ya existe un empleado con la Clave Empleado ingresada, empleado no insertado, valor leído: " + ClaveEmpleado;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
                else
                {
                    columnFile.Field = "Clave Empleado";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
            }
            else
            {
                columnFile.Field = "Clave Empleado";
                columnFile.ColumnDetail = "El campo Clave Empleado solo puede contener números y letras, empleado no insertado, valor leído: " + ClaveEmpleado;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método par avalidar que la clave del empledo no sea nula
        /// </summary>
        /// <param name="ClaveEmpleado">Clave del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateClaveEmpleadoBaja(string ClaveEmpleado, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 1 };

            if (string.IsNullOrEmpty(ClaveEmpleado))
            {
                columnFile.Field = "Clave Empleado";
                columnFile.ColumnDetail = "El campo Clave Empleado es requerido, no se realiza baja.";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            Regex exp = new Regex(@"^[a-zA-Z0-9]*$");
            if (exp.IsMatch(ClaveEmpleado))
            {
                if (EmpleadosAlta.Exists(x => x.ClaveEmpleado.Equals(ClaveEmpleado)))
                {
                    columnFile.Field = "Clave Empleado";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Clave Empleado";
                    columnFile.ColumnDetail = "No existe empleado con la Clave Empleado ingresada, no se realiza baja, valor leído: " + ClaveEmpleado;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            else
            {
                columnFile.Field = "Clave Empleado";
                columnFile.ColumnDetail = "El campo Clave Empleado solo puede contener números y letras, no se realiza baja, valor leído: " + ClaveEmpleado;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que en el registro contenga el campo de nombre
        /// </summary>
        /// <param name="Nombre">Nombre del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateNombre(string Nombre, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 7 };

            if (string.IsNullOrEmpty(Nombre))
            {
                columnFile.Field = "Nombre";
                columnFile.ColumnDetail = "El campo Nombre es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            Regex exp = new Regex(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ ]*$");

            if (exp.IsMatch(Nombre))
            {
                columnFile.Field = "Nombre";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Nombre";
                columnFile.ColumnDetail = "El campo Nombre solo puede contener Letras, empleado no insertado valor leído: " + Nombre;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo de apellido paterno
        /// </summary>
        /// <param name="ApellidoPaterno">Apellido paterno del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateApellidoPaterno(string ApellidoPaterno, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 8 };

            if (string.IsNullOrEmpty(ApellidoPaterno))
            {
                columnFile.Field = "Apellido Paterno";
                columnFile.ColumnDetail = "El campo Apellido Paterno es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            Regex exp = new Regex(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ ]*$");

            if (exp.IsMatch(ApellidoPaterno))
            {
                columnFile.Field = "Apellido Paterno";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Apellido Paterno";
                columnFile.ColumnDetail = "El campo Apellido Paterno solo puede contener Letras, empleado no insertado, valor leído: " + ApellidoPaterno;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

        }

        /// <summary>
        /// Método para validar que el registro contenga el campo de apellido materno
        /// </summary>
        /// <param name="ApellidoMaterno">Apellido materno del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateApellidoMaterno(string ApellidoMaterno, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 9 };

            if (string.IsNullOrEmpty(ApellidoMaterno))
            {
                columnFile.Field = "Apellido Materno";
                columnFile.ColumnDetail = "El campo Apellido Materno es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ ]*$");

            if (exp.IsMatch(ApellidoMaterno))
            {
                columnFile.Field = "Apellido Materno";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Apellido Materno";
                columnFile.ColumnDetail = "El campo Apellido Materno solo puede contener Letras se sustituye valor a nulo, valor leído: " + ApellidoMaterno;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }

        }

        /// <summary>
        /// Método para validar que el registro conteng el campo sexo correctamen
        /// </summary>
        /// <param name="sexo">sexo del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateSexo(string sexo, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 10 };

            if (string.IsNullOrEmpty(sexo))
            {
                columnFile.Field = "Sexo";
                columnFile.ColumnDetail = "El campo Sexo es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            if (sexo.Equals("MASCULINO") || sexo.Equals("FEMENINO"))
            {
                columnFile.Field = "Sexo";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Sexo";
                columnFile.ColumnDetail = "El campo Sexo solo puede contener la palabra \"MASCULINO\" o \"FEMENINO\" se sustituye valor a nulo, valor leído: " + sexo;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }

        }

        /// <summary>
        /// Método para validar que el registro contenga el campo estado civil correctamente
        /// </summary>
        /// <param name="EstadoCivil">Estado civil del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateEstadoCivil(string EstadoCivil, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 11 };

            if (string.IsNullOrEmpty(EstadoCivil))
            {
                columnFile.Field = "Estado Civil";
                columnFile.ColumnDetail = "El campo Estado Civil es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            if (EstadoCivil.Equals("SOLTERO") || EstadoCivil.Equals("CASADO"))
            {
                columnFile.Field = "Estado Civil";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Estado Civil";
                columnFile.ColumnDetail = "El campo Estado Civil solo puede contener la palabra \"SOLTERO\" o \"CASADO\" se sustituye valor a nulo,  valor leído: " + EstadoCivil;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;

            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo Fecha de nacimiento
        /// </summary>
        /// <param name="FechaDeNaciemiento">Fecha de nacimiento del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateFechaNacimiento(string FechaDeNaciemiento, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 12 };

            if (string.IsNullOrEmpty(FechaDeNaciemiento))
            {
                columnFile.Field = "Fecha de Nacimiento";
                columnFile.ColumnDetail = "El campo Fecha de Nacimiento es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            try
            {
                DateTime? date = null;
                date = Convert.ToDateTime(FechaDeNaciemiento);

                Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                if (date != null && exp.IsMatch(FechaDeNaciemiento))
                {
                    columnFile.Field = "Fecha de Nacimiento";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }else
                {
                    columnFile.Field = "Fecha de Naciemiento";
                    columnFile.ColumnDetail = "El campo Fecha de Nacimiento debe tener el formato correcto \"dd/mm/aaaa\" se sustituye valor a nulo, valor leído: " + FechaDeNaciemiento;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
            catch
            {
                columnFile.Field = "Fecha de Naciemiento";
                columnFile.ColumnDetail = "El campo Fecha de Nacimiento es incorrecta, valor leído: " + FechaDeNaciemiento;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo SD
        /// </summary>
        /// <param name="SD">Salario diario del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateSD(string SD, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 13 };

            if (string.IsNullOrEmpty(SD))
            {
                columnFile.Field = "Sueldo Diario";
                columnFile.ColumnDetail = "El campo Sueldo Diario es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            try
            {
                double value = Convert.ToDouble(SD);
                columnFile.Field = "Sueldo Diario";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            catch
            {
                columnFile.Field = "Sueldo Diario";
                columnFile.ColumnDetail = "El campo Sueldo Diario debe de ser valor numérico decimal se sustituye valor a 0 (cero), valor leído: " + SD;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo SDIMSS
        /// </summary>
        /// <param name="SDIMSS">Salario diario integrado</param>
        /// <param name="rowFile">Fila del archivo txt<</param>
        /// <param name="idregistropatronal">Identificador del registro patronal</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateSDIMMS(string SDIMSS, RowFile rowFile, string idregistropatronal)
        {
            ClassEmpleado ce = new ClassEmpleado();
            ColumnFile columnFile = new ColumnFile { Column = 14 };

            if (string.IsNullOrEmpty(idregistropatronal))
            {
                if (string.IsNullOrEmpty(SDIMSS))
                {
                    columnFile.Field = "Sueldo Diario IMSS";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Sueldo Diario IMSS";
                    columnFile.ColumnDetail = "El Salario Diario debe estar vacio al no pertener a ningun registro patronal";
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            try
            {
                decimal sdimss = Convert.ToDecimal(SDIMSS);
                decimal salariominimo = ce.ObtenSMV();
                if (sdimss >= salariominimo)
                {

                    double value = Convert.ToDouble(SDIMSS);
                    columnFile.Field = "Sueldo Diario IMSS";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;

                }
                else
                {
                    columnFile.Field = "Sueldo Diario IMSS";
                    columnFile.ColumnDetail = "El campo Sueldo Diario IMSS debe de ser mayor o igual al Salario Minimo General vigente, valor leído: " + SDIMSS;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            catch
            {
                columnFile.Field = "Sueldo Diario IMSS";
                columnFile.ColumnDetail = "El campo Sueldo Diario IMSS debe de ser valor numérico decimal se sustituye valor a 0 (cero), valor leído: " + SDIMSS;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo SDI
        /// </summary>
        /// <param name="SDI">Salario diario integrado</param>
        /// <param name="rowFile">Fila del archivo txt<</param>
        /// <param name="idregistropatronal">Identificador del registro patronal</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateSDI(string SDI, RowFile rowFile, string idregistropatronal)
        {
            ColumnFile columnFile = new ColumnFile { Column = 15 };

            if (string.IsNullOrEmpty(idregistropatronal))
            {
                if (string.IsNullOrEmpty(SDI))
                {
                    columnFile.Field = "Sueldo Diario Integrado";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Sueldo Diario Integrado";
                    columnFile.ColumnDetail = "El campo Sueldo Diario Integrado debe estar vacio debido a que no pertenece a ningun registro patronal";
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            try
            {
                double value = Convert.ToDouble(SDI);
                columnFile.Field = "Sueldo Diario Integrado";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            catch
            {
                columnFile.Field = "Sueldo Diario Integrado";
                columnFile.ColumnDetail = "El campo Sueldo Diario Integrado debe de ser valor numérico decimal se sustituye valor a 0 (cero), valor leído: " + SDI;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

        }

        /// <summary>
        /// Método para validar que el registro contenga el campo IdBancoTradicional
        /// </summary>
        /// <param name="IdBancoTradicional">Identificador del banco</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdBancoTradicional(string IdBancoTradicional, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 16 };

            if (string.IsNullOrEmpty(IdBancoTradicional))
            {
                columnFile.Field = "Identificador de Banco de Tradicional";
                columnFile.ColumnDetail = "El campo Identificador de Banco Tradicional es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdBancoTradicional);

                    if (Bancos.Exists(x => x.IdBanco.Equals(value)))
                    {
                        columnFile.Field = "Identificador de Banco de Tradicional";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Banco Tradicional";
                        columnFile.ColumnDetail = "El campo Identificador de Banco Tradicional no existe en el catálogo de bancos se sustituye valor a nulo, valor leído: " + IdBancoTradicional;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Banco Tradicional";
                    columnFile.ColumnDetail = "El campo Identificador de Banco Tradicional debe ser valor numérico se sustituye valor a nulo, valor leído: " + IdBancoTradicional;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo CuentaBancariaTrad
        /// </summary>
        /// <param name="CuentaBancariaTrad">Cuenta bancaria del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateCuentaBancariaTrad(string CuentaBancariaTrad, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 17 };

            if (string.IsNullOrEmpty(CuentaBancariaTrad))
            {
                columnFile.Field = "Cuenta Bancaria Tradicional";
                columnFile.ColumnDetail = "El campo Cuenta Bancaria Tradicional es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"^[0-9]*$");

            if (exp.IsMatch(CuentaBancariaTrad))
            {
                columnFile.Field = "Cuenta Bancaria Tradicional";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Cuenta Bancaria Tradicional";
                columnFile.ColumnDetail = "El campo Cuenta Bancaria Tradicional solo puede contener números se sustituye valor a nulo, valor leído: " + CuentaBancariaTrad;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo CuentaInterbancariaTrad
        /// </summary>
        /// <param name="CuentaInterbancariaTrad">Clave interbancaria del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateCuentaInterbancariaTrad(string CuentaInterbancariaTrad, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 18 };

            if (string.IsNullOrEmpty(CuentaInterbancariaTrad))
            {
                columnFile.Field = "Cuenta Interbancaria Tradicional";
                columnFile.ColumnDetail = "El campo Cuenta Interbancaria Tradicional es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"^[0-9]*$");

            if (exp.IsMatch(CuentaInterbancariaTrad))
            {
                columnFile.Field = "Cuenta Interbancaria Tradicional";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Cuenta Interbancaria Tradicional";
                columnFile.ColumnDetail = "El campo Cuenta Interbancaria Tradicional solo puede contener números se sustituye valor a nulo, valor leído: " + CuentaInterbancariaTrad;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }


        }

        /// <summary>
        /// Método para validar que el registro contenga el campo CURP
        /// </summary>
        /// <param name="CURP">CURP del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateCURP(string CURP, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 19 };
            if (string.IsNullOrEmpty(CURP))
            {
                columnFile.Field = "CURP";
                columnFile.ColumnDetail = "El campo CURP es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            Regex exp = new Regex(@"^.*(?=.{18})(?=.*[0-9])(?=.*[A-ZÑ]).*$");

            if (exp.IsMatch(CURP))
            {
                if (Empleados.Exists(x => x.Curp.Equals(CURP)))
                {
                    columnFile.Field = "CURP";
                    columnFile.ColumnDetail = "Ya existe un empleado con la Clave Curp ingresada, empleado no insertado, valor leído: " + CURP;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
                else
                {
                    columnFile.Field = "CURP";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;

                }

            }
            else
            {
                columnFile.Field = "CURP";
                columnFile.ColumnDetail = "El campo CURP debe tener el formato correcto, empleado no insertado, valor leído: " + CURP;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

        }

        /// <summary>
        /// Método para validar que el registro contenga el campo RFC
        /// </summary>
        /// <param name="RFC">RFC del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateRFC(string RFC, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 20 };

            if (string.IsNullOrEmpty(RFC))
            {
                columnFile.Field = "RFC";
                columnFile.ColumnDetail = "El campo RFC es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            Regex exp = new Regex(@"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([A-Z\d]{3})?$");

            if (exp.IsMatch(RFC) || RFC.Length == 13)
            {
                if (Empleados.Exists(x => x.Rfc.Equals(RFC)))
                {
                    columnFile.Field = "RFC";
                    columnFile.ColumnDetail = "Ya existe un empleado con la Clave RFC ingresada, empleado no insertado, valor leído: " + RFC;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
                else
                {
                    columnFile.Field = "RFC";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;

                }

            }
            else
            {
                columnFile.Field = "RFC";
                columnFile.ColumnDetail = "El campo RFC no cumple con el formato correcto, empleado no insertado, valor leído: " + RFC;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

        }

        /// <summary>
        /// Método para validar que el registro contenga el campo Imss
        /// </summary>
        /// <param name="Imss">Numero de seguro social del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateImss(string Imss, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 21 };

            if (string.IsNullOrEmpty(Imss))
            {
                columnFile.Field = "Número de IMSS";
                columnFile.ColumnDetail = "El campo Número de IMSS es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"^[0-9]*$");

            if (Imss.Length == 11)
            {

                if (exp.IsMatch(Imss))
                {
                    columnFile.Field = "Número de IMSS";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Número de IMSS";
                    columnFile.ColumnDetail = "El campo Número de IMSS solo puede contener números, valor leído: " + Imss;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            else
            {
                columnFile.Field = "Número de IMSS";
                columnFile.ColumnDetail = "El campo Número de IMSS solo puede contener números y la longitud debe de ser de 11 dígitos se sustituye valor a nulo, valor leído: " + Imss;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo correo electronico
        /// </summary>
        /// <param name="CorreoElectronico">Correo electronico del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateCorreoElectronico(string CorreoElectronico, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 22 };

            if (string.IsNullOrEmpty(CorreoElectronico))
            {
                columnFile.Field = "Correo Electrónico";
                columnFile.ColumnDetail = "El campo Correo Electrónico es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            if (exp.IsMatch(CorreoElectronico))
            {
                columnFile.Field = "Correo Electrónico";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Correo Electrónico";
                columnFile.ColumnDetail = "El campo Correo Electrónico debe tener el formato correcto ej. \"user@mail.com\" se sustituye valor a nulo, valor leído: " + CorreoElectronico;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo Fecha de Reconocimiento de Antigüeda
        /// </summary>
        /// <param name="FechaReconocimientoAntiguedad">Fecha de reconocimiento de antigüedad del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateFechaReconocimientoAntiguedad(string FechaReconocimientoAntiguedad, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 23 };

            if (string.IsNullOrEmpty(FechaReconocimientoAntiguedad))
            {
                columnFile.Field = "Fecha de Reconocimiento de Antigüedad";
                columnFile.ColumnDetail = "El campo Fecha de Reconocimiento de Antigüedad es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            try
            {
                DateTime? date = null;
                date = Convert.ToDateTime(FechaReconocimientoAntiguedad);

                Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                if (exp.IsMatch(FechaReconocimientoAntiguedad))
                {
                    columnFile.Field = "Fecha de Reconocimiento de Antigüedad";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Fecha de Reconocimiento de Antigüedad";
                    columnFile.ColumnDetail = "El campo Fecha de Reconocimiento de Antigüedad debe tener el formato correcto \"dd/mm/aaaa\", empleado no insertado, valor leído: " + FechaReconocimientoAntiguedad;
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            catch (Exception)
            {
                columnFile.Field = "Fecha de Reconocimiento de Antigüedad";
                columnFile.ColumnDetail = "El campo Fecha de Reconocimiento de Antigüedad es incorrecta, valor leído: " + FechaReconocimientoAntiguedad;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo Fecha de FechaAltaImss
        /// </summary>
        /// <param name="FechaAltaImss">Fecha de alta ante el imss del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <param name="IdUnidadNegocio">Identificador unico de la Unidad de Negocio (debera ser obtenida de la variable de session: ["sIdUnidad"]</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateFechaAltaIMSS(string FechaAltaImss, RowFile rowFile, int IdUnidadNegocio)
        {
            ColumnFile columnFile = new ColumnFile { Column = 24 };

            if (string.IsNullOrEmpty(FechaAltaImss))
            {
                columnFile.Field = "Fecha de Alta IMSS";
                columnFile.ColumnDetail = "El campo Fecha de Alta IMSS es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            DateTime FechaFinal = DateTime.Today;
            DateTime FechaInicial = Convert.ToDateTime(FechaAltaImss);
            if (FechaInicial <= FechaFinal)
            {
                //La siguente variable es para obtener los dias configurados por el cliente 
                int diasConfigurados = DiasConfiguradosUnidadNegocio(IdUnidadNegocio);

                //En la siguente condicion se valida que si el valor de diasConfigurados es igyual a 0 se admite cualquier fecha sin restriccion alguna, solo se validara que la fecha cuente con el formato correcto
                if (diasConfigurados == 0)
                {
                    Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                    if (exp.IsMatch(FechaAltaImss))
                    {
                        columnFile.Field = "Fecha de Alta IMSS";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Alta IMSS";
                        columnFile.ColumnDetail = "El campo Fecha de Alta IMSS debe tener el formato correcto \"dd/mm/aaaa\" se sustituye valor a nulo, valor leído: " + FechaAltaImss;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }

                if (GetDiasHabiles(FechaInicial, FechaFinal) <= diasConfigurados)
                {
                    Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                    if (exp.IsMatch(FechaAltaImss))
                    {
                        columnFile.Field = "Fecha de Alta IMSS";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Alta IMSS";
                        columnFile.ColumnDetail = "El campo Fecha de Alta IMSS debe tener el formato correcto \"dd/mm/aaaa\" se sustituye valor a nulo, valor leído: " + FechaAltaImss;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                else
                {
                    columnFile.Field = "Fecha de Alta IMSS";
                    columnFile.ColumnDetail = $"La fecha de alta ante el IMSS excede los {diasConfigurados} dias habiles extemporaneos";
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
            else
            {
                if (GetDiasAdelatados(FechaInicial, FechaFinal) == 1)
                {
                    Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                    if (exp.IsMatch(FechaAltaImss))
                    {
                        columnFile.Field = "Fecha de Alta IMSS";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Alta IMSS";
                        columnFile.ColumnDetail = "El campo Fecha de Alta IMSS debe tener el formato correcto \"dd/mm/aaaa\" se sustituye valor a nulo, valor leído: " + FechaAltaImss;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                else
                {
                    columnFile.Field = "Fecha de Alta IMSS";
                    columnFile.ColumnDetail = "La fecha de alta ante el IMSS excede el dia anticipado";
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
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
        /// Método para validar que el registro contenga el campo esquema
        /// </summary>
        /// <param name="Esquema">Esquema del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateEsquema(string Esquema, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 25 };

            if (string.IsNullOrEmpty(Esquema))
            {
                columnFile.Field = "Esquema";
                columnFile.ColumnDetail = "El campo Esquema es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            if (Esquema.Equals("100% ESQUEMA") || Esquema.Equals("100% TRADICIONAL") || Esquema.Equals("MIXTO"))
            {
                columnFile.Field = "Esquema";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Esquema";
                columnFile.ColumnDetail = "El campo Esquema solo puede contener la palabra \"100% ESQUEMA\" o \"100% TRADICIONAL\" o \"MIXTO\", empleado no insertado, valor leído: " + Esquema;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo tipo de contrato
        /// </summary>
        /// <param name="TipoContrato">Tipo de contrato del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateTipoContrato(string TipoContrato, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 26 };

            if (string.IsNullOrEmpty(TipoContrato))
            {
                columnFile.Field = "Tipo de Contrato";
                columnFile.ColumnDetail = "El campo Tipo de Contrato es requerido, empleado no insertado";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            if (TipoContrato.Equals("DETERMINADO") || TipoContrato.Equals("INDETERMINADO") || TipoContrato.Equals("HONORARIOS") || TipoContrato.Equals("RESICO"))
            {
                columnFile.Field = "Tipo de Contrato";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Tipo de Contrato";
                columnFile.ColumnDetail = "El campo Tipo de Contrato solo puede contener la palabra \"DETERMINADO\" o \"INDETERMINADO\", empleado no insertado, valor leído: " + TipoContrato;
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo tipo de RFCSUBCONTRACION
        /// </summary>
        /// <param name="RFCSubcontracion">RFC de subciontratación del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateRFCSub(string RFCSubcontracion, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 27 };

            if (string.IsNullOrEmpty(RFCSubcontracion))
            {
                columnFile.Field = "RFC Subcontración";
                columnFile.ColumnDetail = "El campo RFC Subcontración es nulo.";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([A-Z\d]{3})?$");

            if (exp.IsMatch(RFCSubcontracion))
            {
                columnFile.Field = "RFC Subcontración";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "RFC Subcontración";
                columnFile.ColumnDetail = "El campo RFC Subcontración no cumple con el formato correcto, valor sustituido por nulo: " + RFCSubcontracion;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }

        }

        /// <summary>
        /// Método para validar que el registro contenga el campo calle
        /// </summary>
        /// <param name="calle">Calle del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateCalle(string calle, RowFile rowFile, string tipoDireccion, int columna)
        {
            ColumnFile columnFile = new ColumnFile { Column = columna };

            if (string.IsNullOrEmpty(calle))
            {
                columnFile.Field = "Calle " + tipoDireccion;
                columnFile.ColumnDetail = "El campo Calle " + tipoDireccion + " es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            columnFile.Field = "Calle " + tipoDireccion;
            columnFile.ColumnDetail = "Ok";
            columnFile.Type = Type.Success;
            rowFile.Columns.Add(columnFile);
            return Type.Success;
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo Numero exterior
        /// </summary>
        /// <param name="NumeroExt">Numero exterior del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateNumeroExt(string NumeroExt, RowFile rowFile, string tiopDireccion, int columna)
        {
            ColumnFile columnFile = new ColumnFile { Column = columna };

            if (string.IsNullOrEmpty(NumeroExt))
            {
                columnFile.Field = "Número Exterior " + tiopDireccion;
                columnFile.ColumnDetail = "El campo Número Exterior " + tiopDireccion + " es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            columnFile.Field = "Número Exterior " + tiopDireccion;
            columnFile.ColumnDetail = "Ok";
            columnFile.Type = Type.Success;
            rowFile.Columns.Add(columnFile);
            return Type.Success;
        }

        /// <summary>
        /// Método para validar que el registro contenga el campo Numero interior
        /// </summary>
        /// <param name="NumeroInt">Numero interior del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateNumeroInt(string NumeroInt, RowFile rowFile, string tiopDireccion, int columna)
        {
            ColumnFile columnFile = new ColumnFile { Column = columna };

            if (string.IsNullOrEmpty(NumeroInt))
            {
                columnFile.Field = "Número Interior " + tiopDireccion;
                columnFile.ColumnDetail = "El campo Número Interior " + tiopDireccion + " es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            columnFile.Field = "Número Interior " + tiopDireccion;
            columnFile.ColumnDetail = "Ok";
            columnFile.Type = Type.Success;
            rowFile.Columns.Add(columnFile);
            return Type.Success;
        }
        #endregion

        /// <summary>
        /// Método para validar el codigo postal
        /// </summary>
        /// <param name="cp">Codigo postal</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateCodigoPostal(string cp, RowFile rowFile, string tipoDireccion, int columna)
        {
            ColumnFile columnFile = new ColumnFile { Column = columna };

            if (string.IsNullOrWhiteSpace(cp))
            {
                columnFile.Field = "Código Postal " + tipoDireccion;
                columnFile.ColumnDetail = "El campo Código Postal " + tipoDireccion + " es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {

                Regex exp = new Regex(@"^([1-9]{2}|[0-9][1-9]|[1-9][0-9])[0-9]{3}");

                var _cp = cp.Replace(" ", "");
                if (exp.IsMatch(_cp))
                {
                    ClassEmpleado service = new ClassEmpleado();
                    var cps = service.GetCodigoPostalesByString(_cp);
                    int? value = null;
                    try { value = cps.Find(x => x.Codigo.Equals(_cp)).Id; } catch { }

                    if (value != null)
                    {
                        columnFile.Field = "Código Postal " + tipoDireccion;
                        columnFile.ColumnDetail = "OK";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Código Postal " + tipoDireccion;
                        columnFile.ColumnDetail = "El campo Código Postal " + tipoDireccion + " no existe se sustituye valor a nulo, valor leído: " + cp;
                        columnFile.Type = Type.Warning;
                        rowFile.Columns.Add(columnFile);
                        return Type.Warning;
                    }
                }
                else
                {
                    columnFile.Field = "Código Postal " + tipoDireccion;
                    columnFile.ColumnDetail = "El campo Código Postal " + tipoDireccion + " debe contener solo números se sustituye valor a nulo, valor leído: " + cp;
                    columnFile.Type = Type.Warning;
                    rowFile.Columns.Add(columnFile);
                    return Type.Warning;
                }
            }
        }

        /// <summary>
        /// Método para validar el área
        /// </summary>
        /// <param name="Idarea">Identificador del area</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateIdArea(string Idarea, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 36 };

            if (string.IsNullOrEmpty(Idarea))
            {
                columnFile.Field = "Identificador de Departamento ";
                columnFile.ColumnDetail = "El campo Identificador de Departamento es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(Idarea);

                    if (Areas.Exists(x => x.IdArea.Equals(value)))
                    {
                        columnFile.Field = "Identificador de Area ";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Area ";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Departamento no existe en el catálogo de Areas se sustituye valor a nulo, valor leído: " + Idarea;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Area ";
                    columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Departamento no existe en el catálogo de Areas se sustituye valor a nulo, valor leído: " + Idarea;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        /// <summary>
        /// Método para validar el sindicato
        /// </summary>
        /// <param name="idSindicato">Identificador del sindicato</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateSindicato(string idSindicato, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 37 };

            if (string.IsNullOrEmpty(idSindicato))
            {
                columnFile.Field = "Identificador de Sindicato ";
                columnFile.ColumnDetail = "El campo Identificador de Sindicato es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(idSindicato);

                    if (Sindicatos.Exists(x => x.IdSindicato.Equals(value)))
                    {
                        columnFile.Field = "Identificador de Sindicato ";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Sindicato ";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Sindicato no existe en el catálogo de Areas se sustituye valor a nulo, valor leído: " + idSindicato;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Area ";
                    columnFile.ColumnDetail = "El valor introducido en el campo Identificador de idsindicato no existe en el catálogo de Sindicatos se sustituye valor a nulo, valor leído: " + idSindicato;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        /// <summary>
        /// Método para validar la sucursal
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateSucursal(string idSucursal, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 38 };

            if (string.IsNullOrEmpty(idSucursal))
            {
                columnFile.Field = "Identificador de Sucursal";
                columnFile.ColumnDetail = "El campo Identificador de Sucursal es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(idSucursal);

                    if (Sucursales.Exists(x => x.IdSucursal == value))
                    {
                        columnFile.Field = "Identificador de Sucursal";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Sucursal";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Sucursal no existe en el catálogo de Sucursales o no peretence al cliente se sustituye valor a nulo, valor leído: " + idSucursal;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Sucursal";
                    columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Sucursal no es un valor numerico se sustituye valor a nulo, valor leído: " + idSucursal;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        /// <summary>
        /// Método para validar la jornada
        /// </summary>
        /// <param name="idJornada">Identificador de la jornada</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateJornada(string idJornada, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 39 };

            if (string.IsNullOrEmpty(idJornada))
            {
                columnFile.Field = "Identificador de Jornada";
                columnFile.ColumnDetail = "El campo Identificador de Jornada es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(idJornada);

                    if (Jornadas.Exists(x => x.Value == value.ToString()))
                    {
                        columnFile.Field = "Identificador de Jornada";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Identificador de Jornada";
                        columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Jornada no existe en el catálogo de Jornadas o no peretence al cliente se sustituye valor a nulo, valor leído: " + idJornada;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                catch
                {
                    columnFile.Field = "Identificador de Jornada";
                    columnFile.ColumnDetail = "El valor introducido en el campo Identificador de Jornada no es un valor numerico se sustituye valor a nulo, valor leído: " + idJornada;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        //Valida el campo fecha de baja
        /// <summary>
        /// Método para validar la fecha de baja del empleado
        /// </summary>
        /// <param name="FechaBaja">Fecha de baja del empleado</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateFechaBaja(string FechaBaja, RowFile rowFile, int IdUnidadNegocio)
        {
            ColumnFile columnFile = new ColumnFile { Column = 2 };

            if (string.IsNullOrEmpty(FechaBaja))
            {
                columnFile.Field = "Fecha de Baja";
                columnFile.ColumnDetail = "El campo Fecha de Baja es requerido, baja no realizada.";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }

            DateTime FechaFinal = DateTime.Today;//Validacion de los 5 dias habiles para bajas DRR
            DateTime FechaInicial = Convert.ToDateTime(FechaBaja);//Validacion de los 5 dias habiles para bajas DRR
            ClassUnidadesNegocio Unidad = new ClassUnidadesNegocio();
            var dias = Unidad.getUnidadesnegocioId(IdUnidadNegocio);

            if (dias.DiasMenosImss != null && (dias.DiasMenosImss >= 0 || string.IsNullOrEmpty(dias.DIasImss.ToString())))
            {
                if (dias.DiasMenosImss == 0)
                {
                    Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                    if (exp.IsMatch(FechaBaja))
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "El campo Fecha de Baja debe tener el formato correcto \"dd/mm/aaaa\", baja no realizada, valor leído: " + FechaBaja;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }

                }

                else
                {

                    if (FechaInicial > FechaFinal)
                    {

                        if (GetDiasAdelatados(FechaInicial, FechaFinal) <= 1)
                        {
                            Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                            if (exp.IsMatch(FechaBaja))
                            {
                                columnFile.Field = "Fecha de Baja";
                                columnFile.ColumnDetail = "Ok";
                                columnFile.Type = Type.Success;
                                rowFile.Columns.Add(columnFile);
                                return Type.Success;
                            }
                            else
                            {
                                columnFile.Field = "Fecha de Baja";
                                columnFile.ColumnDetail = "El campo Fecha de Baja debe tener el formato correcto \"dd/mm/aaaa\", baja no realizada, valor leído: " + FechaBaja;
                                columnFile.Type = Type.Invalid;
                                rowFile.Columns.Add(columnFile);
                                return Type.Invalid;
                            }
                        }
                        else
                        {
                            columnFile.Field = "Fecha de Baja";
                            columnFile.ColumnDetail = "El campo de Fecha Baja no debe de exceder de los 5 dias habiles";
                            columnFile.Type = Type.Error;
                            rowFile.Columns.Add(columnFile);
                            return Type.Error;//Validacion de los 5 dias habiles para bajas DRR
                        }

                    }


                    else if (GetDiasHabiles(FechaInicial, FechaFinal) <= dias.DiasMenosImss)
                    {
                        Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                        if (exp.IsMatch(FechaBaja))
                        {
                            columnFile.Field = "Fecha de Baja";
                            columnFile.ColumnDetail = "Ok";
                            columnFile.Type = Type.Success;
                            rowFile.Columns.Add(columnFile);
                            return Type.Success;
                        }
                        else
                        {
                            columnFile.Field = "Fecha de Baja";
                            columnFile.ColumnDetail = "El campo Fecha de Baja debe tener el formato correcto \"dd/mm/aaaa\", baja no realizada, valor leído: " + FechaBaja;
                            columnFile.Type = Type.Invalid;
                            rowFile.Columns.Add(columnFile);
                            return Type.Invalid;
                        }
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "El campo de Fecha Baja no debe de exceder de los 5 dias habiles";
                        columnFile.Type = Type.Error;
                        rowFile.Columns.Add(columnFile);
                        return Type.Error;//Validacion de los 5 dias habiles para bajas DRR
                    }


                }
            }



             else   if (FechaInicial <= FechaFinal)
            {
                if (GetDiasHabiles(FechaInicial, FechaFinal) <= 5)//Validacion de los 5 dias habiles para bajas DRR
                {
                    Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                    if (exp.IsMatch(FechaBaja))
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "El campo Fecha de Baja debe tener el formato correcto \"dd/mm/aaaa\", baja no realizada, valor leído: " + FechaBaja;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                else//Validacion de los 5 dias habiles para bajas DRR
                {
                    columnFile.Field = "Fecha de Baja";
                    columnFile.ColumnDetail = "El campo de Fecha Baja no debe de exceder de los 5 dias habiles";
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;//Validacion de los 5 dias habiles para bajas DRR
                }
            }
            else
            {
                if (GetDiasAdelatados(FechaInicial, FechaFinal) <= 1)
                {
                    Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                    if (exp.IsMatch(FechaBaja))
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "Ok";
                        columnFile.Type = Type.Success;
                        rowFile.Columns.Add(columnFile);
                        return Type.Success;
                    }
                    else
                    {
                        columnFile.Field = "Fecha de Baja";
                        columnFile.ColumnDetail = "El campo Fecha de Baja debe tener el formato correcto \"dd/mm/aaaa\", baja no realizada, valor leído: " + FechaBaja;
                        columnFile.Type = Type.Invalid;
                        rowFile.Columns.Add(columnFile);
                        return Type.Invalid;
                    }
                }
                else
                {
                    columnFile.Field = "Fecha de Baja";
                    columnFile.ColumnDetail = "El campo de Fecha Baja no debe de exceder a 1 dias adelantado";
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
        }

        //Valida 
        /// <summary>
        /// Método para validar el motivo de baja
        /// </summary>
        /// <param name="motivoBaja">Motivo de baja</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateMotivoBaja(string motivoBaja, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 3 };

            if (string.IsNullOrEmpty(motivoBaja))
            {
                columnFile.Field = "Motivo Baja";
                columnFile.ColumnDetail = "El campo Motivo Baja es requerido, baja no realizada.";
                columnFile.Type = Type.Error;
                rowFile.Columns.Add(columnFile);
                return Type.Error;
            }
            else
            {
                if (motivoBaja == "TERMINO DE CONTRATO" || motivoBaja == "SEPARACION VOLUNTARIA" || motivoBaja == "ABANDONO DE EMPLEO" || motivoBaja == "DEFUNCION" || motivoBaja == "CLAUSURA" || motivoBaja == "OTRA" || motivoBaja == "AUSENTISMO" || motivoBaja == "RESCISION DE CONTRATO" || motivoBaja == "JUBILACION" || motivoBaja == "PENSION")//unificar el motivo ed bajas DRR
                {
                    columnFile.Field = "Motivo Baja";//unificar el motivo ed bajas DRR
                    columnFile.ColumnDetail = "Ok";//unificar el motivo ed bajas DRR
                    columnFile.Type = Type.Success;//unificar el motivo ed bajas DRR
                    rowFile.Columns.Add(columnFile);//unificar el motivo ed bajas DRR
                    return Type.Success;//unificar el motivo ed bajas DRR
                }
                else
                {
                    columnFile.Field = "Motivo Baja";
                    columnFile.ColumnDetail = "El campo Motivo Baja debe ser igual a las aceptadas por el IMSS, baja no realizada.";//unificar el motivo ed bajas DRR
                    columnFile.Type = Type.Error;
                    rowFile.Columns.Add(columnFile);
                    return Type.Error;
                }
            }
        }

        public Type ValidateTelefono(string NumeroTelefónico, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 27 };

            if (string.IsNullOrEmpty(NumeroTelefónico))
            {
                columnFile.Field = "Número telefónico";
                columnFile.ColumnDetail = "El campo Número telefónico es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            Regex exp = new Regex(@"^[0-9]*$");

            if (exp.IsMatch(NumeroTelefónico))
            {
                columnFile.Field = "Número telefónico";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                string numero = NumeroTelefónico.Replace("-", "").Replace(" ", "").Replace("/", "");
                if (exp.IsMatch(numero))
                {
                    columnFile.Field = "Número telefónico";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Número telefónico";
                    columnFile.ColumnDetail = "El campo Número telefónico solo puede contener números se sustituye valor a nulo, valor leído: " + NumeroTelefónico;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }
            }
        }

        /// <summary>
        /// Método para validar el compo de recontratable
        /// </summary>
        /// <param name="Recontratable">Si o No</param>
        /// <param name="rowFile">Fila del archivo txt</param>
        /// <returns>Texto par al afila del archivo txt</returns>
        public Type ValidateRecontratable(string Recontratable, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 4 };

            if (string.IsNullOrEmpty(Recontratable))
            {
                columnFile.Field = "Recontratable";
                columnFile.ColumnDetail = "El campo Recontratable es nulo.";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            if (Recontratable.Equals("SI") || Recontratable.Equals("NO"))
            {
                columnFile.Field = "Recontratable";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Recontratable";
                columnFile.ColumnDetail = "El campo Recontratable solo puede contener la palabra \"SI\" o \"NO\" se sustituye valor a nulo, valor leído: " + Recontratable;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }

        }

        public Type ValidateFechaTerinoContrato(string FechaTerminoContrato, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 41 };

            if (string.IsNullOrEmpty(FechaTerminoContrato))
            {
                columnFile.Field = "Fecha de término de contrato";
                columnFile.ColumnDetail = "El campo Fecha de término de contrato es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            try
            {
                DateTime? date = null;
                date = Convert.ToDateTime(FechaTerminoContrato);

                Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

                if (exp.IsMatch(FechaTerminoContrato))
                {
                    columnFile.Field = "Fecha de término de contrato";
                    columnFile.ColumnDetail = "Ok";
                    columnFile.Type = Type.Success;
                    rowFile.Columns.Add(columnFile);
                    return Type.Success;
                }
                else
                {
                    columnFile.Field = "Fecha de término de contrato";
                    columnFile.ColumnDetail = "El campo Fecha de término de contrato debe tener el formato correcto \"dd/mm/aaaa\" se sustituye valor a nulo, valor leído: " + FechaTerminoContrato;
                    columnFile.Type = Type.Invalid;
                    rowFile.Columns.Add(columnFile);
                    return Type.Invalid;
                }

            }
            catch
            {
                columnFile.Field = "Fecha de término de contrato";
                columnFile.ColumnDetail = "El campo Fecha de término de contrato es incorrecta, valor leído: " + FechaTerminoContrato;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdCentroCostos">Identificador del cento de costos</param>
        /// <returns>valor del centro de costos</returns>
        public int? CentroCostos(string IdCentroCostos)
        {

            if (string.IsNullOrEmpty(IdCentroCostos))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdCentroCostos);
                    if (CentrosCostos.Exists(x => x.IdCentroCostos.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdDepartamento">Identificador del departamento</param>
        /// <returns>valor del departamento</returns>
        public int? Departamento(string IdDepartamento)
        {

            if (string.IsNullOrEmpty(IdDepartamento))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdDepartamento);
                    if (Departamentos.Exists(x => x.IdDepartamento.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdPuesto">Identificador del puesto</param>
        /// <returns>valor del puesto</returns>
        public int? Puesto(string IdPuesto)
        {
            if (string.IsNullOrEmpty(IdPuesto))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdPuesto);
                    if (Puestos.Exists(x => x.IdPuesto.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdRegistroPatronal"></param>
        /// <returns>Valor del registro patronal</returns>
        public int? RegistroPatronal(string IdRegistroPatronal)
        {
            if (string.IsNullOrEmpty(IdRegistroPatronal))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdRegistroPatronal);
                    if (RegistrosPatronales.Exists(x => x.IdRegistroPatronal.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="ApellidoMaterno">Apellido materno del empleado</param>
        /// <returns>Apellido materno del empleado</returns>
        public string ApellidoMaterno(string ApellidoMaterno)
        {

            if (string.IsNullOrEmpty(ApellidoMaterno))
            {
                return null;
            }

            Regex exp = new Regex(@"^[a-zA-ZÑñ ]*$");

            if (exp.IsMatch(ApellidoMaterno))
            {
                return ApellidoMaterno;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="sexo">Sexo del empleado</param>
        /// <returns>Sexo del empleado</returns>
        public string Sexo(string sexo)
        {
            if (string.IsNullOrEmpty(sexo))
            {
                return null;
            }

            if (sexo.Equals("MASCULINO") || sexo.Equals("FEMENINO"))
            {
                return sexo;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="EstadoCivil">Estado civil del epleado</param>
        /// <returns>Estado civil del empleado</returns>
        public string EstadoCivil(string EstadoCivil)
        {
            if (string.IsNullOrEmpty(EstadoCivil))
            {
                return null;
            }

            if (EstadoCivil.Equals("SOLTERO") || EstadoCivil.Equals("CASADO"))
            {
                return EstadoCivil;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdBancoTradicional">Identificador del banco tradicional</param>
        /// <returns>Valor del identificador del banco tradicional</returns>
        public int? IdBanco(string IdBancoTradicional)
        {
            if (string.IsNullOrEmpty(IdBancoTradicional))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(IdBancoTradicional);

                    if (Bancos.Exists(x => x.IdBanco.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="Cuenta">Cuenta bancaria del empleado</param>
        /// <returns>Cuenta bancaria del empleado</returns>
        public string Cuenta(string Cuenta)
        {
            if (string.IsNullOrEmpty(Cuenta))
            {
                return null;
            }

            Regex exp = new Regex(@"^[0-9]*$");

            if (exp.IsMatch(Cuenta))
            {
                return Cuenta;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="Fecha">Fecha</param>
        /// <returns>Fecha</returns>
        public string Fecha(string Fecha)
        {

            if (string.IsNullOrEmpty(Fecha))
            {
                return null;
            }

            Regex exp = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");

            if (exp.IsMatch(Fecha))
            {
                try
                {
                    DateTime? date = null;
                    date = Convert.ToDateTime(Fecha);
                }
                catch
                {
                    return null;
                }
                return Fecha;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="CorreoElectronico">Correo electronico dek empleado</param>
        /// <returns>Correo electronico del empleado</returns>
        public string CorreoElectronico(string CorreoElectronico)
        {
            if (string.IsNullOrEmpty(CorreoElectronico))
            {
                return null;
            }

            Regex exp = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            if (exp.IsMatch(CorreoElectronico))
            {
                return CorreoElectronico;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="Imss">Numero de seguro social del empleado</param>
        /// <returns>Numero de seguro social del empleado</returns>
        public string Imss(string Imss)
        {
            if (string.IsNullOrEmpty(Imss))
            {
                return null;
            }

            Regex exp = new Regex(@"^[0-9]*$");

            if (exp.IsMatch(Imss) || Imss.Length == 11)
            {
                return Imss;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="value">cualquier texto</param>
        /// <returns>string ó null</returns>
        public string NulableString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="cp">Codigo postal</param>
        /// <returns>codigo postal</returns>
        public int? CodigoPostal(string cp)
        {
            if (string.IsNullOrWhiteSpace(cp))
            {
                return null;
            }
            else
            {
                Regex exp = new Regex(@"^([1-9]{2}|[0-9][1-9]|[1-9][0-9])[0-9]{3}");

                var _cp = cp.Replace(" ", "");
                if (exp.IsMatch(_cp))
                {
                    ClassEmpleado service = new ClassEmpleado();
                    var cps = service.GetCodigoPostalesByString(_cp);
                    int? value = null;
                    try { value = cps.Find(x => x.Codigo.Equals(cp)).Id; } catch { }

                    if (value != null)
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="idAreas">Identificador del area</param>
        /// <returns>Identificador del area</returns>
        public int? IdArea(string idAreas)
        {
            if (string.IsNullOrEmpty(idAreas))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(idAreas);

                    if (Areas.Exists(x => x.IdArea.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="Idarea">Identificador del sindicato</param>
        /// <returns>Identificador del sindicato</returns>
        public int? idSindicato(string Idarea)
        {
            if (string.IsNullOrEmpty(Idarea))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = Convert.ToInt32(Idarea);

                    if (Sindicatos.Exists(x => x.IdSindicato.Equals(value)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <returns>Identificador de la sucursal</returns>
        public int? idSucursal(string IdSucursal)
        {
            if (string.IsNullOrEmpty(IdSucursal))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = int.Parse(IdSucursal);
                    if (Sucursales.Exists(p => p.IdSucursal == value))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <returns>Identificador de la sucursal</returns>
        public int? idJornada(string IdJornada)
        {
            if (string.IsNullOrEmpty(IdJornada))
            {
                return null;
            }
            else
            {
                try
                {
                    int value = int.Parse(IdJornada);
                    if (Jornadas.Exists(p => p.Value == value.ToString()))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="value">decimal</param>
        /// <returns>decimal</returns>
        public decimal NulableDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Math.Round(Convert.ToDecimal(value), 4);
                }
                catch
                {
                    return 0;
                }
            }

        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="RFCSubcontracion">Rfc de subcontratación</param>
        /// <returns>RFC de subcontratación</returns>
        public string ValidateRFCSub(string RFCSubcontracion)
        {

            if (string.IsNullOrEmpty(RFCSubcontracion))
            {
                return null;
            }

            Regex exp = new Regex(@"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([A-Z\d]{3})?$");

            if (exp.IsMatch(RFCSubcontracion))
            {
                return RFCSubcontracion;
            }
            else
            {
                return null;
            }

        }

        public int? idBancoViaticos(string IdBancoViatico)
        {
            int? result = null;
            if (!string.IsNullOrEmpty(IdBancoViatico))
            {
                int value = int.Parse(IdBancoViatico);
                result = value;
            }
            return result;
        }

        /// <summary>
        /// Método par aconvertir valdiar la Nacionalidad
        /// </summary>
        /// <param name="nacionalidad">Texto con la nacionalidad</param>
        /// <returns>Texto con la nacionalidad</returns>
        public string ValidaNacionalidad(string nacionalidad)
        {
            string nac = "";
            if (nacionalidad != null)
            {
                if (nacionalidad.ToUpper() == "EXTRANJERA")
                    nac = "EXTRANJERA";
                else if (nacionalidad.ToUpper() == "MEXICANA")
                    nac = "MEXICANA";
                else
                    nac = null;
            }
            else
                nac = null;

            return nac;
        }

        /// <summary>
        /// Método que verifica que el campo NAcionalidad no esté nulo
        /// </summary>
        /// <param name="Nacionalidad">Contiene la nacionalidad del empleado</param>
        /// <param name="rowFile">Obtiene la información de la celda correspondiente</param>
        /// <returns></returns>
        public Type ValidateNacionalidad(string Nacionalidad, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 40 };

            if (string.IsNullOrEmpty(Nacionalidad))
            {
                columnFile.Field = "Nacionalidad";
                columnFile.ColumnDetail = "El campo Nacionalidad es nulo";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            columnFile.Field = "Nacionalidad";
            columnFile.ColumnDetail = "Ok";
            columnFile.Type = Type.Success;
            rowFile.Columns.Add(columnFile);
            return Type.Success;
        }

        /// <summary>
        /// Método par aconvertir valores a nulos
        /// </summary>
        /// <param name="ApellidoMaterno">Apellido materno del empleado</param>
        /// <returns>Apellido materno del empleado</returns>
        public string Nombre(string Nombre)
        {
            try
            {
                if (string.IsNullOrEmpty(Nombre))
                {
                    return null;
                }
                string apeM = Nombre.Normalize(NormalizationForm.FormD);

                StringBuilder result = new StringBuilder();

                foreach (char c in apeM)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    {
                        result.Append(c);
                    }
                }

                Regex exp = new Regex(@"^[a-zA-ZÑñ ]*$");

                if (exp.IsMatch(result.ToString()))
                {
                    return result.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// En este metodo de obtendran los dias configurados (DiasImss) por el cliente en la Unidad de Negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Identificador unico de la Unidad de Negocio (debera ser obtenida de la variable de session: ["sIdUnidad"]</param>
        /// <returns>Numero de dias configurados en la unidad de negocio</returns>
        public int DiasConfiguradosUnidadNegocio(int IdUnidadNegocio)
        {
            int? unidadnegociovalue = null;
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                unidadnegociovalue = ctx.Cat_UnidadNegocio.Where(p => p.IdUnidadNegocio == IdUnidadNegocio).Select(p => p.DIasImss).FirstOrDefault();
            }
            int result = unidadnegociovalue == null ? 5 : unidadnegociovalue.Value;
            return result;
        }


        public Type ValidateTimbradoNomna(string Valida, RowFile rowFile)
        {
            ColumnFile columnFile = new ColumnFile { Column = 4 };

            if (string.IsNullOrEmpty(Valida))
            {
                columnFile.Field = "Timbrado";
                columnFile.ColumnDetail = "El campo Recontratable es nulo.";
                columnFile.Type = Type.Warning;
                rowFile.Columns.Add(columnFile);
                return Type.Warning;
            }

            if (Valida.Equals("S") || Valida.Equals("N"))
            {
                columnFile.Field = "Timbrado";
                columnFile.ColumnDetail = "Ok";
                columnFile.Type = Type.Success;
                rowFile.Columns.Add(columnFile);
                return Type.Success;
            }
            else
            {
                columnFile.Field = "Timbrado";
                columnFile.ColumnDetail = "El campo Recontratable solo puede contener la palabra \"S\" o \"N\" se sustituye valor a nulo, valor leído: " + Valida;
                columnFile.Type = Type.Invalid;
                rowFile.Columns.Add(columnFile);
                return Type.Invalid;
            }

        }
    }
}