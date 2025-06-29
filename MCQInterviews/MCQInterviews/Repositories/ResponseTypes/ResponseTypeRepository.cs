using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.ResponseTypes
{
    public class ResponseTypeRepository : IResponseTypeRepository
    {
        private readonly ApplicaationDbContext _context;

        public ResponseTypeRepository(ApplicaationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResponseType>> GetResponseTypesAsync()
        {
            return await _context.ResponseTypes.ToListAsync();
        }

        public async Task<ResponseType> GetResponseTypeByIdAsync(int id)
        {
            return await _context.ResponseTypes.FindAsync(id);
        }

        public async Task<ResponseType> AddResponseTypeAsync(ResponseType responseType)
        {
            _context.ResponseTypes.Add(responseType);
            await _context.SaveChangesAsync();
            return responseType;
        }

        public async Task<bool> UpdateResponseTypeAsync(ResponseType responseType)
        {
            _context.ResponseTypes.Update(responseType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteResponseTypeAsync(int id)
        {
            var responseType = await _context.ResponseTypes.FindAsync(id);
            if (responseType != null)
            {
                _context.ResponseTypes.Remove(responseType);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
