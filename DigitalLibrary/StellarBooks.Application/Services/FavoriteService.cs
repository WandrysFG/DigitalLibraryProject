using AutoMapper;
using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UpdateFavoriteDto>> GetFavorites()
        {
            var favorites = await _unitOfWork.Favorites.GetAllAsync();
            var favoritesResponse = _mapper.Map<List<UpdateFavoriteDto>>(favorites);

            return favoritesResponse;
        }

        public async Task<List<UpdateFavoriteDto>> GetAllWithUserAndTale()
        {
            var favorites = await _unitOfWork.Favorites.GetAllWithUserAndTale();
            var favoritesResponse = _mapper.Map<List<UpdateFavoriteDto>>(favorites);

            return favoritesResponse;
        }

        public async Task<UpdateFavoriteDto> GetFavoriteById(int id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdWithUserAndTale(id);

            if (favorite == null)
            {
                throw new Exception($"Favorite with ID {id} not found.");
            }

            var favoriteResponse = _mapper.Map<UpdateFavoriteDto>(favorite);
            return favoriteResponse;
        }

        public async Task<int> CreateFavorite(CreateFavoriteDto request)
        {
            if (request == null)
            {
                throw new Exception("Favorite cannot be null.");
            }

            var favorite = _mapper.Map<Favorite>(request);
            await _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.CompleteAsync();

            return favorite.Id;
        }

        public async Task UpdateFavorite(int id, UpdateFavoriteDto request)
        {
            var existingFavorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (existingFavorite == null)
            {
                throw new Exception("Favorite is null or ID mismatch.");
            }

            _mapper.Map(request, existingFavorite);
            await _unitOfWork.Favorites.UpdateAsync(existingFavorite);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteFavorite(int id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (favorite == null)
            {
                throw new Exception("Favorite not found.");
            }

            await _unitOfWork.Favorites.DeleteAsync(favorite);
            await _unitOfWork.CompleteAsync();
        }
    }
}
