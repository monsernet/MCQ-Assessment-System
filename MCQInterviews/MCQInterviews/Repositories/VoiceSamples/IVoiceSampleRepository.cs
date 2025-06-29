using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.VoiceSamples
{
    public interface IVoiceSampleRepository
    {
        Task<VoiceSample> GetVoiceSampleByIdAsync(int id);
        Task<IEnumerable<VoiceSample>> GetVoiceSamplesByCandidateIdAsync(string candidateId);
        Task AddVoiceSampleAsync(VoiceSample voiceSample);
        Task UpdateVoiceSampleAsync(VoiceSample voiceSample);
        Task DeleteVoiceSampleAsync(int id);
    }
}
