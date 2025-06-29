using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.Users
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetUsers();
        Task<IEnumerable<ApplicationUser>> GetNonAdminUsers();
        Task<ApplicationUser> GetUserById(string userId);
        Task<ApplicationUser> ActivateUser(string userId);
        Task<ApplicationUser> DeactivateUser(string userId);
        Task<ApplicationUser> DeleteUser(string userId);
        Task<UserLogin> LogUserLogin(string userId);
        Task<int> UserLogins(string userId);
        Task<int> GetNonAdminUserCountAsync();
    }
}
