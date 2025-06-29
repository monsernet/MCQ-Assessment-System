using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    public class CandidateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
