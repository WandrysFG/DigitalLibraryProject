using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class TaleService : ITaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UpdateTaleDto>> GetTales()
        {
            var tales = await _unitOfWork.Tales.GetAllAsync();

            var result = tales.Select(t => new UpdateTaleDto
            {
                Id = t.Id,
                Title = t.Title,
                RecommendedAge = t.RecommendedAge,
                Theme = t.Theme,
                Content = t.Content,
                CoverImage = t.CoverImage,
                NarrationAudio = t.NarrationAudio,
                IsAvailable = t.IsAvailable,
                PublicationDate = t.PublicationDate
            }).ToList();

            return result;
        }

        public async Task<UpdateTaleDto> GetTaleById(int id)
        {
            var tale = await _unitOfWork.Tales.GetByIdAsync(id);

            if (tale == null)
            {
                throw new Exception($"Tale with ID {id} not found.");
            }

            var result = new UpdateTaleDto
            {
                Id = tale.Id,
                Title = tale.Title,
                RecommendedAge = tale.RecommendedAge,
                Theme = tale.Theme,
                Content = tale.Content,
                CoverImage = tale.CoverImage,
                NarrationAudio = tale.NarrationAudio,
                IsAvailable = tale.IsAvailable,
                PublicationDate = tale.PublicationDate
            };

            return result;
        }

        public async Task<int> CreateTale(CreateTaleDto request)
        {
            if (request == null)
            {
                throw new Exception("Tale cannot be null.");
            }

            var tale = new Tale
            {
                Title = request.Title,
                RecommendedAge = request.RecommendedAge,
                Theme = request.Theme,
                Content = request.Content,
                CoverImage = request.CoverImage,
                NarrationAudio = request.NarrationAudio,
                IsAvailable = request.IsAvailable,
                PublicationDate = System.DateTime.UtcNow.Date
            };

            await _unitOfWork.Tales.AddAsync(tale);
            await _unitOfWork.CompleteAsync();

            return tale.Id;
        }

        public async Task UpdateTale(int id, UpdateTaleDto request)
        {
            if (request == null)
            {
                throw new Exception("Tale is null or ID mismatch.");
            }

            var existingTale = await _unitOfWork.Tales.GetByIdAsync(id);

            if (existingTale == null)
            {
                throw new Exception($"Tale with ID {id} not found.");
            }

            existingTale.Title = request.Title;
            existingTale.RecommendedAge = request.RecommendedAge;
            existingTale.Theme = request.Theme;
            existingTale.Content = request.Content;
            existingTale.CoverImage = request.CoverImage;
            existingTale.NarrationAudio = request.NarrationAudio;
            existingTale.IsAvailable = request.IsAvailable;
            existingTale.PublicationDate = request.PublicationDate;

            await _unitOfWork.Tales.UpdateAsync(existingTale);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTale(int id)
        {
            var tale = await _unitOfWork.Tales.GetByIdAsync(id);
            if (tale == null)
            {
                throw new Exception("Tale not found.");
            }

            await _unitOfWork.Tales.DeleteAsync(tale);
            await _unitOfWork.CompleteAsync();
        }
    }
}