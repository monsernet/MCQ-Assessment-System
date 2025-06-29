using MCQInterviews.Models.Domain;

namespace MCQInterviews.Services
{
    public interface IVoiceSampleService
    {
        Task<VoiceSample> UploadVoiceSampleAsync(VoiceSample voiceSample);
        Task<VoiceSample> ApproveVoiceSampleAsync(int id, string feedback);
        Task<VoiceSample> RejectVoiceSampleAsync(int id, string feedback);
        Task<IEnumerable<VoiceSample>> GetAllPendingSamplesAsync();
    }
}
