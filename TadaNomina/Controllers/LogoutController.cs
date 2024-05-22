﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TadaNomina.Controllers
{
    public class LogoutController : BaseController
    {
        // GET: Logout
        public ActionResult Index()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login"); 
        }
    }
}
