using StellarBooks.Applications.DTOs;

namespace StellarBooks.Application.Interfaces
{
    public interface IActivityService
    {
        Task<int> CreateActivity(CreateActivityDto request);
        Task DeleteActivity(int id);
        Task<List<UpdateActivityDto>> GetActivities();
        Task<UpdateActivityDto> GetActivityById(int id);
        Task UpdateActivity(int id, UpdateActivityDto dto);
    }
}