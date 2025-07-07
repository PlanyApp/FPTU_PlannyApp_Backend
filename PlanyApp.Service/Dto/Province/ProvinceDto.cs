using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Province
{
    public class ProvinceDto
    {
        public int ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;
       
        public string? ImageUrl { get; set; }
      
        }
}
