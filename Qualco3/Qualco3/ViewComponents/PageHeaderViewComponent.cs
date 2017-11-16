using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Qualco3.ViewComponents
{
    
    public class PageHeaderViewComponent : ViewComponent
    {

        public PageHeaderViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            Tuple<string, string> message;

            if (ViewBag.PageHeader == null)
            {
                message = Tuple.Create(string.Empty, string.Empty);
            }
            else
            {
                message = ViewBag.PageHeader as Tuple<string, string>;
            }
            return View(message);
        }
    }
}
