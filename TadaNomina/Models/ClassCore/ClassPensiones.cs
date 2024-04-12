using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;

namespace TadaNomina.Models.ClassCore
{
    public class ClassPensiones
    {
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
        public void procesaPensionAlimenticia(vPensionAlimenticia pension, int IdPeriodoNomina, decimal TotalPercepciones, decimal TotalPercepcionesEsquema, int IdConcepto, int IdUsuario)
        {
            eliminaIncidenciasPension(IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia);

            var ins = new Incidencias();

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
                ins = creaIncidenciaPension((int)pension.IdEmpleado, IdPeriodoNomina, IdConcepto, pension.IdPensionAlimenticia, montoPension, montoPensionEsq, IdUsuario);
                guardaIncidenciasPension(ins);
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
        public Incidencias creaIncidenciaPension(int IdEmpleado, int IdPeriodoNomina, int IdConcepto, int IdPension, decimal Monto, decimal MontoEsq, int IdUsuario)
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
            i.Observaciones = "PDUP SYSTEM";
            i.BanderaPensionAlimenticia = IdPension;
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