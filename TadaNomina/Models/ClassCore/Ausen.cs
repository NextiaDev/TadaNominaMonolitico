using SW.Services.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.ClassCore.MovimientosIMSS;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Nominas;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    public class Ausen 
    {

        public vAusentismos GetvAusensId(int id)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var periodos = (from b in entidad.vAusentismos.Where(x => x.IdAusentismo == id) select b).FirstOrDefault();

                return periodos;
            }
        }
        public ModelAusentismos GetIsAusentismose(int id)
        {
            vAusentismos lperiodos = GetvAusensId(id);
            ModelAusentismos modelo = new ModelAusentismos();

            modelo.IdEmpleado = lperiodos.IdEmpleado;
            return modelo;
        }




        public List<vAusentismos> getAusentismoUnidadn(int unidadnegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.vAusentismos
                          where b.IdEstatus == 1 && b.IdUnidadNegocio == unidadnegocio
                          select b);

                return cc.ToList();
            }
        }

        public ModelAusentismos FindListAusen(int IdCliente)
        {

            ClassConceptos cconceptos = new ClassConceptos();
            List<vConceptos> lconceptosNomina = cconceptos.GetvConceptos(IdCliente);
            List<SelectListItem> Lausentismos = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaVac = lconceptosNomina.Where(x => x.ClaveGpo == "500").ToList();
            lconceptosNominaVac.ForEach(x => { Lausentismos.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });
            List<SelectListItem> Lincapacidades = new List<SelectListItem>();
            List<vConceptos> lconceptosNominaPrestaciones = lconceptosNomina.Where(x => x.ClaveGpo == "501").ToList();
            lconceptosNominaPrestaciones.ForEach(x => { Lincapacidades.Add(new SelectListItem { Text = x.ClaveConcepto + "-" + x.Concepto, Value = x.IdConcepto.ToString() }); });
            List<SelectListItem> _Einca = new List<SelectListItem>();
            _Einca.Add(new SelectListItem { Text = "Si", Value = "Si" });
            _Einca.Add(new SelectListItem { Text = "No", Value = "No" });
            List<SelectListItem> _Incidencias = new List<SelectListItem>();
            _Incidencias.Add(new SelectListItem { Text = "Incapacidad", Value = "Incapacidad" });
            _Incidencias.Add(new SelectListItem { Text = "Ausentismo", Value = "Ausentismo" });
            ModelAusentismos model = new ModelAusentismos
            {
                Subsecuente = _Einca,
                Incapacidad = Lincapacidades,
                LAusentismos = Lausentismos,
                TipodeIncidencia = _Incidencias,
                Subsidio = _Einca,
            };

            return model;
        }

        public ModelAusentismos GetausenById(ModelAusentismos model, int idempleado, int idunidadnegocio)
        {
            using (TadaEmpleados entidad = new TadaEmpleados())
            {

                var cc = (from b in entidad.Empleados
                          where b.IdEmpleado == idempleado && b.IdUnidadNegocio == idunidadnegocio
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    model.Nombre = cc.Nombre;
                    model.ClaveEmp = cc.ClaveEmpleado;
                    model.rfc = cc.Rfc;


                }

                return model;
            }
        }

        public string getfolioR(string folio)
        {
            string resul = null;
            using (NominaEntities1 entity = new NominaEntities1())
            {
                var query = (from b in entity.Ausentismos where b.NoFolio == folio select b.FechaCreacion).FirstOrDefault();
                if (query == null)
                {

                    resul = null;
                }
                else
                {
                    resul = "Si";
                }
                return resul;
            }
        }


        public void AddAusen(ModelAusentismos ausen, int IdUsuario)
        {
            int value = 0;
            string cempleado = ausen.ClaveEmp;
            string resul = string.Empty;
            string Subsecuenten = string.Empty;
            string FolioSub = string.Empty;
            string FolioN = string.Empty;
            string TipoIncapacidadN = string.Empty;
            int ClaveC = 0;

            ClassEmpleado cemp = new ClassEmpleado();
            var empl = cemp.GetEmpleadosByClave(ausen.ClaveEmp, ausen.unidadnegocio).FirstOrDefault();

            if (ausen.idAusentismos == null)
            {
                ClaveC = int.Parse(ausen.IdIncapacidad);
                TipoIncapacidadN = "Inicial";
            }
            else
            {
                ClaveC = int.Parse(ausen.idAusentismos);
                TipoIncapacidadN = "Ausentismo";
            }
            if (ausen.Folio == null)
            {
                FolioN = "S/N";
            }
            else
            {
                FolioN = ausen.Folio;

            }

            if (ausen.FolioSub == null)
            {
                FolioSub = "S/N";
            }
            else
            {
                FolioSub = ausen.FolioSub;

            }
            using (NominaEntities1 entity = new NominaEntities1())
            {
                int ID = (from b in entity.PeriodoNomina where b.IdUnidadNegocio == ausen.unidadnegocio && b.IdEstatus == 1 && b.TipoNomina == "Nomina" select b.IdPeriodoNomina).FirstOrDefault();

                Ausentismos emp = new Ausentismos()
                {
                    Idconcepto = ClaveC.ToString(),
                    TipoAusentismo = FolioSub,
                    NoFolio = FolioN,
                    FolioIncapacidadInicial = ausen.FolioSub,
                    FechaInicioAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion),
                    FechaIncio = DateTime.Parse(ausen.FechaInicial),
                    Dias = ausen.Dias,
                    IdCaptura = IdUsuario,
                    IdEmpleado = empl.IdEmpleado,
                    Archivo = ausen.Imagen.ToString(),
                    FechaFinAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion).AddDays(ausen.Dias - 1),
                    FechaFin = DateTime.Parse(ausen.FechaInicial).AddDays(ausen.Dias - 1),
                    IdEstatus = 1,
                    AplicaSubsidio = ausen.SubAusen,
                    DiasSubsidioInicial = ausen.DiasSubidioInicial,
                    PorcentajeSubsidioInicial = ausen.PorcentajeSubsidio,
                    PorcentajeSubsidioRestante = ausen.PorcentajeSubsidioRestante,
                    TipoIncapacidad = TipoIncapacidadN,
                    FechaCreacion = DateTime.Now
                };

                entity.Ausentismos.Add(emp);
                value = entity.SaveChanges();
            }
        }


        public void AddAusenCons(ModelAusentismos ausen, int Idusuario)
        {
            int value = 0;
            string cempleado = ausen.ClaveEmp;
            string resul = string.Empty;
            ClassEmpleado cemp = new ClassEmpleado();
            var empl = cemp.GetEmpleadosByClave(ausen.ClaveEmp, ausen.unidadnegocio).FirstOrDefault();

            using (NominaEntities1 entity = new NominaEntities1())
            {
                int ID = (from b in entity.PeriodoNomina where b.IdUnidadNegocio == ausen.unidadnegocio && b.TipoNomina == "Nomina" && b.IdEstatus == 1 select b.IdPeriodoNomina).FirstOrDefault();
                Ausentismos emp = new Ausentismos()
                {

                    NoFolio = ausen.Folio,
                    TipoAusentismo = ausen.TipoIncidencia,
                    Idconcepto = ausen.IdIncapacidad,
                    FechaInicioAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion),
                    FechaIncio = DateTime.Parse(ausen.FechaInicial),
                    Dias = ausen.Dias,
                    IdEmpleado = empl.IdEmpleado,
                    Archivo = ausen.Imagen.ToString(),
                    FechaFinAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion).AddDays(ausen.Dias - 1),
                    FechaFin = DateTime.Parse(ausen.FechaInicial).AddDays(ausen.Dias - 1),
                    IdEstatus = 1,
                    FolioIncapacidadInicial = ausen.FolioSub,
                    TipoIncapacidad = "Subsecuente",
                    DiasSubsidioInicial = ausen.DiasSubidioInicial,
                    PorcentajeSubsidioInicial = ausen.PorcentajeSubsidio,
                    PorcentajeSubsidioRestante = ausen.PorcentajeSubsidioRestante,
                    AplicaSubsidio = ausen.idAusentismos,
                    FechaCreacion = DateTime.Now,

                };

                entity.Ausentismos.Add(emp);
                value = entity.SaveChanges();
            }
        }

        //Editar
        public void AddAusenedit(ModelAusentismos ausen, string idfolio)
        {


            if (ausen.IdSubsecuente == "No")
            {
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    var concepto = (from b in entidad.Ausentismos.Where(x => x.FolioIncapacidadInicial == idfolio) select b).FirstOrDefault();

                    if (concepto != null)
                    {

                        concepto.NoFolio = ausen.Folio;
                        concepto.FechaIncio = DateTime.Parse(ausen.FechaInicial);
                        concepto.Dias = ausen.Dias;
                        concepto.IdEmpleado = ausen.IdEmpleado;
                        concepto.Archivo = ausen.Imagen.ToString();
                        concepto.FechaFinAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion).AddDays(ausen.Dias - 1);
                        
                        concepto.FechaFin = DateTime.Parse(ausen.FechaInicial).AddDays(ausen.Dias - 1);
                        concepto.IdEstatus = 1;
                        concepto.TipoIncapacidad = "Inicial";
                        concepto.FechaCreacion = DateTime.Now;
                    }

                    entidad.SaveChanges();
                }
            }

            else
            {
                using (NominaEntities1 entidad = new NominaEntities1())
                {
                    var concepto = (from b in entidad.Ausentismos.Where(x => x.FolioIncapacidadInicial == idfolio) select b).FirstOrDefault();

                    if (concepto != null)
                    {

                        concepto.NoFolio = ausen.Folio;
                        concepto.FechaIncio = DateTime.Parse(ausen.FechaInicial);
                        concepto.Dias = ausen.Dias;
                        concepto.IdEmpleado = ausen.IdEmpleado;
                        concepto.Archivo = ausen.Imagen.ToString();
                        concepto.FechaFinAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion).AddDays(ausen.Dias - 1);

                        concepto.FechaFin = DateTime.Parse(ausen.FechaInicial).AddDays(ausen.Dias - 1);
                        concepto.IdEstatus = 1;
                        concepto.TipoIncapacidad = "Subsecuente";
                        concepto.FechaCreacion = DateTime.Now;
                    }

                    entidad.SaveChanges();
                }
            }
        }

        public void AddAusenConsedit(ModelAusentismos ausen, string idfolio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var concepto = (from b in entidad.Ausentismos.Where(x => x.FolioIncapacidadInicial == idfolio) select b).FirstOrDefault();

                if (concepto != null)
                {

                    concepto.NoFolio = ausen.Folio;
                    concepto.FechaIncio = DateTime.Parse(ausen.FechaInicial);
                    concepto.Dias = ausen.Dias;
                    concepto.IdEmpleado = ausen.IdEmpleado;
                    concepto.Archivo = ausen.Imagen.ToString();
                    concepto.FechaFinAplicacion = DateTime.Parse(ausen.FechaInicialAplicacion).AddDays(ausen.Dias - 1);
                    concepto.FechaFin = DateTime.Parse(ausen.FechaInicial).AddDays(ausen.Dias - 1);
                    concepto.IdEstatus = 1;
                    concepto.TipoIncapacidad = "Inicial";
                    concepto.FechaCreacion = DateTime.Now;
                }

                entidad.SaveChanges();
            }
        }

        public ModelAusentismos GeteditAusentismossById(ModelAusentismos model, int idfolio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {

                var cc = (from b in entidad.Ausentismos
                          where b.IdAusentismo == idfolio
                          select b).FirstOrDefault();

                if (cc != null)
                {

                    model.IdEmpleado = cc.IdEmpleado;
                    model.Folio = cc.NoFolio;
                    model.FechaInicial = cc.FechaIncio.ToString();
                    model.Dias = cc.Dias.Value;
                    model.FolioSub = cc.FolioIncapacidadInicial;

                }

                return model;
            }
        }

        public ModelAusentismos GetModelAusenB(int idfolio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ModelAusentismos model = new ModelAusentismos();
                var cc = (from b in entidad.Ausentismos
                          where b.IdAusentismo == idfolio
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    model.IdAusentismo = cc.IdAusentismo;
                    model.IdEmpleado = cc.IdEmpleado;
                    model.Folio = cc.NoFolio;
                    model.Concepto = Concetp(int.Parse(cc.Idconcepto));
                    model.FechaInicialAplicacion = cc.FechaInicioAplicacion.ToString();
                    model.FechaInicial = cc.FechaIncio.ToString();
                    model.FechaFinal = cc.FechaFin.ToString();
                    model.Dias = cc.Dias.Value;
                    model.FolioSub = cc.FolioIncapacidadInicial;
                }

                return model;
            }
        }

        public string Concetp(int idConcepto)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ModelAusentismos model = new ModelAusentismos();
                var cc = (from b in entidad.vConceptos
                          where b.IdConcepto == idConcepto
                          select b.Concepto).FirstOrDefault();

                return cc;
            }
        }


        public void DeleteAusentism(int idfolio, int pIdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.Ausentismos
                          where b.IdAusentismo == idfolio
                          select b).FirstOrDefault();

                if (cc != null)
                {

                    cc.IdEstatus = 2;

                    entidad.SaveChanges();
                }
            }
        }

        public void EliminaIncidenciasNominaAbierta(int IdAusentismo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.BanderaAusentismos == IdAusentismo).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }


        public string BuscarIdIncidenciasC(int IdAusentismo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.Incidencias_Consolidadas
                          where b.BanderaAusentismos == IdAusentismo
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    return "S";
                }
                return "N";
            }
        }

        public void ProcesaAusentismos( List<sp_RegresaAusentismos_Result> ausentismos, vConfiguracionConceptosFiniquitos conceptosConfigurados , int IdPeriodoNomina, int IdUsuario)
        {
            foreach (var iAusen in ausentismos)
            {
                int IdConcepto = 0;
                DeleteIncidenciaAusentismos(IdPeriodoNomina, iAusen.IdAusentismo);
                AgregaIncidenciaAusentismoEmpleado(iAusen, IdPeriodoNomina, IdUsuario);
                if (iAusen.AplicaSubsidio == "Si")
                {
                    if (iAusen.PorcentajeSubsidioRestante != 0)
                    {
                       var dias = ObtenDias(iAusen.idempleado.Value);
                        var restad = (dias - iAusen.DiasSubsidioInicial) * iAusen.SD * (iAusen.PorcentajeSubsidioRestante);
                        try
                        {
                            //IdConcepto = (int)conceptosConfigurados.idConceptoSubsidioIncapacidad;
                        }
                        catch { throw new Exception(" Subsidio Por Incapacidad , no se configuro ningun concepto. "); }
                        AgregaIncidenciaAusentismoEmpleadoSubsidio(iAusen, IdPeriodoNomina, IdConcepto, restad.Value, IdUsuario);
                    }
                    var operacion = (iAusen.SD * iAusen.DiasSubsidioInicial) * iAusen.PorcentajeSubsidioInicial;
                    try {
                        //IdConcepto = (int)conceptosConfigurados.idConceptoSubsidioIncapacidad; 
                    } catch { throw new Exception(" Subsidio Por Incapacidad , no se configuro ningun concepto. "); }
                    AgregaIncidenciaAusentismoEmpleadoSubsidio(iAusen, IdPeriodoNomina, IdConcepto, operacion.Value, IdUsuario);

                }
            }
        }


        public decimal ObtenDias (int idEmpleado)
        {
            
            using (TadaEmpleados entidad = new TadaEmpleados())
            {
                var _empleados = (from b in entidad.vEmpleados.Where(x => x.IdEmpleado == idEmpleado) select b.DiasPago).FirstOrDefault();
                if (_empleados != 0)
                    return _empleados;
                else
                    return 0;
            }
        }
   
        public void DeleteIncidenciaAusentismos(int IdPeriodoNomina, int IdAusentismo)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidencias = entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.BanderaAusentismos == IdAusentismo).ToList();

                entidad.Incidencias.RemoveRange(incidencias);
                entidad.SaveChanges();
            }
        }

        public void AgregaIncidenciaAusentismoEmpleado(sp_RegresaAusentismos_Result ausentismo, int IdPeriodoNomina, int IdUsuario)
        {
            Incidencias i = new Incidencias();
            i.IdEmpleado = ausentismo.idempleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = int.Parse(ausentismo.idconcepto);
            i.Cantidad = ausentismo.dias;
            i.Monto = 0;
            i.Exento = 0;
            i.Gravado = 0;
            i.MontoEsquema = 0;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaAusentismos = ausentismo.IdAusentismo;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                entidad.Incidencias.Add(i);
                entidad.SaveChanges();
            }
        }


        public void AgregaIncidenciaAusentismoEmpleadoSubsidio(sp_RegresaAusentismos_Result ausentismo, int IdPeriodoNomina,int incidencia,decimal monto,  int IdUsuario)
        {


            Incidencias i = new Incidencias();
            i.IdEmpleado = ausentismo.idempleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = int.Parse(ausentismo.idconcepto);
            i.Cantidad = ausentismo.dias;
            i.Monto = 0;
            i.Exento = 0;
            i.Gravado = 0;
            i.MontoEsquema = 0;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaAusentismos = ausentismo.IdAusentismo;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                entidad.Incidencias.Add(i);
                entidad.SaveChanges();
            }
        }


    }

}
