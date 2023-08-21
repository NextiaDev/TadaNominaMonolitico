using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Delva.AppCode.TimbradoTurboPAC
{
    public class Constantes
    {
        public static IDictionary<int, string> ErroresValidacion
        {
            get
            {
                var result = new Dictionary<int, string>
            {
                #region Cancelacion
                
                {201, "UUID Cancelado exitosamente"},
                {202, "UUID Previamente cancelado"},
                {203, "UUID no corresponde a emisor"},
                {204, "UUID no aplicable para cancelacion"},
                {205, "UUID no existe"},

                #endregion Cancelacion
                
                #region Validacion
                
                {301, "XML mal formado"},
                {302, "Sello mal formado o inválido"},
                {303, "Certificado no corresponde a RFC de emisor o caduco"},
                {304, "Certificado revocado o caduco"},
                {305, "La fecha de emisión no esta dentro de la vigencia del CSD del Emisor"},
                {306, "EL cerificado no es de tipo CSD"},
                {307, "El CFDI contiene un timbre previo"},
                {308, "Certificado no expedido por el SAT"},
                {398, "El Emisor no tiene obligaciones"},
                {399, "El CSD del emisor es invalido"},
                {401, "Fecha y hora de generación fuera de rango"},
                {402, "RFC del emisor no se encuentra en el régimen de contribuyentes"},
                {403, "La fecha de emisión no es posterior al 01 de enero 2011"},
                
                #endregion Validacion
                };
                return result;
            }
        }
    }
}