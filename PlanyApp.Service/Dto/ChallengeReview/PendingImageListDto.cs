using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.ChallengeReview
{
    public class PendingImageListDto
    {
        /// <summary>Id để gọi approve hoặc xem chi tiết nếu cần</summary>
        public int UserChallengeProgressId { get; set; }

        /// <summary>Tên challenge</summary>
        public string ChallengeName { get; set; } = null!;

        /// <summary>Họ tên người dùng</summary>
        public string UserFullName { get; set; } = null!;

        /// <summary>URL hình ảnh (thumbnail)</summary>
        public string ImageUrl { get; set; } = null!;
    }
}
