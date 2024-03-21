using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Services;

namespace TadaNomina.Models.ClassCore
{
    public class ClassUnidadesNegocio
    {
        /// <summary>
        /// Método para listar la unidades de negocio.
        /// </summary>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la lista de las unidades de negocio.</returns>
        public List<vUnidadesNegocio> getUnidadesnegocio(int pIdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var unidades = from b in entidad.vUnidadesNegocio.Where(x => x.IdEstatus == 1 && x.IdCliente == pIdCliente) select b;

                return unidades.ToList();
            }
        }


        public void UpdateUnidadNegocioEspeciales(int pIdUnidadNegocio,string SeptimoDia, string PrestacionesEntero, string CuotaSindical, string CargasSFaltas, string DiasEquiv, string CobroCops, string RetenciISRSMGV, string SubirArchivo, string GeneraIntegrado, string Isr74, string NCargaObrera, string NCargaPatronal, string FechaInicio, string FechaFin, string CAA, string AEC, int? DImss, int? DImssB, string DaMas, string DaMenos, string DaMasF, string DaMenosF, string ISRM, decimal? ISRC, string FFD, string FEC, string FS, string FTM, int pIdUsuario)
        {

            try
            {
                if (bool.Parse(SeptimoDia) == true)
                {
                    SeptimoDia = "S";
                }
                else
                {
                    SeptimoDia = "N";

                }

                if (bool.Parse(PrestacionesEntero) == true)
                {
                    PrestacionesEntero = "S";
                }
                else
                {
                    PrestacionesEntero = "N";

                }



                if (bool.Parse(FFD) == true)
                {
                    FFD = "S";
                }
                else
                {
                    FFD = "N";

                }

                if (bool.Parse(FEC) == true)
                {
                    FEC = "S";
                }
                else
                {
                    FEC = "N";

                }

                if (bool.Parse(FS) == true)
                {
                    FS = "S";
                }
                else
                {
                    FS = "N";

                }

                if (bool.Parse(FTM) == true)
                {
                    FTM = "S";
                }
                else
                {
                    FTM = "N";

                }

                if (bool.Parse(ISRM) == true)
                {
                    ISRM = "S";
                }
                else
                {
                    ISRM = "N";

                }

                if (bool.Parse(DaMasF) == true)
                {
                    DaMasF = "S";
                }
                else
                {
                    DaMasF = "N";

                }


                if (bool.Parse(DaMenosF) == true)
                {
                    DaMenosF = "S";
                }
                else
                {
                    DaMenosF = "N";

                }


                if (bool.Parse(DaMas) == true)
                {
                    DaMas = "S";
                }
                else
                {
                    DaMas = "N";

                }


                if (bool.Parse(DaMenos) == true)
                {
                    DaMenos = "S";
                }
                else
                {
                    DaMenos = "N";

                }




                if (bool.Parse(AEC) == true)
                {
                    AEC = "S";
                }
                else
                {
                    AEC = "N";

                }

                
                if (bool.Parse(CAA) == true)
                {
                    CAA = "S";
                }
                else
                {
                    CAA = "N";

                }



                if (bool.Parse(CuotaSindical) == true)
                {
                    CuotaSindical = "S";
                }
                else
                {
                    CuotaSindical = "N";

                }

                if (bool.Parse(CargasSFaltas) == true)
                {
                    CargasSFaltas = "S";
                }
                else
                {
                    CargasSFaltas = "N";

                }
                if (bool.Parse(DiasEquiv) == true)
                {
                    DiasEquiv = "SI";
                }
                else
                {
                    DiasEquiv = "N";

                }
                if (bool.Parse(CobroCops) == true)
                {
                    CobroCops = "S";
                }
                else
                {
                    CobroCops = "N";

                }
                if (bool.Parse(RetenciISRSMGV) == true)
                {
                    RetenciISRSMGV = "S";
                }
                else
                {
                    RetenciISRSMGV = "N";

                }
                if (bool.Parse(SubirArchivo) == true)
                {
                    SubirArchivo = "S";
                }
                else
                {
                    SubirArchivo = "N";

                }
                if (bool.Parse(GeneraIntegrado) == true)
                {
                    GeneraIntegrado = "S";
                }
                else
                {
                    GeneraIntegrado = "N";

                }
                if (bool.Parse(Isr74) == true)
                {
                    Isr74 = "S";
                }
                else
                {
                    Isr74 = "N";

                }
                if (bool.Parse(NCargaObrera) == true)
                {
                    NCargaObrera = "S";
                }
                else
                {
                    NCargaObrera = "N";

                }
                if (bool.Parse(NCargaPatronal) == true)
                {
                    NCargaPatronal = "S";
                }
                else
                {
                    NCargaPatronal = "N";

                }



                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    Cat_UnidadNegocio unidad = (from b in entidad.Cat_UnidadNegocio
                                                where b.IdUnidadNegocio == pIdUnidadNegocio
                                                select b).FirstOrDefault();

                    if (unidad != null)
                    {
                        unidad.SeptimoDia = SeptimoDia;
                        unidad.BanderaCuotaSindical = CuotaSindical;
                        unidad.BanderaCargasSocialesSinFaltas = CargasSFaltas;
                        unidad.BanderaDiasEquivalentes = DiasEquiv;
                        unidad.CobraCOPS_Empleado_SMGV = CobroCops;
                        unidad.RetencionISR_SMGV = RetenciISRSMGV;
                        unidad.ValidaFechaSubirArchivo = SubirArchivo;
                        unidad.GenerarIntegradoPVyAgui = GeneraIntegrado;
                        unidad.ISRAguinaldoL174 = Isr74;
                        unidad.IdModificacion = pIdUsuario;
                        unidad.NoCalcularCargaObrera = NCargaObrera;
                        unidad.NoCalcularCargaPatronal = NCargaPatronal;
                        unidad.FechaInicioValidacionSubir = FechaInicio;
                        unidad.FechaFinValidacionSubir = FechaFin;

                        unidad.ConsideraAusentismosEnAguinaldo = CAA;
                        unidad.AguinaldoExentoCompleto = AEC;
                        unidad.FechaModificacion = DateTime.Now;
                        unidad.DIasImss = DImss;
                        unidad.DiasMenosImss = DImssB;
                        unidad.DiasAltaMas = DaMas;
                        unidad.DiasAltaMenos = DaMenos;
                        unidad.DiasAltaMasFraccionados = DaMasF;
                        unidad.DiasAltaMenosFraccionados = DaMenosF;
                        unidad.ISRProyeccionMensual = ISRM;
                        unidad.FactorDiasMesISR = ISRC;
                        unidad.FiniquitosFechasDiferentes = FFD;
                        unidad.FiniquitosExentoCompleto = FEC;
                        unidad.FiniquitosSubsidio = FS;
                        unidad.FiniquitosTablaMensual = FTM;
                        unidad.BanderaPrestacionesPatronEnteros = PrestacionesEntero;
                        entidad.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }



        /// <summary>
        /// Método para mostrar las unidades de negocio por Id.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public Cat_UnidadNegocio getUnidadesnegocioId(int IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var unidades = entidad.Cat_UnidadNegocio.Where(x => x.IdUnidadNegocio == IdUnidadNegocio).FirstOrDefault();

                return unidades;
            }
        }

        /// <summary>
        /// Método para mostrar las unidades de negocio por Id.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public vUnidadesNegocio getvUnidadesnegocioId(int IdUnidadNegocio)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var unidades = entidad.vUnidadesNegocio.Where(x => x.IdUnidadNegocio == IdUnidadNegocio).FirstOrDefault();

                return unidades;
            }
        }

        /// <summary>
        /// Método para listar las unidades de negocio que contengan algún Id específico.
        /// </summary>
        /// <param name="Ids">Recibe un arreglo de identificadores de unidad negocio.</param>
        /// <returns>Regresa la lista de unidades de negocio que contengan el arreglo de identificadores.</returns>
        public List<Cat_UnidadNegocio> getUnidadesnegocio(int[] Ids)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var unidades = from b in entidad.Cat_UnidadNegocio.Where(x => Ids.Contains(x.IdUnidadNegocio)) select b;

                return unidades.ToList();
            }
        }

        /// <summary>
        /// Método para listar el tipo de nómina.
        /// </summary>
        /// <returns>Regresa la lista de tipos de nómina.</returns>
        public List<ModelTipoNomina> getTipoNomina()
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                List<ModelTipoNomina> listmodelTipoNomina = new List<ModelTipoNomina>();

                foreach (var item in entidad.Cat_TipoNomina.OrderBy(x => x.TipoNomina).GroupBy(o => new { o.IdTipoNomina }).Select(o => o.FirstOrDefault()).ToList())
                {
                    ModelTipoNomina modelTipoNomina = new ModelTipoNomina();
                    modelTipoNomina.IdTipoNomina = item.IdTipoNomina;
                    modelTipoNomina.TipoNomina = item.TipoNomina + " - " + item.DiasPago + " - " + item.Observaciones;

                    listmodelTipoNomina.Add(modelTipoNomina);
                }

                return listmodelTipoNomina;
            }
        }

        // <summary>
        /// Método para obtener el tipo de nómina en base a su Id.
        /// </summary>
        /// <returns>Regresa el registro del tipo de nómina del Id proporcionado</returns>
        public Cat_TipoNomina getTipoNominaById(int IdTipoNomina)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var tipo = entidad.Cat_TipoNomina.Where(x => x.IdTipoNomina == IdTipoNomina).FirstOrDefault();

                return tipo;
            }
        }


        /// <summary>
        /// Método para agregar una unidad de negocio.
        /// </summary>
        /// <param name="model">Recibe el modelo de la unidad negocio.</param>
        /// <param name="pIdusuario">Recibe el identificador del usuario.</param>
        /// <param name="pIdCliente">Recibe el identificador del cliente.</param>
        public void AddUnidadNegocio(ModelUnidadesNegocio model, int pIdusuario, int pIdCliente)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                Cat_UnidadNegocio unidad = new Cat_UnidadNegocio()
                {
                    IdCliente =pIdCliente,
                    UnidadNegocio = model.UnidadNegocio,
                    IdTipoNomina = (int)model.IdTipoNomina,
                    PorcentajeISN = model.PorcentajeISN,                    
                    ConfiguracionSueldos = model.ConfiguracionSueldos,
                    IdEstatus = 1,
                    IdCaptura = pIdusuario,
                    FechaCaptura = DateTime.Now
                };

                entidad.Cat_UnidadNegocio.Add(unidad);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para obtener una unidad de negocio por un Id específico.
        /// </summary>
        /// <param name="pIdUnidadnegocio">Recibe el identificador de la unidad negocio.</param>
        /// <returns>Regresa el modelo de la unidad negocio por identificador específico.</returns>
        public ModelUnidadesNegocio GetModelUnidadesNegocioById(int pIdUnidadnegocio)
        {
            ModelUnidadesNegocio modelUnidad = new ModelUnidadesNegocio();

            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                vUnidadesNegocio unidad = (from b in entidad.vUnidadesNegocio
                                           where b.IdUnidadNegocio == pIdUnidadnegocio
                                           select b).FirstOrDefault();

                if (unidad != null)
                {
                    modelUnidad = new ModelUnidadesNegocio()
                    {
                        IdUnidadNegocio = unidad.IdUnidadNegocio,
                        IdCliente = unidad.IdCliente,
                        Cliente = unidad.Cliente,
                        UnidadNegocio = unidad.UnidadNegocio,
                        IdTipoNomina = unidad.IdTipoNomina,
                        PorcentajeISN = unidad.PorcentajeISN,                      
                        ConfiguracionSueldos = unidad.ConfiguracionSueldos
                    };
                }

                return modelUnidad;
            }
        }

        /// <summary>
        /// Método para modificar una unidad de negocio.
        /// </summary>
        /// <param name="pIdUnidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <param name="model">Recibe el modelo de la unidad negocio.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void UpdateUnidadNegocio(int pIdUnidadNegocio, ModelUnidadesNegocio model, int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                Cat_UnidadNegocio unidad = (from b in entidad.Cat_UnidadNegocio
                                            where b.IdUnidadNegocio == pIdUnidadNegocio
                                            select b).FirstOrDefault();

                if (unidad != null)
                {
                    unidad.UnidadNegocio = model.UnidadNegocio;
                    unidad.IdTipoNomina = (int)model.IdTipoNomina;
                    unidad.PorcentajeISN = model.PorcentajeISN;                    
                    unidad.ConfiguracionSueldos = model.ConfiguracionSueldos;
                    unidad.IdModificacion = pIdUsuario;
                    unidad.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }

            }
        }

        /// <summary>
        /// Método para modificar una unidad de negocio, con respecto a la información de la liquidación.
        /// </summary>
        /// <param name="pIdUnidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <param name="model">Recibe el modelo de la unidad negocio.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void UpdateUnidadNegocioDatosLiquidacion(int pIdUnidadNegocio, bool LiquidacionSDI, string ConceptosIntegran)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                Cat_UnidadNegocio unidad = entidad.Cat_UnidadNegocio.Where(b => b.IdUnidadNegocio == pIdUnidadNegocio).FirstOrDefault();                                            

                if (unidad != null)
                {
                    unidad.CalcularLiquidacionSDI = LiquidacionSDI ? "S" : null;
                    unidad.ConceptosSDILiquidacion = ConceptosIntegran;

                    entidad.SaveChanges();
                }

            }
        }

        /// <summary>
        /// Método para activar una advertencia cuando hay incidencias en los aguinaldos. 
        /// </summary>
        /// <param name="pIdUnidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <param name="IncidenciasAutomaticas">Recibe el identificador de la incidencia.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void ActualizaBanderaIncidenciasAguinaldo(int pIdUnidadNegocio, int? IncidenciasAutomaticas, int pIdUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                Cat_UnidadNegocio unidad = (from b in entidad.Cat_UnidadNegocio
                                            where b.IdUnidadNegocio == pIdUnidadNegocio
                                            select b).FirstOrDefault();

                if (unidad != null)
                {
                    unidad.IncidenciasAguinaldoAutomaticas = IncidenciasAutomaticas;
                    unidad.IdModificacion = pIdUsuario;
                    unidad.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }

            }
        }

        /// <summary>
        /// Método para eliminar un registro en las unidades de negocio.
        /// </summary>
        /// <param name="pIdunidadNegocio">Recibe el identificador de la unidad negocio.</param>
        /// <param name="pIdUsuario">Recibe el identificador del usuario.</param>
        public void DeleteUnidadNegocio(int pIdunidadNegocio,int pIdUsuario)
        {
            using (TadaNominaEntities entidad= new TadaNominaEntities())
            {
                Cat_UnidadNegocio unidad = (from b in entidad.Cat_UnidadNegocio
                                            where b.IdUnidadNegocio == pIdunidadNegocio
                                            select b).FirstOrDefault();

                if (unidad != null)
                {
                    unidad.IdModificacion = pIdUsuario;
                    unidad.FechaModificacion = DateTime.Now;
                    unidad.IdEstatus = 2;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método que lista las unidades de negocio por cliente específico.
        /// </summary>
        /// <param name="IdCliente">Recibe el identificador del cliente.</param>
        /// <param name="Token">Recibe una variable tipo string.</param>
        /// <returns>Regresa el listado de las unidades negocio.</returns>
        public List<SelectListItem> getSelectUnidadNegocio(int IdCliente, string Token)
        {
            var sUnidad = new sUnidadNegocio();
            var select = new List<SelectListItem>();
            var unidades = sUnidad.getSelectUnidadesNegocio(IdCliente, Token);

            unidades.ForEach(x=> { select.Add(new SelectListItem { Value = x.IdUnidadNegocio.ToString(), Text = x.UnidadNegocio  }); });

            return select;
        }

        public Cat_Clientes getClienteById(int IdCliente)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                var cliente = entidad.Cat_Clientes.Where(x => x.IdCliente == IdCliente).FirstOrDefault();

                return cliente;
            }
        }



    }
}