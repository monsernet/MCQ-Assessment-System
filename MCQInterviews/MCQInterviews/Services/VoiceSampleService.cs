using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using MCQInterviews.Repositories.VoiceSamples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Services
{
    public class VoiceSampleService : IVoiceSampleService
    {
        private readonly IVoiceSampleRepository _voiceSampleRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VoiceSampleService(
            IVoiceSampleRepository voiceSampleRepository, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _voiceSampleRepository = voiceSampleRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<VoiceSample> UploadVoiceSampleAsync(VoiceSample voiceSample)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentUser == null)
            {
                throw new InvalidOperationException("Current user is not authenticated.");
            }

            
            voiceSample.CandidateId = currentUser.Id;
            voiceSample.UploadedOn = DateTime.UtcNow;
            voiceSample.Status = "Pending";
            voiceSample.AdminFeedback = string.Empty;

            // Save the voice sample
            await _voiceSampleRepository.AddVoiceSampleAsync(voiceSample);

            return voiceSample;
        }

        public async Task<VoiceSample> ApproveVoiceSampleAsync(int id, string feedback)
        {
            var voiceSample = await _voiceSampleRepository.GetVoiceSampleByIdAsync(id);

            if (voiceSample == null)
            {
                throw new InvalidOperationException($"No voice sample found with ID {id}");
            }

            voiceSample.Status = "Approved";
            voiceSample.AdminFeedback = feedback;

            await _voiceSampleRepository.UpdateVoiceSampleAsync(voiceSample);

            return voiceSample;
        }

        public async Task<VoiceSample> RejectVoiceSampleAsync(int id, string feedback)
        {
            var voiceSample = await _voiceSampleRepository.GetVoiceSampleByIdAsync(id);

            if (voiceSample == null)
            {
                throw new InvalidOperationException($"No voice sample found with ID {id}");
            }

            voiceSample.Status = "Rejected";
            voiceSample.AdminFeedback = feedback;

            await _voiceSampleRepository.UpdateVoiceSampleAsync(voiceSample);

            return voiceSample;
        }

        public async Task<IEnumerable<VoiceSample>> GetAllPendingSamplesAsync()
        {
            
            var allVoiceSamples = await _voiceSampleRepository.GetVoiceSamplesByCandidateIdAsync(null); 
            var pendingSamples = allVoiceSamples.Where(vs => vs.Status == "Pending");
            return pendingSamples;
        }
    }
}
