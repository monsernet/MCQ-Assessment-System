using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace MCQInterviews.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await RoleSeeder.SeedRolesAsync(roleManager);
            await RoleSeeder.SeedAdminAsync(userManager);
        }


    }
}
