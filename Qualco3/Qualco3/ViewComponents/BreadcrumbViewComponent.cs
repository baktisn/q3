using Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Qualco3.ViewComponents
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class BreadcrumbViewComponent : ViewComponent
    {

        public BreadcrumbViewComponent()
        {
            
        }

        public IViewComponentResult Invoke(string filter)
        {
            if (ViewBag.Breadcrumb == null)
            {
                ViewBag.Breadcrumb = new List<Message>();
            }
            
            return View(ViewBag.Breadcrumb as List<Message>);
        }
    }
}
