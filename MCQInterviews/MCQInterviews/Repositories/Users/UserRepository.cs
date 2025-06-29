using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(
            ApplicaationDbContext applicaationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            this.applicaationDbContext = applicaationDbContext;
            this.userManager = userManager;
        }
        public async Task<ApplicationUser> ActivateUser(string userId)
        {
            var user = await applicaationDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException($"No user found with ID {userId}");
            }

            user.IsActive = 1;
            await applicaationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<ApplicationUser> DeactivateUser(string userId)
        {
            var user = await applicaationDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException($"No user found with ID {userId}");
            }

            user.IsActive = 0;
            await applicaationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<ApplicationUser> DeleteUser(string userId)
        {
            var user = await applicaationDbContext.Users.FindAsync(userId);

            if (user != null)
            {
                applicaationDbContext.Users.Remove(user);
                await applicaationDbContext.SaveChangesAsync();
                return user;
            }
            else
            {
                throw new InvalidOperationException($"No user found with ID {userId}");
            }
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var user = await applicaationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            var users = await applicaationDbContext.Users.ToListAsync();
            return users;
        }

        public async Task<IEnumerable<ApplicationUser>> GetNonAdminUsers()
        {

            var users = await applicaationDbContext.Users.ToListAsync();

            var nonAdminUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            return nonAdminUsers;
        }

        public async Task<UserLogin> LogUserLogin(string userId)
        {
            var user = await applicaationDbContext.Users.FindAsync(userId);

            if (user != null)
            {
                if (user.UserLogins == null)
                {
                    user.UserLogins = new List<UserLogin>();
                }
                // Create a new UserLogin record
                var userLogin = new UserLogin
                {
                    UserId = userId,
                    LoginDate = DateTime.UtcNow
                };

                // Associate the UserLogin with the ApplicationUser
                user.UserLogins.Add(userLogin);
                await applicaationDbContext.SaveChangesAsync();

                return userLogin;
            }
            return null;
        }

        public async Task<int> UserLogins(string userId)
        {
            var user = await applicaationDbContext.Users
                .Include(u => u.UserLogins)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.UserLogins?.Count ?? 0;
        }

        public async Task<int> GetNonAdminUserCountAsync()
        {
            var users = applicaationDbContext.Users.ToList();
            var nonAdminUsers = new List<IdentityUser>();

            foreach (var user in users)
            {
                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            return nonAdminUsers.Count;
        }
    }
}
