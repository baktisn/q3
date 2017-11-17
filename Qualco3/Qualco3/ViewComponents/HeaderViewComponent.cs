using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Qualco3.ViewComponents
{
    [Authorize]
    public class HeaderViewComponent : ViewComponent
    {

        public HeaderViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View();
        }
    }
}
