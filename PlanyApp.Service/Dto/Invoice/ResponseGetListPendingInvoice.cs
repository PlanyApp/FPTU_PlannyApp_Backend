using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Invoice
{
    public class ResponseGetListPendingInvoice
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal Discount { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}
