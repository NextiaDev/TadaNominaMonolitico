using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels;

namespace TadaNomina.Models.ClassCore
{
    public class ClassSindicatos
    {
        /// <summary>
        /// Método para listar los sindicatos.
        /// </summary>
        /// <returns>Regresa la lista de los sindicatos.</returns>
        public List<ModelSindicatos> GetSindicatos()
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var sindicatos = (from a in ctx.Sindicatos
                                  where a.IdEstatus==1
                                  select new ModelSindicatos
                                  {
                                      NombreSindicato=a.NombreSindicato,
                                      NombreCorto=a.NombreCorto,
                                      IdSindicato=a.IdSindicato,
                                      IdBanco=a.IdBanco,
                                      Grupo=a.Grupo
                                  });
                return sindicatos.ToList();
            }
        }

        /// <summary>
        /// Método para gregar un nuevo sindicato.
        /// </summary>
        /// <param name="s">Recibe el modelo del sindicato.</param>
        /// <param name="IdCaptura">Recibe el identificador del usuario que captura los datos.</param>
        public void newSindicato(ModelSindicatos s, int IdCaptura)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                Sindicatos obd = new Sindicatos();
                obd.NombreSindicato = s.NombreSindicato;
                obd.NombreCorto = s.NombreCorto;
                obd.Grupo = s.Grupo;
                obd.IdEstatus = 1;
                obd.IdCaptura = IdCaptura;
                obd.FechaCaptura = DateTime.Now;

                ctx.Sindicatos.Add(obd);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Método que muestra los datos del sindicato.
        /// </summary>
        /// <param name="IdSindicato">Recibe el identificador del sindicato.</param>
        /// <returns>Regresa el modelo del sindicato.</returns>
        public ModelSindicatos GetModelSindicatos(int IdSindicato)
        {
            Sindicatos s = GetSindicato(IdSindicato);
            ModelSindicatos ms = new ModelSindicatos
            {
                IdSindicato = s.IdSindicato,
                NombreSindicato = s.NombreSindicato,
                NombreCorto = s.NombreCorto,
                Grupo = s.Grupo,
                FechaModificacion = DateTime.Now
            };
            return ms;
        }

        /// <summary>
        /// Método para obtener un sindicato por medio de un Id específico.
        /// </summary>
        /// <param name="IdSindicato">Recibe el identificador del sindicato.</param>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public Sindicatos GetSindicato(int IdSindicato)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var s = (from b in ctx.Sindicatos.Where(x => x.IdSindicato == IdSindicato) select b).FirstOrDefault();
                return s;
            }
        }

        /// <summary>
        /// Método para modificar los datos de un sindicato.
        /// </summary>
        /// <param name="IdSindicato">Recibe el identificador del sindicato.</param>
        /// <param name="ms">Recibe el modelo del sindicato.</param>
        /// <param name="IdModificacion">Recibe el identificador de la modificación.</param>
        public void EditSindicato(int IdSindicato, ModelSindicatos ms, int IdModificacion)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var s = (from b in ctx.Sindicatos.Where(x => x.IdSindicato == IdSindicato) select b).FirstOrDefault();
                if (s != null)
                {
                    s.NombreSindicato = ms.NombreSindicato;
                    s.NombreCorto = ms.NombreCorto;
                    s.Grupo = ms.Grupo;
                    s.IdModificacion = IdModificacion;
                    s.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para eliminar un sindicato.
        /// </summary>
        /// <param name="IdSindicato">Recibe el identificador del sindicato.</param>
        /// <param name="IdModificacion">Recibe el identificador de la modificación.</param>
        public void DeleteUser(int IdSindicato, int IdModificacion)
        {
            using (TadaNominaEntities ctx = new TadaNominaEntities())
            {
                var s = (from b in ctx.Sindicatos.Where(x => x.IdSindicato == IdSindicato) select b).FirstOrDefault();
                if (s != null)
                {
                    s.IdEstatus = 2;
                    s.IdModificacion = IdModificacion;
                    s.FechaModificacion = DateTime.Now;

                    ctx.SaveChanges();
                }
            }
        }
    }
}