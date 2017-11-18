using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Db.Models;
using Microsoft.AspNetCore.Authorization;
using Db.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Qualco3.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class HomeController : Controller

    {
        
        public IActionResult Index()
        {

            return View();


        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Μπορείτε να επικοινωνίσετε μαζί μας με τους παρκάτω τρόπους:";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
