using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    /// <summary>
    /// Modelo para la creación de un periodo de nómina
    /// </summary>
    public class ModelCreatePeriodoNomina
    {
        /// <summary>
        /// Constructor de la clase ModelCreatePeriodoNomina
        /// </summary>
        /// <param name="idUnidadNegocio">Identificador de la unidad de negocio</param>
        /// <param name="periodo">Periodo de la nómina</param>
        /// <param name="fechaInicio">Fecha de inicio del periodo</param>
        /// <param name="fechaFin">Fecha de fin del periodo</param>
        /// <param name="ajusteImpuestos">Ajuste de impuestos</param>
        /// <param name="idsPeriodosAjuste">Ids de los periodos de ajuste</param>
        /// <param name="observaciones">Observaciones</param>
        /// <param name="tipoNomina">Tipo de nómina</param>
        /// <param name="tablaIDiaria">Tabla I Diaria</param>
        /// <param name="ajusteAnual">Ajuste anual</param>
        /// <param name="omitirDescuentosFijos">Omitir descuentos fijos</param>
        /// <param name="idClientePTU">Id del cliente PTU</param>
        /// <param name="idRegistroPatronalPTU">Id del registro patronal PTU</param>
        /// <param name="montoPTU">Monto de la PTU</param>
        /// <param name="anioPTU">Año de la PTU</param>
        /// <param name="calculoPatronaPtu">Cálculo de la patrona PTU</param>
        /// <param name="fechaInicioChecador">Fecha de inicio del checador</param>
        /// <param name="fechaFinChecador">Fecha de fin del checador</param>
        public ModelCreatePeriodoNomina(int idUnidadNegocio,
                                        string periodo,
                                        DateTime fechaInicio,
                                        DateTime fechaFin,
                                        string ajusteImpuestos,
                                        string idsPeriodosAjuste,
                                        string observaciones,
                                        string tipoNomina,
                                        bool tablaIDiaria,
                                        bool ajusteAnual,
                                        bool omitirDescuentosFijos,
                                        int? idClientePTU,
                                        int? idRegistroPatronalPTU,
                                        decimal? montoPTU,
                                        int? anioPTU,
                                        bool? calculoPatronaPtu,
                                        string fechaInicioChecador,
                                        string fechaFinChecador)
        {
            IdUnidadNegocio = idUnidadNegocio; // Identificador de la unidad de negocio
            Periodo = periodo; // Periodo de la nómina
            FechaInicio = fechaInicio; // Fecha de inicio del periodo
            FechaFin = fechaFin; // Fecha de fin del periodo
            AjusteImpuestos = ajusteImpuestos; // Ajuste de impuestos
            IdsPeriodosAjuste = idsPeriodosAjuste; // Ids de los periodos de ajuste
            Observaciones = observaciones; // Observaciones
            TipoNomina = tipoNomina; // Tipo de nómina
            TablaIDiaria = tablaIDiaria; // Tabla I Diaria
            AjusteAnual = ajusteAnual; // Ajuste anual
            OmitirDescuentosFijos = omitirDescuentosFijos; // Omitir descuentos fijos
            IdClientePTU = idClientePTU; // Id del cliente PTU
            IdRegistroPatronalPTU = idRegistroPatronalPTU; // Id del registro patronal PTU
            MontoPTU = montoPTU; // Monto de la PTU
            AnioPTU = anioPTU; // Año de la PTU
            CalculoPatronaPtu = calculoPatronaPtu; // Cálculo de la patrona PTU
            FechaInicioChecador = fechaInicioChecador; // Fecha de inicio del checador
            FechaFinChecador = fechaFinChecador; // Fecha de fin del checador
        }

        public int IdUnidadNegocio { get; set; } //Identificador de la unidad de negocio
        public string Periodo { get; set; } //Periodo de la nómina
        public DateTime FechaInicio { get; set; } //Fecha de inicio del periodo
        public DateTime FechaFin { get; set; } //Fecha de fin del periodo
        public string AjusteImpuestos { get; set; } //Ajuste de impuestos
        public string IdsPeriodosAjuste { get; set; } // Ids de los periodos de ajuste
        public string Observaciones { get; set; } // Observaciones
        public string TipoNomina { get; set; } // Tipo de nómina
        public bool TablaIDiaria { get; set; } // Tabla I Diaria
        public bool AjusteAnual { get; set; } // Ajuste anual
        public bool OmitirDescuentosFijos { get; set; } // Omitir descuentos fijos
        public int? IdClientePTU { get; set; } // Id del cliente PTU
        public int? IdRegistroPatronalPTU { get; set; } // Id del registro patronal PTU
        public decimal? MontoPTU { get; set; } // Monto de la PTU
        public int? AnioPTU { get; set; } // Año de la PTU
        public bool? CalculoPatronaPtu { get; set; } // Cálculo de la patrona PTU
        public string FechaInicioChecador { get; set; } // Fecha de inicio del checador
        public string FechaFinChecador { get; set; } // Fecha de fin del checador
    }
}