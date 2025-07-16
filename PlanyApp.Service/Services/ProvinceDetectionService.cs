using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Interfaces;
using System.Text.RegularExpressions;

namespace PlanyApp.Service.Services
{
    public class ProvinceDetectionService : IProvinceDetectionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProvinceDetectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int?> DetectProvinceFromNameAsync(string planName)
        {
            if (string.IsNullOrWhiteSpace(planName))
                return null;

            // Get all provinces from database
            var provinces = await _unitOfWork.ProvinceRepository.GetAllAsync();
            
            // Normalize the plan name for better matching
            var normalizedPlanName = NormalizeName(planName);

            // Try to find exact matches first
            foreach (var province in provinces)
            {
                var normalizedProvinceName = NormalizeName(province.Name);
                
                // Check if province name appears in plan name
                if (normalizedPlanName.Contains(normalizedProvinceName))
                {
                    return province.ProvinceId;
                }

                // Check common alternative names and abbreviations
                if (CheckAlternativeNames(normalizedPlanName, province.Name))
                {
                    return province.ProvinceId;
                }
            }

            return null;
        }

        private string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Remove diacritics, convert to lowercase, remove extra spaces
            var normalized = RemoveDiacritics(name.ToLowerInvariant().Trim());
            
            // Remove common prefixes and suffixes
            normalized = Regex.Replace(normalized, @"\b(tp\.|thanh pho|tinh|province|city)\b", "", RegexOptions.IgnoreCase);
            
            // Remove extra whitespace
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();
            
            return normalized;
        }

        private bool CheckAlternativeNames(string planName, string provinceName)
        {
            // Handle special cases and common abbreviations
            var alternatives = GetProvinceAlternatives(provinceName);
            
            foreach (var alternative in alternatives)
            {
                if (planName.Contains(NormalizeName(alternative)))
                {
                    return true;
                }
            }

            return false;
        }

        private List<string> GetProvinceAlternatives(string provinceName)
        {
            var alternatives = new List<string>();
            
            switch (provinceName.ToLowerInvariant())
            {
                case "tp. hồ chí minh":
                    alternatives.AddRange(new[] { "ho chi minh", "saigon", "sai gon", "hcm", "tphcm", "tp hcm" });
                    break;
                case "hà nội":
                    alternatives.AddRange(new[] { "ha noi", "hanoi" });
                    break;
                case "đà nẵng":
                    alternatives.AddRange(new[] { "da nang", "danang" });
                    break;
                case "đà lạt":
                    alternatives.AddRange(new[] { "da lat", "dalat" });
                    break;
                case "cần thơ":
                    alternatives.AddRange(new[] { "can tho", "cantho" });
                    break;
                case "thành phố huế":
                    alternatives.AddRange(new[] { "hue", "thanh pho hue", "tp hue" });
                    break;
                case "quảng ninh":
                    alternatives.AddRange(new[] { "quang ninh", "ha long", "halong" });
                    break;
                case "khánh hòa":
                    alternatives.AddRange(new[] { "khanh hoa", "nha trang" });
                    break;
                case "lào cai":
                    alternatives.AddRange(new[] { "lao cai", "sapa", "sa pa" });
                    break;
                case "cao bằng":
                    alternatives.AddRange(new[] { "cao bang" });
                    break;
                case "điện biên":
                    alternatives.AddRange(new[] { "dien bien" });
                    break;
                case "lai châu":
                    alternatives.AddRange(new[] { "lai chau" });
                    break;
                // Add more alternatives as needed
            }

            return alternatives;
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Vietnamese diacritics removal
            var replacements = new Dictionary<string, string>
            {
                {"à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ", "a"},
                {"è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ", "e"},
                {"ì|í|ị|ỉ|ĩ", "i"},
                {"ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ", "o"},
                {"ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ", "u"},
                {"ỳ|ý|ỵ|ỷ|ỹ", "y"},
                {"đ", "d"},
                {"À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ", "A"},
                {"È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ", "E"},
                {"Ì|Í|Ị|Ỉ|Ĩ", "I"},
                {"Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ", "O"},
                {"Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ", "U"},
                {"Ỳ|Ý|Ỵ|Ỷ|Ỹ", "Y"},
                {"Đ", "D"}
            };

            foreach (var replacement in replacements)
            {
                text = Regex.Replace(text, replacement.Key, replacement.Value);
            }

            return text;
        }
    }
} 