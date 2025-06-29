using MCQInterviews.Models.ViewModels;

namespace MCQInterviews.Repositories.Admin
{
    public interface IAdminDashboardRepository
    {
        Task<AdminDashboardViewModel> GetAdminDashboardDataAsync();


    }
}
