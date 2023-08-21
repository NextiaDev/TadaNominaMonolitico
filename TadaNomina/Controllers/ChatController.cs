using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TadaNomina.Models.ClassCore;

namespace TadaNomina.Controllers
{
    public class ChatController : BaseController
    {
        // GET: Chat
        public ActionResult Index()
        {
            ClassUsuarios cu = new ClassUsuarios(); 

            var model = cu.GetUsuarios().Select(x=>x.Usuario).ToList();
                        

            return View(model);
        }
    }
}