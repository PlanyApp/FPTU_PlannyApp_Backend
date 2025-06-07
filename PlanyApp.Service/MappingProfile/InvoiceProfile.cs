using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PlanyApp.Service.MappingProfile
{
    public class InvoiceProfile: Profile
    {
        public InvoiceProfile()
        {
            CreateMap<RequestCreatePendingInvoice, Invoice>()
              // Gán InvoiceDate = thời điểm hiện tại (UTC)
              .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => DateTime.UtcNow))
              .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(48))) // Gán DueDate = 48 giờ sau InvoiceDate

              // Gán CreatedAt = thời điểm hiện tại (UTC)
              .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))

              // Gán Discount = 0 (chưa áp dụng giảm giá khi tạo mới)
              .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => 0))

              // Gán Status = "Pending" (mặc định trạng thái hóa đơn mới tạo là Pending)
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"));

            CreateMap<Invoice, ResponseGetListPendingInvoice>();
            CreateMap<RequestUpdateInvoice, Invoice>()
          .ForMember(dest => dest.InvoiceId, opt => opt.Ignore()) // Không update khóa chính
          .ForMember(dest => dest.TransactionId, opt => opt.Condition(src => src.TransactionId != null))
          .ForMember(dest => dest.Notes, opt => opt.Condition(src => src.Notes != null))
          .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status != null));
          //.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
