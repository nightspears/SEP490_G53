﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TCViettetlFC_Client.Models;

namespace TCViettelFC_Client.Controllers.Admin
{
    public class TeamManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
