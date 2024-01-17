using System.Web.Mvc;
using TadaNomina.Models.ViewModels.Catalogos;
using TadaNomina.Services;

namespace TadaNomina.Controllers.Administracion
{
    public class ClientesController : BaseController
    {        
        /// <summary>
        /// Acción que muestra la lista de clientes.
        /// </summary>
        /// <returns>Regresa la vista con la lista de los clientes.</returns>
        public ActionResult Index()
        {
            //if (Session["sTipoUsuario"].ToString() != "System") { return RedirectToAction("Index", "Default"); }
            //else
            //{
                string token = Session["sToken"].ToString();
                var clsCliente = new sClientes();
                var lClientes = clsCliente.getListClientes(token);

                return View(lClientes);
            //}
        }
                
        /// <summary>
        /// Acción que muestra los detalles de un cliente específico.
        /// </summary>
        /// <param name="id">Recibe el identificador del cliente.</param>
        /// <returns>Regresa una vista parcial con el detalle del cliente específico.</returns>
        public ActionResult Details(int id)
        {
            string token = Session["sToken"].ToString();
            var clsCliente = new sClientes();
            return PartialView(clsCliente.getModelClienteByID(id, token));
        }
        
        /// <summary>
        /// Acción que genera un registro de cliente.
        /// </summary>
        /// <returns>Regresa la vista con el modelo del cliente.</returns>
        public ActionResult Create()
        {
            sClientes service = new sClientes();
            string token = Session["sToken"].ToString();
            var clientes = service.GetInfoToCreate(token);
            return View(clientes);
        }
        
        /// <summary>
        /// Acción que guarda el registro del cliente.
        /// </summary>
        /// <param name="collection">Recibe el modelo del cliente.</param>
        /// <returns>Regresa la vista de clientes.</returns>
        [HttpPost]
        public ActionResult Create(ModelClientes collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var clsClientes = new sClientes();
                    string token = Session["sToken"].ToString();
                    clsClientes.AddCliente(collection, token);
                    return RedirectToAction("Index");
                }
                else
                {
                    var clientes = new ModelClientes();
                    return View(clientes);
                }                
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Acción para modificar un cliente.
        /// </summary>
        /// <param name="id">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la vista con el modelo de clientes.</returns>
        public ActionResult Edit(int id)
        {
            var clsCliente = new sClientes();
            string token = Session["sToken"].ToString();
            return View(clsCliente.getModelClienteByID(id, token));
        }
        
        /// <summary>
        /// Acción para guardar los cambios del cliente.
        /// </summary>
        /// <param name="id">Recibe el identificador del cliente.</param>
        /// <param name="collection">Recibe el modelo de clientes.</param>
        /// <returns>Regresa la vista de la lista de clientes.</returns>
        [HttpPost]
        public ActionResult Edit(int id, ModelClientes collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var clsClientes = new sClientes();                    
                    string token = Session["sToken"].ToString();
                    clsClientes.UpdateCliente(collection, token);
                    return RedirectToAction("Index");
                }
                else
                {
                    var clientes = new ModelClientes();
                    return View(clientes);
                }                
            }
            catch
            {
                return View();
            }
        }
        
        /// <summary>
        /// Acción para eliminar un registro de clientes.
        /// </summary>
        /// <param name="id">Recibe el identificador del cliente.</param>
        /// <param name="collection">Recibe el modelo de clientes.</param>
        /// <returns>Regresa una vista parcial de la confirmación de la eliminación del registro.</returns>
        public ActionResult Delete(int id, ModelClientes collection)
        {
            var clsCliente = new sClientes();
            string token = Session["sToken"].ToString();
            return PartialView(clsCliente.getModelClienteByID(id, token));
        }
        
        /// <summary>
        /// Acción para eliminar un registro de clientes.
        /// </summary>
        /// <param name="id">Recibe el identificador del cliente.</param>
        /// <returns>Regresa la vista con la lista de clientes.</returns>
        [HttpPost]
        public ActionResult Delete(ModelDeleteCliente model)
        {
            try
            {
                var clsClientes = new sClientes();
                int idUsuario = (int)Session["sIdUsuario"];
                string token = Session["sToken"].ToString();
                clsClientes.DeleteCliente(model, token);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
