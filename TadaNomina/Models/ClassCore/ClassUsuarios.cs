using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TadaNomina.Models.DB;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Models.ViewModels;
using System.Data.Entity.Core.Objects;
using System.Web.Mvc;
using System.Data;
using TadaNomina.Services;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace TadaNomina.Models.ClassCore
{
    public class ClassUsuarios
    {
        /// <summary>
        /// Método que lista los usuarios activos.
        /// </summary>
        /// <returns>Regresa la lista de usuarios activos.</returns>
        public List<Usuarios> GetUsuarios()
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var usuarios = from b in entidad.Usuarios.Where(x => x.IdEstatus == 1) select b;

                return usuarios.ToList();
            }
        }

        /// <summary>
        /// Método que agrega un usuario.
        /// </summary>
        /// <param name="u">Recibe el modelo usuarios.</param>
        /// <param name="IdCaptura">Recibe el identificador del usuario que captura los datos.</param>
        public void newUsuario(ModelUsuarios u, int IdCaptura)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                u.Usuario = u.Usuario.ToUpper();
                u.TipoUsuario = "Usuario";
                string query = "insert into Usuarios ([Nombre], [ApellidoPaterno], [ApellidoMaterno], [Correo], [Usuario], [Contraseña], [TipoUsuario], [IdCaptura], [IdEstatus]) values('" + u.Nombre + "', '" + u.ApellidoPaterno + "', '" + u.ApellidoMaterno + "', '" + u.Correo + "', '" + u.Usuario + "', pwdencrypt('" + u.Contraseña + "'), '" + u.TipoUsuario + "', " + IdCaptura + ", 1 )";

                entidad.Database.ExecuteSqlCommand(query);
                entidad.SaveChanges();
            }
        }

        /// <summary>
        /// Método que elimina el registro de un usuario.
        /// </summary>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <param name="IdModifica">Recibe el identificador del usuario que modifica los datos.</param>
        public void DeleteUser(int IdUsuario, int IdModifica)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var usuario = (from b in entidad.Usuarios.Where(x => x.IdUsuario == IdUsuario) select b).FirstOrDefault();

                if (usuario != null)
                {
                    usuario.IdEstatus = 2;
                    usuario.IdModificacion = IdModifica;
                    usuario.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para modificar los datos de un usuario.
        /// </summary>
        /// <param name="Idusuario">Recibe el identificador del usuario.</param>
        /// <param name="m">Recibe el modelo de usuarios.</param>
        /// <param name="IdModifica">Recibe el identificador del usuario que captura los datos.</param>
        public void EditeUser(int Idusuario, ModelUsuarios m, int IdModifica)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var usuario = (from b in entidad.Usuarios.Where(x => x.IdUsuario == Idusuario) select b).FirstOrDefault();

                m.Usuario = m.Usuario.ToUpper();
                if (usuario != null)
                {
                    usuario.Nombre = m.Nombre;
                    usuario.ApellidoPaterno = m.ApellidoPaterno;
                    usuario.ApellidoMaterno = m.ApellidoMaterno;
                    usuario.Correo = m.Correo;
                    usuario.Usuario = m.Usuario;
                    usuario.IdModificacion = IdModifica;
                    usuario.FechaModificacion = DateTime.Now;

                    entidad.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método que lista los datos del usuario.
        /// </summary>
        /// <returns>Regresa la lista del modelo de usuarios.</returns>
        public List<ModelUsuarios> GetModelUsuarios()
        {
            List<ModelUsuarios> modelusers = new List<ModelUsuarios>();
            List<Usuarios> users = GetUsuarios();

            foreach (var user in users)
            {
                ModelUsuarios modelUser = new ModelUsuarios
                {
                    IdUsuario = user.IdUsuario,
                    IdCliente = user.IdCliente,
                    Nombre = user.Nombre,
                    ApellidoPaterno = user.ApellidoPaterno,
                    ApellidoMaterno = user.ApellidoMaterno,
                    Usuario = user.Usuario,
                    Correo = user.Correo,
                    TipoUsuario = user.TipoUsuario,
                    Foto = user.ImagenUsuario != null ? Convert.ToBase64String(user.ImagenUsuario) : ""
            };

                modelusers.Add(modelUser);
            }

            return modelusers;
        }

        /// <summary>
        /// Método para mostrar los datos de los usuarios.
        /// </summary>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el modelo usuarios.</returns>
        public ModelUsuarios GetModelUsuarios(int IdUsuario)
        {            
            Usuarios users = GetVUsuario(IdUsuario);

            ModelUsuarios modelUser = new ModelUsuarios
            {
                IdUsuario = users.IdUsuario,
                IdCliente = users.IdCliente,
                Nombre = users.Nombre,
                ApellidoPaterno = users.ApellidoPaterno,
                ApellidoMaterno = users.ApellidoMaterno,
                Usuario = users.Usuario,
                Correo = users.Correo,
                TipoUsuario = users.TipoUsuario,
                ClientesAcceso = getClientesDescripcion(users.IdCliente, users.TipoUsuario),
                UnidadesAcceso = getUnidadesDescripcion(users.IdUnidadNegocio, users.TipoUsuario),
                
            };                

            return modelUser;
        }

        public string getClientesDescripcion(string clientes, string TipoUsuario)
        {            
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                string clientesAcceso = "";
                if (TipoUsuario == "System")
                {                    
                    clientesAcceso = "Todos";
                }
                else
                {
                    if (clientes != "")
                    {
                        string[] _clientes = clientes.Replace(" ", "").Split(',').ToArray();
                        int[] clientes_ = Array.ConvertAll(_clientes, int.Parse);

                        var cliente = entidad.Cat_Clientes.Where(x => x.IdEstatus == 1 && clientes_.Contains(x.IdCliente)).Select(x => x.Cliente).ToArray();

                        clientesAcceso = String.Join(",", cliente);
                    }
                }
                return clientesAcceso;
            }
        }

        public string getUnidadesDescripcion(string unidades, string TipoUsuario)
        {
            using (TadaNominaEntities entidad = new TadaNominaEntities())
            {
                string unidadesAcceso = "";
                if (TipoUsuario == "System")
                {
                    unidadesAcceso = "Todos";
                }
                else
                {
                    if (unidades != "")
                    {
                        string[] _unidades = unidades != null ? unidades.Replace(" ", "").Split(',').ToArray() : new string[0];
                        int[] unidades_ = Array.ConvertAll(_unidades, int.Parse);

                        var cliente = entidad.Cat_UnidadNegocio.Where(x => x.IdEstatus == 1 && unidades_.Contains(x.IdUnidadNegocio)).Select(x => x.UnidadNegocio).ToArray();

                        unidadesAcceso = String.Join(",", cliente);
                    }
                }
                return unidadesAcceso;
            }
        }
              
        /// <summary>
        /// Método para obtener un usuario por medio del Id.
        /// </summary>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el resultado de la consulta.</returns>
        public Usuarios GetVUsuario(int IdUsuario)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var usuario = (from b in entidad.Usuarios.Where(x => x.IdUsuario == IdUsuario) select b).FirstOrDefault();

                return usuario;
            }
        }

        /// <summary>
        /// Método para mostrar los datos en el cambio de contraseña.
        /// </summary>
        /// <param name="IdUsuario">Recibe el identificador del usuario.</param>
        /// <returns>Regresa el modelo del cambio de contraseña.</returns>
        public ModelCambioPass GetModelCambio(int IdUsuario)
        {            
            Usuarios user = GetVUsuario(IdUsuario);

            ModelCambioPass mcambio = new ModelCambioPass() {
                IdUsuario = user.IdUsuario,
                Usuario = user.Usuario,
                Nombre = user.Nombre + " " + user.ApellidoPaterno + " " + user.ApellidoMaterno
            };
            return mcambio;
        }

        /// <summary>
        /// Método para realizar el cambio de contraseña.
        /// </summary>
        /// <param name="model">Recibe el modelo de cambio de contraseña.</param>
        /// <returns>Regresa el modelo del cambio de contraseña.</returns>
        public ModelCambioPass CambiaPass(ModelCambioPass model)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {                
                if (model.NuevaContraseña == model.ConfirmaContraseña)
                {                    
                    ObjectParameter resultado = new ObjectParameter("resultado", 0);
                    var result = entidad.sp_CambioPass1(model.IdUsuario, model.Contraseña, model.NuevaContraseña, resultado);
                    
                    if (result == 1)
                    {
                        model.validacion = true;
                        model.Mensaje = "La contraseña se actualizo de forma correcta!";
                    }
                    else
                    {
                        model.validacion = false;
                        model.Mensaje = "NO se pudo actualizar la contraseña!";
                    }
                }
                else
                {
                    model.validacion = false;
                    model.Mensaje = "Las contraseñas no coinciden!";
                }

                return model;
            }
        }

        /// <summary>
        /// Método para obtener el perfil del usuario.
        /// </summary>
        /// <param name="IdUsuario"Recibe el identificador del usuario.></param>
        /// <param name="Token">Recibe una variable de tipo string.</param>
        /// <returns>Regresa el modelo del perfil de usuario.</returns>
        public ModelPerfil getPerfil(int IdUsuario, string Token)
        {
            var user = GetVUsuario(IdUsuario);

            var cc = new sClientes();
            ClassUnidadesNegocio cun = new ClassUnidadesNegocio();
            ModelPerfil perfil = new ModelPerfil();
            perfil.Nombre = user.Nombre + " " + user.ApellidoPaterno +  " " + user.ApellidoMaterno;
            var clientes = cc.getListClientes(Token);
            List<SelectListItem> slCli = new List<SelectListItem>();
            clientes.ForEach(x=> { slCli.Add(new SelectListItem { Text = x.Cliente, Value = x.IdCliente.ToString() }); });
            perfil.sLClientes = slCli;
            perfil.sLUnidades = new List<SelectListItem>();
                        
            if (user.IdCliente != null)
            {                
                string[] idsCli = user.IdCliente.Split(',').ToArray();
                int[] idsCliInt = Array.ConvertAll(idsCli, int.Parse);
                var cli = clientes.Where(x=> idsCliInt.Contains(x.IdCliente)).ToList();

                perfil.lClientes = cli;
            }
            else
            {
                perfil.lClientes = new List<Cat_Clientes>();
            }

            if (user.IdUnidadNegocio != null)
            {                
                string[] idsUni = user.IdUnidadNegocio.Split(',').ToArray();
                int[] idsUniInt = Array.ConvertAll(idsUni, int.Parse);
                var unidades = cun.getUnidadesnegocio(idsUniInt);

                perfil.lUnidad = unidades;
            }
            else
            {
                perfil.lUnidad = new List<Cat_UnidadNegocio>();
            }

            return perfil;
        }

        public string GuardaFoto(Image image, int IdUsuario)
        {           

            var newImagen = resizeImage(image, new Size(300, 300));
            var imgbyte = ImageToByteArray(newImagen);

            AddImage(IdUsuario, imgbyte);

            var nuevaImg = Convert.ToBase64String(imgbyte);

            return nuevaImg;
        }

        public void AddImage(int IdUsuario, byte[] img)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var usuario = entidad.Usuarios.Where(x => x.IdUsuario == IdUsuario).FirstOrDefault();

                if (usuario != null) 
                {
                    usuario.ImagenUsuario= img;

                    entidad.SaveChanges();
                }
            }
        }        

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        public void deleteImage(int IdUsuario)
        {
            using (TadaAccesoEntities entidad = new TadaAccesoEntities())
            {
                var usuario = entidad.Usuarios.Where(x => x.IdUsuario == IdUsuario).FirstOrDefault();

                if (usuario != null)
                {
                    usuario.ImagenUsuario = null;

                    entidad.SaveChanges();
                }
            }
        }
    }
}