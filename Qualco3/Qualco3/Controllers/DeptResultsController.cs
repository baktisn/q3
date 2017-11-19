using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Db.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Qualco3.Controllers
{
    public class DeptResultsController : Controller
    {

        public IActionResult DeptResults(DeptResults model)
        {
            ///model.ErrorLines = TempData["list"] as List<ErrorLines>;
            return View("DeptResultsView",model);
        }
    }
}
