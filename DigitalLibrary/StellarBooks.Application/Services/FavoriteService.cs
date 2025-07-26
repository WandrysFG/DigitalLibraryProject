using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UpdateFavoriteDto>> GetFavorites()
        {
            var favorites = await _unitOfWork.Favorites.GetAllAsync();

            var result = favorites.Select(f => new UpdateFavoriteDto
            {
                Id = f.Id,
                DateAdded = f.DateAdded,
                UserId = f.UserId,
                TaleId = f.TaleId,
            }).ToList();

            return result;
        }

        public async Task<List<UpdateFavoriteDto>> GetAllWithUserAndTale()
        {
            var favorites = await _unitOfWork.Favorites.GetAllWithUserAndTale();

            var favoritesWithUserAndTale = favorites.Select(f => new UpdateFavoriteDto
            {
                Id = f.Id,
                DateAdded = f.DateAdded,
                UserId = f.UserId,
                TaleId = f.TaleId,
                User = new UpdateUserDto
                {
                    Id = f.User.Id,
                    FirstName = f.User.FirstName,
                    LastName = f.User.LastName
                },
                Tale = new UpdateTaleDto
                {
                    Id = f.Tale.Id,
                    Title = f.Tale.Title
                }
            }).ToList();

            return favoritesWithUserAndTale;
        }

        public async Task<UpdateFavoriteDto> GetFavoriteById(int id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdWithUserAndTale(id);
            if (favorite == null)
                throw new Exception($"Favorite with ID {id} not found.");

            var result = new UpdateFavoriteDto
            {
                Id = favorite.Id,
                DateAdded = favorite.DateAdded,
                UserId = favorite.UserId,
                TaleId = favorite.TaleId,
                User = new UpdateUserDto
                {
                    Id = favorite.User.Id,
                    FirstName = favorite.User.FirstName,
                    LastName = favorite.User.LastName
                },
                Tale = new UpdateTaleDto
                {
                    Id = favorite.Tale.Id,
                    Title = favorite.Tale.Title
                }
            };

            return result;
        }

        public async Task<int> CreateFavorite(CreateFavoriteDto request)
        {
            if (request == null)
            {
                throw new Exception("Favorite cannot be null.");
            }

            var favorite = new Favorite
            {
                UserId = request.UserId,
                TaleId = request.TaleId,
                DateAdded = DateTime.UtcNow.Date
            };

            await _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.CompleteAsync();

            return favorite.Id;
        }

        public async Task UpdateFavorite(int id, UpdateFavoriteDto request)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (favorite == null)
            {
                throw new Exception("Favorite is null or ID mismatch.");
            }

            favorite.UserId = request.UserId;
            favorite.TaleId = request.TaleId;
            favorite.DateAdded = request.DateAdded;

            await _unitOfWork.Favorites.UpdateAsync(favorite);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteFavorite(int id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (favorite == null)
            {
                throw new Exception("Favorite not found.");
            }

            _unitOfWork.Favorites.DeleteAsync(favorite);
            await _unitOfWork.CompleteAsync();
        }
    }
}
