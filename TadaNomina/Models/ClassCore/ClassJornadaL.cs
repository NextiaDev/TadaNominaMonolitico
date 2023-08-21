using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    public class ClassJornadaL
    {

        public List<Cat_Jornadas> GetJornadas(int pIdCliente)
        {
            using (NominaEntities1 bd = new NominaEntities1())
            {
                var Area = (from b in bd.Cat_Jornadas
                            where b.IdEstatus == 1 && b.IdCliente == pIdCliente
                            select b);

                return Area.ToList();
            }
        }


        public ModelJornada GetJornadaid(int IdJornada)
        {
            string fecha = (DateTime.Now).ToString();

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ModelJornada model = new ModelJornada();
                var cc = (from b in entidad.Cat_Jornadas
                          where b.IdJornada == IdJornada
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    model.Clave = cc.Clave;
                    model.Jornada = cc.Jornada;
                    model.Horas = cc.Horas;
                    model.FechaCaptura = cc.FechaCaptura;

                }

                return model;
            }
        }


        public void addJornada(ModelJornada model, int idcliente, int idusuario)
        {
            using (NominaEntities1 bd = new NominaEntities1())
            {
                Cat_Jornadas ar = new Cat_Jornadas()
                {
                    IdCliente = idcliente,
                    Clave = model.Clave,
                    Jornada = model.Jornada,
                    Horas = model.Horas,
                    IdEstatus = 1,
                    IdCaptura = idusuario,
                    FechaCaptura = DateTime.Now
                };

                bd.Cat_Jornadas.Add(ar);
                bd.SaveChanges();
            }
        }


        public void UpdateJornada(int IdJornada, int Idusuario, ModelJornada model)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.Cat_Jornadas
                          where b.IdJornada == IdJornada
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    cc.Clave = model.Clave;
                    cc.Jornada = model.Jornada;
                    cc.Horas = model.Horas;
                    cc.IdModificacion = Idusuario;
                    cc.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }

            }
        }


        public void DeleteJornada(int IdJornada, int pIdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.Cat_Jornadas
                          where b.IdJornada == IdJornada
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    cc.IdModificacion = pIdUsuario;
                    cc.FechaModificacion = DateTime.Now;
                    cc.IdEstatus = 2;

                    entidad.SaveChanges();
                }
            }
        }

    }
}