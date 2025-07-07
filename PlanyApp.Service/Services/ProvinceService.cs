using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Province;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PlanyApp.Service.Services
{
    public class ProvinceService: IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProvinceService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<ProvinceDto>? GetListProvinces()
        {
            var list = _unitOfWork.ProvinceRepository.GetAll()
                .Select(p => new ProvinceDto {ProvinceId= p.ProvinceId, Name= p.Name, ImageUrl= p.Image }).ToList();
                                                               
            return list;
        }
    }
   
}
