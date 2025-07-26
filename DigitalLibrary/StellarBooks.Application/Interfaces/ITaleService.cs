using StellarBooks.Applications.DTOs;

namespace StellarBooks.Application.Interfaces
{
    public interface ITaleService
    {
        Task<int> CreateTale(CreateTaleDto request);
        Task DeleteTale(int id);
        Task<UpdateTaleDto> GetTaleById(int id);
        Task<List<UpdateTaleDto>> GetTales();
        Task UpdateTale(int id, UpdateTaleDto request);
    }
}