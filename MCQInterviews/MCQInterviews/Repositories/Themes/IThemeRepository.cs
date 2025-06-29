using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.Themes
{
    public interface IThemeRepository
    {
        Task<IEnumerable<Theme>> GetThemesAsync();
        Task<Theme?> GetThemeByIdAsync(int id);
        Task<Theme> AddThemeAsync(Theme theme);
        Task<Theme?> UpdateThemAsync(Theme theme);
        Task<Theme?> DeleteThemeAsync(Theme theme);
        Task<int> GetThemesCountAsync();
    }
}
