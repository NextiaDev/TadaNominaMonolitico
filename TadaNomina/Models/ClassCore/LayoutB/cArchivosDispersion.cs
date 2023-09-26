using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ViewModels.LayoutBancos;

namespace TadaNomina.Models.ClassCore.LayoutB
{
    public class cArchivosDispersion
    {
        public mArchivoDispersion getArchivoDispersion(int IdUnidadNegocio, int IdCliente)
        {
            mArchivoDispersion result = new mArchivoDispersion();
            ClassPeriodoNomina cpn = new ClassPeriodoNomina();
            result.selectListPeriodoNomina = cpn.GetSeleccionPeriodoAcumulado(IdUnidadNegocio);
            ClassBancos cb = new ClassBancos();
            result.selectListBancos = cb.getSelectBancosCliente(IdCliente);
            result.selectListTipoArchivos = getSelectListTipoArchivo();
            return result;
        }

        public List<SelectListItem> getSelectListTipoArchivo()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem { Text = "MISMO BANCO", Value = "2" });
            result.Add(new SelectListItem { Text = "INTERBANCARIOS", Value = "3" });
            return result;
        }

        public string getValidaciónBanco(int IdBanco)
        {
            int[] BancosOK = new int[] { -1, 1, 4, 5, 18, 8 };
            string resultado = BancosOK.Contains(IdBanco) ? "OK" : "No existe";
            return resultado;
        }

        public byte[] getTxt(mArchivoDispersion model, int IdUnidadNegocio)
        {
            byte[] bytes = new byte[1];
            switch (model.IdBanco)
            {
                case 1:
                    ClassBanamex cbnx = new ClassBanamex();
                    string txtbnx = cbnx.GeneraTxtBanamex(model.IdPeriodoNomina, IdUnidadNegocio, model.NumCliente, model.ClvSucursal, model.RefNumerica, model.RefAlfaNum, model.NombreEmpresa);
                    bytes = Encoding.ASCII.GetBytes(txtbnx);
                    break;
                case 4:
                    ClassBancomer cbbva = new ClassBancomer();
                    string txtbbva = model.TipoArchivo == 2 ? cbbva.GeneraTxtBBVA(model.IdPeriodoNomina, IdUnidadNegocio) : cbbva.GeneraTxtBBVAInterbancario(model.IdPeriodoNomina, IdUnidadNegocio);
                    bytes = Encoding.ASCII.GetBytes(txtbbva);
                    break;
                case 5:
                    ClassSantander cstd = new ClassSantander();
                    string txtstd = model.TipoArchivo == 2 ? cstd.GeneraTxtSantander(model.IdPeriodoNomina, IdUnidadNegocio) : cstd.GeneraTxtSantanderInter(model.IdPeriodoNomina, IdUnidadNegocio);
                    bytes = Encoding.ASCII.GetBytes(txtstd);
                    break;
                case 18:
                    ClassBanorte cbnte = new ClassBanorte();
                    string txtbnte = model.TipoArchivoBnte == 2 ? cbnte.GeneraTxtBanorte(model.IdPeriodoNomina, IdUnidadNegocio, model.Empresa) : cbnte.GeneraTxtBanorteInter(model.IdPeriodoNomina, IdUnidadNegocio, model.Empresa);
                    bytes = Encoding.ASCII.GetBytes(txtbnte);
                    break;
                case 8:
                    ClassBajio cbajio = new ClassBajio();
                    string txtbajio = cbajio.GeneraTxtBajio(model.IdPeriodoNomina, IdUnidadNegocio);
                    bytes = Encoding.ASCII.GetBytes(txtbajio);
                    break;
            }
            return bytes;
        }
    }
}