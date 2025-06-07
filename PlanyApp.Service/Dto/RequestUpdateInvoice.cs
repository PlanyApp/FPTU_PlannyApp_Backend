using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto
{
    public class RequestUpdateInvoice
    {
        [Required]
        public int InvoiceId { get; set; } // tra cứu invoice id để cập nhật
        [Required]
        public string Status { get; set; } // pending, paid, cancelled
        [Required]
        public string TransactionId { get; set; } // mã giao dịch thanh toán
        public string? Notes { get; set; }
   
    }
}
