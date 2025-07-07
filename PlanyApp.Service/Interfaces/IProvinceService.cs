using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Province;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IProvinceService
    {
        List<ProvinceDto>? GetListProvinces();
    }
}
