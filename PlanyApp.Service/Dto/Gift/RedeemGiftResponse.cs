using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Gift
{
    public class RedeemGiftResponse
    {
        public string GiftName { get; set; }
        public string Code { get; set; }
        public DateTime RedeemedAt { get; set; }
        public int? RemainingPoints { get; set; }
    }

}
