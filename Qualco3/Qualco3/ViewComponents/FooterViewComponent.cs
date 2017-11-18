using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Qualco3.ViewComponents
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class FooterViewComponent : ViewComponent
    {

        public FooterViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View();
        }
    }
}
