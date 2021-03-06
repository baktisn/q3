﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Qualco3.Common;
using Db.Models;
using System.Security.Claims;
using Qualco3.Common.Extensions;
using System;
using Microsoft.AspNetCore.Authorization;


namespace Qualco3.ViewComponents
{   [Authorize]
    [Route("[controller]/[action]")]
    public class SidebarViewComponent : ViewComponent
    {
        public SidebarViewComponent()
        {
        }
      
        [Authorize(Roles = "Admin")]
        public IViewComponentResult Invoke(string filter)
        {
            //you can do the access rights checking here by using session, user, and/or filter parameter
            var sidebars = new List<SidebarMenu>();
            //if (((ClaimsPrincipal)User).GetUserProperty("AccessProfile").Contains("VES_008, Payroll"))
            //{
            //}


            sidebars.Add(ModuleHelper.AddModule(ModuleHelper.Module.Home));
            if (!User.IsInRole("Admin"))
            {
                sidebars.Add(ModuleHelper.AddModule(ModuleHelper.Module.Bills));
                sidebars.Add(ModuleHelper.AddModule(ModuleHelper.Module.Contact));
            }

            if (User.IsInRole("Admin"))
            { 
                sidebars.Add(ModuleHelper.AddTree("Admin"));
                sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                ModuleHelper.AddModule(ModuleHelper.Module.GetFile),
                ModuleHelper.AddModule(ModuleHelper.Module.PostFile),
              
            };
            }

            return View(sidebars);
        }
    }
}

