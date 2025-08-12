using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Package;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PackageDto>> GetAllPackagesAsync()
        {
            var entities = await _unitOfWork.PackageRepository.GetAllAsync();

            // Map thủ công
            var result = entities.Select(e => new PackageDto
            {
                PackageId = e.PackageId,
                Name = e.Name,
               
                Price = e.Price,
                Type = e.Type,
              
            }).ToList();

            return result;
        }

        public async Task<PackageDto?> GetByIdAsync(int packageId)
        {
            var entity = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
            if (entity == null) return null;

            return new PackageDto
            {
                PackageId = entity.PackageId,
                Name = entity.Name,
                Price = entity.Price,
                Type = entity.Type,
            };
        }

        public async Task<PackageDto> CreateAsync(CreatePackageRequestDto request)
        {
            var entity = new Package
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                DurationDays = request.DurationDays,
                Type = request.Type,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PackageRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return new PackageDto
            {
                PackageId = entity.PackageId,
                Name = entity.Name,
                Price = entity.Price,
                Type = entity.Type,
            };
        }

        public async Task<PackageDto?> UpdateAsync(int packageId, UpdatePackageRequestDto request)
        {
            var entity = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Name)) entity.Name = request.Name;
            if (request.Price.HasValue) entity.Price = request.Price.Value;
            if (request.Description != null) entity.Description = request.Description;
            if (request.DurationDays.HasValue) entity.DurationDays = request.DurationDays;
            if (!string.IsNullOrWhiteSpace(request.Type)) entity.Type = request.Type;
            if (!string.IsNullOrWhiteSpace(request.Status)) entity.Status = request.Status;

            _unitOfWork.PackageRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return new PackageDto
            {
                PackageId = entity.PackageId,
                Name = entity.Name,
                Price = entity.Price,
                Type = entity.Type,
            };
        }

        public async Task<bool> DeleteAsync(int packageId)
        {
            var entity = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
            if (entity == null) return false;
            await _unitOfWork.PackageRepository.RemoveAsync(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
