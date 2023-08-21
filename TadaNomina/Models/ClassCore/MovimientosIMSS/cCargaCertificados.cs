using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.MovimientosIMSS;

namespace TadaNomina.Models.ClassCore.MovimientosIMSS
{
    public class cCargaCertificados
    {
        public List<Cat_RegistroPatronal> GetRegistroPatronalByIdCliente(int IdCliente)
        {
            using(TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var listado = ctx.Cat_RegistroPatronal.Where(p => p.IdCliente == IdCliente && p.IdEstatus == 1).ToList();
                return listado;
            }
        }

        public void AddCertificadoIMSS(mCredencialesMovimientosIMSS model, string ruta, string rutaKey, int IdModifica)
        {
            using (TadaEmpleados ctx = new TadaEmpleados())
            {
                var regpatronal = ctx.Cat_RegistroPatronal.Where(p => p.IdRegistroPatronal == model.IdReg).FirstOrDefault();
                if (rutaKey == null)
                {
                    regpatronal.CertificadoIMSS = ruta;
                    regpatronal.UsuarioIMSS = model.UsuarioIMSS;
                    regpatronal.ContraseñaIMSS = model.ContrasenaIMSS;

                    ctx.SaveChanges();
                }
                else
                {
                    regpatronal.CertificadoIMSS = ruta;
                    regpatronal.KeyIMSS = rutaKey;
                    regpatronal.UsuarioIMSS = model.UsuarioIMSS;
                    regpatronal.ContraseñaIMSS = model.ContrasenaIMSS;

                    ctx.SaveChanges();
                }
            }
        }
    }
}