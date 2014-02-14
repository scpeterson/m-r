using System.Web.Mvc;

namespace SimpleCQRS.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return new RedirectResult("index.html", true);
        }
    }
}