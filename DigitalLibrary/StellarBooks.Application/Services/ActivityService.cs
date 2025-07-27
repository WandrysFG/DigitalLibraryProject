using AutoMapper;
using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class ActivityService : IActivityService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UpdateActivityDto>> GetActivities()
        {
            var activities = await _unitOfWork.Activities.GetAllAsync();

            var activityResponse = _mapper.Map<List<UpdateActivityDto>>(activities);

            return activityResponse;
        }

        public async Task<UpdateActivityDto> GetActivityById(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                throw new Exception($"Activity with ID {id} not found.");
            }

            var activityResponse = _mapper.Map<UpdateActivityDto>(activity);

            return activityResponse;
        }

        public async Task<int> CreateActivity(CreateActivityDto request)
        {
            if (request == null)
            {
                throw new Exception("Activity cannot be null.");
            }

            var activity = _mapper.Map<Activity>(request);
            activity = await _unitOfWork.Activities.AddAsync(activity);
            await _unitOfWork.CompleteAsync();

            return activity.Id;
        }

        public async Task UpdateActivity(int id, UpdateActivityDto request)
        {
            var existingActivity = await _unitOfWork.Activities.GetByIdAsync(id);
            if (existingActivity == null)
            {
                throw new Exception("Activity with ID {id} not found.");
            }

            _mapper.Map(request, existingActivity);
            await _unitOfWork.Activities.UpdateAsync(existingActivity);
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