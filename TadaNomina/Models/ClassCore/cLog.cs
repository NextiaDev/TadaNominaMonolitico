using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore
{
    public class cLog
    {
        private string Path = @"D:\TadaNomina\LogsFRONT\";


        public void Add(string sLog)
        {
            CreateDirectory();
            string nombre = GetNameFile();
            string cadena = "";

            cadena += "> " + DateTime.Now + " | " + sLog + Environment.NewLine;

            StreamWriter sw = new StreamWriter(Path + @"\" + nombre, true);
            sw.Write(cadena);
            sw.Close();
        }

        public string getLog(DateTime fecha)
        {
            var name = "log_" + fecha.Year + "_" + fecha.Month + "_" + fecha.Day + ".txt";
            var path = Path + name;

            string text = File.ReadAllText(path);
            return text;
        }

        public byte[] getLogBytes(DateTime fecha)
        {
            var name = "log_" + fecha.Year + "_" + fecha.Month + "_" + fecha.Day + ".txt";
            var path = Path + name;

            var text = File.ReadAllBytes(path);
            return text;
        }

        private string GetNameFile()
        {
            string nombre = "";

            nombre = "log_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".txt";

            return nombre;
        }

        private void CreateDirectory()
        {
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

            }
            catch (DirectoryNotFoundException ex)
            {
                throw new Exception(ex.Message);

            }
        }

    }
}