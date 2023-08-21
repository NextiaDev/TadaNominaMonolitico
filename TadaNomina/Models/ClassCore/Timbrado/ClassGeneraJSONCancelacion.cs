using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.Timbrado.ModelosJSON;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore.Timbrado
{
    public class ClassGeneraJSONCancelacion
    {
        /// <summary>
        /// Metodo que genera un modelo cancelacion para un timbrado
        /// </summary>
        /// <param name="timbrado">Timbrado a cancelar</param>
        /// <returns>Modelo a cancelar</returns>
        public ModeloCancelacion CreaJSONCancelacion(vTimbradoNomina timbrado)
        {
            ModeloCancelacion cancelacion = new ModeloCancelacion();
            cancelacion.fechaTImbrado = timbrado.FechaTimbrado;
            cancelacion.rfcEmisor = timbrado.RFC_Patronal;
            cancelacion.noCertificado = timbrado.SelloDigital;
            cancelacion.UUID = timbrado.FolioUDDI;

            return cancelacion;
        }
    }
}