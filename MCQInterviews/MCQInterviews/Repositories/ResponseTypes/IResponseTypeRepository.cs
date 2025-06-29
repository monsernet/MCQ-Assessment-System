using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.ResponseTypes
{
    public interface IResponseTypeRepository
    {
        Task<List<ResponseType>> GetResponseTypesAsync();
        Task<ResponseType> GetResponseTypeByIdAsync(int id);
        Task<ResponseType> AddResponseTypeAsync(ResponseType responseType);
        Task<bool> UpdateResponseTypeAsync(ResponseType responseType);
        Task<bool> DeleteResponseTypeAsync(int id);
    }
}
