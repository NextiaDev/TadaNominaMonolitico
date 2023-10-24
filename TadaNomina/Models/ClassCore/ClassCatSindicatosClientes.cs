using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;

namespace TadaNomina.Models.ClassCore
{
    public class ClassCatSindicatosClientes
    {

        public List<Cat_SindicatoCliente> getSindicatosCliente(int idCliente)
        {
            using (NominaEntities1 bd = new NominaEntities1())
            {
                var Sindicatos = (from b in bd.Cat_SindicatoCliente
                            where b.IdEstatus == 1 && b.idCliente == idCliente
                                  select b);

                return Sindicatos.ToList();
            }
        }

        public void addSindicatos(ModelCatSindicatosClientes model, int idcliente, int idusuario)
        {
            using (NominaEntities1 bd = new NominaEntities1())
            {
                Cat_SindicatoCliente ar = new Cat_SindicatoCliente()
                {
                    idCliente = idcliente,
                    Sindicato = model.Sindicato,
                    CuotaSindical = model.CuotaSindical,
                    IdEstatus = 1,
                    IdCaptura = idusuario,
                    FechaCaptura = DateTime.Now
                };

                bd.Cat_SindicatoCliente.Add(ar);
                bd.SaveChanges();
            }
        }


        public ModelCatSindicatosClientes GetModelSindicato(int idSindicato)
        {
            string fecha = (DateTime.Now).ToString();

            using (NominaEntities1 entidad = new NominaEntities1())
            {
                ModelCatSindicatosClientes model = new ModelCatSindicatosClientes();
                var cc = (from b in entidad.Cat_SindicatoCliente
                          where b.idCatSindicatoCliente == idSindicato
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    model.Sindicato = cc.Sindicato;
                    model.CuotaSindical = (decimal)cc.CuotaSindical;
                }

                return model;
            }
        }


        public void UpdateSindicatos(int idSindicato, int Idusuario, ModelCatSindicatosClientes model)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.Cat_SindicatoCliente
                          where b.idCatSindicatoCliente == idSindicato
                          select b).FirstOrDefault();

                if (cc != null)
                {
                    cc.Sindicato = model.Sindicato;
                    cc.CuotaSindical = model.CuotaSindical;

                    cc.IdModificacion = Idusuario;
                    cc.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }

            }
        }



        public void DeleteSindicatos(int idSindic, int pIdUsuario)
        {
            using (NominaEntities1 entidad = new NominaEntities1())
            {
                var cc = (from b in entidad.Cat_SindicatoCliente
                          where b.idCatSindicatoCliente == idSindic
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