using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MaintenanceController : Controller
    {
        [HttpGet("/maintenance")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
