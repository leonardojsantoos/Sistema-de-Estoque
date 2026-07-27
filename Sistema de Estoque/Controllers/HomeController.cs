using Microsoft.AspNetCore.Mvc;

namespace Sistema_de_Estoque.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.DataHora = DateTime.Now;
            return View();
        }
    }
}
