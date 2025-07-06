using AutoMapper;
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
    }
}
