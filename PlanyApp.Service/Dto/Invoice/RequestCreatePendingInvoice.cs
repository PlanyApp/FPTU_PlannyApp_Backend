using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Invoice
{
    public class RequestCreatePendingInvoice
    {
        [Required]
        public int PackageId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string PaymentMethod { get; set; }

       
    }
}
