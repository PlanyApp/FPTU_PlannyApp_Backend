using PlanyApp.Service.Dto;
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
        public Task<bool> UpdatePendingInvoiceAsync(RequestUpdateInvoice request);
    }
}
