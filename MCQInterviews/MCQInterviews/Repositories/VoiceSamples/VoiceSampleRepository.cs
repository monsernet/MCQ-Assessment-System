using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.VoiceSamples
{
    public class VoiceSampleRepository : IVoiceSampleRepository
    {
        private readonly ApplicaationDbContext _context;

        public VoiceSampleRepository(ApplicaationDbContext context)
        {
            _context = context;
        }

        public async Task<VoiceSample> GetVoiceSampleByIdAsync(int id)
        {
            return await _context.VoiceSamples
                .FindAsync(id);
        }

        public async Task<IEnumerable<VoiceSample>> GetVoiceSamplesByCandidateIdAsync(string candidateId)
        {
            return await _context.VoiceSamples
                .Where(vs => vs.CandidateId == candidateId)
                .ToListAsync();
        }

        public async Task AddVoiceSampleAsync(VoiceSample voiceSample)
        {
            await _context.VoiceSamples.AddAsync(voiceSample);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVoiceSampleAsync(VoiceSample voiceSample)
        {
            _context.VoiceSamples.Update(voiceSample);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVoiceSampleAsync(int id)
        {
            var voiceSample = await _context.VoiceSamples.FindAsync(id);
            if (voiceSample != null)
            {
                _context.VoiceSamples.Remove(voiceSample);
                await _context.SaveChangesAsync();
            }
        }
    }
}
