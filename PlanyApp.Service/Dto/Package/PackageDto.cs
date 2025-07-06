using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Package
{
    public class PackageDto
    {
        public int PackageId { get; set; }
        public string Name { get; set; } = default!;
        
        public decimal Price { get; set; }
        public string? Type { get; set; }
       
    }
}
