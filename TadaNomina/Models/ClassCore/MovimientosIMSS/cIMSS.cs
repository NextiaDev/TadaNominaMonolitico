using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cIMSS
    {
        public mRespuestaActualizacionSDI UpdateEmpleadosSDI(string ruta, int IdUsuario, string Observaciones, int IdUnidadNegocio)
        {
            mRespuestaActualizacionSDI r = new mRespuestaActualizacionSDI();
            r.listErrores = new List<string>();
            r.Correctos = 0;
            r.Errores = 0;
            r.noRegistros = 0;
            r.Path = Path.GetFileName(ruta);

            ArrayList array = GetArrayModificaciones(ruta);

            // Si hay registros en el archivo
            if (array.Count > 0)
            {
                //string noLote = RegresaNoLote();
                List<vEmpleados> empleados = GetAllVEmpleadosByIdUnidadNegocio(IdUnidadNegocio);

                if (empleados.Count <= 0)
                {
                    r.listErrores.Add("NO SE ENCONTRARON A LOS COLABORADORES EN LA UNIDAD DE NEGOCIO");
                    return r;
                }


                foreach (var item in array)
                {
                    r.noRegistros++;

                    bool valida = ActualizaSDI(empleados, item, IdUsuario, Observaciones);

                    if (valida)
                    {
                        r.Correctos++;
                    }
                    else
                    {
                        r.Errores++;
                        r.listErrores.Add(regresaError(item));
                    }
                }
            }

            return r;
        }

        public bool ActualizaSDI(List<vEmpleados> empleadosIMSS, object item, int IdUsuario, string Observaciones)
        {
            string[] campos = item.ToString().Split(',');

            string _rp = campos[0].Trim();
            string _nss = campos[1].Trim();
            decimal _sdi = decimal.Parse(campos[2].Trim());
            DateTime _FechaAplicacion = DateTime.Parse(campos[3].Trim());
            int _TipoSalario = int.Parse(campos[4].Trim());

            decimal _sdiAnterior = 0;

            if (_nss.Length == 10)
            {
                _nss = "0" + _nss;
            }

            int _IdEmpleado = empleadosIMSS.Where(x => x.Imss == _nss && x.IdEstatus == 1 && x.RegistroPatronal == _rp).Select(x => x.IdEmpleado).FirstOrDefault();

            if (_IdEmpleado > 0)
            {
                using (TadaEmpleadosEntities entidad = new TadaEmpleadosEntities())
                {
                    Empleados emp = (from b in entidad.Empleados
                                     where b.IdEmpleado == _IdEmpleado
                                     select b).FirstOrDefault();

                    if (emp.SDI == null)
                    {
                        // Si el SDI (Salario Diario Integrado) del empleado es nulo,
                        // establecemos el valor de _sdiAnterior a 0.
                        _sdiAnterior = 0;
                    }
                    else
                    {
                        // Si el SDI del empleado no es nulo, lo convertimos a decimal
                        // y lo asignamos a _sdiAnterior.
                        _sdiAnterior = (decimal)emp.SDI;
                    }

                    // Se retira este parseo ya que existen SDI de colaboradores con valores nulos
                    //_sdiAnterior = (decimal)emp.SDI;

                    emp.SDI = _sdi;
                    emp.IdModificacion = IdUsuario;
                    emp.FechaModificacion = DateTime.Now;
                    entidad.SaveChanges();


                    ModificacionSueldos mds = new ModificacionSueldos()
                    {
                        IdEmpleado = emp.IdEmpleado,
                        FechaMovimiento = _FechaAplicacion,
                        SDIMSS_Anterior = (decimal)emp.SDIMSS,
                        SDI_Anterior = _sdiAnterior,
                        SDIMSS_Nuevo = (decimal)emp.SDIMSS,
                        SDI_Nuevo = _sdi,
                        TipoSalario = _TipoSalario,
                        Observaciones = Observaciones,
                        IdCaptura = IdUsuario,
                        FechaCaptura = DateTime.Now
                    };

                    entidad.ModificacionSueldos.Add(mds);
                    entidad.SaveChanges();

                }
                return true;
            }
            else
            {
                return false;
            }

        }

        public ArrayList GetArrayModificaciones(string ruta)
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

        public string regresaError(object item)
        {
            string[] campos = item.ToString().Split(',');

            string _nss = campos[1].Trim();
            if (_nss.Length == 10)
            {
                _nss = "0" + _nss;
            }

            return "EL NSS " + _nss + " NO SE ENCONTRO O ES BAJA";
        }

        public List<vEmpleados> GetAllVEmpleadosByIdUnidadNegocio(int IdUnidadNegocio)
        {
            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                var empleadosIMSS = (from b in entidad.vEmpleados where b.IdUnidadNegocio == IdUnidadNegocio && b.IdEstatus == 1 select b);

                return empleadosIMSS.ToList();
            }
        }
    }
}