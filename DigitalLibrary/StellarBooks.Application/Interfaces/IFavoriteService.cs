using StellarBooks.Applications.DTOs;

namespace StellarBooks.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<int> CreateFavorite(CreateFavoriteDto request);
        Task DeleteFavorite(int id);
        Task<List<UpdateFavoriteDto>> GetAllWithUserAndTale();
        Task<List<UpdateFavoriteDto>> GetFavorites();
        Task<UpdateFavoriteDto> GetFavoriteById(int id);
        Task UpdateFavorite(int id, UpdateFavoriteDto request);
    }
}