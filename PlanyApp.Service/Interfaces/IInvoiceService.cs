using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IInvoiceService
    {
        // create pending invoice
        public Task<String> CreatePendingInvoiceAsync(RequestCreatePendingInvoice request);
        // get list pending invoices 
        public Task<List<ResponseGetListPendingInvoice>> GetPendingInvoicesAsync();
        // update pending invoice
        public Task<int?> UpdatePendingInvoiceAsync(RequestUpdateInvoice request);
        Task<InvoiceSummaryDto?> GetInvoiceByReferenceCodeAsync(string referenceCode);
        Task<List<Invoice>> GetInvoicesByStatusAsync(string? status = null);

        Task<int> CancelExpiredUnpaidInvoicesAsync();
    }
}
