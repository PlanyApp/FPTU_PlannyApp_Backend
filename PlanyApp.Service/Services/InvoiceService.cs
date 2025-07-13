using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Dto.Invoice;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGroupService _groupService;
        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, IGroupService groupService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _groupService = groupService;
        }
        public async Task<string> CreatePendingInvoiceAsync(RequestCreatePendingInvoice request)
        {

            var invoice = _mapper.Map<Invoice>(request);
            invoice.UserId = request.UserId;
            var package = await _unitOfWork.PackageRepository.GetByIdAsync(request.PackageId);
            if (package == null)
            {
                throw new Exception("Package not found");
            }
            invoice.ReferenceCode = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            invoice.FinalAmount = package.Price;
            invoice.Amount= package.Price; // Assuming Amount is the same as FinalAmount for pending invoices
            
            _unitOfWork.InvoiceRepository.Add(invoice);
            await _unitOfWork.SaveAsync();
            return invoice.ReferenceCode;
        }

        public async Task<List<ResponseGetListPendingInvoice>> GetPendingInvoicesAsync()
        {
            var invoices = await _unitOfWork.InvoiceRepository.FindAsync(i => i.Status == "Pending");
            var response = _mapper.Map<List<ResponseGetListPendingInvoice>>(invoices);
            return response;
        }

        public async Task<int?> UpdatePendingInvoiceAsync(RequestUpdateInvoice request)
        {
            var invoiceList = await _unitOfWork.InvoiceRepository
     .FindIncludeAsync(x => x.InvoiceId == request.InvoiceId, x => x.User, x=> x.Package);

            var invoice = invoiceList.FirstOrDefault();

            if (invoice == null)
            {
                throw new Exception("Invoice not found");
            }
            bool wasPaidBefore = invoice.Status?.ToLower() == "paid";

            _mapper.Map(request, invoice);  // Mapping vào object đã có

            _unitOfWork.InvoiceRepository.Update(invoice);
            await _unitOfWork.SaveAsync();
            int? createdGroupId = null;
            if (request.Status.ToLower() == "paid" && !wasPaidBefore)
            {
                // 1. Insert UserPackage record
                var userPackage = new UserPackage
                {
                    UserId = invoice.UserId,
                    PackageId = invoice.PackageId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1), 
                    IsActive = true
                };

               
                // 2. If type is "Group", Create a group
                if (invoice.Package.Type == "group")
                {
                    Console.WriteLine("Creating group for user: " + invoice.PackageId);
                    var createGroupRequest = new CreateGroupRequest
                    {
                        GroupName = $"Nhóm của {invoice.User.FullName} và những người bạn",
                        UserId = invoice.UserId,
                        GroupPackage = invoice.PackageId,
                    };
                  
                    var createdGroup= await _groupService.CreateGroupAsync(createGroupRequest);
                    createdGroupId = createdGroup?.GroupId;
                }
                userPackage.GroupId = createdGroupId; // Assign GroupId if group was created
                await _unitOfWork.UserPackageRepository.AddAsync(userPackage);
                await _unitOfWork.SaveAsync();
            }
            return createdGroupId;
        }

        public async Task<InvoiceSummaryDto?> GetInvoiceByReferenceCodeAsync(string referenceCode)
        {
            var invoice = await _unitOfWork.InvoiceRepository
                .FirstOrDefaultAsync(i => i.ReferenceCode == referenceCode);

            if (invoice == null) return null;

            return new InvoiceSummaryDto
            {
                InvoiceId = invoice.InvoiceId,
                ReferenceCode = invoice.ReferenceCode,
                Amount = invoice.Amount,
                //Discount = invoice.Discount,
                //FinalAmount = invoice.FinalAmount,
                Status = invoice.Status
            };
        }
    }
}
