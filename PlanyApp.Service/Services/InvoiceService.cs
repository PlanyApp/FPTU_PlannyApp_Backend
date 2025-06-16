using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto;
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
        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<string> CreatePendingInvoiceAsync(RequestCreatePendingInvoice request)
        {

            var invoice = _mapper.Map<Invoice>(request);
            
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

        public async Task<bool> UpdatePendingInvoiceAsync(RequestUpdateInvoice request)
        {
            var invoice = await _unitOfWork.InvoiceRepository.GetByIdAsync(request.InvoiceId);
            if (invoice == null)
            {
                throw new Exception("Invoice not found");
            }

            _mapper.Map(request, invoice);  // Mapping vào object đã có

            _unitOfWork.InvoiceRepository.Update(invoice);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
