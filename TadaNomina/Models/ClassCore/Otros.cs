using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore
{
    public class Otros
    {
        public string RemueveAcentos(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Reemplazar vocales con acento agudo
            input = input.Replace("á", "a")
                         .Replace("é", "e")
                         .Replace("í", "i")
                         .Replace("ó", "o")
                         .Replace("ú", "u")
                         .Replace("Á", "A")
                         .Replace("É", "E")
                         .Replace("Í", "I")
                         .Replace("Ó", "O")
                         .Replace("Ú", "U");

            // Reemplazar vocales con acento grave
            input = input.Replace("à", "a")
                         .Replace("è", "e")
                         .Replace("ì", "i")
                         .Replace("ò", "o")
                         .Replace("ù", "u")
                         .Replace("À", "A")
                         .Replace("È", "E")
                         .Replace("Ì", "I")
                         .Replace("Ò", "O")
                         .Replace("Ù", "U");

            // Reemplazar la vocal con diéresis o crema
            input = input.Replace("ä", "a")
                         .Replace("ë", "e")
                         .Replace("ï", "i")
                         .Replace("ö", "o")
                         .Replace("ü", "u")
                         .Replace("Ä", "A")
                         .Replace("Ë", "E")
                         .Replace("Ï", "I")
                         .Replace("Ö", "O")
                         .Replace("Ü", "U");

            // Reemplazar la vocal con tilde
            input = input.Replace("ã", "a")
                         .Replace("õ", "o")
                         .Replace("Ã", "A")
                         .Replace("Õ", "O");

            return input;
        }
    }
}