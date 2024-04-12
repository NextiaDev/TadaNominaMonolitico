using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.ClassCore.CalculoNomina;
using TadaNomina.Models.DB;
using System.Linq.Dynamic.Core;


namespace TadaNomina.Models.ClassCore
{
    public class ClassPensiones : ClassProcesosNomina
    {

        internal List<vConceptos> conceptosNominaFormula;
        internal List<FormulasEquivalencias> tablaEquivalencias;
        internal List<vIncidencias> incidenciasEmpleado;
        public decimal montoPension { get; set; }
        public decimal montoPensionEsq { get; set; }

        /// <summary>
        /// Método para listar las pensiones activas por unidad de negocio.
        /// </summary>
        /// <param name="IdUnidadNegocio">Recibe el parámetro del identificador de la unidad de negocio.</param>
        /// <returns>Regresa la lista de las pensiones alimenticias activas.</returns>
        public List<vPensionAlimenticia> getPensiones(int IdUnidadNegocio)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var pensiones = (from b in entidad.vPensionAlimenticia.Where(x => x.IdUnidadNegocio == IdUnidadNegocio && x.IdEstatus == 1) select b).ToList();

                return pensiones;
            }
        }


        /// <summary>
        /// Método que procesa la pensión alimenticia de un empleado específico.
        /// </summary>
        /// <param name="pension">Recibe el modelo de la vista de la pensión alimenticia.</param>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <param name="TotalPercepciones">Recibe la cantidad del total de las percepciones.</param>
        /// <param name="TotalPercepcionesEsquema">Recibe la cantidad del total de las percepciones de esquema.</param>
        /// <param name="IdConcepto">Recibe el identificador del concepto.</param>
        /// <param name="IdUsuario">Recibe el identificador del empleado.</param>
        //public void procesaPensionAlimenticia(vPensionAlimenticia pension, int IdPeriodoNomina, decimal TotalPercepciones, decimal TotalPercepcionesEsquema, int IdConcepto, int IdUsuario)
        //{
        //    eliminaIncidenciasPension(IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia);

        //    var ins = new Incidencias();

        //    montoPension = 0;
        //    montoPensionEsq = 0;
        //    switch (pension.TipoPension)
        //    {
        //        case "CuotaFija":
        //            try { montoPension = (decimal)pension.Valor; } catch { montoPension = 0; }
        //            try { montoPensionEsq = (decimal)pension.ValorEsq; } catch { montoPensionEsq = 0; }
        //            break;
        //        case "Porcentaje":
        //            try { montoPension = TotalPercepciones * ((decimal)pension.Valor / 100); } catch { montoPension = 0; }
        //            try { montoPensionEsq = TotalPercepcionesEsquema * ((decimal)pension.ValorEsq / 100); } catch { montoPensionEsq = 0; }
        //            break;
        //        default:
        //            montoPension = 0;
        //            montoPensionEsq = 0;
        //            break;
        //    }

        //    if (montoPension != 0 || montoPensionEsq != 0)
        //    {
        //        ins = creaIncidenciaPension((int)pension.IdEmpleado, IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia, montoPension, montoPensionEsq, IdUsuario);
        //        guardaIncidenciasPension(ins);
        //    }
        //}


        public void procesaPensionAlimenticia(List<vIncidencias> incidenciasEmpleado, NominaTrabajo nom, vEmpleados datosEmpleados, vPensionAlimenticia pension, int IdPeriodoNomina, decimal TotalPercepciones, decimal TotalPercepcionesEsquema, int IdConcepto, int IdUsuario, int idcliente)
        {
            ExpressionContext context = new ExpressionContext();

            GetListConceptosNominaFormula(idcliente);
            ClassCalculoNomina cl = new ClassCalculoNomina();
            var ins = new Incidencias();
            var conce = new vIncidencias();
            var bases = new BasePensionAlimenticia();
            decimal? valorCalculo = null;
            string Formula = string.Empty;


            if (pension.idBasePension != null || string.IsNullOrEmpty(pension.idBasePension.ToString()))
            {

                montoPension = 0;
                montoPensionEsq = 0;
                eliminaIncidenciasPension(IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia);

                bases = BasePension((int)pension.idBasePension);
                bases.Formula.Replace(" ", "").Replace("\n", "").Replace("\r", "").Split(';').ToList();
                Formula = GetFormulaConceptosNominaIncidencias(bases.Formula);
                Formula = GetFormulaTablaEquivalencias(nom, datosEmpleados, IdConcepto, bases.Formula, idcliente);
                Formula = Formula.Replace("VALOR_CONDICION", (valorCalculo ?? 0).ToString());

                IDynamicExpression e = context.CompileDynamic(Formula);
                var resul = Math.Round(Convert.ToDecimal(e.Evaluate()), 2);

                if (resul != 0)



                    switch (pension.TipoPension)
                    {
                        case "CuotaFija":
                            try { montoPension = resul; } catch { montoPension = 0; }
                            try { montoPensionEsq = resul; } catch { montoPensionEsq = 0; }
                            break;
                        case "Porcentaje":
                            try { montoPension = resul * ((decimal)pension.Valor / 100); } catch { montoPension = 0; }
                            try { montoPensionEsq = resul * ((decimal)pension.ValorEsq / 100); } catch { montoPensionEsq = 0; }
                            break;
                        default:
                            montoPension = 0;
                            montoPensionEsq = 0;
                            break;
                    }


                if (montoPension != 0 || montoPensionEsq != 0)
                {

                    ins = creaIncidenciaPension(datosEmpleados.IdEmpleado, IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia, montoPension, montoPensionEsq, IdUsuario, Formula);
                    guardaIncidenciasPension(ins);

                }



            }
            else
            {
                eliminaIncidenciasPension(IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia);

                montoPension = 0;
                montoPensionEsq = 0;
                switch (pension.TipoPension)
                {
                    case "CuotaFija":
                        try { montoPension = (decimal)pension.Valor; } catch { montoPension = 0; }
                        try { montoPensionEsq = (decimal)pension.ValorEsq; } catch { montoPensionEsq = 0; }
                        break;
                    case "Porcentaje":
                        try { montoPension = TotalPercepciones * ((decimal)pension.Valor / 100); } catch { montoPension = 0; }
                        try { montoPensionEsq = TotalPercepcionesEsquema * ((decimal)pension.ValorEsq / 100); } catch { montoPensionEsq = 0; }
                        break;
                    default:
                        montoPension = 0;
                        montoPensionEsq = 0;
                        break;
                }

                if (montoPension != 0 || montoPensionEsq != 0)
                {
                    ins = creaIncidenciaPension((int)pension.IdEmpleado, IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia, montoPension, montoPensionEsq, IdUsuario, Formula);
                    guardaIncidenciasPension(ins);
                }

            }

        }

        private void GetTablaEquivalencias(int IdCliente)
        {
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    tablaEquivalencias = entidad.FormulasEquivalencias.Where(x => x.IdEstatus == 1 && x.IdCliente == IdCliente).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        private string GetFormulaTablaEquivalencias(NominaTrabajo nom, vEmpleados datosEmpleados, int idconcepto, string Formula, int idcliente)
        {
            GetTablaEquivalencias(idcliente);

            foreach (var iEquiv in tablaEquivalencias)
            {
                var resultado = "0";
                if (Formula.Contains(iEquiv.Clave))
                {
                    if (tablaEquivalencias != null)
                    {
                        if (iEquiv.Tabla == "Nomina")
                        {
                            List<NominaTrabajo> lnomina = new List<NominaTrabajo>();
                            lnomina.Add(nom);

                            resultado = lnomina.AsQueryable().Select(iEquiv.Campo).Sum().ToString();
                        }

                        if (iEquiv.Tabla == "Empleados")
                        {
                            List<vEmpleados> lemp = new List<vEmpleados>();
                            lemp.Add(datosEmpleados);

                            resultado = lemp.AsQueryable().Select(iEquiv.Campo).Sum().ToString();
                        }

                        if (iEquiv.Tabla == "FactorIntegracion")
                        {
                            if (iEquiv.Clave == "SBC")
                            {
                                decimal factor = (prestaciones.Where(x => nominaTrabajo.Anios >= x.Limite_Inferior && nominaTrabajo.Anios <= x.Limite_Superior)
                                    .Select(x => x.FactorIntegracion).FirstOrDefault() ?? 1.0493M);

                                resultado = (SD_IMSS * factor).ToString();
                            }
                        }
                    }


                    Formula = Formula.Replace(iEquiv.Clave, resultado);
                }
            }

            return Formula;
        }


        public string GetFormulaTablaEquivalencias(vEmpleados datosEmpleados, string Formula, int idcliente)
        {
            try
            {
                var icform = new List<vConceptos>();
                var equiva = new List<FormulasEquivalencias>();

                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    icform = entidad.vConceptos.Where(x => x.IdCliente == idcliente && x.IdEstatus == 1).ToList();
                }

                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    equiva = entidad.FormulasEquivalencias.Where(x => x.IdCliente == idcliente && x.IdEstatus == 1).ToList();
                }

                foreach (var item in equiva)
                {
                    var resultado = "0";
                    foreach (var itema in icform)
                    {

                    }

                }

                return Formula;
            }
            catch (Exception)
            {

                throw;
            }


        }


        private string GetFormulaConceptosNominaIncidencias(string Formula)
        {
            try
            {
                foreach (var lc in conceptosNominaFormula)
                {
                    decimal monto = 0;

                    var claveConcepto = "\"" + lc.ClaveConcepto.Trim().ToUpper() + "\"";
                    if (Formula.Contains(claveConcepto))
                    {

                        //Se agrega una condiciones para calcular monto
                        if (lc.TipoDato == "Cantidades")
                            monto = incidenciasEmpleado.Where(x => x.ClaveConcepto == lc.ClaveConcepto).Select(x => x.Cantidad).Sum() ?? 0;
                        else
                            monto = incidenciasEmpleado.Where(x => x.ClaveConcepto == lc.ClaveConcepto).Select(x => x.Monto).Sum() ?? 0;

                        Formula = Formula.Replace(claveConcepto, monto.ToString());
                    }
                }

                return Formula;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public BasePensionAlimenticia BasePension(int idbase)
        {
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    var incidenccias = (from b in entidad.BasePensionAlimenticia.Where(x => x.IdBasePensionAlimenticia == idbase) select b).FirstOrDefault();

                    return incidenccias;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        private void GetListConceptosNominaFormula(int idCliente)
        {
            try
            {
                using (TadaNominaEntities entidad = new TadaNominaEntities())
                {
                    conceptosNominaFormula = entidad.vConceptos.Where(x => x.IdCliente == idCliente && x.IdEstatus == 1).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// Método que elimina una incidencia en la pensión.
        /// </summary>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <param name="IdConcepto">Recibe el identificador del concepto</param>
        /// <param name="IdPension">Recibe el identificador de la pensión.</param>
        public void eliminaIncidenciasPension(int IdPeriodoNomina, int IdConcepto, int IdPension)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var incidenccias = (from b in entidad.Incidencias.Where(x => x.IdPeriodoNomina == IdPeriodoNomina && x.IdConcepto == IdConcepto && x.BanderaPensionAlimenticia == IdPension) select b).ToList();

                entidad.Incidencias.RemoveRange(incidenccias);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método para generar el modelo de una incidencia en la pensión.
        /// </summary>
        /// <param name="IdEmpleado">Recibe el identificador del empleado.</param>
        /// <param name="IdPeriodoNomina">Recibe el identificador del periodo de nómina.</param>
        /// <param name="IdConcepto">Recibe el identificador del concepto.</param>
        /// <param name="IdPension">Recibe el identificador de la pensión.</param>
        /// <param name="Monto">Recibe el monto.</param>
        /// <param name="MontoEsq">Recibe el monto del esquema.</param>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el modelo con datos.</returns>
        public Incidencias creaIncidenciaPension(int IdEmpleado, int IdPeriodoNomina, int IdConcepto, int IdPension, decimal Monto, decimal MontoEsq, int IdUsuario, string formula)
        {
            Incidencias i = new Incidencias();
            i.IdEmpleado = IdEmpleado;
            i.IdPeriodoNomina = IdPeriodoNomina;
            i.IdConcepto = IdConcepto;
            i.Cantidad = 0;
            i.Monto = Monto;
            i.Exento = 0;
            i.Gravado = 0;
            i.MontoEsquema = MontoEsq;
            i.ExentoEsquema = 0;
            i.GravadoEsquema = 0;
            i.Observaciones = "PDUP SYSTEM Concepto creado por el sistema para los conceptos formulados";
            i.BanderaPensionAlimenticia = IdPension;
            i.FormulaEjecutada = formula;
            i.IdEstatus = 1;
            i.IdCaptura = IdUsuario;
            i.FechaCaptura = DateTime.Now;

            return i;
        }

        /// <summary>
        /// Método para guardar la incidencia de la pensión.
        /// </summary>
        /// <param name="lInc">Recibe el modelo de la incidencia.</param>
        public void guardaIncidenciasPension(Incidencias lInc)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                entidad.Incidencias.Add(lInc);
                entidad.SaveChanges();
            }
        }
    }
}