using AutoMapper;
using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class TaleService : ITaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UpdateTaleDto>> GetTales()
        {
            var tales = await _unitOfWork.Tales.GetAllAsync();

            var talesResponse = _mapper.Map<List<UpdateTaleDto>>(tales);
            return talesResponse;
        }

        public async Task<UpdateTaleDto> GetTaleByIdWithRelations(int id)
        {
            var tale = await _unitOfWork.Tales.GetTaleWithRelations(id);

            if (tale == null)
            {
                throw new Exception($"Tale with ID {id} not found.");
            }

            var taleResponse = _mapper.Map<UpdateTaleDto>(tale);

            return taleResponse;
        }

        public async Task<UpdateTaleDto> GetTaleById(int id)
        {
            var tale = await _unitOfWork.Tales.GetByIdAsync(id);

            if (tale == null)
            {
                throw new Exception($"Tale with ID {id} not found.");
            }

            var taleResponse = _mapper.Map<UpdateTaleDto>(tale);

            return taleResponse;
        }

        public async Task<int> CreateTale(CreateTaleDto request)
        {
            if (request == null)
            {
                throw new Exception("Tale cannot be null.");
            }

            var tale = _mapper.Map<Tale>(request);
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

            _mapper.Map(request, existingTale);
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