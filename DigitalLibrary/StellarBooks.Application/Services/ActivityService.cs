using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class ActivityService : IActivityService
    {

        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UpdateActivityDto>> GetActivities()
        {
            var activities = await _unitOfWork.Activities.GetAllAsync();

            var result = activities.Select(a => new UpdateActivityDto
            {
                Id = a.Id,
                TaleId = a.TaleId,
                ActivityType = a.ActivityType,
                Description = a.Description,
                MultimediaResource = a.MultimediaResource
            }).ToList();

            return result;
        }

        public async Task<UpdateActivityDto> GetActivityById(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                throw new Exception($"Activity with ID {id} not found.");
            }

            var result = new UpdateActivityDto
            {
                Id = activity.Id,
                TaleId = activity.TaleId,
                ActivityType = activity.ActivityType,
                Description = activity.Description,
                MultimediaResource = activity.MultimediaResource,
                Tale = activity.Tale == null ? null : new UpdateTaleDto
                {
                    Id = activity.Tale.Id,
                    Title = activity.Tale.Title,
                    RecommendedAge = activity.Tale.RecommendedAge
                }
            };

            return result;
        }

        public async Task<int> CreateActivity(CreateActivityDto request)
        {
            if (request == null)
            {
                throw new Exception("Activity cannot be null.");
            }

            var activity = new Activity
            {
                TaleId = request.TaleId,
                ActivityType = request.ActivityType,
                Description = request.Description,
                MultimediaResource = request.MultimediaResource
            };

            activity = await _unitOfWork.Activities.AddAsync(activity);
            await _unitOfWork.CompleteAsync();

            return activity.Id;
        }

        public async Task UpdateActivity(int id, UpdateActivityDto request)
        {
            var existing = await _unitOfWork.Activities.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Activity with ID {id} not found.");
            }

            existing.TaleId = request.TaleId;
            existing.ActivityType = request.ActivityType;
            existing.Description = request.Description;
            existing.MultimediaResource = request.MultimediaResource;

            await _unitOfWork.Activities.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteActivity(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);
            if (activity == null)
            {
                throw new Exception("Activity not found.");
            }

            await _unitOfWork.Activities.DeleteAsync(activity);
            await _unitOfWork.CompleteAsync();
        }
    }
}