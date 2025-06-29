using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.Themes
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;

        public ThemeRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }
        public async Task<Theme> AddThemeAsync(Theme theme)
        {
            if (theme == null)
            {
                return null;
            }
            await applicaationDbContext.Themes.AddAsync(theme);
            await applicaationDbContext.SaveChangesAsync();
            return theme;
        }

        public async Task<Theme?> DeleteThemeAsync(Theme theme)
        {
            applicaationDbContext.Themes.Remove(theme);
            await applicaationDbContext.SaveChangesAsync();
            return theme;

        }

        public async Task<Theme?> GetThemeByIdAsync(int id)
        {
            var theme = await applicaationDbContext.Themes.FirstOrDefaultAsync(x => x.Id == id);
            return theme;
        }

        public async Task<IEnumerable<Theme>> GetThemesAsync()
        {
            var themes = await applicaationDbContext.Themes.ToListAsync();
            return themes;
        }

        public async Task<Theme?> UpdateThemAsync(Theme theme)
        {
            var themeToUpdate = await applicaationDbContext.Themes.FindAsync(theme.Id);
            if (themeToUpdate != null)
            {
                themeToUpdate.Name = theme.Name;
                await applicaationDbContext.SaveChangesAsync();
                return themeToUpdate;
            }
            else
            {
                return null;
            }

        }
        public async Task<int> GetThemesCountAsync()
        {
            return await applicaationDbContext.Themes.CountAsync();
        }
    }
}
