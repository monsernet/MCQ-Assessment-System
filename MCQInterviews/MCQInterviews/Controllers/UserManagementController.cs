using MCQInterviews.Repositories.Users;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUserRepository userRepository;

        public UserManagementController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<IActionResult> List()
        {
            var users = await userRepository.GetUsers();
            return View(users);
        }
    }
}
