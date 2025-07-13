using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Invoice
{
    public class InvoiceSummaryDto
    {
        public int InvoiceId { get; set; }
        public string ReferenceCode { get; set; }
        public decimal Amount { get; set; }
       //public decimal Discount { get; set; }
       // public decimal? FinalAmount { get; set; }
        public string Status { get; set; }
    }

}
